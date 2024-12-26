using Microsoft.AspNetCore.Mvc;
using System.Data;
using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PruebaPatrickLisby.Models;
using Microsoft.AspNetCore.Identity.Data;

namespace PruebaPatrickLisby.Controllers
{
    public class LoginController : Controller
    {
        private readonly string _connectionString;

        public LoginController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        public IActionResult LoginForm()
        {
            return View("Login");
        }


    }//fin clase LoginController

}//Fin namespace PruebaPatrickLisby.Controllers
