using System.Collections.Generic;
using System.Linq;
using Data.Infrastructure;
using Model.ViewModel;
using System;
using Data.Models;
using System.Data.SqlClient;
using Model.ViewModels;

namespace Service
{
    public interface IPackingReportService : IDisposable
    {
        IEnumerable<PackingReportDataViewModel> PackingReportDetail(PackingReportFilterSettings Settings);
        IQueryable<ComboBoxResult> GetCustomSaleOrders(string term);
        IQueryable<ComboBoxResult> GetPackings(string term, string BuyerId);
        IEnumerable<ComboBoxResult> GetBuyerSpecificationPackings(string term, string BuyerId, string PackingHeaderId, string BuyerSpecification3);
        IEnumerable<ComboBoxResult> GetBuyerSpecification1Packings(string term, string BuyerId, string PackingHeaderId, string BuyerSpecification3, string BuyerSpecification);
        IEnumerable<ComboBoxResult> GetBuyerSpecification2Packings(string term, string BuyerId, string PackingHeaderId, string BuyerSpecification3, string BuyerSpecification, string BuyerSpecification1);
        IEnumerable<ComboBoxResult> GetBuyerSpecification3Packings(string term, string BuyerId, string PackingHeaderId);
    }

    public class PackingReportService : IPackingReportService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;



        public IQueryable<ComboBoxResult> GetCustomSaleOrders( string term)
        {
            
            var list = (from p in db.SaleOrderHeader
                        join TY in db.DocumentType on p.DocTypeId equals TY.DocumentTypeId into TYTable
                        from TYTab in TYTable.DefaultIfEmpty()
                        join B in db.Persons  on p.SaleToBuyerId equals B.PersonID into BTable
                        from BTab in BTable.DefaultIfEmpty()
                        where 1==1
                        && (string.IsNullOrEmpty(term) ? 1 == 1 : (p.DocNo.ToLower().Contains(term.ToLower()) || TYTab.DocumentTypeShortName.ToLower().Contains(term.ToLower()) || BTab.Code.ToLower().Contains(term.ToLower())))
                        group new { p, TYTab, BTab } by new { p.SaleOrderHeaderId } into Result
                        orderby Result.Max(m => m.p.DocNo )
                        select new ComboBoxResult
                        {
                            id = Result.Key.SaleOrderHeaderId.ToString(),
                            text = Result.Max(m => m.TYTab.DocumentTypeShortName + "-" + m.p.DocNo + "{" + m.BTab.Code + "}"),
                        }
              );

            return list;
        }

        public IQueryable<ComboBoxResult> GetPackings(string term, string BuyerId)
        {

            var list = (from p in db.PackingHeader
                        join TY in db.DocumentType on p.DocTypeId equals TY.DocumentTypeId into TYTable
                        from TYTab in TYTable.DefaultIfEmpty()
                        join B in db.Persons on p.BuyerId equals B.PersonID into BTable
                        from BTab in BTable.DefaultIfEmpty()
                        where 1 == 1
                        && (string.IsNullOrEmpty(BuyerId) ? 1 == 1 : (p.BuyerId.ToString().ToLower().Contains(BuyerId.ToLower()) ))
                        && (string.IsNullOrEmpty(term) ? 1 == 1 : (p.DocNo.ToLower().Contains(term.ToLower()) || TYTab.DocumentTypeShortName.ToLower().Contains(term.ToLower()) || BTab.Code.ToLower().Contains(term.ToLower())))
                        group new { p, TYTab, BTab } by new { p.PackingHeaderId } into Result
                        orderby Result.Max(m => m.p.DocNo)
                        select new ComboBoxResult
                        {
                            id = Result.Key.PackingHeaderId.ToString(),
                            text = Result.Max(m => m.TYTab.DocumentTypeShortName + "-" + m.p.DocNo + "{" + m.BTab.Code + "}"),
                        }
              );

            return list;
        }

        public IEnumerable<ComboBoxResult> GetBuyerSpecificationPackings(string term, string BuyerId, string PackingHeaderId, string BuyerSpecification3)
        {
            string mQry = "";
            mQry = @"SELECT DISTINCT  PB.BuyerSpecification AS Code, PB.BuyerSpecification AS Name
                    FROM Web.PackingHeaders H WITH (Nolock)
                    LEFT JOIN Web.PackingLines L WITH (Nolock) ON L.PackingHeaderId = H.PackingHeaderId 
                    LEFT JOIN Web.SaleOrderLines SOL WITH (Nolock) ON SOL.SaleOrderLineId = L.SaleOrderLineId 
                    LEFT JOIN web.SaleEnquiryLines SEL WITH (Nolock) ON SEL.SaleEnquiryLineId = SOL.ReferenceDocLineId 
                    LEFT JOIN web.SaleEnquiryLineExtendeds PB WITH (Nolock) ON PB.SaleEnquiryLineId = SEL.SaleEnquiryLineId 
                    WHERE PB.BuyerSpecification IS NOT NULL ";

            if (BuyerId != "")
            {
                mQry = mQry + " AND H.BuyerId IN ( " + BuyerId + " ) ";
            }

            if (PackingHeaderId != "")
            {
                mQry = mQry + " AND H.PackingHeaderId IN ( " + PackingHeaderId + " ) ";
            }

            if (BuyerSpecification3 != "")
            {
                mQry = mQry + " AND PB.BuyerSpecification3 IN (SELECT Items FROM [dbo].[Split] ('" + BuyerSpecification3 + "', ',')) ";
            }

            if (term != "")
            {
                mQry = mQry + " AND PB.BuyerSpecification LIKE '%" + term + "%'";
            }

            IEnumerable<PackingReportBuyerSpecificationFilter> DT = db.Database.SqlQuery<PackingReportBuyerSpecificationFilter>(mQry).ToList();


            var list = (from p in DT
                        select new ComboBoxResult
                        {
                            id = p.Code.ToString(),
                            text = p.Name.ToString(),
                        }
                    );

            return list;
        }

        public IEnumerable<ComboBoxResult> GetBuyerSpecification1Packings(string term, string BuyerId, string PackingHeaderId, string BuyerSpecification3, string BuyerSpecification)
        {
            string mQry = "";
            mQry = @"SELECT DISTINCT  PB.BuyerSpecification1 AS Code, PB.BuyerSpecification1 AS Name
                    FROM Web.PackingHeaders H WITH (Nolock)
                    LEFT JOIN Web.PackingLines L WITH (Nolock) ON L.PackingHeaderId = H.PackingHeaderId 
                    LEFT JOIN Web.SaleOrderLines SOL WITH (Nolock) ON SOL.SaleOrderLineId = L.SaleOrderLineId 
                    LEFT JOIN web.SaleEnquiryLines SEL WITH (Nolock) ON SEL.SaleEnquiryLineId = SOL.ReferenceDocLineId 
                    LEFT JOIN web.SaleEnquiryLineExtendeds PB WITH (Nolock) ON PB.SaleEnquiryLineId = SEL.SaleEnquiryLineId 
                    WHERE PB.BuyerSpecification1 IS NOT NULL ";

            if (BuyerId != "")
            {
                mQry = mQry + " AND H.BuyerId IN ( " + BuyerId + " ) ";
            }

            if (PackingHeaderId != "")
            {
                mQry = mQry + " AND H.PackingHeaderId IN ( " + PackingHeaderId + " ) ";
            }

            if (BuyerSpecification3 != "")
            {
                mQry = mQry + " AND PB.BuyerSpecification3 IN (SELECT Items FROM [dbo].[Split] ('" + BuyerSpecification3 + "', ',')) ";
            }

            if (BuyerSpecification != "")
            {
                mQry = mQry + " AND PB.BuyerSpecification IN (SELECT Items FROM [dbo].[Split] ('" + BuyerSpecification + "', ',')) ";
            }

            if (term != "")
            {
                mQry = mQry + " AND PB.BuyerSpecification1 LIKE '%" + term + "%'";
            }

            IEnumerable<PackingReportBuyerSpecificationFilter> DT = db.Database.SqlQuery<PackingReportBuyerSpecificationFilter>(mQry).ToList();


            var list = (from p in DT
                        select new ComboBoxResult
                        {
                            id = p.Code.ToString(),
                            text = p.Name.ToString(),
                        }
                    );

            return list;
        }

        public IEnumerable<ComboBoxResult> GetBuyerSpecification2Packings(string term, string BuyerId, string PackingHeaderId, string BuyerSpecification3, string BuyerSpecification, string BuyerSpecification1)
        {
            string mQry = "";
            mQry = @"SELECT DISTINCT  PB.BuyerSpecification2 AS Code, PB.BuyerSpecification2 AS Name
                    FROM Web.PackingHeaders H WITH (Nolock)
                    LEFT JOIN Web.PackingLines L WITH (Nolock) ON L.PackingHeaderId = H.PackingHeaderId 
                    LEFT JOIN Web.SaleOrderLines SOL WITH (Nolock) ON SOL.SaleOrderLineId = L.SaleOrderLineId 
                    LEFT JOIN web.SaleEnquiryLines SEL WITH (Nolock) ON SEL.SaleEnquiryLineId = SOL.ReferenceDocLineId 
                    LEFT JOIN web.SaleEnquiryLineExtendeds PB WITH (Nolock) ON PB.SaleEnquiryLineId = SEL.SaleEnquiryLineId 
                    WHERE PB.BuyerSpecification2 IS NOT NULL ";

            if (BuyerId != "")
            {
                mQry = mQry + " AND H.BuyerId IN ( " + BuyerId + " ) ";
            }

            if (PackingHeaderId != "")
            {
                mQry = mQry + " AND H.PackingHeaderId IN ( " + PackingHeaderId + " ) ";
            }

            if (BuyerSpecification3 != "")
            {
                mQry = mQry + " AND PB.BuyerSpecification3 IN (SELECT Items FROM [dbo].[Split] ('" + BuyerSpecification3 + "', ',')) ";
            }

            if (BuyerSpecification != "")
            {
                mQry = mQry + " AND PB.BuyerSpecification IN (SELECT Items FROM [dbo].[Split] ('" + BuyerSpecification + "', ',')) ";
            }

            if (BuyerSpecification1 != "")
            {
                mQry = mQry + " AND PB.BuyerSpecification1 IN (SELECT Items FROM [dbo].[Split] ('" + BuyerSpecification1 + "', ',')) ";
            }

            if (term != "")
            {
                mQry = mQry + " AND PB.BuyerSpecification2 LIKE '%" + term + "%'";
            }

            IEnumerable<PackingReportBuyerSpecificationFilter> DT = db.Database.SqlQuery<PackingReportBuyerSpecificationFilter>(mQry).ToList();


            var list = (from p in DT
                        select new ComboBoxResult
                        {
                            id = p.Code.ToString(),
                            text = p.Name.ToString(),
                        }
                    );

            return list;
        }

        public IEnumerable<ComboBoxResult> GetBuyerSpecification3Packings(string term, string BuyerId, string PackingHeaderId)
        {
            string mQry = "";
            mQry = @"SELECT DISTINCT  PB.BuyerSpecification3 AS Code, PB.BuyerSpecification3 AS Name
                    FROM Web.PackingHeaders H WITH (Nolock)
                    LEFT JOIN Web.PackingLines L WITH (Nolock) ON L.PackingHeaderId = H.PackingHeaderId 
                    LEFT JOIN Web.SaleOrderLines SOL WITH (Nolock) ON SOL.SaleOrderLineId = L.SaleOrderLineId 
                    LEFT JOIN web.SaleEnquiryLines SEL WITH (Nolock) ON SEL.SaleEnquiryLineId = SOL.ReferenceDocLineId 
                    LEFT JOIN web.SaleEnquiryLineExtendeds PB WITH (Nolock) ON PB.SaleEnquiryLineId = SEL.SaleEnquiryLineId 
                    WHERE PB.BuyerSpecification3 IS NOT NULL ";

            if (BuyerId != "")
            {
                mQry = mQry + " AND H.BuyerId IN ( " + BuyerId + " ) ";
            }

            if (PackingHeaderId != "")
            {
                mQry = mQry + " AND H.PackingHeaderId IN ( " + PackingHeaderId + " ) ";
            }

            if (term != "")
            {
                mQry = mQry + " AND PB.BuyerSpecification3 LIKE '%" + term + "%'";
            }

            IEnumerable<PackingReportBuyerSpecificationFilter> DT = db.Database.SqlQuery<PackingReportBuyerSpecificationFilter>(mQry).ToList();


            var list = (from p in DT
                        select new ComboBoxResult
                        {
                            id = p.Code.ToString(),
                            text = p.Name.ToString(),
                        }
                    );

            return list;
        }
        public PackingReportService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;            
        }

        public IEnumerable<PackingReportDataViewModel> PackingReportDetail(PackingReportFilterSettings Settings)
        {

            var ReportFormatSetting = (from H in Settings.PackingReportFilterParameters where H.ParameterName == "ReportFormat" select H).FirstOrDefault();
            var AreaUnitSetting = (from H in Settings.PackingReportFilterParameters where H.ParameterName == "AreaUnit" select H).FirstOrDefault();
            var SiteSetting = (from H in Settings.PackingReportFilterParameters where H.ParameterName == "Site" select H).FirstOrDefault();
            var DivisionSetting = (from H in Settings.PackingReportFilterParameters where H.ParameterName == "Division" select H).FirstOrDefault();
            var FromDateSetting = (from H in Settings.PackingReportFilterParameters where H.ParameterName == "FromDate" select H).FirstOrDefault();
            var ToDateSetting = (from H in Settings.PackingReportFilterParameters where H.ParameterName == "ToDate" select H).FirstOrDefault();
            var BuyerSetting = (from H in Settings.PackingReportFilterParameters where H.ParameterName == "Buyer" select H).FirstOrDefault();
            var PackingHeaderIdSetting = (from H in Settings.PackingReportFilterParameters where H.ParameterName == "PackingHeaderId" select H).FirstOrDefault();
            var SaleOrderHeaderIdSetting = (from H in Settings.PackingReportFilterParameters where H.ParameterName == "SaleOrderHeaderId" select H).FirstOrDefault();
            var ProductCategorySetting = (from H in Settings.PackingReportFilterParameters where H.ParameterName == "ProductCategory" select H).FirstOrDefault();
            var ProductQualitySetting = (from H in Settings.PackingReportFilterParameters where H.ParameterName == "ProductQuality" select H).FirstOrDefault();
            var ProductGroupSetting = (from H in Settings.PackingReportFilterParameters where H.ParameterName == "ProductGroup" select H).FirstOrDefault();
            var ProductSizeSetting = (from H in Settings.PackingReportFilterParameters where H.ParameterName == "ProductSize" select H).FirstOrDefault();
            var ProductSetting = (from H in Settings.PackingReportFilterParameters where H.ParameterName == "Product" select H).FirstOrDefault();
            var ReportTypeSetting = (from H in Settings.PackingReportFilterParameters where H.ParameterName == "ReportType" select H).FirstOrDefault();
            var BuyerQualitySetting = (from H in Settings.PackingReportFilterParameters where H.ParameterName == "BuyerQuality" select H).FirstOrDefault();
            var BuyerDesignSetting = (from H in Settings.PackingReportFilterParameters where H.ParameterName == "BuyerDesign" select H).FirstOrDefault();
            var BuyerSizeSetting = (from H in Settings.PackingReportFilterParameters where H.ParameterName == "BuyerSize" select H).FirstOrDefault();
            var BuyerColourSetting = (from H in Settings.PackingReportFilterParameters where H.ParameterName == "BuyerColour" select H).FirstOrDefault();




            string ReportFormate = ReportFormatSetting.Value;
            string AreaUnit = AreaUnitSetting.Value;
            string Site = "1";
            string Division = "1";
            string FromDate = FromDateSetting.Value;
            string ToDate = ToDateSetting.Value;
            string Buyer = BuyerSetting.Value;
            string PackingHeaderId = PackingHeaderIdSetting.Value;
            string SaleOrderHeaderId = SaleOrderHeaderIdSetting.Value;
            string ProductCategory = ProductCategorySetting.Value;
            string ProductQuality = ProductQualitySetting.Value;
            string ProductGroup = ProductGroupSetting.Value;
            string ProductSize = ProductSizeSetting.Value;
            string Product = ProductSetting.Value;
            string BuyerQuality = BuyerQualitySetting.Value;
            string BuyerDesign = BuyerDesignSetting.Value;
            string BuyerSize = BuyerSizeSetting.Value;
            string BuyerColour = BuyerColourSetting.Value;


            string mQry;

         
            string SqlParameterReportFormat = !string.IsNullOrEmpty(ReportFormate) ? "'" + ReportFormate + "'" : "Null";
            string SqlParameterAreaUnit = !string.IsNullOrEmpty(AreaUnit) ? "'" + AreaUnit + "'" : "Null";
            string SqlParameterSite = !string.IsNullOrEmpty(Site) ? "'" + Site + "'" : "Null";
            string SqlParameterDivision = !string.IsNullOrEmpty(Division) ? "'" + Division + "'" : "Null";
            string SqlParameterFromDate = !string.IsNullOrEmpty(FromDate) ? "'" + FromDate + "'" : "Null";
            string SqlParameterToDate = !string.IsNullOrEmpty(ToDate) ? "'" + ToDate + "'" : "Null";
            string SqlParameterPackingHeaderId = !string.IsNullOrEmpty(PackingHeaderId) ? "'" + PackingHeaderId + "'" : "Null";
            string SqlParameterBuyer = !string.IsNullOrEmpty(Buyer) ? "'" + Buyer + "'" : "Null";
            string SqlParameterSaleOrderHeaderId = !string.IsNullOrEmpty(SaleOrderHeaderId) ? "'" + SaleOrderHeaderId + "'" : "Null";
            string SqlParameterProductCategory = !string.IsNullOrEmpty(ProductCategory) ? "'" + ProductCategory + "'" : "Null";
            string SqlParameterProductGroup = !string.IsNullOrEmpty(ProductGroup) ? "'" + ProductGroup + "'" : "Null";
            string SqlParameterProductSize = !string.IsNullOrEmpty(ProductSize) ? "'" + ProductSize + "'" : "Null";
            string SqlParameterProduct = !string.IsNullOrEmpty(Product) ? "'" + Product + "'" : "Null";
            string SqlParameterBuyerQuality = !string.IsNullOrEmpty(BuyerQuality) ? "'" + BuyerQuality + "'" : "Null";
            string SqlParameterBuyerDesign = !string.IsNullOrEmpty(BuyerDesign) ? "'" + BuyerDesign + "'" : "Null";
            string SqlParameterBuyerSize = !string.IsNullOrEmpty(BuyerSize) ? "'" + BuyerSize + "'" : "Null";
            string SqlParameterBuyerColour = !string.IsNullOrEmpty(BuyerColour) ? "'" + BuyerColour + "'" : "Null";

            mQry = @"Web.Grid_PackingRegister 
            @ReportType='P04',  
            @ReportFormat=" + SqlParameterReportFormat + @",
            @AreaUnit=" + SqlParameterAreaUnit + @",
            @Site=" + SqlParameterSite + @",
            @Division=" + SqlParameterDivision + @",
            @FromDate=" + SqlParameterFromDate + @",
            @ToDate=" + SqlParameterToDate + @", 
            @Buyer=" + SqlParameterBuyer + @",
            @PackingHeaderId=" + SqlParameterPackingHeaderId + @",
            @SaleOrderHeaderId=" + SqlParameterSaleOrderHeaderId + @",
            @ProductCategory=" + SqlParameterProductCategory + @",
            @ProductGroup=" + SqlParameterProductGroup + @",
            @ProductSize=" + SqlParameterProductSize + @",
            @Product=" + SqlParameterProduct + @",
            @BuyerSpecification=" + SqlParameterBuyerDesign + @",
            @BuyerSpecification1=" + SqlParameterBuyerSize + @",
            @BuyerSpecification2=" + SqlParameterBuyerColour + @",
            @BuyerSpecification3 = " + SqlParameterBuyerQuality + @"";

            IEnumerable<PackingReportDataViewModel> SaleOrderInventoryStatusList = db.Database.SqlQuery<PackingReportDataViewModel>(mQry).ToList();

            //mQry = "Web.Grid_PackingRegister @ReportType, @ReportFormat, @Site, @Division, @FromDate, @ToDate, @PackingHeaderId @AreaUnit ";
            //mQry = "Web.Grid_PackingRegister @ReportType='P04',  @ReportFormat='Quality Design Wise Summary',@Site='1',@Division=" + SqlParameterDivision  + ",@FromDate='01/Apr/2017',@ToDate='01/Apr/2019', @PackingHeaderId='8021'";

            //IEnumerable<PackingReportDataViewModel> SaleOrderInventoryStatusList = db.Database.SqlQuery<PackingReportDataViewModel>(mQry,  SqlParameterDocTypeId, SqlParameterSite, SqlParameterDivision, SqlParameterFromDate, SqlParameterToDate, SqlParameterBuyer, SqlParameterSaleOrderHeaderId, SqlParameterPackingHeaderId, SqlParameterProduct, SqlParameterProductCategory, SqlParameterProductQuality, SqlParameterProductGroup, SqlParameterProductSize, SqlParameterReportFor, SqlParameterBuyerDesign).ToList();
            //IEnumerable<PackingReportDataViewModel> SaleOrderInventoryStatusList = db.Database.SqlQuery<PackingReportDataViewModel>(mQry, null, null, SqlParameterSite, SqlParameterDivision, SqlParameterFromDate, SqlParameterToDate, SqlParameterPackingHeaderId, SqlParameterReportFor).ToList();



            return SaleOrderInventoryStatusList;

        }
        

        public void Dispose()
        {
        }
    }

    public class PackingReportViewModel
    {
        public string ReportFormat { get; set; }
        public string AreaUnit { get; set; }
        public string Site { get; set; }
        public string Division { get; set; }  
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Buyer { get; set; }
        public string PackingHeaderId { get; set; }
        public string SaleOrderHeaderId { get; set; }
        public string ProductCategory { get; set; }
        public string ProductQuality { get; set; }
        public string ProductGroup { get; set; }
        public string ProductSize { get; set; }
        public string Product { get; set; }
        public string BuyerQuality { get; set; }
        public string BuyerDesign { get; set; }
        public string BuyerSize { get; set; }
        public string BuyerColour { get; set; }

    }

    [Serializable()]
    public class PackingReportFilterSettings
    {
        public string Format { get; set; }
        public List<PackingReportFilterParameters> PackingReportFilterParameters { get; set; }
    }

    [Serializable()]
    public class PackingReportFilterParameters
    {
        public string ParameterName { get; set; }
        public bool IsApplicable { get; set; }
        public string Value { get; set; }
    }

    public class PackingReportDataViewModel
    {
        
        public string Invoice_No { get; set; }
        public string Buyer { get; set; }
        public string Quality { get; set; }
        public string Design { get; set; }
        public string Size { get; set; }
        public string Colour { get; set; }
        public decimal? Pcs { get; set; }
        public decimal? Area { get; set; } 


    }

    public class PackingReportBuyerSpecificationFilter
    {

        public string Code { get; set; }
        public string Name { get; set; }

    }
}

