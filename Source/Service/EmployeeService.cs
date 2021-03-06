﻿using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Infrastructure;
using Model.Models;
using Model.ViewModels;
using Core.Common;
using System;
using Model;
using System.Threading.Tasks;
using Data.Models;
using System.Data.SqlClient;

namespace Service
{
    public interface IEmployeeService : IDisposable
    {
        Employee Create(Employee Employee);
        void Delete(int id);
        void Delete(Employee Employee);
        Employee GetEmployee(int EmployeeId);
        IEnumerable<Employee> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(Employee Employee);
        Employee Add(Employee Employee);
        IEnumerable<Employee> GetEmployeeList();
        Task<IEquatable<Employee>> GetAsync();
        Task<Employee> FindAsync(int id);
        Employee GetEmployeeByName(string Name);
        Employee Find(int id);
        Employee FindByPersonId(int PersonId);
        EmployeeViewModel GetEmployeeViewModel(int id);
        int NextId(int id);
        int PrevId(int id);
        IQueryable<EmployeeIndexViewModel> GetEmployeeListForIndex();
        int ? GetEmloyeeForUser(string UserId);
        IEnumerable<EmployeeSalaryChangeWizardResultViewModel> GetSalaryDetail(EmployeeSalaryChangeWizardViewModel vm);
    }

    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWorkForService _unitOfWork;
        ApplicationDbContext db = new ApplicationDbContext();
        public EmployeeService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public Employee GetEmployeeByName(string Employee)
        {
            return (from b in db.Employee
                    join p in db.Persons on b.PersonID equals p.PersonID into PersonTable
                    from PersonTab in PersonTable.DefaultIfEmpty()
                    where PersonTab.Name == Employee
                    select b
                        ).FirstOrDefault();
        }
        public Employee GetEmployee(int EmployeeId)
        {
            return _unitOfWork.Repository<Employee>().Find(EmployeeId);
        }

        public Employee Find(int id)
        {
            return _unitOfWork.Repository<Employee>().Find(id);
        }

        public Employee FindByPersonId(int PersonId)
        {

            return _unitOfWork.Repository<Employee>().Query().Get().Where(m => m.PersonID == PersonId).FirstOrDefault(); 

        }

        public Employee Create(Employee Employee)
        {
            Employee.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<Employee>().Insert(Employee);
            return Employee;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<Employee>().Delete(id);
        }

        public void Delete(Employee Employee)
        {
            _unitOfWork.Repository<Employee>().Delete(Employee);
        }

        public void Update(Employee Employee)
        {
            Employee.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<Employee>().Update(Employee);
        }

        public IEnumerable<Employee> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var Employee = _unitOfWork.Repository<Employee>()
                .Query().Include(m => m.Person)
                .OrderBy(q => q.OrderBy(c => c.Person.Name))
                .Filter(q => !string.IsNullOrEmpty(q.Person.Name))
                .GetPage(pageNumber, pageSize, out totalRecords);

            return Employee;
        }

        public IEnumerable<Employee> GetEmployeeList()
        {
            var Employee = _unitOfWork.Repository<Employee>().Query().Include(m => m.Person).Get().Where(m => m.Person.IsActive == true).OrderBy(m => m.Person.Name);

            return Employee;
        }

        public Employee Add(Employee Employee)
        {
            _unitOfWork.Repository<Employee>().Insert(Employee);
            return Employee;
        }

        public void Dispose()
        {
        }


        public Task<IEquatable<Employee>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Employee> FindAsync(int id)
        {
            throw new NotImplementedException();
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from b in db.Employee
                        join p in db.Persons on b.PersonID equals p.PersonID into PersonTable
                        from PersonTab in PersonTable.DefaultIfEmpty()
                        orderby PersonTab.Name
                        select b.EmployeeId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from b in db.Employee
                        join p in db.Persons on b.PersonID equals p.PersonID into PersonTable
                        from PersonTab in PersonTable.DefaultIfEmpty()
                        orderby PersonTab.Name
                        select b.EmployeeId).FirstOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        public int PrevId(int id)
        {

            int temp = 0;
            if (id != 0)
            {

                temp = (from b in db.Employee
                        join p in db.Persons on b.PersonID equals p.PersonID into PersonTable
                        from PersonTab in PersonTable.DefaultIfEmpty()
                        orderby PersonTab.Name
                        select b.EmployeeId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from b in db.Employee
                        join p in db.Persons on b.PersonID equals p.PersonID into PersonTable
                        from PersonTab in PersonTable.DefaultIfEmpty()
                        orderby PersonTab.Name
                        select b.EmployeeId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        public EmployeeViewModel GetEmployeeViewModel(int id)
        {
            EmployeeViewModel Employeeviewmodel = (from b in db.Employee
                                                     join bus in db.BusinessEntity on b.PersonID equals bus.PersonID into BusinessEntityTable
                                                     from BusinessEntityTab in BusinessEntityTable.DefaultIfEmpty()
                                                     join p in db.Persons on BusinessEntityTab.PersonID equals p.PersonID into PersonTable
                                                     from PersonTab in PersonTable.DefaultIfEmpty()
                                   join pa in db.PersonAddress on b.PersonID equals pa.PersonId into PersonAddressTable
                                   from PersonAddressTab in PersonAddressTable.DefaultIfEmpty()
                                   join ac in db.LedgerAccount on b.PersonID equals ac.PersonId into AccountTable
                                   from AccountTab in AccountTable.DefaultIfEmpty()
                                   where b.EmployeeId == id
                                   select new EmployeeViewModel
                                   {
                                       EmployeeId = b.EmployeeId,
                                       PersonId = b.PersonID,
                                       DocTypeId = PersonTab.DocTypeId,
                                       Name = PersonTab.Name,
                                       Suffix = PersonTab.Suffix,
                                       Code = PersonTab.Code,
                                       Phone = PersonTab.Phone,
                                       Mobile = PersonTab.Mobile,
                                       Email = PersonTab.Email,
                                       Address = PersonAddressTab.Address,
                                       CityId = PersonAddressTab.CityId,
                                       Zipcode = PersonAddressTab.Zipcode,
                                       TdsCategoryId = BusinessEntityTab.TdsCategoryId,
                                       TdsGroupId = BusinessEntityTab.TdsGroupId,
                                       IsSisterConcern = BusinessEntityTab.IsSisterConcern,
                                       DesignationId = b.DesignationID,
                                       DepartmentId = b.DepartmentID,
                                       CreaditDays = BusinessEntityTab.CreaditDays,
                                       CreaditLimit = BusinessEntityTab.CreaditLimit,
                                       IsActive = PersonTab.IsActive,
                                       LedgerAccountGroupId = AccountTab.LedgerAccountGroupId,
                                       CreatedBy = PersonTab.CreatedBy,
                                       CreatedDate = PersonTab.CreatedDate,
                                       PersonAddressID = PersonAddressTab.PersonAddressID,
                                       AccountId = AccountTab.LedgerAccountId,
                                       DivisionIds = BusinessEntityTab.DivisionIds,
                                       SiteIds = BusinessEntityTab.SiteIds,
                                       Tags = PersonTab.Tags,
                                       BasicSalary = b.BasicSalary,
                                       GrossSalary = b.GrossSalary,
                                       DateOfJoining = b.DateOfJoining,
                                       DateOfRelieving = b.DateOfRelieving,
                                       WagesPayType = b.WagesPayType,
                                       PaymentType = b.PaymentType,
                                       ImageFileName = PersonTab.ImageFileName,
                                       ImageFolderName = PersonTab.ImageFolderName
                                   }
                   ).FirstOrDefault();

            var PersonProcess = (from pp in db.PersonProcess
                                 where pp.PersonId == Employeeviewmodel.PersonId
                                 select new
                                 {
                                     ProcessId = pp.ProcessId
                                 }).ToList();

            foreach (var item in PersonProcess)
            {
                if (Employeeviewmodel.ProcessIds == "" || Employeeviewmodel.ProcessIds == null)
                {
                    Employeeviewmodel.ProcessIds = item.ProcessId.ToString();
                }
                else
                {
                    Employeeviewmodel.ProcessIds = Employeeviewmodel.ProcessIds + "," + item.ProcessId.ToString();
                }
            }


            var PersonRegistration = (from pp in db.PersonRegistration
                                      where pp.PersonId == Employeeviewmodel.PersonId
                                      select new
                                      {
                                          PersonRegistrationId = pp.PersonRegistrationID,
                                          RregistrationType = pp.RegistrationType,
                                          RregistrationNo = pp.RegistrationNo
                                      }).ToList();

            if (PersonRegistration != null)
            {
                foreach (var item in PersonRegistration)
                {
                    if (item.RregistrationType == PersonRegistrationType.PANNo)
                    {
                        Employeeviewmodel.PersonRegistrationPanNoID = item.PersonRegistrationId;
                        Employeeviewmodel.PanNo = item.RregistrationNo;
                    }
                }
            }

            string Divisions = Employeeviewmodel.DivisionIds;
            if (Divisions != null)
            {
                Divisions = Divisions.Replace('|', ' ');
                Employeeviewmodel.DivisionIds = Divisions;
            }

            string Sites = Employeeviewmodel.SiteIds;
            if (Sites != null)
            {
                Sites = Sites.Replace('|', ' ');
                Employeeviewmodel.SiteIds = Sites;
            }

            return Employeeviewmodel;

          }

        public IQueryable<EmployeeIndexViewModel> GetEmployeeListForIndex()
        {
            var temp = from p in db.Employee
                       join p1 in db.Persons on p.PersonID equals p1.PersonID into PersonTable
                       from PersonTab in PersonTable.DefaultIfEmpty()
                       join D in db.Department on p.DepartmentID equals D.DepartmentId into DTable
                       from DTab in DTable.DefaultIfEmpty()
                       orderby PersonTab.Name
                       select new EmployeeIndexViewModel
                       {
                           EmployeeId = p.EmployeeId,
                           PersonId = PersonTab.PersonID,
                           Name = PersonTab.Name,
                           Code=PersonTab.Code,
                           Mobile=PersonTab.Mobile,
                           Phone=PersonTab.Phone,
                           Suffix = PersonTab.Suffix,
                           Department= DTab.DepartmentName,
                       };
            return temp;
        }

        public int ? GetEmloyeeForUser(string UserId)
        {
            var Emp = (from temp in db.Employee
                       join t in db.Persons on temp.PersonID equals t.PersonID
                       where t.ApplicationUser.Id == UserId
                       select temp.PersonID).FirstOrDefault();

            return Emp;


        }

        public IEnumerable<EmployeeSalaryChangeWizardResultViewModel> GetSalaryDetail(EmployeeSalaryChangeWizardViewModel vm)
        {
            string mQry = "";

            SqlParameter SqlParameterDocDate = new SqlParameter("@DocDate", vm.DocDate.ToString());
                SqlParameter SqlParameterRemark = new SqlParameter("@Remark", (vm.Remark == null) ? DBNull.Value : (object)vm.Remark);
                SqlParameter SqlParameterDepartmentId = new SqlParameter("@DepartmentId", (vm.DepartmentId == null) ? DBNull.Value : (object)vm.DepartmentId);
                SqlParameter SqlParameterWagesPayType = new SqlParameter("@WagesPayType", (vm.WagesPayType == null) ? DBNull.Value : (object)vm.WagesPayType);
            

            mQry = @"SELECT E.PersonID AS EmployeeId, E.Name+','+E.Suffix AS EmployeeName, Convert(int,E.Code) AS Code, isnull(EE.BasicSalary,0) as CurrentSalary, 
                    @DocDate As DocDate,  @Remark As HeaderRemark
                    FROM Web.People E
                    LEFT JOIN web.DocumentTypes DT ON DT.DocumentTypeId = E.DocTypeId
                    LEFT JOIN web.Employees EE ON EE.PersonID = E.PersonID
                    WHERE isnull(E.IsActive,1) =1 AND DT.DocumentTypeName = 'Employee'" +
                    (vm.DepartmentId != null ? " AND EE.DepartmentId IN (SELECT Items FROM [dbo].[Split] (@DepartmentId, ',')) " : "") +
                    (vm.WagesPayType != null ? " AND EE.WagesPayType = @WagesPayType" : "") +
                    " Order By E.Name+','+E.Suffix ";

            IEnumerable<EmployeeSalaryChangeWizardResultViewModel> EmployeeSalaryChangeWizardResultViewModel = db.Database.SqlQuery<EmployeeSalaryChangeWizardResultViewModel>(mQry, SqlParameterDocDate, SqlParameterRemark, SqlParameterDepartmentId, SqlParameterWagesPayType).ToList();
                return EmployeeSalaryChangeWizardResultViewModel;
            }




    }

    public class EmployeeSalaryChangeWizardResultViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string DocDate { get; set; }
        public int Code { get; set; }
        public string WagesPayType { get; set; }
        public Decimal CurrentSalary { get; set; }
        public Decimal NewSalary { get; set; }
        public string HeaderRemark { get; set; }
    }

}
