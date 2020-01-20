using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class StockHeaderTransport: StockHeader
    {

        [ForeignKey("Transport"), Display(Name = "Transport")]
        public int ? TransportId { get; set; }
        public virtual Person Transport { get; set; }

        [MaxLength(20)]
        public string LrNo { get; set; }

        [Display(Name = "Lr Date")]
        public DateTime ? LrDate { get; set; }

        [Display(Name = "Private Mark")]
        [MaxLength(20)]
        public string PrivateMark { get; set; }

        [Display(Name = "Weight")]
        public Decimal? Weight { get; set; }

        [Display(Name = "ChargedWeight")]
        public Decimal? ChargedWeight { get; set; }

        [Display(Name = "Freight")]
        public Decimal? Freight { get; set; }


        [Display(Name = "Payment Type")]
        [MaxLength(20)]
        public string PaymentType { get; set; }

        [Display(Name = "Road Permit No")]
        [MaxLength(50)]
        public string RoadPermitNo { get; set; }

        [Display(Name = "Road Permit Date")]
        public DateTime ? RoadPermitDate { get; set; }


        [Display(Name = "EWay Bill No")]
        [MaxLength(50)]
        public string EWayBillNo { get; set; }

        [Display(Name = "EWay Bill Date")]
        public DateTime? EWayBillDate { get; set; }

        [Display(Name = "Upload Date")]
        public DateTime ? UploadDate { get; set; }

        [Display(Name = "Vehicle No")]
        [MaxLength(50)]
        public string VehicleNo { get; set; }

        //[Display(Name = "Ship Method")]
        //[ForeignKey("ShipMethod")]
        //public int? ShipMethodId { get; set; }
        //public virtual ShipMethod ShipMethod { get; set; }

        [Display(Name = "PreCarriage By")]
        [MaxLength(50)]
        public string PreCarriageBy { get; set; }


        [Display(Name = "PreCarriage Place")]
        [MaxLength(50)]
        public string PreCarriagePlace { get; set; }

        [Display(Name = "Booked From")]
        [MaxLength(50)]
        public string BookedFrom { get; set; }

        [Display(Name = "Booked To")]
        [MaxLength(50)]
        public string BookedTo { get; set; }


        [Display(Name = "Destination")]
        [MaxLength(100)]
        public string Destination { get; set; }


        [Display(Name = "Description Of Goods")]
        [MaxLength(500)]
        public string DescriptionOfGoods { get; set; }


        [Display(Name = "Description Of Packing")]
        [MaxLength(500)]
        public string DescriptionOfPacking { get; set; }


        [MaxLength(50)]
        public string OMSId { get; set; }
    }
}
