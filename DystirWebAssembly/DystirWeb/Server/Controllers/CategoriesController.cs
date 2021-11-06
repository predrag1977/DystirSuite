using System.Linq;
using DystirWeb.Server.DystirDB;
using DystirWeb.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DystirWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private DystirDBContext _dystirDBContext;

        public CategoriesController(DystirDBContext dystirDBContext)
        {
            _dystirDBContext = dystirDBContext;
        }

        // GET: api/Categories
        [HttpGet]
        public IQueryable<Categories> GetCategories()
        {
            return _dystirDBContext.Categories;
        }

        // GET: api/Categories/5
        [HttpGet("{id}", Name = "GetCategorie")]
        public IActionResult GetCategories(int id)
        {
            Categories categories = _dystirDBContext.Categories.Find(id);
            if (categories == null)
            {
                return NotFound();
            }

            return Ok(categories);
        }

        // PUT: api/Categories/5
        [HttpPut("{id}")]
        public IActionResult PutCategories(int id, Categories categories)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != categories.Id)
            {
                return BadRequest();
            }

            _dystirDBContext.Entry(categories).State = EntityState.Modified;

            try
            {
                _dystirDBContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoriesExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(StatusCodes.Status204NoContent);
        }

        // POST: api/Categories
        [HttpPost]
        public IActionResult PostCategories(Categories categories)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _dystirDBContext.Categories.Add(categories);
            _dystirDBContext.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = categories.Id }, categories);
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public IActionResult DeleteCategories(int id)
        {
            Categories categories = _dystirDBContext.Categories.Find(id);
            if (categories == null)
            {
                return NotFound();
            }

            _dystirDBContext.Categories.Remove(categories);
            _dystirDBContext.SaveChanges();

            return Ok(categories);
        }

        private bool CategoriesExists(int id)
        {
            return _dystirDBContext.Categories.Count(e => e.Id == id) > 0;
        }
    }
}