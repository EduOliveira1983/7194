using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Model;

namespace Shop.Controllers.v1
{
    [Route("v1/products")]
    public class ProductController : ControllerBase
    {
        [HttpGet]    
        [Route("")]   
        public async Task<ActionResult<List<Product>>> Get([FromServices]DataContext context)
        {
            var products = await context.Products
                                        .Include(x => x.Category)
                                        .AsNoTracking()
                                        .ToListAsync();
            if (products == null)
                return NotFound();
            else
                return Ok(products);
        }       
        
        [HttpGet]
        [Route("{id:int}")]
        [Authorize]
        public async Task<ActionResult<Product>> GetById(int id,
                                                          [FromServices]DataContext context)
        {
            var product = await context.Products
                                        .Include(x =>x.Category)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(x => x.ID == id);
            if (product == null)
                return NotFound();
            else
                return Ok(product);
        }

        [HttpGet]
        [Route("categories/{id:int}")]         
        public async Task<ActionResult<List<Product>>> GetByCategory(int id,
                                                                    [FromServices]DataContext context)
        {
            var products = await context.Products
                                        .Include(x =>x.Category)
                                        .AsNoTracking()
                                        .Where(x => x.Category.ID == id).ToListAsync();
            if (products == null)
                return NotFound();
            else
                return Ok(products);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Product>> Post([FromBody]Product product, 
                                                       [FromServices] DataContext context )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Products.Add(product);
                await context.SaveChangesAsync();

                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = "Erro ao inserir produto",
                                       detalhes = $"Erro:{ex.Message}"});                
            }

        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize]
        public async Task<ActionResult<Product>>  Put( int id, 
                                                        [FromBody]Product product,
                                                        [FromServices]DataContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (product.ID != id)
                return NotFound(new {message = "Produto não encontrado"});

                context.Entry<Product>(product).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(product);            
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return BadRequest(new {message="O registro já foi atualizado",
                                       detalhes=$"Erro:{ex.Message}"});
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
                var product = await context.Products.FirstOrDefaultAsync(x => x.ID == id);
                if (product == null)
                    return NotFound(new {message="Produto não encontrado"});

                context.Products.Remove(product);
                await context.SaveChangesAsync();

                return Ok(new {message="Produto removido com sucesso"});
            }
            catch (Exception ex)
            {
                return BadRequest(new { message="Erro ao remover produto",
                                        detalhes=$"{ex.Message}"});
            }
        }


    }
}