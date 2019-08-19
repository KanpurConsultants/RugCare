using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Core.Common;
using Model.Models;
using Data.Models;
using Service;
using Data.Infrastructure;
using System.Configuration;
using Model.ViewModel;
using System.Data.SqlClient;
using System.Data;
using System.Xml.Linq;
using Model.DatabaseViews;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using Jobs.Helpers;
using System.Text;
using Model.ViewModels;
using System.Web.Script.Serialization;

namespace Jobs.Controllers
{
    [Authorize]
    public class EmployeeSalaryChangeWizardController : System.Web.Mvc.Controller
    {
        ActiivtyLogViewModel LogVm = new ActiivtyLogViewModel();
        private ApplicationDbContext db = new ApplicationDbContext();
        protected string connectionString = (string)System.Web.HttpContext.Current.Session["DefaultConnectionString"];
        IEmployeeService _EmployeeService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;
        List<string> UserRoles = new List<string>();


        public EmployeeSalaryChangeWizardController(IEmployeeService EmployeeService, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _EmployeeService = EmployeeService;
            UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];

            _exception = exec;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public ActionResult EmployeeSalaryChangeWizard()
        {
            EmployeeSalaryChangeWizardViewModel vm = new EmployeeSalaryChangeWizardViewModel();
            vm.DocDate = DateTime.Now;

            List<SelectListItem> WagesPayTypeList = new List<SelectListItem>();
            WagesPayTypeList.Add(new SelectListItem { Text = "Daily", Value = "Daily" });
            WagesPayTypeList.Add(new SelectListItem { Text = "Monthly", Value = "Monthly" });
            ViewBag.WagesPayTypeList = WagesPayTypeList;

            return View(vm);
        }

        public JsonResult EmployeeSalaryChangeWizardFill(EmployeeSalaryChangeWizardViewModel vm)
        {
            IEnumerable<EmployeeSalaryChangeWizardResultViewModel> EmployeeSalaryChangeWizardResultViewModel = _EmployeeService.GetSalaryDetail(vm);

            JsonResult json = Json(new { Success = true, Data = EmployeeSalaryChangeWizardResultViewModel }, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }

        public string Post(List<EmployeeSalaryChangeWizardResultViewModel> SalaryDataList)   
        {
            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];


            Charge  CGrossAmount = new ChargeService(_unitOfWork).Find("Gross Amount");
            Charge CBasicSalary = new ChargeService(_unitOfWork).Find("Basic Salary");
            Charge CNetSalary = new ChargeService(_unitOfWork).Find("Net Salary");

            if (SalaryDataList.Count > 0)
            {



                DateTime DocDate = Convert.ToDateTime(SalaryDataList.FirstOrDefault().DocDate);
                string Remark = SalaryDataList.FirstOrDefault().HeaderRemark;

                int i = 0;
                foreach (var SalaryData in SalaryDataList)
                {
                    if (SalaryData.NewSalary > 0 && SalaryData.NewSalary != SalaryData.CurrentSalary)
                    {

                        Person P = new PersonService(_unitOfWork).Find(SalaryData.EmployeeId);

                        Employee E = new EmployeeService(_unitOfWork).FindByPersonId(SalaryData.EmployeeId);
                        E.BasicSalary = SalaryData.NewSalary;
                        _EmployeeService.Update(E);


                        EmployeeLineCharge ELC = (from p in db.EmployeeLineCharge
                                                 where p.PersonID == E.PersonID && p.ChargeId == CGrossAmount.ChargeId
                                                  select p).FirstOrDefault();

                        if (ELC != null)
                        {
                            ELC.Amount = SalaryData.NewSalary;
                            new EmployeeLineChargeService(_unitOfWork).Update(ELC);
                        }

                        EmployeeCharge EC = (from p in db.EmployeeCharge
                                             where p.PersonID == E.PersonID && p.ChargeId == CBasicSalary.ChargeId
                                             select p).FirstOrDefault();
                        if (EC != null)
                        {
                            EC.Amount = SalaryData.NewSalary;
                            new EmployeeChargeService(_unitOfWork).Update(EC);

                        }


                       EmployeeCharge ECN = (from p in db.EmployeeCharge
                                             where p.PersonID == E.PersonID && p.ChargeId == CNetSalary.ChargeId
                                             select p).FirstOrDefault();
                        if (ECN != null)
                        {
                            ECN.Amount = SalaryData.NewSalary;
                            new EmployeeChargeService(_unitOfWork).Update(ECN);
                        }

                        string logstring = " Basic Salary Changed From " + SalaryData.CurrentSalary.ToString() + " To " + SalaryData.NewSalary.ToString();


                        LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                        {
                            DocTypeId = P.DocTypeId,
                            DocId = E.EmployeeId,
                            Narration= logstring,
                            ActivityType = (int)ActivityTypeContants.Modified,
                            DocNo = P.Name,
                            CreatedDate = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                        }));

                    }

                }


                try
                {
                    db.SaveChanges();
                }

                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    TempData["CSEXC"] += message;
                }

                return (string)System.Configuration.ConfigurationManager.AppSettings["JobsDomain"] + "/EmployeeSalaryChangeWizard/EmployeeSalaryChangeWizard";
            }
            return null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
