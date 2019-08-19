using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Model.Models;
using Data.Models;
using Service;
using Data.Infrastructure;
using Core.Common;
using Model.ViewModels;
using AutoMapper;
using System.Text;
using Model.ViewModel;
using System.Xml.Linq;
using Reports.Controllers;
using Jobs.Helpers;

namespace Jobs.Areas.Rug.Controllers
{

    [Authorize]
    public class CostingLineController : System.Web.Mvc.Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        List<string> UserRoles = new List<string>();
        ActiivtyLogViewModel LogVm = new ActiivtyLogViewModel();

        ICostingLineService _CostingLineService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;

        bool TimePlanValidation = true;
        string ExceptionMsg = "";
        bool Continue = true;

        public CostingLineController(ICostingLineService Costing, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _CostingLineService = Costing;
            _unitOfWork = unitOfWork;
            _exception = exec;

            UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];

            //Log Initialization
            LogVm.SessionId = 0;
            LogVm.ControllerName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("controller");
            LogVm.ActionName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("action");
            LogVm.User = System.Web.HttpContext.Current.Request.RequestContext.HttpContext.User.Identity.Name;
        }
   
        [HttpGet]
        public JsonResult Index(int id)
        {
            var p = _CostingLineService.GetCostingLineListForIndex(id).ToList();
            return Json(p, JsonRequestBehavior.AllowGet);

        }

        public ActionResult _Index(int id, int Status)
        {
            ViewBag.Status = Status;
            ViewBag.CostingHeaderId = id;
            var p = _CostingLineService.GetCostingLineListForIndex(id).ToList();
            return PartialView(p);
        }

        private void PrepareViewBag(CostingHeader H)
        {
            ViewBag.Docno = H.DocNo;
            ViewBag.DeliveryUnitList = new UnitService(_unitOfWork).GetUnitList().ToList();
        }

        [HttpGet]
        public ActionResult CreateLine(int id)
        {
            return _Create(id);
        }

        [HttpGet]
        public ActionResult CreateLineAfter_Submit(int id)
        {
            return _Create(id);
        }

        public ActionResult _Create(int Id) //Id ==>Sale Order Header Id
        {
            CostingHeader H = new CostingHeaderService(_unitOfWork).GetCostingHeader(Id);
            CostingLineViewModel s = new CostingLineViewModel();
            s.CostingHeaderId = H.CostingHeaderId;
            ViewBag.DocNo = H.DocNo;
            ViewBag.Status = H.Status;
            ViewBag.LineMode = "Create";


            s.DocumentTypeSettings = new DocumentTypeSettingsService(_unitOfWork).GetDocumentTypeSettingsForDocument(H.DocTypeId);

            PrepareViewBag(H);

            var LastTrRec = (from p in db.CostingLine
                             where p.CostingHeaderId == Id
                             orderby p.CostingLineId descending
                             select new
                             {
                                 ProductGroupId = p.ProductGroupId,
                                 ProductGroupName = p.ProductGroup.ProductGroupName,
                                 ColourId = p.ColourId,
                                 ColourName = p.Colour.ColourName,
                                 SizeId = p.SizeId,
                                 SizeName = p.Size.SizeName,
                                 Qty = p.Qty,
                                 PileWeight = p.PileWeight,
                             }).FirstOrDefault();

            if (LastTrRec != null)
            {
                ViewBag.LastTransaction = "Last Line -Product Group : " + LastTrRec.ProductGroupName + ", " + "Qty : " + LastTrRec.Qty;
                s.ProductGroupId = LastTrRec.ProductGroupId;
                s.ColourId = LastTrRec.ColourId;
                s.PileWeight = LastTrRec.PileWeight;
            }

            return PartialView("_Create", s);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _CreatePost(CostingLineViewModel svm)
        {
            CostingLine s = Mapper.Map<CostingLineViewModel, CostingLine>(svm);
            CostingHeader temp = new CostingHeaderService(_unitOfWork).Find(s.CostingHeaderId);
            //if (Command == "Submit" && (s.ProductId == 0))
            //    return RedirectToAction("Submit", "CostingHeader", new { id = s.CostingHeaderId }).Success("Data saved successfully");

            //var settings = new CostingSettingsService(_unitOfWork).GetCostingSettingsForDocument(temp.DocTypeId, temp.DivisionId, temp.SiteId);

            if (svm.Qty <= 0)
            {
                ModelState.AddModelError("Qty", "Please Check Qty");
            }
            //else if (svm.Rate <= 0 && settings.isMandatoryRate == true)
            //{
            //    ModelState.AddModelError("Rate", "Please Check Rate");
            //}
            //else if (svm.Amount <= 0 && settings.isMandatoryRate == true)
            //{
            //    ModelState.AddModelError("Amount", "Please Check Amount");
            //}
            //if (svm.DueDate < temp.DocDate)
            //{
            //    ModelState.AddModelError("DueDate", "DueDate greater than DocDate");
            //}

            if (svm.CostingLineId <= 0)
            {
                ViewBag.LineMode = "Create";
            }
            else
            {
                ViewBag.LineMode = "Edit";
            }

            if (ModelState.IsValid)
            {
                if (svm.CostingLineId <= 0)
                {

                 
                    s.CreatedDate = DateTime.Now;
                    s.ModifiedDate = DateTime.Now;
                    s.CreatedBy = User.Identity.Name;
                    s.ModifiedBy = User.Identity.Name;
                    s.ObjectState = Model.ObjectState.Added;
                    _CostingLineService.Create(s);


                    CostingHeader header = new CostingHeaderService(_unitOfWork).Find(s.CostingHeaderId);



                    if (header.Status != (int)StatusConstants.Drafted && header.Status != (int)StatusConstants.Import)
                    {
                        header.Status = (int)StatusConstants.Modified;
                        header.ModifiedDate = DateTime.Now;
                        header.ModifiedBy = User.Identity.Name;
                    }

                    new CostingHeaderService(_unitOfWork).Update(header);


                    try
                    {
                        _unitOfWork.Save();
                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXCL"] += message;
                        PrepareViewBag(temp);
                        return PartialView("_Create", svm);
                    }

                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = header.DocTypeId,
                        DocId = header.CostingHeaderId,
                        DocLineId = s.CostingLineId,
                        ActivityType = (int)ActivityTypeContants.Added,
                        DocNo = header.DocNo,
                        DocDate = header.DocDate,
                        DocStatus = header.Status,
                    }));


                    return RedirectToAction("_Create", new { id = s.CostingHeaderId });
                }
                else
                {
                    List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();


                    CostingHeader header = new CostingHeaderService(_unitOfWork).Find(svm.CostingHeaderId);
                    StringBuilder logstring = new StringBuilder();
                    int status = header.Status;
                    CostingLine temp1 = _CostingLineService.Find(svm.CostingLineId);

                    CostingLine ExRec = new CostingLine();
                    ExRec = Mapper.Map<CostingLine>(temp1);

                    //End of Tracking the Modifications::





                    temp1.ProductGroupId = svm.ProductGroupId ?? 0;
                    //temp1.Specification = svm.Specification;
                    //temp1.Dimension1Id = svm.Dimension1Id;
                    //temp1.Dimension2Id = svm.Dimension2Id;
                    //temp1.Dimension3Id = svm.Dimension3Id;
                    //temp1.Dimension4Id = svm.Dimension4Id;
                    temp1.Qty = svm.Qty ?? 0;
                    temp1.PileWeight = svm.PileWeight ?? 0;
                    temp1.Remark = svm.Remark;
                    temp1.ModifiedDate = DateTime.Now;
                    temp1.ModifiedBy = User.Identity.Name;
                    _CostingLineService.Update(temp1);

                    LogList.Add(new LogTypeViewModel
                    {
                        ExObj = ExRec,
                        Obj = temp1,
                    });

                    if (header.Status != (int)StatusConstants.Drafted && header.Status != (int)StatusConstants.Import)
                    {

                        header.Status = (int)StatusConstants.Modified;
                        header.ModifiedBy = User.Identity.Name;
                        header.ModifiedDate = DateTime.Now;
                        new CostingHeaderService(_unitOfWork).Update(header);

                    }
                    XElement Modifications = new ModificationsCheckService().CheckChanges(LogList);
                    try
                    {
                        _unitOfWork.Save();
                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXCL"] += message;
                        PrepareViewBag(temp);
                        return PartialView("_Create", svm);
                    }

                    //Saving the Activity Log

                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = temp.DocTypeId,
                        DocId = temp1.CostingHeaderId,
                        DocLineId = temp1.CostingLineId,
                        ActivityType = (int)ActivityTypeContants.Modified,
                        DocNo = temp.DocNo,
                        xEModifications = Modifications,
                        DocDate = temp.DocDate,
                        DocStatus = temp.Status,
                    }));

                    //End of Saving the Activity Log

                    return Json(new { success = true });

                }
            }

            ViewBag.Status = temp.Status;
            PrepareViewBag(temp);
            return PartialView("_Create", svm);
        }

        public JsonResult GetProductDetailJson(int ProductId)
        {
            //FinishedProduct FP = new FinishedProductService(_unitOfWork).Find(ProductId);

            //Product P = new ProductService(_unitOfWork).Find(ProductId);

            //Size A = (from P in db.Product
            //        join VRS in db.ViewRugSize on P.ProductId equals VRS.ProductId into VRStable
            //        from VRStab in VRStable
            //         join S in db.Size on VRStab.ManufaturingSizeID equals S.SizeId  into Stable
            //         from Stab in Stable
            //         select Stab).FirstOrDefault();

            //Decimal ? PileWeight = new ProductService(_unitOfWork).Find(ProductId).GrossWeight;


            CostingLineDetail P = new CostingLineService(_unitOfWork).GetProductDetail(ProductId).FirstOrDefault();


            return Json(new
            {
                PileWeight = P.PileWeight,
                ProductGroupId = P.ProductGroupId,
                ProductGroupName=P.ProductGroupName,
                ColourId = P.ColourId,
                ColourName = P.ColourName,
                SizeId = P.SizeId,
                SizeName = P.SizeName
            });
        }


        [HttpGet]
        public ActionResult _ModifyLine(int id)
        {
            return _Modify(id);
        }

        [HttpGet]
        public ActionResult _ModifyLineAfterSubmit(int id)
        {
            return _Modify(id);
        }

        [HttpGet]
        private ActionResult _Modify(int id)
        {
            CostingLine temp = _CostingLineService.GetCostingLine(id);

            if (temp == null)
            {
                return HttpNotFound();
            }

            #region DocTypeTimeLineValidation
            try
            {

                TimePlanValidation = DocumentValidation.ValidateDocumentLine(new DocumentUniqueId { LockReason = temp.LockReason }, User.Identity.Name, out ExceptionMsg, out Continue);

            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXCL"] += message;
                TimePlanValidation = false;
            }

            if (!TimePlanValidation)
                TempData["CSEXCL"] += ExceptionMsg;
            #endregion

            if ((TimePlanValidation || Continue))
                ViewBag.LineMode = "Edit";

            CostingHeader H = new CostingHeaderService(_unitOfWork).GetCostingHeader(temp.CostingHeaderId);
            ViewBag.DocNo = H.DocNo;
            CostingLineViewModel s = Mapper.Map<CostingLine, CostingLineViewModel>(temp);


            s.DocumentTypeSettings = new DocumentTypeSettingsService(_unitOfWork).GetDocumentTypeSettingsForDocument(H.DocTypeId);

            PrepareViewBag(H);

            return PartialView("_Create", s);
        }

        [HttpGet]
        public ActionResult _DeleteLine(int id)
        {
            return _Delete(id);
        }
        [HttpGet]
        public ActionResult _DeleteLine_AfterSubmit(int id)
        {
            return _Delete(id);
        }


        [HttpGet]
        private ActionResult _Delete(int id)
        {
            CostingLine temp = _CostingLineService.GetCostingLine(id);

            if (temp == null)
            {
                return HttpNotFound();
            }

            #region DocTypeTimeLineValidation
            try
            {

                TimePlanValidation = DocumentValidation.ValidateDocumentLine(new DocumentUniqueId { LockReason = temp.LockReason }, User.Identity.Name, out ExceptionMsg, out Continue);

            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXCL"] += message;
                TimePlanValidation = false;
            }

            if (!TimePlanValidation)
                TempData["CSEXCL"] += ExceptionMsg;
            #endregion

            if ((TimePlanValidation || Continue))
                ViewBag.LineMode = "Delete";

            CostingHeader H = new CostingHeaderService(_unitOfWork).GetCostingHeader(temp.CostingHeaderId);
            ViewBag.DocNo = H.DocNo;


            CostingLineViewModel s = Mapper.Map<CostingLine, CostingLineViewModel>(temp);


            s.DocumentTypeSettings = new DocumentTypeSettingsService(_unitOfWork).GetDocumentTypeSettingsForDocument(H.DocTypeId);


            PrepareViewBag(H);

            return PartialView("_Create", s);
        }

        [HttpGet]
        private ActionResult _Detail(int id)
        {
            CostingLine temp = _CostingLineService.GetCostingLine(id);

            if (temp == null)
            {
                return HttpNotFound();
            }

            CostingHeader H = new CostingHeaderService(_unitOfWork).GetCostingHeader(temp.CostingHeaderId);
            ViewBag.DocNo = H.DocNo;
            CostingLineViewModel s = Mapper.Map<CostingLine, CostingLineViewModel>(temp);
            PrepareViewBag(H);


            return PartialView("_Create", s);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePost(CostingLineViewModel vm)
        {
            List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();

            CostingLine CostingLine = _CostingLineService.Find(vm.CostingLineId);

            
            _CostingLineService.Delete(vm.CostingLineId);
            CostingHeader header = new CostingHeaderService(_unitOfWork).Find(CostingLine.CostingHeaderId);

            if (header.Status != (int)StatusConstants.Drafted && header.Status != (int)StatusConstants.Import)
            {
                header.Status = (int)StatusConstants.Modified;
                header.ModifiedBy = User.Identity.Name;
                header.ModifiedDate = DateTime.Now;
                new CostingHeaderService(_unitOfWork).Update(header);
            }



            LogList.Add(new LogTypeViewModel
            {
                Obj = CostingLine,
            });

            XElement Modifications = new ModificationsCheckService().CheckChanges(LogList);

            try
            {
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXCL"] += message;
                ViewBag.Docno = header.DocNo;
                ViewBag.DeliveryUnitList = new UnitService(_unitOfWork).GetUnitList().ToList();
                return PartialView("_Create", vm);
            }

            LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
            {
                DocTypeId = header.DocTypeId,
                DocId = header.CostingHeaderId,
                DocLineId = CostingLine.CostingLineId,
                ActivityType = (int)ActivityTypeContants.Deleted,
                DocNo = header.DocNo,
                xEModifications = Modifications,
                DocDate = header.DocDate,
                DocStatus = header.Status,
            }));

            return Json(new { success = true });
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



        public JsonResult CheckForValidationinEdit(int ProductId, int CostingHeaderId, int CostingLineId)
        {
            var temp = (_CostingLineService.CheckForProductExists(ProductId, CostingHeaderId, CostingLineId));
            return Json(new { returnvalue = temp });
        }

        public JsonResult CheckForValidation(int ProductId, int CostingHeaderId)
        {
            var temp = (_CostingLineService.CheckForProductExists(ProductId, CostingHeaderId));
            return Json(new { returnvalue = temp });
        }




        public ActionResult GetCustomProductGroups(string searchTerm, int pageSize, int pageNum, int filter)//DocTypeId
        {
            var Query = _CostingLineService.GetCustomProductGroups(filter, searchTerm);
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
