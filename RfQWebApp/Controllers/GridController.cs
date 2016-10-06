using System;
﻿using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using RFQWebApp.Models;
using System.Data.Objects;
using RfQWebApp.Classes;
using System.Configuration;
using System.Data.Objects.SqlClient;

namespace RfQWebApp.Controllers
{
    public partial class GridController : Controller
    {
        public RfQDBContext _rfqDbContext = new RfQDBContext();

        public ActionResult RfqTransactions_Read([DataSourceRequest]DataSourceRequest request, string username)
        {
            username = Common.GetWebCurrentUser(RfQWebApp.Classes.Common.WebUserInformation.Username);

            var rfqResult = (from rfq in _rfqDbContext.RfqTransactions
                             where rfq.CreatedBy == username && rfq.Status == "Draft"
                             select new
                             {
                                 RfqNumber = rfq.RfqNumber,
                                 Status = rfq.Status,
                                 DateCreated = rfq.DateCreated
                             }).OrderByDescending(a => a.DateCreated).ToList();
            return Json(rfqResult.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult RfqTransactionsChart_Read([DataSourceRequest]DataSourceRequest request)
        {            
            var rfqResult = (from rfq in _rfqDbContext.RfqCount                             
                             select new
                             {
                                 Guid = Guid.NewGuid(),
                                 Submitted = rfq.Submitted,
                                 Acknowledged = rfq.Acknowledged,
                                 Opened = rfq.Opened,
                                 Cancelled = rfq.Cancelled,
                                 Completed = rfq.Completed,
                                 Draft = rfq.Draft,
                                 DateCreated = rfq.DateCreated
                             }).ToList();
            return Json(rfqResult.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult RfqTransactionsChartSum_Read()
        {
            var rfqResult = (from rfq in _rfqDbContext.RfqCountChart
                             select new
                             {
                                 Guid = Guid.NewGuid(),
                                 RfqCount = rfq.RfqCount,
                                 Status = rfq.Status,
                                 //DateMonthCreated = rfq.DateMonthCreated
                                 DateCreated = rfq.DateCreated
                             }).ToList();
            //return Json(rfqResult.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

            return Json(rfqResult, JsonRequestBehavior.AllowGet);
            
        }

        public ActionResult RfqTransactionsNotDraft_Read([DataSourceRequest]DataSourceRequest request, string username)
        {
            username = Common.GetWebCurrentUser(RfQWebApp.Classes.Common.WebUserInformation.Username);


            var rfqResult = (from rfq in _rfqDbContext.RfqTransactions
                             where rfq.CreatedBy == username && new[] { "Opened", "Completed", "Cancelled", "Acknowledged" }.Contains(rfq.Status)
                             select new
                             {
                                 RfqNumber = rfq.RfqNumber,
                                 Status = rfq.Status,
                                 DateCreated = rfq.DateCreated
                             }).OrderByDescending(a => a.DateCreated).ToList();
            return Json(rfqResult.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult RfqTransactionsDetailsNotDraft_Read([DataSourceRequest]DataSourceRequest request, string username)
        {
            username = Common.GetWebCurrentUser(RfQWebApp.Classes.Common.WebUserInformation.Username);

            var rfqResult = (from rfq in _rfqDbContext.RfqTransactionDetails
                             where rfq.CreatedBy == username && rfq.ItemStatusCode != null && SqlFunctions.DateDiff("day", rfq.DateCreated, DateTime.Now) <= 60 //rfq.ItemStatusCode.Trim() == "1"
                             select new
                             {
                                 RfqNumber = rfq.RfqNumber,
                                 ItemDescription = rfq.ItemDescription,
                                 ItemStatusCode = rfq.ItemStatusCode.Trim() == "0" ? "Acknowledged" : rfq.ItemStatusCode.Trim() == "1" ? "Canvassed" : rfq.ItemStatusCode.Trim() == "2" ? "Rejected" : rfq.ItemStatusCode.Trim() == "3" ? "Completed" : rfq.ItemStatusCode,
                                 DateCreated = rfq.DateCreated
                             }).OrderByDescending(a => a.DateCreated).ToList();
            return Json(rfqResult.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult RfqTransactionsBuyerView_Read([DataSourceRequest]DataSourceRequest request, string username)
        {
            username = Common.GetWebCurrentUser(RfQWebApp.Classes.Common.WebUserInformation.Username);
            //username = "AALVAREZ";
            var userInfo = DataRetrieval.UserProfile(username);
            string buyerName = userInfo[0].FullName.ToString();

            var rfqResult = (from rfq in _rfqDbContext.RfqTransactions
                             where (rfq.Buyer == buyerName || rfq.ReAssignedBuyerTo == buyerName) && rfq.Status != "Draft" //new[] { "Opened", "Completed", "Cancelled", "Acknowledged" }.Contains(rfq.Status)
                             select new
                             {
                                 RfqNumber = rfq.RfqNumber,
                                 Status = rfq.Status,
                                 DateCreated = rfq.DateCreated
                             }).OrderByDescending(a => a.DateCreated).ToList();
            return Json(rfqResult.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult RfqTransDetails_Read([DataSourceRequest]DataSourceRequest request, string rfqNo)
        {
            using (_rfqDbContext)
            {
                IQueryable<RfqTransactionDetails> rfq = (from rfqTransDetails in _rfqDbContext.RfqTransactionDetails
                                                         where rfqTransDetails.RfqNumber == rfqNo
                                                         orderby rfqTransDetails.ItemName
                                                         select rfqTransDetails
                                                        );

                DataSourceResult result = rfq.ToDataSourceResult(request);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult RfqTransDetails_Create([DataSourceRequest]DataSourceRequest request, RfqTransactionDetails rfq)
        {
            if (ModelState.IsValid)
            {
                using (_rfqDbContext)
                {
                    // Create a new from Model class
                    var entity = new RfqTransactionDetails
                    {
                        Guid = System.Guid.NewGuid().ToString().ToUpper(),
                        //Guid = rfq.Guid,                        
                        RfqNumber = rfq.RfqNumber,
                        ItemName = rfq.ItemName,
                        ItemDescription = rfq.ItemDescription,
                        DrawingSpecification = rfq.DrawingSpecification,
                        SupplierItemPN = rfq.SupplierItemPN,
                        ItemNoOracleNo = rfq.ItemNoOracleNo,
                        MachineModel = rfq.MachineModel,
                        SerialNo = rfq.SerialNo,
                        Quantity = rfq.Quantity,
                        UoM = rfq.UoM,
                        IsRepeatOrderCode = rfq.IsRepeatOrderCode,
                        ReferencePrPo = rfq.ReferencePrPo,
                        Remarks = rfq.Remarks,
                        CreatedBy = RfQWebApp.Classes.Common.GetWebCurrentUser(RfQWebApp.Classes.Common.WebUserInformation.Username),
                        DateCreated = DateTime.Now
                    };
                    // Add the entity
                    _rfqDbContext.RfqTransactionDetails.Add(entity);
                    // Insert the entity in the database
                    _rfqDbContext.SaveChanges();
                    // Get the ProductID generated by the database
                    rfq.Guid = entity.Guid;
                    rfq.DateCreated = entity.DateCreated;
                    rfq.CreatedBy = entity.CreatedBy;
                    rfq.IsRepeatOrderCode = entity.IsRepeatOrderCode;
                }
            }
            // Return the inserted product. The grid needs the generated ProductID. Also return any validation errors.
            return Json(new[] { rfq }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RfqTransDetails_Update([DataSourceRequest]DataSourceRequest request, RfqTransactionDetails rfq)
        {

            if (rfq != null && ModelState.IsValid)
            {
                using (_rfqDbContext)
                {
                    // Create a new from Model class
                    var entity = new RfqTransactionDetails
                    {
                        Guid = rfq.Guid,
                        RfqNumber = rfq.RfqNumber,
                        ItemName = rfq.ItemName,
                        ItemDescription = rfq.ItemDescription,
                        DrawingSpecification = rfq.DrawingSpecification,
                        SupplierItemPN = rfq.SupplierItemPN,
                        ItemNoOracleNo = rfq.ItemNoOracleNo,
                        MachineModel = rfq.MachineModel,
                        SerialNo = rfq.SerialNo,
                        Quantity = rfq.Quantity,
                        UoM = rfq.UoM,
                        IsRepeatOrderCode = rfq.IsRepeatOrderCode,
                        ReferencePrPo = rfq.ReferencePrPo,
                        Remarks = rfq.Remarks,
                        ItemStatusCode = rfq.ItemStatusCode,
                        SupplierName = rfq.SupplierName,
                        Comments = rfq.Comments,
                        DateCreated = rfq.DateCreated,
                        CreatedBy = rfq.CreatedBy,
                        DateAcknowledged = rfq.DateAcknowledged,
                        AcknowledgedBy = rfq.AcknowledgedBy,
                        ModifiedBy = RfQWebApp.Classes.Common.GetWebCurrentUser(RfQWebApp.Classes.Common.WebUserInformation.Username),
                        DateModified = DateTime.Now
                    };

                    // Attach the entity
                    _rfqDbContext.RfqTransactionDetails.Attach(entity);
                    // Change its state to Modified so Entity Framework can update the existing product instead of creating a new one
                    _rfqDbContext.Entry(entity).State = EntityState.Modified;

                    // Or use ObjectStateManager if using a previous version of Entity Framework
                    // northwind.ObjectStateManager.ChangeObjectState(entity, EntityState.Modified);

                    // Update the entity in the database
                    _rfqDbContext.SaveChanges();

                    // Get the ProductID generated by the database
                    rfq.ModifiedBy = entity.ModifiedBy;
                    rfq.DateModified = entity.DateModified;
                }
            }
            // Return the updated product. Also return any validation errors.
            return Json(new[] { rfq }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RfqTransDetails_Destroy([DataSourceRequest]DataSourceRequest request, RfqTransactionDetails rfq)
        {
            if (ModelState.IsValid)
            {
                using (_rfqDbContext)
                {
                    // Create a new from Model class
                    var entity = new RfqTransactionDetails
                    {
                        Guid = rfq.Guid,
                        RfqNumber = rfq.RfqNumber,
                        //ItemName = rfq.ItemName,
                        ItemDescription = rfq.ItemDescription,
                        //DrawingSpecification = rfq.DrawingSpecification,
                        SupplierItemPN = rfq.SupplierItemPN,
                        ItemNoOracleNo = rfq.ItemNoOracleNo,
                        MachineModel = rfq.MachineModel,
                        SerialNo = rfq.SerialNo,
                        Quantity = rfq.Quantity,
                        UoM = rfq.UoM,
                        IsRepeatOrderCode = rfq.IsRepeatOrderCode,
                        ReferencePrPo = rfq.ReferencePrPo,
                        Remarks = rfq.Remarks
                    };
                    // Attach the entity
                    _rfqDbContext.RfqTransactionDetails.Attach(entity);
                    // Delete the entity
                    _rfqDbContext.RfqTransactionDetails.Remove(entity);
                    // Or use DeleteObject if using a previous versoin of Entity Framework
                    // northwind.Products.DeleteObject(entity);
                    // Delete the entity in the database
                    _rfqDbContext.SaveChanges();
                }
            }
            // Return the removed product. Also return any validation errors.
            return Json(new[] { rfq }.ToDataSourceResult(request, ModelState));
        }


        public ActionResult RfqTransDetailsBuyer_Read([DataSourceRequest]DataSourceRequest request, string rfqNo)
        {
            using (_rfqDbContext)
            {
                IQueryable<RfqTransactionDetails> rfq = (from rfqTransDetails in _rfqDbContext.RfqTransactionDetails
                                                         where rfqTransDetails.RfqNumber == rfqNo
                                                         orderby rfqTransDetails.ItemName
                                                         select rfqTransDetails
                                                        );

                DataSourceResult result = rfq.ToDataSourceResult(request);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RfqTransDetailsBuyer_Update([DataSourceRequest]DataSourceRequest request, RfqTransactionDetails rfq)
        {
            //if (rfq.ItemStatusCode.Trim() == "2" && rfq.Comments == null)
            //{
            //    ModelState.AddModelError("Comments", "Comments are required if Item Status is Rejected");
            //}

            //if (rfq.ItemStatusCode.Trim() == "3")
            //{
            //    ModelState.AddModelError("ItemStatusCode", "Status to complete can only be set by user");
            //}

           

            if (rfq != null && ModelState.IsValid)
            {
                using (_rfqDbContext)
                {
                    // Create a new from Model class
                    var entity = new RfqTransactionDetails
                    {
                        Guid = rfq.Guid,
                        RfqNumber = rfq.RfqNumber,
                        ItemName = rfq.ItemName,
                        ItemDescription = rfq.ItemDescription,
                        DrawingSpecification = rfq.DrawingSpecification,
                        SupplierItemPN = rfq.SupplierItemPN,
                        ItemNoOracleNo = rfq.ItemNoOracleNo,
                        MachineModel = rfq.MachineModel,
                        SerialNo = rfq.SerialNo,
                        Quantity = rfq.Quantity,
                        UoM = rfq.UoM,
                        IsRepeatOrderCode = rfq.IsRepeatOrderCode,
                        ReferencePrPo = rfq.ReferencePrPo,
                        Remarks = rfq.Remarks,
                        ItemStatusCode = rfq.ItemStatusCode,
                        SupplierName = rfq.SupplierName,
                        Comments = rfq.Comments,
                        DateCreated = rfq.DateCreated,
                        CreatedBy = rfq.CreatedBy,
                        DateAcknowledged = rfq.ItemStatusCode.Trim() != "0" ? rfq.DateAcknowledged : DateTime.Now,
                        AcknowledgedBy = rfq.ItemStatusCode.Trim() != "0" ? rfq.AcknowledgedBy : RfQWebApp.Classes.Common.GetWebCurrentUser(RfQWebApp.Classes.Common.WebUserInformation.Username),
                        DateCanvassed = rfq.ItemStatusCode.Trim() != "1" ? rfq.DateCanvassed : DateTime.Now,
                        CanvassedBy = rfq.ItemStatusCode.Trim() != "1" ? rfq.CanvassedBy : RfQWebApp.Classes.Common.GetWebCurrentUser(RfQWebApp.Classes.Common.WebUserInformation.Username),
                        DateRejected = rfq.ItemStatusCode.Trim() != "2" ? rfq.DateRejected : DateTime.Now,
                        RejectedBy = rfq.ItemStatusCode.Trim() != "2" ? rfq.RejectedBy : RfQWebApp.Classes.Common.GetWebCurrentUser(RfQWebApp.Classes.Common.WebUserInformation.Username),
                        ModifiedBy = rfq.ModifiedBy,
                        DateModified = rfq.DateModified
                    };

                    // Attach the entity
                    _rfqDbContext.RfqTransactionDetails.Attach(entity);
                    // Change its state to Modified so Entity Framework can update the existing product instead of creating a new one
                    _rfqDbContext.Entry(entity).State = EntityState.Modified;

                    // Or use ObjectStateManager if using a previous version of Entity Framework
                    // northwind.ObjectStateManager.ChangeObjectState(entity, EntityState.Modified);

                    // Update the entity in the database
                    _rfqDbContext.SaveChanges();

                    // Get the ProductID generated by the database
                    rfq.AcknowledgedBy = entity.AcknowledgedBy;
                    rfq.DateAcknowledged = entity.DateAcknowledged;
                    rfq.CanvassedBy = entity.CanvassedBy;
                    rfq.DateCanvassed = entity.DateCanvassed;
                    rfq.RejectedBy = entity.RejectedBy;
                    rfq.DateRejected = entity.DateRejected;
                    rfq.ItemStatusCode = entity.ItemStatusCode;

                    rfq.DateCreated = entity.DateCreated;
                    rfq.CreatedBy = entity.CreatedBy;
                    rfq.DateModified = entity.DateModified;
                    rfq.ModifiedBy = entity.ModifiedBy;

                    var rfqNo = Int64.Parse(rfq.RfqNumber);

                    RfqTransactions rfqTrans = _rfqDbContext.RfqTransactions.Single(a => a.RfqNumber == rfqNo);

                    var rfqBuyer = rfqTrans.ReAssignedBuyerTo == null ? rfqTrans.Buyer : rfqTrans.ReAssignedBuyerTo == "" ? rfqTrans.Buyer : rfqTrans.ReAssignedBuyerTo;

                    if (rfq.ItemStatusCode.Trim() == "1")
                    {
                        SendEmailNotificationItemCanvassed(rfq.RfqNumber, rfq.Guid, rfqTrans.Requestor, rfqTrans.Username + "@llegromicro.com", rfqTrans.Department, rfqBuyer);
                    }

                }
            }
            // Return the updated product. Also return any validation errors.
            return Json(new[] { rfq }.ToDataSourceResult(request, ModelState));
        }


        public ActionResult SendEmailNotificationItemCanvassed(string rfqNumber, string rfqTransGuid, string userFullname, string userEmail, string dept, string rfqBuyer)
        {
            var buyerEmail = (from a in _rfqDbContext.BuyersAccess
                              where a.FullName == rfqBuyer
                              select new
                              {
                                  Email = a.Email
                              }).ToList();

            var rfqNo = Int64.Parse(rfqNumber);

            var requestor = (from a in _rfqDbContext.RfqTransactions
                             where a.RfqNumber == rfqNo
                             select new
                             {
                                 Requestor = a.Requestor,
                                 Username = a.Username,
                                 Department = a.Department
                             }).ToList();

            var errorMsg = "";
            string DLINKA = "";
            string DLINKB = "";
            string Subject = "";
            string buyEmail = buyerEmail[0].Email.ToString();

            //userFullname = requestor[0].Requestor;
            userEmail = requestor[0].Username + "@allegromicro.com";
            //dept = requestor[0].Department;
            Subject = "RFQ Canvassed - RFQ # " + rfqNumber + "";
            //DLINKA = "http://ampiweb01:8099/Home/UpdateRfQStatus?rfqNumber=" + rfqNumber;
            //DLINKB = "http://ampiweb01:8099/Home/ViewAckCanRejRfQ?rfqNumber=" + rfqNumber;

            //DLINKA = "http://ampiappdev1:8095/Home/UpdateRfQStatus?rfqNumber=" + rfqNumber;
            //DLINKB = "http://ampiappdev1:8095/Home/ViewAckCanRejRfQ?rfqNumber=" + rfqNumber;

            //DLINKA = "http://ampiappdev1:8094/Home/UpdateRfQStatus?rfqNumber=" + rfqNumber;
            //DLINKB = "http://ampiappdev1:8094/Home/ViewAckCanRejRfQ?rfqNumber=" + rfqNumber;

            //DLINKA = "http://localhost:63734/Home/UpdateRfQStatus?rfqNumber=" + rfqNumber;
            //DLINKB = "http://localhost:63734/Home/ViewAckCanRejRfQ?rfqNumber=" + rfqNumber;

            //DLINKA = "http://localhost:63734/Home/UpdateRfQStatus?rfqNumber=" + rfqNumber;
            //DLINKB = "http://localhost:63734/Home/ViewRFQStatusViaMail?rfqNumber=" + rfqNumber;

            //DLINKA = "http://ampiappdev1:8095/Home/UpdateRfQStatus?rfqNumber=" + rfqNumber;
            //DLINKB = "http://ampiappdev1:8095/Home/ViewRFQStatusViaMail?rfqNumber=" + rfqNumber;

            DLINKA = "http://ampiweb01:8099/Home/UpdateRfQStatus?rfqNumber=" + rfqNumber;
            DLINKB = "http://ampiweb01:8099/Home/ViewRFQStatusViaMail?rfqNumber=" + rfqNumber;

            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["RfQContext"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    using (SqlCommand cmd1 = new SqlCommand())
                    {
                        cmd1.Connection = conn;
                        cmd1.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd1.CommandText = "AutoEmail_ItemCanvassed_BuyerView";
                        cmd1.Parameters.Add("@RfQNo", SqlDbType.NVarChar).Value = rfqNumber;
                        cmd1.Parameters.Add("@Guid", SqlDbType.NVarChar).Value = rfqTransGuid;
                        cmd1.Parameters.Add("@REQUESTOR", SqlDbType.NVarChar).Value = userFullname;
                        cmd1.Parameters.Add("@DEPARTMENT", SqlDbType.NVarChar).Value = dept;
                        cmd1.Parameters.Add("@SUBJ", SqlDbType.NVarChar).Value = Subject;
                        cmd1.Parameters.Add("@DLINKA", SqlDbType.NVarChar).Value = DLINKA;
                        cmd1.Parameters.Add("@RECIP", SqlDbType.NVarChar).Value = buyEmail;
                        //cmd1.Parameters.Add("@RECIP", SqlDbType.NVarChar).Value = userEmail;


                        cmd1.ExecuteNonQuery();
                    }

                    using (SqlCommand cmd2 = new SqlCommand())
                    {
                        cmd2.Connection = conn;
                        cmd2.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd2.CommandText = "AutoEmail_ItemCanvassed_UserView";
                        cmd2.Parameters.Add("@RfQNo", SqlDbType.NVarChar).Value = rfqNumber;
                        cmd2.Parameters.Add("@Guid", SqlDbType.NVarChar).Value = rfqTransGuid;
                        cmd2.Parameters.Add("@REQUESTOR", SqlDbType.NVarChar).Value = userFullname;
                        cmd2.Parameters.Add("@DEPARTMENT", SqlDbType.NVarChar).Value = dept;
                        cmd2.Parameters.Add("@SUBJ", SqlDbType.NVarChar).Value = Subject;
                        cmd2.Parameters.Add("@DLINKB", SqlDbType.NVarChar).Value = DLINKB;
                        cmd2.Parameters.Add("@RECIP", SqlDbType.NVarChar).Value = userEmail;

                        cmd2.ExecuteNonQuery();
                    }
                }
            }


            catch (SqlException ex)
            {

                errorMsg = ("SQL Error" + ex.Message.ToString());

            }

            return Json("success", JsonRequestBehavior.AllowGet);


        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RfqTransDetailsFeedback_Update([DataSourceRequest]DataSourceRequest request, RfqTransactionDetails rfq)
        {
            //if (rfq.Feedback.Length <= 5)
            //{
            //    ModelState.AddModelError("Feedback", "Feedback should be at least five characters long.");

            //}

            if (rfq != null && ModelState.IsValid)
            {
                using (_rfqDbContext)
                {
                    // Create a new from Model class
                    var entity = new RfqTransactionDetails
                    {
                        Guid = rfq.Guid,
                        RfqNumber = rfq.RfqNumber,
                        ItemName = rfq.ItemName,
                        ItemDescription = rfq.ItemDescription,
                        DrawingSpecification = rfq.DrawingSpecification,
                        SupplierItemPN = rfq.SupplierItemPN,
                        ItemNoOracleNo = rfq.ItemNoOracleNo,
                        MachineModel = rfq.MachineModel,
                        SerialNo = rfq.SerialNo,
                        Quantity = rfq.Quantity,
                        UoM = rfq.UoM,
                        IsRepeatOrderCode = rfq.IsRepeatOrderCode,
                        ReferencePrPo = rfq.ReferencePrPo,
                        Remarks = rfq.Remarks,
                        ItemStatusCode = rfq.ItemStatusCode,
                        SupplierName = rfq.SupplierName,
                        Comments = rfq.Comments,
                        Feedback = rfq.Feedback,
                        DateCreated = rfq.DateCreated,
                        CreatedBy = rfq.CreatedBy,
                        DateAcknowledged = rfq.DateAcknowledged,
                        AcknowledgedBy = rfq.AcknowledgedBy,
                        DateCanvassed = rfq.DateCanvassed,
                        CanvassedBy = rfq.CanvassedBy,
                        DateRejected = rfq.DateRejected,
                        RejectedBy = rfq.RejectedBy,
                        ModifiedBy = rfq.ModifiedBy,
                        DateModified = rfq.DateModified
                    };

                    // Attach the entity
                    _rfqDbContext.RfqTransactionDetails.Attach(entity);
                    // Change its state to Modified so Entity Framework can update the existing product instead of creating a new one
                    _rfqDbContext.Entry(entity).State = EntityState.Modified;

                    // Or use ObjectStateManager if using a previous version of Entity Framework
                    // northwind.ObjectStateManager.ChangeObjectState(entity, EntityState.Modified);

                    // Update the entity in the database
                    _rfqDbContext.SaveChanges();

                    // Get the ProductID generated by the database
                    rfq.AcknowledgedBy = entity.AcknowledgedBy;
                    rfq.DateAcknowledged = entity.DateAcknowledged;

                    rfq.CanvassedBy = entity.CanvassedBy;
                    rfq.DateCanvassed = entity.DateCanvassed;

                    rfq.RejectedBy = entity.RejectedBy;
                    rfq.DateRejected = entity.DateRejected;

                    rfq.DateCreated = entity.DateCreated;
                    rfq.CreatedBy = entity.CreatedBy;

                    rfq.DateModified = entity.DateModified;
                    rfq.ModifiedBy = entity.ModifiedBy;

                    rfq.ItemStatusCode = entity.ItemStatusCode;
                }
            }
            // Return the updated product. Also return any validation errors.
            return Json(new[] { rfq }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult RfqTransDetailsView_Read([DataSourceRequest]DataSourceRequest request, string rfqNo)
        {
            //var rfqResult = (from rfq in _rfqDbContext.RfqTransactionDetails
            //                 where rfq.RfqNumber == rfqNo
            //                 select new
            //                 {
            //                     Guid = rfq.Guid,
            //                     RfqNumber = rfq.RfqNumber,
            //                     ItemName = rfq.ItemName,
            //                     ItemDescription = rfq.ItemDescription,
            //                     DrawingSpecification = rfq.DrawingSpecification,
            //                     SupplierItemPN = rfq.SupplierItemPN,
            //                     ItemNoOracleNo = rfq.ItemNoOracleNo,
            //                     MachineModel = rfq.MachineModel,
            //                     SerialNo = rfq.SerialNo,
            //                     Quantity = rfq.Quantity,
            //                     UoM = rfq.UoM,
            //                     IsRepeatOrderCode = rfq.IsRepeatOrderCode,
            //                     ReferencePrPo = rfq.ReferencePrPo,
            //                     Remarks = rfq.Remarks,
            //                     ItemStatusCode = rfq.ItemStatusCode,
            //                     SupplierName = rfq.SupplierName,
            //                     Comments = rfq.Comments

            //                 }).ToList();
            //return Json(rfqResult.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            using (_rfqDbContext)
            {
                IQueryable<RfqTransactionDetails> rfq = (from rfqTransDetails in _rfqDbContext.RfqTransactionDetails
                                                         where rfqTransDetails.RfqNumber == rfqNo
                                                         orderby rfqTransDetails.ItemDescription
                                                         select rfqTransDetails
                                                        );

                DataSourceResult result = rfq.ToDataSourceResult(request);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public class PseudoRfQAttachments : RfqAttachments { }

        public ActionResult RfqTransDetailsFilesUserView_Read([DataSourceRequest]DataSourceRequest request, string rfqTransDetailsGuid)
        {
            //    var rfqResult = (from rfq in _rfqDbContext.RfqAttachments
            //                     where rfq.TransDetailsGuid == rfqTransDetailsGuid
            //                     select new
            //                     {
            //                         FileID = rfq.FileID,
            //                         TransDetailsGuid = rfq.TransDetailsGuid,
            //                         RfqNumber = rfq.RfqNumber,
            //                         FileName = rfq.FileName,
            //                         FileBytes = rfq.FileBytes,
            //                         FileExtension = rfq.FileExtension,
            //                         FileIsDeleted = rfq.FileIsDeleted
            //                     }).ToList();
            //    return Json(rfqResult.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

            IQueryable<RfqAttachments> rfqFiles = (from rfq in _rfqDbContext.RfqAttachments
                                                   //where rfq.TransDetailsGuid == rfqTransDetailsGuid && rfq.FileName.Contains("best")
                                                   where rfq.TransDetailsGuid == rfqTransDetailsGuid && rfq.FileName.StartsWith("best")
                                                   select new PseudoRfQAttachments()
                                                    {
                                                        FileID = rfq.FileID,
                                                        TransDetailsGuid = rfq.TransDetailsGuid,
                                                        RfqNumber = rfq.RfqNumber,
                                                        FileName = rfq.FileName,
                                                        //FileBytes = rfq.FileBytes,
                                                        FileExtension = rfq.FileExtension,
                                                        FileIsDeleted = rfq.FileIsDeleted
                                                    });

            DataSourceResult rfqResult = rfqFiles.ToDataSourceResult(request);
            return Json(rfqResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RfqTransDetailsDrawSpecUserView_Read([DataSourceRequest]DataSourceRequest request, string rfqTransDetailsGuid)
        {
            var rfqResult = (from rfq in _rfqDbContext.RfqUserAttachments
                             where rfq.TransDetailsGuid == rfqTransDetailsGuid
                             select new
                             {
                                 FileID = rfq.FileID,
                                 TransDetailsGuid = rfq.TransDetailsGuid,
                                 RfqNumber = rfq.RfqNumber,
                                 FileName = rfq.FileName,
                                 //FileBytes = rfq.FileBytes,
                                 FileExtension = rfq.FileExtension,
                                 FileIsDeleted = rfq.FileIsDeleted
                             }).ToList();
            return Json(rfqResult.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult RfqTransDetailsFilesBuyerView_Read([DataSourceRequest]DataSourceRequest request, string rfqTransDetailsGuid)
        {
            IQueryable<RfqAttachments> rfqFiles = (from rfq in _rfqDbContext.RfqAttachments
                                                   where rfq.TransDetailsGuid == rfqTransDetailsGuid
                                                   select new PseudoRfQAttachments()
                                                   {
                                                       FileID = rfq.FileID,
                                                       TransDetailsGuid = rfq.TransDetailsGuid,
                                                       RfqNumber = rfq.RfqNumber,
                                                       FileName = rfq.FileName,
                                                       //FileBytes = rfq.FileBytes,
                                                       FileExtension = rfq.FileExtension,
                                                       FileIsDeleted = rfq.FileIsDeleted
                                                   });

            DataSourceResult rfqResult = rfqFiles.ToDataSourceResult(request);
            return Json(rfqResult, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RfqTransDetailsFiles_Destroy([DataSourceRequest]DataSourceRequest request, RfqAttachments rfq)
        {
            if (ModelState.IsValid)
            {
                using (_rfqDbContext)
                {
                    // Create a new from Model class
                    var entity = new RfqAttachments
                    {
                        FileID = rfq.FileID,
                        TransDetailsGuid = rfq.TransDetailsGuid,
                        RfqNumber = rfq.RfqNumber,
                        FileName = rfq.FileName,
                        FileBytes = rfq.FileBytes,
                        FileExtension = rfq.FileExtension,
                        FileIsDeleted = rfq.FileIsDeleted
                    };
                    // Attach the entity
                    _rfqDbContext.RfqAttachments.Attach(entity);
                    // Delete the entity
                    _rfqDbContext.RfqAttachments.Remove(entity);
                    // Or use DeleteObject if using a previous versoin of Entity Framework
                    // northwind.Products.DeleteObject(entity);
                    // Delete the entity in the database
                    _rfqDbContext.SaveChanges();
                }
            }
            // Return the removed product. Also return any validation errors.
            return Json(new[] { rfq }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RfqTransDetailsDrawSpec_Destroy([DataSourceRequest]DataSourceRequest request, RfqUserAttachments rfq)
        {
            if (ModelState.IsValid)
            {
                using (_rfqDbContext)
                {
                    // Create a new from Model class
                    var entity = new RfqUserAttachments
                    {
                        FileID = rfq.FileID,
                        TransDetailsGuid = rfq.TransDetailsGuid,
                        RfqNumber = rfq.RfqNumber,
                        FileName = rfq.FileName,
                        FileBytes = rfq.FileBytes,
                        FileExtension = rfq.FileExtension,
                        FileIsDeleted = rfq.FileIsDeleted
                    };
                    // Attach the entity
                    _rfqDbContext.RfqUserAttachments.Attach(entity);
                    // Delete the entity
                    _rfqDbContext.RfqUserAttachments.Remove(entity);
                    // Or use DeleteObject if using a previous versoin of Entity Framework
                    // northwind.Products.DeleteObject(entity);
                    // Delete the entity in the database
                    _rfqDbContext.SaveChanges();
                }
            }
            // Return the removed product. Also return any validation errors.
            return Json(new[] { rfq }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult RfqTransDetailsReport_Read([DataSourceRequest]DataSourceRequest request, string strBuyer, string strStat, DateTime? startDateSubmitted, DateTime? endDateSubmitted)
        {
            using (_rfqDbContext)
            {
                if (strBuyer != "" && strStat == "")
                {
                    IQueryable<RfqReportByDateSubmitted> rfq = (from rfqReport in _rfqDbContext.RfqReportByDateSubmitted
                                                                where (rfqReport.Buyer == strBuyer || rfqReport.ReAssignedBuyerTo == strBuyer) && (rfqReport.DateSubmitted >= startDateSubmitted && rfqReport.DateSubmitted <= endDateSubmitted)
                                                                orderby rfqReport.RfqNumber
                                                                select rfqReport
                                                        );

                    DataSourceResult result = rfq.ToDataSourceResult(request);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                else if (strBuyer == "" && strStat != "")
                {
                    IQueryable<RfqReportByDateSubmitted> rfq = (from rfqReport in _rfqDbContext.RfqReportByDateSubmitted
                                                                where rfqReport.ItemStatusCodeDesc == strStat && (rfqReport.DateSubmitted >= startDateSubmitted && rfqReport.DateSubmitted <= endDateSubmitted)
                                                                orderby rfqReport.RfqNumber
                                                                select rfqReport
                                                        );

                    DataSourceResult result = rfq.ToDataSourceResult(request);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                else if (strBuyer != "" && strStat != "")
                {
                    IQueryable<RfqReportByDateSubmitted> rfq = (from rfqReport in _rfqDbContext.RfqReportByDateSubmitted
                                                                where (rfqReport.Buyer == strBuyer || rfqReport.ReAssignedBuyerTo == strBuyer) && rfqReport.ItemStatusCodeDesc == strStat && (rfqReport.DateSubmitted >= startDateSubmitted && rfqReport.DateSubmitted <= endDateSubmitted)
                                                                orderby rfqReport.RfqNumber
                                                                select rfqReport
                                                        );

                    DataSourceResult result = rfq.ToDataSourceResult(request);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                else
                {

                    IQueryable<RfqReportByDateSubmitted> rfq = (from rfqReport in _rfqDbContext.RfqReportByDateSubmitted
                                                                where (rfqReport.DateSubmitted >= startDateSubmitted && rfqReport.DateSubmitted <= endDateSubmitted)
                                                                orderby rfqReport.RfqNumber
                                                                select rfqReport
                                                        );

                    DataSourceResult result = rfq.ToDataSourceResult(request);
                    return Json(result, JsonRequestBehavior.AllowGet);

                }

            }
        }

        public ActionResult RfqTransDetailsReportByOwner_Read([DataSourceRequest]DataSourceRequest request, string strStat, DateTime? startDateSubmitted, DateTime? endDateSubmitted)
        {
            var username = Common.GetWebCurrentUser(RfQWebApp.Classes.Common.WebUserInformation.Username);

            using (_rfqDbContext)
            {
                if (strStat == "")
                {
                    IQueryable<RfqReportByDateSubmitted> rfq = (from rfqReport in _rfqDbContext.RfqReportByDateSubmitted
                                                            where (rfqReport.DateSubmitted >= startDateSubmitted && rfqReport.DateSubmitted <= endDateSubmitted) && rfqReport.SubmittedBy == username
                                                            orderby rfqReport.RfqNumber
                                                            select rfqReport
                                                        );

                    DataSourceResult result = rfq.ToDataSourceResult(request);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                else 
                {
                    IQueryable<RfqReportByDateSubmitted> rfq = (from rfqReport in _rfqDbContext.RfqReportByDateSubmitted
                                                                where rfqReport.ItemStatusCodeDesc == strStat && (rfqReport.DateSubmitted >= startDateSubmitted && rfqReport.DateSubmitted <= endDateSubmitted) && rfqReport.SubmittedBy == username
                                                                orderby rfqReport.RfqNumber
                                                                select rfqReport
                                                        );

                    DataSourceResult result = rfq.ToDataSourceResult(request);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public ActionResult RfqBuyersLookup_Read([DataSourceRequest]DataSourceRequest request)
        {
            using (_rfqDbContext)
            {
                IQueryable<BuyersLookup> rfq = (from rfqBuyers in _rfqDbContext.BuyersLookup                                                
                                                select rfqBuyers
                                                );               
                DataSourceResult result = rfq.ToDataSourceResult(request);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult RfqBuyersLookup_Create([DataSourceRequest]DataSourceRequest request, BuyersLookup rfq)
        {
            if (ModelState.IsValid)
            {
                using (_rfqDbContext)
                {
                    // Create a new from Model class
                    var entity = new BuyersLookup
                    {
                        Guid = System.Guid.NewGuid().ToString().ToUpper(),                                              
                        Category = rfq.Category,
                        AssignedBuyer = rfq.AssignedBuyer,
                        Remarks = rfq.Remarks,
                        Comments = rfq.Comments,
                        CreatedBy = RfQWebApp.Classes.Common.GetWebCurrentUser(RfQWebApp.Classes.Common.WebUserInformation.Username),
                        DateCreated = DateTime.Now
                    };
                    // Add the entity
                    _rfqDbContext.BuyersLookup.Add(entity);
                    // Insert the entity in the database
                    _rfqDbContext.SaveChanges();
                    // Get the ProductID generated by the database
                    rfq.Guid = entity.Guid;
                    rfq.DateCreated = entity.DateCreated;
                    rfq.CreatedBy = entity.CreatedBy;
                }
            }
            // Return the inserted product. The grid needs the generated ProductID. Also return any validation errors.
            return Json(new[] { rfq }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RfqBuyersLookup_Update([DataSourceRequest]DataSourceRequest request, BuyersLookup rfq)
        {

            if (rfq != null && ModelState.IsValid)
            {
                using (_rfqDbContext)
                {
                    // Create a new from Model class
                    var entity = new BuyersLookup
                    {
                        Guid = rfq.Guid,                        
                        Category = rfq.Category,
                        AssignedBuyer = rfq.AssignedBuyer,
                        Remarks = rfq.Remarks,
                        Comments = rfq.Comments,
                        CreatedBy = rfq.CreatedBy,
                        DateCreated = rfq.DateCreated,                        
                        ModifiedBy = RfQWebApp.Classes.Common.GetWebCurrentUser(RfQWebApp.Classes.Common.WebUserInformation.Username),
                        DateModified = DateTime.Now
                        
                    };

                    // Attach the entity
                    _rfqDbContext.BuyersLookup.Attach(entity);
                    // Change its state to Modified so Entity Framework can update the existing product instead of creating a new one
                    _rfqDbContext.Entry(entity).State = EntityState.Modified;

                    // Or use ObjectStateManager if using a previous version of Entity Framework
                    // northwind.ObjectStateManager.ChangeObjectState(entity, EntityState.Modified);

                    // Update the entity in the database
                    _rfqDbContext.SaveChanges();

                    // Get the ProductID generated by the database
                    rfq.ModifiedBy = entity.ModifiedBy;
                    rfq.DateModified = entity.DateModified;

                }
            }
            // Return the updated product. Also return any validation errors.
            return Json(new[] { rfq }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RfqBuyersLookup_Destroy([DataSourceRequest]DataSourceRequest request, BuyersLookup rfq)
        {
            if (ModelState.IsValid)
            {
                using (_rfqDbContext)
                {
                    // Create a new from Model class
                    var entity = new BuyersLookup
                    {
                        Guid = rfq.Guid,
                        Category = rfq.Category,
                        AssignedBuyer = rfq.AssignedBuyer,
                        Remarks = rfq.Remarks,
                        Comments = rfq.Comments
                    };
                    // Attach the entity
                    _rfqDbContext.BuyersLookup.Attach(entity);
                    // Delete the entity
                    _rfqDbContext.BuyersLookup.Remove(entity);
                    // Or use DeleteObject if using a previous versoin of Entity Framework
                    // northwind.Products.DeleteObject(entity);
                    // Delete the entity in the database
                    _rfqDbContext.SaveChanges();
                }
            }
            // Return the removed product. Also return any validation errors.
            return Json(new[] { rfq }.ToDataSourceResult(request, ModelState));
        }



    }
}