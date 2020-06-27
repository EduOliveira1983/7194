using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Model;

namespace Shop.Controllers.v1
{
    [Route("v1")]
    public class HomeController : ControllerBase
    {
        [HttpGet]       
        public async Task<ActionResult<dynamic>> GetConfig([FromServices]DataContext context)
        {
            try
            {
                var employee = new User {ID = 1, Username = "empregado", Password = "empregado", Role= "employee"};
                var admin = new User{ID = 2, Username = "admin", Password = "admin", Role="admin"};
                var category = new Category{ID = 1, Title = "categoria 1"};
                var product = new Product{ ID = 1, CategoryId = 1, Description = "Produto 1", Price = 301, Title = "Produto 1"};

                context.Users.Add(employee);
                context.Users.Add(admin);
                context.Categories.Add(category);
                context.Products.Add(product);

                await context.SaveChangesAsync();

                return Ok(new {message = "Dados Configurados"});

            }
            catch (Exception ex)
            {                
                return BadRequest(new {message=$"Erro ao Inserir Dados: {ex.Message}"});
            }
            
        }   
    }
}