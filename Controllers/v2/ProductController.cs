using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Model;

namespace Shop.Controllers.v2
{
     [Route("v2/products")]   
    public class ProductController : ControllerBase
    {
       [HttpGet]    
       [Authorize]
        public async Task<ActionResult<List<Product>>> Get([FromServices]DataContext context)
        {
            var products = await context.Products                                        
                                        .AsNoTracking()
                                        .ToListAsync();
            if (products == null)
                return NotFound();
            else
                return Ok(products);
        }
        
    }
}