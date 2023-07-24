using CommonModel.DBModel;
using CommonModel.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IEmployeeRepository
    {
        void AddUsers(Users user);
        Users UserGetByEmailandPassword(string email);
        List<EmployeeDetails> AddEmployee(EmployeeDetails employee);
        IList<EmployeeDetails> GetAllData();
        IList<EmployeeDesignation> GetDesignation();
        EmployeeDetails GetEmployeeById(int id);
        void UpdateEmployee(EmployeeDetails employee);
        void DeleteEmployee(int id);
    }
}
