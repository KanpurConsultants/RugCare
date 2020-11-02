using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Core.Common;
using Model.Models;
using Data.Models;
using Model.ViewModels;
using Service;
using Jobs.Helpers;
using Data.Infrastructure;
using System.Web.UI.WebControls;
using AutoMapper;
using Microsoft.AspNet.Identity;
using System.Configuration;
using Presentation;
using Model.ViewModel;
using Reports.Controllers;



namespace Jobs.Controllers
{
    [Authorize]
    public class BankReconciliation1Controller : System.Web.Mvc.Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        List<string> UserRoles = new List<string>();
        ActiivtyLogViewModel LogVm = new ActiivtyLogViewModel();

        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;

        public BankReconciliation1Controller(IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _exception = exec;
            _unitOfWork = unitOfWork;

            UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];

            //Log Initialization
            LogVm.SessionId = 0;
            LogVm.ControllerName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("controller");
            LogVm.ActionName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("action");
            LogVm.User = System.Web.HttpContext.Current.Request.RequestContext.HttpContext.User.Identity.Name;
        }

        public ActionResult Index()
        {
            IEnumerable<BankReconciliationIndexViewModel> BankReconciliationIndexViewModel = (from A in db.LedgerAccount
                                                                                              where A.LedgerAccountGroup.LedgerAccountNature == LedgerAccountNatureConstants.Bank
                                                                                              select new BankReconciliationIndexViewModel
                                                                                              {
                                                                                                  LedgerAccountId = A.LedgerAccountId,
                                                                                                  LedgerAccountName = A.LedgerAccountName
                                                                                              }).ToList();
            return View(BankReconciliationIndexViewModel);
        }

        public ActionResult BankReconciliationIndex(int id)//LedgerAccountid
        {
            ViewBag.TransactionType = "Pending";
            ViewBag.LedgerAccountId = id;
            ViewBag.Title = "Bank Reconciliation : " + new LedgerAccountService(_unitOfWork).Find(id).LedgerAccountName;
            return View("BankReconciliation");
        }

        public JsonResult LedgerAccountLedgerList(BankReconciliationFilterArgs Fvm)//LedgerAccountId
        {
            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];


            if (Fvm.TransactionType == "" || Fvm.TransactionType == null)
            {
                Fvm.TransactionType = "Pending";
            }

            var list = (from L in db.Ledger
                        join LL in db.LedgerLine on L.LedgerLineId equals LL.LedgerLineId into LLTable
                        from LLTab in LLTable.DefaultIfEmpty()
                        join H in db.LedgerHeader on L.LedgerHeaderId equals H.LedgerHeaderId into HTable
                        from HTab in HTable.DefaultIfEmpty()
                        where L.LedgerAccountId == Fvm.LedgerAccountId && HTab.SiteId== SiteId && HTab.DivisionId== DivisionId
                            && HTab.AdjustmentType=="Advance"
                            && (Fvm.TransactionType == "All" ? 1 == 1 : L.BankDate == null)
                            && (Fvm.FromDate == null ? 1 == 1 : L.LedgerHeader.DocDate >= Fvm.FromDate)
                            && (Fvm.ToDate == null ? 1 == 1 : L.LedgerHeader.DocDate <= Fvm.ToDate)
                            orderby L.LedgerHeader.DocDate, L.LedgerHeader.DocNo
                        select new BankReconciliationViewModel
                        {
                            LedgerId = L.LedgerId,
                            DocNo = L.LedgerHeader.DocNo,
                            DocDate = L.LedgerHeader.DocDate,
                            AccountName = L.ContraLedgerAccount.LedgerAccountName,
                            Narration = HTab.Narration +" "+LLTab.Remark,
                            ChqNo = L.ChqNo,
                            ChqDate = L.ChqDate,
                            AmtDr = L.AmtDr,
                            AmtCr = L.AmtCr,
                            PassedAmt = LLTab.Amount,
                            BankDate = L.BankDate,
                            PassedBy = L.PassedBy
                        }).ToList();

            var temp = list.Select(m => new 
            {
                LedgerId = m.LedgerId,
                DocNo = m.DocNo,
                DocDate = m.DocDate.ToString("dd/MMM/yyyy"),
                AccountName = m.AccountName,
                Narration = m.Narration,
                ChqNo = m.ChqNo,
                ChqDate = m.ChqDate == null ? "" : Convert.ToDateTime(m.ChqDate).ToString("dd/MMM/yyyy"),
                AmtDr = m.AmtDr,
                AmtCr = m.AmtCr,
                PassedAmt = m.PassedAmt,
                BankDate = m.BankDate == null ? "" : Convert.ToDateTime(m.BankDate).ToString("dd/MMM/yyyy"),
                PassedBy = m.PassedBy == null ? "" : m.PassedBy
            }).ToList();
            
            return Json(new { data = temp }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult UpdateBankDate(int LedgerId, Decimal PassedAmt, DateTime? BankDate, string PassedBy)
        {
            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            Ledger L = db.Ledger.Where(m =>m.LedgerId== LedgerId).FirstOrDefault();
            LedgerHeader H = db.LedgerHeader.Where(m => m.LedgerHeaderId == L.LedgerHeaderId).FirstOrDefault();

            if (L.AmtCr > PassedAmt)
            {
                var settings = new LedgerSettingService(_unitOfWork).GetLedgerSettingForDocument(H.DocTypeId, H.DivisionId, H.SiteId);
                int D = Convert.ToInt16(settings.filterContraDocTypes.ToString());
                DocumentType dt = db.DocumentType.Where(m => m.DocumentTypeId == D).FirstOrDefault();

                LedgerHeader LedgerHeader = new LedgerHeader();
                LedgerHeader.DocDate = (DateTime)BankDate;
                LedgerHeader.DivisionId = DivisionId;
                LedgerHeader.SiteId = SiteId;
                LedgerHeader.DocTypeId = dt.DocumentTypeId;
                LedgerHeader.DocNo = new DocumentTypeService(_unitOfWork).FGetNewDocNo("DocNo", ConfigurationManager.AppSettings["DataBaseSchema"] + ".LedgerHeaders", dt.DocumentTypeId, (DateTime)BankDate, DivisionId, SiteId);
                LedgerHeader.LedgerAccountId = L.LedgerAccountId;
                LedgerHeader.LockReason  = "Auto Generated From Petty Cash Hisab !";
                LedgerHeader.CreatedDate = DateTime.Now;
                LedgerHeader.ModifiedDate = DateTime.Now;
                LedgerHeader.CreatedBy = User.Identity.Name;
                LedgerHeader.ModifiedBy = User.Identity.Name;
                LedgerHeader.ObjectState = Model.ObjectState.Added;
                new LedgerHeaderService(_unitOfWork).Create(LedgerHeader);

                LedgerLine LedgerLine = new LedgerLine();
                LedgerLine.LedgerHeaderId = LedgerHeader.LedgerHeaderId;
                LedgerLine.LedgerAccountId = (int)L.ContraLedgerAccountId;
                LedgerLine.Amount = L.AmtCr - PassedAmt;
                LedgerLine.ReferenceId = L.LedgerId;
                LedgerLine.ReferenceDocTypeId = H.DocTypeId;
                LedgerLine.ReferenceDocId = H.LedgerHeaderId;
                LedgerLine.Remark = "Hisab "+L.Narration;
                LedgerLine.CreatedDate = DateTime.Now;
                LedgerLine.ModifiedDate = DateTime.Now;
                LedgerLine.CreatedBy = User.Identity.Name;
                LedgerLine.ModifiedBy = User.Identity.Name;
                LedgerLine.ObjectState = Model.ObjectState.Added;
                new LedgerLineService(_unitOfWork).Create(LedgerLine);

                Ledger Ledger = new Ledger();
                Ledger.LedgerAccountId = L.LedgerAccountId;
                Ledger.ContraLedgerAccountId = (int)L.ContraLedgerAccountId;
                Ledger.AmtCr = 0;
                Ledger.AmtDr = LedgerLine.Amount;
                Ledger.LedgerHeaderId = LedgerLine.LedgerHeaderId;
                Ledger.LedgerLineId = LedgerLine.LedgerLineId;
                Ledger.Narration = LedgerHeader.Narration + LedgerLine.Remark;
                Ledger.ObjectState = Model.ObjectState.Added;
                Ledger.LedgerId = 1;
                new LedgerService(_unitOfWork).Create (Ledger);

                Ledger CLedger = new Ledger();
                CLedger.LedgerAccountId = (int)L.ContraLedgerAccountId;
                CLedger.ContraLedgerAccountId = L.LedgerAccountId;
                CLedger.AmtCr = LedgerLine.Amount;
                CLedger.AmtDr = 0;
                CLedger.LedgerHeaderId = LedgerLine.LedgerHeaderId;
                CLedger.LedgerLineId = LedgerLine.LedgerLineId;
                CLedger.Narration = LedgerHeader.Narration + LedgerLine.Remark;
                CLedger.ObjectState = Model.ObjectState.Added;
                CLedger.LedgerId = 2;
                new LedgerService(_unitOfWork).Create(CLedger);

            }

            Ledger ledger = new LedgerService(_unitOfWork).Find(LedgerId);
            ledger.PassedAmount = PassedAmt;
            ledger.BankDate = BankDate;
            ledger.PassedBy = PassedBy;
            ledger.ReconciliedBy = LogVm.User;
            new LedgerService(_unitOfWork).Update(ledger);
            _unitOfWork.Save();

            return Json(new { Success = true });
        }

        public ActionResult Filters(BankReconciliationFilterArgs Fvm)
        {
            List<SelectListItem> temp = new List<SelectListItem>();
            temp.Add(new SelectListItem { Text = "Pending", Value = "Pending" });
            temp.Add(new SelectListItem { Text = "All", Value = "All" });

            ViewBag.TransactionType = new SelectList(temp, "Value", "Text", Fvm.TransactionType);


            return PartialView("_Filters", Fvm);
        }

        public JsonResult GetPassedByJson()
        {
            var UserRole = db.Roles.Where(m => m.Name == "Managing Director").FirstOrDefault();
            var temp = (from L in db.UserRole
                        join U in db.Users on L.UserId equals U.Id into UTable
                        from UTab in UTable.DefaultIfEmpty()
                        where UTab.UserName !=null && L.RoleId == UserRole.Id
                        group new { UTab } by new { UTab.UserName } into Result
                        select new 
                        {
                            User = Result.Key
                        }).ToList();
            return Json(temp);
        }

        public JsonResult GetBalanceAsPerBooksJson(int LedgerAccountId, DateTime? ToDate)
        {
            Decimal Balance = 0;
            var temp = (from L in db.Ledger
                        where L.LedgerAccountId == LedgerAccountId
                        && (ToDate == null ? 1 == 1 : L.LedgerHeader.DocDate <= ToDate)
                        group new { L } by new { L.LedgerAccountId } into Result
                        select new
                        {
                            Balance = Result.Sum(i => i.L.AmtDr - i.L.AmtCr)
                        }).FirstOrDefault();

            if (temp != null)
            {
                Balance = temp.Balance;
            }
            return Json(Balance);
        }

        public JsonResult GetBalanceAsPerBankJson(int LedgerAccountId, DateTime? ToDate)
        {
            Decimal Balance = 0;
            var temp = (from L in db.Ledger
                        where L.LedgerAccountId == LedgerAccountId && L.BankDate != null
                        && (ToDate == null ? 1 == 1 : L.LedgerHeader.DocDate <= ToDate)
                        group new { L } by new { L.LedgerAccountId } into Result
                        select new
                        {
                            Balance = Result.Sum(i => i.L.AmtDr - i.L.AmtCr)
                        }).FirstOrDefault();

            if (temp != null)
            {
                Balance = temp.Balance;
            }
            return Json(Balance);
        }




        protected override void Dispose(bool disposing)
        {
            if (!string.IsNullOrEmpty((string)TempData["CSEXC"]))
            {
                CookieGenerator.CreateNotificationCookie(NotificationTypeConstants.Danger, (string)TempData["CSEXC"]);
                TempData.Remove("CSEXC");
            }
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }


    //public class BankReconciliationFilterArgs
    //{
    //    public string TransactionType { get; set; }
    //    public DateTime? FromDate { get; set; }
    //    public DateTime? ToDate { get; set; }
    //    public int LedgerAccountId { get; set; }
    //}
}
