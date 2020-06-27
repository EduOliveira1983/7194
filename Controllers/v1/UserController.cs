using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Model;
using Shop.Services;

namespace Shop.Controllers.v1
{
    [Route("v1/users")]
    public class UserController : ControllerBase
    {

        [HttpGet]    
        [Route("")]  
        [Authorize(Roles = "Admin")] 
        public async Task<ActionResult<List<User>>> Get([FromServices]DataContext context)
        {
            var users = await context.Users
                                     .AsNoTracking()
                                     .ToListAsync();
            if (users == null)
                return NotFound();
            else
                return Ok(users);
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<User>> Post([FromBody]User user, 
                                                       [FromServices] DataContext context )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                user.Role = "employee";
                context.Users.Add(user);
                await context.SaveChangesAsync();

                user.Password = "";

                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = "Erro ao inserir produto",
                                       detalhes = $"Erro:{ex.Message}"});                
            }

        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<ActionResult<dynamic>> Authenticate(
            [FromServices] DataContext context,
            [FromBody]User model)
            {
                var user = await context.Users
                    .AsNoTracking()
                    .Where(x => x.Username == model.Username && x.Password == model.Password)
                    .FirstOrDefaultAsync();

                if (user == null)
                    return NotFound(new {message = "Usuário ou senha inválidos"});

                var token = TokenService.GenerateToken(user);
                user.Password = "";
                return new {
                    user = user,
                    token = token

                };
            } 

    }

}