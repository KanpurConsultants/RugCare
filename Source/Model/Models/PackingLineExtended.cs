using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class PackingLineExtended : EntityBase
    {
        [Key]
        [ForeignKey("PackingLine")]
        [Display(Name = "PackingLine")]
        public int PackingLineId { get; set; }
        public virtual PackingLine PackingLine { get; set; }
        public Decimal? Length { get; set; }
        public Decimal? Width { get; set; }
        public Decimal? Height { get; set; }

        public Decimal? PackingLength { get; set; }
        public Decimal? PackingWidth { get; set; }
        public Decimal? PackingHeight { get; set; }

        [Display(Name = "Packing Unit")]
        [ForeignKey("PackingUnit")]
        public string PackingUnitId { get; set; }
        public virtual Unit PackingUnit { get; set; }

    }
}

