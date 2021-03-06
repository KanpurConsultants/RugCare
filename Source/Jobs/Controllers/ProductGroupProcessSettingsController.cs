﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Model.Models;
using Data.Models;
using Service;
using Data.Infrastructure;
using Presentation;
using Core.Common;
using Model.ViewModel;
using System.Xml.Linq;
using AutoMapper;
using System.Net;
using Presentation.ViewModels;
using Jobs.Helpers;

namespace Jobs.Controllers
{
    [Authorize]
    public class ProductGroupProcessSettingsController : System.Web.Mvc.Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        List<string> UserRoles = new List<string>();
        ActiivtyLogViewModel LogVm = new ActiivtyLogViewModel();

        IProductGroupProcessSettingsService _ProductGroupProcessSettingsService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;
        public ProductGroupProcessSettingsController(IProductGroupProcessSettingsService ProductGroupProcessSettingsService, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _ProductGroupProcessSettingsService = ProductGroupProcessSettingsService;
            _unitOfWork = unitOfWork;
            _exception = exec;

            UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];

            //Log Initialization
            LogVm.SessionId = 0;
            LogVm.ControllerName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("controller");
            LogVm.ActionName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("action");
            LogVm.User = System.Web.HttpContext.Current.Request.RequestContext.HttpContext.User.Identity.Name;

        }

        private void PrepareViewBag(ProductGroupProcessSettingsViewModel s)
        {
            ViewBag.Name = new ProductGroupService(_unitOfWork).Find(s.ProductGroupId).ProductGroupName;
            ViewBag.id = s.ProductGroupId;
        }

        public ActionResult Index(int id)//ProductGroupId
        {
            var p = _ProductGroupProcessSettingsService.GetProductGroupProcessSettingList(id);
            ViewBag.id = id;
            ViewBag.Name = new ProductGroupService(_unitOfWork).Find(id).ProductGroupName;
            return View(p);
        }


        // GET: /ProductGroupSettingMaster/Create

        public ActionResult Create(int id)//ProductGroupId
        {
            if (!UserRoles.Contains("SysAdmin"))
            {
                return View("~/Views/Shared/InValidSettings.cshtml");
            }
            ProductGroupProcessSettingsViewModel vm = new ProductGroupProcessSettingsViewModel();
            vm.ProductGroupId = id;
            PrepareViewBag(vm);
            return View("Create", vm);

        }

        // POST: /ProductMaster/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Post(ProductGroupProcessSettingsViewModel vm)
        {
            ProductGroupProcessSettings pt = AutoMapper.Mapper.Map<ProductGroupProcessSettingsViewModel, ProductGroupProcessSettings>(vm);


            if (ModelState.IsValid)
            {
                ProductGroup ProductGroup = new ProductGroupService(_unitOfWork).Find(vm.ProductGroupId);
                if (vm.ProductGroupProcessSettingsId <= 0)
                {
                    pt.CreatedDate = DateTime.Now;
                    pt.ModifiedDate = DateTime.Now;
                    pt.CreatedBy = User.Identity.Name;
                    pt.ModifiedBy = User.Identity.Name;
                    pt.ObjectState = Model.ObjectState.Added;
                    _ProductGroupProcessSettingsService.Create(pt);

                    try
                    {
                        _unitOfWork.Save();
                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        ModelState.AddModelError("", message);
                        PrepareViewBag(vm);
                        return View("Create", vm);
                    }

                    int DocTypeId = new DocumentTypeService(_unitOfWork).Find(MasterDocTypeConstants.Carpet).DocumentTypeId;

                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocId = pt.ProductGroupProcessSettingsId,
                        DocTypeId = DocTypeId,
                        ActivityType = (int)ActivityTypeContants.SettingsAdded,
                    }));



                    return RedirectToAction("Index", "ProductGroupProcessSettings", new { id = ProductGroup.ProductGroupId }).Success("Data saved successfully");
                }
                else
                {
                    List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();

                    ProductGroupProcessSettings temp = _ProductGroupProcessSettingsService.Find(pt.ProductGroupProcessSettingsId);

                    ProductGroupProcessSettings ExRec = Mapper.Map<ProductGroupProcessSettings>(temp);

                    temp.ProcessId = vm.ProcessId;
                    temp.QAGroupId = vm.QAGroupId;
                    temp.ModifiedDate = DateTime.Now;
                    temp.ModifiedBy = User.Identity.Name;
                    temp.ObjectState = Model.ObjectState.Modified;
                    _ProductGroupProcessSettingsService.Update(temp);

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
                        ModelState.AddModelError("", message);
                        PrepareViewBag(vm);
                        return View("Create", pt);
                    }

                    int DocTypeId = new DocumentTypeService(_unitOfWork).Find(MasterDocTypeConstants.Carpet).DocumentTypeId;

                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocId = temp.ProductGroupProcessSettingsId,
                        ActivityType = (int)ActivityTypeContants.SettingsModified,
                        DocTypeId = DocTypeId,
                        xEModifications = Modifications,
                    }));

                    return RedirectToAction("Index", "ProductGroupProcessSettings", new { id = ProductGroup.ProductGroupId }).Success("Data saved successfully");

                }

            }
            PrepareViewBag(vm);
            return View("Create", vm);
        }


        public ActionResult Edit(int id)//ProductGroupProcessSettingsId
        {
            if (!UserRoles.Contains("SysAdmin"))
            {
                return View("~/Views/Shared/InValidSettings.cshtml");
            }
            var settings = new ProductGroupProcessSettingsService(_unitOfWork).Find(id);
            ProductGroupProcessSettingsViewModel temp = AutoMapper.Mapper.Map<ProductGroupProcessSettings, ProductGroupProcessSettingsViewModel>(settings);

            PrepareViewBag(temp);
            return View("Create", temp);
        }

        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductGroupProcessSettings ProductGroupProcessSettings = _ProductGroupProcessSettingsService.Find(id);
            if (ProductGroupProcessSettings == null)
            {
                return HttpNotFound();
            }

            ReasonViewModel vm = new ReasonViewModel()
            {
                id = id,
            };

            return PartialView("_Reason", vm);
        }

        // POST: /ProductGroupMaster/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(ReasonViewModel vm)
        {
            List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();
            if (ModelState.IsValid)
            {
                var temp = _ProductGroupProcessSettingsService.Find(vm.id);

                LogList.Add(new LogTypeViewModel
                {
                    ExObj = temp,
                });

                _ProductGroupProcessSettingsService.Delete(vm.id);
                XElement Modifications = new ModificationsCheckService().CheckChanges(LogList);
                try
                {
                    _unitOfWork.Save();
                }

                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    ModelState.AddModelError("", message);
                    return PartialView("_Reason", vm);
                }

                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocId = vm.id,
                    ActivityType = (int)ActivityTypeContants.Deleted,
                    UserRemark = vm.Reason,
                    xEModifications = Modifications,
                }));

                return Json(new { success = true });

            }
            return PartialView("_Reason", vm);
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
