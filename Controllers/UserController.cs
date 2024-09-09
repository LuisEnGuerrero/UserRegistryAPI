using Microsoft.AspNetCore.Mvc;
using UserRegistryAPI.Models;
using UserRegistryAPI.Services;

namespace UserRegistryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

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
                await _userService.CreateUserAsync(user);
                return Ok("Usuario creado exitosamente.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

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
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
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
                await _userService.UpdateUserAsync(user);
                return Ok("Usuario actualizado exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (id <= 0)
            {
                return BadRequest("El ID debe ser un número positivo.");
            }

            if (user == null)
            {
                return NotFound($"Usuario con ID {id} no encontrado.");
            }

            try
            {
                await _userService.DeleteUserAsync(id);
                return Ok("Usuario eliminado exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }


    }
}
