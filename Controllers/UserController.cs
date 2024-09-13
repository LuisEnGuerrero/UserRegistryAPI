using Microsoft.AspNetCore.Mvc;
using UserRegistryAPI.Models;
using UserRegistryAPI.Services;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Npgsql;

namespace UserRegistryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly string _connectionString;

        // Constructor que inyecta el servicio de usuarios
        public UserController(UserService userService)
        {
            _userService = userService;
            _connectionString = "your_connection_string_here"; // Cadena de conexión a la base de datos
        }

        /// <summary>
        /// Test de conexión a la base de datos. Este método es temporal y solo debe usarse para pruebas.
        /// </summary>
        [HttpGet("test-connection")]
        public IActionResult TestConnection()
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    return Ok("Connection to the database was successful!");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Connection failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Crear un nuevo usuario-registro. Solo AdminMaster y CreatorAdmin pueden realizar esta acción.
        /// </summary>
        [Authorize(Roles = "AdminMaster, CreatorAdmin")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] Models.User request)
        {
            var user = new User
            {
                Name = request.Name,
                Phone = request.Phone,
                CountryId = request.CountryId,
                DepartmentId = request.DepartmentId,
                MunicipalityId = request.MunicipalityId
            };

            // Validar que los campos no estén vacíos o nulos
            if (string.IsNullOrWhiteSpace(user.Name) || string.IsNullOrWhiteSpace(user.Phone))
            {
                return BadRequest("El nombre y teléfono son requeridos.");
            }

            // Validar que los IDs numéricos sean positivos
            if (user.CountryId <= 0 || user.DepartmentId <= 0 || user.MunicipalityId <= 0)
            {
                return BadRequest("IDs de país, departamento y municipalidad deben ser mayores a cero.");
            }

            try
            {
                // Intentar crear el usuario utilizando el servicio
                await _userService.CreateUserAsync(user);
                return Ok("Usuario creado exitosamente.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Manejo de errores específicos
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno. Por favor, inténtelo de nuevo más tarde."); // Manejo genérico de excepciones
            }
        }

        /// <summary>
        /// Obtener la lista de usuarios-registro. Accesible por AdminMaster, Viewer, CreatorAdmin, EditorAdmin.
        /// </summary>
        [Authorize(Roles = "AdminMaster, Viewer, CreatorAdmin, EditorAdmin")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();

                // Validar si no se encontraron usuarios
                if (users == null || !users.Any())
                {
                    return NotFound("No se encontraron usuarios.");
                }

                return Ok(users);
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno. Por favor, inténtelo de nuevo más tarde.");
            }
        }

        /// <summary>
        /// Obtener un usuario por su ID. Accesible por AdminMaster, Viewer, CreatorAdmin, EditorAdmin.
        /// </summary>
        [Authorize(Roles = "AdminMaster, Viewer, CreatorAdmin, EditorAdmin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("El ID debe ser un número positivo.");
            }

            try
            {
                var user = await _userService.GetUserByIdAsync(id);

                if (user == null)
                {
                    return NotFound($"Usuario con ID {id} no encontrado.");
                }

                return Ok(user);
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno. Por favor, inténtelo de nuevo más tarde.");
            }
        }

        /// <summary>
        /// Actualizar un usuario-registro existente. Solo AdminMaster y EditorAdmin pueden modificar usuarios.
        /// </summary>
        [Authorize(Roles = "AdminMaster, EditorAdmin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            if (id != user.Id)
            {
                return BadRequest("El ID proporcionado no coincide con el ID del usuario.");
            }

            if (string.IsNullOrWhiteSpace(user.Name) || string.IsNullOrWhiteSpace(user.Phone))
            {
                return BadRequest("El nombre y teléfono son requeridos.");
            }

            if (user.CountryId <= 0 || user.DepartmentId <= 0 || user.MunicipalityId <= 0)
            {
                return BadRequest("IDs de país, departamento y municipalidad deben ser mayores a cero.");
            }

            try
            {
                await _userService.UpdateUserAsync(user);
                return Ok("Usuario actualizado exitosamente.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno. Por favor, inténtelo de nuevo más tarde.");
            }
        }

        /// <summary>
        /// Eliminar un usuario por su ID. Solo AdminMaster puede realizar esta acción.
        /// </summary>
        [Authorize(Roles = "AdminMaster")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (id <= 0)
            {
                return BadRequest("El ID debe ser un número positivo.");
            }

            try
            {
                var user = await _userService.GetUserByIdAsync(id);

                if (user == null)
                {
                    return NotFound($"Usuario con ID {id} no encontrado.");
                }

                await _userService.DeleteUserAsync(id);
                return Ok("Usuario eliminado exitosamente.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno. Por favor, inténtelo de nuevo más tarde.");
            }
        }
    }
}
