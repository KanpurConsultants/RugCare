using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Core.Common;
using Model.Models;
using Data.Models;
using Model.ViewModels;
using Service;
using Jobs.Helpers;
using Data.Infrastructure;
using AutoMapper;
using System.Configuration;
using Presentation;
using Model.ViewModel;
using JobOrderInspectionDocumentEvents;
using CustomEventArgs;
using DocumentEvents;
using Jobs.Controllers;

namespace Jobs.Areas.Rug.Controllers
{
    [Authorize]
    public class WeavingOrderInspectionWizardController : System.Web.Mvc.Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        private bool EventException = false;

        List<string> UserRoles = new List<string>();
        ActiivtyLogViewModel LogVm = new ActiivtyLogViewModel();

        IJobOrderInspectionHeaderService _JobOrderInspectionHeaderService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;

        public WeavingOrderInspectionWizardController(IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _JobOrderInspectionHeaderService = new JobOrderInspectionHeaderService(db);
            _exception = exec;
            _unitOfWork = unitOfWork;
            if (!JobOrderInspectionEvents.Initialized)
            {
                JobOrderInspectionEvents Obj = new JobOrderInspectionEvents();
            }

            UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];

            //Log Initialization
            LogVm.SessionId = 0;
            LogVm.ControllerName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("controller");
            LogVm.ActionName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("action");
            LogVm.User = System.Web.HttpContext.Current.Request.RequestContext.HttpContext.User.Identity.Name;
        }

        public void PrepareViewBag(int id)
        {
            DocumentType DocType = new DocumentTypeService(_unitOfWork).Find(id);
            ViewBag.Name = DocType.DocumentTypeName;
            ViewBag.id = id;
            ViewBag.ReasonList = new ReasonService(_unitOfWork).GetReasonList(DocType.DocumentTypeName).ToList();

        }
        public ActionResult DocumentTypeIndex(int id)//DocumentCategoryId
        {
            var p = new DocumentTypeService(_unitOfWork).FindByDocumentCategory(id).ToList();

            if (p != null)
            {
                if (p.Count == 1)
                    return RedirectToAction("OrderInspectionWizard", new { id = p.FirstOrDefault().DocumentTypeId });
            }

            return View("DocumentTypeList", p);
        }

        public ActionResult OrderInspectionWizard(int id)//DocumentTypeId
        {
            PrepareViewBag(id);
            JobOrderInspectionHeaderViewModel vm = new JobOrderInspectionHeaderViewModel();
            vm.DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            vm.SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            vm.DocTypeId = id;

            //Getting Settings
            var settings = new JobOrderInspectionSettingsService(db).GetJobOrderInspectionSettingsForDocument(id, vm.DivisionId, vm.SiteId);

            if (settings == null && UserRoles.Contains("SysAdmin"))
            {
                return RedirectToAction("CreateJobOrderInspection", "JobOrderInspectionSettings", new { id = id }).Warning("Please create Purchase order cancel settings");
            }
            else if (settings == null && !UserRoles.Contains("SysAdmin"))
            {
                return View("~/Views/Shared/InValidSettings.cshtml");
            }
            vm.JobOrderInspectionSettings = Mapper.Map<JobOrderInspectionSettingsViewModel>(settings);
            ViewBag.ProcId = settings.ProcessId;
            vm.ProcessId = settings.ProcessId;

            int? JobWorkerId = new JobWorkerDbService(db).GetJobWorkerForUser(User.Identity.Name);

            if (JobWorkerId.HasValue && JobWorkerId.Value > 0)
            {
                vm.JobWorkerId = JobWorkerId.Value;
            }
            //vm.JobWorkerId = 33907;
            return View(vm);
        }

        public JsonResult AjaxGetJsonData(int DocType, DateTime? FromDate, DateTime? ToDate, string JobOrderHeaderId, string JobWorkerId
            , string ProductId, string Dimension1Id, string Dimension2Id, string ProductGroupId, string ProductCategoryId, decimal? BalanceQty, decimal Qty
            , string Sample)
        {
            string search = Request.Form["search[value]"];
            int sortColumn = -1;
            string sortDirection = "asc";
            string SortColName = "";


            // note: we only sort one column at a time
            if (Request.Form["order[0][column]"] != null)
            {
                sortColumn = int.Parse(Request.Form["order[0][column]"]);
                SortColName = Request.Form["columns[" + sortColumn + "][data]"];
            }
            if (Request.Form["order[0][dir]"] != null)
            {
                sortDirection = Request.Form["order[0][dir]"];
            }

            bool Success = true;

            int? JId = new JobWorkerDbService(db).GetJobWorkerForUser(User.Identity.Name);

            if (JId.HasValue && JId.Value > 0)
            {

                var data = FilterData(DocType, null, null, null, JId.ToString(),
                                               null, null, null, null, null, null, 0, null);

                var CList = data.ToList().Select(m => new JobOrderInspectionWizardViewModel
                {
                    SOrderDate = m.OrderDate.ToString("dd/MMM/yyyy"),
                    JobOrderLineId = m.JobOrderLineId,
                    OrderNo = m.OrderNo,
                    JobWorkerName = m.JobWorkerName,
                    ProductName = m.ProductName,
                    JobWorkerId = m.JobWorkerId,
                    Dimension1Name = m.Dimension1Name,
                    Dimension2Name = m.Dimension2Name,
                    BalanceQty = m.BalanceQty,
                    Qty = m.Qty,
                    InspectedLength = m.InspectedLength,
                    InspectedWidth = m.InspectedWidth,
                    InspectedUnit = m.InspectedUnit,
                    ProductUidName = m.ProductUidName,
                    ProductGroupName = m.ProductGroupName,
                    SizeName = m.SizeName
                }).ToList();

                return Json(new { Data = CList, Success = Success }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var data = FilterData(DocType, FromDate, ToDate, JobOrderHeaderId, JobWorkerId,
                                                ProductId, Dimension1Id, Dimension2Id, ProductGroupId, ProductCategoryId, BalanceQty, Qty, Sample);

                var RecCount = data.Count();

                if (RecCount > 1000 || RecCount == 0)
                {
                    Success = false;
                    return Json(new { Success = Success, Message = (RecCount > 1000 ? "No of records exceeding 1000." : "No Records found.") }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var CList = data.ToList().Select(m => new JobOrderInspectionWizardViewModel
                    {
                        SOrderDate = m.OrderDate.ToString("dd/MMM/yyyy"),
                        JobOrderLineId = m.JobOrderLineId,
                        OrderNo = m.OrderNo,
                        JobWorkerName = m.JobWorkerName,
                        ProductName = m.ProductName,
                        JobWorkerId = m.JobWorkerId,
                        Dimension1Name = m.Dimension1Name,
                        Dimension2Name = m.Dimension2Name,
                        BalanceQty = m.BalanceQty,
                        Qty = m.Qty,
                        InspectedLength = m.InspectedLength,
                        InspectedWidth = m.InspectedWidth,
                        InspectedUnit = m.InspectedUnit,
                        ProductUidName = m.ProductUidName,
                        ProductGroupName = m.ProductGroupName,
                        SizeName = m.SizeName
                    }).ToList();

                    return Json(new { Data = CList, Success = Success }, JsonRequestBehavior.AllowGet);
                }

            }

        }

        private int SortString(string s1, string s2, string sortDirection)
        {
            return sortDirection == "asc" ? s1.CompareTo(s2) : s2.CompareTo(s1);
        }

        private int SortInteger(string s1, string s2, string sortDirection)
        {
            int i1 = int.Parse(s1);
            int i2 = int.Parse(s2);
            return sortDirection == "asc" ? i1.CompareTo(i2) : i2.CompareTo(i1);
        }

        private int SortDateTime(string s1, string s2, string sortDirection)
        {
            DateTime d1 = DateTime.Parse(s1);
            DateTime d2 = DateTime.Parse(s2);
            return sortDirection == "asc" ? d1.CompareTo(d2) : d2.CompareTo(d1);
        }

        // here we simulate SQL search, sorting and paging operations
        private IQueryable<JobOrderInspectionWizardViewModel> FilterData(int DocType, DateTime? FromDate, DateTime? ToDate,
                                                                    string JobOrderHeaderId, string JobWorkerId, string ProductId, string Dimension1Id,
            string Dimension2Id, string ProductGroupId, string ProductCategoryId, decimal? BalanceQty, decimal Qty, string Sample)
        {

            List<int> JobOrderHeaderIds = new List<int>();
            if (!string.IsNullOrEmpty(JobOrderHeaderId))
                foreach (var item in JobOrderHeaderId.Split(','))
                    JobOrderHeaderIds.Add(Convert.ToInt32(item));

            List<int> JobWorkerIds = new List<int>();
            if (!string.IsNullOrEmpty(JobWorkerId))
                foreach (var item in JobWorkerId.Split(','))
                    JobWorkerIds.Add(Convert.ToInt32(item));

            List<int> Dimension1Ids = new List<int>();
            if (!string.IsNullOrEmpty(Dimension1Id))
                foreach (var item in Dimension1Id.Split(','))
                    Dimension1Ids.Add(Convert.ToInt32(item));

            List<int> Dimension2Ids = new List<int>();
            if (!string.IsNullOrEmpty(Dimension2Id))
                foreach (var item in Dimension2Id.Split(','))
                    Dimension2Ids.Add(Convert.ToInt32(item));

            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            var Settings = new JobOrderInspectionSettingsService(db).GetJobOrderInspectionSettingsForDocument(DocType, DivisionId, SiteId);

            string[] contraDocTypes = null;
            if (!string.IsNullOrEmpty(Settings.filterContraDocTypes)) { contraDocTypes = Settings.filterContraDocTypes.Split(",".ToCharArray()); }
            else { contraDocTypes = new string[] { "NA" }; }

            string[] contraSites = null;
            if (!string.IsNullOrEmpty(Settings.filterContraSites)) { contraSites = Settings.filterContraSites.Split(",".ToCharArray()); }
            else { contraSites = new string[] { "NA" }; }

            string[] contraDivisions = null;
            if (!string.IsNullOrEmpty(Settings.filterContraDivisions)) { contraDivisions = Settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { contraDivisions = new string[] { "NA" }; }

            IQueryable<JobOrderInspectionWizardViewModel> _data = from p in db.ViewJobOrderBalanceForInspection
                                                                         join t in db.JobOrderLine on p.JobOrderLineId equals t.JobOrderLineId
                                                                         join jrh in db.JobOrderHeader on p.JobOrderHeaderId equals jrh.JobOrderHeaderId
                                                                         join jw in db.Persons on p.JobWorkerId equals jw.PersonID into jwtable
                                                                         from jwtab in jwtable.DefaultIfEmpty()
                                                                         join prod in db.FinishedProduct on p.ProductId equals prod.ProductId into prodtable
                                                                         from prodtab in prodtable.DefaultIfEmpty()
                                                                          join dim1 in db.Dimension1 on t.Dimension1Id equals dim1.Dimension1Id into dimtable
                                                                          from dimtab in dimtable.DefaultIfEmpty()
                                                                          join dim2 in db.Dimension2 on t.Dimension2Id equals dim2.Dimension2Id into dim2table
                                                                          from dim2tab in dim2table.DefaultIfEmpty()
                                                                          join VRS in db.ViewRugSize on p.ProductId equals VRS.ProductId into VRStable
                                                                         from VRStab in VRStable.DefaultIfEmpty()
                                                                         join S in db.Size on VRStab.StandardSizeID equals S.SizeId into Stable
                                                                         from Stab in Stable.DefaultIfEmpty()
                                                                          join U in db.Units on Stab.UnitId equals U.UnitId into Utable
                                                                          from Utab in Utable.DefaultIfEmpty()
                                                                          join pg in db.ProductGroups on prodtab.ProductGroupId equals pg.ProductGroupId into pgtable
                                                                         from pgtab in pgtable.DefaultIfEmpty()
                                                                         join pc in db.ProductCategory on prodtab.ProductCategoryId equals pc.ProductCategoryId into pctable
                                                                         from pctab in pctable.DefaultIfEmpty()
                                                                         join ProdUid in db.ProductUid on p.ProductUidId equals ProdUid.ProductUIDId
                                                                         into produidtable
                                                                         from produidtab in produidtable.DefaultIfEmpty()
                                                                         where jrh.SiteId==SiteId && jrh.DivisionId==DivisionId && p.BalanceQty > 0 && jrh.ProcessId == Settings.ProcessId
                                                                         select new JobOrderInspectionWizardViewModel
                                                                      {
                                                                          OrderDate = p.OrderDate,
                                                                          OrderNo = p.JobOrderNo,
                                                                          JobOrderLineId = p.JobOrderLineId,
                                                                          BalanceQty = p.BalanceQty,
                                                                          Qty = Qty,
                                                                          InspectedLength = Stab.Length*(Decimal)Utab.FractionUnits+ Stab.LengthFraction,
                                                                          InspectedWidth = Stab.Width * (Decimal)Utab.FractionUnits + Stab.WidthFraction- (Decimal)p.InspectedWidth,
                                                                          InspectedUnit = Utab.FractionName,
                                                                          JobWorkerName = jwtab.Name,
                                                                          ProductName = prodtab.ProductName,
                                                                          JobOrderHeaderId = p.JobOrderHeaderId,
                                                                          JobWorkerId = p.JobWorkerId,
                                                                          ProductGroupId = pgtab.ProductGroupId,
                                                                          ProductGroupName = pgtab.ProductGroupName,
                                                                          SizeId = VRStab.StandardSizeID,
                                                                          SizeName = VRStab.StandardSizeName,
                                                                          Dimension1Name = dimtab.Dimension1Name,
                                                                          Dimension2Name = dim2tab.Dimension2Name,
                                                                          Dimension1Id = dimtab.Dimension1Id,
                                                                          Dimension2Id = dim2tab.Dimension2Id,
                                                                          ProductCategoryId = pctab.ProductCategoryId,
                                                                          ProductCategoryName = pctab.ProductCategoryName,
                                                                          ProdId = p.ProductId,
                                                                          UnitConversionMultiplier = t.UnitConversionMultiplier,
                                                                          DocTypeId = jrh.DocTypeId,
                                                                          ProductUidName = produidtab.ProductUidName
                                                                      };



            //if (FromDate.HasValue)
            //    _data = from p in _data
            //            where p.OrderDate >= FromDate
            //            select p;



            if (FromDate.HasValue)
                _data = _data.Where(m => m.OrderDate >= FromDate);

            if (ToDate.HasValue)
                _data = _data.Where(m => m.OrderDate <= ToDate);

            if (BalanceQty.HasValue && BalanceQty.Value > 0)
                _data = _data.Where(m => m.BalanceQty == BalanceQty.Value);

            if (!string.IsNullOrEmpty(JobOrderHeaderId))
                _data = _data.Where(m => JobOrderHeaderIds.Contains(m.JobOrderHeaderId));

            //if (!string.IsNullOrEmpty(JobWorkerId))
            //    _data = _data.Where(m => JobWorkerIds.Contains(m.JobWorkerId));

            if (!string.IsNullOrEmpty(ProductId))
                _data = _data.Where(m => m.ProductName.Contains(ProductId));

            if (!string.IsNullOrEmpty(Dimension1Id))
                _data = _data.Where(m => Dimension1Ids.Contains(m.Dimension1Id ?? 0));

            if (!string.IsNullOrEmpty(Dimension2Id))
                _data = _data.Where(m => Dimension2Ids.Contains(m.Dimension2Id ?? 0));

            if (!string.IsNullOrEmpty(ProductGroupId))
                _data = _data.Where(m => m.ProductGroupName.Contains(ProductGroupId));

            if (!string.IsNullOrEmpty(ProductCategoryId))
                _data = _data.Where(m => m.ProductCategoryName.Contains(ProductCategoryId));

            if (!string.IsNullOrEmpty(Settings.filterContraDocTypes))
                _data = _data.Where(m => contraDocTypes.Contains(m.DocTypeId.ToString()));

            //if (!string.IsNullOrEmpty(Settings.filterContraSites))
            //    _data = _data.Where(m => contraSites.Contains(m.SiteId.ToString()));
            //else
            //    _data = _data.Where(m => m.SiteId == SiteId);

            //if (!string.IsNullOrEmpty(Settings.filterContraDivisions))
            //    _data = _data.Where(m => contraDivisions.Contains(m.DivisionId.ToString()));
            //else
            //    _data = _data.Where(m => m.DivisionId == DivisionId);

            //if (!string.IsNullOrEmpty(Sample) && Sample != "Include")
            //{
            //    if (Sample == "Exclude")
            //        _data = _data.Where(m => m.Sample == false);
            //    else if (Sample == "Only")
            //        _data = _data.Where(m => m.Sample == true);
            //}

            _data = _data.OrderBy(m => m.OrderDate).ThenBy(m => m.OrderNo);

            // get just one page of data
            return _data.Select(m => new JobOrderInspectionWizardViewModel
            {
                OrderDate = m.OrderDate,
                OrderNo = m.OrderNo,
                JobOrderLineId = m.JobOrderLineId,
                BalanceQty = m.BalanceQty,
                Qty = Qty,
                InspectedLength = m.InspectedLength,
                InspectedWidth = m.InspectedWidth,
                InspectedUnit = m.InspectedUnit,
                JobWorkerName = m.JobWorkerName,
                ProductName = m.ProductName,
                JobOrderHeaderId = m.JobOrderHeaderId,
                JobWorkerId = m.JobWorkerId,
                ProductGroupId = m.ProductGroupId,
                ProductGroupName = m.ProductGroupName,
                SizeId = m.SizeId,
                SizeName = m.SizeName,
                Dimension1Name = m.Dimension1Name,
                Dimension2Name = m.Dimension2Name,
                Dimension1Id = m.Dimension1Id,
                Dimension2Id = m.Dimension2Id,
                ProductCategoryId = m.ProductCategoryId,
                ProductCategoryName = m.ProductCategoryName,
                ProdId = m.ProdId,
                UnitConversionMultiplier = m.UnitConversionMultiplier,
                DocTypeId = m.DocTypeId,
                ProductUidName = m.ProductUidName

            });
        }


        public ActionResult ConfirmedJobOrders(List<JobOrderInspectionWizardViewModel> ConfirmedList, int DocTypeId, string UserRemark)
        {

            if (ConfirmedList.Count() > 0 && ConfirmedList.GroupBy(m => m.JobWorkerId).Count() > 1)
                return Json(new { Success = false, Data = " Multiple Headers are selected. " }, JsonRequestBehavior.AllowGet);
            else if (ConfirmedList.Count() == 0)
                return Json(new { Success = false, Data = " No Records are selected. " }, JsonRequestBehavior.AllowGet);
            else
            {

                int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
                int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

                bool BeforeSave = true;
                int Serial = 1;
                List<JobOrderInspectionLineViewModel> LineStatus = new List<JobOrderInspectionLineViewModel>();

                try
                {
                    //BeforeSave = JobOrderInspectionDocEvents.beforeWizardSaveEvent(this, new JobEventArgs(0), ref db);
                }
                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    return Json(new { Success = false, Data = message }, JsonRequestBehavior.AllowGet);
                }


                if (!BeforeSave)
                    TempData["CSEXC"] += "Failed validation before save";


                int Cnt = 0;
                int Sr = 0;


                JobOrderInspectionSettings Settings = new JobOrderInspectionSettingsService(db).GetJobOrderInspectionSettingsForDocument(DocTypeId, DivisionId, SiteId);

                //int? MaxLineId = 0;

                //if (ModelState.IsValid && BeforeSave && !EventException)
                //{

                //    JobOrderInspectionHeader pt = new JobOrderInspectionHeader();

                //    //Getting Settings
                //    pt.SiteId = SiteId;
                //    pt.JobWorkerId = ConfirmedList.FirstOrDefault().JobWorkerId;
                //    pt.DivisionId = DivisionId;
                //    pt.ProcessId = Settings.ProcessId;
                //    pt.Remark = UserRemark;
                //    pt.DocTypeId = DocTypeId;
                //    pt.DocDate = DateTime.Now;
                //    pt.DocNo = new DocumentTypeService(_unitOfWork).FGetNewDocNo("DocNo", ConfigurationManager.AppSettings["DataBaseSchema"] + ".JobOrderInspectionHeaders", pt.DocTypeId, pt.DocDate, pt.DivisionId, pt.SiteId);
                //    pt.ModifiedBy = User.Identity.Name;
                //    pt.ModifiedDate = DateTime.Now;
                //    pt.CreatedBy = User.Identity.Name;
                //    pt.CreatedDate = DateTime.Now;

                //    pt.Status = (int)StatusConstants.Drafted;
                //    pt.ObjectState = Model.ObjectState.Added;

                //    db.JobOrderInspectionHeader.Add(pt);

                //    var SelectedJobOrders = ConfirmedList;

                //    var JobOrderLineIds = SelectedJobOrders.Select(m => m.JobOrderLineId).ToArray();

                //    var JobOrderBalanceRecords = (from p in db.ViewJobOrderBalanceForInspection
                //                                  where JobOrderLineIds.Contains(p.JobOrderLineId)
                //                                  select p).AsNoTracking().ToList();

                //    var JobOrderRecords = (from p in db.JobOrderLine
                //                           where JobOrderLineIds.Contains(p.JobOrderLineId)
                //                           select p).AsNoTracking().ToList();

                //    foreach (var item in SelectedJobOrders)
                //    {
                //        JobOrderLine Recline = JobOrderRecords.Where(m => m.JobOrderLineId == item.JobOrderLineId).FirstOrDefault();
                //        var balRecline = JobOrderBalanceRecords.Where(m => m.JobOrderLineId == item.JobOrderLineId).FirstOrDefault();

                //        if (item.Qty <= JobOrderBalanceRecords.Where(m => m.JobOrderLineId == item.JobOrderLineId).FirstOrDefault().BalanceQty)
                //        {
                //            JobOrderInspectionLine line = new JobOrderInspectionLine();

                //            line.JobOrderInspectionHeaderId = pt.JobOrderInspectionHeaderId;
                //            line.JobOrderLineId = item.JobOrderLineId;
                //            line.Qty = item.Qty;
                //            line.Remark = item.Remark;
                //            line.ProductUidId = Recline.ProductUidId;
                //            line.Sr = Serial++;
                //            line.JobOrderInspectionLineId = Cnt;
                //            line.CreatedDate = DateTime.Now;
                //            line.ModifiedDate = DateTime.Now;
                //            line.CreatedBy = User.Identity.Name;
                //            line.ModifiedBy = User.Identity.Name;
                //            LineStatus.Add(Mapper.Map<JobOrderInspectionLineViewModel>(line));

                //            line.ObjectState = Model.ObjectState.Added;
                //            db.JobOrderInspectionLine.Add(line);
                //            Cnt = Cnt + 1;

                //        }
                //    }

                //    //new JobOrderLineStatusService(_unitOfWork).UpdateJobOrderQtyQAMultiple(LineStatus, pt.DocDate, ref db);

                //    try
                //    {
                //        //JobOrderInspectionDocEvents.onWizardSaveEvent(this, new JobEventArgs(pt.JobOrderInspectionHeaderId, EventModeConstants.Add), ref db);
                //    }
                //    catch (Exception ex)
                //    {
                //        string message = _exception.HandleException(ex);
                //        TempData["CSEXC"] += message;
                //        EventException = true;
                //    }

                //    try
                //    {
                //        if (EventException)
                //        { throw new Exception(); }
                //        db.SaveChanges();
                //        //_unitOfWork.Save();
                //    }

                //    catch (Exception ex)
                //    {
                //        string message = _exception.HandleException(ex);
                //        return Json(new { Success = false, Data = message }, JsonRequestBehavior.AllowGet);
                //    }

                //    try
                //    {
                //        //JobOrderInspectionDocEvents.afterWizardSaveEvent(this, new JobEventArgs(pt.JobOrderInspectionHeaderId, EventModeConstants.Add), ref db);
                //    }
                //    catch (Exception ex)
                //    {
                //        string message = _exception.HandleException(ex);
                //        TempData["CSEXC"] += message;
                //    }

                //    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                //    {
                //        DocTypeId = pt.DocTypeId,
                //        DocId = pt.JobOrderInspectionHeaderId,                        
                //        ActivityType = (int)ActivityTypeContants.WizardCreate,                       
                //        DocNo = pt.DocNo,
                //        DocDate = pt.DocDate,
                //        DocStatus=pt.Status,
                //    }));

                //    return Json(new { Success = "URL", Data = "/JobOrderInspectionHeader/Submit/" + pt.JobOrderInspectionHeaderId }, JsonRequestBehavior.AllowGet);


                //}

                //else
                //    return Json(new { Success = false, Data = "ModelState is Invalid" }, JsonRequestBehavior.AllowGet);
                System.Web.HttpContext.Current.Session["WeavingOrderInspectionWizardJobOrder"] = ConfirmedList;

                return Json(new { Success = "URL", Data = "/Rug/WeavingOrderInspectionWizard/Create/" + DocTypeId.ToString() }, JsonRequestBehavior.AllowGet);

            }

        }
        
        public ActionResult Filters(int DocTypeId, DateTime? FromDate, DateTime? ToDate,
            string JobOrderHeaderId, string JobWorkerId, string ProductId, string Dimension1Id, string Dimension2Id, string ProductGroupId,
            string ProductCategoryId, decimal? BalanceQty, decimal Qty, string Sample)
        {
            JobOrderInspectionWizardFilterViewModel vm = new JobOrderInspectionWizardFilterViewModel();

            List<SelectListItem> tempSOD = new List<SelectListItem>();
            tempSOD.Add(new SelectListItem { Text = "Include Sample", Value = "Include" });
            tempSOD.Add(new SelectListItem { Text = "Exculde Sample", Value = "Exculde" });
            tempSOD.Add(new SelectListItem { Text = "Only Sample", Value = "Only" });

            ViewBag.SOD = new SelectList(tempSOD, "Value", "Text", Sample);


            vm.DocTypeId = DocTypeId;
            vm.FromDate = FromDate;
            vm.ToDate = ToDate;
            vm.JobOrderHeaderId = JobOrderHeaderId;
            vm.JobWorkerId = JobWorkerId;
            vm.ProductId = ProductId;
            vm.Dimension1Id = Dimension1Id;
            vm.Dimension2Id = Dimension2Id;
            vm.ProductGroupId = ProductGroupId;
            vm.ProductCategoryId = ProductCategoryId;
            vm.BalanceQty = BalanceQty;
            //vm.Qty = Qty;
            vm.Sample = Sample;
            return PartialView("_Filters", vm);
        }

        public ActionResult GetCustomPerson(string searchTerm, int pageSize, int pageNum, int filter)//DocTypeId
        {
            var Query = new JobOrderHeaderService(_unitOfWork).GetCustomPerson(filter, searchTerm);
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


        // GET: /JobOrderInspectionHeader/Create

        public ActionResult Create(int id)//DocumentTypeId
        {
            JobOrderInspectionHeaderViewModel p = new JobOrderInspectionHeaderViewModel();

            p.DocDate = DateTime.Now;
            p.CreatedDate = DateTime.Now;

            p.DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            p.SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];

            int DocTypeId = id;

            //Getting Settings
            var settings = new JobOrderInspectionSettingsService(db).GetJobOrderInspectionSettingsForDocument(DocTypeId, p.DivisionId, p.SiteId);

            if (settings == null && UserRoles.Contains("SysAdmin"))
            {
                return RedirectToAction("Create", "JobOrderInspectionSettings", new { id = DocTypeId }).Warning("Please create job order settings");
            }
            else if (settings == null && !UserRoles.Contains("SysAdmin"))
            {
                return View("~/Views/Shared/InValidSettings.cshtml");
            }
            p.JobOrderInspectionSettings = Mapper.Map<JobOrderInspectionSettings, JobOrderInspectionSettingsViewModel>(settings);

            if (System.Web.HttpContext.Current.Session["WeavingOrderInspectionWizardJobOrder"] == null)
                return Redirect(System.Configuration.ConfigurationManager.AppSettings["JobsDomain"] + "/JobOrderInspectionHeader/Index/" + DocTypeId);

            var JobOrderLineList = ((List<JobOrderInspectionWizardViewModel>)System.Web.HttpContext.Current.Session["WeavingOrderInspectionWizardJobOrder"]).FirstOrDefault();

            int JobOrderLineId = JobOrderLineList.JobOrderLineId;

            //var DesignPatternId = (from pol in db.JobOrderLine
            //                       where pol.ProdOrderLineId == ProdOrderLineId
            //                       join t in db.FinishedProduct on pol.ProductId equals t.ProductId
            //                       select t.ProductDesignPatternId).FirstOrDefault();



            //p.InspectionById = new EmployeeService(_unitOfWork).GetEmloyeeForUser(User.Identity.GetUserId());
            p.ProcessId = settings.ProcessId;
            //PrepareViewBag();
            p.DocTypeId = DocTypeId;
            p.DocNo = new DocumentTypeService(_unitOfWork).FGetNewDocNo("DocNo", ConfigurationManager.AppSettings["DataBaseSchema"] + ".JobOrderInspectionHeaders", p.DocTypeId, p.DocDate, p.DivisionId, p.SiteId);





            ////RatesFetching from RateList Master
            //var Lines = (List<WeavingOrderWizardViewModel>)System.Web.HttpContext.Current.Session["BarCodesWeavingWizardProdOrder"];

            //var DesignName = Lines.FirstOrDefault().DesignName;
            //var ProductGroupId = new ProductGroupService(_unitOfWork).Find(DesignName).ProductGroupId;

            //var RateListLine = new RateListLineService(_unitOfWork).GetRateListForDesign(ProductGroupId, settings.ProcessId);


            return View(p);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Post(JobOrderInspectionHeaderViewModel svm)
        {
            bool TimePlanValidation = true;
            string ExceptionMsg = "";
            bool Continue = true;
            DateTime LastPODate;
            JobOrderInspectionHeader s = Mapper.Map<JobOrderInspectionHeaderViewModel, JobOrderInspectionHeader>(svm);

            List<JobOrderInspectionWizardViewModel> JobOrderList = (List<JobOrderInspectionWizardViewModel>)System.Web.HttpContext.Current.Session["WeavingOrderInspectionWizardJobOrder"];

            var ListPODate = (from p in JobOrderList
                              join L in db.JobOrderLine on p.JobOrderLineId equals L.JobOrderLineId
                              join H in db.JobOrderHeader on L.JobOrderHeaderId equals H.JobOrderHeaderId
                              select new
                              {
                                  DocDate = H.DocDate,
                              }).ToList();

            LastPODate = ListPODate.Max(m => m.DocDate);

            if (svm.JobOrderInspectionSettings != null)
            {
                //if (svm.JobOrderInspectionSettings.isMandatoryCostCenter == true && (string.IsNullOrEmpty(svm.CostCenterName)))
                //{
                //    ModelState.AddModelError("CostCenterName", "The CostCenter field is required");
                //}
                //if (svm.JobOrderInspectionSettings.isMandatoryMachine == true && (svm.MachineId <= 0 || svm.MachineId == null))
                //{
                //    ModelState.AddModelError("MachineId", "The Machine field is required");
                //}
                //if (svm.JobOrderInspectionSettings.isVisibleGodown && svm.JobOrderInspectionSettings.isMandatoryGodown && !svm.GodownId.HasValue)
                //{
                //    ModelState.AddModelError("GodownId", "The Godown field is required");
                //}
                //if (svm.JobOrderInspectionSettings.MaxDays.HasValue && (svm.DueDate - svm.DocDate).Days > svm.JobOrderInspectionSettings.MaxDays.Value)
                //{
                //    ModelState.AddModelError("DueDate", "DueDate is exceeding MaxDueDays.");
                //}
            }

            if (svm.DocDate < LastPODate)
            {
                ModelState.AddModelError("DocDate", "Doc Date should not be less than " + LastPODate.ToString());
            }

            //if (svm.DueDate < svm.DocDate)
            //{
            //    ModelState.AddModelError("DueDate", "DueDate should not be less than " + svm.DocDate.ToString());
            //}

            //if (svm.Rate <= 0 && svm.JobOrderInspectionSettings.isMandatoryRate)
            //    ModelState.AddModelError("Rate", "Rate field is required");

            if (JobOrderList.Count() <= 0)
                ModelState.AddModelError("", "No Records Selected");

            List<JobOrderInspectionLine> BarCodesToUpdate = new List<JobOrderInspectionLine>();


            #region DocTypeTimeLineValidation

            try
            {

                //if (svm.JobOrderInspectionHeaderId <= 0)
                //    TimePlanValidation = DocumentValidation.ValidateDocument(Mapper.Map<DocumentUniqueId>(svm), DocumentTimePlanTypeConstants.Create, User.Identity.Name, out ExceptionMsg, out Continue);
                //else
                //    TimePlanValidation = DocumentValidation.ValidateDocument(Mapper.Map<DocumentUniqueId>(svm), DocumentTimePlanTypeConstants.Modify, User.Identity.Name, out ExceptionMsg, out Continue);

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

                if (svm.JobOrderInspectionHeaderId <= 0)
                {


                    if (JobOrderList.Count() > 0)
                    {
                        s.CreatedDate = DateTime.Now;
                        s.ModifiedDate = DateTime.Now;
                        s.CreatedBy = User.Identity.Name;
                        s.ModifiedBy = User.Identity.Name;
                        s.Status = (int)StatusConstants.Drafted;
                        _JobOrderInspectionHeaderService.Create(s);

                        int Cnt = 0;
                        int Sr = 0;
                        int pk = 0;

                        JobOrderInspectionSettings Settings = new JobOrderInspectionSettingsService(db).GetJobOrderInspectionSettingsForDocument(s.DocTypeId, s.DivisionId, s.SiteId);


                        int PersonCount = 0;
                        //if (!Settings.CalculationId.HasValue)
                        //{
                        //    throw new Exception("Calculation not configured in Job order settings");
                        //}



                        List<LineDetailListViewModel> LineList = new List<LineDetailListViewModel>();

                        var JobOrderLineIds = JobOrderList.Select(m => m.JobOrderLineId).ToArray();

                        var BalQtyandUnits = (from p in db.ViewJobOrderBalance
                                              join t in db.Product on p.ProductId equals t.ProductId
                                              where JobOrderLineIds.Contains(p.JobOrderLineId)
                                              select new
                                              {
                                                  BalQty = p.BalanceQty,
                                                  JobOrderLineId = p.JobOrderLineId,
                                                  UnitId = t.UnitId,
                                              }).ToList();

                        if (ModelState.IsValid)
                        {

                            decimal OrderQty = 0;
                            decimal OrderDealQty = 0;
                            decimal BomQty = 0;

                            foreach (var SelectedJobOrderLine in JobOrderList)
                            {
                                //if (SelectedJobOrderLine.ProdOrderLineId > 0 && !SelectedJobOrderLine.RefDocLineId.HasValue)
                                if (SelectedJobOrderLine.JobOrderLineId > 0 )
                                {
                                    var JobOrderLine = new JobOrderLineService(_unitOfWork).Find((SelectedJobOrderLine.JobOrderLineId));
                                    var Product = new ProductService(_unitOfWork).Find(JobOrderLine.ProductId);

                                    //decimal balQty = (from p in db.ViewProdOrderBalance
                                    //                  where p.ProdOrderLineId == SelectedJobOrderLine.ProdOrderLineId
                                    //                  select p.BalanceQty).FirstOrDefault();

                                    var bal = BalQtyandUnits.Where(m => m.JobOrderLineId == SelectedJobOrderLine.JobOrderLineId).FirstOrDefault();

                                    //if (item.Qty > 0 &&  ((Settings.isMandatoryRate.HasValue && Settings.isMandatoryRate == true )? item.Rate > 0 : 1 == 1))
                                    //if (SelectedJobOrderLine.Qty <= bal.BalQty)
                                    if (bal.BalQty == bal.BalQty)
                                    {

                                        List<JobOrderInspectionLine> LIL = db.JobOrderInspectionLine.Where(m => m.JobOrderLineId == JobOrderLine.JobOrderLineId).ToList();
                                        Decimal LastMark = (Decimal) LIL.Sum(m => m.InspectedWidth);
                                        JobOrderInspectionLine line = new JobOrderInspectionLine();
                                        
                                        line.JobOrderInspectionHeaderId = s.JobOrderInspectionHeaderId;
                                        line.JobOrderLineId = JobOrderLine.JobOrderLineId;
                                        line.ProductUidId = JobOrderLine.ProductUidId;
                                        line.Qty = JobOrderLine.Qty;
                                        line.InspectedQty = JobOrderLine.Qty;
                                        line.InspectedLength = SelectedJobOrderLine.InspectedLength;
                                        line.InspectedWidth = SelectedJobOrderLine.InspectedWidth;
                                        line.InspectedUnitId = SelectedJobOrderLine.InspectedUnit;
                                        line.Remark = SelectedJobOrderLine.Remark;
                                        

                                        if (LIL !=null)
                                            line.Marks = SelectedJobOrderLine.InspectedWidth+ LastMark;
                                        else
                                            line.Marks = SelectedJobOrderLine.InspectedWidth;

                                        //line.Rate = SelectedJobOrderLine.Rate > 0 ? SelectedJobOrderLine.Rate : svm.Rate;
                                        ////line.LossQty = svm.Loss;
                                        //line.LossQty = SelectedJobOrderLine.Loss > 0 ? SelectedJobOrderLine.Loss : svm.Loss;
                                        //line.NonCountedQty = svm.UnCountedQty;

                                        //if (DealUnit == "PCS")
                                        //    line.UnitConversionMultiplier = 1;
                                        //else
                                        //    line.UnitConversionMultiplier = Math.Round(uc.Multiplier, Unit.DecimalPlaces);

                                        //line.DealQty = SelectedJobOrderLine.Qty * line.UnitConversionMultiplier;
                                        //line.UnitId = bal.UnitId;
                                        //line.DealUnitId = DealUnit;
                                        //line.Amount = (line.DealQty * line.Rate);
                                        line.Sr = Sr++;
                                        line.CreatedDate = DateTime.Now;
                                        line.ModifiedDate = DateTime.Now;
                                        line.CreatedBy = User.Identity.Name;
                                        line.ModifiedBy = User.Identity.Name;
                                        line.JobOrderInspectionLineId = pk;
                                        line.ObjectState = Model.ObjectState.Added;
                                        new JobOrderInspectionLineService(db).Create(line);

                                        //new JobOrderInspectionLineStatusService(_unitOfWork).CreateLineStatus(line.JobOrderInspectionLineId, ref db, false);



                                        //LineList.Add(new LineDetailListViewModel { Amount = line.Amount, Rate = line.Rate, LineTableId = line.JobOrderInspectionLineId, HeaderTableId = s.JobOrderInspectionHeaderId, PersonID = s.JobWorkerId, DealQty = line.DealQty, Incentive = SelectedJobOrderLine.Incentive });

                                        pk++;
                                        Cnt = Cnt + 1;



                                    }

                                }


                            }

            




                        }
                        string Errormessage = "";
                        try
                        {
                            db.SaveChanges();
                            _unitOfWork.Save();
                        }

                        catch (Exception ex)
                        {
                            Errormessage = _exception.HandleException(ex);
                            ModelState.AddModelError("", Errormessage);
                            //PrepareViewBag();
                            ViewBag.Mode = "Add";
                            return View("Create", svm);

                        }



                        LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                        {
                            DocTypeId = s.DocTypeId,
                            DocId = s.JobOrderInspectionHeaderId,
                            ActivityType = (int)ActivityTypeContants.WizardCreate,
                            DocNo = s.DocNo,
                            DocDate = s.DocDate,
                            DocStatus = s.Status,
                        }));

                        System.Web.HttpContext.Current.Session.Remove("WeavingOrderInspectionWizardJobOrder");

                        return Redirect(System.Configuration.ConfigurationManager.AppSettings["JobsDomain"] + "/Rug/WeavingOrderInspectionHeader/Submit/" + s.JobOrderInspectionHeaderId);
                    }
                    else
                    {
                        return Redirect(System.Configuration.ConfigurationManager.AppSettings["JobsDomain"] + "/Rug/WeavingOrderInspectionHeader/Index/" + s.DocTypeId);
                    }

                }
                else
                {

                }

            }
            //PrepareViewBag();
            ViewBag.Mode = "Add";
            //return Redirect(System.Configuration.ConfigurationManager.AppSettings["JobsDomain"] + "/JobOrderInspectionHeader/Submit/"+s.JobOrderInspectionHeaderId);
            return View("Create", svm);
        }


        public JsonResult GetPendingJobOrdersHelpList(string searchTerm, int pageSize, int pageNum, int filter)//Order Header ID
        {

            var Records = new JobOrderInspectionRequestLineService(db).GetPendingJobOrdersForWizardFilters(searchTerm, filter);

            var temp = Records.Skip(pageSize * (pageNum - 1)).Take(pageSize).ToList();

            var count = Records.Count();

            ComboBoxPagedResult Data = new ComboBoxPagedResult();
            Data.Results = temp;
            Data.Total = count;

            return new JsonpResult
            {
                Data = Data,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

        }

        public JsonResult GetPendingJobWorkerHelpList(string searchTerm, int pageSize, int pageNum, int filter)//Order Header ID
        {
            var Records = new JobOrderInspectionRequestLineService(db).GetPendingJobWorkerHelpList(searchTerm, filter);

            var temp = Records.Skip(pageSize * (pageNum - 1)).Take(pageSize).ToList();

            var count = Records.Count();

            ComboBoxPagedResult Data = new ComboBoxPagedResult();
            Data.Results = temp;
            Data.Total = count;

            return new JsonpResult
            {
                Data = Data,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

        }

        public JsonResult GetPendingProductHelpList(string searchTerm, int pageSize, int pageNum, int filter)//Order Header ID
        {
            var Records = new JobOrderInspectionRequestLineService(db).GetPendingProductHelpList(searchTerm, filter);

            var temp = Records.Skip(pageSize * (pageNum - 1)).Take(pageSize).ToList();

            var count = Records.Count();

            ComboBoxPagedResult Data = new ComboBoxPagedResult();
            Data.Results = temp;
            Data.Total = count;

            return new JsonpResult
            {
                Data = Data,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

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


}
