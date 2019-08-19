using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Models;
using System.ComponentModel.DataAnnotations;

namespace Model.ViewModel
{
    public class DocumentTypeTimeExtensionRequestViewModel
    {
        public int DocumentTypeTimeExtensionRequestId { get; set; }        

        [Required]
        public int? DocTypeId { get; set; }
        public string DocTypeName { get; set; }

        public int DivisionId { get; set; }
        public string DivisionName { get; set; }
        public int SiteId { get; set; }
        public string SiteName { get; set; }
        [Required]
        public string Type { get; set; }
        public decimal Days { get; set; }
        [Required]
        public DateTime ExpiryDate { get; set; }
        [Required]
        public bool IsApproved { get; set; }
        public string UserName { get; set; }
        [Required]
        public string Reason { get; set; }
        public int NoOfRecords { get; set; }
        [Required]
        public DateTime DocDate { get; set; }        

    }

}
