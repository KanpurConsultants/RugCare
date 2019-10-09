﻿using Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Models
{
    public class DiscountType : EntityBase, IHistoryLog
    {
        [Key]
        [Display(Name = "DiscountType Id")]
        public int DiscountTypeId { get; set; }

        [ForeignKey("DocType")]
        public int DocTypeId { get; set; }
        public virtual DocumentType DocType { get; set; }

        [MaxLength(50, ErrorMessage = "DiscountType Name cannot exceed 50 characters"), Required]
        [Index("IX_DiscountType_DiscountType", IsUnique = true)]
        [Display(Name = "DiscountType Name")]
        public string DiscountTypeName { get; set; }
        public Decimal Rate { get; set; }
        
        [Display(Name = "Is Active ?")]
        public Boolean IsActive { get; set; }

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