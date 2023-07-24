using CommonModel.DBModel;
using CommonModel.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public  interface IEmployeeServices
    {
        void AddUsers(UsersViewModel users);
        bool Login(LoginViewModel login);
        void AddEmployee(EmployeeDetailsViewModel employee);
        IList<EmployeeDetailsViewModel> GetAllData();
        EmployeeDetailsViewModel GetEmployeeDetails(int id);
        void UpdateEmployee(EmployeeDetailsViewModel employee);
        IList<EmployeeDesignationViewModel> GetDesignationData();
        void DeleteEmployee(int id);
    }
}
