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
    public class ProductConsumptionLineController : System.Web.Mvc.Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        ActiivtyLogViewModel LogVm = new ActiivtyLogViewModel();

        IBomDetailService _BomDetailService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;

        public ProductConsumptionLineController(IBomDetailService BomDetail, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _BomDetailService = BomDetail;
            _unitOfWork = unitOfWork;
            _exception = exec;

            //Log Initialization
            LogVm.SessionId = 0;
            LogVm.ControllerName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("controller");
            LogVm.ActionName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("action");
            LogVm.User = System.Web.HttpContext.Current.Request.RequestContext.HttpContext.User.Identity.Name;
        }

        [HttpGet]
        public JsonResult IndexForFaceContent(int id)
        {
            var p = _BomDetailService.GetDesignConsumptionFaceContentForIndexForProduct(id);
            return Json(p, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult IndexForOtherContent(int id)
        {
            var p = _BomDetailService.GetDesignConsumptionOtherContentForIndexForProduct(id);
            return Json(p, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult IndexForOverTuftContent(int id)
        {
            var p = _BomDetailService.GetDesignConsumptionOverTuftContentForIndexForProduct(id);
            return Json(p, JsonRequestBehavior.AllowGet);

        }

        public ActionResult _Create(int Id) //Id ==>Sale Order Header Id
        {
            ProductConsumptionLineViewModel temp = _BomDetailService.GetBaseProductDetailForProduct(Id);
            temp.BaseProcessId= new ProcessService(_unitOfWork).Find(ProcessConstants.Weaving).ProcessId;
            temp.MainWeight = temp.Weight;

            var MainContens = _BomDetailService.GetDesignConsumptionFaceContentForIndexForProduct(Id);
            var LastMainContentLine = (from L in MainContens
                                       orderby L.BomDetailId descending
                                       select new
                                       {
                                           BomDetailId = L.BomDetailId,
                                           ProductId = L.ProductId
                                       }).FirstOrDefault();
            if (LastMainContentLine != null)
            {
                temp.ProductId = LastMainContentLine.ProductId;
                temp.ConsumptionPer = 100 - MainContens.Sum(m => m.ConsumptionPer);
            }

            return PartialView("_Create", temp);
        }

        public ActionResult _CreateOverTufted(int Id) //Id ==>Base ProductId
        {
            ProductConsumptionLineViewModel temp = _BomDetailService.GetBaseProductDetailForProduct(Id);
            temp.BaseProcessId = new ProcessService(_unitOfWork).Find(ProcessConstants.OverTuft).ProcessId;
            temp.ContentType = "OverTuft Contents";
            return PartialView("_Create", temp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _CreatePost(ProductConsumptionLineViewModel svm)
        {
            if (ModelState.IsValid)
            {
                if (svm.BomDetailId == 0)
                {
                    BomDetail bomdetail = new BomDetail();

                    bomdetail.BaseProductId = svm.BaseProductId;
                    bomdetail.BatchQty = 1;
                    bomdetail.ConsumptionPer = svm.ConsumptionPer;
                    bomdetail.Dimension1Id = svm.Dimension1Id;
                    bomdetail.ProcessId = new ProcessService(_unitOfWork).Find(ProcessConstants.Weaving).ProcessId;
                    bomdetail.ProductId = svm.ProductId;
                    bomdetail.Qty = svm.Qty;
                    bomdetail.BaseProcessId = svm.BaseProcessId;
                    bomdetail.MainWeight = svm.MainWeight;

                    bomdetail.CreatedDate = DateTime.Now;
                    bomdetail.ModifiedDate = DateTime.Now;
                    bomdetail.CreatedBy = User.Identity.Name;
                    bomdetail.ModifiedBy = User.Identity.Name;
                    bomdetail.ObjectState = Model.ObjectState.Added;
                    _BomDetailService.Create(bomdetail);


                    if (bomdetail.BaseProductId == bomdetail.ProductId)
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


                    Product P = new ProductService(_unitOfWork).Find(svm.BaseProductId);

                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.ProductConsumption).DocumentTypeId,
                        DocId = svm.BaseProductId,
                        DocLineId = bomdetail.BomDetailId,
                        DocNo = P.ProductName,
                        DocDate = DateTime.Now,
                        ActivityType = (int)ActivityTypeContants.Added
                    }));

                    return RedirectToAction("_Create", new { id = svm.BaseProductId });
                }
                else
                {
                    List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();
                    BomDetail bomdetail = _BomDetailService.Find(svm.BomDetailId);

                    BomDetail ExRec = Mapper.Map<BomDetail>(bomdetail);

                    bomdetail.BaseProductId = svm.BaseProductId;
                    bomdetail.BatchQty = 1;
                    bomdetail.ConsumptionPer = svm.ConsumptionPer;
                    bomdetail.Dimension1Id = svm.Dimension1Id;
                    bomdetail.ProductId = svm.ProductId;
                    bomdetail.Qty = svm.Qty;


                    bomdetail.ModifiedDate = DateTime.Now;
                    bomdetail.ModifiedBy = User.Identity.Name;
                    bomdetail.ObjectState = Model.ObjectState.Modified;
                    _BomDetailService.Update(bomdetail);


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

                    Product P = new ProductService(_unitOfWork).Find(svm.BaseProductId);

                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.ProductConsumption).DocumentTypeId,
                        DocId = svm.BaseProductId,
                        DocLineId = bomdetail.BomDetailId,
                        DocNo = P.ProductName,
                        DocDate = DateTime.Now,
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
            ProductConsumptionLineViewModel s = _BomDetailService.GetDesignConsumptionLineForEditForProduct(id);
            ProductConsumptionLineViewModel temp = _BomDetailService.GetBaseProductDetailForProduct(s.BaseProductId);
            s.ProductName = temp.ProductName;
            s.QualityName = temp.QualityName;
            s.Weight = temp.Weight;

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
            BomDetail BomDetail = _BomDetailService.Find(id);
            if (BomDetail == null)
            {
                return HttpNotFound();
            }
            return View(BomDetail);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePost(ProductConsumptionLineViewModel vm)
        {
            List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();
            BomDetail BomDetail = _BomDetailService.Find(vm.BomDetailId);

            LogList.Add(new LogTypeViewModel
            {
                ExObj = BomDetail,
            });

            _BomDetailService.Delete(vm.BomDetailId);
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


            Product P = new ProductService(_unitOfWork).Find(vm.BaseProductId);

            LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
            {
                DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.ProductConsumption).DocumentTypeId,
                DocId = vm.BaseProductId,
                DocLineId = vm.BomDetailId,
                DocNo = P.ProductName,
                DocDate = DateTime.Now,
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

        public JsonResult GetProductDetailJson(int ProductId)
        {
            ProductWithGroupAndUnit productgroupandunit = _BomDetailService.GetProductGroupAndUnit(ProductId);
            List<ProductWithGroupAndUnit> productgroupandunitJson = new List<ProductWithGroupAndUnit>();

            productgroupandunitJson.Add(new ProductWithGroupAndUnit()
            {
                ProductGroupId = productgroupandunit.ProductGroupId,
                ProductGroupName = productgroupandunit.ProductGroupName,
                UnitName = productgroupandunit.UnitName
            });

            return Json(productgroupandunitJson);
        }




        public JsonResult CheckForValidationinEdit(int ProductId, int? Dimension1Id, int BaseProductId, int BomDetailId, int BaseProcessId)
        {
            var temp = (_BomDetailService.CheckForProductShadeExists(ProductId, Dimension1Id, BaseProductId, BomDetailId, BaseProcessId));
            return Json(new { returnvalue = temp });
        }

        public JsonResult CheckForValidation(int ProductId, int? Dimension1Id, int BaseProductId, int BaseProcessId)
        {
            var temp = (_BomDetailService.CheckForProductShadeExists(ProductId, Dimension1Id, BaseProductId, BaseProcessId));
            return Json(new { returnvalue = temp });
        }
    }
}
