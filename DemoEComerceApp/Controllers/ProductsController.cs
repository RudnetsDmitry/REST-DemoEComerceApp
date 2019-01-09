using DemoEComerceApp.Interfaces;
using DemoEComerceApp.Model;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DemoEComerceApp.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        IProductService _productService { get; set; }

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/Products
        [HttpGet]
        public IEnumerable<Product> Get()
        {
            return new Product[]
            {
                new Product(1, "Oats", new decimal(3.07)),
                new Product(2, "Toothpaste", new decimal(10.89)),
                new Product(3, "Television", new decimal(500.90))
            };
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public Task<Product> Get(int id)
            => _productService.GetOrderAsync(id);

        // POST: api/Products
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Product product)
            => (await _productService.CreateProductAsync(product))
                ? (IActionResult)Created($"api/products/{product.Id}", product) // HTTP 201
                : StatusCode(500); // HTTP 500

        // PUT: api/Products/1
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody]Product product)
            => (await _productService.UpdateProductAsync(id, product))
                ? Ok()
                : StatusCode(500);

        // DELETE: api/Products/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
            => (await _productService.DeleteOrderAsync(id))
                ? (IActionResult)Ok()
                : NoContent();

    }
}
