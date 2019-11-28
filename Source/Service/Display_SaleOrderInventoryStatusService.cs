﻿using System.Collections.Generic;
using System.Linq;
using Data.Infrastructure;
using Model.ViewModel;
using System;
using Data.Models;
using System.Data.SqlClient;
using Model.ViewModels;

namespace Service
{
    public interface IDisplay_SaleOrderInventoryStatusService : IDisposable
    {
        IEnumerable<SaleOrderInventoryStatusViewModel> SaleOrderInventoryStatusDetail(SaleOrderInventoryStatusDisplayFilterSettings Settings);
        IEnumerable<SaleOrderInventoryStatus_StockViewModel> StockDetail(SaleOrderInventoryStatusDisplayFilterSettings Settings);
        IEnumerable<SaleOrderInventoryStatus_LoomViewModel> LoomDetail(SaleOrderInventoryStatusDisplayFilterSettings Settings);
        IEnumerable<SaleOrderInventoryStatus_ShipViewModel> ShipDetail(SaleOrderInventoryStatusDisplayFilterSettings Settings);
        IEnumerable<SaleOrderInventoryStatus_ToBeIssueViewModel> ToBeIssueDetail(SaleOrderInventoryStatusDisplayFilterSettings Settings);
        IEnumerable<SaleOrderInventoryStatus_DOViewModel> DODetail(SaleOrderInventoryStatusDisplayFilterSettings Settings);
        IEnumerable<SaleOrderInventoryStatus_DOViewModel> DBDetail(SaleOrderInventoryStatusDisplayFilterSettings Settings);
        IEnumerable<SaleOrderInventoryStatus_OXViewModel> OXDetail(SaleOrderInventoryStatusDisplayFilterSettings Settings);
        IQueryable<ComboBoxResult> GetCustomSaleOrders(string term);
    }

    public class Display_SaleOrderInventoryStatusService : IDisplay_SaleOrderInventoryStatusService
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

        public Display_SaleOrderInventoryStatusService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;            
        }

        public IEnumerable<SaleOrderInventoryStatusViewModel> SaleOrderInventoryStatusDetail(SaleOrderInventoryStatusDisplayFilterSettings Settings)
        {
            var StatusOnDateSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "StatusOnDate" select H).FirstOrDefault();
            var DocTypeIdSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "DocTypeId" select H).FirstOrDefault();
            var SiteSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "Site" select H).FirstOrDefault();
            var DivisionSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "Division" select H).FirstOrDefault();
            var FromDateSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "FromDate" select H).FirstOrDefault();
            var ToDateSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ToDate" select H).FirstOrDefault();
            var BuyerSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "Buyer" select H).FirstOrDefault();
            var SaleOrderHeaderIdSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "SaleOrderHeaderId" select H).FirstOrDefault();
            var ProductSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "Product" select H).FirstOrDefault();
            var ProductCategorySetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ProductCategory" select H).FirstOrDefault();
            var ProductQualitySetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ProductQuality" select H).FirstOrDefault();
            var ProductGroupSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ProductGroup" select H).FirstOrDefault();
            var ProductSizeSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ProductSize" select H).FirstOrDefault();
            var ReportTypeSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ReportType" select H).FirstOrDefault();
            var ReportForSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ReportFor" select H).FirstOrDefault();
            var NextFormatSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "NextFormat" select H).FirstOrDefault();
            var BuyerDesignSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "BuyerDesign" select H).FirstOrDefault();
            var TextHiddenSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "TextHidden" select H).FirstOrDefault();



            string StatusOnDate = StatusOnDateSetting.Value;
            string DocTypeId = DocTypeIdSetting.Value;
            //string Site = SiteSetting.Value;
            //string Division = DivisionSetting.Value;
            string Site = "1";
            string Division = "1";
            string FromDate = FromDateSetting.Value;
            string ToDate = ToDateSetting.Value;
            string Buyer = BuyerSetting.Value;
            string SaleOrderHeaderId = SaleOrderHeaderIdSetting.Value;
            string Product = ProductSetting.Value;
            string ProductCategory = ProductCategorySetting.Value;
            string ProductQuality = ProductQualitySetting.Value;
            string ProductGroup = ProductGroupSetting.Value;
            string ProductSize = ProductSizeSetting.Value;
            string ReportType = ReportTypeSetting.Value;
            string ReportFor = ReportForSetting.Value;
            string NextFormat = NextFormatSetting.Value;
            string BuyerDesign = BuyerDesignSetting.Value;
            string TextHidden = TextHiddenSetting.Value;  

            
            string mQry, mCondStr;


            mCondStr = "";

            if (Buyer == "-1")
                Buyer = null;

            SqlParameter SqlParameterStatusOnDate = new SqlParameter("@StatusOnDate", !string.IsNullOrEmpty(StatusOnDate) ? StatusOnDate : (object)DBNull.Value);
            SqlParameter SqlParameterDocTypeId = new SqlParameter("@DocumentType", !string.IsNullOrEmpty(DocTypeId) ? DocTypeId : (object)DBNull.Value);
            SqlParameter SqlParameterSite = new SqlParameter("@Site", !string.IsNullOrEmpty(Site) ? Site : (object)DBNull.Value);
            SqlParameter SqlParameterDivision = new SqlParameter("@Division", !string.IsNullOrEmpty(Division) ? Division : (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", !string.IsNullOrEmpty(FromDate) ? FromDate : (object)DBNull.Value);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", !string.IsNullOrEmpty(ToDate) ? ToDate : (object)DBNull.Value);
            SqlParameter SqlParameterBuyer = new SqlParameter("@Buyer", !string.IsNullOrEmpty(Buyer) ? Buyer : (object)DBNull.Value);
            SqlParameter SqlParameterSaleOrderHeaderId = new SqlParameter("@SaleOrderHeaderId", !string.IsNullOrEmpty(SaleOrderHeaderId) ? SaleOrderHeaderId : (object)DBNull.Value);
            SqlParameter SqlParameterProduct = new SqlParameter("@Product", !string.IsNullOrEmpty(Product) ? Product : (object)DBNull.Value);
            SqlParameter SqlParameterProductCategory = new SqlParameter("@ProductCategory", !string.IsNullOrEmpty(ProductCategory) ? ProductCategory : (object)DBNull.Value);
            SqlParameter SqlParameterProductQuality = new SqlParameter("@ProductQuality", !string.IsNullOrEmpty(ProductQuality) ? ProductQuality : (object)DBNull.Value);
            SqlParameter SqlParameterProductGroup = new SqlParameter("@ProductGroup", !string.IsNullOrEmpty(ProductGroup) ? ProductGroup : (object)DBNull.Value);
            SqlParameter SqlParameterProductSize = new SqlParameter("@ProductSize", !string.IsNullOrEmpty(ProductSize) ? ProductSize : (object)DBNull.Value);
            SqlParameter SqlParameterReportType = new SqlParameter("@ReportType", !string.IsNullOrEmpty(ReportType) ? ReportType : (object)DBNull.Value);
            SqlParameter SqlParameterReportFor = new SqlParameter("@ReportFor", !string.IsNullOrEmpty(ReportFor) ? ReportFor : (object)DBNull.Value);
            SqlParameter SqlParameterNextFormat = new SqlParameter("@NextFormat", !string.IsNullOrEmpty(NextFormat) ? NextFormat : (object)DBNull.Value);
            SqlParameter SqlParameterBuyerDesign = new SqlParameter("@BuyerDesign", !string.IsNullOrEmpty(BuyerDesign) ? BuyerDesign : (object)DBNull.Value);
            SqlParameter SqlParameterTextHidden = new SqlParameter("@TextHidden", !string.IsNullOrEmpty(TextHidden) ? TextHidden : (object)DBNull.Value);


            mQry = @"IF OBJECT_ID ('TempGrid_SaleOrderTracking') IS NOT NULL
	                    DROP TABLE TempGrid_SaleOrderTracking
	
                    DECLARE @Process  VARCHAR(Max)=43

                    SET @StatusOnDate=(CASE WHEN @StatusOnDate IS NULL THEN getdate() ELSE @StatusOnDate END)

                    ---------------Sale Order------------
                    SELECT  Max(H.SaleOrderHeaderId) AS SaleOrderHeaderId, Max(SOH.SaleToBuyerId) SaleToBuyerId,  H.SaleOrderLineId, Max(B.Code) AS Buyer, max(SOH.DocDate) AS DocDate,max(SOH.DocNo) AS DocNo, max(H.DivisionId) AS DivisionId,max(H.SiteId) AS SiteId,  max(H.DueDate) AS DueDate, Max(H.ProductId) AS ProductId, 
                    Sum(isnull(H.SaleOrderQty,0)) AS  OrdQty,Sum(isnull(H.CancelQty,0)) AS  CancelQty, Sum(isnull(H.DispatchQty,0)) AS  DispQty,
                    Sum(isnull(H.SaleOrderQty,0)) - Sum(isnull(H.CancelQty,0)) - Sum(isnull(H.DispatchQty,0)) AS  BalQty
                    INTO #FSaleOrder FROM Web.FSaleOrderStatus1(@StatusOnDate,@Site,@Division,@FromDate,@ToDate,@DocumentType,@Product,@SaleOrderHeaderId) H
                    LEFT JOIN web.SaleOrderHeaders SOH WITH (Nolock) ON SOH.SaleOrderHeaderId = H.SaleOrderHeaderId
                    LEFT JOIN web.People B WITH (Nolock) ON B.PersonID = SOH.SaleToBuyerId 
                    LEFT JOIN web.Products PR WITH (Nolock) ON PR.ProductId =H.ProductId
                    LEFT JOIN web.FinishedProduct FP WITH (Nolock) ON FP.ProductId = H.ProductId
                    WHERE 1=1 " +
                    (DocTypeId != null ? " AND H.DocTypeId IN (SELECT Items FROM [dbo].[Split] (@DocumentType, ','))" : "") +
                    (Product != null ? " AND H.ProductId IN (SELECT Items FROM [dbo].[Split] (@Product, ','))" : "") +
                    (ProductQuality != null ? " AND Fp.ProductQualityId IN (SELECT Items FROM  Web.[Split] (@ProductQuality, ','))" : "") +
                    (ProductCategory != null ? " AND PR.ProductCategoryId  IN (SELECT Items FROM  Web.[Split] (@ProductCategory, ','))" : "") +
                    (ProductGroup != null ? " AND PR.ProductGroupId IN (SELECT Items FROM  Web.[Split] (@ProductGroup, ','))" : "") +
                    (SaleOrderHeaderId != null ? " AND H.SaleOrderHeaderId IN (SELECT Items FROM [dbo].[Split] (@SaleOrderHeaderId, ','))" : "") +
                    (Buyer != null ? " AND SOH.SaleToBuyerId  IN (SELECT Items FROM [dbo].[Split] (@Buyer, ','))" : "") +
                    @"GROUP BY H.SaleOrderLineId


                    ---------------Prod Order------------
                    SELECT  H.ProdOrderHeaderId, H.SaleOrderLineId, max(H.DocDate) AS DocDate,max(H.DocNo) AS DocNo, max(H.DivisionId) AS DivisionId,max(H.SiteId) AS SiteId,  max(H.DueDate) AS DueDate, Max(H.ProductId) AS ProductId , Sum(isnull(H.Qty,0)) AS  Qty
                    INTO #FProdOrder FROM Web.FProdOrder_OneDocumentType1(@Site,@Division,@FromDate,@ToDate,273,@StatusOnDate) H
                    WHERE 1=1 " +
                    (Product != null ? " AND H.ProductId IN (SELECT Items FROM [dbo].[Split] (@Product, ','))" : "") +
                    (Buyer != null ? " AND H.BuyerId IN (SELECT Items FROM [dbo].[Split] (@Buyer, ','))" : "") +
                    @" GROUP BY H.ProdOrderHeaderId,H.SaleOrderLineId

                    ------------------------Prod Order Balance----------------

                    SELECT H.ProdOrderHeaderId,H.ProductId,isnull(sum(H.BalanceQty),0) AS BalanceQty,Sum(isnull(H.BalanceQty,0)) * max(VRS.StandardSqYard) AS ToBesqyard 
                    INTO #FProdOrderBalance 
                    FROM 
                    Web.FProdOrderBalance_OneDocumentType(@StatusOnDate,@Site,@Division,@FromDate,@StatusOnDate,273) H
                    LEFT JOIN Web._ViewRugSize VRS WITH (Nolock) ON VRS.ProductId=H.ProductId
                    WHERE 1=1 " +
                    (Buyer != null ? " And H.BuyerId IN (SELECT Items FROM [dbo].[Split] (@Buyer, ','))" : "") +
                    @" GROUP BY H.ProdOrderHeaderId,H.ProductId

                    ------------------------Prod Order Balance In Branch----------------

                    SELECT POL1.ProdOrderHeaderId,POL1.ProductId,isnull(sum(H.BalanceQty),0) AS BalanceQty
                    INTO #FProdOrderBalanceinBranch
                    FROM Web.FProdOrderBalance_OneDocumentType_InBrabch(@StatusOnDate,273) H
                    LEFT JOIN web.ProdOrderLines POL WITH (Nolock) ON POL.ProdOrderLineId = H.ProdOrderLineId
                    LEFT JOIN web.JobOrderLines JOL WITH (Nolock) ON JOL.JobOrderLineId = POL.ReferenceDocLineId 
                    LEFT JOIN web.ProdOrderLines POL1 WITH (Nolock) ON POL1.ProdOrderLineId = JOL.ProdOrderLineId 
                    LEFT JOIN web.ProdOrderHeaders POH WITH (Nolock) ON  POH.ProdOrderHeaderId = POL1.ProdOrderHeaderId 
                    WHERE 1=1 AND POL1.ProdOrderHeaderId IS NOT NULL " +
                    (Buyer != null ? " And POH.BuyerId IN (SELECT Items FROM [dbo].[Split] (@Buyer, ','))" : "") +
                    @" GROUP BY POL1.ProdOrderHeaderId,POL1.ProductId

                    --------------------------------OrderIssue---------------


                    SELECT POL.ProdOrderHeaderId,sum(isnull(H.OrderQty,0)) AS OrderQty, H.ProductId,
                    sum(isnull(H.BalanceQty,0)) * max(VRS.StandardSqYard) AS LoomAqyard INTO #OrderIssue
                    FROM [Web].[FWeavingOrder_OneProcess] (@StatusOnDate,@Site,@Division,@FromDate,@StatusOnDate,@Process) H
                    LEFT JOIN Web.ProdOrderLines POL WITH (Nolock) ON POL.ProdOrderLineId=H.ProdOrderLineId
                    LEFT JOIN Web._ViewRugSize VRS WITH (Nolock) ON VRS.ProductId=H.ProductId
                    GROUP BY POL.ProdOrderHeaderId,H.ProductId



                    ------------------F Weaving Receive---------------------------
                 /*   SELECT POL.ProdOrderHeaderId,Sum(isnull(H.Qty,0)) AS Qty,Sum(isnull(H.Qty,0)) * max(VRS.StandardSqYard) AS StockYard
                    ,H.ProductId,
                    min(H.DocDate) AS FBDate,Max(H.DocDate) AS LBDate INTO #FWR
                    FROM Web.FJobReceive_OneProcess (@StatusOnDate,@Site,@Division,@FromDate,@StatusOnDate,@Process) H
                    LEFT JOIN Web._ViewRugSize VRS WITH (Nolock) ON VRS.ProductId=H.ProductId
                    LEFT JOIN Web.ProdOrderLines POL WITH (Nolock) ON POL.ProdOrderLineId=H.ProdOrderLineId
                    GROUP BY POL.ProdOrderHeaderId,H.ProductId
 

                    SELECT V.* 
                    INTO #FWR
                    FROM 
                    (
                    SELECT POL.ProdOrderHeaderId,Sum(isnull(H.Qty,0)) AS Qty,Sum(isnull(H.Qty,0)) * max(VRS.StandardSqYard) AS StockYard
                    ,H.ProductId, 
                    min(H.DocDate) AS FBDate,Max(H.DocDate) AS LBDate 
                    FROM Web.FJobReceive_OneProcess (@StatusOnDate,@Site,@Division,@FromDate,@StatusOnDate,@Process) H
                    LEFT JOIN Web._ViewRugSize VRS WITH (Nolock) ON VRS.ProductId=H.ProductId
                    LEFT JOIN Web.ProdOrderLines POL WITH (Nolock) ON POL.ProdOrderLineId=H.ProdOrderLineId
                    LEFT JOIN web.Products P WITH (Nolock) ON P.ProductId = POL.ProductId
                    LEFT JOIN web.ProductCategories  PC WITH (Nolock) ON PC.ProductCategoryId = P.ProductCategoryId 
                    WHERE PC.ProductCategoryName <> 'OVER TUFT'
                    GROUP BY POL.ProdOrderHeaderId,H.ProductId
                    UNION ALL 
                     SELECT POL.ProdOrderHeaderId,Sum(isnull(H.Qty,0)) AS Qty,Sum(isnull(H.Qty,0)) * max(VRS.StandardSqYard) AS StockYard
                    ,H.ProductId, 
                    min(H.DocDate) AS FBDate,Max(H.DocDate) AS LBDate 
                    FROM Web.FJobReceive_OneProcess (@StatusOnDate,@Site,@Division,@FromDate,@StatusOnDate,2008) H
                    LEFT JOIN Web._ViewRugSize VRS WITH (Nolock) ON VRS.ProductId=H.ProductId
                    LEFT JOIN web.JobReceiveLines JRL ON JRL.JobReceiveLineId = H.JobReceiveLineId
                    LEFT JOIN 
                    (
                    SELECT L.ProductUidId, L.LotNo, JOL.JobOrderLineId, JOL.ProdOrderLineId, JOL.ProductId 
					FROM web.JobReceiveHeaders H WITH (NoLock)
					LEFT JOIN web.JobReceiveLines L WITH (NoLock) ON L.JobReceiveHeaderId = H.JobReceiveHeaderId
					LEFT JOIN web.JobOrderLines JOL WITH (NoLock) ON JOL.JobOrderLineId = L.JobOrderLineId
					WHERE H.SiteId =1 AND H.DivisionId =1 AND H.DocTypeId =448
                    ) V ON V.ProductUidId = JRL.ProductUidId OR JRL.lotNo = V.LotNo
                    LEFT JOIN Web.ProdOrderLines POL WITH (Nolock) ON POL.ProdOrderLineId=V.ProdOrderLineId
                    LEFT JOIN web.Products P WITH (Nolock) ON P.ProductId = POL.ProductId
                    LEFT JOIN web.ProductCategories  PC WITH (Nolock) ON PC.ProductCategoryId = P.ProductCategoryId 
                    WHERE PC.ProductCategoryName = 'OVER TUFT'
                    GROUP BY POL.ProdOrderHeaderId,H.ProductId
                    ) V

*/
                    ----------------- Stock Balance -------------

 /* 
                    SELECT VS.SaleOrderLineId, sum(VS.Qty) AS StockQty  
                    INTO #FStock
                    FROM 
                    (
                    SELECT PU.SaleOrderLineId, sum(isnull(S.Qty_Rec,0)- isnull(S.Qty_Iss,0)) AS Qty
                    FROM Web.StockHeaders H WITH (Nolock)
                    LEFT JOIN web.Stocks S WITH (Nolock) ON S.StockHeaderId = H.StockHeaderId 
                    LEFT JOIN web.Products P WITH (Nolock) ON P.ProductId = S.ProductId 
                    LEFT JOIN web.ProductGroups PG WITH (Nolock) ON PG.ProductGroupId = P.ProductGroupId
                    LEFT JOIN web.ProductCategories PC WITH (Nolock) ON PC.ProductCategoryId = P.ProductCategoryId 
                    LEFT JOIN web.ProductUids PU WITH (Nolock) ON PU.ProductUIDId = S.ProductUIDId   
                    WHERE PG.ProductTypeId =1 
                    AND H.SiteId=1 AND H.DivisionId=1
                    AND S.ProductUIDId IS NOT NULL AND PU.SaleOrderLineId IS NOT NULL  AND PC.ProductCategoryName <> 'OVER TUFT'
                    GROUP BY PU.SaleOrderLineId
                    UNION ALL 
                    SELECT PU.SaleOrderLineId, sum(isnull(S.Qty_Rec,0)- isnull(S.Qty_Iss,0)) AS Qty
                    FROM Web.StockHeaders H WITH (Nolock)
                    LEFT JOIN web.Stocks S WITH (Nolock) ON S.StockHeaderId = H.StockHeaderId 
                    LEFT JOIN web.Products P WITH (Nolock) ON P.ProductId = S.ProductId 
                    LEFT JOIN web.ProductGroups PG WITH (Nolock) ON PG.ProductGroupId = P.ProductGroupId
                    LEFT JOIN web.ProductCategories PC WITH (Nolock) ON PC.ProductCategoryId = P.ProductCategoryId 
                    LEFT JOIN web.ProductUids PU WITH (Nolock) ON PU.ProductUidName = S.LotNo  
                    WHERE PG.ProductTypeId =1 
                    AND H.SiteId=1 AND H.DivisionId=1
                    AND S.ProductUIDId IS NULL AND PU.SaleOrderLineId IS NOT NULL  AND PC.ProductCategoryName <> 'OVER TUFT'
                    GROUP BY PU.SaleOrderLineId  
                    UNION ALL 
                    SELECT PU.SaleOrderLineId, sum(isnull(S.Qty_Rec,0)- isnull(S.Qty_Iss,0)) AS Qty
                    FROM Web.StockHeaders H WITH (Nolock)
                    LEFT JOIN web.StockProcesses S WITH (Nolock) ON S.StockHeaderId = H.StockHeaderId 
                    LEFT JOIN web.Products P WITH (Nolock) ON P.ProductId = S.ProductId 
                    LEFT JOIN web.ProductGroups PG WITH (Nolock) ON PG.ProductGroupId = P.ProductGroupId
                    LEFT JOIN web.ProductCategories PC WITH (Nolock) ON PC.ProductCategoryId = P.ProductCategoryId 
                    LEFT JOIN web.ProductUids PU WITH (Nolock) ON PU.ProductUIDId = S.ProductUIDId --OR PU.ProductUidName = S.LotNo  
                    WHERE PG.ProductTypeId =1 
                    AND H.SiteId=1 AND H.DivisionId=1
                    AND S.ProductUIDId IS NOT NULL AND PU.SaleOrderLineId IS NOT NULL  AND PC.ProductCategoryName <> 'OVER TUFT'
                    GROUP BY PU.SaleOrderLineId
                    UNION ALL 
                    SELECT PU.SaleOrderLineId, sum(isnull(S.Qty_Rec,0)- isnull(S.Qty_Iss,0)) AS Qty
                    FROM Web.StockHeaders H WITH (Nolock)
                    LEFT JOIN web.StockProcesses S WITH (Nolock) ON S.StockHeaderId = H.StockHeaderId 
                    LEFT JOIN web.Products P WITH (Nolock) ON P.ProductId = S.ProductId 
                    LEFT JOIN web.ProductGroups PG WITH (Nolock) ON PG.ProductGroupId = P.ProductGroupId
                    LEFT JOIN web.ProductCategories PC WITH (Nolock) ON PC.ProductCategoryId = P.ProductCategoryId 
                    LEFT JOIN web.ProductUids PU WITH (Nolock) ON PU.ProductUidName = S.LotNo  
                    WHERE PG.ProductTypeId =1 
                    AND H.SiteId=1 AND H.DivisionId=1
                    AND S.ProductUIDId IS NULL AND PU.SaleOrderLineId IS NOT NULL  AND PC.ProductCategoryName <> 'OVER TUFT'
                    GROUP BY PU.SaleOrderLineId
                    UNION ALL 
                    SELECT PU.SaleOrderLineId, sum(isnull(S.Qty_Rec,0)- isnull(S.Qty_Iss,0)) AS Qty
                    FROM Web.StockHeaders H WITH (Nolock)
                    LEFT JOIN web.Stocks S WITH (Nolock) ON S.StockHeaderId = H.StockHeaderId 
                    LEFT JOIN web.Products P WITH (Nolock) ON P.ProductId = S.ProductId 
                    LEFT JOIN web.ProductGroups PG WITH (Nolock) ON PG.ProductGroupId = P.ProductGroupId
                    LEFT JOIN web.ProductCategories PC WITH (Nolock) ON PC.ProductCategoryId = P.ProductCategoryId 
                    LEFT JOIN web.ProductUids PU WITH (Nolock) ON PU.ProductUIDId = S.ProductUIDId --OR PU.ProductUidName = S.LotNo  
                    WHERE PG.ProductTypeId =1 
                    AND H.DocTypeId NOT IN (4024,448)
                    AND H.SiteId=1 AND H.DivisionId=1
                    AND S.ProductUIDId IS NOT NULL AND PU.SaleOrderLineId IS NOT NULL  AND PC.ProductCategoryName = 'OVER TUFT'
                    GROUP BY PU.SaleOrderLineId
                    UNION ALL 
                    SELECT PU.SaleOrderLineId, sum(isnull(S.Qty_Rec,0)- isnull(S.Qty_Iss,0)) AS Qty
                    FROM Web.StockHeaders H WITH (Nolock)
                    LEFT JOIN web.Stocks S WITH (Nolock) ON S.StockHeaderId = H.StockHeaderId 
                    LEFT JOIN web.Products P WITH (Nolock) ON P.ProductId = S.ProductId 
                    LEFT JOIN web.ProductGroups PG WITH (Nolock) ON PG.ProductGroupId = P.ProductGroupId
                    LEFT JOIN web.ProductCategories PC WITH (Nolock) ON PC.ProductCategoryId = P.ProductCategoryId 
                    LEFT JOIN web.ProductUids PU WITH (Nolock) ON PU.ProductUidName = S.LotNo  
                    WHERE PG.ProductTypeId =1 
                    AND H.SiteId=1 AND H.DivisionId=1
                    AND H.DocTypeId NOT IN (4024,448)
                    AND S.ProductUIDId IS NULL AND PU.SaleOrderLineId IS NOT NULL  AND PC.ProductCategoryName = 'OVER TUFT'
                    GROUP BY PU.SaleOrderLineId
                    UNION ALL 
                    SELECT PU.SaleOrderLineId, sum(isnull(S.Qty_Rec,0)- isnull(S.Qty_Iss,0)) AS Qty
                    FROM Web.StockHeaders H WITH (Nolock)
                    LEFT JOIN web.StockProcesses S WITH (Nolock) ON S.StockHeaderId = H.StockHeaderId 
                    LEFT JOIN web.Products P WITH (Nolock) ON P.ProductId = S.ProductId 
                    LEFT JOIN web.ProductGroups PG WITH (Nolock) ON PG.ProductGroupId = P.ProductGroupId
                    LEFT JOIN web.ProductCategories PC WITH (Nolock) ON PC.ProductCategoryId = P.ProductCategoryId 
                    LEFT JOIN web.ProductUids PU WITH (Nolock) ON PU.ProductUIDId = S.ProductUIDId --OR PU.ProductUidName = S.LotNo  
                    WHERE PG.ProductTypeId =1 
                    AND H.DocTypeId NOT IN (4024,448,5026)
                    AND H.SiteId=1 AND H.DivisionId=1
                    AND S.ProductUIDId IS NOT NULL AND PU.SaleOrderLineId IS NOT NULL  AND PC.ProductCategoryName = 'OVER TUFT'
                    GROUP BY PU.SaleOrderLineId
                    UNION ALL 
                    SELECT PU.SaleOrderLineId, sum(isnull(S.Qty_Rec,0)- isnull(S.Qty_Iss,0)) AS Qty
                    FROM Web.StockHeaders H WITH (Nolock)
                    LEFT JOIN web.StockProcesses S WITH (Nolock) ON S.StockHeaderId = H.StockHeaderId 
                    LEFT JOIN web.Products P WITH (Nolock) ON P.ProductId = S.ProductId 
                    LEFT JOIN web.ProductGroups PG WITH (Nolock) ON PG.ProductGroupId = P.ProductGroupId
                    LEFT JOIN web.ProductCategories PC WITH (Nolock) ON PC.ProductCategoryId = P.ProductCategoryId 
                    LEFT JOIN web.ProductUids PU WITH (Nolock) ON PU.ProductUidName = S.LotNo  
                    WHERE PG.ProductTypeId =1 
                    AND H.SiteId=1 AND H.DivisionId=1
                    AND H.DocTypeId NOT IN (4024,448,5026)
                    AND S.ProductUIDId IS NULL AND PU.SaleOrderLineId IS NOT NULL  AND PC.ProductCategoryName = 'OVER TUFT'
                    GROUP BY PU.SaleOrderLineId
                    UNION ALL 
                    SELECT A.SaleOrderLineId , A.StockPlanQty - isnull(B.PackQty,0) AS Qty
                    FROM
                    (
                    SELECT S.SaleOrderLineId, Sum(L.StockPlanQty) as  StockPlanQty
                    FROM Web.MaterialPlanHeaders H WITH (Nolock)
                    LEFT JOIN Web.MaterialPlanLines  L WITH (Nolock) ON H.MaterialPlanHeaderId = L.MaterialPlanHeaderId
                    LEFT JOIN web.MaterialPlanForSaleOrders S WITH (Nolock) ON S.MaterialPlanLineId = L.MaterialPlanLineId
                    WHERE H.DocTypeId = 313 AND S.SaleOrderLineId IS NOT NULL AND L.StockPlanQty > 0
                    Group By S.SaleOrderLineId
					) A
					LEFT JOIN 
					(
					SELECT P1.SaleOrderLineId,  sum(P1.Qty) AS PackQty
					FROM 
					(
                    SELECT L.SaleOrderLineId, sum(L.Qty) AS  Qty
                    FROM web.PackingHeaders H WITH (Nolock) 
                    LEFT JOIN web.PackingLines L WITH (Nolock) ON L.PackingHeaderId = H.PackingHeaderId 
                    LEFT JOIN web.ProductUids PU WITH (Nolock) ON PU.ProductUIDId =  L.ProductUidId 
                    WHERE PU.SaleOrderLineId IS  NULL AND L.SaleOrderLineId IS NOT NULL AND L.ProductUIDId IS NOT NULL 
                    GROUP BY L.SaleOrderLineId 
                    UNION ALL 
                    SELECT L.SaleOrderLineId, sum(L.Qty) AS  Qty
                    FROM web.PackingHeaders H WITH (Nolock) 
                    LEFT JOIN web.PackingLines L WITH (Nolock) ON L.PackingHeaderId = H.PackingHeaderId 
                    LEFT JOIN web.ProductUids PU WITH (Nolock) ON PU.ProductUidName = L.LotNo 
                    WHERE PU.SaleOrderLineId IS  NULL AND L.SaleOrderLineId IS NOT NULL  AND L.ProductUIDId IS NULL 
                    GROUP BY L.SaleOrderLineId 
                    ) P1
                    GROUP BY P1.SaleOrderLineId
                    ) B ON A.SaleOrderLineId = B.SaleOrderLineId
                    WHERE A.StockPlanQty - isnull(B.PackQty,0) > 0
                    ) VS
                    GROUP BY VS.SaleOrderLineId

*/


                    SELECT VS.SaleOrderLineId, sum(VS.Qty) AS StockQty  
                    INTO #FStock
                    FROM 
                    (                   
                    
                    SELECT SOL.SaleOrderLineId, sum(H.Qty) AS Qty 
                    FROM Web.ViewCarpetStock H WITH (Nolock)
					LEFT JOIN web.ProductUids PU WITH (Nolock) ON PU.ProductUIDId = H.ProductUIDId
					LEFT JOIN web.SaleOrderLines SOL WITH (Nolock) ON SOL.SaleOrderLineId = PU.SaleOrderLineId
					LEFT JOIN web.SaleOrderHeaders SOH WITH (Nolock) ON SOH.SaleOrderHeaderId = SOL.SaleOrderHeaderId
					WHERE 1=1 " +
                    (Buyer != null ? " And SOH.SaleToBuyerId IN (SELECT Items FROM[dbo].[Split](@Buyer, ','))" : "") + 
                    @" GROUP BY SOL.SaleOrderLineId
                    UNION ALL 
                    SELECT A.SaleOrderLineId , A.StockPlanQty - isnull(B.PackQty,0) AS Qty
                    FROM
                    (
                    SELECT S.SaleOrderLineId, Sum(L.StockPlanQty) as  StockPlanQty
                    FROM Web.MaterialPlanHeaders H WITH (Nolock)
                    LEFT JOIN Web.MaterialPlanLines  L WITH (Nolock) ON H.MaterialPlanHeaderId = L.MaterialPlanHeaderId
                    LEFT JOIN web.MaterialPlanForSaleOrders S WITH (Nolock) ON S.MaterialPlanLineId = L.MaterialPlanLineId
                    WHERE H.DocTypeId = 313 AND S.SaleOrderLineId IS NOT NULL AND L.StockPlanQty > 0
                    Group By S.SaleOrderLineId
					) A
					LEFT JOIN 
					(
					SELECT P1.SaleOrderLineId,  sum(P1.Qty) AS PackQty
					FROM 
					(
                    SELECT L.SaleOrderLineId, sum(L.Qty) AS  Qty
                    FROM web.PackingHeaders H WITH (Nolock) 
                    LEFT JOIN web.PackingLines L WITH (Nolock) ON L.PackingHeaderId = H.PackingHeaderId 
                    LEFT JOIN web.ProductUids PU WITH (Nolock) ON PU.ProductUIDId =  L.ProductUidId 
                    WHERE PU.SaleOrderLineId IS  NULL AND L.SaleOrderLineId IS NOT NULL AND L.ProductUIDId IS NOT NULL 
                    GROUP BY L.SaleOrderLineId 
                    UNION ALL 
                    SELECT L.SaleOrderLineId, sum(L.Qty) AS  Qty
                    FROM web.PackingHeaders H WITH (Nolock) 
                    LEFT JOIN web.PackingLines L WITH (Nolock) ON L.PackingHeaderId = H.PackingHeaderId 
                    LEFT JOIN web.ProductUids PU WITH (Nolock) ON PU.ProductUidName = L.LotNo 
                    WHERE PU.SaleOrderLineId IS  NULL AND L.SaleOrderLineId IS NOT NULL  AND L.ProductUIDId IS NULL 
                    GROUP BY L.SaleOrderLineId 
                    ) P1
                    GROUP BY P1.SaleOrderLineId
                    ) B ON A.SaleOrderLineId = B.SaleOrderLineId
                    WHERE A.StockPlanQty - isnull(B.PackQty,0) > 0
                    ) VS
                    GROUP BY VS.SaleOrderLineId

                    -----------------F Weaving Order balance in Branch-------------


                    SELECT POH.ProdOrderHeaderId, sum(isnull(H.OrderQty,0)) AS OrderQty,sum(isnull(H.BalanceQty,0)) AS BalanceQty,H.ProductId,
                    sum(isnull(H.BalanceQty,0)) * max(VRS.StandardSqYard) AS LoomAqyard INTO #FWob
                    FROM [Web].[FWeavingOrderStatus_OneProcess] (@StatusOnDate,NULL ,NULL,@FromDate,@StatusOnDate,@Process) H
                    LEFT JOIN web.JobOrderHeaders JOH WITH (Nolock) ON JOH.JobOrderHeaderId =H.JobOrderHeaderId
                    LEFT JOIN web.People P ON P.PersonID = JOH.JobWorkerId 
                    LEFT JOIN Web.ProdOrderLines POL WITH (Nolock) ON POL.ProdOrderLineId=H.ProdOrderLineId
                    LEFT JOIN web.JobOrderLines JOL WITH (Nolock) ON JOL.JobOrderLineId = POL.ReferenceDocLineId 
                    LEFT JOIN web.ProdOrderLines POL1 WITH (Nolock) ON POL1.ProdOrderLineId = JOL.ProdOrderLineId 
                    LEFT JOIN web.ProdOrderHeaders POH WITH (Nolock) ON  POH.ProdOrderHeaderId = POL1.ProdOrderHeaderId 
                    LEFT JOIN Web._ViewRugSize VRS WITH (Nolock) ON VRS.ProductId=H.ProductId
                    WHERE 1=1 AND isnull(P.IsSisterConcern,0)=0 " +
                    (Buyer != null ? " And POH.BuyerId IN (SELECT Items FROM [dbo].[Split] (@Buyer, ','))" : "") +
                    @" GROUP BY POH.ProdOrderHeaderId,H.ProductId

                    -----------------F Weaving Order balance in Main-------------

/*
                    SELECT POL.ProdOrderHeaderId, sum(isnull(H.OrderQty,0)) AS OrderQty,sum(isnull(H.BalanceQty,0)) AS BalanceQty,H.ProductId,
                    sum(isnull(H.BalanceQty,0)) * max(VRS.StandardSqYard) AS LoomAqyard INTO #FWobM
                    FROM [Web].[FWeavingOrderStatus_OneProcess] (@StatusOnDate,NULL ,NULL,@FromDate,@StatusOnDate,@Process) H
                    LEFT JOIN web.JobOrderHeaders JOH WITH (Nolock) ON JOH.JobOrderHeaderId =H.JobOrderHeaderId
                    LEFT JOIN web.People P WITH (Nolock) ON P.PersonID = JOH.JobWorkerId 
                    LEFT JOIN Web.ProdOrderLines POL WITH (Nolock) ON POL.ProdOrderLineId=H.ProdOrderLineId
                    LEFT JOIN Web._ViewRugSize VRS WITH (Nolock) ON VRS.ProductId=H.ProductId
                    LEFT JOIN web.ProdOrderHeaders POH WITH (Nolock) ON  POH.ProdOrderHeaderId = POL.ProdOrderHeaderId 
                    WHERE 1=1 AND isnull(P.IsSisterConcern,0)=0 " +
                    (Buyer != null ? " And POH.BuyerId IN (SELECT Items FROM [dbo].[Split] (@Buyer, ','))" : "") +
                    @" GROUP BY POL.ProdOrderHeaderId,H.ProductId

*/

					SELECT V.* 
					INTO #FWobM
					FROM
					(
                    SELECT POL.ProdOrderHeaderId, sum(isnull(H.OrderQty,0)) AS OrderQty,sum(isnull(H.BalanceQty,0)) AS BalanceQty,H.ProductId,
                    sum(isnull(H.BalanceQty,0)) * max(VRS.StandardSqYard) AS LoomAqyard 
                    FROM [Web].[FWeavingOrderStatus_OneProcess] (@StatusOnDate,NULL ,NULL,@FromDate,@StatusOnDate,@Process) H
                    LEFT JOIN web.JobOrderHeaders JOH WITH (Nolock) ON JOH.JobOrderHeaderId =H.JobOrderHeaderId
                    LEFT JOIN web.People JW ON JW.PersonID = JOH.JobWorkerId 
                    LEFT JOIN Web.ProdOrderLines POL WITH (Nolock) ON POL.ProdOrderLineId=H.ProdOrderLineId
                    LEFT JOIN Web._ViewRugSize VRS WITH (Nolock) ON VRS.ProductId=H.ProductId
                    LEFT JOIN web.ProdOrderHeaders POH WITH (Nolock) ON  POH.ProdOrderHeaderId = POL.ProdOrderHeaderId 
                    LEFT JOIN web.Products P WITH (Nolock) ON P.ProductId = H.ProductId
                    LEFT JOIN web.ProductCategories  PC WITH (Nolock) ON PC.ProductCategoryId = P.ProductCategoryId 
                    WHERE 1=1 AND isnull(JW.IsSisterConcern,0)=0 AND PC.ProductCategoryName <> 'OVER TUFT' " +
                    (Buyer != null ? " And POH.BuyerId IN (SELECT Items FROM [dbo].[Split] (@Buyer, ','))" : "") +
                    @" GROUP BY POL.ProdOrderHeaderId,H.ProductId
					UNION ALL
                    SELECT POL.ProdOrderHeaderId, sum(isnull(L.Qty,0)) AS OrderQty,sum(isnull(L.Qty,0)-isnull(V.RecQty,0)) AS BalanceQty,L.ProductId,
                    sum(isnull(L.Qty,0)-isnull(V.RecQty,0)) * max(VRS.StandardSqYard) AS LoomAqyard 
                    FROM web.JobOrderHeaders H WITH (Nolock)
                    LEFT JOIN web.People JW WITH (Nolock) ON JW.PersonID = H.JobWorkerId 
                    LEFT JOIN web.JobOrderLines L ON L.JobOrderHeaderId = H.JobOrderHeaderId 
                    LEFT JOIN Web.ProdOrderLines POL WITH (Nolock) ON POL.ProdOrderLineId=L.ProdOrderLineId
                    LEFT JOIN web.ProdOrderHeaders POH WITH (Nolock) ON  POH.ProdOrderHeaderId = POL.ProdOrderHeaderId 
                    LEFT JOIN Web._ViewRugSize VRS WITH (Nolock) ON VRS.ProductId=L.ProductId
                    LEFT JOIN web.Products P WITH (Nolock) ON P.ProductId = L.ProductId
                    LEFT JOIN web.ProductCategories  PC WITH (Nolock) ON PC.ProductCategoryId = P.ProductCategoryId 
                    LEFT JOIN
                    (                    
                    SELECT V1.JobOrderLineId, sum(L.Qty) AS RecQty  
					FROM web.JobReceiveHeaders H WITH (NoLock)
					LEFT JOIN web.JobReceiveLines L WITH (NoLock) ON L.JobReceiveHeaderId = H.JobReceiveHeaderId
					LEFT JOIN 
					(
					SELECT PU.ProductUidId, JOL.JobOrderLineId, JOL.ProdOrderLineId, JOL.ProductId 
					FROM web.JobReceiveHeaders H WITH (NoLock)
					LEFT JOIN web.JobReceiveLines L WITH (NoLock) ON L.JobReceiveHeaderId = H.JobReceiveHeaderId
					LEFT JOIN web.JobOrderLines JOL WITH (NoLock) ON JOL.JobOrderLineId = L.JobOrderLineId
					LEFT JOIN web.ProductUids PU ON PU.ProductUIDId = L.ProductUidId OR PU.LotNo = L.LotNo 
					WHERE H.SiteId =1 AND H.DivisionId =1 AND H.DocTypeId =448
					AND PU.ProductUidId IS NOT NULL 
					) V1 ON V1.ProductUidId = L.ProductUidId
					WHERE H.SiteId =1 AND H.DivisionId =1 AND H.ProcessId =2008  AND V1.JobOrderLineId IS NOT NULL 
                    GROUP BY V1.JobOrderLineId

                    ) V ON V.JobOrderLineId = L.JobOrderLineId
                WHERE H.SiteId =1 AND H.DivisionId =1 AND H.ProcessId =43 " +
                    (Buyer != null ? " And POH.BuyerId IN (SELECT Items FROM[dbo].[Split](@Buyer, ','))" : "") +
                    @"   AND isnull(JW.IsSisterConcern,0)=0  AND POL.ProdOrderHeaderId IS NOT NULL AND PC.ProductCategoryName = 'OVER TUFT'
                GROUP BY POL.ProdOrderHeaderId,L.ProductId                   
                    ) V
                    -----------------F Dispatched -------------


                    SELECT PL.SaleOrderLineId, sum(PL.Qty) AS DispQty, sum(CASE WHEN (isnull(PU.SaleOrderLineId,0) <> isnull(PL.SaleOrderLineId,0) AND PU.ProductUIDId IS NOT NULL) THEN  PL.Qty ELSE 0 END  ) AS O_X, sum(isnull(PL.Qty,0)) * max(VRS.StandardSqYard)  AS DispYard 
                    INTO #FDisp
                    FROM web.SaleDispatchHeaders H WITH (Nolock)
                    LEFT JOIN web.SaleDispatchLines L WITH (Nolock) ON L.SaleDispatchHeaderId = H.SaleDispatchHeaderId 
                    LEFT JOIN web.PackingLines PL WITH (Nolock) ON PL.PackingLineId = L.PackingLineId 
                    LEFT JOIN web.ProductUids PU WITH (Nolock) ON PU.ProductUIDId = PL.ProductUidId OR PU.ProductUidName = PL.LotNo
                    LEFT JOIN Web._ViewRugSize VRS WITH (Nolock) ON PL.ProductId=VRS.ProductId
                    WHERE 1 = 1 
                    And H.SiteId =@Site 
                    AND H.DivisionId = @Division " +
                    (Buyer != null ? " And H.SaleToBuyerId IN (SELECT Items FROM [dbo].[Split] (@Buyer, ','))" : "") +
                    @" GROUP BY PL.SaleOrderLineId 


                    -----------------F Dispatched For Other -------------

                    SELECT PU.SaleOrderLineId, sum(CASE WHEN isnull(SOH.SaleToBuyerId,0) = isnull(SOH1.SaleToBuyerId,0) THEN PL.Qty ELSE 0 END ) AS D_O,sum(CASE WHEN isnull(SOH.SaleToBuyerId,0) <> isnull(SOH1.SaleToBuyerId,0) THEN PL.Qty ELSE 0 END ) AS O_B
                    INTO #FDispO
                    FROM web.SaleDispatchHeaders H WITH (Nolock)
                    LEFT JOIN web.SaleDispatchLines L WITH (Nolock) ON L.SaleDispatchHeaderId = H.SaleDispatchHeaderId 
                    LEFT JOIN web.PackingLines PL WITH (Nolock) ON PL.PackingLineId = L.PackingLineId 
                    LEFT JOIN web.SaleOrderLines SOL ON SOL.SaleOrderLineId = PL.SaleOrderLineId 
                    LEFT JOIN web.SaleOrderHeaders SOH ON SOH.SaleOrderHeaderId = SOL.SaleOrderHeaderId 
                    LEFT JOIN web.ProductUids PU WITH (Nolock) ON PU.ProductUIDId = PL.ProductUidId 
                    LEFT JOIN web.SaleOrderLines SOL1 ON SOL1.SaleOrderLineId = PU.SaleOrderLineId 
                    LEFT JOIN web.SaleOrderHeaders SOH1 ON SOH1.SaleOrderHeaderId = SOL1.SaleOrderHeaderId 
                    LEFT JOIN Web._ViewRugSize VRS WITH (Nolock) ON PL.ProductId=VRS.ProductId
                    WHERE H.SiteId =@Site AND H.DivisionId = @Division " +
                    (Buyer != null ? " And SOH1.SaleToBuyerId IN (SELECT Items FROM [dbo].[Split] (@Buyer, ','))" : "") +
                    @" AND isnull(PU.SaleOrderLineId,0) <> isnull(PL.SaleOrderLineId,0) 
                    AND PU.SaleOrderLineId IS NOT NULL 
                    GROUP BY PU.SaleOrderLineId 


                    ----------- Main Query-------------

                    SELECT Max(H.SaleOrderLineId) AS SaleOrderLineId,P.ProductId, Max(PO.ProdOrderHeaderId) ProdOrderHeaderId,  Max(H.Buyer) Buyer,max(H.DocDate) AS Order_Date, max(H.DocNo) AS OrderNo, max(H.DueDate) AS DueDate,	 DateDiff(Day,max(H.DueDate),GetDate()) AS Days,
                    Max(P.ProductName) ProductName, Max(PQ.ProductQualityName) AS Quality,Max(PG.ProductGroupName) AS Design, Max(VRS.ManufaturingSizeName) AS Size, Max(C.ColourName) AS Colour,
                    Max(SELS.BuyerSpecification) AS BuyerSpecification,Max(SELS.BuyerSpecification1) AS BuyerSpecification1,Max(SELS.BuyerSpecification2) AS BuyerSpecification2,Max(SELS.BuyerSpecification3) AS BuyerSpecification3, null AS BuyerSpecification4,
                    isnull(sum(H.OrdQty),0) AS Order_Qty,isnull(sum(H.CancelQty),0) AS Cancel_Qty,isnull(sum(H.OrdQty),0) - isnull(sum(H.CancelQty),0) AS Net_Qty, isnull(sum(H.DispQty),0) AS Ship_Qty,isnull(sum(H.BalQty),0) AS Bal_Qty,
                    isnull(sum(POB.BalanceQty),0) AS To_Be_Issue, isnull(sum(OrderIssue.OrderQty),0) AS Slip_Qty, 
                    isnull(sum(FWob.BalanceQty),0) + isnull(sum(FWobM.BalanceQty),0) AS Loom_Qty, 
                    --case When isnull(sum(FWR.Qty),0)+isnull(sum(VM.StockPlanQty),0)- isnull(sum(Disp.DispQty),0) <=0 then 0 else isnull(sum(FWR.Qty),0)+isnull(sum(VM.StockPlanQty),0)- isnull(sum(Disp.DispQty),0) end  AS Stock_Qty,
                    case When isnull(sum(FStock.StockQty),0) <=0 then 0 else isnull(sum(FStock.StockQty),0) end  AS Stock_Qty,
                    isnull(sum(H.OrdQty),0)-isnull(sum(H.CancelQty),0)-isnull(sum(PO.Qty),0) AS PendingToPlanQty,isnull(sum(POB.BalanceQty),0) AS ToBeIssue,
                    isnull(sum(OrderIssueB.BalanceQty),0) AS To_Be_IssueInBranch, 
                    isnull(sum(FWob.OrderQty),0) AS OrderIssueinBranch,
                    isnull(sum(DispO.D_O),0) AS D_O,isnull(sum(DispO.O_B),0) AS O_B, isnull(sum(Disp.O_X),0) AS O_X,
                    max(H.DivisionId) AS DivisionId,max(H.SiteId) AS SiteId, NULL AS ReportName, NULL AS SubReportProcList, 'Sale Order Tracking' AS ReportTitle
                    INTO TempGrid_SaleOrderTracking
                    FROM #FSaleOrder H
                    LEFT JOIN Web.Products P WITH (Nolock) ON P.ProductId=H.ProductId
                    LEFT JOIN Web.ProductGroups PG WITH (Nolock) ON PG.ProductGroupId=P.ProductGroupId
                    LEFT JOIN web.FinishedProduct FP WITH (Nolock) ON FP.ProductId = P.ProductId 
                    LEFT JOIN web.Colours C WITH (Nolock) ON C.ColourId = FP.ColourId 
                    LEFT JOIN web.ProductQualities PQ WITH (Nolock) ON PQ.ProductQualityId = FP.ProductQualityId 
                    LEFT JOIN web.ViewRugSize VRS ON VRS.ProductId = P.ProductId 
                    LEFT JOIN 
                    (
                        SELECT S.SaleOrderLineId, L.StockPlanQty 
                        FROM Web.MaterialPlanHeaders H WITH (Nolock)
                        LEFT JOIN Web.MaterialPlanLines  L WITH (Nolock) ON H.MaterialPlanHeaderId = L.MaterialPlanHeaderId
                        LEFT JOIN web.MaterialPlanForSaleOrders S WITH (Nolock) ON S.MaterialPlanLineId = L.MaterialPlanLineId
                        WHERE H.DocTypeId = 313 AND S.SaleOrderLineId IS NOT NULL AND L.StockPlanQty > 0
                    ) VM on VM.SaleOrderLineId =H.SaleOrderLineId
                    LEFT JOIN #FProdOrder PO WITH (Nolock) ON PO.SaleOrderLineId=H.SaleOrderLineId
                    LEFT JOIN #FProdOrderBalance POB ON  POB.ProdOrderHeaderId=PO.ProdOrderHeaderId AND H.ProductId=POB.ProductId
                    LEFT JOIN #OrderIssue OrderIssue ON OrderIssue.ProdOrderHeaderId=PO.ProdOrderHeaderId AND H.ProductId=OrderIssue.ProductId
                    LEFT JOIN #FProdOrderBalanceinBranch OrderIssueB ON OrderIssueB.ProdOrderHeaderId=PO.ProdOrderHeaderId AND H.ProductId=OrderIssueB.ProductId
                    --LEFT JOIN #FWR  FWR ON FWR.ProdOrderHeaderId=PO.ProdOrderHeaderId AND H.ProductId=FWR.ProductId
                    LEFT JOIN #FStock  FStock ON FStock.SaleOrderLineId=H.SaleOrderLineId 
                    LEFT JOIN #FWob FWob ON FWob.ProdOrderHeaderId=PO.ProdOrderHeaderId AND H.ProductId=FWob.ProductId
                    LEFT JOIN #FWobM FWobM ON FWobM.ProdOrderHeaderId=PO.ProdOrderHeaderId AND H.ProductId=FWobM.ProductId
                    LEFT JOIN #FDisp Disp WITH (Nolock) ON Disp.SaleOrderLineId=H.SaleOrderLineId
                    LEFT JOIN #FDispO DispO WITH (Nolock) ON DispO.SaleOrderLineId=H.SaleOrderLineId
                    --LEFT JOIN web.ProductBuyers PB ON PB.ProductId = H.ProductId AND PB.BuyerId = H.SaleToBuyerId 
                    LEFT JOIN web.SaleOrderLines L ON L.SaleOrderLineId = H.SaleOrderLineId
                    LEFT JOIN web.SaleEnquiryLines SEL ON SEL.SaleEnquiryLineId = L.ReferenceDocLineId 
					LEFT JOIN web.SaleEnquiryLineExtendeds SELS ON SELS.SaleEnquiryLineId = SEL.SaleEnquiryLineId  
                    WHERE 1=1 " + 
                    (ProductSize != null ? " And VRS.ManufaturingSizeID IN (SELECT Items FROM  Web.[Split] (@ProductSize, ','))" : "") +
                    (BuyerDesign != null ? " And SELS.BuyerSpecification IN (SELECT Items FROM [dbo].[Split] (@BuyerDesign, ','))" : "") +
                    @" GROUP BY H.SaleOrderHeaderId,P.ProductId 
                    HAVING 1=1
                    AND ( @ReportFor is null or @ReportFor ='All' OR ( @ReportFor ='Pending' AND isnull(sum(H.BalQty),0) > 0 ) OR ( @ReportFor ='Delay PO' AND isnull(sum(H.BalQty),0) > 0 AND max(H.DueDate) < @StatusOnDate ) ) ";



            if (ReportType == "Detail")
            {
                mQry += @" SELECT H.SaleOrderLineId, 
                            H.Buyer, H.OrderNo AS Order_No, Replace( Convert(NVARCHAR,H.Order_Date,106),' ','/') AS Order_Date , Replace(Convert(NVARCHAR,H.DueDate,106),' ','/') AS Delivery_Date,
                            '`'+H.BuyerSpecification3 Quality, H.BuyerSpecification AS Design, H.BuyerSpecification1 Size, H.BuyerSpecification2 Colour, 
                            H.ProductName, H.Order_Qty AS ORD, H.Cancel_Qty AS O_C,
                            H.Slip_Qty AS SLP, H.To_Be_IssueInBranch AS UX, H.Loom_Qty AS LOOM, H.Stock_Qty AS STK, H.Ship_Qty AS SHP, H.Bal_Qty AS BAL,H.D_O, H.O_B AS D_B, H.O_X 
                            FROM TempGrid_SaleOrderTracking H WITH (Nolock)
                            ORDER BY H.Order_Date, H.OrderNo,H.SaleOrderLineId ";
            }
            else if (ReportType == "Summary")
            {
                mQry += @" SELECT H.ProductId, '`'+H.BuyerSpecification3 Quality, H.BuyerSpecification AS Design, H.BuyerSpecification1 Size, H.BuyerSpecification2 Colour, 
                    Max(H.ProductName) AS ProductName, sum(H.Order_Qty) AS ORD, sum(H.Cancel_Qty) AS O_C,
                    sum(H.Slip_Qty) AS SLP, SUM(H.To_Be_IssueInBranch) AS UX, sum(H.Loom_Qty) AS LOOM, sum(H.Stock_Qty) AS STK, sum(H.Ship_Qty) AS SHP, sum(H.Bal_Qty) AS BAL, sum(H.D_O) AS D_O, sum(H.O_B) AS D_B, sum(H.O_X) AS O_X 
                    FROM TempGrid_SaleOrderTracking H WITH (Nolock)
                    GROUP BY H.BuyerSpecification3,H.BuyerSpecification,H.BuyerSpecification1,H.BuyerSpecification2, H.ProductId ";
            }

            IEnumerable<SaleOrderInventoryStatusViewModel> SaleOrderInventoryStatusList = db.Database.SqlQuery<SaleOrderInventoryStatusViewModel>(mQry, SqlParameterStatusOnDate, SqlParameterDocTypeId, SqlParameterSite, SqlParameterDivision, SqlParameterFromDate, SqlParameterToDate, SqlParameterBuyer, SqlParameterSaleOrderHeaderId, SqlParameterProduct, SqlParameterProductCategory, SqlParameterProductQuality, SqlParameterProductGroup, SqlParameterProductSize, SqlParameterReportFor, SqlParameterBuyerDesign).ToList();


            return SaleOrderInventoryStatusList;

        }

        public IEnumerable<SaleOrderInventoryStatus_StockViewModel> StockDetail(SaleOrderInventoryStatusDisplayFilterSettings Settings)
        {
            var StatusOnDateSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "StatusOnDate" select H).FirstOrDefault();
            var DocTypeIdSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "DocTypeId" select H).FirstOrDefault();
            var SiteSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "Site" select H).FirstOrDefault();
            var DivisionSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "Division" select H).FirstOrDefault();
            var FromDateSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "FromDate" select H).FirstOrDefault();
            var ToDateSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ToDate" select H).FirstOrDefault();
            var BuyerSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "Buyer" select H).FirstOrDefault();
            var SaleOrderHeaderIdSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "SaleOrderHeaderId" select H).FirstOrDefault();
            var ProductSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "Product" select H).FirstOrDefault();
            var ProductCategorySetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ProductCategory" select H).FirstOrDefault();
            var ProductQualitySetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ProductQuality" select H).FirstOrDefault();
            var ProductGroupSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ProductGroup" select H).FirstOrDefault();
            var ProductSizeSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ProductSize" select H).FirstOrDefault();
            var ReportTypeSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ReportType" select H).FirstOrDefault();
            var ReportForSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ReportFor" select H).FirstOrDefault();
            var NextFormatSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "NextFormat" select H).FirstOrDefault();
            var BuyerDesignSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "BuyerDesign" select H).FirstOrDefault();
            var TextHiddenSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "TextHidden" select H).FirstOrDefault();



            string StatusOnDate = StatusOnDateSetting.Value;
            string DocTypeId = DocTypeIdSetting.Value;
            //string Site = SiteSetting.Value;
            //string Division = DivisionSetting.Value;
            string Site = "1";
            string Division = "1";
            string FromDate = FromDateSetting.Value;
            string ToDate = ToDateSetting.Value;
            string Buyer = BuyerSetting.Value;
            string SaleOrderHeaderId = SaleOrderHeaderIdSetting.Value;
            string Product = ProductSetting.Value;
            string ProductCategory = ProductCategorySetting.Value;
            string ProductQuality = ProductQualitySetting.Value;
            string ProductGroup = ProductGroupSetting.Value;
            string ProductSize = ProductSizeSetting.Value;
            string ReportType = ReportTypeSetting.Value;
            string ReportFor = ReportForSetting.Value;
            string NextFormat = NextFormatSetting.Value;
            string BuyerDesign = BuyerDesignSetting.Value;
            string TextHidden = TextHiddenSetting.Value;


            string mQry;

            if (Buyer == "-1")
                Buyer = null;

            SqlParameter SqlParameterStatusOnDate = new SqlParameter("@StatusOnDate", !string.IsNullOrEmpty(StatusOnDate) ? StatusOnDate : (object)DBNull.Value);
            SqlParameter SqlParameterDocTypeId = new SqlParameter("@DocTypeId", !string.IsNullOrEmpty(DocTypeId) ? DocTypeId : (object)DBNull.Value);
            SqlParameter SqlParameterSite = new SqlParameter("@Site", !string.IsNullOrEmpty(Site) ? Site : (object)DBNull.Value);
            SqlParameter SqlParameterDivision = new SqlParameter("@Division", !string.IsNullOrEmpty(Division) ? Division : (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", !string.IsNullOrEmpty(FromDate) ? FromDate : (object)DBNull.Value);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", !string.IsNullOrEmpty(ToDate) ? ToDate : (object)DBNull.Value);
            SqlParameter SqlParameterBuyer = new SqlParameter("@Buyer", !string.IsNullOrEmpty(Buyer) ? Buyer : (object)DBNull.Value);
            SqlParameter SqlParameterSaleOrderHeaderId = new SqlParameter("@SaleOrderHeaderId", !string.IsNullOrEmpty(SaleOrderHeaderId) ? SaleOrderHeaderId : (object)DBNull.Value);
            SqlParameter SqlParameterProduct = new SqlParameter("@Product", !string.IsNullOrEmpty(Product) ? Product : (object)DBNull.Value);
            SqlParameter SqlParameterProductCategory = new SqlParameter("@ProductCategory", !string.IsNullOrEmpty(ProductCategory) ? ProductCategory : (object)DBNull.Value);
            SqlParameter SqlParameterProductQuality = new SqlParameter("@ProductQuality", !string.IsNullOrEmpty(ProductQuality) ? ProductQuality : (object)DBNull.Value);
            SqlParameter SqlParameterProductGroup = new SqlParameter("@ProductGroup", !string.IsNullOrEmpty(ProductGroup) ? ProductGroup : (object)DBNull.Value);
            SqlParameter SqlParameterProductSize = new SqlParameter("@ProductSize", !string.IsNullOrEmpty(ProductSize) ? ProductSize : (object)DBNull.Value);
            SqlParameter SqlParameterReportType = new SqlParameter("@ReportType", !string.IsNullOrEmpty(ReportType) ? ReportType : (object)DBNull.Value);
            SqlParameter SqlParameterReportFor = new SqlParameter("@ReportFor", !string.IsNullOrEmpty(ReportFor) ? ReportFor : (object)DBNull.Value);
            SqlParameter SqlParameterNextFormat = new SqlParameter("@NextFormat", !string.IsNullOrEmpty(NextFormat) ? NextFormat : (object)DBNull.Value);
            SqlParameter SqlParameterBuyerDesign = new SqlParameter("@BuyerDesign", !string.IsNullOrEmpty(BuyerDesign) ? BuyerDesign : (object)DBNull.Value);
            SqlParameter SqlParameterTextHidden = new SqlParameter("@TextHidden", !string.IsNullOrEmpty(TextHidden) ? TextHidden : (object)DBNull.Value);

     //       mQry = @"SELECT PU.ProductUidName AS CarpetNo, Replace(Convert(NVARCHAR,H.DocDate,106),' ','/') AS Date,PC.ProductCategoryName AS Type, PQ.ProductQualityName AS Quality, PG.ProductGroupName AS Design, C.ColourName AS Colour, VRS.ManufaturingSizeName AS Size,  H.Qty - isnull(VINV.Qty,0) AS Qty, A.OTO AS OTI, A.OTR AS OTR,
     //               CASE WHEN isnull(PU.CurrenctProcessId,0) =46 THEN 'Rejected' WHEN isnull(PU.CurrenctGodownId,0) =1 AND charindex(',',PU.ProcessesDone) > 0 THEN 'Finished' ELSE 'Un-Finished' END  Status  
     //               FROM Web.FJobReceive_OneProcess (@StatusOnDate,@Site,@Division,@FromDate,@ToDate,43) H
     //               LEFT JOIN web.ProductUids PU WITH (Nolock) ON PU.ProductUIDId = H.ProductUIDId OR PU.ProductUidName = H.LotNo
     //               LEFT JOIN web.Products P WITH (Nolock) ON P.ProductId = H.ProductId 
     //               LEFT JOIN web.ProductGroups PG WITH (Nolock) ON PG.ProductGroupId = P.ProductGroupId
     //               LEFT JOIN web.FinishedProduct FP WITH (Nolock) ON FP.ProductId = P.ProductId 
     //               LEFT JOIN web.Colours C WITH (Nolock) ON C.ColourId = FP.ColourId 
     //               LEFT JOIN web.ViewRugSize VRS WITH (Nolock) ON VRS.ProductId = P.ProductId 
     //               LEFT JOIN web.ProductCategories PC WITH (Nolock) ON PC.ProductCategoryId = P.ProductCategoryId 
     //               LEFT JOIN web.ProductQualities PQ WITH (Nolock) ON PQ.ProductQualityId = FP.ProductQualityId  
     //               LEFT JOIN web.SaleOrderLines SOL WITH (Nolock) ON SOL.SaleOrderLineId = PU.SaleOrderLineId
     //               LEFT JOIN web.SaleOrderHeaders SOH WITH (Nolock) ON SOH.SaleOrderHeaderId = SOL.SaleOrderHeaderId
     //               LEFT JOIN 
     //               (
					//SELECT JOL.ProductUIDId, Max(JOL.Qty) AS OTO, Max(RL.RecQty) AS OTR 
					//FROM WEb.JobOrderHeaders  JOH WITH (Nolock)
					//LEFT JOIN web.JobOrderLines JOL WITH (Nolock) ON JOL.JobOrderHeaderId = JOH.JobOrderHeaderId 
					//LEFT JOIN
					//(
					//SELECT RL.JobOrderLineId, sum(RL.Qty) AS RecQty  FROM web.JobReceiveLines RL GROUP BY RL.JobOrderLineId 
					//) RL ON RL.JobOrderLineId = JOL.JobOrderLineId
					//WHERE JOH.DocTypeId  =4024 AND JOL.ProductUIDId IS NOT NULL 
					//GROUP BY JOL.ProductUIDId
					//) A  ON A.ProductUIDId = PU.ProductUIDId
					//LEFT JOIN 
					//(
					//SELECT PU.ProductUIDId, sum(L.Qty) AS Qty   
					//FROM web.SaleInvoiceHeaders H WITH (Nolock)
					//LEFT JOIN web.SaleInvoiceLines L WITH (Nolock) ON L.SaleInvoiceHeaderId = H.SaleInvoiceHeaderId
					//LEFT JOIN web.SaleDispatchLines SDL WITH (Nolock) ON SDL.SaleDispatchLineId = L.SaleDispatchLineId 
					//LEFT JOIN web.PackingLines PL WITH (Nolock) ON PL.PackingLineId = SDL.PackingLineId
					//LEFT JOIN web.ProductUids PU WITH (Nolock) ON PU.ProductUIDId = PL.ProductUidId  OR PU.ProductUidName = PL.LotNo
					//WHERE PU.ProductUIDId IS NOT NULL 
					//GROUP BY PU.ProductUIDId
					//) AS VINV ON  PU.ProductUidId = VINV.ProductUidId
     //               WHERE 1=1 
     //               AND ( PU.Status <> 'Dispatch' OR H.Qty - isnull(VINV.Qty,0) >0) " +
     //               (DocTypeId != null ? " AND SOH.DocTypeId IN (SELECT Items FROM [dbo].[Split] (@DocTypeId, ','))" : "") +
     //               (Product != null ? " AND P.ProductId IN (SELECT Items FROM [dbo].[Split] (@Product, ','))" : "") +
     //               (ProductQuality != null ? " AND Fp.ProductQualityId IN (SELECT Items FROM  Web.[Split] (@ProductQuality, ','))" : "") +
     //               (ProductCategory != null ? " AND P.ProductCategoryId  IN (SELECT Items FROM  Web.[Split] (@ProductCategory, ','))" : "") +
     //               (ProductGroup != null ? " AND P.ProductGroupId IN (SELECT Items FROM  Web.[Split] (@ProductGroup, ','))" : "") +
     //               (SaleOrderHeaderId != null ? " AND SOH.SaleOrderHeaderId IN (SELECT Items FROM [dbo].[Split] (@SaleOrderHeaderId, ','))" : "") +
     //               (Buyer != null ? " AND SOH.SaleToBuyerId  IN (SELECT Items FROM [dbo].[Split] (@Buyer, ','))" : "") ;


            string mQry1;
            if (ReportTypeSetting.Value == "Summary")
                mQry1 = " AND PU.ProductId = @TextHidden ";
            else
                mQry1 =  " AND PU.SaleOrderLineId = @TextHidden ";


            //       mQry = @"
            //               SELECT PU.ProductUidName AS CarpetNo, Replace(Convert(NVARCHAR, PU.gendocDate, 106), ' ', '/') AS Date, PC.ProductCategoryName AS Type, PQ.ProductQualityName AS Quality, PG.ProductGroupName AS Design, C.ColourName AS Colour, VRS.ManufaturingSizeName AS Size,  VM.StockQty AS Qty, A.OTO AS OTI, A.OTR AS OTR,
            //               CASE WHEN isnull(PU.CurrenctProcessId, 0) = 46 THEN 'Rejected' WHEN isnull(PU.CurrenctGodownId, 0) = 1 AND charindex(',', PU.ProcessesDone) > 0 THEN 'Finished' ELSE 'Un-Finished' END Status
            //               FROM
            //               (
            //               SELECT VS.ProductUIDId, sum(VS.Qty) AS StockQty
            //               FROM
            //               (
            //               SELECT PU.ProductUIDId, sum(isnull(S.Qty_Rec, 0) - isnull(S.Qty_Iss, 0)) AS Qty
            //               FROM Web.StockHeaders H WITH(Nolock)
            //               LEFT JOIN web.Stocks S WITH(Nolock) ON S.StockHeaderId = H.StockHeaderId
            //               LEFT JOIN web.Products P WITH(Nolock) ON P.ProductId = S.ProductId
            //               LEFT JOIN web.ProductGroups PG WITH(Nolock) ON PG.ProductGroupId = P.ProductGroupId
            //               LEFT JOIN web.ProductCategories PC WITH(Nolock) ON PC.ProductCategoryId = P.ProductCategoryId
            //               LEFT JOIN web.ProductUids PU WITH(Nolock) ON PU.ProductUIDId = S.ProductUIDId
            //               WHERE PG.ProductTypeId = 1 
            //               AND H.SiteId = 1 AND H.DivisionId = 1
            //               " + mQry1 + @"
            //               AND S.ProductUIDId IS NOT NULL AND PU.SaleOrderLineId IS NOT NULL  AND PC.ProductCategoryName <> 'OVER TUFT'
            //               GROUP BY PU.ProductUIDId
            //               UNION ALL
            //               SELECT PU.ProductUIDId, sum(isnull(S.Qty_Rec, 0) - isnull(S.Qty_Iss, 0)) AS Qty
            //               FROM Web.StockHeaders H WITH(Nolock)
            //               LEFT JOIN web.Stocks S WITH(Nolock) ON S.StockHeaderId = H.StockHeaderId
            //               LEFT JOIN web.Products P WITH(Nolock) ON P.ProductId = S.ProductId
            //               LEFT JOIN web.ProductGroups PG WITH(Nolock) ON PG.ProductGroupId = P.ProductGroupId
            //               LEFT JOIN web.ProductCategories PC WITH(Nolock) ON PC.ProductCategoryId = P.ProductCategoryId
            //               LEFT JOIN web.ProductUids PU WITH(Nolock) ON PU.ProductUidName = S.LotNo
            //               WHERE PG.ProductTypeId = 1 
            //               AND H.SiteId = 1 AND H.DivisionId = 1
            //               " + mQry1 + @"
            //               AND S.ProductUIDId IS NULL AND PU.SaleOrderLineId IS NOT NULL  AND PC.ProductCategoryName <> 'OVER TUFT'
            //               GROUP BY PU.ProductUIDId
            //               UNION ALL
            //               SELECT PU.ProductUIDId, sum(isnull(S.Qty_Rec, 0) - isnull(S.Qty_Iss, 0)) AS Qty
            //               FROM Web.StockHeaders H WITH(Nolock)
            //               LEFT JOIN web.StockProcesses S WITH(Nolock) ON S.StockHeaderId = H.StockHeaderId
            //               LEFT JOIN web.Products P WITH(Nolock) ON P.ProductId = S.ProductId
            //               LEFT JOIN web.ProductGroups PG WITH(Nolock) ON PG.ProductGroupId = P.ProductGroupId
            //               LEFT JOIN web.ProductCategories PC WITH(Nolock) ON PC.ProductCategoryId = P.ProductCategoryId
            //               LEFT JOIN web.ProductUids PU WITH(Nolock) ON PU.ProductUIDId = S.ProductUIDId--OR PU.ProductUidName = S.LotNo
            //               WHERE PG.ProductTypeId = 1 
            //               AND H.SiteId = 1 AND H.DivisionId = 1
            //               " + mQry1 + @"
            //               AND S.ProductUIDId IS NOT NULL AND PU.SaleOrderLineId IS NOT NULL  AND PC.ProductCategoryName <> 'OVER TUFT'
            //               GROUP BY PU.ProductUIDId
            //               UNION ALL
            //               SELECT PU.ProductUIDId, sum(isnull(S.Qty_Rec, 0) - isnull(S.Qty_Iss, 0)) AS Qty
            //               FROM Web.StockHeaders H WITH(Nolock)
            //               LEFT JOIN web.StockProcesses S WITH(Nolock) ON S.StockHeaderId = H.StockHeaderId
            //               LEFT JOIN web.Products P WITH(Nolock) ON P.ProductId = S.ProductId
            //               LEFT JOIN web.ProductGroups PG WITH(Nolock) ON PG.ProductGroupId = P.ProductGroupId
            //               LEFT JOIN web.ProductCategories PC WITH(Nolock) ON PC.ProductCategoryId = P.ProductCategoryId
            //               LEFT JOIN web.ProductUids PU WITH(Nolock) ON PU.ProductUidName = S.LotNo
            //               WHERE PG.ProductTypeId = 1 
            //               AND H.SiteId = 1 AND H.DivisionId = 1
            //               " + mQry1 + @"
            //               AND S.ProductUIDId IS NULL AND PU.SaleOrderLineId IS NOT NULL  AND PC.ProductCategoryName <> 'OVER TUFT'
            //               GROUP BY PU.ProductUIDId
            //               UNION ALL
            //               SELECT PU.ProductUIDId, sum(isnull(S.Qty_Rec, 0) - isnull(S.Qty_Iss, 0)) AS Qty
            //               FROM Web.StockHeaders H WITH(Nolock)
            //               LEFT JOIN web.Stocks S WITH(Nolock) ON S.StockHeaderId = H.StockHeaderId
            //               LEFT JOIN web.Products P WITH(Nolock) ON P.ProductId = S.ProductId
            //               LEFT JOIN web.ProductGroups PG WITH(Nolock) ON PG.ProductGroupId = P.ProductGroupId
            //               LEFT JOIN web.ProductCategories PC WITH(Nolock) ON PC.ProductCategoryId = P.ProductCategoryId
            //               LEFT JOIN web.ProductUids PU WITH(Nolock) ON PU.ProductUIDId = S.ProductUIDId--OR PU.ProductUidName = S.LotNo
            //               WHERE PG.ProductTypeId = 1 
            //               AND H.DocTypeId NOT IN (4024, 448)
            //               AND H.SiteId = 1 AND H.DivisionId = 1
            //               " + mQry1 + @"
            //               AND S.ProductUIDId IS NOT NULL AND PU.SaleOrderLineId IS NOT NULL  AND PC.ProductCategoryName = 'OVER TUFT'
            //               GROUP BY PU.ProductUIDId
            //               UNION ALL
            //               SELECT PU.ProductUIDId, sum(isnull(S.Qty_Rec, 0) - isnull(S.Qty_Iss, 0)) AS Qty
            //               FROM Web.StockHeaders H WITH(Nolock)
            //               LEFT JOIN web.Stocks S WITH(Nolock) ON S.StockHeaderId = H.StockHeaderId
            //               LEFT JOIN web.Products P WITH(Nolock) ON P.ProductId = S.ProductId
            //               LEFT JOIN web.ProductGroups PG WITH(Nolock) ON PG.ProductGroupId = P.ProductGroupId
            //               LEFT JOIN web.ProductCategories PC WITH(Nolock) ON PC.ProductCategoryId = P.ProductCategoryId
            //               LEFT JOIN web.ProductUids PU WITH(Nolock) ON PU.ProductUidName = S.LotNo
            //               WHERE PG.ProductTypeId = 1 
            //               AND H.SiteId = 1 AND H.DivisionId = 1
            //               " + mQry1 + @"
            //               AND H.DocTypeId NOT IN(4024, 448)
            //               AND S.ProductUIDId IS NULL AND PU.SaleOrderLineId IS NOT NULL  AND PC.ProductCategoryName = 'OVER TUFT'
            //               GROUP BY PU.ProductUIDId
            //               UNION ALL
            //               SELECT PU.ProductUIDId, sum(isnull(S.Qty_Rec, 0) - isnull(S.Qty_Iss, 0)) AS Qty
            //               FROM Web.StockHeaders H WITH(Nolock)
            //               LEFT JOIN web.StockProcesses S WITH(Nolock) ON S.StockHeaderId = H.StockHeaderId
            //               LEFT JOIN web.Products P WITH(Nolock) ON P.ProductId = S.ProductId
            //               LEFT JOIN web.ProductGroups PG WITH(Nolock) ON PG.ProductGroupId = P.ProductGroupId
            //               LEFT JOIN web.ProductCategories PC WITH(Nolock) ON PC.ProductCategoryId = P.ProductCategoryId
            //               LEFT JOIN web.ProductUids PU WITH(Nolock) ON PU.ProductUIDId = S.ProductUIDId--OR PU.ProductUidName = S.LotNo
            //               WHERE PG.ProductTypeId = 1 
            //               AND H.DocTypeId NOT IN (4024, 448, 5026)
            //               AND H.SiteId = 1 AND H.DivisionId = 1
            //               " + mQry1 + @"
            //               AND S.ProductUIDId IS NOT NULL AND PU.SaleOrderLineId IS NOT NULL  AND PC.ProductCategoryName = 'OVER TUFT'
            //               GROUP BY PU.ProductUIDId
            //               UNION ALL
            //               SELECT PU.ProductUIDId, sum(isnull(S.Qty_Rec, 0) - isnull(S.Qty_Iss, 0)) AS Qty
            //               FROM Web.StockHeaders H WITH(Nolock)
            //               LEFT JOIN web.StockProcesses S WITH(Nolock) ON S.StockHeaderId = H.StockHeaderId
            //               LEFT JOIN web.Products P WITH(Nolock) ON P.ProductId = S.ProductId
            //               LEFT JOIN web.ProductGroups PG WITH(Nolock) ON PG.ProductGroupId = P.ProductGroupId
            //               LEFT JOIN web.ProductCategories PC WITH(Nolock) ON PC.ProductCategoryId = P.ProductCategoryId
            //               LEFT JOIN web.ProductUids PU WITH(Nolock) ON PU.ProductUidName = S.LotNo
            //               WHERE PG.ProductTypeId = 1 
            //               AND H.SiteId = 1 AND H.DivisionId = 1
            //               " + mQry1 + @"
            //               AND H.DocTypeId NOT IN(4024, 448, 5026)
            //               AND S.ProductUIDId IS NULL AND PU.SaleOrderLineId IS NOT NULL  AND PC.ProductCategoryName = 'OVER TUFT'
            //               GROUP BY PU.ProductUIDId
            //               ) VS
            //               GROUP BY VS.ProductUIDId
            //               HAVING sum(VS.Qty) > 0
            //               ) VM
            //               LEFT JOIN web.ProductUids PU WITH(Nolock) ON PU.ProductUIDId = VM.ProductUIDId
            //               LEFT JOIN web.Products P WITH(Nolock) ON P.ProductId = PU.ProductId
            //               LEFT JOIN web.ProductGroups PG WITH(Nolock) ON PG.ProductGroupId = P.ProductGroupId
            //               LEFT JOIN web.FinishedProduct FP WITH(Nolock) ON FP.ProductId = P.ProductId
            //               LEFT JOIN web.Colours C WITH(Nolock) ON C.ColourId = FP.ColourId
            //               LEFT JOIN web.ViewRugSize VRS WITH(Nolock) ON VRS.ProductId = P.ProductId
            //               LEFT JOIN web.ProductCategories PC WITH(Nolock) ON PC.ProductCategoryId = P.ProductCategoryId
            //               LEFT JOIN web.ProductQualities PQ WITH(Nolock) ON PQ.ProductQualityId = FP.ProductQualityId
            //               LEFT JOIN web.SaleOrderLines SOL WITH(Nolock) ON SOL.SaleOrderLineId = PU.SaleOrderLineId
            //               LEFT JOIN web.SaleOrderHeaders SOH WITH(Nolock) ON SOH.SaleOrderHeaderId = SOL.SaleOrderHeaderId
            //               LEFT JOIN
            //               (
            //               SELECT JOL.ProductUIDId, Max(JOL.Qty) AS OTO, Max(RL.RecQty) AS OTR
            //               FROM WEb.JobOrderHeaders  JOH WITH(Nolock)
            //               LEFT JOIN web.JobOrderLines JOL WITH(Nolock) ON JOL.JobOrderHeaderId = JOH.JobOrderHeaderId
            //               LEFT JOIN
            //               (
            //               SELECT RL.JobOrderLineId, sum(RL.Qty) AS RecQty  FROM web.JobReceiveLines RL GROUP BY RL.JobOrderLineId
            //               ) RL ON RL.JobOrderLineId = JOL.JobOrderLineId
            //               WHERE JOH.DocTypeId = 4024 AND JOL.ProductUIDId IS NOT NULL
            //               GROUP BY JOL.ProductUIDId
            //) A ON A.ProductUIDId = PU.ProductUIDId   
            //               WHERE 1=1 " +
            //               (DocTypeId != null ? " AND SOH.DocTypeId IN (SELECT Items FROM [dbo].[Split] (@DocTypeId, ','))" : "") +
            //               (Product != null ? " AND P.ProductId IN (SELECT Items FROM [dbo].[Split] (@Product, ','))" : "") +
            //               (ProductQuality != null ? " AND Fp.ProductQualityId IN (SELECT Items FROM  Web.[Split] (@ProductQuality, ','))" : "") +
            //               (ProductCategory != null ? " AND P.ProductCategoryId  IN (SELECT Items FROM  Web.[Split] (@ProductCategory, ','))" : "") +
            //               (ProductGroup != null ? " AND P.ProductGroupId IN (SELECT Items FROM  Web.[Split] (@ProductGroup, ','))" : "") +
            //               (SaleOrderHeaderId != null ? " AND SOH.SaleOrderHeaderId IN (SELECT Items FROM [dbo].[Split] (@SaleOrderHeaderId, ','))" : "") +
            //               (Buyer != null ? " AND SOH.SaleToBuyerId  IN (SELECT Items FROM [dbo].[Split] (@Buyer, ','))" : ""); ;



            mQry = @"
                    SELECT PU.ProductUidName AS CarpetNo, 
                    CASE WHEN PC.ProductCategoryName ='Over Tuft' THEN  Replace(Convert(NVARCHAR, A.RecDate, 106), ' ', '/') ELSE Replace(Convert(NVARCHAR, PU.gendocDate, 106), ' ', '/') END AS Date, 
                    PC.ProductCategoryName AS Type, PQ.ProductQualityName AS Quality, PG.ProductGroupName AS Design, C.ColourName AS Colour, VRS.ManufaturingSizeName AS Size,  VM.StockQty AS Qty, A.OTO AS OTI, A.OTR AS OTR,
                    CASE WHEN isnull(PU.CurrenctProcessId, 0) = 46 THEN 'Rejected' WHEN isnull(PU.CurrenctGodownId, 0) = 1 AND charindex(',', PU.ProcessesDone) > 0 THEN 'Finished' ELSE 'Un-Finished' END Status
                    FROM
                    (
                    SELECT VS.ProductUIDId, sum(VS.Qty) AS StockQty
                    FROM
                    (
                    SELECT PU.ProductUIDId, sum(isnull(S.Qty_Rec, 0) - isnull(S.Qty_Iss, 0)) AS Qty
                    FROM Web.StockHeaders H WITH(Nolock)
                    LEFT JOIN web.Stocks S WITH(Nolock) ON S.StockHeaderId = H.StockHeaderId
                    LEFT JOIN web.Products P WITH(Nolock) ON P.ProductId = S.ProductId
                    LEFT JOIN web.ProductGroups PG WITH(Nolock) ON PG.ProductGroupId = P.ProductGroupId
                    LEFT JOIN web.ProductCategories PC WITH(Nolock) ON PC.ProductCategoryId = P.ProductCategoryId
                    LEFT JOIN web.ProductUids PU WITH(Nolock) ON PU.ProductUIDId = S.ProductUIDId
                    WHERE PG.ProductTypeId = 1 
                    AND H.SiteId = 1 AND H.DivisionId = 1
                    " + mQry1 + @"
                    AND S.ProductUIDId IS NOT NULL AND PU.SaleOrderLineId IS NOT NULL 
                    GROUP BY PU.ProductUIDId
                    UNION ALL
                    SELECT PU.ProductUIDId, sum(isnull(S.Qty_Rec, 0) - isnull(S.Qty_Iss, 0)) AS Qty
                    FROM Web.StockHeaders H WITH(Nolock)
                    LEFT JOIN web.Stocks S WITH(Nolock) ON S.StockHeaderId = H.StockHeaderId
                    LEFT JOIN web.Products P WITH(Nolock) ON P.ProductId = S.ProductId
                    LEFT JOIN web.ProductGroups PG WITH(Nolock) ON PG.ProductGroupId = P.ProductGroupId
                    LEFT JOIN web.ProductCategories PC WITH(Nolock) ON PC.ProductCategoryId = P.ProductCategoryId
                    LEFT JOIN web.ProductUids PU WITH(Nolock) ON PU.ProductUidName = S.LotNo
                    WHERE PG.ProductTypeId = 1 
                    AND H.SiteId = 1 AND H.DivisionId = 1
                    " + mQry1 + @"
                    AND S.ProductUIDId IS NULL AND PU.SaleOrderLineId IS NOT NULL 
                    GROUP BY PU.ProductUIDId
                    UNION ALL
                    SELECT PU.ProductUIDId, sum(isnull(S.Qty_Rec, 0) - isnull(S.Qty_Iss, 0)) AS Qty
                    FROM Web.StockHeaders H WITH(Nolock)
                    LEFT JOIN web.StockProcesses S WITH(Nolock) ON S.StockHeaderId = H.StockHeaderId
                    LEFT JOIN web.Products P WITH(Nolock) ON P.ProductId = S.ProductId
                    LEFT JOIN web.ProductGroups PG WITH(Nolock) ON PG.ProductGroupId = P.ProductGroupId
                    LEFT JOIN web.ProductCategories PC WITH(Nolock) ON PC.ProductCategoryId = P.ProductCategoryId
                    LEFT JOIN web.ProductUids PU WITH(Nolock) ON PU.ProductUIDId = S.ProductUIDId--OR PU.ProductUidName = S.LotNo
                    WHERE PG.ProductTypeId = 1 
                    AND H.SiteId = 1 AND H.DivisionId = 1
                    " + mQry1 + @"
                    AND S.ProductUIDId IS NOT NULL AND PU.SaleOrderLineId IS NOT NULL  
                    GROUP BY PU.ProductUIDId
                    UNION ALL
                    SELECT PU.ProductUIDId, sum(isnull(S.Qty_Rec, 0) - isnull(S.Qty_Iss, 0)) AS Qty
                    FROM Web.StockHeaders H WITH(Nolock)
                    LEFT JOIN web.StockProcesses S WITH(Nolock) ON S.StockHeaderId = H.StockHeaderId
                    LEFT JOIN web.Products P WITH(Nolock) ON P.ProductId = S.ProductId
                    LEFT JOIN web.ProductGroups PG WITH(Nolock) ON PG.ProductGroupId = P.ProductGroupId
                    LEFT JOIN web.ProductCategories PC WITH(Nolock) ON PC.ProductCategoryId = P.ProductCategoryId
                    LEFT JOIN web.ProductUids PU WITH(Nolock) ON PU.ProductUidName = S.LotNo
                    WHERE PG.ProductTypeId = 1 
                    AND H.SiteId = 1 AND H.DivisionId = 1
                    " + mQry1 + @"
                    AND S.ProductUIDId IS NULL AND PU.SaleOrderLineId IS NOT NULL  
                    GROUP BY PU.ProductUIDId

                    ) VS
                    GROUP BY VS.ProductUIDId
                    HAVING sum(VS.Qty) > 0
                    ) VM
                    LEFT JOIN web.ProductUids PU WITH(Nolock) ON PU.ProductUIDId = VM.ProductUIDId
                    LEFT JOIN web.Products P WITH(Nolock) ON P.ProductId = PU.ProductId
                    LEFT JOIN web.ProductGroups PG WITH(Nolock) ON PG.ProductGroupId = P.ProductGroupId
                    LEFT JOIN web.FinishedProduct FP WITH(Nolock) ON FP.ProductId = P.ProductId
                    LEFT JOIN web.Colours C WITH(Nolock) ON C.ColourId = FP.ColourId
                    LEFT JOIN web.ViewRugSize VRS WITH(Nolock) ON VRS.ProductId = P.ProductId
                    LEFT JOIN web.ProductCategories PC WITH(Nolock) ON PC.ProductCategoryId = P.ProductCategoryId
                    LEFT JOIN web.ProductQualities PQ WITH(Nolock) ON PQ.ProductQualityId = FP.ProductQualityId
                    LEFT JOIN web.SaleOrderLines SOL WITH(Nolock) ON SOL.SaleOrderLineId = PU.SaleOrderLineId
                    LEFT JOIN web.SaleOrderHeaders SOH WITH(Nolock) ON SOH.SaleOrderHeaderId = SOL.SaleOrderHeaderId
                    LEFT JOIN
                    (
                    SELECT JOL.ProductUIDId, Max(JOL.Qty) AS OTO, Max(RL.RecQty) AS OTR, Max(RL.RecDate) AS RecDate
                    FROM WEb.JobOrderHeaders  JOH WITH(Nolock)
                    LEFT JOIN web.JobOrderLines JOL WITH(Nolock) ON JOL.JobOrderHeaderId = JOH.JobOrderHeaderId
                    LEFT JOIN
                    (
                    SELECT RL.JobOrderLineId, Max(RH.DocDate) AS RecDate, sum(RL.Qty) AS RecQty  
                    FROM web.JobReceiveLines RL WITH(Nolock)
                    LEFT JOIN web.JobReceiveHeaders RH WITH(Nolock) ON RH.JobReceiveHeaderId = RL.JobReceiveHeaderId
                    GROUP BY RL.JobOrderLineId
                    ) RL ON RL.JobOrderLineId = JOL.JobOrderLineId
                    WHERE JOH.DocTypeId = 4024 AND JOL.ProductUIDId IS NOT NULL
                    GROUP BY JOL.ProductUIDId
					) A ON A.ProductUIDId = PU.ProductUIDId   
                    WHERE 1=1 " +
        (DocTypeId != null ? " AND SOH.DocTypeId IN (SELECT Items FROM [dbo].[Split] (@DocTypeId, ','))" : "") +
        (Product != null ? " AND P.ProductId IN (SELECT Items FROM [dbo].[Split] (@Product, ','))" : "") +
        (ProductQuality != null ? " AND Fp.ProductQualityId IN (SELECT Items FROM  Web.[Split] (@ProductQuality, ','))" : "") +
        (ProductCategory != null ? " AND P.ProductCategoryId  IN (SELECT Items FROM  Web.[Split] (@ProductCategory, ','))" : "") +
        (ProductGroup != null ? " AND P.ProductGroupId IN (SELECT Items FROM  Web.[Split] (@ProductGroup, ','))" : "") +
        (SaleOrderHeaderId != null ? " AND SOH.SaleOrderHeaderId IN (SELECT Items FROM [dbo].[Split] (@SaleOrderHeaderId, ','))" : "") +
        (Buyer != null ? " AND SOH.SaleToBuyerId  IN (SELECT Items FROM [dbo].[Split] (@Buyer, ','))" : ""); ;


            IEnumerable<SaleOrderInventoryStatus_StockViewModel> SaleOrderInventoryStatusList = db.Database.SqlQuery<SaleOrderInventoryStatus_StockViewModel>(mQry, SqlParameterTextHidden, SqlParameterStatusOnDate, SqlParameterDocTypeId, SqlParameterSite, SqlParameterDivision, SqlParameterFromDate, SqlParameterToDate, SqlParameterBuyer, SqlParameterSaleOrderHeaderId, SqlParameterProduct, SqlParameterProductCategory, SqlParameterProductQuality, SqlParameterProductGroup, SqlParameterProductSize, SqlParameterReportFor, SqlParameterBuyerDesign).ToList();


            return SaleOrderInventoryStatusList;

        }

        public IEnumerable<SaleOrderInventoryStatus_LoomViewModel> LoomDetail(SaleOrderInventoryStatusDisplayFilterSettings Settings)
        {
            var StatusOnDateSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "StatusOnDate" select H).FirstOrDefault();
            var DocTypeIdSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "DocTypeId" select H).FirstOrDefault();
            var SiteSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "Site" select H).FirstOrDefault();
            var DivisionSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "Division" select H).FirstOrDefault();
            var FromDateSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "FromDate" select H).FirstOrDefault();
            var ToDateSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ToDate" select H).FirstOrDefault();
            var BuyerSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "Buyer" select H).FirstOrDefault();
            var SaleOrderHeaderIdSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "SaleOrderHeaderId" select H).FirstOrDefault();
            var ProductSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "Product" select H).FirstOrDefault();
            var ProductCategorySetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ProductCategory" select H).FirstOrDefault();
            var ProductQualitySetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ProductQuality" select H).FirstOrDefault();
            var ProductGroupSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ProductGroup" select H).FirstOrDefault();
            var ProductSizeSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ProductSize" select H).FirstOrDefault();
            var ReportTypeSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ReportType" select H).FirstOrDefault();
            var ReportForSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ReportFor" select H).FirstOrDefault();
            var NextFormatSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "NextFormat" select H).FirstOrDefault();
            var BuyerDesignSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "BuyerDesign" select H).FirstOrDefault();
            var TextHiddenSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "TextHidden" select H).FirstOrDefault();



            string StatusOnDate = StatusOnDateSetting.Value;
            string DocTypeId = DocTypeIdSetting.Value;
            //string Site = SiteSetting.Value;
            //string Division = DivisionSetting.Value;
            string Site = "1";
            string Division = "1";
            string FromDate = FromDateSetting.Value;
            string ToDate = ToDateSetting.Value;
            string Buyer = BuyerSetting.Value;
            string SaleOrderHeaderId = SaleOrderHeaderIdSetting.Value;
            string Product = ProductSetting.Value;
            string ProductCategory = ProductCategorySetting.Value;
            string ProductQuality = ProductQualitySetting.Value;
            string ProductGroup = ProductGroupSetting.Value;
            string ProductSize = ProductSizeSetting.Value;
            string ReportType = ReportTypeSetting.Value;
            string ReportFor = ReportForSetting.Value;
            string NextFormat = NextFormatSetting.Value;
            string BuyerDesign = BuyerDesignSetting.Value;
            string TextHidden = TextHiddenSetting.Value;


            string mQry;

            if (Buyer == "-1")
                Buyer = null;

            SqlParameter SqlParameterStatusOnDate = new SqlParameter("@StatusOnDate", !string.IsNullOrEmpty(StatusOnDate) ? StatusOnDate : (object)DBNull.Value);
            SqlParameter SqlParameterDocTypeId = new SqlParameter("@DocTypeId", !string.IsNullOrEmpty(DocTypeId) ? DocTypeId : (object)DBNull.Value);
            SqlParameter SqlParameterSite = new SqlParameter("@Site", !string.IsNullOrEmpty(Site) ? Site : (object)DBNull.Value);
            SqlParameter SqlParameterDivision = new SqlParameter("@Division", !string.IsNullOrEmpty(Division) ? Division : (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", !string.IsNullOrEmpty(FromDate) ? FromDate : (object)DBNull.Value);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", !string.IsNullOrEmpty(ToDate) ? ToDate : (object)DBNull.Value);
            SqlParameter SqlParameterBuyer = new SqlParameter("@Buyer", !string.IsNullOrEmpty(Buyer) ? Buyer : (object)DBNull.Value);
            SqlParameter SqlParameterSaleOrderHeaderId = new SqlParameter("@SaleOrderHeaderId", !string.IsNullOrEmpty(SaleOrderHeaderId) ? SaleOrderHeaderId : (object)DBNull.Value);
            SqlParameter SqlParameterProduct = new SqlParameter("@Product", !string.IsNullOrEmpty(Product) ? Product : (object)DBNull.Value);
            SqlParameter SqlParameterProductCategory = new SqlParameter("@ProductCategory", !string.IsNullOrEmpty(ProductCategory) ? ProductCategory : (object)DBNull.Value);
            SqlParameter SqlParameterProductQuality = new SqlParameter("@ProductQuality", !string.IsNullOrEmpty(ProductQuality) ? ProductQuality : (object)DBNull.Value);
            SqlParameter SqlParameterProductGroup = new SqlParameter("@ProductGroup", !string.IsNullOrEmpty(ProductGroup) ? ProductGroup : (object)DBNull.Value);
            SqlParameter SqlParameterProductSize = new SqlParameter("@ProductSize", !string.IsNullOrEmpty(ProductSize) ? ProductSize : (object)DBNull.Value);
            SqlParameter SqlParameterReportType = new SqlParameter("@ReportType", !string.IsNullOrEmpty(ReportType) ? ReportType : (object)DBNull.Value);
            SqlParameter SqlParameterReportFor = new SqlParameter("@ReportFor", !string.IsNullOrEmpty(ReportFor) ? ReportFor : (object)DBNull.Value);
            SqlParameter SqlParameterNextFormat = new SqlParameter("@NextFormat", !string.IsNullOrEmpty(NextFormat) ? NextFormat : (object)DBNull.Value);
            SqlParameter SqlParameterBuyerDesign = new SqlParameter("@BuyerDesign", !string.IsNullOrEmpty(BuyerDesign) ? BuyerDesign : (object)DBNull.Value);
            SqlParameter SqlParameterTextHidden = new SqlParameter("@TextHidden", !string.IsNullOrEmpty(TextHidden) ? TextHidden : (object)DBNull.Value);


            mQry = @"SELECT S.SiteName AS Branch,CC.CostCenterName AS Purza_No,  Replace( Convert(NVARCHAR,JOH.DocDate,106),' ','/') AS Date,  J.Name AS Weaver,
                    PQ.ProductQualityName AS Quality, PG.ProductGroupName AS Design, C.ColourName AS Colour, VRS.ManufaturingSizeName AS Size,
                     isnull(H.BalanceQty,0) AS LoomQty
                    FROM [Web].[FWeavingOrderStatus_OneProcess] (Getdate(),NULL ,NULL,'"+ FromDate + @"',Getdate(),43) H
                    LEFT JOIN web.JobOrderHeaders JOH WITH (Nolock) ON JOH.JobOrderHeaderId =H.JobOrderHeaderId
                    LEFT JOIN web.Sites S ON S.SiteId = H.SiteId
                    LEFT JOIN web.CostCenters CC ON CC.CostCenterId = JOH.CostCenterId 
                    LEFT JOIN web.People J ON J.PersonID = JOH.JobWorkerId 
                    LEFT JOIN Web.ProdOrderLines POL WITH (Nolock) ON POL.ProdOrderLineId=H.ProdOrderLineId
                    LEFT JOIN web.JobOrderLines JOL WITH (Nolock) ON JOL.JobOrderLineId = POL.ReferenceDocLineId 
                    LEFT JOIN web.ProdOrderLines POL1 WITH (Nolock) ON POL1.ProdOrderLineId = isnull(JOL.ProdOrderLineId,H.ProdOrderLineId) 
                    LEFT JOIN web.ProdOrderHeaders POH WITH (Nolock) ON  POH.ProdOrderHeaderId = POL1.ProdOrderHeaderId 
                    LEFT JOIN web.Products P WITH (Nolock) ON P.ProductId = H.ProductId 
                    LEFT JOIN web.ProductGroups PG WITH (Nolock) ON PG.ProductGroupId = P.ProductGroupId
                    LEFT JOIN web.FinishedProduct FP WITH (Nolock) ON FP.ProductId = P.ProductId 
                    LEFT JOIN web.Colours C WITH (Nolock) ON C.ColourId = FP.ColourId 
                    LEFT JOIN web.ViewRugSize VRS WITH (Nolock) ON VRS.ProductId = P.ProductId 
                    LEFT JOIN web.ProductCategories PC WITH (Nolock) ON PC.ProductCategoryId = P.ProductCategoryId 
                    LEFT JOIN web.ProductQualities PQ WITH (Nolock) ON PQ.ProductQualityId = FP.ProductQualityId 
                    LEFT JOIN web.MaterialPlanForSaleOrders MOS WITH (Nolock) ON MOS.MaterialPlanLineId = POL1.MaterialPlanLineId  
                    LEFT JOIN web.SaleOrderLines SOL WITH (Nolock) ON SOL.SaleOrderLineId = MOS.SaleOrderLineId
                    LEFT JOIN web.SaleOrderHeaders SOH WITH (Nolock) ON SOH.SaleOrderHeaderId = SOL.SaleOrderHeaderId
                    WHERE 1=1 AND isnull(J.IsSisterConcern,0)=0 
                    AND isnull(H.BalanceQty,0) >0 AND PC.ProductCategoryName <> 'OVER TUFT' " +
                    (DocTypeId != null ? " AND SOH.DocTypeId IN (SELECT Items FROM [dbo].[Split] (@DocTypeId, ','))" : "") +
                    (Product != null ? " AND P.ProductId IN (SELECT Items FROM [dbo].[Split] (@Product, ','))" : "") +
                    (ProductQuality != null ? " AND Fp.ProductQualityId IN (SELECT Items FROM  Web.[Split] (@ProductQuality, ','))" : "") +
                    (ProductCategory != null ? " AND P.ProductCategoryId  IN (SELECT Items FROM  Web.[Split] (@ProductCategory, ','))" : "") +
                    (ProductGroup != null ? " AND P.ProductGroupId IN (SELECT Items FROM  Web.[Split] (@ProductGroup, ','))" : "") +
                    (SaleOrderHeaderId != null ? " AND SOH.SaleOrderHeaderId IN (SELECT Items FROM [dbo].[Split] (@SaleOrderHeaderId, ','))" : "") +
                    (Buyer != null ? " AND SOH.SaleToBuyerId  IN (SELECT Items FROM [dbo].[Split] (@Buyer, ','))" : "");

            if (ReportTypeSetting.Value == "Summary")
                mQry = mQry + " AND P.ProductId = @TextHidden ";
            else
                mQry = mQry + " AND MOS.SaleOrderLineId = @TextHidden ";

            string mqry1 = "";
            mqry1 = @"SELECT S.SiteName AS Branch,CC.CostCenterName AS Purza_No,  Replace(Convert(NVARCHAR, H.DocDate, 106), ' ', '/') AS Date, JW.Name AS Weaver,
                    PQ.ProductQualityName AS Quality, PG.ProductGroupName AS Design, C.ColourName AS Colour, VRS.ManufaturingSizeName AS Size,
                     isnull(L.Qty, 0) - isnull(V.RecQty, 0) AS LoomQty
                    FROM web.JobOrderHeaders H WITH(Nolock)
                    LEFT JOIN web.Sites S ON S.SiteId = H.SiteId
                    LEFT JOIN web.CostCenters CC ON CC.CostCenterId = H.CostCenterId
                    LEFT JOIN web.People JW ON JW.PersonID = H.JobWorkerId
                    LEFT JOIN web.JobOrderLines L ON L.JobOrderHeaderId = H.JobOrderHeaderId
                    LEFT JOIN Web.ProdOrderLines POL WITH(Nolock) ON POL.ProdOrderLineId = L.ProdOrderLineId
                    LEFT JOIN web.ProdOrderHeaders POH WITH(Nolock) ON POH.ProdOrderHeaderId = POL.ProdOrderHeaderId
                    LEFT JOIN Web._ViewRugSize VRS WITH(Nolock) ON VRS.ProductId = L.ProductId
                    LEFT JOIN web.Products P WITH(Nolock) ON P.ProductId = L.ProductId
                    LEFT JOIN web.ProductCategories PC WITH(Nolock) ON PC.ProductCategoryId = P.ProductCategoryId
                    LEFT JOIN web.ProductGroups PG WITH(Nolock) ON PG.ProductGroupId = P.ProductGroupId
                    LEFT JOIN web.FinishedProduct FP WITH(Nolock) ON FP.ProductId = P.ProductId
                    LEFT JOIN web.Colours C WITH(Nolock) ON C.ColourId = FP.ColourId
                    LEFT JOIN web.ProductQualities PQ WITH(Nolock) ON PQ.ProductQualityId = FP.ProductQualityId
                    LEFT JOIN web.MaterialPlanForSaleOrders MOS WITH(Nolock) ON MOS.MaterialPlanLineId = POL.MaterialPlanLineId
                    LEFT JOIN web.SaleOrderLines SOL WITH(Nolock) ON SOL.SaleOrderLineId = MOS.SaleOrderLineId
                    LEFT JOIN web.SaleOrderHeaders SOH WITH(Nolock) ON SOH.SaleOrderHeaderId = SOL.SaleOrderHeaderId
                    LEFT JOIN
                    (
                    SELECT V1.JobOrderLineId, sum(L.Qty) AS RecQty
                    FROM web.JobReceiveHeaders H WITH(NoLock)
                    LEFT JOIN web.JobReceiveLines L WITH(NoLock) ON L.JobReceiveHeaderId = H.JobReceiveHeaderId
                    LEFT JOIN
                    (
                    SELECT L.ProductUidId, JOL.JobOrderLineId, JOL.ProdOrderLineId, JOL.ProductId
                    FROM web.JobReceiveHeaders H WITH(NoLock)
                    LEFT JOIN web.JobReceiveLines L WITH(NoLock) ON L.JobReceiveHeaderId = H.JobReceiveHeaderId
                    LEFT JOIN web.JobOrderLines JOL WITH(NoLock) ON JOL.JobOrderLineId = L.JobOrderLineId
                    WHERE H.SiteId = 1 AND H.DivisionId = 1 AND H.DocTypeId = 448
                    AND L.ProductUidId IS NOT NULL
                    ) V1 ON V1.ProductUidId = L.ProductUidId
                    WHERE H.SiteId = 1 AND H.DivisionId = 1 AND H.ProcessId = 2008  AND V1.JobOrderLineId IS NOT NULL
                    GROUP BY V1.JobOrderLineId
                    ) V ON V.JobOrderLineId = L.JobOrderLineId
                    WHERE H.SiteId = 1 AND H.DivisionId = 1 AND H.ProcessId = 43 AND isnull(JW.IsSisterConcern, 0)= 0  AND POL.ProdOrderHeaderId IS NOT NULL AND PC.ProductCategoryName = 'OVER TUFT'
                    AND isnull(L.Qty, 0)-isnull(V.RecQty, 0) > 0 " +
                    (DocTypeId != null ? " AND SOH.DocTypeId IN (SELECT Items FROM [dbo].[Split] (@DocTypeId, ','))" : "") +
                    (Product != null ? " AND P.ProductId IN (SELECT Items FROM [dbo].[Split] (@Product, ','))" : "") +
                    (ProductQuality != null ? " AND Fp.ProductQualityId IN (SELECT Items FROM  Web.[Split] (@ProductQuality, ','))" : "") +
                    (ProductCategory != null ? " AND P.ProductCategoryId  IN (SELECT Items FROM  Web.[Split] (@ProductCategory, ','))" : "") +
                    (ProductGroup != null ? " AND P.ProductGroupId IN (SELECT Items FROM  Web.[Split] (@ProductGroup, ','))" : "") +
                    (SaleOrderHeaderId != null ? " AND SOH.SaleOrderHeaderId IN (SELECT Items FROM [dbo].[Split] (@SaleOrderHeaderId, ','))" : "") +
                    (Buyer != null ? " AND SOH.SaleToBuyerId  IN (SELECT Items FROM [dbo].[Split] (@Buyer, ','))" : "");

            if (ReportTypeSetting.Value == "Summary")
                mqry1 = mqry1 + " AND P.ProductId = @TextHidden ";
            else
                mqry1 = mqry1 + " AND MOS.SaleOrderLineId = @TextHidden ";

            mQry = mQry + " UNION ALL " + mqry1;

            IEnumerable<SaleOrderInventoryStatus_LoomViewModel> SaleOrderInventoryStatusList = db.Database.SqlQuery<SaleOrderInventoryStatus_LoomViewModel>(mQry, SqlParameterTextHidden, SqlParameterStatusOnDate, SqlParameterDocTypeId, SqlParameterSite, SqlParameterDivision, SqlParameterFromDate, SqlParameterToDate, SqlParameterBuyer, SqlParameterSaleOrderHeaderId, SqlParameterProduct, SqlParameterProductCategory, SqlParameterProductQuality, SqlParameterProductGroup, SqlParameterProductSize, SqlParameterReportFor, SqlParameterBuyerDesign).ToList();


            return SaleOrderInventoryStatusList;

        }

        public IEnumerable<SaleOrderInventoryStatus_ShipViewModel> ShipDetail(SaleOrderInventoryStatusDisplayFilterSettings Settings)
        {
            var StatusOnDateSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "StatusOnDate" select H).FirstOrDefault();
            var DocTypeIdSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "DocTypeId" select H).FirstOrDefault();
            var SiteSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "Site" select H).FirstOrDefault();
            var DivisionSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "Division" select H).FirstOrDefault();
            var FromDateSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "FromDate" select H).FirstOrDefault();
            var ToDateSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ToDate" select H).FirstOrDefault();
            var BuyerSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "Buyer" select H).FirstOrDefault();
            var SaleOrderHeaderIdSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "SaleOrderHeaderId" select H).FirstOrDefault();
            var ProductSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "Product" select H).FirstOrDefault();
            var ProductCategorySetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ProductCategory" select H).FirstOrDefault();
            var ProductQualitySetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ProductQuality" select H).FirstOrDefault();
            var ProductGroupSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ProductGroup" select H).FirstOrDefault();
            var ProductSizeSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ProductSize" select H).FirstOrDefault();
            var ReportTypeSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ReportType" select H).FirstOrDefault();
            var ReportForSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ReportFor" select H).FirstOrDefault();
            var NextFormatSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "NextFormat" select H).FirstOrDefault();
            var BuyerDesignSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "BuyerDesign" select H).FirstOrDefault();
            var TextHiddenSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "TextHidden" select H).FirstOrDefault();



            string StatusOnDate = StatusOnDateSetting.Value;
            string DocTypeId = DocTypeIdSetting.Value;
            //string Site = SiteSetting.Value;
            //string Division = DivisionSetting.Value;
            string Site = "1";
            string Division = "1";
            string FromDate = FromDateSetting.Value;
            string ToDate = ToDateSetting.Value;
            string Buyer = BuyerSetting.Value;
            string SaleOrderHeaderId = SaleOrderHeaderIdSetting.Value;
            string Product = ProductSetting.Value;
            string ProductCategory = ProductCategorySetting.Value;
            string ProductQuality = ProductQualitySetting.Value;
            string ProductGroup = ProductGroupSetting.Value;
            string ProductSize = ProductSizeSetting.Value;
            string ReportType = ReportTypeSetting.Value;
            string ReportFor = ReportForSetting.Value;
            string NextFormat = NextFormatSetting.Value;
            string BuyerDesign = BuyerDesignSetting.Value;
            string TextHidden = TextHiddenSetting.Value;


            string mQry;

            if (Buyer == "-1")
                Buyer = null;

            SqlParameter SqlParameterStatusOnDate = new SqlParameter("@StatusOnDate", !string.IsNullOrEmpty(StatusOnDate) ? StatusOnDate : (object)DBNull.Value);
            SqlParameter SqlParameterDocTypeId = new SqlParameter("@DocTypeId", !string.IsNullOrEmpty(DocTypeId) ? DocTypeId : (object)DBNull.Value);
            SqlParameter SqlParameterSite = new SqlParameter("@Site", !string.IsNullOrEmpty(Site) ? Site : (object)DBNull.Value);
            SqlParameter SqlParameterDivision = new SqlParameter("@Division", !string.IsNullOrEmpty(Division) ? Division : (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", !string.IsNullOrEmpty(FromDate) ? FromDate : (object)DBNull.Value);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", !string.IsNullOrEmpty(ToDate) ? ToDate : (object)DBNull.Value);
            SqlParameter SqlParameterBuyer = new SqlParameter("@Buyer", !string.IsNullOrEmpty(Buyer) ? Buyer : (object)DBNull.Value);
            SqlParameter SqlParameterSaleOrderHeaderId = new SqlParameter("@SaleOrderHeaderId", !string.IsNullOrEmpty(SaleOrderHeaderId) ? SaleOrderHeaderId : (object)DBNull.Value);
            SqlParameter SqlParameterProduct = new SqlParameter("@Product", !string.IsNullOrEmpty(Product) ? Product : (object)DBNull.Value);
            SqlParameter SqlParameterProductCategory = new SqlParameter("@ProductCategory", !string.IsNullOrEmpty(ProductCategory) ? ProductCategory : (object)DBNull.Value);
            SqlParameter SqlParameterProductQuality = new SqlParameter("@ProductQuality", !string.IsNullOrEmpty(ProductQuality) ? ProductQuality : (object)DBNull.Value);
            SqlParameter SqlParameterProductGroup = new SqlParameter("@ProductGroup", !string.IsNullOrEmpty(ProductGroup) ? ProductGroup : (object)DBNull.Value);
            SqlParameter SqlParameterProductSize = new SqlParameter("@ProductSize", !string.IsNullOrEmpty(ProductSize) ? ProductSize : (object)DBNull.Value);
            SqlParameter SqlParameterReportType = new SqlParameter("@ReportType", !string.IsNullOrEmpty(ReportType) ? ReportType : (object)DBNull.Value);
            SqlParameter SqlParameterReportFor = new SqlParameter("@ReportFor", !string.IsNullOrEmpty(ReportFor) ? ReportFor : (object)DBNull.Value);
            SqlParameter SqlParameterNextFormat = new SqlParameter("@NextFormat", !string.IsNullOrEmpty(NextFormat) ? NextFormat : (object)DBNull.Value);
            SqlParameter SqlParameterBuyerDesign = new SqlParameter("@BuyerDesign", !string.IsNullOrEmpty(BuyerDesign) ? BuyerDesign : (object)DBNull.Value);
            SqlParameter SqlParameterTextHidden = new SqlParameter("@TextHidden", !string.IsNullOrEmpty(TextHidden) ? TextHidden : (object)DBNull.Value);


            mQry = @"SELECT H.DocNo AS Invoice_No, Replace(Convert(NVARCHAR,H.DocDate,106),' ','/') AS Date, PL.BaleNo AS Roll_No, PL.Remark, PU.ProductUidName AS CarpetNo, PC.ProductCategoryName AS Type,
                    PQ.ProductQualityName AS Quality, PG.ProductGroupName AS Design, C.ColourName AS Colour, VRS.ManufaturingSizeName AS Size, PL.Qty  
                    FROM web.SaleInvoiceHeaders H WITH (Nolock)
                    LEFT JOIN web.SaleInvoiceLines L WITH (Nolock) ON L.SaleInvoiceHeaderId = H.SaleInvoiceHeaderId 
                    LEFT JOIN web.SaleDispatchLines SDL WITH (Nolock) ON SDL.SaleDispatchLineId = L.SaleDispatchLineId 
                    LEFT JOIN web.PackingLines PL WITH (Nolock) ON PL.PackingLineId = SDL.PackingLineId
                    LEFT JOIN web.ProductUids PU ON PU.ProductUIDId = PL.ProductUIDId OR PU.ProductUidName = PL.LotNo 
                    LEFT JOIN web.Products P WITH (Nolock) ON P.ProductId = PL.ProductId 
                    LEFT JOIN web.ProductGroups PG WITH (Nolock) ON PG.ProductGroupId = P.ProductGroupId
                    LEFT JOIN web.FinishedProduct FP WITH (Nolock) ON FP.ProductId = P.ProductId 
                    LEFT JOIN web.Colours C WITH (Nolock) ON C.ColourId = FP.ColourId 
                    LEFT JOIN web.ViewRugSize VRS WITH (Nolock) ON VRS.ProductId = P.ProductId 
                    LEFT JOIN web.ProductCategories PC WITH (Nolock) ON PC.ProductCategoryId = P.ProductCategoryId 
                    LEFT JOIN web.ProductQualities PQ WITH (Nolock) ON PQ.ProductQualityId = FP.ProductQualityId 
                    LEFT JOIN web.SaleOrderLines SOL WITH (Nolock) ON SOL.SaleOrderLineId = PL.SaleOrderLineId
                    LEFT JOIN web.SaleOrderHeaders SOH WITH (Nolock) ON SOH.SaleOrderHeaderId = SOL.SaleOrderHeaderId
                    Where 1=1 " +
                    (DocTypeId != null ? " AND SOH.DocTypeId IN (SELECT Items FROM[dbo].[Split](@DocTypeId, ','))" : "") +
                      (Product != null ? " AND P.ProductId IN (SELECT Items FROM [dbo].[Split] (@Product, ','))" : "") +
                      (ProductQuality != null ? " AND Fp.ProductQualityId IN (SELECT Items FROM  Web.[Split] (@ProductQuality, ','))" : "") +
                      (ProductCategory != null ? " AND P.ProductCategoryId  IN (SELECT Items FROM  Web.[Split] (@ProductCategory, ','))" : "") +
                      (ProductGroup != null ? " AND P.ProductGroupId IN (SELECT Items FROM  Web.[Split] (@ProductGroup, ','))" : "") +
                      (SaleOrderHeaderId != null ? " AND SOH.SaleOrderHeaderId IN (SELECT Items FROM [dbo].[Split] (@SaleOrderHeaderId, ','))" : "") +
                      (Buyer != null ? " AND SOH.SaleToBuyerId  IN (SELECT Items FROM [dbo].[Split] (@Buyer, ','))" : "");


             if (ReportTypeSetting.Value == "Summary")
                mQry = mQry + " AND P.ProductId = @TextHidden ";
            else
                mQry = mQry + " AND PL.SaleOrderLineId = @TextHidden ";

            IEnumerable<SaleOrderInventoryStatus_ShipViewModel> SaleOrderInventoryStatusList = db.Database.SqlQuery<SaleOrderInventoryStatus_ShipViewModel>(mQry, SqlParameterTextHidden, SqlParameterStatusOnDate, SqlParameterDocTypeId, SqlParameterSite, SqlParameterDivision, SqlParameterFromDate, SqlParameterToDate, SqlParameterBuyer, SqlParameterSaleOrderHeaderId, SqlParameterProduct, SqlParameterProductCategory, SqlParameterProductQuality, SqlParameterProductGroup, SqlParameterProductSize, SqlParameterReportFor, SqlParameterBuyerDesign).ToList();


            return SaleOrderInventoryStatusList;

        }

        public IEnumerable<SaleOrderInventoryStatus_ToBeIssueViewModel> ToBeIssueDetail(SaleOrderInventoryStatusDisplayFilterSettings Settings)
        {
            var StatusOnDateSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "StatusOnDate" select H).FirstOrDefault();
            var DocTypeIdSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "DocTypeId" select H).FirstOrDefault();
            var SiteSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "Site" select H).FirstOrDefault();
            var DivisionSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "Division" select H).FirstOrDefault();
            var FromDateSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "FromDate" select H).FirstOrDefault();
            var ToDateSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ToDate" select H).FirstOrDefault();
            var BuyerSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "Buyer" select H).FirstOrDefault();
            var SaleOrderHeaderIdSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "SaleOrderHeaderId" select H).FirstOrDefault();
            var ProductSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "Product" select H).FirstOrDefault();
            var ProductCategorySetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ProductCategory" select H).FirstOrDefault();
            var ProductQualitySetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ProductQuality" select H).FirstOrDefault();
            var ProductGroupSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ProductGroup" select H).FirstOrDefault();
            var ProductSizeSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ProductSize" select H).FirstOrDefault();
            var ReportTypeSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ReportType" select H).FirstOrDefault();
            var ReportForSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ReportFor" select H).FirstOrDefault();
            var NextFormatSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "NextFormat" select H).FirstOrDefault();
            var BuyerDesignSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "BuyerDesign" select H).FirstOrDefault();
            var TextHiddenSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "TextHidden" select H).FirstOrDefault();



            string StatusOnDate = StatusOnDateSetting.Value;
            string DocTypeId = DocTypeIdSetting.Value;
            //string Site = SiteSetting.Value;
            //string Division = DivisionSetting.Value;
            string Site = "1";
            string Division = "1";
            string FromDate = FromDateSetting.Value;
            string ToDate = ToDateSetting.Value;
            string Buyer = BuyerSetting.Value;
            string SaleOrderHeaderId = SaleOrderHeaderIdSetting.Value;
            string Product = ProductSetting.Value;
            string ProductCategory = ProductCategorySetting.Value;
            string ProductQuality = ProductQualitySetting.Value;
            string ProductGroup = ProductGroupSetting.Value;
            string ProductSize = ProductSizeSetting.Value;
            string ReportType = ReportTypeSetting.Value;
            string ReportFor = ReportForSetting.Value;
            string NextFormat = NextFormatSetting.Value;
            string BuyerDesign = BuyerDesignSetting.Value;
            string TextHidden = TextHiddenSetting.Value;


            string mQry;

            if (Buyer == "-1")
                Buyer = null;

            SqlParameter SqlParameterStatusOnDate = new SqlParameter("@StatusOnDate", !string.IsNullOrEmpty(StatusOnDate) ? StatusOnDate : (object)DBNull.Value);
            SqlParameter SqlParameterDocTypeId = new SqlParameter("@DocTypeId", !string.IsNullOrEmpty(DocTypeId) ? DocTypeId : (object)DBNull.Value);
            SqlParameter SqlParameterSite = new SqlParameter("@Site", !string.IsNullOrEmpty(Site) ? Site : (object)DBNull.Value);
            SqlParameter SqlParameterDivision = new SqlParameter("@Division", !string.IsNullOrEmpty(Division) ? Division : (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", !string.IsNullOrEmpty(FromDate) ? FromDate : (object)DBNull.Value);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", !string.IsNullOrEmpty(ToDate) ? ToDate : (object)DBNull.Value);
            SqlParameter SqlParameterBuyer = new SqlParameter("@Buyer", !string.IsNullOrEmpty(Buyer) ? Buyer : (object)DBNull.Value);
            SqlParameter SqlParameterSaleOrderHeaderId = new SqlParameter("@SaleOrderHeaderId", !string.IsNullOrEmpty(SaleOrderHeaderId) ? SaleOrderHeaderId : (object)DBNull.Value);
            SqlParameter SqlParameterProduct = new SqlParameter("@Product", !string.IsNullOrEmpty(Product) ? Product : (object)DBNull.Value);
            SqlParameter SqlParameterProductCategory = new SqlParameter("@ProductCategory", !string.IsNullOrEmpty(ProductCategory) ? ProductCategory : (object)DBNull.Value);
            SqlParameter SqlParameterProductQuality = new SqlParameter("@ProductQuality", !string.IsNullOrEmpty(ProductQuality) ? ProductQuality : (object)DBNull.Value);
            SqlParameter SqlParameterProductGroup = new SqlParameter("@ProductGroup", !string.IsNullOrEmpty(ProductGroup) ? ProductGroup : (object)DBNull.Value);
            SqlParameter SqlParameterProductSize = new SqlParameter("@ProductSize", !string.IsNullOrEmpty(ProductSize) ? ProductSize : (object)DBNull.Value);
            SqlParameter SqlParameterReportType = new SqlParameter("@ReportType", !string.IsNullOrEmpty(ReportType) ? ReportType : (object)DBNull.Value);
            SqlParameter SqlParameterReportFor = new SqlParameter("@ReportFor", !string.IsNullOrEmpty(ReportFor) ? ReportFor : (object)DBNull.Value);
            SqlParameter SqlParameterNextFormat = new SqlParameter("@NextFormat", !string.IsNullOrEmpty(NextFormat) ? NextFormat : (object)DBNull.Value);
            SqlParameter SqlParameterBuyerDesign = new SqlParameter("@BuyerDesign", !string.IsNullOrEmpty(BuyerDesign) ? BuyerDesign : (object)DBNull.Value);
            SqlParameter SqlParameterTextHidden = new SqlParameter("@TextHidden", !string.IsNullOrEmpty(TextHidden) ? TextHidden : (object)DBNull.Value);


            mQry = @"SELECT S.SiteCode AS Branch, Replace(convert(NVARCHAR,POH.DocDate,106),' ','/') AS Slip_Date, JOH.DocNo AS Slip_No,  PQ.ProductQualityName AS Quality, PG.ProductGroupName AS Design, C.ColourName AS Colour, VRS.ManufaturingSizeName AS Size,
                    H.Qty, H.ORdQty AS Iss,isnull((H.BalanceQty),0) AS UX
                    FROM Web.FProdOrderBalance_OneDocumentType_InBrabch(getdate(),273) H
                    LEFT JOIN web.Sites S ON S.SiteId = H.SiteId
                    LEFT JOIN web.ProdOrderLines POL WITH (Nolock) ON POL.ProdOrderLineId = H.ProdOrderLineId
                    LEFT JOIN web.JobOrderLines JOL WITH (Nolock) ON JOL.JobOrderLineId = POL.ReferenceDocLineId 
                    LEFT JOIN web.JobOrderHeaders JOH WITH (Nolock) ON JOH.JobOrderHeaderId = JOL.JobOrderHeaderId 
                    LEFT JOIN web.ProdOrderLines POL1 WITH (Nolock) ON POL1.ProdOrderLineId = JOL.ProdOrderLineId 
                    LEFT JOIN web.ProdOrderHeaders POH WITH (Nolock) ON  POH.ProdOrderHeaderId = POL.ProdOrderHeaderId 
                    LEFT JOIN web.Products P WITH (Nolock) ON P.ProductId = POL.ProductId 
                    LEFT JOIN web.ProductGroups PG WITH (Nolock) ON PG.ProductGroupId = P.ProductGroupId
                    LEFT JOIN web.FinishedProduct FP WITH (Nolock) ON FP.ProductId = P.ProductId 
                    LEFT JOIN web.Colours C WITH (Nolock) ON C.ColourId = FP.ColourId 
                    LEFT JOIN web.ViewRugSize VRS WITH (Nolock) ON VRS.ProductId = P.ProductId 
                    LEFT JOIN web.ProductCategories PC WITH (Nolock) ON PC.ProductCategoryId = P.ProductCategoryId 
                    LEFT JOIN web.ProductQualities PQ WITH (Nolock) ON PQ.ProductQualityId = FP.ProductQualityId 
                    LEFT JOIN web.MaterialPlanForSaleOrders MOS WITH (Nolock) ON MOS.MaterialPlanLineId = POL1.MaterialPlanLineId  
                    LEFT JOIN web.SaleOrderLines SOL WITH (Nolock) ON SOL.SaleOrderLineId = MOS.SaleOrderLineId
                    LEFT JOIN web.SaleOrderHeaders SOH WITH (Nolock) ON SOH.SaleOrderHeaderId = SOL.SaleOrderHeaderId
                    WHERE 1=1 " +
                    (DocTypeId != null ? " AND SOH.DocTypeId IN (SELECT Items FROM[dbo].[Split](@DocTypeId, ','))" : "") +
                  (Product != null ? " AND P.ProductId IN (SELECT Items FROM [dbo].[Split] (@Product, ','))" : "") +
                  (ProductQuality != null ? " AND Fp.ProductQualityId IN (SELECT Items FROM  Web.[Split] (@ProductQuality, ','))" : "") +
                  (ProductCategory != null ? " AND P.ProductCategoryId  IN (SELECT Items FROM  Web.[Split] (@ProductCategory, ','))" : "") +
                  (ProductGroup != null ? " AND P.ProductGroupId IN (SELECT Items FROM  Web.[Split] (@ProductGroup, ','))" : "") +
                  (SaleOrderHeaderId != null ? " AND SOH.SaleOrderHeaderId IN (SELECT Items FROM [dbo].[Split] (@SaleOrderHeaderId, ','))" : "") +
                  (Buyer != null ? " AND SOH.SaleToBuyerId  IN (SELECT Items FROM [dbo].[Split] (@Buyer, ','))" : "");

            if (ReportTypeSetting.Value == "Summary")
                mQry = mQry + " AND H.ProductId = @TextHidden ";
            else
                mQry = mQry + " AND MOS.SaleOrderLineId = @TextHidden ";


            IEnumerable<SaleOrderInventoryStatus_ToBeIssueViewModel> SaleOrderInventoryStatusList = db.Database.SqlQuery<SaleOrderInventoryStatus_ToBeIssueViewModel>(mQry, SqlParameterTextHidden, SqlParameterStatusOnDate, SqlParameterDocTypeId, SqlParameterSite, SqlParameterDivision, SqlParameterFromDate, SqlParameterToDate, SqlParameterBuyer, SqlParameterSaleOrderHeaderId, SqlParameterProduct, SqlParameterProductCategory, SqlParameterProductQuality, SqlParameterProductGroup, SqlParameterProductSize, SqlParameterReportFor, SqlParameterBuyerDesign).ToList();


            return SaleOrderInventoryStatusList;

        }

        public IEnumerable<SaleOrderInventoryStatus_DOViewModel> DODetail(SaleOrderInventoryStatusDisplayFilterSettings Settings)
        {
            var StatusOnDateSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "StatusOnDate" select H).FirstOrDefault();
            var DocTypeIdSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "DocTypeId" select H).FirstOrDefault();
            var SiteSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "Site" select H).FirstOrDefault();
            var DivisionSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "Division" select H).FirstOrDefault();
            var FromDateSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "FromDate" select H).FirstOrDefault();
            var ToDateSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ToDate" select H).FirstOrDefault();
            var BuyerSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "Buyer" select H).FirstOrDefault();
            var SaleOrderHeaderIdSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "SaleOrderHeaderId" select H).FirstOrDefault();
            var ProductSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "Product" select H).FirstOrDefault();
            var ProductCategorySetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ProductCategory" select H).FirstOrDefault();
            var ProductQualitySetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ProductQuality" select H).FirstOrDefault();
            var ProductGroupSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ProductGroup" select H).FirstOrDefault();
            var ProductSizeSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ProductSize" select H).FirstOrDefault();
            var ReportTypeSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ReportType" select H).FirstOrDefault();
            var ReportForSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ReportFor" select H).FirstOrDefault();
            var NextFormatSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "NextFormat" select H).FirstOrDefault();
            var BuyerDesignSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "BuyerDesign" select H).FirstOrDefault();
            var TextHiddenSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "TextHidden" select H).FirstOrDefault();



            string StatusOnDate = StatusOnDateSetting.Value;
            string DocTypeId = DocTypeIdSetting.Value;
            //string Site = SiteSetting.Value;
            //string Division = DivisionSetting.Value;
            string Site = "1";
            string Division = "1";
            string FromDate = FromDateSetting.Value;
            string ToDate = ToDateSetting.Value;
            string Buyer = BuyerSetting.Value;
            string SaleOrderHeaderId = SaleOrderHeaderIdSetting.Value;
            string Product = ProductSetting.Value;
            string ProductCategory = ProductCategorySetting.Value;
            string ProductQuality = ProductQualitySetting.Value;
            string ProductGroup = ProductGroupSetting.Value;
            string ProductSize = ProductSizeSetting.Value;
            string ReportType = ReportTypeSetting.Value;
            string ReportFor = ReportForSetting.Value;
            string NextFormat = NextFormatSetting.Value;
            string BuyerDesign = BuyerDesignSetting.Value;
            string TextHidden = TextHiddenSetting.Value;


            string mQry;

            if (Buyer == "-1")
                Buyer = null;

            SqlParameter SqlParameterStatusOnDate = new SqlParameter("@StatusOnDate", !string.IsNullOrEmpty(StatusOnDate) ? StatusOnDate : (object)DBNull.Value);
            SqlParameter SqlParameterDocTypeId = new SqlParameter("@DocTypeId", !string.IsNullOrEmpty(DocTypeId) ? DocTypeId : (object)DBNull.Value);
            SqlParameter SqlParameterSite = new SqlParameter("@Site", !string.IsNullOrEmpty(Site) ? Site : (object)DBNull.Value);
            SqlParameter SqlParameterDivision = new SqlParameter("@Division", !string.IsNullOrEmpty(Division) ? Division : (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", !string.IsNullOrEmpty(FromDate) ? FromDate : (object)DBNull.Value);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", !string.IsNullOrEmpty(ToDate) ? ToDate : (object)DBNull.Value);
            SqlParameter SqlParameterBuyer = new SqlParameter("@Buyer", !string.IsNullOrEmpty(Buyer) ? Buyer : (object)DBNull.Value);
            SqlParameter SqlParameterSaleOrderHeaderId = new SqlParameter("@SaleOrderHeaderId", !string.IsNullOrEmpty(SaleOrderHeaderId) ? SaleOrderHeaderId : (object)DBNull.Value);
            SqlParameter SqlParameterProduct = new SqlParameter("@Product", !string.IsNullOrEmpty(Product) ? Product : (object)DBNull.Value);
            SqlParameter SqlParameterProductCategory = new SqlParameter("@ProductCategory", !string.IsNullOrEmpty(ProductCategory) ? ProductCategory : (object)DBNull.Value);
            SqlParameter SqlParameterProductQuality = new SqlParameter("@ProductQuality", !string.IsNullOrEmpty(ProductQuality) ? ProductQuality : (object)DBNull.Value);
            SqlParameter SqlParameterProductGroup = new SqlParameter("@ProductGroup", !string.IsNullOrEmpty(ProductGroup) ? ProductGroup : (object)DBNull.Value);
            SqlParameter SqlParameterProductSize = new SqlParameter("@ProductSize", !string.IsNullOrEmpty(ProductSize) ? ProductSize : (object)DBNull.Value);
            SqlParameter SqlParameterReportType = new SqlParameter("@ReportType", !string.IsNullOrEmpty(ReportType) ? ReportType : (object)DBNull.Value);
            SqlParameter SqlParameterReportFor = new SqlParameter("@ReportFor", !string.IsNullOrEmpty(ReportFor) ? ReportFor : (object)DBNull.Value);
            SqlParameter SqlParameterNextFormat = new SqlParameter("@NextFormat", !string.IsNullOrEmpty(NextFormat) ? NextFormat : (object)DBNull.Value);
            SqlParameter SqlParameterBuyerDesign = new SqlParameter("@BuyerDesign", !string.IsNullOrEmpty(BuyerDesign) ? BuyerDesign : (object)DBNull.Value);
            SqlParameter SqlParameterTextHidden = new SqlParameter("@TextHidden", !string.IsNullOrEmpty(TextHidden) ? TextHidden : (object)DBNull.Value);


            mQry = @"SELECT H.DocNo AS Invoice_No, Replace(Convert(NVARCHAR,H.DocDate,106),' ','/') AS Date, PU.ProductUidName AS CarpetNo,   SOH.DocNo AS Order_No, PC.ProductCategoryName AS Type,
                    PQ.ProductQualityName AS Quality, PG.ProductGroupName AS Design, C.ColourName AS Colour, VRS.ManufaturingSizeName AS Size, PL.Qty , B.Name AS Buyer 
                    FROM web.SaleInvoiceHeaders H WITH (Nolock)
                    LEFT JOIN web.SaleInvoiceLines L WITH (Nolock) ON L.SaleInvoiceHeaderId = H.SaleInvoiceHeaderId 
                    LEFT JOIN web.SaleDispatchLines SDL WITH (Nolock) ON SDL.SaleDispatchLineId = L.SaleDispatchLineId 
                    LEFT JOIN web.PackingLines PL WITH (Nolock) ON PL.PackingLineId = SDL.PackingLineId
                    LEFT JOIN web.ProductUids PU ON PU.ProductUIDId = PL.ProductUIDId OR PU.ProductUidName = PL.LotNo 
                    LEFT JOIN web.SaleOrderLines SOL1 WITH (Nolock) ON SOL1.SaleOrderLineId = PU.SaleOrderLineId
                    LEFT JOIN web.SaleOrderHeaders SOH1 WITH (Nolock) ON SOH1.SaleOrderHeaderId = SOL1.SaleOrderHeaderId
                    LEFT JOIN web.Products P WITH (Nolock) ON P.ProductId = PL.ProductId 
                    LEFT JOIN web.ProductGroups PG WITH (Nolock) ON PG.ProductGroupId = P.ProductGroupId
                    LEFT JOIN web.FinishedProduct FP WITH (Nolock) ON FP.ProductId = P.ProductId 
                    LEFT JOIN web.Colours C WITH (Nolock) ON C.ColourId = FP.ColourId 
                    LEFT JOIN web.ViewRugSize VRS WITH (Nolock) ON VRS.ProductId = P.ProductId 
                    LEFT JOIN web.ProductCategories PC WITH (Nolock) ON PC.ProductCategoryId = P.ProductCategoryId 
                    LEFT JOIN web.ProductQualities PQ WITH (Nolock) ON PQ.ProductQualityId = FP.ProductQualityId 
                    LEFT JOIN web.SaleOrderLines SOL WITH (Nolock) ON SOL.SaleOrderLineId = PL.SaleOrderLineId
                    LEFT JOIN web.SaleOrderHeaders SOH WITH (Nolock) ON SOH.SaleOrderHeaderId = SOL.SaleOrderHeaderId
                    LEFT JOIN web.People B WITH (Nolock) ON SOH.SaleToBuyerId = B.PersonID
                    Where 1=1 AND PU.SaleOrderLineId IS NOT NULL AND PU.SaleOrderLineId <> PL.SaleOrderLineId AND SOH1.SaleToBuyerId = H.SaleToBuyerId " +
                    (DocTypeId != null ? " AND SOH.DocTypeId IN (SELECT Items FROM[dbo].[Split](@DocTypeId, ','))" : "") +
                      (Product != null ? " AND P.ProductId IN (SELECT Items FROM [dbo].[Split] (@Product, ','))" : "") +
                      (ProductQuality != null ? " AND Fp.ProductQualityId IN (SELECT Items FROM  Web.[Split] (@ProductQuality, ','))" : "") +
                      (ProductCategory != null ? " AND P.ProductCategoryId  IN (SELECT Items FROM  Web.[Split] (@ProductCategory, ','))" : "") +
                      (ProductGroup != null ? " AND P.ProductGroupId IN (SELECT Items FROM  Web.[Split] (@ProductGroup, ','))" : "") +
                      (SaleOrderHeaderId != null ? " AND SOH.SaleOrderHeaderId IN (SELECT Items FROM [dbo].[Split] (@SaleOrderHeaderId, ','))" : "") ;


            if (ReportTypeSetting.Value == "Summary")
                mQry = mQry + " AND P.ProductId = @TextHidden ";
            else
                mQry = mQry + " AND PU.SaleOrderLineId = @TextHidden ";

            IEnumerable<SaleOrderInventoryStatus_DOViewModel> SaleOrderInventoryStatusList = db.Database.SqlQuery<SaleOrderInventoryStatus_DOViewModel>(mQry, SqlParameterTextHidden, SqlParameterStatusOnDate, SqlParameterDocTypeId, SqlParameterSite, SqlParameterDivision, SqlParameterFromDate, SqlParameterToDate, SqlParameterBuyer, SqlParameterSaleOrderHeaderId, SqlParameterProduct, SqlParameterProductCategory, SqlParameterProductQuality, SqlParameterProductGroup, SqlParameterProductSize, SqlParameterReportFor, SqlParameterBuyerDesign).ToList();


            return SaleOrderInventoryStatusList;

        }

        public IEnumerable<SaleOrderInventoryStatus_DOViewModel> DBDetail(SaleOrderInventoryStatusDisplayFilterSettings Settings)
        {
            var StatusOnDateSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "StatusOnDate" select H).FirstOrDefault();
            var DocTypeIdSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "DocTypeId" select H).FirstOrDefault();
            var SiteSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "Site" select H).FirstOrDefault();
            var DivisionSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "Division" select H).FirstOrDefault();
            var FromDateSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "FromDate" select H).FirstOrDefault();
            var ToDateSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ToDate" select H).FirstOrDefault();
            var BuyerSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "Buyer" select H).FirstOrDefault();
            var SaleOrderHeaderIdSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "SaleOrderHeaderId" select H).FirstOrDefault();
            var ProductSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "Product" select H).FirstOrDefault();
            var ProductCategorySetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ProductCategory" select H).FirstOrDefault();
            var ProductQualitySetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ProductQuality" select H).FirstOrDefault();
            var ProductGroupSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ProductGroup" select H).FirstOrDefault();
            var ProductSizeSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ProductSize" select H).FirstOrDefault();
            var ReportTypeSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ReportType" select H).FirstOrDefault();
            var ReportForSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ReportFor" select H).FirstOrDefault();
            var NextFormatSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "NextFormat" select H).FirstOrDefault();
            var BuyerDesignSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "BuyerDesign" select H).FirstOrDefault();
            var TextHiddenSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "TextHidden" select H).FirstOrDefault();



            string StatusOnDate = StatusOnDateSetting.Value;
            string DocTypeId = DocTypeIdSetting.Value;
            //string Site = SiteSetting.Value;
            //string Division = DivisionSetting.Value;
            string Site = "1";
            string Division = "1";
            string FromDate = FromDateSetting.Value;
            string ToDate = ToDateSetting.Value;
            string Buyer = BuyerSetting.Value;
            string SaleOrderHeaderId = SaleOrderHeaderIdSetting.Value;
            string Product = ProductSetting.Value;
            string ProductCategory = ProductCategorySetting.Value;
            string ProductQuality = ProductQualitySetting.Value;
            string ProductGroup = ProductGroupSetting.Value;
            string ProductSize = ProductSizeSetting.Value;
            string ReportType = ReportTypeSetting.Value;
            string ReportFor = ReportForSetting.Value;
            string NextFormat = NextFormatSetting.Value;
            string BuyerDesign = BuyerDesignSetting.Value;
            string TextHidden = TextHiddenSetting.Value;


            string mQry;


            SqlParameter SqlParameterStatusOnDate = new SqlParameter("@StatusOnDate", !string.IsNullOrEmpty(StatusOnDate) ? StatusOnDate : (object)DBNull.Value);
            SqlParameter SqlParameterDocTypeId = new SqlParameter("@DocTypeId", !string.IsNullOrEmpty(DocTypeId) ? DocTypeId : (object)DBNull.Value);
            SqlParameter SqlParameterSite = new SqlParameter("@Site", !string.IsNullOrEmpty(Site) ? Site : (object)DBNull.Value);
            SqlParameter SqlParameterDivision = new SqlParameter("@Division", !string.IsNullOrEmpty(Division) ? Division : (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", !string.IsNullOrEmpty(FromDate) ? FromDate : (object)DBNull.Value);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", !string.IsNullOrEmpty(ToDate) ? ToDate : (object)DBNull.Value);
            SqlParameter SqlParameterBuyer = new SqlParameter("@Buyer", !string.IsNullOrEmpty(Buyer) ? Buyer : (object)DBNull.Value);
            SqlParameter SqlParameterSaleOrderHeaderId = new SqlParameter("@SaleOrderHeaderId", !string.IsNullOrEmpty(SaleOrderHeaderId) ? SaleOrderHeaderId : (object)DBNull.Value);
            SqlParameter SqlParameterProduct = new SqlParameter("@Product", !string.IsNullOrEmpty(Product) ? Product : (object)DBNull.Value);
            SqlParameter SqlParameterProductCategory = new SqlParameter("@ProductCategory", !string.IsNullOrEmpty(ProductCategory) ? ProductCategory : (object)DBNull.Value);
            SqlParameter SqlParameterProductQuality = new SqlParameter("@ProductQuality", !string.IsNullOrEmpty(ProductQuality) ? ProductQuality : (object)DBNull.Value);
            SqlParameter SqlParameterProductGroup = new SqlParameter("@ProductGroup", !string.IsNullOrEmpty(ProductGroup) ? ProductGroup : (object)DBNull.Value);
            SqlParameter SqlParameterProductSize = new SqlParameter("@ProductSize", !string.IsNullOrEmpty(ProductSize) ? ProductSize : (object)DBNull.Value);
            SqlParameter SqlParameterReportType = new SqlParameter("@ReportType", !string.IsNullOrEmpty(ReportType) ? ReportType : (object)DBNull.Value);
            SqlParameter SqlParameterReportFor = new SqlParameter("@ReportFor", !string.IsNullOrEmpty(ReportFor) ? ReportFor : (object)DBNull.Value);
            SqlParameter SqlParameterNextFormat = new SqlParameter("@NextFormat", !string.IsNullOrEmpty(NextFormat) ? NextFormat : (object)DBNull.Value);
            SqlParameter SqlParameterBuyerDesign = new SqlParameter("@BuyerDesign", !string.IsNullOrEmpty(BuyerDesign) ? BuyerDesign : (object)DBNull.Value);
            SqlParameter SqlParameterTextHidden = new SqlParameter("@TextHidden", !string.IsNullOrEmpty(TextHidden) ? TextHidden : (object)DBNull.Value);


            mQry = @"SELECT H.DocNo AS Invoice_No, Replace(Convert(NVARCHAR,H.DocDate,106),' ','/') AS Date, PU.ProductUidName AS CarpetNo,   SOH.DocNo AS Order_No, PC.ProductCategoryName AS Type,
                    PQ.ProductQualityName AS Quality, PG.ProductGroupName AS Design, C.ColourName AS Colour, VRS.ManufaturingSizeName AS Size, PL.Qty , B.Name AS Buyer 
                    FROM web.SaleInvoiceHeaders H WITH (Nolock)
                    LEFT JOIN web.SaleInvoiceLines L WITH (Nolock) ON L.SaleInvoiceHeaderId = H.SaleInvoiceHeaderId 
                    LEFT JOIN web.SaleDispatchLines SDL WITH (Nolock) ON SDL.SaleDispatchLineId = L.SaleDispatchLineId 
                    LEFT JOIN web.PackingLines PL WITH (Nolock) ON PL.PackingLineId = SDL.PackingLineId
                    LEFT JOIN web.ProductUids PU ON PU.ProductUIDId = PL.ProductUIDId OR PU.ProductUidName = PL.LotNo 
                    LEFT JOIN web.SaleOrderLines SOL1 WITH (Nolock) ON SOL1.SaleOrderLineId = PU.SaleOrderLineId
                    LEFT JOIN web.SaleOrderHeaders SOH1 WITH (Nolock) ON SOH1.SaleOrderHeaderId = SOL1.SaleOrderHeaderId
                    LEFT JOIN web.Products P WITH (Nolock) ON P.ProductId = PL.ProductId 
                    LEFT JOIN web.ProductGroups PG WITH (Nolock) ON PG.ProductGroupId = P.ProductGroupId
                    LEFT JOIN web.FinishedProduct FP WITH (Nolock) ON FP.ProductId = P.ProductId 
                    LEFT JOIN web.Colours C WITH (Nolock) ON C.ColourId = FP.ColourId 
                    LEFT JOIN web.ViewRugSize VRS WITH (Nolock) ON VRS.ProductId = P.ProductId 
                    LEFT JOIN web.ProductCategories PC WITH (Nolock) ON PC.ProductCategoryId = P.ProductCategoryId 
                    LEFT JOIN web.ProductQualities PQ WITH (Nolock) ON PQ.ProductQualityId = FP.ProductQualityId 
                    LEFT JOIN web.SaleOrderLines SOL WITH (Nolock) ON SOL.SaleOrderLineId = PL.SaleOrderLineId
                    LEFT JOIN web.SaleOrderHeaders SOH WITH (Nolock) ON SOH.SaleOrderHeaderId = SOL.SaleOrderHeaderId
                    LEFT JOIN web.People B WITH (Nolock) ON SOH.SaleToBuyerId = B.PersonID
                    Where 1=1 AND PU.SaleOrderLineId IS NOT NULL AND SOH1.SaleToBuyerId <> H.SaleToBuyerId " +
                    (DocTypeId != null ? " AND SOH.DocTypeId IN (SELECT Items FROM[dbo].[Split](@DocTypeId, ','))" : "") +
                      (Product != null ? " AND P.ProductId IN (SELECT Items FROM [dbo].[Split] (@Product, ','))" : "") +
                      (ProductQuality != null ? " AND Fp.ProductQualityId IN (SELECT Items FROM  Web.[Split] (@ProductQuality, ','))" : "") +
                      (ProductCategory != null ? " AND P.ProductCategoryId  IN (SELECT Items FROM  Web.[Split] (@ProductCategory, ','))" : "") +
                      (ProductGroup != null ? " AND P.ProductGroupId IN (SELECT Items FROM  Web.[Split] (@ProductGroup, ','))" : "") +
                      (SaleOrderHeaderId != null ? " AND SOH.SaleOrderHeaderId IN (SELECT Items FROM [dbo].[Split] (@SaleOrderHeaderId, ','))" : "") ;


            if (ReportTypeSetting.Value == "Summary")
                mQry = mQry + " AND P.ProductId = @TextHidden ";
            else
                mQry = mQry + " AND PU.SaleOrderLineId = @TextHidden ";

            IEnumerable<SaleOrderInventoryStatus_DOViewModel> SaleOrderInventoryStatusList = db.Database.SqlQuery<SaleOrderInventoryStatus_DOViewModel>(mQry, SqlParameterTextHidden, SqlParameterStatusOnDate, SqlParameterDocTypeId, SqlParameterSite, SqlParameterDivision, SqlParameterFromDate, SqlParameterToDate, SqlParameterBuyer, SqlParameterSaleOrderHeaderId, SqlParameterProduct, SqlParameterProductCategory, SqlParameterProductQuality, SqlParameterProductGroup, SqlParameterProductSize, SqlParameterReportFor, SqlParameterBuyerDesign).ToList();


            return SaleOrderInventoryStatusList;

        }

        public IEnumerable<SaleOrderInventoryStatus_OXViewModel> OXDetail(SaleOrderInventoryStatusDisplayFilterSettings Settings)
        {
            var StatusOnDateSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "StatusOnDate" select H).FirstOrDefault();
            var DocTypeIdSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "DocTypeId" select H).FirstOrDefault();
            var SiteSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "Site" select H).FirstOrDefault();
            var DivisionSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "Division" select H).FirstOrDefault();
            var FromDateSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "FromDate" select H).FirstOrDefault();
            var ToDateSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ToDate" select H).FirstOrDefault();
            var BuyerSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "Buyer" select H).FirstOrDefault();
            var SaleOrderHeaderIdSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "SaleOrderHeaderId" select H).FirstOrDefault();
            var ProductSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "Product" select H).FirstOrDefault();
            var ProductCategorySetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ProductCategory" select H).FirstOrDefault();
            var ProductQualitySetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ProductQuality" select H).FirstOrDefault();
            var ProductGroupSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ProductGroup" select H).FirstOrDefault();
            var ProductSizeSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ProductSize" select H).FirstOrDefault();
            var ReportTypeSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ReportType" select H).FirstOrDefault();
            var ReportForSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "ReportFor" select H).FirstOrDefault();
            var NextFormatSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "NextFormat" select H).FirstOrDefault();
            var BuyerDesignSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "BuyerDesign" select H).FirstOrDefault();
            var TextHiddenSetting = (from H in Settings.SaleOrderInventoryStatusDisplayFilterParameters where H.ParameterName == "TextHidden" select H).FirstOrDefault();



            string StatusOnDate = StatusOnDateSetting.Value;
            string DocTypeId = DocTypeIdSetting.Value;
            //string Site = SiteSetting.Value;
            //string Division = DivisionSetting.Value;
            string Site = "1";
            string Division = "1";
            string FromDate = FromDateSetting.Value;
            string ToDate = ToDateSetting.Value;
            string Buyer = BuyerSetting.Value;
            string SaleOrderHeaderId = SaleOrderHeaderIdSetting.Value;
            string Product = ProductSetting.Value;
            string ProductCategory = ProductCategorySetting.Value;
            string ProductQuality = ProductQualitySetting.Value;
            string ProductGroup = ProductGroupSetting.Value;
            string ProductSize = ProductSizeSetting.Value;
            string ReportType = ReportTypeSetting.Value;
            string ReportFor = ReportForSetting.Value;
            string NextFormat = NextFormatSetting.Value;
            string BuyerDesign = BuyerDesignSetting.Value;
            string TextHidden = TextHiddenSetting.Value;


            string mQry;

            if (Buyer == "-1")
                Buyer = null;

            SqlParameter SqlParameterStatusOnDate = new SqlParameter("@StatusOnDate", !string.IsNullOrEmpty(StatusOnDate) ? StatusOnDate : (object)DBNull.Value);
            SqlParameter SqlParameterDocTypeId = new SqlParameter("@DocTypeId", !string.IsNullOrEmpty(DocTypeId) ? DocTypeId : (object)DBNull.Value);
            SqlParameter SqlParameterSite = new SqlParameter("@Site", !string.IsNullOrEmpty(Site) ? Site : (object)DBNull.Value);
            SqlParameter SqlParameterDivision = new SqlParameter("@Division", !string.IsNullOrEmpty(Division) ? Division : (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", !string.IsNullOrEmpty(FromDate) ? FromDate : (object)DBNull.Value);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", !string.IsNullOrEmpty(ToDate) ? ToDate : (object)DBNull.Value);
            SqlParameter SqlParameterBuyer = new SqlParameter("@Buyer", !string.IsNullOrEmpty(Buyer) ? Buyer : (object)DBNull.Value);
            SqlParameter SqlParameterSaleOrderHeaderId = new SqlParameter("@SaleOrderHeaderId", !string.IsNullOrEmpty(SaleOrderHeaderId) ? SaleOrderHeaderId : (object)DBNull.Value);
            SqlParameter SqlParameterProduct = new SqlParameter("@Product", !string.IsNullOrEmpty(Product) ? Product : (object)DBNull.Value);
            SqlParameter SqlParameterProductCategory = new SqlParameter("@ProductCategory", !string.IsNullOrEmpty(ProductCategory) ? ProductCategory : (object)DBNull.Value);
            SqlParameter SqlParameterProductQuality = new SqlParameter("@ProductQuality", !string.IsNullOrEmpty(ProductQuality) ? ProductQuality : (object)DBNull.Value);
            SqlParameter SqlParameterProductGroup = new SqlParameter("@ProductGroup", !string.IsNullOrEmpty(ProductGroup) ? ProductGroup : (object)DBNull.Value);
            SqlParameter SqlParameterProductSize = new SqlParameter("@ProductSize", !string.IsNullOrEmpty(ProductSize) ? ProductSize : (object)DBNull.Value);
            SqlParameter SqlParameterReportType = new SqlParameter("@ReportType", !string.IsNullOrEmpty(ReportType) ? ReportType : (object)DBNull.Value);
            SqlParameter SqlParameterReportFor = new SqlParameter("@ReportFor", !string.IsNullOrEmpty(ReportFor) ? ReportFor : (object)DBNull.Value);
            SqlParameter SqlParameterNextFormat = new SqlParameter("@NextFormat", !string.IsNullOrEmpty(NextFormat) ? NextFormat : (object)DBNull.Value);
            SqlParameter SqlParameterBuyerDesign = new SqlParameter("@BuyerDesign", !string.IsNullOrEmpty(BuyerDesign) ? BuyerDesign : (object)DBNull.Value);
            SqlParameter SqlParameterTextHidden = new SqlParameter("@TextHidden", !string.IsNullOrEmpty(TextHidden) ? TextHidden : (object)DBNull.Value);


            mQry = @"SELECT H.DocNo AS Invoice_No, Replace(Convert(NVARCHAR,H.DocDate,106),' ','/') AS Date, PU.ProductUidName AS CarpetNo,   PC.ProductCategoryName AS Type,
                    PQ.ProductQualityName AS Quality, PG.ProductGroupName AS Design, C.ColourName AS Colour, VRS.ManufaturingSizeName AS Size, PL.Qty, B.Name AS Buyer  
                    FROM web.SaleInvoiceHeaders H WITH (Nolock)
                    LEFT JOIN web.SaleInvoiceLines L WITH (Nolock) ON L.SaleInvoiceHeaderId = H.SaleInvoiceHeaderId 
                    LEFT JOIN web.SaleDispatchLines SDL WITH (Nolock) ON SDL.SaleDispatchLineId = L.SaleDispatchLineId 
                    LEFT JOIN web.PackingLines PL WITH (Nolock) ON PL.PackingLineId = SDL.PackingLineId
                    LEFT JOIN web.ProductUids PU ON PU.ProductUIDId = PL.ProductUIDId OR PU.ProductUidName = PL.LotNo 
                    LEFT JOIN web.Products P WITH (Nolock) ON P.ProductId = PL.ProductId 
                    LEFT JOIN web.ProductGroups PG WITH (Nolock) ON PG.ProductGroupId = P.ProductGroupId
                    LEFT JOIN web.FinishedProduct FP WITH (Nolock) ON FP.ProductId = P.ProductId 
                    LEFT JOIN web.Colours C WITH (Nolock) ON C.ColourId = FP.ColourId 
                    LEFT JOIN web.ViewRugSize VRS WITH (Nolock) ON VRS.ProductId = P.ProductId 
                    LEFT JOIN web.ProductCategories PC WITH (Nolock) ON PC.ProductCategoryId = P.ProductCategoryId 
                    LEFT JOIN web.ProductQualities PQ WITH (Nolock) ON PQ.ProductQualityId = FP.ProductQualityId 
                    LEFT JOIN web.SaleOrderLines SOL WITH (Nolock) ON SOL.SaleOrderLineId = PL.SaleOrderLineId
                    LEFT JOIN web.SaleOrderHeaders SOH WITH (Nolock) ON SOH.SaleOrderHeaderId = SOL.SaleOrderHeaderId
                    LEFT JOIN web.SaleOrderLines SOL1 WITH (Nolock) ON SOL1.SaleOrderLineId = PU.SaleOrderLineId
                    LEFT JOIN web.SaleOrderHeaders SOH1 WITH (Nolock) ON SOH1.SaleOrderHeaderId = SOL1.SaleOrderHeaderId
                    LEFT JOIN web.People B WITH (Nolock) ON SOH1.SaleToBuyerId = B.PersonID
                    Where 1=1 AND PU.ProductUidName IS NOT NULL AND PU.SaleOrderLineId <> PL.SaleOrderLineId " +
                    (DocTypeId != null ? " AND SOH.DocTypeId IN (SELECT Items FROM[dbo].[Split](@DocTypeId, ','))" : "") +
                      (Product != null ? " AND P.ProductId IN (SELECT Items FROM [dbo].[Split] (@Product, ','))" : "") +
                      (ProductQuality != null ? " AND Fp.ProductQualityId IN (SELECT Items FROM  Web.[Split] (@ProductQuality, ','))" : "") +
                      (ProductCategory != null ? " AND P.ProductCategoryId  IN (SELECT Items FROM  Web.[Split] (@ProductCategory, ','))" : "") +
                      (ProductGroup != null ? " AND P.ProductGroupId IN (SELECT Items FROM  Web.[Split] (@ProductGroup, ','))" : "") +
                      (SaleOrderHeaderId != null ? " AND SOH.SaleOrderHeaderId IN (SELECT Items FROM [dbo].[Split] (@SaleOrderHeaderId, ','))" : "") +
                      (Buyer != null ? " AND SOH.SaleToBuyerId  IN (SELECT Items FROM [dbo].[Split] (@Buyer, ','))" : "");


            if (ReportTypeSetting.Value == "Summary")
                mQry = mQry + " AND P.ProductId = @TextHidden ";
            else
                mQry = mQry + " AND PL.SaleOrderLineId = @TextHidden ";

            IEnumerable<SaleOrderInventoryStatus_OXViewModel> SaleOrderInventoryStatusList = db.Database.SqlQuery<SaleOrderInventoryStatus_OXViewModel>(mQry, SqlParameterTextHidden, SqlParameterStatusOnDate, SqlParameterDocTypeId, SqlParameterSite, SqlParameterDivision, SqlParameterFromDate, SqlParameterToDate, SqlParameterBuyer, SqlParameterSaleOrderHeaderId, SqlParameterProduct, SqlParameterProductCategory, SqlParameterProductQuality, SqlParameterProductGroup, SqlParameterProductSize, SqlParameterReportFor, SqlParameterBuyerDesign).ToList();


            return SaleOrderInventoryStatusList;

        }

        public void Dispose()
        {
        }
    }

    public class Display_SaleOrderInventoryStatusViewModel
    {
        public string StatusOnDate { get; set; }
        public string DocTypeId { get; set; }
        public string Site { get; set; }
        public string Division { get; set; }  
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Buyer { get; set; }
        public string SaleOrderHeaderId { get; set; }
        public string Product { get; set; }
        public string ProductCategory { get; set; }
        public string ProductQuality { get; set; }
        public string ProductGroup { get; set; }
        public string ProductSize { get; set; }
        public string ReportType { get; set; }
        public string ReportFor { get; set; }
        public string NextFormat { get; set; }
        public string BuyerDesign { get; set; }
        public string TextHidden { get; set; }
    }

    [Serializable()]
    public class SaleOrderInventoryStatusDisplayFilterSettings
    {
        public string Format { get; set; }
        public List<SaleOrderInventoryStatusDisplayFilterParameters> SaleOrderInventoryStatusDisplayFilterParameters { get; set; }
    }

    [Serializable()]
    public class SaleOrderInventoryStatusDisplayFilterParameters
    {
        public string ParameterName { get; set; }
        public bool IsApplicable { get; set; }
        public string Value { get; set; }
    }

    public class SaleOrderInventoryStatusViewModel
    {
        public int SaleOrderLineId { get; set; }
        public int ProductId { get; set; }
        public string Buyer { get; set; }
        public string Order_No { get; set; }
        public string Order_Date { get; set; }
        public string Delivery_Date { get; set; }
        public string Quality { get; set; }
        public string Design { get; set; }
        public string Size { get; set; }
        public string Colour { get; set; }
        public string ProductName { get; set; }
        public decimal? ORD { get; set; }
        public decimal? O_C { get; set; }
        public decimal? SLP { get; set; }
        public decimal? UX { get; set; }
        public decimal? LOOM { get; set; }
        public decimal? STK { get; set; }
        public decimal? SHP { get; set; }
        public decimal? BAL { get; set; }
        public decimal? D_O { get; set; }
        public decimal? D_B { get; set; }
        public decimal? O_X { get; set; } 

        //Summary
        //public string Quality { get; set; }
        //public string Design { get; set; }
        //public string Size { get; set; }
        //public string Colour { get; set; }
        //public string ProductName { get; set; }
        //public decimal? ORD { get; set; }
        //public decimal? O_C { get; set; }
        //public decimal? SLP { get; set; }
        //public decimal? LOOM { get; set; }
        //public decimal? STK { get; set; }
        //public decimal? SHP { get; set; }
        //public decimal? BAL { get; set; }
        //public decimal? D_O { get; set; }
        //public decimal? D_B { get; set; }
        //public decimal? O_X { get; set; }
    }



    public class SaleOrderInventoryStatus_StockViewModel
    {
        public string CarpetNo { get; set; }
        public string Date { get; set; }
        public string Type { get; set; }
        public string Quality { get; set; }
        public string Design { get; set; }
        public string Colour { get; set; }
        public string Size { get; set; }
        public decimal? Qty { get; set; }
        public decimal? OTI { get; set; }
        public decimal? OTR { get; set; }
        public string Status { get; set; }

    }

    public class SaleOrderInventoryStatus_LoomViewModel
    {
        public string Branch { get; set; }
        public string Purza_No { get; set; }
        public string Date { get; set; }
        public string Weaver { get; set; }
        public string Quality { get; set; }
        public string Design { get; set; }
        public string Colour { get; set; }
        public string Size { get; set; }
        public decimal? LoomQty { get; set; }

    }

    public class SaleOrderInventoryStatus_ShipViewModel
    {
        public string Invoice_No { get; set; }
        public string Date { get; set; }
        public string Roll_No { get; set; }
        public string Remark { get; set; }
        public string CarpetNo { get; set; }
        public string Type { get; set; }
        public string Quality { get; set; }
        public string Design { get; set; }
        public string Colour { get; set; }
        public string Size { get; set; }
        public decimal? Qty { get; set; }

    }

    public class SaleOrderInventoryStatus_ToBeIssueViewModel
    {
        public string Branch { get; set; }
        public string Slip_Date { get; set; }
        public string Slip_No { get; set; }        
        public string Quality { get; set; }
        public string Design { get; set; }
        public string Colour { get; set; }
        public string Size { get; set; }
        public decimal? Qty { get; set; }
        public decimal? Iss { get; set; }
        public decimal? UX { get; set; }

    }

    public class SaleOrderInventoryStatus_DOViewModel
    {
        public string Invoice_No { get; set; }
        public string Date { get; set; }
        public string CarpetNo { get; set; }
        public string Order_No { get; set; }
        public string Type { get; set; }
        public string Quality { get; set; }
        public string Design { get; set; }
        public string Colour { get; set; }
        public string Size { get; set; }
        public decimal? Qty { get; set; }
        public string Buyer { get; set; }

    }

    public class SaleOrderInventoryStatus_OXViewModel
    {
        public string Invoice_No { get; set; }
        public string Date { get; set; }
        public string CarpetNo { get; set; }      
        public string Type { get; set; }
        public string Quality { get; set; }
        public string Design { get; set; }
        public string Colour { get; set; }
        public string Size { get; set; }
        public decimal? Qty { get; set; }
        public string Buyer { get; set; }

    }

}

