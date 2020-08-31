using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Data.Models;
using Model.ViewModels;
using Service;
using Data.Infrastructure;
using System.Xml.Linq;
using Core.Common;
using Model.ViewModel;
using System;

namespace Jobs.Controllers
{
    [Authorize]
    public class JobOrderLineStatusUpdateWizardController : System.Web.Mvc.Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        ActiivtyLogViewModel LogVm = new ActiivtyLogViewModel();

        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;

        public JobOrderLineStatusUpdateWizardController(IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _exception = exec;
            _unitOfWork = unitOfWork;

            //Log Initialization
            LogVm.SessionId = 0;
            LogVm.ControllerName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("controller");
            LogVm.ActionName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("action");
            LogVm.User = System.Web.HttpContext.Current.Request.RequestContext.HttpContext.User.Identity.Name;
        }

        public ActionResult RateListHeaderIndex(int MenuId)
        {
            var RateListHeader = new RateListHeaderService(_unitOfWork).GetProductRateListHeader(MenuId);
            return View(RateListHeader);
        }


        public ActionResult DocumentTypeIndex(int id)//DocumentCategoryId
        {
            var p = new DocumentTypeService(_unitOfWork).FindByDocumentCategory(id).ToList();

            if (p != null)
            {
                if (p.Count == 1)
                    return RedirectToAction("JobOrderLineStatusUpdate", new { id = p.FirstOrDefault().DocumentTypeId });
            }

            return View("DocumentTypeList", p);
        }

        public ActionResult JobOrderLineStatusUpdate(int Id)
        {
            ViewBag.Id = Id;
            ViewBag.Name = "Status Update -" + new DocumentTypeService(_unitOfWork).Find(Id).DocumentTypeName;
            ViewBag.WizardType = "Pending To Production";

            return View();
        }


        private static int TOTAL_ROWS = 0;



        public JsonResult AjaxGetJsonProductData(int draw, int start, int length, FilterJobOrderLineStatusArgs Fvm)
        {
            string search = Request.Form["search[value]"];
            int sortColumn = -1;
            string sortDirection = "asc";
            string SortColName = "";
            if (length == -1)
            {
                length = TOTAL_ROWS;
            }

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

            DataTableProductData dataTableData = new DataTableProductData();
            dataTableData.draw = draw;
            int recordsFiltered = 0;
            dataTableData.data = FilterProductData(ref recordsFiltered, ref TOTAL_ROWS, start, length, search, sortColumn, sortDirection, Fvm);
            dataTableData.recordsTotal = TOTAL_ROWS;
            dataTableData.recordsFiltered = recordsFiltered;

            return Json(dataTableData, JsonRequestBehavior.AllowGet);
        }

        public class DataTableProductData
        {
            public int draw { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public List<JobOrderLineStatusViewModel> data { get; set; }
        }


        public class JobOrderLineStatusViewModel : JobOrderLineViewModel
        {
            public string JobWorkerName { get; set; }
            public string ProductionNo { get; set; }
            public string MyDate { get; set; }
            public string ProductionDate { get; set; }
            public string DispatchNo { get; set; }
            public string DispatchDate { get; set; }
        }
            // here we simulate SQL search, sorting and paging operations
            private List<JobOrderLineStatusViewModel> FilterProductData(ref int recordFiltered, ref int recordTotal, int start, int length, string search, int sortColumn, string sortDirection, FilterJobOrderLineStatusArgs Fvm)
        {



            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];


            List<JobOrderLineStatusViewModel> list = new List<JobOrderLineStatusViewModel>();

            string mQry;

            mQry = @"	    SELECT L.JobOrderLineId,  JOH.DocNo AS JobOrderHeaderDocNo, Replace(convert(NVARCHAR,JOH.DocDate,106),' ','/') JobOrderHeaderDocDate, JW.Name AS JobWorkerName, POH.DocNo AS ProdOrderDocNo,
                            P.ProductName, D1.Dimension1Name, D2.Dimension2Name, H.BalanceQty  AS Qty ,  
                            VS.ProductionNo, VS.ProductionDate AS ProductionDate,
                            VS.DispatchNo, VS.DispatchDate AS DispatchDate
                            FROM web.ViewJobOrderBalance H WITH (Nolock)
                            LEFT JOIN web.JobOrderLines L WITH (Nolock) ON L.JobOrderLineId = H.JobOrderLineId 
                            LEFT JOIN web.ProdOrderLines POL WITH (Nolock) ON L.ProdOrderLineId = POL.ProdOrderLineId
                            LEFT JOIN web.ProdOrderHeaders POH WITH (Nolock) ON POH.ProdOrderHeaderId = POL.ProdOrderHeaderId
                            LEFT JOIN web.JobOrderHeaders JOH WITH (Nolock) ON JOH.JobOrderHeaderId = L.JobOrderHeaderId 
                            LEFT JOIN web.Products P WITH (Nolock) ON P.ProductId = L.ProductId 
                            LEFT JOIN web.People JW WITH (Nolock) ON JW.PersonID = JOH.JobWorkerId 
                            LEFT JOIN web.Dimension1 D1 WITH (Nolock) ON D1.Dimension1Id = L.Dimension1Id 
                            LEFT JOIN web.Dimension2 D2 WITH (Nolock) ON D2.Dimension2Id = L.Dimension2Id
                            LEFT JOIN 
                            (
                            SELECT C.HeaderTableId, C.DocTypeId, C.LineTableId,
							Max(CASE WHEN DA.Name ='Production No' THEN C.Value ELSE NULL END) AS ProductionNo,
							Max(CASE WHEN DA.Name ='Production Date' THEN C.Value ELSE NULL END) AS ProductionDate,
                            Max(CASE WHEN DA.Name ='Dispatch No' THEN C.Value ELSE NULL END) AS DispatchNo,
							Max(CASE WHEN DA.Name ='Dispatch Date' THEN C.Value ELSE NULL END) AS DispatchDate
							FROM web.CustomLineAttributes C WITH (Nolock)
							LEFT JOIN web.DocumentTypeHeaderAttributes DA WITH (Nolock) ON DA.DocumentTypeHeaderAttributeId = C.DocumentTypeHeaderAttributeId 
							GROUP BY C.HeaderTableId, C.DocTypeId, C.LineTableId 
                            ) VS ON VS.HeaderTableId = JOH.JobOrderHeaderId AND VS.DocTypeId = JOH.DocTypeId AND VS.LineTableId = L.JobOrderLineId 
                            WHERE JOH.DocTypeId = " + Fvm.DocumentTypeId.ToString() + @"
							AND JOH.SiteId = " + SiteId.ToString() + @" 
                            AND JOH.DivisionId =" + DivisionId.ToString() + @" ";


            if (string.IsNullOrEmpty(Fvm.WizardType) || Fvm.WizardType == "Pending To Production")
                mQry = mQry + " AND  VS.ProductionNo is Null";
            else if (string.IsNullOrEmpty(Fvm.WizardType) || Fvm.WizardType == "All Production")
                mQry = mQry + " AND  VS.ProductionNo is Not Null ";
            else if (string.IsNullOrEmpty(Fvm.WizardType) || Fvm.WizardType == "Pending To Dispatch")
                mQry = mQry + " AND  VS.ProductionNo is Not Null AND  VS.DispatchNo is Null ";
            else if (string.IsNullOrEmpty(Fvm.WizardType) || Fvm.WizardType == "All Dispatch")
                mQry = mQry + " AND  VS.DispatchNo is Not Null ";

            IEnumerable<JobOrderLineStatusViewModel> _data1 = db.Database.SqlQuery<JobOrderLineStatusViewModel>(mQry).ToList();

            IQueryable<JobOrderLineStatusViewModel> _data = _data1.AsQueryable(); 
            recordTotal = _data.Count();

            if (string.IsNullOrEmpty(search))
            {

            }
            else
            {

                // simulate search
                _data = from m in _data
                        where (m.ProductName).ToLower().Contains(search.ToLower()) || (m.Dimension1Name).ToLower().Contains(search.ToLower())
                        || (m.JobOrderHeaderDocNo).ToLower().Contains(search.ToLower()) || (m.JobWorkerName).ToLower().Contains(search.ToLower())
                        select m;

            }

            _data = _data.OrderBy(m => m.ProductName);


            recordFiltered = _data.Count();

            // get just one page of data
            list = _data.Select(m => new JobOrderLineStatusViewModel
            {
                JobOrderLineId = m.JobOrderLineId,
                JobWorkerName = m.JobWorkerName,
                JobOrderHeaderDocNo = m.JobOrderHeaderDocNo,
                JobOrderHeaderDocDate = m.JobOrderHeaderDocDate,
                ProdOrderDocNo = m.ProdOrderDocNo,
                ProductName = m.ProductName,
                Dimension1Name = m.Dimension1Name,
                Dimension2Name = m.Dimension2Name,
                Dimension3Name = m.Dimension3Name,
                Dimension4Name = m.Dimension4Name,
                Qty = m.Qty,
                ProductionNo = (Fvm.WizardType.Contains("Production") ? m.ProductionNo : m.DispatchNo ),
                ProductionDate = (Fvm.WizardType.Contains("Production") ? m.ProductionDate : m.DispatchDate),
            })
            .Skip(start).Take((start == 0) ? 90 : length).ToList();

            return list;
        }



        public ActionResult UpdateCustomLineAttributes(int JobOrderLineId, string Attribute, string AttributeValue)
        {
            XElement Modifications;
            bool Flag = new JobOrderLineService(_unitOfWork).UpdateJobOrderLineStatus(JobOrderLineId, Attribute, AttributeValue, User.Identity.Name, out Modifications);

            if (Flag)
            {
                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.RateListHeader).DocumentTypeId,
                    ActivityType = (int)ActivityTypeContants.Modified,
                    DocId = JobOrderLineId,
                    xEModifications = Modifications,
                }));
            }

            return Json(new { Success = Flag });

            //return View();
        }



        public ActionResult Filters(FilterJobOrderLineStatusArgs Fvm)
        {
            List<SelectListItem> temp = new List<SelectListItem>();
            temp.Add(new SelectListItem { Text = "Pending To Production", Value = "Pending To Production" });
            temp.Add(new SelectListItem { Text = "All Production", Value = "All Production" });
            temp.Add(new SelectListItem { Text = "Pending To Dispatch", Value = "Pending To Dispatch" });
            temp.Add(new SelectListItem { Text = "All Dispatch", Value = "All Dispatch" });

            ViewBag.WizType = new SelectList(temp, "Value", "Text", Fvm.WizardType);

            return PartialView("_Filters", Fvm);
        }


        //[HttpPost]
        //public ActionResult _FilterPost(FilterJobOrderLineStatusArgs vm)
        //{         
        // return RedirectToAction("JobOrderLineStatusUpdate", new { Id=vm.DocumentTypeId, WizardType=vm.WizardType });
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    public class FilterJobOrderLineStatusArgs
    {
        public string WizardType { get; set; }
        public int? DocumentTypeId { get; set; }
        public string JobWorker { get; set; }
        public string ProductName { get; set; }

    }

}
