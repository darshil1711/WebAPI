using CommonModel.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using System.Security.Claims;

namespace Web_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeServices _employeeServices;
        private readonly ITokenServices _tokenService;

        public EmployeeController(IEmployeeServices employeeServices, ITokenServices tokenService)
        {
            _employeeServices = employeeServices;
            _tokenService = tokenService;
        }

        [HttpGet("all")]
        public IActionResult Get()
        {
            try
            {
                var employeedatalist = _employeeServices.GetAllData();
                return Ok(employeedatalist);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [AllowAnonymous]
        [HttpPost("add-user")]
        public IActionResult AddUser(UsersViewModel user)
        {
            try
            {
                if (user.Password != user.ConfirmPassword)
                {
                    return BadRequest("Password and Confirm Password do not match.");
                }

                _employeeServices.AddUsers(user);
                return Ok("User created successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to add user. Error: {ex.Message}");
            }
        }

        [AllowAnonymous] // Allows anonymous access
        [HttpPost("login")]
        public IActionResult Login(LoginViewModel usersLogin)
        {
            try
            {
                // Assuming you have validated the user's ID and password here  
                var user = _employeeServices.Login(usersLogin);

                if (!user)
                {
                    return BadRequest("Invalid email or password.");
                }

                // Generate JWT token
                var claims = new List<Claim>
        {
            new Claim(Microsoft.IdentityModel.Claims.ClaimTypes.Email, usersLogin.Email),
            // Add any additional claims here as needed
        };
                var claimsIdentity = new ClaimsIdentity(claims, "EmployeeAuthentication");
                var jwtToken = _tokenService.CreatejWTToken(claimsIdentity);

                // Return the token in the response
                var response = new LoginJWT
                {
                    JWToken = jwtToken
                };

                return Ok(new { Token = response.JWToken, Message = "Login successful" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to login. Error: {ex.Message}");
            }
        }

        [HttpPost("add")]
        public IActionResult Create(EmployeeDetailsViewModel employee)
        {
            try
            {
                _employeeServices.AddEmployee(employee);
                return Ok("Employee created successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to add employee. Error: {ex.Message}");
            }
        }

        [HttpPut("update/{id}")]
        public IActionResult Update(int id, EmployeeDetailsViewModel employee)
        {
            try
            {
                employee.Id = id;
                _employeeServices.UpdateEmployee(employee);
                return Ok("Employee updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to update employee. Error: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("Designation")]
        public IActionResult GetDesignations()
        {
            try
            {
                var designations = _employeeServices.GetDesignationData();
                return Ok(designations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to retrieve employee designations. Error: {ex.Message}");
            }
        }

        [HttpGet("get/{id}")]
        public IActionResult GetEmployeeDetailsById(int id)
        {
            try
            {
                var employee = _employeeServices.GetEmployeeDetails(id);
                if (employee == null)
                {
                    return NotFound("Employee not found.");
                }
                return Ok(employee);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Failed to retrieve employee details. Error: {ex.Message}");
            }
        }

        [HttpDelete("Delete/{id}")]
        public IActionResult DeleteEmployee(int id)
        {
            try
            {
                _employeeServices.DeleteEmployee(id);
                return Ok("Employee deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to delete employee. Error: {ex.Message}");
            }
        }
    }
}
