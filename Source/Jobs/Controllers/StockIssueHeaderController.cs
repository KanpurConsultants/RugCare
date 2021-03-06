﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Core.Common;
using Model.Models;
using Data.Models;
using Service;
using Jobs.Helpers;
using Data.Infrastructure;
using Presentation.ViewModels;
using AutoMapper;
using System.Configuration;
using Presentation;
using Model.ViewModel;
using System.Data.SqlClient;
using System.Xml.Linq;
using DocumentEvents;
using CustomEventArgs;
using StockIssueDocumentEvents;
using Reports.Reports;
using Reports.Controllers;
using Model.ViewModels;



namespace Jobs.Controllers
{
    [Authorize]
    public class StockIssueHeaderController : System.Web.Mvc.Controller
    {

        private ApplicationDbContext context = new ApplicationDbContext();

        List<string> UserRoles = new List<string>();
        ActiivtyLogViewModel LogVm = new ActiivtyLogViewModel();

        private bool EventException = false;
        bool TimePlanValidation = true;
        string ExceptionMsg = "";
        bool Continue = true;

        IStockHeaderService _StockHeaderService;
        IActivityLogService _ActivityLogService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;

        public StockIssueHeaderController(IStockHeaderService PurchaseOrderHeaderService, IActivityLogService ActivityLogService, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _StockHeaderService = PurchaseOrderHeaderService;
            _ActivityLogService = ActivityLogService;
            _exception = exec;
            _unitOfWork = unitOfWork;
            if (!StockIssueEvents.Initialized)
            {
                StockIssueEvents Obj = new StockIssueEvents();
            }

            UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];

            //Log Initialization
            LogVm.SessionId = 0;
            LogVm.ControllerName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("controller");
            LogVm.ActionName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("action");
            LogVm.User = System.Web.HttpContext.Current.Request.RequestContext.HttpContext.User.Identity.Name;
        }

        // GET: /StockHeader

        [HttpGet]
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


        public ActionResult Index(int id, string IndexType)//DocumentTypeId 
        {
            if (IndexType == "PTS")
            {
                return RedirectToAction("Index_PendingToSubmit", new { id });
            }
            else if (IndexType == "PTR")
            {
                return RedirectToAction("Index_PendingToReview", new { id });
            }
            IQueryable<StockHeaderViewModel> p = _StockHeaderService.GetStockHeaderList(id, User.Identity.Name);
            ViewBag.Name = new DocumentTypeService(_unitOfWork).Find(id).DocumentTypeName;
            ViewBag.id = id;
            PrepareViewBag(id);
            ViewBag.PendingToSubmit = PendingToSubmitCount(id);
            ViewBag.PendingToReview = PendingToReviewCount(id);
            ViewBag.IndexStatus = "All";
            return View(p);
        }

        public ActionResult Index_PendingToSubmit(int id)
        {
            IQueryable<StockHeaderViewModel> p = _StockHeaderService.GetStockHeaderListPendingToSubmit(id, User.Identity.Name);

            PrepareViewBag(id);
            ViewBag.PendingToSubmit = PendingToSubmitCount(id);
            ViewBag.PendingToReview = PendingToReviewCount(id);
            ViewBag.IndexStatus = "PTS";
            return View("Index", p);
        }

        public ActionResult Index_PendingToReview(int id)
        {
            IQueryable<StockHeaderViewModel> p = _StockHeaderService.GetStockHeaderListPendingToReview(id, User.Identity.Name);
            PrepareViewBag(id);
            ViewBag.PendingToSubmit = PendingToSubmitCount(id);
            ViewBag.PendingToReview = PendingToReviewCount(id);
            ViewBag.IndexStatus = "PTR";
            return View("Index", p);
        }


        private void PrepareViewBag(int id)
        {
            ViewBag.Name = new DocumentTypeService(_unitOfWork).Find(id).DocumentTypeName;
            ViewBag.id = id;

            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            ViewBag.AdminSetting = UserRoles.Contains("Admin").ToString();
            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(id,DivisionId, SiteId);
            if(settings !=null)
            {
                ViewBag.ImportMenuId = settings.ImportMenuId;
                ViewBag.SqlProcDocumentPrint = settings.SqlProcDocumentPrint;
                ViewBag.ExportMenuId = settings.ExportMenuId;
                ViewBag.SqlProcGatePass = settings.SqlProcGatePass;
            }

        }

        // GET: /StockHeader/Create
        [HttpGet]
        public ActionResult Create(int id)//DocumentTypeId
        {
            StockHeaderViewModel p = new StockHeaderViewModel();

            p.DocDate = DateTime.Now;
            p.DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            p.SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            p.CreatedDate = DateTime.Now;

            //Getting Settings
            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(id, p.DivisionId, p.SiteId);

            if (settings == null && UserRoles.Contains("SysAdmin"))
            {
                return RedirectToAction("Create", "StockHeaderSettings", new { id = id }).Warning("Please create Material Issue settings");
            }
            else if (settings == null && !UserRoles.Contains("SysAdmin"))
            {
                return View("~/Views/Shared/InValidSettings.cshtml");
            }
            p.StockHeaderSettings = Mapper.Map<StockHeaderSettings, StockHeaderSettingsViewModel>(settings);

            if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, id, settings.ProcessId, this.ControllerContext.RouteData.Values["controller"].ToString(), "Create") == false)
            {
                return View("~/Views/Shared/PermissionDenied.cshtml").Warning("You don't have permission to do this task.");
            }

            if ((settings.isVisibleProcessHeader ?? false) == false)
            {
                p.ProcessId = settings.ProcessId;
            }

            if (System.Web.HttpContext.Current.Session["DefaultGodownId"] != null)
                p.GodownId = (int)System.Web.HttpContext.Current.Session["DefaultGodownId"];

            PrepareViewBag(id);

            p.DocTypeId = id;
            p.DocNo = new DocumentTypeService(_unitOfWork).FGetNewDocNo("DocNo", ConfigurationManager.AppSettings["DataBaseSchema"] + ".StockHeaders", p.DocTypeId, p.DocDate, p.DivisionId, p.SiteId);
            ViewBag.Mode = "Add";
            return View(p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Post(StockHeaderViewModel svm)
        {
            StockHeaderTransport s = Mapper.Map<StockHeaderViewModel, StockHeaderTransport>(svm);

            #region BeforeSave
            bool BeforeSave = true;
            try
            {
                if (svm.StockHeaderId <= 0)
                    BeforeSave = StockIssueDocEvents.beforeHeaderSaveEvent(this, new StockEventArgs(svm.StockHeaderId, EventModeConstants.Add), ref context);
                else
                    BeforeSave = StockIssueDocEvents.beforeHeaderSaveEvent(this, new StockEventArgs(svm.StockHeaderId, EventModeConstants.Edit), ref context);
            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXC"] += message;
                EventException = true;
            }
            if (!BeforeSave)
                TempData["CSEXC"] += "Failed validation before save";
            #endregion

            #region DocTypeTimeLineValidation

            try
            {

                if (svm.StockHeaderId <= 0)
                    TimePlanValidation = DocumentValidation.ValidateDocument(Mapper.Map<DocumentUniqueId>(svm), DocumentTimePlanTypeConstants.Create, User.Identity.Name, out ExceptionMsg, out Continue);
                else
                    TimePlanValidation = DocumentValidation.ValidateDocument(Mapper.Map<DocumentUniqueId>(svm), DocumentTimePlanTypeConstants.Modify, User.Identity.Name, out ExceptionMsg, out Continue);

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

            if (svm.StockHeaderSettings != null)
            {
                if (svm.StockHeaderSettings.isMandatoryHeaderCostCenter == true && (svm.CostCenterId <= 0 || svm.CostCenterId == null))
                {
                    ModelState.AddModelError("CostCenterId", "The CostCenter field is required");
                }
                if (svm.StockHeaderSettings.isMandatoryMachine == true && (svm.MachineId <= 0 || svm.MachineId == null))
                {
                    ModelState.AddModelError("MachineId", "The Machine field is required");
                }
            }

            if (ModelState.IsValid && BeforeSave && !EventException && (TimePlanValidation || Continue))
            {

                #region CreateRecord
                if (svm.StockHeaderId <= 0)
                {
                    s.TransportId= svm.TransportId;
                    s.CreatedDate = DateTime.Now;
                    s.ModifiedDate = DateTime.Now;
                    s.CreatedBy = User.Identity.Name;
                    s.ModifiedBy = User.Identity.Name;
                    s.Status = (int)StatusConstants.Drafted;
                    //_StockHeaderService.Create(s);
                    s.ObjectState = Model.ObjectState.Added;
                    context.StockHeader.Add(s);

                    try
                    {
                        StockIssueDocEvents.onHeaderSaveEvent(this, new StockEventArgs(s.StockHeaderId, EventModeConstants.Add), ref context);
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                        EventException = true;
                    }

                    try
                    {
                        if (EventException)
                        { throw new Exception(); }

                        context.SaveChanges();
                        //_unitOfWork.Save();
                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                        PrepareViewBag(svm.DocTypeId);
                        ViewBag.Mode = "Add";
                        return View("Create", svm);
                    }

                    try
                    {
                        StockIssueDocEvents.afterHeaderSaveEvent(this, new StockEventArgs(s.StockHeaderId, EventModeConstants.Add), ref context);
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                    }

                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = s.DocTypeId,
                        DocId = s.StockHeaderId,
                        ActivityType = (int)ActivityTypeContants.Added,
                        DocNo = s.DocNo,
                        DocDate = s.DocDate,
                        DocStatus = s.Status,
                    }));

                    return RedirectToAction("Modify", "StockIssueHeader", new { Id = s.StockHeaderId }).Success("Data saved successfully");

                }
                #endregion

                #region EditRecord
                else
                {
                    bool GodownChanged = false;
                    bool DocDateChanged = false;
                    List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();

                    StockHeaderTransport temp = context.StockHeaderTransport.Where(m => m.StockHeaderId == s.StockHeaderId).FirstOrDefault();

                    GodownChanged = (temp.GodownId == s.GodownId) ? false : true;
                    DocDateChanged = (temp.DocDate == s.DocDate) ? false : true;

                    StockHeader ExRec = new StockHeader();
                    ExRec = Mapper.Map<StockHeader>(temp);

                    int status = temp.Status;

                    if (temp.Status != (int)StatusConstants.Drafted && temp.Status != (int)StatusConstants.Import)
                        temp.Status = (int)StatusConstants.Modified;


                    temp.DocDate = s.DocDate;
                    temp.DocNo = s.DocNo;
                    temp.CostCenterId = s.CostCenterId;
                    temp.MachineId = s.MachineId;
                    temp.PersonId = s.PersonId;
                    temp.ProcessId = s.ProcessId;
                    temp.GodownId = s.GodownId;
                    temp.Reading = s.Reading;
                    temp.Remark = s.Remark;
                    temp.TransportId = svm.TransportId;
                    temp.VehicleNo = svm.VehicleNo;
                    temp.LrNo = svm.LrNo;
                    temp.EWayBillNo = svm.EWayBillNo;
                    temp.EWayBillDate = svm.EWayBillDate;
                    temp.LrDate = svm.LrDate;
                    temp.PaymentType = svm.PaymentType;
                    temp.Destination = svm.Destination;

                    temp.ModifiedDate = DateTime.Now;
                    temp.ModifiedBy = User.Identity.Name;
                    //_StockHeaderService.Update(temp);
                    temp.ObjectState = Model.ObjectState.Modified;
                    context.StockHeader.Add(temp);


                    //if (GodownChanged)
                    //    new StockService(_unitOfWork).UpdateStockGodownId(temp.StockHeaderId, temp.GodownId, context);


                    IEnumerable<Stock> stocklist = new StockService(_unitOfWork).GetStockForStockHeaderId(temp.StockHeaderId);

                    foreach (Stock item in stocklist)
                    {
                        Stock Stock = new StockService(_unitOfWork).Find(item.StockId);


                        if (GodownChanged == true)
                        {
                            Stock.GodownId = (int)temp.GodownId;
                        }

                        if (DocDateChanged == true)
                        {
                            Stock.DocDate = temp.DocDate;
                        }

                        Stock.ObjectState = Model.ObjectState.Modified;
                        context.Stock.Add(Stock);


                        if (item.ProductUidId != null && item.ProductUidId != 0)
                        {
                            ProductUid ProductUid = new ProductUidService(_unitOfWork).Find((int)item.ProductUidId);

                            if (DocDateChanged == true)
                            {
                                ProductUid.LastTransactionDocDate = temp.DocDate;
                            }

                            ProductUid.ObjectState = Model.ObjectState.Modified;
                            context.ProductUid.Add(ProductUid);
                        }
                    }


                    LogList.Add(new LogTypeViewModel
                    {
                        ExObj = ExRec,
                        Obj = temp,
                    });

                    XElement Modifications = new ModificationsCheckService().CheckChanges(LogList);

                    try
                    {
                        StockIssueDocEvents.onHeaderSaveEvent(this, new StockEventArgs(temp.StockHeaderId, EventModeConstants.Edit), ref context);
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                        EventException = true;
                    }

                    try
                    {
                        if (EventException)
                        { throw new Exception(); }

                        context.SaveChanges();
                        //_unitOfWork.Save();
                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                        PrepareViewBag(svm.DocTypeId);
                        ViewBag.id = svm.DocTypeId;
                        return View("Create", svm);
                    }

                    try
                    {
                        StockIssueDocEvents.afterHeaderSaveEvent(this, new StockEventArgs(s.StockHeaderId, EventModeConstants.Edit), ref context);
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                    }

                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = temp.DocTypeId,
                        DocId = temp.StockHeaderId,
                        ActivityType = (int)ActivityTypeContants.Modified,
                        DocNo = temp.DocNo,
                        xEModifications = Modifications,
                        DocDate = temp.DocDate,
                        DocStatus = temp.Status,
                    }));


                    return RedirectToAction("Index", new { id = svm.DocTypeId }).Success("Data saved successfully");

                }
                #endregion

            }
            PrepareViewBag(svm.DocTypeId);
            ViewBag.Mode = "Add";
            return View("Create", svm);
        }

        public ActionResult GetGodown(string searchTerm, int pageSize, int pageNum, int filter)//DocTypeId
        {
            var Query = _StockHeaderService.GetGodown(filter, searchTerm);
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

        [HttpGet]
        public ActionResult Modify(int id, string IndexType)
        {
            StockHeader header = _StockHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Drafted || header.Status == (int)StatusConstants.Import)
                return Edit(id, IndexType);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult ModifyAfter_Submit(int id, string IndexType)
        {
            StockHeader header = _StockHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Submitted || header.Status == (int)StatusConstants.Modified)
                return Edit(id, IndexType);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult ModifyAfter_Approve(int id, string IndexType)
        {
            StockHeader header = _StockHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Approved)
                return Edit(id, IndexType);
            else
                return HttpNotFound();
        }

        // GET: /StockHeader/Edit/5
        [HttpGet]
        private ActionResult Edit(int id, string IndexType)
        {
            ViewBag.IndexStatus = IndexType;
            StockHeaderViewModel s = _StockHeaderService.GetStockHeader(id);

            if (s == null)
            {
                return HttpNotFound();
            }

            if (s.Status != (int)StatusConstants.Drafted)
                if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, s.DocTypeId, s.ProcessId, this.ControllerContext.RouteData.Values["controller"].ToString(), "Edit") == false)
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

            //Job Order Settings
            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(s.DocTypeId, s.DivisionId, s.SiteId);

            if (settings == null && UserRoles.Contains("SysAdmin"))
            {
                return RedirectToAction("Create", "StockHeaderSettings", new { id = s.DocTypeId }).Warning("Please create Material Issue settings");
            }
            else if (settings == null && !UserRoles.Contains("SysAdmin"))
            {
                return View("~/Views/Shared/InValidSettings.cshtml");
            }
            s.StockHeaderSettings = Mapper.Map<StockHeaderSettings, StockHeaderSettingsViewModel>(settings);

            ViewBag.Mode = "Edit";
            PrepareViewBag(s.DocTypeId);

            if (!(System.Web.HttpContext.Current.Request.UrlReferrer.PathAndQuery).Contains("Create"))
                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = s.DocTypeId,
                    DocId = s.StockHeaderId,
                    ActivityType = (int)ActivityTypeContants.Detail,
                    DocNo = s.DocNo,
                    DocDate = s.DocDate,
                    DocStatus = s.Status,
                }));


            return View("Create", s);
        }

        [HttpGet]
        public ActionResult DetailInformation(int id, string IndexType)
        {
            return RedirectToAction("Detail", new { id = id, transactionType = "detail", IndexType = IndexType });
        }

        [Authorize]
        [HttpGet]
        public ActionResult Detail(int id, string IndexType, string transactionType)
        {

            //Saving ViewBag Data::

            ViewBag.transactionType = transactionType;
            ViewBag.IndexStatus = IndexType;

            StockHeaderViewModel s = _StockHeaderService.GetStockHeader(id);

            //Job Order Settings
            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(s.DocTypeId, s.DivisionId, s.SiteId);

            if (settings == null)
            {
                return RedirectToAction("Create", "StockHeaderSettings", new { id = s.DocTypeId }).Warning("Please create Material Issue settings");
            }

            s.StockHeaderSettings = Mapper.Map<StockHeaderSettings, StockHeaderSettingsViewModel>(settings);

            PrepareViewBag(s.DocTypeId);
            if (s == null)
            {
                return HttpNotFound();
            }

            if (String.IsNullOrEmpty(transactionType) || transactionType == "detail")
                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = s.DocTypeId,
                    DocId = s.StockHeaderId,
                    ActivityType = (int)ActivityTypeContants.Detail,
                    DocNo = s.DocNo,
                    DocDate = s.DocDate,
                    DocStatus = s.Status,
                }));

            return View("Create", s);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            StockHeader header = _StockHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Drafted || header.Status == (int)StatusConstants.Import)
                return Remove(id);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult DeleteAfter_Submit(int id)
        {
            StockHeader header = _StockHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Submitted || header.Status == (int)StatusConstants.Modified)
                return Remove(id);
            else
                return HttpNotFound();
        }



        // GET: /PurchaseOrderHeader/Delete/5
        [HttpGet]
        private ActionResult Remove(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockHeader StockHeader = _StockHeaderService.Find(id);
            if (StockHeader == null)
            {
                return HttpNotFound();
            }

            if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, StockHeader.DocTypeId, StockHeader.ProcessId, this.ControllerContext.RouteData.Values["controller"].ToString(), "Remove") == false)
            {
                return PartialView("~/Views/Shared/PermissionDenied_Modal.cshtml").Warning("You don't have permission to do this task.");
            }

            #region DocTypeTimeLineValidation
            try
            {
                TimePlanValidation = DocumentValidation.ValidateDocument(Mapper.Map<DocumentUniqueId>(StockHeader), DocumentTimePlanTypeConstants.Delete, User.Identity.Name, out ExceptionMsg, out Continue);
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

            ReasonViewModel rvm = new ReasonViewModel()
            {
                id = id,
            };
            return PartialView("_Reason", rvm);
        }



        // POST: /PurchaseOrderHeader/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(ReasonViewModel vm)
        {
            bool BeforeSave = true;
            try
            {
                BeforeSave = StockIssueDocEvents.beforeHeaderDeleteEvent(this, new StockEventArgs(vm.id), ref context);
            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXC"] += message;
                EventException = true;
            }

            if (!BeforeSave)
                TempData["CSEXC"] += "Failed validation before delete";

            StockHeader StockHeader = (from p in context.StockHeader
                                       where p.StockHeaderId == vm.id
                                       select p).FirstOrDefault();

            var GatePassHeader = (from p in context.GatePassHeader
                                  where p.GatePassHeaderId == StockHeader.GatePassHeaderId
                                  select p).FirstOrDefault();

            if (GatePassHeader != null && GatePassHeader.Status == (int)StatusConstants.Submitted)
            {
                BeforeSave = false;
                TempData["CSEXC"] += "Cannot delete record because gatepass is submitted.";
            }


            if (ModelState.IsValid && BeforeSave && !EventException)
            {
                List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();

                try
                {
                    StockIssueDocEvents.onHeaderDeleteEvent(this, new StockEventArgs(vm.id), ref context);
                }
                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    TempData["CSEXC"] += message;
                    EventException = true;
                }


                StockHeader ExRec = new StockHeader();
                ExRec = Mapper.Map<StockHeader>(StockHeader);
                StockHeader Rec = new StockHeader();

                LogList.Add(new LogTypeViewModel
                {
                    ExObj = ExRec,
                    Obj = Rec,
                });

                //Then find all the Purchase Order Header Line associated with the above ProductType.
                //var StockLine = new StockLineService(_unitOfWork).GetStockLineforDelete(vm.id);

                var StockLine = (from p in context.StockLine
                                 where p.StockHeaderId == vm.id
                                 select p).ToList();

                var ProductUids = StockLine.Select(m => m.ProductUidId).ToArray();

                var ProdUidRecords = (from p in context.ProductUid
                                      where ProductUids.Contains(p.ProductUIDId)
                                      select p).ToList();


                List<int> StockIdList = new List<int>();
                List<int> StockProcessIdList = new List<int>();

                var StockProcessIds = (from p in context.StockProcess
                                       where p.StockHeaderId == vm.id
                                       select p).ToList();

                new RequisitionLineStatusService(_unitOfWork).DeleteRequisitionQtyOnIssueMultiple(StockHeader.StockHeaderId, ref context);

                //Mark ObjectState.Delete to all the Purchase Order Lines. 
                foreach (var item in StockLine)
                {
                    StockLine ExRecLine = new StockLine();
                    ExRecLine = Mapper.Map<StockLine>(item);
                    StockLine RecLine = new StockLine();

                    LogList.Add(new LogTypeViewModel
                    {
                        ExObj = ExRecLine,
                        Obj = RecLine,
                    });

                    if (item.StockId != null)
                    {
                        StockIdList.Add((int)item.StockId);
                    }

                    if (item.StockProcessId != null)
                    {
                        StockProcessIdList.Add((int)item.StockProcessId);
                    }

                    if (item.ProductUidId != null && item.ProductUidId > 0)
                    {
                        ProductUid ProductUid = ProdUidRecords.Where(m => m.ProductUIDId == item.ProductUidId).FirstOrDefault();

                        ProductUid.LastTransactionDocDate = item.ProductUidLastTransactionDocDate;
                        ProductUid.LastTransactionDocId = item.ProductUidLastTransactionDocId;
                        ProductUid.LastTransactionDocNo = item.ProductUidLastTransactionDocNo;
                        ProductUid.LastTransactionDocTypeId = item.ProductUidLastTransactionDocTypeId;
                        ProductUid.LastTransactionPersonId = item.ProductUidLastTransactionPersonId;
                        ProductUid.CurrenctGodownId = item.ProductUidCurrentGodownId;
                        ProductUid.CurrenctProcessId = item.ProductUidCurrentProcessId;
                        ProductUid.Status = item.ProductUidStatus;
                        ProductUid.ObjectState = Model.ObjectState.Modified;
                        context.ProductUid.Add(ProductUid);
                    }

                    item.ObjectState = Model.ObjectState.Deleted;
                    context.StockLine.Remove(item);
                }

                foreach (var item in StockProcessIds)
                {
                    StockProcessIdList.Add((int)item.StockProcessId);
                }

                foreach (var item in StockIdList)
                {
                    StockAdj Adj = (from L in context.StockAdj
                                    where L.StockOutId == item
                                    select L).FirstOrDefault();

                    if (Adj != null)
                    {
                        Adj.ObjectState = Model.ObjectState.Deleted;
                        context.StockAdj.Remove(Adj);
                    }

                    new StockService(_unitOfWork).DeleteStockDB(item, ref context, true);
                }

                foreach (var item in StockProcessIdList)
                {
                    new StockProcessService(_unitOfWork).DeleteStockProcessDB(item, ref context, true);
                }

                var GatePassHeaderId = StockHeader.GatePassHeaderId;

                StockHeader.ObjectState = Model.ObjectState.Deleted;
                context.StockHeader.Remove(StockHeader);

                if (GatePassHeaderId.HasValue)
                {

                    var GatePassLines = (from p in context.GatePassLine
                                         where p.GatePassHeaderId == GatePassHeaderId
                                         select p).ToList();

                    foreach (var item in GatePassLines)
                    {
                        item.ObjectState = Model.ObjectState.Deleted;
                        context.GatePassLine.Remove(item);
                    }

                    GatePassHeader.ObjectState = Model.ObjectState.Deleted;

                    context.GatePassHeader.Remove(GatePassHeader);


                }

                XElement Modifications = new ModificationsCheckService().CheckChanges(LogList);

                //Commit the DB
                try
                {
                    if (EventException)
                        throw new Exception();
                    context.SaveChanges();
                }

                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    TempData["CSEXC"] += message;
                    return PartialView("_Reason", vm);
                }

                try
                {
                    StockIssueDocEvents.afterHeaderDeleteEvent(this, new StockEventArgs(vm.id), ref context);
                }
                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    TempData["CSEXC"] += message;
                }

                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = StockHeader.DocTypeId,
                    DocId = StockHeader.StockHeaderId,
                    ActivityType = (int)ActivityTypeContants.Deleted,
                    UserRemark = vm.Reason,
                    DocNo = StockHeader.DocNo,
                    xEModifications = Modifications,
                    DocDate = StockHeader.DocDate,
                    DocStatus = StockHeader.Status,
                }));


                return Json(new { success = true });
            }
            return PartialView("_Reason", vm);
        }

        

        
        public ActionResult Submit(int id, string IndexType, string TransactionType)
        {
            StockHeader s = context.StockHeader.Find(id);
            if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, s.DocTypeId, s.ProcessId, this.ControllerContext.RouteData.Values["controller"].ToString(), "Submit") == false)
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


            bool BeforeSave = true;            
            try
            {
                BeforeSave = StockIssueDocEvents.beforeHeaderSubmitEvent(this, new StockEventArgs(Id), ref context);
            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXC"] += message;
                EventException = true;
            }

            if (!BeforeSave)
                TempData["CSEXC"] += "Falied validation before submit.";

            StockHeader pd = new StockHeaderService(_unitOfWork).Find(Id);

            if (ModelState.IsValid && BeforeSave && !EventException)
            {
                if (User.Identity.Name == pd.ModifiedBy || UserRoles.Contains("Admin"))
                {
                    int ActivityType;

                    pd.Status = (int)StatusConstants.Submitted;
                    ActivityType = (int)ActivityTypeContants.Submitted;

                    StockHeaderSettings Settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(pd.DocTypeId, pd.DivisionId, pd.SiteId);

                    if (!string.IsNullOrEmpty(GenGatePass) && GenGatePass == "true")
                    {

                        if (!String.IsNullOrEmpty(Settings.SqlProcGatePass))
                        {

                            SqlParameter SqlParameterUserId = new SqlParameter("@Id", Id);
                            IEnumerable<GatePassGeneratedViewModel> GatePasses = context.Database.SqlQuery<GatePassGeneratedViewModel>(Settings.SqlProcGatePass + " @Id", SqlParameterUserId).ToList();

                            if (pd.PersonId != null)
                            {
                                if (pd.GatePassHeaderId == null)
                                {
                                    int g= new DocumentTypeService(_unitOfWork).FindByName(TransactionDocCategoryConstants.GatePass).DocumentTypeId;
                                    SqlParameter DocDate = new SqlParameter("@DocDate", pd.DocDate);
                                    DocDate.SqlDbType = SqlDbType.DateTime;
                                    SqlParameter Godown = new SqlParameter("@GodownId", pd.GodownId);
                                    SqlParameter DocType = new SqlParameter("@DocTypeId", new DocumentTypeService(_unitOfWork).FindByName(TransactionDocCategoryConstants.GatePass).DocumentTypeId);
                                    GatePassHeader GPHeader = new GatePassHeader();
                                    GPHeader.CreatedBy = User.Identity.Name;
                                    GPHeader.CreatedDate = DateTime.Now;
                                    GPHeader.DivisionId = pd.DivisionId;
                                    GPHeader.DocDate = pd.DocDate;
                                    GPHeader.DocNo = context.Database.SqlQuery<string>("Web.GetNewDocNoGatePass @DocTypeId, @DocDate, @GodownId ", DocType, DocDate, Godown).FirstOrDefault();
                                    GPHeader.DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(TransactionDocCategoryConstants.GatePass).DocumentTypeId;
                                    GPHeader.ModifiedBy = User.Identity.Name;
                                    GPHeader.ModifiedDate = DateTime.Now;
                                    GPHeader.Remark = pd.Remark;
                                    GPHeader.PersonId = (int)pd.PersonId;
                                    GPHeader.SiteId = pd.SiteId;
                                    GPHeader.GodownId = (int)pd.GodownId;
                                    GPHeader.ReferenceDocTypeId = pd.DocTypeId;
                                    GPHeader.ReferenceDocId = pd.StockHeaderId;
                                    GPHeader.ReferenceDocNo = pd.DocNo;
                                    GPHeader.ObjectState = Model.ObjectState.Added;
                                    context.GatePassHeader.Add(GPHeader);

                                    //new GatePassHeaderService(_unitOfWork).Create(GPHeader);


                                    foreach (GatePassGeneratedViewModel item in GatePasses)
                                    {
                                        GatePassLine Gline = new GatePassLine();
                                        Gline.CreatedBy = User.Identity.Name;
                                        Gline.CreatedDate = DateTime.Now;
                                        Gline.GatePassHeaderId = GPHeader.GatePassHeaderId;
                                        Gline.ModifiedBy = User.Identity.Name;
                                        Gline.ModifiedDate = DateTime.Now;
                                        Gline.Product = item.ProductName;
                                        Gline.Qty = item.Qty;
                                        Gline.Specification = item.Specification;
                                        Gline.UnitId = item.UnitId;

                                        //new GatePassLineService(_unitOfWork).Create(Gline);
                                        Gline.ObjectState = Model.ObjectState.Added;
                                        context.GatePassLine.Add(Gline);
                                    }

                                    pd.GatePassHeaderId = GPHeader.GatePassHeaderId;

                                }
                                else
                                {
                                    //List<GatePassLine> LineList = new GatePassLineService(_unitOfWork).GetGatePassLineList(pd.GatePassHeaderId ?? 0).ToList();

                                    var LineList = (from p in context.GatePassLine
                                                    where p.GatePassHeaderId == pd.GatePassHeaderId
                                                    select p).ToList();

                                    foreach (var ittem in LineList)
                                    {
                                        ittem.ObjectState = Model.ObjectState.Deleted;
                                        context.GatePassLine.Remove(ittem);
                                        //new GatePassLineService(_unitOfWork).Delete(ittem);
                                    }

                                    GatePassHeader GPHeader = new GatePassHeaderService(_unitOfWork).Find(pd.GatePassHeaderId ?? 0);

                                    foreach (GatePassGeneratedViewModel item in GatePasses)
                                    {
                                        GatePassLine Gline = new GatePassLine();
                                        Gline.CreatedBy = User.Identity.Name;
                                        Gline.CreatedDate = DateTime.Now;
                                        Gline.GatePassHeaderId = GPHeader.GatePassHeaderId;
                                        Gline.ModifiedBy = User.Identity.Name;
                                        Gline.ModifiedDate = DateTime.Now;
                                        Gline.Product = item.ProductName;
                                        Gline.Qty = item.Qty;
                                        Gline.Specification = item.Specification;
                                        Gline.UnitId = item.UnitId;
                                        Gline.ObjectState = Model.ObjectState.Added;
                                        context.GatePassLine.Add(Gline);

                                        //new GatePassLineService(_unitOfWork).Create(Gline);
                                    }
                                    pd.GatePassHeaderId = GPHeader.GatePassHeaderId;
                                }
                            }
                        }

                    }





                    if (!String.IsNullOrEmpty(Settings.SqlProcStockProcessPost))
                    {
                        string ConnectionString = (string)System.Web.HttpContext.Current.Session["DefaultConnectionString"];
                        using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                        {
                            sqlConnection.Open();

                            using (SqlCommand cmd = new SqlCommand("" + Settings.SqlProcStockProcessPost))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Connection = sqlConnection;
                                cmd.Parameters.AddWithValue("@StockHeaderId", pd.StockHeaderId);
                                cmd.CommandTimeout = 1000;
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    pd.ReviewBy = null;
                    pd.ObjectState = Model.ObjectState.Modified;

                    context.StockHeader.Add(pd);
                    //_StockHeaderService.Update(pd);

                    //_unitOfWork.Save();

                    try
                    {
                        StockIssueDocEvents.onHeaderSubmitEvent(this, new StockEventArgs(Id), ref context);
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                        EventException = true;
                    }

                    try
                    {
                        if (EventException)
                        { throw new Exception(); }

                        context.SaveChanges();
                        //_unitOfWork.Save();
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                        return RedirectToAction("Index", new { id = pd.DocTypeId, IndexType = IndexType });
                    }


                    try
                    {
                        StockIssueDocEvents.afterHeaderSubmitEvent(this, new StockEventArgs(Id), ref context);
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                    }

                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = pd.DocTypeId,
                        DocId = pd.StockHeaderId,
                        ActivityType = ActivityType,
                        UserRemark = UserRemark,
                        DocNo = pd.DocNo,
                        DocDate = pd.DocDate,
                        DocStatus = pd.Status,
                    }));

                    return RedirectToAction("Index", new { id = pd.DocTypeId, IndexType = IndexType }).Success("Record Submitted Successfully");
                }
                else
                    return RedirectToAction("Index", new { id = pd.DocTypeId, IndexType = IndexType }).Warning("Record can be submitted by user " + pd.ModifiedBy + " only.");
            }


            return RedirectToAction("Index", new { id = pd.DocTypeId, IndexType = IndexType });
        }


        [HttpGet]
        public ActionResult Print(int id)
        {
            StockHeader s = _StockHeaderService.Find(id);
            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(s.DocTypeId, s.DivisionId, s.SiteId);
            String query = settings.SqlProcDocumentPrint;
            return Redirect((string)System.Configuration.ConfigurationManager.AppSettings["JobsDomain"] + "/Report_DocumentPrint/DocumentPrint/?DocumentId=" + id + "&queryString=" + query);

        }


        [HttpGet]
        public ActionResult Report(int id)
        {
            DocumentType Dt = new DocumentType();
            Dt = new DocumentTypeService(_unitOfWork).Find(id);

            Dictionary<int, string> DefaultValue = new Dictionary<int, string>();

            //if (!Dt.ReportMenuId.HasValue)
            //    throw new Exception("Report Menu not configured in document types");
            if (!Dt.ReportMenuId.HasValue)
                return Redirect((string)System.Configuration.ConfigurationManager.AppSettings["JobsDomain"] + "/GridReport/GridReportLayout/?MenuName=Store Issue Report&DocTypeId=" + id.ToString());


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

            }

            TempData["ReportLayoutDefaultValues"] = DefaultValue;

            return Redirect((string)System.Configuration.ConfigurationManager.AppSettings["JobsDomain"] + "/Report_ReportPrint/ReportPrint/?MenuId=" + Dt.ReportMenuId);

        }


        public ActionResult Review(int id, string IndexType, string TransactionType)
        {
            return RedirectToAction("Detail", new { id = id, IndexType = IndexType, transactionType = string.IsNullOrEmpty(TransactionType) ? "review" : TransactionType });
        }


        [HttpPost, ActionName("Detail")]
        [MultipleButton(Name = "Command", Argument = "Review")]
        public ActionResult Reviewed(int Id, string IndexType, string UserRemark, string IsContinue)
        {
            bool BeforeSave = true;
            try
            {
                BeforeSave = StockIssueDocEvents.beforeHeaderReviewEvent(this, new StockEventArgs(Id), ref context);
            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXC"] += message;
            }

            if (!BeforeSave)
                TempData["CSEXC"] += "Falied validation before Review.";

            StockHeader pd = new StockHeaderService(_unitOfWork).Find(Id);

            if (ModelState.IsValid && BeforeSave)
            {

                pd.ReviewCount = (pd.ReviewCount ?? 0) + 1;
                pd.ReviewBy += User.Identity.Name + ", ";

                pd.ObjectState = Model.ObjectState.Modified;
                context.StockHeader.Add(pd);

                try
                {
                    StockIssueDocEvents.onHeaderReviewEvent(this, new StockEventArgs(Id), ref context);
                }
                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    TempData["CSEXC"] += message;
                }

                context.SaveChanges();

                try
                {
                    StockIssueDocEvents.afterHeaderReviewEvent(this, new StockEventArgs(Id), ref context);
                }
                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    TempData["CSEXC"] += message;
                }

                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = pd.DocTypeId,
                    DocId = pd.StockHeaderId,
                    ActivityType = (int)ActivityTypeContants.Reviewed,
                    UserRemark = UserRemark,
                    DocNo = pd.DocNo,
                    DocDate = pd.DocDate,
                    DocStatus = pd.Status,
                }));

                //SendEmail_POReviewd(Id);
                return RedirectToAction("Index", new { id = pd.DocTypeId, IndexType = IndexType });
            }

            return RedirectToAction("Index", new { id = pd.DocTypeId, IndexType = IndexType }).Warning("Error in Reviewing.");
        }


        public ActionResult Import(int id)//Document Type Id
        {
            //ControllerAction ca = new ControllerActionService(_unitOfWork).Find(id);
            StockHeaderViewModel vm = new StockHeaderViewModel();

            vm.DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            vm.SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(id, vm.DivisionId, vm.SiteId);

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
                        if (menuviewmodel.AreaName != null && menuviewmodel.AreaName != "")
                        {
                            return Redirect(System.Configuration.ConfigurationManager.AppSettings[menuviewmodel.URL] + "/" + menuviewmodel.AreaName + "/" + menuviewmodel.ControllerName + "/" + menuviewmodel.ActionName + "/" + id + "?MenuId=" + menuviewmodel.MenuId);
                        }
                        else
                        {
                            return Redirect(System.Configuration.ConfigurationManager.AppSettings[menuviewmodel.URL] + "/" + menuviewmodel.ControllerName + "/" + menuviewmodel.ActionName + "/" + id + "?MenuId=" + menuviewmodel.MenuId);
                        }
                    }
                    else
                    {
                        return RedirectToAction(menuviewmodel.ActionName, menuviewmodel.ControllerName, new { MenuId = menuviewmodel.MenuId, id = id });
                    }
                }
            }
            return RedirectToAction("Index", new { id = id });
        }

        public int PendingToSubmitCount(int id)
        {
            return (_StockHeaderService.GetStockHeaderListPendingToSubmit(id, User.Identity.Name)).Count();
        }

        public int PendingToReviewCount(int id)
        {
            return (_StockHeaderService.GetStockHeaderListPendingToReview(id, User.Identity.Name)).Count();
        }

        [HttpGet]
        public ActionResult NextPage(int DocId, int DocTypeId)//CurrentHeaderId
        {
            var nextId = new NextPrevIdService(_unitOfWork).GetNextPrevId(DocId, DocTypeId, User.Identity.Name, "", "Web.StockHeaders", "StockHeaderId", PrevNextConstants.Next);
            return Edit(nextId, "");
        }
        [HttpGet]
        public ActionResult PrevPage(int DocId, int DocTypeId)//CurrentHeaderId
        {
            var PrevId = new NextPrevIdService(_unitOfWork).GetNextPrevId(DocId, DocTypeId, User.Identity.Name, "", "Web.StockHeaders", "StockHeaderId", PrevNextConstants.Prev);
            return Edit(PrevId, "");
        }



        public ActionResult GenerateGatePass(string Ids, int DocTypeId)
        {

            if (!string.IsNullOrEmpty(Ids))
            {
                int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
                int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
                int PK = 0;

                var Settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(DocTypeId, DivisionId, SiteId);
                var GatePassDocTypeID = new DocumentTypeService(_unitOfWork).FindByName(TransactionDocCategoryConstants.GatePass).DocumentTypeId;
                string StockHeaderIds = "";

                try
                {
                    if (!string.IsNullOrEmpty(Settings.SqlProcGatePass))
                        foreach (var item in Ids.Split(',').Select(Int32.Parse))
                        {
                            TimePlanValidation = true;

                            var pd = context.StockHeader.Find(item);

                            #region DocTypeTimeLineValidation
                            try
                            {

                                TimePlanValidation = DocumentValidation.ValidateDocument(Mapper.Map<DocumentUniqueId>(pd), DocumentTimePlanTypeConstants.GatePassCreate, User.Identity.Name, out ExceptionMsg, out Continue);

                            }
                            catch (Exception ex)
                            {
                                string message = _exception.HandleException(ex);
                                TempData["CSEXC"] += message;
                                TimePlanValidation = false;
                            }
                            #endregion

                            if (!pd.GatePassHeaderId.HasValue)
                            {
                                if ((TimePlanValidation || Continue))
                                {
                                    if ((pd.Status == (int)StatusConstants.Submitted) && !pd.GatePassHeaderId.HasValue)
                                    {

                                        SqlParameter SqlParameterUserId = new SqlParameter("@Id", item);
                                        IEnumerable<GatePassGeneratedViewModel> GatePasses = context.Database.SqlQuery<GatePassGeneratedViewModel>(Settings.SqlProcGatePass + " @Id", SqlParameterUserId).ToList();

                                        if (pd.PersonId != null)
                                        {
                                            if (pd.GatePassHeaderId == null)
                                            {
                                                SqlParameter DocDate = new SqlParameter("@DocDate", DateTime.Now.Date);
                                                DocDate.SqlDbType = SqlDbType.DateTime;
                                                SqlParameter Godown = new SqlParameter("@GodownId", pd.GodownId);
                                                SqlParameter DocType = new SqlParameter("@DocTypeId", GatePassDocTypeID);
                                                GatePassHeader GPHeader = new GatePassHeader();
                                                GPHeader.CreatedBy = User.Identity.Name;
                                                GPHeader.CreatedDate = DateTime.Now;
                                                GPHeader.DivisionId = pd.DivisionId;
                                                GPHeader.DocDate = DateTime.Now.Date;
                                                GPHeader.DocNo = context.Database.SqlQuery<string>("Web.GetNewDocNoGatePass @DocTypeId, @DocDate, @GodownId ", DocType, DocDate, Godown).FirstOrDefault();
                                                GPHeader.DocTypeId = GatePassDocTypeID;
                                                GPHeader.ModifiedBy = User.Identity.Name;
                                                GPHeader.ModifiedDate = DateTime.Now;
                                                GPHeader.Remark = pd.Remark;
                                                GPHeader.PersonId = (int)pd.PersonId;
                                                GPHeader.SiteId = pd.SiteId;
                                                GPHeader.GodownId = (int)pd.GodownId;
                                                GPHeader.ReferenceDocTypeId = pd.DocTypeId;
                                                GPHeader.ReferenceDocId = pd.StockHeaderId;
                                                GPHeader.ReferenceDocNo = pd.DocNo;
                                                GPHeader.GatePassHeaderId = PK++;
                                                GPHeader.ObjectState = Model.ObjectState.Added;
                                                context.GatePassHeader.Add(GPHeader);

                                                //new GatePassHeaderService(_unitOfWork).Create(GPHeader);


                                                foreach (GatePassGeneratedViewModel GPLine in GatePasses)
                                                {
                                                    GatePassLine Gline = new GatePassLine();
                                                    Gline.CreatedBy = User.Identity.Name;
                                                    Gline.CreatedDate = DateTime.Now;
                                                    Gline.GatePassHeaderId = GPHeader.GatePassHeaderId;
                                                    Gline.ModifiedBy = User.Identity.Name;
                                                    Gline.ModifiedDate = DateTime.Now;
                                                    Gline.Product = GPLine.ProductName;
                                                    Gline.Qty = GPLine.Qty;
                                                    Gline.Specification = GPLine.Specification;
                                                    Gline.UnitId = GPLine.UnitId;

                                                    //new GatePassLineService(_unitOfWork).Create(Gline);
                                                    Gline.ObjectState = Model.ObjectState.Added;
                                                    context.GatePassLine.Add(Gline);
                                                }

                                                pd.GatePassHeaderId = GPHeader.GatePassHeaderId;


                                                pd.ObjectState = Model.ObjectState.Modified;
                                                context.StockHeader.Add(pd);

                                                StockHeaderIds += pd.StockHeaderId + ", ";
                                            }
                                            //else
                                            //{
                                            //    //List<GatePassLine> LineList = new GatePassLineService(_unitOfWork).GetGatePassLineList(pd.GatePassHeaderId ?? 0).ToList();

                                            //    var LineList = (from p in context.GatePassLine
                                            //                    where p.GatePassHeaderId == pd.GatePassHeaderId
                                            //                    select p).ToList();

                                            //    foreach (var ittem in LineList)
                                            //    {
                                            //        ittem.ObjectState = Model.ObjectState.Deleted;
                                            //        context.GatePassLine.Remove(ittem);
                                            //        //new GatePassLineService(_unitOfWork).Delete(ittem);
                                            //    }

                                            //    GatePassHeader GPHeader = new GatePassHeaderService(_unitOfWork).Find(pd.GatePassHeaderId ?? 0);

                                            //    foreach (GatePassGeneratedViewModel GPLine in GatePasses)
                                            //    {
                                            //        GatePassLine Gline = new GatePassLine();
                                            //        Gline.CreatedBy = User.Identity.Name;
                                            //        Gline.CreatedDate = DateTime.Now;
                                            //        Gline.GatePassHeaderId = GPHeader.GatePassHeaderId;
                                            //        Gline.ModifiedBy = User.Identity.Name;
                                            //        Gline.ModifiedDate = DateTime.Now;
                                            //        Gline.Product = GPLine.ProductName;
                                            //        Gline.Qty = GPLine.Qty;
                                            //        Gline.Specification = GPLine.Specification;
                                            //        Gline.UnitId = GPLine.UnitId;
                                            //        Gline.ObjectState = Model.ObjectState.Added;
                                            //        context.GatePassLine.Add(Gline);

                                            //        //new GatePassLineService(_unitOfWork).Create(Gline);
                                            //    }
                                            //    pd.GatePassHeaderId = GPHeader.GatePassHeaderId;
                                            //}
                                        }
                                        context.SaveChanges();
                                    }
                                }
                                else
                                    TempData["CSEXC"] += ExceptionMsg;
                            }

                        }


                    //_unitOfWork.Save();
                }

                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    return Json(new { success = "Error", data = message }, JsonRequestBehavior.AllowGet);
                }

                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = GatePassDocTypeID,
                    ActivityType = (int)ActivityTypeContants.Added,
                    Narration = "GatePass created for StockHeaders " + StockHeaderIds,
                }));

                return Json(new { success = "Success" }, JsonRequestBehavior.AllowGet).Success("Gate passes generated successfully");

            }
            return Json(new { success = "Error", data = "No Records Selected." }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult DeleteGatePass(int Id)
        {
            List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();
            if (Id > 0)
            {
                int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
                int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

                try
                {

                    var pd = context.StockHeader.Find(Id);

                    #region DocTypeTimeLineValidation
                    try
                    {

                        TimePlanValidation = DocumentValidation.ValidateDocument(Mapper.Map<DocumentUniqueId>(pd), DocumentTimePlanTypeConstants.GatePassCancel, User.Identity.Name, out ExceptionMsg, out Continue);

                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                        TimePlanValidation = false;
                    }

                    if (!TimePlanValidation && !Continue)
                        throw new Exception(ExceptionMsg);
                    #endregion

                    var GatePass = context.GatePassHeader.Find(pd.GatePassHeaderId);

                    if (GatePass.Status != (int)StatusConstants.Submitted)
                    {

                        pd.Status = (int)StatusConstants.Modified;
                        pd.GatePassHeaderId = null;
                        pd.ModifiedBy = User.Identity.Name;
                        pd.ModifiedDate = DateTime.Now;
                        pd.IsGatePassPrinted = false;
                        pd.ObjectState = Model.ObjectState.Modified;
                        context.StockHeader.Add(pd);


                        GatePass.Status = (int)StatusConstants.Cancel;
                        GatePass.ObjectState = Model.ObjectState.Modified;
                        context.GatePassHeader.Add(GatePass);

                        XElement Modifications = new ModificationsCheckService().CheckChanges(LogList);

                        context.SaveChanges();

                        LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                        {
                            DocTypeId = GatePass.DocTypeId,
                            DocId = GatePass.GatePassHeaderId,
                            ActivityType = (int)ActivityTypeContants.Deleted,
                            DocNo = GatePass.DocNo,
                            DocDate = GatePass.DocDate,
                            xEModifications = Modifications,
                            DocStatus = GatePass.Status,
                        }));

                    }
                    else
                        throw new Exception("Gatepass cannot be deleted because it is already submitted");
                }

                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    return Json(new { success = "Error", data = message }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = "Success" }, JsonRequestBehavior.AllowGet).Success("Gate pass Deleted successfully");

            }
            return Json(new { success = "Error", data = "No Records Selected." }, JsonRequestBehavior.AllowGet);

        }

        //public ActionResult GeneratePrints(string Ids, int DocTypeId)
        //{

        //    if (!string.IsNullOrEmpty(Ids))
        //    {
        //        int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
        //        int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

        //        var Settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(DocTypeId, DivisionId, SiteId);

        //        try
        //        {

        //            List<byte[]> PdfStream = new List<byte[]>();
        //            foreach (var item in Ids.Split(',').Select(Int32.Parse))
        //            {
        //                int Copies = 1;
        //                int AdditionalCopies = Settings.NoOfPrintCopies ?? 0;
        //                bool UpdateGatePassPrint = true;
        //                DirectReportPrint drp = new DirectReportPrint();

        //                var pd = context.StockHeader.Find(item);

        //                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
        //                {
        //                    DocTypeId = pd.DocTypeId,
        //                    DocId = pd.StockHeaderId,
        //                    ActivityType = (int)ActivityTypeContants.Print,
        //                    DocNo = pd.DocNo,
        //                    DocDate = pd.DocDate,
        //                    DocStatus = pd.Status,
        //                }));

        //                do
        //                {
        //                    byte[] Pdf;

        //                    if (pd.Status == (int)StatusConstants.Drafted || pd.Status == (int)StatusConstants.Modified || pd.Status == (int)StatusConstants.Import)
        //                    {
        //                        //LogAct(item.ToString());
        //                        Pdf = drp.DirectDocumentPrint(Settings.SqlProcDocumentPrint, User.Identity.Name, item);

        //                        PdfStream.Add(Pdf);
        //                    }
        //                    else if (pd.Status == (int)StatusConstants.Submitted || pd.Status == (int)StatusConstants.ModificationSubmitted)
        //                    {
        //                        Pdf = drp.DirectDocumentPrint(Settings.SqlProcDocumentPrint, User.Identity.Name, item);

        //                        PdfStream.Add(Pdf);
        //                    }
        //                    else
        //                    {
        //                        Pdf = drp.DirectDocumentPrint(Settings.SqlProcDocumentPrint, User.Identity.Name, item);
        //                        PdfStream.Add(Pdf);
        //                    }

        //                    if (UpdateGatePassPrint && !(pd.IsGatePassPrinted ?? false))
        //                    {
        //                        if (pd.GatePassHeaderId.HasValue)
        //                        {
        //                            pd.IsGatePassPrinted = true;
        //                            pd.ObjectState = Model.ObjectState.Modified;
        //                            context.StockHeader.Add(pd);
        //                            context.SaveChanges();

        //                            UpdateGatePassPrint = false;
        //                            Copies = AdditionalCopies;
        //                            if (Copies > 0)
        //                                continue;
        //                        }
        //                    }

        //                    Copies--;

        //                } while (Copies > 0);
        //            }

        //            PdfMerger pm = new PdfMerger();

        //            byte[] Merge = pm.MergeFiles(PdfStream);

        //            if (Merge != null)
        //                return File(Merge, "application/pdf");
        //        }

        //        catch (Exception ex)
        //        {
        //            string message = _exception.HandleException(ex);
        //            return Json(new { success = "Error", data = message }, JsonRequestBehavior.AllowGet);
        //        }



        //        return Json(new { success = "Success" }, JsonRequestBehavior.AllowGet);

        //    }
        //    return Json(new { success = "Error", data = "No Records Selected." }, JsonRequestBehavior.AllowGet);

        //}

        public ActionResult GeneratePrints(string Ids, int DocTypeId)
        {

            if (!string.IsNullOrEmpty(Ids))
            {
                int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
                int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

                var Settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(DocTypeId, DivisionId, SiteId);

                if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, DocTypeId, Settings.ProcessId, this.ControllerContext.RouteData.Values["controller"].ToString(), "GeneratePrints") == false)
                {
                    return View("~/Views/Shared/PermissionDenied.cshtml").Warning("You don't have permission to do this task.");
                }

                string ReportSql = "";

                if (Settings.DocumentPrintReportHeaderId.HasValue)
                    ReportSql = context.ReportHeader.Where((m) => m.ReportHeaderId == Settings.DocumentPrintReportHeaderId).FirstOrDefault().ReportSQL;

                try
                {

                    List<byte[]> PdfStream = new List<byte[]>();
                    foreach (var item in Ids.Split(',').Select(Int32.Parse))
                    {
                        int Copies = 1;
                        int AdditionalCopies = Settings.NoOfPrintCopies ?? 0;
                        bool UpdateGatePassPrint = true;
                        DirectReportPrint drp = new DirectReportPrint();

                        var pd = context.StockHeader.Find(item);

                        LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                        {
                            DocTypeId = pd.DocTypeId,
                            DocId = pd.StockHeaderId,
                            ActivityType = (int)ActivityTypeContants.Print,
                            DocNo = pd.DocNo,
                            DocDate = pd.DocDate,
                            DocStatus = pd.Status,
                        }));

                        do
                        {
                            byte[] Pdf;
                            if (!string.IsNullOrEmpty(ReportSql))
                            {
                                Pdf = drp.rsDirectDocumentPrint(ReportSql, User.Identity.Name, item);
                                PdfStream.Add(Pdf);
                            }
                            else
                            {

                                if (pd.Status == (int)StatusConstants.Drafted || pd.Status == (int)StatusConstants.Modified || pd.Status == (int)StatusConstants.Import)
                                {
                                    if (Settings.SqlProcDocumentPrint == null || Settings.SqlProcDocumentPrint == "")
                                    {
                                        StockHeaderRDL cr = new StockHeaderRDL();
                                        drp.CreateRDLFile("Std_StockIssue_Print", cr.Create_Std_StockIssue_Print_Lanscape());
                                        List<ListofQuery> QueryList = new List<ListofQuery>();
                                        QueryList = DocumentPrintData(item);
                                        Pdf = drp.DocumentPrint_New(QueryList, User.Identity.Name);
                                    }
                                    else
                                        Pdf = drp.DirectDocumentPrint(Settings.SqlProcDocumentPrint, User.Identity.Name, item);

                                    PdfStream.Add(Pdf);
                                }
                                else if (pd.Status == (int)StatusConstants.Submitted || pd.Status == (int)StatusConstants.ModificationSubmitted)
                                {
                                    if (Settings.SqlProcDocumentPrint == null || Settings.SqlProcDocumentPrint == "")
                                    {
                                        StockHeaderRDL cr = new StockHeaderRDL();
                                        drp.CreateRDLFile("Std_StockIssue_Print", cr.Create_Std_StockIssue_Print_Lanscape());
                                        List<ListofQuery> QueryList = new List<ListofQuery>();
                                        QueryList = DocumentPrintData(item);
                                        Pdf = drp.DocumentPrint_New(QueryList, User.Identity.Name);
                                    }
                                    else
                                        Pdf = drp.DirectDocumentPrint(Settings.SqlProcDocumentPrint, User.Identity.Name, item);

                                    PdfStream.Add(Pdf);
                                }
                                else
                                {
                                    if (Settings.SqlProcDocumentPrint == null || Settings.SqlProcDocumentPrint == "")
                                    {
                                        StockHeaderRDL cr = new StockHeaderRDL();
                                        drp.CreateRDLFile("Std_StockIssue_Print", cr.Create_Std_StockIssue_Print_Lanscape());
                                        List<ListofQuery> QueryList = new List<ListofQuery>();
                                        QueryList = DocumentPrintData(item);
                                        Pdf = drp.DocumentPrint_New(QueryList, User.Identity.Name);
                                    }
                                    else
                                        Pdf = drp.DirectDocumentPrint(Settings.SqlProcDocumentPrint, User.Identity.Name, item);
                                    PdfStream.Add(Pdf);
                                }
                            }

                            if (UpdateGatePassPrint && !(pd.IsGatePassPrinted ?? false))
                            {
                                if (pd.GatePassHeaderId.HasValue)
                                {
                                    pd.IsGatePassPrinted = true;
                                    pd.ObjectState = Model.ObjectState.Modified;
                                    context.StockHeader.Add(pd);
                                    context.SaveChanges();

                                    UpdateGatePassPrint = false;
                                    Copies = AdditionalCopies;
                                    if (Copies > 0)
                                        continue;
                                }
                            }

                            Copies--;

                        } while (Copies > 0);
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

        private List<ListofQuery> DocumentPrintData(int item)
        {
            StockHeader SH = new StockHeaderService(_unitOfWork).Find(item);

            List<ListofQuery> DocumentPrintData = new List<ListofQuery>();
            String QueryMain;

            QueryMain = @"DECLARE @CostCenterCnt INT =0
    SELECT @CostCenterCnt=sum(CASE WHEN CostCenterId IS NULL THEN 0 ELSE 1 END ) FROM Web.StockLines WHERE StockHeaderId=" + item + @"


	DECLARE @DocDate DATETIME
	SET @DocDate = (SELECT DocDate FROM Web.JobOrderHeaders WHERE JobOrderHeaderId=" + item + @") 
	
 
	
	SELECT 
	--Header Table Fields	
	H.StockHeaderId,H.DocTypeId,Dt.DocumentTypeShortName +'-'+ H.DocNo AS DocNo,DocIdCaption+' No' AS DocIdCaption ,
	H.SiteId,H.DivisionId,H.DocDate,DTS.DocIdCaption +' Date' AS DocIdCaptionDate,
	PS.ProcessName AS ProcessName, 	H.Remark,DT.DocumentTypeShortName,(CASE WHEN H.IsGatePassPrinted=1 THEN NULL ELSE  H.GatePassHeaderId END) as GatePassHeaderId,H.ModifiedBy +' ' + Replace(replace(convert(NVARCHAR, H.ModifiedDate, 106), ' ', '/'),'/20','/') + substring (convert(NVARCHAR,H.ModifiedDate),13,7) AS ModifiedBy,
	H.ModifiedDate,(CASE WHEN Isnull(H.Status,0)=0 OR Isnull(H.Status,0)=8 THEN 0 ELSE 1 END)  AS Status,
   	(CASE WHEN SPR.[Party GST NO] IS NULL THEN 'Yes' ELSE 'No' END ) AS ReverseCharge,
	VDC.CompanyName,
	--Godown Detail
	G.GodownName,
	F.GodownName AS FromGodownName,
	--Person Detail
	P.Name AS PartyName, DTS.PartyCaption AS  PartyCaption, P.Suffix AS PartySuffix,	
	isnull(PA.Address,'')+' '+isnull(C.CityName,'')+','+isnull(PA.ZipCode,'')+(CASE WHEN isnull(CS.StateName,'') <> isnull(S.StateName,'') AND SPR.[Party GST NO] IS NOT NULL THEN ',State : '+isnull(S.StateName,'')+(CASE WHEN S.StateCode IS NULL THEN '' ELSE ', Code : '+S.StateCode END)    ELSE '' END ) AS PartyAddress,
	isnull(S.StateName,'') AS PartyStateName,isnull(S.StateCode,'') AS PartyStateCode,		
	P.Mobile AS PartyMobileNo,	SPR.*,	
	--Caption Fields	
	DTS.SignatoryMiddleCaption,DTS.SignatoryRightCaption,
	--Line Table
	PD.ProductName,DTS.ProductCaption,U.UnitName,U.DecimalPlaces,
	isnull(L.Qty,0) AS Qty,isnull(L.Rate,0) AS Rate,isnull(L.Amount,0) AS Amount,
	D1.Dimension1Name,DTS.Dimension1Caption,D2.Dimension2Name,DTS.Dimension2Caption,D3.Dimension3Name,DTS.Dimension3Caption,D4.Dimension4Name,DTS.Dimension4Caption,
	L.LotNo AS LotNo,(CASE WHEN DTS.PrintSpecification >0 THEN   L.Specification ELSE '' END)  AS Specification,DTS.SpecificationCaption,DTS.SignatoryleftCaption,L.Remark AS LineRemark,
   	--STC.Code AS SalesTaxProductCodes,
   	--(CASE WHEN H.ProcessId IN (26,28) THEN  STC.Code ELSE PSSTC.Code END) AS SalesTaxProductCodes ,
   	--SDS.SalesTaxProductCodeCaption,
    STC.Code AS SalesTaxProductCodes ,
   	(SELECT TOP 1 SalesTaxProductCodeCaption FROM web.SiteDivisionSettings WHERE H.DocDate BETWEEN StartDate AND IsNull(EndDate,getdate()) AND SiteId=H.SiteId AND DivisionId=H.DivisionId)  AS SalesTaxProductCodeCaption,
	(CASE WHEN DTS.PrintProductGroup >0 THEN isnull(PG.ProductGroupName,'') ELSE '' END)+(CASE WHEN DTS.PrintProductdescription >0 THEN isnull(','+PD.Productdescription,'') ELSE '' END) AS ProductGroupName,
	DTS.ProductGroupCaption,  PU.ProductUidName,	DTS.ProductUidCaption,	Cost.CostCenterName,
	DTS.CostCenterCaption,	isnull(CGPD.PrintingDescription,CGPD.ChargeGroupProductName) AS ChargeGroupProductName,
	T.Name TransportName, SHT.VehicleNo, SHT.LrNo, SHT.LrDate, SHT.EWayBillNo, SHT.EWayBillDate, SHT.PrivateMark, SHT.Weight, SHT.Freight, SHT.PaymentType, SHT.Destination, SHT.DescriptionOfGoods, SHT.DescriptionOfPacking, SHT.ChargedWeight,
	--SalesTaxGroupPersonId
	--CGP.ChargeGroupPersonName,
	--Other Fields
   		@CostCenterCnt AS CostCenterCnt,
	Null SubReportProcList,
	(CASE WHEN Isnull(H.Status,0)=0 OR Isnull(H.Status,0)=8 THEN 'Provisional ' +isnull(DT.PrintTitle,DT.DocumentTypeName) ELSE isnull(PrintTitle,DT.DocumentTypeName) END) AS ReportTitle, 
	CASE WHEN HS.isVisibleRate =1 THEN  'StdDocPrint_StockIssueWithRate.rdl' ELSE 'Std_StockIssue_Print.rdl' END AS ReportName,									
	SalesTaxGroupProductCaption	
	FROM Web.StockHeaders H WITH (Nolock)
	LEFT JOIN web.StockHeaderTransport SHT WITH (Nolock) ON SHT.StockHeaderId = H.StockHeaderId
	LEFT JOIN web.People T WITH (Nolock) ON T.PersonID = SHT.TransportId
    LEFT JOIN web.StockHeaderSettings HS WITH (Nolock) ON HS.DocTypeId=H.DocTypeId AND HS.SiteId = H.SiteId AND HS.DivisionId = H.DivisionId
	LEFT JOIN web.DocumentTypes DT WITH (Nolock) ON DT.DocumentTypeId=H.DocTypeId
	LEFT JOIN Web._DocumentTypeSettings DTS WITH (Nolock) ON DTS.DocumentTypeId=DT.DocumentTypeId	
	LEFT JOIN web.ViewDivisionCompany VDC WITH (Nolock) ON VDC.DivisionId=H.DivisionId
	LEFT JOIN Web.Sites SI WITH (Nolock) ON SI.SiteId=H.SiteId
	LEFT JOIN Web.Divisions DIV WITH (Nolock) ON DIV.DivisionId=H.DivisionId	
	LEFT JOIN Web.Companies Com ON Com.CompanyId = DIV.CompanyId
	LEFT JOIN Web.Cities CC WITH (Nolock) ON CC.CityId=Com.CityId
	LEFT JOIN Web.States CS WITH (Nolock) ON CS.StateId=CC.StateId
	LEFT JOIN Web.Processes PS WITH (Nolock) ON PS.ProcessId=H.ProcessId  	
    LEFT JOIN Web.SalesTaxProductCodes PSSTC WITH (Nolock) ON PSSTC.SalesTaxProductCodeId=PS.SalesTaxProductCodeId	
	LEFT JOIN Web.People P WITH (Nolock) ON P.PersonID=H.PersonId
	LEFT JOIN Web.Std_PersonRegistrations SPR WITH (Nolock) ON SPR.CustomerId=H.PersonId
	LEFT JOIN web.Godowns G WITH (Nolock) ON G.GodownId=H.GodownId
	LEFT JOIN web.Godowns F WITH (Nolock) ON F.GodownId=H.FromGodownId
	LEFT JOIN (SELECT TOP 1 * FROM web.SiteDivisionSettings WHERE @DocDate BETWEEN StartDate AND IsNull(EndDate,getdate()) ORDER BY StartDate) SDS  ON H.DivisionId = SDS.DivisionId AND  H.SiteId = SDS.SiteId
    LEFT JOIN (SELECT * FROM Web.PersonAddresses WITH (nolock) WHERE AddressType IS NULL) PA  ON PA.PersonId = P.PersonID 
	LEFT JOIN Web.Cities C WITH (nolock) ON C.CityId = PA.CityId
	LEFT JOIN Web.States S WITH (Nolock) ON S.StateId=C.StateId
	LEFT JOIN Web.Stocklines L WITH (Nolock) ON L.StockHeaderId=H.StockHeaderId
	LEFT JOIN Web.ProductUids PU WITH (Nolock) ON PU.ProductUIDId=L.ProductUidId
	LEFT JOIN Web.CostCenters COST WITH (nolock) ON COST.CostCenterId=L.CostCenterId
	LEFT JOIN web.Products PD WITH (Nolock) ON PD.ProductId=L.ProductId
	LEFT JOIN web.ProductGroups PG WITH (Nolock) ON PG.ProductGroupId=PD.ProductGroupid
	LEFT JOIN Web.SalesTaxProductCodes STC WITH (Nolock) ON STC.SalesTaxProductCodeId= IsNull(PD.SalesTaxProductCodeId, Pg.DefaultSalesTaxProductCodeId)
	LEFT JOIN Web.Dimension1 D1 WITH (Nolock) ON D1.Dimension1Id=L.Dimension1Id
	LEFT JOIN web.Dimension2 D2 WITH (Nolock) ON D2.Dimension2Id=L.Dimension2Id
	LEFT JOIN web.Dimension3 D3 WITH (Nolock) ON D3.Dimension3Id=L.Dimension3Id
	LEFT JOIN Web.Dimension4 D4 WITH (nolock) ON D4.Dimension4Id=L.Dimension4Id
	LEFT JOIN web.Units U WITH (Nolock) ON U.UnitId=PD.UnitId	
	LEFT JOIN web.ChargeGroupProducts CGPD WITH (Nolock) ON PD.SalesTaxGroupProductId = CGPD.ChargeGroupProductId	
   	WHERE H.StockHeaderId=" + item + @"
   	ORDER BY L.Sr";

            ListofQuery QryMain = new ListofQuery();
            QryMain.Query = QueryMain;
            QryMain.QueryName = nameof(QueryMain);
            DocumentPrintData.Add(QryMain);


            String QueryGatePass;
            QueryGatePass = @"SELECT JOH.StockHeaderId, H.GatePassHeaderId,  DT.DocumentTypeShortName +'-'+ H.DocNo AS DocNo, H.DocDate,  H.Remark, P.Name AS PersonName, G.GodownName,  
                L.GatePassLineId, L.Product, L.Specification, L.Qty, U.UnitName, U.DecimalPlaces,
                DT1.DocumentTypeShortName +'-'+ JOH.DocNo AS ReferenceDocNo,'GatePassPrint.rdl'  AS ReportName, 'Gate Pass' AS ReportTitle,
                NULL AS SubReportProcList,
                DTS.SignatoryleftCaption,
                DTS.SignatoryMiddleCaption,
                DTS.SignatoryRightCaption    
                FROM Web.StockHeaders JOH
                LEFT JOIN [Web].DocumentTypes DT1 WITH (nolock) ON DT1.DocumentTypeId = JOH.DocTypeId 
                LEFT JOIN  Web.GatePassHeaders H ON JOH.GatePassHeaderId = H.GatePassHeaderId 
                LEFT JOIN web.Godowns G ON G.GodownId = H.GodownId 
                LEFT JOIN web.People P ON P.PersonID  = H.PersonID 
                LEFT JOIN [Web].DocumentTypes DT WITH (nolock) ON DT.DocumentTypeId = H.DocTypeId 
                LEFT JOIN web._DocumentTypeSettings DTS WITH (Nolock) ON DTS.DocumentTypeId=H.DocTypeId 
                LEFT JOIN web.GatePassLines L ON L.GatePassHeaderId = H.GatePassHeaderId 
                LEFT JOIN web.Units U ON U.UnitId = L.UnitId 
                WHERE JOH.StockHeaderId = " + item + @" ";


            ListofQuery QryGatePass = new ListofQuery();
            QryGatePass.Query = QueryGatePass;
            QryGatePass.QueryName = nameof(QueryGatePass);
            DocumentPrintData.Add(QryGatePass);



            String QueryCalculation;
            QueryCalculation = @"SELECT 1 id, Max(H.StockHeaderId) AS HeaderTableId, 1 Sr,'Gross Amount' ChargeName, 0 AS Amount, NULL ChargeTypeId, NULL ChargeTypeName,0 Rate,'StdDocPrintSub_CalculationHeader.rdl' ReportName 
                            FROM web.StockHeaders H
                            LEFT JOIN web.StockLines L ON L.StockHeaderId = H.StockHeaderId
                            WHERE H.StockHeaderId = " + item + @" 
                            UNION ALL 
                            SELECT 2 id, Max(H.StockHeaderId) AS HeaderTableId, 2 Sr,'CGST' ChargeName, 0 AS Amount, NULL ChargeTypeId, NULL ChargeTypeName,0 Rate,'StdDocPrintSub_CalculationHeader.rdl' ReportName 
                            FROM web.StockHeaders H
                            LEFT JOIN web.StockLines L ON L.StockHeaderId = H.StockHeaderId
                            WHERE H.StockHeaderId = " + item + @" 
                            UNION ALL 
                            SELECT 3 id, Max(H.StockHeaderId) AS HeaderTableId, 3 Sr,'SGST' ChargeName, 0 AS Amount, NULL ChargeTypeId, NULL ChargeTypeName,0 Rate,'StdDocPrintSub_CalculationHeader.rdl' ReportName 
                            FROM web.StockHeaders H
                            LEFT JOIN web.StockLines L ON L.StockHeaderId = H.StockHeaderId
                            WHERE H.StockHeaderId = " + item + @" 
                            UNION ALL 
                            SELECT 4 id, Max(H.StockHeaderId) AS HeaderTableId, 4 Sr,'Net Amount' ChargeName, 0 AS Amount, NULL ChargeTypeId, NULL ChargeTypeName,0 Rate,'StdDocPrintSub_CalculationHeader.rdl' ReportName 
                            FROM web.StockHeaders H
                            LEFT JOIN web.StockLines L ON L.StockHeaderId = H.StockHeaderId
                            WHERE H.StockHeaderId = " + item + @" 	";


            ListofQuery QryCalculation = new ListofQuery();
            QryCalculation.Query = QueryCalculation;
            QryCalculation.QueryName = nameof(QueryCalculation);
            DocumentPrintData.Add(QryCalculation);


            String QueryGSTSummary;
            QueryGSTSummary = @"   SELECT  
        isnull(STGP.PrintingDescription,STGP.ChargeGroupProductName) as ChargeGroupProductName, 
        Sum(L.Amount) AS TaxableAmount,0 AS IGST,0 AS CGST,0 AS SGST,0 AS GSTCess,
        'StdDocPrintSub_GSTSummary.rdl' AS ReportName
        FROM web.StockHeaders H
        LEFT JOIN web.StockLines L ON L.StockHeaderId = H.StockHeaderId
        LEFT JOIN web.Products P ON P.ProductId = L.ProductId
        LEFT JOIN web.ChargeGroupProducts STGP ON P.SalesTaxGroupProductId = STGP.ChargeGroupProductId
        WHERE H.StockHeaderId = " + item + @"
        GROUP BY isnull(STGP.PrintingDescription,STGP.ChargeGroupProductName)	";


            ListofQuery QryGSTSummary = new ListofQuery();
            QryGSTSummary.Query = QueryGSTSummary;
            QryGSTSummary.QueryName = nameof(QueryGSTSummary);
            DocumentPrintData.Add(QryGSTSummary);

            return DocumentPrintData;

        }

        public ActionResult GetCustomPerson(string searchTerm, int pageSize, int pageNum, int filter, int? filter2)//DocTypeId
        {
            var Query = _StockHeaderService.GetCustomPerson(filter, searchTerm, filter2);
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

        


        #region submitValidation
        public bool Submitvalidation(int id, out string Msg)
        {
            Msg = "";
            int Stockline = (new StockLineService(_unitOfWork).GetStockLineListForIndex(id)).Count();
            if (Stockline == 0)
            {
                Msg = "Add Line Record. <br />";
            }
            else
            {
                Msg = "";
            }
            return (string.IsNullOrEmpty(Msg));
        }

        #endregion submitValidation
        protected override void Dispose(bool disposing)
        {
            if (!string.IsNullOrEmpty((string)TempData["CSEXC"]))
            {
                CookieGenerator.CreateNotificationCookie(NotificationTypeConstants.Danger, (string)TempData["CSEXC"]);
                TempData.Remove("CSEXC");
            }

            if (disposing)
            {
                context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
