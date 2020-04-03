using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class Settings : EntityBase, IHistoryLog
    {

        [Key]
        public int SettingsId { get; set; }

        [MaxLength(100)]
        public string FieldName { get; set; }

        [ForeignKey("Site")]
        [Display(Name = "Site")]
        public int? SiteId { get; set; }
        public virtual Site Site { get; set; }

        [ForeignKey("Division")]
        [Display(Name = "Division")]
        public int? DivisionId { get; set; }
        public virtual Division Division { get; set; }

        [ForeignKey("DocCategory"), Display(Name = "Document Category")]
        public int? DocCategoryId { get; set; }
        public virtual DocumentCategory DocCategory { get; set; }


        [ForeignKey("DocType"), Display(Name = "Document Type")]
        public int ? DocTypeId { get; set; }
        public virtual DocumentType DocType { get; set; }

        [ForeignKey("ProductCategory"), Display(Name = "Product Category")]
        public int? ProductCategoryId { get; set; }
        public virtual ProductCategory ProductCategory { get; set; }


        [ForeignKey("ProductType"), Display(Name = "Product Type")]
        public int? ProductTypeId { get; set; }
        public virtual ProductType ProductType { get; set; }

        public string BaseHead { get; set; }

        public string BaseValue { get; set; }

        public string Value { get; set; }
        
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

    }
}
