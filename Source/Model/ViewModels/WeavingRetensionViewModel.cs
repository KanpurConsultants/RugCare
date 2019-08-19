using System.ComponentModel.DataAnnotations;

// New namespace imports:
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using Model.Models;
using System;
using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.ViewModels
{
    public class WeavingRetensionViewModel
    {
        public int WeavingRetensionId { get; set; }

        [Display(Name = "Division"), Required]
        [ForeignKey("Division")]
        [Index("IX_WeavingRetension_DocID", IsUnique = true, Order = 3)]
        public int DivisionId { get; set; }
        public virtual Division Division { get; set; }

        [Display(Name = "Site"), Required]
        [ForeignKey("Site")]
        [Index("IX_WeavingRetension_DocID", IsUnique = true, Order = 4)]
        public int SiteId { get; set; }
        public virtual Site Site { get; set; }


        [ForeignKey("ProductCategory")]
        [Display(Name = "Product Category")]
        public int? ProductCategoryId { get; set; }
        public virtual ProductCategory ProductCategory { get; set; }
        public string ProductCategoryName { get; set; }


        [ForeignKey("ProductQuality")]
        [Display(Name = "Product Quality")]
        public int? ProductQualityId { get; set; }
        public virtual ProductQuality ProductQuality { get; set; }
        public string ProductQualityName { get; set; }

        [ForeignKey("Person")]
        [Display(Name = "Person")]
        public int? PersonId { get; set; }
        public virtual Person Person { get; set; }
        public string PersonName { get; set; }


        [Display(Name = "RetensionPer")]
        public Decimal RetensionPer { get; set; }

        [Display(Name = "MinimumAmount")]
        public Decimal MinimumAmount { get; set; }

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

    public class WeavingRetensionIndexViewModel
    {
        public int PersonId { get; set; }
        public string Name { get; set; }
    }
}
