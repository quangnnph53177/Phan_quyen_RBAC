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
        [HttpGet]
        [Authorize(Policy = "CanReadProducts")] 
        public IActionResult GetAllProducts()
        {
            return Ok(new List<string> { "Product A", "Product B", "Product C" });
        }

        [HttpPost]
        [Authorize(Policy = "CanWriteProducts")] // Chỉ những ai có quyền "Product.Write" mới truy cập được
        public IActionResult CreateProduct([FromBody] string productName)
        {
            return Ok($"Product '{productName}' created successfully.");
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "CanWriteProducts")] // Quyền ghi cũng được dùng cho xóa trong demo này
        public IActionResult DeleteProduct(int id)
        {
            return Ok($"Product with ID {id} deleted successfully.");
        }
   
    }
}
