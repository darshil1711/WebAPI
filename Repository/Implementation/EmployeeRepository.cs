using CommonModel.DBModel;
using Dapper;
using Repository.Interface;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.Metadata;

namespace Repository.Implementation
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly string connectionString;

        public EmployeeRepository()
        {
            connectionString = "Data Source=LAPTOP-HL8L5JT3\\SQLEXPRESS;Initial Catalog=EmployeeManagements;Integrated Security=True;";
        }

        public void AddUsers(Users user)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    var parameters = new DynamicParameters();
                    parameters.Add("@Email", user.Email);
                    parameters.Add("@Password", user.Password);

                    var affectedRows = connection.Execute("UsersRegistration", parameters, commandType: CommandType.StoredProcedure);

                    if (affectedRows == 0)
                    {
                        throw new Exception("Failed to add user. Please try again. LoginUser ");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                throw;
            }
        }

        public Users UserGetByEmailandPassword(string email)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var parameters = new DynamicParameters();
                parameters.Add("@Email", email);

                return connection.QueryFirstOrDefault<Users>("LoginUser", parameters, commandType: CommandType.StoredProcedure);
            }
        }


        public List<EmployeeDetails> AddEmployee(EmployeeDetails employee)
        {
            try
            {
                var parameter = new DynamicParameters();

                using (IDbConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    parameter.Add("@Name", employee.Name);
                    parameter.Add("@DesignationId", employee.DesignationId);                    
                    parameter.Add("@ProfilePicture", employee.ProfilePicture);
                    parameter.Add("@Salary", employee.Salary);
                    parameter.Add("@DateOfBirth", employee.DateOfBirth);
                    parameter.Add("@Email", employee.Email);
                    parameter.Add("@Address", employee.Address);
                    return connection.Query<EmployeeDetails>("InsertEmployeeDetails", parameter, commandType: CommandType.StoredProcedure).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                throw;
            }
        }

        public IList<EmployeeDetails> GetAllData()
        {
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                return connection.Query<EmployeeDetails>("GetAllEmployeeDetails").ToList();
            }
        }

        public IList<EmployeeDesignation> GetDesignation()
        {
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                return connection.Query<EmployeeDesignation>("GetEmployeeDesignations").ToList();
            }
        }

        public EmployeeDetails GetEmployeeById(int id)
        {
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@Id",id);
                string query = "SELECT * FROM EmployeeDetails WHERE Id = @Id";
                return connection.QueryFirstOrDefault<EmployeeDetails>("GetEmployeeDetailsById", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public void UpdateEmployee(EmployeeDetails employee)
        {
            try
            {
                var parameter = new DynamicParameters();

                using (IDbConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    parameter.Add("@Id", employee.Id);
                    parameter.Add("@Name", employee.Name);
                    parameter.Add("@DesignationId", employee.DesignationId);                    
                    parameter.Add("@ProfilePicture", employee.ProfilePicture);
                    parameter.Add("@Salary", employee.Salary);
                    parameter.Add("@DateOfBirth", employee.DateOfBirth);
                    parameter.Add("@Email", employee.Email);
                    parameter.Add("@Address", employee.Address);
                    connection.Query<EmployeeDetails>("UpdateEmployeeDetails", parameter, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                throw;
            }
        }

        public void DeleteEmployee(int id)
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var parameter = new DynamicParameters();
                    parameter.Add("@Id", id);
                    connection.Execute("DeleteEmployeeDetailsById", parameter, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                throw;
            }
        }
    }
}
