using Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DatabaseViews
{


    [Table("ViewCustomHeaderAttribute")]
    public class ViewCustomHeaderAttribute
    {
        [Key]
        public int CustomHeaderId { get; set; }
        public int SiteId { get; set; }
        public int DivisionId { get; set; }
        public int DocTypeId { get; set; }
        public DateTime DocDate { get; set; }
        public string DocNo { get; set; }
        public string Remark { get; set; }

    }

    [Table("ViewShipmentDetailAttribute")]
    public class ViewShipmentDetailAttribute
    {
        [Key]
        public int CustomHeaderId { get; set; }
        public int SiteId { get; set; }
        public int DivisionId { get; set; }
        public int DocTypeId { get; set; }
        public DateTime DocDate { get; set; }
        public string DocNo { get; set; }
        public string Remark { get; set; }

        public string ShippingBillNo { get; set; }
        public string ShippingBillDate { get; set; }
        public string BLNo { get; set; }
        public string BLDate { get; set; }
        public string BankReferenceNo { get; set; }
        public string BankReferenceDate { get; set; }
        public string DrawBackAmount { get; set; }
        public string DrawBackReceivedAmount { get; set; }

        public string LicenseNo { get; set; }
        public string LicenseAmount { get; set; }

        public string GoodsDispatchdate { get; set; }
        public string Transporter { get; set; }

    }

}
