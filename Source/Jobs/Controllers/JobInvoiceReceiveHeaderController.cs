﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Model.Models;
using Data.Models;
using Service;
using Data.Infrastructure;
using Presentation.ViewModels;
using Presentation;
using Core.Common;
using Model.ViewModel;
using Jobs.Helpers;
using AutoMapper;
using System.Configuration;
using System.Xml.Linq;
using JobInvoiceReceiveDocumentEvents;
using CustomEventArgs;
using DocumentEvents;
using Reports.Reports;
using Reports.Controllers;
using Model.ViewModels;

namespace Jobs.Controllers
{
    [Authorize]
    public class JobInvoiceReceiveHeaderController : System.Web.Mvc.Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        List<string> UserRoles = new List<string>();
        ActiivtyLogViewModel LogVm = new ActiivtyLogViewModel();

        private bool EventException = false;
        bool TimePlanValidation = true;
        string ExceptionMsg = "";
        bool Continue = true;

        IJobReceiveHeaderService _JobReceiveHeaderService;
        IJobInvoiceHeaderService _JobInvoiceHeaderService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;
        public JobInvoiceReceiveHeaderController(IJobReceiveHeaderService JobReceiveHeaderService, IJobInvoiceHeaderService JobInvoiceHeaderService, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _JobReceiveHeaderService = JobReceiveHeaderService;
            _JobInvoiceHeaderService = JobInvoiceHeaderService;
            _unitOfWork = unitOfWork;
            _exception = exec;
            if (!JobInvoiceReceiveEvents.Initialized)
            {
                JobInvoiceReceiveEvents Obj = new JobInvoiceReceiveEvents();
            }

            UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];

            //Log Initialization
            LogVm.SessionId = 0;
            LogVm.ControllerName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("controller");
            LogVm.ActionName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("action");
            LogVm.User = System.Web.HttpContext.Current.Request.RequestContext.HttpContext.User.Identity.Name;
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

        void PrepareViewBag(int id)
        {
            ViewBag.Name = new DocumentTypeService(_unitOfWork).Find(id).DocumentTypeName;

            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            ViewBag.AdminSetting = UserRoles.Contains("Admin").ToString();
            var settings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(id, DivisionId, SiteId);
            if (settings != null)
            {
                ViewBag.IsPostedInStock = settings.isPostedInStock;
                ViewBag.WizardId = settings.WizardMenuId;
                ViewBag.ImportMenuId = settings.ImportMenuId;
                ViewBag.SqlProcDocumentPrint = settings.SqlProcDocumentPrint;
                ViewBag.ExportMenuId = settings.ExportMenuId;
				ViewBag.isVisibleProcessHeader= settings.isVisibleProcessHeader;
                ViewBag.isVisibleHeaderJobWorker = settings.isVisibleHeaderJobWorker; 	
            }

            ViewBag.id = id;
        }

        // GET: /JobInvoiceHeaderMaster/

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
            IQueryable<JobInvoiceHeaderViewModel> JobInvoiceHeader = _JobInvoiceHeaderService.GetJobInvoiceHeaderList(id, User.Identity.Name);
            PrepareViewBag(id);
            ViewBag.PendingToSubmit = PendingToSubmitCount(id);
            ViewBag.PendingToReview = PendingToReviewCount(id);
            ViewBag.IndexStatus = "All";


            #region "Setting Section"
            var DocumentTypeSettings = new DocumentTypeSettingsService(_unitOfWork).GetDocumentTypeSettingsForDocument(id);
            ViewBag.ProductUidCaption = DocumentTypeSettings.ProductUidCaption ?? "Product Uid";
            ViewBag.ProductCaption = DocumentTypeSettings.ProductCaption ?? "Product";
            ViewBag.ProductGroupCaption = DocumentTypeSettings.ProductGroupCaption ?? "Product Group";


            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            var settings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(id, DivisionId, SiteId);
            if (settings != null)
            {
                ViewBag.isVisibleGodown = settings.isVisibleGodown ?? false;
                ViewBag.isVisibleProductUid_Index = settings.isVisibleProductUid_Index ?? false;
                ViewBag.isVisibleProduct_Index = settings.isVisibleProduct_Index ?? false;
                ViewBag.isVisibleProductGroup_Index = settings.isVisibleProductGroup_Index ?? false;
            }
            else
            {
                ViewBag.isVisibleGodown = false;
                ViewBag.isVisibleProductUid_Index = false;
                ViewBag.isVisibleProduct_Index = false;
                ViewBag.isVisibleProductGroup_Index = false;
            }
            #endregion

            return View(JobInvoiceHeader);
        }


        public ActionResult Index_PendingToSubmit(int id)
        {
            IQueryable<JobInvoiceHeaderViewModel> p = _JobInvoiceHeaderService.GetJobInvoiceHeaderListPendingToSubmit(id, User.Identity.Name);

            PrepareViewBag(id);
            ViewBag.PendingToSubmit = PendingToSubmitCount(id);
            ViewBag.PendingToReview = PendingToReviewCount(id);
            ViewBag.IndexStatus = "PTS";

            #region "Setting Section"
            var DocumentTypeSettings = new DocumentTypeSettingsService(_unitOfWork).GetDocumentTypeSettingsForDocument(id);
            ViewBag.ProductUidCaption = DocumentTypeSettings.ProductUidCaption ?? "Product Uid";
            ViewBag.ProductCaption = DocumentTypeSettings.ProductCaption ?? "Product";
            ViewBag.ProductGroupCaption = DocumentTypeSettings.ProductGroupCaption ?? "Product Group";


            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            var settings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(id, DivisionId, SiteId);
            if (settings != null)
            {
                ViewBag.isVisibleGodown = settings.isVisibleGodown ?? false;
                ViewBag.isVisibleProductUid_Index = settings.isVisibleProductUid_Index ?? false;
                ViewBag.isVisibleProduct_Index = settings.isVisibleProduct_Index ?? false;
                ViewBag.isVisibleProductGroup_Index = settings.isVisibleProductGroup_Index ?? false;
            }
            else
            {
                ViewBag.isVisibleGodown = false;
                ViewBag.isVisibleProductUid_Index = false;
                ViewBag.isVisibleProduct_Index = false;
                ViewBag.isVisibleProductGroup_Index = false;
            }
            #endregion

            return View("Index", p);
        }
        public ActionResult Index_PendingToReview(int id)
        {
            IQueryable<JobInvoiceHeaderViewModel> p = _JobInvoiceHeaderService.GetJobInvoiceHeaderListPendingToReview(id, User.Identity.Name);
            PrepareViewBag(id);
            ViewBag.PendingToSubmit = PendingToSubmitCount(id);
            ViewBag.PendingToReview = PendingToReviewCount(id);
            ViewBag.IndexStatus = "PTR";

            #region "Setting Section"
            var DocumentTypeSettings = new DocumentTypeSettingsService(_unitOfWork).GetDocumentTypeSettingsForDocument(id);
            ViewBag.ProductUidCaption = DocumentTypeSettings.ProductUidCaption ?? "Product Uid";
            ViewBag.ProductCaption = DocumentTypeSettings.ProductCaption ?? "Product";
            ViewBag.ProductGroupCaption = DocumentTypeSettings.ProductGroupCaption ?? "Product Group";


            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            var settings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(id, DivisionId, SiteId);
            if (settings != null)
            {
                ViewBag.isVisibleGodown = settings.isVisibleGodown ?? false;
                ViewBag.isVisibleProductUid_Index = settings.isVisibleProductUid_Index ?? false;
                ViewBag.isVisibleProduct_Index = settings.isVisibleProduct_Index ?? false;
                ViewBag.isVisibleProductGroup_Index = settings.isVisibleProductGroup_Index ?? false;
            }
            else
            {
                ViewBag.isVisibleGodown = false;
                ViewBag.isVisibleProductUid_Index = false;
                ViewBag.isVisibleProduct_Index = false;
                ViewBag.isVisibleProductGroup_Index = false;
            }
            #endregion

            return View("Index", p);
        }



        // GET: /JobInvoiceHeaderMaster/Create

        public ActionResult Create(int id)//DocumentTypeId
        {
            PrepareViewBag(id);
            JobInvoiceHeaderViewModel vm = new JobInvoiceHeaderViewModel();
            vm.DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            vm.SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            vm.CreatedDate = DateTime.Now;

            List<DocumentTypeHeaderAttributeViewModel> tem = new DocumentTypeService(_unitOfWork).GetDocumentTypeHeaderAttribute(id).ToList();
            vm.DocumentTypeHeaderAttributes = tem;

            //Getting Settings
            var settings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(id, vm.DivisionId, vm.SiteId);

            if (settings == null && UserRoles.Contains("SysAdmin"))
            {
                return RedirectToAction("CreateInvoiceReceive", "JobInvoiceSettings", new { id = id }).Warning("Please create job Invoice Receive settings");
            }
            else if (settings == null && !UserRoles.Contains("SysAdmin"))
            {
                return View("~/Views/Shared/InValidSettings.cshtml");
            }



            vm.JobInvoiceSettings = Mapper.Map<JobInvoiceSettings, JobInvoiceSettingsViewModel>(settings);


            if ((settings.isVisibleProcessHeader ?? false) == false)
            {
                vm.ProcessId = settings.ProcessId;
            }

            if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, id, vm.ProcessId, this.ControllerContext.RouteData.Values["controller"].ToString(), "Create") == false)
            {
                return View("~/Views/Shared/PermissionDenied.cshtml").Warning("You don't have permission to do this task.");
            }

            if (System.Web.HttpContext.Current.Session["DefaultGodownId"] != null)
                vm.GodownId = (int)System.Web.HttpContext.Current.Session["DefaultGodownId"];

            if (vm.GodownId == null || vm.GodownId == 0)
            {
                Site Site = new SiteService(_unitOfWork).Find(vm.SiteId);
                if (Site != null)
                {
                    if (Site.DefaultGodownId != null)
                        vm.GodownId = (int)Site.DefaultGodownId;
                }

                if (vm.GodownId == null || vm.GodownId == 0)
                {
                    var SiteGodownList = (from G in db.Godown where G.SiteId == vm.SiteId select G).FirstOrDefault();
                    if (SiteGodownList != null)
                        vm.GodownId = (int)SiteGodownList.GodownId;
                }

                if (vm.GodownId == null || vm.GodownId == 0)
                {
                    var GodownList = db.Godown.FirstOrDefault();
                    if (GodownList != null)
                        vm.GodownId = (int)GodownList.GodownId;
                }
            }

            JobReceiveHeader LastReceive = (from H in db.JobReceiveHeader where H.DocTypeId == settings.JobReceiveDocTypeId && H.SiteId == settings.SiteId
                                            && H.DivisionId == settings.DivisionId
                                            orderby H.DocDate descending, H.JobReceiveHeaderId descending select H).FirstOrDefault();
            if (LastReceive != null)
            {
                if (LastReceive.JobReceiveById != null)
                {
                    vm.JobReceiveById = LastReceive.JobReceiveById;
                }
            }
            //else
            //{
            //    Employee Employee = new EmployeeService(_unitOfWork).GetEmployeeList().FirstOrDefault();
            //    if (Employee != null)
            //    {
            //        vm.JobReceiveById = Employee.PersonID;
            //    }
            //}


            if (settings != null)
            {
                vm.SalesTaxGroupPersonId = settings.SalesTaxGroupPersonId;
            }


            if (settings != null)
            {
                if (settings.CalculationId != null)
                {
                    var CalculationHeaderLedgerAccount = (from H in db.CalculationHeaderLedgerAccount where H.CalculationId == settings.CalculationId && H.DocTypeId == id && H.SiteId == vm.SiteId && H.DivisionId == vm.DivisionId select H).FirstOrDefault();
                    var CalculationLineLedgerAccount = (from H in db.CalculationLineLedgerAccount where H.CalculationId == settings.CalculationId && H.DocTypeId == id && H.SiteId == vm.SiteId && H.DivisionId == vm.DivisionId select H).FirstOrDefault();

                    if (CalculationHeaderLedgerAccount == null && CalculationLineLedgerAccount == null && UserRoles.Contains("SysAdmin"))
                    {
                        return RedirectToAction("Create", "CalculationHeaderLedgerAccount", null).Warning("Ledger posting settings is not defined for current site and division.");
                    }
                    else if (CalculationHeaderLedgerAccount == null && CalculationLineLedgerAccount == null && !UserRoles.Contains("SysAdmin"))
                    {
                        return View("~/Views/Shared/InValidSettings.cshtml").Warning("Ledger posting settings is not defined for current site and division.");
                    }
                }
            }


            //vm.ProcessId = settings.ProcessId;



            vm.DocDate = DateTime.Now;
            vm.DocTypeId = id;
            vm.DocNo = new DocumentTypeService(_unitOfWork).FGetNewDocNo("DocNo", ConfigurationManager.AppSettings["DataBaseSchema"] + ".JobInvoiceHeaders", vm.DocTypeId, vm.DocDate, vm.DivisionId, vm.SiteId);
            vm.DocumentTypeSettings = new DocumentTypeSettingsService(_unitOfWork).GetDocumentTypeSettingsForDocument(vm.DocTypeId);
            ViewBag.Mode = "Add";
            return View("Create", vm);
        }

        // POST: /ProductMaster/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Post(JobInvoiceHeaderViewModel vm)
        {
            bool BeforeSave = true;
            JobInvoiceHeader pt = AutoMapper.Mapper.Map<JobInvoiceHeaderViewModel, JobInvoiceHeader>(vm);
            JobReceiveHeader pt2 = AutoMapper.Mapper.Map<JobInvoiceHeaderViewModel, JobReceiveHeader>(vm);

            var Settings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(pt.DocTypeId, vm.DivisionId, vm.SiteId);

            if ((Settings.isVisibleGodown ?? false) == true)
            {
                if (vm.GodownId <= 0)
                    ModelState.AddModelError("GodownId", "The Godown field is required");
            }

            if ((Settings.isVisibleJobReceiveBy ?? false) == true)
            {
                if (vm.JobReceiveById <= 0)
                    ModelState.AddModelError("JobReceiveById", "The Job Receiveby field is required");
            }

            if ((Settings.isVisibleHeaderJobWorker ?? false) == true)
            {
                if (!vm.JobWorkerId.HasValue || vm.JobWorkerId.Value <= 0)
                    ModelState.AddModelError("JobWorkerId", "The JobWorker field is required");
            }

            if (vm.JobWorkerId != null && vm.JobWorkerId != 0)
            {
                SiteDivisionSettings SiteDivisionSettings = new SiteDivisionSettingsService(_unitOfWork).GetSiteDivisionSettings(vm.SiteId, vm.DivisionId, vm.DocDate);
                if (SiteDivisionSettings != null)
                {
                    if (SiteDivisionSettings.IsApplicableGST == true)
                    {
                        if (vm.SalesTaxGroupPersonId == 0 || vm.SalesTaxGroupPersonId == null)
                        {
                            ModelState.AddModelError("", "Sales Tax Group Person is not defined for party, it is required.");
                        }
                    }
                }
            }



            #region BeforeSave
            try
            {

                if (vm.JobInvoiceHeaderId <= 0)
                    BeforeSave = JobInvoiceReceiveDocEvents.beforeHeaderSaveEvent(this, new JobEventArgs(vm.JobInvoiceHeaderId, EventModeConstants.Add), ref db);
                else
                    BeforeSave = JobInvoiceReceiveDocEvents.beforeHeaderSaveEvent(this, new JobEventArgs(vm.JobInvoiceHeaderId, EventModeConstants.Edit), ref db);

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

                if (vm.JobInvoiceHeaderId <= 0)
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

            if (ModelState.IsValid && BeforeSave && !EventException && (TimePlanValidation || Continue))
            {
                #region CreateRecord
                if (vm.JobInvoiceHeaderId <= 0)
                {
                    if (vm.JobWorkerId != null && vm.JobWorkerId != 0)
                    {
                        if (Settings.JobReceiveDocTypeId != null)
                        {
                            pt2.DocTypeId = Settings.JobReceiveDocTypeId.Value;
                        }
                        else
                        {
                            pt2.DocTypeId = vm.DocTypeId;
                        }
                        pt2.CreatedBy = User.Identity.Name;
                        pt2.CreatedDate = DateTime.Now;
                        pt2.DocNo = new DocumentTypeService(_unitOfWork).FGetNewDocNo("DocNo", ConfigurationManager.AppSettings["DataBaseSchema"] + ".JobReceiveHeaders", pt2.DocTypeId, vm.DocDate, vm.DivisionId, vm.SiteId);
                        pt2.ModifiedBy = User.Identity.Name;
                        pt2.ModifiedDate = DateTime.Now;
                        pt2.Status = (int)StatusConstants.System;
                        pt2.ObjectState = Model.ObjectState.Added;
                        db.JobReceiveHeader.Add(pt2);
                        pt.JobReceiveHeaderId = pt2.JobReceiveHeaderId;
                    }

                    pt.DivisionId = vm.DivisionId;
                    pt.SiteId = vm.SiteId;
                    pt.CreatedDate = DateTime.Now;
                    pt.ModifiedDate = DateTime.Now;
                    pt.CreatedBy = User.Identity.Name;
                    pt.ModifiedBy = User.Identity.Name;
                    pt.ObjectState = Model.ObjectState.Added;
                    db.JobInvoiceHeader.Add(pt);
                    //_JobInvoiceHeaderService.Create(pt);     

                    if (vm.DocumentTypeHeaderAttributes != null)
                    {
                        foreach (var Attributes in vm.DocumentTypeHeaderAttributes)
                        {
                            JobInvoiceHeaderAttributes JobInvoiceHeaderAttribute = (from A in db.JobInvoiceHeaderAttributes
                                                                                    where A.HeaderTableId == pt.JobInvoiceHeaderId
                                                                                && A.DocumentTypeHeaderAttributeId == Attributes.DocumentTypeHeaderAttributeId
                                                                                    select A).FirstOrDefault();

                            if (JobInvoiceHeaderAttribute != null)
                            {
                                JobInvoiceHeaderAttribute.Value = Attributes.Value;
                                JobInvoiceHeaderAttribute.ObjectState = Model.ObjectState.Modified;
                                db.JobInvoiceHeaderAttributes.Add(JobInvoiceHeaderAttribute);
                            }
                            else
                            {
                                JobInvoiceHeaderAttributes HeaderAttribute = new JobInvoiceHeaderAttributes()
                                {
                                    HeaderTableId = pt.JobInvoiceHeaderId,
                                    Value = Attributes.Value,
                                    DocumentTypeHeaderAttributeId = Attributes.DocumentTypeHeaderAttributeId,
                                };
                                HeaderAttribute.ObjectState = Model.ObjectState.Added;
                                db.JobInvoiceHeaderAttributes.Add(HeaderAttribute);
                            }
                        }
                    }


                    try
                    {
                        JobInvoiceReceiveDocEvents.onHeaderSaveEvent(this, new JobEventArgs(pt.JobInvoiceHeaderId, EventModeConstants.Add), ref db);
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

                        db.SaveChanges();
                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                        PrepareViewBag(vm.DocTypeId);
                        ViewBag.Mode = "Add";
                        return View("Create", vm);
                    }

                    try
                    {
                        JobInvoiceReceiveDocEvents.afterHeaderSaveEvent(this, new JobEventArgs(pt.JobInvoiceHeaderId, EventModeConstants.Add), ref db);
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                    }

                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = pt.DocTypeId,
                        DocId = pt.JobInvoiceHeaderId,
                        ActivityType = (int)ActivityTypeContants.Added,
                        DocNo = pt.DocNo,
                        DocDate = pt.DocDate,
                        DocStatus = pt.Status,
                    }));

                    return RedirectToAction("Modify", new { id = pt.JobInvoiceHeaderId }).Success("Data saved successfully");
                }
                #endregion

                #region EditRecord
                else
                {
                    bool GodownChanged = false;
                    bool DocDateChanged = false;
                    List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();

                    JobInvoiceHeader temp = _JobInvoiceHeaderService.Find(pt.JobInvoiceHeaderId);

                    bool IsBillToPartyChanged = false;
                    if (vm.JobWorkerId != temp.JobWorkerId)
                        IsBillToPartyChanged = true;


                    JobInvoiceHeader ExRec = new JobInvoiceHeader();
                    ExRec = Mapper.Map<JobInvoiceHeader>(temp);

                    if (temp.JobReceiveHeaderId != null)
                    {
                        JobReceiveHeader temp2 = _JobReceiveHeaderService.Find(temp.JobReceiveHeaderId.Value);
                        GodownChanged = (temp2.GodownId == vm.GodownId) ? false : true;
                        DocDateChanged = (temp2.DocDate == vm.DocDate) ? false : true;

                        JobReceiveHeader ExRecR = new JobReceiveHeader();
                        ExRecR = Mapper.Map<JobReceiveHeader>(temp2);

                        temp2.DocDate = vm.DocDate;
                        //temp2.DocNo = vm.DocNo;
                        temp2.JobWorkerDocNo = vm.JobWorkerDocNo;
                        temp2.ProcessId = vm.ProcessId;
                        temp2.JobWorkerId = vm.JobWorkerId.Value;
                        temp2.JobReceiveById = vm.JobReceiveById;
                        temp2.GodownId = vm.GodownId;
                        temp2.Remark = vm.Remark;
                        temp2.ModifiedDate = DateTime.Now;
                        temp2.ModifiedBy = User.Identity.Name;
                        temp2.ObjectState = Model.ObjectState.Modified;
                        db.JobReceiveHeader.Add(temp2);

                        if (GodownChanged)
                            new StockService(_unitOfWork).UpdateStockGodownId(temp2.StockHeaderId, temp2.GodownId, temp.DocDate, db);

                        LogList.Add(new LogTypeViewModel
                        {
                            ExObj = ExRecR,
                            Obj = temp2,
                        });


                        if (temp2.StockHeaderId != null)
                        {
                            StockHeader StockHeader = (from p in db.StockHeader
                                                       where p.StockHeaderId == temp2.StockHeaderId
                                                       select p).FirstOrDefault();

                            StockHeader.DocTypeId = temp2.DocTypeId;
                            StockHeader.DocDate = temp2.DocDate;
                            StockHeader.DocNo = temp2.DocNo;
                            StockHeader.DivisionId = temp2.DivisionId;
                            StockHeader.SiteId = temp2.SiteId;
                            StockHeader.ProcessId = temp2.ProcessId;
                            StockHeader.GodownId = temp2.GodownId;
                            StockHeader.Remark = temp2.Remark;
                            StockHeader.Status = temp2.Status;
                            StockHeader.ModifiedBy = temp2.ModifiedBy;
                            StockHeader.ModifiedDate = temp2.ModifiedDate;

                            StockHeader.ObjectState = Model.ObjectState.Modified;
                            db.StockHeader.Add(StockHeader);

                        }
                    }



                    int status = temp.Status;

                    if (temp.Status != (int)StatusConstants.Drafted && temp.Status != (int)StatusConstants.Import)
                    {
                        temp.Status = (int)StatusConstants.Modified;
                        //temp2.Status = (int)StatusConstants.Modified;
                    }

                    temp.Remark = pt.Remark;
                    temp.DocNo = pt.DocNo;
                    temp.DocDate = pt.DocDate;
                    temp.JobWorkerId = pt.JobWorkerId;
                    temp.JobWorkerDocNo = pt.JobWorkerDocNo;
                    temp.GovtInvoiceNo = pt.GovtInvoiceNo;
                    temp.JobWorkerDocDate = pt.JobWorkerDocDate;
                    temp.ProcessId = pt.ProcessId;
                    temp.SalesTaxGroupPersonId = pt.SalesTaxGroupPersonId;
                    temp.FinancierId = pt.FinancierId;
                    temp.ModifiedDate = DateTime.Now;
                    temp.ModifiedBy = User.Identity.Name;
                    temp.ObjectState = Model.ObjectState.Modified;
                    db.JobInvoiceHeader.Add(temp);
                    //_JobInvoiceHeaderService.Update(temp);

                    LogList.Add(new LogTypeViewModel
                    {
                        ExObj = ExRec,
                        Obj = temp,
                    });



                    if (vm.DocumentTypeHeaderAttributes != null)
                    {
                        foreach (var Attributes in vm.DocumentTypeHeaderAttributes)
                        {

                            JobInvoiceHeaderAttributes JobInvoiceHeaderAttribute = (from A in db.JobInvoiceHeaderAttributes
                                                                                    where A.HeaderTableId == temp.JobInvoiceHeaderId
                                                                                && A.DocumentTypeHeaderAttributeId == Attributes.DocumentTypeHeaderAttributeId
                                                                                    select A).FirstOrDefault();

                            if (JobInvoiceHeaderAttribute != null)
                            {
                                JobInvoiceHeaderAttribute.Value = Attributes.Value;
                                JobInvoiceHeaderAttribute.ObjectState = Model.ObjectState.Modified;
                                db.JobInvoiceHeaderAttributes.Add(JobInvoiceHeaderAttribute);
                            }
                            else
                            {
                                JobInvoiceHeaderAttributes HeaderAttribute = new JobInvoiceHeaderAttributes()
                                {
                                    Value = Attributes.Value,
                                    HeaderTableId = temp.JobInvoiceHeaderId,
                                    DocumentTypeHeaderAttributeId = Attributes.DocumentTypeHeaderAttributeId,
                                };
                                HeaderAttribute.ObjectState = Model.ObjectState.Added;
                                db.JobInvoiceHeaderAttributes.Add(HeaderAttribute);
                            }
                        }
                    }


                    if (IsBillToPartyChanged == true)
                    {
                        IEnumerable<JobInvoiceHeaderCharge> HeaderChargeList = (from Hc in db.JobInvoiceHeaderCharges where Hc.HeaderTableId == temp.JobInvoiceHeaderId select Hc).ToList();
                        foreach (JobInvoiceHeaderCharge HeaderCharge in HeaderChargeList)
                        {
                            HeaderCharge.PersonID = vm.JobWorkerId;
                            _unitOfWork.Repository<JobInvoiceHeaderCharge>().Update(HeaderCharge);
                        }

                        IEnumerable<JobInvoiceLineCharge> LineChargeList = (from Hc in db.JobInvoiceLineCharge where Hc.HeaderTableId == temp.JobInvoiceHeaderId select Hc).ToList();
                        foreach (JobInvoiceLineCharge LineCharge in LineChargeList)
                        {
                            LineCharge.PersonID = vm.JobWorkerId;
                            _unitOfWork.Repository<JobInvoiceLineCharge>().Update(LineCharge);
                        }
                    }


                    XElement Modifications = new ModificationsCheckService().CheckChanges(LogList);

                    try
                    {
                        JobInvoiceReceiveDocEvents.onHeaderSaveEvent(this, new JobEventArgs(temp.JobInvoiceHeaderId, EventModeConstants.Edit), ref db);
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

                        db.SaveChanges();
                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                        PrepareViewBag(vm.DocTypeId);
                        ViewBag.Mode = "Edit";
                        return View("Create", vm);
                    }

                    try
                    {
                        JobInvoiceReceiveDocEvents.afterHeaderSaveEvent(this, new JobEventArgs(temp.JobInvoiceHeaderId, EventModeConstants.Edit), ref db);
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                    }

                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = temp.DocTypeId,
                        DocId = temp.JobInvoiceHeaderId,
                        ActivityType = (int)ActivityTypeContants.Modified,
                        DocNo = pt.DocNo,
                        xEModifications = Modifications,
                        DocDate = pt.DocDate,
                        DocStatus = pt.Status,
                    }));

                    return RedirectToAction("Index", new { id = vm.DocTypeId }).Success("Data saved successfully");
                }
                #endregion
            }

            var ModelStateErrorList = ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList();
            string Messsages = "";
            if (ModelStateErrorList.Count > 0)
            {
                foreach (var ModelStateError in ModelStateErrorList)
                {
                    foreach (var Error in ModelStateError)
                    {
                        if (!Messsages.Contains(Error.ErrorMessage))
                            Messsages = Error.ErrorMessage  + System.Environment.NewLine;
                    }
                }
                if (Messsages != "")
                    ModelState.AddModelError("", Messsages);
            }




            PrepareViewBag(vm.DocTypeId);
            ViewBag.Mode = "Add";
            return View("Create", vm);
        }


        [HttpGet]
        public ActionResult Modify(int id, string IndexType)
        {
            JobInvoiceHeader header = _JobInvoiceHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Drafted || header.Status == (int)StatusConstants.Import)
                return Edit(id, IndexType);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult ModifyAfter_Submit(int id, string IndexType)
        {
            JobInvoiceHeader header = _JobInvoiceHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Submitted || header.Status == (int)StatusConstants.Modified)
                return Edit(id, IndexType);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult ModifyAfter_Approve(int id, string IndexType)
        {
            JobInvoiceHeader header = _JobInvoiceHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Approved)
                return Edit(id, IndexType);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            JobInvoiceHeader header = _JobInvoiceHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Drafted || header.Status == (int)StatusConstants.Import)
                return Remove(id);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult DeleteAfter_Submit(int id)
        {
            JobInvoiceHeader header = _JobInvoiceHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Submitted || header.Status == (int)StatusConstants.Modified)
                return Remove(id);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult DeleteAfter_Approve(int id)
        {
            JobInvoiceHeader header = _JobInvoiceHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Approved)
                return Remove(id);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult DetailInformation(int id)
        {
            return RedirectToAction("Detail", new { id = id, transactionType = "detail" });
        }


        // GET: /ProductMaster/Edit/5

        private ActionResult Edit(int id, string IndexType)
        {
            ViewBag.IndexStatus = IndexType;
            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];

            JobInvoiceHeaderViewModel pt = _JobInvoiceHeaderService.GetJobInvoiceHeader(id);

            if (pt.Status != (int)StatusConstants.Drafted)
                if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, pt.DocTypeId, pt.ProcessId, this.ControllerContext.RouteData.Values["controller"].ToString(), "Edit") == false)
                    return RedirectToAction("DetailInformation", new { id = id, IndexType = IndexType }).Warning("You don't have permission to do this task."); ;


            string SiteName = db.Site.Find(pt.SiteId).SiteName;
            int LoginSiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            if (pt.SiteId != LoginSiteId)
                pt.LockReason = "Can't modify " + SiteName + " record.You have to login with " + SiteName;

            #region DocTypeTimeLineValidation
            try
            {

                TimePlanValidation = DocumentValidation.ValidateDocument(Mapper.Map<DocumentUniqueId>(pt), DocumentTimePlanTypeConstants.Modify, User.Identity.Name, out ExceptionMsg, out Continue);

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

            var JobInvoiceReturn = db.JobInvoiceReturnLine.Where(i => i.JobInvoiceLine.JobInvoiceHeaderId == id).FirstOrDefault();
            if (JobInvoiceReturn != null)
            {
                pt.ReturnNature = (from H in db.JobInvoiceReturnHeader where H.JobInvoiceReturnHeaderId == JobInvoiceReturn.JobInvoiceReturnHeaderId select new { DocTypeNature = H.DocType.Nature }).FirstOrDefault().DocTypeNature;
                if (pt.ReturnNature == TransactionNatureConstants.Credit)
                    pt.AdditionalInfo = "Credit Note is generated for this invoice.";
                else if (pt.ReturnNature == TransactionNatureConstants.Debit)
                    pt.AdditionalInfo = "Debit Note is generated for this invoice.";
                else
                    pt.AdditionalInfo = "Invoice is cancelled.";
            }


            if (pt.JobReceiveHeaderId != null)
            {
                JobReceiveHeader Pt2 = _JobReceiveHeaderService.Find((int)pt.JobReceiveHeaderId);
                pt.GodownId = Pt2.GodownId;
                pt.JobReceiveById = Pt2.JobReceiveById;
            }


            //Job Order Settings
            var settings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(pt.DocTypeId, pt.DivisionId, pt.SiteId);

            if (settings == null && UserRoles.Contains("SysAdmin"))
            {
                return RedirectToAction("Create", "JobInvoiceSettings", new { id = pt.DocTypeId }).Warning("Please create job Invoice settings");
            }
            else if (settings == null && !UserRoles.Contains("SysAdmin"))
            {
                return View("~/Views/Shared/InValidSettings.cshtml");
            }

            pt.JobInvoiceSettings = Mapper.Map<JobInvoiceSettings, JobInvoiceSettingsViewModel>(settings);
            pt.DocumentTypeSettings = new DocumentTypeSettingsService(_unitOfWork).GetDocumentTypeSettingsForDocument(pt.DocTypeId);

            List<DocumentTypeHeaderAttributeViewModel> tem = _JobInvoiceHeaderService.GetDocumentHeaderAttribute(id).ToList();
            pt.DocumentTypeHeaderAttributes = tem;

            PrepareViewBag(pt.DocTypeId);
            if (pt == null)
            {
                return HttpNotFound();
            }
            ViewBag.Mode = "Edit";
            if ((System.Web.HttpContext.Current.Request.UrlReferrer.PathAndQuery).Contains("Create"))
                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = pt.DocTypeId,
                    DocId = pt.JobInvoiceHeaderId,
                    ActivityType = (int)ActivityTypeContants.Detail,
                    DocNo = pt.DocNo,
                    DocDate = pt.DocDate,
                    DocStatus = pt.Status,
                }));

            return View("Create", pt);
        }


        [Authorize]
        public ActionResult Detail(int id, string IndexType, string transactionType)
        {

            ViewBag.transactionType = transactionType;
            ViewBag.IndexStatus = IndexType;

            JobInvoiceHeaderViewModel pt = _JobInvoiceHeaderService.GetJobInvoiceHeader(id);

            //Getting Settings
            var settings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(pt.DocTypeId, pt.DivisionId, pt.SiteId);

            pt.JobInvoiceSettings = Mapper.Map<JobInvoiceSettings, JobInvoiceSettingsViewModel>(settings);
            pt.DocumentTypeSettings = new DocumentTypeSettingsService(_unitOfWork).GetDocumentTypeSettingsForDocument(pt.DocTypeId);

            pt.SiteName = db.Site.Find(pt.SiteId).SiteName;

            JobReceiveHeader Pt2 = _JobReceiveHeaderService.Find(pt.JobReceiveHeaderId.Value);
            pt.GodownId = Pt2.GodownId;
            pt.JobReceiveById = Pt2.JobReceiveById;

            PrepareViewBag(pt.DocTypeId);
            if (pt == null)
            {
                return HttpNotFound();
            }
            if (String.IsNullOrEmpty(transactionType) || transactionType == "detail")
                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = pt.DocTypeId,
                    DocId = pt.JobInvoiceHeaderId,
                    ActivityType = (int)ActivityTypeContants.Detail,
                    DocNo = pt.DocNo,
                    DocDate = pt.DocDate,
                    DocStatus = pt.Status,
                }));

            return View("Create", pt);
        }




        public ActionResult Submit(int id, string IndexType, string TransactionType)
        {
            JobInvoiceHeader s = db.JobInvoiceHeader.Find(id);
            if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, s.DocTypeId, s.ProcessId, this.ControllerContext.RouteData.Values["controller"].ToString(), "Submit") == false)
            {
                return View("~/Views/Shared/PermissionDenied.cshtml").Warning("You don't have permission to do this task.");
            }

            #region DocTypeTimeLineValidation

            bool TimePlanValidation = true;
            string ExceptionMsg = "";
            bool Continue = true;

            
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
        public ActionResult Submitted(int Id, string IndexType, string UserRemark, string IsContinue)
        {
            #region BeforeSave
            bool BeforeSave = true;
            try
            {
                BeforeSave = JobInvoiceReceiveDocEvents.beforeHeaderSubmitEvent(this, new JobEventArgs(Id), ref db);
            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXC"] += message;
                EventException = true;
            }

            if (!BeforeSave)
                TempData["CSEXC"] += "Falied validation before submit.";
            #endregion


            if (ModelState.IsValid && BeforeSave && !EventException)
            {

                JobInvoiceHeader pd = new JobInvoiceHeaderService(_unitOfWork).Find(Id);
                JobReceiveHeader pd2 = _JobReceiveHeaderService.Find(pd.JobReceiveHeaderId.Value);

                int ActivityType;
                if (User.Identity.Name == pd.ModifiedBy || UserRoles.Contains("Admin"))
                {

                    pd.Status = (int)StatusConstants.Submitted;
                    //pd2.Status = (int)StatusConstants.Submitted;
                    ActivityType = (int)ActivityTypeContants.Submitted;

                    #region "Ledger Posting"

                    LedgerHeaderViewModel LedgerHeaderViewModel = new LedgerHeaderViewModel();

                    LedgerHeaderViewModel.LedgerHeaderId = pd.LedgerHeaderId ?? 0;
                    LedgerHeaderViewModel.DocHeaderId = pd.JobInvoiceHeaderId;
                    LedgerHeaderViewModel.DocTypeId = pd.DocTypeId;
                    LedgerHeaderViewModel.ProcessId = pd.ProcessId;
                    LedgerHeaderViewModel.DocDate = pd.DocDate;
                    LedgerHeaderViewModel.DocNo = pd.DocNo;
                    LedgerHeaderViewModel.DivisionId = pd.DivisionId;
                    LedgerHeaderViewModel.SiteId = pd.SiteId;
                    LedgerHeaderViewModel.PartyDocNo = pd.JobWorkerDocNo;
                    LedgerHeaderViewModel.PartyDocDate = pd.JobWorkerDocDate;
                    LedgerHeaderViewModel.Narration = _JobInvoiceHeaderService.GetNarration(pd.JobInvoiceHeaderId);
                    LedgerHeaderViewModel.Remark = pd.Remark;
                    LedgerHeaderViewModel.CreatedBy = pd.CreatedBy;
                    LedgerHeaderViewModel.CreatedDate = DateTime.Now.Date;
                    LedgerHeaderViewModel.ModifiedBy = pd.ModifiedBy;
                    LedgerHeaderViewModel.ModifiedDate = DateTime.Now.Date;

                    IEnumerable<JobInvoiceHeaderCharge> JobInvoiceHeaderCharges = from H in db.JobInvoiceHeaderCharges where H.HeaderTableId == Id select H;
                    IEnumerable<JobInvoiceLineCharge> JobInvoiceLineCharges = from L in db.JobInvoiceLineCharge where L.HeaderTableId == Id select L;

                    try
                    {
                        new CalculationService(_unitOfWork).LedgerPostingDB(ref LedgerHeaderViewModel, JobInvoiceHeaderCharges, JobInvoiceLineCharges, ref db);
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        ModelState.AddModelError("", message);
                        return RedirectToAction("Detail", new { id = Id, IndexType = IndexType, transactionType = "submit" });
                    }


                    if (pd.LedgerHeaderId == null)
                    {
                        pd.LedgerHeaderId = LedgerHeaderViewModel.LedgerHeaderId;
                        //_JobInvoiceHeaderService.Update(pd);
                    }

                    #endregion

                    pd.ReviewBy = null;
                    pd2.ReviewBy = null;

                    pd.ObjectState = Model.ObjectState.Modified;
                    pd2.ObjectState = Model.ObjectState.Modified;

                    db.JobInvoiceHeader.Add(pd);
                    db.JobReceiveHeader.Add(pd2);

                    try
                    {
                        JobInvoiceReceiveDocEvents.onHeaderSubmitEvent(this, new JobEventArgs(Id), ref db);
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                        EventException = true;
                    }

                    db.SaveChanges();

                    try
                    {
                        JobInvoiceReceiveDocEvents.afterHeaderSubmitEvent(this, new JobEventArgs(Id), ref db);
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                    }

                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = pd.DocTypeId,
                        DocId = pd.JobInvoiceHeaderId,
                        ActivityType = ActivityType,
                        UserRemark = UserRemark,
                        DocNo = pd.DocNo,
                        DocDate = pd.DocDate,
                        DocStatus = pd.Status,
                    }));

                    string ReturnUrl = System.Configuration.ConfigurationManager.AppSettings["CurrentDomain"] + "/" + "JobReceiveHeader" + "/" + "Index" + "/" + pd.DocTypeId;
                    if (!string.IsNullOrEmpty(IsContinue) && IsContinue == "True")
                    {

                        int nextId = new NextPrevIdService(_unitOfWork).GetNextPrevId(Id, pd.DocTypeId, User.Identity.Name, ForActionConstants.PendingToSubmit, "Web.JobInvoiceHeaders", "JobInvoiceHeaderId", PrevNextConstants.Next);

                        if (nextId == 0)
                        {


                            var PendingtoSubmitCount = PendingToSubmitCount(pd.DocTypeId);
                            if (PendingtoSubmitCount > 0)
                                return RedirectToAction("Index_PendingToSubmit", new { id = pd.DocTypeId, IndexType = IndexType });
                            else
                                return RedirectToAction("Index", new { id = pd.DocTypeId, IndexType = IndexType });

                        }

                        return RedirectToAction("Detail", new { id = nextId, TransactionType = "submitContinue", IndexType = IndexType }).Success("Invoice " + pd.DocNo + " submitted successfully.");

                    }

                    else
                        return RedirectToAction("Index", new { id = pd.DocTypeId, IndexType = IndexType }).Success("Invoice " + pd.DocNo + " submitted successfully.");
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

            if (ModelState.IsValid)
            {
                JobInvoiceHeader pd = new JobInvoiceHeaderService(_unitOfWork).Find(Id);
                JobReceiveHeader pd2 = _JobReceiveHeaderService.Find(pd.JobReceiveHeaderId.Value);

                pd.ReviewCount = (pd.ReviewCount ?? 0) + 1;
                pd.ReviewBy += User.Identity.Name + ", ";

                pd2.ReviewCount = (pd.ReviewCount ?? 0) + 1;
                pd2.ReviewBy += User.Identity.Name + ", ";


                pd.ObjectState = Model.ObjectState.Modified;
                pd2.ObjectState = Model.ObjectState.Modified;

                db.JobInvoiceHeader.Add(pd);
                db.JobReceiveHeader.Add(pd2);

                try
                {
                    JobInvoiceReceiveDocEvents.onHeaderReviewEvent(this, new JobEventArgs(Id), ref db);
                }
                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    TempData["CSEXC"] += message;
                }

                db.SaveChanges();

                try
                {
                    JobInvoiceReceiveDocEvents.afterHeaderReviewEvent(this, new JobEventArgs(Id), ref db);
                }
                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    TempData["CSEXC"] += message;
                }

                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = pd.DocTypeId,
                    DocId = pd.JobInvoiceHeaderId,
                    ActivityType = (int)ActivityTypeContants.Reviewed,
                    UserRemark = UserRemark,
                    DocNo = pd.DocNo,
                    DocDate = pd.DocDate,
                    DocStatus = pd.Status,
                }));
                if (!string.IsNullOrEmpty(IsContinue) && IsContinue == "True")
                {
                    JobInvoiceHeader HEader = _JobInvoiceHeaderService.Find(Id);

                    int nextId = new NextPrevIdService(_unitOfWork).GetNextPrevId(Id, HEader.DocTypeId, User.Identity.Name, ForActionConstants.PendingToReview, "Web.JobInvoiceHeaders", "JobInvoiceHeaderId", PrevNextConstants.Next);
                    if (nextId == 0)
                    {
                        var PendingtoSubmitCount = PendingToReviewCount(HEader.DocTypeId);
                        if (PendingtoSubmitCount > 0)
                            return RedirectToAction("Index_PendingToReview", new { id = HEader.DocTypeId, IndexType = IndexType });
                        else
                            return RedirectToAction("Index", new { id = HEader.DocTypeId, IndexType = IndexType });

                    }

                    ViewBag.PendingToReview = PendingToReviewCount(Id);
                    return RedirectToAction("Detail", new { id = nextId, transactionType = "ReviewContinue", IndexType = IndexType });
                }


                else
                    return RedirectToAction("Index", new { id = pd.DocTypeId, IndexType = IndexType });
            }

            return View();
        }







        // GET: /ProductMaster/Delete/5

        private ActionResult Remove(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JobInvoiceHeader JobInvoiceHeader = db.JobInvoiceHeader.Find(id);

            if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, JobInvoiceHeader.DocTypeId, JobInvoiceHeader.ProcessId, this.ControllerContext.RouteData.Values["controller"].ToString(), "Remove") == false)
            {
                return PartialView("~/Views/Shared/PermissionDenied_Modal.cshtml").Warning("You don't have permission to do this task.");
            }

            #region DocTypeTimeLineValidation

            bool TimePlanValidation = true;
            string ExceptionMsg = "";
            try
            {
                TimePlanValidation = DocumentValidation.ValidateDocument(Mapper.Map<DocumentUniqueId>(JobInvoiceHeader), DocumentTimePlanTypeConstants.Delete, User.Identity.Name, out ExceptionMsg, out Continue);
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

            if (JobInvoiceHeader == null)
            {
                return HttpNotFound();
            }
            ReasonViewModel vm = new ReasonViewModel()
            {
                id = id,
            };

            return PartialView("_Reason", vm);
        }

        // POST: /ProductMaster/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(ReasonViewModel vm)
        {
            #region BeforeSave
            bool BeforeSave = true;
            try
            {
                BeforeSave = JobInvoiceReceiveDocEvents.beforeHeaderDeleteEvent(this, new JobEventArgs(vm.id), ref db);
            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXC"] += message;
                EventException = true;
            }

            if (!BeforeSave)
                TempData["CSEXC"] += "Failed validation before delete";
            #endregion

            if (ModelState.IsValid && BeforeSave && !EventException)
            {
                List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();

                int? StockHeaderId = 0;

                var temp = (from p in db.JobInvoiceHeader
                            where p.JobInvoiceHeaderId == vm.id
                            select p).FirstOrDefault();

                try
                {
                    JobInvoiceReceiveDocEvents.onHeaderDeleteEvent(this, new JobEventArgs(vm.id), ref db);
                }
                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    TempData["CSEXC"] += message;
                    EventException = true;
                }

                LogList.Add(new LogTypeViewModel
                {
                    ExObj = Mapper.Map<JobInvoiceHeader>(temp),
                });

                var attributes = (from A in db.JobInvoiceHeaderAttributes where A.HeaderTableId == vm.id select A).ToList();

                foreach (var ite2 in attributes)
                {
                    ite2.ObjectState = Model.ObjectState.Deleted;
                    db.JobInvoiceHeaderAttributes.Remove(ite2);
                }


                var temp2 = (from p in db.JobReceiveHeader
                             where p.JobReceiveHeaderId == temp.JobReceiveHeaderId
                             select p).FirstOrDefault();


                var line = (from p in db.JobInvoiceLine
                            where p.JobInvoiceHeaderId == vm.id
                            select p).ToList();

                new JobOrderLineStatusService(_unitOfWork).DeleteJobQtyOnInvoiceReceiveMultiple(temp.JobInvoiceHeaderId, ref db);

                var LedgerHeaders = (from p in db.LedgerHeader
                                     where p.LedgerHeaderId == temp.LedgerHeaderId
                                     select p).FirstOrDefault();

                if (LedgerHeaders != null)
                {
                    var LedgerLines = (from p in db.Ledger
                                       where p.LedgerHeaderId == LedgerHeaders.LedgerHeaderId
                                       select p).ToList();

                    foreach (var item in LedgerLines)
                    {
                        item.ObjectState = Model.ObjectState.Deleted;
                        db.Ledger.Remove(item);
                        //new LedgerLineService(_unitOfWork).Delete(item.LedgerLineId );
                    }

                    LedgerHeaders.ObjectState = Model.ObjectState.Deleted;
                    db.LedgerHeader.Remove(LedgerHeaders);
                }

                var LineIds = line.Select(m => m.JobInvoiceLineId).ToArray();

                var LineCharges = (from p in db.JobInvoiceLineCharge
                                   where LineIds.Contains(p.LineTableId)
                                   select p).ToList();

                foreach (var item in LineCharges)
                {
                    item.ObjectState = Model.ObjectState.Deleted;
                    db.JobInvoiceLineCharge.Remove(item);
                }

                foreach (var item in line)
                {

                    LogList.Add(new LogTypeViewModel
                    {
                        ExObj = Mapper.Map<JobInvoiceLine>(item),
                    });

                    JobInvoiceLineStatus Status = new JobInvoiceLineStatus();
                    Status.JobInvoiceLineId = item.JobInvoiceLineId;
                    db.JobInvoiceLineStatus.Attach(Status);
                    Status.ObjectState = Model.ObjectState.Deleted;

                    db.JobInvoiceLineStatus.Remove(Status);

                    item.ObjectState = Model.ObjectState.Deleted;
                    db.JobInvoiceLine.Remove(item);
                }

                var headercharges = (from p in db.JobInvoiceHeaderCharges
                                     where p.HeaderTableId == vm.id
                                     select p).ToList();

                foreach (var item in headercharges)
                {
                    item.ObjectState = Model.ObjectState.Deleted;
                    db.JobInvoiceHeaderCharges.Remove(item);
                }

                temp.ObjectState = Model.ObjectState.Deleted;
                db.JobInvoiceHeader.Remove(temp);


                if (temp2 != null)
                {
                    LogList.Add(new LogTypeViewModel
                    {
                        ExObj = Mapper.Map<JobReceiveHeader>(temp2),
                    });

                    StockHeaderId = temp2.StockHeaderId;

                    var line2 = (from p in db.JobReceiveLine
                                 where p.JobReceiveHeaderId == temp2.JobReceiveHeaderId
                                 select p).ToList();

                    var JRLineIds = line2.Select(m => m.JobReceiveLineId).ToArray();

                    var JobReceiveLineStatusRecords = (from p in db.JobReceiveLineStatus
                                                       where JRLineIds.Contains(p.JobReceiveLineId ?? 0)
                                                       select p).ToList();

                    var ProductUids = line2.Select(m => m.ProductUidId).ToArray();

                    var BarCodeRecords = (from p in db.ProductUid
                                          where ProductUids.Contains(p.ProductUIDId)
                                          select p).ToList();

                    List<int> StockIdList = new List<int>();
                    List<int> StockProcessIdList = new List<int>();

                    foreach (var item in JobReceiveLineStatusRecords)
                    {
                        item.ObjectState = Model.ObjectState.Deleted;
                        db.JobReceiveLineStatus.Remove(item);
                    }


                    foreach (var item in line2)
                    {
                        LogList.Add(new LogTypeViewModel
                        {
                            ExObj = Mapper.Map<JobReceiveLine>(item),
                        });

                        if (item.StockId != null)
                        {
                            StockIdList.Add((int)item.StockId);
                        }

                        if (item.StockProcessId != null)
                        {
                            StockProcessIdList.Add((int)item.StockProcessId);
                        }

                        var Productuid = item.ProductUidId;



                        if (Productuid != null && Productuid != 0)
                        {
                            ProductUid ProductUid = BarCodeRecords.Where(m => m.ProductUIDId == Productuid).FirstOrDefault();

                            if (!(item.ProductUidLastTransactionDocNo == ProductUid.LastTransactionDocNo && item.ProductUidLastTransactionDocTypeId == ProductUid.LastTransactionDocTypeId) || temp.SiteId == 17)
                            {

                                if ((temp2.DocNo != ProductUid.LastTransactionDocNo || temp2.DocTypeId != ProductUid.LastTransactionDocTypeId))
                                {
                                    ModelState.AddModelError("", "Bar Code Can't be deleted because this is already Proceed to another process.");
                                    return PartialView("_Reason", vm);
                                }



                                ProductUid.LastTransactionDocDate = item.ProductUidLastTransactionDocDate;
                                ProductUid.LastTransactionDocId = item.ProductUidLastTransactionDocId;
                                ProductUid.LastTransactionDocNo = item.ProductUidLastTransactionDocNo;
                                ProductUid.LastTransactionDocTypeId = item.ProductUidLastTransactionDocTypeId;
                                ProductUid.LastTransactionPersonId = item.ProductUidLastTransactionPersonId;
                                ProductUid.CurrenctGodownId = item.ProductUidCurrentGodownId;
                                ProductUid.CurrenctProcessId = item.ProductUidCurrentProcessId;
                                ProductUid.Status = item.ProductUidStatus;
                                if (!string.IsNullOrEmpty(ProductUid.ProcessesDone))
                                    ProductUid.ProcessesDone = ProductUid.ProcessesDone.Replace("|" + temp2.ProcessId.ToString() + "|", "");
                                ProductUid.ModifiedBy = User.Identity.Name;
                                ProductUid.ModifiedDate = DateTime.Now;

                                ProductUid.ObjectState = Model.ObjectState.Modified;
                                db.ProductUid.Add(ProductUid);

                                new StockUidService(_unitOfWork).DeleteStockUidForDocLineDB(item.JobReceiveHeaderId, temp2.DocTypeId, temp2.SiteId, temp2.DivisionId, ref db);

                            }
                        }


                        item.ObjectState = Model.ObjectState.Deleted;
                        db.JobReceiveLine.Remove(item);
                    }

                    var Boms = (from p in db.JobReceiveBom
                                where p.JobReceiveHeaderId == temp2.JobReceiveHeaderId
                                select p).ToList();

                    var StockProcessIds = Boms.Select(m => m.StockProcessId).ToArray();

                    var StockProcessRecords = (from p in db.StockProcess
                                               where StockProcessIds.Contains(p.StockProcessId)
                                               select p).ToList();

                    foreach (var item2 in Boms)
                    {
                        if (item2.StockProcessId != null)
                        {
                            var StockProcessRec = StockProcessRecords.Where(m => m.StockProcessId == item2.StockProcessId).FirstOrDefault();
                            StockProcessRec.ObjectState = Model.ObjectState.Deleted;
                            db.StockProcess.Remove(StockProcessRec);
                        }
                        item2.ObjectState = Model.ObjectState.Deleted;
                        db.JobReceiveBom.Remove(item2);
                    }

                    new StockService(_unitOfWork).DeleteStockDBMultiple(StockIdList, ref db, true);

                    new StockProcessService(_unitOfWork).DeleteStockProcessDBMultiple(StockProcessIdList, ref db, true);

                    temp2.ObjectState = Model.ObjectState.Deleted;
                    db.JobReceiveHeader.Remove(temp2);

                    if (StockHeaderId != null)
                    {
                        var STockHeader = (from p in db.StockHeader
                                           where p.StockHeaderId == StockHeaderId
                                           select p).FirstOrDefault();
                        STockHeader.ObjectState = Model.ObjectState.Deleted;
                        db.StockHeader.Remove(STockHeader);
                    }
                }


                XElement Modifications = new ModificationsCheckService().CheckChanges(LogList);

                try
                {
                    if (EventException)
                    { throw new Exception(); }

                    db.SaveChanges();
                }

                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    TempData["CSEXC"] += message;
                    return PartialView("_Reason", vm);
                }

                try
                {
                    JobInvoiceReceiveDocEvents.afterHeaderDeleteEvent(this, new JobEventArgs(vm.id), ref db);
                }
                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    TempData["CSEXC"] += message;
                }

                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = temp.DocTypeId,
                    DocId = vm.id,
                    ActivityType = (int)ActivityTypeContants.Deleted,
                    UserRemark = vm.Reason,
                    DocNo = temp.DocNo,
                    xEModifications = Modifications,
                    DocDate = temp.DocDate,
                    DocStatus = temp.Status,
                }));


                return Json(new { success = true });

            }
            return PartialView("_Reason", vm);
        }

        [HttpGet]
        public ActionResult NextPage(int DocId, int DocTypeId)//CurrentHeaderId
        {
            var nextId = new NextPrevIdService(_unitOfWork).GetNextPrevId(DocId, DocTypeId, User.Identity.Name, "", "Web.JobInvoiceHeaders", "JobInvoiceHeaderId", PrevNextConstants.Next);
            return Edit(nextId, "");
        }
        [HttpGet]
        public ActionResult PrevPage(int DocId, int DocTypeId)//CurrentHeaderId
        {
            var PrevId = new NextPrevIdService(_unitOfWork).GetNextPrevId(DocId, DocTypeId, User.Identity.Name, "", "Web.JobInvoiceHeaders", "JobInvoiceHeaderId", PrevNextConstants.Prev);
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

        [HttpGet]
        private ActionResult PrintOut(int id, string SqlProcForPrint)
        {
            String query = SqlProcForPrint;
            return Redirect((string)System.Configuration.ConfigurationManager.AppSettings["JobsDomain"] + "/Report_DocumentPrint/DocumentPrint/?DocumentId=" + id + "&queryString=" + query);
        }

        [HttpGet]
        public ActionResult Print(int id)
        {
            JobInvoiceHeader header = _JobInvoiceHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Drafted || header.Status == (int)StatusConstants.Import)
            {
                var SEttings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(header.DocTypeId, header.DivisionId, header.SiteId);
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
            JobInvoiceHeader header = _JobInvoiceHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Submitted || header.Status == (int)StatusConstants.Modified)
            {
                var SEttings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(header.DocTypeId, header.DivisionId, header.SiteId);
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
            JobInvoiceHeader header = _JobInvoiceHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Approved)
            {
                var SEttings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(header.DocTypeId, header.DivisionId, header.SiteId);
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

            JobInvoiceSettings SEttings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(Dt.DocumentTypeId, (int)System.Web.HttpContext.Current.Session["DivisionId"], (int)System.Web.HttpContext.Current.Session["SiteId"]);

            Dictionary<int, string> DefaultValue = new Dictionary<int, string>();

            //if (!Dt.ReportMenuId.HasValue)
            //    throw new Exception("Report Menu not configured in document types");

            if (!Dt.ReportMenuId.HasValue)
                return Redirect((string)System.Configuration.ConfigurationManager.AppSettings["JobsDomain"] + "/GridReport/GridReportLayout/?MenuName=Job Invoice Report&DocTypeId=" + id.ToString());

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
                ReportLine Process = new ReportLineService(_unitOfWork).GetReportLineByName("Process", header.ReportHeaderId);
                if (Process != null)
                    DefaultValue.Add(Process.ReportLineId, ((int)SEttings.ProcessId).ToString());
            }

            TempData["ReportLayoutDefaultValues"] = DefaultValue;

            return Redirect((string)System.Configuration.ConfigurationManager.AppSettings["JobsDomain"] + "/Report_ReportPrint/ReportPrint/?MenuId=" + Dt.ReportMenuId);

        }


        public int PendingToSubmitCount(int id)
        {
            return (_JobInvoiceHeaderService.GetJobInvoiceHeaderListPendingToSubmit(id, User.Identity.Name)).Count();
        }
        public int PendingToReviewCount(int id)
        {
            return (_JobInvoiceHeaderService.GetJobInvoiceHeaderListPendingToReview(id, User.Identity.Name)).Count();
        }


        public ActionResult Import(int id)//Document Type Id
        {
            //ControllerAction ca = new ControllerActionService(_unitOfWork).Find(id);
            JobInvoiceHeaderViewModel vm = new JobInvoiceHeaderViewModel();

            vm.DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            vm.SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            var settings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(id, vm.DivisionId, vm.SiteId);

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


        public ActionResult GeneratePrints(string Ids, int DocTypeId)
        {

            if (!string.IsNullOrEmpty(Ids))
            {
                int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
                int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

                var Settings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(DocTypeId, DivisionId, SiteId);

                if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, DocTypeId, Settings.ProcessId, this.ControllerContext.RouteData.Values["controller"].ToString(), "GeneratePrints") == false)
                {
                    return View("~/Views/Shared/PermissionDenied.cshtml").Warning("You don't have permission to do this task.");
                }

                string ReportSql = "";

                if (!string.IsNullOrEmpty(Settings.DocumentPrint))
                    ReportSql = db.ReportHeader.Where((m) => m.ReportName == Settings.DocumentPrint).FirstOrDefault().ReportSQL;

                try
                {

                    List<byte[]> PdfStream = new List<byte[]>();
                    foreach (var item in Ids.Split(',').Select(Int32.Parse))
                    {

                        DirectReportPrint drp = new DirectReportPrint();

                        var pd = db.JobInvoiceHeader.Find(item);

                        LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                        {
                            DocTypeId = pd.DocTypeId,
                            DocId = pd.JobInvoiceHeaderId,
                            ActivityType = (int)ActivityTypeContants.Print,
                            DocNo = pd.DocNo,
                            DocDate = pd.DocDate,
                            DocStatus = pd.Status,
                        }));

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
                                    JobInvoiceHeaderRDL cr = new JobInvoiceHeaderRDL();
                                    drp.CreateRDLFile("Std_JobInvoice_Print", cr.Create_Std_JobInvoice_Print());
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
                                if (Settings.SqlProcDocumentPrint_AfterSubmit == null || Settings.SqlProcDocumentPrint_AfterSubmit == "")
                                {
                                    JobInvoiceHeaderRDL cr = new JobInvoiceHeaderRDL();
                                    drp.CreateRDLFile("Std_JobInvoice_Print", cr.Create_Std_JobInvoice_Print());
                                    List<ListofQuery> QueryList = new List<ListofQuery>();
                                    QueryList = DocumentPrintData(item);
                                    Pdf = drp.DocumentPrint_New(QueryList, User.Identity.Name);
                                }
                                else
                                    Pdf = drp.DirectDocumentPrint(Settings.SqlProcDocumentPrint_AfterSubmit, User.Identity.Name, item);

                                PdfStream.Add(Pdf);
                            }
                            else if (pd.Status == (int)StatusConstants.Approved)
                            {
                                if (Settings.SqlProcDocumentPrint_AfterApprove == null || Settings.SqlProcDocumentPrint_AfterApprove == "")
                                {
                                    JobInvoiceHeaderRDL cr = new JobInvoiceHeaderRDL();
                                    drp.CreateRDLFile("Std_JobInvoice_Print", cr.Create_Std_JobInvoice_Print());
                                    List<ListofQuery> QueryList = new List<ListofQuery>();
                                    QueryList = DocumentPrintData(item);
                                    Pdf = drp.DocumentPrint_New(QueryList, User.Identity.Name);
                                }
                                else
                                    Pdf = drp.DirectDocumentPrint(Settings.SqlProcDocumentPrint_AfterApprove, User.Identity.Name, item);
                                PdfStream.Add(Pdf);
                            }
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

        private List<ListofQuery> DocumentPrintData(int item)
        {

            List<ListofQuery> DocumentPrintData = new List<ListofQuery>();
            String QueryMain;
            QueryMain = @"DECLARE @TotalAmount DECIMAL 
SET @TotalAmount = (SELECT Max(Amount) FROM Web.JobInvoiceHeaderCharges WHERE HeaderTableId = " + item + @" AND ChargeId = 34 ) 
 
DECLARE @ReturnAmount DECIMAL
DECLARE @DebitAmount DECIMAL
DECLARE @CreaditAmount DECIMAL
	  
SELECT 
@ReturnAmount=sum(CASE WHEN CT.ChargeTypeName IN ('Amount','CGST','SGST','IGST') AND JIRH.Nature='Return' THEN isnull(H.Amount,0) ELSE 0 END ),
@DebitAmount=sum(CASE WHEN JIRH.Nature='Debit Note' AND C.ChargeName='Net Amount' THEN isnull(H.Amount,0) ELSE 0 END ),
@CreaditAmount=sum(CASE WHEN JIRH.Nature='Credit Note' AND C.ChargeName='Net Amount' THEN isnull(H.Amount,0) ELSE 0 END )
FROM Web.JobInvoiceReturnLineCharges H
LEFT JOIN Web.ChargeTypes CT WITH (Nolock) ON CT.ChargeTypeId=H.ChargeTypeId
LEFT JOIN web.Charges C WITH (Nolock) ON C.ChargeId=H.ChargeId
LEFT JOIN Web.JobInvoiceReturnLines JIRL WITH (Nolock) ON JIRL.JobInvoiceReturnLineId=H.LineTableId
LEFT JOIN Web.JobInvoiceLines JIL WITH (Nolock) ON JIL.JobInvoiceLineId=JIRL.JobInvoiceLineId
LEFT JOIN Web.JobInvoiceReturnHeaders JIRH WITH (Nolock) ON JIRH.JobInvoiceReturnHeaderId=JIRL.JobInvoiceReturnHeaderId
WHERE JIL.JobInvoiceHeaderId=" + item + @"
	  
SET @TotalAmount=isnull(@TotalAmount,0)-isnull(@ReturnAmount,0)-isnull(@DebitAmount,0)+isnull(@CreaditAmount,0)
	  
	  	  
DECLARE @UnitDealCnt INT	
SELECT 
@UnitDealCnt=sum(CASE WHEN JOL.UnitId != JOL.DealunitId THEN 1 ELSE 0 END )
FROM Web.JobInvoiceLines L WITH (Nolock) 
LEFT JOIN web.JobReceiveLines JRL WITH (Nolock) ON JRL.JobReceiveLineId=L.JobReceiveLineId
LEFT JOIN Web.JobOrderLines JOL WITH (Nolock) ON JOL.JobOrderLineId=JRL.JobOrderLineId
WHERE L.JobInvoiceHeaderId=" + item + @"
  
	
DECLARE @DocDate DATETIME
DECLARE @Site INT 
DECLARE @Division INT 
SELECT  @DocDate=DocDate,@Site=SiteId,@Division=DivisionId FROM Web.JobInvoiceHeaders WHERE JobInvoiceHeaderId=" + item + @"

	
	
SELECT	H.JobInvoiceHeaderId,H.DocTypeId,H.DocNo,DocIdCaption+' No' AS DocIdCaption ,
H.SiteId,H.DivisionId,H.DocDate,DTS.DocIdCaption +' Date' AS DocIdCaptionDate ,	
H.JobWorkerDocNo AS PartyDocNo,	DTS.PartyCaption +' Doc No' AS PartyDocCaption,format(H.JobWorkerDocDate,'dd/MMM/yy') AS PartyDocDate,
DTS.PartyCaption +' Doc Date' AS PartyDocDateCaption,H.CreditDays,H.Remark,DT.DocumentTypeShortName,	
H.ModifiedBy +' ' + Replace(replace(convert(NVARCHAR, H.ModifiedDate, 106), ' ', '/'),'/20','/') + substring (convert(NVARCHAR,H.ModifiedDate),13,7) AS ModifiedBy,
H.ModifiedDate,(CASE WHEN Isnull(H.Status,0)=0 OR Isnull(H.Status,0)=8 THEN 0 ELSE 1 END)  AS Status,
CUR.Name AS CurrencyName,(CASE WHEN SPR.[Party GST NO] IS NULL THEN 'Yes' ELSE 'No' END ) AS ReverseCharge,
VDC.CompanyName,P.Name AS PartyName, DTS.PartyCaption AS  PartyCaption, P.Suffix AS PartySuffix,	
isnull(PA.Address,'')+' '+isnull(C.CityName,'')+','+isnull(PA.ZipCode,'')+(CASE WHEN isnull(CS.StateName,'') <> isnull(S.StateName,'') AND SPR.[Party GST NO] IS NOT NULL THEN ',State : '+isnull(S.StateName,'')+(CASE WHEN S.StateCode IS NULL THEN '' ELSE ', Code : '+S.StateCode END)    ELSE '' END ) AS PartyAddress,
isnull(S.StateName,'') AS PartyStateName,isnull(S.StateCode,'') AS PartyStateCode,	
P.Mobile AS PartyMobileNo,	SPR.*,
	--Plan Detail
	JRH.DocNo  AS PlanNo,DTS.ContraDocTypeCaption,
	--Caption Fields	
	DTS.SignatoryMiddleCaption,DTS.SignatoryRightCaption,
	--Line Table
	PD.ProductName,DTS.ProductCaption,U.UnitName,U.DecimalPlaces,DU.UnitName AS DealUnitName,DTS.DealQtyCaption,DU.DecimalPlaces AS DealDecimalPlaces,
	isnull(L.Qty,0) AS Qty,isnull(L.Rate,0) AS Rate,isnull(L.Amount,0) AS Amount,isnull(L.DealQty,0) AS DealQty,
	D1.Dimension1Name,DTS.Dimension1Caption,D2.Dimension2Name,DTS.Dimension2Caption,D3.Dimension3Name,DTS.Dimension3Caption,D4.Dimension4Name,DTS.Dimension4Caption,
    DTS.SpecificationCaption,DTS.SignatoryleftCaption,L.Remark AS LineRemark,
	--L.DiscountPer AS DiscountPer,L.DiscountAmt AS DiscountAmt,
	Convert(DECIMAL(18,2),L.RateDiscountPer) AS DiscountPer,Convert(DECIMAL(18,2),L.RateDiscountAmt) AS DiscountAmt,
	--STC.Code AS SalesTaxProductCodes,
	(CASE WHEN H.ProcessId IN (26,28) THEN  STC.Code ELSE PSSTC.Code END)  AS SalesTaxProductCodes ,
	(CASE WHEN DTS.PrintProductGroup >0 THEN isnull(PG.ProductGroupName,'') ELSE '' END)+(CASE WHEN DTS.PrintProductdescription >0 THEN isnull(','+PD.Productdescription,'') ELSE '' END) AS ProductGroupName,
	DTS.ProductGroupCaption,isnull(CGPD.PrintingDescription,CGPD.ChargeGroupProductName) AS ChargeGroupProductName,
	
   --Receive Lines
	 DTS.ProductUidCaption,PU.ProductUidName,
	(CASE WHEN isnull(JRL.LossQty,0) >0 THEN isnull(JRL.LossQty,0) ELSE NULL END)  AS LossQty,
	(CASE WHEN isnull(JRL.Qty,0) <> isnull(L.Qty,0) THEN CASE WHEN isnull(JRL.Qty,0) <> 0 THEN isnull(JRL.Qty,0) ELSE NULL END   ELSE NULL END) AS RecQty,
	JRL.LotNo AS LotNo, 
	(CASE WHEN DTS.PrintSpecification >0 THEN   JRL.Specification ELSE '' END)  AS Specification,
	--Formula Fields
	isnull(@TotalAmount,0) AS NetAmount,  
	isnull(@ReturnAmount,0) AS ReturnAmount,
	isnull(@DebitAmount,0) AS DebitAmount,
	isnull(@CreaditAmount,0) AS CreaditAmount,   	
	--SalesTaxGroupPersonId
	CGP.ChargeGroupPersonName,
	--Other Fields
	@UnitDealCnt  AS DealUnitCnt,		
	(CASE WHEN Isnull(H.Status,0)=0 OR Isnull(H.Status,0)=8 THEN 'Provisional ' +isnull(DT.PrintTitle,DT.DocumentTypeName) ELSE isnull(DT.PrintTitle,DT.DocumentTypeName) END) AS ReportTitle, 
	'Std_JobInvoice_Print.rdl' AS ReportName,		
	SalesTaxGroupProductCaption,
	SDS.SalesTaxProductCodeCaption
	FROM Web.JobInvoiceHeaders H WITH (Nolock)
	LEFT JOIN web.DocumentTypes DT WITH (Nolock) ON DT.DocumentTypeId=H.DocTypeId
	LEFT JOIN Web._DocumentTypeSettings DTS WITH (Nolock) ON DTS.DocumentTypeId=DT.DocumentTypeId
	LEFT JOIN Web.JobInvoiceSettings JIS WITH (Nolock) ON JIS.DocTypeId=DT.DocumentTypeId AND JIS.SiteId =H.siteid AND H.DivisionId=JIS.DivisionId
	LEFT JOIN web.ViewDivisionCompany VDC WITH (Nolock) ON VDC.DivisionId=H.DivisionId
	LEFT JOIN Web.Sites SI WITH (Nolock) ON SI.SiteId=H.SiteId
	LEFT JOIN Web.Divisions DIV WITH (Nolock) ON DIV.DivisionId=H.DivisionId	
	LEFT JOIN Web.Companies Com ON Com.CompanyId = DIV.CompanyId
	LEFT JOIN Web.Cities CC WITH (Nolock) ON CC.CityId=Com.CityId
	LEFT JOIN Web.States CS WITH (Nolock) ON CS.StateId=CC.StateId
	LEFT JOIN Web.Processes PS WITH (Nolock) ON PS.ProcessId=H.ProcessId
	LEFT JOIN Web.SalesTaxProductCodes PSSTC WITH (Nolock) ON PSSTC.SalesTaxProductCodeId=PS.SalesTaxProductCodeId
	LEFT JOIN Web.People P WITH (Nolock) ON P.PersonID=H.JobWorkerId
	LEFT JOIN (SELECT TOP 1 * FROM web.SiteDivisionSettings WHERE @DocDate BETWEEN StartDate AND IsNull(EndDate,getdate()) AND SiteId=@Site AND DivisionId=@Division ORDER BY StartDate) SDS  ON H.DivisionId = SDS.DivisionId AND H.SiteId = SDS.SiteId	
   	LEFT JOIN (SELECT * FROM Web.PersonAddresses WITH (nolock) WHERE AddressType IS NULL) PA ON PA.PersonId = P.PersonID 
	LEFT JOIN Web.Cities C WITH (nolock) ON C.CityId = PA.CityId
	LEFT JOIN Web.States S WITH (Nolock) ON S.StateId=C.StateId
	LEFT JOIN web.ChargeGroupPersons CGP WITH (Nolock) ON CGP.ChargeGroupPersonId=H.SalesTaxGroupPersonId
	LEFT JOIN Web.Currencies CUR WITH (Nolock) ON CUR.Id=H.CurrencyId
  	LEFT JOIN Web.JobInvoiceLines L WITH (Nolock) ON L.JobInvoiceHeaderId=H.JobInvoiceHeaderId
	LEFT JOIN Web.JobReceiveLines JRL WITH (Nolock) ON JRL.JobReceiveLineId=L.JobReceiveLineId
	LEFT JOIN web.ProductUids PU WITH (Nolock) ON PU.ProductUidId=JRL.ProductUidId	
	LEFT JOIN Web.JobReceiveHeaders JRH WITH (Nolock) ON JRH.JobReceiveHeaderId=JRL.JobReceiveHeaderId
	LEFT JOIN Web.JobOrderLines JOL WITH (Nolock) ON JOL.JobOrderLineId=JRL.JobOrderLineId
   	LEFT JOIN Web.ProdOrderLines POl WITH (Nolock) ON POl.ProdOrderLineId=JOL.ProdOrderLineId
    LEFT JOIN Web.ProdOrderHeaders POH WITH (Nolock) ON POH.ProdOrderHeaderId=POL.ProdOrderHeaderId
	LEFT JOIN web.Products PD WITH (Nolock) ON PD.ProductId=isnull(JOL.ProductId,JRL.ProductId)
	LEFT JOIN web.ProductGroups PG WITH (Nolock) ON PG.ProductGroupId=PD.ProductGroupid
	LEFT JOIN Web.SalesTaxProductCodes STC WITH (Nolock) ON STC.SalesTaxProductCodeId= IsNull(PD.SalesTaxProductCodeId, Pg.DefaultSalesTaxProductCodeId)
	LEFT JOIN Web.Dimension1 D1 WITH (Nolock) ON D1.Dimension1Id=JOL.Dimension1Id
	LEFT JOIN web.Dimension2 D2 WITH (Nolock) ON D2.Dimension2Id=JOL.Dimension2Id
	LEFT JOIN web.Dimension3 D3 WITH (Nolock) ON D3.Dimension3Id=JOL.Dimension3Id
	LEFT JOIN Web.Dimension4 D4 WITH (nolock) ON D4.Dimension4Id=JOL.Dimension4Id
	LEFT JOIN web.Units U WITH (Nolock) ON U.UnitId=PD.UnitId
	LEFT JOIN web.Units DU WITH (Nolock) ON DU.UnitId=JOL.DealUnitId
	LEFT JOIN Web.Std_PersonRegistrations SPR WITH (Nolock) ON SPR.CustomerId=H.JobWorkerId
	LEFT JOIN web.ChargeGroupProducts CGPD WITH (Nolock) ON L.SalesTaxGroupProductId = CGPD.ChargeGroupProductId	
   	WHERE H.JobInvoiceHeaderId=" + item + @"
   	ORDER BY L.Sr";

            ListofQuery QryMain = new ListofQuery();
            QryMain.Query = QueryMain;
            QryMain.QueryName = nameof(QueryMain);
            DocumentPrintData.Add(QryMain);


            String QueryCalculation;
            QueryCalculation = @"DECLARE @StrGrossAmount AS NVARCHAR(50)  
DECLARE @StrBasicExciseDuty AS NVARCHAR(50)  
DECLARE @StrExciseECess AS NVARCHAR(50)  
DECLARE @StrExciseHECess AS NVARCHAR(50)  

DECLARE @StrSalesTaxTaxableAmt AS NVARCHAR(50)  
DECLARE @StrVAT AS NVARCHAR(50)  
DECLARE @StrSAT AS NVARCHAR(50) 
DECLARE @StrCST AS NVARCHAR(50) 

SET @StrGrossAmount = 'Gross Amount'
SET @StrBasicExciseDuty = 'Basic Excise Duty'
SET @StrExciseECess ='Excise ECess'
SET @StrExciseHECess = 'Excise HECess'

SET @StrSalesTaxTaxableAmt = 'Sales Tax Taxable Amt'
SET @StrVAT = 'VAT'
SET @StrSAT = 'SAT'
SET @StrCST = 'CST'

DECLARE @Qry NVARCHAR(Max);
SET @Qry = '
		DECLARE @GrossAmount AS DECIMAL 
		DECLARE @BasicExciseDutyAmount AS DECIMAL 
		DECLARE @SalesTaxTaxableAmt AS DECIMAL 
		
		SELECT @GrossAmount = sum ( CASE WHEN C.ChargeName = ''' + @StrGrossAmount + ''' THEN  H.Amount  ELSE 0 END ) ,
		@BasicExciseDutyAmount = sum( CASE WHEN C.ChargeName = ''' + @StrBasicExciseDuty + ''' THEN  H.Amount  ELSE 0 END ) ,
		@SalesTaxTaxableAmt = sum( CASE WHEN C.ChargeName = ''' + @StrSalesTaxTaxableAmt + ''' THEN  H.Amount  ELSE 0 END )
		FROM web.jobInvoiceheadercharges H
		LEFT JOIN web.ChargeTypes CT ON CT.ChargeTypeId = H.ChargeTypeId 
		LEFT JOIN web.Charges C ON C.ChargeId = H.ChargeId 
		WHERE H.Amount <> 0 AND H.HeaderTableId	= ' + Convert(Varchar," + item + @" ) + '
		GROUP BY H.HeaderTableId
		
		
		SELECT H.Id, H.HeaderTableId, H.Sr, C.ChargeName, H.Amount, H.ChargeTypeId,  CT.ChargeTypeName, 
		--CASE WHEN C.ChargeName = ''Vat'' THEN ( H.Amount*100/ @GrossAmount ) ELSE H.Rate End  AS Rate,
		CASE 
		WHEN @SalesTaxTaxableAmt>0 And C.ChargeName IN ( ''' + @StrVAT + ''',''' + @StrSAT + ''',''' + @StrCST+ ''')  THEN ( H.Amount*100/ @SalesTaxTaxableAmt   ) 
		WHEN @GrossAmount>0 AND C.ChargeName IN ( ''' + @StrBasicExciseDuty + ''')  THEN ( H.Amount*100/ @GrossAmount   ) 
		WHEN  @BasicExciseDutyAmount>0 AND  C.ChargeName IN ( ''' + @StrExciseECess + ''', ''' +@StrExciseHECess+ ''')  THEN ( H.Amount*100/ @BasicExciseDutyAmount   ) 
		ELSE 0 End  AS Rate,
		''StdDocPrintSub_CalculationHeader.rdl'' AS ReportName,
		''Transaction Charges'' AS ReportTitle     
		FROM  web.jobInvoiceheadercharges  H
		LEFT JOIN web.ChargeTypes CT ON CT.ChargeTypeId = H.ChargeTypeId 
		LEFT JOIN web.Charges C ON C.ChargeId = H.ChargeId 
		WHERE  ( isnull(H.ChargeTypeId,0) <> ''4'' OR C.ChargeName = ''Net Amount'') AND H.Amount <> 0
--WHERE  1=1
		AND H.HeaderTableId	= ' + Convert(Varchar," + item + @"  ) + ''
		
	--PRINT @Qry; 	
	
	DECLARE @TmpData TABLE
	(
	id BIGINT,
	HeaderTableId INT,
	Sr INT,
	ChargeName NVARCHAR(50),
	Amount DECIMAL(18,4),
	ChargeTypeId INT,
	ChargeTypeName NVARCHAR(50),
	Rate DECIMAL(38,20),
	ReportName nVarchar(255),
	ReportTitle nVarchar(255)
	);
	
	
	Insert Into @TmpData EXEC(@Qry)
	SELECT id,HeaderTableId,Sr,ChargeName,Amount,ChargeTypeId,ChargeTypeName,Rate,ReportName 
	FROM @TmpData
	ORDER BY Sr	";


            ListofQuery QryCalculation = new ListofQuery();
            QryCalculation.Query = QueryCalculation;
            QryCalculation.QueryName = nameof(QueryCalculation);
            DocumentPrintData.Add(QryCalculation);


            String QueryGSTSummary;
            QueryGSTSummary = @"DECLARE @Qry NVARCHAR(Max);

SET @Qry = '
SELECT  
--CASE WHEN PS.ProcessName IN (''Purchase'',''Sale'') THEN isnull(STGP.PrintingDescription,STGP.ChargeGroupProductName) ELSE PS.GSTPrintDesc END as ChargeGroupProductName, 
isnull(STGP.PrintingDescription,STGP.ChargeGroupProductName) as ChargeGroupProductName, 
Sum(CASE WHEN ct.ChargeTypeName =''Sales Taxable Amount'' THEN lc.Amount ELSE 0 End) AS TaxableAmount,
Sum(CASE WHEN ct.ChargeTypeName =''IGST'' THEN lc.Amount ELSE 0 End) AS IGST,
Sum(CASE WHEN ct.ChargeTypeName =''CGST'' THEN lc.Amount ELSE 0 End) AS CGST,
Sum(CASE WHEN ct.ChargeTypeName =''SGST'' THEN lc.Amount ELSE 0 End) AS SGST,
Sum(CASE WHEN ct.ChargeTypeName =''GST Cess'' THEN lc.Amount ELSE 0 End) AS GSTCess,
''StdDocPrintSub_GSTSummary.rdl'' AS ReportName
FROM Web.JobInvoiceLines L
LEFT JOIN Web.JobInvoiceLineCharges LC ON L.JobInvoiceLineId = LC.LineTableId 
LEFT JOIN web.jobInvoiceheaders H ON H.JobInvoiceHeaderId = L.JobInvoiceHeaderId
LEFT JOIN Web.Processes PS WITH (Nolock) ON PS.ProcessId=H.ProcessId
LEFT JOIN Web.Charges C ON C.ChargeId=LC.ChargeId
LEFT JOIN web.ChargeTypes CT ON LC.ChargeTypeId = CT.ChargeTypeId 
LEFT JOIN web.ChargeGroupProducts STGP ON L.SalesTaxGroupProductId = STGP.ChargeGroupProductId
WHERE L.JobInvoiceHeaderId =" + item + @" 
GROUP BY isnull(STGP.PrintingDescription,STGP.ChargeGroupProductName)
--GROUP BY CASE WHEN PS.ProcessName IN (''Purchase'',''Sale'') THEN isnull(STGP.PrintingDescription,STGP.ChargeGroupProductName) ELSE PS.GSTPrintDesc END '

--PRINT @Qry;
EXEC(@Qry);	";


            ListofQuery QryGSTSummary = new ListofQuery();
            QryGSTSummary.Query = QueryGSTSummary;
            QryGSTSummary.QueryName = nameof(QueryGSTSummary);
            DocumentPrintData.Add(QryGSTSummary);

            return DocumentPrintData;

        }

        public ActionResult GetCustomPerson(string searchTerm, int pageSize, int pageNum, int filter, int? filter2)//DocTypeId
        {
            var Query = _JobInvoiceHeaderService.GetCustomPerson(filter, searchTerm, filter2);
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

        public JsonResult GetJobWorkerDetailJson(int JobWorkerId)
        {
            return Json(new JobInvoiceHeaderService(_unitOfWork).GetJobWorkerDetail(JobWorkerId));
        }

        public JsonResult GetProcessPermissionJson(int DocTypeId, int ProcessId)
        {
            var temp = new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, DocTypeId, ProcessId, this.ControllerContext.RouteData.Values["controller"].ToString(), "Create");

            return Json(temp);
        }

        #region submitValidation
        public bool Submitvalidation(int id, out string Msg)
        {
            Msg = "";
            int Stockline = (new JobInvoiceLineService(_unitOfWork).GetLineListForIndex(id)).Count();
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
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpGet]
        public ActionResult _CreateInvoiceReturn(int id)
        {
            JobInvoiceReturn JobInvoiceReturn = new JobInvoiceReturn();
            JobInvoiceReturn.JobInvoiceHeaderId = id;
            ViewBag.ReasonList = new ReasonService(_unitOfWork).GetReasonList(TransactionDocCategoryConstants.JobInvoiceReturn).ToList();
            JobInvoiceReturn.DocDate = DateTime.Now;
            return PartialView("_InvoiceReturn", JobInvoiceReturn);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult _CreateInvoiceReturnPost(JobInvoiceReturn svm)
        {
            int Cnt = 0;
            int Serial = 0;
            int pk = 0;
            int Gpk = 0;
            int PersonCount = 0;
            bool HeaderChargeEdit = false;

            JobInvoiceHeader JobInvoiceHeader = new JobInvoiceHeaderService(_unitOfWork).Find(svm.JobInvoiceHeaderId);
            JobReceiveHeader JobReceiveHeader = new JobReceiveHeaderService(_unitOfWork).Find(JobInvoiceHeader.JobReceiveHeaderId ?? 0);
            var DispatchLine = new JobReceiveLineService(_unitOfWork).GetJobReceiveLineList(JobReceiveHeader.JobReceiveHeaderId);
            List<LineChargeRates> LineChargeRates = new List<LineChargeRates>();

            var JobInvoiceSettings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(JobInvoiceHeader.DocTypeId, JobInvoiceHeader.DivisionId, JobInvoiceHeader.SiteId);
            int InvoiceRetHeaderDocTypeId = 0;

            if (JobInvoiceSettings.JobInvoiceReturnDocTypeId == null)
            {
                string message = "Invoice Return Document Type is not difined in settings.";
                ModelState.AddModelError("", message);
                return PartialView("_InvoiceReturn", svm);
            }
            else
            {
                InvoiceRetHeaderDocTypeId = (int)JobInvoiceSettings.JobInvoiceReturnDocTypeId;
            }


            if (ModelState.IsValid)
            {
                var JobInvoiceLineList = (from p in db.ViewJobInvoiceBalance
                                          join l in db.JobInvoiceLine on p.JobInvoiceLineId equals l.JobInvoiceLineId into linetable
                                          from linetab in linetable.DefaultIfEmpty()
                                          join t in db.JobInvoiceHeader on p.JobInvoiceHeaderId equals t.JobInvoiceHeaderId into table
                                          from tab in table.DefaultIfEmpty()
                                          join t1 in db.JobReceiveLine on p.JobReceiveLineId equals t1.JobReceiveLineId into table1
                                          from tab1 in table1.DefaultIfEmpty()
                                          join product in db.Product on tab1.ProductId equals product.ProductId into table2
                                          from tab2 in table2.DefaultIfEmpty()
                                          where p.JobInvoiceHeaderId == JobInvoiceHeader.JobInvoiceHeaderId
                                          && p.BalanceQty > 0
                                          select new JobInvoiceReturnLineViewModel
                                          {
                                              Dimension1Name = tab1.Dimension1.Dimension1Name,
                                              Dimension2Name = tab1.Dimension2.Dimension2Name,
                                              Specification = tab1.Specification,
                                              InvoiceBalQty = p.BalanceQty,
                                              Qty = p.BalanceQty,
                                              JobInvoiceHeaderDocNo = tab.DocNo,
                                              ProductName = tab2.ProductName,
                                              ProductId = p.ProductId,
                                              JobInvoiceLineId = p.JobInvoiceLineId,
                                              UnitId = tab2.UnitId,
                                              UnitConversionMultiplier = tab1.UnitConversionMultiplier,
                                              DealUnitId = linetab.DealUnitId,
                                              Rate = linetab.Rate,
                                              Amount = linetab.Amount,
                                              unitDecimalPlaces = tab2.Unit.DecimalPlaces,
                                              DealunitDecimalPlaces = linetab.DealUnit.DecimalPlaces,
                                              DiscountPer = linetab.RateDiscountPer,
                                              ProductUidName = tab1.ProductUid.ProductUidName,
                                              SalesTaxGroupProductId = linetab.SalesTaxGroupProductId
                                          }).ToList();

                if (JobInvoiceLineList.Sum(i => i.InvoiceBalQty) > 0)
                {
                    JobInvoiceSettings Settings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(InvoiceRetHeaderDocTypeId, JobInvoiceHeader.DivisionId, JobInvoiceHeader.SiteId);

                    JobReturnHeader GoodsRetHeader = new JobReturnHeader();
                    GoodsRetHeader.DocTypeId = (int)Settings.JobReturnDocTypeId;
                    GoodsRetHeader.DocDate = svm.DocDate;
                    GoodsRetHeader.DocNo = new DocumentTypeService(_unitOfWork).FGetNewDocNo("DocNo", ConfigurationManager.AppSettings["DataBaseSchema"] + ".JobReturnHeaders", GoodsRetHeader.DocTypeId, svm.DocDate, JobInvoiceHeader.DivisionId, JobInvoiceHeader.SiteId);
                    GoodsRetHeader.SiteId = JobInvoiceHeader.SiteId;
                    GoodsRetHeader.DivisionId = JobInvoiceHeader.DivisionId;
                    GoodsRetHeader.ProcessId = JobInvoiceHeader.ProcessId;
                    GoodsRetHeader.JobWorkerId = (int)JobInvoiceHeader.JobWorkerId;
                    GoodsRetHeader.ReasonId = svm.ReasonId;
                    GoodsRetHeader.GodownId = JobReceiveHeader.GodownId;
                    GoodsRetHeader.Remark = svm.Remark;
                    GoodsRetHeader.CreatedDate = DateTime.Now;
                    GoodsRetHeader.ModifiedDate = DateTime.Now;
                    GoodsRetHeader.CreatedBy = User.Identity.Name;
                    GoodsRetHeader.ModifiedBy = User.Identity.Name;
                    GoodsRetHeader.ObjectState = Model.ObjectState.Added;
                    //new JobReturnHeaderService(_unitOfWork).Create(GoodsRetHeader);
                    db.JobReturnHeader.Add(GoodsRetHeader);

                    JobInvoiceReturnHeader InvoiceRetHeader = new JobInvoiceReturnHeader();
                    InvoiceRetHeader.DocTypeId = InvoiceRetHeaderDocTypeId;
                    InvoiceRetHeader.DocDate = svm.DocDate;
                    InvoiceRetHeader.DocNo = new DocumentTypeService(_unitOfWork).FGetNewDocNo("DocNo", ConfigurationManager.AppSettings["DataBaseSchema"] + ".JobInvoiceReturnHeaders", InvoiceRetHeader.DocTypeId, svm.DocDate, JobInvoiceHeader.DivisionId, JobInvoiceHeader.SiteId);
                    InvoiceRetHeader.JobWorkerId = (int)JobInvoiceHeader.JobWorkerId;
                    InvoiceRetHeader.SiteId = JobInvoiceHeader.SiteId;
                    InvoiceRetHeader.DivisionId = JobInvoiceHeader.DivisionId;
                    InvoiceRetHeader.ProcessId = JobInvoiceHeader.ProcessId;
                    InvoiceRetHeader.ReasonId = svm.ReasonId;
                    InvoiceRetHeader.Remark = svm.Remark ?? "Cancellation";
                    InvoiceRetHeader.Nature = TransactionNatureConstants.Return;
                    InvoiceRetHeader.CreatedDate = DateTime.Now;
                    InvoiceRetHeader.ModifiedDate = DateTime.Now;
                    InvoiceRetHeader.CreatedBy = User.Identity.Name;
                    InvoiceRetHeader.ModifiedBy = User.Identity.Name;
                    InvoiceRetHeader.JobReturnHeaderId = GoodsRetHeader.JobReturnHeaderId;
                    InvoiceRetHeader.SalesTaxGroupPersonId = JobInvoiceHeader.SalesTaxGroupPersonId;
                    InvoiceRetHeader.ObjectState = Model.ObjectState.Added;
                    //new JobInvoiceReturnHeaderService(db).Create(InvoiceRetHeader);
                    db.JobInvoiceReturnHeader.Add(InvoiceRetHeader);

                    int CalculationId = (int)Settings.CalculationId;

                    List<LineDetailListViewModel> LineList = new List<LineDetailListViewModel>();
                    List<HeaderChargeViewModel> HeaderCharges = new List<HeaderChargeViewModel>();
                    List<LineChargeViewModel> LineCharges = new List<LineChargeViewModel>();


                    foreach (var item in JobInvoiceLineList)
                    {
                        decimal balqty = (from p in db.ViewJobInvoiceBalance
                                          where p.JobInvoiceLineId == item.JobInvoiceLineId
                                          select p.BalanceQty).FirstOrDefault();


                        if (item.Qty > 0 && item.Qty <= balqty)
                        {
                            JobInvoiceReturnLine line = new JobInvoiceReturnLine();
                            //var receipt = new JobReceiveLineService(_unitOfWork).Find(item.JobReceiveLineId );


                            line.JobInvoiceReturnHeaderId = InvoiceRetHeader.JobInvoiceReturnHeaderId;
                            line.JobInvoiceLineId = item.JobInvoiceLineId;
                            line.Qty = item.Qty;
                            line.Sr = Serial++;
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
                            line.JobInvoiceReturnLineId = pk;


                            JobReturnLine GLine = Mapper.Map<JobInvoiceReturnLine, JobReturnLine>(line);
                            GLine.JobReceiveLineId = new JobInvoiceLineService(_unitOfWork).Find(line.JobInvoiceLineId).JobReceiveLineId;
                            GLine.JobReturnHeaderId = GoodsRetHeader.JobReturnHeaderId;
                            GLine.JobReturnLineId = Gpk;
                            GLine.Qty = line.Qty;
                            GLine.ObjectState = Model.ObjectState.Added;


                            JobReceiveLine JobReceiveLine = new JobReceiveLineService(_unitOfWork).Find(GLine.JobReceiveLineId);

                            StockViewModel StockViewModel = new StockViewModel();


                            if (Cnt == 0)
                            {
                                StockViewModel.StockHeaderId = GoodsRetHeader.StockHeaderId ?? 0;
                            }
                            else
                            {
                                if (GoodsRetHeader.StockHeaderId != null && GoodsRetHeader.StockHeaderId != 0)
                                {
                                    StockViewModel.StockHeaderId = (int)GoodsRetHeader.StockHeaderId;
                                }
                                else
                                {
                                    StockViewModel.StockHeaderId = -1;
                                }

                            }

                            StockViewModel.StockId = -Cnt;

                            StockViewModel.DocHeaderId = GoodsRetHeader.JobReturnHeaderId;
                            StockViewModel.DocLineId = JobReceiveLine.JobReceiveLineId;
                            StockViewModel.DocTypeId = GoodsRetHeader.DocTypeId;
                            StockViewModel.StockHeaderDocDate = GoodsRetHeader.DocDate;
                            StockViewModel.StockDocDate = GoodsRetHeader.DocDate;
                            StockViewModel.DocNo = GoodsRetHeader.DocNo;
                            StockViewModel.DivisionId = GoodsRetHeader.DivisionId;
                            StockViewModel.SiteId = GoodsRetHeader.SiteId;
                            StockViewModel.CurrencyId = null;
                            StockViewModel.PersonId = GoodsRetHeader.JobWorkerId;
                            StockViewModel.ProductId = JobReceiveLine.ProductId;
                            StockViewModel.ProductUidId = JobReceiveLine.ProductUidId;
                            StockViewModel.HeaderFromGodownId = null;
                            StockViewModel.HeaderGodownId = null;
                            StockViewModel.HeaderProcessId = Settings.ProcessId;
                            StockViewModel.GodownId = GoodsRetHeader.GodownId;
                            StockViewModel.Remark = svm.Remark;
                            StockViewModel.Status = 0;
                            StockViewModel.ProcessId = null;
                            StockViewModel.LotNo = null;
                            StockViewModel.CostCenterId = null;
                            StockViewModel.Qty_Iss = 0;
                            StockViewModel.Qty_Rec = GLine.Qty;
                            StockViewModel.Rate = null;
                            StockViewModel.ExpiryDate = null;
                            StockViewModel.Specification = JobReceiveLine.Specification;
                            StockViewModel.Dimension1Id = JobReceiveLine.Dimension1Id;
                            StockViewModel.Dimension2Id = JobReceiveLine.Dimension2Id;
                            StockViewModel.CreatedBy = User.Identity.Name;
                            StockViewModel.CreatedDate = DateTime.Now;
                            StockViewModel.ModifiedBy = User.Identity.Name;
                            StockViewModel.ModifiedDate = DateTime.Now;

                            string StockPostingError = "";
                            StockPostingError = new StockService(_unitOfWork).StockPostDB(ref StockViewModel, ref db);

                            if (StockPostingError != "")
                            {
                                string message = StockPostingError;
                                ModelState.AddModelError("", message);
                                return PartialView("_InvoiceReturn", svm);
                            }


                            if (Cnt == 0)
                            {
                                GoodsRetHeader.StockHeaderId = StockViewModel.StockHeaderId;
                            }


                            GLine.StockId = StockViewModel.StockId;


                            //new JobReturnLineService(_unitOfWork).Create(GLine);
                            GLine.ObjectState = Model.ObjectState.Added;
                            db.JobReturnLine.Add(GLine);


                            line.JobReturnLineId = GLine.JobReturnLineId;
                            line.ObjectState = Model.ObjectState.Added;
                            //new JobInvoiceReturnLineService(db).Create(line);
                            db.JobInvoiceReturnLine.Add(line);

                            LineList.Add(new LineDetailListViewModel { Amount = line.Amount, Rate = line.Rate, LineTableId = line.JobInvoiceReturnLineId, HeaderTableId = item.JobInvoiceReturnHeaderId, PersonID = InvoiceRetHeader.JobWorkerId, DealQty = line.DealQty });

                            List<CalculationProductViewModel> ChargeRates = new CalculationProductService(_unitOfWork).GetChargeRates(CalculationId, InvoiceRetHeader.DocTypeId, InvoiceRetHeader.SiteId, InvoiceRetHeader.DivisionId,
                                    Settings.ProcessId, InvoiceRetHeader.SalesTaxGroupPersonId, item.SalesTaxGroupProductId).ToList();
                            if (ChargeRates != null)
                            {
                                LineChargeRates.Add(new LineChargeRates { LineId = line.JobInvoiceReturnLineId, ChargeRates = ChargeRates });
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
                        new ChargesCalculationService(_unitOfWork).CalculateCharges(LineListWithReferences, InvoiceRetHeader.JobInvoiceReturnHeaderId, (int)CalculationId, null, out LineCharges, out HeaderChargeEdit, out HeaderCharges, "Web.JobInvoiceReturnHeaderCharges", "Web.JobInvoiceReturnLineCharges", out PersonCount, InvoiceRetHeader.DocTypeId, InvoiceRetHeader.SiteId, InvoiceRetHeader.DivisionId);
                    }

                    // Saving Charges
                    foreach (var item in LineCharges)
                    {
                        JobInvoiceReturnLineCharge PoLineCharge = Mapper.Map<LineChargeViewModel, JobInvoiceReturnLineCharge>(item);
                        PoLineCharge.HeaderTableId = InvoiceRetHeader.JobInvoiceReturnHeaderId;
                        PoLineCharge.ObjectState = Model.ObjectState.Added;
                        new JobInvoiceReturnLineChargeService(db).Create(PoLineCharge);
                    }


                    //Saving Header charges
                    for (int i = 0; i < HeaderCharges.Count(); i++)
                    {
                        JobInvoiceReturnHeaderCharge POHeaderCharge = Mapper.Map<HeaderChargeViewModel, JobInvoiceReturnHeaderCharge>(HeaderCharges[i]);
                        POHeaderCharge.HeaderTableId = InvoiceRetHeader.JobInvoiceReturnHeaderId;
                        POHeaderCharge.PersonID = InvoiceRetHeader.JobWorkerId;
                        POHeaderCharge.ObjectState = Model.ObjectState.Added;
                        new JobInvoiceReturnHeaderChargeService(db).Create(POHeaderCharge);
                    }

                    try
                    {
                        //_unitOfWork.Save();
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        ModelState.AddModelError("", message);
                        return PartialView("_InvoiceReturn", svm);
                    }

                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = JobInvoiceHeader.DocTypeId,
                        DocId = InvoiceRetHeader.JobInvoiceReturnHeaderId,
                        ActivityType = (int)ActivityTypeContants.MultipleCreate,
                        DocNo = JobInvoiceHeader.DocNo,
                        DocDate = JobInvoiceHeader.DocDate,
                        DocStatus = JobInvoiceHeader.Status,
                    }));


                    try
                    {
                        StockHeader StockHeader = new StockHeaderService(_unitOfWork).Find((int)GoodsRetHeader.StockHeaderId);
                        StockHeader.DocHeaderId = GoodsRetHeader.JobReturnHeaderId;
                        new StockHeaderService(_unitOfWork).Update(StockHeader);
                        _unitOfWork.Save();
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                    }

                    return Json(new { success = true, Url = "/JobInvoiceReturnHeader/Submit/" + InvoiceRetHeader.JobInvoiceReturnHeaderId });
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

        public ActionResult _InvoiceReturnSubmit(int id)
        {
            return Redirect(System.Configuration.ConfigurationManager.AppSettings["JobDomain"] + "/JobInvoiceReturnHeader/Submit/" + id);
        }
    }

    public class JobInvoiceReturn
    {
        public int JobInvoiceHeaderId { get; set; }
        public DateTime DocDate { get; set; }
        public int ReasonId { get; set; }
        public string ReasonName { get; set; }
        public string Remark { get; set; }
    }
}
