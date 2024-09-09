using Microsoft.AspNetCore.Mvc;
using UserRegistryAPI.Models;
using UserRegistryAPI.Services;
using System;
using System.Threading.Tasks;

namespace UserRegistryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        // Constructor que inyecta el servicio de usuarios
        public UserController(UserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Método POST para crear un nuevo usuario.
        /// </summary>
        /// <param name="user">Información del usuario a crear</param>
        /// <returns>Resultado de la creación</returns>
        [HttpPost]
        public async Task<IActionResult> CreateUser(User user)
        {
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
                // Manejo de errores específicos (por ejemplo, errores de validación específicos)
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                // Manejo genérico de excepciones
                return StatusCode(500, "Error interno. Por favor, inténtelo de nuevo más tarde.");
            }
        }

        /// <summary>
        /// Método GET para obtener todos los usuarios.
        /// </summary>
        /// <returns>Lista de usuarios</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();

                // Validar si no se encontraron usuarios
                if (users == null || users.Count() == 0)
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
        /// Método GET para obtener un usuario por su ID.
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <returns>Usuario con el ID especificado</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            // Validar que el ID sea positivo
            if (id <= 0)
            {
                return BadRequest("El ID debe ser un número positivo.");
            }

            try
            {
                var user = await _userService.GetUserByIdAsync(id);

                // Verificar si el usuario no fue encontrado
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
        /// Método PUT para actualizar un usuario existente.
        /// </summary>
        /// <param name="id">ID del usuario a actualizar</param>
        /// <param name="user">Información del usuario actualizada</param>
        /// <returns>Resultado de la actualización</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            // Verificar que el ID proporcionado coincida con el ID del usuario
            if (id != user.Id)
            {
                return BadRequest("El ID proporcionado no coincide con el ID del usuario.");
            }

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
                // Intentar actualizar el usuario utilizando el servicio
                await _userService.UpdateUserAsync(user);
                return Ok("Usuario actualizado exitosamente.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno. Por favor, inténtelo de nuevo más tarde.");
            }
        }

        /// <summary>
        /// Método DELETE para eliminar un usuario por su ID.
        /// </summary>
        /// <param name="id">ID del usuario a eliminar</param>
        /// <returns>Resultado de la eliminación</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            // Validar que el ID sea positivo
            if (id <= 0)
            {
                return BadRequest("El ID debe ser un número positivo.");
            }

            try
            {
                var user = await _userService.GetUserByIdAsync(id);

                // Verificar si el usuario no fue encontrado
                if (user == null)
                {
                    return NotFound($"Usuario con ID {id} no encontrado.");
                }

                // Intentar eliminar el usuario
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
