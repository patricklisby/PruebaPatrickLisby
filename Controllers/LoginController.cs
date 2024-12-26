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
    /// <summary>
    /// Controlador que gestiona las vistas para el inicio de sesión y el registro de usuarios.
    /// </summary>
    public class LoginController : Controller
    {
        private readonly string _connectionString;

        /// <summary>
        /// Constructor que inicializa el controlador con la configuración de conexión a la base de datos.
        /// </summary>
        /// <param name="configuration">Proporciona acceso a las configuraciones de la aplicación.</param>
        public LoginController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        /// <summary>
        /// Muestra la vista del formulario de inicio de sesión.
        /// </summary>
        /// <returns>Vista `Login` para que el usuario ingrese sus credenciales.</returns>
        [HttpGet]
        public IActionResult LoginForm()
        {
            return View("Login");
        }

        /// <summary>
        /// Muestra la vista para registrar un nuevo usuario.
        /// </summary>
        /// <returns>Vista `Register` para que el usuario pueda registrarse.</returns>
        [HttpGet]
        public IActionResult Registrar()
        {
            return View("Register");
        }


    }//fin clase LoginController

}//Fin namespace PruebaPatrickLisby.Controllers
