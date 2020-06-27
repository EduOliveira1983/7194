using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Model;

namespace Shop.Controllers.v1
{
    [Route("v1/categories")]
    public class CategoryController : ControllerBase
    {
        [HttpGet]    
        [Route("")]   
        [AllowAnonymous]
        [ResponseCache(VaryByHeader="User-Agent",Location = ResponseCacheLocation.Any, Duration = 30)]
        public async Task<ActionResult<List<Category>>> Get([FromServices]DataContext context)
        {
            var categories = await context.Categories.AsNoTracking().ToListAsync();
            if (categories == null)
                return NotFound();
            else
                return Ok(categories);
        }

         [HttpGet]
         [Route("{id:int}")]
         [Authorize]
         [ResponseCache(Duration=0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<ActionResult<Category>> GetById(int id,
                                                          [FromServices]DataContext context)
        {
            var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.ID == id);
            if (category == null)
                return NotFound();
            else
                return Ok(category);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Category>> Post([FromBody]Category category, 
                                                       [FromServices] DataContext context )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Categories.Add(category);
                await context.SaveChangesAsync();

                return Ok(category);
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = "Erro ao inserir categoria",
                                       detalhes = $"Erro:{ex.Message}"});                
            }

        }

        [HttpPut]
        [Authorize]
        [Route("{id:int}")]
        public async Task<ActionResult<Category>>  Put( int id, 
                                                        [FromBody]Category category,
                                                        [FromServices]DataContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (category.ID != id)
                return NotFound(new {message = "Categoria não encontrada"});

                context.Entry<Category>(category).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(category);            
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new {message="O registro já foi atualizado"});
            }
            catch (Exception ex)
            {                
                return BadRequest(new {message="Erro ao atualizar o registro",
                                       detalhes=$"{ex.Message}"});
            }           
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles="Admin")]
        public async Task<ActionResult> Delete(int id,
                                               [FromServices]DataContext context)
        {
            try
            {
                var category = await context.Categories.FirstOrDefaultAsync(x => x.ID == id);
                if (category == null)
                    return NotFound(new {message="Categoria não encontrada"});

                context.Categories.Remove(category);
                await context.SaveChangesAsync();

                return Ok(new {message="Categoria removida com sucesso"});
            }
            catch (Exception ex)
            {
                return BadRequest(new { message="Erro ao remover categoria",
                                        detalhes=$"{ex.Message}"});
            }
        }
        
    }
}