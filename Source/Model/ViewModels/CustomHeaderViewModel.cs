using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Model.ViewModel;

namespace Model.ViewModels
{
    
	public class CustomHeaderViewModel
    {
        [Key]
        public int CustomHeaderId { get; set; }
        public int? DocId { get; set; }
        public int DocTypeId { get; set; }      
        public string DocumentTypeName { get; set; }

        [Display(Name = "Division")]
        public int DivisionId { get; set; }
        public string DivisionName { get; set; }
        [Display(Name = "Site")]
        public int SiteId { get; set; }
        public string SiteName { get; set; }

        [Display(Name = "Doc Date"), DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}"), Required(ErrorMessage = "Please select Doc Date")]
        public DateTime DocDate { get; set; }

        [Display(Name = "Doc No"), MaxLength(20), Required(ErrorMessage = "The DocNo Field is Required")]
        public string DocNo { get; set; }
        public List<DocumentTypeHeaderAttributeViewModel> DocumentTypeHeaderAttributes { get; set; }
        public DocumentTypeSettingsViewModel DocumentTypeSettings { get; set; }
        [Display(Name = "Remark")]
        public string Remark { get; set; }
    }


    public class CustomHeaderIndexViewModel
    {
        [Key]
        public int CustomHeaderId { get; set; }

        [ForeignKey("DocType"), Display(Name = "Custom Type"), Required(ErrorMessage = "Please select a Document type")]
        public int DocTypeId { get; set; }
        public string DocumentTypeName { get; set; }

        [Display(Name = "Custom Date"), DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}"), Required(ErrorMessage = "Please select Custom Date")]
        public DateTime DocDate { get; set; }

        [Display(Name = "Custom No"), MaxLength(20), Required(ErrorMessage = "The OrderNo Field is Required")]
        public string DocNo { get; set; }

        [Display(Name = "Division")]
        public int DivisionId { get; set; }
        public string DivisionName { get; set; }

        [Display(Name = "Site")]
        public int SiteId { get; set; }
        public string SiteName { get; set; }        
        
        public int Status { get; set; }        

        [Display(Name = "Remark")]
        public string Remark { get; set; }

        public string CreatedBy { get; set; }

        [Display(Name = "Created Date"), DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        [Display(Name = "Modified Date"), DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime ModifiedDate { get; set; }
        public string LockReason { get; set; }



    }

    public class ShipmentDetailIndexViewModel
    {
        [Key]
        public int CustomHeaderId { get; set; }

        [ForeignKey("DocType"), Display(Name = "Custom Type"), Required(ErrorMessage = "Please select a Document type")]
        public int DocTypeId { get; set; }
        public string DocumentTypeName { get; set; }

        [Display(Name = "Custom Date"), DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime ? DocDate { get; set; }

        [Display(Name = "Custom No"), MaxLength(20), Required(ErrorMessage = "The OrderNo Field is Required")]
        public string DocNo { get; set; }

        [Display(Name = "Division")]
        public int DivisionId { get; set; }
        public string DivisionName { get; set; }

        [Display(Name = "Site")]
        public int SiteId { get; set; }
        public string SiteName { get; set; }

        public int Status { get; set; }

        [Display(Name = "Remark")]
        public string Remark { get; set; }

        public string ShippingBillNo { get; set; }
        public string ShippingBillDate { get; set; }
        public string BLNo { get; set; }
        public string BLDate { get; set; }
        public string BankReferenceNo { get; set; }
        public string BankReferenceDate { get; set; }
        public string PortOfLoading { get; set; }
        public string DrawBackAmount { get; set; }
        public string DrawBackReceivedAmount { get; set; }
        public decimal  InvoiceAmount { get; set; }

        public string LicenseNo { get; set; }
        public string LicenseAmount { get; set; }

        public string GoodsDispatchdate { get; set; }
        public string Transporter { get; set; }
        

        public string CreatedBy { get; set; }

        [Display(Name = "Created Date"), DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        [Display(Name = "Modified Date"), DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime ModifiedDate { get; set; }
        public string LockReason { get; set; }



    }


}
