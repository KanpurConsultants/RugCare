using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class PersonGodown : EntityBase, IHistoryLog
    {
        public PersonGodown()
        {
        }

        [Key]
        public int PersonGodownId { get; set; }

        [Display(Name = "Division"), Required]
        [ForeignKey("Division")]
        [Index("IX_PersonGodown_DocID", IsUnique = true, Order = 3)]
        public int DivisionId { get; set; }
        public virtual Division Division { get; set; }

        [Display(Name = "Site"), Required]
        [ForeignKey("Site")]
        [Index("IX_PersonGodown_DocID", IsUnique = true, Order = 4)]
        public int SiteId { get; set; }
        public virtual Site Site { get; set; }

        [ForeignKey("Person")]
        [Display(Name = "Person")]
        public int PersonId { get; set; }
        public virtual Person Person { get; set; }

        [ForeignKey("ProductCategory")]
        [Display(Name = "Product Category")]
        public int? ProductCategoryId { get; set; }
        public virtual ProductCategory ProductCategory { get; set; }

        [ForeignKey("Godown")]
        [Display(Name = "Godown")]
        public int? GodownId { get; set; }
        public virtual Godown Godown { get; set; }

        public string GodownCode { get; set; }

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
