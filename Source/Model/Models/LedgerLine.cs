﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class LedgerLine : EntityBase, IHistoryLog
    {
        public LedgerLine()
        {
        }

        [Key]        
        public int LedgerLineId { get; set; }

        [Display(Name = "Ledger Header")]
        [ForeignKey("LedgerHeader")]
        public int LedgerHeaderId { get; set; }
        public virtual LedgerHeader  LedgerHeader { get; set; }

        [Display(Name = "LedgerAccount"), Required]
        [ForeignKey("LedgerAccount")]
        public int LedgerAccountId { get; set; }
        public virtual LedgerAccount LedgerAccount { get; set; }

        public int? ReferenceId { get; set; }


        [Display(Name = "Chq No"), MaxLength(10)]
        public string ChqNo { get; set; }

        [Display(Name = "Chq Date")]
        public DateTime? ChqDate { get; set; }

        [ForeignKey("CostCenter")]
        [Display(Name = "Cost Center")]
        public int? CostCenterId { get; set; }
        public virtual CostCenter CostCenter { get; set; }

        //[Display(Name = "Ledger"), Required]
        //[ForeignKey("ReferenceNo")]
        //public int? ReferenceId { get; set; }
        //public virtual Ledger ReferenceNo { get; set; }

        [Display(Name = "Due Date")]
        public DateTime? DueDate { get; set; }

        public decimal BaseValue { get; set; }
        public decimal BaseRate { get; set; }

        public int ? ReferenceDocTypeId { get; set; }
        public int ? ReferenceDocId { get; set; }
        public int ? ReferenceDocLineId { get; set; }
        public Decimal Amount { get; set; }

        public Decimal? Qty { get; set; }
        public Decimal? DealQty { get; set; }
        public Decimal? Rate { get; set; }

        [Display(Name = "Specification"), MaxLength(50)]
        public string Specification { get; set; }

        [ForeignKey("ProductUid"), Display(Name = "ProductUid")]
        public int? ProductUidId { get; set; }
        public virtual ProductUid ProductUid { get; set; }

        [Display(Name = "DrCr"), MaxLength(2)]
        public string DrCr { get; set; }

        [Display(Name = "Supplementary For Ledger")]
        public int? SupplementaryForLedgerId { get; set; }

        [Display(Name = "Remark")]
        public string Remark { get; set; }

        [MaxLength(50)]
        public string PassedBy { get; set; }

        [Display(Name = "Lock Reason")]
        public string LockReason { get; set; }

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


        //Property Tax
        public virtual PaymentMode PaymentMode { get; set; }
        [Display(Name = "Payment Mode")]
        [ForeignKey("PaymentMode")]
        public int? PaymentModeId { get; set; }

        public virtual LedgerAccount ReferenceLedgerAccount { get; set; }
        [Display(Name = "ReferenceLedgerAccount")]
        [ForeignKey("ReferenceLedgerAccount")]
        public int? ReferenceLedgerAccountId { get; set; }

        public virtual Person Agent { get; set; }
        [Display(Name = "Agent")]
        [ForeignKey("Agent")]
        public int? AgentId { get; set; }

        public decimal? DiscountAmount { get; set; }
        //End Property Tax
    }
}
