using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Core.Common;
using Model.Models;
using Data.Models;
using Model.ViewModels;
using Service;
using Jobs.Helpers;
using Data.Infrastructure;
using Presentation.ViewModels;
using AutoMapper;
using Presentation;
using System.Text;
using EmailContents;
using Model.ViewModel;
using System.Xml.Linq;
using Reports.Controllers;
using Reports.Reports;
using System.Configuration;

namespace Jobs.Areas.Rug.Controllers
{
    [Authorize]
    public class CostingHeaderController : System.Web.Mvc.Controller
    {

        private ApplicationDbContext context = new ApplicationDbContext();        

        List<string> UserRoles = new List<string>();
        ActiivtyLogViewModel LogVm = new ActiivtyLogViewModel();

        bool TimePlanValidation = true;
        string ExceptionMsg = "";
        bool Continue = true;

        ICostingHeaderService _CostingHeaderService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;
        public CostingHeaderController(ICostingHeaderService PurchaseOrderHeaderService, IActivityLogService ActivityLogService, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _CostingHeaderService = PurchaseOrderHeaderService;
            _unitOfWork = unitOfWork;
            _exception = exec;

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

        // GET: /CostingHeader/

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
            IQueryable<CostingHeaderIndexViewModel> p = _CostingHeaderService.GetCostingHeaderList(id, User.Identity.Name);
            PrepareViewBagSettings(id);
            ViewBag.PendingToSubmit = PendingToSubmitCount(id);
            ViewBag.PendingToReview = PendingToReviewCount(id);
            ViewBag.IndexStatus = "All";
            return View(p);
        }

        public ActionResult Index_PendingToSubmit(int id)
        {
            var PendingToSubmit = _CostingHeaderService.GetCostingHeaderListPendingToSubmit(id, User.Identity.Name);
            PrepareViewBagSettings(id);
            ViewBag.PendingToSubmit = PendingToSubmitCount(id);
            ViewBag.PendingToReview = PendingToReviewCount(id);
            ViewBag.IndexStatus = "PTS";
            return View("Index", PendingToSubmit);
        }

        public ActionResult Index_PendingToReview(int id)
        {
            var PendingtoReview = _CostingHeaderService.GetCostingHeaderListPendingToReview(id, User.Identity.Name);
            PrepareViewBagSettings(id);
            ViewBag.PendingToSubmit = PendingToSubmitCount(id);
            ViewBag.PendingToReview = PendingToReviewCount(id);
            ViewBag.IndexStatus = "PTR";
            return View("Index", PendingtoReview);
        }


        private void PrepareViewBagSettings(int id)
        {
            ViewBag.Name = new DocumentTypeService(_unitOfWork).Find(id).DocumentTypeName;
            ViewBag.id = id;
            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            //var settings = new CostingSettingsService(_unitOfWork).GetCostingSettings(id, DivisionId, SiteId);
            ViewBag.AdminSetting = UserRoles.Contains("Admin").ToString();
            //if (settings != null)
            //{
            //    ViewBag.ImportMenuId = settings.ImportMenuId;
            //    ViewBag.SqlProcDocumentPrint = settings.SqlProcDocumentPrint;
            //    ViewBag.ExportMenuId = settings.ExportMenuId;
            //}

        }


        [HttpGet]
        public ActionResult NextPage(int DocId, int DocTypeId)//CurrentHeaderId
        {
            var nextId = new NextPrevIdService(_unitOfWork).GetNextPrevId(DocId, DocTypeId, User.Identity.Name, "", "Web.CostingHeaders", "CostingHeaderId", PrevNextConstants.Next);
            return Edit(nextId, "");
        }
        [HttpGet]
        public ActionResult PrevPage(int DocId, int DocTypeId)//CurrentHeaderId
        {
            var PrevId = new NextPrevIdService(_unitOfWork).GetNextPrevId(DocId, DocTypeId, User.Identity.Name, "", "Web.CostingHeaders", "CostingHeaderId", PrevNextConstants.Prev);
            return Edit(PrevId, "");
        }

        [HttpGet]
        public ActionResult History()
        {
            //To Be Implemented
            //return View("~/Views/Shared/UnderImplementation.cshtml");
            MultipleDocumentPrint rvm = new MultipleDocumentPrint()
            {
                // id = 5,
            };
            return PartialView("MultipleDocumentPrint", rvm);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult History(MultipleDocumentPrint vm)
        {
            ApplicationDbContext Db = new ApplicationDbContext();
            String query = Db.strSchemaName + ".ProcCostingPrint " + 3.ToString();
            //string PackingId = (vm.CostingHeaderId.ToString());
            //String query = Db.strSchemaName + ".ProcCostingPrintToMultiple '" + PackingId.ToString() + "'";
            String ReportTitle = "Sale Order";
            return RedirectToAction("DocumentPrint", "Report_DocumentPrint", new { queryString = query, ReportTitle = ReportTitle });
        }

        [HttpGet]
        public ActionResult Email()
        {
            //To Be Implemented
            return View("~/Views/Shared/UnderImplementation.cshtml");
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
                return Redirect((string)System.Configuration.ConfigurationManager.AppSettings["JobsDomain"] + "/GridReport/GridReportLayout/?MenuName=Sale Order Report&DocTypeId=" + id.ToString());


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

        [HttpGet]
        public ActionResult Remove()
        {
            //To Be Implemented
            return View("~/Views/Shared/UnderImplementation.cshtml");
        }

        private void PrepareViewBag(CostingHeaderIndexViewModel s)
        {


            ViewBag.UnitConvForList = (from p in context.UnitConversonFor
                                       select p).ToList();
            ViewBag.Name = new DocumentTypeService(_unitOfWork).Find(s.DocTypeId).DocumentTypeName;
            ViewBag.id = s.DocTypeId;
            //ViewBag.DivisionList = new DivisionService(_unitOfWork).GetDivisionList().ToList();            
            ViewBag.ShipMethodList = new ShipMethodService(_unitOfWork).GetShipMethodList().ToList();
            ViewBag.DeliveryTermsList = new DeliveryTermsService(_unitOfWork).GetDeliveryTermsList().ToList();
            ViewBag.BuyerList = new BuyerService(_unitOfWork).GetBuyerList().ToList();
            ViewBag.CurrencyList = new CurrencyService(_unitOfWork).GetCurrencyList().ToList();
            //ViewBag.SiteList = new SiteService(_unitOfWork).GetSiteList().ToList();
            //List<SelectListItem> temp = new List<SelectListItem>();
            //temp.Add(new SelectListItem { Text = Enum.GetName(typeof(CostingPriority), -10), Value = ((int)(CostingPriority.Low)).ToString() });
            //temp.Add(new SelectListItem { Text = Enum.GetName(typeof(CostingPriority), 0), Value = ((int)(CostingPriority.Normal)).ToString() });
            //temp.Add(new SelectListItem { Text = Enum.GetName(typeof(CostingPriority), 10), Value = ((int)(CostingPriority.High)).ToString() });

            //if (s == null)
            //    ViewBag.Priority = new SelectList(temp, "Value", "Text");
            //else
            //    ViewBag.Priority = new SelectList(temp, "Value", "Text", s.Priority);

        }

        // GET: /CostingHeader/Create

        public ActionResult Create(int id)
        {

            CostingHeaderIndexViewModel p = new CostingHeaderIndexViewModel();

            p.DocDate = DateTime.Now.Date;
            p.CreatedDate = DateTime.Now;
            p.DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            p.SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            //CostingSettings settings = new CostingSettingsService(_unitOfWork).GetCostingSettings(id, p.DivisionId, p.SiteId);
            //if (settings != null)
            //{
                p.DocTypeId = id;
                PrepareViewBag(p);
            //}
            //else
            //{
            //    return RedirectToAction("Edit", "CostingSettings", new { id = id }).Warning("Please create sale order settings");
            //}

            //if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, id, settings.ProcessId, this.ControllerContext.RouteData.Values["controller"].ToString(), "Create") == false)
            //{
            //    return View("~/Views/Shared/PermissionDenied.cshtml").Warning("You don't have permission to do this task.");
            //}

            //p.CostingSettings = Mapper.Map<CostingSettings, CostingSettingsViewModel>(settings);
            ViewBag.Mode = "Add";
            p.DocNo = new DocumentTypeService(_unitOfWork).FGetNewDocNo("DocNo", ConfigurationManager.AppSettings["DataBaseSchema"] + ".CostingHeaders", p.DocTypeId, p.DocDate, p.DivisionId, p.SiteId);
            return View("Create", p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HeaderPost(CostingHeaderIndexViewModel svm)
        {

            #region DocTypeTimeLineValidation

            //try
            //{

            //    if (svm.CostingHeaderId <= 0)
            //        TimePlanValidation = DocumentValidation.ValidateDocument(Mapper.Map<DocumentUniqueId>(svm), DocumentTimePlanTypeConstants.Create, User.Identity.Name, out ExceptionMsg, out Continue);
            //    else
            //        TimePlanValidation = DocumentValidation.ValidateDocument(Mapper.Map<DocumentUniqueId>(svm), DocumentTimePlanTypeConstants.Modify, User.Identity.Name, out ExceptionMsg, out Continue);

            //}
            //catch (Exception ex)
            //{
            //    string message = _exception.HandleException(ex);
            //    TempData["CSEXC"] += message;
            //    TimePlanValidation = false;
            //}

            //if (!TimePlanValidation)
            //    TempData["CSEXC"] += ExceptionMsg;

            #endregion

            if (ModelState.IsValid && (TimePlanValidation || Continue))
            {

                #region CreateRecord
                if (svm.CostingHeaderId == 0)
                {
                    CostingHeader s = Mapper.Map<CostingHeaderIndexViewModel, CostingHeader>(svm);

                    s.CreatedDate = DateTime.Now;
                    s.ModifiedDate = DateTime.Now;
                    s.CreatedBy = User.Identity.Name;
                    s.ModifiedBy = User.Identity.Name;
                    s.Status = (int)StatusConstants.Drafted;
                    _CostingHeaderService.Create(s);

                    try
                    {
                        _unitOfWork.Save();
                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                        PrepareViewBag(svm);
                        ViewBag.Mode = "Add";
                        return View("Create", svm);
                    }

                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = s.DocTypeId,
                        DocId = s.CostingHeaderId,
                        ActivityType = (int)ActivityTypeContants.Added,
                        DocNo = s.DocNo,
                        DocDate = s.DocDate,
                        DocStatus = s.Status,
                    }));

                    return RedirectToAction("Modify", new { id = s.CostingHeaderId }).Success("Data saved Successfully");
                } 
                #endregion

                #region EditRecord
                else
                {
                    List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();

                    //string tempredirect = (Request["Redirect"].ToString());
                    CostingHeader s = Mapper.Map<CostingHeaderIndexViewModel, CostingHeader>(svm);
                    StringBuilder logstring = new StringBuilder();
                    CostingHeader temp = _CostingHeaderService.Find(s.CostingHeaderId);

                    CostingHeader ExRec = new CostingHeader();
                    ExRec = Mapper.Map<CostingHeader>(temp);

                    int status = temp.Status;

                    if (temp.Status != (int)StatusConstants.Drafted && temp.Status != (int)StatusConstants.Import)
                        temp.Status = (int)StatusConstants.Modified;

                    temp.DocTypeId = s.DocTypeId;
                    temp.DocDate = s.DocDate;
                    temp.DocNo = s.DocNo;
                    temp.PersonId = s.PersonId;
                    temp.Remark = s.Remark;
                    temp.ModifiedDate = DateTime.Now;
                    temp.ModifiedBy = User.Identity.Name;
                    _CostingHeaderService.Update(temp);

                    LogList.Add(new LogTypeViewModel
                    {
                        ExObj = ExRec,
                        Obj = temp,
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
                        PrepareViewBag(svm);
                        ViewBag.Mode = "Edit";
                        return View("Create", svm);
                    }

                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = temp.DocTypeId,
                        DocId = temp.CostingHeaderId,
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
            PrepareViewBag(svm);
            ViewBag.Mode = "Add";
            return View("Create", svm);
        }

        [HttpGet]
        public ActionResult Modify(int id, string IndexType)
        {
            CostingHeader header = _CostingHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Drafted || header.Status == (int)StatusConstants.Import)
                return Edit(id, IndexType);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult ModifyAfter_Submit(int id, string IndexType)
        {
            CostingHeader header = _CostingHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Submitted || header.Status == (int)StatusConstants.Modified || header.Status == (int)StatusConstants.ModificationSubmitted)
                return Edit(id, IndexType);
            else
                return HttpNotFound();
        }


        // GET: /CostingHeader/Edit/5
        private ActionResult Edit(int id, string IndexType)
        {
            ViewBag.IndexStatus = IndexType;

            CostingHeader s = _CostingHeaderService.GetCostingHeader(id);
            if (s == null)
            {
                return HttpNotFound();
            }

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


            CostingHeaderIndexViewModel svm = Mapper.Map<CostingHeader, CostingHeaderIndexViewModel>(s);
            PrepareViewBag(svm);
            ViewBag.Mode = "Edit";

            //CostingSettings temp = new CostingSettingsService(_unitOfWork).GetCostingSettings(s.DocTypeId, s.DivisionId, s.SiteId);
            //svm.CostingSettings = Mapper.Map<CostingSettings, CostingSettingsViewModel>(temp);
            //svm.ProcessId = temp.ProcessId;

            if (!(System.Web.HttpContext.Current.Request.UrlReferrer.PathAndQuery).Contains("Create"))
                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = s.DocTypeId,
                    DocId = s.CostingHeaderId,
                    ActivityType = (int)ActivityTypeContants.Detail,
                    DocNo = s.DocNo,
                    DocDate = s.DocDate,
                    DocStatus = s.Status,
                }));

            return View("Create", svm);
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

            CostingHeader s = _CostingHeaderService.GetCostingHeader(id);
            CostingHeaderIndexViewModel svm = Mapper.Map<CostingHeader, CostingHeaderIndexViewModel>(s);


            //var settings = new CostingSettingsService(_unitOfWork).GetCostingSettings(s.DocTypeId, s.DivisionId , s.SiteId);
            //svm.CostingSettings = Mapper.Map<CostingSettings, CostingSettingsViewModel>(settings);


            PrepareViewBag(svm);
            if (s == null)
            {
                return HttpNotFound();
            }

            if (String.IsNullOrEmpty(transactionType) || transactionType == "detail")
                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = s.DocTypeId,
                    DocId = s.CostingHeaderId,
                    ActivityType = (int)ActivityTypeContants.Detail,
                    DocNo = s.DocNo,
                    DocDate = s.DocDate,
                    DocStatus = s.Status,
                }));


            return View("Create", svm);
        }


        public ActionResult Delete(int id)
        {
            CostingHeader header = _CostingHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Drafted || header.Status == (int)StatusConstants.Import)
                return Remove(id);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult DeleteAfter_Submit(int id)
        {
            CostingHeader header = _CostingHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Submitted || header.Status == (int)StatusConstants.Modified)
                return Remove(id);
            else
                return HttpNotFound();
        }


        // GET: /PurchaseOrderHeader/Delete/5

        private ActionResult Remove(int id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CostingHeader CostingHeader = _CostingHeaderService.GetCostingHeader(id);
            if (CostingHeader == null)
            {
                return HttpNotFound();
            }

            if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, CostingHeader.DocTypeId, null, this.ControllerContext.RouteData.Values["controller"].ToString(), "Remove") == false)
            {
                return PartialView("~/Views/Shared/PermissionDenied_Modal.cshtml").Warning("You don't have permission to do this task.");
            }

            #region DocTypeTimeLineValidation
            try
            {
                TimePlanValidation = DocumentValidation.ValidateDocument(Mapper.Map<DocumentUniqueId>(CostingHeader), DocumentTimePlanTypeConstants.Delete, User.Identity.Name, out ExceptionMsg, out Continue);
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(ReasonViewModel vm)
        {
            if (ModelState.IsValid)
            {
                List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();



                


                //string temp = (Request["Redirect"].ToString());
                //first find the Purchase Order Object based on the ID. (sience this object need to marked to be deleted IE. ObjectState.Deleted)
                var CostingHeader = _CostingHeaderService.GetCostingHeader(vm.id);


                //For Updating Enquiry Header and Lines so that it can be edited and deleted as needed.
                //if (CostingHeader.ReferenceDocId != null && CostingHeader.ReferenceDocId != 0)
                //{
                //    var SaleEnquiryHeader = (from H in context.SaleEnquiryHeader where H.SaleEnquiryHeaderId == CostingHeader.ReferenceDocId && H.DocTypeId == CostingHeader.ReferenceDocTypeId select H).FirstOrDefault();
                //    if (SaleEnquiryHeader != null)
                //    {
                //        SaleEnquiryHeader Header = new SaleEnquiryHeaderService(_unitOfWork).Find(SaleEnquiryHeader.SaleEnquiryHeaderId);
                //        Header.LockReason = null;
                //        new SaleEnquiryHeaderService(_unitOfWork).Update(Header);

                //        IEnumerable<SaleEnquiryLine> LineList = new SaleEnquiryLineService(_unitOfWork).GetSaleEnquiryLineListForHeader(SaleEnquiryHeader.SaleEnquiryHeaderId);
                //        foreach (SaleEnquiryLine Line in LineList)
                //        {
                //            Line.LockReason = null;
                //            new SaleEnquiryLineService(_unitOfWork).Update(Line);
                //        }
                //    }
                //}

                

                LogList.Add(new LogTypeViewModel
                {
                    ExObj = CostingHeader,
                });

                //Then find all the Purchase Order Header Line associated with the above ProductType.
                var CostingLine = new CostingLineService(_unitOfWork).GetCostingLineList(vm.id);



                //List<int> StockIdList = new List<int>();

                ////Mark ObjectState.Delete to all the Purchase Order Lines. 
                //foreach (var item in CostingLine)
                //{
                //    if (item.StockId != null)
                //    {
                //        StockIdList.Add((int)item.StockId);
                //    }

                //    LogList.Add(new LogTypeViewModel
                //    {
                //        ExObj = item,
                //    });

                //}




                

                // Now delete the Sale Order Header
                new CostingHeaderService(_unitOfWork).Delete(vm.id);



                XElement Modifications = new ModificationsCheckService().CheckChanges(LogList);
                //Commit the DB
                try
                {
                    _unitOfWork.Save();
                }

                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    TempData["CSEXC"] += message;
                    return PartialView("_Reason", vm);
                }

                //Logging Activity

                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = CostingHeader.DocTypeId,
                    DocId = CostingHeader.CostingHeaderId,
                    ActivityType = (int)ActivityTypeContants.Deleted,
                    UserRemark = vm.Reason,
                    DocNo = CostingHeader.DocNo,
                    xEModifications = Modifications,
                    DocDate = CostingHeader.DocDate,
                    DocStatus = CostingHeader.Status,
                }));

                return Json(new { success = true });
            }
            return PartialView("_Reason", vm);
        }



        public ActionResult Submit(int id, string IndexType, string TransactionType)
        {
            CostingHeader s = context.CostingHeader.Find(id);
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
        public ActionResult Submitted(int Id, string IndexType, string UserRemark, string IsContinue)
        {
            CostingHeader pd = new CostingHeaderService(_unitOfWork).Find(Id);
            if (ModelState.IsValid)
            {
                if (User.Identity.Name == pd.ModifiedBy || UserRoles.Contains("Admin"))
                {
                    int ActivityType;

                    pd.ReviewBy = null;
                    pd.Status = (int)StatusConstants.Submitted;
                    ActivityType = (int)ActivityTypeContants.Submitted;



                    _CostingHeaderService.Update(pd);


                    _unitOfWork.Save();

                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = pd.DocTypeId,
                        DocId = pd.CostingHeaderId,
                        ActivityType = ActivityType,
                        UserRemark = UserRemark,
                        DocNo = pd.DocNo,
                        DocDate = pd.DocDate,
                        DocStatus = pd.Status,
                    }));

                    //SendEmail_PODrafted(Id);
                    //if (pd.Status == (int)StatusConstants.Submitted)
                    //    CostingEmailContents.SendCostingSubmitEmail(Id);
                    //else if (pd.Status == (int)StatusConstants.ModificationSubmitted)
                    //    CostingEmailContents.SendCostingModifiedEmail(Id, UserRemark);

                    return RedirectToAction("Index", new { id = pd.DocTypeId, IndexType = IndexType }).Success("Record Submitted successfully.");
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
            CostingHeader pd = new CostingHeaderService(_unitOfWork).Find(Id);

            if (ModelState.IsValid)
            {


                pd.ReviewCount = (pd.ReviewCount ?? 0) + 1;
                pd.ReviewBy += User.Identity.Name + ", ";

                _CostingHeaderService.Update(pd);

                _unitOfWork.Save();

                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = pd.DocTypeId,
                    DocId = pd.CostingHeaderId,
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

        public int PendingToSubmitCount(int id)
        {
            return (_CostingHeaderService.GetCostingHeaderListPendingToSubmit(id, User.Identity.Name)).Count();
        }

        public int PendingToReviewCount(int id)
        {
            return (_CostingHeaderService.GetCostingHeaderListPendingToReview(id, User.Identity.Name)).Count();
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


        public ActionResult GeneratePrints(string Ids, int DocTypeId)
        {

            if (!string.IsNullOrEmpty(Ids))
            {
                int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
                int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];


                //var Settings = new CostingSettingsService(_unitOfWork).GetCostingSettingsForDocument(DocTypeId, DivisionId, SiteId);

                //if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, DocTypeId, Settings.ProcessId, this.ControllerContext.RouteData.Values["controller"].ToString(), "GeneratePrints") == false)
                //{
                //    return View("~/Views/Shared/PermissionDenied.cshtml").Warning("You don't have permission to do this task.");
                //}

                try
                {

                    List<byte[]> PdfStream = new List<byte[]>();
                    foreach (var item in Ids.Split(',').Select(Int32.Parse))
                    {

                        DirectReportPrint drp = new DirectReportPrint();

                        var pd = context.CostingHeader.Find(item);

                        LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                        {
                            DocTypeId = pd.DocTypeId,
                            DocId = pd.CostingHeaderId,
                            ActivityType = (int)ActivityTypeContants.Print,
                            DocNo = pd.DocNo,
                            DocDate = pd.DocDate,
                            DocStatus = pd.Status,
                        }));

                        byte[] Pdf;

                        if (pd.Status == (int)StatusConstants.Drafted || pd.Status == (int)StatusConstants.Import || pd.Status == (int)StatusConstants.Modified)
                        {
                            //LogAct(item.ToString());
                            Pdf = drp.DirectDocumentPrint("Web.ProcCostingPrint", User.Identity.Name, item);

                            PdfStream.Add(Pdf);
                        }
                        else if (pd.Status == (int)StatusConstants.Submitted || pd.Status == (int)StatusConstants.ModificationSubmitted)
                        {
                            Pdf = drp.DirectDocumentPrint("Web.ProcCostingPrint", User.Identity.Name, item);

                            PdfStream.Add(Pdf);
                        }
                        else
                        {
                            Pdf = drp.DirectDocumentPrint("Web.ProcCostingPrint", User.Identity.Name, item);
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

        public ActionResult GeneratePrintMix(string Ids)
        {

            if (!string.IsNullOrEmpty(Ids))
            {
                int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
                int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];


                //var Settings = new CostingSettingsService(_unitOfWork).GetCostingSettingsForDocument(DocTypeId, DivisionId, SiteId);

                //if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, DocTypeId, Settings.ProcessId, this.ControllerContext.RouteData.Values["controller"].ToString(), "GeneratePrints") == false)
                //{
                //    return View("~/Views/Shared/PermissionDenied.cshtml").Warning("You don't have permission to do this task.");
                //}

                try
                {

                    List<byte[]> PdfStream = new List<byte[]>();
                    foreach (var item in Ids.Split(',').Select(Int32.Parse))
                    {

                        DirectReportPrint drp = new DirectReportPrint();

                        var pd = context.CostingHeader.Find(item);

                        LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                        {
                            DocTypeId = pd.DocTypeId,
                            DocId = pd.CostingHeaderId,
                            ActivityType = (int)ActivityTypeContants.Print,
                            DocNo = pd.DocNo,
                            DocDate = pd.DocDate,
                            DocStatus = pd.Status,
                        }));

                        byte[] Pdf;

                        if (pd.Status == (int)StatusConstants.Drafted || pd.Status == (int)StatusConstants.Import || pd.Status == (int)StatusConstants.Modified)
                        {
                            //LogAct(item.ToString());
                            Pdf = drp.DirectDocumentPrint("Web.ProcCostingPrintMix", User.Identity.Name, item);

                            PdfStream.Add(Pdf);
                        }
                        else if (pd.Status == (int)StatusConstants.Submitted || pd.Status == (int)StatusConstants.ModificationSubmitted)
                        {
                            Pdf = drp.DirectDocumentPrint("Web.ProcCostingPrintMix", User.Identity.Name, item);

                            PdfStream.Add(Pdf);
                        }
                        else
                        {
                            Pdf = drp.DirectDocumentPrint("Web.ProcCostingPrintMix", User.Identity.Name, item);
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


        #region submitValidation
        public bool Submitvalidation(int id, out string Msg)
        {
            Msg = "";
            //int Stockline = (new CostingLineService(_unitOfWork).GetCostingLineListForIndex(id)).Count();
            //if (Stockline == 0)
            //{
            //    Msg = "Add Line Record. <br />";
            //}
            //else
            //{
            //    Msg = "";
            //}
            return (string.IsNullOrEmpty(Msg));
        }

        #endregion submitValidation
        public ActionResult GetCustomPerson(string searchTerm, int pageSize, int pageNum, int filter)//DocTypeId
        {
            var Query = _CostingHeaderService.GetCustomPerson(filter, searchTerm);
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
    }
}
