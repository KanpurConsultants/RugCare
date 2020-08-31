using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Infrastructure;
using Model.Models;
using Model.ViewModel;
using Core.Common;
using System;
using Model;
using System.Threading.Tasks;
using Data.Models;
using System.Data.SqlClient;
using System.Configuration;

namespace Service
{
    public interface IRugDashBoardBCService : IDisposable
    {
        //1 Block
        IEnumerable<DashBoardDoubleValue> GetSaleOrderBalanceSummary();
        IEnumerable<DashBoardTabularData_ThreeColumns> GetSaleOrderBalanceDetailCategoryWise();
        IEnumerable<DashBoardTabularData_ThreeColumns> GetSaleOrderBalanceDetailQualityWise();
        IEnumerable<DashBoardTabularData_ThreeColumns> GetSaleOrderBalanceDetailBuyerWise();


        //2 Block
        IEnumerable<DashBoardDoubleValue> GetUnExecuteSummary();
        IEnumerable<DashBoardTabularData_ThreeColumns> GetUnExecuteDetailCategoryWise();
        IEnumerable<DashBoardTabularData_ThreeColumns> GetUnExecuteDetailBranchWise();
        IEnumerable<DashBoardTabularData_ThreeColumns> GetUnExecuteDetailBuyerWise();

        //3 Block
        IEnumerable<DashBoardTrippleValue> GetSaleInvoiceSummary();
        IEnumerable<DashBoardTabularData_ThreeColumns> GetSaleInvoiceDetailCategoryWise();
        IEnumerable<DashBoardTabularData_ThreeColumns> GetSaleInvoiceDetailQualityWise();
        IEnumerable<DashBoardTabularData_ThreeColumns> GetSaleInvoiceDetailBuyerWise();
        IEnumerable<DashBoardSaleBarChartData> GetSaleInvoiceBarChartData();

        //4 Block


        //5 Block
        IEnumerable<DashBoardDoubleValue> GetStockSummary();
        IEnumerable<DashBoardTabularData_ThreeColumns> GetStockDetailCategoryWise();
        IEnumerable<DashBoardTabularData_ThreeColumns> GetStockDetailQualityWise();
        IEnumerable<DashBoardTabularData_ThreeColumns> GetStockDetailBuyerWise();


        //6 Block
        IEnumerable<DashBoardDoubleValue> GetToBeIssueSummary();
        IEnumerable<DashBoardTabularData_ThreeColumns> GetToBeIssueDetailCategoryWise();
        IEnumerable<DashBoardTabularData_ThreeColumns> GetToBeIssueDetailQualityWise();
        IEnumerable<DashBoardTabularData_ThreeColumns> GetToBeIssueDetailBuyerWise();


        //7 Block
        IEnumerable<DashBoardTrippleValue> GetProductionSummary();
        IEnumerable<DashBoardTabularData_ThreeColumns> GetProductionDetailCategoryWise();
        IEnumerable<DashBoardTabularData_ThreeColumns> GetProductionDetailQualityWise();
        IEnumerable<DashBoardTabularData_ThreeColumns> GetProductionDetailBuyerWise();
        IEnumerable<DashBoardPieChartData> GetProductionPieChartData();

        //8 Block


        //9 Block
        IEnumerable<DashBoardDoubleValue> GetOnLoomSummary();
        IEnumerable<DashBoardTabularData_ThreeColumns> GetOnLoomDetailCategoryWise();
        IEnumerable<DashBoardTabularData_ThreeColumns> GetOnLoomDetailJobWorkerWise();
        IEnumerable<DashBoardTabularData_ThreeColumns> GetOnLoomDetailBuyerWise();


        //10 Block
        IEnumerable<DashBoardSingleValue> GetDyeingOrderBalanceSummary();
        IEnumerable<DashBoardTabularData> GetDyeingOrderBalanceDetailProductWise();
        IEnumerable<DashBoardTabularData> GetDyeingOrderBalanceDetailMonthWise();
        IEnumerable<DashBoardTabularData> GetDyeingOrderBalanceDetailJobWorkerWise();


        //11 Block



        //12 Block
        IEnumerable<DashBoardSingleValue> GetLoanBalanceSummary();
        IEnumerable<DashBoardTabularData> GetLoanBalanceDetailDepartmentWise();
        IEnumerable<DashBoardTabularData> GetLoanBalanceDetailMonthWise();
        IEnumerable<DashBoardTabularData> GetLoanBalanceDetailLedgerAccountWise();

        //IEnumerable<DashBoardTrippleValue> GetSaleOrderStatus();



        //IEnumerable<DashBoardSingleValue> GetExpense();
        //IEnumerable<DashBoardSingleValue> GetDebtors();
        //IEnumerable<DashBoardSingleValue> GetCreditors();
        //IEnumerable<DashBoardSingleValue> GetBankBalance();
        //IEnumerable<DashBoardSingleValue> GetCashBalance();


        //IEnumerable<DashBoardDoubleValue> GetWorkshopSale();
        //IEnumerable<DashBoardDoubleValue> GetSpareSale();









        //IEnumerable<DashBoardTabularData> GetDebtorsDetail();

        //IEnumerable<DashBoardTabularData> GetBankBalanceDetailBankAc();
        //IEnumerable<DashBoardTabularData> GetBankBalanceDetailBankODAc();
        //IEnumerable<DashBoardTabularData> GetBankBalanceDetailChannelFinanceAc();


        //IEnumerable<DashBoardTabularData> GetExpenseDetailLedgerAccountWise();
        //IEnumerable<DashBoardTabularData> GetExpenseDetailBranchWise();
        //IEnumerable<DashBoardTabularData> GetExpenseDetailCostCenterWise();

        //IEnumerable<DashBoardTabularData> GetCreditorsDetail();

        //IEnumerable<DashBoardTabularData> GetCashBalanceDetailLedgerAccountWise();
        //IEnumerable<DashBoardTabularData> GetCashBalanceDetailBranchWise();


        //IEnumerable<DashBoardTabularData> GetWorkshopSaleDetailProductTypeWise();
        //IEnumerable<DashBoardTabularData> GetWorkshopSaleDetailProductGroupWise();

        //IEnumerable<DashBoardTabularData> GetSpareSaleDetailProductTypeWise();
        //IEnumerable<DashBoardTabularData> GetSpareSaleDetailProductGroupWise();



        //IEnumerable<DashBoardPieChartData> GetSpareSalePieChartData();
        //IEnumerable<DashBoardSaleBarChartData> GetSpareSaleBarChartData();
    }

    public class RugDashBoardBCService : IRugDashBoardBCService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        string mQry = "";
        private readonly IUnitOfWorkForService _unitOfWork;

        DateTime? MonthStartDate = null;
        DateTime? MonthEndDate = null;
        DateTime? YearStartDate = null;
        DateTime? YearEndDate = null;
        DateTime? SoftwareStartDate = null;
        DateTime? TodayDate = null;


        public RugDashBoardBCService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            Init();
        }
        public RugDashBoardBCService()
        {
            Init();
        }

        private void Init()
        {
            mQry = @"DECLARE @Month INT 
                    DECLARE @Year INT
                    SELECT @Month =  Datepart(MONTH,getdate())
                    SELECT @Year =  Datepart(YEAR,getdate())
                    DECLARE @FromDate DATETIME
                    DECLARE @ToDate DATETIME
                    SELECT DATEADD(month,@Month-1,DATEADD(year,@Year-1900,0)) As MonthStartDate, 
                    DATEADD(day,-1,DATEADD(month,@Month,DATEADD(year,@Year-1900,0))) As MonthEndDate,
                    CASE WHEN DATEPART(MM,GETDATE()) < 4 THEN DATEADD(MONTH,-9,DATEADD(DD,-DATEPART(DY,GETDATE())+1,GETDATE()))
                    ELSE DATEADD(MONTH,3,DATEADD(DD,-DATEPART(DY,GETDATE())+1,GETDATE())) END AS YearStartDate,
                    CASE WHEN DATEPART(MM,GETDATE()) < 4 THEN DATEADD(MONTH,-9,DATEADD(DD,-1,DATEADD(YY,DATEDIFF(YY,0,GETDATE())+1,0)))
                    ELSE DATEADD(MONTH,3,DATEADD(DD,-1,DATEADD(YY,DATEDIFF(YY,0,GETDATE())+1,0))) END AS YearEndDate,
                    Convert(DATETIME,'01/Apr/2001') AS SoftwareStartDate,
                    GETDATE() As TodayDate ";
            SessnionValues SessnionValues = db.Database.SqlQuery<SessnionValues>(mQry).FirstOrDefault();

            MonthStartDate = SessnionValues.MonthStartDate;
            MonthEndDate = SessnionValues.MonthEndDate;
            YearStartDate = SessnionValues.YearStartDate;
            YearEndDate = SessnionValues.YearEndDate;
            SoftwareStartDate = SessnionValues.SoftwareStartDate;
            TodayDate = SessnionValues.TodayDate;
        }


        //1 Block
        public string GetSaleOrderBalance()
        {
            mQry = @"SELECT B.Code AS Buyer, P.ProductName, PC.ProductCategoryName, PQ.ProductQualityName,
					L.Qty - isnull(CL.CQty,0) - isnull(SL.Qty,0) AS Qty,
					(L.Qty - isnull(CL.CQty,0) - isnull(SL.Qty,0))*(web.FConvertSqFeetToSqYard(VRS.ManufaturingSizeArea)) AS Area
                    FROM Web.SaleOrderHeaders H WITH (Nolock)
                    LEFT JOIN Web.SaleOrderLines L WITH (Nolock) ON H.SaleOrderHeaderId = L.SaleOrderHeaderId
                    LEFT JOIN Web.People B WITH (Nolock) ON H.SaleToBuyerId = B.PersonID
                    LEFT JOIN web.PersonBlockedDocumentTypes PB WITH (Nolock) ON PB.PersonID = B.PersonID AND PB.BlockedDocumentTypeId =448
                    LEFT JOIN Web.ViewRugSize  VRS WITH (Nolock) ON VRS.ProductId = L.ProductId
                    LEFT JOIN web.Products P WITH (Nolock) ON P.ProductId = L.ProductId 
                    LEFT JOIN web.FinishedProduct FP WITH (Nolock) ON FP.ProductId = P.ProductId 
                    LEFT JOIN web.ProductCategories PC WITH (Nolock) ON PC.ProductCategoryId = P.ProductCategoryId
                    LEFT JOIN web.ProductQualities PQ WITH (Nolock) ON PQ.ProductQualityId = FP.ProductQualityId
                    LEFT JOIN 
                    (
                    SELECT CL.SaleOrderLineId, sum(CL.Qty) AS CQty  FROM web.SaleOrderCancelLines CL WITH (Nolock) GROUP BY CL.SaleOrderLineId                    
                    ) CL ON CL.SaleOrderLineId = L.SaleOrderLineId
                    LEFT JOIN 
                    (
                    SELECT PL.SaleOrderLineId, sum(L.Qty) AS Qty  
                    FROM web.SaleInvoiceHeaders H WITH (Nolock)
                    LEFT JOIN web.SaleInvoiceLines L WITH (Nolock) ON L.SaleInvoiceHeaderId = H.SaleInvoiceHeaderId
                    LEFT JOIN web.SaleDispatchLines SL WITH (Nolock) ON SL.SaleDispatchLineId = L.SaleDispatchLineId
                    LEFT JOIN web.PackingLines PL WITH (Nolock) ON PL.PackingLineId = SL.PackingLineId
                    GROUP BY PL.SaleOrderLineId                    
                    ) SL ON SL.SaleOrderLineId = L.SaleOrderLineId
                    WHERE 1=1 AND PB.PersonBlockedDocumentTypeId IS NULL
                    AND L.Qty - isnull(CL.CQty,0) - isnull(SL.Qty,0) > 0";
            return mQry;
        }
        public IEnumerable<DashBoardDoubleValue> GetSaleOrderBalanceSummary()
        {
            mQry = @"Select Convert(NVARCHAR,IsNull(Sum(VMain.Qty),0)) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.Area),0)) AS Value2
                    From (" + GetSaleOrderBalance() + ") As VMain ";
            IEnumerable<DashBoardDoubleValue> SaleOrderBalanceSummary = db.Database.SqlQuery<DashBoardDoubleValue>(mQry).ToList();
            return SaleOrderBalanceSummary;
        }
        public IEnumerable<DashBoardTabularData_ThreeColumns> GetSaleOrderBalanceDetailCategoryWise()
        {

            mQry = @"SELECT VMain.ProductCategoryName AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.Qty))) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.Area),0)) AS Value2
                    FROM ( " + GetSaleOrderBalance() + @") As VMain
                    GROUP BY VMain.ProductCategoryName
                    ORDER BY IsNull(Sum(VMain.Area),0) DESC";

            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData_ThreeColumns>(mQry).ToList();
            return DashBoardTabularData_ThreeColumns;
        }
        public IEnumerable<DashBoardTabularData_ThreeColumns> GetSaleOrderBalanceDetailQualityWise()
        {
            mQry = @"SELECT VMain.ProductQualityName AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.Qty))) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.Area),0)) AS Value2
                    FROM ( " + GetSaleOrderBalance() + @") As VMain
                    GROUP BY VMain.ProductQualityName
                    ORDER BY IsNull(Sum(VMain.Area),0) DESC ";

            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData_ThreeColumns>(mQry).ToList();
            return DashBoardTabularData_ThreeColumns;
        }
        public IEnumerable<DashBoardTabularData_ThreeColumns> GetSaleOrderBalanceDetailBuyerWise()
        {
            mQry = @"SELECT VMain.Buyer AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.Qty))) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.Area),0)) AS Value2
                    FROM ( " + GetSaleOrderBalance() + @") As VMain
                    GROUP BY VMain.Buyer
                    ORDER BY IsNull(Sum(VMain.Area),0) DESC ";

            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData_ThreeColumns>(mQry).ToList();
            return DashBoardTabularData_ThreeColumns;
        }


        //2 Block
        public string GetUnExecute()
        {
            mQry = @"                
					SELECT Max(B.Code) AS Buyer, S.SiteCode, Max(P.ProductName) AS ProductName, Max(PC.ProductCategoryName) ProductCategoryName, Max(PQ.ProductQualityName) AS ProductQualityName,
                    Sum(L.Qty - isnull(VPC.CanQty,0)- isnull(VJO.JobQty,0) + isnull(VJC.JobCanQty,0)) AS Qty,  Sum((L.Qty - isnull(VPC.CanQty,0)- isnull(VJO.JobQty,0) + isnull(VJC.JobCanQty,0))*(web.FConvertSqFeetToSqYard(VRS.ManufaturingSizeArea))) AS Area
                    FROM web.ProdOrderHeaders H WITH (Nolock)
                    LEFT JOIN web.Sites S WITH (Nolock) ON S.SiteId = H.SiteId                  
                    LEFT JOIN web.ProdOrderLines L WITH (Nolock) ON L.ProdOrderHeaderId = H.ProdOrderHeaderId 
                    LEFT JOIN web.JobOrderLines JOL WITH (Nolock) ON JOL.JobOrderLineId = L.ReferenceDocLineId 
                    LEFT JOIN web.ProdOrderLines POL WITH (Nolock) ON POL.ProdOrderLineId = JOL.ProdOrderLineId 
                    LEFT JOIN web.ProdOrderHeaders POH WITH (Nolock) ON POH.ProdOrderHeaderId = POL.ProdOrderHeaderId
                    LEFT JOIN web.People B  WITH (Nolock) ON B.PersonID = POH.BuyerId 
                    LEFT JOIN web.Products P WITH (Nolock) ON P.ProductId = L.ProductId 
                    LEFT JOIN Web.ViewRugSize  VRS WITH (Nolock) ON VRS.ProductId = L.ProductId
                    LEFT JOIN web.FinishedProduct FP WITH (Nolock) ON FP.ProductId = P.ProductId 
                    LEFT JOIN web.ProductCategories PC WITH (Nolock) ON PC.ProductCategoryId = P.ProductCategoryId
                    LEFT JOIN web.ProductQualities PQ WITH (Nolock) ON PQ.ProductQualityId = FP.ProductQualityId
                    LEFT JOIN 
                    (
                    SELECT PC.ProdOrderLineId, sum(PC.Qty) AS CanQty  
                    FROM web.ProdOrderCancelLines PC WITH (Nolock)
                    WHERE PC.ProdOrderLineId IS NOT NULL 
                    GROUP BY PC.ProdOrderLineId 
                    ) VPC ON VPC.ProdOrderLineId = L.ProdOrderLineId
                    LEFT JOIN 
                    (
                    SELECT JO.ProdOrderLineId, sum(JO.Qty) AS JobQty  
                    FROM web.jobOrderLines JO WITH (Nolock)
                    WHERE JO.ProdOrderLineId IS NOT NULL 
                    GROUP BY JO.ProdOrderLineId 
                    ) VJO ON VJO.ProdOrderLineId = L.ProdOrderLineId
                    LEFT JOIN 
                    (
                    SELECT JO.ProdOrderLineId, sum(JC.Qty) AS JobCanQty  
                    FROM web.JobOrderCancelLines JC  WITH (Nolock)
                    LEFT JOIN web.JobOrderLines JO WITH (Nolock) ON JO.JobOrderLineId = JC.JobOrderLineId 
                    WHERE JO.ProdOrderLineId IS NOT NULL 
                    GROUP BY JO.ProdOrderLineId 
                    ) VJC ON VJC.ProdOrderLineId = L.ProdOrderLineId
                    WHERE H.DocTypeId =273 AND H.siteId >1
                    AND L.Qty - isnull(VPC.CanQty,0)- isnull(VJO.JobQty,0) + isnull(VJC.JobCanQty,0) > 0
                    Group BY S.SiteCode, B.Code, P.ProductName, PC.ProductCategoryName, PQ.ProductQualityName  ";
            return mQry;
        }
        public IEnumerable<DashBoardDoubleValue> GetUnExecuteSummary()
        {
            mQry = @"Select Convert(NVARCHAR,IsNull(Sum(VMain.Qty),0)) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.Area),0)) AS Value2
                    From (" + GetUnExecute() + ") As VMain ";
            IEnumerable<DashBoardDoubleValue> UnExecuteSummary = db.Database.SqlQuery<DashBoardDoubleValue>(mQry).ToList();
            return UnExecuteSummary;
        }
        public IEnumerable<DashBoardTabularData_ThreeColumns> GetUnExecuteDetailCategoryWise()
        {

            mQry = @"SELECT VMain.ProductCategoryName AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.Qty))) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.Area),0)) AS Value2
                    FROM ( " + GetUnExecute() + @") As VMain
                    GROUP BY VMain.ProductCategoryName
                    ORDER BY IsNull(Sum(VMain.Area),0) DESC";

            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData_ThreeColumns>(mQry).ToList();
            return DashBoardTabularData_ThreeColumns;
        }
        public IEnumerable<DashBoardTabularData_ThreeColumns> GetUnExecuteDetailBranchWise()
        {
            mQry = @"SELECT VMain.SiteCode AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.Qty))) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.Area),0)) AS Value2
                    FROM ( " + GetUnExecute() + @") As VMain
                    GROUP BY VMain.SiteCode
                    ORDER BY IsNull(Sum(VMain.Area),0) DESC ";

            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData_ThreeColumns>(mQry).ToList();
            return DashBoardTabularData_ThreeColumns;
        }
        public IEnumerable<DashBoardTabularData_ThreeColumns> GetUnExecuteDetailBuyerWise()
        {
            mQry = @"SELECT VMain.Buyer AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.Qty))) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.Area),0)) AS Value2
                    FROM ( " + GetUnExecute() + @") As VMain
                    GROUP BY VMain.Buyer
                    ORDER BY IsNull(Sum(VMain.Area),0) DESC ";

            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData_ThreeColumns>(mQry).ToList();
            return DashBoardTabularData_ThreeColumns;
        }



        //3 Block
        public string GetSaleInvoice()
        {
            mQry = @"SELECT B.Code AS Buyer, P.ProductName, PC.ProductCategoryName, PQ.ProductQualityName,
					Sum(L.Qty) AS Qty, Sum((L.Qty)*(web.FConvertSqFeetToSqYard(VRS.ManufaturingSizeArea))) AS Area,
                    IsNull(Sum(L.Amount),0) AS Amount, IsNull(Sum((L.Qty)*(web.FConvertSqFeetToSqYard(VRS.ManufaturingSizeArea))),0) AS MonthSale,
                    IsNull(Sum(Case When H.DocDate BETWEEN dateadd(Day,-7,@TodayDate) AND @TodayDate Then (L.Qty)*(web.FConvertSqFeetToSqYard(VRS.ManufaturingSizeArea)) Else 0 End),0) AS WeekSale,
                    IsNull(Sum(Case When H.DocDate = @TodayDate Then (L.Qty)*(web.FConvertSqFeetToSqYard(VRS.ManufaturingSizeArea)) Else 0 End),0) AS TodaySale         
                    FROM Web.SaleInvoiceHeaders H WITH (Nolock)
                    LEFT JOIN Web.SaleInvoiceLines L WITH (Nolock) ON L.SaleInvoiceHeaderId = H.SaleInvoiceHeaderId
                    LEFT JOIN web.Products P WITH (Nolock) ON P.ProductId = L.ProductId 
                    LEFT JOIN web.PersonBlockedDocumentTypes PB ON PB.PersonID = H.SaleToBuyerId AND PB.BlockedDocumentTypeId =448
                    LEFT JOIN web.People B ON B.PersonID = H.SaleToBuyerId
                    LEFT JOIN Web.ViewRugSize  VRS WITH (Nolock) ON VRS.ProductId = L.ProductId
                    LEFT JOIN web.FinishedProduct FP WITH (Nolock) ON FP.ProductId = P.ProductId 
                    LEFT JOIN web.ProductCategories PC WITH (Nolock) ON PC.ProductCategoryId = P.ProductCategoryId
                    LEFT JOIN web.ProductQualities PQ WITH (Nolock) ON PQ.ProductQualityId = FP.ProductQualityId
					WHERE 1=1
                    AND  H.DocDate BETWEEN @FromDate AND @ToDate 
                    Group By B.Code, P.ProductName, PC.ProductCategoryName, PQ.ProductQualityName ";
            return mQry;
        }
        public IEnumerable<DashBoardTrippleValue> GetSaleInvoiceSummary()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);
            SqlParameter SqlParameterTodayDate = new SqlParameter("@TodayDate", TodayDate);

            mQry = @"Select Convert(NVARCHAR,IsNull(Sum(VMain.MonthSale),0)) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.WeekSale),0)) AS Value2,
                    Convert(NVARCHAR,IsNull(Sum(VMain.TodaySale),0)) AS Value3
                    From (" + GetSaleInvoice() + ") As VMain ";
            IEnumerable<DashBoardTrippleValue> VehicleSale = db.Database.SqlQuery<DashBoardTrippleValue>(mQry, SqlParameterFromDate, SqlParameterToDate, SqlParameterTodayDate).ToList();
            return VehicleSale;
        }
        public IEnumerable<DashBoardTabularData_ThreeColumns> GetSaleInvoiceDetailCategoryWise()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);
            SqlParameter SqlParameterTodayDate = new SqlParameter("@TodayDate", TodayDate);

            mQry = @"SELECT VMain.ProductCategoryName AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.Qty))) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.Area),0)) AS Value2
                    From (" + GetSaleInvoice() + @") As VMain
                    GROUP BY VMain.ProductCategoryName
                    ORDER BY IsNull(Sum(VMain.Amount),0) DESC ";

            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData_ThreeColumns>(mQry, SqlParameterFromDate, SqlParameterToDate, SqlParameterTodayDate).ToList();
            return DashBoardTabularData_ThreeColumns;
        }
        public IEnumerable<DashBoardTabularData_ThreeColumns> GetSaleInvoiceDetailQualityWise()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);
            SqlParameter SqlParameterTodayDate = new SqlParameter("@TodayDate", TodayDate);

            mQry = @"SELECT VMain.ProductQualityName AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.Qty))) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.Area),0)) AS Value2
                    From (" + GetSaleInvoice() + @") As VMain
                    GROUP BY VMain.ProductQualityName
                    ORDER BY IsNull(Sum(VMain.Amount),0) DESC ";

            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData_ThreeColumns>(mQry, SqlParameterFromDate, SqlParameterToDate, SqlParameterTodayDate).ToList();
            return DashBoardTabularData_ThreeColumns;
        }
        public IEnumerable<DashBoardTabularData_ThreeColumns> GetSaleInvoiceDetailBuyerWise()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);
            SqlParameter SqlParameterTodayDate = new SqlParameter("@TodayDate", TodayDate);

            mQry = @"SELECT VMain.Buyer AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.Qty))) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.Area),0)) AS Value2
                     From (" + GetSaleInvoice() + @") As VMain
                    GROUP BY VMain.Buyer
                    ORDER BY IsNull(Sum(VMain.Amount),0) DESC ";

            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData_ThreeColumns>(mQry, SqlParameterFromDate, SqlParameterToDate, SqlParameterTodayDate).ToList();
            return DashBoardTabularData_ThreeColumns;
        }
        public IEnumerable<DashBoardSaleBarChartData> GetSaleInvoiceBarChartData()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", YearStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", YearEndDate);

            mQry = @"SELECT LEFT(DATENAME(month, H.DocDate),3) AS Month, 
                    Round(Sum( CASE WHEN C.Name ='INR' THEN  L.Amount/70 WHEN C.Name ='USD' THEN  L.Amount ELSE L.Amount END )/100000,2) AS Amount
                    FROM Web.SaleInvoiceHeaders H 
                    LEFT JOIN Web.SaleInvoiceLines L WITH (Nolock) ON L.SaleInvoiceHeaderId = H.SaleInvoiceHeaderId
                    LEFT JOIN Web.Currencies C WITH (Nolock) ON C.ID = H.CurrencyId
                    WHERE 1=1
                    AND  H.DocDate BETWEEN @FromDate AND @ToDate
                    GROUP BY DATENAME(month, H.DocDate)
                    ORDER BY DatePart(Year,Max(H.DocDate)) + Convert(DECIMAL(18,2),DatePart(month,Max(H.DocDate))) / 100  ";

            IEnumerable<DashBoardSaleBarChartData> ChartData = db.Database.SqlQuery<DashBoardSaleBarChartData>(mQry, SqlParameterFromDate, SqlParameterToDate).ToList();
            return ChartData;
        }



        //5 Block
        public string GetStock()
        {
            mQry = @"  SELECT Max(B.Code) AS Buyer, Max(P.ProductName) AS ProductName, Max(PC.ProductCategoryName) ProductCategoryName, Max(PQ.ProductQualityName) AS ProductQualityName,
                        Sum(H.Qty) AS Qty,
                        Sum((H.Qty)*(web.FConvertSqFeetToSqYard(VRS.ManufaturingSizeArea))) AS Area
                        FROM Web.ViewCarpetStock H WITH (Nolock)
                        LEFT JOIN web.ProductUids PU WITH (Nolock) ON PU.ProductUIDId = H.ProductUIDId
                        LEFT JOIN web.Products P WITH (Nolock) ON P.ProductId = H.ProductId 
                        LEFT JOIN web.FinishedProduct FP WITH (Nolock) ON FP.ProductId = P.ProductId 
                        LEFT JOIN web.ViewRugSize VRS WITH (Nolock) ON VRS.ProductId = P.ProductId
                        LEFT JOIN web.ProductQualities PQ WITH (Nolock) ON PQ.ProductQualityId = FP.ProductQualityId  
						LEFT JOIN web.ProductCategories PC WITH (Nolock) ON PC.ProductCategoryId = P.ProductCategoryId
                        LEFT JOIN web.SaleOrderLines SOL WITH (Nolock) ON SOL.SaleOrderLineId = PU.SaleOrderLineId
						LEFT JOIN web.SaleOrderHeaders SOH WITH (Nolock) ON SOH.SaleOrderHeaderId = SOL.SaleOrderHeaderId
                        LEFT JOIN web.StockHeaders SH WITH (Nolock) ON SH.StockHeaderId = PU.GenDocId AND PU.GenDocTYpeId = SH.DocTYpeId AND PU.GenDocTYpeId =354
						LEFT JOIN web.People B WITH (Nolock) ON B.PersonID = isnull(SOH.SaleToBuyerId,SH.PersonId)
                        GROUP BY B.PersonID,H.ProductId
                        HAVING Sum(isnull(H.Qty,0)) <>0 ";
            return mQry;
        }
        public IEnumerable<DashBoardDoubleValue> GetStockSummary()
        {
            mQry = @"Select Convert(NVARCHAR,IsNull(Sum(VMain.Qty),0)) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.Area),0)) AS Value2
                    From (" + GetStock() + ") As VMain ";
            IEnumerable<DashBoardDoubleValue> StockSummary = db.Database.SqlQuery<DashBoardDoubleValue>(mQry).ToList();
            return StockSummary;
        }
        public IEnumerable<DashBoardTabularData_ThreeColumns> GetStockDetailCategoryWise()
        {

            mQry = @"SELECT VMain.ProductCategoryName AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.Qty))) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.Area),0)) AS Value2
                    FROM ( " + GetStock() + @") As VMain
                    GROUP BY VMain.ProductCategoryName
                    ORDER BY IsNull(Sum(VMain.Area),0) DESC";

            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData_ThreeColumns>(mQry).ToList();
            return DashBoardTabularData_ThreeColumns;
        }
        public IEnumerable<DashBoardTabularData_ThreeColumns> GetStockDetailQualityWise()
        {
            mQry = @"SELECT VMain.ProductQualityName AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.Qty))) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.Area),0)) AS Value2
                    FROM ( " + GetStock() + @") As VMain
                    GROUP BY VMain.ProductQualityName
                    ORDER BY IsNull(Sum(VMain.Area),0) DESC ";

            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData_ThreeColumns>(mQry).ToList();
            return DashBoardTabularData_ThreeColumns;
        }
        public IEnumerable<DashBoardTabularData_ThreeColumns> GetStockDetailBuyerWise()
        {
            mQry = @"SELECT VMain.Buyer AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.Qty))) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.Area),0)) AS Value2
                    FROM ( " + GetStock() + @") As VMain
                    GROUP BY VMain.Buyer
                    ORDER BY IsNull(Sum(VMain.Area),0) DESC ";

            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData_ThreeColumns>(mQry).ToList();
            return DashBoardTabularData_ThreeColumns;
        }


        //6 Block
        public string GetToBeIssue()
        {
            mQry = @"                
					SELECT Max(B.Code) AS Buyer,  Max(P.ProductName) AS ProductName, Max(PC.ProductCategoryName) ProductCategoryName, Max(PQ.ProductQualityName) AS ProductQualityName,
                    Sum(L.Qty - isnull(VPC.CanQty,0)- isnull(VJO.JobQty,0) + isnull(VJC.JobCanQty,0)) AS Qty,  Sum((L.Qty - isnull(VPC.CanQty,0)- isnull(VJO.JobQty,0) + isnull(VJC.JobCanQty,0))*(web.FConvertSqFeetToSqYard(VRS.ManufaturingSizeArea))) AS Area
                    FROM web.ProdOrderHeaders H WITH (Nolock)
                    LEFT JOIN web.People B ON B.PersonID = H.BuyerId 
                    LEFT JOIN web.ProdOrderLines L WITH (Nolock) ON L.ProdOrderHeaderId = H.ProdOrderHeaderId 
                    LEFT JOIN web.Products P WITH (Nolock) ON P.ProductId = L.ProductId 
                    LEFT JOIN Web.ViewRugSize  VRS WITH (Nolock) ON VRS.ProductId = L.ProductId
                    LEFT JOIN web.FinishedProduct FP WITH (Nolock) ON FP.ProductId = P.ProductId 
                    LEFT JOIN web.ProductCategories PC WITH (Nolock) ON PC.ProductCategoryId = P.ProductCategoryId
                    LEFT JOIN web.ProductQualities PQ WITH (Nolock) ON PQ.ProductQualityId = FP.ProductQualityId
                    LEFT JOIN 
                    (
                    SELECT PC.ProdOrderLineId, sum(PC.Qty) AS CanQty  
                    FROM web.ProdOrderCancelLines PC WITH (Nolock)
                    WHERE PC.ProdOrderLineId IS NOT NULL 
                    GROUP BY PC.ProdOrderLineId 
                    ) VPC ON VPC.ProdOrderLineId = L.ProdOrderLineId
                    LEFT JOIN 
                    (
                    SELECT JO.ProdOrderLineId, sum(JO.Qty) AS JobQty  
                    FROM web.jobOrderLines JO WITH (Nolock)
                    WHERE JO.ProdOrderLineId IS NOT NULL 
                    GROUP BY JO.ProdOrderLineId 
                    ) VJO ON VJO.ProdOrderLineId = L.ProdOrderLineId
                    LEFT JOIN 
                    (
                    SELECT JO.ProdOrderLineId, sum(JC.Qty) AS JobCanQty  
                    FROM web.JobOrderCancelLines JC  WITH (Nolock)
                    LEFT JOIN web.JobOrderLines JO WITH (Nolock) ON JO.JobOrderLineId = JC.JobOrderLineId 
                    WHERE JO.ProdOrderLineId IS NOT NULL 
                    GROUP BY JO.ProdOrderLineId 
                    ) VJC ON VJC.ProdOrderLineId = L.ProdOrderLineId
                    WHERE H.DocTypeId =273 AND H.siteId =1
                    AND L.Qty - isnull(VPC.CanQty,0)- isnull(VJO.JobQty,0) + isnull(VJC.JobCanQty,0) > 0
                    Group By B.Code, P.ProductName, PC.ProductCategoryName, PQ.ProductQualityName  ";
            return mQry;
        }
        public IEnumerable<DashBoardDoubleValue> GetToBeIssueSummary()
        {
            mQry = @"Select Convert(NVARCHAR,IsNull(Sum(VMain.Qty),0)) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.Area),0)) AS Value2
                    From (" + GetToBeIssue() + ") As VMain ";
            IEnumerable<DashBoardDoubleValue> ToBeIssueSummary = db.Database.SqlQuery<DashBoardDoubleValue>(mQry).ToList();
            return ToBeIssueSummary;
        }
        public IEnumerable<DashBoardTabularData_ThreeColumns> GetToBeIssueDetailCategoryWise()
        {

            mQry = @"SELECT VMain.ProductCategoryName AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.Qty))) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.Area),0)) AS Value2
                    FROM ( " + GetToBeIssue() + @") As VMain
                    GROUP BY VMain.ProductCategoryName
                    ORDER BY IsNull(Sum(VMain.Area),0) DESC";

            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData_ThreeColumns>(mQry).ToList();
            return DashBoardTabularData_ThreeColumns;
        }
        public IEnumerable<DashBoardTabularData_ThreeColumns> GetToBeIssueDetailQualityWise()
        {
            mQry = @"SELECT VMain.ProductQualityName AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.Qty))) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.Area),0)) AS Value2
                    FROM ( " + GetToBeIssue() + @") As VMain
                    GROUP BY VMain.ProductQualityName
                    ORDER BY IsNull(Sum(VMain.Area),0) DESC ";

            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData_ThreeColumns>(mQry).ToList();
            return DashBoardTabularData_ThreeColumns;
        }
        public IEnumerable<DashBoardTabularData_ThreeColumns> GetToBeIssueDetailBuyerWise()
        {
            mQry = @"SELECT VMain.Buyer AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.Qty))) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.Area),0)) AS Value2
                    FROM ( " + GetToBeIssue() + @") As VMain
                    GROUP BY VMain.Buyer
                    ORDER BY IsNull(Sum(VMain.Area),0) DESC ";

            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData_ThreeColumns>(mQry).ToList();
            return DashBoardTabularData_ThreeColumns;
        }



        //7 Block
        public string GetProduction()
        {
            mQry = @"SELECT B.Code AS Buyer, P.ProductName, PC.ProductCategoryName, PQ.ProductQualityName,
					Sum(L.Qty) AS Qty, Sum((L.Qty)*(web.FConvertSqFeetToSqYard(VRS.ManufaturingSizeArea))) AS Area,
                    IsNull(Sum((L.Qty)*(web.FConvertSqFeetToSqYard(VRS.ManufaturingSizeArea))),0) AS MonthProduction,
                    IsNull(Sum(Case When H.DocDate BETWEEN dateadd(Day,-7,@TodayDate) AND @TodayDate Then (L.Qty)*(web.FConvertSqFeetToSqYard(VRS.ManufaturingSizeArea)) Else 0 End),0) AS WeekProduction,
                    IsNull(Sum(Case When H.DocDate = @TodayDate Then (L.Qty)*(web.FConvertSqFeetToSqYard(VRS.ManufaturingSizeArea)) Else 0 End),0) AS TodayProduction
                    FROM Web.JobReceiveHeaders H 
                    LEFT JOIN Web.JobReceiveLines L WITH (Nolock) ON L.JobReceiveHeaderId = H.JobReceiveHeaderId
                    LEFT JOIN web.Products P WITH (Nolock) ON P.ProductId = L.ProductId 
                    LEFT JOIN Web.ViewRugSize  VRS WITH (Nolock) ON VRS.ProductId = L.ProductId
                    LEFT JOIN web.FinishedProduct FP WITH (Nolock) ON FP.ProductId = P.ProductId 
                    LEFT JOIN web.ProductCategories PC WITH (Nolock) ON PC.ProductCategoryId = P.ProductCategoryId
                    LEFT JOIN web.ProductQualities PQ WITH (Nolock) ON PQ.ProductQualityId = FP.ProductQualityId
                    LEFT JOIN Web.People  JW ON JW.PersonID = H.JobWorkerId
                    LEFT JOIN web.ProductUids PU WITH (Nolock) ON PU.ProductUIDId = L.ProductUIDId OR L.LotNo = PU.LotNo 
                    LEFT JOIN web.SaleOrderLines SOL WITH (Nolock) ON SOL.SaleOrderLineId =PU.SaleOrderLineId 
                    LEFT JOIN web.SaleOrderHeaders SOH WITH (Nolock) ON SOH.SaleOrderHeaderId =SOL.SaleOrderHeaderId 
                    LEFT JOIN web.PersonBlockedDocumentTypes PB ON PB.PersonID = SOH.SaleToBuyerId AND PB.BlockedDocumentTypeId =448
                    LEFT JOIN web.People B ON B.PersonID = SOH.SaleToBuyerId
                    WHERE 1=1 AND H.DocTypeId =448 AND isnull(JW.IsSisterConcern,0) =0 
                    AND  H.DocDate BETWEEN @FromDate AND @ToDate 
                    Group By B.Code, P.ProductName, PC.ProductCategoryName, PQ.ProductQualityName ";
            return mQry;
        }
        public IEnumerable<DashBoardTrippleValue> GetProductionSummary()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);
            SqlParameter SqlParameterTodayDate = new SqlParameter("@TodayDate", TodayDate);

            mQry = @"Select Convert(NVARCHAR,IsNull(Sum(VMain.MonthProduction),0)) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.WeekProduction),0)) AS Value2,
                    Convert(NVARCHAR,IsNull(Sum(VMain.TodayProduction),0)) AS Value3
                    From (" + GetProduction() + ") As VMain ";
            IEnumerable<DashBoardTrippleValue> VehicleSale = db.Database.SqlQuery<DashBoardTrippleValue>(mQry, SqlParameterFromDate, SqlParameterToDate, SqlParameterTodayDate).ToList();
            return VehicleSale;
        }
        public IEnumerable<DashBoardTabularData_ThreeColumns> GetProductionDetailCategoryWise()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);
            SqlParameter SqlParameterTodayDate = new SqlParameter("@TodayDate", TodayDate);

            mQry = @"SELECT VMain.ProductCategoryName AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.Qty))) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.Area),0)) AS Value2
                    From (" + GetProduction() + @") As VMain
                    GROUP BY VMain.ProductCategoryName
                    ORDER BY IsNull(Sum(VMain.Area),0) DESC ";

            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData_ThreeColumns>(mQry, SqlParameterFromDate, SqlParameterToDate, SqlParameterTodayDate).ToList();
            return DashBoardTabularData_ThreeColumns;
        }
        public IEnumerable<DashBoardTabularData_ThreeColumns> GetProductionDetailQualityWise()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);
            SqlParameter SqlParameterTodayDate = new SqlParameter("@TodayDate", TodayDate);

            mQry = @"SELECT VMain.ProductQualityName AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.Qty))) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.Area),0)) AS Value2
                    From (" + GetProduction() + @") As VMain
                    GROUP BY VMain.ProductQualityName
                    ORDER BY IsNull(Sum(VMain.Area),0) DESC ";

            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData_ThreeColumns>(mQry, SqlParameterFromDate, SqlParameterToDate, SqlParameterTodayDate).ToList();
            return DashBoardTabularData_ThreeColumns;
        }
        public IEnumerable<DashBoardTabularData_ThreeColumns> GetProductionDetailBuyerWise()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);
            SqlParameter SqlParameterTodayDate = new SqlParameter("@TodayDate", TodayDate);

            mQry = @"SELECT VMain.Buyer AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.Qty))) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.Area),0)) AS Value2
                     From (" + GetProduction() + @") As VMain
                    GROUP BY VMain.Buyer
                    ORDER BY IsNull(Sum(VMain.Area),0) DESC ";

            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData_ThreeColumns>(mQry, SqlParameterFromDate, SqlParameterToDate, SqlParameterTodayDate).ToList();
            return DashBoardTabularData_ThreeColumns;
        }
        public IEnumerable<DashBoardPieChartData> GetProductionPieChartData()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);

            mQry = @"SELECT S.SiteCode As label, Round(Sum(L.Qty*(web.FConvertSqFeetToSqYard(VRS.ManufaturingSizeArea)))/1,2) AS value,
                        CASE WHEN row_number() OVER (ORDER BY Round(Sum(L.Qty*(web.FConvertSqFeetToSqYard(VRS.ManufaturingSizeArea)))/1,2)  DESC ) = 1 THEN '#008000' 
	                     WHEN row_number() OVER (ORDER BY Round(Sum(L.Qty*(web.FConvertSqFeetToSqYard(VRS.ManufaturingSizeArea)))/1,2) DESC ) = 2 THEN '#9acd32'
	                     WHEN row_number() OVER (ORDER BY Round(Sum(L.Qty*(web.FConvertSqFeetToSqYard(VRS.ManufaturingSizeArea)))/1,2) DESC ) = 3 THEN '#ffff00'
	                     WHEN row_number() OVER (ORDER BY Round(Sum(L.Qty*(web.FConvertSqFeetToSqYard(VRS.ManufaturingSizeArea)))/1,2) DESC ) = 4 THEN '#ffae42'
	                     WHEN row_number() OVER (ORDER BY Round(Sum(L.Qty*(web.FConvertSqFeetToSqYard(VRS.ManufaturingSizeArea)))/1,2) DESC ) = 5 THEN '#ffa500'
	                     WHEN row_number() OVER (ORDER BY Round(Sum(L.Qty*(web.FConvertSqFeetToSqYard(VRS.ManufaturingSizeArea)))/1,2) DESC ) = 6 THEN '#ff5349'
                         WHEN row_number() OVER (ORDER BY Round(Sum(L.Qty*(web.FConvertSqFeetToSqYard(VRS.ManufaturingSizeArea)))/1,2) DESC ) = 7 THEN '#ff0000'
                         WHEN row_number() OVER (ORDER BY Round(Sum(L.Qty*(web.FConvertSqFeetToSqYard(VRS.ManufaturingSizeArea)))/1,2) DESC ) = 8 THEN '#c71585'
                         WHEN row_number() OVER (ORDER BY Round(Sum(L.Qty*(web.FConvertSqFeetToSqYard(VRS.ManufaturingSizeArea)))/1,2) DESC ) = 9 THEN '#800080'
                         WHEN row_number() OVER (ORDER BY Round(Sum(L.Qty*(web.FConvertSqFeetToSqYard(VRS.ManufaturingSizeArea)))/1,2) DESC ) = 10 THEN '#8a2be2'
                         WHEN row_number() OVER (ORDER BY Round(Sum(L.Qty*(web.FConvertSqFeetToSqYard(VRS.ManufaturingSizeArea)))/1,2) DESC ) = 11 THEN '#0000ff'
                         WHEN row_number() OVER (ORDER BY Round(Sum(L.Qty*(web.FConvertSqFeetToSqYard(VRS.ManufaturingSizeArea)))/1,2) DESC ) = 12 THEN '#0d98ba'
	                     ELSE '#f56954'
                    END AS color 
                    FROM Web.JobReceiveHeaders H  WITH (Nolock)
                    LEFT JOIN web.Sites S  WITH (Nolock) ON S.SiteId = H.SiteId
                    LEFT JOIN Web.JobReceiveLines L WITH (Nolock) ON L.JobReceiveHeaderId = H.JobReceiveHeaderId
                    LEFT JOIN Web.ViewRugSize  VRS WITH (Nolock) ON VRS.ProductId = L.ProductId
                    LEFT JOIN Web.People  JW  WITH (Nolock) ON JW.PersonID = H.JobWorkerId
                    WHERE 1=1 AND H.DocTypeId =448 AND isnull(JW.IsSisterConcern,0) =0 
                    AND  H.DocDate BETWEEN @FromDate AND @ToDate
                    GROUP BY S.SiteCode
					ORDER BY Round(Sum(L.Qty*(web.FConvertSqFeetToSqYard(VRS.ManufaturingSizeArea)))/1,2) DESC ";

            IEnumerable<DashBoardPieChartData> VehicleSalePieChartData = db.Database.SqlQuery<DashBoardPieChartData>(mQry, SqlParameterFromDate, SqlParameterToDate).ToList();
            return VehicleSalePieChartData;
        }


        //9 Block
        public string GetOnLoom()
        {
            mQry = @"                   
					SELECT Max(B.Code) AS Buyer, Max(J.Name)+'-'+Max(S.SiteCode) JobWorker, Max(P.ProductName) AS ProductName, Max(PC.ProductCategoryName) ProductCategoryName, Max(PQ.ProductQualityName) AS ProductQualityName,
                    Sum(H.BalanceQty) AS Qty,  Sum((H.BalanceQty)*(web.FConvertSqFeetToSqYard(VRS.ManufaturingSizeArea))) AS Area
                    FROM [Web].[FWeavingOrderStatus_OneProcess] (Getdate(),NULL ,NULL,'01/Apr/2010',Getdate(),43) H
                    LEFT JOIN web.JobOrderHeaders JOH WITH (Nolock) ON JOH.JobOrderHeaderId =H.JobOrderHeaderId
                    LEFT JOIN web.Sites S WITH (Nolock) ON S.SiteId = JOH.SiteId 
                    LEFT JOIN web.People J WITH (Nolock) ON J.PersonID = JOH.JobWorkerId 
                    LEFT JOIN Web.ProdOrderLines POL WITH (Nolock) ON POL.ProdOrderLineId=H.ProdOrderLineId
                    LEFT JOIN web.JobOrderLines JOL WITH (Nolock) ON JOL.JobOrderLineId = POL.ReferenceDocLineId 
                    LEFT JOIN web.ProdOrderLines POL1 WITH (Nolock) ON POL1.ProdOrderLineId = isnull(JOL.ProdOrderLineId,H.ProdOrderLineId) 
                    LEFT JOIN web.Products P WITH (Nolock) ON P.ProductId = H.ProductId 
                    LEFT JOIN web.MaterialPlanForSaleOrders MOS WITH (Nolock) ON MOS.MaterialPlanLineId = POL1.MaterialPlanLineId  
                    LEFT JOIN web.SaleOrderLines SOL WITH (Nolock) ON SOL.SaleOrderLineId = MOS.SaleOrderLineId
                    LEFT JOIN web.SaleOrderHeaders SOH WITH (Nolock) ON SOH.SaleOrderHeaderId = SOL.SaleOrderHeaderId
                    LEFT JOIN web.PersonBlockedDocumentTypes PB WITH (Nolock) ON PB.PersonID = SOH.SaleToBuyerId AND PB.BlockedDocumentTypeId =448
                    LEFT JOIN web.People B WITH (Nolock) ON B.PersonID = SOH.SaleToBuyerId
                    LEFT JOIN Web.ViewRugSize  VRS WITH (Nolock) ON VRS.ProductId = H.ProductId
                    LEFT JOIN web.FinishedProduct FP WITH (Nolock) ON FP.ProductId = P.ProductId 
                    LEFT JOIN web.ProductCategories PC WITH (Nolock) ON PC.ProductCategoryId = P.ProductCategoryId
                    LEFT JOIN web.ProductQualities PQ WITH (Nolock) ON PQ.ProductQualityId = FP.ProductQualityId
                    WHERE 1=1 AND isnull(J.IsSisterConcern,0)=0 
                    And PB.PersonBlockedDocumentTypeId IS NULL 
                    AND isnull(H.BalanceQty,0) >0 --AND PC.ProductCategoryName <> 'OVER TUFT' 
					GROUP BY SOH.SaleToBuyerId, JOH.JobWorkerId, H.ProductId					
					/*UNION ALL 
                    SELECT Max(B.Code) AS Buyer, Max(JW.Name)+'-'+Max(S.SiteCode) JobWorker, Max(P.ProductName) AS ProductName, Max(PC.ProductCategoryName) ProductCategoryName, Max(PQ.ProductQualityName) AS ProductQualityName,
                    Sum(isnull(L.Qty, 0)-isnull(V.RecQty, 0)) AS Qty,  Sum((isnull(L.Qty, 0)-isnull(V.RecQty, 0))*(web.FConvertSqFeetToSqYard(VRS.ManufaturingSizeArea))) AS Area
                    FROM web.JobOrderHeaders H WITH(Nolock)
                    LEFT JOIN web.Sites S WITH (Nolock) ON S.SiteId = H.SiteId 
                    LEFT JOIN web.People JW WITH (Nolock) ON JW.PersonID = H.JobWorkerId
                    LEFT JOIN web.JobOrderLines L WITH (Nolock) ON L.JobOrderHeaderId = H.JobOrderHeaderId
                    LEFT JOIN Web.ProdOrderLines POL WITH(Nolock) ON POL.ProdOrderLineId = L.ProdOrderLineId
                    LEFT JOIN web.Products P WITH(Nolock) ON P.ProductId = L.ProductId
                    LEFT JOIN web.MaterialPlanForSaleOrders MOS WITH(Nolock) ON MOS.MaterialPlanLineId = POL.MaterialPlanLineId
                    LEFT JOIN web.SaleOrderLines SOL WITH(Nolock) ON SOL.SaleOrderLineId = MOS.SaleOrderLineId
                    LEFT JOIN web.SaleOrderHeaders SOH WITH(Nolock) ON SOH.SaleOrderHeaderId = SOL.SaleOrderHeaderId
                    LEFT JOIN web.PersonBlockedDocumentTypes PB WITH (Nolock) ON PB.PersonID = SOH.SaleToBuyerId AND PB.BlockedDocumentTypeId =448
                    LEFT JOIN web.People B WITH (Nolock) ON B.PersonID = SOH.SaleToBuyerId
                    LEFT JOIN Web.ViewRugSize  VRS WITH (Nolock) ON VRS.ProductId = L.ProductId
                    LEFT JOIN web.FinishedProduct FP WITH (Nolock) ON FP.ProductId = P.ProductId 
                    LEFT JOIN web.ProductCategories PC WITH (Nolock) ON PC.ProductCategoryId = P.ProductCategoryId
                    LEFT JOIN web.ProductQualities PQ WITH (Nolock) ON PQ.ProductQualityId = FP.ProductQualityId
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
                    WHERE H.SiteId = 1 AND H.DivisionId = 1 AND H.ProcessId = 43 AND isnull(JW.IsSisterConcern, 0)= 0  
                    And PB.PersonBlockedDocumentTypeId IS NULL 
                    AND POL.ProdOrderHeaderId IS NOT NULL AND PC.ProductCategoryName = 'OVER TUFT'
                    AND isnull(L.Qty, 0)-isnull(V.RecQty, 0) > 0 
                    GROUP BY SOH.SaleToBuyerId,H.JobWorkerId, L.ProductId */ ";
            return mQry;
        }
        public IEnumerable<DashBoardDoubleValue> GetOnLoomSummary()
        {
            mQry = @"Select Convert(NVARCHAR,IsNull(Sum(VMain.Qty),0)) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.Area),0)) AS Value2
                    From (" + GetOnLoom() + ") As VMain ";
            IEnumerable<DashBoardDoubleValue> OnLoomSummary = db.Database.SqlQuery<DashBoardDoubleValue>(mQry).ToList();
            return OnLoomSummary;
        }
        public IEnumerable<DashBoardTabularData_ThreeColumns> GetOnLoomDetailCategoryWise()
        {

            mQry = @"SELECT VMain.ProductCategoryName AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.Qty))) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.Area),0)) AS Value2
                    FROM ( " + GetOnLoom() + @") As VMain
                    GROUP BY VMain.ProductCategoryName
                    ORDER BY IsNull(Sum(VMain.Area),0) DESC";

            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData_ThreeColumns>(mQry).ToList();
            return DashBoardTabularData_ThreeColumns;
        }
        public IEnumerable<DashBoardTabularData_ThreeColumns> GetOnLoomDetailJobWorkerWise()
        {
            mQry = @"SELECT VMain.JobWorker AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.Qty))) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.Area),0)) AS Value2
                    FROM ( " + GetOnLoom() + @") As VMain
                    GROUP BY VMain.JobWorker
                    ORDER BY IsNull(Sum(VMain.Area),0) DESC ";

            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData_ThreeColumns>(mQry).ToList();
            return DashBoardTabularData_ThreeColumns;
        }
        public IEnumerable<DashBoardTabularData_ThreeColumns> GetOnLoomDetailBuyerWise()
        {
            mQry = @"SELECT VMain.Buyer AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.Qty))) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.Area),0)) AS Value2
                    FROM ( " + GetOnLoom() + @") As VMain
                    GROUP BY VMain.Buyer
                    ORDER BY IsNull(Sum(VMain.Area),0) DESC ";

            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData_ThreeColumns>(mQry).ToList();
            return DashBoardTabularData_ThreeColumns;
        }






        //10 Block
        public string GetDyeingOrderBalance()
        {
            mQry = @"SELECT  Max(H.DocDate) AS DocDate, Max(P.ProductName) AS ProductName, Max(JW.Name) AS JobWorkerName,
					sum(L.Qty - isnull(CL.CQty,0) - isnull(SL.Qty,0) + isnull(RT.Qty,0) ) AS Qty
                    FROM Web.JobOrderHeaders H WITH (Nolock)
                    LEFT JOIN web.People JW ON JW.PersonID =  H.JobWorkerId 
                    LEFT JOIN Web.JobOrderLines L WITH (Nolock) ON H.JobOrderHeaderId = L.JobOrderHeaderId
                    LEFT JOIN web.Products P WITH (Nolock) ON P.ProductId = L.ProductId 
                    LEFT JOIN 
                    (
                    SELECT CL.JobOrderLineId, sum(CL.Qty) AS CQty  
                    FROM web.JobOrderCancelLines CL WITH (Nolock) GROUP BY CL.JobOrderLineId                    
                    ) CL ON CL.JobOrderLineId = L.JobOrderLineId
                    LEFT JOIN 
                    (
                    SELECT L.JobOrderLineId, sum(isnull(L.Qty,0)+isnull(L.LossQty,0)) AS Qty 
                    FROM web.JobReceiveHeaders  H WITH (Nolock)
                    LEFT JOIN web.JobReceiveLines L WITH (Nolock) ON L.JobReceiveHeaderId = H.JobReceiveHeaderId
                    GROUP BY L.JobOrderLineId                    
                    ) SL ON SL.JobOrderLineId = L.JobOrderLineId
                    LEFT JOIN 
                    (
                    SELECT L.JobOrderLineId, sum( isnull(RT.Qty,0)+isnull(RT.LossQty,0)) AS Qty  
                    FROM web.JobReturnLines  RT WITH (Nolock)
                    LEFT JOIN web.JobReceiveLines L WITH (Nolock) ON L.JobReceiveLineId = RT.JobReceiveLineId
                    GROUP BY L.JobOrderLineId                    
                    ) RT ON RT.JobOrderLineId = L.JobOrderLineId
                    WHERE 1=1 AND H.ProcessId =1003 AND H.SiteId =1
                    AND L.Qty - isnull(CL.CQty,0) - isnull(SL.Qty,0) > 0
                    GROUP BY L.JobOrderLineId ";
            return mQry;
        }
        public IEnumerable<DashBoardSingleValue> GetDyeingOrderBalanceSummary()
        {
            mQry = @"Select Convert(NVARCHAR,IsNull(Sum(VMain.Qty),0)) AS Value
                    From (" + GetDyeingOrderBalance() + ") As VMain ";
            IEnumerable<DashBoardSingleValue> DyeingOrderBalanceSummary = db.Database.SqlQuery<DashBoardSingleValue>(mQry).ToList();
            return DyeingOrderBalanceSummary;
        }
        public IEnumerable<DashBoardTabularData> GetDyeingOrderBalanceDetailProductWise()
        {

            mQry = @"SELECT VMain.ProductName AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.Qty))) AS Value
                    FROM ( " + GetDyeingOrderBalance() + @") As VMain
                    GROUP BY VMain.ProductName
                    ORDER BY IsNull(Sum(VMain.Qty),0) DESC";

            IEnumerable<DashBoardTabularData> DashBoardTabularData = db.Database.SqlQuery<DashBoardTabularData>(mQry).ToList();
            return DashBoardTabularData;
        }
        public IEnumerable<DashBoardTabularData> GetDyeingOrderBalanceDetailMonthWise()
        {
            mQry = @"SELECT REPLACE(RIGHT(CONVERT(VARCHAR(9), VMain.DocDate, 6), 6), ' ', '-') AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.Qty))) AS Value
                    FROM ( " + GetDyeingOrderBalance() + @") As VMain
                    GROUP BY REPLACE(RIGHT(CONVERT(VARCHAR(9), VMain.DocDate, 6), 6), ' ', '-')
                    ORDER BY IsNull(Sum(VMain.Qty),0) DESC";

            IEnumerable<DashBoardTabularData> DashBoardTabularData = db.Database.SqlQuery<DashBoardTabularData>(mQry).ToList();
            return DashBoardTabularData;
        }
        public IEnumerable<DashBoardTabularData> GetDyeingOrderBalanceDetailJobWorkerWise()
        {
            mQry = @"SELECT VMain.JobWorkerName AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.Qty))) AS Value
                    FROM ( " + GetDyeingOrderBalance() + @") As VMain
                    GROUP BY VMain.JobWorkerName
                    ORDER BY IsNull(Sum(VMain.Qty),0) DESC";

            IEnumerable<DashBoardTabularData> DashBoardTabularData = db.Database.SqlQuery<DashBoardTabularData>(mQry).ToList();
            return DashBoardTabularData;
        }



        //11 Block




        //12 Block
        public string GetLoanBalance()
        {
            mQry = @"
                    SELECT H.DocDate, LA.LedgerAccountName, Isnull(D.DepartmentName,D1.DepartmentName) AS DepartmentName, sum(isnull(LL.Amount,0) -  isnull(LADS.AdjAmount,0)) AS BalAmount
                    FROM web.LedgerHeaders H
                    LEFT JOIN web.DocumentTypes DT1 ON DT1.DocumentTypeId = H.DocTypeId
                    LEFT JOIN web.Processes P ON P.ProcessId = H.ProcessId
                    LEFT JOIN web.LedgerLines LL ON LL.LedgerHeaderId = H.LedgerHeaderId 
                    LEFT JOIN web.LedgerAccounts LA ON LA.LedgerAccountId = LL.LedgerAccountId 
                    LEFT JOIN web.Ledgers L ON L.LedgerLineId = LL.LedgerLineId AND L.LedgerAccountId = LL.LedgerAccountId 
                    LEFT JOIN web.Employees E ON E.PersonId = LA.PersonId
                    LEFT JOIN web.Departments D ON D.DepartmentId = E.DepartmentId
                    LEFT JOIN web.Departments D1 ON D1.DepartmentId = P.DepartmentId 
                    LEFT JOIN 
                    (
                    SELECT LADS.DrLedgerId, 
                    sum(LADS.Amount) AS AdjAmount 
                    FROM WEB.LedgerAdjs LADS  GROUP BY LADS.DrLedgerId 
                    ) LADS ON LADS.DrLedgerId = L.LedgerId
                    WHERE H.DocTypeId IN (7047,7043)
                    AND  H.SiteId =1 AND  H.DivisionId =1
                    AND isnull(LL.Amount,0) -  isnull(LADS.AdjAmount,0) > 0
                    GROUP BY H.DocDate, LA.LedgerAccountName, Isnull(D.DepartmentName,D1.DepartmentName)  ";
            return mQry;
        }
        public IEnumerable<DashBoardSingleValue> GetLoanBalanceSummary()
        {
            mQry = @"Select Convert(NVARCHAR,IsNull(Sum(VMain.BalAmount),0)) AS Value
                    From (" + GetLoanBalance() + ") As VMain ";
            IEnumerable<DashBoardSingleValue> LoanBalanceSummary = db.Database.SqlQuery<DashBoardSingleValue>(mQry).ToList();
            return LoanBalanceSummary;
        }
        public IEnumerable<DashBoardTabularData> GetLoanBalanceDetailDepartmentWise()
        {

            mQry = @"SELECT VMain.DepartmentName AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.BalAmount))) AS Value
                    FROM ( " + GetLoanBalance() + @") As VMain
                    GROUP BY VMain.DepartmentName
                    ORDER BY IsNull(Sum(VMain.BalAmount),0) DESC";

            IEnumerable<DashBoardTabularData> DashBoardTabularData = db.Database.SqlQuery<DashBoardTabularData>(mQry).ToList();
            return DashBoardTabularData;
        }
        public IEnumerable<DashBoardTabularData> GetLoanBalanceDetailMonthWise()
        {
            mQry = @"SELECT REPLACE(RIGHT(CONVERT(VARCHAR(9), VMain.DocDate, 6), 6), ' ', '-') AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.BalAmount))) AS Value
                    FROM ( " + GetLoanBalance() + @") As VMain
                    GROUP BY REPLACE(RIGHT(CONVERT(VARCHAR(9), VMain.DocDate, 6), 6), ' ', '-')
                    ORDER BY IsNull(Sum(VMain.BalAmount),0) DESC";

            IEnumerable<DashBoardTabularData> DashBoardTabularData = db.Database.SqlQuery<DashBoardTabularData>(mQry).ToList();
            return DashBoardTabularData;
        }
        public IEnumerable<DashBoardTabularData> GetLoanBalanceDetailLedgerAccountWise()
        {
            mQry = @"SELECT VMain.LedgerAccountName AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.BalAmount))) AS Value
                    FROM ( " + GetLoanBalance() + @") As VMain
                    GROUP BY VMain.LedgerAccountName
                    ORDER BY IsNull(Sum(VMain.BalAmount),0) DESC";

            IEnumerable<DashBoardTabularData> DashBoardTabularData = db.Database.SqlQuery<DashBoardTabularData>(mQry).ToList();
            return DashBoardTabularData;
        }

















        public IEnumerable<DashBoardTrippleValue> GetSaleOrderStatus()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);
            SqlParameter SqlParameterTodayDate = new SqlParameter("@TodayDate", TodayDate);

            mQry = @"Select Convert(NVARCHAR,IsNull(Sum(VMain.HRisk),0)) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.LRisk),0)) AS Value2,
                    Convert(NVARCHAR,IsNull(Sum(VMain.NRisk),0)) AS Value3
                    From (" + GetSaleOrderStatusSummarySubQry(464) + ") As VMain ";

            IEnumerable<DashBoardTrippleValue> SaleOrder = db.Database.SqlQuery<DashBoardTrippleValue>(mQry, SqlParameterFromDate, SqlParameterToDate, SqlParameterTodayDate).ToList();
            return SaleOrder;
        }


        public IEnumerable<DashBoardSingleValue> GetExpense()
        {
            mQry = "SELECT Convert(nvarchar,LedgerAccountGroupId) As Value FROM Web.LedgerAccountGroups WHERE LedgerAccountGroupName = '" + Jobs.Constants.LedgerAccountGroup.LedgerAccountGroupConstants.DirectExpenses.LedgerAccountGroupName + "'";
            DashBoardSingleValue SundryDebtorsLedgerAccountGroup = db.Database.SqlQuery<DashBoardSingleValue>(mQry).FirstOrDefault();

            SqlParameter SqlParameterSiteId = new SqlParameter("@Site", (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@Division", (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", SoftwareStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", TodayDate);
            SqlParameter SqlParameterCostCenter = new SqlParameter("@CostCenter", (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccountGroup = new SqlParameter("@LedgerAccountGroup", SundryDebtorsLedgerAccountGroup.Value);

            mQry = new FinancialDisplayService(_unitOfWork).GetQryForTrialBalance(null, null, SoftwareStartDate.ToString(), TodayDate.ToString(), null, "False", "False", SundryDebtorsLedgerAccountGroup.Value) +
                                        @" SELECT Convert(NVARCHAR,IsNull(Sum(VMain.Balance),0)) As Value
                                        FROM
                                        (
                                            SELECT Sum(isnull(H.Balance,0)) AS Balance
                                            FROM cteAcGroup H 
                                            GROUP BY H.BaseLedgerAccountGroupId 

                                            UNION ALL 
                
                                            SELECT isnull(H.Balance,0) AS Balance
                                            FROM cteLedgerBalance H 
                                         ) As VMain ";


            IEnumerable<DashBoardSingleValue> DashBoardSingleValue = db.Database.SqlQuery<DashBoardSingleValue>(mQry, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterFromDate, SqlParameterToDate, SqlParameterCostCenter, SqlParameterLedgerAccountGroup).ToList();
            return DashBoardSingleValue;
        }
        public IEnumerable<DashBoardSingleValue> GetDebtors()
        {
            mQry = "SELECT Convert(nvarchar,LedgerAccountGroupId) As Value FROM Web.LedgerAccountGroups WHERE LedgerAccountGroupName = '"+ Jobs.Constants.LedgerAccountGroup.LedgerAccountGroupConstants.SundryDebtors.LedgerAccountGroupName + "'";
            DashBoardSingleValue SundryDebtorsLedgerAccountGroup = db.Database.SqlQuery<DashBoardSingleValue>(mQry).FirstOrDefault();

            SqlParameter SqlParameterSiteId = new SqlParameter("@Site", (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@Division", (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", SoftwareStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", TodayDate);
            SqlParameter SqlParameterCostCenter = new SqlParameter("@CostCenter", (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccountGroup = new SqlParameter("@LedgerAccountGroup", SundryDebtorsLedgerAccountGroup.Value);

            mQry = new FinancialDisplayService(_unitOfWork).GetQryForTrialBalance(null, null, SoftwareStartDate.ToString(), TodayDate.ToString(), null, "False", "False", SundryDebtorsLedgerAccountGroup.Value) +
                                        @" SELECT Convert(NVARCHAR,IsNull(Sum(VMain.Balance),0)) As Value
                                        FROM
                                        (
                                            SELECT Sum(isnull(H.Balance,0)) AS Balance
                                            FROM cteAcGroup H 
                                            GROUP BY H.BaseLedgerAccountGroupId 

                                            UNION ALL 
                
                                            SELECT isnull(H.Balance,0) AS Balance
                                            FROM cteLedgerBalance H 
                                         ) As VMain ";


            IEnumerable<DashBoardSingleValue> DashBoardSingleValue = db.Database.SqlQuery<DashBoardSingleValue>(mQry, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterFromDate, SqlParameterToDate, SqlParameterCostCenter, SqlParameterLedgerAccountGroup).ToList();
            return DashBoardSingleValue;
        }
        public IEnumerable<DashBoardSingleValue> GetCreditors()
        {
            mQry = "SELECT Convert(nvarchar,LedgerAccountGroupId) As Value FROM Web.LedgerAccountGroups WHERE LedgerAccountGroupName = '" + Jobs.Constants.LedgerAccountGroup.LedgerAccountGroupConstants.SundryCreditors.LedgerAccountGroupName + "'";
            DashBoardSingleValue SundryDebtorsLedgerAccountGroup = db.Database.SqlQuery<DashBoardSingleValue>(mQry).FirstOrDefault();

            SqlParameter SqlParameterSiteId = new SqlParameter("@Site", (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@Division", (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", SoftwareStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", TodayDate);
            SqlParameter SqlParameterCostCenter = new SqlParameter("@CostCenter", (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccountGroup = new SqlParameter("@LedgerAccountGroup", SundryDebtorsLedgerAccountGroup.Value);

            mQry = new FinancialDisplayService(_unitOfWork).GetQryForTrialBalance(null, null, SoftwareStartDate.ToString(), TodayDate.ToString(), null, "False", "False", SundryDebtorsLedgerAccountGroup.Value) +
                                        @" SELECT Convert(NVARCHAR,IsNull(-Sum(VMain.Balance),0)) As Value
                                        FROM
                                        (
                                            SELECT Sum(isnull(H.Balance,0)) AS Balance
                                            FROM cteAcGroup H 
                                            GROUP BY H.BaseLedgerAccountGroupId 

                                            UNION ALL 
                
                                            SELECT isnull(H.Balance,0) AS Balance
                                            FROM cteLedgerBalance H 
                                         ) As VMain ";


            IEnumerable<DashBoardSingleValue> DashBoardSingleValue = db.Database.SqlQuery<DashBoardSingleValue>(mQry, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterFromDate, SqlParameterToDate, SqlParameterCostCenter, SqlParameterLedgerAccountGroup).ToList();

            return DashBoardSingleValue;
        }
        public IEnumerable<DashBoardSingleValue> GetBankBalance()
        {
            mQry = "SELECT Convert(nvarchar,LedgerAccountGroupId) As Value FROM Web.LedgerAccountGroups WHERE LedgerAccountGroupName = '" + Jobs.Constants.LedgerAccountGroup.LedgerAccountGroupConstants.BankAccounts.LedgerAccountGroupName + "'";
            DashBoardSingleValue SundryDebtorsLedgerAccountGroup = db.Database.SqlQuery<DashBoardSingleValue>(mQry).FirstOrDefault();

            SqlParameter SqlParameterSiteId = new SqlParameter("@Site", (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@Division", (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", SoftwareStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", TodayDate);
            SqlParameter SqlParameterCostCenter = new SqlParameter("@CostCenter", (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccountGroup = new SqlParameter("@LedgerAccountGroup", SundryDebtorsLedgerAccountGroup.Value);

            mQry = new FinancialDisplayService(_unitOfWork).GetQryForTrialBalance(null, null, SoftwareStartDate.ToString(), TodayDate.ToString(), null, "False", "False", SundryDebtorsLedgerAccountGroup.Value) +
                                        @" SELECT Convert(NVARCHAR,IsNull(Sum(VMain.Balance),0)) As Value
                                        FROM
                                        (
                                            SELECT Sum(isnull(H.Balance,0)) AS Balance
                                            FROM cteAcGroup H 
                                            GROUP BY H.BaseLedgerAccountGroupId 

                                            UNION ALL 
                
                                            SELECT isnull(H.Balance,0) AS Balance
                                            FROM cteLedgerBalance H 
                                         ) As VMain ";


            IEnumerable<DashBoardSingleValue> DashBoardSingleValue = db.Database.SqlQuery<DashBoardSingleValue>(mQry, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterFromDate, SqlParameterToDate, SqlParameterCostCenter, SqlParameterLedgerAccountGroup).ToList();
            return DashBoardSingleValue;
        }
        public IEnumerable<DashBoardSingleValue> GetCashBalance()
        {
            mQry = "SELECT Convert(nvarchar,LedgerAccountGroupId) As Value FROM Web.LedgerAccountGroups WHERE LedgerAccountGroupName = '" + Jobs.Constants.LedgerAccountGroup.LedgerAccountGroupConstants.CashinHand.LedgerAccountGroupName + "'";
            DashBoardSingleValue SundryDebtorsLedgerAccountGroup = db.Database.SqlQuery<DashBoardSingleValue>(mQry).FirstOrDefault();

            SqlParameter SqlParameterSiteId = new SqlParameter("@Site", (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@Division", (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", SoftwareStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", TodayDate);
            SqlParameter SqlParameterCostCenter = new SqlParameter("@CostCenter", (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccountGroup = new SqlParameter("@LedgerAccountGroup", SundryDebtorsLedgerAccountGroup.Value);

            mQry = new FinancialDisplayService(_unitOfWork).GetQryForTrialBalance(null, null, SoftwareStartDate.ToString(), TodayDate.ToString(), null, "False", "False", SundryDebtorsLedgerAccountGroup.Value) +
                                        @" SELECT Convert(NVARCHAR,IsNull(Sum(VMain.Balance),0)) As Value
                                        FROM
                                        (
                                            SELECT Sum(isnull(H.Balance,0)) AS Balance
                                            FROM cteAcGroup H 
                                            GROUP BY H.BaseLedgerAccountGroupId 

                                            UNION ALL 
                
                                            SELECT isnull(H.Balance,0) AS Balance
                                            FROM cteLedgerBalance H 
                                         ) As VMain ";


            IEnumerable<DashBoardSingleValue> DashBoardSingleValue = db.Database.SqlQuery<DashBoardSingleValue>(mQry, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterFromDate, SqlParameterToDate, SqlParameterCostCenter, SqlParameterLedgerAccountGroup).ToList();
            return DashBoardSingleValue;
        }
        //public IEnumerable<DashBoardDoubleValue> GetWorkshopSale()
        //{
        //    SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
        //    SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);
        //    SqlParameter SqlParameterTodayDate = new SqlParameter("@TodayDate", TodayDate);

        //    mQry = @"Select Convert(NVARCHAR,IsNull(Sum(VMain.MonthSale),0)) AS Value1,
        //            Convert(NVARCHAR,IsNull(Sum(VMain.TodaySale),0)) AS Value2
        //            From (" + GetSaleInvoice(244) + ") As VMain ";

        //    IEnumerable<DashBoardDoubleValue> WorkshopSale = db.Database.SqlQuery<DashBoardDoubleValue>(mQry, SqlParameterFromDate, SqlParameterToDate, SqlParameterTodayDate).ToList();
        //    return WorkshopSale;
        //}
        //public IEnumerable<DashBoardDoubleValue> GetSpareSale()
        //{
        //    SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
        //    SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);
        //    SqlParameter SqlParameterTodayDate = new SqlParameter("@TodayDate", TodayDate);

        //    mQry = @"Select Convert(NVARCHAR,IsNull(Sum(VMain.MonthSale),0)) AS Value1,
        //            Convert(NVARCHAR,IsNull(Sum(VMain.TodaySale),0)) AS Value2
        //            From (" + GetSaleInvoice(4012) + ") As VMain ";

        //    IEnumerable<DashBoardDoubleValue> SpareSale = db.Database.SqlQuery<DashBoardDoubleValue>(mQry, SqlParameterFromDate, SqlParameterToDate, SqlParameterTodayDate).ToList();
        //    return SpareSale;
        //}




        
        public IEnumerable<DashBoardTabularData> GetDebtorsDetail()
        {
            mQry = "SELECT Convert(nvarchar,LedgerAccountGroupId) As Value FROM Web.LedgerAccountGroups WHERE LedgerAccountGroupName = '"+ Jobs.Constants.LedgerAccountGroup.LedgerAccountGroupConstants.SundryDebtors.LedgerAccountGroupName + "'";
            DashBoardSingleValue SundryDebtorsLedgerAccountGroup = db.Database.SqlQuery<DashBoardSingleValue>(mQry).FirstOrDefault();

            SqlParameter SqlParameterSiteId = new SqlParameter("@Site", (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@Division", (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", SoftwareStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", TodayDate);
            SqlParameter SqlParameterCostCenter = new SqlParameter("@CostCenter", (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccountGroup = new SqlParameter("@LedgerAccountGroup", SundryDebtorsLedgerAccountGroup.Value);

            mQry = new FinancialDisplayService(_unitOfWork).GetQryForTrialBalance(null, null, SoftwareStartDate.ToString(), TodayDate.ToString(), null, "False", "False", SundryDebtorsLedgerAccountGroup.Value) +
                                        @"SELECT H.BaseLedgerAccountGroupId AS LedgerAccountGroupId, Max(BaseLedgerAccountGroupName) AS Head, 
                                        Convert(NVARCHAR,Sum(isnull(H.Balance,0))) AS Value
                                        FROM cteAcGroup H 
                                        GROUP BY H.BaseLedgerAccountGroupId 
                                        Having Sum(isnull(H.Balance,0)) <> 0 

                                        UNION ALL 

                                        SELECT Ag.LedgerAccountGroupId AS LedgerAccountGroupId, Max(Ag.LedgerAccountGroupName) AS Head, 
                                        Convert(NVARCHAR,Sum(isnull(H.Balance,0)))  AS Value
                                        FROM cteLedgerBalance H 
                                        LEFT JOIN Web.LedgerAccounts A ON H.LedgerAccountId = A.LedgerAccountId
                                        LEFT JOIN Web.LedgerAccountGroups Ag On A.LedgerAccountGroupId = Ag.LedgerAccountGroupId
                                        Where isnull(H.Balance,0) <> 0 
                                        Group By Ag.LedgerAccountGroupId ";

            IEnumerable<DashBoardTabularData> DashBoardTabularData = db.Database.SqlQuery<DashBoardTabularData>(mQry, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterFromDate, SqlParameterToDate, SqlParameterCostCenter, SqlParameterLedgerAccountGroup).ToList();
            return DashBoardTabularData;
        }

        public IEnumerable<DashBoardTabularData> GetBankBalanceDetailBankAc()
        {
            mQry = "SELECT Convert(nvarchar,LedgerAccountGroupId) As Value FROM Web.LedgerAccountGroups WHERE LedgerAccountGroupName = '" + Jobs.Constants.LedgerAccountGroup.LedgerAccountGroupConstants.BankAccounts.LedgerAccountGroupName + "'";
            DashBoardSingleValue SundryDebtorsLedgerAccountGroup = db.Database.SqlQuery<DashBoardSingleValue>(mQry).FirstOrDefault();

            SqlParameter SqlParameterSiteId = new SqlParameter("@Site", (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@Division", (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", SoftwareStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", TodayDate);
            SqlParameter SqlParameterCostCenter = new SqlParameter("@CostCenter", (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccountGroup = new SqlParameter("@LedgerAccountGroup", SundryDebtorsLedgerAccountGroup.Value);

            mQry = new FinancialDisplayService(_unitOfWork).GetQryForTrialBalance(null, null, SoftwareStartDate.ToString(), TodayDate.ToString(), null, "False", "False", SundryDebtorsLedgerAccountGroup.Value) +
                                        @"SELECT H.BaseLedgerAccountGroupId AS LedgerAccountGroupId, Max(BaseLedgerAccountGroupName) AS Head, 
                                        Convert(NVARCHAR,Sum(isnull(H.Balance,0))) AS Value
                                        FROM cteAcGroup H 
                                        GROUP BY H.BaseLedgerAccountGroupId 
                                        Having Sum(isnull(H.Balance,0)) <> 0 

                                        UNION ALL 

                                        SELECT H.LedgerAccountId AS LedgerAccountGroupId, LedgerAccountName AS Head, 
                                        Convert(NVARCHAR,isnull(H.Balance,0))  AS Value
                                        FROM cteLedgerBalance H 
                                        Where isnull(H.Balance,0) <> 0 ";

            IEnumerable<DashBoardTabularData> DashBoardTabularData = db.Database.SqlQuery<DashBoardTabularData>(mQry, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterFromDate, SqlParameterToDate, SqlParameterCostCenter, SqlParameterLedgerAccountGroup).ToList();
            return DashBoardTabularData;
        }

        public IEnumerable<DashBoardTabularData> GetBankBalanceDetailBankODAc()
        {
            mQry = "SELECT Convert(nvarchar,LedgerAccountGroupId) As Value FROM Web.LedgerAccountGroups WHERE LedgerAccountGroupName = '" + Jobs.Constants.LedgerAccountGroup.LedgerAccountGroupConstants.BankODAc.LedgerAccountGroupName + "'";
            DashBoardSingleValue SundryDebtorsLedgerAccountGroup = db.Database.SqlQuery<DashBoardSingleValue>(mQry).FirstOrDefault();

            SqlParameter SqlParameterSiteId = new SqlParameter("@Site", (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@Division", (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", SoftwareStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", TodayDate);
            SqlParameter SqlParameterCostCenter = new SqlParameter("@CostCenter", (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccountGroup = new SqlParameter("@LedgerAccountGroup", SundryDebtorsLedgerAccountGroup.Value);

            mQry = new FinancialDisplayService(_unitOfWork).GetQryForTrialBalance(null, null, SoftwareStartDate.ToString(), TodayDate.ToString(), null, "False", "False", SundryDebtorsLedgerAccountGroup.Value) +
                                        @"SELECT H.BaseLedgerAccountGroupId AS LedgerAccountGroupId, Max(BaseLedgerAccountGroupName) AS Head, 
                                        Convert(NVARCHAR,Sum(isnull(H.Balance,0))) AS Value
                                        FROM cteAcGroup H 
                                        GROUP BY H.BaseLedgerAccountGroupId 
                                        Having Sum(isnull(H.Balance,0)) <> 0 

                                        UNION ALL 

                                        SELECT H.LedgerAccountId AS LedgerAccountGroupId, LedgerAccountName AS Head, 
                                        Convert(NVARCHAR,isnull(H.Balance,0))  AS Value
                                        FROM cteLedgerBalance H 
                                        Where isnull(H.Balance,0) <> 0 ";

            IEnumerable<DashBoardTabularData> DashBoardTabularData = db.Database.SqlQuery<DashBoardTabularData>(mQry, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterFromDate, SqlParameterToDate, SqlParameterCostCenter, SqlParameterLedgerAccountGroup).ToList();
            return DashBoardTabularData;
        }

        public IEnumerable<DashBoardTabularData> GetBankBalanceDetailChannelFinanceAc()
        {
            mQry = "SELECT Convert(nvarchar,LedgerAccountGroupId) As Value FROM Web.LedgerAccountGroups WHERE LedgerAccountGroupName = 'Sundry Debtors'";
            DashBoardSingleValue SundryDebtorsLedgerAccountGroup = db.Database.SqlQuery<DashBoardSingleValue>(mQry).FirstOrDefault();

            SqlParameter SqlParameterSiteId = new SqlParameter("@Site", (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@Division", (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", SoftwareStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", TodayDate);
            SqlParameter SqlParameterCostCenter = new SqlParameter("@CostCenter", (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccountGroup = new SqlParameter("@LedgerAccountGroup", SundryDebtorsLedgerAccountGroup.Value);

            mQry = new FinancialDisplayService(_unitOfWork).GetQryForTrialBalance(null, null, SoftwareStartDate.ToString(), TodayDate.ToString(), null, "False", "False", SundryDebtorsLedgerAccountGroup.Value) +
                                        @"SELECT H.BaseLedgerAccountGroupId AS LedgerAccountGroupId, Max(BaseLedgerAccountGroupName) AS Head, 
                                        Convert(NVARCHAR,Sum(isnull(H.Balance,0))) AS Value
                                        FROM cteAcGroup H 
                                        GROUP BY H.BaseLedgerAccountGroupId 
                                        Having Sum(isnull(H.Balance,0)) <> 0 

                                        UNION ALL 

                                        SELECT H.LedgerAccountId AS LedgerAccountGroupId, LedgerAccountName AS Head, 
                                        Convert(NVARCHAR,isnull(H.Balance,0))  AS Value
                                        FROM cteLedgerBalance H 
                                        Where isnull(H.Balance,0) <> 0 ";

            IEnumerable<DashBoardTabularData> DashBoardTabularData = db.Database.SqlQuery<DashBoardTabularData>(mQry, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterFromDate, SqlParameterToDate, SqlParameterCostCenter, SqlParameterLedgerAccountGroup).ToList();
            return DashBoardTabularData;
        }


        public IEnumerable<DashBoardTabularData> GetExpenseDetailLedgerAccountWise()
        {
            mQry = "SELECT Convert(nvarchar,LedgerAccountGroupId) As Value FROM Web.LedgerAccountGroups WHERE LedgerAccountGroupName = '" + Jobs.Constants.LedgerAccountGroup.LedgerAccountGroupConstants.DirectExpenses.LedgerAccountGroupName + "'";
            DashBoardSingleValue SundryDebtorsLedgerAccountGroup = db.Database.SqlQuery<DashBoardSingleValue>(mQry).FirstOrDefault();

            SqlParameter SqlParameterSiteId = new SqlParameter("@Site", (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@Division", (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", SoftwareStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", TodayDate);
            SqlParameter SqlParameterCostCenter = new SqlParameter("@CostCenter", (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccountGroup = new SqlParameter("@LedgerAccountGroup", SundryDebtorsLedgerAccountGroup.Value);

            mQry = new FinancialDisplayService(_unitOfWork).GetQryForTrialBalance(null, null, SoftwareStartDate.ToString(), TodayDate.ToString(), null, "False", "False", SundryDebtorsLedgerAccountGroup.Value) +
                                        @"SELECT H.BaseLedgerAccountGroupId AS LedgerAccountGroupId, Max(BaseLedgerAccountGroupName) AS Head, 
                                        Convert(NVARCHAR,Sum(isnull(H.Balance,0))) AS Value
                                        FROM cteAcGroup H 
                                        GROUP BY H.BaseLedgerAccountGroupId 
                                        Having Sum(isnull(H.Balance,0)) <> 0 

                                        UNION ALL 

                                        SELECT Ag.LedgerAccountGroupId AS LedgerAccountGroupId, Max(Ag.LedgerAccountGroupName) AS Head, 
                                        Convert(NVARCHAR,Sum(isnull(H.Balance,0)))  AS Value
                                        FROM cteLedgerBalance H 
                                        LEFT JOIN Web.LedgerAccounts A ON H.LedgerAccountId = A.LedgerAccountId
                                        LEFT JOIN Web.LedgerAccountGroups Ag On A.LedgerAccountGroupId = Ag.LedgerAccountGroupId
                                        Where isnull(H.Balance,0) <> 0 
                                        Group By Ag.LedgerAccountGroupId ";

            IEnumerable<DashBoardTabularData> DashBoardTabularData = db.Database.SqlQuery<DashBoardTabularData>(mQry, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterFromDate, SqlParameterToDate, SqlParameterCostCenter, SqlParameterLedgerAccountGroup).ToList();
            return DashBoardTabularData;
        }
        public IEnumerable<DashBoardTabularData> GetExpenseDetailBranchWise()
        {
            mQry = "SELECT Convert(nvarchar,LedgerAccountGroupId) As Value FROM Web.LedgerAccountGroups WHERE LedgerAccountGroupName = '" + Jobs.Constants.LedgerAccountGroup.LedgerAccountGroupConstants.DirectExpenses.LedgerAccountGroupName + "'";
            DashBoardSingleValue SundryDebtorsLedgerAccountGroup = db.Database.SqlQuery<DashBoardSingleValue>(mQry).FirstOrDefault();

            SqlParameter SqlParameterAsOnDate = new SqlParameter("@AsOnDate", TodayDate);
            SqlParameter SqlParameterLedgerAccountGroup = new SqlParameter("@LedgerAccountGroupId", SundryDebtorsLedgerAccountGroup.Value);

            mQry = GetLedgerAccountHierarchySubQry() +
                    @"SELECT S.SiteName AS Head, 
                        Convert(NVARCHAR,IsNull(Sum(L.AmtDr),0) - IsNull(Sum(L.AmtCr),0)) AS Value
                        FROM CTE C
                        LEFT JOIN Web.LedgerAccounts A ON C.LedgerAccountGroupId = A.LedgerAccountGroupId
                        LEFT JOIN Web.Ledgers L ON A.LedgerAccountId = L.LedgerAccountId
                        LEFT JOIN Web.LedgerHeaders H ON L.LedgerHeaderId = H.LedgerHeaderId
                        LEFT JOIN Web.Sites S ON H.SiteId = S.SiteId
                        WHERE H.DocDate <= getdate()
                        GROUP BY S.SiteName
                        HAVING IsNull(Sum(L.AmtDr),0) - IsNull(Sum(L.AmtCr),0) <> 0 ";

            IEnumerable<DashBoardTabularData> DashBoardTabularData = db.Database.SqlQuery<DashBoardTabularData>(mQry, SqlParameterAsOnDate, SqlParameterLedgerAccountGroup).ToList();
            return DashBoardTabularData;
        }
        public IEnumerable<DashBoardTabularData> GetExpenseDetailCostCenterWise()
        {
            mQry = "SELECT Convert(nvarchar,LedgerAccountGroupId) As Value FROM Web.LedgerAccountGroups WHERE LedgerAccountGroupName = '" + Jobs.Constants.LedgerAccountGroup.LedgerAccountGroupConstants.DirectExpenses.LedgerAccountGroupName + "'";
            DashBoardSingleValue SundryDebtorsLedgerAccountGroup = db.Database.SqlQuery<DashBoardSingleValue>(mQry).FirstOrDefault();

            SqlParameter SqlParameterAsOnDate = new SqlParameter("@AsOnDate", TodayDate);
            SqlParameter SqlParameterLedgerAccountGroup = new SqlParameter("@LedgerAccountGroupId", SundryDebtorsLedgerAccountGroup.Value);

            mQry = GetLedgerAccountHierarchySubQry() +
                    @"SELECT CT.CostCenterName AS Head, 
                        Convert(NVARCHAR,IsNull(Sum(L.AmtDr),0) - IsNull(Sum(L.AmtCr),0)) AS Value
                        FROM CTE C
                        LEFT JOIN Web.LedgerAccounts A ON C.LedgerAccountGroupId = A.LedgerAccountGroupId
                        LEFT JOIN Web.Ledgers L ON A.LedgerAccountId = L.LedgerAccountId
                        LEFT JOIN Web.LedgerHeaders H ON L.LedgerHeaderId = H.LedgerHeaderId
                        LEFT JOIN Web.CostCenters CT ON L.CostCenterId = CT.CostCenterId
                        WHERE H.DocDate <= getdate()
                        GROUP BY CT.CostCenterName
                        HAVING IsNull(Sum(L.AmtDr),0) - IsNull(Sum(L.AmtCr),0) <> 0 ";

            IEnumerable<DashBoardTabularData> DashBoardTabularData = db.Database.SqlQuery<DashBoardTabularData>(mQry, SqlParameterAsOnDate, SqlParameterLedgerAccountGroup).ToList();
            return DashBoardTabularData;
        }

        public IEnumerable<DashBoardTabularData> GetCreditorsDetail()
        {
            mQry = "SELECT Convert(nvarchar,LedgerAccountGroupId) As Value FROM Web.LedgerAccountGroups WHERE LedgerAccountGroupName = '" + Jobs.Constants.LedgerAccountGroup.LedgerAccountGroupConstants.SundryCreditors.LedgerAccountGroupName + "'";
            DashBoardSingleValue SundryDebtorsLedgerAccountGroup = db.Database.SqlQuery<DashBoardSingleValue>(mQry).FirstOrDefault();

            SqlParameter SqlParameterSiteId = new SqlParameter("@Site", (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@Division", (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", SoftwareStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", TodayDate);
            SqlParameter SqlParameterCostCenter = new SqlParameter("@CostCenter", (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccountGroup = new SqlParameter("@LedgerAccountGroup", SundryDebtorsLedgerAccountGroup.Value);

            mQry = new FinancialDisplayService(_unitOfWork).GetQryForTrialBalance(null, null, SoftwareStartDate.ToString(), TodayDate.ToString(), null, "False", "False", SundryDebtorsLedgerAccountGroup.Value) +
                                        @"SELECT H.BaseLedgerAccountGroupId AS LedgerAccountGroupId, Max(BaseLedgerAccountGroupName) AS Head, 
                                        Convert(NVARCHAR,-Sum(isnull(H.Balance,0))) AS Value
                                        FROM cteAcGroup H 
                                        GROUP BY H.BaseLedgerAccountGroupId 
                                        Having Sum(isnull(H.Balance,0)) <> 0 

                                        UNION ALL 

                                        SELECT Ag.LedgerAccountGroupId AS LedgerAccountGroupId, Max(Ag.LedgerAccountGroupName) AS Head, 
                                        Convert(NVARCHAR,-Sum(isnull(H.Balance,0)))  AS Value
                                        FROM cteLedgerBalance H 
                                        LEFT JOIN Web.LedgerAccounts A ON H.LedgerAccountId = A.LedgerAccountId
                                        LEFT JOIN Web.LedgerAccountGroups Ag On A.LedgerAccountGroupId = Ag.LedgerAccountGroupId
                                        Where isnull(H.Balance,0) <> 0 
                                        Group By Ag.LedgerAccountGroupId ";

            IEnumerable<DashBoardTabularData> DashBoardTabularData = db.Database.SqlQuery<DashBoardTabularData>(mQry, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterFromDate, SqlParameterToDate, SqlParameterCostCenter, SqlParameterLedgerAccountGroup).ToList();
            return DashBoardTabularData;
        }

        public IEnumerable<DashBoardTabularData> GetCashBalanceDetailLedgerAccountWise()
        {
            mQry = "SELECT Convert(nvarchar,LedgerAccountGroupId) As Value FROM Web.LedgerAccountGroups WHERE LedgerAccountGroupName = '" + Jobs.Constants.LedgerAccountGroup.LedgerAccountGroupConstants.CashinHand.LedgerAccountGroupName + "'";
            DashBoardSingleValue SundryDebtorsLedgerAccountGroup = db.Database.SqlQuery<DashBoardSingleValue>(mQry).FirstOrDefault();

            SqlParameter SqlParameterSiteId = new SqlParameter("@Site", (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@Division", (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", SoftwareStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", TodayDate);
            SqlParameter SqlParameterCostCenter = new SqlParameter("@CostCenter", (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccountGroup = new SqlParameter("@LedgerAccountGroup", SundryDebtorsLedgerAccountGroup.Value);

            mQry = new FinancialDisplayService(_unitOfWork).GetQryForTrialBalance(null, null, SoftwareStartDate.ToString(), TodayDate.ToString(), null, "False", "False", SundryDebtorsLedgerAccountGroup.Value) +
                                        @"SELECT H.LedgerAccountId AS LedgerAccountGroupId, LedgerAccountName AS Head, 
                                        Convert(NVARCHAR,isnull(H.Balance,0))  AS Value
                                        FROM cteLedgerBalance H 
                                        Where isnull(H.Balance,0) <> 0 ";

            IEnumerable<DashBoardTabularData> DashBoardTabularData = db.Database.SqlQuery<DashBoardTabularData>(mQry, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterFromDate, SqlParameterToDate, SqlParameterCostCenter, SqlParameterLedgerAccountGroup).ToList();
            return DashBoardTabularData;
        }
        public IEnumerable<DashBoardTabularData> GetCashBalanceDetailBranchWise()
        {
            mQry = "SELECT Convert(nvarchar,LedgerAccountGroupId) As Value FROM Web.LedgerAccountGroups WHERE LedgerAccountGroupName = '" + Jobs.Constants.LedgerAccountGroup.LedgerAccountGroupConstants.CashinHand.LedgerAccountGroupName + "'";
            DashBoardSingleValue SundryDebtorsLedgerAccountGroup = db.Database.SqlQuery<DashBoardSingleValue>(mQry).FirstOrDefault();

            SqlParameter SqlParameterAsOnDate = new SqlParameter("@AsOnDate", TodayDate);
            SqlParameter SqlParameterLedgerAccountGroup = new SqlParameter("@LedgerAccountGroupId", SundryDebtorsLedgerAccountGroup.Value);

            mQry = GetLedgerAccountHierarchySubQry() +
                    @"SELECT S.SiteName AS Head, 
                        Convert(NVARCHAR,IsNull(Sum(L.AmtDr),0) - IsNull(Sum(L.AmtCr),0))  AS Value
                        FROM CTE C
                        LEFT JOIN Web.LedgerAccounts A ON C.LedgerAccountGroupId = A.LedgerAccountGroupId
                        LEFT JOIN Web.Ledgers L ON A.LedgerAccountId = L.LedgerAccountId
                        LEFT JOIN Web.LedgerHeaders H ON L.LedgerHeaderId = H.LedgerHeaderId
                        LEFT JOIN Web.Sites S ON H.SiteId = S.SiteId
                        WHERE H.DocDate <= getdate()
                        GROUP BY S.SiteName
                        HAVING IsNull(Sum(L.AmtDr),0) - IsNull(Sum(L.AmtCr),0) <> 0 ";

            IEnumerable<DashBoardTabularData> DashBoardTabularData = db.Database.SqlQuery<DashBoardTabularData>(mQry, SqlParameterAsOnDate, SqlParameterLedgerAccountGroup).ToList();
            return DashBoardTabularData;
        }



        //public IEnumerable<DashBoardTabularData> GetWorkshopSaleDetailProductTypeWise()
        //{
        //    SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
        //    SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);

        //    mQry = @"SELECT VMain.ProductType AS Head, Convert(NVARCHAR,IsNull(Sum(VMain.Amount),0)) AS Value
        //            FROM ( " + GetSaleDetailSubQry(244) + @") As VMain
        //            GROUP BY VMain.ProductType
        //            ORDER BY VMain.ProductType ";

        //    IEnumerable<DashBoardTabularData> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData>(mQry, SqlParameterFromDate, SqlParameterToDate).ToList();
        //    return DashBoardTabularData_ThreeColumns;
        //}
        //public IEnumerable<DashBoardTabularData> GetWorkshopSaleDetailProductGroupWise()
        //{
        //    SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
        //    SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);

        //    mQry = @"SELECT VMain.ProductGroup AS Head, Convert(NVARCHAR,IsNull(Sum(VMain.Amount),0)) AS Value
        //            FROM ( " + GetSaleDetailSubQry(244) + @") As VMain
        //            GROUP BY VMain.ProductGroup
        //            ORDER BY VMain.ProductGroup ";

        //    IEnumerable<DashBoardTabularData> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData>(mQry, SqlParameterFromDate, SqlParameterToDate).ToList();
        //    return DashBoardTabularData_ThreeColumns;
        //}

        //public IEnumerable<DashBoardTabularData> GetSpareSaleDetailProductTypeWise()
        //{
        //    SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
        //    SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);

        //    mQry = @"SELECT VMain.ProductType AS Head, Convert(NVARCHAR,IsNull(Sum(VMain.Amount),0)) AS Value
        //            FROM ( " + GetSaleDetailSubQry(4012) + @") As VMain
        //            GROUP BY VMain.ProductType
        //            ORDER BY VMain.ProductType ";

        //    IEnumerable<DashBoardTabularData> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData>(mQry, SqlParameterFromDate, SqlParameterToDate).ToList();
        //    return DashBoardTabularData_ThreeColumns;
        //}
        //public IEnumerable<DashBoardTabularData> GetSpareSaleDetailProductGroupWise()
        //{
        //    SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
        //    SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);

        //    mQry = @"SELECT VMain.ProductGroup AS Head, Convert(NVARCHAR,IsNull(Sum(VMain.Amount),0)) AS Value
        //            FROM ( " + GetSaleDetailSubQry(4012) + @") As VMain
        //            GROUP BY VMain.ProductGroup
        //            ORDER BY VMain.ProductGroup ";

        //    IEnumerable<DashBoardTabularData> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData>(mQry, SqlParameterFromDate, SqlParameterToDate).ToList();
        //    return DashBoardTabularData_ThreeColumns;
        //}









        public IEnumerable<DashBoardPieChartData> GetSpareSalePieChartData()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);

            mQry = @"SELECT S.SiteName As label, Round(Sum(Hc.Amount)/100000,2) AS value,
                    CASE WHEN row_number() OVER (ORDER BY S.SiteName) = 1 THEN '#f56954'
	                     WHEN row_number() OVER (ORDER BY S.SiteName) = 2 THEN '#00a65a'
	                     WHEN row_number() OVER (ORDER BY S.SiteName) = 3 THEN '#f39c12'
	                     WHEN row_number() OVER (ORDER BY S.SiteName) = 4 THEN '#00c0ef'
	                     WHEN row_number() OVER (ORDER BY S.SiteName) = 5 THEN '#3c8dbc'
	                     WHEN row_number() OVER (ORDER BY S.SiteName) = 6 THEN '#d2d6de'
	                     ELSE '#f56954'
                    END AS color 
                    FROM Web.SaleInvoiceHeaders H 
                    LEFT JOIN Web.SaleInvoiceHeaderCharges Hc ON H.SaleInvoiceHeaderId = Hc.HeaderTableId
                    LEFT JOIN Web.DocumentTypes D ON H.DocTypeId = D.DocumentTypeId
                    LEFT JOIN Web.Charges C ON Hc.ChargeId = C.ChargeId
                    LEFT JOIN Web.Sites S ON h.SiteId = S.SiteId
                    WHERE C.ChargeName = 'Net Amount'
                    AND  H.DocDate BETWEEN @FromDate AND @ToDate
                    AND D.DocumentCategoryId = 4012
                    GROUP BY S.SiteName ";

            IEnumerable<DashBoardPieChartData> SaleSalePieChartData = db.Database.SqlQuery<DashBoardPieChartData>(mQry, SqlParameterFromDate, SqlParameterToDate).ToList();
            return SaleSalePieChartData;
        }
        public IEnumerable<DashBoardSaleBarChartData> GetSpareSaleBarChartData()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", YearStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", YearEndDate);


            mQry = @"SELECT LEFT(DATENAME(month, H.DocDate),3) AS Month, 
                    Round(Sum(Hc.Amount)/100000,2) AS Amount
                    FROM Web.SaleInvoiceHeaders H 
                    LEFT JOIN Web.SaleInvoiceHeaderCharges Hc ON H.SaleInvoiceHeaderId = Hc.HeaderTableId
                    LEFT JOIN Web.DocumentTypes D ON H.DocTypeId = D.DocumentTypeId
                    LEFT JOIN Web.Charges C ON Hc.ChargeId = C.ChargeId
                    WHERE C.ChargeName = 'Net Amount'
                    AND D.DocumentCategoryId = 4012
                    AND  H.DocDate BETWEEN @FromDate AND @ToDate
                    GROUP BY DATENAME(month, H.DocDate)
                    ORDER BY DatePart(Year,Max(H.DocDate)) + Convert(DECIMAL(18,2),DatePart(month,Max(H.DocDate))) / 100 ";

            IEnumerable<DashBoardSaleBarChartData> ChartData = db.Database.SqlQuery<DashBoardSaleBarChartData>(mQry, SqlParameterFromDate, SqlParameterToDate).ToList();
            return ChartData;
        }



        public string GetSaleDetailSubQry(int DocumentCategoryId)
        {
            mQry = @"SELECT H.DocNo, H.DocDate,  L.Qty AS Qty, (L.Qty)* VPS.StandardSizeArea Area,L.Amount AS Amount,
					D.DocumentTypeName AS DocumentTypeName, B.Name AS Buyer, P.ProductName, PC.ProductCategoryName , PG.ProductGroupName AS ProductGroup, PT.ProductTypeName as ProductType
                    FROM Web.SaleInvoiceHeaders H WITH (Nolock) 
                    LEFT JOIN Web.SaleInvoiceLines L WITH (Nolock) ON L.SaleInvoiceHeaderId = H.SaleInvoiceHeaderId
                    LEFT JOIN Web.DocumentTypes D WITH (Nolock) ON H.DocTypeId = D.DocumentTypeId
                    LEFT JOIN Web.SaleDispatchLines SL WITH (Nolock) ON SL.SaleDispatchLineId = L.SaleDispatchLineId
                    LEFT JOIN web.PackingLines PL WITH (Nolock) ON PL.PackingLineId = SL.PackingLineId
                    LEFT JOIN Web.ViewProductSize VPS WITH (Nolock) ON VPS.ProductId = PL.ProductId
                    LEFT JOIN web.People B WITH (Nolock) ON B.PersonID = H.SaleToBuyerId
                    LEFT JOIN web.Products P WITH (Nolock) ON P.ProductId = L.ProductId
                    LEFT JOIN web.ProductCategories PC WITH (Nolock) ON PC.ProductCategoryId = P.ProductCategoryId 
                    LEFT JOIN web.ProductGroups PG WITH (Nolock) ON PG.ProductGroupId = P.ProductGroupId
                    LEFT JOIN web.ProductTypes PT WITH (Nolock) ON PT.ProductTypeId = PG.ProductTypeId
                    WHERE H.DocDate BETWEEN @FromDate AND @ToDate ";

            return mQry;
        }

        public string GetSaleOrderBalanceDetailSubQry(int DocumentCategoryId)
        {
            mQry = @"SELECT H.DocNo, H.DocDate,  L.Qty - isnull(CL.CQty,0) AS Qty, (L.Qty - isnull(CL.CQty,0))* VPS.StandardSizeArea Area,
					D.DocumentTypeName AS DocumentTypeName, B.Name AS Buyer, P.ProductName, PC.ProductCategoryName 
                    FROM Web.SaleOrderHeaders H WITH (Nolock)
                    LEFT JOIN Web.SaleOrderLines L WITH (Nolock) ON H.SaleOrderHeaderId = L.SaleOrderHeaderId
                    LEFT JOIN Web.DocumentTypes D WITH (Nolock) ON H.DocTypeId = D.DocumentTypeId
                    LEFT JOIN Web.ViewProductSize VPS WITH (Nolock) ON VPS.ProductId = L.ProductId
                    LEFT JOIN web.People B WITH (Nolock) ON B.PersonID = H.SaleToBuyerId
                    LEFT JOIN web.Products P WITH (Nolock) ON P.ProductId = L.ProductId
                    LEFT JOIN web.ProductCategories PC WITH (Nolock) ON PC.ProductCategoryId = P.ProductCategoryId 
                    LEFT JOIN 
                    (
                    SELECT CL.SaleOrderLineId, sum(CL.Qty) AS CQty  FROM web.SaleOrderCancelLines CL WITH (Nolock) GROUP BY CL.SaleOrderLineId                    
                    ) CL ON CL.SaleOrderLineId = L.SaleOrderLineId
                    WHERE 1=1 AND L.Qty - isnull(CL.CQty,0) > 0
                    AND  H.DocDate BETWEEN @FromDate AND @ToDate ";

            return mQry;
        }

        public string GetLedgerAccountHierarchySubQry()
        {
            mQry = @"WITH CTE AS (
                      SELECT *, LedgerAccountGroupId as TopParent FROM Web.LedgerAccountGroups WHERE LedgerAccountGroupId = @LedgerAccountGroupId 
                      UNION ALL
                      SELECT Ag.*, C.TopParent 
                      FROM Web.LedgerAccountGroups Ag
                      JOIN CTE C on C.LedgerAccountGroupId = Ag.ParentLedgerAccountGroupId
                      WHERE Ag.LedgerAccountGroupId <> Ag.ParentLedgerAccountGroupId
                    ) ";

            return mQry;
        }





        public string GetSaleOrderStatusSummarySubQry(int DocumentCategoryId)
        {
            mQry = @"SELECT 
                    IsNull(Sum(Case When H.DueDate  < @TodayDate Then (L.Qty - isnull(CL.CQty,0) - isnull(SL.Qty,0))*VPS.StandardSizeArea Else 0 End),0) AS HRisk,
                    IsNull(Sum(Case When H.DueDate  BETWEEN dateadd(Day,-30,@TodayDate) AND  @TodayDate Then (L.Qty - isnull(CL.CQty,0) - isnull(SL.Qty,0))*VPS.StandardSizeArea Else 0 End),0) AS LRisk,
                    IsNull(Sum(Case When H.DueDate  > @TodayDate  Then (L.Qty - isnull(CL.CQty,0) - isnull(SL.Qty,0))*VPS.StandardSizeArea Else 0 End),0) AS NRisk
                    FROM Web.SaleOrderHeaders H WITH (Nolock)
                    LEFT JOIN Web.SaleOrderLines L WITH (Nolock) ON H.SaleOrderHeaderId = L.SaleOrderHeaderId
                    LEFT JOIN Web.DocumentTypes D WITH (Nolock) ON H.DocTypeId = D.DocumentTypeId
                    LEFT JOIN Web.ViewProductSize VPS WITH (Nolock) ON VPS.ProductId = L.ProductId
                    LEFT JOIN 
                    (
                    SELECT CL.SaleOrderLineId, sum(CL.Qty) AS CQty  FROM web.SaleOrderCancelLines CL WITH (Nolock) GROUP BY CL.SaleOrderLineId                    
                    ) CL ON CL.SaleOrderLineId = L.SaleOrderLineId
                    LEFT JOIN 
                    (
                    SELECT PL.SaleOrderLineId, sum(L.Qty) AS Qty  
                    FROM web.SaleInvoiceHeaders H WITH (Nolock)
                    LEFT JOIN web.SaleInvoiceLines L WITH (Nolock) ON L.SaleInvoiceHeaderId = H.SaleInvoiceHeaderId
                    LEFT JOIN web.SaleDispatchLines SL WITH (Nolock) ON SL.SaleDispatchLineId = L.SaleDispatchLineId
                    LEFT JOIN web.PackingLines PL WITH (Nolock) ON PL.PackingLineId = SL.PackingLineId
                    GROUP BY PL.SaleOrderLineId                    
                    ) SL ON SL.SaleOrderLineId = L.SaleOrderLineId
                    WHERE 1=1
                    AND L.Qty - isnull(CL.CQty,0) - isnull(SL.Qty,0) > 0";
            return mQry;
        }

        public string GetFormattedValue(string FieldName)
        {
            string Value = @" SELECT 
                            CASE WHEN IsNull(@Value, 0) <= 100000 THEN Convert(NVARCHAR, Convert(DECIMAL(18, 2), Round(IsNull(@Value, 0) / 1000, 2))) +' K'
                                WHEN IsNull(@Value,0) <= 10000000 THEN Convert(NVARCHAR, Convert(DECIMAL(18, 2), Round(IsNull(@Value, 0) / 100000, 2))) +' L'
     ELSE Convert(NVARCHAR, Convert(DECIMAL(18, 2), Round(IsNull(@Value, 0) / 10000000, 2)))+' C' END     ";

            return Value;
        }

        public void Dispose()
        {
        }
    }


    





}
