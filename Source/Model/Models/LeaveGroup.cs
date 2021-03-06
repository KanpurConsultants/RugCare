﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class LeaveGroup : EntityBase, IHistoryLog
    {
        [Key]
        public int LeaveGroupId { get; set; }
        
        [Display (Name="Name")]
        [MaxLength(50, ErrorMessage = "LeaveGroup Name cannot exceed 50 characters"), Required]
        [Index("IX_LeaveGroup_LeaveGroupName", IsUnique = true)]
        public string LeaveGroupName { get; set; }
                     
        [Display(Name = "Is Active ?")]
        public Boolean IsActive { get; set; }

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
}
