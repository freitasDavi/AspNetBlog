using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers;

[ApiController]
public class CategoryController : ControllerBase
{
    [HttpGet("v1/categories")]
    public async Task<IActionResult> GetAsync(
        [FromServices] BlogDataContext context)
    {
        try
        {
            var categories = await context.Categories.ToListAsync();

            return Ok(categories);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "05XE04 - Falha interna no servidor");
        }
    }
    
    [HttpGet("v1/categories/{id:int}")]
    public async Task<IActionResult> GetByIdAsync(
        [FromRoute] int id,
        [FromServices] BlogDataContext context)
    {
        try
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if (category == null) return NotFound();
        
            return Ok(category);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "05XE05 - Falha interna no servidor");
        }
    }
    
    [HttpPost("v1/categories")]
    public async Task<IActionResult> PostAsync(
        [FromBody] Category model,
        [FromServices] BlogDataContext context)
    {
        try
        {
            await context.Categories.AddAsync(model);
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, "05XE9 - Não foi possível incluir a categoria");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "05XE10 - Falha interna no servidor");
        }

        return Created($"v1/categories/{model.Id}", model);
    }
    
    [HttpPut("v1/categories/{id:int}")]
    public async Task<IActionResult> PutAsync(
        [FromRoute] int id,
        [FromBody] Category model,
        [FromServices] BlogDataContext context)
    {
        try
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if (category == null) return NotFound();

            category.Name = model.Name;
            category.Slug = model.Slug;

            context.Categories.Update(category);
            await context.SaveChangesAsync();
            
            return Ok(category);
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, "05XE8 - Não foi possível editar  a categoria");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "05XE11 - Falha interna no servidor");
        }
    }
    
    [HttpDelete("v1/categories/{id:int}")]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] int id,
        [FromServices] BlogDataContext context)
    {
        try
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if (category == null) return NotFound();

            context.Categories.Remove(category);
            await context.SaveChangesAsync();
        
            return Ok(category);
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, "05XE7 - Não foi possível remover a categoria");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "05XE12 - Falha interna no servidor");
        }
        
        
    }
}