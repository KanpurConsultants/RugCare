﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class JobOrderInspectionSettings : EntityBase, IHistoryLog
    {

        [Key]
        public int JobOrderInspectionSettingsId { get; set; }

        [ForeignKey("DocType"), Display(Name = "Order Type")]
        public int DocTypeId { get; set; }
        public virtual DocumentType DocType { get; set; }

        [ForeignKey("ImportMenu")]
        [Display(Name = "ImportMenu")]
        public int? ImportMenuId { get; set; }
        public virtual Menu ImportMenu { get; set; }


        [ForeignKey("ExportMenu")]
        [Display(Name = "ExportMenu")]
        public int? ExportMenuId { get; set; }
        public virtual Menu ExportMenu { get; set; }

        public bool? isVisibleProductUID { get; set; }
        public bool? isMandatoryProductUID { get; set; }
        public bool? isVisibleDimension1 { get; set; }
        public bool? isVisibleDimension2 { get; set; }
        public bool? isVisibleDimension3 { get; set; }
        public bool? isVisibleDimension4 { get; set; }
        public string filterContraDocTypes { get; set; }
        public string filterContraSites { get; set; }
        public string filterContraDivisions { get; set; }
        public string filterPersonRoles { get; set; }

        [ForeignKey("DocumentPrintReportHeader")]
        public int? DocumentPrintReportHeaderId { get; set; }
        public virtual ReportHeader DocumentPrintReportHeader { get; set; }


        [ForeignKey("Process"), Display(Name = "Process")]
        public int ProcessId { get; set; }
        public virtual Process Process { get; set; }
        public int SiteId { get; set; }
        public virtual Site Site { get; set; }
        public int DivisionId { get; set; }
        public virtual Division Division { get; set; }

        [MaxLength(100)]
        public string DocumentPrint { get; set; }
        
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime ModifiedDate { get; set; }

        [MaxLength(50)]
        public string OMSId { get; set; }
    }
}