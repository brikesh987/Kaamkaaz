using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KaamkaazServices.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KaamkaazServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {       
        // GET: api/User/5
        [HttpGet("{id}")]
        public User Get(string id)
        {
            return new Models.User();
        }

        // POST: api/User
        [HttpPost]
        public CreateUserResponse Post(User user)
        {
            //Create new user
            return new CreateUserResponse();
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public CreateUserResponse Put(User user)
        {
            //Update existing user
            return new CreateUserResponse();
        }

    }
}
