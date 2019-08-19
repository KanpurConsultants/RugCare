$(function () {

    'use strict';

    /* ChartJS
     * -------
     * Here we will create a few charts using ChartJS
     */



    //-------------
    //- Vehicle Sale PIE CHART -
    //-------------
    // Get context with jQuery - using jQuery's .get() method.

    var ProductionPieChartCanvas = $("#ProductionPieChart").get(0).getContext("2d");
    var ProductionPieChart = new Chart(ProductionPieChartCanvas);
    var ProductionPieChartDataArray = null
    GetProductionPieChartData();

    function GetProductionPieChartData() {
        $.ajax({
            async: false,
            cache: false,
            type: "POST",
            url: '/RugDashBoardBC/GetProductionPieChartData',
            success: function (result) {
                ProductionPieChartDataArray = result.Data;
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert('Failed to retrieve product details.' + thrownError);
            }
        });
    }


    var ProductionPieChartHint = '<ul class="chart-legend clearfix">'
    ProductionPieChartDataArray.forEach(function (value) {
        ProductionPieChartHint = ProductionPieChartHint + '<li><i class="fa fa-circle" style="color:' + value.color + '"></i> ' + value.label + '</li>'
    });
    ProductionPieChartHint = ProductionPieChartHint + '</ul>'

    $('#ProductionPieChartHint').html(ProductionPieChartHint)

    var ProductionPieChartOptions = {
        //Boolean - Whether we should show a stroke on each segment
        segmentShowStroke: true,
        //String - The colour of each segment stroke
        segmentStrokeColor: "#fff",
        //Number - The width of each segment stroke
        segmentStrokeWidth: 1,
        //Number - The percentage of the chart that we cut out of the middle
        percentageInnerCutout: 50, // This is 0 for Pie charts
        //Number - Amount of animation steps
        animationSteps: 100,
        //String - Animation easing effect
        animationEasing: "easeOutBounce",
        //Boolean - Whether we animate the rotation of the Doughnut
        animateRotate: true,
        //Boolean - Whether we animate scaling the Doughnut from the centre
        animateScale: false,
        //Boolean - whether to make the chart responsive to window resizing
        responsive: true,
        // Boolean - whether to maintain the starting aspect ratio or not when responsive, if set to false, will take up entire container
        maintainAspectRatio: false,
        //String - A legend template
        legendTemplate: "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<segments.length; i++){%><li><span style=\"background-color:<%=segments[i].fillColor%>\"></span><%if(segments[i].label){%><%=segments[i].label%><%}%></li><%}%></ul>",
        //String - A tooltip template
        tooltipTemplate: "<%=value %> <%=label%>"
    };
    //Create pie or douhnut chart
    // You can switch between pie and douhnut using the method below.
    ProductionPieChart.Doughnut(ProductionPieChartDataArray, ProductionPieChartOptions);
    //-----------------
    //- END Vehicle Sale PIE CHART -
    //-----------------

    //---------------------------------------------------------------------------------------------------------
    //-------------
    //- Spare Sale PIE CHART -
    //-------------
    // Get context with jQuery - using jQuery's .get() method.
    //var SpareSalepieChartCanvas = $("#SpareSalePieChart").get(0).getContext("2d");
    //var SpareSalepieChart = new Chart(SpareSalepieChartCanvas);
    //var SpareSalePieChartDataArray = null
    //GetSpareSalePieChartData();

    //function GetSpareSalePieChartData() {
    //    $.ajax({
    //        async: false,
    //        cache: false,
    //        type: "POST",
    //        url: '/RugDashBoardBC/GetSpareSalePieChartData',
    //        success: function (result) {
    //            SpareSalePieChartDataArray = result.Data;
    //        },
    //        error: function (xhr, ajaxOptions, thrownError) {
    //            alert('Failed to retrieve product details.' + thrownError);
    //        }
    //    });
    //}


    //var SpareSalePieChartHint = '<ul class="chart-legend clearfix">'
    //SpareSalePieChartDataArray.forEach(function (value) {
    //    SpareSalePieChartHint = SpareSalePieChartHint + '<li><i class="fa fa-circle-o" style="color:' + value.color + '"></i> ' + value.label + '</li>'
    //});
    //SpareSalePieChartHint = SpareSalePieChartHint + '</ul>'

    //$('#SpareSalePieChartHint').html(SpareSalePieChartHint)

    //var SpareSalePieChartOptions = {
    //    //Boolean - Whether we should show a stroke on each segment
    //    segmentShowStroke: true,
    //    //String - The colour of each segment stroke
    //    segmentStrokeColor: "#fff",
    //    //Number - The width of each segment stroke
    //    segmentStrokeWidth: 1,
    //    //Number - The percentage of the chart that we cut out of the middle
    //    percentageInnerCutout: 50, // This is 0 for Pie charts
    //    //Number - Amount of animation steps
    //    animationSteps: 100,
    //    //String - Animation easing effect
    //    animationEasing: "easeOutBounce",
    //    //Boolean - Whether we animate the rotation of the Doughnut
    //    animateRotate: true,
    //    //Boolean - Whether we animate scaling the Doughnut from the centre
    //    animateScale: false,
    //    //Boolean - whether to make the chart responsive to window resizing
    //    responsive: true,
    //    // Boolean - whether to maintain the starting aspect ratio or not when responsive, if set to false, will take up entire container
    //    maintainAspectRatio: false,
    //    //String - A legend template
    //    legendTemplate: "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<segments.length; i++){%><li><span style=\"background-color:<%=segments[i].fillColor%>\"></span><%if(segments[i].label){%><%=segments[i].label%><%}%></li><%}%></ul>",
    //    //String - A tooltip template
    //    tooltipTemplate: "<%=value %> <%=label%>"
    //};
    //Create pie or douhnut chart
    // You can switch between pie and douhnut using the method below.
    //SpareSalepieChart.Doughnut(SpareSalePieChartDataArray, SpareSalePieChartOptions);
    //-----------------
    //- END Spare Sale PIE CHART -
    //-----------------


    //---------------------------------------------------------------------------------------------------------


    //-------------
    //-Vehicle Sale BAR CHART -
    //-------------
    var SaleInvoicebarChartCanvas = $("#SaleInvoiceBarChart").get(0).getContext("2d");
    var SaleInvoicebarChart = new Chart(SaleInvoicebarChartCanvas);


    var SaleInvoiceChartDataArray = null
    GetSaleInvoiceChartData();

    function GetSaleInvoiceChartData() {
        $.ajax({
            async: false,
            cache: false,
            type: "POST",
            url: '/RugDashBoardBC/GetSaleInvoiceChartData',
            success: function (result) {
                SaleInvoiceChartDataArray = result.Data;
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert('Failed to retrieve product details.' + thrownError);
            }
        });
    }

    var labels_SalesChart = [], data_SaleChart = []
    SaleInvoiceChartDataArray.forEach(function (value) {
        labels_SalesChart.push(value.Month);
        data_SaleChart.push(value.Amount);
    });


    var SaleInvoicesChartData = {
        labels: labels_SalesChart,
        datasets: [
          {
              label: "Amount",
              fillColor: "rgba(210, 214, 222, 1)",
              strokeColor: "rgba(210, 214, 222, 1)",
              pointColor: "rgba(210, 214, 222, 1)",
              pointStrokeColor: "#c1c7d1",
              pointHighlightFill: "#fff",
              pointHighlightStroke: "rgba(210, 214, 222, 1)",
              data: data_SaleChart
          }
        ]
    };

    var SaleInvoicebarChartData = SaleInvoicesChartData;
    SaleInvoicebarChartData.datasets[0].fillColor = "#00c0ef";
    SaleInvoicebarChartData.datasets[0].strokeColor = "#00c0ef";
    SaleInvoicebarChartData.datasets[0].pointColor = "#00c0ef";
    var SaleInvoicebarChartOptions = {
        //Boolean - Whether the scale should start at zero, or an order of magnitude down from the lowest value
        scaleBeginAtZero: true,
        //Boolean - Whether grid lines are shown across the chart
        scaleShowGridLines: true,
        //String - Colour of the grid lines
        scaleGridLineColor: "rgba(0,0,0,.05)",
        //Number - Width of the grid lines
        scaleGridLineWidth: 1,
        //Boolean - Whether to show horizontal lines (except X axis)
        scaleShowHorizontalLines: true,
        //Boolean - Whether to show vertical lines (except Y axis)
        scaleShowVerticalLines: true,
        //Boolean - If there is a stroke on each bar
        barShowStroke: true,
        //Number - Pixel width of the bar stroke
        barStrokeWidth: 1,
        //Number - Spacing between each of the X value sets
        barValueSpacing: 5,
        //Number - Spacing between data sets within X values
        barDatasetSpacing: 1,
        //String - A legend template
        legendTemplate: "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<datasets.length; i++){%><li><span style=\"background-color:<%=datasets[i].fillColor%>\"></span><%if(datasets[i].label){%><%=datasets[i].label%><%}%></li><%}%></ul>",
        //Boolean - whether to make the chart responsive
        responsive: true,
        maintainAspectRatio: true
    };

    SaleInvoicebarChartOptions.datasetFill = false;
    SaleInvoicebarChart.Bar(SaleInvoicesChartData, SaleInvoicebarChartOptions);


    //-------------------------------------------------------------------------------------------------


    //-------------
    //-Spare Sale BAR CHART -
    //-------------
    //var SpareSalebarChartCanvas = $("#SpareSaleBarChart").get(0).getContext("2d");
    //var SpareSalebarChart = new Chart(SpareSalebarChartCanvas);


    //var SpareSaleChartDataArray = null
    //GetSpareSaleChartData();

    //function GetSpareSaleChartData() {
    //    $.ajax({
    //        async: false,
    //        cache: false,
    //        type: "POST",
    //        url: '/RugDashBoardBC/GetSpareSaleChartData',
    //        success: function (result) {
    //            SpareSaleChartDataArray = result.Data;
    //        },
    //        error: function (xhr, ajaxOptions, thrownError) {
    //            alert('Failed to retrieve product details.' + thrownError);
    //        }
    //    });
    //}

    //var labels_SalesChart = [], data_SaleChart = []
    //SpareSaleChartDataArray.forEach(function (value) {
    //    labels_SalesChart.push(value.Month);
    //    data_SaleChart.push(value.Amount);
    //});


    //var SpareSalesChartData = {
    //    labels: labels_SalesChart,
    //    datasets: [
    //      {
    //          label: "Amount",
    //          fillColor: "rgb(210, 214, 222)",
    //          strokeColor: "rgb(210, 214, 222)",
    //          pointColor: "rgb(210, 214, 222)",
    //          pointStrokeColor: "#c1c7d1",
    //          pointHighlightFill: "#fff",
    //          pointHighlightStroke: "rgb(220,220,220)",
    //          data: data_SaleChart
    //      }
    //    ]
    //};

    //var SpareSalebarChartData = SpareSalesChartData;
    //SpareSalebarChartData.datasets[0].fillColor = "#00c0ef";
    //SpareSalebarChartData.datasets[0].strokeColor = "#00c0ef";
    //SpareSalebarChartData.datasets[0].pointColor = "#00c0ef";
    //var SpareSalebarChartOptions = {
    //    //Boolean - Whether the scale should start at zero, or an order of magnitude down from the lowest value
    //    scaleBeginAtZero: true,
    //    //Boolean - Whether grid lines are shown across the chart
    //    scaleShowGridLines: true,
    //    //String - Colour of the grid lines
    //    scaleGridLineColor: "rgba(0,0,0,.05)",
    //    //Number - Width of the grid lines
    //    scaleGridLineWidth: 1,
    //    //Boolean - Whether to show horizontal lines (except X axis)
    //    scaleShowHorizontalLines: true,
    //    //Boolean - Whether to show vertical lines (except Y axis)
    //    scaleShowVerticalLines: true,
    //    //Boolean - If there is a stroke on each bar
    //    barShowStroke: true,
    //    //Number - Pixel width of the bar stroke
    //    barStrokeWidth: 1,
    //    //Number - Spacing between each of the X value sets
    //    barValueSpacing: 5,
    //    //Number - Spacing between data sets within X values
    //    barDatasetSpacing: 1,
    //    //String - A legend template
    //    legendTemplate: "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<datasets.length; i++){%><li><span style=\"background-color:<%=datasets[i].fillColor%>\"></span><%if(datasets[i].label){%><%=datasets[i].label%><%}%></li><%}%></ul>",
    //    //Boolean - whether to make the chart responsive
    //    responsive: true,
    //    maintainAspectRatio: true
    //};

    //SpareSalebarChartOptions.datasetFill = false;
    //SpareSalebarChart.Bar(SpareSalesChartData, SpareSalebarChartOptions);


    //-------------------------------------------------------------------------------------------------

});

//----------------------Set Single Value------------------------------------------------

function SetSingleValue(functionname, Div_Id) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: '/RugDashBoardBC/' + functionname,
        success: function (result) {
            $(Div_Id).text(FormatValues(result.Data[0].Value));
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert('Failed to retrieve product details.' + thrownError);
        }
    });
}

//-----------------------------End Single Value Function---------------------------------------------------------


//----------------------Set Double Value------------------------------------------------

function SetDoubleValue(functionname, Div_Id_Value1, Div_Id_Value2) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: '/RugDashBoardBC/' + functionname,
        success: function (result) {
            $(Div_Id_Value1).text(FormatValues(result.Data[0].Value1));
            $(Div_Id_Value2).text(FormatValues(result.Data[0].Value2));
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert('Failed to retrieve product details.' + thrownError);
        }
    });
}

//-----------------------------End Double Value Function---------------------------------------------------------

//----------------------Set Double Value------------------------------------------------

function SetTrippleValue(functionname, Div_Id_Value1, Div_Id_Value2, Div_Id_Value3) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: '/RugDashBoardBC/' + functionname,
        success: function (result) {
            $(Div_Id_Value1).text(FormatValues(result.Data[0].Value1));
            $(Div_Id_Value2).text(FormatValues(result.Data[0].Value2));
            $(Div_Id_Value3).text(FormatValues(result.Data[0].Value3));
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert('Failed to retrieve product details.' + thrownError);
        }
    });
}

//-----------------------------End Double Value Function---------------------------------------------------------


//----------------------Start For Readymande Table Design-------------------------------


function DesignTable(functionname, Head_Caption, Value_Caption, Div_Id) {
    var TableHTML = '<div class="box-body" style="overflow-y:auto; height: 400px;"> ' +
                                ' <table class="table table-bordered"> '
    TableHTML = TableHTML + '<tr> ' +
                                '<th style="width: 200px">' + Head_Caption + '</th> ' +
                                '<th style="width: 100px">' + Value_Caption + '</th> ' +
                            '</tr>'

    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: '/RugDashBoardBC/' + functionname,
        success: function (result) {
            result.Data.forEach(function (value) {
                TableHTML = TableHTML + '<tr> ' +
                        ' <td style="width: 200px">' + value.Head + '</td> ' +
                        ' <td style="width: 100px">' + FormatValues(value.Value) + '</td> ' +
                        ' </tr>'
            });
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert('Failed to retrieve product details.' + thrownError);
        }
    });
    TableHTML = TableHTML + '</table></div>'
    $(Div_Id).html(TableHTML)
}
//----------------------End For Readymande Table Design-------------------------------





//----------------------Start For Readymande Table Design-------------------------------


function DesignTable_ThreeColumns(functionname, Head_Caption, Value1_Caption, Value2_Caption, Div_Id) {
    var TableHTML = '<div class="box-body" style="overflow-y:scroll; height: 400px;"> ' +
                                ' <table class="table table-bordered"> '
    TableHTML = TableHTML + '<tr> ' +
                                '<th style="width: 200px">' + Head_Caption + '</th> ' +
                                '<th style="width: 100px">' + Value1_Caption + '</th> ' +
                                '<th style="width: 100px">' + Value2_Caption + '</th> ' +
                            '</tr>'

    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: '/RugDashBoardBC/' + functionname,
        success: function (result) {
            result.Data.forEach(function (value) {
                TableHTML = TableHTML + '<tr> ' +
                        ' <td style="width: 200px">' + value.Head + '</td> ' +
                        ' <td style="width: 100px">' + value.Value1 + '</td> ' +
                        ' <td style="width: 100px">' + FormatValues(value.Value2) + '</td> ' +
                        ' </tr>'
            });
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert('Failed to retrieve product details.' + thrownError);
        }
    });
    TableHTML = TableHTML + '</table></div>'
    $(Div_Id).html(TableHTML)
}
//----------------------End For Readymande Table Design-------------------------------

//---------------------------------Start For Formet Values-------------------------------------

function FormatValues(Value) {
    return parseFloat(Value).toFixed(0);
    //if (Math.abs(Value) < 1000)
    //    return parseFloat(Value).toFixed(2);
    //if (Math.abs(Value) < 100000)
    //    return parseFloat(Value / 1000).toFixed(2) + ' K';
    //else if (Math.abs(Value) < 10000000)
    //    return parseFloat(Value / 100000).toFixed(2) + ' L';
    //else if (Math.abs(Value) >= 10000000)
    //    return parseFloat(Value / 10000000).toFixed(2) + ' C';
}

//---------------------------------End Formet Values-------------------------------------


$(document).ready(function () {

//----------------- For Main Blocks
    SetDoubleValue('GetSaleOrderBalanceSummary', '#SaleOrderBalanceSummary_Qty', '#SaleOrderBalanceSummary_Area')
    SetDoubleValue('GetUnExecuteSummary', '#UnExecuteSummary_Qty', '#UnExecuteSummary_Area')
    SetTrippleValue('GetSaleInvoiceSummary', '#SaleInvoiceSummary_Month', '#SaleInvoiceSummary_Week', '#SaleInvoiceSummary_Today')
    SetDoubleValue('GetStockSummary', '#StockSummary_Qty', '#StockSummary_Area')
    SetDoubleValue('GetToBeIssueSummary', '#ToBeIssueSummary_Qty', '#ToBeIssueSummary_Area')
    SetTrippleValue('GetProductionSummary', '#ProductionSummary_Month', '#ProductionSummary_Week', '#ProductionSummary_Today')
    SetDoubleValue('GetOnLoomSummary', '#OnLoomSummary_Qty', '#OnLoomSummary_Area')
    SetSingleValue('GetDyeingOrderBalanceSummary', '#DyeingOrderBalanceSummary_KG')
    SetSingleValue('GetLoanBalanceSummary', '#LoanBalanceAmount')
   
   



    //SetTrippleValue('GetSaleOrderStatus', '#SOHighRisk', '#SOLowRisk', '#SOOnTime')
    //SetSingleValue('GetVehicleProfit', '#VehicleProfitAmount')
    //SetSingleValue('GetDebtors', '#DebtorsAmount')
    //SetSingleValue('GetBankBalance', '#BankBalanceAmount')
    //SetSingleValue('GetVehicleStock', '#VehicleStockAmount')
    //SetSingleValue('GetExpense', '#ExpenseAmount')
    //SetSingleValue('GetCreditors', '#CreditorsAmount')
    //SetSingleValue('GetCashBalance', '#CashBalanceAmount')
    //SetDoubleValue('GetWorkshopSale', '#WorkshopSaleAmount', '#WorkshopSaleAmount_Today')


    //----------------- For Detail Blocks
    //1 Block
    DesignTable_ThreeColumns('GetSaleOrderBalanceDetailCategoryWise', 'Category', "Qty", 'Area', '#SaleOrderBalanceDetailCategoryWise');
    DesignTable_ThreeColumns('GetSaleOrderBalanceDetailQualityWise', 'Quality', "Qty", 'Area', '#SaleOrderBalanceDetailQualityWise');
    DesignTable_ThreeColumns('GetSaleOrderBalanceDetailBuyerWise', 'Buyer', "Qty", 'Area', '#SaleOrderBalanceDetailBuyerWise');

    //2 Block
    DesignTable_ThreeColumns('GetUnExecuteDetailCategoryWise', 'Category', "Qty", 'Area', '#UnExecuteDetailCategoryWise');
    DesignTable_ThreeColumns('GetUnExecuteDetailBranchWise', 'Quality', "Qty", 'Area', '#UnExecuteDetailBranchWise');
    DesignTable_ThreeColumns('GetUnExecuteDetailBuyerWise', 'Buyer', "Qty", 'Area', '#UnExecuteDetailBuyerWise');

    //3 Block
    DesignTable_ThreeColumns('GetSaleInvoiceDetailCategoryWise', 'Category', "Qty", 'Area', '#SaleInvoiceDetailCategoryWise');
    DesignTable_ThreeColumns('GetSaleInvoiceDetailQualityWise', 'Quality', "Qty", 'Area', '#SaleInvoiceDetailQualityWise');
    DesignTable_ThreeColumns('GetSaleInvoiceDetailBuyerWise', 'Buyer', "Qty", 'Area', '#SaleInvoiceDetailBuyerWise');

    //5 Block
    DesignTable_ThreeColumns('GetStockDetailCategoryWise', 'Category', "Qty", 'Area', '#StockDetailCategoryWise');
    DesignTable_ThreeColumns('GetStockDetailQualityWise', 'Quality', "Qty", 'Area', '#StockDetailQualityWise');
    DesignTable_ThreeColumns('GetStockDetailBuyerWise', 'Buyer', "Qty", 'Area', '#StockDetailBuyerWise');

    //6 Block
    DesignTable_ThreeColumns('GetToBeIssueDetailCategoryWise', 'Category', "Qty", 'Area', '#ToBeIssueDetailCategoryWise');
    DesignTable_ThreeColumns('GetToBeIssueDetailQualityWise', 'Quality', "Qty", 'Area', '#ToBeIssueDetailQualityWise');
    DesignTable_ThreeColumns('GetToBeIssueDetailBuyerWise', 'Buyer', "Qty", 'Area', '#ToBeIssueDetailBuyerWise');

    //7 Block
    DesignTable_ThreeColumns('GetProductionDetailCategoryWise', 'Category', "Qty", 'Area', '#ProductionDetailCategoryWise');
    DesignTable_ThreeColumns('GetProductionDetailQualityWise', 'Quality', "Qty", 'Area', '#ProductionDetailQualityWise');
    DesignTable_ThreeColumns('GetProductionDetailBuyerWise', 'Buyer', "Qty", 'Area', '#ProductionDetailBuyerWise');

    //9 Block
    DesignTable_ThreeColumns('GetOnLoomDetailCategoryWise', 'Category', "Qty", 'Area', '#OnLoomDetailCategoryWise');
    DesignTable_ThreeColumns('GetOnLoomDetailJobWorkerWise', 'JobWorker', "Qty", 'Area', '#OnLoomDetailJobWorkerWise');
    DesignTable_ThreeColumns('GetOnLoomDetailBuyerWise', 'Buyer', "Qty", 'Area', '#OnLoomDetailBuyerWise');


    //10 Block
    DesignTable('GetDyeingOrderBalanceDetailProductWise', 'Product', "KG", '#DyeingOrderBalanceDetailProductWise');
    DesignTable('GetDyeingOrderBalanceDetailMonthWise', 'Month', "KG", '#DyeingOrderBalanceDetailMonthWise');
    DesignTable('GetDyeingOrderBalanceDetailJobWorkerWise', 'Job Worker', "KG", '#DyeingOrderBalanceDetailJobWorkerWise');



    //11 Block

    //12 Block
    DesignTable('GetLoanBalanceDetailDepartmentWise', 'Department', "INR", '#LoanBalanceDetailDepartmentWise');
    DesignTable('GetLoanBalanceDetailMonthWise', 'Month', "INR", '#LoanBalanceDetailMonthWise');
    DesignTable('GetLoanBalanceDetailLedgerAccountWise', 'Party', "INR", '#LoanBalanceDetailLedgerAccountWise');



    //DesignTable('GetVehicleProfitDetailProductGroupWise', 'Group', 'Amount', '#VehicleProfitDetailProductGroupWise');
    //DesignTable('GetVehicleProfitDetailSalesManWise', 'Sales Man', 'Amount', '#VehicleProfitDetailSalesManWise');
    //DesignTable('GetVehicleProfitDetailBranchWise', 'Branch', 'Amount', '#VehicleProfitDetailBranchWise');

    //DesignTable('GetDebtorsDetail', 'Group', 'Amount', '#DebtorsDetailTable');

    //DesignTable('GetBankBalanceDetailBankAc', 'Bank Account', 'Amount', '#BankBalanceDetailBankAc');
    //DesignTable('GetBankBalanceDetailBankODAc', 'Bank OD Account', 'Amount', '#BankBalanceDetailBankODAc');
    //DesignTable('GetBankBalanceDetailChannelFinanceAc', 'Channel Finance', 'Amount', '#BankBalanceDetailChannelFinanceAc');


    //DesignTable('GetExpenseDetailLedgerAccountWise', 'Group', 'Amount', '#ExpenseDetailLedgerAccountWise');
    //DesignTable('GetExpenseDetailBranchWise', 'Branch', 'Amount', '#ExpenseDetailBranchWise');
    //DesignTable('GetExpenseDetailCostCenterWise', 'Cost Center', 'Amount', '#ExpenseDetailCostCenterWise');

    //DesignTable('GetCreditorsDetail', 'Group', 'Amount', '#CreditorsDetailTable');

    //DesignTable('GetCashBalanceDetailLedgerAccountWise', 'Group', 'Amount', '#CashBalanceDetailLedgerAccountWise');
    //DesignTable('GetCashBalanceDetailBranchWise', 'Branch', 'Amount', '#CashBalanceDetailBranchWise');




    //DesignTable('GetWorkshopSaleDetailProductTypeWise', 'Type', 'Amount', '#WorkshopSaleDetailProductTypeWise');
    //DesignTable('GetWorkshopSaleDetailProductGroupWise', 'Group', 'Amount', '#WorkshopSaleDetailProductGroupWise');


});

