﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class PackingSetting : EntityBase, IHistoryLog
    {

        [Key]
        public int PackingSettingId { get; set; }

        [ForeignKey("DocType"), Display(Name = "Order Type")]
        public int DocTypeId { get; set; }
        public virtual DocumentType DocType { get; set; }      
        public int SiteId { get; set; }
        public virtual Site Site { get; set; }
        public int DivisionId { get; set; }
        public virtual Division Division { get; set; }
        public bool? isVisibleCostCenter { get; set; }
        public bool? isMandatoryCostCenter { get; set; }
        public bool? isVisibleProductUID { get; set; }
        public bool? isVisibleDimension1 { get; set; }
        public bool? isVisibleDimension2 { get; set; }
        public bool? isVisibleDimension3 { get; set; }
        public bool? isVisibleDimension4 { get; set; }
        public bool? isVisibleBaleCount { get; set; }

        public bool? isVisibleShipMethod { get; set; }
        public bool? isVisibleStockIn { get; set; }
        public bool? isVisibleSpecification { get; set; }
        public bool? isVisibleLotNo { get; set; }
        public bool? isVisibleBaleNo { get; set; }
        public bool? isVisibleDealUnit { get; set; }
        public bool? isPostedInSaleInvoice { get; set; }

        [ForeignKey("SaleInvoiceType"), Display(Name = "Sale Invoice Type")]
        public int? SaleInvoiceTypeId { get; set; }
        public virtual DocumentType SaleInvoiceType { get; set; }

        public bool? isAllowtoUpdateBuyerSpecification { get; set; }
        public bool? IsMandatoryStockIn { get; set; }
        public string filterProductTypes { get; set; }
        public string filterProductGroups { get; set; }
        public string filterProducts { get; set; }
        public string filterProductDivision { get; set; }
        public string filterContraDocTypes { get; set; }
        public string filterContraSites { get; set; }
        public string filterContraDivisions { get; set; }
        public string filterPersonRoles { get; set; }
        public string filterLedgerAccountGroups { get; set; }
        public string filterLedgerAccounts { get; set; }

        public bool? isVisibleHeaderJobWorker { get; set; }
        public bool? isVisibleBaleNoPattern { get; set; }
        public bool? isVisibleGrossWeight { get; set; }
        public bool? isVisibleNetWeight { get; set; }
        public bool? isVisibleProductInvoiceGroup { get; set; }
        public bool? isVisibleSaleDeliveryOrder { get; set; }

        [ForeignKey("Process")]
        public int? ProcessId { get; set; }
        public virtual Process Process { get; set; }

        [ForeignKey("UnitConversionFor")]
        [Display(Name = "Unit Conversion For Type")]
        public byte? UnitConversionForId { get; set; }
        public virtual UnitConversionFor UnitConversionFor { get; set; }

        /// <summary>
        /// DocId will be passed as a parameter in specified procedure.
        /// Procedure should have only one parameter of type int.
        /// </summary>
        [MaxLength(100)]
        public string SqlProcDocumentPrint { get; set; }


        /// <summary>
        /// DocId will be passed as a parameter in specified procedure.
        /// Procedure should have only one parameter of type int.
        /// </summary>
        [MaxLength(100)]
        public string SqlProcDocumentPrint_AfterSubmit { get; set; }

        /// <summary>
        /// DocId will be passed as a parameter in specified procedure.
        /// Procedure should have only one parameter of type int.
        /// </summary>
        [MaxLength(100)]
        public string SqlProcDocumentPrint_AfterApprove { get; set; }


        [MaxLength(20)]
        public string ExtraSaleOrderNo { get; set; }

        [ForeignKey("ImportMenu")]
        [Display(Name = "ImportMenu")]
        public int? ImportMenuId { get; set; }
        public virtual Menu ImportMenu { get; set; }

        [ForeignKey("ExportMenu")]
        [Display(Name = "ExportMenu")]
        public int? ExportMenuId { get; set; }
        public virtual Menu ExportMenu { get; set; }



        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime ModifiedDate { get; set; }
    }
}