PackingReport = angular.module('PackingReport', ['ngTouch', 'ui.grid', 'ui.grid.resizeColumns', 'ui.grid.moveColumns',
    'ui.grid.exporter', 'ui.grid.cellNav', 'ui.grid.pinning'])



PackingReport.controller('MainCtrl', ['$scope', '$log', '$http', 'uiGridConstants',



  function ($scope, $log, $http, uiGridConstants, uiGridTreeViewConstants) {
      $scope.gridOptions = {
          enableGridMenu: true,
          exporterCsvFilename: 'myFile.csv',
          exporterPdfDefaultStyle: { fontSize: 9 },
          exporterPdfTableStyle: { margin: [30, 30, 30, 30] },
          exporterPdfTableHeaderStyle: { fontSize: 10, bold: true, italics: true, color: 'red' },
          exporterPdfHeader: { text: "My Header", style: 'headerStyle' },
          exporterPdfFooter: function (currentPage, pageCount) {
              return { text: currentPage.toString() + ' of ' + pageCount.toString(), style: 'footerStyle' };
          },
          exporterPdfCustomFormatter: function (docDefinition) {
              docDefinition.styles.headerStyle = { fontSize: 22, bold: true };
              docDefinition.styles.footerStyle = { fontSize: 10, bold: true };
              return docDefinition;
          },
          exporterPdfOrientation: 'portrait',
          exporterPdfPageSize: 'LETTER',
          exporterPdfMaxGridWidth: 500,



          enableHorizontalScrollbar: uiGridConstants.scrollbars.ALWAYS,
          showTreeExpandNoChildren : false,
          enableFiltering: true,
          //enableTreeView : true,
          showColumnFooter: true,
          enableGridMenu: true,
          exporterCsvLinkElement: angular.element(document.querySelectorAll(".custom-csv-link-location")),
          exporterCsvFilename: 'myFile.csv',
          onRegisterApi: function (gridApi) {
              $scope.gridApi = gridApi;
          }
      };







      $scope.ShowDetail = function () {
          var rowCol = $scope.gridApi.cellNav.getFocusedCell();

          //alert(rowCol.col.name);
          if (rowCol.col.name == "STK")
          {
              $("#NextFormat").val("Stock Detail");
               if ($("#ReportType").val() == "Summary")
                   $("#TextHidden").val(rowCol.row.entity.ProductId);
               else
                   $("#TextHidden").val(rowCol.row.entity.SaleOrderLineId);
          }

          if (rowCol.col.name == "LOOM") {
              $("#NextFormat").val("Loom Detail");
              if ($("#ReportType").val() == "Summary")
                  $("#TextHidden").val(rowCol.row.entity.ProductId);
              else
                  $("#TextHidden").val(rowCol.row.entity.SaleOrderLineId);
          }
          if (rowCol.col.name == "SHP") {
              $("#NextFormat").val("Ship Detail");
              if ($("#ReportType").val() == "Summary")
                  $("#TextHidden").val(rowCol.row.entity.ProductId);
              else
                  $("#TextHidden").val(rowCol.row.entity.SaleOrderLineId);
          }
          if (rowCol.col.name == "UX") {
              $("#NextFormat").val("ToBeIssue Detail");
              if ($("#ReportType").val() == "Summary")
                  $("#TextHidden").val(rowCol.row.entity.ProductId);
              else
                  $("#TextHidden").val(rowCol.row.entity.SaleOrderLineId);
          }
          if (rowCol.col.name == "D_O") {
              $("#NextFormat").val("DO Detail");
              if ($("#ReportType").val() == "Summary")
                  $("#TextHidden").val(rowCol.row.entity.ProductId);
              else
                  $("#TextHidden").val(rowCol.row.entity.SaleOrderLineId);
          }
         if (rowCol.col.name == "O_X") {
              $("#NextFormat").val("OX Detail");
              if ($("#ReportType").val() == "Summary")
                  $("#TextHidden").val(rowCol.row.entity.ProductId);
              else
                  $("#TextHidden").val(rowCol.row.entity.SaleOrderLineId);
         }
           var DocTypeId = parseInt(rowCol.row.entity.DocTypeId);
           var DocId = parseInt(rowCol.row.entity.SaleOrderHeaderId);
           if (rowCol.row.entity.SaleOrderHeaderId != null)
          {
              window.open('/PackingReport/DocumentMenu/?DocTypeId=' + DocTypeId + '&DocId=' + DocId, '_blank');
              return;
          }
          

          //$.ajax({
          //    async : false,
          //    cache: false,
          //    type: "POST",
          //    url: '/PackingReport/SaveCurrentSetting',
          //    success: function (data) {
          //    },
          //    error: function (xhr, ajaxOptions, thrownError) {
          //        alert('Failed to retrieve product details.' + thrownError);
          //    }
          //});

          $scope.BindData();

      };
      

      $(document).keyup(function (e) {
          if (e.keyCode == 27) { // escape key maps to keycode `27`
              $.ajax({
                    async: false,
                    cache: false,
                    type: "POST",
                    url: '/PackingReport/GetParameterSettingsForLastDisplay',
                success: function (result) {
                   $("#ReportFormat").val(result.ReportFormat);
                   $("#AreaUnit").val(result.AreaUnit);
                   $("#Site").val(result.Site);
                   $("#Division").val(result.Division);
                   $("#FromDate").val(result.FromDate);
                   $("#ToDate").val(result.ToDate);
                   $("#Buyer").val(result.Buyer);
                   $("#PackingHeaderId").val(result.PackingHeaderId);
                   $("#SaleOrderHeaderId").val(result.SaleOrderHeaderId);
                   $("#ProductCategory").val(result.ProductCategory);
                   $("#ProductQuality").val(result.ProductQuality);
                   $("#ProductGroup").val(result.ProductGroup);
                   $("#ProductSize").val(result.ProductSize);
                   $("#Product").val(result.Product);

                   //$("#NextFormat").val(result.NextFormat);
                   $("#BuyerQuality").val(result.BuyerQuality);
                   $("#BuyerDesign").val(result.BuyerDesign);
                   $("#BuyerSize").val(result.BuyerSize);
                   $("#BuyerColour").val(result.BuyerColour);
                   //$("#TextHidden").val(result.TextHidden);
                   CustomSelectFunction($("#Format"), '/Display_SaleOrderBalance/GetFilterFormat', '/Display_JobOrderBalance/SetFilterFormat', ' ', false, 0);
                   CustomSelectFunction($("#Division"), '/ComboHelpList/GetDivision', '/ComboHelpList/SetSingleDivision', ' ', false, 0);
                   CustomSelectFunction($("#Site"), '/ComboHelpList/GetSite', '/ComboHelpList/SetSingleSite', ' ', false, 0);
                   CustomSelectFunction($("#ProductNature"), '/ComboHelpList/GetProductNature', '/ComboHelpList/SetProductNature', ' ', true, 0);
                   CustomSelectFunction($("#ProductType"), '/ComboHelpList/GetProductType', '/ComboHelpList/SetProductType', ' ', true, 0);
                   CustomSelectFunction($("#Buyer"), '/ComboHelpList/GetPerson', '/ComboHelpList/SetBuyers', ' ', true, 0);
                   CustomSelectFunction($("#ProductCategory"), '/ComboHelpList/GetProductCategory', '/ComboHelpList/SetProductCategory', ' ', true, 0);
                   CustomSelectFunction($("#ProductQuality"), '/ComboHelpList/GetProductQuality', '/ComboHelpList/SetProductQuality', ' ', true, 0);
                   CustomSelectFunction($("#DocTypeId"), '/ComboHelpList/GetDocumentType', '/ComboHelpList/SetDocumentType', ' ', true, 0);
                   CustomSelectFunction($("#Product"), '/ComboHelpList/GetProducts', '/ComboHelpList/SetProducts', ' ', true, 0);
                   CustomSelectFunction($("#SaleOrderHeaderId"), '/ComboHelpList/GetSaleOrders', '/ComboHelpList/SetSaleOrder', ' ', true, 0);
                   if (result.Buyer == null)
                       $("#Buyer").select2('data', { id: '', text: '' });
                   if (result.ProductType == null)
                       $("#ProductType").select2('data', { id: '', text: '' });
                   if (result.SiteId == null)
                       $("#Site").select2('data', { id: '', text: '' });
                   if (result.DivisionId == null)
                       $("#Division").select2('data', { id: '', text: '' });
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert('Failed to retrieve product details.' + thrownError);
                }
              });

            $scope.BindData();
          }


          if (e.keyCode == 13) {
              // escape key maps to keycode `27`
              if ($scope.gridApi.cellNav.getFocusedCell() != null)
              {
                  $scope.ShowDetail();
              }
          }
      });



      var i = 0;
      $scope.BindData = function ()
      {
          $scope.myData = [];
          //alert("1");

          $.ajax({
              url: '/PackingReport/PackingReportFill/' + $(this).serialize(),
              type: "POST",
              data: $("#registerSubmit").serialize(),
              success: function (result) {
                  Lock = false;
                  if (result.Success == true) {
                      $scope.gridOptions.columnDefs = new Array();

                      //alert($("#ReportType").val());
                      if (1==1)
                      {
                          $scope.gridOptions.columnDefs.push({ field: 'Invoice_No', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Buyer', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });              
                          $scope.gridOptions.columnDefs.push({ field: 'Quality', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Design', width: 150, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Size', width: 150, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Colour', width: 150, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Pcs', width: 100, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Area', width: 100, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text' });
                      }
                      else if ($("#ReportType").val() == "Summary" && ($("#NextFormat").val() == "" || $("#NextFormat").val() == null))
                      {
                          $scope.gridOptions.columnDefs.push({ field: 'ProductId', width: 50, visible: false });
                          $scope.gridOptions.columnDefs.push({ field: 'Quality', width: 150, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Design', width: 150, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Size', width: 150, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Colour', width: 150, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'ProductName', width: 250, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'ORD', width: 100, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'O_C', width: 100, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'SLP', width: 100, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'UX', width: 90, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'LOOM', width: 100, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'STK', width: 100, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'SHP', width: 100, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'BAL', width: 100, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'D_O', width: 100, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'D_B', width: 100, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'O_X', width: 100, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text' });
                      }
                      else if ($("#NextFormat").val() == "Loom Detail") {
                          $scope.gridOptions.columnDefs.push({ field: 'Branch', width: 150, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Purza_No', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Date', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Weaver', width: 150, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Quality', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Design', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Colour', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Size', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'LoomQty', width: 100, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text' });
                      }
                      else if ($("#NextFormat").val() == "Stock Detail") {
                          $scope.gridOptions.columnDefs.push({ field: 'CarpetNo', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Date', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Type', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Quality', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Design', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Colour', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Size', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Qty', width: 100, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'OTI', width: 100, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'OTR', width: 100, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text' });
                      }
                      else if ($("#NextFormat").val() == "Ship Detail") {
                          $scope.gridOptions.columnDefs.push({ field: 'Invoice_No', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Date', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Roll_No', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Remark', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'CarpetNo', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Type', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Quality', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Design', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Colour', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Size', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Qty', width: 100, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text' });
                      }
                      else if ($("#NextFormat").val() == "ToBeIssue Detail") {
                          $scope.gridOptions.columnDefs.push({ field: 'Branch', width: 150, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Slip_Date', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Quality', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Design', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Colour', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Size', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Qty', width: 100, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Iss', width: 100, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'UX', width: 100, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text' });
                      }
                      else if ($("#NextFormat").val() == "DO Detail") {
                          $scope.gridOptions.columnDefs.push({ field: 'Carpet No', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Date', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Type', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Quality', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Design', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Colour', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Size', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Qty', width: 100, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Buyer', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                      }
                  
                      else if ($("#NextFormat").val() == "OX Detail") {
                          $scope.gridOptions.columnDefs.push({ field: 'Order No', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Carpet No', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Date', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Type', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Quality', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Design', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Colour', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Size', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Qty', width: 100, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Buyer', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                      $scope.gridOptions.data = result.Data;
                      $scope.gridApi.grid.refresh();

                   }
                      $scope.gridOptions.data = result.Data;
                      $scope.gridApi.grid.refresh();

                   }
                  else if (!result.Success) {
                      alert('Something went wrong');
                  }
              },
              error: function () {
                  Lock: false;
                  alert('Something went wrong');
              }
          });
      }
  }
]);

function CustomSelectFunction2_WithFilter(ElementId, GetAction, SetAction, placehold, IsMultiple, MinLength, SqlProcGetSet, closeOnSel, filterid) {
    var geturl = GetAction;
    //The url we will send our get request to
    var attendeeUrl = GetAction;
    var pageSize = 10;

    if (closeOnSel == false)
        closeOnSel = false;
    else
        closeOnSel = true;

    ElementId.select2(
    {

        placeholder: placehold,
        //Does the user have to enter any data before sending the ajax request
        minimumInputLength: MinLength,
        allowClear: true,
        closeOnSelect: closeOnSel,
        multiple: IsMultiple,
        ajax: {
            //How long the user has to pause their typing before sending the next request
            quietMillis: 500,
            //The url of the json service
            url: attendeeUrl,
            dataType: 'jsonp',
            //Our search term and what page we are on
            data: function (term, page) {
                return {
                    pageSize: pageSize,
                    pageNum: page,
                    searchTerm: term,
                    filter: filterid,
                    SqlProcGet: SqlProcGetSet
                };
            },
            results: function (data, page) {
                //Used to determine whether or not there are more results available,
                //and if requests for more data should be sent in the infinite scrolling
                var more = (page * pageSize) < data.Total;
                return { results: data.Results, more: more };
            }
        },
        initSelection: function (element, callback) {

            var xval = element.val();
            if (xval != 0) {
                $.ajax({
                    cache: false,
                    type: "POST",
                    url: SetAction,
                    data: { Ids: element.val(), SqlProcSet: SqlProcGetSet },
                    success: function (data) {
                        callback(data);
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert('Failed to Retrive Qty' + thrownError);
                    }
                })
            }
        }
    });

    function SetProducts() {
        //alert("set product called");
        Id.select2("data", [{ id: "1", text: "arpit" }, { id: "2", text: "akash" }]);
    }
}

PackingReport.filter('trusted', function ($sce) {
    return function (value) {
        return $sce.trustAsHtml(value);
    }
});




