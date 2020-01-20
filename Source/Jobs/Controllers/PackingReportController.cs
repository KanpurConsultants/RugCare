using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Model.Models;
using Data.Models;
using Service;
using Data.Infrastructure;
using System.Data;
using Model.ViewModels;
using Jobs.Helpers;

namespace Jobs.Controllers
{
    [Authorize]
    public class PackingReportController : System.Web.Mvc.Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        protected string connectionString = (string)System.Web.HttpContext.Current.Session["DefaultConnectionString"];
        IPackingReportService _PackingReportService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;


        private const string ReportFormat_QualityWiseSummary = "Quality Wise Summary";
        private const string ReportFormat_QualityDesignWiseSummary = "Quality Design Wise Summary";
        private const string ReportFormat_QualityDesignSizeWiseSummary = "Quality Design Size Wise Summary";
        private const string ReportFormat_QualitySizeWiseSummary = "Quality Size Wise Summary";
        private const string ReportFormat_QualityDesignColourWiseSummary = "Quality Design Colour Wise Summary";
        private const string ReportFormat_Detail = "Detail";


        private const string ReportFor_SqFeet = " Sq.Feet";
        private const string ReportFor_SqMeter = "Sq.Meter";
        private const string ReportFor_SqYard = "Sq.Yard";

        public PackingReportController(IPackingReportService PackingReportService, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _PackingReportService = PackingReportService;
            _exception = exec;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public ActionResult PackingReport(int MenuId)
        {
            Menu Menu = new MenuService(_unitOfWork).Find(MenuId);

            PackingReportViewModel vm = new PackingReportViewModel();
            

            System.Web.HttpContext.Current.Session["SettingList"] = new List<PackingReportFilterSettings>();
            System.Web.HttpContext.Current.Session["CurrentSetting"] = new PackingReportFilterSettings();
            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

            List<SelectListItem> ReportFormat = new List<SelectListItem>();
            ReportFormat.Add(new SelectListItem { Text = ReportFormat_QualityWiseSummary, Value = ReportFormat_QualityWiseSummary });
            ReportFormat.Add(new SelectListItem { Text = ReportFormat_QualityDesignWiseSummary, Value = ReportFormat_QualityDesignWiseSummary });
            ReportFormat.Add(new SelectListItem { Text = ReportFormat_QualityDesignSizeWiseSummary, Value = ReportFormat_QualityDesignSizeWiseSummary });
            ReportFormat.Add(new SelectListItem { Text = ReportFormat_QualitySizeWiseSummary, Value = ReportFormat_QualitySizeWiseSummary });
            ReportFormat.Add(new SelectListItem { Text = ReportFormat_QualityDesignColourWiseSummary, Value = ReportFormat_QualityDesignColourWiseSummary });
            ReportFormat.Add(new SelectListItem { Text = ReportFormat_Detail, Value = ReportFormat_Detail });
            ViewBag.ReportFormat = new SelectList(ReportFormat, "Value", "Text");

            List<SelectListItem> AreaUnit = new List<SelectListItem>();
            AreaUnit.Add(new SelectListItem { Text = ReportFor_SqFeet, Value = ReportFor_SqFeet });
            AreaUnit.Add(new SelectListItem { Text = ReportFor_SqMeter, Value = ReportFor_SqMeter });
            AreaUnit.Add(new SelectListItem { Text = ReportFor_SqYard, Value = ReportFor_SqYard });
            ViewBag.AreaUnit = new SelectList(AreaUnit, "Value", "Text");


            //vm.FromDate = "01/Apr/" + DateTime.Now.Date.Year.ToString();
            vm.FromDate = "01/Apr/2018";
            vm.ToDate = DateTime.Now.Date.ToString("dd/MMM/yyyy");


            vm.ReportFormat = ReportFormat_QualityWiseSummary;
			vm.AreaUnit = ReportFor_SqFeet;
            vm.Site = SiteId.ToString();
            vm.Division = DivisionId.ToString();
            return View("PackingReport", vm);
        }

        public JsonResult PackingReportFill(PackingReportViewModel vm)
        {
            PackingReportFilterSettings SettingParameter = SetCurrentParameterSettings(vm);


                IEnumerable<PackingReportDataViewModel> SaleOrderInventoryStatus = _PackingReportService.PackingReportDetail(SettingParameter);
                if (SaleOrderInventoryStatus != null)
                {
                    JsonResult json = Json(new { Success = true, Data = SaleOrderInventoryStatus.ToList() }, JsonRequestBehavior.AllowGet);
                    json.MaxJsonLength = int.MaxValue;
                    return json;
                }



            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }


        public PackingReportFilterSettings SetCurrentParameterSettings(PackingReportViewModel vm)
        {
            PackingReportFilterSettings PackingReportFilterSettings = new PackingReportFilterSettings();
            
            PackingReportFilterParameters ReportFormatParameter = new PackingReportFilterParameters();
            ReportFormatParameter.ParameterName = "ReportFormat";
            ReportFormatParameter.Value = vm.ReportFormat;
            ReportFormatParameter.IsApplicable = true;

            PackingReportFilterParameters AreaUnitParameter = new PackingReportFilterParameters();
            AreaUnitParameter.ParameterName = "AreaUnit";
            AreaUnitParameter.Value = vm.AreaUnit;
            AreaUnitParameter.IsApplicable = true;

            PackingReportFilterParameters SiteParameter = new PackingReportFilterParameters();
            SiteParameter.ParameterName = "Site";
            SiteParameter.Value = vm.Site;
            SiteParameter.IsApplicable = true;

            PackingReportFilterParameters DivisionParameter = new PackingReportFilterParameters();
            DivisionParameter.ParameterName = "Division";
            DivisionParameter.Value = vm.Division;
            DivisionParameter.IsApplicable = true;

            PackingReportFilterParameters FromDateParameter = new PackingReportFilterParameters();
            FromDateParameter.ParameterName = "FromDate";
            FromDateParameter.Value = vm.FromDate;
            FromDateParameter.IsApplicable = true;

            PackingReportFilterParameters ToDateParameter = new PackingReportFilterParameters();
            ToDateParameter.ParameterName = "ToDate";
            ToDateParameter.Value = vm.ToDate;
            ToDateParameter.IsApplicable = true;

            PackingReportFilterParameters BuyerParameter = new PackingReportFilterParameters();
            BuyerParameter.ParameterName = "Buyer";
            BuyerParameter.Value = vm.Buyer;
            BuyerParameter.IsApplicable = true;

            PackingReportFilterParameters PackingHeaderIdParameter = new PackingReportFilterParameters();
            PackingHeaderIdParameter.ParameterName = "PackingHeaderId";
            PackingHeaderIdParameter.Value = vm.PackingHeaderId;
            PackingHeaderIdParameter.IsApplicable = true;

            PackingReportFilterParameters SaleOrderHeaderIdParameter = new PackingReportFilterParameters();
            SaleOrderHeaderIdParameter.ParameterName = "SaleOrderHeaderId";
            SaleOrderHeaderIdParameter.Value = vm.SaleOrderHeaderId;
            SaleOrderHeaderIdParameter.IsApplicable = true;

            
            PackingReportFilterParameters ProductCategoryParameter = new PackingReportFilterParameters();
            ProductCategoryParameter.ParameterName = "ProductCategory";
            ProductCategoryParameter.Value = vm.ProductCategory;
            ProductCategoryParameter.IsApplicable = true;

            PackingReportFilterParameters ProductQualityParameter = new PackingReportFilterParameters();
            ProductQualityParameter.ParameterName = "ProductQuality";
            ProductQualityParameter.Value = vm.ProductQuality;
            ProductQualityParameter.IsApplicable = true;

            PackingReportFilterParameters ProductGroupParameter = new PackingReportFilterParameters();
            ProductGroupParameter.ParameterName = "ProductGroup";
            ProductGroupParameter.Value = vm.ProductGroup;
            ProductGroupParameter.IsApplicable = true;

            PackingReportFilterParameters ProductSizeParameter = new PackingReportFilterParameters();
            ProductSizeParameter.ParameterName = "ProductSize";
            ProductSizeParameter.Value = vm.ProductSize;
            ProductSizeParameter.IsApplicable = true;

            PackingReportFilterParameters ProductParameter = new PackingReportFilterParameters();
            ProductParameter.ParameterName = "Product";
            ProductParameter.Value = vm.Product;
            ProductParameter.IsApplicable = true;

            PackingReportFilterParameters BuyerQualityParameter = new PackingReportFilterParameters();
            BuyerQualityParameter.ParameterName = "BuyerQuality";
            BuyerQualityParameter.Value = vm.BuyerQuality;
            BuyerQualityParameter.IsApplicable = true;

            PackingReportFilterParameters BuyerDesignParameter = new PackingReportFilterParameters();
            BuyerDesignParameter.ParameterName = "BuyerDesign";
            BuyerDesignParameter.Value = vm.BuyerDesign;
            BuyerDesignParameter.IsApplicable = true;

            PackingReportFilterParameters BuyerSizeParameter = new PackingReportFilterParameters();
            BuyerSizeParameter.ParameterName = "BuyerSize";
            BuyerSizeParameter.Value = vm.BuyerSize;
            BuyerSizeParameter.IsApplicable = true;

            PackingReportFilterParameters BuyerColourParameter = new PackingReportFilterParameters();
            BuyerColourParameter.ParameterName = "BuyerColour";
            BuyerColourParameter.Value = vm.BuyerColour;
            BuyerColourParameter.IsApplicable = true;

            PackingReportFilterSettings.PackingReportFilterParameters = new List<PackingReportFilterParameters>();
            PackingReportFilterSettings.PackingReportFilterParameters.Add(ReportFormatParameter);
            PackingReportFilterSettings.PackingReportFilterParameters.Add(AreaUnitParameter);
            PackingReportFilterSettings.PackingReportFilterParameters.Add(SiteParameter);
            PackingReportFilterSettings.PackingReportFilterParameters.Add(DivisionParameter);
            PackingReportFilterSettings.PackingReportFilterParameters.Add(FromDateParameter);
            PackingReportFilterSettings.PackingReportFilterParameters.Add(ToDateParameter);
            PackingReportFilterSettings.PackingReportFilterParameters.Add(BuyerParameter);
            PackingReportFilterSettings.PackingReportFilterParameters.Add(PackingHeaderIdParameter);
            PackingReportFilterSettings.PackingReportFilterParameters.Add(SaleOrderHeaderIdParameter);
            PackingReportFilterSettings.PackingReportFilterParameters.Add(ProductCategoryParameter);
            PackingReportFilterSettings.PackingReportFilterParameters.Add(ProductQualityParameter);
            PackingReportFilterSettings.PackingReportFilterParameters.Add(ProductGroupParameter);
            PackingReportFilterSettings.PackingReportFilterParameters.Add(ProductSizeParameter);
            PackingReportFilterSettings.PackingReportFilterParameters.Add(ProductParameter);
            PackingReportFilterSettings.PackingReportFilterParameters.Add(BuyerQualityParameter);
            PackingReportFilterSettings.PackingReportFilterParameters.Add(BuyerDesignParameter);
            PackingReportFilterSettings.PackingReportFilterParameters.Add(BuyerSizeParameter);
            PackingReportFilterSettings.PackingReportFilterParameters.Add(BuyerColourParameter);


            System.Web.HttpContext.Current.Session["CurrentSetting"] = PackingReportFilterSettings;
            return PackingReportFilterSettings;
        }

        //public void SaveCurrentSetting()
        //{
        //    ((List<PackingReportFilterSettings>)System.Web.HttpContext.Current.Session["SettingList"]).Add((PackingReportFilterSettings)System.Web.HttpContext.Current.Session["CurrentSetting"]);
        //}


        public ActionResult GetSaleOrders(string searchTerm, int pageSize, int pageNum)
        {
            var Query = _PackingReportService.GetCustomSaleOrders(searchTerm);
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

        public JsonResult SetSaleOrders(string Ids)
        {
            string[] subStr = Ids.Split(',');
            List<ComboBoxResult> ProductJson = new List<ComboBoxResult>();
            for (int i = 0; i < subStr.Length; i++)
            {
                int temp = Convert.ToInt32(subStr[i]);
                //IEnumerable<Product> products = db.Products.Take(3);
                IEnumerable<SaleOrderHeader> prod = from p in db.SaleOrderHeader
                                                    where p.SaleOrderHeaderId == temp
                                                    select p;
                ProductJson.Add(new ComboBoxResult()
                {
                    id = prod.FirstOrDefault().SaleOrderHeaderId.ToString(),
                    text = prod.FirstOrDefault().DocType.DocumentTypeShortName + "-" +prod.FirstOrDefault().DocNo + "{" + prod.FirstOrDefault().SaleToBuyer.Code +"}"
                });
            }
            return Json(ProductJson);
        }


        public ActionResult GetBuyerSpecificationPackings(string searchTerm, int pageSize, int pageNum, string filter1, string filter2, string filter3, string filter4)
        {
            var Query = _PackingReportService.GetBuyerSpecificationPackings(searchTerm, filter1, filter2, filter3);
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

        public ActionResult GetBuyerSpecification1Packings(string searchTerm, int pageSize, int pageNum, string filter1, string filter2, string filter3, string filter4)
        {
            var Query = _PackingReportService.GetBuyerSpecification1Packings(searchTerm, filter1, filter2, filter3, filter4);
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
        public ActionResult GetBuyerSpecification2Packings(string searchTerm, int pageSize, int pageNum, string filter1, string filter2, string filter3, string filter4, string filter5)
        {
            var Query = _PackingReportService.GetBuyerSpecification2Packings(searchTerm, filter1, filter2, filter3, filter4, filter5);
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
        public ActionResult GetBuyerSpecification3Packings(string searchTerm, int pageSize, int pageNum, string filter1, string filter2)
        {
            var Query = _PackingReportService.GetBuyerSpecification3Packings(searchTerm, filter1, filter2);
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

        public ActionResult GetPackings(string searchTerm, int pageSize, int pageNum, string filter1)
        {
            var Query = _PackingReportService.GetPackings(searchTerm, filter1);
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

        public JsonResult SetPackings(string Ids)
        {
            string[] subStr = Ids.Split(',');
            List<ComboBoxResult> ProductJson = new List<ComboBoxResult>();
            for (int i = 0; i < subStr.Length; i++)
            {
                int temp = Convert.ToInt32(subStr[i]);
                //IEnumerable<Product> products = db.Products.Take(3);
                IEnumerable<PackingHeader> prod = from p in db.PackingHeader
                                                    where p.PackingHeaderId == temp
                                                    select p;
                ProductJson.Add(new ComboBoxResult()
                {
                    id = prod.FirstOrDefault().PackingHeaderId.ToString(),
                    text = prod.FirstOrDefault().DocType.DocumentTypeShortName + "-" + prod.FirstOrDefault().DocNo + "{" + prod.FirstOrDefault().Buyer.Code + "}"
                });
            }
            return Json(ProductJson);
        }

        public JsonResult GetParameterSettingsForLastDisplay()
        {
            var SettingList = (List<PackingReportFilterSettings>)System.Web.HttpContext.Current.Session["SettingList"];

            var LastSetting = (from H in SettingList select H).LastOrDefault();
            System.Web.HttpContext.Current.Session["CurrentSetting"] = LastSetting;

            var StatusOnDateSetting = (from H in LastSetting.PackingReportFilterParameters where H.ParameterName == "StatusOnDate" select H).FirstOrDefault();
            var DocTypeIdSetting = (from H in LastSetting.PackingReportFilterParameters where H.ParameterName == "DocTypeId" select H).FirstOrDefault();
            var SiteSetting = (from H in LastSetting.PackingReportFilterParameters where H.ParameterName == "Site" select H).FirstOrDefault();
            var DivisionSetting = (from H in LastSetting.PackingReportFilterParameters where H.ParameterName == "Division" select H).FirstOrDefault();
            var FromDateSetting = (from H in LastSetting.PackingReportFilterParameters where H.ParameterName == "FromDate" select H).FirstOrDefault();
            var ToDateSetting = (from H in LastSetting.PackingReportFilterParameters where H.ParameterName == "ToDate" select H).FirstOrDefault();
            var BuyerSetting = (from H in LastSetting.PackingReportFilterParameters where H.ParameterName == "Buyer" select H).FirstOrDefault();
            var SaleOrderHeaderIdSetting = (from H in LastSetting.PackingReportFilterParameters where H.ParameterName == "SaleOrderHeaderId" select H).FirstOrDefault();
            var ProductSetting = (from H in LastSetting.PackingReportFilterParameters where H.ParameterName == "Product" select H).FirstOrDefault();
            var ProductCategorySetting = (from H in LastSetting.PackingReportFilterParameters where H.ParameterName == "ProductCategory" select H).FirstOrDefault();
            var ProductQualitySetting = (from H in LastSetting.PackingReportFilterParameters where H.ParameterName == "ProductQuality" select H).FirstOrDefault();
            var ProductGroupSetting = (from H in LastSetting.PackingReportFilterParameters where H.ParameterName == "ProductGroup" select H).FirstOrDefault();
            var ProductSizeSetting = (from H in LastSetting.PackingReportFilterParameters where H.ParameterName == "ProductSize" select H).FirstOrDefault();
            var ReportFormatSetting = (from H in LastSetting.PackingReportFilterParameters where H.ParameterName == "ReportFormat" select H).FirstOrDefault();
            var ReportForSetting = (from H in LastSetting.PackingReportFilterParameters where H.ParameterName == "ReportFor" select H).FirstOrDefault();
            var NextFormatSetting = (from H in LastSetting.PackingReportFilterParameters where H.ParameterName == "NextFormat" select H).FirstOrDefault();
            var BuyerDesignSetting = (from H in LastSetting.PackingReportFilterParameters where H.ParameterName == "BuyerDesign" select H).FirstOrDefault();
            var TextHiddenSetting = (from H in LastSetting.PackingReportFilterParameters where H.ParameterName == "TextHidden" select H).FirstOrDefault();
            SettingList.Remove(LastSetting);
            
            return Json(new
            {
                StatusOnDate = StatusOnDateSetting.Value,
                DocTypeId = DocTypeIdSetting.Value,
                Site = SiteSetting.Value,
                Division = DivisionSetting.Value,
                FromDate = FromDateSetting.Value,
                ToDate = ToDateSetting.Value,
                Buyer = BuyerSetting.Value,
                SaleOrderHeaderId = SaleOrderHeaderIdSetting.Value,
                Product = ProductSetting.Value,
                ProductCategory = ProductCategorySetting.Value,
                ProductQuality = ProductQualitySetting.Value,
                ProductGroup = ProductGroupSetting.Value,
                ProductSize = ProductSizeSetting.Value,
                ReportFormat = ReportFormatSetting.Value,
                ReportFor = ReportForSetting.Value,
                NextFormat = NextFormatSetting.Value,
                BuyerDesign = BuyerDesignSetting.Value,
                TextHidden = TextHiddenSetting.Value,
            });
        }

        public ActionResult DocumentMenu(int DocTypeId, int DocId)
        {
            if (DocTypeId == 0 || DocId == 0)
            {
                return View("Error");
            }

            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];


            var DocumentType = new DocumentTypeService(_unitOfWork).Find(DocTypeId);


            if (DocumentType.ControllerActionId.HasValue && DocumentType.ControllerActionId.Value > 0)
            {
                ControllerAction CA = db.ControllerAction.Find(DocumentType.ControllerActionId.Value);

                if (CA == null)
                {
                    return View("~/Views/Shared/UnderImplementation.cshtml");
                }
                else if (!string.IsNullOrEmpty(DocumentType.DomainName))
                {
                    return Redirect(System.Configuration.ConfigurationManager.AppSettings[DocumentType.DomainName] + "/" + CA.ControllerName + "/" + CA.ActionName + "/" + DocId);
                }
                else
                {
                    return RedirectToAction(CA.ActionName, CA.ControllerName, new { id = DocId });
                }
            }

            ViewBag.RetUrl = System.Web.HttpContext.Current.Request.UrlReferrer;
            HandleErrorInfo Excp = new HandleErrorInfo(new Exception("Document Settings not Configured"), "TrialBalance", "DocumentMenu");
            return View("Error", Excp);


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
