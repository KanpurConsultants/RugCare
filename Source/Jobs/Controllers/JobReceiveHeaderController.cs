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
using System.Data.SqlClient;
using Core.Common;
using Model.ViewModel;
using AutoMapper;
using Jobs.Helpers;
using System.Configuration;
using System.Xml.Linq;
using DocumentEvents;
using CustomEventArgs;
using JobReceiveDocumentEvents;
using Reports.Reports;
using Reports.Controllers;
using Model.ViewModels;

namespace Jobs.Controllers
{
    [Authorize]
    public class JobReceiveHeaderController : System.Web.Mvc.Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        List<string> UserRoles = new List<string>();
        ActiivtyLogViewModel LogVm = new ActiivtyLogViewModel();

        private bool EventException = false;

        bool TimePlanValidation = true;
        string ExceptionMsg = "";
        bool Continue = true;

        IJobReceiveHeaderService _JobReceiveHeaderService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;
        public JobReceiveHeaderController(IJobReceiveHeaderService JobReceiveHeaderService, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _JobReceiveHeaderService = JobReceiveHeaderService;
            _unitOfWork = unitOfWork;
            _exception = exec;
            if (!JobReceiveEvents.Initialized)
            {
                JobReceiveEvents Obj = new JobReceiveEvents();
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

        public void PrepareViewBag(int id)
        {
            ViewBag.Name = new DocumentTypeService(_unitOfWork).Find(id).DocumentTypeName;
            ViewBag.id = id;
            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var  SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            ViewBag.AdminSetting = UserRoles.Contains("Admin").ToString();
            var settings = new JobReceiveSettingsService(_unitOfWork).GetJobReceiveSettingsForDocument(id, DivisionId, SiteId);
            if(settings !=null)
            {
                ViewBag.WizardId = settings.WizardMenuId;
                ViewBag.ImportMenuId = settings.ImportMenuId;
                ViewBag.SqlProcDocumentPrint = settings.SqlProcDocumentPrint;
                ViewBag.SqlProcGatePass = settings.SqlProcGatePass;
                ViewBag.ExportMenuId = settings.ExportMenuId;
            }

        }

        // GET: /JobReceiveHeaderMaster/

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
            var JobReceiveHeader = _JobReceiveHeaderService.GetJobReceiveHeaderList(id, User.Identity.Name);
            ViewBag.Name = new DocumentTypeService(_unitOfWork).Find(id).DocumentTypeName;
            ViewBag.id = id;
            PrepareViewBag(id);
            ViewBag.PendingToSubmit = PendingToSubmitCount(id);
            ViewBag.PendingToReview = PendingToReviewCount(id);
            ViewBag.IndexStatus = "All";
            return View(JobReceiveHeader);
        }

        public ActionResult Index_PendingToSubmit(int id)
        {
            IQueryable<JobReceiveIndexViewModel> p = _JobReceiveHeaderService.GetJobReceiveHeaderListPendingToSubmit(id, User.Identity.Name);

            PrepareViewBag(id);
            ViewBag.PendingToSubmit = PendingToSubmitCount(id);
            ViewBag.PendingToReview = PendingToReviewCount(id);
            ViewBag.IndexStatus = "PTS";
            return View("Index", p);
        }
        public ActionResult Index_PendingToReview(int id)
        {
            IQueryable<JobReceiveIndexViewModel> p = _JobReceiveHeaderService.GetJobReceiveHeaderListPendingToReview(id, User.Identity.Name);
            PrepareViewBag(id);
            ViewBag.PendingToSubmit = PendingToSubmitCount(id);
            ViewBag.PendingToReview = PendingToReviewCount(id);
            ViewBag.IndexStatus = "PTR";
            return View("Index", p);
        }
        // GET: /JobReceiveHeaderMaster/Create

        public ActionResult Create(int id)//DocumentTypeId
        {
            JobReceiveHeaderViewModel vm = new JobReceiveHeaderViewModel();
            vm.DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            vm.SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            vm.CreatedDate = DateTime.Now;

            List<DocumentTypeHeaderAttributeViewModel> tem = new DocumentTypeService(_unitOfWork).GetDocumentTypeHeaderAttribute(id).ToList();
            vm.DocumentTypeHeaderAttributes = tem;

            //Getting Settings
            var settings = new JobReceiveSettingsService(_unitOfWork).GetJobReceiveSettingsForDocument(id, vm.DivisionId, vm.SiteId);

            if (settings == null && UserRoles.Contains("SysAdmin"))
            {
                return RedirectToAction("Create", "JobReceiveSettings", new { id = id }).Warning("Please create job receive settings");
            }
            else if (settings == null && !UserRoles.Contains("SysAdmin"))
            {
                return View("~/Views/Shared/InValidSettings.cshtml");
            }
            vm.JobReceiveSettings = Mapper.Map<JobReceiveSettings, JobReceiveSettingsViewModel>(settings);

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


            var LastTrRec = (from H in db.JobReceiveHeader
                             where H.SiteId == vm.SiteId && H.DivisionId == vm.DivisionId && H.DocTypeId == id && H.CreatedBy == User.Identity.Name
                             orderby H.JobReceiveHeaderId descending
                             select new
                             {
                                 OrderById = H.JobReceiveById,
                             }).FirstOrDefault();
            if (LastTrRec != null)
                vm.JobReceiveById = LastTrRec.OrderById;


            vm.DocDate = DateTime.Now;
            vm.DocTypeId = id;
            vm.DocNo = new DocumentTypeService(_unitOfWork).FGetNewDocNo("DocNo", ConfigurationManager.AppSettings["DataBaseSchema"] + ".JobReceiveHeaders", vm.DocTypeId, vm.DocDate, vm.DivisionId, vm.SiteId);
            vm.DocumentTypeSettings = new DocumentTypeSettingsService(_unitOfWork).GetDocumentTypeSettingsForDocument(vm.DocTypeId);
            PrepareViewBag(id);
            ViewBag.Mode = "Add";
            return View("Create", vm);
        }

        // POST: /ProductMaster/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Post(JobReceiveHeaderViewModel vm)
        {
            #region BeforeSave
            bool BeforeSave = true;
            try
            {

                if (vm.JobReceiveHeaderId <= 0)
                    BeforeSave = JobReceiveDocEvents.beforeHeaderSaveEvent(this, new JobEventArgs(vm.JobReceiveHeaderId, EventModeConstants.Add), ref db);
                else
                    BeforeSave = JobReceiveDocEvents.beforeHeaderSaveEvent(this, new JobEventArgs(vm.JobReceiveHeaderId, EventModeConstants.Edit), ref db);
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

            if (vm.JobWorkerDocDate > vm.DocDate)
                ModelState.AddModelError("JobWorkerDocDate", "Party Doc Date can not be greater then Receive Date.");

            if (vm.DocumentTypeHeaderAttributes != null)
            {
                foreach (var pta in vm.DocumentTypeHeaderAttributes)
                {
                    if (pta.DataType == "Number")
                    {
                        if (pta.Value != null)
                        {
                            var count = pta.Value.Count(x => x == '.');
                            if (count > 1)
                                ModelState.AddModelError("", pta.Name + " should be a numeric value.");
                            else
                            {
                                if (pta.Value.Replace(".", "").All(char.IsDigit) == false)
                                    ModelState.AddModelError("", pta.Name + " should be a numeric value.");
                            }
                        }
                    }
                    else if (pta.DataType == "Date")
                    {
                        if (pta.Value != null)
                        {
                            DateTime dDate;
                            if (DateTime.TryParse(pta.Value, out dDate))
                            {
                                String.Format("{0:d/MM/yyyy}", dDate);
                            }
                            else
                            {
                                ModelState.AddModelError("", pta.Name + " should be a Date value.");
                            }
                        }
                    }

                }
            }

            #region DocTypeTimeLineValidation

            try
            {

                if (vm.JobReceiveHeaderId <= 0)
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
                if (vm.JobReceiveHeaderId <= 0)
                {

                    JobReceiveHeader header = new JobReceiveHeader();
                    header = Mapper.Map<JobReceiveHeaderViewModel, JobReceiveHeader>(vm);
                    header.CreatedBy = User.Identity.Name;
                    header.CreatedDate = DateTime.Now;
                    header.ModifiedBy = User.Identity.Name;
                    header.ModifiedDate = DateTime.Now;
                    header.Status = (int)StatusConstants.Drafted;

                    header.ObjectState = Model.ObjectState.Added;
                    db.JobReceiveHeader.Add(header);
                    //_JobReceiveHeaderService.Create(header);

                    try
                    {
                        JobReceiveDocEvents.onHeaderSaveEvent(this, new JobEventArgs(header.JobReceiveHeaderId, EventModeConstants.Add), ref db);
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
                        JobReceiveDocEvents.afterHeaderSaveEvent(this, new JobEventArgs(header.JobReceiveHeaderId, EventModeConstants.Add), ref db);
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                    }

                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = header.DocTypeId,
                        DocId = header.JobReceiveHeaderId,
                        ActivityType = (int)ActivityTypeContants.Added,
                        DocDate = header.DocDate,
                        DocNo = header.DocNo,
                        DocStatus = header.Status,
                    }));

                    #region CustomRecord
                    if (vm.DocumentTypeHeaderAttributes != null)
                    {
                        List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();
                        CustomHeader Customheaderdetail = new CustomHeader();

                        Customheaderdetail.SiteId = vm.SiteId;
                        Customheaderdetail.DivisionId = vm.DivisionId;
                        Customheaderdetail.DocDate = vm.DocDate;
                        Customheaderdetail.DocNo = vm.DocNo;
                        Customheaderdetail.DocTypeId = vm.DocTypeId;
                        Customheaderdetail.DocId  = header.JobReceiveHeaderId;

                        Customheaderdetail.CreatedDate = DateTime.Now;
                        Customheaderdetail.ModifiedDate = DateTime.Now;
                        Customheaderdetail.CreatedBy = User.Identity.Name;
                        Customheaderdetail.ModifiedBy = User.Identity.Name;
                        Customheaderdetail.Status = (int)StatusConstants.Drafted;
                        Customheaderdetail.ObjectState = Model.ObjectState.Added;
                        new CustomHeaderService(_unitOfWork).Create(Customheaderdetail);

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
                                DocTypeId = Customheaderdetail.DocTypeId,
                                DocId = Customheaderdetail.CustomHeaderId,
                                ActivityType = (int)ActivityTypeContants.Added,
                                DocNo = Customheaderdetail.DocNo,
                                DocDate = Customheaderdetail.DocDate,
                                DocStatus = Customheaderdetail.Status,
                            }));

                            foreach (var pta in vm.DocumentTypeHeaderAttributes)
                        {

                            CustomHeaderAttributes CustomHeaderAttribute = (from A in db.CustomHeaderAttributes
                                                                            where A.HeaderTableId == Customheaderdetail.CustomHeaderId && A.DocumentTypeHeaderAttributeId == pta.DocumentTypeHeaderAttributeId
                                                                            select A).FirstOrDefault();

                            if (CustomHeaderAttribute != null)
                            {
                                CustomHeaderAttribute.Value = pta.Value;
                                CustomHeaderAttribute.ObjectState = Model.ObjectState.Modified;
                                _unitOfWork.Repository<CustomHeaderAttributes>().Add(CustomHeaderAttribute);
                            }
                            else
                            {
                                CustomHeaderAttributes pa = new CustomHeaderAttributes()
                                {
                                    Value = pta.Value,
                                    HeaderTableId = Customheaderdetail.CustomHeaderId,
                                    DocumentTypeHeaderAttributeId = pta.DocumentTypeHeaderAttributeId,
                                };
                                pa.ObjectState = Model.ObjectState.Added;
                                _unitOfWork.Repository<CustomHeaderAttributes>().Add(pa);
                            }
                        }


                        LogList.Add(new LogTypeViewModel
                        {
                            //ExObj = ExRec,
                            Obj = Customheaderdetail,
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
                            DocTypeId = Customheaderdetail.DocTypeId,
                            DocId = Customheaderdetail.CustomHeaderId,
                            ActivityType = (int)ActivityTypeContants.Modified,
                            DocNo = Customheaderdetail.DocNo,
                            xEModifications = Modifications,
                            DocDate = Customheaderdetail.DocDate,
                            DocStatus = Customheaderdetail.Status,
                        }));

                    }
                        #endregion

                        return RedirectToAction("Modify", new { id = header.JobReceiveHeaderId }).Success("Data saved successfully");
                }
                #endregion

                #region EditRecord
                else
                {
                    bool GodownChanged = false;
                    bool DocDateChanged = false;
                    List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();

                    JobReceiveHeader temp = _JobReceiveHeaderService.Find(vm.JobReceiveHeaderId);

                    GodownChanged = (temp.GodownId == vm.GodownId) ? false : true;
                    DocDateChanged = (temp.DocDate == vm.DocDate) ? false : true;

                    JobReceiveHeader ExRec = new JobReceiveHeader();
                    ExRec = Mapper.Map<JobReceiveHeader>(temp);


                    int status = temp.Status;

                    if (temp.Status != (int)StatusConstants.Drafted && temp.Status != (int)StatusConstants.Import)
                        temp.Status = (int)StatusConstants.Modified;

                    temp.DocDate = vm.DocDate;
                    temp.DocNo = vm.DocNo;
                    temp.JobWorkerDocNo = vm.JobWorkerDocNo;
                    temp.JobWorkerDocDate = vm.JobWorkerDocDate;
                    temp.ProcessId = vm.ProcessId;
                    temp.JobWorkerId = vm.JobWorkerId;
                    temp.JobReceiveById = vm.JobReceiveById;
                    temp.GodownId = vm.GodownId;
                    temp.Remark = vm.Remark;
                    temp.ModifiedDate = DateTime.Now;
                    temp.ModifiedBy = User.Identity.Name;
                    temp.ObjectState = Model.ObjectState.Modified;
                    //_JobReceiveHeaderService.Update(temp);
                    db.JobReceiveHeader.Add(temp);

                    if (temp.StockHeaderId != null)
                    {
                        StockHeader S = new StockHeaderService(_unitOfWork).Find(temp.StockHeaderId.Value);

                        S.DocDate = temp.DocDate;
                        S.DocNo = temp.DocNo;
                        S.PersonId = temp.JobWorkerId;
                        S.ProcessId = temp.ProcessId;
                        S.GodownId = temp.GodownId;
                        S.Remark = temp.Remark;
                        S.Status = temp.Status;
                        S.ModifiedBy = temp.ModifiedBy;
                        S.ModifiedDate = temp.ModifiedDate;
                        S.ObjectState = Model.ObjectState.Modified;
                        db.StockHeader.Add(S);
                        //new StockHeaderService(_unitOfWork).UpdateStockHeader(S);
                    }

                    if (GodownChanged || DocDateChanged)
                        new StockService(_unitOfWork).UpdateStockGodownId(temp.StockHeaderId, temp.GodownId, temp.DocDate, db);



                    if (temp.JobWorkerId != ExRec.JobWorkerId || temp.DocNo != ExRec.DocNo || temp.DocDate != ExRec.DocDate)
                    {
                        _JobReceiveHeaderService.UpdateProdUidJobWorkers(ref db, temp);
                    }

                    LogList.Add(new LogTypeViewModel
                    {
                        ExObj = ExRec,
                        Obj = temp,
                    });

                    XElement Modifications = new ModificationsCheckService().CheckChanges(LogList);

                    try
                    {
                        JobReceiveDocEvents.onHeaderSaveEvent(this, new JobEventArgs(temp.JobReceiveHeaderId, EventModeConstants.Edit), ref db);
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
                        PrepareViewBag(temp.DocTypeId);
                        ViewBag.Mode = "Edit";
                        return View("Create", vm);
                    }

                    try
                    {
                        JobReceiveDocEvents.afterHeaderSaveEvent(this, new JobEventArgs(temp.JobReceiveHeaderId, EventModeConstants.Edit), ref db);
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                    }


                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = temp.DocTypeId,
                        DocId = temp.JobReceiveHeaderId,
                        ActivityType = (int)ActivityTypeContants.Modified,
                        DocNo = temp.DocNo,
                        DocDate = temp.DocDate,
                        xEModifications = Modifications,
                        DocStatus = temp.Status,
                    }));

                    #region CustomRecord
                    if (vm.DocumentTypeHeaderAttributes != null)
                    {
                         LogList = new List<LogTypeViewModel>();
                        CustomHeader Customheaderdetail = db.CustomHeader.Where(m =>m.DocTypeId == vm.DocTypeId && m.DocId == vm.JobReceiveHeaderId).FirstOrDefault();


                        Customheaderdetail.DocDate = vm.DocDate;
                        Customheaderdetail.DocNo = vm.DocNo;

                        Customheaderdetail.ModifiedDate = DateTime.Now;
                        Customheaderdetail.ModifiedBy = User.Identity.Name;
                        Customheaderdetail.Status = (int)StatusConstants.Drafted;
                        Customheaderdetail.ObjectState = Model.ObjectState.Modified;
                        new CustomHeaderService(_unitOfWork).Update(Customheaderdetail);

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
                            DocTypeId = Customheaderdetail.DocTypeId,
                            DocId = Customheaderdetail.CustomHeaderId,
                            ActivityType = (int)ActivityTypeContants.Added,
                            DocNo = Customheaderdetail.DocNo,
                            DocDate = Customheaderdetail.DocDate,
                            DocStatus = Customheaderdetail.Status,
                        }));

                        foreach (var pta in vm.DocumentTypeHeaderAttributes)
                        {

                            CustomHeaderAttributes CustomHeaderAttribute = (from A in db.CustomHeaderAttributes
                                                                            where A.HeaderTableId == Customheaderdetail.CustomHeaderId && A.DocumentTypeHeaderAttributeId == pta.DocumentTypeHeaderAttributeId
                                                                            select A).FirstOrDefault();

                            if (CustomHeaderAttribute != null)
                            {
                                CustomHeaderAttribute.Value = pta.Value;
                                CustomHeaderAttribute.ObjectState = Model.ObjectState.Modified;
                                _unitOfWork.Repository<CustomHeaderAttributes>().Add(CustomHeaderAttribute);
                            }
                            else
                            {
                                CustomHeaderAttributes pa = new CustomHeaderAttributes()
                                {
                                    Value = pta.Value,
                                    HeaderTableId = Customheaderdetail.CustomHeaderId,
                                    DocumentTypeHeaderAttributeId = pta.DocumentTypeHeaderAttributeId,
                                };
                                pa.ObjectState = Model.ObjectState.Added;
                                _unitOfWork.Repository<CustomHeaderAttributes>().Add(pa);
                            }
                        }


                        LogList.Add(new LogTypeViewModel
                        {
                            //ExObj = ExRec,
                            Obj = Customheaderdetail,
                        });

                         Modifications = new ModificationsCheckService().CheckChanges(LogList);

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
                            DocTypeId = Customheaderdetail.DocTypeId,
                            DocId = Customheaderdetail.CustomHeaderId,
                            ActivityType = (int)ActivityTypeContants.Modified,
                            DocNo = Customheaderdetail.DocNo,
                            xEModifications = Modifications,
                            DocDate = Customheaderdetail.DocDate,
                            DocStatus = Customheaderdetail.Status,
                        }));

                    }
                    #endregion

                    return RedirectToAction("Index", new { id = temp.DocTypeId }).Success("Data saved successfully");

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
                            Messsages = Error.ErrorMessage + System.Environment.NewLine;
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
            JobReceiveHeader header = _JobReceiveHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Drafted || header.Status == (int)StatusConstants.Import)
                return Edit(id, IndexType);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult ModifyAfter_Submit(int id, string IndexType)
        {
            JobReceiveHeader header = _JobReceiveHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Submitted || header.Status == (int)StatusConstants.Modified)
                return Edit(id, IndexType);
            else
                return HttpNotFound();
        }


        [HttpGet]
        public ActionResult Delete(int id)
        {
            JobReceiveHeader header = _JobReceiveHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Drafted || header.Status == (int)StatusConstants.Import)
                return Remove(id);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult DeleteAfter_Submit(int id)
        {
            JobReceiveHeader header = _JobReceiveHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Submitted || header.Status == (int)StatusConstants.Modified)
                return Remove(id);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult DetailInformation(int id, int? DocLineId)
        {
            return RedirectToAction("Detail", new { id = id, transactionType = "detail", DocLineId = DocLineId });
        }



        // GET: /ProductMaster/Edit/5
        private ActionResult Edit(int id, string IndexType)
        {
            ViewBag.IndexStatus = IndexType;
            JobReceiveHeaderViewModel pt = _JobReceiveHeaderService.GetJobReceiveHeader(id);
            //Job Receive Settings
            var settings = new JobReceiveSettingsService(_unitOfWork).GetJobReceiveSettingsForDocument(pt.DocTypeId, pt.DivisionId, pt.SiteId);

            if (settings.isVisibleCustomHeaderAttribute==true)
            {
                var tem = new CustomHeaderService(_unitOfWork).GetDocumentHeaderAttributeByDocId(pt.DocTypeId, pt.JobReceiveHeaderId).ToList();            
                pt.DocumentTypeHeaderAttributes = tem;
            }


            if (pt.Status != (int)StatusConstants.Drafted)
                if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, pt.DocTypeId, pt.ProcessId, this.ControllerContext.RouteData.Values["controller"].ToString(), "Edit") == false)
                    return RedirectToAction("DetailInformation", new { id = id, IndexType = IndexType }).Warning("You don't have permission to do this task.");

            if (pt == null)
            {
                return HttpNotFound();
            }

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

            if (settings == null && UserRoles.Contains("SysAdmin"))
            {
                return RedirectToAction("Create", "JobReceiveSettings", new { id = pt.DocTypeId }).Warning("Please create job receive settings");
            }
            else if (settings == null && !UserRoles.Contains("SysAdmin"))
            {
                return View("~/Views/Shared/InValidSettings.cshtml");
            }

            pt.JobReceiveSettings = Mapper.Map<JobReceiveSettings, JobReceiveSettingsViewModel>(settings);
            pt.DocumentTypeSettings = new DocumentTypeSettingsService(_unitOfWork).GetDocumentTypeSettingsForDocument(pt.DocTypeId);

            PrepareViewBag(pt.DocTypeId);

            ViewBag.Mode = "Edit";
            if ((System.Web.HttpContext.Current.Request.UrlReferrer.PathAndQuery).Contains("Create"))
                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = pt.DocTypeId,
                    DocId = pt.JobReceiveHeaderId,
                    ActivityType = (int)ActivityTypeContants.Detail,
                    DocNo = pt.DocNo,
                    DocDate = pt.DocDate,
                    DocStatus = pt.Status,
                }));

            return View("Create", pt);
        }



        [Authorize]
        public ActionResult Detail(int id, string IndexType, string transactionType, int? DocLineId)
        {
            if (DocLineId.HasValue)
                ViewBag.DocLineId = DocLineId;

            ViewBag.transactionType = transactionType;
            ViewBag.IndexStatus = IndexType;

            JobReceiveHeaderViewModel pt = _JobReceiveHeaderService.GetJobReceiveHeader(id);

            //Job Receive Settings
            var settings = new JobReceiveSettingsService(_unitOfWork).GetJobReceiveSettingsForDocument(pt.DocTypeId, pt.DivisionId, pt.SiteId);

            if (settings == null && UserRoles.Contains("SysAdmin"))
            {
                return RedirectToAction("Create", "JobReceiveSettings", new { id = pt.DocTypeId }).Warning("Please create job receive settings");
            }
            else if (settings == null && !UserRoles.Contains("SysAdmin"))
            {
                return View("~/Views/Shared/InValidSettings.cshtml");
            }

            pt.JobReceiveSettings = Mapper.Map<JobReceiveSettings, JobReceiveSettingsViewModel>(settings);
            pt.DocumentTypeSettings = new DocumentTypeSettingsService(_unitOfWork).GetDocumentTypeSettingsForDocument(pt.DocTypeId);

            PrepareViewBag(pt.DocTypeId);
            if (pt == null)
            {
                return HttpNotFound();
            }
            if (String.IsNullOrEmpty(transactionType) || transactionType == "detail")
                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = pt.DocTypeId,
                    DocId = pt.JobReceiveHeaderId,
                    ActivityType = (int)ActivityTypeContants.Detail,
                    DocNo = pt.DocNo,
                    DocDate = pt.DocDate,
                    DocStatus = pt.Status,
                }));

            return View("Create", pt);
        }



        // GET: /ProductMaster/Delete/5

        private ActionResult Remove(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JobReceiveHeader JobReceiveHeader = _JobReceiveHeaderService.Find(id);

            if (JobReceiveHeader == null)
            {
                return HttpNotFound();
            }

            if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, JobReceiveHeader.DocTypeId, JobReceiveHeader.ProcessId, this.ControllerContext.RouteData.Values["controller"].ToString(), "Remove") == false)
            {
                return PartialView("~/Views/Shared/PermissionDenied_Modal.cshtml").Warning("You don't have permission to do this task.");
            }

            #region DocTypeTimeLineValidation

            bool TimePlanValidation = true;
            string ExceptionMsg = "";
            try
            {
                TimePlanValidation = DocumentValidation.ValidateDocument(Mapper.Map<DocumentUniqueId>(JobReceiveHeader), DocumentTimePlanTypeConstants.Delete, User.Identity.Name, out ExceptionMsg, out Continue);
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
            bool BeforeSave = true;

            int MainSiteId = (from S in db.Site where S.SiteCode == "MAIN" select S).FirstOrDefault().SiteId;

            try
            {
                BeforeSave = JobReceiveDocEvents.beforeHeaderDeleteEvent(this, new JobEventArgs(vm.id), ref db);
            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXC"] += message;
                EventException = true;
            }

            if (!BeforeSave)
                TempData["CSEXC"] += "Failed validation before delete";

            if (ModelState.IsValid && BeforeSave && !EventException)
            {
                int? StockHeaderId = 0;
                //int ProcessId = new ProcessService(_unitOfWork).Find(ProcessConstants.Weaving).ProcessId;

                List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();

                var temp = (from p in db.JobReceiveHeader
                            where p.JobReceiveHeaderId == vm.id
                            select p).FirstOrDefault();

                LogList.Add(new LogTypeViewModel
                {
                    ExObj = Mapper.Map<JobReceiveHeader>(temp),
                });


                StockHeaderId = temp.StockHeaderId;

                //var line = new JobReceiveLineService(_unitOfWork).GetLineListForIndex(vm.id);
                var line = (from p in db.JobReceiveLine
                            where p.JobReceiveHeaderId == vm.id
                            select p).ToList();

                var JRLineIds = line.Select(m => m.JobReceiveLineId).ToArray();

                var JobReceiveLineStatusRecords = (from p in db.JobReceiveLineStatus
                                                   where JRLineIds.Contains(p.JobReceiveLineId ?? 0)
                                                   select p).ToList();

                var ProductUids = line.Select(m => m.ProductUidId).ToArray();

                var BarCodeRecords = (from p in db.ProductUid
                                      where ProductUids.Contains(p.ProductUIDId)
                                      select p).ToList();

                var GeneratedBarCodeRecords = (from L in db.JobReceiveLine
                                               join P in db.ProductUid on L.ProductUidId equals P.ProductUIDId
                                      where L.JobReceiveHeaderId == vm.id && L.ProductUidHeaderId != null && L.ProductUidId != null
                                      select P).ToList();


                List<int> StockIdList = new List<int>();
                List<int> StockProcessIdList = new List<int>();

                try
                {
                    JobReceiveDocEvents.onHeaderDeleteEvent(this, new JobEventArgs(vm.id), ref db);
                }
                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    TempData["CSEXC"] += message;
                    EventException = true;
                }

                new JobOrderLineStatusService(_unitOfWork).DeleteJobQtyOnReceiveMultiple(temp.JobReceiveHeaderId, ref db);

                foreach (var item in JobReceiveLineStatusRecords)
                {
                    item.ObjectState = Model.ObjectState.Deleted;
                    db.JobReceiveLineStatus.Remove(item);
                }

                if (StockHeaderId != null)
                {
                    var Stocks = new StockService(_unitOfWork).GetStockForStockHeaderId((int)StockHeaderId);
                    foreach (var item in Stocks)
                    {
                        if (item.StockId != null)
                        {
                            StockIdList.Add((int)item.StockId);
                        }
                    }

                    var StockProcesses = new StockProcessService(_unitOfWork).GetStockProcessForStockHeaderId((int)StockHeaderId);
                    foreach (var item in StockProcesses)
                    {
                        if (item.StockProcessId != null)
                        {
                            StockProcessIdList.Add((int)item.StockProcessId);
                        }
                    }
                }

                

                foreach (var item in line)
                {
                    LogList.Add(new LogTypeViewModel
                    {
                        ExObj = Mapper.Map<JobReceiveLine>(item),
                    });

                    //if (item.StockId != null)
                    //{
                    //    StockIdList.Add((int)item.StockId);
                    //}


                    //if (item.StockProcessId != null)
                    //{
                    //    StockProcessIdList.Add((int)item.StockProcessId);
                    //}


                    var Productuid = item.ProductUidId;



                    if (Productuid != null && Productuid != 0)
                    {
                        //Service.ProductUidDetail ProductUidDetail = new ProductUidService(_unitOfWork).FGetProductUidLastValues((int)Productuid, "Job Receive-" + item.JobReceiveHeaderId.ToString());

                        ProductUid ProductUid = BarCodeRecords.Where(m => m.ProductUIDId == Productuid).FirstOrDefault();

                        if (!(item.ProductUidLastTransactionDocNo == ProductUid.LastTransactionDocNo && item.ProductUidLastTransactionDocTypeId == ProductUid.LastTransactionDocTypeId) || temp.SiteId == MainSiteId)
                        {

                            if ((temp.DocNo != ProductUid.LastTransactionDocNo || temp.DocTypeId != ProductUid.LastTransactionDocTypeId))
                            {
                                ModelState.AddModelError("", "Bar Code Can't be deleted because this is already Proceed to another process.");
                                return PartialView("_Reason", vm);
                            }

                            if (item.ProductUidHeaderId == null || item.ProductUidHeaderId == 0)
                            {
                                ProductUid.LastTransactionDocDate = item.ProductUidLastTransactionDocDate;
                                ProductUid.LastTransactionDocId = item.ProductUidLastTransactionDocId;
                                ProductUid.LastTransactionDocNo = item.ProductUidLastTransactionDocNo;
                                ProductUid.LastTransactionDocTypeId = item.ProductUidLastTransactionDocTypeId;
                                ProductUid.LastTransactionPersonId = item.ProductUidLastTransactionPersonId;
                                ProductUid.CurrenctGodownId = item.ProductUidCurrentGodownId;
                                ProductUid.CurrenctProcessId = item.ProductUidCurrentProcessId;
                                ProductUid.Status = item.ProductUidStatus;
                                if (!string.IsNullOrEmpty(ProductUid.ProcessesDone))
                                    ProductUid.ProcessesDone = ProductUid.ProcessesDone.Replace("|" + temp.ProcessId.ToString() + "|", "");
                                ProductUid.ModifiedBy = User.Identity.Name;
                                ProductUid.ModifiedDate = DateTime.Now;

                                ProductUid.ObjectState = Model.ObjectState.Modified;
                                db.ProductUid.Add(ProductUid);
                            }

                            new StockUidService(_unitOfWork).DeleteStockUidForDocLineDB(item.JobReceiveHeaderId, temp.DocTypeId, temp.SiteId, temp.DivisionId, ref db);

                        }
                        else
                        {
                            var MainJobRec = (from p in db.JobReceiveLine
                                              join t in db.JobReceiveHeader on p.JobReceiveHeaderId equals t.JobReceiveHeaderId
                                              join d in db.DocumentType on t.DocTypeId equals d.DocumentTypeId
                                              where p.ProductUidId == Productuid && t.SiteId != temp.SiteId && d.DocumentTypeName == TransactionDoctypeConstants.WeavingBazar
                                              && p.LockReason != null
                                              select p).ToList().LastOrDefault();

                            if (MainJobRec != null)
                            {
                                MainJobRec.LockReason = null;
                                MainJobRec.ObjectState = Model.ObjectState.Modified;
                                db.JobReceiveLine.Add(MainJobRec);
                            }
                        }
                    }

                    item.ObjectState = Model.ObjectState.Deleted;
                    db.JobReceiveLine.Remove(item);
                }

                var Boms = (from p in db.JobReceiveBom
                            where p.JobReceiveHeaderId == temp.JobReceiveHeaderId
                            select p).ToList();

                //var StockProcessIds = Boms.Select(m => m.StockProcessId).ToArray();

                //var StockProcessRecords = (from p in db.StockProcess
                //                           where StockProcessIds.Contains(p.StockProcessId)
                //                           select p).ToList();

                foreach (var item2 in Boms)
                {
                    //if (item2.StockProcessId != null)
                    //{
                    //    var StockProcessRec = StockProcessRecords.Where(m => m.StockProcessId == item2.StockProcessId).FirstOrDefault();
                    //    StockProcessRec.ObjectState = Model.ObjectState.Deleted;
                    //    db.StockProcess.Remove(StockProcessRec);
                    //}
                    item2.ObjectState = Model.ObjectState.Deleted;
                    db.JobReceiveBom.Remove(item2);
                }

                new StockService(_unitOfWork).DeleteStockDBMultiple(StockIdList, ref db, true);
                new StockProcessService(_unitOfWork).DeleteStockProcessDBMultiple(StockProcessIdList, ref db, true);

                temp.ObjectState = Model.ObjectState.Deleted;
                db.JobReceiveHeader.Remove(temp);

                if (StockHeaderId != null)
                {
                    var STockHeader = (from p in db.StockHeader
                                       where p.StockHeaderId == StockHeaderId
                                       select p).FirstOrDefault();

                    STockHeader.ObjectState = Model.ObjectState.Deleted;
                    db.StockHeader.Remove(STockHeader);
                }

                foreach(var item in GeneratedBarCodeRecords)
                {
                    item.ObjectState = Model.ObjectState.Deleted;
                    db.ProductUid.Remove(item);
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
                    JobReceiveDocEvents.afterHeaderDeleteEvent(this, new JobEventArgs(vm.id), ref db);
                }
                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    TempData["CSEXC"] += message;
                }


                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = temp.DocTypeId,
                    DocId = temp.JobReceiveHeaderId,
                    ActivityType = (int)ActivityTypeContants.Deleted,
                    UserRemark = vm.Reason,
                    DocNo = temp.DocNo,
                    DocDate = temp.DocDate,
                    xEModifications = Modifications,
                    DocStatus = temp.Status,
                }));


                return Json(new { success = true });

            }
            return PartialView("_Reason", vm);
        }



        public ActionResult Submit(int id, string IndexType, string TransactionType)
        {
            JobReceiveHeader s = db.JobReceiveHeader.Find(id);

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

            bool BeforeSave = true;
            try
            {
                BeforeSave = JobReceiveDocEvents.beforeHeaderSubmitEvent(this, new JobEventArgs(Id), ref db);
            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXC"] += message;
                EventException = true;
            }

            if (!BeforeSave)
                TempData["CSEXC"] += "Falied validation before submit.";

            if (ModelState.IsValid && BeforeSave && !EventException)
            {
                JobReceiveHeader pd = new JobReceiveHeaderService(_unitOfWork).Find(Id);
                int ActivityType;
                if (User.Identity.Name == pd.ModifiedBy || UserRoles.Contains("Admin"))
                {
                    int Cnt = 0;
                    int CountUid = 0;

                    pd.Status = (int)StatusConstants.Submitted;
                    ActivityType = (int)ActivityTypeContants.Submitted;

                    JobReceiveSettings Settings = new JobReceiveSettingsService(_unitOfWork).GetJobReceiveSettingsForDocument(pd.DocTypeId, pd.DivisionId, pd.SiteId);
                    
                    List<string> uids = new List<string>();

                    if (!string.IsNullOrEmpty(Settings.SqlProcGenProductUID))
                    {

                        var lines = (from p in db.JobReceiveLine
                                     where p.JobReceiveHeaderId == pd.JobReceiveHeaderId
                                     select p).ToList();


                        decimal Qty = lines.Where(m => m.ProductUidHeaderId == null).Sum(m => m.Qty);


                        using (SqlConnection sqlConnection = new SqlConnection((string)System.Web.HttpContext.Current.Session["DefaultConnectionString"]))
                        {
                            sqlConnection.Open();

                            int TypeId = pd.DocTypeId;

                            SqlCommand Totalf = new SqlCommand("SELECT * FROM " + Settings.SqlProcGenProductUID + "( " + TypeId + ", " + Qty + ",Null)", sqlConnection);

                            SqlDataReader ExcessStockQty = (Totalf.ExecuteReader());
                            while (ExcessStockQty.Read())
                            {
                                uids.Add((string)ExcessStockQty.GetValue(0));
                            }
                        }

                        //uids = new JobReceiveLineService(_unitOfWork).GetProcGenProductUids(pd.DocTypeId, Qty, pd.DivisionId, pd.SiteId);

                        foreach (var item in lines.Where(m => m.ProductUidHeaderId == null))
                        {
                            if (uids.Count > 0)
                            {
                                ProductUidHeader ProdUidHeader = new ProductUidHeader();

                                ProdUidHeader.ProductUidHeaderId = Cnt;
                                ProdUidHeader.ProductId = item.ProductId;
                                ProdUidHeader.Dimension1Id = item.Dimension1Id;
                                ProdUidHeader.Dimension2Id = item.Dimension2Id;
                                ProdUidHeader.Dimension3Id = item.Dimension3Id;
                                ProdUidHeader.Dimension4Id = item.Dimension4Id;
                                ProdUidHeader.GenDocId = pd.JobReceiveHeaderId;
                                ProdUidHeader.GenDocNo = pd.DocNo;
                                ProdUidHeader.GenDocTypeId = pd.DocTypeId;
                                ProdUidHeader.GenDocDate = pd.DocDate;
                                ProdUidHeader.GenPersonId = pd.JobWorkerId;
                                ProdUidHeader.CreatedBy = User.Identity.Name;
                                ProdUidHeader.CreatedDate = DateTime.Now;
                                ProdUidHeader.ModifiedBy = User.Identity.Name;
                                ProdUidHeader.ModifiedDate = DateTime.Now;
                                ProdUidHeader.ObjectState = Model.ObjectState.Added;
                                db.ProductUidHeader.Add(ProdUidHeader);

                                item.ProductUidHeaderId = ProdUidHeader.ProductUidHeaderId;


                                int count = 0;
                                //foreach (string UidItem in uids)
                                for (int A = 0; A < item.Qty; A++)
                                {
                                    ProductUid ProdUid = new ProductUid();

                                    ProdUid.ProductUidHeaderId = ProdUidHeader.ProductUidHeaderId;
                                    ProdUid.ProductUidName = uids[CountUid];
                                    ProdUid.ProductId = item.ProductId;
                                    ProdUid.IsActive = true;
                                    ProdUid.CreatedBy = User.Identity.Name;
                                    ProdUid.CreatedDate = DateTime.Now;
                                    ProdUid.ModifiedBy = User.Identity.Name;
                                    ProdUid.ModifiedDate = DateTime.Now;
                                    ProdUid.GenLineId = item.JobReceiveLineId;
                                    ProdUid.GenDocId = pd.JobReceiveHeaderId;
                                    ProdUid.GenDocNo = pd.DocNo;
                                    ProdUid.GenDocTypeId = pd.DocTypeId;
                                    ProdUid.GenDocDate = pd.DocDate;
                                    ProdUid.GenPersonId = pd.JobWorkerId;
                                    ProdUid.Dimension1Id = item.Dimension1Id;
                                    ProdUid.Dimension2Id = item.Dimension2Id;
                                    ProdUid.Dimension3Id = item.Dimension3Id;
                                    ProdUid.Dimension4Id = item.Dimension4Id;
                                    ProdUid.CurrenctProcessId = pd.ProcessId;
                                    ProdUid.CurrenctGodownId = pd.GodownId;
                                    ProdUid.Status = (!string.IsNullOrEmpty(Settings.BarcodeStatusUpdate) ? Settings.BarcodeStatusUpdate : ProductUidStatusConstants.Receive);
                                    ProdUid.LastTransactionDocId = pd.JobReceiveHeaderId;
                                    ProdUid.LastTransactionDocNo = pd.DocNo;
                                    ProdUid.LastTransactionDocTypeId = pd.DocTypeId;
                                    ProdUid.LastTransactionDocDate = pd.DocDate;
                                    ProdUid.LastTransactionPersonId = pd.JobWorkerId;
                                    ProdUid.LastTransactionLineId = item.JobReceiveLineId;
                                    ProdUid.ProductUIDId = count;
                                    ProdUid.ObjectState = Model.ObjectState.Added;
                                    db.ProductUid.Add(ProdUid);

                                    count++;
                                    CountUid++;
                                }
                                Cnt++;
                                item.ObjectState = Model.ObjectState.Modified;
                                db.JobReceiveLine.Add(item);
                            }
                        }
                    }
                    

             
                    pd.ReviewBy = null;
                    pd.ObjectState = Model.ObjectState.Modified;
                    db.JobReceiveHeader.Add(pd);
                    //_JobReceiveHeaderService.Update(pd);

                    try
                    {
                        JobReceiveDocEvents.onHeaderSubmitEvent(this, new JobEventArgs(Id), ref db);
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
                        return RedirectToAction("Index", new { id = pd.DocTypeId });
                    }

                    try
                    {
                        JobReceiveDocEvents.afterHeaderSubmitEvent(this, new JobEventArgs(Id), ref db);
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                    }

                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = pd.DocTypeId,
                        DocId = pd.JobReceiveHeaderId,
                        ActivityType = ActivityType,
                        UserRemark = UserRemark,
                        DocDate = pd.DocDate,
                        DocNo = pd.DocNo,
                        DocStatus = pd.Status,
                    }));

                    string ReturnUrl = System.Configuration.ConfigurationManager.AppSettings["CurrentDomain"] + "/" + "JobReceiveHeader" + "/" + "Index" + "/" + pd.DocTypeId + "?IndexType=" + IndexType;
                    if (!string.IsNullOrEmpty(IsContinue) && IsContinue == "True")
                    {

                        int nextId = new NextPrevIdService(_unitOfWork).GetNextPrevId(Id, pd.DocTypeId, User.Identity.Name, ForActionConstants.PendingToSubmit, "Web.JobReceiveHeaders", "JobReceiveHeaderId", PrevNextConstants.Next);

                        if (nextId == 0)
                        {
                            var PendingtoSubmitCount = PendingToSubmitCount(pd.DocTypeId);
                            if (PendingtoSubmitCount > 0)
                                //return RedirectToAction("Index_PendingToSubmit", new { id = pd.DocTypeId });
                                ReturnUrl = System.Configuration.ConfigurationManager.AppSettings["CurrentDomain"] + "/" + "JobReceiveHeader" + "/" + "Index_PendingToSubmit" + "/" + pd.DocTypeId + "?IndexType=" + IndexType;
                            else
                                //return RedirectToAction("Index", new { id = pd.DocTypeId });
                                ReturnUrl = System.Configuration.ConfigurationManager.AppSettings["CurrentDomain"] + "/" + "JobReceiveHeader" + "/" + "Index" + "/" + pd.DocTypeId + "?IndexType=" + IndexType;

                        }
                        ReturnUrl = System.Configuration.ConfigurationManager.AppSettings["CurrentDomain"] + "/" + "JobReceiveHeader" + "/" + "Submit" + "/" + nextId + "?TransactionType=submitContinue&IndexType=" + IndexType;
                    }
                    else
                    {
                        ReturnUrl = System.Configuration.ConfigurationManager.AppSettings["CurrentDomain"] + "/" + "JobReceiveHeader" + "/" + "Index" + "/" + pd.DocTypeId + "?IndexType=" + IndexType;
                    }

                    #region "For Calling Customise Menu"
                    int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
                    int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

                    var settings = new JobReceiveSettingsService(_unitOfWork).GetJobReceiveSettingsForDocument(pd.DocTypeId, DivisionId, SiteId);

                    if (settings != null)
                    {
                        if (settings.OnSubmitMenuId != null)
                        {
                            //ReturnUrl = System.Configuration.ConfigurationManager.AppSettings["CurrentDomain"] + "/" + "JobOrderHeader" + "/" + "Index" + "/" + pd.DocTypeId;
                            return Action_Menu(Id, (int)settings.OnSubmitMenuId, ReturnUrl);
                        }
                        else
                            return Redirect(ReturnUrl);
                    }
                    #endregion
                }
                else
                {
                    return RedirectToAction("Index", new { id = pd.DocTypeId, IndexType = IndexType }).Warning("Record can be submitted by user " + pd.ModifiedBy + " only.");
                }
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
            bool BeforeSave = true;
            try
            {
                BeforeSave = JobReceiveDocEvents.beforeHeaderReviewEvent(this, new JobEventArgs(Id), ref db);
            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXC"] += message;
            }

            if (!BeforeSave)
                TempData["CSEXC"] += "Falied validation before review.";

            if (ModelState.IsValid && BeforeSave)
            {
                JobReceiveHeader pd = new JobReceiveHeaderService(_unitOfWork).Find(Id);

                pd.ReviewCount = (pd.ReviewCount ?? 0) + 1;
                pd.ReviewBy += User.Identity.Name + ", ";

                pd.ObjectState = Model.ObjectState.Modified;
                db.JobReceiveHeader.Add(pd);


                //_JobReceiveHeaderService.Update(pd);

                try
                {
                    JobReceiveDocEvents.onHeaderReviewEvent(this, new JobEventArgs(Id), ref db);
                }
                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    TempData["CSEXC"] += message;
                }


                db.SaveChanges();
                //_unitOfWork.Save();

                try
                {
                    JobReceiveDocEvents.afterHeaderReviewEvent(this, new JobEventArgs(Id), ref db);
                }
                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    TempData["CSEXC"] += message;
                }

                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = pd.DocTypeId,
                    DocId = pd.JobReceiveHeaderId,
                    ActivityType = (int)ActivityTypeContants.Reviewed,
                    UserRemark = UserRemark,
                    DocDate = pd.DocDate,
                    DocNo = pd.DocNo,
                    DocStatus = pd.Status,
                }));

                if (!string.IsNullOrEmpty(IsContinue) && IsContinue == "True")
                {
                    JobReceiveHeader HEader = _JobReceiveHeaderService.Find(Id);

                    int nextId = new NextPrevIdService(_unitOfWork).GetNextPrevId(Id, HEader.DocTypeId, User.Identity.Name, ForActionConstants.PendingToReview, "Web.JobReceiveHeaders", "JobReceiveHeaderId", PrevNextConstants.Next);
                    if (nextId == 0)
                    {
                        var PendingtoSubmitCount = PendingToReviewCount(HEader.DocTypeId);
                        if (PendingtoSubmitCount > 0)
                            return RedirectToAction("Index_PendingToReview", new { id = HEader.DocTypeId });
                        else
                            return RedirectToAction("Index", new { id = HEader.DocTypeId, IndexType = IndexType });

                    }

                    return RedirectToAction("Detail", new { id = nextId, transactionType = "ReviewContinue", IndexType = IndexType });
                }


                else
                    return RedirectToAction("Index", new { id = pd.DocTypeId, IndexType = IndexType }).Success(pd.DocNo + " Reviewed Successfully.");
            }

            return View();
        }







        [HttpGet]
        public ActionResult NextPage(int DocId, int DocTypeId)//CurrentHeaderId
        {
            var nextId = new NextPrevIdService(_unitOfWork).GetNextPrevId(DocId, DocTypeId, User.Identity.Name, "", "Web.JobReceiveHeaders", "JobReceiveHeaderId", PrevNextConstants.Next);
            return Edit(nextId, "");
        }
        [HttpGet]
        public ActionResult PrevPage(int DocId, int DocTypeId)//CurrentHeaderId
        {
            var PrevId = new NextPrevIdService(_unitOfWork).GetNextPrevId(DocId, DocTypeId, User.Identity.Name, "", "Web.JobReceiveHeaders", "JobReceiveHeaderId", PrevNextConstants.Prev);
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
        public ActionResult BarcodePrint(int id)
        {

            string GenDocId = "";

            JobReceiveHeader header = _JobReceiveHeaderService.Find(id);
            GenDocId = header.DocTypeId.ToString() + '-' + header.DocNo;
            //return RedirectToAction("PrintBarCode", "Report_BarcodePrint", new { GenHeaderId = id });
            return Redirect((string)System.Configuration.ConfigurationManager.AppSettings["JobsDomain"] + "/Report_BarcodePrint/PrintBarCode/?GenHeaderId=" + GenDocId + "&queryString=" + GenDocId);


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
            JobReceiveHeader header = _JobReceiveHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Drafted || header.Status == (int)StatusConstants.Import)
            {
                var SEttings = new JobReceiveSettingsService(_unitOfWork).GetJobReceiveSettingsForDocument(header.DocTypeId, header.DivisionId, header.SiteId);
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
            JobReceiveHeader header = _JobReceiveHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Submitted || header.Status == (int)StatusConstants.Modified)
            {
                var SEttings = new JobReceiveSettingsService(_unitOfWork).GetJobReceiveSettingsForDocument(header.DocTypeId, header.DivisionId, header.SiteId);
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
            JobReceiveHeader header = _JobReceiveHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Approved)
            {
                var SEttings = new JobReceiveSettingsService(_unitOfWork).GetJobReceiveSettingsForDocument(header.DocTypeId, header.DivisionId, header.SiteId);
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

            JobReceiveSettings SEttings = new JobReceiveSettingsService(_unitOfWork).GetJobReceiveSettingsForDocument(Dt.DocumentTypeId, (int)System.Web.HttpContext.Current.Session["DivisionId"], (int)System.Web.HttpContext.Current.Session["SiteId"]);

            Dictionary<int, string> DefaultValue = new Dictionary<int, string>();

            //if (!Dt.ReportMenuId.HasValue)
            //    throw new Exception("Report Menu not configured in document types");
            if (!Dt.ReportMenuId.HasValue)
                return Redirect((string)System.Configuration.ConfigurationManager.AppSettings["JobsDomain"] + "/GridReport/GridReportLayout/?MenuName=Job Receive Report&DocTypeId=" + id.ToString());


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


        public int PendingToSubmitCount(int id)
        {
            return (_JobReceiveHeaderService.GetJobReceiveHeaderListPendingToSubmit(id, User.Identity.Name)).Count();
        }

        public int PendingToReviewCount(int id)
        {
            return (_JobReceiveHeaderService.GetJobReceiveHeaderListPendingToReview(id, User.Identity.Name)).Count();
        }

        public ActionResult GetSummary(int id)
        {
            return Redirect(System.Configuration.ConfigurationManager.AppSettings["JobsDomain"] + "/Rug/WeavingReceive/GetSummary/" + id);
        }

        public ActionResult GetBarCodesForIAP(int id)
        {
            return Redirect(System.Configuration.ConfigurationManager.AppSettings["JobsDomain"] + "/Rug/WeavingReceive/GetBarCodesForIAP/" + id);
        }


        public ActionResult Wizard(int id)//Document Type Id
        {
            //ControllerAction ca = new ControllerActionService(_unitOfWork).Find(id);
            JobReceiveHeaderViewModel vm = new JobReceiveHeaderViewModel();

            vm.DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            vm.SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            var settings = new JobReceiveSettingsService(_unitOfWork).GetJobReceiveSettingsForDocument(id, vm.DivisionId, vm.SiteId);

            if (settings != null)
            {
                if (settings.WizardMenuId != null)
                {
                    MenuViewModel menuviewmodel = new MenuService(_unitOfWork).GetMenu((int)settings.WizardMenuId);

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
                            return Redirect(System.Configuration.ConfigurationManager.AppSettings[menuviewmodel.URL] + "/" + menuviewmodel.ControllerName + "/" + menuviewmodel.ActionName + "/" + menuviewmodel.RouteId + "?MenuId=" + menuviewmodel.MenuId);
                        }
                    }
                    else
                    {
                        return RedirectToAction(menuviewmodel.ActionName, menuviewmodel.ControllerName, new { MenuId = menuviewmodel.MenuId, id = menuviewmodel.RouteId });
                    }
                }
            }
            return RedirectToAction("Index", new { id = id });
        }

        public ActionResult Import(int id)//Document Type Id
        {
            //ControllerAction ca = new ControllerActionService(_unitOfWork).Find(id);
            JobReceiveHeaderViewModel vm = new JobReceiveHeaderViewModel();

            vm.DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            vm.SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            var settings = new JobReceiveSettingsService(_unitOfWork).GetJobReceiveSettingsForDocument(id, vm.DivisionId, vm.SiteId);

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

        public ActionResult Action_Menu(int Id, int MenuId, string ReturnUrl)
        {
            MenuViewModel menuviewmodel = new MenuService(_unitOfWork).GetMenu(MenuId);

            if (menuviewmodel != null)
            {
                if (!string.IsNullOrEmpty(menuviewmodel.URL))
                {
                    return Redirect(System.Configuration.ConfigurationManager.AppSettings[menuviewmodel.URL] + "/" + menuviewmodel.ControllerName + "/" + menuviewmodel.ActionName + "/" + Id + "?ReturnUrl=" + ReturnUrl);
                }
                else
                {
                    return RedirectToAction(menuviewmodel.ActionName, menuviewmodel.ControllerName, new { id = Id, ReturnUrl = ReturnUrl });
                }
            }
            return null;
        }



        public ActionResult GeneratePrints(string Ids, int DocTypeId)
        {

            if (!string.IsNullOrEmpty(Ids))
            {
                int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
                int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

                var Settings = new JobReceiveSettingsService(_unitOfWork).GetJobReceiveSettingsForDocument(DocTypeId, DivisionId, SiteId);

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

                        var pd = db.JobReceiveHeader.Find(item);

                        LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                        {
                            DocTypeId = pd.DocTypeId,
                            DocId = pd.JobReceiveHeaderId,
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
                                    JobReceiveHeaderRDL cr = new JobReceiveHeaderRDL();
                                    drp.CreateRDLFile("Std_JobReceive_Print", cr.Create_Std_JobReceive_Print());
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
                                    JobReceiveHeaderRDL cr = new JobReceiveHeaderRDL();
                                    drp.CreateRDLFile("Std_JobReceive_Print", cr.Create_Std_JobReceive_Print());
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
                                    JobReceiveHeaderRDL cr = new JobReceiveHeaderRDL();
                                    drp.CreateRDLFile("Std_JobReceive_Print", cr.Create_Std_JobReceive_Print());
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
                    SET @TotalAmount = (SELECT Max(Amount) FROM web.JobReceiveheaderCharges WHERE HeaderTableId = " + item + @" AND ChargeId = 34 ) 
	
	                DECLARE @DocDate DATETIME
                    SET @DocDate = (SELECT DocDate FROM Web.JobReceiveHeaders WHERE JobReceiveHeaderid = " + item + @") 
  
	  
	                DECLARE @UnitDealCnt INT
                    SELECT
                    @UnitDealCnt = sum(CASE WHEN JOL.UnitId != JOL.DealunitId THEN 1 ELSE 0 END)
                    FROM Web.JobReceiveLines L WITH(Nolock)
                    LEFT JOIN Web.JobOrderLines JOL WITH(Nolock) ON JOL.JobOrderLineId = L.JobOrderLineId
                    WHERE L.JobReceiveHeaderId = " + item + @"

                    SELECT
                    --Header Table Fields
                    H.JobreceiveHeaderId,H.DocTypeId,H.DocNo,DocIdCaption + ' No' AS DocIdCaption, H.JobWorkerDocNo,
	                H.SiteId,H.DivisionId,H.DocDate,DTS.DocIdCaption + ' Date' AS DocIdCaptionDate, DocIdCaption+'Due Date' AS DocIdCaptionDueDate, Pp.Name AS ReceiveBy,PS.ProcessName AS ProcessName,
	                H.Remark AS HeaderRemark,DT.DocumentTypeShortName,H.ModifiedBy + ' ' + Replace(replace(convert(NVARCHAR, H.ModifiedDate, 106), ' ', '/'), '/20', '/') + substring(convert(NVARCHAR, H.ModifiedDate), 13, 7) AS ModifiedBy,
                         H.ModifiedDate,(CASE WHEN Isnull(H.Status, 0)= 0 OR Isnull(H.Status, 0)= 8 THEN 0 ELSE 1 END)  AS Status,
                             (CASE WHEN SPR.[Party GST NO] IS NULL THEN 'Yes' ELSE 'No' END ) AS ReverseCharge,
                             VDC.CompanyName,
   	                --Godown Detail
                    G.GodownName,
	                --Person Detail
                    P.Name AS PartyName, DTS.PartyCaption AS  PartyCaption, P.Suffix AS PartySuffix,	
	                isnull(PA.Address, '') + ' ' + isnull(C.CityName, '') + ',' + isnull(PA.ZipCode, '') + (CASE WHEN isnull(CS.StateName, '') <> isnull(S.StateName, '') AND SPR.[Party GST NO]
                        IS NOT NULL THEN ',State : '+isnull(S.StateName,'')+(CASE WHEN S.StateCode IS NULL THEN '' ELSE ', Code : '+S.StateCode END)    ELSE '' END ) AS PartyAddress,
                isnull(S.StateName, '') AS PartyStateName, isnull(S.StateCode, '') AS PartyStateCode,

                P.Mobile AS PartyMobileNo,	SPR.*,
                   --Plan Detail
                    JOH.DocNo AS OrdNo,DTS.ContraDocTypeCaption,
	                --Caption Fields
                    DTS.SignatoryMiddleCaption,DTS.SignatoryRightCaption,
	                --Line Table
                    PD.ProductName,DTS.ProductCaption,U.UnitName,U.DecimalPlaces,DU.UnitName AS DealUnitName,DTS.DealQtyCaption,DU.DecimalPlaces AS DealDecimalPlaces,
                    isnull(L.Qty,0) AS Qty, isnull(L.DealQty, 0) AS DealQty,
                      L.LossQty,L.PassQty,
	                D1.Dimension1Name,DTS.Dimension1Caption,D2.Dimension2Name,DTS.Dimension2Caption,D3.Dimension3Name,DTS.Dimension3Caption,D4.Dimension4Name,DTS.Dimension4Caption,
	                L.LotNo AS LotNo,(CASE WHEN DTS.PrintSpecification >0 THEN L.Specification ELSE '' END)  AS Specification, DTS.SpecificationCaption,DTS.SignatoryleftCaption,L.Remark AS LineRemark,

   	                --STC.Code AS SalesTaxProductCodes,
	                (CASE WHEN H.ProcessId IN(26,28) THEN STC.Code ELSE PSSTC.Code END)  AS SalesTaxProductCodes,
                    (SELECT TOP 1 SalesTaxProductCodeCaption FROM web.SiteDivisionSettings WHERE H.DocDate BETWEEN StartDate AND IsNull(EndDate, getdate()) AND SiteId = H.SiteId AND DivisionId = H.DivisionId)  AS SalesTaxProductCodeCaption,
                       (CASE WHEN DTS.PrintProductGroup > 0 THEN isnull(PG.ProductGroupName, '') ELSE '' END)+(CASE WHEN DTS.PrintProductdescription >0 THEN isnull(','+PD.Productdescription,'') ELSE '' END) AS ProductGroupName,
                         DTS.ProductGroupCaption,  
	                PU.ProductUidName,
	                --(CASE WHEN PS.ProcessName IN('Purchase','Sale') THEN isnull(CGPD.PrintingDescription, CGPD.ChargeGroupProductName) ELSE PS.GSTPrintDesc END)  AS ChargeGroupProductName,
                   PS.GSTPrintDesc AS ChargeGroupProductName,
	                DTS.ProductUidCaption,
   	                --Formula Fields
                    @TotalAmount AS NetAmount, 
	                --Other Fields
                    @UnitDealCnt AS DealUnitCnt,	
	                NULL AS SubReportProcList,	
	                --'StdDocPrintSub_CalculationHeaders ' + convert(NVARCHAR, @Id) + ', ' + '''web.jobOrderheadercharges'''+ ', ' + '''Web.JobOrderLineCharges'''+ ', ' + '''Web.JobOrderLines''' AS SubReportProcList,
                     (CASE WHEN Isnull(H.Status, 0) = 0 OR Isnull(H.Status, 0) = 8 THEN 'Provisional ' + isnull(DT.PrintTitle, DT.DocumentTypeName) ELSE isnull(DT.PrintTitle, DT.DocumentTypeName) END) AS ReportTitle,
          	                'Std_JobReceive_Print.rdl' AS ReportName,
                              SalesTaxGroupProductCaption
                    FROM Web.JobReceiveHeaders H WITH (Nolock)
                    LEFT JOIN web.DocumentTypes DT WITH(Nolock) ON DT.DocumentTypeId=H.DocTypeId
                   LEFT JOIN Web._DocumentTypeSettings DTS WITH (Nolock) ON DTS.DocumentTypeId=DT.DocumentTypeId
                   LEFT JOIN Web.JobReceiveSettings JRS WITH (Nolock) ON JRS.DocTypeId=DT.DocumentTypeId AND JRS.SiteId= H.SiteId AND JRS.DivisionId= H.DivisionId
                    LEFT JOIN web.ViewDivisionCompany VDC WITH (Nolock) ON VDC.DivisionId=H.DivisionId
                   LEFT JOIN Web.Sites SI WITH (Nolock) ON SI.SiteId=H.SiteId
                   LEFT JOIN Web.Divisions DIV WITH (Nolock) ON DIV.DivisionId=H.DivisionId
                   LEFT JOIN Web.Companies Com ON Com.CompanyId = DIV.CompanyId
                    LEFT JOIN Web.Cities CC WITH (Nolock) ON CC.CityId=Com.CityId
                   LEFT JOIN Web.States CS WITH (Nolock) ON CS.StateId=CC.StateId
                   LEFT JOIN Web.Processes PS WITH (Nolock) ON PS.ProcessId=H.ProcessId
                   LEFT JOIN Web.SalesTaxProductCodes PSSTC WITH (Nolock) ON PSSTC.SalesTaxProductCodeId=PS.SalesTaxProductCodeId
                   LEFT JOIN Web.People P WITH (Nolock) ON P.PersonID=H.JobWorkerId
                   LEFT JOIN web.Godowns G WITH (Nolock) ON G.GodownId=H.GodownId
                   LEFT JOIN (SELECT TOP 1 * FROM web.SiteDivisionSettings WHERE @DocDate BETWEEN StartDate AND IsNull(EndDate, getdate()) ORDER BY StartDate) SDS ON H.DivisionId = SDS.DivisionId AND  H.SiteId = SDS.SiteId
                   LEFT JOIN(SELECT* FROM Web.PersonAddresses WITH (nolock) WHERE AddressType IS NULL) PA ON PA.PersonId = P.PersonID
                LEFT JOIN Web.Cities C WITH (nolock) ON C.CityId = PA.CityId
                LEFT JOIN Web.States S WITH (Nolock) ON S.StateId=C.StateId
                LEFT JOIN web.People Pp WITH (Nolock) ON Pp.PersonID=H.JobReceiveById
                LEFT JOIN web.JobReceiveLines L WITH (Nolock) ON L.JobReceiveHeaderid=H.JobReceiveHeaderId
                LEFT JOIN Web.JobOrderLines JOL WITH (Nolock) ON JOL.JobOrderLineId=L.JobOrderLineId
                LEFT JOIN Web.ProductUids PU WITH (Nolock) ON PU.ProductUIDId=L.ProductUidId
                LEFT JOIN Web.JobOrderHeaders JOH WITH (Nolock) ON JOH.JobOrderHeaderid=JOL.JobOrderHeaderId
                LEFT JOIN web.Products PD WITH (Nolock) ON PD.ProductId=JOL.ProductId
                LEFT JOIN web.ProductGroups PG WITH (Nolock) ON PG.ProductGroupId=PD.ProductGroupid
                LEFT JOIN Web.SalesTaxProductCodes STC WITH (Nolock) ON STC.SalesTaxProductCodeId= IsNull(PD.SalesTaxProductCodeId, Pg.DefaultSalesTaxProductCodeId)
                    LEFT JOIN Web.Dimension1 D1 WITH(Nolock) ON D1.Dimension1Id=JOL.Dimension1Id
                   LEFT JOIN web.Dimension2 D2 WITH (Nolock) ON D2.Dimension2Id=JOL.Dimension2Id
                   LEFT JOIN web.Dimension3 D3 WITH (Nolock) ON D3.Dimension3Id=JOL.Dimension3Id
                   LEFT JOIN Web.Dimension4 D4 WITH (nolock) ON D4.Dimension4Id=JOL.Dimension4Id
                   LEFT JOIN web.Units U WITH (Nolock) ON U.UnitId=PD.UnitId
                   LEFT JOIN web.Units DU WITH (Nolock) ON DU.UnitId=L.DealUnitId
                   LEFT JOIN Web.Std_PersonRegistrations SPR WITH (Nolock) ON SPR.CustomerId=H.JobWorkerId
                      WHERE H.JobReceiveHeaderid= " + item + @"
                    ORDER BY L.Sr";

            ListofQuery QryMain = new ListofQuery();
            QryMain.Query = QueryMain;
            QryMain.QueryName = nameof(QueryMain);
            DocumentPrintData.Add(QryMain);


            return DocumentPrintData;

        }


        public ActionResult GetCustomPerson(string searchTerm, int pageSize, int pageNum, int filter)//DocTypeId
        {
            var Query = _JobReceiveHeaderService.GetCustomPerson(filter, searchTerm);
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

		public ActionResult GetCustomPerson_WithProcess(string searchTerm, int pageSize, int pageNum, int filter, int? filter2)//DocTypeId
        {
            var Query = _JobReceiveHeaderService.GetCustomPerson_WithProcess(filter, searchTerm, filter2);
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

        public ActionResult GetGodown(string searchTerm, int pageSize, int pageNum, int filter)//DocTypeId
        {
            var Query = _JobReceiveHeaderService.GetGodown(filter, searchTerm);
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

        public JsonResult IsDuplicatePartyDocNo(int PersonId, string PartyDocNo)
        {
            var temp = (from H in db.JobReceiveHeader
                        where H.JobWorkerId == PersonId && H.JobWorkerDocNo == PartyDocNo
                        select H).FirstOrDefault();
            if (temp == null)
            {
                return Json(false);
            }
            else{
                return Json(true);
            }
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
            int Stockline = (new JobReceiveLineService(_unitOfWork).GetLineListForIndex(id)).Count();
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
    }
}
