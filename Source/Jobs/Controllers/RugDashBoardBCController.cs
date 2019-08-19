using System.Collections.Generic;
using System.Web.Mvc;
using Service;
using System.Data.SqlClient;
using System.Data;
using System;

namespace Module
{
    [Authorize]
    public class RugDashBoardBCController : Controller
    {
        IRugDashBoardBCService _RugDashBoardBCService;
        public RugDashBoardBCController(IRugDashBoardBCService RugDashBoardBCService)
        {
            _RugDashBoardBCService = RugDashBoardBCService;
        }

        public ActionResult RugDashBoardBC()
        {
            return View();
        }

        //1 Block
        public JsonResult GetSaleOrderBalanceSummary()
        {
            IEnumerable<DashBoardDoubleValue> SaleOrderBalanceSummary = _RugDashBoardBCService.GetSaleOrderBalanceSummary();

            JsonResult json = Json(new { Success = true, Data = SaleOrderBalanceSummary }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetSaleOrderBalanceDetailCategoryWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardSaleOrderBalanceDetailCategoryWise = _RugDashBoardBCService.GetSaleOrderBalanceDetailCategoryWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardSaleOrderBalanceDetailCategoryWise }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetSaleOrderBalanceDetailQualityWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardSaleOrderBalanceDetailDetailQualityWise = _RugDashBoardBCService.GetSaleOrderBalanceDetailQualityWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardSaleOrderBalanceDetailDetailQualityWise }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetSaleOrderBalanceDetailBuyerWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardSaleOrderBalanceDetailBuyerWise = _RugDashBoardBCService.GetSaleOrderBalanceDetailBuyerWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardSaleOrderBalanceDetailBuyerWise }, JsonRequestBehavior.AllowGet);
            return json;
        }


        //2 Block
        public JsonResult GetUnExecuteSummary()
        {
            IEnumerable<DashBoardDoubleValue> UnExecuteSummary = _RugDashBoardBCService.GetUnExecuteSummary();

            JsonResult json = Json(new { Success = true, Data = UnExecuteSummary }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetUnExecuteDetailCategoryWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardUnExecuteetailCategoryWise = _RugDashBoardBCService.GetUnExecuteDetailCategoryWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardUnExecuteetailCategoryWise }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetUnExecuteDetailBranchWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardUnExecuteetailDetailJobWorkerWise = _RugDashBoardBCService.GetUnExecuteDetailBranchWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardUnExecuteetailDetailJobWorkerWise }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetUnExecuteDetailBuyerWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardUnExecuteetailBuyerWise = _RugDashBoardBCService.GetUnExecuteDetailBuyerWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardUnExecuteetailBuyerWise }, JsonRequestBehavior.AllowGet);
            return json;
        }

        //3 Block
        public JsonResult GetSaleInvoiceSummary()
        {
            IEnumerable<DashBoardTrippleValue> SaleInvoice = _RugDashBoardBCService.GetSaleInvoiceSummary();

            JsonResult json = Json(new { Success = true, Data = SaleInvoice }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetSaleInvoiceDetailCategoryWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardSaleInvoiceDetailCategoryWise = _RugDashBoardBCService.GetSaleInvoiceDetailCategoryWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardSaleInvoiceDetailCategoryWise }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetSaleInvoiceDetailQualityWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardSaleInvoiceDetailQualityWise = _RugDashBoardBCService.GetSaleInvoiceDetailQualityWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardSaleInvoiceDetailQualityWise }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetSaleInvoiceDetailBuyerWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardSaleInvoiceDetailBuyerWise = _RugDashBoardBCService.GetSaleInvoiceDetailBuyerWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardSaleInvoiceDetailBuyerWise }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetSaleInvoiceChartData()
        {
            IEnumerable<DashBoardSaleBarChartData> VehicleSaleChartData = _RugDashBoardBCService.GetSaleInvoiceBarChartData();

            JsonResult json = Json(new { Success = true, Data = VehicleSaleChartData }, JsonRequestBehavior.AllowGet);
            return json;
        }


        //5 Block
        public JsonResult GetStockSummary()
        {
            IEnumerable<DashBoardDoubleValue> StockSummary = _RugDashBoardBCService.GetStockSummary();

            JsonResult json = Json(new { Success = true, Data = StockSummary }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetStockDetailCategoryWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardStocketailCategoryWise = _RugDashBoardBCService.GetStockDetailCategoryWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardStocketailCategoryWise }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetStockDetailQualityWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardStocketailDetailQualityWise = _RugDashBoardBCService.GetStockDetailQualityWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardStocketailDetailQualityWise }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetStockDetailBuyerWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardStocketailBuyerWise = _RugDashBoardBCService.GetStockDetailBuyerWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardStocketailBuyerWise }, JsonRequestBehavior.AllowGet);
            return json;
        }



        //6 Block
        public JsonResult GetToBeIssueSummary()
        {
            IEnumerable<DashBoardDoubleValue> ToBeIssueSummary = _RugDashBoardBCService.GetToBeIssueSummary();

            JsonResult json = Json(new { Success = true, Data = ToBeIssueSummary }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetToBeIssueDetailCategoryWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardToBeIssueetailCategoryWise = _RugDashBoardBCService.GetToBeIssueDetailCategoryWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardToBeIssueetailCategoryWise }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetToBeIssueDetailQualityWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardToBeIssueetailDetailJobWorkerWise = _RugDashBoardBCService.GetToBeIssueDetailQualityWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardToBeIssueetailDetailJobWorkerWise }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetToBeIssueDetailBuyerWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardToBeIssueetailBuyerWise = _RugDashBoardBCService.GetToBeIssueDetailBuyerWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardToBeIssueetailBuyerWise }, JsonRequestBehavior.AllowGet);
            return json;
        }


        //7 Block
        public JsonResult GetProductionSummary()
        {
            IEnumerable<DashBoardTrippleValue> Production = _RugDashBoardBCService.GetProductionSummary();

            JsonResult json = Json(new { Success = true, Data = Production }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetProductionDetailCategoryWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardProductionDetailCategoryWise = _RugDashBoardBCService.GetProductionDetailCategoryWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardProductionDetailCategoryWise }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetProductionDetailQualityWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardProductionDetailQualityWise = _RugDashBoardBCService.GetProductionDetailQualityWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardProductionDetailQualityWise }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetProductionDetailBuyerWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardProductionDetailBuyerWise = _RugDashBoardBCService.GetProductionDetailBuyerWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardProductionDetailBuyerWise }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetProductionPieChartData()
        {
            IEnumerable<DashBoardPieChartData> VehiclePieChartData = _RugDashBoardBCService.GetProductionPieChartData();

            JsonResult json = Json(new { Success = true, Data = VehiclePieChartData }, JsonRequestBehavior.AllowGet);
            return json;
        }



        //9 Block
        public JsonResult GetOnLoomSummary()
        {
            IEnumerable<DashBoardDoubleValue> OnLoomSummary = _RugDashBoardBCService.GetOnLoomSummary();

            JsonResult json = Json(new { Success = true, Data = OnLoomSummary }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetOnLoomDetailCategoryWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardOnLoometailCategoryWise = _RugDashBoardBCService.GetOnLoomDetailCategoryWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardOnLoometailCategoryWise }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetOnLoomDetailJobWorkerWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardOnLoometailDetailJobWorkerWise = _RugDashBoardBCService.GetOnLoomDetailJobWorkerWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardOnLoometailDetailJobWorkerWise }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetOnLoomDetailBuyerWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardOnLoometailBuyerWise = _RugDashBoardBCService.GetOnLoomDetailBuyerWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardOnLoometailBuyerWise }, JsonRequestBehavior.AllowGet);
            return json;
        }



        //10 Block
        public JsonResult GetDyeingOrderBalanceSummary()
        {
            IEnumerable<DashBoardSingleValue> DyeingOrderBalanceSummary = _RugDashBoardBCService.GetDyeingOrderBalanceSummary();

            JsonResult json = Json(new { Success = true, Data = DyeingOrderBalanceSummary }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetDyeingOrderBalanceDetailProductWise()
        {
            IEnumerable<DashBoardTabularData> DashBoardDyeingOrderBalanceDetailCategoryWise = _RugDashBoardBCService.GetDyeingOrderBalanceDetailProductWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardDyeingOrderBalanceDetailCategoryWise }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetDyeingOrderBalanceDetailMonthWise()
        {
            IEnumerable<DashBoardTabularData> DashBoardDyeingOrderBalanceDetailDetailQualityWise = _RugDashBoardBCService.GetDyeingOrderBalanceDetailMonthWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardDyeingOrderBalanceDetailDetailQualityWise }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetDyeingOrderBalanceDetailJobWorkerWise()
        {
            IEnumerable<DashBoardTabularData> DashBoardDyeingOrderBalanceDetailBuyerWise = _RugDashBoardBCService.GetDyeingOrderBalanceDetailJobWorkerWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardDyeingOrderBalanceDetailBuyerWise }, JsonRequestBehavior.AllowGet);
            return json;
        }





        //12 Block
        public JsonResult GetLoanBalanceSummary()
        {
            IEnumerable<DashBoardSingleValue> LoanBalanceSummary = _RugDashBoardBCService.GetLoanBalanceSummary();

            JsonResult json = Json(new { Success = true, Data = LoanBalanceSummary }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetLoanBalanceDetailDepartmentWise()
        {
            IEnumerable<DashBoardTabularData> DashBoardLoanBalanceDetailCategoryWise = _RugDashBoardBCService.GetLoanBalanceDetailDepartmentWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardLoanBalanceDetailCategoryWise }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetLoanBalanceDetailMonthWise()
        {
            IEnumerable<DashBoardTabularData> DashBoardLoanBalanceDetailDetailQualityWise = _RugDashBoardBCService.GetLoanBalanceDetailMonthWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardLoanBalanceDetailDetailQualityWise }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetLoanBalanceDetailLedgerAccountWise()
        {
            IEnumerable<DashBoardTabularData> DashBoardLoanBalanceDetailBuyerWise = _RugDashBoardBCService.GetLoanBalanceDetailLedgerAccountWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardLoanBalanceDetailBuyerWise }, JsonRequestBehavior.AllowGet);
            return json;
        }




        //public JsonResult GetSaleOrderStatus()
        //{
        //    IEnumerable<DashBoardTrippleValue> SaleOrderStatus = _RugDashBoardBCService.GetSaleOrderStatus();

        //    JsonResult json = Json(new { Success = true, Data = SaleOrderStatus }, JsonRequestBehavior.AllowGet);
        //    return json;
        //}

        //public JsonResult GetVehicleProfit()
        //{
        //    IEnumerable<DashBoardSingleValue> VehicleProfit = _RugDashBoardBCService.GetVehicleProfit();

        //    JsonResult json = Json(new { Success = true, Data = VehicleProfit }, JsonRequestBehavior.AllowGet);
        //    return json;
        //}




        //public JsonResult GetExpense()
        //{
        //    IEnumerable<DashBoardSingleValue> Expense = _RugDashBoardBCService.GetExpense();

        //    JsonResult json = Json(new { Success = true, Data = Expense }, JsonRequestBehavior.AllowGet);
        //    return json;
        //}

        //public JsonResult GetDebtors()
        //{
        //    IEnumerable<DashBoardSingleValue> Debtors = _RugDashBoardBCService.GetDebtors();




        //    JsonResult json = Json(new { Success = true, Data = Debtors }, JsonRequestBehavior.AllowGet);
        //    return json;
        //}

        //public JsonResult GetCreditors()
        //{
        //    IEnumerable<DashBoardSingleValue> Creditors = _RugDashBoardBCService.GetCreditors();

        //    JsonResult json = Json(new { Success = true, Data = Creditors }, JsonRequestBehavior.AllowGet);
        //    return json;
        //}

        //public JsonResult GetBankBalance()
        //{
        //    IEnumerable<DashBoardSingleValue> BankBalance = _RugDashBoardBCService.GetBankBalance();

        //    JsonResult json = Json(new { Success = true, Data = BankBalance }, JsonRequestBehavior.AllowGet);
        //    return json;
        //}

        //public JsonResult GetCashBalance()
        //{
        //    IEnumerable<DashBoardSingleValue> CashBalance = _RugDashBoardBCService.GetCashBalance();

        //    JsonResult json = Json(new { Success = true, Data = CashBalance }, JsonRequestBehavior.AllowGet);
        //    return json;
        //}



        //public JsonResult GetWorkshopSale()
        //{
        //    IEnumerable<DashBoardDoubleValue> WorkshopSale = _RugDashBoardBCService.GetWorkshopSale();

        //    JsonResult json = Json(new { Success = true, Data = WorkshopSale }, JsonRequestBehavior.AllowGet);
        //    return json;
        //}
        //public JsonResult GetSpareSale()
        //{
        //    IEnumerable<DashBoardDoubleValue> SpareSale = _RugDashBoardBCService.GetSpareSale();

        //    JsonResult json = Json(new { Success = true, Data = SpareSale }, JsonRequestBehavior.AllowGet);
        //    return json;
        //}





        //public JsonResult GetSpareSalePieChartData()
        //{
        //    IEnumerable<DashBoardPieChartData> SparePieChartData = _RugDashBoardBCService.GetSpareSalePieChartData();

        //    JsonResult json = Json(new { Success = true, Data = SparePieChartData }, JsonRequestBehavior.AllowGet);
        //    return json;
        //}
        //public JsonResult GetSpareSaleChartData()
        //{
        //    IEnumerable<DashBoardSaleBarChartData> SpareSaleChartData = _RugDashBoardBCService.GetSpareSaleBarChartData();

        //    JsonResult json = Json(new { Success = true, Data = SpareSaleChartData }, JsonRequestBehavior.AllowGet);
        //    return json;
        //}














        //public JsonResult GetVehicleProfitDetailProductGroupWise()
        //{
        //    IEnumerable<DashBoardTabularData> DashBoardTabularData = _RugDashBoardBCService.GetVehicleProfitDetailProductGroupWise();

        //    JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
        //    return json;
        //}

        //public JsonResult GetVehicleProfitDetailSalesManWise()
        //{
        //    IEnumerable<DashBoardTabularData> DashBoardTabularData = _RugDashBoardBCService.GetVehicleProfitDetailSalesManWise();

        //    JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
        //    return json;
        //}

        //public JsonResult GetVehicleProfitDetailBranchWise()
        //{
        //    IEnumerable<DashBoardTabularData> DashBoardTabularData = _RugDashBoardBCService.GetVehicleProfitDetailBranchWise();

        //    JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
        //    return json;
        //}

        //public JsonResult GetDebtorsDetail()
        //{
        //    IEnumerable<DashBoardTabularData> DashBoardTabularData = _RugDashBoardBCService.GetDebtorsDetail();

        //    JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
        //    return json;
        //}

        //public JsonResult GetBankBalanceDetailBankAc()
        //{
        //    IEnumerable<DashBoardTabularData> DashBoardTabularData = _RugDashBoardBCService.GetBankBalanceDetailBankAc();

        //    JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
        //    return json;
        //}

        //public JsonResult GetBankBalanceDetailBankODAc()
        //{
        //    IEnumerable<DashBoardTabularData> DashBoardTabularData = _RugDashBoardBCService.GetBankBalanceDetailBankODAc();

        //    JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
        //    return json;
        //}

        //public JsonResult GetBankBalanceDetailChannelFinanceAc()
        //{
        //    IEnumerable<DashBoardTabularData> DashBoardTabularData = _RugDashBoardBCService.GetBankBalanceDetailChannelFinanceAc();

        //    JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
        //    return json;
        //}


        //public JsonResult GetExpenseDetailLedgerAccountWise()
        //{
        //    IEnumerable<DashBoardTabularData> DashBoardTabularData = _RugDashBoardBCService.GetExpenseDetailLedgerAccountWise();

        //    JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
        //    return json;
        //}

        //public JsonResult GetExpenseDetailBranchWise()
        //{
        //    IEnumerable<DashBoardTabularData> DashBoardTabularData = _RugDashBoardBCService.GetExpenseDetailBranchWise();

        //    JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
        //    return json;
        //}

        //public JsonResult GetExpenseDetailCostCenterWise()
        //{
        //    IEnumerable<DashBoardTabularData> DashBoardTabularData = _RugDashBoardBCService.GetExpenseDetailCostCenterWise();

        //    JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
        //    return json;
        //}

        //public JsonResult GetCreditorsDetail()
        //{
        //    IEnumerable<DashBoardTabularData> DashBoardTabularData = _RugDashBoardBCService.GetCreditorsDetail();

        //    JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
        //    return json;
        //}

        //public JsonResult GetCashBalanceDetailLedgerAccountWise()
        //{
        //    IEnumerable<DashBoardTabularData> DashBoardTabularData = _RugDashBoardBCService.GetCashBalanceDetailLedgerAccountWise();

        //    JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
        //    return json;
        //}

        //public JsonResult GetCashBalanceDetailBranchWise()
        //{
        //    IEnumerable<DashBoardTabularData> DashBoardTabularData = _RugDashBoardBCService.GetCashBalanceDetailBranchWise();

        //    JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
        //    return json;
        //}


    }   
}