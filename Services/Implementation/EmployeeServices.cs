using CommonModel.DBModel;
using CommonModel.ViewModel;
using Repository.Interface;
using Services.Interface;
using System.Data;
using System.Data.SqlClient;
using Dapper;


namespace Services.Implementation
{
    public class EmployeeServices : IEmployeeServices
    {
        public readonly IEmployeeRepository _employeeRepository;

        public EmployeeServices(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }


        public void AddUsers(UsersViewModel users)
        {
            try
            {
                // Check if user with the provided email already exists
                var existingUser = _employeeRepository.UserGetByEmailandPassword(users.Email);
                if (existingUser != null)
                {
                    throw new Exception("User with the provided email already exists.");
                }

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(users.Password);

                var userDetails = new Users()
                {
                    Email = users.Email,
                    Password = hashedPassword,
                };

                _employeeRepository.AddUsers(userDetails);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                throw;
            }
        }

        public bool Login(LoginViewModel login)
        {
            try
            {
                if (login.Password == null)
                {
                    return false; // Invalid email or password
                }

                var user = _employeeRepository.UserGetByEmailandPassword(login.Email);

                if (user != null && BCrypt.Net.BCrypt.Verify(login.Password, user.Password))
                {
                    // Login successful
                    // You can add further logic here, such as setting a login session or generating a token
                    return true;
                }
                else
                {
                    // Login failed
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                throw;
            }
        }



        /// <summary>Adds the employee.</summary>
        /// <param name="employee">The employee.</param>
        public void AddEmployee(EmployeeDetailsViewModel employee)
        {
            try
            {
                var employeeDetails = new EmployeeDetails()

                {
                    Name = employee.Name,
                    DesignationId = employee.DesignationId,                    
                    ProfilePicture = employee.ProfilePicture,
                    Salary = employee.Salary,
                    DateOfBirth = employee.DateOfBirth,
                    Email = employee.Email,
                    Address = employee.Address,

                };

                _employeeRepository.AddEmployee(employeeDetails);

            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                throw;
            }
        }

        /// <summary>Gets all data.</summary>
        public IList<EmployeeDetailsViewModel> GetAllData()
        {
            try
            {
                var employeeList = _employeeRepository.GetAllData();
                List<EmployeeDetailsViewModel> employeedata = new List<EmployeeDetailsViewModel>();

                foreach (var Item in employeeList)
                {
                    employeedata.Add(new EmployeeDetailsViewModel()
                    {
                        Id = Item.Id,
                        Name = Item.Name,
                        DesignationId = Item.DesignationId,
                        Designation = Item.Designation,
                        ProfilePicture = Item.ProfilePicture,
                        Salary = Item.Salary,
                        DateOfBirth = Item.DateOfBirth,
                        Email = Item.Email,
                        Address = Item.Address
                    });
                }
                return employeedata;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                throw;
            }
        }

        /// <summary>Gets the employee details.</summary>
        public EmployeeDetailsViewModel GetEmployeeDetails(int id)
        {
            // Retrieve the existing employee details from the database
            var existingEmployee = _employeeRepository.GetEmployeeById(id);

            if (existingEmployee != null)
            {
                EmployeeDetailsViewModel employeedata = new EmployeeDetailsViewModel();

                // Update the necessary properties of the existing employee with the values from the provided employee object
                employeedata.Id = existingEmployee.Id;
                employeedata.Name = existingEmployee.Name;
                employeedata.ProfilePicture = existingEmployee.ProfilePicture;                      
                employeedata.DesignationId = existingEmployee.DesignationId;
                employeedata.Salary = existingEmployee.Salary;
                employeedata.DateOfBirth = existingEmployee.DateOfBirth;
                employeedata.Email = existingEmployee.Email;
                employeedata.Address = existingEmployee.Address;

                // Save the updated employee details back to the database
                return employeedata;
            }
            else
            {
                throw new Exception("Employee not found!");
            }
        }

        /// <summary>Updates the employee.</summary>
        /// <param name="employee">The employee.</param>
        public void UpdateEmployee(EmployeeDetailsViewModel employee)
        {
            try
            {
                EmployeeDetails employeedata = new EmployeeDetails();
                {
                    employeedata.Id = employee.Id;
                    employeedata.Name = employee.Name;
                    employeedata.DesignationId = employee.DesignationId;                 
                    if (employee.ProfilePicture != null)
                    {
                        employeedata.ProfilePicture = employee.ProfilePicture;
                    }
                    employeedata.Salary = employee.Salary;
                    employeedata.DateOfBirth = employee.DateOfBirth;
                    employeedata.Email = employee.Email;
                    employeedata.Address = employee.Address;

                }

                _employeeRepository.UpdateEmployee(employeedata);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                throw;
            }
        }

        /// <summary>Gets the designation data.</summary>
        public IList<EmployeeDesignationViewModel> GetDesignationData()
        {
            try
            {
                var employeedesignation = _employeeRepository.GetDesignation();
                List<EmployeeDesignationViewModel> employeedata = new List<EmployeeDesignationViewModel>();

                foreach (var item in employeedesignation)
                {
                    employeedata.Add(new EmployeeDesignationViewModel()
                    {
                        Id = item.Id,
                        Designation = item.Designation,
                    });
                }
                return employeedata;
            }
            catch (Exception ex)
            {
                // Handle the exception and log the error
                var errorMessage = $"An error occurred while retrieving employee designations. Error: {ex.Message}";
                throw new (errorMessage, ex);
            }
        }

        /// <summary>Deletes the employee.</summary>
        /// <param name="id">The identifier.</param>
        public void DeleteEmployee(int id)
        {
            try
            {
                _employeeRepository.DeleteEmployee(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                throw;
            }
        }
    }
}

