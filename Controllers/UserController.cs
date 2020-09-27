using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly DataContext datacontext;
        public UsersController(DataContext _datacontext)
        {
            this.datacontext = _datacontext;
        }
        [HttpGet]
        public async Task<ActionResult <IEnumerable<WeatherForecast>>> GetUsers()
        {
              return Ok (await datacontext.Users.ToListAsync());             
        }
        [HttpGet("{Id}")]
        public async Task<ActionResult <IEnumerable<WeatherForecast>>> GetUser(int Id)
        {
              return Ok(await datacontext.Users.FirstOrDefaultAsync(x=>x.Id==Id));             
        }
    }
    }
 