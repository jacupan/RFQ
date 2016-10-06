using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace RFQWebApp.Models
{
    [Table("RfqBuyers")]
    public class BuyersAccess
    {
        [Key]
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserAccess { get; set; }        
    }
    
    [Table("RfqBuyersLookup")]
    public class BuyersLookup
    {
        [Key]
        public string Guid { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        [UIHint("AssignedBuyerForeignKey")]
        public string AssignedBuyer { get; set; }
        public string Remarks { get; set; }
        public string Comments { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }

    }

    [Table("RfqStatuses")]
    public class OverallStatus
    {
        [Key]
        public string Guid { get; set; }                
        public string RfqStatusCodeDesc { get; set; }
    }

    [Table("RfqRepeatOrderCode")]
    public class RepeatOrderCode
    {
        public string Guid { get; set; }
        [Key]
        public string IsRepeatOrderCode { get; set; }
        public string IsRepeatOrderCodeDesc { get; set; }
    }

    [Table("RfqItemStatusCode")]
    public class ItemStatusCodeDdl
    {
        public string Guid { get; set; }
        [Key]
        public string ItemStatusCode { get; set; }
        public string ItemStatusCodeDesc { get; set; }
    }

    [Table("RfqUomCode")]
    public class UomCode
    {
        public string Guid { get; set; }
        [Key]
        public string UoM { get; set; }
        public string UomCodeWithDesc { get; set; }
    }

    [Table("VN_RfQCategoryList")]
    public class CategoryList
    {
        [Key]
        public string Guid { get; set; }
        public string Category { get; set; }
    }

    [Table("RfqType")]
    public class RfqType
    {
        [Key]
        public string Guid { get; set; }
        public string Type { get; set; }
    }

    [Table("RfqAttachment")]
    public class RfqAttachments
    {
        [Key]
        public string FileID { get; set; }
        public string TransDetailsGuid { get; set; }
        public string RfqNumber { get; set; }
        public string FileName { get; set; }
        public byte[] FileBytes { get; set; }
        public string FileExtension { get; set; }
        public string FileIsDeleted { get; set; }        
    }

    [Table("RfqUserAttachment")]
    public class RfqUserAttachments
    {
        [Key]
        public string FileID { get; set; }
        public string TransDetailsGuid { get; set; }
        public string RfqNumber { get; set; }
        public string FileName { get; set; }
        public byte[] FileBytes { get; set; }
        public string FileExtension { get; set; }
        public string FileIsDeleted { get; set; }
    }

    [Table("RfqReAssignedBuyer")]
    public class RfqReAssignedBuyer
    {
        [Key]
        public string Gui { get; set; }
        public string RfqNumber { get; set; }
        public string ReassignedFromBuyer { get; set; }
        public string ReassignedToBuyer { get; set; }
        public string ReassignedBy { get; set; }
        public DateTime? ReassignedFromDate { get; set; }
        public DateTime? ReassignedToDate { get; set; }
    }



    [Table("RfqTransactions")]
    public class RfqTransactions
    {
        [Key]
        public string Guid { get; set; }        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 RfqNumber { get; set; }
        public string Requestor { get; set; }
        public string Username { get; set; }
        public string Department { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Category is required!")]
        public string Category { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Type is required!")]
        public string Type { get; set; }
        public string Buyer { get; set; }
        public string ReAssignedBuyerTo { get; set; }
        public string ReAssignedBuyerBy { get; set; }
        public DateTime? ReAssignedDateTo { get; set; }
        public string Status { get; set; }      
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public DateTime? DateSubmitted { get; set; }
        public DateTime? DateAcknowledged { get; set; }        
        public DateTime? DateCompleted { get; set; }
        public DateTime? DateCancelled { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string SubmittedBy { get; set; }
        public string AcknowledgedBy { get; set; }
        public string CancelledBy { get; set; }
        public string CompletedBy { get; set; }        
    }

    [Table("RfqTransactionDetails")]
    public class RfqTransactionDetails
    {
        [Key]
        public string Guid { get; set; }
        public string RfqNumber { get; set; }
        public string ItemName { get; set; }
        [DataType(DataType.MultilineText)]
        [StringLength(255, MinimumLength = 3)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Description is required!")]
        public string ItemDescription { get; set; }
        public string DrawingSpecification { get; set; }
        public string SupplierItemPN { get; set; }
        public string ItemNoOracleNo { get; set; }
        public string MachineModel { get; set; }
        public string SerialNo { get; set; }
        [Required(ErrorMessage = "Quantity is required!")]
        [Range(1, Int32.MaxValue)]
        public Int64 Quantity { get; set; }
        [UIHint("UomCodeForeignKey")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "UoM is required!")]
        public string UoM { get; set; }
        [UIHint("IsRepeatOrderCodeForeignKey")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Repeat Order is required!")]
        public string IsRepeatOrderCode { get; set; }        
        public string ReferencePrPo { get; set; }
        [DataType(DataType.MultilineText)]        
        public string Remarks { get; set; }
        public string ItemStatusCode { get; set; }
        public string SupplierName { get; set; }        
        public string Comments { get; set; }  
        public string Feedback { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public DateTime? DateAcknowledged { get; set; }
        public DateTime? DateCanvassed { get; set; }
        public DateTime? DateRejected { get; set; }
        public DateTime? DateCompleted { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string AcknowledgedBy { get; set; }
        public string CanvassedBy { get; set; }
        public string RejectedBy { get; set; }
        public string CompletedBy { get; set; }
    }

    //[Table("RfqTransactionDetails")]
    public class RfqTransactionDetailsBuyer
    {
        [Key]
        public string Guid { get; set; }
        public string RfqNumber { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public string DrawingSpecification { get; set; }
        public string SupplierItemPN { get; set; }
        public string ItemNoOracleNo { get; set; }
        public string MachineModel { get; set; }
        public string SerialNo { get; set; }
        public Int64 Quantity { get; set; }        
        public string UoM { get; set; }
        public string IsRepeatOrderCode { get; set; }
        public string ReferencePrPo { get; set; }        
        public string Remarks { get; set; }
        [UIHint("ItemStatusCodeForeignKey")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Buyer Status is required!")]
        public string ItemStatusCode { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "SupplierName is required!")]
        public string SupplierName { get; set; }
        [DataType(DataType.MultilineText)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Comment is required!")]
        public string Comments { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public DateTime? DateAcknowledged { get; set; }
        public DateTime? DateCanvassed { get; set; }
        public DateTime? DateRejected { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string AcknowledgedBy { get; set; }
        public string CanvassedBy { get; set; }
        public string RejectedBy { get; set; }
    }

    [Table("RfQReportByDateSubmitted")]
    public class RfqReportByDateSubmitted
    {
        [Key]
        public string Guid { get; set; }
        public string RfqNumber { get; set; }
        public string ItemName { get; set; }     
        public string ItemDescription { get; set; }
        public string SupplierItemPN { get; set; } 
        public Int64 Quantity { get; set; }   
        public string UoM { get; set; }
        public string SubmittedBy { get; set; }
        public string Department { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public string Buyer { get; set; }
        public string ReAssignedBuyerTo { get; set; }
        public DateTime? ReAssignedDateTo { get; set; }
        public DateTime? DateSubmitted { get; set; }
        public DateTime? DateAcknowledged { get; set; }
        public DateTime? DateCanvassed { get; set; }
        public DateTime? DateRejected { get; set; }
        public DateTime? DateCompleted { get; set; }
        public string ItemStatusCodeDesc { get; set; }
        public string Status { get; set; }
        public string Comments { get; set; }
        public string Remarks { get; set; }
        public string Feedback { get; set; }
       
    }

    [Table("RfqCount")]
    public class RfqCount
    {
        [Key]
        public string Guid { get; set; }
        public Int32 Submitted { get; set; }
        public Int32 Acknowledged { get; set; }
        public Int32 Opened { get; set; }
        public Int32 Cancelled { get; set; }
        public Int32 Completed { get; set; }
        public Int32 Draft { get; set; }
        public string DateCreated { get; set; }        
    }

    [Table("RfqCountChart")]
    public class RfqCountChart
    {
        [Key]
        public string Guid { get; set; }
        public Int32 RfqCount { get; set; }
        public string Status { get; set; }
        //public string DateMonthCreated { get; set; }
        //public DateTime? DateCreated { get; set; }
        public string DateCreated { get; set; }
    }  

}

