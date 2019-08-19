﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class SaleOrderLineProgress : EntityBase, IHistoryLog
    {
        [Key]
        public int SaleOrderLineProgressId { get; set; }

        [ForeignKey("SaleOrderLine")]
        public int SaleOrderLineId { get; set; }
        public virtual SaleOrderLine SaleOrderLine { get; set; }


        [ForeignKey("ProgressAttribute")]
        public int ProgressAttributeId { get; set; }
        public virtual ProgressAttribute ProgressAttribute { get; set; }

        public Decimal? Value { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        [Display(Name = "Created Date"), DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Modified Date"), DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime ModifiedDate { get; set; }

        [MaxLength(50)]
        public string OMSId { get; set; }
    }
}
