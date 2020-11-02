using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Model.Models;
using Data.Models;
using Service;
using Data.Infrastructure;
using Core.Common;
using Model.ViewModels;
using AutoMapper;
using System.Configuration;
using Jobs.Helpers;
using System.Text;
using Model.ViewModel;
using System.Xml.Linq;
using Reports.Controllers;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

namespace Jobs.Areas.Rug.Controllers
{
    
    [Authorize]
    public class PackingLineWithBuyerDetailController : System.Web.Mvc.Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        List<string> UserRoles = new List<string>();
        ActiivtyLogViewModel LogVm = new ActiivtyLogViewModel();

        IPackingLineService _PackingLineService;
        ISaleDispatchLineService _SaleDispatchLineService;
        ISaleInvoiceLineService _SaleInvoiceLineService;
        IStockService _StockService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;

        bool TimePlanValidation = true;
        string ExceptionMsg = "";
        bool Continue = true;

        public PackingLineWithBuyerDetailController(IPackingLineService PackingLineService, ISaleDispatchLineService SaleDispatchLineService, ISaleInvoiceLineService SaleInvoiceLineService, IStockService StockService, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _PackingLineService = PackingLineService;
            _SaleDispatchLineService = SaleDispatchLineService;
            _SaleInvoiceLineService = SaleInvoiceLineService;
            _StockService = StockService;
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
            var p = _PackingLineService.GetPackingLineViewModelForHeaderId(id);
            //return Json(p, JsonRequestBehavior.AllowGet);

            return new JsonResult()
            {
                Data = p,
                MaxJsonLength = Int32.MaxValue,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

        }


        [HttpGet]
        public ActionResult GetProducts(string searchTerm, int pageSize, int pageNum)
        {
            //Get the paged results and the total count of the results for this query. ProductCacheKeyHint
            var productCacheKeyHint = ConfigurationManager.AppSettings["ProductCacheKeyHint"];
            AutoCompleteComboBoxRepositoryAndHelper ar = new AutoCompleteComboBoxRepositoryAndHelper(productCacheKeyHint);
            List<ComboBoxList> prodLst = ar.GetListForComboBox(searchTerm, pageSize, pageNum);
            int prodCount = ar.GetCountForComboBox(searchTerm, pageSize, pageNum);

            //Translate the attendees into a format the select2 dropdown expects
            ComboBoxPagedResult pagedAttendees = ar.TranslateToComboBoxFormat(prodLst, prodCount);

            //Return the data as a jsonp result
            return new JsonpResult
            {
                Data = pagedAttendees,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }


        public ActionResult Print(int id)
        {

            //List<string> CurrentRollNoList = new List<string>();

            // if (System.Web.HttpContext.Current.Session["CurrentRollNoList"] != null)
            // {
            //      CurrentRollNoList = (List<string>)System.Web.HttpContext.Current.Session["CurrentRollNoList"];
            // }

            //CurrentRollNoList.Add("1");

            string CurrentRollNoList = "";

            if (System.Web.HttpContext.Current.Session["CurrentRollNoList"] != null)
            {
                CurrentRollNoList = (string)System.Web.HttpContext.Current.Session["CurrentRollNoList"];
                System.Web.HttpContext.Current.Session["CurrentRollNoList"] = null;
            }
            return RedirectToAction("PrintBarCode", "Report_PackingPrint", new { PackingHeaderId = id, ListofRollNo = CurrentRollNoList });

        }

        public ActionResult PrintOnlyQRCode(int id)
        {

            string CurrentRollNoList = "";

            if (System.Web.HttpContext.Current.Session["CurrentRollNoList"] != null)
            {
                CurrentRollNoList = (string)System.Web.HttpContext.Current.Session["CurrentRollNoList"];
                System.Web.HttpContext.Current.Session["CurrentRollNoList"] = null;
            }
            return RedirectToAction("PrintOnlyQRCode", "Report_PackingPrint", new { PackingHeaderId = id, ListofRollNo = CurrentRollNoList });

        }

        private void PrepareViewBag(PackingLineViewModel s)
        {
            int BuyerId = new PackingHeaderService(_unitOfWork).Find(s.PackingHeaderId).BuyerId;
            if (s==null)
            {
                ViewBag.ProductId = new SelectList (new ProductService(_unitOfWork).GetProductList(),"ProductId","ProductName");
            }
            else
            {
                ViewBag.ProductId = new SelectList(new ProductService(_unitOfWork).GetProductList(), "ProductId", "ProductName",s.ProductId );
            }
            
            ViewBag.DeliveryUnitList = new UnitService(_unitOfWork).GetUnitList().ToList();
            ViewBag.BuyerSkuList = new SaleEnquiryLineService(_unitOfWork).GetBuyerSku(BuyerId);
            //ViewBag.BuyerSpecificationList = new SaleEnquiryLineService(_unitOfWork).GetBuyerSpecification(BuyerId);
            //ViewBag.BuyerSpecification1List = new SaleEnquiryLineService(_unitOfWork).GetBuyerSpecification1(BuyerId);
            //ViewBag.BuyerSpecification2List = new SaleEnquiryLineService(_unitOfWork).GetBuyerSpecification2(BuyerId);
            //ViewBag.BuyerSpecification3List = new SaleEnquiryLineService(_unitOfWork).GetBuyerSpecification3(BuyerId);
        }


        public ActionResult _Create(int Id) //Id ==>Sale Invoice Header Id
        {
            PackingHeader H = new PackingHeaderService(_unitOfWork).GetPackingHeader(Id);
            PackingLineViewModel s = new PackingLineViewModel();

            s.PackingHeaderId = H.PackingHeaderId;
            s.DealUnitId = H.DealUnitId;
            s.DealUnitName = new UnitService(_unitOfWork).Find(s.DealUnitId).UnitName;
            s.GodownId = H.GodownId;
            s.GodownName = new GodownService(_unitOfWork).Find(H.GodownId).GodownName;
            //s.FromProcessId = new ProcessService(_unitOfWork).Find(ProcessConstants.Weaving).ProcessId;
            s.CreateExtraSaleOrder = false;
            s.PackingShipMethodId = H.ShipMethodId;
            s.DocTypeId = H.DocTypeId;

            ProductBuyerSettings ProductBuyerSettings = new ProductBuyerSettingsService(_unitOfWork).GetProductBuyerSettings(H.DivisionId, H.SiteId);
            s.ProductBuyerSettings = Mapper.Map<ProductBuyerSettings, ProductBuyerSettingsViewModel>(ProductBuyerSettings);

            PackingSetting PackingSetting = new PackingSettingService(_unitOfWork).GetPackingSettingForDocument(H.DocTypeId, H.DivisionId, H.SiteId);
            s.PackingSettings = Mapper.Map<PackingSetting, PackingSettingsViewModel>(PackingSetting);

            s.IsShowAllProducts = false ;


            ViewBag.DocNo = H.DocNo;
            ViewBag.Status = H.Status;
            ViewBag.BaleNoPattern = H.BaleNoPattern;
            if (PackingSetting.isAllowtoUpdateBuyerSpecification==true )
                ViewBag.isAllowtoUpdateBuyerSpecification = "Yes";
            else
                ViewBag.isAllowtoUpdateBuyerSpecification = "No";
            ViewBag.LineMode = "Create";
            PrepareViewBag(s);

            var LastTrRec = (from p in db.PackingLine
                             join Pu in db.ProductUid on p.ProductUidId equals Pu.ProductUIDId into PUTable
                             from PUTab in PUTable.DefaultIfEmpty()
                             join PL in db.PackingLineExtended on p.PackingLineId equals PL.PackingLineId into PLTable
                             from PLTab in PLTable.DefaultIfEmpty()
                             join S in db.SaleOrderLine on p.SaleOrderLineId equals S.SaleOrderLineId into STable
                             from STab in STable.DefaultIfEmpty()
                             join SE in db.SaleEnquiryLine on STab.ReferenceDocLineId equals SE.SaleEnquiryLineId into SETable
                             from SETab in SETable.DefaultIfEmpty()
                             join SEE in db.SaleEnquiryLineExtended on SETab.SaleEnquiryLineId equals SEE.SaleEnquiryLineId into SEETable
                             from SEETab in SEETable.DefaultIfEmpty()
                             where p.PackingHeaderId == Id
                             orderby p.PackingLineId descending
                             select new
                             {
                                 ProductUidId = p.ProductUidId,
                                 ProductUidName = PUTab.ProductUidName,
                                 ProductId = p.ProductId,
                                 ProductName = p.Product.ProductName,
                                 BuyerSpecification = SEETab.BuyerSpecification,
                                 BuyerSpecification1 = SEETab.BuyerSpecification1,
                                 BuyerSpecification2 = SEETab.BuyerSpecification2,
                                 BuyerSpecification3 = SEETab.BuyerSpecification3,
                                 BuyerSKU = SEETab.BuyerSpecification+"-"+ SEETab.BuyerSpecification1 + "-" + SEETab.BuyerSpecification2 ,
                                 Barcode = PUTab.ProductUidName ==null ? p.LotNo : PUTab.ProductUidName,
                                 Length = (decimal)PLTab.Length,
                                 Width = (decimal)PLTab.Width,
                                 BaleNo =p.BaleNo,
                                 Qty = p.Qty,
                                 DealQty = p.DealQty,
                                 DealUnitId= p.DealUnitId,
                                 GrossWeight = p.GrossWeight,
                                 NetWeight = p.NetWeight,
                                 Remark = p.Remark,
                                 RateRemark = p.RateRemark,
                                 SaleOrderLineId = p.SaleOrderLineId,
                                 PackingLength = PLTab.PackingLength,
                                 PackingWidth = PLTab.PackingWidth,
                                 PackingHeight = PLTab.PackingHeight,
                                 PackingUnitId = PLTab.PackingUnitId,
                             }).FirstOrDefault();




            var Total = (from L in db.PackingLine
                                   where L.PackingHeaderId == Id
                                   group new { L } by new { L.PackingHeaderId } into Result
                                   select new
                                   {
                                       Qty = Result.Sum(i => i.L.Qty),
                                       DealQty = Result.Sum(i => i.L.DealQty),
                                       Bale = Result.Select(i => i.L.BaleNo).Distinct().Count()
                                   }).FirstOrDefault();

            if (LastTrRec != null)
            {
                decimal BaleQty = (from p in db.PackingLine
                                   where p.PackingHeaderId == Id && p.BaleNo == LastTrRec.BaleNo
                                   select p.Qty).Sum();

                ViewBag.StockLastTransaction = "Last Line -Barcode : " + LastTrRec.Barcode + ", Bale No : " + LastTrRec.BaleNo + "-" + BaleQty.ToString("N0") + ", Product : " + LastTrRec.ProductName + ", BuyerSKU : " + LastTrRec.BuyerSKU + ", Size : " + LastTrRec.Length.ToString("N2") + "X" + LastTrRec.Width.ToString("N2") + " , " + "Qty : " + LastTrRec.Qty.ToString("N0")
                                               + ", " + "Total Qty : " + Total.Qty.ToString("N0") + ", " + "Deal Qty : " + Total.DealQty.ToString("N2") + ", " + "Bale : " + Total.Bale.ToString("N0");

                if (PackingSetting.isAllowtoPackMultipleBarcode==true)
                {
                    String FromProductUidName = "";
                    String ToProductUidName = "";
                    s.BaleNo = (Convert.ToInt64(LastTrRec.BaleNo) + 1).ToString();
                    FromProductUidName = (Convert.ToInt64( LastTrRec.ProductUidName)+1).ToString();
                    ProductUid FromProductUID = db.ProductUid.Where(m=>m.ProductUidName== FromProductUidName).FirstOrDefault();
                    ToProductUidName = (Convert.ToInt64(LastTrRec.ProductUidName) + Convert.ToInt64(BaleQty)).ToString();
                    ProductUid ToProductUID = db.ProductUid.Where(m => m.ProductUidName == ToProductUidName).FirstOrDefault();

                    if (FromProductUID != null && ToProductUID != null)
                    {
                        if (FromProductUID.ProductId == ToProductUID.ProductId && FromProductUID.SaleOrderLineId == ToProductUID.SaleOrderLineId)
                        {
                            s.ProductUidId = FromProductUID.ProductUIDId;
                            s.ProductUidName = FromProductUID.ProductUidName;
                            s.ToProductUidId = ToProductUID.ProductUIDId;
                            s.ToProductUidName = ToProductUID.ProductUidName;
                            s.ProductName = LastTrRec.ProductName;
                            s.ProductId = LastTrRec.ProductId;
                            s.BuyerSku = LastTrRec.BuyerSKU;
                            s.BuyerSKUName = LastTrRec.BuyerSKU;
                            s.BuyerSpecification = LastTrRec.BuyerSpecification;
                            s.BuyerSpecification1 = LastTrRec.BuyerSpecification1;
                            s.BuyerSpecification2 = LastTrRec.BuyerSpecification2;
                            s.BuyerSpecification3 = LastTrRec.BuyerSpecification3;
                            s.Width = LastTrRec.Width;
                            s.Length = LastTrRec.Length;
                            s.Qty = LastTrRec.Qty * BaleQty;
                            s.DealQty = LastTrRec.DealQty * BaleQty;
                            s.DealUnitId = LastTrRec.DealUnitId;
                            s.GrossWeight = LastTrRec.GrossWeight * BaleQty;
                            s.NetWeight = LastTrRec.NetWeight * BaleQty;
                            s.Remark = LastTrRec.Remark;
                            s.RateRemark = LastTrRec.RateRemark;
                            s.SaleOrderLineId = LastTrRec.SaleOrderLineId;
                            s.PackingLength = LastTrRec.PackingLength;
                            s.PackingWidth = LastTrRec.PackingWidth;
                            s.PackingHeight = LastTrRec.PackingHeight * BaleQty;
                            s.PackingUnitId = LastTrRec.PackingUnitId;
                        }
                    }


                }
            }



            return PartialView("_Create", s);
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

        [HttpGet]
        public ActionResult CreateLineAfter_Approve(int id)
        {
            return _Create(id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _CreatePost(PackingLineViewModel svm)
        {
            bool IsModelValid = true;

            PackingHeader packingheader = new PackingHeaderService(_unitOfWork).Find(svm.PackingHeaderId);

            PackingSetting PackingSetting = new PackingSettingService(_unitOfWork).GetPackingSettingForDocument(packingheader.DocTypeId, packingheader.DivisionId, packingheader.SiteId);
            //s.PackingSettings = Mapper.Map<PackingSetting, PackingSettingsViewModel>(PackingSetting);

            if (svm.Qty < 1)
            {
                ModelState.AddModelError("Qty", "Qty is Invalid");
                IsModelValid = false;
            }

            Product Pro = new ProductService(_unitOfWork).Find((int)svm.ProductId);
            Decimal UnitConversionMultiplier = 0;
            Decimal DealQty = 0;
            int PackedPcs = 0;
            UnitConversionMultiplier = new ProductService(_unitOfWork).GetUnitConversionMultiplier(1, Pro.UnitId, (Decimal)svm.Length, (Decimal)svm.Width, svm.Height, svm.DealUnitId, db, svm.DocTypeId);

            DealQty = Math.Round(UnitConversionMultiplier,2) * svm.Qty;

            if ( DealQty !=svm.DealQty)
            {
                svm.DealQty = DealQty;
            }

            if (PackingSetting.isPostedInSaleInvoice == true)
            {
                //PendingOrderQtyForPacking = new PackingLineService(_unitOfWork).FGetPendingOrderQtyForPacking((int)Line.SaleOrderLineId, 0);
                //if (PendingOrderQtyForPacking < 0)
                //{
                //    ValidationMsg = "Balance Qty For Product : " + Line.ProductName + " And Sale Order : " + Line.SaleOrderNo + " is going negative. Can't Submit Record.";
                //    ModelState.AddModelError("", ValidationMsg);
                //    PrepareViewBag(null);
                //    return PartialView("FillProducts", svm);
                //}

                //if (new PackingLineService(_unitOfWork).FSaleOrderProductMatchWithPacking((int)Line.SaleOrderLineId, Line.ProductId) == false)
                //{
                //    ValidationMsg = "Product : " + Line.ProductName + " does not exist in Sale Order : " + Line.SaleOrderNo + " . Can't Submit Record.";
                //    ModelState.AddModelError("", ValidationMsg);
                //    PrepareViewBag(null);
                //    return PartialView("FillProducts", svm);
                //}
            }



            if (ModelState.IsValid && IsModelValid)
            {
                if (svm.PackingLineId == 0)
                {
                    string Message = "";
                    if (PackingSetting.isAllowtoPackMultipleBarcode == true && svm.ToProductUidName !=null && svm.ToProductUidName !="")
                    {
                        PackedPcs = Convert.ToInt32(svm.ToProductUidName) - Convert.ToInt32(svm.ProductUidName);
                    }
                    else
                        PackedPcs = 0;

                    PackedPcs = PackedPcs + 1;

                    decimal PackingQty = svm.Qty / PackedPcs;
                    decimal PackingDealQty = svm.DealQty / PackedPcs;
                    decimal ? PackingHeight = svm.PackingHeight / PackedPcs;
                    decimal PackingGrossWeight = svm.GrossWeight / PackedPcs;
                    decimal PackingNetWeight = svm.NetWeight / PackedPcs;
                    Int64 PackingProductUidName = 0;
                    Int64 result;
                    if (Int64.TryParse(svm.ProductUidName, out result))
                    {
                        PackingProductUidName = Convert.ToInt64(svm.ProductUidName);
                    }

                    for (int Pcs = 0; Pcs < PackedPcs; Pcs++)
                    {
                        string ProductUidName = "";
                        PackingLineViewModel svmp = new PackingLineViewModel();
                        svmp = Mapper.Map<PackingLineViewModel>(svm);

                        if (PackedPcs != 1)
                        {
                            svmp.Qty = PackingQty;
                            svmp.DealQty = PackingDealQty;
                            svmp.GrossWeight = PackingGrossWeight;
                            svmp.NetWeight = PackingNetWeight;
                            svmp.PackingHeight = PackingHeight;
                            ProductUidName = (PackingProductUidName + Pcs).ToString();

                            ProductUid PU = db.ProductUid.Where(m => m.ProductUidName == ProductUidName).FirstOrDefault();
                            svmp.ProductUidName = PU.ProductUidName;
                            svmp.ProductUidId = PU.ProductUIDId;

                            var temp = (from p in db.ViewStockInBalanceForPacking
                                        join S in db.Stock on p.StockInId equals S.StockId into StockTable
                                        from StockTab in StockTable.DefaultIfEmpty()
                                        join Pu in db.ProductUid on StockTab.ProductUidId equals Pu.ProductUIDId into PUTable
                                        from PUTab in PUTable.DefaultIfEmpty()
                                        where PUTab.ProductUIDId == PU.ProductUIDId 
                                        select new
                                        {
                                            StockInId=p.StockInId,
                                            ProductUidId = StockTab.ProductUidId,
                                            ProductUidName = StockTab.ProductUid.ProductUidName,
                                            ProductId = p.ProductId,
                                            BalanceQty = p.BalanceQty,
                                            LotNo = p.LotNo,
                                            FromProcessId = StockTab.ProcessId,
                                            FromProcessName = StockTab.Process.ProcessName,
                                            CurrenctGodownId = StockTab.ProductUidId == null ? StockTab.GodownId : PUTab.CurrenctGodownId,
                                            Status = PUTab.Status

                                        }).FirstOrDefault();
                            svmp.StockInId = temp.StockInId;
                        }

                        Message =CreatePackingLine(svmp);

                        if (Message != "")
                        {
                            ModelState.AddModelError("", Message);
                            return PartialView("_Create", svm);
                        }
                    }

                    //Change On 11122019
                    //string DataValidationMsg = DataValidation(svm);

                    //if (DataValidationMsg != "")
                    //{
                    //    PrepareViewBag(svm);

                    //    ViewBag.BaleNoPattern = packingheader.BaleNoPattern;
                    //    if (PackingSetting.isAllowtoUpdateBuyerSpecification == true)
                    //        ViewBag.isAllowtoUpdateBuyerSpecification = "Yes";
                    //    else
                    //        ViewBag.isAllowtoUpdateBuyerSpecification = "No";

                    //    //return View(svm).Danger(DataValidationMsg);
                    //    ViewBag.LineMode = "Create";
                    //    ModelState.AddModelError("", DataValidationMsg);
                    //    ViewBag.DocNo = packingheader.DocNo;
                    //    return PartialView("_Create", svm);
                    //}

                    //if (svm.SaleOrderLineId == null)
                    //{
                    //    _CreateExtraSaleOrder(ref svm);
                    //}

                    //if (svm.SaleOrderLineId == null)
                    //{
                    //    PrepareViewBag(svm);
                    //    ViewBag.LineMode = "Create";
                    //    ModelState.AddModelError("", "Sale Order is required");
                    //    ViewBag.DocNo = packingheader.DocNo;
                    //    return PartialView("_Create", svm);
                    //}

                    //if (PackingSetting.isAllowtoPackMultipleBarcode == true)
                    //{
                    //    PackedPcs = Convert.ToInt32(svm.ToProductUidName) - Convert.ToInt32(svm.ProductUidName);
                    //}
                    //else
                    //    PackedPcs = 0;

                    //PackedPcs = PackedPcs + 1;


                    //PackingLine packingline = Mapper.Map<PackingLineViewModel, PackingLine>(svm);

                    //for (int Pcs = 0; Pcs < PackedPcs; Pcs++)
                    //{
                    //    StockViewModel StockViewModel_Issue = new StockViewModel();
                    //    //Posting in Stock
                    //    StockViewModel_Issue.StockHeaderId = packingheader.StockHeaderId ?? 0;
                    //    StockViewModel_Issue.DocHeaderId = packingheader.PackingHeaderId;
                    //    StockViewModel_Issue.DocLineId = packingline.PackingLineId;
                    //    StockViewModel_Issue.DocTypeId = packingheader.DocTypeId;
                    //    StockViewModel_Issue.StockHeaderDocDate = packingheader.DocDate;
                    //    StockViewModel_Issue.StockDocDate = DateTime.Now.Date;
                    //    StockViewModel_Issue.DocNo = packingheader.DocNo;
                    //    StockViewModel_Issue.DivisionId = packingheader.DivisionId;
                    //    StockViewModel_Issue.SiteId = packingheader.SiteId;
                    //    StockViewModel_Issue.CurrencyId = null;
                    //    StockViewModel_Issue.HeaderProcessId = null;
                    //    StockViewModel_Issue.PersonId = null;
                    //    StockViewModel_Issue.ProductId = packingline.ProductId;
                    //    StockViewModel_Issue.ProductUidId = packingline.ProductUidId;
                    //    StockViewModel_Issue.HeaderFromGodownId = null;
                    //    StockViewModel_Issue.HeaderGodownId = null;
                    //    StockViewModel_Issue.GodownId = packingheader.GodownId;
                    //    StockViewModel_Issue.ProcessId = packingline.FromProcessId;
                    //    StockViewModel_Issue.LotNo = packingline.LotNo;
                    //    StockViewModel_Issue.CostCenterId = null;
                    //    StockViewModel_Issue.Qty_Iss = packingline.Qty;
                    //    StockViewModel_Issue.Qty_Rec = 0;
                    //    StockViewModel_Issue.Rate = null;
                    //    StockViewModel_Issue.ExpiryDate = null;
                    //    StockViewModel_Issue.Specification = null;
                    //    StockViewModel_Issue.Dimension1Id = null;
                    //    StockViewModel_Issue.Dimension2Id = null;
                    //    StockViewModel_Issue.Remark = packingline.Remark;
                    //    StockViewModel_Issue.Status = packingheader.Status;
                    //    StockViewModel_Issue.CreatedBy = packingheader.CreatedBy;
                    //    StockViewModel_Issue.CreatedDate = DateTime.Now;
                    //    StockViewModel_Issue.ModifiedBy = packingheader.ModifiedBy;
                    //    StockViewModel_Issue.ModifiedDate = DateTime.Now;


                    //    string StockPostingError = "";
                    //    StockPostingError = new StockService(_unitOfWork).StockPost(ref StockViewModel_Issue);

                    //    if (StockPostingError != "")
                    //    {
                    //        ModelState.AddModelError("", StockPostingError);
                    //        return PartialView("_Create", svm);
                    //    }

                    //    if (PackingSetting.isPostedInSaleInvoice != true)
                    //    {

                    //        StockViewModel StockViewModel_Receive = new StockViewModel();
                    //        StockViewModel_Receive.StockHeaderId = packingheader.StockHeaderId ?? -1;
                    //        StockViewModel_Receive.StockId = -1;
                    //        StockViewModel_Receive.DocHeaderId = packingheader.PackingHeaderId;
                    //        StockViewModel_Receive.DocLineId = packingline.PackingLineId;
                    //        StockViewModel_Receive.DocTypeId = packingheader.DocTypeId;
                    //        StockViewModel_Receive.StockHeaderDocDate = packingheader.DocDate;
                    //        StockViewModel_Receive.StockDocDate = DateTime.Now.Date;
                    //        StockViewModel_Receive.DocNo = packingheader.DocNo;
                    //        StockViewModel_Receive.DivisionId = packingheader.DivisionId;
                    //        StockViewModel_Receive.SiteId = packingheader.SiteId;
                    //        StockViewModel_Receive.CurrencyId = null;
                    //        StockViewModel_Receive.HeaderProcessId = null;
                    //        StockViewModel_Receive.PersonId = null;
                    //        StockViewModel_Receive.ProductId = packingline.ProductId;
                    //        StockViewModel_Receive.ProductUidId = packingline.ProductUidId;
                    //        StockViewModel_Receive.HeaderFromGodownId = null;
                    //        StockViewModel_Receive.HeaderGodownId = null;
                    //        StockViewModel_Receive.GodownId = packingheader.GodownId;
                    //        StockViewModel_Receive.ProcessId = new ProcessService(_unitOfWork).Find(ProcessConstants.Packing).ProcessId;
                    //        //StockViewModel_Receive.LotNo = packingheader.DocNo;
                    //        StockViewModel_Receive.LotNo = packingline.LotNo;
                    //        StockViewModel_Receive.CostCenterId = null;
                    //        StockViewModel_Receive.Qty_Iss = 0;
                    //        StockViewModel_Receive.Qty_Rec = packingline.Qty;
                    //        StockViewModel_Receive.Rate = null;
                    //        StockViewModel_Receive.ExpiryDate = null;
                    //        StockViewModel_Receive.Specification = null;
                    //        StockViewModel_Receive.Dimension1Id = null;
                    //        StockViewModel_Receive.Dimension2Id = null;
                    //        StockViewModel_Receive.Remark = packingline.Remark;
                    //        StockViewModel_Receive.Status = packingheader.Status;
                    //        StockViewModel_Receive.StockHeaderExist = 1;
                    //        StockViewModel_Receive.CreatedBy = packingheader.CreatedBy;
                    //        StockViewModel_Receive.CreatedDate = DateTime.Now;
                    //        StockViewModel_Receive.ModifiedBy = packingheader.ModifiedBy;
                    //        StockViewModel_Receive.ModifiedDate = DateTime.Now;



                    //        StockPostingError = new StockService(_unitOfWork).StockPost(ref StockViewModel_Receive);

                    //        if (StockPostingError != "")
                    //        {
                    //            ModelState.AddModelError("", StockPostingError);
                    //            return PartialView("_Create", svm);
                    //        }


                    //        packingline.StockReceiveId = StockViewModel_Receive.StockId;

                    //    }


                    //    if (svm.SaleOrderNo != "" && svm.SaleOrderNo != null && svm.ProductName != "" && svm.ProductName != null)
                    //    {
                    //        string label = new PackingLineService(_unitOfWork).FGetRandonNoForLabel();
                    //        packingline.PartyProductUid = (svm.SaleOrderNo + "|" + svm.ProductName + "_" + label);
                    //        PersonProductUid PUU = new PersonProductUidService(_unitOfWork).FindPendingToPack((int)svm.SaleOrderLineId);
                    //        if (PUU != null)
                    //            packingline.PersonProductUidId = PUU.PersonProductUidId;
                    //    }

                    //    if (packingline.PartyProductUid.ToString().Length > 50)
                    //        packingline.PartyProductUid = packingline.PartyProductUid.ToString().Substring(0, 50);

                    //    packingline.StockIssueId = StockViewModel_Issue.StockId;
                    //    packingline.CreatedDate = DateTime.Now;
                    //    packingline.ModifiedDate = DateTime.Now;
                    //    packingline.CreatedBy = User.Identity.Name;
                    //    packingline.ModifiedBy = User.Identity.Name;


                    //    //if (svm.StockInId != null)
                    //    //{
                    //    //    StockAdj Adj_IssQty = new StockAdj();
                    //    //    Adj_IssQty.StockInId = (int)svm.StockInId;
                    //    //    Adj_IssQty.StockOutId = (int)StockViewModel_Issue.StockId;
                    //    //    Adj_IssQty.DivisionId = packingheader.DivisionId;
                    //    //    Adj_IssQty.SiteId = packingheader.SiteId;
                    //    //    Adj_IssQty.AdjustedQty = packingline.Qty;
                    //    //    Adj_IssQty.ObjectState = Model.ObjectState.Added;
                    //    //    //db.StockAdj.Add(Adj_IssQty);
                    //    //    new StockAdjService(_unitOfWork).Create(Adj_IssQty);
                    //    //}




                    //    if (svm.ProductUidId != null && svm.ProductUidName != null)
                    //    {
                    //        ProductUid productuid = new ProductUidService(_unitOfWork).Find(svm.ProductUidName);


                    //        packingline.ProductUidLastTransactionDocId = productuid.LastTransactionDocId;
                    //        packingline.ProductUidLastTransactionDocDate = productuid.LastTransactionDocDate;
                    //        packingline.ProductUidLastTransactionDocNo = productuid.LastTransactionDocNo;
                    //        packingline.ProductUidLastTransactionDocTypeId = productuid.LastTransactionDocTypeId;
                    //        packingline.ProductUidLastTransactionPersonId = productuid.LastTransactionPersonId;
                    //        packingline.ProductUidStatus = productuid.Status;
                    //        packingline.ProductUidCurrentProcessId = productuid.CurrenctProcessId;
                    //        packingline.ProductUidCurrentGodownId = productuid.CurrenctGodownId;

                    //        //productuid.LastTransactionDocId = packingheader.PackingHeaderId;
                    //        //productuid.LastTransactionDocNo = packingheader.DocNo;
                    //        //productuid.LastTransactionDocTypeId = packingheader.DocTypeId;
                    //        //productuid.LastTransactionDocDate = packingheader.DocDate;
                    //        //productuid.LastTransactionPersonId = packingheader.JobWorkerId;
                    //        //productuid.CurrenctGodownId = packingheader.GodownId;
                    //        //productuid.CurrenctProcessId = new ProcessService(_unitOfWork).Find(ProcessConstants.Packing).ProcessId;
                    //        //productuid.Status = ProductUidStatusConstants.Dispatch;
                    //        //if (productuid.ProcessesDone == null)
                    //        //{
                    //        //    productuid.ProcessesDone = "|" + new ProcessService(_unitOfWork).Find(ProcessConstants.Packing).ProcessId.ToString() + "|";
                    //        //}
                    //        //else
                    //        //{
                    //        //    productuid.ProcessesDone = productuid.ProcessesDone + ",|" + new ProcessService(_unitOfWork).Find(ProcessConstants.Packing).ProcessId.ToString() + "|";
                    //        //}


                    //        //new ProductUidService(_unitOfWork).Update(productuid);
                    //    }


                    //    if (svm.LotNo != null && svm.ProductUidId == null)
                    //    {
                    //        ProductUid productuid = new ProductUidService(_unitOfWork).Find(svm.LotNo);
                    //        if (productuid != null)
                    //        {
                    //            packingline.ProductUidLastTransactionDocId = productuid.LastTransactionDocId;
                    //            packingline.ProductUidLastTransactionDocDate = productuid.LastTransactionDocDate;
                    //            packingline.ProductUidLastTransactionDocNo = productuid.LastTransactionDocNo;
                    //            packingline.ProductUidLastTransactionDocTypeId = productuid.LastTransactionDocTypeId;
                    //            packingline.ProductUidLastTransactionPersonId = productuid.LastTransactionPersonId;
                    //            packingline.ProductUidStatus = productuid.Status;
                    //            packingline.ProductUidCurrentProcessId = productuid.CurrenctProcessId;
                    //            packingline.ProductUidCurrentGodownId = productuid.CurrenctGodownId;

                    //            //productuid.LastTransactionDocId = packingheader.PackingHeaderId;
                    //            //productuid.LastTransactionDocNo = packingheader.DocNo;
                    //            //productuid.LastTransactionDocTypeId = packingheader.DocTypeId;
                    //            //productuid.LastTransactionDocDate = packingheader.DocDate;
                    //            //productuid.LastTransactionPersonId = packingheader.JobWorkerId;
                    //            //productuid.CurrenctGodownId = packingheader.GodownId;
                    //            //productuid.CurrenctProcessId = new ProcessService(_unitOfWork).Find(ProcessConstants.Packing).ProcessId;
                    //            //productuid.Status = ProductUidStatusConstants.Dispatch;
                    //            //if (productuid.ProcessesDone == null)
                    //            //{
                    //            //    productuid.ProcessesDone = "|" + new ProcessService(_unitOfWork).Find(ProcessConstants.Packing).ProcessId.ToString() + "|";
                    //            //}
                    //            //else
                    //            //{
                    //            //    productuid.ProcessesDone = productuid.ProcessesDone + ",|" + new ProcessService(_unitOfWork).Find(ProcessConstants.Packing).ProcessId.ToString() + "|";
                    //            //}


                    //            //new ProductUidService(_unitOfWork).Update(productuid);
                    //        }
                    //    }

                    //    packingline.ObjectState = Model.ObjectState.Added;
                    //    _PackingLineService.Create(packingline);

                    //    PackingLineExtended LineExtended = new PackingLineExtended();
                    //    LineExtended.PackingLineId = LineExtended.PackingLineId;
                    //    LineExtended.Length = svm.Length;
                    //    LineExtended.Width = svm.Width;
                    //    LineExtended.Height = svm.Height;
                    //    new PackingLineExtendedService(_unitOfWork).Create(LineExtended);



                    //    if (packingheader.Status != (int)StatusConstants.Drafted)
                    //    {
                    //        packingheader.Status = (int)StatusConstants.Modified;
                    //        new PackingHeaderService(_unitOfWork).Update(packingheader);
                    //    }


                    //    if (packingheader.StockHeaderId == null)
                    //    {
                    //        packingheader.StockHeaderId = StockViewModel_Issue.StockHeaderId;
                    //        new PackingHeaderService(_unitOfWork).Update(packingheader);
                    //    }




                    //    string OldProductInvoiceGroupName;
                    //    string NewProductInvoiceGroupName;

                    //    if (svm.ProductInvoiceGroupId != null)
                    //    {
                    //        FinishedProduct FinishedProduct = new FinishedProductService(_unitOfWork).Find(svm.ProductId);

                    //        if (svm.ProductInvoiceGroupId != FinishedProduct.ProductInvoiceGroupId)
                    //        {
                    //            if (FinishedProduct.ProductInvoiceGroupId != null)
                    //            {
                    //                OldProductInvoiceGroupName = new ProductInvoiceGroupService(_unitOfWork).Find((int)FinishedProduct.ProductInvoiceGroupId).ProductInvoiceGroupName;
                    //            }
                    //            else
                    //            {
                    //                OldProductInvoiceGroupName = "Blank";
                    //            }

                    //            NewProductInvoiceGroupName = new ProductInvoiceGroupService(_unitOfWork).Find((int)svm.ProductInvoiceGroupId).ProductInvoiceGroupName;

                    //            int ProductTypeId = (from P in db.Product
                    //                                 join Pg in db.ProductGroups on P.ProductGroupId equals Pg.ProductGroupId into ProductGroupTable
                    //                                 from ProductGroupTab in ProductGroupTable.DefaultIfEmpty()
                    //                                 where P.ProductId == FinishedProduct.ProductId
                    //                                 select new
                    //                                 {
                    //                                     ProductTypeId = ProductGroupTab.ProductTypeId
                    //                                 }).FirstOrDefault().ProductTypeId;

                    //            if (ProductTypeId == new ProductTypeService(_unitOfWork).Find(ProductTypeConstants.Rug).ProductTypeId)
                    //            {
                    //                IEnumerable<FinishedProduct> FinishedProductList = (from P in db.FinishedProduct
                    //                                                                    where P.ProductGroupId == FinishedProduct.ProductGroupId
                    //                                                                    select P).ToList();

                    //                foreach (FinishedProduct item in FinishedProductList)
                    //                {
                    //                    item.ProductInvoiceGroupId = svm.ProductInvoiceGroupId;
                    //                    new FinishedProductService(_unitOfWork).Update(item);
                    //                }
                    //            }
                    //            else
                    //            {
                    //                FinishedProduct.ProductInvoiceGroupId = svm.ProductInvoiceGroupId;
                    //                new FinishedProductService(_unitOfWork).Update(FinishedProduct);
                    //            }

                    //            LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    //            {
                    //                DocTypeId = packingheader.DocTypeId,
                    //                DocId = packingline.PackingHeaderId,
                    //                DocLineId = packingline.PackingLineId,
                    //                ActivityType = (int)ActivityTypeContants.Modified,
                    //                DocNo = packingheader.DocNo,
                    //                Narration = "Product invoice group for " + svm.ProductName + " has been changed to " + NewProductInvoiceGroupName + " from " + OldProductInvoiceGroupName + ".",
                    //                DocDate = packingheader.DocDate,
                    //                DocStatus = packingheader.Status,
                    //            }));

                    //        }
                    //    }

                    //    //List<string> CurrentRollNoList = new List<string>();

                    //    //if (System.Web.HttpContext.Current.Session["CurrentRollNoList"] != null)
                    //    //{
                    //    //     CurrentRollNoList = (List<string>)System.Web.HttpContext.Current.Session["CurrentRollNoList"];
                    //    //}

                    //    //CurrentRollNoList.Add(svm.BaleNo);
                    //    //System.Web.HttpContext.Current.Session["CurrentRollNoList"] = CurrentRollNoList ;

                    //    string CurrentRollNoList = "";

                    //    if (System.Web.HttpContext.Current.Session["CurrentRollNoList"] != null)
                    //    {
                    //        CurrentRollNoList = (string)System.Web.HttpContext.Current.Session["CurrentRollNoList"];
                    //    }

                    //    if (CurrentRollNoList == "")
                    //    {
                    //        CurrentRollNoList = svm.BaleNo;
                    //    }
                    //    CurrentRollNoList = CurrentRollNoList + "," + svm.BaleNo;
                    //    System.Web.HttpContext.Current.Session["CurrentRollNoList"] = CurrentRollNoList;


                    //    #region "Update Product Buyer"
                    //    if (svm.SaleOrderLineId != null)
                    //    {
                    //        SaleOrderLine SaleOrderLine = db.SaleOrderLine.Where(m => m.SaleOrderLineId == svm.SaleOrderLineId).FirstOrDefault();
                    //        if (SaleOrderLine != null)
                    //        {
                    //            SaleEnquiryLine SEL = new SaleEnquiryLineService(_unitOfWork).Find((int)SaleOrderLine.ReferenceDocLineId);
                    //            if (SEL != null)
                    //            {
                    //                SaleEnquiryLineExtended productbuyer = (from p in db.SaleEnquiryLineExtended
                    //                                                        where p.SaleEnquiryLineId == SEL.SaleEnquiryLineId
                    //                                                        select p).AsNoTracking().FirstOrDefault();
                    //                if (productbuyer == null)
                    //                {
                    //                    List<LogTypeViewModel> LogListProductBuyer = new List<LogTypeViewModel>();
                    //                    SaleEnquiryLineExtended ExRecProductBuyer = new SaleEnquiryLineExtended();

                    //                    ExRecProductBuyer.SaleEnquiryLineId = productbuyer.SaleEnquiryLineId;
                    //                    ExRecProductBuyer.BuyerSku = svm.BuyerSku;
                    //                    ExRecProductBuyer.BuyerSpecification = svm.BuyerSpecification;
                    //                    ExRecProductBuyer.BuyerSpecification1 = svm.BuyerSpecification1;
                    //                    ExRecProductBuyer.BuyerSpecification2 = svm.BuyerSpecification2;
                    //                    ExRecProductBuyer.BuyerSpecification3 = svm.BuyerSpecification3;
                    //                    ExRecProductBuyer.ObjectState = Model.ObjectState.Added;
                    //                    new SaleEnquiryLineExtendedService(_unitOfWork).Create(ExRecProductBuyer);

                    //                    LogListProductBuyer.Add(new LogTypeViewModel
                    //                    {
                    //                        ExObj = ExRecProductBuyer,
                    //                        Obj = ExRecProductBuyer,
                    //                    });
                    //                    XElement ModificationsProductBuyer = new ModificationsCheckService().CheckChanges(LogListProductBuyer);



                    //                    //Saving the Activity Log

                    //                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    //                    {
                    //                        DocTypeId = packingheader.DocTypeId,
                    //                        DocId = packingline.PackingHeaderId,
                    //                        DocLineId = packingline.PackingLineId,
                    //                        ActivityType = (int)ActivityTypeContants.Modified,
                    //                        DocNo = packingheader.DocNo,
                    //                        xEModifications = ModificationsProductBuyer,
                    //                        DocDate = packingheader.DocDate,
                    //                        DocStatus = (int)StatusConstants.Modified,
                    //                    }));


                    //                }
                    //                else
                    //                {
                    //                    if (productbuyer.BuyerSku != svm.BuyerSku || productbuyer.BuyerSpecification != svm.BuyerSpecification || productbuyer.BuyerSpecification1 != svm.BuyerSpecification1 || productbuyer.BuyerSpecification2 != svm.BuyerSpecification2 || productbuyer.BuyerSpecification3 != svm.BuyerSpecification3)
                    //                    {
                    //                        List<LogTypeViewModel> LogListProductBuyer = new List<LogTypeViewModel>();
                    //                        SaleEnquiryLineExtended ExRecProductBuyer = new SaleEnquiryLineExtended();
                    //                        ExRecProductBuyer = Mapper.Map<SaleEnquiryLineExtended>(productbuyer);

                    //                        productbuyer.BuyerSku = svm.BuyerSku;
                    //                        productbuyer.BuyerSpecification = svm.BuyerSpecification;
                    //                        productbuyer.BuyerSpecification1 = svm.BuyerSpecification1;
                    //                        productbuyer.BuyerSpecification2 = svm.BuyerSpecification2;
                    //                        productbuyer.BuyerSpecification3 = svm.BuyerSpecification3;
                    //                        new SaleEnquiryLineExtendedService(_unitOfWork).Update(productbuyer);

                    //                        LogListProductBuyer.Add(new LogTypeViewModel
                    //                        {
                    //                            ExObj = ExRecProductBuyer,
                    //                            Obj = productbuyer,
                    //                        });
                    //                        XElement ModificationsProductBuyer = new ModificationsCheckService().CheckChanges(LogListProductBuyer);



                    //                        //        //Saving the Activity Log

                    //                        LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    //                        {
                    //                            DocTypeId = packingheader.DocTypeId,
                    //                            DocId = packingline.PackingHeaderId,
                    //                            DocLineId = packingline.PackingLineId,
                    //                            ActivityType = (int)ActivityTypeContants.Modified,
                    //                            DocNo = packingheader.DocNo,
                    //                            xEModifications = ModificationsProductBuyer,
                    //                            DocDate = packingheader.DocDate,
                    //                            DocStatus = (int)StatusConstants.Modified,
                    //                        }));

                    //                        //End of Saving the Activity Log
                    //                    }
                    //                }

                    //            }

                    //        }
                    //    }


                    //    #endregion


                    //    if (PackingSetting.isPostedInSaleInvoice == true)
                    //    {

                    //        SaleDispatchHeader saledispatchheader = db.SaleDispatchHeader.Where(m => m.PackingHeaderId == packingheader.PackingHeaderId).FirstOrDefault();
                    //        SaleInvoiceHeader SH = db.SaleInvoiceHeader.Where(m => m.SaleDispatchHeaderId == saledispatchheader.SaleDispatchHeaderId).FirstOrDefault();
                    //        SaleInvoiceHeaderDetail saleinvoiceheaderdetail = db.SaleInvoiceHeaderDetail.Find(SH.SaleInvoiceHeaderId);

                    //        if (saledispatchheader.StockHeaderId == null)
                    //        {
                    //            saledispatchheader.StockHeaderId = StockViewModel_Issue.StockHeaderId;
                    //            new SaleDispatchHeaderService(_unitOfWork).Update(saledispatchheader);
                    //        }

                    //        SaleDispatchLine saledispatchline = new SaleDispatchLine();
                    //        saledispatchline.SaleDispatchHeaderId = (int)saledispatchheader.SaleDispatchHeaderId;
                    //        saledispatchline.PackingLineId = LineExtended.PackingLineId;
                    //        saledispatchline.GodownId = svm.GodownId;
                    //        saledispatchline.StockId = StockViewModel_Issue.StockId;
                    //        saledispatchline.CreatedBy = User.Identity.Name;
                    //        saledispatchline.ModifiedBy = User.Identity.Name;
                    //        saledispatchline.CreatedDate = DateTime.Now;
                    //        saledispatchline.ModifiedDate = DateTime.Now.Date;






                    //        //For Inserting Product Uid Last Stage

                    //        //if (item.ProductUidId != null)
                    //        //{
                    //        //    ProductUid productuid = new ProductUidService(_unitOfWork).Find((int)item.ProductUidId);

                    //        //    saledispatchline.ProductUidLastTransactionDocId = productuid.LastTransactionDocId;
                    //        //    saledispatchline.ProductUidLastTransactionDocDate = productuid.LastTransactionDocDate;
                    //        //    saledispatchline.ProductUidLastTransactionDocNo = productuid.LastTransactionDocNo;
                    //        //    saledispatchline.ProductUidLastTransactionDocTypeId = productuid.LastTransactionDocTypeId;
                    //        //    saledispatchline.ProductUidLastTransactionPersonId = productuid.LastTransactionPersonId;
                    //        //    saledispatchline.ProductUidStatus = productuid.Status;
                    //        //    saledispatchline.ProductUidCurrentProcessId = productuid.CurrenctProcessId;
                    //        //    saledispatchline.ProductUidCurrentGodownId = productuid.CurrenctGodownId;

                    //        //    productuid.LastTransactionDocId = saledispatchheader.SaleDispatchHeaderId;
                    //        //    productuid.LastTransactionDocNo = saledispatchheader.DocNo;
                    //        //    productuid.LastTransactionDocTypeId = saledispatchheader.DocTypeId;
                    //        //    productuid.LastTransactionDocDate = saledispatchheader.DocDate;
                    //        //    productuid.LastTransactionPersonId = saledispatchheader.SaleToBuyerId;
                    //        //    productuid.CurrenctGodownId = null;
                    //        //    productuid.CurrenctProcessId = null;
                    //        //    productuid.CurrenctProcess = null;
                    //        //    productuid.Status = ProductUidStatusConstants.Dispatch;
                    //        //    productuid.IsActive = false;

                    //        //    productuid.ObjectState = Model.ObjectState.Modified;
                    //        //    db.ProductUid.Add(productuid);
                    //        //}


                    //        saledispatchline.ObjectState = Model.ObjectState.Added;
                    //        _SaleDispatchLineService.Create(saledispatchline);

                    //        SaleInvoiceLine saleinvoiceline = new SaleInvoiceLine();
                    //        saleinvoiceline.SaleDispatchLineId = saledispatchline.SaleDispatchLineId;
                    //        saleinvoiceline.SaleInvoiceHeaderId = saleinvoiceheaderdetail.SaleInvoiceHeaderId;
                    //        saleinvoiceline.SaleOrderLineId = packingline.SaleOrderLineId;
                    //        saleinvoiceline.Qty = packingline.Qty;
                    //        saleinvoiceline.DealQty = packingline.DealQty;
                    //        saleinvoiceline.DealUnitId = packingline.DealUnitId;
                    //        saleinvoiceline.UnitConversionMultiplier = packingline.UnitConversionMultiplier;
                    //        saleinvoiceline.ProductId = packingline.ProductId;
                    //        saleinvoiceline.Remark = packingline.Remark;
                    //        saleinvoiceline.RateRemark = packingline.RateRemark;
                    //        saleinvoiceline.CreatedBy = User.Identity.Name;
                    //        saleinvoiceline.ModifiedBy = User.Identity.Name;
                    //        saleinvoiceline.CreatedDate = DateTime.Now;
                    //        saleinvoiceline.ModifiedDate = DateTime.Now.Date;

                    //        saleinvoiceline.ObjectState = Model.ObjectState.Added;
                    //        _SaleInvoiceLineService.Create(saleinvoiceline);


                    //    }
                    //}

                    //    try
                    //{
                    //    _unitOfWork.Save();
                    //}

                    //catch (Exception ex)
                    //{
                    //    string message = _exception.HandleException(ex);
                    //    ViewBag.DocNo = packingheader.DocNo;
                    //    ViewBag.LineMode = "Create";
                    //    TempData["CSEXCL"] += message;
                    //    return PartialView("_Create", svm);
                    //}


                    //ReAssignSessionVariables(svm);
                    //Change On 11122019

                    return RedirectToAction("_Create", new { id = svm.PackingHeaderId});
                }
                else
                {
                    List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();

                    PackingLine packingline = (from p in db.PackingLine
                                        where p.PackingLineId == svm.PackingLineId
                                        select p).AsNoTracking().FirstOrDefault();

                    PackingLine ExRec = new PackingLine();
                    ExRec = Mapper.Map<PackingLine>(packingline);                    

                    StringBuilder logstring = new StringBuilder();
                    int status = packingheader.Status;


                    packingline.BaleNo = svm.BaleNo;
                    packingline.Specification = svm.Specification;
                    packingline.GrossWeight = svm.GrossWeight;
                    packingline.NetWeight = svm.NetWeight;
                    packingline.Remark = svm.Remark;
                    packingline.SealNo = svm.SealNo;
                    packingline.DealQty = svm.DealQty;
                    packingline.UnitConversionMultiplier = svm.DealQty/svm.Qty;
                    packingline.RateRemark = svm.RateRemark;
                    packingline.ModifiedDate = DateTime.Now;
                    packingline.ModifiedBy = User.Identity.Name;
                    _PackingLineService.Update(packingline);



                    StockAdj Adj = (from L in db.StockAdj
                                    where L.StockOutId == packingline.StockIssueId
                                    select L).FirstOrDefault();

                    //if (Adj != null)
                    //{
                    //    Adj.ObjectState = Model.ObjectState.Deleted;
                    //    db.StockAdj.Remove(Adj);
                    //    new StockAdjService(_unitOfWork).Delete(Adj);
                    //}

                    //if (svm.StockInId != null)
                    //{
                    //    StockAdj Adj_IssQty = new StockAdj();
                    //    Adj_IssQty.StockInId = (int)svm.StockInId;
                    //    Adj_IssQty.StockOutId = (int)packingline.StockIssueId;
                    //    Adj_IssQty.DivisionId = packingheader.DivisionId;
                    //    Adj_IssQty.SiteId = packingheader.SiteId;
                    //    Adj_IssQty.AdjustedQty = svm.Qty;
                    //    Adj_IssQty.ObjectState = Model.ObjectState.Added;
                    //    //db.StockAdj.Add(Adj_IssQty);
                    //    new StockAdjService(_unitOfWork).Create(Adj_IssQty);
                    //}


                    PackingLineExtended LineExtended = (from Ld in db.PackingLineExtended where Ld.PackingLineId == packingline.PackingLineId select Ld).FirstOrDefault();
                    if (LineExtended != null)
                    {
                        LineExtended.Length = svm.Length;
                        LineExtended.Width = svm.Width;
                        LineExtended.Height = svm.Height;
                        LineExtended.PackingLength = svm.PackingLength;
                        LineExtended.PackingWidth = svm.PackingWidth;
                        LineExtended.PackingHeight = svm.PackingHeight;
                        LineExtended.PackingUnitId = svm.PackingUnitId;

                        new PackingLineExtendedService(_unitOfWork).Update(LineExtended);
                    }

                    SaleOrderLine SaleOrderLine = db.SaleOrderLine.Where(m => m.SaleOrderLineId == svm.SaleOrderLineId).FirstOrDefault();

                    if (PackingSetting.isPostedInSaleInvoice == true)
                    {
                        SaleDispatchLine saledispatchline = db.SaleDispatchLine.Where(m => m.PackingLineId == packingline.PackingLineId).FirstOrDefault();
                        saledispatchline.Remark = svm.Remark;
                        saledispatchline.ModifiedDate = DateTime.Now;
                        saledispatchline.ModifiedBy = User.Identity.Name;
                        _SaleDispatchLineService.Update(saledispatchline);


                        SaleInvoiceLine saleinvoiceline = db.SaleInvoiceLine.Where(m => m.SaleDispatchLineId == saledispatchline.SaleDispatchLineId).FirstOrDefault();
                        saleinvoiceline.Remark = svm.Remark;
                        saleinvoiceline.DealQty = svm.DealQty;
                        saleinvoiceline.Rate = SaleOrderLine.Rate;
                        saleinvoiceline.Amount = svm.DealQty* saleinvoiceline.Rate;
                        saleinvoiceline.UnitConversionMultiplier = svm.DealQty / svm.Qty;
                        saleinvoiceline.RateRemark = svm.RateRemark;
                        saleinvoiceline.ModifiedDate = DateTime.Now;
                        saleinvoiceline.ModifiedBy = User.Identity.Name;
                        _SaleInvoiceLineService.Update(saleinvoiceline);

                    }

                    #region "Update Product Buyer"
                    if (svm.SaleOrderLineId != null)
                    {
                       
                        if (SaleOrderLine != null)
                        {
                            SaleEnquiryLine SEL = new SaleEnquiryLineService(_unitOfWork).Find((int)SaleOrderLine.ReferenceDocLineId);
                            if (SEL != null)
                            {
                                SaleEnquiryLineExtended productbuyer = (from p in db.SaleEnquiryLineExtended
                                                                        where p.SaleEnquiryLineId == SEL.SaleEnquiryLineId
                                                                        select p).AsNoTracking().FirstOrDefault();
                                if (productbuyer == null)
                                {
                                    List<LogTypeViewModel> LogListProductBuyer = new List<LogTypeViewModel>();
                                    SaleEnquiryLineExtended ExRecProductBuyer = new SaleEnquiryLineExtended();

                                    ExRecProductBuyer.SaleEnquiryLineId = productbuyer.SaleEnquiryLineId;
                                    ExRecProductBuyer.BuyerSku = svm.BuyerSku;
                                    ExRecProductBuyer.BuyerSpecification = svm.BuyerSpecification;
                                    ExRecProductBuyer.BuyerSpecification1 = svm.BuyerSpecification1;
                                    ExRecProductBuyer.BuyerSpecification2 = svm.BuyerSpecification2;
                                    ExRecProductBuyer.BuyerSpecification3 = svm.BuyerSpecification3;
                                    ExRecProductBuyer.ObjectState = Model.ObjectState.Added;
                                    new SaleEnquiryLineExtendedService(_unitOfWork).Create(ExRecProductBuyer);

                                    LogListProductBuyer.Add(new LogTypeViewModel
                                    {
                                        ExObj = ExRecProductBuyer,
                                        Obj = ExRecProductBuyer,
                                    });
                                    XElement ModificationsProductBuyer = new ModificationsCheckService().CheckChanges(LogListProductBuyer);



                                    //Saving the Activity Log

                                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                                    {
                                        DocTypeId = packingheader.DocTypeId,
                                        DocId = packingline.PackingHeaderId,
                                        DocLineId = packingline.PackingLineId,
                                        ActivityType = (int)ActivityTypeContants.Modified,
                                        DocNo = packingheader.DocNo,
                                        xEModifications = ModificationsProductBuyer,
                                        DocDate = packingheader.DocDate,
                                        DocStatus = (int)StatusConstants.Modified,
                                    }));


                                }
                                else
                                {
                                    if (productbuyer.BuyerSku != svm.BuyerSku || productbuyer.BuyerSpecification != svm.BuyerSpecification || productbuyer.BuyerSpecification1 != svm.BuyerSpecification1 || productbuyer.BuyerSpecification2 != svm.BuyerSpecification2 || productbuyer.BuyerSpecification3 != svm.BuyerSpecification3)
                                    {
                                        List<LogTypeViewModel> LogListProductBuyer = new List<LogTypeViewModel>();
                                        SaleEnquiryLineExtended ExRecProductBuyer = new SaleEnquiryLineExtended();
                                        ExRecProductBuyer = Mapper.Map<SaleEnquiryLineExtended>(productbuyer);

                                        productbuyer.BuyerSku = svm.BuyerSku;
                                        productbuyer.BuyerSpecification = svm.BuyerSpecification;
                                        productbuyer.BuyerSpecification1 = svm.BuyerSpecification1;
                                        productbuyer.BuyerSpecification2 = svm.BuyerSpecification2;
                                        productbuyer.BuyerSpecification3 = svm.BuyerSpecification3;
                                        new SaleEnquiryLineExtendedService(_unitOfWork).Update(productbuyer);

                                        LogListProductBuyer.Add(new LogTypeViewModel
                                        {
                                            ExObj = ExRecProductBuyer,
                                            Obj = productbuyer,
                                        });
                                        XElement ModificationsProductBuyer = new ModificationsCheckService().CheckChanges(LogListProductBuyer);



                                        //        //Saving the Activity Log

                                        LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                                        {
                                            DocTypeId = packingheader.DocTypeId,
                                            DocId = packingline.PackingHeaderId,
                                            DocLineId = packingline.PackingLineId,
                                            ActivityType = (int)ActivityTypeContants.Modified,
                                            DocNo = packingheader.DocNo,
                                            xEModifications = ModificationsProductBuyer,
                                            DocDate = packingheader.DocDate,
                                            DocStatus = (int)StatusConstants.Modified,
                                        }));

                                        //End of Saving the Activity Log
                                    }
                                }

                            }

                        }
                    }


                    #endregion


                    LogList.Add(new LogTypeViewModel
                    {
                        ExObj = ExRec,
                        Obj = packingline,
                    });

                    if (packingheader.Status != (int)StatusConstants.Drafted)
                    {
                        packingheader.Status = (int)StatusConstants.Modified;
                        new PackingHeaderService(_unitOfWork).Update(packingheader);
                    }
                    XElement Modifications = new ModificationsCheckService().CheckChanges(LogList);
                    try
                    {
                        _unitOfWork.Save();
                    }
                 
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        ViewBag.DocNo = packingheader.DocNo;
                        ViewBag.LineMode = "Edit";
                        TempData["CSEXCL"] += message;
                        return PartialView("_Create", svm);
                    }

                    //Saving the Activity Log

                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = packingheader.DocTypeId,
                        DocId = packingline.PackingHeaderId,
                        DocLineId = packingline.PackingLineId,
                        ActivityType = (int)ActivityTypeContants.Modified,
                        DocNo = packingheader.DocNo,
                        xEModifications = Modifications,
                        DocDate = packingheader.DocDate,
                        DocStatus = packingheader.Status,
                    }));
                    
                    //End of Saving the Activity Log


                    return Json(new { success = true });
                }
            }

            ViewBag.Status = packingheader.Status;
            PrepareViewBag(svm);
            return PartialView("_Create", svm);
        }
        public String CreatePackingLine(PackingLineViewModel svm)
        {
            string Message = "";
            PackingHeader packingheader = new PackingHeaderService(_unitOfWork).Find(svm.PackingHeaderId);
            PackingSetting PackingSetting = new PackingSettingService(_unitOfWork).GetPackingSettingForDocument(packingheader.DocTypeId, packingheader.DivisionId, packingheader.SiteId);

            string DataValidationMsg = DataValidation(svm);

            if (DataValidationMsg != "")
            {
                PrepareViewBag(svm);

                ViewBag.BaleNoPattern = packingheader.BaleNoPattern;
                if (PackingSetting.isAllowtoUpdateBuyerSpecification == true)
                    ViewBag.isAllowtoUpdateBuyerSpecification = "Yes";
                else
                    ViewBag.isAllowtoUpdateBuyerSpecification = "No";

                ViewBag.LineMode = "Create";
                ModelState.AddModelError("", DataValidationMsg);
                ViewBag.DocNo = packingheader.DocNo;
                Message = DataValidationMsg;
            }

            if (svm.SaleOrderLineId == null)
            {
                _CreateExtraSaleOrder(ref svm);
            }

            if (svm.SaleOrderLineId == null)
            {
                PrepareViewBag(svm);
                ViewBag.LineMode = "Create";
                ModelState.AddModelError("", "Sale Order is required");
                ViewBag.DocNo = packingheader.DocNo;
                Message = "Sale Order is required";
            }


            if (ModelState.IsValid == true)
            {
                PackingLine packingline = Mapper.Map<PackingLineViewModel, PackingLine>(svm);

                StockViewModel StockViewModel_Issue = new StockViewModel();
                //Posting in Stock
                StockViewModel_Issue.StockHeaderId = packingheader.StockHeaderId ?? 0;
                StockViewModel_Issue.DocHeaderId = packingheader.PackingHeaderId;
                StockViewModel_Issue.DocLineId = packingline.PackingLineId;
                StockViewModel_Issue.DocTypeId = packingheader.DocTypeId;
                StockViewModel_Issue.StockHeaderDocDate = packingheader.DocDate;
                StockViewModel_Issue.StockDocDate = DateTime.Now.Date;
                StockViewModel_Issue.DocNo = packingheader.DocNo;
                StockViewModel_Issue.DivisionId = packingheader.DivisionId;
                StockViewModel_Issue.SiteId = packingheader.SiteId;
                StockViewModel_Issue.CurrencyId = null;
                StockViewModel_Issue.HeaderProcessId = null;
                StockViewModel_Issue.PersonId = packingheader.BuyerId;
                StockViewModel_Issue.ProductId = packingline.ProductId;
                StockViewModel_Issue.ProductUidId = packingline.ProductUidId;
                StockViewModel_Issue.HeaderFromGodownId = null;
                StockViewModel_Issue.HeaderGodownId = null;
                StockViewModel_Issue.GodownId = packingheader.GodownId;
                StockViewModel_Issue.ProcessId = packingline.FromProcessId;
                StockViewModel_Issue.LotNo = packingline.LotNo;
                StockViewModel_Issue.CostCenterId = null;
                StockViewModel_Issue.Qty_Iss = packingline.Qty;
                StockViewModel_Issue.Qty_Rec = 0;
                StockViewModel_Issue.Rate = null;
                StockViewModel_Issue.ExpiryDate = null;
                StockViewModel_Issue.Specification = null;
                StockViewModel_Issue.Dimension1Id = null;
                StockViewModel_Issue.Dimension2Id = null;
                StockViewModel_Issue.Remark = packingline.Remark;
                StockViewModel_Issue.Status = packingheader.Status;
                StockViewModel_Issue.CreatedBy = packingheader.CreatedBy;
                StockViewModel_Issue.CreatedDate = DateTime.Now;
                StockViewModel_Issue.ModifiedBy = packingheader.ModifiedBy;
                StockViewModel_Issue.ModifiedDate = DateTime.Now;


                string StockPostingError = "";
                StockPostingError = new StockService(_unitOfWork).StockPost(ref StockViewModel_Issue);

                if (StockPostingError != "")
                {
                    ModelState.AddModelError("", StockPostingError);
                    Message = StockPostingError;
                }

                if (PackingSetting.isPostedInSaleInvoice != true)
                {

                    StockViewModel StockViewModel_Receive = new StockViewModel();
                    StockViewModel_Receive.StockHeaderId = packingheader.StockHeaderId ?? -1;
                    StockViewModel_Receive.StockId = -1;
                    StockViewModel_Receive.DocHeaderId = packingheader.PackingHeaderId;
                    StockViewModel_Receive.DocLineId = packingline.PackingLineId;
                    StockViewModel_Receive.DocTypeId = packingheader.DocTypeId;
                    StockViewModel_Receive.StockHeaderDocDate = packingheader.DocDate;
                    StockViewModel_Receive.StockDocDate = DateTime.Now.Date;
                    StockViewModel_Receive.DocNo = packingheader.DocNo;
                    StockViewModel_Receive.DivisionId = packingheader.DivisionId;
                    StockViewModel_Receive.SiteId = packingheader.SiteId;
                    StockViewModel_Receive.CurrencyId = null;
                    StockViewModel_Receive.HeaderProcessId = null;
                    StockViewModel_Receive.PersonId = null;
                    StockViewModel_Receive.ProductId = packingline.ProductId;
                    StockViewModel_Receive.ProductUidId = packingline.ProductUidId;
                    StockViewModel_Receive.HeaderFromGodownId = null;
                    StockViewModel_Receive.HeaderGodownId = null;
                    StockViewModel_Receive.GodownId = packingheader.GodownId;
                    StockViewModel_Receive.ProcessId = new ProcessService(_unitOfWork).Find(ProcessConstants.Packing).ProcessId;
                    StockViewModel_Receive.LotNo = packingline.LotNo;
                    StockViewModel_Receive.CostCenterId = null;
                    StockViewModel_Receive.Qty_Iss = 0;
                    StockViewModel_Receive.Qty_Rec = packingline.Qty;
                    StockViewModel_Receive.Rate = null;
                    StockViewModel_Receive.ExpiryDate = null;
                    StockViewModel_Receive.Specification = null;
                    StockViewModel_Receive.Dimension1Id = null;
                    StockViewModel_Receive.Dimension2Id = null;
                    StockViewModel_Receive.Remark = packingline.Remark;
                    StockViewModel_Receive.Status = packingheader.Status;
                    StockViewModel_Receive.StockHeaderExist = 1;
                    StockViewModel_Receive.CreatedBy = packingheader.CreatedBy;
                    StockViewModel_Receive.CreatedDate = DateTime.Now;
                    StockViewModel_Receive.ModifiedBy = packingheader.ModifiedBy;
                    StockViewModel_Receive.ModifiedDate = DateTime.Now;



                    StockPostingError = new StockService(_unitOfWork).StockPost(ref StockViewModel_Receive);

                    if (StockPostingError != "")
                    {
                        ModelState.AddModelError("", StockPostingError);
                        Message = StockPostingError;
                    }


                    packingline.StockReceiveId = StockViewModel_Receive.StockId;

                }


                if (svm.SaleOrderNo != "" && svm.SaleOrderNo != null && svm.ProductName != "" && svm.ProductName != null)
                {
                    string label = new PackingLineService(_unitOfWork).FGetRandonNoForLabel();
                    packingline.PartyProductUid = (svm.SaleOrderNo + "|" + svm.ProductName + "_" + label);
                    PersonProductUid PUU = new PersonProductUidService(_unitOfWork).FindPendingToPack((int)svm.SaleOrderLineId);
                    if (PUU != null)
                        packingline.PersonProductUidId = PUU.PersonProductUidId;
                }

                if (packingline.PartyProductUid.ToString().Length > 50)
                    packingline.PartyProductUid = packingline.PartyProductUid.ToString().Substring(0, 50);

                packingline.StockIssueId = StockViewModel_Issue.StockId;
                packingline.CreatedDate = DateTime.Now;
                packingline.ModifiedDate = DateTime.Now;
                packingline.CreatedBy = User.Identity.Name;
                packingline.ModifiedBy = User.Identity.Name;

                if (PackingSetting.isAllowtoDirectPacking != true  && svm.StockInId != null)
                {
                    StockAdj Adj_IssQty = new StockAdj();
                    Adj_IssQty.StockInId = (int)svm.StockInId;
                    Adj_IssQty.StockOutId = (int)StockViewModel_Issue.StockId;
                    Adj_IssQty.DivisionId = packingheader.DivisionId;
                    Adj_IssQty.SiteId = packingheader.SiteId;
                    Adj_IssQty.AdjustedQty = packingline.Qty;
                    Adj_IssQty.ObjectState = Model.ObjectState.Added;
                    //db.StockAdj.Add(Adj_IssQty);
                    new StockAdjService(_unitOfWork).Create(Adj_IssQty);
                }

                if (svm.ProductUidId != null && svm.ProductUidName != null)
                {
                    ProductUid productuid = new ProductUidService(_unitOfWork).Find(svm.ProductUidName);
                    packingline.ProductUidLastTransactionDocId = productuid.LastTransactionDocId;
                    packingline.ProductUidLastTransactionDocDate = productuid.LastTransactionDocDate;
                    packingline.ProductUidLastTransactionDocNo = productuid.LastTransactionDocNo;
                    packingline.ProductUidLastTransactionDocTypeId = productuid.LastTransactionDocTypeId;
                    packingline.ProductUidLastTransactionPersonId = productuid.LastTransactionPersonId;
                    packingline.ProductUidStatus = productuid.Status;
                    packingline.ProductUidCurrentProcessId = productuid.CurrenctProcessId;
                    packingline.ProductUidCurrentGodownId = productuid.CurrenctGodownId;

                    //productuid.LastTransactionDocId = packingheader.PackingHeaderId;
                    //productuid.LastTransactionDocNo = packingheader.DocNo;
                    //productuid.LastTransactionDocTypeId = packingheader.DocTypeId;
                    //productuid.LastTransactionDocDate = packingheader.DocDate;
                    //productuid.LastTransactionPersonId = packingheader.JobWorkerId;
                    //productuid.CurrenctGodownId = packingheader.GodownId;
                    //productuid.CurrenctProcessId = new ProcessService(_unitOfWork).Find(ProcessConstants.Packing).ProcessId;
                    //productuid.Status = ProductUidStatusConstants.Dispatch;
                    //if (productuid.ProcessesDone == null)
                    //{
                    //    productuid.ProcessesDone = "|" + new ProcessService(_unitOfWork).Find(ProcessConstants.Packing).ProcessId.ToString() + "|";
                    //}
                    //else
                    //{
                    //    productuid.ProcessesDone = productuid.ProcessesDone + ",|" + new ProcessService(_unitOfWork).Find(ProcessConstants.Packing).ProcessId.ToString() + "|";
                    //}


                    //new ProductUidService(_unitOfWork).Update(productuid);
                }


                if (svm.LotNo != null && svm.ProductUidId == null)
                {
                    ProductUid productuid = new ProductUidService(_unitOfWork).Find(svm.LotNo);
                    if (productuid != null)
                    {
                        packingline.ProductUidLastTransactionDocId = productuid.LastTransactionDocId;
                        packingline.ProductUidLastTransactionDocDate = productuid.LastTransactionDocDate;
                        packingline.ProductUidLastTransactionDocNo = productuid.LastTransactionDocNo;
                        packingline.ProductUidLastTransactionDocTypeId = productuid.LastTransactionDocTypeId;
                        packingline.ProductUidLastTransactionPersonId = productuid.LastTransactionPersonId;
                        packingline.ProductUidStatus = productuid.Status;
                        packingline.ProductUidCurrentProcessId = productuid.CurrenctProcessId;
                        packingline.ProductUidCurrentGodownId = productuid.CurrenctGodownId;

                        //productuid.LastTransactionDocId = packingheader.PackingHeaderId;
                        //productuid.LastTransactionDocNo = packingheader.DocNo;
                        //productuid.LastTransactionDocTypeId = packingheader.DocTypeId;
                        //productuid.LastTransactionDocDate = packingheader.DocDate;
                        //productuid.LastTransactionPersonId = packingheader.JobWorkerId;
                        //productuid.CurrenctGodownId = packingheader.GodownId;
                        //productuid.CurrenctProcessId = new ProcessService(_unitOfWork).Find(ProcessConstants.Packing).ProcessId;
                        //productuid.Status = ProductUidStatusConstants.Dispatch;
                        //if (productuid.ProcessesDone == null)
                        //{
                        //    productuid.ProcessesDone = "|" + new ProcessService(_unitOfWork).Find(ProcessConstants.Packing).ProcessId.ToString() + "|";
                        //}
                        //else
                        //{
                        //    productuid.ProcessesDone = productuid.ProcessesDone + ",|" + new ProcessService(_unitOfWork).Find(ProcessConstants.Packing).ProcessId.ToString() + "|";
                        //}


                        //new ProductUidService(_unitOfWork).Update(productuid);
                    }
                }

                packingline.ObjectState = Model.ObjectState.Added;
                _PackingLineService.Create(packingline);

                PackingLineExtended LineExtended = new PackingLineExtended();
                LineExtended.PackingLineId = LineExtended.PackingLineId;
                LineExtended.Length = svm.Length;
                LineExtended.Width = svm.Width;
                LineExtended.Height = svm.Height;
                LineExtended.PackingLength = svm.PackingLength;
                LineExtended.PackingWidth = svm.PackingWidth;
                LineExtended.PackingHeight = svm.PackingHeight;
                LineExtended.PackingUnitId = svm.PackingUnitId;
                new PackingLineExtendedService(_unitOfWork).Create(LineExtended);



                if (packingheader.Status != (int)StatusConstants.Drafted)
                {
                    packingheader.Status = (int)StatusConstants.Modified;
                    new PackingHeaderService(_unitOfWork).Update(packingheader);
                }


                if (packingheader.StockHeaderId == null)
                {
                    packingheader.StockHeaderId = StockViewModel_Issue.StockHeaderId;
                    new PackingHeaderService(_unitOfWork).Update(packingheader);
                }




                string OldProductInvoiceGroupName;
                string NewProductInvoiceGroupName;

                if (svm.ProductInvoiceGroupId != null)
                {
                    FinishedProduct FinishedProduct = new FinishedProductService(_unitOfWork).Find(svm.ProductId);

                    if (svm.ProductInvoiceGroupId != FinishedProduct.ProductInvoiceGroupId)
                    {
                        if (FinishedProduct.ProductInvoiceGroupId != null)
                        {
                            OldProductInvoiceGroupName = new ProductInvoiceGroupService(_unitOfWork).Find((int)FinishedProduct.ProductInvoiceGroupId).ProductInvoiceGroupName;
                        }
                        else
                        {
                            OldProductInvoiceGroupName = "Blank";
                        }

                        NewProductInvoiceGroupName = new ProductInvoiceGroupService(_unitOfWork).Find((int)svm.ProductInvoiceGroupId).ProductInvoiceGroupName;

                        int ProductTypeId = (from P in db.Product
                                             join Pg in db.ProductGroups on P.ProductGroupId equals Pg.ProductGroupId into ProductGroupTable
                                             from ProductGroupTab in ProductGroupTable.DefaultIfEmpty()
                                             where P.ProductId == FinishedProduct.ProductId
                                             select new
                                             {
                                                 ProductTypeId = ProductGroupTab.ProductTypeId
                                             }).FirstOrDefault().ProductTypeId;

                        if (ProductTypeId == new ProductTypeService(_unitOfWork).Find(ProductTypeConstants.Rug).ProductTypeId)
                        {
                            IEnumerable<FinishedProduct> FinishedProductList = (from P in db.FinishedProduct
                                                                                where P.ProductGroupId == FinishedProduct.ProductGroupId
                                                                                select P).ToList();

                            foreach (FinishedProduct item in FinishedProductList)
                            {
                                item.ProductInvoiceGroupId = svm.ProductInvoiceGroupId;
                                new FinishedProductService(_unitOfWork).Update(item);
                            }
                        }
                        else
                        {
                            FinishedProduct.ProductInvoiceGroupId = svm.ProductInvoiceGroupId;
                            new FinishedProductService(_unitOfWork).Update(FinishedProduct);
                        }

                        LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                        {
                            DocTypeId = packingheader.DocTypeId,
                            DocId = packingline.PackingHeaderId,
                            DocLineId = packingline.PackingLineId,
                            ActivityType = (int)ActivityTypeContants.Modified,
                            DocNo = packingheader.DocNo,
                            Narration = "Product invoice group for " + svm.ProductName + " has been changed to " + NewProductInvoiceGroupName + " from " + OldProductInvoiceGroupName + ".",
                            DocDate = packingheader.DocDate,
                            DocStatus = packingheader.Status,
                        }));

                    }
                }

                string CurrentRollNoList = "";

                if (System.Web.HttpContext.Current.Session["CurrentRollNoList"] != null)
                {
                    CurrentRollNoList = (string)System.Web.HttpContext.Current.Session["CurrentRollNoList"];
                }

                if (CurrentRollNoList == "")
                {
                    CurrentRollNoList = svm.BaleNo;
                }
                CurrentRollNoList = CurrentRollNoList + "," + svm.BaleNo;
                System.Web.HttpContext.Current.Session["CurrentRollNoList"] = CurrentRollNoList;


                #region "Update Product Buyer"
                if (svm.SaleOrderLineId != null)
                {
                    SaleOrderLine SaleOrderLine = db.SaleOrderLine.Where(m => m.SaleOrderLineId == svm.SaleOrderLineId).FirstOrDefault();
                    if (SaleOrderLine != null)
                    {
                        SaleEnquiryLine SEL = new SaleEnquiryLineService(_unitOfWork).Find((int)SaleOrderLine.ReferenceDocLineId);
                        if (SEL != null)
                        {
                            SaleEnquiryLineExtended productbuyer = (from p in db.SaleEnquiryLineExtended
                                                                    where p.SaleEnquiryLineId == SEL.SaleEnquiryLineId
                                                                    select p).AsNoTracking().FirstOrDefault();
                            if (productbuyer == null)
                            {
                                List<LogTypeViewModel> LogListProductBuyer = new List<LogTypeViewModel>();
                                SaleEnquiryLineExtended ExRecProductBuyer = new SaleEnquiryLineExtended();

                                ExRecProductBuyer.SaleEnquiryLineId = productbuyer.SaleEnquiryLineId;
                                ExRecProductBuyer.BuyerSku = svm.BuyerSku;
                                ExRecProductBuyer.BuyerSpecification = svm.BuyerSpecification;
                                ExRecProductBuyer.BuyerSpecification1 = svm.BuyerSpecification1;
                                ExRecProductBuyer.BuyerSpecification2 = svm.BuyerSpecification2;
                                ExRecProductBuyer.BuyerSpecification3 = svm.BuyerSpecification3;
                                ExRecProductBuyer.ObjectState = Model.ObjectState.Added;
                                new SaleEnquiryLineExtendedService(_unitOfWork).Create(ExRecProductBuyer);

                                LogListProductBuyer.Add(new LogTypeViewModel
                                {
                                    ExObj = ExRecProductBuyer,
                                    Obj = ExRecProductBuyer,
                                });
                                XElement ModificationsProductBuyer = new ModificationsCheckService().CheckChanges(LogListProductBuyer);



                                //Saving the Activity Log

                                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                                {
                                    DocTypeId = packingheader.DocTypeId,
                                    DocId = packingline.PackingHeaderId,
                                    DocLineId = packingline.PackingLineId,
                                    ActivityType = (int)ActivityTypeContants.Modified,
                                    DocNo = packingheader.DocNo,
                                    xEModifications = ModificationsProductBuyer,
                                    DocDate = packingheader.DocDate,
                                    DocStatus = (int)StatusConstants.Modified,
                                }));


                            }
                            else
                            {
                                if (productbuyer.BuyerSku != svm.BuyerSku || productbuyer.BuyerSpecification != svm.BuyerSpecification || productbuyer.BuyerSpecification1 != svm.BuyerSpecification1 || productbuyer.BuyerSpecification2 != svm.BuyerSpecification2 || productbuyer.BuyerSpecification3 != svm.BuyerSpecification3)
                                {
                                    List<LogTypeViewModel> LogListProductBuyer = new List<LogTypeViewModel>();
                                    SaleEnquiryLineExtended ExRecProductBuyer = new SaleEnquiryLineExtended();
                                    ExRecProductBuyer = Mapper.Map<SaleEnquiryLineExtended>(productbuyer);

                                    productbuyer.BuyerSku = svm.BuyerSku;
                                    productbuyer.BuyerSpecification = svm.BuyerSpecification;
                                    productbuyer.BuyerSpecification1 = svm.BuyerSpecification1;
                                    productbuyer.BuyerSpecification2 = svm.BuyerSpecification2;
                                    productbuyer.BuyerSpecification3 = svm.BuyerSpecification3;
                                    new SaleEnquiryLineExtendedService(_unitOfWork).Update(productbuyer);

                                    LogListProductBuyer.Add(new LogTypeViewModel
                                    {
                                        ExObj = ExRecProductBuyer,
                                        Obj = productbuyer,
                                    });
                                    XElement ModificationsProductBuyer = new ModificationsCheckService().CheckChanges(LogListProductBuyer);



                                    //        //Saving the Activity Log

                                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                                    {
                                        DocTypeId = packingheader.DocTypeId,
                                        DocId = packingline.PackingHeaderId,
                                        DocLineId = packingline.PackingLineId,
                                        ActivityType = (int)ActivityTypeContants.Modified,
                                        DocNo = packingheader.DocNo,
                                        xEModifications = ModificationsProductBuyer,
                                        DocDate = packingheader.DocDate,
                                        DocStatus = (int)StatusConstants.Modified,
                                    }));

                                    //End of Saving the Activity Log
                                }
                            }

                        }

                    }
                }


                #endregion


                if (PackingSetting.isPostedInSaleInvoice == true)
                {

                    SaleDispatchHeader saledispatchheader = db.SaleDispatchHeader.Where(m => m.PackingHeaderId == packingheader.PackingHeaderId).FirstOrDefault();
                    SaleInvoiceHeader SH = db.SaleInvoiceHeader.Where(m => m.SaleDispatchHeaderId == saledispatchheader.SaleDispatchHeaderId).FirstOrDefault();
                    SaleInvoiceHeaderDetail saleinvoiceheaderdetail = db.SaleInvoiceHeaderDetail.Find(SH.SaleInvoiceHeaderId);

                    if (saledispatchheader.StockHeaderId == null)
                    {
                        saledispatchheader.StockHeaderId = StockViewModel_Issue.StockHeaderId;
                        new SaleDispatchHeaderService(_unitOfWork).Update(saledispatchheader);
                    }

                    SaleDispatchLine saledispatchline = new SaleDispatchLine();
                    saledispatchline.SaleDispatchHeaderId = (int)saledispatchheader.SaleDispatchHeaderId;
                    saledispatchline.PackingLineId = LineExtended.PackingLineId;
                    saledispatchline.GodownId = svm.GodownId;
                    saledispatchline.StockId = StockViewModel_Issue.StockId;
                    saledispatchline.CreatedBy = User.Identity.Name;
                    saledispatchline.ModifiedBy = User.Identity.Name;
                    saledispatchline.CreatedDate = DateTime.Now;
                    saledispatchline.ModifiedDate = DateTime.Now.Date;






                    //For Inserting Product Uid Last Stage

                    //if (item.ProductUidId != null)
                    //{
                    //    ProductUid productuid = new ProductUidService(_unitOfWork).Find((int)item.ProductUidId);

                    //    saledispatchline.ProductUidLastTransactionDocId = productuid.LastTransactionDocId;
                    //    saledispatchline.ProductUidLastTransactionDocDate = productuid.LastTransactionDocDate;
                    //    saledispatchline.ProductUidLastTransactionDocNo = productuid.LastTransactionDocNo;
                    //    saledispatchline.ProductUidLastTransactionDocTypeId = productuid.LastTransactionDocTypeId;
                    //    saledispatchline.ProductUidLastTransactionPersonId = productuid.LastTransactionPersonId;
                    //    saledispatchline.ProductUidStatus = productuid.Status;
                    //    saledispatchline.ProductUidCurrentProcessId = productuid.CurrenctProcessId;
                    //    saledispatchline.ProductUidCurrentGodownId = productuid.CurrenctGodownId;

                    //    productuid.LastTransactionDocId = saledispatchheader.SaleDispatchHeaderId;
                    //    productuid.LastTransactionDocNo = saledispatchheader.DocNo;
                    //    productuid.LastTransactionDocTypeId = saledispatchheader.DocTypeId;
                    //    productuid.LastTransactionDocDate = saledispatchheader.DocDate;
                    //    productuid.LastTransactionPersonId = saledispatchheader.SaleToBuyerId;
                    //    productuid.CurrenctGodownId = null;
                    //    productuid.CurrenctProcessId = null;
                    //    productuid.CurrenctProcess = null;
                    //    productuid.Status = ProductUidStatusConstants.Dispatch;
                    //    productuid.IsActive = false;

                    //    productuid.ObjectState = Model.ObjectState.Modified;
                    //    db.ProductUid.Add(productuid);SaleOrderLine.Rate
                    //}


                    saledispatchline.ObjectState = Model.ObjectState.Added;
                    _SaleDispatchLineService.Create(saledispatchline);

                    SaleInvoiceLine saleinvoiceline = new SaleInvoiceLine();
                    saleinvoiceline.SaleDispatchLineId = saledispatchline.SaleDispatchLineId;
                    saleinvoiceline.SaleInvoiceHeaderId = saleinvoiceheaderdetail.SaleInvoiceHeaderId;
                    saleinvoiceline.SaleOrderLineId = packingline.SaleOrderLineId;
                    saleinvoiceline.Qty = packingline.Qty;
                    saleinvoiceline.DealQty = packingline.DealQty;

                    SaleOrderLine SaleOrderLine = db.SaleOrderLine.Where(m => m.SaleOrderLineId == svm.SaleOrderLineId).FirstOrDefault();
                    if (SaleOrderLine != null)
                    {
                        saleinvoiceline.Rate = SaleOrderLine.Rate;
                        saleinvoiceline.Amount = SaleOrderLine.Rate * packingline.DealQty;
                    }
                    saleinvoiceline.DealUnitId = packingline.DealUnitId;
                    saleinvoiceline.UnitConversionMultiplier = packingline.UnitConversionMultiplier;
                    saleinvoiceline.ProductId = packingline.ProductId;
                    saleinvoiceline.Remark = packingline.Remark;
                    saleinvoiceline.RateRemark = packingline.RateRemark;
                    saleinvoiceline.CreatedBy = User.Identity.Name;
                    saleinvoiceline.ModifiedBy = User.Identity.Name;
                    saleinvoiceline.CreatedDate = DateTime.Now;
                    saleinvoiceline.ModifiedDate = DateTime.Now.Date;

                    saleinvoiceline.ObjectState = Model.ObjectState.Added;
                    _SaleInvoiceLineService.Create(saleinvoiceline);


                }


                try
                {
                    _unitOfWork.Save();
                }

                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    ViewBag.DocNo = packingheader.DocNo;
                    ViewBag.LineMode = "Create";
                    TempData["CSEXCL"] += message;
                    Message = message;
                }
            }
             
            ReAssignSessionVariables(svm);
            return Message;
        }
        public void ReAssignSessionVariables(PackingLineViewModel svm)
        {
            //string BaleNoSessionVarName = "CurrentBaleNo"  + svm.PackingHeaderId.ToString().ToString();
            //System.Web.HttpContext.Current.Session[BaleNoSessionVarName] = svm.BaleNo;

            string LastProductSessionVarName = "LastProduct" + svm.PackingHeaderId.ToString().ToString();
            LastProduct lastproduct = new LastProduct();
            lastproduct.ProductId = svm.ProductId;
            lastproduct.ProductName = svm.ProductName;
            lastproduct.Remark = svm.Remark;
            lastproduct.SealNo = svm.SealNo;
            System.Web.HttpContext.Current.Session[LastProductSessionVarName] = lastproduct;


            string PendingToPrintSessionVarName = "PendingToPrint" + svm.PackingHeaderId.ToString().ToString();
            if (System.Web.HttpContext.Current.Session[PendingToPrintSessionVarName] == null)
            {
                System.Web.HttpContext.Current.Session[PendingToPrintSessionVarName] = 1;
            }
            else{
                System.Web.HttpContext.Current.Session[PendingToPrintSessionVarName] = (int)System.Web.HttpContext.Current.Session[PendingToPrintSessionVarName] + 1;
            }


            //string LastProductUidSessionVarName = "LastProductUid" + svm.PackingHeaderId.ToString().ToString();
            //LastProductUid lastproductuid = new LastProductUid();
            //lastproductuid.ProductUidId = svm.ProductId;
            //lastproductuid.ProductName = svm.ProductName;
            //System.Web.HttpContext.Current.Session[LastProductSessionVarName] = lastproduct;

        }

        //public void DisposeSessionVariable(int PackingHeaderId)
        //{
        //    string LastProductSessionVarName = "LastProduct" + PackingHeaderId.ToString().ToString();
        //    string PendingToPrintSessionVarName = "PendingToPrint" + PackingHeaderId.ToString().ToString();

        //    System.Web.HttpContext.Current.Session.Remove(LastProductSessionVarName);
        //    System.Web.HttpContext.Current.Session.Remove(PendingToPrintSessionVarName);
        //}

        public void DisposeSessionPrintVariable(int PackingHeaderId)
        {
            string PendingToPrintSessionVarName = "PendingToPrint" + PackingHeaderId.ToString().ToString();

            System.Web.HttpContext.Current.Session.Remove(PendingToPrintSessionVarName);
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
        public ActionResult _ModifyLineAfterApprove(int id)
        {
            return _Modify(id);
        }


        
     


        [HttpGet]
        private ActionResult _Modify(int id)
        {
            PackingLine temp = _PackingLineService.GetPackingLineForLineId(id);
            SaleOrderLine SOL = new SaleOrderLineService(_unitOfWork).Find((int)temp.SaleOrderLineId);

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


            PackingHeader H = new PackingHeaderService(_unitOfWork).GetPackingHeader(temp.PackingHeaderId);

            ViewBag.DocNo = H.DocNo;
            ViewBag.BaleNoPattern = H.BaleNoPattern;

            PackingLineViewModel s = _PackingLineService.GetPackingLineViewModelForLineId(id);
            s.DocTypeId = H.DocTypeId;

            PackingSetting PackingSetting = new PackingSettingService(_unitOfWork).GetPackingSettingForDocument(H.DocTypeId, H.DivisionId, H.SiteId);
            s.PackingSettings = Mapper.Map<PackingSetting, PackingSettingsViewModel>(PackingSetting);

            if (PackingSetting.isAllowtoUpdateBuyerSpecification == true)
                ViewBag.isAllowtoUpdateBuyerSpecification = "Yes";
            else
                ViewBag.isAllowtoUpdateBuyerSpecification = "No";
            ProductBuyerSettings ProductBuyerSettings = new ProductBuyerSettingsService(_unitOfWork).GetProductBuyerSettings(H.DivisionId, H.SiteId);
            s.ProductBuyerSettings = Mapper.Map<ProductBuyerSettings, ProductBuyerSettingsViewModel>(ProductBuyerSettings);

            //ProductBuyer productbuyer = new ProductBuyerService(_unitOfWork).Find(H.BuyerId, temp.ProductId);

            SaleEnquiryLineViewModel productbuyer = new SaleEnquiryLineService(_unitOfWork).GetSaleEnquiryLineModel((int)SOL.ReferenceDocLineId);
            
            if (productbuyer != null)
            {
                s.BuyerSku = productbuyer.BuyerSku;
                s.BuyerSpecification = productbuyer.BuyerSpecification;
                s.BuyerSpecification1 = productbuyer.BuyerSpecification1;
                s.BuyerSpecification2 = productbuyer.BuyerSpecification2;
                s.BuyerSpecification3 = productbuyer.BuyerSpecification3;
            }

            SaleDispatchLine SDL = new SaleDispatchLineService(_unitOfWork).GetSaleDispatchLineByPackingLineId(id);

            if (SDL != null)
            {
                if (PackingSetting.isPostedInSaleInvoice == true)
                    s.IsLocked = false;
                else
                    s.IsLocked = true;
            }
            else
                s.IsLocked = false;


            PrepareViewBag(s);           
            return PartialView("_Create", s);
        }



        [HttpGet]
        public ActionResult _Detail(int id)
        {
            PackingLine temp = _PackingLineService.GetPackingLineForLineId(id);

            PackingHeader H = new PackingHeaderService(_unitOfWork).GetPackingHeader(temp.PackingHeaderId);
            ViewBag.DocNo = H.DocNo;
            PackingLineViewModel s = _PackingLineService.GetPackingLineViewModelForLineId(id);
            PrepareViewBag(s);

            if (temp == null)
            {
                return HttpNotFound();
            }
            ViewBag.LineMode = "Detail";
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
        public ActionResult _DeleteLine_AfterApprove(int id)
        {
            return _Delete(id);
        }


        
        private ActionResult _Delete(int id)
        {
            PackingLine temp = _PackingLineService.GetPackingLineForLineId(id);
            SaleOrderLine SOL = new SaleOrderLineService(_unitOfWork).Find((int)temp.SaleOrderLineId);

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



            PackingHeader H = new PackingHeaderService(_unitOfWork).GetPackingHeader(temp.PackingHeaderId);
            ViewBag.DocNo = H.DocNo;
            PackingLineViewModel s = _PackingLineService.GetPackingLineViewModelForLineId(id);

            ProductBuyerSettings ProductBuyerSettings = new ProductBuyerSettingsService(_unitOfWork).GetProductBuyerSettings(H.DivisionId, H.SiteId);
            s.ProductBuyerSettings = Mapper.Map<ProductBuyerSettings, ProductBuyerSettingsViewModel>(ProductBuyerSettings);

            ViewBag.BaleNoPattern = H.BaleNoPattern;
            PackingSetting PackingSetting = new PackingSettingService(_unitOfWork).GetPackingSettingForDocument(H.DocTypeId, H.DivisionId, H.SiteId);
            s.PackingSettings = Mapper.Map<PackingSetting, PackingSettingsViewModel>(PackingSetting);
            if (PackingSetting.isAllowtoUpdateBuyerSpecification == true)
                ViewBag.isAllowtoUpdateBuyerSpecification = "Yes";
            else
                ViewBag.isAllowtoUpdateBuyerSpecification = "No";
            //ProductBuyer productbuyer = new ProductBuyerService(_unitOfWork).Find(H.BuyerId, temp.ProductId);
            SaleEnquiryLineViewModel productbuyer = new SaleEnquiryLineService(_unitOfWork).GetSaleEnquiryLineModel((int)SOL.ReferenceDocLineId);

            if (productbuyer != null)
            {
                s.BuyerSku = productbuyer.BuyerSku;
                s.BuyerSpecification = productbuyer.BuyerSpecification;
                s.BuyerSpecification1 = productbuyer.BuyerSpecification1;
                s.BuyerSpecification2 = productbuyer.BuyerSpecification2;
                s.BuyerSpecification3 = productbuyer.BuyerSpecification3;
            }

            if (productbuyer != null)
            {
                s.BuyerSku = productbuyer.BuyerSku;
                s.BuyerSpecification = productbuyer.BuyerSpecification;
                s.BuyerSpecification1 = productbuyer.BuyerSpecification1;
                s.BuyerSpecification2 = productbuyer.BuyerSpecification2;
                s.BuyerSpecification3 = productbuyer.BuyerSpecification3;
            }

            PrepareViewBag(s);            
            return PartialView("_Create", s);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePost(PackingLineViewModel vm)
        {
            int? StockIssueId = 0;
            int? StockReceiveId = 0;
            List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();


            PackingLine PackingLine = _PackingLineService.GetPackingLineForLineId(vm.PackingLineId);
            PackingHeader PackingHeader = new PackingHeaderService(_unitOfWork).Find(PackingLine.PackingHeaderId);

            PackingSetting PackingSetting = new PackingSettingService(_unitOfWork).GetPackingSettingForDocument(PackingHeader.DocTypeId, PackingHeader.DivisionId, PackingHeader.SiteId);
            int status = PackingHeader.Status;
            SaleOrderLine SaleOrderLine = null;
            if (PackingLine.SaleOrderLineId != null)
            {
                SaleOrderLine = new SaleOrderLineService(_unitOfWork).Find((int)PackingLine.SaleOrderLineId);
            }

            LogList.Add(new LogTypeViewModel
            {
                ExObj = PackingLine,
            });

            StockIssueId = PackingLine.StockIssueId;
            StockReceiveId = PackingLine.StockReceiveId;



           

            //StockViewModel StockViewModel_OldIssue = GetStockViewModelIssueForPackingLine(PackingLine.PackingLineId);
            //StockViewModel StockViewModel_OldReceive = GetStockViewModelReceiveForPackingLine(PackingLine.PackingLineId);

            //string StockPostingError = "";

            //StockPostingError = new StockService(_unitOfWork).StockPost(null, StockViewModel_OldIssue);
            //StockPostingError = new StockService(_unitOfWork).StockPost(null, StockViewModel_OldReceive);

            //if (StockPostingError != "")
            //{
            //    ModelState.AddModelError("", StockPostingError);
            //    return PartialView("_Create", vm);
            //}


            if (SaleOrderLine != null)
            {
                SaleEnquiryLine SEL = new SaleEnquiryLineService(_unitOfWork).Find((int)SaleOrderLine.ReferenceDocLineId);

                if (SaleOrderLine.ReferenceDocTypeId == PackingHeader.DocTypeId)
                {
                    new SaleOrderLineStatusService(_unitOfWork).Delete((SaleOrderLine.SaleOrderLineId));
                    new SaleOrderLineService(_unitOfWork).Delete(SaleOrderLine);
                }

                if (SEL != null)
                {
                    if (SEL.ReferenceDocTypeId == PackingHeader.DocTypeId)
                    {
                        new SaleEnquiryLineExtendedService(_unitOfWork).Delete((SEL.SaleEnquiryLineId));
                        new SaleEnquiryLineService(_unitOfWork).Delete(SEL.SaleEnquiryLineId);
                    }

                }


            }
            


            if (PackingLine.ProductUidId != null && PackingLine.ProductUidId != 0)
            {
                //ProductUidDetail ProductUidDetail = new ProductUidService(_unitOfWork).FGetProductUidLastValues((int)PackingLine.ProductUidId, "Packing-" +  vm.PackingHeaderId.ToString());

                ProductUid ProductUid = new ProductUidService(_unitOfWork).Find((int)PackingLine.ProductUidId);


                ProductUid.LastTransactionDocDate = PackingLine.ProductUidLastTransactionDocDate;
                ProductUid.LastTransactionDocId = PackingLine.ProductUidLastTransactionDocId;
                ProductUid.LastTransactionDocNo = PackingLine.ProductUidLastTransactionDocNo;
                ProductUid.LastTransactionDocTypeId = PackingLine.ProductUidLastTransactionDocTypeId;
                ProductUid.LastTransactionPersonId = PackingLine.ProductUidLastTransactionPersonId;
                ProductUid.CurrenctGodownId = PackingLine.ProductUidCurrentGodownId;
                ProductUid.CurrenctProcessId = PackingLine.ProductUidCurrentProcessId;
                ProductUid.Status = PackingLine.ProductUidStatus;
                ProductUid.IsActive = true;
                if (ProductUid.ProcessesDone !=null)
                {
                    ProductUid.ProcessesDone = ProductUid.ProcessesDone.Replace("|" + new ProcessService(_unitOfWork).Find(ProcessConstants.Packing).ProcessId.ToString() + "|", "");
                }                

                //ProductUid.LastTransactionDocDate = ProductUidDetail.LastTransactionDocDate;
                //ProductUid.LastTransactionDocId = ProductUidDetail.LastTransactionDocId;
                //ProductUid.LastTransactionDocNo = ProductUidDetail.LastTransactionDocNo;
                //ProductUid.LastTransactionDocTypeId = ProductUidDetail.LastTransactionDocTypeId;
                //ProductUid.LastTransactionPersonId = ProductUidDetail.LastTransactionPersonId;
                //ProductUid.CurrenctGodownId = ProductUidDetail.CurrenctGodownId;
                //ProductUid.CurrenctProcessId = ProductUidDetail.CurrenctProcessId;
                //ProductUid.Status = ProductUidDetail.Status;
                

                //new ProductUidService(_unitOfWork).Update(ProductUid);

                new StockUidService(_unitOfWork).DeleteStockUidForDocLine(PackingLine.PackingLineId, PackingHeader.DocTypeId, PackingHeader.SiteId, PackingHeader.DivisionId);
            }


            if (PackingLine.LotNo  != null && PackingLine.ProductUidId == null )
            {
                //ProductUidDetail ProductUidDetail = new ProductUidService(_unitOfWork).FGetProductUidLastValues((int)PackingLine.ProductUidId, "Packing-" +  vm.PackingHeaderId.ToString());

                ProductUid ProductUid = new ProductUidService(_unitOfWork).Find(PackingLine.LotNo);

                if (ProductUid != null)
                {

                    ProductUid.LastTransactionDocDate = PackingLine.ProductUidLastTransactionDocDate;
                    ProductUid.LastTransactionDocId = PackingLine.ProductUidLastTransactionDocId;
                    ProductUid.LastTransactionDocNo = PackingLine.ProductUidLastTransactionDocNo;
                    ProductUid.LastTransactionDocTypeId = PackingLine.ProductUidLastTransactionDocTypeId;
                    ProductUid.LastTransactionPersonId = PackingLine.ProductUidLastTransactionPersonId;
                    ProductUid.CurrenctGodownId = PackingLine.ProductUidCurrentGodownId;
                    ProductUid.CurrenctProcessId = PackingLine.ProductUidCurrentProcessId;
                    ProductUid.Status = PackingLine.ProductUidStatus;
                    ProductUid.IsActive = true;
                    if (ProductUid.ProcessesDone != null)
                        ProductUid.ProcessesDone = ProductUid.ProcessesDone.Replace("|" + new ProcessService(_unitOfWork).Find(ProcessConstants.Packing).ProcessId.ToString() + "|", "");

                }
                //ProductUid.LastTransactionDocDate = ProductUidDetail.LastTransactionDocDate;
                //ProductUid.LastTransactionDocId = ProductUidDetail.LastTransactionDocId;
                //ProductUid.LastTransactionDocNo = ProductUidDetail.LastTransactionDocNo;
                //ProductUid.LastTransactionDocTypeId = ProductUidDetail.LastTransactionDocTypeId;
                //ProductUid.LastTransactionPersonId = ProductUidDetail.LastTransactionPersonId;
                //ProductUid.CurrenctGodownId = ProductUidDetail.CurrenctGodownId;
                //ProductUid.CurrenctProcessId = ProductUidDetail.CurrenctProcessId;
                //ProductUid.Status = ProductUidDetail.Status;


                //new ProductUidService(_unitOfWork).Update(ProductUid);

                new StockUidService(_unitOfWork).DeleteStockUidForDocLine(PackingLine.PackingLineId, PackingHeader.DocTypeId, PackingHeader.SiteId, PackingHeader.DivisionId);
            }

            if (PackingSetting.isPostedInSaleInvoice == true)
            {

                SaleDispatchLine saledispatchline = db.SaleDispatchLine.Where(m => m.PackingLineId == vm.PackingLineId).FirstOrDefault();
                SaleInvoiceLine saleinvoiceline = db.SaleInvoiceLine.Where(m => m.SaleDispatchLineId == saledispatchline.SaleDispatchLineId).FirstOrDefault();

                _SaleInvoiceLineService.Delete(saleinvoiceline.SaleInvoiceLineId);
                _SaleDispatchLineService.Delete(saledispatchline.SaleDispatchLineId);
            }

            new PackingLineExtendedService(_unitOfWork).Delete(vm.PackingLineId);
            _PackingLineService.Delete(vm.PackingLineId);




            if (StockIssueId != null)
            {
                StockAdj Adj = (from L in db.StockAdj
                                where L.StockOutId == StockIssueId
                                select L).FirstOrDefault();

                if (Adj != null)
                {
                    new StockAdjService(_unitOfWork).Delete(Adj);
                    //Adj.ObjectState = Model.ObjectState.Deleted;
                    //db.StockAdj.Remove(Adj);
                }

                new StockService(_unitOfWork).DeleteStock((int)StockIssueId);
            }

            if (StockReceiveId != null)
            {
                new StockService(_unitOfWork).DeleteStock((int)StockReceiveId);
            }


            if (PackingHeader.Status != (int)StatusConstants.Drafted)
            {
                PackingHeader.Status = (int)StatusConstants.Modified;
                new PackingHeaderService(_unitOfWork).Update(PackingHeader);
            }
            XElement Modifications = new ModificationsCheckService().CheckChanges(LogList);
            try
            {
                _unitOfWork.Save();
            }
           
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                ViewBag.DocNo = PackingHeader.DocNo;
                ViewBag.LineMode = "Delete";
                PrepareViewBag(vm);
                TempData["CSEXCL"] += message;
                return PartialView("_Create", vm);
            }

            LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
            {
                DocTypeId = PackingHeader.DocTypeId,
                DocId = PackingLine.PackingHeaderId,
                DocLineId = PackingLine.PackingLineId,
                ActivityType = (int)ActivityTypeContants.Deleted,
                DocNo = PackingHeader.DocNo,
                xEModifications = Modifications,
                DocDate = PackingHeader.DocDate,
                DocStatus = PackingHeader.Status,
            }));

            return Json(new { success = true });
        }

        //public void StockPost(PackingHeader H, PackingLine L)
        //{
        //    //To Issue Stock For Packing 
        //    Stock Stock_Iss = new Stock()
        //    {
        //        DocHeaderId = H.PackingHeaderId,
        //        DocLineId = L.PackingLineId,
        //        DocTypeId = H.DocTypeId,
        //        DocNo = H.DocNo,
        //        DocDate = H.DocDate,
        //        DivisionId = H.DivisionId,
        //        SiteId = H.SiteId,
        //        CurrencyId = null,
        //        PersonId = H.BuyerId,
        //        GodownId = H.GodownId,
        //        ProductId = L.ProductId,
        //        ProcessId = L.FromProcessId,
        //        LotNo = null,
        //        CostCenterId = null,
        //        Qty_Iss = L.Qty,
        //        Qty_Rec = 0,
        //        Rate = null,
        //        ExpiryDate = null,
        //        Specification = null,
        //        Dimension1Id = null,
        //        Dimension2Id = null
        //    };
        //    _StockService.Create(Stock_Iss);

        //    //To Receive Stock From Packing 
        //    Stock Stock_Rec = new Stock()
        //    {
        //        DocHeaderId = H.PackingHeaderId,
        //        DocLineId = L.PackingLineId,
        //        DocTypeId = H.DocTypeId,
        //        DocNo = H.DocNo,
        //        DocDate = H.DocDate,
        //        DivisionId = H.DivisionId,
        //        SiteId = H.SiteId,
        //        CurrencyId = null,
        //        PersonId = H.BuyerId,
        //        GodownId = H.GodownId,
        //        ProductId = L.ProductId,
        //        ProcessId = null,
        //        LotNo = H.DocNo,
        //        CostCenterId = null,
        //        Qty_Iss = 0,
        //        Qty_Rec = L.Qty,
        //        Rate = null,
        //        ExpiryDate = null,
        //        Specification = null,
        //        Dimension1Id = null,
        //        Dimension2Id = null
        //    };
        //    _StockService.Create(Stock_Rec);
        //}

        public string DataValidation(PackingLineViewModel svm)
        {

            PackingHeader PH = new PackingHeaderService(_unitOfWork).GetPackingHeader(svm.PackingHeaderId);

            PackingSetting PS =new PackingSettingService(_unitOfWork).GetPackingSettingForDocument(PH.DocTypeId, PH.DivisionId, PH.SiteId);
            string ValidationMsg = "";

            if (PS.IsMandatoryStockIn == true  )
            {
                if (svm.StockInId == null)
                {
                    ValidationMsg = "StockIn No is required.";
                    return ValidationMsg;
                }
            }

            if (svm.Qty <= 0)
            {
                ValidationMsg = "Qty is required.";
                return ValidationMsg;
            }

            if (svm.DealQty <= 0)
            {
                ValidationMsg = "Deal Qty is required.";
                return ValidationMsg;
            }

            if (svm.ProductId == 0 )
            {
                ValidationMsg = "Product is required.";
                return ValidationMsg;
            }

            if (svm.UnitConversionMultiplier > (Decimal) 6.00 && svm.Qty > 1 && svm.DealUnitId != UnitConstants.Pieces && svm.DealUnitId != UnitConstants.Kgs)
            {
                ValidationMsg = "Packed Qty can not be greater then 1.";
                return ValidationMsg;
            }

            if (svm.CreateExtraSaleOrder == false)
            {
                if (svm.SaleOrderLineId == 0 || svm.SaleOrderLineId == null)
                {
                    ValidationMsg = "Sale Order is required.";
                    return ValidationMsg;
                }
            }

            if (svm.BaleNo == "" || svm.BaleNo == null)
            {
                ValidationMsg = "Bale is required.";
                return ValidationMsg;
            }

            //if (svm.ProductUidName != "" && svm.ProductUidName != null && svm.Qty > 1)
            //{
            //    ValidationMsg = "Qty can't be greater then 1 for a Bar Code.";
            //    return ValidationMsg;
            //}

            if (svm.GrossWeight > Decimal.MaxValue || svm.GrossWeight < Decimal.MinValue)
            {
                ValidationMsg = "Gross weight value is out of range.";
                return ValidationMsg;
            }

            if (svm.NetWeight > Decimal.MaxValue || svm.NetWeight < Decimal.MinValue)
            {
                ValidationMsg = "Net weight value is out of range.";
                return ValidationMsg;
            }

            if (svm.StockInId !=null)
            {
                Decimal BalanceQty = 0;
                if (PS.isAllowtoDirectPacking == true)
                    BalanceQty = (from L in db.ViewStockInBalanceForPacking
                                     where L.StockInId == svm.StockInId
                                     select new { BalanceQty = L.BalanceQty }).FirstOrDefault().BalanceQty;
                else 
                    BalanceQty = (from L in db.ViewStockInBalance
                                  where L.StockInId == svm.StockInId
                                  select new { BalanceQty = L.BalanceQty }).FirstOrDefault().BalanceQty;

                if (BalanceQty < svm.Qty)
                {
                    ValidationMsg = "Qty can't be greater then " + BalanceQty.ToString(); ;
                    return ValidationMsg;
                }
            }

            //if (svm.ProductUidName != "" && svm.ProductUidName != null)
            //{
            //    int ProductUidCnt = (from L in db.PackingLine where L.ProductUidId == svm.ProductUidId && L.PackingLineId != svm.PackingLineId select new { Cnt = L.ProductUidId }).Count();

            //    if (ProductUidCnt != 0)
            //    {
            //        ValidationMsg = "Bar Code is already packed.";
            //        return ValidationMsg;
            //    }

            //    var ProductUidStock = (from L in db.StockUid
            //                           where L.ProductUidId == svm.ProductUidId
            //                           group new { L } by new { L.ProductUidId } into Result
            //                           select new
            //                           {
            //                               ProductUidStockValue = Result.Sum(i => i.L.Qty_Rec) - Result.Sum(i => i.L.Qty_Iss)
            //                           }).FirstOrDefault();


            //    if (ProductUidStock  == null)
            //    {
            //        ValidationMsg = "Bar Code does not exist in stock.";
            //        return ValidationMsg;
            //    }
            //    else if (ProductUidStock.ProductUidStockValue == 0)
            //    {
            //        ValidationMsg = "Bar Code does not exist in stock.";
            //        return ValidationMsg;
            //    }

            //    ProductUid Pu = new ProductUidService(_unitOfWork).Find(svm.ProductName);
            //    if (Pu.ProductId != Pu.ProductId)
            //    {
            //        ValidationMsg = "Bar Code does not belong to " + svm.ProductName;
            //        return ValidationMsg;
            //    }
            //}












            if (svm.ProductUidName != "" && svm.ProductUidName != null)
            {
                ProductUidDetail ProductUidDetail = new ProductUidService(_unitOfWork).FGetProductUidDetail(svm.ProductUidName);

                if (ProductUidDetail != null)
                {
                    if (ProductUidDetail.PrevProcessName == ProcessConstants.Packing && ProductUidDetail.Status == ProductUidStatusConstants.Pack)
                    {
                        ValidationMsg = "Bar Code is already packed.";
                        return ValidationMsg;
                    }

                    //if (ProductUidDetail.Status == ProductUidStatusConstants.Issue && ProductUidDetail.PrevProcessName != ProcessConstants.Packing)
                    //{
                    //    ValidationMsg = "Bar Code is issued to " + ProductUidDetail.LastTransactionPersonName + " for " + ProductUidDetail.PrevProcessName + " on date " + ProductUidDetail.LastTransactionDocDate + ".";
                    //    return ValidationMsg;
                    //}

                    if (ProductUidDetail.ProductId != svm.ProductId)
                    {
                        ValidationMsg = "Bar Code does not belong to " + svm.ProductName;
                        return ValidationMsg;
                    }

                    //if (ProductUidDetail.CurrenctGodownId != svm.GodownId)
                    //{
                    //    ValidationMsg = "Bar Code is not Present in " + svm.GodownName;
                    //    return ValidationMsg;
                    //}


                    if (svm.Qty > 1)
                    {
                        ValidationMsg = "Qty can't be greater then 1.";
                        return ValidationMsg;
                    }


                    if (ProductUidDetail.DivisionId == (int)DivisionEnum.TUFTED)
                    {
                        if ((ProductUidDetail.GenDocTypeId ?? 0) == new DocumentTypeService(_unitOfWork).Find(TransactionDoctypeConstants.WeavingOrder).DocumentTypeId)
                        {
                            if (new ProductUidService(_unitOfWork).IsProcessDone((int)svm.ProductUidId, new ProcessService(_unitOfWork).Find(ProcessConstants.FullFinishing).ProcessId) == false)
                            {
                                if (new ProductUidService(_unitOfWork).IsProcessDone((int)svm.ProductUidId, new ProcessService(_unitOfWork).Find(ProcessConstants.ThirdBacking).ProcessId) == false)
                                {
                                    ValidationMsg = "Third Backing Process is not done for Barcode + " + svm.ProductUidName + ".";

                                    return ValidationMsg;
                                }
                            }
                        }
                    }
                    
                }
                else
                {
                    ValidationMsg = "Invalid Bar Code.";
                    return ValidationMsg;
                }
            }


            

            //var SaleOrderPacking = from L in db.PackingLine
            //                       where L.SaleOrderLineId == svm.SaleOrderLineId && L.PackingLineId != svm.PackingLineId 
            //                       group new { L } by new { L.SaleOrderLineId } into Result
            //                       select new
            //                       {
            //                           SaleOrderLineId = Result.Key.SaleOrderLineId,
            //                           PackedQty = Result.Sum(i => i.L.Qty)
            //                       };


            //var PendingSaleOrderForPacking = (from L in db.SaleOrderLine
            //                                  join VPack in SaleOrderPacking on L.SaleOrderLineId equals VPack.SaleOrderLineId into VPackTable
            //                                  from VPackTab in VPackTable.DefaultIfEmpty()
            //                                  where L.SaleOrderLineId == svm.SaleOrderLineId
            //                                  group new { L, VPackTab } by new { L.SaleOrderLineId } into Result
            //                                  select new
            //                                  {
            //                                      SaleOrderLineItem = Result.Max(i=>i.L.ProductId),
            //                                      SaleOrderLineBalanceFprPacking = ((Decimal?)Result.Sum(i => i.L.Qty) ?? 0) - ((Decimal?)Result.Sum(i => i.VPackTab.PackedQty) ?? 0)

            //                                  }).FirstOrDefault();

            //if (svm.ProductId != PendingSaleOrderForPacking.SaleOrderLineItem)
            //{
            //    ValidationMsg = "Product " + svm.ProductName +  "doesn't exist in PO" + svm.SaleOrderNo;
            //    return ValidationMsg;
            //}


            if (svm.SaleDeliveryOrderLineId != 0 && svm.SaleDeliveryOrderLineId != null)
            {
                Decimal PendingDeliveryOrderQtyForPacking = _PackingLineService.FGetPendingDeliveryOrderQtyForPacking((int)svm.SaleDeliveryOrderLineId, svm.PackingLineId);

                if (svm.Qty > PendingDeliveryOrderQtyForPacking)
                {
                    ValidationMsg = "Pending Qty For Item : " + svm.ProductName + " And Delivery Order : " + svm.SaleOrderNo + " is < " + PendingDeliveryOrderQtyForPacking.ToString() + "> And Packing Qty is < " + svm.Qty + ">. Can't Continue.";
                    return ValidationMsg;
                }

                Decimal PendingDeliveryOrderQtyForDispatch = _PackingLineService.FGetPendingDeliveryOrderQtyForDispatch((int)svm.SaleDeliveryOrderLineId);

                if (svm.Qty > PendingDeliveryOrderQtyForDispatch)
                {
                    ValidationMsg = "Pending Qty For Item : " + svm.ProductName + " And Delivery Order : " + svm.SaleOrderNo + " is < " + PendingDeliveryOrderQtyForDispatch.ToString() + "> And Packing Qty is < " + svm.Qty + ">. Can't Continue.";
                    return ValidationMsg;
                }
            }
            else
            {
                if (svm.SaleOrderLineId != null)
                {
                    var SaleOrderHeader = (from L in db.SaleOrderLine
                                          join H in db.SaleOrderHeader on L.SaleOrderHeaderId equals H.SaleOrderHeaderId into SaleOrderHeaderTable
                                          from SaleOrderHeaderTab in SaleOrderHeaderTable.DefaultIfEmpty()
                                          where L.SaleOrderLineId == svm.SaleOrderLineId
                                           select new
                                           {
                                               ShipMethodId = SaleOrderHeaderTab.ShipMethodId,
                                               ShipMethodName = SaleOrderHeaderTab.ShipMethod.ShipMethodName,
                                               SaleOrderNo = SaleOrderHeaderTab.BuyerOrderNo
                                           }).FirstOrDefault();


                    var PackingHeader = new PackingHeaderService(_unitOfWork).Find(svm.PackingHeaderId);

                    if (SaleOrderHeader != null)
                    {
                        if (SaleOrderHeader.ShipMethodId != PackingHeader.ShipMethodId)
                        {
                            ValidationMsg = "Product " + svm.ProductName + " should be shipped "+ SaleOrderHeader.ShipMethodName +".That is defined in Sale Order " + SaleOrderHeader.SaleOrderNo   + ".";
                            return ValidationMsg;
                        }
                    }
                }
            }



            if (svm.CreateExtraSaleOrder == false)
            {
                Decimal PendingOrderQtyForPacking = _PackingLineService.FGetPendingOrderQtyForPacking((int)svm.SaleOrderLineId, svm.PackingLineId);

                if (svm.Qty > PendingOrderQtyForPacking)
                {
                    ValidationMsg = "Pending Qty For Item : " + svm.ProductName + " And Sale Order : " + svm.SaleOrderNo + " is < " + PendingOrderQtyForPacking.ToString() + "> And Packing Qty is < " + svm.Qty + ">. Can't Continue.";
                    return ValidationMsg;
                }

                Decimal PendingOrderQtyForDispatch = _PackingLineService.FGetPendingOrderQtyForDispatch((int)svm.SaleOrderLineId);

                if (svm.Qty > PendingOrderQtyForDispatch)
                {
                    ValidationMsg = "Pending Qty For Item : " + svm.ProductName + " And Sale Order : " + svm.SaleOrderNo + " is < " + PendingOrderQtyForDispatch.ToString() + "> And Packing Qty is < " + svm.Qty + ">. Can't Continue.";
                    return ValidationMsg;
                }

                if (new PackingLineService(_unitOfWork).FSaleOrderProductMatchWithPacking((int)svm.SaleOrderLineId,svm.ProductId) == false)
                {
                    ValidationMsg = "Product : " + svm.ProductName + " does not exist in Sale Order : " + svm.SaleOrderNo + " . Can't Submit Record.";
                    return ValidationMsg;
                }
            }

#region "This function was created for stock validation but due to data is not fully migrated in stock table we have to create stock validation procedure on basis of old stock tables.When data will be fully migrated then we will use this code."
            

            //var ProductPacking = (from L in db.PackingLine
            //                      where L.ProductId == svm.ProductId
            //                      group new { L } by new { L.ProductId } into Result
            //                      select new
            //                      {
            //                          PackedQty = Result.Sum(i => i.L.Qty)
            //                      }).FirstOrDefault();

            //var ProductDispatch = (from L in db.SaleDispatchLine
            //                       join P in db.PackingLine on L.PackingLineId equals P.PackingLineId into PackingTable
            //                       from PackingTab in PackingTable.DefaultIfEmpty()
            //                       where PackingTab.ProductId == svm.ProductId
            //                       group new { PackingTab } by new { PackingTab.ProductId } into Result
            //                       select new
            //                       {
            //                           DispatchedQty = Result.Sum(i => i.PackingTab.Qty)
            //                       }).FirstOrDefault();


            //int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            //var ProductStockAndStockProcess = (from L in db.Stock
            //                                   join H in db.StockHeader on L.StockHeaderId equals H.StockHeaderId into StockHeaderTable
            //                                   from StockHeaderTab in StockHeaderTable.DefaultIfEmpty()
            //                                   where L.ProductId == svm.ProductId && StockHeaderTab.SiteId == SiteId
            //                                   select new
            //                                   {
            //                                       ProductId = L.ProductId,
            //                                       Qty_Iss = L.Qty_Iss,
            //                                       Qty_Rec = L.Qty_Rec
            //                                   }).
            //                      Union(
            //                      from L in db.StockProcess
            //                      join H in db.StockHeader on L.StockHeaderId equals H.StockHeaderId into StockHeaderTable
            //                      from StockHeaderTab in StockHeaderTable.DefaultIfEmpty()
            //                      where L.ProductId == svm.ProductId && StockHeaderTab.SiteId == SiteId
            //                      select new
            //                      {
            //                          ProductId = L.ProductId,
            //                          Qty_Iss = L.Qty_Iss,
            //                          Qty_Rec = L.Qty_Rec
            //                      });

            //var ProductFinalStock = (from VStock in ProductStockAndStockProcess
            //                         group new { VStock } by new { VStock.ProductId } into Result
            //                         select new
            //                         {
            //                             StockQty = Result.Sum(i => i.VStock.Qty_Rec) - Result.Sum(i => i.VStock.Qty_Iss)
            //                         }).FirstOrDefault();

            //if (ProductFinalStock != null && ProductPacking != null && ProductDispatch != null)
            //{
            //    if (svm.Qty > ProductFinalStock.StockQty - (ProductPacking.PackedQty - ProductDispatch.DispatchedQty))
            //    {
            //        ValidationMsg = "Stock for Item : " + svm.ProductName + " Is Less Then " + svm.Qty.ToString() + ">. Can't Continue.";
            //        return ValidationMsg;
            //    }
            //}
            //else{
            //    ValidationMsg = "Stock for Item : " + svm.ProductName + " Is Less Then " + svm.Qty.ToString() + ">. Can't Continue.";
            //    return ValidationMsg;
            //}
#endregion


            try
            { 
                Decimal StockQty = 0;
                Decimal StockInQCQty = 0;
                int ProductId = svm.ProductId;
                int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            
                //StockQty = _PackingLineService.FGetStockForPacking(ProductId, SiteId, svm.PackingLineId);
                StockQty = _PackingLineService.FGetStockForPacking(ProductId, SiteId, svm.GodownId);

                if (svm.Qty > StockQty)
                {
                    ValidationMsg = "Stock for Item : " + svm.ProductName + " Is Less Then <" + svm.Qty.ToString() + ">. Can't Continue.";
                    return ValidationMsg;
                }


                StockInQCQty = _PackingLineService.FGetQCStockForPacking(ProductId, SiteId, StockQty);

                if (svm.Qty > StockInQCQty)
                {
                    ValidationMsg = "Stock for Item : " + svm.ProductName + " Is Less Then <" + svm.Qty.ToString() + ">.It is pending in Quality Checking. Can't Continue.";
                    return ValidationMsg;
                }
            }
            
            catch(Exception e)
            {
                ValidationMsg = e.Message;
                return ValidationMsg;
            }
            return ValidationMsg;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public JsonResult GetUnitConversionDetailJson(int ProductId, string UnitId, string DeliveryUnitId)
        {
            UnitConversion uc = new UnitConversionService(_unitOfWork).GetUnitConversion(ProductId, UnitId, DeliveryUnitId);
            List<SelectListItem> UnitConversionJson = new List<SelectListItem>();
            if (uc != null)
            {
                UnitConversionJson.Add(new SelectListItem
                {
                    Text = uc.Multiplier.ToString(),
                    Value = uc.Multiplier.ToString()
                });
            }
            else
            {
                UnitConversionJson.Add(new SelectListItem
                {
                    Text = "0",
                    Value = "0"
                });
            }


            return Json(UnitConversionJson);
        }

        public JsonResult GetProductDetailJson(int ProductId, int PackingHeaderId)
        {
            var product = (from p in db.FinishedProduct
                           join Pig in db.ProductInvoiceGroup on p.ProductInvoiceGroupId equals Pig.ProductInvoiceGroupId into ProductInvoiceGroupTable
                           from ProductInvoiceGroupTab in ProductInvoiceGroupTable.DefaultIfEmpty()
                           where p.ProductId == ProductId
                           select new
                           {
                               UnitId = p.UnitId,
                               ImageFolderName = p.ImageFolderName,
                               ImageFileName = p.ImageFileName,
                               ProductInvoiceGroupId = p.ProductInvoiceGroupId,
                               ProductInvoiceGroupName = ProductInvoiceGroupTab.ProductInvoiceGroupName,
                               IsSample = p.IsSample
                           }).FirstOrDefault();



            List<ProductViewModel> ProductJson = new List<ProductViewModel>();


            var DeliveryUnit = (from H in db.PackingHeader
                                where H.PackingHeaderId == PackingHeaderId
                                select new { DeliveryUnitId = H.DealUnitId }).FirstOrDefault();

            ProductAreaDetail productarea = _PackingLineService.FGetProductArea(ProductId);

            int a = (int)ProductSizeTypeConstants.StandardSize;          

            ProductSize ps = new ProductSizeService(_unitOfWork).FindProductSize(a, ProductId);
            Size s = new SizeService(_unitOfWork).Find(ps.SizeId);

            decimal AreaFT2=0;

            if (s.UnitId == "MET")
            {
                using (SqlConnection sqlConnection = new SqlConnection((string)System.Web.HttpContext.Current.Session["DefaultConnectionString"]))
                {
                    sqlConnection.Open();

                    SqlCommand Totalf = new SqlCommand("SELECT * FROM Web.FuncGetSqFeetFromCMSize( " + s.SizeId + ")", sqlConnection);

                    AreaFT2 = Convert.ToDecimal(Totalf.ExecuteScalar() == DBNull.Value ? 0 : Totalf.ExecuteScalar());
                }

                if (productarea != null)
                {
                    productarea.ProductArea = AreaFT2;
                }
            }



            Decimal AreaInDeliveryUnit = 0;

            if (productarea != null)
            {
                if (DeliveryUnit.DeliveryUnitId == "FT2")
                {
                    AreaInDeliveryUnit = Math.Round(productarea.ProductArea, 4);
                }
                else if (DeliveryUnit.DeliveryUnitId.ToUpper() == "MT2")
                {
                    AreaInDeliveryUnit = Math.Round((productarea.ProductArea * (Decimal)0.09), 4);
                }
                else if (DeliveryUnit.DeliveryUnitId.ToUpper() == "YD2")
                {
                    AreaInDeliveryUnit = Math.Round((productarea.ProductArea * (Decimal)0.111111111), 4);
                }
                else if (DeliveryUnit.DeliveryUnitId.ToUpper() == "PCS")
                {
                    AreaInDeliveryUnit = 1;
                }
                else
                {
                    AreaInDeliveryUnit = 1;
                }
            }

            Product Product = new ProductService(_unitOfWork).Find(ProductId);

            if (Product.UnitId == DeliveryUnit.DeliveryUnitId)
            {
                AreaInDeliveryUnit = 1;
            }           

            ProductJson.Add(new ProductViewModel()
            {
                UnitId = product.UnitId,
                ImageFolderName = product.ImageFolderName,
                ImageFileName = product.ImageFileName,
                ProductInvoiceGroupId = product.ProductInvoiceGroupId,
                ProductInvoiceGroupName = product.ProductInvoiceGroupName,
                IsSample = product.IsSample,
                NField1=AreaInDeliveryUnit,
            });

            return Json(ProductJson);
        }


        public JsonResult GetProductUIDDetailJson(string ProductUIDNo, int PackingHeaderId)
        {
            ProductUidDetail productuiddetail = new ProductUidService(_unitOfWork).FGetProductUidDetail(ProductUIDNo);

            List<ProductUidDetail> ProductUidDetailJson = new List<ProductUidDetail>();

            if (productuiddetail != null)
            {


                //var DeliveryUnit = (from H in db.PackingHeader
                //                    where H.PackingHeaderId == PackingHeaderId
                //                    select new { DeliveryUnitId = H.DealUnitId }).FirstOrDefault();

                //ProductAreaDetail productarea = _PackingLineService.FGetProductArea(productuiddetail.ProductId);

                //Decimal AreaInDeliveryUnit = 0;

                //if (productarea != null)
                //{
                //    if (DeliveryUnit.DeliveryUnitId == "FT2")
                //    {
                //        AreaInDeliveryUnit = Math.Round(productarea.ProductArea, 4);
                //    }
                //    else if (DeliveryUnit.DeliveryUnitId.ToUpper() == "MT2")
                //    {
                //        AreaInDeliveryUnit = Math.Round((productarea.ProductArea * (Decimal)0.09), 4);
                //    }
                //    else if (DeliveryUnit.DeliveryUnitId.ToUpper() == "YD2")
                //    {
                //        AreaInDeliveryUnit = Math.Round((productarea.ProductArea * (Decimal)0.111111111), 4);
                //    }
                //    else if (DeliveryUnit.DeliveryUnitId.ToUpper() == "PCS")
                //    {
                //        AreaInDeliveryUnit = 1;
                //    }
                //    else
                //    {
                //        AreaInDeliveryUnit = 1;
                //    }
                //}

                //Product Product = new ProductService(_unitOfWork).Find(productuiddetail.ProductId);

                //if (Product.UnitId == DeliveryUnit.DeliveryUnitId)
                //{
                //    AreaInDeliveryUnit = 1;
                //}           


                ProductUidDetailJson.Add(new ProductUidDetail()
                {
                    ProductId = productuiddetail.ProductId,
                    Status= productuiddetail.Status, 
                    ProductName = productuiddetail.ProductName,
                    ProductUidId = productuiddetail.ProductUidId,
                    PrevProcessId = productuiddetail.PrevProcessId,
                    PrevProcessName = productuiddetail.PrevProcessName,
                    DivisionId = productuiddetail.DivisionId,
                    //NField1=AreaInDeliveryUnit,
                });
            }


           


            return Json(ProductUidDetailJson);
        }




        //public JsonResult GetProductSaleOrderFifoJson(int ProductId, int PackingHeaderId)
        //{
        //    int BuyerId = (from H in db.PackingHeader where H.PackingHeaderId == PackingHeaderId select new { BuyerId = H.BuyerId }).FirstOrDefault().BuyerId;
        //    PendingOrderListForPacking fifosaleorderforproduct = _PackingLineService.FGetFifoSaleOrder(ProductId, BuyerId);
        //    List<PendingOrderListForPacking> FiFoSaleOrderForProductJson = new List<PendingOrderListForPacking>();

        //    if (fifosaleorderforproduct != null)
        //    {
        //        FiFoSaleOrderForProductJson.Add(new PendingOrderListForPacking()
        //        {
        //            SaleOrderLineId = fifosaleorderforproduct.SaleOrderLineId,
        //            SaleOrderNo = fifosaleorderforproduct.SaleOrderNo
        //        });
        //    }

        //    return Json(FiFoSaleOrderForProductJson);
        //}

        public JsonResult GetProductAreaJson(int ProductId,int PackingHeaderId)
        {
            var DeliveryUnit = (from H in db.PackingHeader
                                where H.PackingHeaderId == PackingHeaderId
                                select new { DeliveryUnitId = H.DealUnitId }).FirstOrDefault();

            ProductAreaDetail productarea = _PackingLineService.FGetProductArea(ProductId);

            Decimal AreaInDeliveryUnit = 0;

            if (productarea != null)
            { 
                if (DeliveryUnit.DeliveryUnitId == "FT2")
                {
                    AreaInDeliveryUnit = Math.Round(productarea.ProductArea , 4);
                }
                else if (DeliveryUnit.DeliveryUnitId.ToUpper() == "MT2")
                {
                    AreaInDeliveryUnit = Math.Round((productarea.ProductArea * (Decimal) 0.09), 4);
                }
                else if (DeliveryUnit.DeliveryUnitId.ToUpper() == "YD2")
                {
                    AreaInDeliveryUnit = Math.Round((productarea.ProductArea * (Decimal)0.111111111), 4);
                }
                else if (DeliveryUnit.DeliveryUnitId.ToUpper() == "PCS")
                {
                    AreaInDeliveryUnit = 1;
                }
                else
                {
                    AreaInDeliveryUnit = 1;
                }
            }

            Product Product = new ProductService(_unitOfWork).Find(ProductId);

            if (Product.UnitId == DeliveryUnit.DeliveryUnitId)
            {
                AreaInDeliveryUnit = 1;
            }


            List<ProductAreaDetail> ProductAreaJson = new List<ProductAreaDetail>();

            ProductAreaJson.Add(new ProductAreaDetail()
            {
                ProductId = ProductId,
                ProductArea = AreaInDeliveryUnit,
            });

            return Json(ProductAreaJson);
        }

        //public JsonResult GetNewPackingBaleNoJson(int PackingHeaderId)
        //{
        //    //PackingBaleNo packingbaleno = _PackingLineService.FGetNewPackingBaleNo(PackingHeaderId);
        //    List<PackingBaleNo> PackingBaleNoJson = new List<PackingBaleNo>();


        //    string BaleNoSessionVarName = "CurrentBaleNo" + PackingHeaderId.ToString();
        //    int baleNo = 0;

        //    if (System.Web.HttpContext.Current.Session[BaleNoSessionVarName] == null)
        //    {
        //        System.Web.HttpContext.Current.Session[BaleNoSessionVarName] = 1;
        //    }
        //    else
        //    {
        //        System.Web.HttpContext.Current.Session[BaleNoSessionVarName] = Convert.ToInt32(System.Web.HttpContext.Current.Session[BaleNoSessionVarName]) + 1;
        //    }

        //    baleNo = Convert.ToInt32(System.Web.HttpContext.Current.Session[BaleNoSessionVarName]);


        //    PackingBaleNoJson.Add(new PackingBaleNo()
        //    {
        //        PackingHeaderId = PackingHeaderId,
        //        NewBaleNo = baleNo

        //    });
        //    return Json(PackingBaleNoJson);
        //}





        public JsonResult GetNewPackingBaleNoJson_WithProductId(int PackingHeaderId, int? ProductInvoiceGroupId, int? SaleOrderLineId, int BaleNoPatternId,decimal DealQty, int? ProductId)
        {
            PackingBaleNo packingbaleno = _PackingLineService.FGetNewPackingBaleNo_WithProductId(PackingHeaderId, ProductInvoiceGroupId, SaleOrderLineId, BaleNoPatternId,DealQty, ProductId);
            List<PackingBaleNo> PackingBaleNoJson = new List<PackingBaleNo>();


            PackingBaleNoJson.Add(new PackingBaleNo()
            {
                PackingHeaderId = packingbaleno.PackingHeaderId,
                NewBaleNo = packingbaleno.NewBaleNo

            });
            return Json(PackingBaleNoJson);
        }


        public JsonResult GetNewPackingBaleNoJson(int PackingHeaderId, int? ProductInvoiceGroupId, int? SaleOrderLineId, int BaleNoPatternId, decimal DealQty)
        {
            PackingBaleNo packingbaleno = _PackingLineService.FGetNewPackingBaleNo(PackingHeaderId, ProductInvoiceGroupId, SaleOrderLineId, BaleNoPatternId, DealQty);
            List<PackingBaleNo> PackingBaleNoJson = new List<PackingBaleNo>();


            PackingBaleNoJson.Add(new PackingBaleNo()
            {
                PackingHeaderId = packingbaleno.PackingHeaderId,
                NewBaleNo = packingbaleno.NewBaleNo

            });
            return Json(PackingBaleNoJson);
        }


        public JsonResult GetSaleOrderLineIdListJson(int ProductId, int PackingHeaderId, int PackingLineId)
        {
            if (PackingLineId == 0)
            { 
                if (ProductId != 0 && PackingHeaderId != 0)
                { 
                    int BuyerId = (from H in db.PackingHeader where H.PackingHeaderId == PackingHeaderId select new { BuyerId = H.BuyerId }).FirstOrDefault().BuyerId;
                    List<PendingOrderListForPacking> saleorderlistforproductjson = _PackingLineService.FGetPendingOrderListForPacking(ProductId, BuyerId, PackingLineId).ToList();
                    return Json(saleorderlistforproductjson);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                List<PendingOrderListForPacking> saleorderlistforproductjson = (from L in db.PackingLine
                                                                               join Sl in db.SaleOrderLine on L.SaleOrderLineId equals Sl.SaleOrderLineId into SaleOrderLineTable
                                                                               from SaleOrderLineTab in SaleOrderLineTable.DefaultIfEmpty()
                                                                               join Sh in db.SaleOrderHeader on SaleOrderLineTab.SaleOrderHeaderId equals Sh.SaleOrderHeaderId into SaleOrderHeaderTable
                                                                               from SaleOrderHeaderTab in SaleOrderHeaderTable.DefaultIfEmpty()
                                                                                orderby L.SaleOrderLineId descending
                                                                                where L.PackingLineId == PackingLineId
                                                                               select new PendingOrderListForPacking
                                                                               {
                                                                                   SaleOrderLineId = (int) L.SaleOrderLineId,
                                                                                   SaleOrderNo = SaleOrderHeaderTab.DocNo
                                                                               }).ToList();
                return Json(saleorderlistforproductjson);
            }
        }

        public JsonResult GetSaleOrderLineIdListForProductUidJson(int ProductUidId, int PackingHeaderId,  int PackingLineId)
        {
            if (ProductUidId != 0 && PackingHeaderId != 0)
            {
                int BuyerId = (from H in db.PackingHeader where H.PackingHeaderId == PackingHeaderId select new { BuyerId = H.BuyerId }).FirstOrDefault().BuyerId;
                List<PendingOrderListForPacking> saleorderlistforproductjson = _PackingLineService.FGetPendingOrderListForPackingForProductUid(ProductUidId, BuyerId, PackingLineId).ToList();
                return Json(saleorderlistforproductjson);
            }
            else
            {
                return null;
            }
        }


        public JsonResult GetSaleOrderLineIdListForDeliveryOrderJson(int SaleDeliveryOrderLineId)
        {
            if (SaleDeliveryOrderLineId != 0 && SaleDeliveryOrderLineId != null)
            {
                List<PendingOrderListForPacking> saleorderlistforproductjson = (from L in db.SaleDeliveryOrderLine
                                                                                where L.SaleDeliveryOrderLineId == SaleDeliveryOrderLineId
                                                                                select new PendingOrderListForPacking
                                                                                {
                                                                                    SaleOrderLineId = L.SaleOrderLineId,
                                                                                    SaleOrderNo = L.SaleOrderLine.SaleOrderHeader.BuyerOrderNo
                                                                                }).ToList();
                return Json(saleorderlistforproductjson);
            }
            else
            {
                return null;
            }
        }

        //public JsonResult GetProductPrevProcess(int ProductId)
        //{
        //    Process p = new ProcessService(_unitOfWork).Find("Packing");
        //    List<ProductPrevProcess> productprevprocessjson = new ProductService(_unitOfWork).FGetProductPrevProcess(ProductId, p.ProcessId).ToList();
        //    return Json(productprevprocessjson);
        //}

        public JsonResult GetProductPrevProcess(int ProductId, int GodownId, int DocTypeId)
        {
            ProductPrevProcess ProductPrevProcess = new ProductService(_unitOfWork).FGetProductPrevProcess(ProductId, GodownId, DocTypeId);
            List<ProductPrevProcess> ProductPrevProcessJson = new List<ProductPrevProcess>();

            if (ProductPrevProcess != null)
            {
                ProductPrevProcessJson.Add(new ProductPrevProcess()
                {
                    ProcessId = ProductPrevProcess.ProcessId
                });
                return Json(ProductPrevProcessJson);
            }
            else
            {
                return null;
            }

        }

        public JsonResult GetSpecification(int SaleOrderLineId)
        {
            var SaleOrderLine = new SaleOrderLineService(_unitOfWork).Find(SaleOrderLineId);

            if (SaleOrderLine != null)
            {
                if (SaleOrderLine.Specification != null)
                {
                    return Json(SaleOrderLine.Specification);
                }
                else
                {
                    return Json("");
                }
            }
            else
            {
                return Json("");
            }
        }


        public JsonResult GetLastProduct(int PackingHeaderId)
        {
            string LastProductSessionVarName = "LastProduct" + PackingHeaderId.ToString().ToString();

            List<LastProduct> LastProductJson = new List<LastProduct>();

            if (System.Web.HttpContext.Current.Session[LastProductSessionVarName] != null)
            {
                LastProductJson.Add((LastProduct)System.Web.HttpContext.Current.Session[LastProductSessionVarName]);
            }
            else
            {
                LastProductJson.Add(null);
            }

            return Json(LastProductJson);
        }


        public JsonResult GetPendingToPrint(int PackingHeaderId)
        {
            int PendingToPrint = 0;

            string PendingToPrintSessionVarName = "PendingToPrint" + PackingHeaderId.ToString().ToString();
            if (System.Web.HttpContext.Current.Session[PendingToPrintSessionVarName] != null)
            {
                PendingToPrint = Convert.ToInt32(System.Web.HttpContext.Current.Session[PendingToPrintSessionVarName]);
            }
            else
            {
                PendingToPrint = 0;
            }

            return Json(PendingToPrint);
        }

        public StockViewModel GetStockViewModelIssueForPackingLine(int PackingLineId)
        {
            StockViewModel StockViewModel = (from H in db.PackingHeader
                                             join L in db.PackingLine on H.PackingHeaderId equals L.PackingHeaderId into PackingLineTable
                                             from PackingLineTab in PackingLineTable.DefaultIfEmpty()
                                             where PackingLineTab.PackingLineId == PackingLineId
                                             select new StockViewModel
                                             {
                                                 DocHeaderId = H.PackingHeaderId,
                                                 DocLineId = PackingLineTab.PackingLineId,
                                                 DocTypeId = H.DocTypeId,
                                                 StockHeaderDocDate = H.DocDate,
                                                 DocNo = H.DocNo,
                                                 DivisionId = H.DivisionId,
                                                 SiteId = H.SiteId,
                                                 CurrencyId = null,
                                                 HeaderProcessId = null,
                                                 PersonId = null,
                                                 ProductId = PackingLineTab.ProductId,
                                                 HeaderFromGodownId = null,
                                                 HeaderGodownId = null,
                                                 GodownId = H.GodownId,
                                                 ProcessId = PackingLineTab.FromProcessId,
                                                 LotNo = null,
                                                 CostCenterId = null,
                                                 Qty_Iss = PackingLineTab.Qty,
                                                 Qty_Rec = 0,
                                                 Rate = null,
                                                 ExpiryDate = null,
                                                 Specification = null,
                                                 Dimension1Id = null,
                                                 Dimension2Id = null,
                                                 Remark = H.Remark,
                                                 Status = H.Status,
                                                 CreatedBy = H.CreatedBy,
                                                 CreatedDate = H.CreatedDate,
                                                 ModifiedBy = H.ModifiedBy,
                                                 ModifiedDate = H.ModifiedDate
                                             }).FirstOrDefault();
            return StockViewModel;
        }


        public StockViewModel GetStockViewModelReceiveForPackingLine(int PackingLineId)
        {
            int ProcessId = new ProcessService(_unitOfWork).Find(ProcessConstants.Packing).ProcessId;

            StockViewModel StockViewModel = (from H in db.PackingHeader
                                             join L in db.PackingLine on H.PackingHeaderId equals L.PackingHeaderId into PackingLineTable
                                             from PackingLineTab in PackingLineTable.DefaultIfEmpty()
                                             where PackingLineTab.PackingLineId == PackingLineId
                                             select new StockViewModel
                                             {
                                                 DocHeaderId = H.PackingHeaderId,
                                                 DocLineId = PackingLineTab.PackingLineId,
                                                 DocTypeId = H.DocTypeId,
                                                 StockHeaderDocDate = H.DocDate,
                                                 DocNo = H.DocNo,
                                                 DivisionId = H.DivisionId,
                                                 SiteId = H.SiteId,
                                                 CurrencyId = null,
                                                 HeaderProcessId = null,
                                                 PersonId = null,
                                                 ProductId = PackingLineTab.ProductId,
                                                 HeaderFromGodownId = null,
                                                 HeaderGodownId = null,
                                                 GodownId = H.GodownId,
                                                 ProcessId = ProcessId,
                                                 LotNo = H.DocNo,
                                                 CostCenterId = null,
                                                 Qty_Iss = 0,
                                                 Qty_Rec = PackingLineTab.Qty,
                                                 Rate = null,
                                                 ExpiryDate = null,
                                                 Specification = null,
                                                 Dimension1Id = null,
                                                 Dimension2Id = null,
                                                 Remark = H.Remark,
                                                 Status = H.Status,
                                                 CreatedBy = H.CreatedBy,
                                                 CreatedDate = H.CreatedDate,
                                                 ModifiedBy = H.ModifiedBy,
                                                 ModifiedDate = H.ModifiedDate
                                             }).FirstOrDefault();
            return StockViewModel;
        }

        public JsonResult IsThirdBackingProcessDone(int ProductUidId)
        {
            bool IsThirdBackingDone = true;
            ProductUid ProductUid = new ProductUidService(_unitOfWork).Find(ProductUidId);

            if ((ProductUid.GenDocTypeId ?? 0) == new DocumentTypeService(_unitOfWork).Find(TransactionDoctypeConstants.WeavingOrder).DocumentTypeId)
            {
                if (new ProductUidService(_unitOfWork).IsProcessDone((int)ProductUidId, new ProcessService(_unitOfWork).Find(ProcessConstants.FullFinishing).ProcessId) == false)
                {
                    if (new ProductUidService(_unitOfWork).IsProcessDone((int)ProductUidId, new ProcessService(_unitOfWork).Find(ProcessConstants.ThirdBacking).ProcessId) == false)
                    {
                        IsThirdBackingDone = false;
                    }
                    else
                    {
                        IsThirdBackingDone = true;
                    }
                }
            }
            else
            {
                IsThirdBackingDone = true;
            }
            return Json(IsThirdBackingDone);
        }



        public void _CreateExtraSaleOrder(ref PackingLineViewModel svm)
        {
            PackingHeader Header = new PackingHeaderService(_unitOfWork).Find(svm.PackingHeaderId);

            Decimal Rate = 0;
            SqlParameter SqlParameterProductId = new SqlParameter("@ProductId", svm.ProductId);
            ProductSaleOrderRate ProductSaleOrderRate = db.Database.SqlQuery<ProductSaleOrderRate>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".sp_GetProductLastSaleOrderRate @ProductId", SqlParameterProductId).FirstOrDefault();
            if (ProductSaleOrderRate != null) Rate = ProductSaleOrderRate.Rate;


            var settings = new PackingSettingService(_unitOfWork).GetPackingSettingForDocument(Header.DocTypeId, Header.DivisionId, Header.SiteId);

            var Buyer = new BuyerService(_unitOfWork).Find(Header.BuyerId);

            if (settings != null)
            {
                if (Buyer.ExtraSaleOrderHeaderId != null)
                {
                    SaleOrderHeader SaleOrderHeader = new SaleOrderHeaderService(_unitOfWork).Find((int)Buyer.ExtraSaleOrderHeaderId);
                    if (SaleOrderHeader != null)
                    {

                        SaleEnquiryHeader SaleEnquiryHeader = new SaleEnquiryHeaderService(_unitOfWork).Find((int)SaleOrderHeader.ReferenceDocId);

                        SaleEnquiryLine SEL = new SaleEnquiryLine();
                        SEL.SaleEnquiryHeaderId = SaleEnquiryHeader.SaleEnquiryHeaderId;
                        SEL.ProductId = svm.ProductId;
                        SEL.Qty = svm.Qty;
                        SEL.DealUnitId = svm.DealUnitId;
                        SEL.DealQty = svm.DealQty;
                        SEL.Rate = 0;
                        SEL.Amount = 0;
                        SEL.ReferenceDocTypeId = Header.DocTypeId;
                        SEL.ReferenceDocLineId = svm.PackingLineId;
                        SEL.CreatedDate = DateTime.Now;
                        SEL.ModifiedDate = DateTime.Now;
                        SEL.CreatedBy = User.Identity.Name;
                        SEL.ModifiedBy = User.Identity.Name;
                        SEL.DueDate = DateTime.Now;
                        SEL.ObjectState = Model.ObjectState.Added;
                        new SaleEnquiryLineService(_unitOfWork).Create(SEL);


                        SaleEnquiryLineExtended Extended = new SaleEnquiryLineExtended();
                        Extended.SaleEnquiryLineId = SEL.SaleEnquiryLineId;
                        Extended.BuyerSpecification = svm.BuyerSpecification;
                        Extended.BuyerSpecification1 = svm.BuyerSpecification1;
                        Extended.BuyerSpecification2 = svm.BuyerSpecification2;
                        Extended.BuyerSpecification3 = svm.BuyerSpecification3;
                        Extended.BuyerSku = svm.BuyerSku;
                        new SaleEnquiryLineExtendedService(_unitOfWork).Create(Extended);

                        SaleOrderLine Line = new SaleOrderLine();
                        Line.SaleOrderHeaderId = SaleOrderHeader.SaleOrderHeaderId;
                        Line.ReferenceDocLineId = SEL.SaleEnquiryLineId;
                        Line.ReferenceDocTypeId = Header.DocTypeId;
                        Line.ProductId = svm.ProductId;
                        Line.Qty = svm.Qty;
                        Line.DealUnitId = svm.DealUnitId;
                        Line.DealQty = svm.DealQty;
                        Line.Rate = Rate;
                        Line.Amount = Line.Qty * Line.Rate;
                        Line.CreatedDate = DateTime.Now;
                        Line.ModifiedDate = DateTime.Now;
                        Line.CreatedBy = User.Identity.Name;
                        Line.ModifiedBy = User.Identity.Name;
                        Line.DueDate = DateTime.Now;
                        Line.ObjectState = Model.ObjectState.Added;
                        new SaleOrderLineService(_unitOfWork).Create(Line);

                        new SaleOrderLineStatusService(_unitOfWork).CreateLineStatus(Line.SaleOrderLineId);

                        svm.SaleOrderLineId = Line.SaleOrderLineId;
                        svm.SaleOrderNo = settings.ExtraSaleOrderNo;
                    }
                }
            }
        }


        public JsonResult GetDeliveryOrderLineIdListJson(int ProductId, int PackingHeaderId, int PackingLineId)
        {
            if (PackingLineId == 0)
            {
                if (ProductId != 0 && PackingHeaderId != 0)
                {
                    int BuyerId = (from H in db.PackingHeader where H.PackingHeaderId == PackingHeaderId select new { BuyerId = H.BuyerId }).FirstOrDefault().BuyerId;
                    List<PendingDeliveryOrderListForPacking> saledeliveryorderlistforproductjson = _PackingLineService.FGetPendingDeliveryOrderListForPacking(ProductId, BuyerId, PackingLineId).ToList();
                    return Json(saledeliveryorderlistforproductjson);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        //public JsonResult GetProductCustomDetailJson(int ProductId, int PackingHeaderId)
        //{
        //    PackingHeader Header = new PackingHeaderService(_unitOfWork).Find(PackingHeaderId);

        //    ProductCustomDetailViewModel ProductCustomDetail = (from P in db.ViewProductBuyer
        //                                                        where P.ProductId == ProductId && P.BuyerId == Header.BuyerId
        //                                                        select new ProductCustomDetailViewModel
        //                                                        {
        //                                                            BuyerSku = P.BuyerSku,
        //                                                            BuyerSpecification = P.BuyerSpecification,
        //                                                            BuyerSpecification1 = P.BuyerSpecification1,
        //                                                            BuyerSpecification2 = P.BuyerSpecification2,
        //                                                            BuyerSpecification3 = P.BuyerSpecification3
        //                                                        }).FirstOrDefault();



        //    List<ProductCustomDetailViewModel> ProductCustomDetailJson = new List<ProductCustomDetailViewModel>();

        //    if (ProductCustomDetail != null)
        //    {
        //        ProductCustomDetailJson.Add(new ProductCustomDetailViewModel()
        //        {
        //            BuyerSku = ProductCustomDetail.BuyerSku,
        //            BuyerSpecification = ProductCustomDetail.BuyerSpecification,
        //            BuyerSpecification1 = ProductCustomDetail.BuyerSpecification1,
        //            BuyerSpecification2 = ProductCustomDetail.BuyerSpecification2,
        //            BuyerSpecification3 = ProductCustomDetail.BuyerSpecification3
        //        });
        //    }

        //    return Json(ProductCustomDetailJson);
        //}

        public JsonResult GetProductCustomDetailJson(int SaleOrderLineId)
        {
            //PackingHeader Header = new PackingHeaderService(_unitOfWork).Find(PackingHeaderId);

            ProductCustomDetailViewModel ProductCustomDetail = (from P in db.SaleOrderLine
                                                                join L in db.SaleEnquiryLine on P.ReferenceDocLineId equals L.SaleEnquiryLineId into SETable
                                                                from SETab in SETable.DefaultIfEmpty()
                                                                join SELE in db.SaleEnquiryLineExtended on SETab.SaleEnquiryLineId equals SELE.SaleEnquiryLineId into SELETable
                                                                from SELETab in SELETable.DefaultIfEmpty()
                                                                where P.SaleOrderLineId == SaleOrderLineId
                                                                select new ProductCustomDetailViewModel
                                                                {
                                                                    BuyerSku = SELETab.BuyerSku,
                                                                    BuyerSpecification = SELETab.BuyerSpecification,
                                                                    BuyerSpecification1 = SELETab.BuyerSpecification1,
                                                                    BuyerSpecification2 = SELETab.BuyerSpecification2,
                                                                    BuyerSpecification3 = SELETab.BuyerSpecification3
                                                                }).FirstOrDefault();



            List<ProductCustomDetailViewModel> ProductCustomDetailJson = new List<ProductCustomDetailViewModel>();

            if (ProductCustomDetail != null)
            {
                ProductCustomDetailJson.Add(new ProductCustomDetailViewModel()
                {
                    BuyerSku = ProductCustomDetail.BuyerSku,
                    BuyerSpecification = ProductCustomDetail.BuyerSpecification,
                    BuyerSpecification1 = ProductCustomDetail.BuyerSpecification1,
                    BuyerSpecification2 = ProductCustomDetail.BuyerSpecification2,
                    BuyerSpecification3 = ProductCustomDetail.BuyerSpecification3
                });
            }

            return Json(ProductCustomDetailJson);
        }

        public JsonResult GetProductCustomDetailForExtraPackingJson(int ProductId)
        {           

            ProductCustomDetailViewModel ProductCustomDetail = (from P in db.Product 
                                                                join PG in db.ProductGroups on P.ProductGroupId equals PG.ProductGroupId into PGTable
                                                                from PGTab in PGTable.DefaultIfEmpty()
                                                                join FP in db.FinishedProduct on P.ProductId equals FP.ProductId into FPTable
                                                                from FPTab in FPTable.DefaultIfEmpty()
                                                                join VRS in db.ViewRugSize on P.ProductId equals VRS.ProductId into VRSTable
                                                                from VRSTab in VRSTable.DefaultIfEmpty()
                                                                where P.ProductId == ProductId
                                                                select new ProductCustomDetailViewModel
                                                                {
                                                                    BuyerSku = P.ProductName,
                                                                    BuyerSpecification = PGTab.ProductGroupName,
                                                                    BuyerSpecification1 = VRSTab.StandardSizeName,
                                                                    BuyerSpecification2 = FPTab.Colour.ColourName,
                                                                    BuyerSpecification3 = FPTab.ProductQuality.ProductQualityName
                                                                }).FirstOrDefault();



            List<ProductCustomDetailViewModel> ProductCustomDetailJson = new List<ProductCustomDetailViewModel>();

            if (ProductCustomDetail != null)
            {
                ProductCustomDetailJson.Add(new ProductCustomDetailViewModel()
                {
                    BuyerSku = ProductCustomDetail.BuyerSku,
                    BuyerSpecification = ProductCustomDetail.BuyerSpecification,
                    BuyerSpecification1 = ProductCustomDetail.BuyerSpecification1,
                    BuyerSpecification2 = ProductCustomDetail.BuyerSpecification2,
                    BuyerSpecification3 = ProductCustomDetail.BuyerSpecification3
                });
            }

            return Json(ProductCustomDetailJson);
        }

        public ActionResult SetFlagForExtraSaleOrder()
        {
            bool CreateExtraSaleOrder = true;

            return Json(CreateExtraSaleOrder);
        }

        //public ActionResult GetCustomProducts(string searchTerm, int pageSize, int pageNum, int filter )//DocTypeId
        //{
        //   Boolean isShowAllProducts= true;

        //     var Query = _PackingLineService.GetCustomProductsWithBuyerSku_ForAllProducts(filter, searchTerm, isShowAllProducts);

        //    var temp = Query.Skip(pageSize * (pageNum - 1))
        //        .Take(pageSize)
        //        .ToList();

        //    var count = Query.Count();

        //    ComboBoxPagedResult Data = new ComboBoxPagedResult();
        //    Data.Results = temp;
        //    Data.Total = count;

        //    return new JsonpResult
        //    {
        //        Data = Data,
        //        JsonRequestBehavior = JsonRequestBehavior.AllowGet
        //    };
        //}


        public ActionResult GetCustomProducts(string searchTerm, int pageSize, int pageNum, int filter, Boolean isShowAllProducts)//DocTypeId
        {
            //Boolean isShowAllProducts = true;

            var Query = _PackingLineService.GetCustomProductsWithBuyerSku_ForAllProducts(filter, searchTerm, isShowAllProducts);

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


        public JsonResult GetUnitConversionMultiplier(int DocumentTypeId, int ProductId, Decimal Length, Decimal Width, Decimal? Height, string ToUnitId)
        {
            ProductViewModel product = new ProductService(_unitOfWork).GetProduct(ProductId);

            Decimal UnitConversionMultiplier = 0;
            UnitConversionMultiplier = new ProductService(_unitOfWork).GetUnitConversionMultiplier(1, product.UnitId, Length, Width, Height, ToUnitId,db, DocumentTypeId);

            return Json(UnitConversionMultiplier);
        }

        public JsonResult GetProductDimensionsJson(int ProductId, string DealUnitId, int DocTypeId)
        {
            ProductDimensions ProductDimensions = new ProductService(_unitOfWork).GetProductDimensions(ProductId, DealUnitId, DocTypeId);
            return Json(ProductDimensions);
        }

        public JsonResult GetProductPackingUnitJson(int ProductId)
        {
            ProductPackingUnits ProductPackingUnits = new PackingLineService(_unitOfWork).GetProductPackingUnits(ProductId);
            return Json(ProductPackingUnits);
        }

        public ActionResult GetStockInForProduct(string searchTerm, int pageSize, int pageNum, int filter, int? ProductId, int? Dimension1Id, int? Dimension2Id, int? Dimension3Id, int? Dimension4Id)//DocTypeId
        {
            IEnumerable<ComboBoxResult> Query;

            PackingHeader H = new PackingHeaderService(_unitOfWork).GetPackingHeader(filter);
            PackingSetting PackingSetting = new PackingSettingService(_unitOfWork).GetPackingSettingForDocument(H.DocTypeId, H.DivisionId, H.SiteId);

            if (PackingSetting.isAllowtoDirectPacking ==true )
                Query = _PackingLineService.GetPendingStockForPacking(filter, ProductId, Dimension1Id, Dimension2Id, Dimension3Id, Dimension4Id, searchTerm);
            else
                Query = _PackingLineService.GetPendingStockInForIssue(filter, ProductId, Dimension1Id, Dimension2Id, Dimension3Id, Dimension4Id, searchTerm);

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


        public JsonResult GetStockInDetailJson(int StockInId, int PackingHeaderId)
        {
            PackingHeader H = new PackingHeaderService(_unitOfWork).GetPackingHeader(PackingHeaderId);
            PackingSetting PackingSetting = new PackingSettingService(_unitOfWork).GetPackingSettingForDocument(H.DocTypeId, H.DivisionId, H.SiteId);

            string mQry = "";
            if (PackingSetting.isAllowtoDirectPacking == true)
                 mQry = @"SELECT S.ProductUidId, PU.ProductUidName, S.ProductId, P.ProductName, 
                            H.BalanceQty, S.LotNo, S.ProcessId FromProcessId, PR.ProcessName AS FromProcessName, 
                            isnull(PU.CurrenctGodownId,S.GodownId) AS CurrenctGodownId, PU.Status 
                            FROM web.ViewStockInBalanceForPacking H WITH (Nolock)
                            LEFT JOIN web.Stocks S WITH (Nolock) ON S.StockId = H.StockInId
                            LEFT JOIN web.Products P WITH (Nolock) ON P.ProductId = S.ProductId 
                            LEFT JOIN web.ProductUids PU WITH (Nolock) ON PU.ProductUIDId = S.ProductUidId
                            LEFT JOIN web.Processes PR WITH (Nolock) ON PR.ProcessId = S.ProcessId  
                            Where H.StockInId = " + StockInId + "";
            else
                mQry = @"SELECT S.ProductUidId, PU.ProductUidName, S.ProductId, P.ProductName, S.Dimension1Id, D1.Dimension1Name, S.Dimension2Id, D2.Dimension2Name,
                            H.BalanceQty, S.LotNo, S.ProcessId FromProcessId, PR.ProcessName AS FromProcessName, 
                            isnull(PU.CurrenctGodownId,S.GodownId) AS CurrenctGodownId, PU.Status 
                            FROM web.ViewStockInBalance H WITH (Nolock)
                            LEFT JOIN web.Stocks S WITH (Nolock) ON S.StockId = H.StockInId
                            LEFT JOIN web.Products P WITH (Nolock) ON P.ProductId = S.ProductId 
                            LEFT JOIN web.Dimension1 D1 WITH (Nolock) ON D1.Dimension1Id = S.Dimension1Id 
                            LEFT JOIN web.Dimension2 D2 WITH (Nolock) ON D2.Dimension2Id = S.Dimension2Id 
                            LEFT JOIN web.ProductUids PU WITH (Nolock) ON PU.ProductUIDId = S.ProductUidId
                            LEFT JOIN web.Processes PR WITH (Nolock) ON PR.ProcessId = S.ProcessId  
                            Where H.StockInId = " + StockInId + "";

            IEnumerable<ViewStockInBalance> temp1 = db.Database.SqlQuery<ViewStockInBalance>(mQry).ToList();

            var temp = (from p in temp1
                        select new
                        {
                            ProductUidId = p.ProductUidId,
                            ProductUidName = p.ProductUidName,
                            ProductId = p.ProductId,
                            ProductName = p.ProductName,
                            BalanceQty = p.BalanceQty,
                            LotNo = p.LotNo,
                            FromProcessId = p.FromProcessId,
                            FromProcessName = p.FromProcessName,
                            CurrenctGodownId = p.CurrenctGodownId,
                            Status = p.Status
                        }).FirstOrDefault();

            //var temp = (from p in db.ViewStockInBalanceForPacking
            //            join S in db.Stock on p.StockInId equals S.StockId into StockTable
            //            from StockTab in StockTable.DefaultIfEmpty()
            //            join pt in db.Product on p.ProductId equals pt.ProductId into ProductTable
            //            from ProductTab in ProductTable.DefaultIfEmpty()
            //            join D1 in db.Dimension1 on p.Dimension1Id equals D1.Dimension1Id into Dimension1Table
            //            from Dimension1Tab in Dimension1Table.DefaultIfEmpty()
            //            join D2 in db.Dimension2 on p.Dimension2Id equals D2.Dimension2Id into Dimension2Table
            //            from Dimension2Tab in Dimension2Table.DefaultIfEmpty()
            //            join D3 in db.Dimension3 on p.Dimension3Id equals D3.Dimension3Id into Dimension3Table
            //            from Dimension3Tab in Dimension3Table.DefaultIfEmpty()
            //            join D4 in db.Dimension4 on p.Dimension4Id equals D4.Dimension4Id into Dimension4Table
            //            from Dimension4Tab in Dimension4Table.DefaultIfEmpty()
            //            join PU in db.ProductUid on StockTab.ProductUidId equals PU.ProductUIDId into PUTable
            //            from PUTab in PUTable.DefaultIfEmpty()
            //            where p.StockInId == StockInId
            //            select new
            //            {
            //                ProductUidId = StockTab.ProductUidId,
            //                ProductUidName = StockTab.ProductUid.ProductUidName,
            //                ProductId = p.ProductId,
            //                ProductName = ProductTab.ProductName,
            //                Dimension1Id = p.Dimension1Id,
            //                Dimension1Name = Dimension1Tab.Dimension1Name,
            //                Dimension2Id = p.Dimension2Id,
            //                Dimension2Name = Dimension2Tab.Dimension2Name,
            //                Dimension3Id = p.Dimension3Id,
            //                Dimension3Name = Dimension3Tab.Dimension3Name,
            //                Dimension4Id = p.Dimension4Id,
            //                Dimension4Name = Dimension4Tab.Dimension4Name,
            //                BalanceQty = p.BalanceQty,
            //                LotNo = p.LotNo,
            //                FromProcessId = StockTab.ProcessId,
            //                FromProcessName = StockTab.Process.ProcessName,
            //                CurrenctGodownId = StockTab.ProductUidId == null ? StockTab.GodownId : PUTab.CurrenctGodownId,
            //                Status = PUTab.Status

            //            }).FirstOrDefault();

            if (temp != null)
            {
                return Json(temp);
            }
            else
            {
                return null;
            }
        }


    }

    public class ViewStockInBalance    {

        public int ? ProductUidId { get; set; }
        public string ProductUidName { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        //public int Dimension1Id { get; set; }
        //public string Dimension1Name { get; set; }
        //public int Dimension2Id { get; set; }
        //public string Dimension2Name { get; set; }
        public Decimal BalanceQty { get; set; }
        public string LotNo { get; set; }
        public int ? FromProcessId { get; set; }
        public string FromProcessName { get; set; }
        public int CurrenctGodownId { get; set; }
        public string Status { get; set; }
    }


}