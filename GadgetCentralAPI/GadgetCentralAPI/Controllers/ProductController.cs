using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GadgetCentralAPI.Data;
using GadgetCentralAPI.Models;

namespace GadgetCentralAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _ctx;
        public ProductController(AppDbContext ctx) => _ctx = ctx;

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _ctx.Products.AsNoTracking().ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var item = await _ctx.Products.AsNoTracking()
                                          .FirstOrDefaultAsync(p => p.GlobalId == id);
            return item is null ? NotFound() : Ok(item);
        }

        [HttpGet("global/{globalProductId}")]
        public async Task<IActionResult> GetByGlobalId(string globalProductId)
        {
            var item = await _ctx.Products.AsNoTracking()
                                          .FirstOrDefaultAsync(p => p.GlobalId == globalProductId);
            return item is null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product p)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _ctx.Products.Add(p);
            await _ctx.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = p.GlobalId }, p);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Product p)
        {
            var existing = await _ctx.Products.FirstOrDefaultAsync(x => x.GlobalId == id);
            if (existing is null) return NotFound();

            existing.ItemName = p.ItemName;
            existing.UnitPrice = p.UnitPrice;
            existing.Inventory = p.Inventory;
            existing.ProductDetails = p.ProductDetails;

            await _ctx.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _ctx.Products.FirstOrDefaultAsync(x => x.GlobalId == id);
            if (existing is null) return NotFound();

            _ctx.Products.Remove(existing);
            await _ctx.SaveChangesAsync();
            return NoContent();
        }
    }
}
