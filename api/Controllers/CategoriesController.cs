using Microsoft.AspNetCore.Mvc;
using api.Models;
using api.DataAccess;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly CategoryDataAccess _categoryDataAccess;

        public CategoriesController()
        {
            _categoryDataAccess = new CategoryDataAccess();
        }

        // GET: api/categories
        [HttpGet]
        public async Task<ActionResult<List<Category>>> GetCategories()
        {
            try
            {
                var categories = await _categoryDataAccess.GetAllCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving categories", error = ex.Message });
            }
        }

        // GET: api/categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            try
            {
                var category = await _categoryDataAccess.GetCategoryByIdAsync(id);
                if (category == null)
                {
                    return NotFound(new { message = "Category not found" });
                }
                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving category", error = ex.Message });
            }
        }
    }
}

