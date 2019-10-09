using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using Model.Models;
using Data.Models;
using Service;
using Data.Infrastructure;
using Core.Common;
using Model.ViewModels;
using AutoMapper;
using Model.ViewModel;
using System.Xml.Linq;
using System.Linq;

namespace Jobs.Areas.Rug.Controllers
{

    [Authorize]
    public class ProductQualityConsumptionLineController : System.Web.Mvc.Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        ActiivtyLogViewModel LogVm = new ActiivtyLogViewModel();

        IProductQualityBomDetailService _ProductQualityBomDetailService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;

        public ProductQualityConsumptionLineController(IProductQualityBomDetailService ProductQualityBomDetail, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _ProductQualityBomDetailService = ProductQualityBomDetail;
            _unitOfWork = unitOfWork;
            _exception = exec;

            //Log Initialization
            LogVm.SessionId = 0;
            LogVm.ControllerName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("controller");
            LogVm.ActionName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("action");
            LogVm.User = System.Web.HttpContext.Current.Request.RequestContext.HttpContext.User.Identity.Name;
        }




        [HttpGet]
        public JsonResult IndexContent(int id)
        {
            var p = _ProductQualityBomDetailService.GetContentForIndexForProduct(id);
            return Json(p, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _Create(int Id) //Id ==>Sale Order Header Id
        {

            ProductQuality PQ = db.ProductQuality.Where(m=>m.ProductQualityId == Id).FirstOrDefault();

            ProductQualityConsumptionLineViewModel temp = new ProductQualityConsumptionLineViewModel();
            temp.ProductQualityId = Id;
            temp.ProductQualityName = PQ.ProductQualityName;
            return PartialView("_Create", temp);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _CreatePost(ProductQualityConsumptionLineViewModel svm)
        {
            if (ModelState.IsValid)
            {
                if (svm.ProductQualityBomDetailId == 0)
                {
                    ProductQualityBomDetail bomdetail = new ProductQualityBomDetail();

                    bomdetail.ProductQualityId = svm.ProductQualityId;
                    bomdetail.BatchQty = 1;
                    bomdetail.Dimension1Id = svm.Dimension1Id;
                    bomdetail.ProcessId = svm.ProcessId;
                    bomdetail.ProductId = svm.ProductId;
                    bomdetail.Qty = svm.Qty;

                    bomdetail.CreatedDate = DateTime.Now;
                    bomdetail.ModifiedDate = DateTime.Now;
                    bomdetail.CreatedBy = User.Identity.Name;
                    bomdetail.ModifiedBy = User.Identity.Name;
                    bomdetail.ObjectState = Model.ObjectState.Added;
                    _ProductQualityBomDetailService.Create(bomdetail);


                    if (bomdetail.ProductQualityId == bomdetail.ProductId)
                    {
                        //return View(svm).Danger(DataValidationMsg);
                        ModelState.AddModelError("", "Invalid Product is Selected!");
                        return PartialView("_Create", svm);
                    }

                    try
                    {
                        _unitOfWork.Save();
                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        ModelState.AddModelError("", message);
                        return PartialView("_Create", svm);
                    }


                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.ProductConsumption).DocumentTypeId,
                        DocId = bomdetail.ProductQualityBomDetailId,
                        ActivityType = (int)ActivityTypeContants.Added,
                    }));

                    return RedirectToAction("_Create", new { id = svm.ProductQualityId });
                }
                else
                {
                    List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();
                    ProductQualityBomDetail bomdetail = _ProductQualityBomDetailService.Find(svm.ProductQualityBomDetailId);

                    ProductQualityBomDetail ExRec = Mapper.Map<ProductQualityBomDetail>(bomdetail);

                    bomdetail.ProcessId = svm.ProcessId;
                    bomdetail.BatchQty = 1;
                    bomdetail.Dimension1Id = svm.Dimension1Id;
                    bomdetail.ProductId = svm.ProductId;
                    bomdetail.Qty = svm.Qty;


                    bomdetail.ModifiedDate = DateTime.Now;
                    bomdetail.ModifiedBy = User.Identity.Name;
                    bomdetail.ObjectState = Model.ObjectState.Modified;
                    _ProductQualityBomDetailService.Update(bomdetail);


                    LogList.Add(new LogTypeViewModel
                    {
                        ExObj = ExRec,
                        Obj = bomdetail,
                    });
                    XElement Modifications = new ModificationsCheckService().CheckChanges(LogList);

                    try
                    {
                        _unitOfWork.Save();
                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        ModelState.AddModelError("", message);
                        return PartialView("_Create", svm);
                    }

                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.ProductConsumption).DocumentTypeId,
                        DocId = bomdetail.ProductQualityBomDetailId,
                        ActivityType = (int)ActivityTypeContants.Modified,
                        xEModifications = Modifications,
                    }));

                    return Json(new { success = true });
                }
            }
            return PartialView("_Create", svm);
        }


        public JsonResult GetConsumptionTotalQty(int BaseProductId, Decimal TotalWeight, Decimal BomQty, int BomDetailId, int BaseProcessId)
        {
            var ProductFaceContentGroups = from p in db.Product
                                           join pg in db.BomDetail on p.ProductId equals pg.ProductId into BOMTable
                                           from BOMTab in BOMTable.DefaultIfEmpty()
                                           join fp in db.FinishedProduct on BOMTab.BaseProductId equals fp.ProductId into FinishedProductTable
                                           from FinishedProductTab in FinishedProductTable.DefaultIfEmpty()
                                           join pcl in db.ProductContentLine on FinishedProductTab.FaceContentId equals pcl.ProductContentHeaderId into ProductContentLineTable
                                           from ProductContentLineTab in ProductContentLineTable.DefaultIfEmpty()
                                           where p.ProductId == BaseProductId && ((int?)ProductContentLineTab.ProductGroupId ?? 0) != 0
                                           group new { ProductContentLineTab } by new { ProductContentLineTab.ProductGroupId } into Result
                                           select new
                                           {
                                               ProductGroupId = Result.Key.ProductGroupId
                                           };


            Decimal TotalFillQty = 0;
            var temp = (from L in db.BomDetail
                        join p in db.Product on L.ProductId equals p.ProductId into ProductTable
                        from ProductTab in ProductTable.DefaultIfEmpty()
                        join pcon in ProductFaceContentGroups on ProductTab.ProductGroupId equals pcon.ProductGroupId into ProductFaceContentTable
                        from ProductFaceContentTab in ProductFaceContentTable.DefaultIfEmpty()
                        where L.BaseProductId == BaseProductId && L.BomDetailId != BomDetailId && L.BaseProcessId == BaseProcessId && ((int?)ProductFaceContentTab.ProductGroupId ?? 0) != 0
                        group (L) by (L.BaseProductId) into Result
                        select new
                        {
                            //TotalQty = Result.Sum(i => i.Qty)
                            TotalQty = Result.Sum(i => i.ConsumptionPer)
                        }).FirstOrDefault();

            if (temp != null)
            {
                TotalFillQty = temp.TotalQty;
            }

            return Json(TotalFillQty);
        }

        public ActionResult _Edit(int id)
        {
            ProductQualityConsumptionLineViewModel s = _ProductQualityBomDetailService.GetDesignConsumptionLineForEditForProduct(id);
            //ProductQualityConsumptionLineViewModel temp = _BomDetailService.GetBaseProductDetailForProduct(s.ProductQualityId);
            //s.ProductName = temp.ProductName;

            if (s == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Create", s);
        }


        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductQualityBomDetail BomDetail = _ProductQualityBomDetailService.Find(id);
            if (BomDetail == null)
            {
                return HttpNotFound();
            }
            return View(BomDetail);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePost(ProductQualityConsumptionLineViewModel vm)
        {
            List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();
            ProductQualityBomDetail BomDetail = _ProductQualityBomDetailService.Find(vm.ProductQualityBomDetailId);

            LogList.Add(new LogTypeViewModel
            {
                ExObj = BomDetail,
            });

            _ProductQualityBomDetailService.Delete(vm.ProductQualityBomDetailId);
            XElement Modifications = new ModificationsCheckService().CheckChanges(LogList);
            try
            {
                _unitOfWork.Save();
            }

            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                ModelState.AddModelError("", message);
                return PartialView("EditSize", vm);
            }

            LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
            {
                DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.ProductConsumption).DocumentTypeId,
                DocId = vm.ProductQualityBomDetailId,
                ActivityType = (int)ActivityTypeContants.Deleted,
                xEModifications = Modifications,
            }));

            return Json(new { success = true });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public JsonResult IsProductContent(int BaseProductId, int ProductId)
        {
            bool IsContent = true;
            var ProductFaceContentGroups = from p in db.Product
                                           join pg in db.BomDetail on p.ProductId equals pg.ProductId into BOMTable
                                           from BOMTab in BOMTable.DefaultIfEmpty()
                                           join fp in db.FinishedProduct on BOMTab.BaseProductId equals fp.ProductId into FinishedProductTable
                                           from FinishedProductTab in FinishedProductTable.DefaultIfEmpty()
                                           join pcl in db.ProductContentLine on FinishedProductTab.FaceContentId equals pcl.ProductContentHeaderId into ProductContentLineTable
                                           from ProductContentLineTab in ProductContentLineTable.DefaultIfEmpty()
                                           where p.ProductId == BaseProductId && ((int?)ProductContentLineTab.ProductGroupId ?? 0) != 0
                                           group new { ProductContentLineTab } by new { ProductContentLineTab.ProductGroupId } into Result
                                           select new
                                           {
                                               ProductGroupId = Result.Key.ProductGroupId
                                           };


            var temp = (from p in db.Product
                        join pcon in ProductFaceContentGroups on p.ProductGroupId equals pcon.ProductGroupId into ProductFaceContentTable
                        from ProductFaceContentTab in ProductFaceContentTable.DefaultIfEmpty()
                        where p.ProductId == ProductId && ((int?)ProductFaceContentTab.ProductGroupId ?? 0) != 0
                        select new
                        {
                            ProductId = p.ProductId
                        }).FirstOrDefault();

            if (temp != null)
            {
                IsContent = true;
            }
            else
            {
                IsContent = false;
            }

            return Json(IsContent);
        }

        //public JsonResult GetProductDetailJson(int ProductId)
        //{
        //    ProductWithGroupAndUnit productgroupandunit = _ProductQualityBomDetailService.GetProductGroupAndUnit(ProductId);
        //    List<ProductWithGroupAndUnit> productgroupandunitJson = new List<ProductWithGroupAndUnit>();

        //    productgroupandunitJson.Add(new ProductWithGroupAndUnit()
        //    {
        //        ProductGroupId = productgroupandunit.ProductGroupId,
        //        ProductGroupName = productgroupandunit.ProductGroupName,
        //        UnitName = productgroupandunit.UnitName
        //    });

        //    return Json(productgroupandunitJson);
        //}




        //public JsonResult CheckForValidationinEdit(int ProductId, int? Dimension1Id, int BaseProductId, int BomDetailId, int BaseProcessId)
        //{
        //    var temp = (_ProductQualityBomDetailService.CheckForProductShadeExists(ProductId, Dimension1Id, BaseProductId, BomDetailId, BaseProcessId));
        //    return Json(new { returnvalue = temp });
        //}

        //public JsonResult CheckForValidation(int ProductId, int? Dimension1Id, int BaseProductId, int BaseProcessId)
        //{
        //    var temp = (_BomDetailService.CheckForProductShadeExists(ProductId, Dimension1Id, BaseProductId, BaseProcessId));
        //    return Json(new { returnvalue = temp });
        //}
    }
}
