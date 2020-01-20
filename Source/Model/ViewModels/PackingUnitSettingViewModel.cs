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
    public class PackingUnitSettingViewModel
    {
        public int PackingUnitSettingId { get; set; }

        [Display(Name = "Division"), Required]
        [ForeignKey("Division")]
        [Index("IX_PackingUnitSetting_DocID", IsUnique = true, Order = 3)]
        public int DivisionId { get; set; }
        public virtual Division Division { get; set; }

        [Display(Name = "Site"), Required]
        [ForeignKey("Site")]
        [Index("IX_PackingUnitSetting_DocID", IsUnique = true, Order = 4)]
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

        [ForeignKey("ProductGroup")]
        [Display(Name = "Product Group")]
        public int? ProductGroupId { get; set; }
        public virtual ProductGroup ProductGroup { get; set; }
        public string ProductGroupName { get; set; }

        [ForeignKey("Size")]
        [Display(Name = "Size")]
        public int? SizeId { get; set; }
        public virtual Size Size { get; set; }
        public string SizeName { get; set; }


        [Display(Name = "Packing Length")]
        public Decimal PackingLength { get; set; }

        [Display(Name = "Packing Width")]
        public Decimal PackingWidth { get; set; }

        [Display(Name = "Packing Height")]
        public Decimal PackingHeight { get; set; }

        [Display(Name = "Packing Unit")]
        public string PackingUnitId { get; set; }

        [Display(Name = "Packing Gross Weight")]
        public Decimal PackingGrossWeight { get; set; }

        [Display(Name = "Packing Net Weight")]
        public Decimal PackingNetWeight { get; set; }

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

    public class PackingUnitSettingIndexViewModel
    {
        public int PersonId { get; set; }
        public string Name { get; set; }
    }
}
