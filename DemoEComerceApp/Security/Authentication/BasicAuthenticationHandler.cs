using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DemoEComerceApp.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DemoEComerceApp.Security.Authentication
{
    public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
    {
        private readonly EComerceDBContext _context;
        private const string AuthorizationHeaderName = "Authorization";
        private const string BasicSchemeName = "Basic";

        public BasicAuthenticationHandler(IOptionsMonitor<BasicAuthenticationOptions> options, ILoggerFactory logger, 
            UrlEncoder encoder, ISystemClock clock, EComerceDBContext context)
            : base(options, logger, encoder, clock)
        {
            _context = context;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // 1. Verify if AuthorizationHeaderName present in the header.
            // AuthorizationHeaderName is a string with value "Authorization".
            if (!Request.Headers.ContainsKey(AuthorizationHeaderName))
            {
                // Authorization header not found
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            // 2. Verify if header is valid
            if (!AuthenticationHeaderValue.TryParse(Request.Headers[AuthorizationHeaderName], 
                out AuthenticationHeaderValue headerValue))
            {
                // Authorization header is not valid
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            // 3. Verify is scheme name is Basic. BasicSchemeName is a string
            // with value 'Basic'. 
            if (!BasicSchemeName.Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                // Authorization header is not Basic.
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            // 4. Fetch email and password from header.
            // If length is not 2, then authentication fails.

            // Extract email and password.
            byte[] headerValueBytes = Convert.FromBase64String(headerValue.Parameter);
            string emailPassword = Encoding.UTF8.GetString(headerValueBytes);

            string[] parts = emailPassword.Split(':');

            if (parts.Length != 2)
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid Basic Authentication Header"));
            }

            string email = parts[0];
            string password = parts[1];

            // Validate if email and password are correct.
            var customer = _context.Customers.SingleOrDefault(x => x.Email == email && x.Password == password);

            if (customer == null)
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid email and password."));
            }

            // Create claims with email and id.
            var claims = new[]
            {   new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString())
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            //57571
            Response.Headers["WWW-Authenticate"] = $"Basic realm=\"http://localhost:5000\", charset=\"UTF-8\"";
            await base.HandleChallengeAsync(properties);
        }
    }
}
