using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Phan_quyen_RBAC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class ProductsController : ControllerBase
    {
        private static List<string> _products = new List<string>
        {
            "Product A",
            "Product B",
            "Product C"
        };

        [HttpGet]
        [Authorize(Policy = "CanReadProducts")]
        public IActionResult GetAllProducts()
        {
            return Ok(_products);
        }

        [HttpPost]
        [Authorize(Policy = "CanWriteProducts")]
        public IActionResult CreateProduct([FromBody] string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
            {
                return BadRequest("Product name cannot be empty.");
            }

            _products.Add(productName);

            return GetAllProducts();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "CanWriteProducts")] // Quyền ghi cũng được dùng cho xóa trong demo này
        public IActionResult DeleteProduct(int id)
        {
            return Ok($"Product with ID {id} deleted successfully.");
        }
   
    }
}
