﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Data.Models;
using Model.ViewModels;
using Service;
using Data.Infrastructure;
using Core.Common;
using System.Xml.Linq;
using Model.ViewModel;



namespace Jobs.Areas.Rug.Controllers
{
    [Authorize]
    public class WeavingDesignRatesController : System.Web.Mvc.Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        ActiivtyLogViewModel LogVm = new ActiivtyLogViewModel();

        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;

        public WeavingDesignRatesController(IUnitOfWork unitOfWork, IExceptionHandlingService exec)
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
            var RateListHeader = new RateListHeaderService(_unitOfWork).GetDesignRateListHeader(MenuId);
            return View(RateListHeader);
        }


        public ActionResult DesignRateList(int Id)
        {
            ViewBag.RateListHeaderId = Id;
            ViewBag.Name = "Weaving Rates -" + db.RateListHeader.Find(Id).RateListName;
            ViewBag.WizardType = "Pending";
            ViewBag.SOD = "Design";
            return View();
        }


        private static int TOTAL_ROWS = 0;



        public JsonResult AjaxGetJsonProductDesignData(int draw, int start, int length, FilterArgs Fvm)
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
            public List<ProductViewModel> data { get; set; }
        }



        // here we simulate SQL search, sorting and paging operations
        private List<ProductViewModel> FilterProductData(ref int recordFiltered, ref int recordTotal, int start, int length, string search, int sortColumn, string sortDirection, FilterArgs Fvm)
        {
            bool Pending = false;
            bool Sample = false;
            bool All = false;

            if (string.IsNullOrEmpty(Fvm.WizardType) || Fvm.WizardType == "Pending")
                Pending = true;

            if (!string.IsNullOrEmpty(Fvm.SOD) && Fvm.SOD == "Sample")
                Sample = true;
            else if (!string.IsNullOrEmpty(Fvm.SOD) && Fvm.SOD == "Design")
                Sample = false;
            else if (!string.IsNullOrEmpty(Fvm.SOD) && Fvm.SOD == "All")
                All = true;

            string[] Collections = null;
            if (!string.IsNullOrEmpty(Fvm.ProductCollection)) { Collections = Fvm.ProductCollection.Split(",".ToCharArray()); }
            else { Collections = new string[] { "NA" }; }

            string[] Category = null;
            if (!string.IsNullOrEmpty(Fvm.ProductCategory)) { Category = Fvm.ProductCategory.Split(",".ToCharArray()); }
            else { Category = new string[] { "NA" }; }

            
            List<ProductViewModel> list = new List<ProductViewModel>();
            IQueryable<ProductViewModel> _data = (from p in db.ProductProcess
                                                  join rh in db.RateListHeader on p.ProcessId equals rh.ProcessId
                                                  join fp in db.FinishedProduct on p.ProductId equals fp.ProductId
                                                  join pcol in db.ProductCollections on fp.ProductCollectionId equals pcol.ProductCollectionId
                                                  join PQ in db.ProductQuality on fp.ProductQualityId equals PQ.ProductQualityId
                                                  join pcat in db.ProductCategory on fp.ProductCategoryId equals pcat.ProductCategoryId
                                                  join pg in db.ProductGroups on fp.ProductGroupId equals pg.ProductGroupId
                                                  join pd in db.ProductDesigns on fp.ProductDesignId equals pd.ProductDesignId
                                                  join rll in db.RateListLine.Where(i => i.ProductId != null) on new { x = p.ProductId, y = rh.RateListHeaderId }
                                                  equals new { x = (int)rll.ProductId, y = rll.RateListHeaderId } into rlltable
                                                  from rlltab in rlltable.DefaultIfEmpty()
                                                  where rh.RateListHeaderId == Fvm.RateListHeaderId && (Pending ? rlltab == null : 1 == 1) && (All ? 1 == 1 : fp.IsSample == Sample)
                                                  where rh.RateListHeaderId == Fvm.RateListHeaderId && (Pending ? rlltab == null : 1 == 1) && (All ? 1 == 1 : fp.IsSample == Sample)
                                                  && (string.IsNullOrEmpty(Fvm.ProductCollection) ? 1 == 1 : Collections.Contains(fp.ProductCollectionId.ToString()))
                                                  && (string.IsNullOrEmpty(Fvm.ProductCategory) ? 1 == 1 : Category.Contains(fp.ProductCategoryId.ToString()))
                                                  && rh.DivisionId == fp.DivisionId && fp.IsActive == true
                                                   && (Fvm.DisContinued == "All" ? 1 == 1 : fp.DiscontinuedDate == null)
                                                  group new { pg, fp, rlltab, pcol, pcat, pd, PQ } by pg.ProductGroupId into g
                                                  select new ProductViewModel
                                                  {
                                                      ProductGroupId = g.Key,
                                                      ProductGroupName = g.Max(m => m.pg.ProductGroupName),
                                                      ProductDesignName = g.Max(m => m.pd.ProductDesignName),
                                                      ProductQualityName = g.Max(m => m.PQ.ProductQualityName),
                                                      ImageFileName = g.Max(m => m.pg.ImageFileName),
                                                      ImageFolderName = g.Max(m => m.pg.ImageFolderName),
                                                      SampleName = g.Max(m => m.fp.IsSample.ToString() == "True" ? "Sample" : "Design"),
                                                      Rate = g.Max(m => m.rlltab.Rate),
                                                      Loss = g.Max(m => m.rlltab.Loss),
                                                      Weight = g.Max(m => m.rlltab.UnCountedQty),
                                                      ProductCollectionName = g.Max(m => m.pcol.ProductCollectionName),
                                                      ProductCategoryName = g.Max(m => m.pcat.ProductCategoryName),
                                                  });


            recordTotal = _data.Count();

            if (string.IsNullOrEmpty(search))
            {

            }
            else
            {

                // simulate search
                _data = from m in _data
                        where (m.ProductGroupName).ToLower().Contains(search.ToLower()) || (m.ProductDesignName).ToLower().Contains(search.ToLower())
                        || (m.SampleName).ToLower().Contains(search.ToLower()) || (m.ProductCollectionName).ToLower().Contains(search.ToLower())
                        || (m.ProductCategoryName).ToLower().Contains(search.ToLower())
                        select m;

            }

            _data = _data.OrderBy(m => m.ProductGroupName);


            recordFiltered = _data.Count();

            // get just one page of data
            list = _data.Select(m => new ProductViewModel
            {
                ProductGroupId = m.ProductGroupId,
                ProductGroupName = m.ProductGroupName,
                ProductDesignName = m.ProductDesignName,
                ProductQualityName = m.ProductQualityName,
                ImageFileName = m.ImageFileName,
                ImageFolderName = m.ImageFolderName,
                SampleName = m.SampleName,
                Rate = m.Rate,
                Loss = m.Loss,
                Weight = m.Weight,
                ProductCollectionName = m.ProductCollectionName,
                ProductCategoryName = m.ProductCategoryName,
            })
            .Skip(start).Take((start == 0) ? 90 : length).ToList();

            return list;
        }



        public ActionResult UpdateDesignRate(int ProductGroupId, decimal Rate, decimal Loss, decimal Weight, int RateListHeaderId)
        {
            XElement Modifications;
            bool Flag = new RateListLineService(_unitOfWork).UpdateRateListLineForDesign(ProductGroupId, Rate, Loss, Weight, RateListHeaderId, User.Identity.Name, out Modifications);

            if (Flag)
            {
                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.RateListHeader).DocumentTypeId,
                    ActivityType = (int)ActivityTypeContants.Modified,
                    xEModifications = Modifications,
                }));            
            }

            return Json(new { Success = Flag });

            //return View();
        }

        public ActionResult Filters(FilterArgs Fvm)
        {
            List<SelectListItem> temp = new List<SelectListItem>();
            temp.Add(new SelectListItem { Text = "Pending", Value = "Pending" });
            temp.Add(new SelectListItem { Text = "All", Value = "All" });

            ViewBag.WizType = new SelectList(temp, "Value", "Text", Fvm.WizardType);

            List<SelectListItem> tempSOD = new List<SelectListItem>();
            tempSOD.Add(new SelectListItem { Text = "Design", Value = "Design" });
            tempSOD.Add(new SelectListItem { Text = "Sample", Value = "Sample" });            
            tempSOD.Add(new SelectListItem { Text = "All", Value = "All" });

            ViewBag.SOD = new SelectList(tempSOD, "Value", "Text", Fvm.SOD);

            List<SelectListItem> tempDisc = new List<SelectListItem>();

            tempDisc.Add(new SelectListItem { Text = "All", Value = "All" });
            ViewBag.Disc = new SelectList(tempDisc, "Value", "Text", Fvm.DisContinued);


            return PartialView("_Filters", Fvm);
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
