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
using Microsoft.AspNet.Identity;
using System.Configuration;
using Model.ViewModel;
using System.Data.SqlClient;
using System.Xml.Linq;
using CustomEventArgs;
using DocumentEvents;
using JobOrderDocumentEvents;
using Reports.Reports;
using Reports.Controllers;
using Model.ViewModels;
using Mailer.Model;

namespace Jobs.Controllers
{

    [Authorize]
    public class JobOrderHeaderController : System.Web.Mvc.Controller
    {
        private ApplicationDbContext context = new ApplicationDbContext();
        private bool EventException = false;

        List<string> UserRoles = new List<string>();
        ActiivtyLogViewModel LogVm = new ActiivtyLogViewModel();

        bool TimePlanValidation = true;
        string ExceptionMsg = "";
        bool Continue = true;

        IJobOrderHeaderService _JobOrderHeaderService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;

        public JobOrderHeaderController(IJobOrderHeaderService PurchaseOrderHeaderService, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _JobOrderHeaderService = PurchaseOrderHeaderService;
            _exception = exec;
            _unitOfWork = unitOfWork;
            if (!JobOrderEvents.Initialized)
            {
                JobOrderEvents Obj = new JobOrderEvents();
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
            IQueryable<JobOrderHeaderViewModel> p = _JobOrderHeaderService.GetJobOrderHeaderList(id, User.Identity.Name);
            ViewBag.Name = new DocumentTypeService(_unitOfWork).Find(id).DocumentTypeName;
            PrepareViewBag(id);
            ViewBag.PendingToSubmit = PendingToSubmitCount(id);
            ViewBag.PendingToReview = PendingToReviewCount(id);
            ViewBag.IndexStatus = "All";


            ViewBag.id = id;
            return View(p);
        }

        public ActionResult Index_PendingToSubmit(int id)
        {
            IQueryable<JobOrderHeaderViewModel> p = _JobOrderHeaderService.GetJobOrderHeaderListPendingToSubmit(id, User.Identity.Name);

            PrepareViewBag(id);
            ViewBag.PendingToSubmit = PendingToSubmitCount(id);
            ViewBag.PendingToReview = PendingToReviewCount(id);
            ViewBag.IndexStatus = "PTS";
            return View("Index", p);
        }

        public ActionResult Index_PendingToReview(int id)
        {
            IQueryable<JobOrderHeaderViewModel> p = _JobOrderHeaderService.GetJobOrderHeaderListPendingToReview(id, User.Identity.Name);
            PrepareViewBag(id);
            ViewBag.PendingToSubmit = PendingToSubmitCount(id);
            ViewBag.PendingToReview = PendingToReviewCount(id);
            ViewBag.IndexStatus = "PTR";
            return View("Index", p);
        }

        public ActionResult Index_PendingToReceive(int id)
        {
            IQueryable<JobOrderHeaderViewModel> p = _JobOrderHeaderService.GetJobOrderHeaderListPendingToReceive(id, User.Identity.Name);
            PrepareViewBag(id);
            ViewBag.PendingToSubmit = PendingToSubmitCount(id);
            ViewBag.PendingToReview = PendingToReviewCount(id);
            ViewBag.IndexStatus = "PTREC";
            return View("Index", p);
        }

        private void PrepareViewBag(int id)
        {
            DocumentType DocType = new DocumentTypeService(_unitOfWork).Find(id);
            DocumentTypeSettingsViewModel DTS = new DocumentTypeSettingsService(_unitOfWork).GetDocumentTypeSettingsForDocument(id);
           
            int Cid = DocType.DocumentCategoryId;
            ViewBag.DocTypeList = new DocumentTypeService(_unitOfWork).FindByDocumentCategory(Cid).ToList();
            //ViewBag.ReasonList = new ReasonService(_unitOfWork).GetReasonList(DC.DocumentCategoryName).ToList();
            ViewBag.Name = DocType.DocumentTypeName;
            ViewBag.PartyCaption = DTS.PartyCaption;
            ViewBag.IsDefaultCreateFromWizard = DTS.IsDefaultCreateFromWizard;
            ViewBag.id = id;
            ViewBag.UnitConvForList = (from p in context.UnitConversonFor
                                       select p).ToList();
              ViewBag.AdminSetting =UserRoles.Contains("Admin").ToString();
            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            
            var settings = new JobOrderSettingsService(_unitOfWork).GetJobOrderSettingsForDocument(id, DivisionId, SiteId);
            if (settings != null)
            {
                ViewBag.WizardId = settings.WizardMenuId;
                ViewBag.IsPostedInStock = settings.isPostedInStock;
                ViewBag.isVisibleCostCenter = settings.isVisibleCostCenter;
				ViewBag.isVisibleProcessHeader = settings.isVisibleProcessHeader;
                ViewBag.ImportMenuId = settings.ImportMenuId;
                ViewBag.SqlProcDocumentPrint = settings.SqlProcDocumentPrint;
                ViewBag.SqlProcGatePass = settings.SqlProcGatePass;
                ViewBag.ExportMenuId = settings.ExportMenuId;
            }



        }


        public ActionResult GetCustomPerson_WithProcess(string searchTerm, int pageSize, int pageNum, int filter, int? filter2)//DocTypeId
        {
            var Query = _JobOrderHeaderService.GetCustomPerson_WithProcess(filter, searchTerm, filter2);
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

        public ActionResult GetMachine(string searchTerm, int pageSize, int pageNum, int filter)//DocTypeId
        {
            var Query = _JobOrderHeaderService.GetMachine(filter, searchTerm);
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
        public ActionResult BarcodePrint(int id)
        {
            string GenDocId = "";
            JobOrderHeader header = _JobOrderHeaderService.Find(id);
            GenDocId = header.DocTypeId.ToString() + '-' + header.DocNo;
            return Redirect((string)System.Configuration.ConfigurationManager.AppSettings["JobsDomain"] + "/Report_BarcodePrint/PrintBarCode/?GenHeaderId=" + GenDocId + "&queryString=" + GenDocId);            
        }


        public JsonResult GetJobWorkerHelpList(int Processid, string term)//Order Header ID
        {
            return Json(_JobOrderHeaderService.GetJobWorkerHelpList(Processid, term), JsonRequestBehavior.AllowGet);
        }


        // GET: /JobOrderHeader/Create
        public ActionResult Create(int id)//DocumentTypeId
        {
            JobOrderHeaderViewModel p = new JobOrderHeaderViewModel();
            p.DocDate = DateTime.Now;
            p.DueDate = DateTime.Now;
            p.CreatedDate = DateTime.Now;
            p.DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            p.SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            List<DocumentTypeHeaderAttributeViewModel> tem = new DocumentTypeService(_unitOfWork).GetDocumentTypeHeaderAttribute(id).ToList();
            p.DocumentTypeHeaderAttributes = tem;

            //Getting Settings
            var settings = new JobOrderSettingsService(_unitOfWork).GetJobOrderSettingsForDocument(id, p.DivisionId, p.SiteId);

            if (settings == null && UserRoles.Contains("SysAdmin"))
            {
                return RedirectToAction("Create", "JobOrderSettings", new { id = id }).Warning("Please create job order settings");
            }
            else if (settings == null && !UserRoles.Contains("SysAdmin"))
            {
                return View("~/Views/Shared/InValidSettings.cshtml");
            }
            p.JobOrderSettings = Mapper.Map<JobOrderSettings, JobOrderSettingsViewModel>(settings);


            if ((settings.isVisibleProcessHeader ?? false) == false)
            {
                p.ProcessId = settings.ProcessId;
            }

            if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, id, p.ProcessId, this.ControllerContext.RouteData.Values["controller"].ToString(), "Create") == false)
            {
                return View("~/Views/Shared/PermissionDenied.cshtml").Warning("You don't have permission to do this task.");
            }


            p.DocTypeId = id;

            p.DocumentTypeSettings = new DocumentTypeSettingsService(_unitOfWork).GetDocumentTypeSettingsForDocument(p.DocTypeId);

            List<PerkViewModel> Perks = new List<PerkViewModel>();
            //Perks
            if (p.JobOrderSettings.Perks != null)
            {
                foreach (var item in p.JobOrderSettings.Perks.Split(',').ToList())
                {
                    PerkViewModel temp = Mapper.Map<Perk, PerkViewModel>(new PerkService(_unitOfWork).Find(Convert.ToInt32(item)));
                    var DocTypePerk = (from p2 in context.PerkDocumentType
                                       where p2.DocTypeId == id && p2.PerkId == temp.PerkId && p2.SiteId == p.SiteId && p2.DivisionId == p.DivisionId
                                       select p2).FirstOrDefault();
                    if (DocTypePerk != null)
                    {
                        temp.Base = DocTypePerk.Base;
                        temp.Worth = DocTypePerk.Worth;
                        temp.CostConversionMultiplier = DocTypePerk.CostConversionMultiplier;
                        temp.IsEditableRate = DocTypePerk.IsEditableRate;
                    }
                    else
                    {
                        temp.Base = 0;
                        temp.Worth = 0;
                        temp.CostConversionMultiplier = 0;
                        temp.IsEditableRate = true;
                    }
                    Perks.Add(temp);
                }
            }

            if (p.JobOrderSettings.isVisibleCostCenter)
            {
                p.CostCenterName = new JobOrderHeaderService(_unitOfWork).FGetJobOrderCostCenter(p.DocTypeId, p.DocDate, p.DivisionId, p.SiteId);
            }

            p.PerkViewModel = Perks;
            p.UnitConversionForId = settings.UnitConversionForId;




            p.TermsAndConditions = settings.TermsAndConditions;


            PrepareViewBag(id);
            

            var LastTrRec = (from H in context.JobOrderHeader
                             where H.SiteId == p.SiteId && H.DivisionId == p.DivisionId && H.DocTypeId == p.DocTypeId && H.CreatedBy == User.Identity.Name
                             orderby H.JobOrderHeaderId descending
                             select new
                             {
                                 OrderById = H.OrderById,
                             }).FirstOrDefault();
            if (LastTrRec != null)
                p.OrderById = LastTrRec.OrderById;
            else
                p.OrderById = new EmployeeService(_unitOfWork).GetEmloyeeForUser(User.Identity.GetUserId());

            if (settings.isVisibleGodown == true && System.Web.HttpContext.Current.Session["DefaultGodownId"] != null)
                p.GodownId = (int)System.Web.HttpContext.Current.Session["DefaultGodownId"];

            if (settings.DueDays.HasValue)
            {
                p.DueDate = _JobOrderHeaderService.AddDueDate(DateTime.Now, settings.DueDays.Value);
            }
            else
                p.DueDate = DateTime.Now;

            p.DocNo = new DocumentTypeService(_unitOfWork).FGetNewDocNo("DocNo", ConfigurationManager.AppSettings["DataBaseSchema"] + ".JobOrderHeaders", p.DocTypeId, p.DocDate, p.DivisionId, p.SiteId);
            ViewBag.Mode = "Add";
            ViewBag.Name = new DocumentTypeService(_unitOfWork).Find(id).DocumentTypeName;
            ViewBag.id = id;
            return View(p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Post(JobOrderHeaderViewModel svm)
        {
            bool BeforeSave = true;
            bool CostCenterGenerated = false;

            JobOrderHeader s = Mapper.Map<JobOrderHeaderViewModel, JobOrderHeader>(svm);

            var settings = new JobOrderSettingsService(_unitOfWork).GetJobOrderSettingsForDocument(svm.DocTypeId, svm.DivisionId, svm.SiteId);

            Settings settingToGenerateProdOrder = new SettingsService(_unitOfWork).GetSettingsForDocument(SettingFieldNameConstants.GeneratedProdOrderDocTypeId, svm.DivisionId, svm.SiteId, svm.DocTypeId, null);
            Settings settingToGenerateProdOrderBuyerId = new SettingsService(_unitOfWork).GetSettingsForDocument(SettingFieldNameConstants.GeneratedProdOrderBuyerId, svm.DivisionId, svm.SiteId, svm.DocTypeId, null);

            if (settings != null)
            {
                if (svm.JobOrderSettings.isVisiblePaymentTerms == true)
                {
                    if ((svm.PayTermAdvancePer ?? 0) + (svm.PayTermOnDeliveryPer ?? 0) + (svm.PayTermOnDueDatePer ?? 0) > 100)
                    {
                        ModelState.AddModelError("PayTermAdvancePer", "Total of Advance %, Delivery % and Due Date % should be equal to 100.");
                    }
                    if ((svm.PayTermBankPer ?? 0) + (svm.PayTermCashPer ?? 0) > 100)
                    {
                        ModelState.AddModelError("PayTermAdvancePer", "Total of Bank %, and Cash % should be equal to 100.");
                    }
                }


                if (svm.JobOrderSettings.isMandatoryCostCenter == true && (string.IsNullOrEmpty(svm.CostCenterName)))
                {
                    ModelState.AddModelError("CostCenterName", "The CostCenter field is required");
                }
                if (svm.JobOrderSettings.isMandatoryMachine == true && (svm.MachineId <= 0 || svm.MachineId == null))
                {
                    ModelState.AddModelError("MachineId", "The Machine field is required");
                }
                if (svm.JobOrderSettings.isVisibleGodown == true && svm.JobOrderSettings.isMandatoryGodown == true && !svm.GodownId.HasValue)
                {
                    ModelState.AddModelError("GodownId", "The Godown field is required");
                }
                if (settings.MaxDays.HasValue && (svm.DueDate - svm.DocDate).Days > settings.MaxDays.Value)
                {
                    ModelState.AddModelError("DueDate", "DueDate is exceeding MaxDueDays.");
                }
            }

            if (settingToGenerateProdOrder != null)
            {
                Person JW = context.Persons.Where(m => m.PersonID == svm.JobWorkerId).FirstOrDefault();
                if (JW.IsSisterConcern ==true )
                {
                    Site S = new SiteService(_unitOfWork).FindByPerson(JW.PersonID);
                    if (S == null)
                    {
                        ModelState.AddModelError("JobWorkerId", "Site Setting Not Find For This Job Worker");
                    }
                    else
                    {
                        if (S.DefaultDivisionId == null)
                        {
                            ModelState.AddModelError("JobWorkerId", "Division Setting Not Find For This Job Worker");
                        }
                    }
                }
            }

            SiteDivisionSettings SiteDivisionSettings = new SiteDivisionSettingsService(_unitOfWork).GetSiteDivisionSettings(svm.SiteId, svm.DivisionId, svm.DocDate);
            if (SiteDivisionSettings != null)
            {
                if (SiteDivisionSettings.IsApplicableGST == true)
                {
                    if (svm.SalesTaxGroupPersonId == 0 || svm.SalesTaxGroupPersonId == null)
                    {
                        ModelState.AddModelError("", "Sales Tax Group Person is not defined for party, it is required.");
                    }
                }
            }

            if (!string.IsNullOrEmpty(svm.CostCenterName))
            {
                string CostCenterValidation = _JobOrderHeaderService.ValidateCostCenter(svm.DocTypeId, svm.JobOrderHeaderId, svm.JobWorkerId, svm.CostCenterName);
                if (!string.IsNullOrEmpty(CostCenterValidation))
                    ModelState.AddModelError("CostCenterName", CostCenterValidation);
            }
            #region BeforeSaveEvents

            try
            {

                if (svm.JobOrderHeaderId <= 0)
                    BeforeSave = JobOrderDocEvents.beforeHeaderSaveEvent(this, new JobEventArgs(svm.JobOrderHeaderId, EventModeConstants.Add), ref context);
                else
                    BeforeSave = JobOrderDocEvents.beforeHeaderSaveEvent(this, new JobEventArgs(svm.JobOrderHeaderId, EventModeConstants.Edit), ref context);

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

                if (svm.JobOrderHeaderId <= 0)
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

            if (ModelState.IsValid && BeforeSave && !EventException && (TimePlanValidation || Continue))
            {
                //CreateLogic
                #region CreateRecord
                if (svm.JobOrderHeaderId <= 0)
                {

                    if (!string.IsNullOrEmpty(svm.CostCenterName))
                    {

                        var CostCenter = new CostCenterService(_unitOfWork).Find(svm.CostCenterName, svm.DivisionId, svm.SiteId, svm.DocTypeId);
                        if (CostCenter != null)
                        {
                            s.CostCenterId = CostCenter.CostCenterId;
                            if (s.CostCenterId.HasValue)
                            {
                                var costcen = new CostCenterService(_unitOfWork).Find(s.CostCenterId.Value);
                                costcen.ProcessId = svm.ProcessId;
                                costcen.ObjectState = Model.ObjectState.Modified;
                                context.CostCenter.Add(costcen);
                            }
                        }
                        else
                        {
                            CostCenter Cs = new CostCenter();
                            Cs.CostCenterName = svm.CostCenterName;
                            Cs.DivisionId = svm.DivisionId;
                            Cs.SiteId = svm.SiteId;
                            Cs.DocTypeId = svm.DocTypeId;
                            Cs.ProcessId = svm.ProcessId;
                            Cs.LedgerAccountId = new LedgerAccountService(_unitOfWork).GetLedgerAccountByPersondId(svm.JobWorkerId).LedgerAccountId;
                            Cs.CreatedBy = User.Identity.Name;
                            Cs.ModifiedBy = User.Identity.Name;
                            Cs.CreatedDate = DateTime.Now;
                            Cs.ModifiedDate = DateTime.Now;
                            Cs.IsActive = true;
                            Cs.ReferenceDocNo = svm.DocNo;
                            Cs.ReferenceDocTypeId = svm.DocTypeId;
                            Cs.StartDate = svm.DocDate;
                            Cs.ParentCostCenterId = new ProcessService(_unitOfWork).Find(svm.ProcessId).CostCenterId;
                            Cs.ObjectState = Model.ObjectState.Added;
                            context.CostCenter.Add(Cs);
                            s.CostCenterId = Cs.CostCenterId;

                            new CostCenterStatusService(_unitOfWork).CreateLineStatus(Cs.CostCenterId, ref context, true);
                            CostCenterGenerated = true;

                        }

                    }

                    if (settings.isAllowedToMaterialIssue == true)
                    {
                        StockHeader SH = new StockHeader();

                        SH.DivisionId = svm.DivisionId;
                        SH.SiteId = svm.SiteId;
                        SH.DocTypeId = svm.DocTypeId;
                        SH.ProcessId = svm.ProcessId;
                        SH.ReferenceDocId = svm.JobOrderHeaderId;
                        SH.ReferenceDocTypeId = svm.DocTypeId;
                        SH.DocDate = svm.DocDate;
                        SH.DocNo = svm.DocNo;
                        SH.PersonId = svm.JobWorkerId;
                        SH.GodownId = svm.GodownId;
                        SH.Remark = svm.Remark;
                        SH.Status = svm.Status;
                        SH.CreatedBy = User.Identity.Name;
                        SH.ModifiedBy = User.Identity.Name;
                        SH.CreatedDate = DateTime.Now;
                        SH.ModifiedDate = DateTime.Now;
                        SH.ObjectState = Model.ObjectState.Added;
                        context.StockHeader.Add(SH);

                        s.StockHeaderId = SH.StockHeaderId;

                    }


                    if (settings.isAllowDirectReceive == true)
                    {
                        JobReceiveHeader RH = new JobReceiveHeader();

                        RH.DivisionId = svm.DivisionId;
                        RH.SiteId = svm.SiteId;
                        RH.DocTypeId = svm.DocTypeId;
                        RH.ProcessId = svm.ProcessId;
                        RH.ReferenceDocId = svm.JobOrderHeaderId;
                        RH.ReferenceDocTypeId = svm.DocTypeId;
                        RH.DocDate = svm.DocDate;
                        RH.DocNo = svm.DocNo;
                        RH.JobWorkerId = svm.JobWorkerId;
                        RH.GodownId = (int)svm.GodownId;
                        RH.Remark = svm.Remark;
                        RH.Status = svm.Status;
                        RH.CreatedBy = User.Identity.Name;
                        RH.ModifiedBy = User.Identity.Name;
                        RH.CreatedDate = DateTime.Now;
                        RH.ModifiedDate = DateTime.Now;
                        RH.ObjectState = Model.ObjectState.Added;
                        context.JobReceiveHeader.Add(RH);

                    }

                    if (settings.isVisibleTransportDetail == true)
                    {
                        StockHeaderTransport ST = new StockHeaderTransport();

                        ST.DivisionId = svm.DivisionId;
                        ST.SiteId = svm.SiteId;
                        ST.DocTypeId = svm.DocTypeId;
                        ST.ProcessId = svm.ProcessId;
                        ST.ReferenceDocId = svm.JobOrderHeaderId;
                        ST.ReferenceDocTypeId = svm.DocTypeId;
                        ST.DocDate = svm.DocDate;
                        ST.DocNo = svm.DocNo;
                        ST.PersonId = svm.JobWorkerId;
                        ST.GodownId = svm.GodownId;
                        ST.Remark = svm.Remark;
                        ST.Status = svm.Status;
                        ST.TransportId = svm.TransportId;
                        ST.VehicleNo = svm.VehicleNo;
                        ST.LrNo = svm.LrNo;
                        ST.EWayBillNo = svm.EWayBillNo;
                        ST.EWayBillDate = svm.EWayBillDate;
                        ST.LrDate = svm.LrDate;
                        ST.PaymentType = svm.PaymentType;
                        ST.Destination = svm.Destination;
                        ST.CreatedBy = User.Identity.Name;
                        ST.ModifiedBy = User.Identity.Name;
                        ST.CreatedDate = DateTime.Now;
                        ST.ModifiedDate = DateTime.Now;
                        ST.ObjectState = Model.ObjectState.Added;
                        context.StockHeaderTransport.Add(ST);

                        s.StockHeaderId = ST.StockHeaderId;

                    }


                    s.CreatedDate = DateTime.Now;
                    s.ModifiedDate = DateTime.Now;
                    s.ActualDueDate = s.DueDate;
                    s.ActualDocDate = s.DocDate;
                    s.CreatedBy = User.Identity.Name;
                    s.ModifiedBy = User.Identity.Name;
                    s.Status = (int)StatusConstants.Drafted;
                    s.ObjectState = Model.ObjectState.Added;
                    context.JobOrderHeader.Add(s);


                    new JobOrderHeaderStatusService(_unitOfWork).CreateHeaderStatus(s.JobOrderHeaderId, ref context, true);

                    if (svm.PerkViewModel != null)
                    {
                        int perkpid = 0;
                        foreach (PerkViewModel item in svm.PerkViewModel)
                        {
                            JobOrderPerk perk = Mapper.Map<PerkViewModel, JobOrderPerk>(item);
                            perk.CreatedBy = User.Identity.Name;
                            perk.CreatedDate = DateTime.Now;
                            perk.ModifiedBy = User.Identity.Name;
                            perk.ModifiedDate = DateTime.Now;
                            perk.JobOrderHeaderId = s.JobOrderHeaderId;
                            perk.JobOrderPerkId = perkpid;
                            perk.ObjectState = Model.ObjectState.Added;
                            context.JobOrderPerk.Add(perk);
                            perkpid++;
                        }
                    }

                    if (svm.DocumentTypeHeaderAttributes != null)
                    {
                        foreach (var Attributes in svm.DocumentTypeHeaderAttributes)
                        {
                            JobOrderHeaderAttributes JobOrderHeaderAttribute = (from A in context.JobOrderHeaderAttributes
                                                                                      where A.HeaderTableId == s.JobOrderHeaderId 
                                                                                      && A.DocumentTypeHeaderAttributeId == Attributes.DocumentTypeHeaderAttributeId
                                                                                      select A).FirstOrDefault();

                            if (JobOrderHeaderAttribute != null)
                            {
                                JobOrderHeaderAttribute.Value = Attributes.Value;
                                JobOrderHeaderAttribute.ObjectState = Model.ObjectState.Modified;
                                context.JobOrderHeaderAttributes.Add(JobOrderHeaderAttribute);
                            }
                            else
                            {
                                JobOrderHeaderAttributes HeaderAttribute = new JobOrderHeaderAttributes()
                                {
                                    HeaderTableId = s.JobOrderHeaderId,
                                    Value = Attributes.Value,
                                    DocumentTypeHeaderAttributeId = Attributes.DocumentTypeHeaderAttributeId,
                                };
                                HeaderAttribute.ObjectState = Model.ObjectState.Added;
                                context.JobOrderHeaderAttributes.Add(HeaderAttribute);
                            }
                        }
                    }

                    try
                    {
                        JobOrderDocEvents.onHeaderSaveEvent(this, new JobEventArgs(s.JobOrderHeaderId, EventModeConstants.Add), ref context);
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
                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                        PrepareViewBag(svm.DocTypeId);
                        ViewBag.Mode = "Add";
                        return View("Create", svm);
                    }

                    if (settings != null)
                    {
                        new CommonService().ExecuteCustomiseEvents(settings.Event_AfterHeaderSave, new object[] { _unitOfWork, s.JobOrderHeaderId, "A" });
                    }

                    try
                    {
                        JobOrderDocEvents.afterHeaderSaveEvent(this, new JobEventArgs(s.JobOrderHeaderId, EventModeConstants.Add), ref context);
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                    }

                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = s.DocTypeId,
                        DocId = s.JobOrderHeaderId,
                        ActivityType = (int)ActivityTypeContants.Added,
                        DocNo = s.DocNo,
                        DocDate = s.DocDate,
                        DocStatus = s.Status,
                    }));


                    //Update DocId in COstCenter
                    if (s.CostCenterId.HasValue && CostCenterGenerated)
                    {
                        var CC = context.CostCenter.Find(s.CostCenterId);
                        CC.ReferenceDocId = s.JobOrderHeaderId;
                        CC.ObjectState = Model.ObjectState.Modified;
                        context.CostCenter.Add(CC);

                        context.SaveChanges();
                    }

                    if (s.StockHeaderId.HasValue && settings.isAllowedToMaterialIssue == true)
                    {
                        var CC = context.StockHeader.Find(s.StockHeaderId);
                        CC.ReferenceDocId = s.JobOrderHeaderId;
                        CC.ObjectState = Model.ObjectState.Modified;
                        context.StockHeader.Add(CC);

                        context.SaveChanges();
                    }

                    if (settings.isAllowDirectReceive == true)
                    {
                        var RH = context.JobReceiveHeader.Where(m => m.SiteId == s.SiteId && m.DivisionId == s.DivisionId && m.DocTypeId == s.DocTypeId && m.DocNo == s.DocNo).FirstOrDefault();
                        if (RH != null)
                        {
                            RH.ReferenceDocId = s.JobOrderHeaderId;
                            RH.ObjectState = Model.ObjectState.Modified;
                            context.JobReceiveHeader.Add(RH);
                            context.SaveChanges();
                        }
                    }

                    // To Create ProdOrder Only For IsSisterConcern ==true
                    if (settingToGenerateProdOrder != null)
                    {
                        Person JW = context.Persons.Where(m => m.PersonID == svm.JobWorkerId).FirstOrDefault();
                        if (JW.IsSisterConcern == true)
                        {
                            Site S = new SiteService(_unitOfWork).FindByPerson(JW.PersonID);
                            DocumentType DT = new DocumentTypeService(_unitOfWork).FindByName(settingToGenerateProdOrder.Value);

                            ProdOrderHeader POH = new ProdOrderHeader();
                            POH.DivisionId = (int)S.DefaultDivisionId;
                            POH.SiteId = S.SiteId;
                            POH.DocTypeId = DT.DocumentTypeId;
                            POH.ReferenceDocId = s.JobOrderHeaderId;
                            POH.ReferenceDocTypeId = svm.DocTypeId;
                            POH.DocDate = svm.DocDate;
                            POH.DueDate = svm.DueDate;
                            POH.DocNo = svm.DocNo;
                            POH.Remark = svm.Remark;

                            if (settingToGenerateProdOrderBuyerId != null)
                            {   int BuyerId =Convert.ToInt32(settingToGenerateProdOrderBuyerId.Value);
                                Person P = new PersonService(_unitOfWork).Find(BuyerId);
                                POH.BuyerId = P.PersonID;
                            }

                            POH.Status = svm.Status;
                            POH.CreatedBy = User.Identity.Name;
                            POH.ModifiedBy = User.Identity.Name;
                            POH.CreatedDate = DateTime.Now;
                            POH.ModifiedDate = DateTime.Now;
                            POH.ObjectState = Model.ObjectState.Added;
                            context.ProdOrderHeader.Add(POH);
                            context.SaveChanges();
                        }
                    }

                    return RedirectToAction("Modify", "JobOrderHeader", new { Id = s.JobOrderHeaderId }).Success("Data saved successfully");

                }
                #endregion


                //EditLogic
                #region EditRecord

                else
                {
                    bool GodownChanged = false;
                    bool JobWorkerChanged = false;
                    bool DocDateChanged = false;
                    List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();

                    JobOrderHeader temp = context.JobOrderHeader.Find(s.JobOrderHeaderId);

                    GodownChanged = (temp.GodownId == s.GodownId) ? false : true;
                    JobWorkerChanged = (temp.JobWorkerId == s.JobWorkerId) ? false : true;
                    DocDateChanged = (temp.DocDate == s.DocDate) ? false : true;

                    JobOrderHeader ExRec = Mapper.Map<JobOrderHeader>(temp);

                    int status = temp.Status;

                    if (temp.Status != (int)StatusConstants.Drafted && temp.Status != (int)StatusConstants.Import)
                        temp.Status = (int)StatusConstants.Modified;


                    if (!string.IsNullOrEmpty(svm.CostCenterName))
                    {
                        var CostCenter = new CostCenterService(_unitOfWork).Find(svm.CostCenterName, svm.DivisionId, svm.SiteId, svm.DocTypeId);
                        if (CostCenter != null)
                        {
                            temp.CostCenterId = CostCenter.CostCenterId;
                            if (temp.CostCenterId.HasValue)
                            {
                                var costcen = (from p in context.CostCenter
                                               where p.CostCenterId == temp.CostCenterId
                                               select p).FirstOrDefault();
                                costcen.ProcessId = svm.ProcessId;
                                costcen.LedgerAccountId = new LedgerAccountService(_unitOfWork).GetLedgerAccountByPersondId(svm.JobWorkerId).LedgerAccountId;

                                costcen.ObjectState = Model.ObjectState.Modified;
                                context.CostCenter.Add(costcen);
                            }
                        }
                        else
                        {

                            var ExistingCostCenter = context.CostCenter.Find(temp.CostCenterId);

                            ExistingCostCenter.CostCenterName = svm.CostCenterName;
                            ExistingCostCenter.ObjectState = Model.ObjectState.Modified;

                            context.CostCenter.Add(ExistingCostCenter);
                        }

                    }

                        temp.ProcessId = s.ProcessId;
                        temp.DocDate = s.DocDate;
                        temp.DueDate = s.DueDate;
                        temp.UnitConversionForId = s.UnitConversionForId;
                        temp.ProcessId = s.ProcessId;
                        temp.JobWorkerId = s.JobWorkerId;
                        temp.MachineId = s.MachineId;
                        temp.BillToPartyId = s.BillToPartyId;
                        temp.OrderById = s.OrderById;
                        temp.ReasonId = s.ReasonId;
                        temp.ActualDueDate = s.DueDate;
                        temp.GodownId = s.GodownId;
                        temp.TermsAndConditions = s.TermsAndConditions;
                        temp.DocNo = s.DocNo;
                        temp.DeliveryTermsId = s.DeliveryTermsId;
                        temp.ShipToAddressId = s.ShipToAddressId;
                        temp.CurrencyId = s.CurrencyId;
                        temp.SalesTaxGroupPersonId = s.SalesTaxGroupPersonId;
                        temp.ShipMethodId = s.ShipMethodId;
                        temp.DocumentShipMethodId = s.DocumentShipMethodId;
                        temp.TransporterId = s.TransporterId;
                        temp.IsDoorDelivery = s.IsDoorDelivery;
                        temp.AgentId = s.AgentId;
                        temp.PayTermAdvancePer = s.PayTermAdvancePer;
                        temp.PayTermOnDeliveryPer = s.PayTermOnDeliveryPer;
                        temp.PayTermOnDueDatePer = s.PayTermOnDueDatePer;
                        temp.PayTermCashPer = s.PayTermCashPer;
                        temp.PayTermBankPer = s.PayTermBankPer;
                        temp.Remark = s.Remark;
                        temp.FinancierId = s.FinancierId;
                        temp.SalesExecutiveId = s.SalesExecutiveId;
                        temp.CreditDays = s.CreditDays;
                        temp.ModifiedDate = DateTime.Now;
                        temp.ModifiedBy = User.Identity.Name;
                        temp.ObjectState = Model.ObjectState.Modified;
                        context.JobOrderHeader.Add(temp);

                    if (temp.JobWorkerId != ExRec.JobWorkerId || temp.DocNo != ExRec.DocNo || temp.DocDate != ExRec.DocDate)
                    {
                        _JobOrderHeaderService.UpdateProdUidJobWorkers(ref context, temp);
                    }

                    if (svm.PerkViewModel != null)
                    {
                        foreach (PerkViewModel item in svm.PerkViewModel)
                        {

                            if (item.JobOrderPerkId > 0)
                            {
                                JobOrderPerk perk = (from p in context.JobOrderPerk
                                                     where p.JobOrderPerkId == item.JobOrderPerkId
                                                     select p).FirstOrDefault();
                                perk.Worth = item.Worth;
                                perk.Base = item.Base;
                                perk.ModifiedBy = User.Identity.Name;
                                perk.ModifiedDate = DateTime.Now;
                                perk.ObjectState = Model.ObjectState.Modified;
                                context.JobOrderPerk.Add(perk);
                            }
                            else
                            {
                                JobOrderPerk perkC = Mapper.Map<PerkViewModel, JobOrderPerk>(item);
                                perkC.CreatedBy = User.Identity.Name;
                                perkC.CreatedDate = DateTime.Now;
                                perkC.ModifiedBy = User.Identity.Name;
                                perkC.ModifiedDate = DateTime.Now;
                                perkC.JobOrderHeaderId = temp.JobOrderHeaderId;
                                perkC.ObjectState = Model.ObjectState.Added;
                                context.JobOrderPerk.Add(perkC);
                            }
                        }
                    }


                    if (temp.StockHeaderId != null)
                    {
                        StockHeader S = (from SH in context.StockHeader
                                         where SH.StockHeaderId == temp.StockHeaderId
                                         select SH).FirstOrDefault();




                        //Updating docdate in stock and stockprocess
                        #region Updating DocDAte in Stock & StockProcess
                        if (S.DocDate != temp.DocDate)
                        {
                            List<Stock> StockLines = (from p in context.Stock
                                                      where p.StockHeaderId == S.StockHeaderId
                                                      select p).ToList();

                            foreach (var item in StockLines)
                            {
                                item.DocDate = temp.DocDate;
                                item.ObjectState = Model.ObjectState.Modified;
                                context.Stock.Add(item);
                            }

                            List<StockProcess> StockProcLines = (from p in context.StockProcess
                                                                 where p.StockHeaderId == temp.StockHeaderId
                                                                 select p).ToList();

                            foreach (var item in StockProcLines)
                            {
                                item.DocDate = temp.DocDate;
                                item.ObjectState = Model.ObjectState.Modified;
                                context.StockProcess.Add(item);
                            }

                        }
                        #endregion

                        StockHeaderTransport ST = (from SH in context.StockHeaderTransport
                                                  where SH.StockHeaderId == temp.StockHeaderId
                                         select SH).FirstOrDefault();

                        if (ST != null)
                        {
                            ST.DocDate = temp.DocDate;
                            ST.DocNo = temp.DocNo;
                            ST.PersonId = temp.JobWorkerId;
                            ST.GodownId = temp.GodownId;
                            ST.Remark = temp.Remark;
                            ST.Status = temp.Status;
                            ST.TransportId = svm.TransportId;
                            ST.VehicleNo = svm.VehicleNo;
                            ST.LrNo = svm.LrNo;
                            ST.EWayBillNo = svm.EWayBillNo;
                            ST.EWayBillDate = svm.EWayBillDate;
                            ST.LrDate = svm.LrDate;
                            ST.PaymentType = svm.PaymentType;
                            ST.Destination = svm.Destination;
                            ST.ModifiedBy = temp.ModifiedBy;
                            ST.ModifiedDate = temp.ModifiedDate;
                            ST.ObjectState = Model.ObjectState.Modified;
                            context.StockHeader.Add(ST);
                        }
                        else
                        {
                            S.DocDate = temp.DocDate;
                            S.DocNo = temp.DocNo;
                            S.PersonId = temp.JobWorkerId;
                            S.GodownId = temp.GodownId;
                            S.Remark = temp.Remark;
                            S.Status = temp.Status;
                            S.ModifiedBy = temp.ModifiedBy;
                            S.ModifiedDate = temp.ModifiedDate;
                            S.ObjectState = Model.ObjectState.Modified;
                            context.StockHeader.Add(S);
                        }                   
                    }

                    if (GodownChanged)
                    {
                        new StockService(_unitOfWork).UpdateStockGodownId(temp.StockHeaderId, temp.GodownId, temp.DocDate, context);
                    }
                        

                    if (JobWorkerChanged)
                    {
                        var JobOrderLineList = (from L in context.JobOrderLine where L.JobOrderHeaderId == temp.JobOrderHeaderId select L).ToList();
                        foreach(var JobOrderLine in JobOrderLineList)
                        {
                            if (JobOrderLine.ProductUidHeaderId != null)
                            {
                                ProductUidHeader ProductUidHeader = context.ProductUidHeader.Find(JobOrderLine.ProductUidHeaderId);
                                ProductUidHeader.GenPersonId = temp.JobWorkerId;
                                ProductUidHeader.ObjectState = Model.ObjectState.Modified;
                                context.ProductUidHeader.Add(ProductUidHeader);

                                var ProductUidList = (from L in context.ProductUid where L.ProductUidHeaderId == JobOrderLine.ProductUidHeaderId select L).ToList();
                                foreach(var ProductUid in ProductUidList)
                                {
                                    ProductUid.GenPersonId = temp.JobWorkerId;
                                    if (ProductUid.LastTransactionDocId == ProductUid.GenDocId && ProductUid.LastTransactionDocTypeId == ProductUid.GenDocTypeId)
                                    {
                                        ProductUid.LastTransactionPersonId = temp.JobWorkerId;
                                    }
                                    ProductUid.ObjectState = Model.ObjectState.Modified;
                                    context.ProductUid.Add(ProductUid);
                                }
                            }
                        }
                    }

                    ProdOrderHeader POH = context.ProdOrderHeader.Where(m => m.ReferenceDocId == temp.JobOrderHeaderId && m.ReferenceDocTypeId == temp.DocTypeId).FirstOrDefault();
                    if (POH != null)
                    {
                        POH.DocDate = temp.DocDate;
                        POH.DocNo = temp.DocNo;
                        POH.Remark = temp.Remark;
                        POH.Status = temp.Status;
                        POH.ModifiedBy = temp.ModifiedBy;
                        POH.ModifiedDate = temp.ModifiedDate;
                        POH.ObjectState = Model.ObjectState.Modified;
                        context.ProdOrderHeader.Add(POH);
                    }


                    if (svm.DocumentTypeHeaderAttributes != null)
                    {
                        foreach (var Attributes in svm.DocumentTypeHeaderAttributes)
                        {

                            JobOrderHeaderAttributes JobOrderHeaderAttribute = (from A in context.JobOrderHeaderAttributes
                                                                                      where A.HeaderTableId == s.JobOrderHeaderId 
                                                                                      && A.DocumentTypeHeaderAttributeId == Attributes.DocumentTypeHeaderAttributeId
                                                                                      select A).FirstOrDefault();

                            if (JobOrderHeaderAttribute != null)
                            {
                                JobOrderHeaderAttribute.Value = Attributes.Value;
                                JobOrderHeaderAttribute.ObjectState = Model.ObjectState.Modified;
                                context.JobOrderHeaderAttributes.Add(JobOrderHeaderAttribute);
                            }
                            else
                            {
                                JobOrderHeaderAttributes HeaderAttribute = new JobOrderHeaderAttributes()
                                {
                                    Value = Attributes.Value,
                                    HeaderTableId = s.JobOrderHeaderId,
                                    DocumentTypeHeaderAttributeId = Attributes.DocumentTypeHeaderAttributeId,
                                };
                                HeaderAttribute.ObjectState = Model.ObjectState.Added;
                                context.JobOrderHeaderAttributes.Add(HeaderAttribute);
                            }
                        }
                    }


                    LogList.Add(new LogTypeViewModel
                    {
                        ExObj = ExRec,
                        Obj = temp,
                    });



                    if (settings != null)
                    {
                        new CommonService().ExecuteCustomiseEvents(settings.Event_OnHeaderSave, new object[] { _unitOfWork, temp.JobOrderHeaderId, "E" });
                    }

                    XElement Modifications = new ModificationsCheckService().CheckChanges(LogList);

                    try
                    {
                        JobOrderDocEvents.onHeaderSaveEvent(this, new JobEventArgs(s.JobOrderHeaderId, EventModeConstants.Edit), ref context);
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
                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);

                        PrepareViewBag(svm.DocTypeId);
                        TempData["CSEXC"] += message;
                        ViewBag.id = svm.DocTypeId;
                        ViewBag.Mode = "Edit";
                        return View("Create", svm);
                    }

                    try
                    {
                        JobOrderDocEvents.afterHeaderSaveEvent(this, new JobEventArgs(s.JobOrderHeaderId, EventModeConstants.Edit), ref context);
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                    }

                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = temp.DocTypeId,
                        DocId = temp.JobOrderHeaderId,
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

            PrepareViewBag(svm.DocTypeId);
            ViewBag.Mode = "Add";
            return View("Create", svm);
        }


        [HttpGet]
        public ActionResult Modify(int id, string IndexType)
        {
            JobOrderHeader header = _JobOrderHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Drafted || header.Status == (int)StatusConstants.Import)
                return Edit(id, IndexType);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult ModifyAfter_Submit(int id, string IndexType)
        {
            JobOrderHeader header = _JobOrderHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Submitted || header.Status == (int)StatusConstants.Modified)
                return Edit(id, IndexType);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult ModifyAfter_Approve(int id, string IndexType)
        {
            JobOrderHeader header = _JobOrderHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Approved)
                return Edit(id, IndexType);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            JobOrderHeader header = _JobOrderHeaderService.Find(id);


            if (header.Status == (int)StatusConstants.Drafted || header.Status == (int)StatusConstants.Import)
                return Remove(id);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult DeleteAfter_Submit(int id)
        {
            JobOrderHeader header = _JobOrderHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Submitted || header.Status == (int)StatusConstants.Modified)
                return Remove(id);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult DeleteAfter_Approve(int id)
        {
            JobOrderHeader header = _JobOrderHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Approved)
                return Remove(id);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult DetailInformation(int id, int? DocLineId, string IndexType)
        {
            return RedirectToAction("Detail", new { id = id, transactionType = "detail", DocLineId = DocLineId, IndexType = IndexType });
        }



        // GET: /JobOrderHeader/Edit/5
        private ActionResult Edit(int id, string IndexType)
        {


            ViewBag.IndexStatus = IndexType;
            JobOrderHeaderViewModel s = _JobOrderHeaderService.GetJobOrderHeader(id);

            //List<StockProcess> SL = context.StockProcess.Where(m => m.CostCenterId == s.CostCenterId).ToList();

            //if (SL.Count > 0)
            //{
            //    s.LockReason = "Material Issued !";
            //}

            if (s.Status != (int)StatusConstants.Drafted)
                if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, s.DocTypeId, s.ProcessId, this.ControllerContext.RouteData.Values["controller"].ToString(),"Edit") == false)
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
            var settings = new JobOrderSettingsService(_unitOfWork).GetJobOrderSettingsForDocument(s.DocTypeId, s.DivisionId, s.SiteId);

            if (settings == null && UserRoles.Contains("SysAdmin"))
            {
                return RedirectToAction("Create", "JobOrderSettings", new { id = s.DocTypeId }).Warning("Please create job order settings");
            }
            else if (settings == null && !UserRoles.Contains("SysAdmin"))
            {
                return View("~/Views/Shared/InValidSettings.cshtml");
            }

            s.JobOrderSettings = Mapper.Map<JobOrderSettings, JobOrderSettingsViewModel>(settings);
            s.DocumentTypeSettings = new DocumentTypeSettingsService(_unitOfWork).GetDocumentTypeSettingsForDocument(s.DocTypeId);

            List<DocumentTypeHeaderAttributeViewModel> tem = _JobOrderHeaderService.GetDocumentHeaderAttribute(id).ToList();
            s.DocumentTypeHeaderAttributes = tem;



            ////Perks
            s.PerkViewModel = new PerkService(_unitOfWork).GetPerkListForDocumentTypeForEdit(id).ToList();

            if (s.PerkViewModel.Count == 0)
            {
                List<PerkViewModel> Perks = new List<PerkViewModel>();
                if (s.JobOrderSettings.Perks != null)
                    foreach (var item in s.JobOrderSettings.Perks.Split(',').ToList())
                    {
                        PerkViewModel temp = Mapper.Map<Perk, PerkViewModel>(new PerkService(_unitOfWork).Find(Convert.ToInt32(item)));

                        var DocTypePerk = (from p2 in context.PerkDocumentType
                                           where p2.DocTypeId == s.DocTypeId && p2.PerkId == temp.PerkId && p2.SiteId == s.SiteId && p2.DivisionId == s.DivisionId
                                           select p2).FirstOrDefault();
                        if (DocTypePerk != null)
                        {
                            temp.Base = DocTypePerk.Base;
                            temp.Worth = DocTypePerk.Worth;
                            temp.CostConversionMultiplier = DocTypePerk.CostConversionMultiplier;
                            temp.IsEditableRate = DocTypePerk.IsEditableRate;
                        }
                        else
                        {
                            temp.Base = 0;
                            temp.Worth = 0;
                            temp.CostConversionMultiplier = 0;
                            temp.IsEditableRate = true;
                        }

                        Perks.Add(temp);
                    }
                s.PerkViewModel = Perks;
            }
            PrepareViewBag(s.DocTypeId);
            if (s == null)
            {
                return HttpNotFound();
            }

            s.CalculationFooterChargeCount = new JobOrderHeaderChargeService(_unitOfWork).GetCalculationFooterList(id).Count();

            //ViewBag.transactionType = "detail";

            ViewBag.Mode = "Edit";
            ViewBag.transactionType = "";

            ViewBag.Name = new DocumentTypeService(_unitOfWork).Find(s.DocTypeId).DocumentTypeName;
            ViewBag.id = s.DocTypeId;

            if (!(System.Web.HttpContext.Current.Request.UrlReferrer.PathAndQuery).Contains("Create"))
                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = s.DocTypeId,
                    DocId = s.JobOrderHeaderId,
                    ActivityType = (int)ActivityTypeContants.Detail,
                    DocNo = s.DocNo,
                    DocDate = s.DocDate,
                    DocStatus = s.Status,
                }));

            return View("Create", s);
        }

        private ActionResult Remove(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JobOrderHeader JobOrderHeader = _JobOrderHeaderService.Find(id);


            if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, JobOrderHeader.DocTypeId, JobOrderHeader.ProcessId, this.ControllerContext.RouteData.Values["controller"].ToString(), "Remove") == false)
            {
                return PartialView("~/Views/Shared/PermissionDenied_Modal.cshtml").Warning("You don't have permission to do this task.");
            }

            if (JobOrderHeader == null)
            {
                return HttpNotFound();
            }

            #region DocTypeTimeLineValidation

            try
            {
                TimePlanValidation = DocumentValidation.ValidateDocument(Mapper.Map<DocumentUniqueId>(JobOrderHeader), DocumentTimePlanTypeConstants.Delete, User.Identity.Name, out ExceptionMsg, out Continue);
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



        [Authorize]
        public ActionResult Detail(int id, string IndexType, string transactionType, int? DocLineId)
        {
            if (DocLineId.HasValue)
                ViewBag.DocLineId = DocLineId;
            //Saving ViewBag Data::

            ViewBag.transactionType = transactionType;
            ViewBag.IndexStatus = IndexType;

            JobOrderHeaderViewModel s = _JobOrderHeaderService.GetJobOrderHeader(id);
            //Getting Settings
            var settings = new JobOrderSettingsService(_unitOfWork).GetJobOrderSettingsForDocument(s.DocTypeId, s.DivisionId, s.SiteId);

            s.JobOrderSettings = Mapper.Map<JobOrderSettings, JobOrderSettingsViewModel>(settings);

            s.DocumentTypeSettings = new DocumentTypeSettingsService(_unitOfWork).GetDocumentTypeSettingsForDocument(s.DocTypeId);


            ////Perks
            s.PerkViewModel = new PerkService(_unitOfWork).GetPerkListForDocumentTypeForEdit(id).ToList();

            PrepareViewBag(s.DocTypeId);
            if (s == null)
            {
                return HttpNotFound();
            }

            if (String.IsNullOrEmpty(transactionType) || transactionType == "detail")
                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = s.DocTypeId,
                    DocId = s.JobOrderHeaderId,
                    ActivityType = (int)ActivityTypeContants.Detail,
                    DocNo = s.DocNo,
                    DocDate = s.DocDate,
                    DocStatus = s.Status,
                }));


            return View("Create", s);
        }



        // POST: /JobOrderHeader/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(ReasonViewModel vm)
        {
            List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();
            bool BeforeSave = true;

            try
            {
                BeforeSave = JobOrderDocEvents.beforeHeaderDeleteEvent(this, new JobEventArgs(vm.id), ref context);
            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXC"] += message;
                EventException = true;
            }

            if (!BeforeSave)
                TempData["CSEXC"] += "Failed validation before delete";

            var JobOrderHeader = (from p in context.JobOrderHeader
                                  where p.JobOrderHeaderId == vm.id
                                  select p).FirstOrDefault();

            var GatePassHEader = (from p in context.GatePassHeader
                                  where p.GatePassHeaderId == JobOrderHeader.GatePassHeaderId
                                  select p).FirstOrDefault();

            if (GatePassHEader != null && GatePassHEader.Status == (int)StatusConstants.Submitted)
            {
                BeforeSave = false;
                TempData["CSEXC"] += "Cannot delete record because gatepass is submitted.";
            }

            if (ModelState.IsValid && BeforeSave && !EventException)
            {

                int? StockHeaderId = 0;


                //first find the Purchase Order Object based on the ID. (sience this object need to marked to be deleted IE. ObjectState.Deleted)


                LogList.Add(new LogTypeViewModel
                {
                    ExObj = Mapper.Map<JobOrderHeader>(JobOrderHeader),
                });

                StockHeaderId = JobOrderHeader.StockHeaderId;

                //Then find all the Purchase Order Header Line associated with the above ProductType.
                //var JobOrderLine = new JobOrderLineService(_unitOfWork).GetJobOrderLineforDelete(vm.id);
                var JobOrderLine = (from p in context.JobOrderLine
                                    where p.JobOrderHeaderId == vm.id
                                    select p).ToList();

                var JOLineIds = JobOrderLine.Select(m => m.JobOrderLineId).ToArray();

                var JobOrderLineStatusRecords = (from p in context.JobOrderLineStatus
                                                 where JOLineIds.Contains(p.JobOrderLineId ?? 0)
                                                 select p).ToList();

                var JobOrderLineExtendedRecords = (from p in context.JobOrderLineExtended
                                                   where JOLineIds.Contains(p.JobOrderLineId ?? 0)
                                                   select p).ToList();

                var LineChargeRecords = (from p in context.JobOrderLineCharge
                                         where JOLineIds.Contains(p.LineTableId)
                                         select p).ToList();

                var HeaderChargeRecords = (from p in context.JobOrderHeaderCharges
                                           where p.HeaderTableId == vm.id
                                           select p).ToList();

                var CreatedBarCodeRecords = (from p in context.ProductUid
                                             where JOLineIds.Contains(p.GenLineId ?? 0) && p.GenDocTypeId == JobOrderHeader.DocTypeId
                                             select p).ToList();

                var ProductUids = JobOrderLine.Select(m => m.ProductUidId).ToArray();

                var BarCodeRecords = (from p in context.ProductUid
                                      where ProductUids.Contains(p.ProductUIDId)
                                      select p).ToList();

                var JobOrderBomRecords = (from p in context.JobOrderBom
                                          where JOLineIds.Contains(p.JobOrderLineId ?? 0)
                                          select p).ToList();

                var JobOrderHeaderBomRecords = (from p in context.JobOrderBom
                                                where p.JobOrderHeaderId == vm.id && p.JobOrderLineId == null
                                                select p).ToList();

                var JobOrderPerks = (from p in context.JobOrderPerk
                                     where p.JobOrderHeaderId == vm.id
                                     select p).ToList();

                try
                {
                    JobOrderDocEvents.onHeaderDeleteEvent(this, new JobEventArgs(vm.id), ref context);
                }
                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    TempData["CSEXC"] += message;
                    EventException = true;
                }


                List<int> StockIdList = new List<int>();
                List<int> StockProcessIdList = new List<int>();

                var attributes = (from A in context.JobOrderHeaderAttributes where A.HeaderTableId == vm.id select A).ToList();

                foreach (var ite2 in attributes)
                {
                    ite2.ObjectState = Model.ObjectState.Deleted;
                    context.JobOrderHeaderAttributes.Remove(ite2);
                }

                new ProdOrderLineStatusService(_unitOfWork).DeleteProdQtyOnJobOrderMultiple(JobOrderHeader.JobOrderHeaderId, ref context, true);

                foreach (var item in LineChargeRecords)
                {
                    item.ObjectState = Model.ObjectState.Deleted;
                    context.JobOrderLineCharge.Remove(item);
                }

                foreach (var item in JobOrderLineStatusRecords)
                {
                    item.ObjectState = Model.ObjectState.Deleted;
                    context.JobOrderLineStatus.Remove(item);
                }

                //Getting Settings
                var settings = new JobOrderSettingsService(_unitOfWork).GetJobOrderSettingsForDocument(JobOrderHeader.DocTypeId, JobOrderHeader.DivisionId, JobOrderHeader.SiteId);

                if (settings.isAllowDirectReceive == true)
                    {
                    var JobReceiveLineRecords = (from p in context.JobReceiveLine
                                                 where JOLineIds.Contains(p.JobOrderLineId ?? 0)
                                                 select p).ToList();

                    var JobReceiveLineStatusRecords = (from p in context.JobReceiveLineStatus
                                                       join t in context.JobReceiveLine on p.JobReceiveLineId equals t.JobReceiveLineId
                                                       where JOLineIds.Contains(t.JobOrderLineId ?? 0)
                                                 select p).ToList();

                    foreach (var item in JobReceiveLineStatusRecords)
                    {
                        item.ObjectState = Model.ObjectState.Deleted;
                        context.JobReceiveLineStatus.Remove(item);
                    }
                    
                    foreach (var item in JobReceiveLineRecords)
                    {
                        item.ObjectState = Model.ObjectState.Deleted;
                        context.JobReceiveLine.Remove(item);
                    }

                }

                foreach (var item in JobOrderLineExtendedRecords)
                {
                    item.ObjectState = Model.ObjectState.Deleted;
                    context.JobOrderLineExtended.Remove(item);
                }


                //Mark ObjectState.Delete to all the Purchase Order Lines. 
                foreach (var item in JobOrderLine)
                {

                    LogList.Add(new LogTypeViewModel
                    {
                        ExObj = Mapper.Map<JobOrderLine>(item),
                    });


                    if (item.StockId != null)
                    {
                        StockAdj Adj = (from L in context.StockAdj
                                        where L.StockOutId == item.StockId
                                        select L).FirstOrDefault();

                        if (Adj != null)
                        {
                            Adj.ObjectState = Model.ObjectState.Deleted;
                            context.StockAdj.Remove(Adj);
                        }

                        StockIdList.Add((int)item.StockId);
                    }

                    if (item.StockProcessId != null)
                    {
                        StockProcessIdList.Add((int)item.StockProcessId);
                    }

                    //var linecharges = new JobOrderLineChargeService(_unitOfWork).GetCalculationProductList(item.JobOrderLineId);
                    //foreach (var citem in linecharges)
                    //    new JobOrderLineChargeService(_unitOfWork).Delete(citem.Id);



                    if ((item.ProductUidHeaderId.HasValue) && (item.ProductUidHeaderId.Value > 0))
                    {

                        var ProductUid = CreatedBarCodeRecords.Where(m => m.GenLineId == item.JobOrderLineId);

                        foreach (var item2 in ProductUid)
                        {
                            if (item2.LastTransactionDocId == null || (item2.LastTransactionDocId == item2.GenDocId && item2.LastTransactionDocTypeId == item2.GenDocTypeId))
                            //new ProductUidService(_unitOfWork).Delete(item2);
                            {
                                item2.ObjectState = Model.ObjectState.Deleted;
                                context.ProductUid.Remove(item2);
                            }
                            else
                            {
                                throw new Exception("Record Cannot be deleted as its Unique Id's are in use by other documents");
                            }
                        }
                    }
                    else
                    {

                        if (item.ProductUidId.HasValue)
                        {
                            Service.ProductUidDetail ProductUidDetail = new ProductUidService(_unitOfWork).FGetProductUidLastValues(item.ProductUidId.Value, "Weaving Order-" + item.JobOrderHeaderId.ToString());

                            // ProductUid ProductUid = new ProductUidService(_unitOfWork).Find(item.ProductUidId.Value);

                            ProductUid ProductUid = BarCodeRecords.Where(m => m.ProductUIDId == item.ProductUidId).FirstOrDefault();

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


                    }

                    var Boms = JobOrderBomRecords.Where(m => m.JobOrderLineId == item.JobOrderLineId);

                    foreach (var item2 in Boms)
                    {
                        item2.ObjectState = Model.ObjectState.Deleted;
                        context.JobOrderBom.Remove(item2);
                    }


                    item.ObjectState = Model.ObjectState.Deleted;
                    context.JobOrderLine.Remove(item);


                }


                new StockService(_unitOfWork).DeleteStockDBMultiple(StockIdList, ref context, true);

                new StockProcessService(_unitOfWork).DeleteStockProcessDBMultiple(StockProcessIdList, ref context, true);


                foreach (var item in HeaderChargeRecords)
                {
                    item.ObjectState = Model.ObjectState.Deleted;
                    context.JobOrderHeaderCharges.Remove(item);
                }


                foreach (var item in JobOrderPerks)
                {
                    item.ObjectState = Model.ObjectState.Deleted;
                    context.JobOrderPerk.Remove(item);
                }


                if (JobOrderHeader.GatePassHeaderId.HasValue)
                {

                    var GatePassLines = (from p in context.GatePassLine
                                         where p.GatePassHeaderId == GatePassHEader.GatePassHeaderId
                                         select p).ToList();


                    foreach (var item in GatePassLines)
                    {
                        item.ObjectState = Model.ObjectState.Deleted;
                        context.GatePassLine.Remove(item);
                    }

                    GatePassHEader.ObjectState = Model.ObjectState.Deleted;
                    context.GatePassHeader.Remove(GatePassHEader);

                }


                foreach (var Hbom in JobOrderHeaderBomRecords)
                {
                    Hbom.ObjectState = Model.ObjectState.Deleted;
                    context.JobOrderBom.Remove(Hbom);
                }

                var JobORderHEaderStatus = (from p in context.JobOrderHeaderStatus
                                            where p.JobOrderHeaderId == vm.id
                                            select p).FirstOrDefault();

                JobORderHEaderStatus.ObjectState = Model.ObjectState.Deleted;
                context.JobOrderHeaderStatus.Remove(JobORderHEaderStatus);

                var JobReceiveHeader = (from p in context.JobReceiveHeader
                                        where p.ReferenceDocId == vm.id && p.ReferenceDocTypeId == JobOrderHeader.DocTypeId
                                            select p).FirstOrDefault();

                if(JobReceiveHeader!=null)
                { 
                JobReceiveHeader.ObjectState = Model.ObjectState.Deleted;
                context.JobReceiveHeader.Remove(JobReceiveHeader);
                 }

                // Now delete the Purhcase Order Header
                //_JobOrderHeaderService.Delete(JobOrderHeader);

                int ReferenceDocId = JobOrderHeader.JobOrderHeaderId;
                int ReferenceDocTypeId = JobOrderHeader.DocTypeId;


                JobOrderHeader.ObjectState = Model.ObjectState.Deleted;
                context.JobOrderHeader.Remove(JobOrderHeader);


                //ForDeleting Generated CostCenter:::

                var GeneratedCostCenter = (from p in context.CostCenter
                                           where p.ReferenceDocId == ReferenceDocId && p.ReferenceDocTypeId == ReferenceDocTypeId
                                           select p).FirstOrDefault();

                if (GeneratedCostCenter != null)
                {
                    var CostCentrerStatusRecord = (from p in context.CostCenterStatus
                                                   where p.CostCenterId == GeneratedCostCenter.CostCenterId
                                                   select p).FirstOrDefault();

                    if (CostCentrerStatusRecord != null)
                    {
                        CostCentrerStatusRecord.ObjectState = Model.ObjectState.Deleted;
                        context.CostCenterStatus.Remove(CostCentrerStatusRecord);
                    }

                    var CostCenterStatusExtendedRecord = (from p in context.CostCenterStatusExtended
                                                   where p.CostCenterId == GeneratedCostCenter.CostCenterId
                                                   select p).FirstOrDefault();

                    if (CostCenterStatusExtendedRecord != null)
                    {
                        CostCenterStatusExtendedRecord.ObjectState = Model.ObjectState.Deleted;
                        context.CostCenterStatusExtended.Remove(CostCenterStatusExtendedRecord);
                    }

                    GeneratedCostCenter.ObjectState = Model.ObjectState.Deleted;
                    context.CostCenter.Remove(GeneratedCostCenter);
                }

                if (StockHeaderId != null)
                {
                    
                    var StockLine = (from p in context.StockLine
                                     where p.StockHeaderId == StockHeaderId
                                       select p).ToList();
                    if(StockLine != null)
                    { 
                        foreach (var SItem in StockLine)
                        {
                            SItem.ObjectState = Model.ObjectState.Deleted;
                            context.StockLine.Remove(SItem);
                        }
                    }

                    var Stocks = (from p in context.Stock
                                     where p.StockHeaderId == StockHeaderId
                                     select p).ToList();

                    if (Stocks != null)
                    {
                        foreach (var SItem1 in Stocks)
                        {
                            SItem1.ObjectState = Model.ObjectState.Deleted;
                            context.Stock.Remove(SItem1);
                        }
                    }

                    var StockPs = (from p in context.StockProcess
                                  where p.StockHeaderId == StockHeaderId
                                  select p).ToList();

                    if (StockPs != null)
                    {
                        foreach (var SItem2 in StockPs)
                        {
                            SItem2.ObjectState = Model.ObjectState.Deleted;
                            context.StockProcess.Remove(SItem2);
                        }
                    }


                    var StockHeader = (from p in context.StockHeader
                                       where p.StockHeaderId == StockHeaderId
                                       select p).FirstOrDefault();
                    StockHeader.ObjectState = Model.ObjectState.Deleted;
                    context.StockHeader.Remove(StockHeader);
                }


                if (settings != null)
                {
                    new CommonService().ExecuteCustomiseEvents(settings.Event_OnHeaderDelete, new object[] { _unitOfWork, JobOrderHeader.JobOrderHeaderId });
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
                    JobOrderDocEvents.afterHeaderDeleteEvent(this, new JobEventArgs(vm.id), ref context);
                }
                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    TempData["CSEXC"] += message;
                }

                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = JobOrderHeader.DocTypeId,
                    DocId = JobOrderHeader.JobOrderHeaderId,
                    ActivityType = (int)ActivityTypeContants.Deleted,
                    UserRemark = vm.Reason,
                    DocNo = JobOrderHeader.DocNo,
                    xEModifications = Modifications,
                    DocDate = JobOrderHeader.DocDate,
                    DocStatus = JobOrderHeader.Status,
                }));

                return Json(new { success = true });
            }
            return PartialView("_Reason", vm);
        }


        public ActionResult Submit(int id, string IndexType, string TransactionType)
        {
            JobOrderHeader s = context.JobOrderHeader.Find(id);
            if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, s.DocTypeId, s.ProcessId, this.ControllerContext.RouteData.Values["controller"].ToString(),"Submit") == false)
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
                BeforeSave = JobOrderDocEvents.beforeHeaderSubmitEvent(this, new JobEventArgs(Id), ref context);
            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXC"] += message;
                EventException = true;
            }

            if (!BeforeSave)
                TempData["CSEXC"] += "Falied validation before submit.";

            JobOrderHeader pd = context.JobOrderHeader.Find(Id);


            if (ModelState.IsValid && BeforeSave && !EventException)
            {
                int Cnt = 0;
                int CountUid = 0;
                //JobOrderHeader pd = new JobOrderHeaderService(_unitOfWork).Find(Id);              

                int ActivityType;
                if (User.Identity.Name == pd.ModifiedBy || UserRoles.Contains("Admin"))
                {

                    pd.Status = (int)StatusConstants.Submitted;
                    ActivityType = (int)ActivityTypeContants.Submitted;

                    JobOrderSettings Settings = new JobOrderSettingsService(_unitOfWork).GetJobOrderSettingsForDocument(pd.DocTypeId, pd.DivisionId, pd.SiteId);


                    if (!string.IsNullOrEmpty(GenGatePass) && GenGatePass == "true")
                    {

                        if (!String.IsNullOrEmpty(Settings.SqlProcGatePass))
                        {

                            SqlParameter SqlParameterUserId = new SqlParameter("@Id", Id);
                            IEnumerable<GatePassGeneratedViewModel> GatePasses = context.Database.SqlQuery<GatePassGeneratedViewModel>(Settings.SqlProcGatePass + " @Id", SqlParameterUserId).ToList();

                            if (pd.GatePassHeaderId == null)
                            {
                                SqlParameter DocDate = new SqlParameter("@DocDate", DateTime.Now.Date);
                                DocDate.SqlDbType = SqlDbType.DateTime;
                                SqlParameter Godown = new SqlParameter("@GodownId", pd.GodownId);
                                SqlParameter DocType = new SqlParameter("@DocTypeId", new DocumentTypeService(_unitOfWork).Find(TransactionDoctypeConstants.GatePass).DocumentTypeId);
                                GatePassHeader GPHeader = new GatePassHeader();
                                GPHeader.CreatedBy = User.Identity.Name;
                                GPHeader.CreatedDate = DateTime.Now;
                                GPHeader.DivisionId = pd.DivisionId;
                                GPHeader.DocDate = DateTime.Now.Date;
                                GPHeader.DocNo = context.Database.SqlQuery<string>("Web.GetNewDocNoGatePass @DocTypeId, @DocDate, @GodownId ", DocType, DocDate, Godown).FirstOrDefault();
                                GPHeader.DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.GatePass).DocumentTypeId;
                                GPHeader.ModifiedBy = User.Identity.Name;
                                GPHeader.ModifiedDate = DateTime.Now;
                                GPHeader.Remark = pd.Remark;
                                GPHeader.PersonId = pd.JobWorkerId;
                                GPHeader.SiteId = pd.SiteId;
                                GPHeader.GodownId = pd.GodownId ?? 0;

                                GPHeader.ReferenceDocTypeId = pd.DocTypeId;
                                GPHeader.ReferenceDocId = pd.JobOrderHeaderId;
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

                                    // new GatePassLineService(_unitOfWork).Create(Gline);
                                    Gline.ObjectState = Model.ObjectState.Added;
                                    context.GatePassLine.Add(Gline);
                                }

                                pd.GatePassHeaderId = GPHeader.GatePassHeaderId;

                            }
                            else
                            {
                                //List<GatePassLine> LineList = new GatePassLineService(_unitOfWork).GetGatePassLineList(pd.GatePassHeaderId ?? 0).ToList();

                                List<GatePassLine> LineList = (from p in context.GatePassLine
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

                                    //new GatePassLineService(_unitOfWork).Create(Gline);
                                    Gline.ObjectState = Model.ObjectState.Added;
                                    context.GatePassLine.Add(Gline);
                                }

                                pd.GatePassHeaderId = GPHeader.GatePassHeaderId;

                            }




                        }

                    }


                    List<string> uids = new List<string>();

                    if (!string.IsNullOrEmpty(Settings.SqlProcGenProductUID))
                    {

                        var lines = (from p in context.JobOrderLine
                                     where p.JobOrderHeaderId == pd.JobOrderHeaderId
                                     && p.UnitConversionMultiplier > (decimal)0.0278
                                     select p).ToList();


                        decimal Qty = lines.Where(m => m.ProductUidHeaderId == null).Sum(m => m.Qty);


                        using (SqlConnection sqlConnection = new SqlConnection((string)System.Web.HttpContext.Current.Session["DefaultConnectionString"]))
                        {
                            sqlConnection.Open();

                            int TypeId = pd.DocTypeId;

                            SqlCommand Totalf = new SqlCommand("SELECT * FROM " + Settings.SqlProcGenProductUID + "( " + TypeId + ", " + Qty + ")", sqlConnection);

                            SqlDataReader ExcessStockQty = (Totalf.ExecuteReader());
                            while (ExcessStockQty.Read())
                            {
                                uids.Add((string)ExcessStockQty.GetValue(0));
                            }
                        }

                        //uids = new JobOrderLineService(_unitOfWork).GetProcGenProductUids(pd.DocTypeId, Qty, pd.DivisionId, pd.SiteId);

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
                                ProdUidHeader.GenDocId = pd.JobOrderHeaderId;
                                ProdUidHeader.GenDocNo = pd.DocNo;
                                ProdUidHeader.GenDocTypeId = pd.DocTypeId;
                                ProdUidHeader.GenDocDate = pd.DocDate;
                                ProdUidHeader.GenPersonId = pd.JobWorkerId;
                                ProdUidHeader.CreatedBy = User.Identity.Name;
                                ProdUidHeader.CreatedDate = DateTime.Now;
                                ProdUidHeader.ModifiedBy = User.Identity.Name;
                                ProdUidHeader.ModifiedDate = DateTime.Now;
                                ProdUidHeader.ObjectState = Model.ObjectState.Added;
                                context.ProductUidHeader.Add(ProdUidHeader);

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
                                    ProdUid.GenLineId = item.JobOrderLineId;
                                    ProdUid.GenDocId = pd.JobOrderHeaderId;
                                    ProdUid.GenDocNo = pd.DocNo;
                                    ProdUid.GenDocTypeId = pd.DocTypeId;
                                    ProdUid.GenDocDate = pd.DocDate;
                                    ProdUid.GenPersonId = pd.JobWorkerId;
                                    ProdUid.Dimension1Id = item.Dimension1Id;
                                    ProdUid.Dimension2Id = item.Dimension2Id;
                                    ProdUid.Dimension3Id = item.Dimension3Id;
                                    ProdUid.Dimension4Id = item.Dimension4Id;
                                    ProdUid.CurrenctProcessId = pd.ProcessId;
                                    ProdUid.Status = (!string.IsNullOrEmpty(Settings.BarcodeStatusUpdate) ? Settings.BarcodeStatusUpdate : ProductUidStatusConstants.Issue);
                                    ProdUid.LastTransactionDocId = pd.JobOrderHeaderId;
                                    ProdUid.LastTransactionDocNo = pd.DocNo;
                                    ProdUid.LastTransactionDocTypeId = pd.DocTypeId;
                                    ProdUid.LastTransactionDocDate = pd.DocDate;
                                    ProdUid.LastTransactionPersonId = pd.JobWorkerId;
                                    ProdUid.LastTransactionLineId = item.JobOrderLineId;
                                    ProdUid.ProductUIDId = count;
                                    ProdUid.ObjectState = Model.ObjectState.Added;
                                    context.ProductUid.Add(ProdUid);

                                    count++;
                                    CountUid++;
                                }
                                Cnt++;
                                item.ObjectState = Model.ObjectState.Modified;
                                context.JobOrderLine.Add(item);
                            }
                        }
                    }



                    //_JobOrderHeaderService.Update(pd);
                    pd.ReviewBy = null;
                    pd.ObjectState = Model.ObjectState.Modified;
                    context.JobOrderHeader.Add(pd);

                    if (Settings != null)
                    {
                        new CommonService().ExecuteCustomiseEvents(Settings.Event_OnHeaderSubmit, new object[] { _unitOfWork, pd.JobOrderHeaderId });
                    }

                    try
                    {
                        JobOrderDocEvents.onHeaderSubmitEvent(this, new JobEventArgs(Id), ref context);
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
                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                        return RedirectToAction("Index", new { id = pd.DocTypeId });
                    }



                    if (Settings != null)
                    {
                        new CommonService().ExecuteCustomiseEvents(Settings.Event_AfterHeaderSubmit, new object[] { _unitOfWork, pd.JobOrderHeaderId });
                    }

                    try
                    {
                        JobOrderDocEvents.afterHeaderSubmitEvent(this, new JobEventArgs(Id), ref context);
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                    }


                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = pd.DocTypeId,
                        DocId = pd.JobOrderHeaderId,
                        ActivityType = ActivityType,
                        UserRemark = UserRemark,
                        DocNo = pd.DocNo,
                        DocDate = pd.DocDate,
                        DocStatus = pd.Status,
                    }));



                    string ReturnUrl = System.Configuration.ConfigurationManager.AppSettings["CurrentDomain"] + "/" + "JobOrderHeader" + "/" + "Index" + "/" + pd.DocTypeId + "?IndexType=" + IndexType;
                    if (!string.IsNullOrEmpty(IsContinue) && IsContinue == "True")
                    {

                        int nextId = new NextPrevIdService(_unitOfWork).GetNextPrevId(Id, pd.DocTypeId, User.Identity.Name, ForActionConstants.PendingToSubmit, "Web.JobOrderHeaders", "JobOrderHeaderId", PrevNextConstants.Next);

                        if (nextId == 0)
                        {
                            var PendingtoSubmitCount = _JobOrderHeaderService.GetJobOrderHeaderListPendingToSubmit(pd.DocTypeId, User.Identity.Name).Count();
                            if (PendingtoSubmitCount > 0)
                                ReturnUrl = System.Configuration.ConfigurationManager.AppSettings["CurrentDomain"] + "/" + "JobOrderHeader" + "/" + "Index_PendingToSubmit" + "/" + pd.DocTypeId + "?IndexType=" + IndexType;
                            else
                                ReturnUrl = System.Configuration.ConfigurationManager.AppSettings["CurrentDomain"] + "/" + "JobOrderHeader" + "/" + "Index" + "/" + pd.DocTypeId + "?IndexType=" + IndexType;
                        }
                        else
                            ReturnUrl = System.Configuration.ConfigurationManager.AppSettings["CurrentDomain"] + "/" + "JobOrderHeader" + "/" + "Submit" + "/" + nextId + "?TransactionType=submitContinue&IndexType=" + IndexType;
                    }

                    #region "For Calling Customise Menu"
                    int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
                    int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

                    var settings = new JobOrderSettingsService(_unitOfWork).GetJobOrderSettingsForDocument(pd.DocTypeId, DivisionId, SiteId);

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
                    return RedirectToAction("Index", new { id = pd.DocTypeId, IndexType = IndexType }).Warning("Record can be submitted by user " + pd.ModifiedBy + " only.");
            }

            return RedirectToAction("Index", new { id = pd.DocTypeId, IndexType = IndexType });
        }





        public ActionResult Review(int id, string IndexType, string TransactionType)
        {
            return RedirectToAction("Detail", new { id = id, IndexType = IndexType, transactionType = string.IsNullOrEmpty(TransactionType) ? "review" : TransactionType });
        }


        [HttpPost, ActionName("Detail")]
        [MultipleButton(Name = "Command", Argument = "Review")]
        public ActionResult Reviewed(int Id, string IndexType, string UserRemark, string IsContinue)
        {
            JobOrderHeader pd = context.JobOrderHeader.Find(Id);

            if (ModelState.IsValid)
            {

                pd.ReviewCount = (pd.ReviewCount ?? 0) + 1;
                pd.ReviewBy += User.Identity.Name + ", ";
                pd.ObjectState = Model.ObjectState.Modified;
                context.JobOrderHeader.Add(pd);

                try
                {
                    JobOrderDocEvents.onHeaderReviewEvent(this, new JobEventArgs(Id), ref context);
                }
                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    TempData["CSEXC"] += message;
                }

                context.SaveChanges();

                try
                {
                    JobOrderDocEvents.afterHeaderReviewEvent(this, new JobEventArgs(Id), ref context);
                }
                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    TempData["CSEXC"] += message;
                }

                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = pd.DocTypeId,
                    DocId = pd.JobOrderHeaderId,
                    ActivityType = (int)ActivityTypeContants.Reviewed,
                    UserRemark = UserRemark,
                    DocNo = pd.DocNo,
                    DocDate = pd.DocDate,
                    DocStatus = pd.Status,
                }));


                string ReturnUrl = System.Configuration.ConfigurationManager.AppSettings["CurrentDomain"] + "/" + "JobOrderHeader" + "/" + "Index" + "/" + pd.DocTypeId + "?IndexType=" + IndexType;
                if (!string.IsNullOrEmpty(IsContinue) && IsContinue == "True")
                {

                    int nextId = new NextPrevIdService(_unitOfWork).GetNextPrevId(Id, pd.DocTypeId, User.Identity.Name, ForActionConstants.PendingToReview, "Web.JobOrderHeaders", "JobOrderHeaderId", PrevNextConstants.Next);

                    if (nextId == 0)
                    {
                        var PendingtoSubmitCount = _JobOrderHeaderService.GetJobOrderHeaderListPendingToSubmit(pd.DocTypeId, User.Identity.Name).Count();
                        if (PendingtoSubmitCount > 0)
                            ReturnUrl = System.Configuration.ConfigurationManager.AppSettings["CurrentDomain"] + "/" + "JobOrderHeader" + "/" + "Index_PendingToReview" + "/" + pd.DocTypeId + "?IndexType=" + IndexType;
                        else
                            ReturnUrl = System.Configuration.ConfigurationManager.AppSettings["CurrentDomain"] + "/" + "JobOrderHeader" + "/" + "Index" + "/" + pd.DocTypeId + "?IndexType=" + IndexType;
                    }
                    else
                        ReturnUrl = System.Configuration.ConfigurationManager.AppSettings["CurrentDomain"] + "/" + "JobOrderHeader" + "/" + "Review" + "/" + nextId + "?TransactionType=ReviewContinue&IndexType=" + IndexType;
                }


                return Redirect(ReturnUrl);

            }

            return RedirectToAction("Index", new { id = pd.DocTypeId, IndexType = IndexType }).Warning("Error in Reviewing.");
        }

        [HttpGet]
        public ActionResult Report(int id)
        {
            DocumentType Dt = new DocumentType();
            Dt = new DocumentTypeService(_unitOfWork).Find(id);

            JobOrderSettings SEttings = new JobOrderSettingsService(_unitOfWork).GetJobOrderSettingsForDocument(Dt.DocumentTypeId, (int)System.Web.HttpContext.Current.Session["DivisionId"], (int)System.Web.HttpContext.Current.Session["SiteId"]);

            Dictionary<int, string> DefaultValue = new Dictionary<int, string>();

            //if (!Dt.ReportMenuId.HasValue)
            //    throw new Exception("Report Menu not configured in document types");
            if (!Dt.ReportMenuId.HasValue)
                return Redirect((string)System.Configuration.ConfigurationManager.AppSettings["JobsDomain"] + "/GridReport/GridReportLayout/?MenuName=Job Order Report&DocTypeId=" + id.ToString());


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


        public ActionResult Wizard(int id)//Document Type Id
        {
            //ControllerAction ca = new ControllerActionService(_unitOfWork).Find(id);
            JobOrderHeaderViewModel vm = new JobOrderHeaderViewModel();

            vm.DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            vm.SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            var settings = new JobOrderSettingsService(_unitOfWork).GetJobOrderSettingsForDocument(id, vm.DivisionId, vm.SiteId);

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
                            return Redirect(System.Configuration.ConfigurationManager.AppSettings[menuviewmodel.URL] + "/" + menuviewmodel.ControllerName + "/" + menuviewmodel.ActionName + "/" + id + "?MenuId=" + menuviewmodel.MenuId);
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
            JobOrderHeaderViewModel vm = new JobOrderHeaderViewModel();

            vm.DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            vm.SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            var settings = new JobOrderSettingsService(_unitOfWork).GetJobOrderSettingsForDocument(id, vm.DivisionId, vm.SiteId);

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
                    return Redirect(System.Configuration.ConfigurationManager.AppSettings[menuviewmodel.URL] + "/" + menuviewmodel.ControllerName + "/" + menuviewmodel.ActionName + "?Id=" + Id + "&ReturnUrl=" + ReturnUrl);
                }
                else
                {
                    return RedirectToAction(menuviewmodel.ActionName, menuviewmodel.ControllerName, new { id = Id, ReturnUrl = ReturnUrl });
                }
            }
            return null;
        }

        public int PendingToSubmitCount(int id)
        {
            return (_JobOrderHeaderService.GetJobOrderHeaderListPendingToSubmit(id, User.Identity.Name)).Count();
        }

        public int PendingToReviewCount(int id)
        {
            return (_JobOrderHeaderService.GetJobOrderHeaderListPendingToReview(id, User.Identity.Name)).Count();
        }

        [HttpGet]
        public ActionResult NextPage(int DocId, int DocTypeId)//CurrentHeaderId
        {
            var nextId = new NextPrevIdService(_unitOfWork).GetNextPrevId(DocId, DocTypeId, User.Identity.Name, "", "Web.JobOrderHeaders", "JobOrderHeaderId", PrevNextConstants.Next);
            return Edit(nextId, "");
        }
        [HttpGet]
        public ActionResult PrevPage(int DocId, int DocTypeId)//CurrentHeaderId
        {
            var PrevId = new NextPrevIdService(_unitOfWork).GetNextPrevId(DocId, DocTypeId, User.Identity.Name, "", "Web.JobOrderHeaders", "JobOrderHeaderId", PrevNextConstants.Prev);
            return Edit(PrevId, "");
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
                context.Dispose();
            }
            base.Dispose(disposing);
        }



        public ActionResult GenerateGatePass(string Ids, int DocTypeId)
        {

            if (!string.IsNullOrEmpty(Ids))
            {
                int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
                int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
                int PK = 0;

                var Settings = new JobOrderSettingsService(_unitOfWork).GetJobOrderSettingsForDocument(DocTypeId, DivisionId, SiteId);
                var GatePassDocTypeID = new DocumentTypeService(_unitOfWork).FindByName(TransactionDocCategoryConstants.GatePass).DocumentTypeId;
                string JobHeaderIds = "";

                try
                {

                    foreach (var item in Ids.Split(',').Select(Int32.Parse))
                    {
                        TimePlanValidation = true;

                        var pd = context.JobOrderHeader.Find(item);

                        if (!pd.GatePassHeaderId.HasValue)
                        {
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

                            if ((TimePlanValidation || Continue))
                            {
                                if (Settings.isPostedInStock.HasValue && Settings.isPostedInStock == true)
                                {

                                    if (!String.IsNullOrEmpty(Settings.SqlProcGatePass) && pd.Status == (int)StatusConstants.Submitted && !pd.GatePassHeaderId.HasValue)
                                    {

                                        SqlParameter SqlParameterUserId = new SqlParameter("@Id", item);
                                        IEnumerable<GatePassGeneratedViewModel> GatePasses = context.Database.SqlQuery<GatePassGeneratedViewModel>(Settings.SqlProcGatePass + " @Id", SqlParameterUserId).ToList();

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
                                            GPHeader.PersonId = pd.JobWorkerId;
                                            GPHeader.SiteId = pd.SiteId;
                                            GPHeader.GodownId = pd.GodownId ?? 0;
                                            GPHeader.GatePassHeaderId = PK++;
                                            GPHeader.ReferenceDocTypeId = pd.DocTypeId;
                                            GPHeader.ReferenceDocId = pd.JobOrderHeaderId;
                                            GPHeader.ReferenceDocNo = pd.DocNo;
                                            GPHeader.ObjectState = Model.ObjectState.Added;
                                            context.GatePassHeader.Add(GPHeader);

                                            //new GatePassHeaderService(_unitOfWork).Create(GPHeader);


                                            foreach (GatePassGeneratedViewModel GatepassLine in GatePasses)
                                            {
                                                GatePassLine Gline = new GatePassLine();
                                                Gline.CreatedBy = User.Identity.Name;
                                                Gline.CreatedDate = DateTime.Now;
                                                Gline.GatePassHeaderId = GPHeader.GatePassHeaderId;
                                                Gline.ModifiedBy = User.Identity.Name;
                                                Gline.ModifiedDate = DateTime.Now;
                                                Gline.Product = GatepassLine.ProductName;
                                                Gline.Qty = GatepassLine.Qty;
                                                Gline.Specification = GatepassLine.Specification;
                                                Gline.UnitId = GatepassLine.UnitId;

                                                // new GatePassLineService(_unitOfWork).Create(Gline);
                                                Gline.ObjectState = Model.ObjectState.Added;
                                                context.GatePassLine.Add(Gline);
                                            }

                                            pd.GatePassHeaderId = GPHeader.GatePassHeaderId;

                                            pd.ObjectState = Model.ObjectState.Modified;
                                            context.JobOrderHeader.Add(pd);

                                            JobHeaderIds += pd.StockHeaderId + ", ";
                                        }

                                        context.SaveChanges();
                                    }

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
                    Narration = "GatePass created for Job Orders " + JobHeaderIds,
                }));

                if (string.IsNullOrEmpty((string)TempData["CSEXC"]))
                    return Json(new { success = "Success" }, JsonRequestBehavior.AllowGet).Success("Gate passes generated successfully");
                else
                    return Json(new { success = "Success" }, JsonRequestBehavior.AllowGet);

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

                    var pd = context.JobOrderHeader.Find(Id);

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
                        pd.GatePassHeaderId = null;
                        pd.Status = (int)StatusConstants.Modified;
                        pd.ModifiedBy = User.Identity.Name;
                        pd.ModifiedDate = DateTime.Now;
                        pd.IsGatePassPrinted = false;
                        pd.ObjectState = Model.ObjectState.Modified;
                        context.JobOrderHeader.Add(pd);

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

        public ActionResult GeneratePrints(string Ids, int DocTypeId)
        {

            if (!string.IsNullOrEmpty(Ids))
            {
                int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
                int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

                var Settings = new JobOrderSettingsService(_unitOfWork).GetJobOrderSettingsForDocument(DocTypeId, DivisionId, SiteId);

                if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, DocTypeId, Settings.ProcessId, this.ControllerContext.RouteData.Values["controller"].ToString(),"GeneratePrints") == false)
                {
                    return View("~/Views/Shared/PermissionDenied.cshtml").Warning("You don't have permission to do this task.");
                }

                string ReportSql = "";

                if (!string.IsNullOrEmpty(Settings.DocumentPrint))
                    ReportSql = context.ReportHeader.Where((m) => m.ReportName == Settings.DocumentPrint).FirstOrDefault().ReportSQL;

                try
                {

                    List<byte[]> PdfStream = new List<byte[]>();
                    foreach (var item in Ids.Split(',').Select(Int32.Parse))
                    {
                        int Copies = 1;
                        int AdditionalCopies = Settings.NoOfPrintCopies ?? 0;
                        bool UpdateGatePassPrint = true;

                        DirectReportPrint drp = new DirectReportPrint();

                        var pd = context.JobOrderHeader.Find(item);

                        if (Settings.isAllowedDuplicatePrint == false && pd.IsDocumentPrinted == true)
                            throw new Exception("Duplicate Print Not Allowed");

                        LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                        {
                            DocTypeId = pd.DocTypeId,
                            DocId = pd.JobOrderHeaderId,
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
                                if (pd.Status == (int)StatusConstants.Drafted || pd.Status == (int)StatusConstants.Import || pd.Status == (int)StatusConstants.Modified)
                                {

                                    if (Settings.SqlProcDocumentPrint == null || Settings.SqlProcDocumentPrint == "")
                                    {
                                        JobOrderHeaderRDL cr = new JobOrderHeaderRDL();
                                        drp.CreateRDLFile("Std_JobOrder_Print", cr.Create_Std_JobOrder_Print());
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
                                        JobOrderHeaderRDL cr = new JobOrderHeaderRDL();
                                        drp.CreateRDLFile("Std_JobOrder_Print", cr.Create_Std_JobOrder_Print());
                                        List<ListofQuery> QueryList = new List<ListofQuery>();
                                        QueryList = DocumentPrintData(item);
                                        Pdf = drp.DocumentPrint_New(QueryList, User.Identity.Name);
                                    }
                                    else
                                        Pdf = drp.DirectDocumentPrint(Settings.SqlProcDocumentPrint_AfterSubmit, User.Identity.Name, item);

                                    PdfStream.Add(Pdf);
                                }
                                else
                                {
                                    if (Settings.SqlProcDocumentPrint_AfterApprove == null || Settings.SqlProcDocumentPrint_AfterApprove == "")
                                    {
                                        JobOrderHeaderRDL cr = new JobOrderHeaderRDL();
                                        drp.CreateRDLFile("Std_JobOrder_Print", cr.Create_Std_JobOrder_Print());
                                        List<ListofQuery> QueryList = new List<ListofQuery>();
                                        QueryList = DocumentPrintData(item);
                                        Pdf = drp.DocumentPrint_New(QueryList, User.Identity.Name);
                                    }
                                    else
                                        Pdf = drp.DirectDocumentPrint(Settings.SqlProcDocumentPrint_AfterApprove, User.Identity.Name, item);
                                    PdfStream.Add(Pdf);
                                }
                            }

                            if (UpdateGatePassPrint && !(pd.IsGatePassPrinted ?? false))
                            {
                                if (pd.GatePassHeaderId.HasValue)
                                {
                                    pd.IsGatePassPrinted = true;
                                    pd.ObjectState = Model.ObjectState.Modified;
                                    context.JobOrderHeader.Add(pd);
                                    context.SaveChanges();

                                    UpdateGatePassPrint = false;
                                    Copies = AdditionalCopies;
                                    if (Copies > 0)
                                        continue;
                                }
                            }

                            Copies--;

                        } while (Copies > 0);

                        if (pd.Status == 1)
                            pd.IsDocumentPrinted = true;
                        pd.ObjectState = Model.ObjectState.Modified;
                        context.JobOrderHeader.Add(pd);
                        context.SaveChanges();

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
            JobOrderHeader JOH = new JobOrderHeaderService(_unitOfWork).Find(item);

            List<ListofQuery> DocumentPrintData = new List<ListofQuery>();
            String QueryMain;
            QueryMain= @"DECLARE @TotalAmount DECIMAL 
    SET @TotalAmount = (SELECT Max(Amount) FROM web.JobOrderheaderCharges WHERE HeaderTableId = " + item + @" AND ChargeId = 34 ) 
	
	DECLARE @DocDate DATETIME
    SET @DocDate = (SELECT DocDate FROM Web.JobOrderHeaders WHERE JobOrderHeaderId = " + item + @") 
  
	  
	DECLARE @UnitDealCnt INT
    SELECT
    @UnitDealCnt = sum(CASE WHEN UnitId != DealunitId THEN 1 ELSE 0 END)
    FROM Web.JobOrderLines WHERE JobOrderHeaderId = " + item + @"

    SELECT
    --Header Table Fields
    H.JobOrderHeaderId,H.DocTypeId,H.DocNo,DocIdCaption + ' No' AS DocIdCaption,
      H.SiteId,H.DivisionId,H.DocDate,DTS.DocIdCaption + ' Date' AS DocIdCaptionDate, format(H.DueDate, 'dd/MMM/yy') AS DueDate, DocIdCaption+'Due Date' AS DocIdCaptionDueDate, Pp.Name AS OrderBy,	(CASE WHEN JOS.isVisibleProcessHeader > 0 THEN PS.ProcessName ELSE NULL END) AS ProcessName,
         H.TermsAndConditions,H.CreditDays,H.Remark,DT.DocumentTypeShortName,(CASE WHEN H.IsGatePassPrinted = 1 THEN NULL ELSE H.GatePassHeaderId END) as GatePassHeaderId,H.ModifiedBy + ' ' + Replace(replace(convert(NVARCHAR, H.ModifiedDate, 106), ' ', '/'), '/20', '/') + substring(convert(NVARCHAR, H.ModifiedDate), 13, 7) AS ModifiedBy,
               H.ModifiedDate,(CASE WHEN Isnull(H.Status, 0)= 0 OR Isnull(H.Status, 0)= 8 THEN 0 ELSE 1 END)  AS Status,
                   CUR.Name AS CurrencyName,SM.ShipMethodName,SMA.Address AS ShipToAddress,DLT.DeliveryTermsName,
	(CASE WHEN SPR.[Party GST NO]
        IS NULL THEN 'Yes' ELSE 'No' END ) AS ReverseCharge,
VDC.CompanyName,
	--Godown Detail
    G.GodownName,
	--Person Detail
    P.Name AS PartyName, DTS.PartyCaption AS  PartyCaption, P.Suffix AS PartySuffix,	
    isnull(PA.Address,'')+' '+isnull(C.CityName,'')+','+isnull(PA.ZipCode,'')+(CASE WHEN isnull(CS.StateName,'') <> isnull(S.StateName,'') AND SPR.[Party GST NO] IS NOT NULL THEN ',State : '+isnull(S.StateName,'')+(CASE WHEN S.StateCode IS NULL THEN '' ELSE ', Code : '+S.StateCode END)    ELSE '' END ) AS PartyAddress,
    isnull(S.StateName, '') AS PartyStateName, isnull(S.StateCode, '') AS PartyStateCode,

       P.Mobile AS PartyMobileNo,	SPR.*,
	--Plan Detail
    POH.DocNo AS PlanNo,DTS.ContraDocTypeCaption,
	--Caption Fields
    DTS.SignatoryMiddleCaption,DTS.SignatoryRightCaption,
	--Line Table
    PD.ProductName,DTS.ProductCaption,U.UnitName,U.DecimalPlaces,DU.UnitName AS DealUnitName,DTS.DealQtyCaption,DU.DecimalPlaces AS DealDecimalPlaces,
    isnull(L.Qty,0) AS Qty, isnull(L.Rate, 0) AS Rate, isnull(L.Amount, 0) AS Amount, isnull(L.DealQty, 0) AS DealQty,
          D1.Dimension1Name,DTS.Dimension1Caption,D2.Dimension2Name,DTS.Dimension2Caption,D3.Dimension3Name,DTS.Dimension3Caption,D4.Dimension4Name,DTS.Dimension4Caption,
	L.LotNo AS LotNo,(CASE WHEN DTS.PrintSpecification >0 THEN L.Specification ELSE '' END)  AS Specification, DTS.SpecificationCaption,DTS.SignatoryleftCaption,L.Remark AS LineRemark,
	L.DiscountPer AS DiscountPer,
	L.DiscountAmount AS DiscountAmt,
	--STC.Code AS SalesTaxProductCodes,
	(CASE WHEN H.ProcessId IN(26,28) THEN STC.Code ELSE PSSTC.Code END)  AS SalesTaxProductCodes,
    (SELECT TOP 1 SalesTaxProductCodeCaption FROM web.SiteDivisionSettings WHERE H.DocDate BETWEEN StartDate AND IsNull(EndDate, getdate()) AND SiteId = H.SiteId AND DivisionId = H.DivisionId)  AS SalesTaxProductCodeCaption,
       (CASE WHEN DTS.PrintProductGroup > 0 THEN isnull(PG.ProductGroupName, '') ELSE '' END)+(CASE WHEN DTS.PrintProductdescription >0 THEN isnull(','+PD.Productdescription,'') ELSE '' END) AS ProductGroupName,
         DTS.ProductGroupCaption,  
	PU.ProductUidName,
	(CASE WHEN PS.ProcessName IN('Purchase','Sale') THEN isnull(CGPD.PrintingDescription, CGPD.ChargeGroupProductName) ELSE PS.GSTPrintDesc END)  AS ChargeGroupProductName,


   DTS.ProductUidCaption,
	--Formula Fields
    @TotalAmount AS NetAmount,     	
	--SalesTaxGroupPersonId
    CGP.ChargeGroupPersonName,
	--Other Fields
    @UnitDealCnt AS DealUnitCnt,
	'StdDocPrintSub_CalculationHeaders ' + convert(NVARCHAR, " + item + @") + ', ' + '''web.jobOrderheadercharges'''+ ', ' + '''Web.JobOrderLineCharges'''+ ', ' + '''Web.JobOrderLines''' AS SubReportProcList,
     (CASE WHEN Isnull(H.Status, 0) = 0 OR Isnull(H.Status, 0) = 8 THEN 'Provisional ' + isnull(DT.PrintTitle, DT.DocumentTypeName) ELSE isnull(DT.PrintTitle, DT.DocumentTypeName) END) AS ReportTitle,
          	'Std_JobOrder_Print.rdl' AS ReportName,
              SalesTaxGroupProductCaption
    FROM Web.JobOrderHeaders H WITH (Nolock)
    LEFT JOIN web.DocumentTypes DT WITH(Nolock) ON DT.DocumentTypeId=H.DocTypeId
   LEFT JOIN Web._DocumentTypeSettings DTS WITH (Nolock) ON DTS.DocumentTypeId=DT.DocumentTypeId
   LEFT JOIN Web.JobOrderSettings JOS WITH (Nolock) ON JOS.DocTypeId=DT.DocumentTypeId AND JOS.SiteId= H.SiteId AND JOS.DivisionId= H.DivisionId
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
LEFT JOIN web.People Pp WITH (Nolock) ON Pp.PersonID=H.OrderById
LEFT JOIN web.ChargeGroupPersons CGP WITH (Nolock) ON CGP.ChargeGroupPersonId=H.SalesTaxGroupPersonId
LEFT JOIN Web.Currencies CUR WITH (Nolock) ON CUR.Id=H.CurrencyId
LEFT JOIN Web.ShipMethods SM WITH (Nolock) ON SM.ShipMethodId=H.ShipMethodId
LEFT JOIN Web.PersonAddresses SMA WITH (Nolock) ON SMA.PersonAddressID=H.ShipToAddressId
LEFT JOIN Web.DeliveryTerms DLT WITH (Nolock) ON DLT.DeliveryTermsId=H.DeliveryTermsId
LEFT JOIN Web.JobOrderLines L WITH (Nolock) ON L.JobOrderHeaderId=H.JobOrderHeaderId
LEFT JOIN Web.ProductUids PU WITH (Nolock) ON PU.ProductUIDId=L.ProductUidId
LEFT JOIN Web.ProdOrderLines POl WITH (Nolock) ON POl.ProdOrderLineId=L.ProdOrderLineId
LEFT JOIN Web.ProdOrderHeaders POH WITH (Nolock) ON POH.ProdOrderHeaderId=POL.ProdOrderHeaderId
LEFT JOIN web.Products PD WITH (Nolock) ON PD.ProductId=L.ProductId
LEFT JOIN web.ProductGroups PG WITH (Nolock) ON PG.ProductGroupId=PD.ProductGroupid
LEFT JOIN Web.SalesTaxProductCodes STC WITH (Nolock) ON STC.SalesTaxProductCodeId= IsNull(PD.SalesTaxProductCodeId, Pg.DefaultSalesTaxProductCodeId)
    LEFT JOIN Web.Dimension1 D1 WITH(Nolock) ON D1.Dimension1Id=L.Dimension1Id
   LEFT JOIN web.Dimension2 D2 WITH (Nolock) ON D2.Dimension2Id=L.Dimension2Id
   LEFT JOIN web.Dimension3 D3 WITH (Nolock) ON D3.Dimension3Id=L.Dimension3Id
   LEFT JOIN Web.Dimension4 D4 WITH (nolock) ON D4.Dimension4Id=L.Dimension4Id
   LEFT JOIN web.Units U WITH (Nolock) ON U.UnitId=PD.UnitId
   LEFT JOIN web.Units DU WITH (Nolock) ON DU.UnitId=L.DealUnitId
   LEFT JOIN Web.Std_PersonRegistrations SPR WITH (Nolock) ON SPR.CustomerId=H.JobWorkerId
   LEFT JOIN web.ChargeGroupProducts CGPD WITH (Nolock) ON L.SalesTaxGroupProductId = CGPD.ChargeGroupProductId
      WHERE H.JoborderheaderId= " + item + @"
    ORDER BY L.Sr";
            ListofQuery QryMain = new ListofQuery();
            QryMain.Query = QueryMain;
            QryMain.QueryName = nameof(QueryMain);
            DocumentPrintData.Add(QryMain);


            String QueryCalculation;
            QueryCalculation = @"


                    DECLARE @StrGrossAmount AS NVARCHAR(50)
                    DECLARE @StrBasicExciseDuty AS NVARCHAR(50)
                    DECLARE @StrExciseECess AS NVARCHAR(50)
                    DECLARE @StrExciseHECess AS NVARCHAR(50)

                    DECLARE @StrSalesTaxTaxableAmt AS NVARCHAR(50)
                    DECLARE @StrVAT AS NVARCHAR(50)
                    DECLARE @StrSAT AS NVARCHAR(50)
                    DECLARE @StrCST AS NVARCHAR(50)

                    SET @StrGrossAmount = 'Gross Amount'
                    SET @StrBasicExciseDuty = 'Basic Excise Duty'
                    SET @StrExciseECess = 'Excise ECess'
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

                            SELECT @GrossAmount = sum(CASE WHEN C.ChargeName = ''' + @StrGrossAmount + ''' THEN  H.Amount  ELSE 0 END),
                            @BasicExciseDutyAmount = sum(CASE WHEN C.ChargeName = ''' + @StrBasicExciseDuty + ''' THEN  H.Amount  ELSE 0 END),
                            @SalesTaxTaxableAmt = sum(CASE WHEN C.ChargeName = ''' + @StrSalesTaxTaxableAmt + ''' THEN  H.Amount  ELSE 0 END)
                            FROM web.JobOrderHeaderCharges H
                            LEFT JOIN web.ChargeTypes CT ON CT.ChargeTypeId = H.ChargeTypeId
                            LEFT JOIN web.Charges C ON C.ChargeId = H.ChargeId
                            WHERE H.Amount <> 0 AND H.HeaderTableId = ' + Convert(Varchar," + item + @" ) + '
                            GROUP BY H.HeaderTableId


                            SELECT H.Id, H.HeaderTableId, H.Sr, C.ChargeName, H.Amount, H.ChargeTypeId,  CT.ChargeTypeName, 
		                    --CASE WHEN C.ChargeName = ''Vat'' THEN(H.Amount * 100 / @GrossAmount) ELSE H.Rate End  AS Rate,
                            CASE
                            WHEN @SalesTaxTaxableAmt> 0 And C.ChargeName IN (''' + @StrVAT + ''', ''' + @StrSAT + ''', ''' + @StrCST+ ''')  THEN(H.Amount * 100 / @SalesTaxTaxableAmt)
                            WHEN @GrossAmount> 0 AND C.ChargeName IN (''' + @StrBasicExciseDuty + ''')  THEN(H.Amount * 100 / @GrossAmount)
                            WHEN @BasicExciseDutyAmount> 0 AND C.ChargeName IN (''' + @StrExciseECess + ''', ''' +@StrExciseHECess+ ''')  THEN(H.Amount * 100 / @BasicExciseDutyAmount)
                            ELSE 0 End AS Rate,
		                    ''TransactionChargesPrint.rdl'' AS ReportName,
                            ''Transaction Charges'' AS ReportTitle
                            FROM Web.JobOrderheaders JOH
                            Left Join web.JobOrderHeaderCharges  H on JOH.JobOrderHeaderId = H.HeaderTableId
                            LEFT JOIN web.ChargeTypes CT ON CT.ChargeTypeId = H.ChargeTypeId
                            LEFT JOIN web.Charges C ON C.ChargeId = H.ChargeId
                            WHERE(isnull(H.ChargeTypeId, 0) <> ''4'' OR C.ChargeName = ''Net Amount'') 
                            --AND H.Amount <> 0
                            AND JOH.JobOrderHeaderId = ' + Convert(Varchar," + item + @" ) + ''


                        DECLARE @TmpData TABLE
                        (
                        id BIGINT,
                        HeaderTableId INT,
                        Sr INT,
                        ChargeName NVARCHAR(50),
                        Amount DECIMAL(18, 4),
                        ChargeTypeId INT,
                        ChargeTypeName NVARCHAR(50),
                        Rate DECIMAL(38, 20),
                        ReportName nVarchar(255),
                        ReportTitle nVarchar(255)
                        );


                                    Insert Into @TmpData EXEC(@Qry)
                                    SELECT id, HeaderTableId, Sr, ChargeName, Amount, ChargeTypeId, ChargeTypeName, Rate, ReportName FROM @TmpData
                                    ORDER BY Sr";


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
FROM Web.JobOrderLines L
LEFT JOIN Web.JobOrderLineCharges LC ON L.JobOrderLineId = LC.LineTableId 
LEFT JOIN web.JobOrderheaders H ON H.JobOrderHeaderId = L.JobOrderHeaderId
LEFT JOIN Web.Processes PS WITH (Nolock) ON PS.ProcessId=H.ProcessId
LEFT JOIN Web.Charges C ON C.ChargeId=LC.ChargeId
LEFT JOIN web.ChargeTypes CT ON LC.ChargeTypeId = CT.ChargeTypeId 
LEFT JOIN web.ChargeGroupProducts STGP ON L.SalesTaxGroupProductId = STGP.ChargeGroupProductId
WHERE L.JobOrderHeaderId =" + item + @" 
GROUP BY isnull(STGP.PrintingDescription,STGP.ChargeGroupProductName)
--GROUP BY CASE WHEN PS.ProcessName IN (''Purchase'',''Sale'') THEN isnull(STGP.PrintingDescription,STGP.ChargeGroupProductName) ELSE PS.GSTPrintDesc END '

--PRINT @Qry;
EXEC(@Qry);	";


            ListofQuery QryGSTSummary = new ListofQuery();
            QryGSTSummary.Query = QueryGSTSummary;
            QryGSTSummary.QueryName = nameof(QueryGSTSummary);
            DocumentPrintData.Add(QryGSTSummary);



                String QueryGatePass;
                QueryGatePass = @"SELECT JOH.JobOrderHeaderId, H.GatePassHeaderId,  DT.DocumentTypeShortName +'-'+ H.DocNo AS DocNo, H.DocDate,  H.Remark, P.Name AS PersonName, G.GodownName,  
                L.GatePassLineId, L.Product, L.Specification, L.Qty, U.UnitName, U.DecimalPlaces,
                ' " + JOH.DocNo + @"' AS ReferenceDocNo,'GatePassPrint.rdl'  AS ReportName, 'Gate Pass' AS ReportTitle,
                NULL AS SubReportProcList,
                DTS.SignatoryleftCaption,
                DTS.SignatoryMiddleCaption,
                DTS.SignatoryRightCaption    
                FROM Web.joborderheaders JOH
                LEFT JOIN  Web.GatePassHeaders H ON JOH.GatePassHeaderId = H.GatePassHeaderId 
                LEFT JOIN web.Godowns G ON G.GodownId = H.GodownId 
                LEFT JOIN web.People P ON P.PersonID  = H.PersonID 
                LEFT JOIN [Web].DocumentTypes DT WITH (nolock) ON DT.DocumentTypeId = H.DocTypeId 
                LEFT JOIN web._DocumentTypeSettings DTS WITH (Nolock) ON DTS.DocumentTypeId=H.DocTypeId 
                LEFT JOIN web.GatePassLines L ON L.GatePassHeaderId = H.GatePassHeaderId 
                LEFT JOIN web.Units U ON U.UnitId = L.UnitId 
                WHERE JOH.JobOrderHeaderId = " + item + @" ";


                ListofQuery QryGatePass = new ListofQuery();
                QryGatePass.Query = QueryGatePass;
                QryGatePass.QueryName = nameof(QueryGatePass);
                DocumentPrintData.Add(QryGatePass);

            return DocumentPrintData;

        }




        protected string GetIPAddress()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }

        public JsonResult ValidateCostCenter(int DocTypeId, int HeaderId, int JobWorkerId, string CostCenterName)
        {
            return Json(_JobOrderHeaderService.ValidateCostCenter(DocTypeId, HeaderId, JobWorkerId, CostCenterName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLineProgress(int LineId)
        {

            var ProgressDetail = _JobOrderHeaderService.GetLineProgressDetail(LineId);

            return PartialView("_LineProgress", ProgressDetail);
        }

        public JsonResult GetJobWorkerDetailJson(int JobWorkerId)
        {
            var temp = new PersonService(_unitOfWork).GetPersonViewModelForEdit(JobWorkerId);
 
            return Json(temp);
        }

        #region submitValidation
        public bool Submitvalidation(int id, out string Msg)
        {
            Msg = "";            
            int Stockline = (new JobOrderLineService(_unitOfWork).GetJobOrderLineListForIndex(id)).Count();
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
        public ActionResult GetCustomPerson(string searchTerm, int pageSize, int pageNum, int filter)//DocTypeId
        {
            var Query = _JobOrderHeaderService.GetCustomPerson(filter, searchTerm);
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

        public JsonResult GetProcessPermissionJson(int DocTypeId, int ProcessId)
        {
            var temp = new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, DocTypeId, ProcessId, this.ControllerContext.RouteData.Values["controller"].ToString(), "Create");

            return Json(temp);
        }

        [HttpGet]
        public ActionResult EMail(int id)
        {
            var  Header = (from H in context.JobOrderHeader
                           where H.JobOrderHeaderId == id
                         select new
                         {
                             JobOrderHeaderId = H.JobOrderHeaderId,
                             DocTypeId = H.DocTypeId,
                             Subject = H.DocType.DocumentTypeName,
                             EMail = H.JobWorker.Person.Email,
                             DocDate = H.DocDate,
                             DocNo = H.DocNo
                         }).FirstOrDefault();

            string ToEMail = "";
            string Subject = "";
            string Body = "";

            if (Header != null)
            {
                ToEMail = Header.EMail;
                Subject = Header.Subject;
            }

            Body = @"Dear Sir,

                    Please find attachment.";


            //For Vehicle Purchase Order
            Subject = "Requisition of Vehicles";
            Body = @" <p> Dear Sir,</p>

                <p>Please find attachment and arrange for billing of Vehicles</p>

                <p>Dt:- " + Header.DocDate.ToString("dd/MMM/yyyy") + @"</p>

                                                               

                <p>Requisition No- " + Header.DocNo + @"</p>

                <p>Sub.: -Requisition of Vehicles</p>

 

                <p>Dear Sir,</p>

                <p>We request you to kindly book the following vehicle details of which are mentioned below:-</p>";



            var JobOrderLine = (from L in context.JobOrderLine
                                where L.JobOrderHeaderId == id
                                select new
                                {
                                    ProductName = L.Product.ProductName,
                                    ProductGroupName = L.Product.ProductGroup.ProductGroupName,
                                    Qty = L.Qty
                                }).ToList();



            Body = Body  + @"<table style='width: 100 %;border: 1px solid black;border-collapse: collapse'>
                        <tr>
                            <th style='border: 1px solid black;border-collapse: collapse'>SL.NO</th>
                            <th style='border: 1px solid black;border-collapse: collapse'>V C NO</th>
                            <th style='border: 1px solid black;border-collapse: collapse'>MODEL</th>
                            <th style='border: 1px solid black;border-collapse: collapse'>QUANTITY</th>
                            <th style='border: 1px solid black;border-collapse: collapse'>PAYER CODE</th>
                        </tr> ";

            int Sr = 0;
            foreach (var item in JobOrderLine)
            {
                Body = Body + @"<tr>
                        <td style='border: 1px solid black;border-collapse: collapse;padding: 15px;'>" + (++Sr).ToString() + @"</td>
                        <td style='border: 1px solid black;border-collapse: collapse;padding: 15px;'>" + item.ProductName + @"</td>
                        <td style='border: 1px solid black;border-collapse: collapse;padding: 15px;'>" + item.ProductGroupName + @"</td>
                        <td style='border: 1px solid black;border-collapse: collapse;padding: 15px;'>" + Math.Round(item.Qty,0) + @"</td>
                        <td style='border: 1px solid black;border-collapse: collapse;padding: 15px;'>1339800</td>
                        </tr>";
            }

            Body = Body + @"<tr>
                        <td style='border: 1px solid black;border-collapse: collapse;padding: 15px;'></td>
                        <td style='border: 1px solid black;border-collapse: collapse;padding: 15px;'></td>
                        <td style='border: 1px solid black;border-collapse: collapse;padding: 15px;'>Total</td>
                        <td style='border: 1px solid black;border-collapse: collapse;padding: 15px;'>" + Math.Round(JobOrderLine.Sum(i => i.Qty),0).ToString() + @"</td>
                        <td style='border: 1px solid black;border-collapse: collapse;padding: 15px;'></td>
                        </tr>";


            Body = Body + "</table>";


            //End Vehicle Purchase Order



            byte[] filedata = GetPrintData(Header.JobOrderHeaderId.ToString(), Header.DocTypeId);
            string attachment = Request.MapPath(ConfigurationManager.AppSettings["ExcelFilePath"]) + Header.JobOrderHeaderId.ToString() + ".pdf";


            

            System.IO.File.WriteAllBytes(attachment, filedata);

            EmailMessage vm = new EmailMessage();
            vm.To = ToEMail;
            vm.Subject = Subject;
            vm.Body = Body;
            vm.Attachment = attachment;


            return View("~/Views/Mailbox/Compose.cshtml", vm);

            //return RedirectToAction("Compose", "Mailbox", new { ToEMail = ToEMail, Subject = Subject, Body = Body, attachment = attachment });
        }


        public byte[] GetPrintData(string Ids, int DocTypeId)
        {
            if (!string.IsNullOrEmpty(Ids))
            {
                int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
                int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

                var Settings = new JobOrderSettingsService(_unitOfWork).GetJobOrderSettingsForDocument(DocTypeId, DivisionId, SiteId);

                string ReportSql = "";

                if (!string.IsNullOrEmpty(Settings.DocumentPrint))
                    ReportSql = context.ReportHeader.Where((m) => m.ReportName == Settings.DocumentPrint).FirstOrDefault().ReportSQL;

                try
                {
                    List<byte[]> PdfStream = new List<byte[]>();
                    foreach (var item in Ids.Split(',').Select(Int32.Parse))
                    {
                        int Copies = 1;
                        int AdditionalCopies = Settings.NoOfPrintCopies ?? 0;
                        bool UpdateGatePassPrint = true;

                        DirectReportPrint drp = new DirectReportPrint();

                        var pd = context.JobOrderHeader.Find(item);

                        LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                        {
                            DocTypeId = pd.DocTypeId,
                            DocId = pd.JobOrderHeaderId,
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
                                if (pd.Status == (int)StatusConstants.Drafted || pd.Status == (int)StatusConstants.Import || pd.Status == (int)StatusConstants.Modified)
                                {

                                    if (Settings.SqlProcDocumentPrint == null || Settings.SqlProcDocumentPrint == "")
                                    {
                                        //List<string> QueryList = new List<string>();
                                        //QueryList = DocumentPrintData(item);
                                        //Pdf = drp.DocumentPrint(QueryList, User.Identity.Name);
                                        Pdf = drp.DirectDocumentPrint(Settings.SqlProcDocumentPrint, User.Identity.Name, item);
                                    }
                                    else
                                        Pdf = drp.DirectDocumentPrint(Settings.SqlProcDocumentPrint, User.Identity.Name, item);

                                    PdfStream.Add(Pdf);
                                }
                                else if (pd.Status == (int)StatusConstants.Submitted || pd.Status == (int)StatusConstants.ModificationSubmitted)
                                {
                                    if (Settings.SqlProcDocumentPrint_AfterSubmit == null || Settings.SqlProcDocumentPrint_AfterSubmit == "")
                                    {
                                        //List<string> QueryList = new List<string>();
                                        //QueryList = DocumentPrintData(item);
                                        //Pdf = drp.DocumentPrint(QueryList, User.Identity.Name);
                                        Pdf = drp.DirectDocumentPrint(Settings.SqlProcDocumentPrint_AfterSubmit, User.Identity.Name, item);
                                    }
                                    else
                                        Pdf = drp.DirectDocumentPrint(Settings.SqlProcDocumentPrint_AfterSubmit, User.Identity.Name, item);

                                    PdfStream.Add(Pdf);
                                }
                                else
                                {
                                    if (Settings.SqlProcDocumentPrint_AfterApprove == null || Settings.SqlProcDocumentPrint_AfterApprove == "")
                                    {
                                        //List<string> QueryList = new List<string>();
                                        //QueryList = DocumentPrintData(item);
                                        //Pdf = drp.DocumentPrint(QueryList, User.Identity.Name);
                                        Pdf = drp.DirectDocumentPrint(Settings.SqlProcDocumentPrint_AfterApprove, User.Identity.Name, item);
                                    }
                                    else
                                        Pdf = drp.DirectDocumentPrint(Settings.SqlProcDocumentPrint_AfterApprove, User.Identity.Name, item);
                                    PdfStream.Add(Pdf);
                                }
                            }

                            if (UpdateGatePassPrint && !(pd.IsGatePassPrinted ?? false))
                            {
                                if (pd.GatePassHeaderId.HasValue)
                                {
                                    pd.IsGatePassPrinted = true;
                                    pd.ObjectState = Model.ObjectState.Modified;
                                    context.JobOrderHeader.Add(pd);
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

                    return Merge;
                }

                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    return null;
                }
            }
            return null;
        }
    }
}
