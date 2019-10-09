﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Model.Models;
using Data.Models;
using Service;
using Data.Infrastructure;
using Presentation.ViewModels;
using Presentation;
using Core.Common;
using Model.ViewModels;
using System.Configuration;
using AutoMapper;
using Jobs.Helpers;
using Model.ViewModel;
using Reports.Controllers;
using System.Xml.Linq;
using System.Data.SqlClient;
using Reports.Reports;
using System.Data;
using SaleInvoiceDocumentEvents;
using CustomEventArgs;


namespace Jobs.Controllers
{
    [Authorize]
    public class SaleInvoiceHeaderController : System.Web.Mvc.Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        private bool EventException = false;
        
        List<string> UserRoles = new List<string>();
        ActiivtyLogViewModel LogVm = new ActiivtyLogViewModel();

        bool TimePlanValidation = true;
        string ExceptionMsg = "";
        bool Continue = true;

        ISaleInvoiceHeaderService _SaleInvoiceHeaderService;
        IActivityLogService _ActivityLogService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;

        public SaleInvoiceHeaderController(ISaleInvoiceHeaderService SaleInvoiceHeaderService, IActivityLogService ActivityLogService, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _SaleInvoiceHeaderService = SaleInvoiceHeaderService;
            _ActivityLogService = ActivityLogService;
            _unitOfWork = unitOfWork;
            _exception = exec;
            if (!SaleInvoiceEvents.Initialized)
            {
                SaleInvoiceEvents Obj = new SaleInvoiceEvents();
            }

             UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];

            //Log Initialization
            LogVm.SessionId = 0;
            LogVm.ControllerName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("controller");
            LogVm.ActionName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("action");
            LogVm.User = System.Web.HttpContext.Current.Request.RequestContext.HttpContext.User.Identity.Name;
        }

        private void PrepareViewBag(int id)
        {
            ViewBag.Name = new DocumentTypeService(_unitOfWork).Find(id).DocumentTypeName;
            ViewBag.id = id;
            ViewBag.DealUnitList = new UnitService(_unitOfWork).GetUnitList().ToList();
            ViewBag.ShipMethodList = new ShipMethodService(_unitOfWork).GetShipMethodList().ToList();
            ViewBag.CurrencyList = new CurrencyService(_unitOfWork).GetCurrencyList().ToList();
            ViewBag.SalesTaxGroupList = new ChargeGroupPersonService(_unitOfWork).GetChargeGroupPersonList((int)(TaxTypeConstants.SalesTax)).ToList();
            ViewBag.DeliveryTermsList = new DeliveryTermsService(_unitOfWork).GetDeliveryTermsList().ToList();

             var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
             var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
             ViewBag.AdminSetting = UserRoles.Contains("Admin").ToString();
             var settings = new SaleInvoiceSettingService(_unitOfWork).GetSaleInvoiceSettingForDocument(id,DivisionId,SiteId);
            if(settings !=null)
            {
                ViewBag.ImportMenuId = settings.ImportMenuId;
                ViewBag.SqlProcDocumentPrint = settings.SqlProcDocumentPrint;
                ViewBag.ExportMenuId = settings.ExportMenuId;
            }

        }

        public ActionResult DocumentTypeIndex(int id)//DocumentCategoryId
        {
            var p = new DocumentTypeService(_unitOfWork).FindByDocumentCategory(id).ToList();

            if (p != null)
            {
                if (p.Count == 1)
                    return RedirectToAction("Index", new { id = p.FirstOrDefault().DocumentTypeId });
            }

            return View("DocumentTypeList", p);
        }

        public ActionResult Index(int id, string IndexType)//DocTypeId 
        {
            if (IndexType == "PTS")
            {
                return RedirectToAction("Index_PendingToSubmit", new { id });
            }
            else if (IndexType == "PTR")
            {
                return RedirectToAction("Index_PendingToReview", new { id });
            }
            IQueryable<SaleInvoiceHeaderIndexViewModel> p = _SaleInvoiceHeaderService.GetSaleInvoiceHeaderList(id, User.Identity.Name);
            PrepareViewBag(id);
            ViewBag.PendingToSubmit = PendingToSubmitCount(id);
            ViewBag.PendingToReview = PendingToReviewCount(id);
            ViewBag.IndexStatus = "All";
            return View(p);
        }

        public ActionResult Index_PendingToSubmit(int id)
        {
            var PendingToSubmit = _SaleInvoiceHeaderService.GetSaleInvoiceHeaderListPendingToSubmit(id, User.Identity.Name);

            ViewBag.Name = new DocumentTypeService(_unitOfWork).Find(id).DocumentTypeName;
            ViewBag.id = id;
            ViewBag.PendingToSubmit = PendingToSubmitCount(id);
            ViewBag.PendingToReview = PendingToReviewCount(id);
            ViewBag.IndexStatus = "PTS";
            return View("Index", PendingToSubmit);
        }

        public ActionResult Index_PendingToReview(int id)
        {
            var PendingtoReview = _SaleInvoiceHeaderService.GetSaleInvoiceHeaderListPendingToReview(id, User.Identity.Name);
            ViewBag.Name = new DocumentTypeService(_unitOfWork).Find(id).DocumentTypeName;
            ViewBag.id = id;
            ViewBag.PendingToSubmit = PendingToSubmitCount(id);
            ViewBag.PendingToReview = PendingToReviewCount(id);
            ViewBag.IndexStatus = "PTR";
            return View("Index", PendingtoReview);
        }



        public ActionResult Create(int id)//DocTypeId
        {
            DirectSaleInvoiceHeaderViewModel vm = new DirectSaleInvoiceHeaderViewModel();

            vm.DocDate = DateTime.Now.Date;
            vm.DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            vm.SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            vm.DocTypeId = id;
            vm.CreatedDate = DateTime.Now;

            //Getting Settings
            var settings = new SaleInvoiceSettingService(_unitOfWork).GetSaleInvoiceSettingForDocument(id, vm.DivisionId, vm.SiteId);

            if (settings == null && UserRoles.Contains("SysAdmin"))
            {
                return RedirectToAction("Create", "SaleInvoiceSetting", new { id = id }).Warning("Please create Sale Invoice settings");
            }
            else if (settings == null && !UserRoles.Contains("SysAdmin"))
            {
                return View("~/Views/Shared/InValidSettings.cshtml");
            }

            if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, id, null, this.ControllerContext.RouteData.Values["controller"].ToString(), "Create") == false)
            {
                return View("~/Views/Shared/PermissionDenied.cshtml").Warning("You don't have permission to do this task.");
            }


            if (settings != null)
            {
                vm.ShipMethodId = settings.ShipMethodId;
                vm.DeliveryTermsId = settings.DeliveryTermsId;
                vm.CurrencyId = settings.CurrencyId;
                vm.SalesTaxGroupPersonId = settings.SalesTaxGroupPersonId;
                vm.GodownId = settings.GodownId;
                vm.ProcessId = settings.ProcessId;
            }

            vm.SaleInvoiceSettings = Mapper.Map<SaleInvoiceSetting, SaleInvoiceSettingsViewModel>(settings);
            ViewBag.Mode = "Add";
            PrepareViewBag(id);

            vm.DocNo = new DocumentTypeService(_unitOfWork).FGetNewDocNo("DocNo", ConfigurationManager.AppSettings["DataBaseSchema"] + ".SaleInvoiceHeaders", vm.DocTypeId, vm.DocDate, vm.DivisionId, vm.SiteId);
            return View("Create", vm);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePost(DirectSaleInvoiceHeaderViewModel vm)
        {

            #region DocTypeTimeLineValidation

            try
            {

                if (vm.SaleInvoiceHeaderId <= 0)
                    TimePlanValidation = DocumentValidation.ValidateDocument(Mapper.Map<DocumentUniqueId>(vm), DocumentTimePlanTypeConstants.Create, User.Identity.Name, out ExceptionMsg, out Continue);
                else
                    TimePlanValidation = DocumentValidation.ValidateDocument(Mapper.Map<DocumentUniqueId>(vm), DocumentTimePlanTypeConstants.Modify, User.Identity.Name, out ExceptionMsg, out Continue);

            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXC"] += message;
                TimePlanValidation = false;
            }

            if (!TimePlanValidation)
                TempData["CSEXC"] += ExceptionMsg;

            #endregion

            if (ModelState.IsValid && (TimePlanValidation || Continue))
            {
                #region CreateRecord
                if (vm.SaleInvoiceHeaderId == 0)
                {
                    SaleInvoiceHeader saleinvoiceheaderdetail = Mapper.Map<DirectSaleInvoiceHeaderViewModel, SaleInvoiceHeader>(vm);


                    saleinvoiceheaderdetail.CreatedDate = DateTime.Now;
                    saleinvoiceheaderdetail.ModifiedDate = DateTime.Now;
                    saleinvoiceheaderdetail.CreatedBy = User.Identity.Name;
                    saleinvoiceheaderdetail.ModifiedBy = User.Identity.Name;
                    saleinvoiceheaderdetail.Status = (int)StatusConstants.Drafted;
                    _SaleInvoiceHeaderService.Create(saleinvoiceheaderdetail);

                    try
                    {
                        _unitOfWork.Save();
                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                        PrepareViewBag(vm.DocTypeId);
                        ViewBag.Mode = "Add";
                        return View("Create", vm);
                    }

                     LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = saleinvoiceheaderdetail.DocTypeId,
                        DocId = saleinvoiceheaderdetail.SaleInvoiceHeaderId,
                        ActivityType = (int)ActivityTypeContants.Added,
                        DocNo = saleinvoiceheaderdetail.DocNo,
                        DocDate = saleinvoiceheaderdetail.DocDate,
                        DocStatus = saleinvoiceheaderdetail.Status,
                    }));


                    return RedirectToAction("Modify", new { id = saleinvoiceheaderdetail.SaleInvoiceHeaderId }).Success("Data saved Successfully");
                } 
                #endregion

                #region EditRecord
                else
                {
                    List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();

                    SaleInvoiceHeader saleinvoiceheaderdetail = _SaleInvoiceHeaderService.FindDirectSaleInvoice(vm.SaleInvoiceHeaderId);

                    SaleInvoiceHeader ExRec = Mapper.Map<SaleInvoiceHeader>(saleinvoiceheaderdetail);

                    int status = saleinvoiceheaderdetail.Status;

                    if (saleinvoiceheaderdetail.Status != (int)StatusConstants.Drafted)
                    {
                        saleinvoiceheaderdetail.Status = (int)StatusConstants.Modified;
                    }

                    bool IsBillToPartyChanges = false;
                    if (saleinvoiceheaderdetail.BillToBuyerId != vm.BillToBuyerId)
                        IsBillToPartyChanges = true;

                    saleinvoiceheaderdetail.BillToBuyerId = vm.BillToBuyerId;
                    saleinvoiceheaderdetail.SaleToBuyerId = vm.SaleToBuyerId;
                    saleinvoiceheaderdetail.CurrencyId = vm.CurrencyId;
                    saleinvoiceheaderdetail.DocDate = vm.DocDate;
                    saleinvoiceheaderdetail.DocNo = vm.DocNo;
                    saleinvoiceheaderdetail.FinancierId = vm.FinancierId;
                    saleinvoiceheaderdetail.SalesExecutiveId = vm.SalesExecutiveId;
                    saleinvoiceheaderdetail.SalesTaxGroupPersonId = vm.SalesTaxGroupPersonId;
                    saleinvoiceheaderdetail.Remark = vm.Remark;
                    saleinvoiceheaderdetail.ModifiedDate = DateTime.Now;
                    saleinvoiceheaderdetail.ModifiedBy = User.Identity.Name;
                    _SaleInvoiceHeaderService.Update(saleinvoiceheaderdetail);

                    if (IsBillToPartyChanges == true)
                    {
                        var LineCharges = (from L in db.SaleInvoiceLineCharge where L.HeaderTableId == vm.SaleInvoiceHeaderId select L).ToList();
                        foreach (var item in LineCharges)
                        {
                            item.PersonID = saleinvoiceheaderdetail.BillToBuyerId;
                            new SaleInvoiceLineChargeService(_unitOfWork).Update(item);
                        }

                        var HeaderCharges = (from L in db.SaleInvoiceHeaderCharge where L.HeaderTableId == vm.SaleInvoiceHeaderId select L).ToList();
                        foreach (var item in HeaderCharges)
                        {
                            item.PersonID = saleinvoiceheaderdetail.BillToBuyerId;
                            new SaleInvoiceHeaderChargeService(_unitOfWork).Update(item);
                        }
                    }



                    LogList.Add(new LogTypeViewModel
                    {
                        ExObj = ExRec,
                        Obj = saleinvoiceheaderdetail,
                    });

                    XElement Modifications = new ModificationsCheckService().CheckChanges(LogList);

                    try
                    {
                        _unitOfWork.Save();
                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                        PrepareViewBag(vm.DocTypeId);
                        ViewBag.Mode = "Edit";
                        return View("Create", vm);
                    }

                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = saleinvoiceheaderdetail.DocTypeId,
                        DocId = saleinvoiceheaderdetail.SaleInvoiceHeaderId,
                        ActivityType = (int)ActivityTypeContants.Modified,
                        DocNo = saleinvoiceheaderdetail.DocNo,
                        xEModifications = Modifications,
                        DocDate = saleinvoiceheaderdetail.DocDate,
                        DocStatus = saleinvoiceheaderdetail.Status,
                    }));

                    return RedirectToAction("Index", new { id = vm.DocTypeId }).Success("Data saved successfully");
                } 
                #endregion

            }
            PrepareViewBag(vm.DocTypeId);
            ViewBag.Mode = "Add";
            return View("Create", vm);
        }


        [HttpGet]
        public ActionResult Modify(int id, string IndexType)
        {
            SaleInvoiceHeader header = _SaleInvoiceHeaderService.FindDirectSaleInvoice(id);
            if (header.Status == (int)StatusConstants.Drafted)
                return Edit(id, IndexType);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult ModifyAfter_Submit(int id, string IndexType)
        {
            SaleInvoiceHeader header = _SaleInvoiceHeaderService.FindDirectSaleInvoice(id);
            if (header.Status == (int)StatusConstants.Submitted || header.Status == (int)StatusConstants.Modified)
                return Edit(id, IndexType);
            else
                return HttpNotFound();
        }



        private ActionResult Edit(int id, string IndexType)
        {

            ViewBag.IndexStatus = IndexType;
            SaleInvoiceHeader s = _SaleInvoiceHeaderService.FindDirectSaleInvoice(id);

            if (s.Status != (int)StatusConstants.Drafted)
                if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, s.DocTypeId, null, this.ControllerContext.RouteData.Values["controller"].ToString(), "Edit") == false)
                    return RedirectToAction("DetailInformation", new { id = id, IndexType = IndexType }).Warning("You don't have permission to do this task.");

            #region DocTypeTimeLineValidation
            try
            {

                TimePlanValidation = DocumentValidation.ValidateDocument(Mapper.Map<DocumentUniqueId>(s), DocumentTimePlanTypeConstants.Modify, User.Identity.Name, out ExceptionMsg, out Continue);

            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXC"] += message;
                TimePlanValidation = false;
            }

            if (!TimePlanValidation)
                TempData["CSEXC"] += ExceptionMsg;
            #endregion

            if ((!TimePlanValidation && !Continue))
            {
                return RedirectToAction("DetailInformation", new { id = id, IndexType = IndexType });
            }

            DirectSaleInvoiceHeaderViewModel vm = new DirectSaleInvoiceHeaderViewModel();

            vm = Mapper.Map<SaleInvoiceHeader, DirectSaleInvoiceHeaderViewModel>(s);
            vm.SaleToBuyerId = s.SaleToBuyerId;

            //Getting Settings
            var settings = new SaleInvoiceSettingService(_unitOfWork).GetSaleInvoiceSettingForDocument(s.DocTypeId, vm.DivisionId, vm.SiteId);

            if (settings == null && UserRoles.Contains("SysAdmin"))
            {
                return RedirectToAction("Create", "SaleInvoiceSetting", new { id = s.DocTypeId }).Warning("Please create Sale Invoice settings");
            }
            else if (settings == null && !UserRoles.Contains("SysAdmin"))
            {
                return View("~/Views/Shared/InValidSettings.cshtml");
            }

            if (settings != null)
            {
                vm.ProcessId = settings.ProcessId;
            }


            vm.SaleInvoiceSettings = Mapper.Map<SaleInvoiceSetting, SaleInvoiceSettingsViewModel>(settings);
            PrepareViewBag(s.DocTypeId);
            ViewBag.Mode = "Edit";

            var SaleInvoiceReturn = db.SaleInvoiceReturnLine.Where(i => i.SaleInvoiceLine.SaleInvoiceHeaderId == id).FirstOrDefault();
            if (SaleInvoiceReturn != null)
            {
                vm.ReturnNature = (from H in db.SaleInvoiceReturnHeader where H.SaleInvoiceReturnHeaderId == SaleInvoiceReturn.SaleInvoiceReturnHeaderId select new { DocTypeNature = H.DocType.Nature }).FirstOrDefault().DocTypeNature;
                if (vm.ReturnNature == TransactionNatureConstants.Credit)
                    vm.AdditionalInfo = "Credit Note is generated for this invoice.";
                else if (vm.ReturnNature == TransactionNatureConstants.Debit)
                    vm.AdditionalInfo = "Debit Note is generated for this invoice.";
                else
                    vm.AdditionalInfo = "Invoice is cancelled.";
            }



            if (!(System.Web.HttpContext.Current.Request.UrlReferrer.PathAndQuery).Contains("Create"))
                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = s.DocTypeId,
                    DocId = s.SaleInvoiceHeaderId,
                    ActivityType = (int)ActivityTypeContants.Detail,
                    DocNo = s.DocNo,
                    DocDate = s.DocDate,
                    DocStatus = s.Status,
                }));

            return View("Create", vm);
        }

        [HttpGet]
        public ActionResult DetailInformation(int id, string IndexType)
        {
            return RedirectToAction("Detail", new { id = id, transactionType = "detail", IndexType = IndexType });
        }

        [Authorize]
        public ActionResult Detail(int id, string transactionType, string IndexType)
        {
            ViewBag.transactionType = transactionType;
            ViewBag.IndexStatus = IndexType;

            SaleInvoiceHeader s = _SaleInvoiceHeaderService.FindDirectSaleInvoice(id);

            DirectSaleInvoiceHeaderViewModel vm = new DirectSaleInvoiceHeaderViewModel();
            vm = Mapper.Map<SaleInvoiceHeader, DirectSaleInvoiceHeaderViewModel>(s);
            vm.SaleToBuyerId = s.SaleToBuyerId;

            //Getting Settings
            var settings = new SaleInvoiceSettingService(_unitOfWork).GetSaleInvoiceSettingForDocument(s.DocTypeId, s.DivisionId, s.SiteId);

            if (settings == null && UserRoles.Contains("SysAdmin"))
            {
                return RedirectToAction("Create", "SaleInvoiceSetting", new { id = id }).Warning("Please create Sale Invoice settings");
            }
            else if (settings == null && !UserRoles.Contains("SysAdmin"))
            {
                return View("~/Views/Shared/InValidSettings.cshtml");
            }
            vm.SaleInvoiceSettings = Mapper.Map<SaleInvoiceSetting, SaleInvoiceSettingsViewModel>(settings);            
            PrepareViewBag(s.DocTypeId);

            if (String.IsNullOrEmpty(transactionType) || transactionType == "detail")
                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = s.DocTypeId,
                    DocId = s.SaleInvoiceHeaderId,
                    ActivityType = (int)ActivityTypeContants.Detail,
                    DocNo = s.DocNo,
                    DocDate = s.DocDate,
                    DocStatus = s.Status,
                }));

            return View("Create", vm);
        }


        [HttpGet]
        public ActionResult Delete(int id)
        {
            SaleInvoiceHeader header = _SaleInvoiceHeaderService.FindDirectSaleInvoice(id);
            if (header.Status == (int)StatusConstants.Drafted)
                return Remove(id);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult DeleteAfter_Submit(int id)
        {
            SaleInvoiceHeader header = _SaleInvoiceHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Submitted || header.Status == (int)StatusConstants.Modified || header.Status == (int)StatusConstants.ModificationSubmitted)
                return Remove(id);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult DeleteAfter_Approve(int id)
        {
            SaleInvoiceHeader header = _SaleInvoiceHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Approved)
                return Remove(id);
            else
                return HttpNotFound();
        }


        private ActionResult Remove(int id)
        {
            ReasonViewModel rvm = new ReasonViewModel()
            {
                id = id,
            };

            SaleInvoiceHeader SaleinvoiceHeader = db.SaleInvoiceHeader.Find(id);

            if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, SaleinvoiceHeader.DocTypeId, null, this.ControllerContext.RouteData.Values["controller"].ToString(), "Remove") == false)
            {
                return PartialView("~/Views/Shared/PermissionDenied_Modal.cshtml").Warning("You don't have permission to do this task.");
            }

            #region DocTypeTimeLineValidation
            try
            {
                TimePlanValidation = DocumentValidation.ValidateDocument(Mapper.Map<DocumentUniqueId>(SaleinvoiceHeader), DocumentTimePlanTypeConstants.Delete, User.Identity.Name, out ExceptionMsg, out Continue);
                TempData["CSEXC"] += ExceptionMsg;
            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXC"] += message;
                TimePlanValidation = false;
            }

            if (!TimePlanValidation && !Continue)
            {
                return PartialView("AjaxError");
            }
            #endregion

            return PartialView("_Reason", rvm);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(ReasonViewModel vm)
        {
            if (ModelState.IsValid)
            {

                db.Configuration.AutoDetectChangesEnabled = false;

                SaleInvoiceHeader Si = (from H in db.SaleInvoiceHeader where H.SaleInvoiceHeaderId == vm.id select H).FirstOrDefault();

                new LedgerService(_unitOfWork).DeleteLedgerForDocHeader(Si.SaleInvoiceHeaderId, Si.DocTypeId, Si.SiteId, Si.DivisionId);



                var SaleInvoiceLine = (from L in db.SaleInvoiceLine where L.SaleInvoiceHeaderId == vm.id select L).ToList();


                int cnt = 0;
                foreach (var item in SaleInvoiceLine)
                {

                    cnt = cnt + 1;

                    var linecharges = (from L in db.SaleInvoiceLineCharge where L.LineTableId == item.SaleInvoiceLineId select L).ToList();

                    foreach (var citem in linecharges)
                    {
                        citem.ObjectState = Model.ObjectState.Deleted;
                        db.SaleInvoiceLineCharge.Attach(citem);
                        db.SaleInvoiceLineCharge.Remove(citem);
                    }

                    try
                    {
                        SaleInvoiceLineDetail LineDetail = (from L in db.SaleInvoiceLineDetail where L.SaleInvoiceLineId == item.SaleInvoiceLineId select L).FirstOrDefault();
                        LineDetail.ObjectState = Model.ObjectState.Deleted;
                        db.SaleInvoiceLineDetail.Attach(LineDetail);
                        db.SaleInvoiceLineDetail.Remove(LineDetail);


                        item.ObjectState = Model.ObjectState.Deleted;
                        db.SaleInvoiceLine.Attach(item);
                        db.SaleInvoiceLine.Remove(item);
                    }
                    catch (Exception e)
                    {
                        string str = e.Message;
                    }

                }


                var headercharges = (from L in db.SaleInvoiceHeaderCharge where L.HeaderTableId == Si.SaleInvoiceHeaderId select L).ToList();

                foreach (var citem in headercharges)
                {
                    citem.ObjectState = Model.ObjectState.Deleted;
                    db.SaleInvoiceHeaderCharge.Attach(citem);
                    db.SaleInvoiceHeaderCharge.Remove(citem);
                }


                Si.ObjectState = Model.ObjectState.Deleted;
                db.SaleInvoiceHeader.Attach(Si);
                db.SaleInvoiceHeader.Remove(Si);


                //Commit the DB
                try
                {
                    db.SaveChanges();
                    db.Configuration.AutoDetectChangesEnabled = true;
                }

                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    db.Configuration.AutoDetectChangesEnabled = true;
                    TempData["CSEXC"] += message;
                    PrepareViewBag(Si.DocTypeId);
                    return PartialView("_Reason", vm);
                }

                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = Si.DocTypeId,
                    DocId = Si.SaleInvoiceHeaderId,
                    ActivityType = (int)ActivityTypeContants.Deleted,
                    UserRemark = vm.Reason,
                    DocNo = Si.DocNo,                    
                    DocDate = Si.DocDate,
                    DocStatus = Si.Status,
                }));

                return Json(new { success = true });
            }
            return PartialView("_Reason", vm);
        }





        public ActionResult Submit(int id, string IndexType, string TransactionType)
        {
            SaleInvoiceHeader s = db.SaleInvoiceHeader.Find(id);

            if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, s.DocTypeId, null, this.ControllerContext.RouteData.Values["controller"].ToString(), "Submit") == false)
            {
                return View("~/Views/Shared/PermissionDenied.cshtml").Warning("You don't have permission to do this task.");
            }

            #region DocTypeTimeLineValidation

            try
            {
                TimePlanValidation = Submitvalidation(id, out ExceptionMsg);
                TempData["CSEXC"] += ExceptionMsg;
            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXC"] += message;
                TimePlanValidation = false;
            }
            try
            {
                TimePlanValidation = DocumentValidation.ValidateDocument(Mapper.Map<DocumentUniqueId>(s), DocumentTimePlanTypeConstants.Submit, User.Identity.Name, out ExceptionMsg, out Continue);
                TempData["CSEXC"] += ExceptionMsg;
            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXC"] += message;
                TimePlanValidation = false;
            }

            if (!TimePlanValidation && !Continue)
            {
                return RedirectToAction("Index", new { id = s.DocTypeId, IndexType = IndexType });
            }
            #endregion

            return RedirectToAction("Detail", new { id = id, IndexType = IndexType, transactionType = string.IsNullOrEmpty(TransactionType) ? "submit" : TransactionType });
        }


        [HttpPost, ActionName("Detail")]
        [MultipleButton(Name = "Command", Argument = "Submit")]
        public ActionResult Submitted(int Id, string IndexType, string UserRemark, string IsContinue, string GenGatePass)
        {
            //int SaleAc = 6650;
            int ActivityType;


            bool BeforeSave = true;
            try
            {
                BeforeSave = SaleInvoiceDocEvents.beforeHeaderSubmitEvent(this, new SaleEventArgs(Id), ref db);
            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXC"] += message;
                EventException = true;
            }

            if (!BeforeSave)
                TempData["CSEXC"] += "Falied validation before submit.";


            SaleInvoiceHeader pd = new SaleInvoiceHeaderService(_unitOfWork).FindDirectSaleInvoice(Id);
  
            if (ModelState.IsValid && BeforeSave && !EventException)
            {
                if (User.Identity.Name == pd.ModifiedBy || UserRoles.Contains("Admin"))
                {

                    pd.Status = (int)StatusConstants.Submitted;
                    ActivityType = (int)ActivityTypeContants.Submitted;

                    pd.ReviewBy = null;





                    SaleInvoiceSetting Settings = new SaleInvoiceSettingService(_unitOfWork).GetSaleInvoiceSettingForDocument(pd.DocTypeId, pd.DivisionId, pd.SiteId);

                    _SaleInvoiceHeaderService.Update(pd);


                    
                    #region "Ledger Posting"
                    try
                    {
                        LedgerPost(pd);
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        ModelState.AddModelError("", message);
                        return RedirectToAction("Detail", new { id = Id, transactionType = "submit" });
                    }
                    #endregion

                     try
                    {
                        SaleInvoiceDocEvents.onHeaderSubmitEvent(this, new SaleEventArgs(Id), ref db);
                    }
                     catch (Exception ex)
                     {
                         string message = _exception.HandleException(ex);
                         TempData["CSEXC"] += message;
                         EventException = true;
                     }


                    try
                    {
                        _unitOfWork.Save();
                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                        return RedirectToAction("Index", new { id = pd.DocTypeId });
                    }


                    try
                    {
                        SaleInvoiceDocEvents.afterHeaderSubmitEvent(this, new SaleEventArgs(Id), ref db);
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                    }


                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = pd.DocTypeId,
                        DocId = pd.SaleInvoiceHeaderId,
                        ActivityType = ActivityType,
                        UserRemark = UserRemark,
                        DocNo = pd.DocNo,
                        DocDate = pd.DocDate,
                        DocStatus = pd.Status,
                    }));
               
                    return RedirectToAction("Index", new { id = pd.DocTypeId, IndexType = IndexType }).Success("Record submitted successfully.");
                }
                else
                    return RedirectToAction("Index", new { id = pd.DocTypeId, IndexType = IndexType }).Warning("Record can be submitted by user " + pd.ModifiedBy + " only.");
            }

            return View();
        }



        public ActionResult Review(int id, string IndexType, string TransactionType)
        {
            return RedirectToAction("Detail", new { id = id, IndexType = IndexType, transactionType = string.IsNullOrEmpty(TransactionType) ? "review" : TransactionType });
        }


        [HttpPost, ActionName("Detail")]
        [MultipleButton(Name = "Command", Argument = "Review")]
        public ActionResult Reviewed(int Id, string IndexType, string UserRemark, string IsContinue)
        {
            SaleInvoiceHeader pd = new SaleInvoiceHeaderService(_unitOfWork).FindDirectSaleInvoice(Id);

            if (ModelState.IsValid)
            {
                pd.ReviewCount = (pd.ReviewCount ?? 0) + 1;
                pd.ReviewBy += User.Identity.Name + ", ";

                _SaleInvoiceHeaderService.Update(pd);
                _unitOfWork.Save();

                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = pd.DocTypeId,
                    DocId = pd.SaleInvoiceHeaderId,
                    ActivityType = (int)ActivityTypeContants.Reviewed,
                    UserRemark = UserRemark,
                    DocNo = pd.DocNo,
                    DocDate = pd.DocDate,
                    DocStatus = pd.Status,
                }));

                return RedirectToAction("Index", new { id = pd.DocTypeId, IndexType = IndexType }).Success("Record reviewed successfully.");
            }

            return RedirectToAction("Index", new { id = pd.DocTypeId, IndexType = IndexType }).Warning("Error in reviewing.");
        }

        public void LedgerPost(SaleInvoiceHeader pd)
        {
            LedgerHeaderViewModel LedgerHeaderViewModel = new LedgerHeaderViewModel();

            SaleInvoiceSetting Settings = new SaleInvoiceSettingService(_unitOfWork).GetSaleInvoiceSettingForDocument(pd.DocTypeId, pd.DivisionId, pd.SiteId);


            LedgerHeaderViewModel.LedgerHeaderId = pd.LedgerHeaderId ?? 0;
            LedgerHeaderViewModel.DocTypeId = pd.DocTypeId;
            LedgerHeaderViewModel.ProcessId  = Settings.ProcessId;
            LedgerHeaderViewModel.DocDate = pd.DocDate;
            LedgerHeaderViewModel.DocNo = pd.DocNo;
            LedgerHeaderViewModel.DivisionId = pd.DivisionId;
            LedgerHeaderViewModel.SiteId = pd.SiteId;
            LedgerHeaderViewModel.Narration = "";
            LedgerHeaderViewModel.Remark = pd.Remark;
            LedgerHeaderViewModel.ExchangeRate = pd.ExchangeRate;
            LedgerHeaderViewModel.CreatedBy = pd.CreatedBy;
            LedgerHeaderViewModel.CreatedDate = DateTime.Now.Date;
            LedgerHeaderViewModel.ModifiedBy = pd.ModifiedBy;
            LedgerHeaderViewModel.ModifiedDate = DateTime.Now.Date;

            IEnumerable<SaleInvoiceHeaderCharge> SaleInvoiceHeaderCharges = from H in db.SaleInvoiceHeaderCharge where H.HeaderTableId == pd.SaleInvoiceHeaderId select H;
            IEnumerable<SaleInvoiceLineCharge> SaleInvoiceLineCharges = from L in db.SaleInvoiceLineCharge where L.HeaderTableId == pd.SaleInvoiceHeaderId select L;

            new CalculationService(_unitOfWork).LedgerPosting(ref LedgerHeaderViewModel, SaleInvoiceHeaderCharges, SaleInvoiceLineCharges);

            if (pd.LedgerHeaderId == null)
            {
                pd.LedgerHeaderId = LedgerHeaderViewModel.LedgerHeaderId;
                _SaleInvoiceHeaderService.Update(pd);
            }
        }


        public int PendingToSubmitCount(int id)
        {
            return (_SaleInvoiceHeaderService.GetSaleInvoiceHeaderListPendingToSubmit(id, User.Identity.Name)).Count();
        }

        public int PendingToReviewCount(int id)
        {
            return (_SaleInvoiceHeaderService.GetSaleInvoiceHeaderListPendingToReview(id, User.Identity.Name)).Count();
        }



        public JsonResult GetPersonLedgerBalance(int PersonId)
        {
            Decimal Balance = 0;
            SqlParameter SqlParameterPersonId = new SqlParameter("@PersonId", PersonId);

            PersonLedgerBalance PersonLedgerBalance = db.Database.SqlQuery<PersonLedgerBalance>("" + System.Configuration.ConfigurationManager.AppSettings["DataBaseSchema"] + ".GetPersonLedgerBalance @PersonId", SqlParameterPersonId).FirstOrDefault();
            if (PersonLedgerBalance != null)
            {
                Balance = PersonLedgerBalance.Balance;
            }

            return Json(Balance);
        }

        public JsonResult GetPersonDetail(int PersonId)
        {
            var PersonDetail = (from B in db.BusinessEntity
                                where B.PersonID == PersonId
                                select new
                                {
                                    CreditDays = B.CreaditDays ?? 0,
                                    CreditLimit = B.CreaditLimit ?? 0
                                }).FirstOrDefault();

            return Json(PersonDetail);
        }

        public ActionResult NextPage(int DocId, int DocTypeId)//CurrentHeaderId
        {
            var nextId = new NextPrevIdService(_unitOfWork).GetNextPrevId(DocId, DocTypeId, User.Identity.Name, "", "Web.SaleInvoiceHeaders", "SaleInvoiceHeaderId", PrevNextConstants.Next);
            return Edit(nextId, "");
        }
        [HttpGet]
        public ActionResult PrevPage(int DocId, int DocTypeId)//CurrentHeaderId
        {
            var PrevId = new NextPrevIdService(_unitOfWork).GetNextPrevId(DocId, DocTypeId, User.Identity.Name, "", "Web.SaleInvoiceHeaders", "SaleInvoiceHeaderId", PrevNextConstants.Prev);
            return Edit(PrevId, "");
        }

        [HttpGet]
        public ActionResult History()
        {
            //To Be Implemented
            return View("~/Views/Shared/UnderImplementation.cshtml");
        }

        [HttpGet]
        public ActionResult Email()
        {
            //To Be Implemented
            return View("~/Views/Shared/UnderImplementation.cshtml");
        }


        public ActionResult GeneratePrints(string Ids, int DocTypeId)
        {

            if (!string.IsNullOrEmpty(Ids))
            {
                int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
                int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

                var Settings = new SaleInvoiceSettingService(_unitOfWork).GetSaleInvoiceSettingForDocument(DocTypeId, DivisionId, SiteId);

                if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, DocTypeId, Settings.ProcessId, this.ControllerContext.RouteData.Values["controller"].ToString(), "GeneratePrints") == false)
                {
                    return View("~/Views/Shared/PermissionDenied.cshtml").Warning("You don't have permission to do this task.");
                }

                try
                {

                    List<byte[]> PdfStream = new List<byte[]>();
                    foreach (var item in Ids.Split(',').Select(Int32.Parse))
                    {

                        DirectReportPrint drp = new DirectReportPrint();

                        var pd = db.SaleInvoiceHeader.Find(item);

                        LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                        {
                            DocTypeId = pd.DocTypeId,
                            DocId = pd.SaleInvoiceHeaderId,
                            ActivityType = (int)ActivityTypeContants.Print,
                            DocNo = pd.DocNo,
                            DocDate = pd.DocDate,
                            DocStatus = pd.Status,
                        }));

                        byte[] Pdf;

                        if (pd.Status == (int)StatusConstants.Drafted || pd.Status == (int)StatusConstants.Modified || pd.Status == (int)StatusConstants.Import)
                        {
                            //LogAct(item.ToString());
                            Pdf = drp.DirectDocumentPrint(Settings.SqlProcDocumentPrint, User.Identity.Name, item);

                            PdfStream.Add(Pdf);
                        }
                        else if (pd.Status == (int)StatusConstants.Submitted || pd.Status == (int)StatusConstants.ModificationSubmitted)
                        {
                            Pdf = drp.DirectDocumentPrint(Settings.SqlProcDocumentPrint_AfterSubmit, User.Identity.Name, item);

                            PdfStream.Add(Pdf);
                        }
                        else if (pd.Status == (int)StatusConstants.Approved)
                        {
                            Pdf = drp.DirectDocumentPrint(Settings.SqlProcDocumentPrint_AfterApprove, User.Identity.Name, item);
                            PdfStream.Add(Pdf);
                        }

                    }

                    PdfMerger pm = new PdfMerger();

                    byte[] Merge = pm.MergeFiles(PdfStream);

                    if (Merge != null)
                        return File(Merge, "application/pdf");

                }

                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    return Json(new { success = "Error", data = message }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = "Success" }, JsonRequestBehavior.AllowGet);

            }
            return Json(new { success = "Error", data = "No Records Selected." }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        private ActionResult PrintOut(int id, string SqlProcForPrint)
        {
            String query = SqlProcForPrint;
            return Redirect((string)System.Configuration.ConfigurationManager.AppSettings["JobsDomain"] + "/Report_DocumentPrint/DocumentPrint/?DocumentId=" + id + "&queryString=" + query);
        }

        [HttpGet]
        public ActionResult Print(int id)
        {
            SaleInvoiceHeader header = _SaleInvoiceHeaderService.FindDirectSaleInvoice(id);
            if (header.Status == (int)StatusConstants.Drafted || header.Status == (int)StatusConstants.Import)
            {
                var SEttings = new SaleInvoiceSettingService(_unitOfWork).GetSaleInvoiceSettingForDocument(header.DocTypeId, header.DivisionId, header.SiteId);
                if (string.IsNullOrEmpty(SEttings.SqlProcDocumentPrint))
                    throw new Exception("Document Print Not Configured");
                else
                    return PrintOut(id, SEttings.SqlProcDocumentPrint);
            }
            else
                return HttpNotFound();

        }

        [HttpGet]
        public ActionResult PrintAfter_Submit(int id)
        {
            SaleInvoiceHeader header = _SaleInvoiceHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Submitted || header.Status == (int)StatusConstants.Modified)
            {
                var SEttings = new SaleInvoiceSettingService(_unitOfWork).GetSaleInvoiceSettingForDocument(header.DocTypeId, header.DivisionId, header.SiteId);
                if (string.IsNullOrEmpty(SEttings.SqlProcDocumentPrint_AfterSubmit))
                    throw new Exception("Document Print Not Configured");
                else
                    return PrintOut(id, SEttings.SqlProcDocumentPrint_AfterSubmit);
            }
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult PrintAfter_Approve(int id)
        {
            SaleInvoiceHeader header = _SaleInvoiceHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Approved)
            {
                var SEttings = new SaleInvoiceSettingService(_unitOfWork).GetSaleInvoiceSettingForDocument(header.DocTypeId, header.DivisionId, header.SiteId);
                if (string.IsNullOrEmpty(SEttings.SqlProcDocumentPrint_AfterApprove))
                    throw new Exception("Document Print Not Configured");
                else
                    return PrintOut(id, SEttings.SqlProcDocumentPrint_AfterApprove);
            }
            else
                return HttpNotFound();
        }


        [HttpGet]
        public ActionResult Report(int id)
        {
            DocumentType Dt = new DocumentType();
            Dt = new DocumentTypeService(_unitOfWork).Find(id);

            SaleInvoiceSetting SEttings = new SaleInvoiceSettingService(_unitOfWork).GetSaleInvoiceSettingForDocument(Dt.DocumentTypeId, (int)System.Web.HttpContext.Current.Session["DivisionId"], (int)System.Web.HttpContext.Current.Session["SiteId"]);

            Dictionary<int, string> DefaultValue = new Dictionary<int, string>();

            //if (!Dt.ReportMenuId.HasValue)
            //    throw new Exception("Report Menu not configured in document types");

            if (!Dt.ReportMenuId.HasValue)
                return Redirect((string)System.Configuration.ConfigurationManager.AppSettings["JobsDomain"] + "/GridReport/GridReportLayout/?MenuName=Sale Invoice Report&DocTypeId=" + id.ToString());


            Model.Models.Menu menu = new MenuService(_unitOfWork).Find(Dt.ReportMenuId ?? 0);

            if (menu != null)
            {
                ReportHeader header = new ReportHeaderService(_unitOfWork).GetReportHeaderByName(menu.MenuName);

                ReportLine Line = new ReportLineService(_unitOfWork).GetReportLineByName("DocumentType", header.ReportHeaderId);
                if (Line != null)
                    DefaultValue.Add(Line.ReportLineId, id.ToString());
                ReportLine Site = new ReportLineService(_unitOfWork).GetReportLineByName("Site", header.ReportHeaderId);
                if (Site != null)
                    DefaultValue.Add(Site.ReportLineId, ((int)System.Web.HttpContext.Current.Session["SiteId"]).ToString());
                ReportLine Division = new ReportLineService(_unitOfWork).GetReportLineByName("Division", header.ReportHeaderId);
                if (Division != null)
                    DefaultValue.Add(Division.ReportLineId, ((int)System.Web.HttpContext.Current.Session["DivisionId"]).ToString());
                //ReportLine Process = new ReportLineService(_unitOfWork).GetReportLineByName("Process", header.ReportHeaderId);
                //if (Process != null)
                //    DefaultValue.Add(Process.ReportLineId, ((int)SEttings.ProcessId).ToString());
            }

            TempData["ReportLayoutDefaultValues"] = DefaultValue;

            return Redirect((string)System.Configuration.ConfigurationManager.AppSettings["JobsDomain"] + "/Report_ReportPrint/ReportPrint/?MenuId=" + Dt.ReportMenuId);

        }

        public ActionResult Import(int id)//Document Type Id
        {
            //ControllerAction ca = new ControllerActionService(_unitOfWork).Find(id);

            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            var settings = new SaleInvoiceSettingService(_unitOfWork).GetSaleInvoiceSettingForDocument(id, DivisionId, SiteId);

            if (settings != null)
            {
                if (settings.ImportMenuId != null)
                {
                    MenuViewModel menuviewmodel = new MenuService(_unitOfWork).GetMenu((int)settings.ImportMenuId);

                    if (menuviewmodel == null)
                    {
                        return View("~/Views/Shared/UnderImplementation.cshtml");
                    }
                    else if (!string.IsNullOrEmpty(menuviewmodel.URL))
                    {
                        return Redirect(System.Configuration.ConfigurationManager.AppSettings[menuviewmodel.URL] + "/" + menuviewmodel.ControllerName + "/" + menuviewmodel.ActionName + "/" + id + "?MenuId=" + menuviewmodel.MenuId);
                    }
                    else
                    {
                        return RedirectToAction(menuviewmodel.ActionName, menuviewmodel.ControllerName, new { MenuId = menuviewmodel.MenuId, id = id });
                    }
                }
            }
            return RedirectToAction("Index", new { id = id });
        }

        public ActionResult GetCustomPerson(string searchTerm, int pageSize, int pageNum, int filter)//DocTypeId
        {
            var Query = _SaleInvoiceHeaderService.GetCustomPerson(filter, searchTerm);
            var temp = Query.Skip(pageSize * (pageNum - 1))
                .Take(pageSize)
                .ToList();

            var count = Query.Count();

            ComboBoxPagedResult Data = new ComboBoxPagedResult();
            Data.Results = temp;
            Data.Total = count;

            return new JsonpResult
            {
                Data = Data,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public bool Submitvalidation(int id, out string Msg)
        {
            Msg = "";
            int SaleInvoiceLine = (new SaleInvoiceLineService(_unitOfWork).GetDirectSaleInvoiceLineListForIndex(id)).Count();
            if (SaleInvoiceLine == 0)
            {
                Msg = "Add Line Record. <br />";
            }
            else
            {
                Msg = "";
            }
            return (string.IsNullOrEmpty(Msg));
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

        [HttpGet]
        public ActionResult _CreateInvoiceReturn(int id)
        {
            InvoiceReturn InvoiceReturn = new InvoiceReturn();
            InvoiceReturn.SaleInvoiceHeaderId = id;
            ViewBag.ReasonList = new ReasonService(_unitOfWork).GetReasonList(TransactionDocCategoryConstants.SaleInvoiceReturn).ToList();
            InvoiceReturn.DocDate = DateTime.Now;
            return PartialView("_InvoiceReturn", InvoiceReturn);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult _CreateInvoiceReturnPost(InvoiceReturn svm)
        {
            int Cnt = 0;
            int Serial = 0;
            int pk = 0;
            int Gpk = 0;
            int PersonCount = 0;
            bool HeaderChargeEdit = false;

            SaleInvoiceHeader SaleInvoiceHeader = new SaleInvoiceHeaderService(_unitOfWork).FindDirectSaleInvoice(svm.SaleInvoiceHeaderId);
            //SaleDispatchHeader SaleDispatchHeader = new SaleDispatchHeaderService(_unitOfWork).Find(SaleInvoiceHeader.SaleDispatchHeaderId ?? 0);
            //var DispatchLine = new SaleDispatchLineService(_unitOfWork).GetSaleDispatchLineList(SaleDispatchHeader.SaleDispatchHeaderId);


            List<LineChargeRates> LineChargeRates = new List<LineChargeRates>();

            var SaleInvoiceSettings = new SaleInvoiceSettingService(_unitOfWork).GetSaleInvoiceSettingForDocument(SaleInvoiceHeader.DocTypeId, SaleInvoiceHeader.DivisionId, SaleInvoiceHeader.SiteId);
            int InvoiceRetHeaderDocTypeId = 0;

            if (SaleInvoiceSettings.SaleInvoiceReturnDocTypeId == null)
            {
                string message = "Invoice Return Document Type is not difined in settings.";
                ModelState.AddModelError("", message);
                return PartialView("_InvoiceReturn", svm);
            }
            else
            {
                InvoiceRetHeaderDocTypeId = (int)SaleInvoiceSettings.SaleInvoiceReturnDocTypeId;
            }


            if (ModelState.IsValid)
            {
                var SaleInvoiceLineList = (from p in db.ViewSaleInvoiceBalance
                                           join l in db.SaleInvoiceLine on p.SaleInvoiceLineId equals l.SaleInvoiceLineId into linetable
                                           from linetab in linetable.DefaultIfEmpty()
                                           join t in db.SaleInvoiceHeader on p.SaleInvoiceHeaderId equals t.SaleInvoiceHeaderId into table
                                           from tab in table.DefaultIfEmpty()
                                           join t1 in db.SaleDispatchLine on p.SaleDispatchLineId equals t1.SaleDispatchLineId into table1
                                           from tab1 in table1.DefaultIfEmpty()
                                           join packtab in db.PackingLine on tab1.PackingLineId equals packtab.PackingLineId
                                           join product in db.Product on p.ProductId equals product.ProductId into table2
                                           from tab2 in table2.DefaultIfEmpty()
                                           join B in db.BusinessEntity on tab.SaleToBuyerId equals B.PersonID into BusinessEntityTable
                                           from BusinessEntityTab in BusinessEntityTable.DefaultIfEmpty()
                                           where p.SaleInvoiceHeaderId == SaleInvoiceHeader.SaleInvoiceHeaderId
                                           && p.BalanceQty > 0
                                           select new SaleInvoiceReturnLineViewModel
                                           {
                                               Dimension1Name = packtab.Dimension1.Dimension1Name,
                                               Dimension2Name = packtab.Dimension2.Dimension2Name,
                                               Specification = packtab.Specification,
                                               InvoiceBalQty = p.BalanceQty,
                                               Qty = p.BalanceQty,
                                               SaleInvoiceHeaderDocNo = tab.DocNo,
                                               ProductName = tab2.ProductName,
                                               ProductId = p.ProductId,
                                               GodownId = tab1.GodownId,
                                               SaleInvoiceLineId = p.SaleInvoiceLineId,
                                               UnitId = tab2.UnitId,
                                               UnitConversionMultiplier = linetab.UnitConversionMultiplier ?? 0,
                                               DealUnitId = linetab.DealUnitId,
                                               Rate = linetab.Rate,
                                               RateAfterDiscount = packtab.SaleOrderLine == null ? 0 : (packtab.SaleOrderLine.Amount / packtab.SaleOrderLine.DealQty),
                                               Amount = linetab.Amount,
                                               unitDecimalPlaces = tab2.Unit.DecimalPlaces,
                                               DealunitDecimalPlaces = linetab.DealUnit.DecimalPlaces,
                                               DiscountPer = linetab.DiscountPer,
                                               DiscountAmount = linetab.DiscountAmount,
                                               ProductUidName = packtab.ProductUid.ProductUidName,
                                               SalesTaxGroupPersonId = BusinessEntityTab.SalesTaxGroupPartyId,
                                               SalesTaxGroupProductId = linetab.SalesTaxGroupProductId
                                           }).ToList();

                if (SaleInvoiceLineList.Sum(i => i.InvoiceBalQty) > 0)
                {
                    SaleInvoiceSetting Settings = new SaleInvoiceSettingService(_unitOfWork).GetSaleInvoiceSettingForDocument(InvoiceRetHeaderDocTypeId, SaleInvoiceHeader.DivisionId, SaleInvoiceHeader.SiteId);

                    //SaleDispatchReturnHeader GoodsRetHeader = new SaleDispatchReturnHeader();
                    //GoodsRetHeader.DocTypeId = (int)Settings.DocTypeDispatchReturnId;
                    //GoodsRetHeader.DocDate = svm.DocDate;
                    //GoodsRetHeader.DocNo = new DocumentTypeService(_unitOfWork).FGetNewDocNo("DocNo", ConfigurationManager.AppSettings["DataBaseSchema"] + ".SaleDispatchReturnHeaders", GoodsRetHeader.DocTypeId, svm.DocDate, SaleInvoiceHeader.DivisionId, SaleInvoiceHeader.SiteId);
                    //GoodsRetHeader.SiteId = SaleInvoiceHeader.SiteId;
                    //GoodsRetHeader.DivisionId = SaleInvoiceHeader.DivisionId;
                    //GoodsRetHeader.BuyerId = SaleInvoiceHeader.SaleToBuyerId;
                    //GoodsRetHeader.ReasonId = svm.ReasonId;
                    //GoodsRetHeader.GodownId = DispatchLine.FirstOrDefault().GodownId;
                    //GoodsRetHeader.Remark = svm.Remark;
                    //GoodsRetHeader.CreatedDate = DateTime.Now;
                    //GoodsRetHeader.ModifiedDate = DateTime.Now;
                    //GoodsRetHeader.CreatedBy = User.Identity.Name;
                    //GoodsRetHeader.ModifiedBy = User.Identity.Name;
                    //GoodsRetHeader.ObjectState = Model.ObjectState.Added;
                    //new SaleDispatchReturnHeaderService(_unitOfWork).Create(GoodsRetHeader);

                    SaleInvoiceReturnHeader InvoiceRetHeader = new SaleInvoiceReturnHeader();
                    InvoiceRetHeader.DocTypeId = InvoiceRetHeaderDocTypeId;
                    InvoiceRetHeader.DocDate = svm.DocDate;
                    InvoiceRetHeader.DocNo = new DocumentTypeService(_unitOfWork).FGetNewDocNo("DocNo", ConfigurationManager.AppSettings["DataBaseSchema"] + ".SaleInvoiceReturnHeaders", InvoiceRetHeader.DocTypeId, svm.DocDate, SaleInvoiceHeader.DivisionId, SaleInvoiceHeader.SiteId);
                    InvoiceRetHeader.BuyerId = SaleInvoiceHeader.SaleToBuyerId;
                    InvoiceRetHeader.SiteId = SaleInvoiceHeader.SiteId;
                    InvoiceRetHeader.DivisionId = SaleInvoiceHeader.DivisionId;
                    InvoiceRetHeader.BuyerId = SaleInvoiceHeader.SaleToBuyerId;
                    InvoiceRetHeader.SalesTaxGroupPersonId = SaleInvoiceLineList.FirstOrDefault().SalesTaxGroupPersonId;


                    //InvoiceRetHeader.CurrencyId = SaleInvoiceHeader.CurrencyId;
                    InvoiceRetHeader.ReasonId = svm.ReasonId;
                    InvoiceRetHeader.Remark = svm.Remark;
                    InvoiceRetHeader.Nature = TransactionNatureConstants.Return;
                    InvoiceRetHeader.CreatedDate = DateTime.Now;
                    InvoiceRetHeader.ModifiedDate = DateTime.Now;
                    InvoiceRetHeader.CreatedBy = User.Identity.Name;
                    InvoiceRetHeader.ModifiedBy = User.Identity.Name;
                    //InvoiceRetHeader.SaleDispatchReturnHeaderId = GoodsRetHeader.SaleDispatchReturnHeaderId;
                    InvoiceRetHeader.ObjectState = Model.ObjectState.Added;
                    new SaleInvoiceReturnHeaderService(_unitOfWork).Create(InvoiceRetHeader);

                    int CalculationId = Settings.CalculationId;

                    //IEnumerable<SaleInvoiceLine> SaleInvoiceLineList = new SaleInvoiceLineService(_unitOfWork).GetSaleInvoiceLineList(Sh.SaleInvoiceHeaderId);



                    List<LineDetailListViewModel> LineList = new List<LineDetailListViewModel>();
                    List<HeaderChargeViewModel> HeaderCharges = new List<HeaderChargeViewModel>();
                    List<LineChargeViewModel> LineCharges = new List<LineChargeViewModel>();


                    foreach (var item in SaleInvoiceLineList)
                    {
                        decimal balqty = (from p in db.ViewSaleInvoiceBalance
                                          where p.SaleInvoiceLineId == item.SaleInvoiceLineId
                                          select p.BalanceQty).FirstOrDefault();


                        if (item.Qty > 0 && item.Qty <= balqty)
                        {
                            SaleInvoiceReturnLine line = new SaleInvoiceReturnLine();
                            //var receipt = new SaleDispatchLineService(_unitOfWork).Find(item.SaleDispatchLineId );


                            line.SaleInvoiceReturnHeaderId = InvoiceRetHeader.SaleInvoiceReturnHeaderId;
                            line.SaleInvoiceLineId = item.SaleInvoiceLineId;
                            line.Qty = item.Qty;
                            line.Sr = Serial++;
                            line.DiscountPer = item.DiscountPer;
                            line.DiscountAmount = item.DiscountAmount;
                            line.Rate = item.Rate;
                            line.DealQty = item.UnitConversionMultiplier * item.Qty;
                            line.DealUnitId = item.DealUnitId;
                            line.UnitConversionMultiplier = item.UnitConversionMultiplier;
                            line.Amount = item.Amount;

                            line.SalesTaxGroupProductId = item.SalesTaxGroupProductId;
                            line.Remark = item.Remark;
                            line.CreatedDate = DateTime.Now;
                            line.ModifiedDate = DateTime.Now;
                            line.CreatedBy = User.Identity.Name;
                            line.ModifiedBy = User.Identity.Name;
                            line.SaleInvoiceReturnLineId = pk;


                            //SaleDispatchReturnLine GLine = Mapper.Map<SaleInvoiceReturnLine, SaleDispatchReturnLine>(line);
                            //GLine.SaleDispatchLineId = new SaleInvoiceLineService(_unitOfWork).Find(line.SaleInvoiceLineId).SaleDispatchLineId;
                            //GLine.SaleDispatchReturnHeaderId = GoodsRetHeader.SaleDispatchReturnHeaderId;
                            //GLine.SaleDispatchReturnLineId = Gpk;
                            //GLine.Qty = line.Qty;
                            //GLine.GodownId = (int)item.GodownId;
                            //GLine.ObjectState = Model.ObjectState.Added;


                            //SaleDispatchLine SaleDispatchLine = new SaleDispatchLineService(_unitOfWork).Find(GLine.SaleDispatchLineId);
                            //PackingLine PackingLin = new PackingLineService(_unitOfWork).Find(SaleDispatchLine.PackingLineId);

                            //StockViewModel StockViewModel = new StockViewModel();


                            //if (Cnt == 0)
                            //{
                            //    StockViewModel.StockHeaderId = GoodsRetHeader.StockHeaderId ?? 0;
                            //}
                            //else
                            //{
                            //    if (GoodsRetHeader.StockHeaderId != null && GoodsRetHeader.StockHeaderId != 0)
                            //    {
                            //        StockViewModel.StockHeaderId = (int)GoodsRetHeader.StockHeaderId;
                            //    }
                            //    else
                            //    {
                            //        StockViewModel.StockHeaderId = -1;
                            //    }

                            //}

                            //StockViewModel.StockId = -Cnt;

                            //StockViewModel.DocHeaderId = GoodsRetHeader.SaleDispatchReturnHeaderId;
                            //StockViewModel.DocLineId = SaleDispatchLine.SaleDispatchLineId;
                            //StockViewModel.DocTypeId = GoodsRetHeader.DocTypeId;
                            //StockViewModel.StockHeaderDocDate = GoodsRetHeader.DocDate;
                            //StockViewModel.StockDocDate = GoodsRetHeader.DocDate;
                            //StockViewModel.DocNo = GoodsRetHeader.DocNo;
                            //StockViewModel.DivisionId = GoodsRetHeader.DivisionId;
                            //StockViewModel.SiteId = GoodsRetHeader.SiteId;
                            //StockViewModel.CurrencyId = null;
                            //StockViewModel.PersonId = GoodsRetHeader.BuyerId;
                            //StockViewModel.ProductId = PackingLin.ProductId;
                            //StockViewModel.ProductUidId = PackingLin.ProductUidId;
                            //StockViewModel.HeaderFromGodownId = null;
                            //StockViewModel.HeaderGodownId = GLine.GodownId;
                            //StockViewModel.HeaderProcessId = Settings.ProcessId;
                            //StockViewModel.GodownId = (int)GLine.GodownId;
                            //StockViewModel.Remark = svm.Remark;
                            //StockViewModel.Status = 0;
                            //StockViewModel.ProcessId = null;
                            //StockViewModel.LotNo = null;
                            //StockViewModel.CostCenterId = null;
                            //StockViewModel.Qty_Iss = 0;
                            //StockViewModel.Qty_Rec = GLine.Qty;
                            //StockViewModel.Rate = null;
                            //StockViewModel.ExpiryDate = null;
                            //StockViewModel.Specification = PackingLin.Specification;
                            //StockViewModel.Dimension1Id = PackingLin.Dimension1Id;
                            //StockViewModel.Dimension2Id = PackingLin.Dimension2Id;
                            //StockViewModel.CreatedBy = User.Identity.Name;
                            //StockViewModel.CreatedDate = DateTime.Now;
                            //StockViewModel.ModifiedBy = User.Identity.Name;
                            //StockViewModel.ModifiedDate = DateTime.Now;

                            //string StockPostingError = "";
                            //StockPostingError = new StockService(_unitOfWork).StockPost(ref StockViewModel);

                            //if (StockPostingError != "")
                            //{
                            //    string message = StockPostingError;
                            //    ModelState.AddModelError("", message);
                            //    return PartialView("_InvoiceReturn", svm);
                            //}


                            //if (Cnt == 0)
                            //{
                            //    GoodsRetHeader.StockHeaderId = StockViewModel.StockHeaderId;
                            //}


                            //GLine.StockId = StockViewModel.StockId;


                            //new SaleDispatchReturnLineService(_unitOfWork).Create(GLine);

                            //line.SaleDispatchReturnLineId = GLine.SaleDispatchReturnLineId;
                            line.ObjectState = Model.ObjectState.Added;
                            new SaleInvoiceReturnLineService(_unitOfWork).Create(line);

                            LineList.Add(new LineDetailListViewModel { Amount = line.Amount, Rate = line.Rate, LineTableId = line.SaleInvoiceReturnLineId, HeaderTableId = item.SaleInvoiceReturnHeaderId, PersonID = InvoiceRetHeader.BuyerId, DealQty = line.DealQty });

                            List<CalculationProductViewModel> ChargeRates = new CalculationProductService(_unitOfWork).GetChargeRates(CalculationId, InvoiceRetHeader.DocTypeId, InvoiceRetHeader.SiteId, InvoiceRetHeader.DivisionId,
                                Settings.ProcessId ?? 0, item.SalesTaxGroupPersonId, item.SalesTaxGroupProductId).ToList();
                            if (ChargeRates != null)
                            {
                                LineChargeRates.Add(new LineChargeRates { LineId = line.SaleInvoiceReturnLineId, ChargeRates = ChargeRates });
                            }



                            Gpk++;
                            pk++;

                            Cnt = Cnt + 1;
                        }
                    }


                    var LineListWithReferences = (from p in LineList
                                                  join t3 in LineChargeRates on p.LineTableId equals t3.LineId into LineChargeRatesTable
                                                  from LineChargeRatesTab in LineChargeRatesTable.DefaultIfEmpty()
                                                  orderby p.LineTableId
                                                  select new LineDetailListViewModel
                                                  {
                                                      Amount = p.Amount,
                                                      DealQty = p.DealQty,
                                                      HeaderTableId = p.HeaderTableId,
                                                      LineTableId = p.LineTableId,
                                                      PersonID = p.PersonID,
                                                      Rate = p.Rate,
                                                      CostCenterId = p.CostCenterId,
                                                      ChargeRates = LineChargeRatesTab.ChargeRates,
                                                  }).ToList();

                    if (CalculationId != null)
                    {
                        new ChargesCalculationService(_unitOfWork).CalculateCharges(LineListWithReferences, InvoiceRetHeader.SaleInvoiceReturnHeaderId, (int)CalculationId, null, out LineCharges, out HeaderChargeEdit, out HeaderCharges, "Web.SaleInvoiceReturnHeaderCharges", "Web.SaleInvoiceReturnLineCharges", out PersonCount, InvoiceRetHeader.DocTypeId, InvoiceRetHeader.SiteId, InvoiceRetHeader.DivisionId);
                    }

                    // Saving Charges
                    foreach (var item in LineCharges)
                    {
                        SaleInvoiceReturnLineCharge PoLineCharge = Mapper.Map<LineChargeViewModel, SaleInvoiceReturnLineCharge>(item);
                        PoLineCharge.HeaderTableId = InvoiceRetHeader.SaleInvoiceReturnHeaderId;
                        PoLineCharge.PersonID = SaleInvoiceHeader.BillToBuyerId;
                        PoLineCharge.ObjectState = Model.ObjectState.Added;
                        new SaleInvoiceReturnLineChargeService(_unitOfWork).Create(PoLineCharge);
                    }


                    //Saving Header charges
                    for (int i = 0; i < HeaderCharges.Count(); i++)
                    {
                        SaleInvoiceReturnHeaderCharge POHeaderCharge = Mapper.Map<HeaderChargeViewModel, SaleInvoiceReturnHeaderCharge>(HeaderCharges[i]);
                        POHeaderCharge.HeaderTableId = InvoiceRetHeader.SaleInvoiceReturnHeaderId;
                        POHeaderCharge.PersonID = InvoiceRetHeader.BuyerId;

                        if (SaleInvoiceHeader.BillToBuyerId != InvoiceRetHeader.BuyerId)
                            POHeaderCharge.PersonID = SaleInvoiceHeader.BillToBuyerId;

                        POHeaderCharge.ObjectState = Model.ObjectState.Added;
                        new SaleInvoiceReturnHeaderChargeService(_unitOfWork).Create(POHeaderCharge);
                    }

                    try
                    {
                        _unitOfWork.Save();
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        ModelState.AddModelError("", message);
                        return PartialView("_InvoiceReturn", svm);
                    }

                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = SaleInvoiceHeader.DocTypeId,
                        DocId = InvoiceRetHeader.SaleInvoiceReturnHeaderId,
                        ActivityType = (int)ActivityTypeContants.MultipleCreate,
                        DocNo = SaleInvoiceHeader.DocNo,
                        DocDate = SaleInvoiceHeader.DocDate,
                        DocStatus = SaleInvoiceHeader.Status,
                    }));


                    try
                    {
                        //StockHeader StockHeader = new StockHeaderService(_unitOfWork).Find((int)GoodsRetHeader.StockHeaderId);
                        //StockHeader.DocHeaderId = GoodsRetHeader.SaleDispatchReturnHeaderId;
                        //new StockHeaderService(_unitOfWork).Update(StockHeader);
                        _unitOfWork.Save();
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                    }




                    //return Json(new { success = true });
                    //return Redirect(System.Configuration.ConfigurationManager.AppSettings["SaleDomain"] + "/SaleInvoiceReturnHeader/Submit/" + InvoiceRetHeader.SaleInvoiceReturnHeaderId);
                    //return Redirect(System.Configuration.ConfigurationManager.AppSettings["SaleDomain"] + "/DirectSaleInvoiceHeader/_InvoiceReturnSubmit/" + InvoiceRetHeader.SaleInvoiceReturnHeaderId);
                    //return RedirectToAction("Index", new { id = SaleInvoiceHeader.DocTypeId, IndexType = "All" }).Success("Record submitted successfully.");
                    return Json(new { success = true, Url = "/SaleInvoiceReturnHeader/Submit/" + InvoiceRetHeader.SaleInvoiceReturnHeaderId });


                }
            }
            else
            {
                string message = "Balance is 0 for this invoice.";
                ModelState.AddModelError("", message);
                return PartialView("_InvoiceReturn", svm);
            }
            return PartialView("_InvoiceReturn", svm);
        }

    }
}