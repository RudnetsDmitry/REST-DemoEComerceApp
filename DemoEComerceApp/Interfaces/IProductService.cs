using DemoEComerceApp.Model;
using System.Threading.Tasks;

namespace DemoEComerceApp.Interfaces
{
    public interface IProductService
    {
        Task<bool> CreateProductAsync(Product product);
        Task<Product> GetOrderAsync(int id);
        Task<bool> UpdateProductAsync(int id, Product product);
        Task<bool> DeleteOrderAsync(int id);
    }
}
