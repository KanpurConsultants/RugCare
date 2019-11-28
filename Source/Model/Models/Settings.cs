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

        public string Value { get; set; }
        
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

    }
}
