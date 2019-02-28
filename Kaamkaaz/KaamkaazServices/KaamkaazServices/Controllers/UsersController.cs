using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KaamkaazServices.Filters;
using KaamkaazServices.Helper;
using KaamkaazServices.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace KaamkaazServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]    
    [Authorize(AuthenticationSchemes = "amx")]
    public class UsersController : ControllerBase
    {
        private readonly IConfiguration configuration;

        // GET: api/User/5
        [HttpGet]
        public User Get(int id)
        {
            var respository = new BlueColorDB(configuration.GetConnectionString("BlueColor"));
            return respository.GetUser(id);
        }
        public UsersController(IConfiguration configuration)
        {
            //TODO: use options pattern
            this.configuration = configuration;
        }

        //[HttpPost]
        //public CreateUserResponse Post()
        //{
        //    return new CreateUserResponse();
        //}
        // POST: api/User
        [HttpPost]           
        public CreateUserResponse Post(User user)
        {
            //Create new user
            var repository = new BlueColorDB(configuration.GetConnectionString("BlueColor"));
            var userId = Guid.NewGuid().ToString();
            var id = repository.CreateUser(user, userId);

            return new CreateUserResponse() { Success = id > 0 };
        }

        // PUT: api/User/5
        [HttpPut]
        public CreateUserResponse Put(User user)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.SelectMany(v => v.Value.Errors.Select(e => e.Exception));

                List<String> messages = new List<string>();

                foreach (Exception e in errors)
                {
                    messages.Add(e.GetType().ToString() + ": " + e.Message);
                }

            }
            if (user == null || user.Id <= 0)
            {
                return new CreateUserResponse() { Success = false, Message = "UserId is required." };
            }
            //Update existing user
            var repository = new BlueColorDB(configuration.GetConnectionString("BlueColor"));
            var rowAffected = repository.UpdateUser(user);
            return new CreateUserResponse() { Success = rowAffected > 0};
        }

    }
}
