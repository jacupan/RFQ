using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RFQWebApp.Models;
using RfQWebApp.Classes;



namespace RfQWebApp.Controllers
{    
    public class HomeController : Controller
    {
        public RfQDBContext _rfqDbContext = new RfQDBContext();              
        
        public ActionResult Index()
        {           
            ViewBag.Message = "Welcome to ASP.NET MVC!";          

            return View();
        }

        public ActionResult MaintenanceTool()
        {
            var currUsr = Common.GetWebCurrentUser(RfQWebApp.Classes.Common.WebUserInformation.Username);

            var userInfo = DataRetrieval.UserProfile(currUsr);

            string userDepartment = userInfo[0].Department.ToString();

            ViewData["userDepartment"] = userDepartment;

            //ViewData["userDepartment"] = "Planning (521)";

            //ViewData["userDepartment"] = "Purchasing (550)";   

            ViewData["buyers"] = PopulateBuyer();

            return View();
        }

        public ActionResult GoToIndex()
        {
            return Json(Url.Action("Index", "Home"));
        }

        public ActionResult GoToSubmitRfq()
        {
            return Json(Url.Action("SubmitRfQ", "Home"));
        }

        public ActionResult GoToUpdateRfq()
        {
            return Json(Url.Action("UpdateRfQ", "Home"));
        }

        public ActionResult BuyerAccess(string userName)
        {
            var user = (from a in _rfqDbContext.BuyersAccess
                        where a.Username == userName
                        select new
                        {
                            Username = a.Username,
                            UserAccess = a.UserAccess

                        }).ToList();

            if (user.Count == 0)
            {
                return Json(Url.Action("AccessDenied", "Home"));
            }
            else
            {
                return Json(user, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult ReportAccess(string department)
        {
            if (department == "Purchasing (550)" || department == "Information Systems (582)")
            {
                return Json("success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(Url.Action("AccessDenied", "Home"));
            }
        }

        public ActionResult AccessDenied()
        {
            return View();
        }

        private List<SelectListItem> PopulateRepeatOrder()
        {
            using (_rfqDbContext)
            {
                var query = (from r in _rfqDbContext.RepeatOrderCode
                             orderby r.IsRepeatOrderCode
                             select new
                             {
                                 IsRepeatOrderCode = r.IsRepeatOrderCode,
                                 IsRepeatOrderCodeDesc = r.IsRepeatOrderCodeDesc
                             }).AsEnumerable();                

                List<SelectListItem> results = new List<SelectListItem>();                

                foreach (var item in query)
                {
                    SelectListItem i = new SelectListItem();

                    i.Text = item.IsRepeatOrderCodeDesc;
                    i.Value = item.IsRepeatOrderCode;
                    

                    results.Add(i);                    
                }

                return results;
            }
        }

        private List<SelectListItem> PopulateItemStatus()
        {
            using (_rfqDbContext)
            {
                var query = (from r in _rfqDbContext.ItemStatusCodeDdl
                             where r.ItemStatusCode.Trim() != "3"
                             orderby r.ItemStatusCode
                             select new
                             {
                                 ItemStatusCode = r.ItemStatusCode,
                                 ItemStatusCodeDesc = r.ItemStatusCodeDesc
                             }).AsEnumerable();

                List<SelectListItem> results = new List<SelectListItem>();

                foreach (var item in query)
                {
                    SelectListItem i = new SelectListItem();

                    i.Text = item.ItemStatusCodeDesc;
                    i.Value = item.ItemStatusCode;


                    results.Add(i);
                }

                return results;
            }
        }

        private List<SelectListItem> PopulateUom()
        {
            using (var rfqDbContext_ = new RfQDBContext())
            {
                var query = (from r in rfqDbContext_.UomCode
                             orderby r.UoM
                             select new
                             {
                                 UoM = r.UoM,
                                 UomCodeWithDesc = r.UomCodeWithDesc
                             }).AsEnumerable();

                List<SelectListItem> results = new List<SelectListItem>();

                foreach (var item in query)
                {
                    SelectListItem i = new SelectListItem();

                    i.Text = item.UomCodeWithDesc;
                    i.Value = item.UoM;


                    results.Add(i);
                }

                return results;
            }
        }

        private List<SelectListItem> PopulateBuyer()
        {
            using (var rfqDbContext_ = new RfQDBContext())
            {
                var query = (from r in rfqDbContext_.BuyersAccess
                             where !(new[] { "JACUPAN", "NPANGILINAN", "OMALASARTE" }.Contains(r.Username))
                             orderby r.Username
                             select new
                             {
                                 Username = r.Username,
                                 AssignedBuyer = r.FullName
                             }).AsEnumerable();

                List<SelectListItem> results = new List<SelectListItem>();

                foreach (var item in query)
                {
                    SelectListItem i = new SelectListItem();

                    i.Text = item.AssignedBuyer;
                    i.Value = item.AssignedBuyer;


                    results.Add(i);
                }

                return results;
            }
        }




        public ActionResult SubmitRfQ()
        {          
            var currUsr = Common.GetWebCurrentUser(RfQWebApp.Classes.Common.WebUserInformation.Username);

            var userInfo = DataRetrieval.UserProfile(currUsr);

            //var managerInfo = DataRetrieval.Approvers(currUsr);

            string userFullName = userInfo[0].FullName.ToString();

            string userDepartment = userInfo[0].Department.ToString();

            string userEmail = userInfo[0].Email.ToString();

            DirectorySearcher objDirSearch = new DirectorySearcher();

            DirectoryEntry dentUser = null;

            objDirSearch.Filter = "(&(objectCategory=user)(SAMAccountName=" + currUsr + "))";

            SearchResult user = objDirSearch.FindOne();

            dentUser = new DirectoryEntry(user.Path);

            string strManager = dentUser.Properties["manager"].Value.ToString();

            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);
            UserPrincipal mngr = UserPrincipal.FindByIdentity(ctx, IdentityType.DistinguishedName, strManager);

            string managerEmail = mngr.EmailAddress;

            //Debug.WriteLine(managerEmail);

            //var manager = user.Properties["manager"];

            //objDirSearch.Filter = "(&(objectCategory=user)(DistinguishedName=" + strManager + "))";

            //Debug.WriteLine(objDirSearch.Filter);

            //SearchResult manager = objDirSearch.FindOne();

            //Debug.WriteLine(manager.Properties["mail"]);

            


            ViewData["userFullName"] = userFullName;
            
            ViewData["userDepartment"] = userDepartment;

            ViewData["userEmail"] = userEmail;

            ViewData["mngrEmail"] = managerEmail;

            ViewData["repeatOrder"] = PopulateRepeatOrder();

            ViewData["UomCode"] = PopulateUom();

            ViewData["transGuid"] = System.Guid.NewGuid().ToString().ToUpper();   

            return View();
        }

        public ActionResult AddRfQ(RfqTransactions newRfq)
        {
            if (ModelState.IsValid)
            {
                newRfq.DateCreated = DateTime.Now;

                if (newRfq.Status == "Submitted")
                {
                    newRfq.DateSubmitted = DateTime.Now;
                    newRfq.SubmittedBy = Common.GetWebCurrentUser(RfQWebApp.Classes.Common.WebUserInformation.Username);
                }

                _rfqDbContext.RfqTransactions.Attach(newRfq);
                _rfqDbContext.RfqTransactions.Add(newRfq);
                _rfqDbContext.SaveChanges();
            }

            var rfqNo = (from a in _rfqDbContext.RfqTransactions
                         where a.Guid == newRfq.Guid
                         select new
                         {
                             RfqNumber = a.RfqNumber
                         }).ToList();

            //SendEmailNotification(userEmail, managerEmail, rfqNumber, userFullname, dept);

            return Json(rfqNo, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateRfQ()
        {
            ViewData["repeatOrder"] = PopulateRepeatOrder();

             ViewData["UomCode"] = PopulateUom();

            var currUsr = Common.GetWebCurrentUser(RfQWebApp.Classes.Common.WebUserInformation.Username);

            var userInfo = DataRetrieval.UserProfile(currUsr);

            string userDepartment = userInfo[0].Department.ToString();

            string userEmail = userInfo[0].Email.ToString();

            DirectorySearcher objDirSearch = new DirectorySearcher();

            DirectoryEntry dentUser = null;

            objDirSearch.Filter = "(&(objectCategory=user)(SAMAccountName=" + currUsr + "))";

            SearchResult user = objDirSearch.FindOne();

            dentUser = new DirectoryEntry(user.Path);

            string strManager = dentUser.Properties["manager"].Value.ToString();

            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);
            UserPrincipal mngr = UserPrincipal.FindByIdentity(ctx, IdentityType.DistinguishedName, strManager);

            string managerEmail = mngr.EmailAddress;

            ViewData["userDepartment"] = userDepartment;

            ViewData["userEmail"] = userEmail;

            ViewData["mngrEmail"] = managerEmail;

            return View();
        }

        public ActionResult UpdateExistingRfQ(string rfqNumber, string rfqDepartment)
        {
            var rfqNo = Int64.Parse(rfqNumber);

            var currUsr = Common.GetWebCurrentUser(RfQWebApp.Classes.Common.WebUserInformation.Username);

            var rfqResult = (from a in _rfqDbContext.RfqTransactions
                             where a.RfqNumber == rfqNo 
                             //&& new[] { "Draft" }.Contains(a.Status) && a.Department == rfqDepartment
                             select new
                             {
                                 RfqNumber = a.RfqNumber,
                                 Username = a.Username,
                                 Requestor = a.Requestor,
                                 Department = a.Department,
                                 Category = a.Category,
                                 Buyer = a.Buyer,
                                 Type = a.Type,
                                 Status = a.Status
                             }).ToList();

            if (rfqResult.Count == 0)
            {
                return Json("");
            }

            else {               

                if (rfqResult[0].Status != "Draft")
                {
                    return Json("Submitted");
                }

                //if (rfqResult[0].Department != rfqDepartment)
                //{
                //    return Json("DifferentDept");
                //}

                if (rfqResult[0].Username != currUsr)
                {
                    if (rfqDepartment == "Information System (582)")
                    {
                        return Json(rfqResult, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("DifferentOwner");
                    }
                    
                }
                
                else                 
                {
                    return Json(rfqResult, JsonRequestBehavior.AllowGet);
                }
                
            }

            
        }

        public ActionResult UpdateExistingRfQStatus(RfqTransactions existingRfq)
        {            
            RfqTransactions updateRfqStat = _rfqDbContext.RfqTransactions.Single(a => a.RfqNumber == existingRfq.RfqNumber);

            updateRfqStat.Status = "Submitted";
            updateRfqStat.Type = existingRfq.Type;
            updateRfqStat.Category = existingRfq.Category;
            updateRfqStat.ModifiedBy = Common.GetWebCurrentUser(RfQWebApp.Classes.Common.WebUserInformation.Username);
            updateRfqStat.DateModified = DateTime.Now;
            updateRfqStat.SubmittedBy = Common.GetWebCurrentUser(RfQWebApp.Classes.Common.WebUserInformation.Username);
            updateRfqStat.DateSubmitted = DateTime.Now;

            using (_rfqDbContext)
            {
                _rfqDbContext.Entry(updateRfqStat).State = System.Data.EntityState.Modified;
                _rfqDbContext.SaveChanges();
            }

            return Json("success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateExistingRfQStatusDraft(RfqTransactions existingRfq)
        {
            RfqTransactions updateRfqStat = _rfqDbContext.RfqTransactions.Single(a => a.RfqNumber == existingRfq.RfqNumber);

            updateRfqStat.Status = "Draft";
            updateRfqStat.Type = existingRfq.Type;
            updateRfqStat.Category = existingRfq.Category;
            updateRfqStat.ModifiedBy = Common.GetWebCurrentUser(RfQWebApp.Classes.Common.WebUserInformation.Username);
            updateRfqStat.DateModified = DateTime.Now;

            using (_rfqDbContext)
            {
                _rfqDbContext.Entry(updateRfqStat).State = System.Data.EntityState.Modified;
                _rfqDbContext.SaveChanges();
            }

            return Json("success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewAckCanRejRfQ1(string rfqNumber)
        {
            var currUsr = Common.GetWebCurrentUser(RfQWebApp.Classes.Common.WebUserInformation.Username);

            var userInfo = DataRetrieval.UserProfile(currUsr);

            string userDepartment = userInfo[0].Department.ToString();

            ViewData["userDepartment"] = userDepartment;

            //ViewData["userDepartment"] = "Planning (521)";

            ViewBag.RfqNo = rfqNumber;

            return View();
        }

        public ActionResult ViewAckCanRejRfQ()
        {
            var currUsr = Common.GetWebCurrentUser(RfQWebApp.Classes.Common.WebUserInformation.Username);

            var userInfo = DataRetrieval.UserProfile(currUsr);

            string userDepartment = userInfo[0].Department.ToString();

            string userEmail = userInfo[0].Email.ToString();

            ViewData["userDepartment"] = userDepartment;

            ViewData["userEmail"] = userEmail;            

            //ViewData["userDepartment"] = "Planning (521)";            

            return View();
        }     

        public ActionResult ViewAckCanRejRfQByRfqNo(string rfqNumber, string rfqDepartment)
        {
            if (rfqNumber == null || rfqNumber == "")
            {
                rfqNumber = "0";
            }

            string currUsr = Common.GetWebCurrentUser(RfQWebApp.Classes.Common.WebUserInformation.Username);

            var rfqNo = Int64.Parse(rfqNumber);

            var rfqResult = (from a in _rfqDbContext.RfqTransactions
                             where a.RfqNumber == rfqNo //&& new[] { "Submitted", "Acknowledged", "Cancelled", "Opened"}.Contains(a.Status) 
                             //&& a.Department == rfqDepartment
                             select new
                             {
                                 RfqNumber = a.RfqNumber,
                                 Username = a.Username,
                                 Requestor = a.Requestor,
                                 Department = a.Department,
                                 Category = a.Category,
                                 //Buyer = a.Buyer,
                                 Buyer = a.ReAssignedBuyerTo == null ? a.Buyer : a.ReAssignedBuyerTo == "" ? a.Buyer : a.ReAssignedBuyerTo,
                                 Type = a.Type,
                                 Status = a.Status

                             }).ToList();
            
            if (rfqResult.Count == 0)
            {
                return Json("");
            }

            else
            {
                if (rfqResult[0].Status == "Draft" && rfqResult[0].RfqNumber == rfqNo)
                {
                    return Json("Draft");
                }

                //if (rfqResult[0].Department != rfqDepartment)
                //{
                //    if (rfqDepartment == "Information System (582)")
                //    {
                //        return Json(rfqResult, JsonRequestBehavior.AllowGet);
                //    }
                //    else
                //    {
                //        return Json("DifferentDept");
                //    }

                //}

                if (rfqResult[0].Username != currUsr)
                {
                    if (rfqDepartment == "Information System (582)")
                    {
                        return Json(rfqResult, JsonRequestBehavior.AllowGet);
                    }                    
                    else
                    {
                        return Json("DifferentOwner");
                    }

                }

                if (rfqResult[0].Status == "Completed")
                {
                    //return Json("Completed");

                    return Json(rfqResult, JsonRequestBehavior.AllowGet);
                }

                //if (rfqResult[0].UserName != currUsr)
                //{                    
                //    return Json(rfqResult, JsonRequestBehavior.AllowGet);
                //}

                else
                {
                    return Json(rfqResult, JsonRequestBehavior.AllowGet);
                }
                
            }
        }

        public ActionResult ViewRFQStatusViaMail(string rfqNumber)
        {
            var currUsr = Common.GetWebCurrentUser(RfQWebApp.Classes.Common.WebUserInformation.Username);

            var userInfo = DataRetrieval.UserProfile(currUsr);

            string userDepartment = userInfo[0].Department.ToString();

            ViewData["userDepartment"] = userDepartment;

            //ViewData["userDepartment"] = "Planning (521)";

            ViewBag.RfqNo = rfqNumber;

            return View();
        }

        public ActionResult ViewRFQStatusViaMailNot(string rfqNumber, string rfqDepartment)
        {
            if (rfqNumber == null || rfqNumber == "")
            {
                rfqNumber = "0";
            }            

            var rfqNo = Int64.Parse(rfqNumber);

            var rfqResult = (from a in _rfqDbContext.RfqTransactions
                             where a.RfqNumber == rfqNo 
                             select new
                             {
                                 RfqNumber = a.RfqNumber,
                                 Username = a.Username,
                                 Requestor = a.Requestor,
                                 Department = a.Department,
                                 Category = a.Category,
                                 Buyer = a.Buyer,
                                 Type = a.Type,
                                 Status = a.Status

                             }).ToList();

            if (rfqResult.Count == 0)
            {
                return Json("");
            }

            else
            {
                if (rfqResult[0].Status == "Draft" && rfqResult[0].RfqNumber == rfqNo)
                {
                    return Json("Draft");
                }

                if (rfqResult[0].Department != rfqDepartment)
                {
                    if (rfqDepartment == "Information System (582)")
                    {
                        return Json(rfqResult, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("DifferentDept");
                    }

                }                      

                else
                {
                    return Json(rfqResult, JsonRequestBehavior.AllowGet);
                }

            }
        }


        public ActionResult UpdateItemStatus(string rfqTransDetailsGuid, string rfqNumber)
        {
            RfqTransactionDetails updateRfqItemStat = _rfqDbContext.RfqTransactionDetails.Single(a => a.Guid == rfqTransDetailsGuid);

            if (updateRfqItemStat.ItemStatusCode.Trim() != "3")
            {
                updateRfqItemStat.ItemStatusCode = "3";
                updateRfqItemStat.CompletedBy = Common.GetWebCurrentUser(RfQWebApp.Classes.Common.WebUserInformation.Username);
                updateRfqItemStat.DateCompleted = DateTime.Now;

                using (_rfqDbContext)
                {
                    _rfqDbContext.Entry(updateRfqItemStat).State = System.Data.EntityState.Modified;
                    _rfqDbContext.SaveChanges();
                }

                return RedirectToAction("UpdateRfQStatusToOverAllStatus", new { rfqNumber = rfqNumber });
            }            


           
            return Json("success", JsonRequestBehavior.AllowGet);
        
        }

        public ActionResult ViewFeedback(string rfqTransDetailsGuid)
        {
            var fb = (from a in _rfqDbContext.RfqTransactionDetails
                      where a.Guid == rfqTransDetailsGuid
                      select new
                      {
                          Feedback = a.Feedback
                      }).ToList();

            //ViewData["feedback"] = fb[0].Feedback.ToString();

            return Json(fb, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateFeedback(string rfqTransDetailsGuid, string strFeedback)
        {
            RfqTransactionDetails updateRfqFeedback = _rfqDbContext.RfqTransactionDetails.Single(a => a.Guid == rfqTransDetailsGuid);

            updateRfqFeedback.Feedback = strFeedback;

            using (_rfqDbContext)
            {
                _rfqDbContext.Entry(updateRfqFeedback).State = System.Data.EntityState.Modified;
                _rfqDbContext.SaveChanges();
            }

            return Json("success", JsonRequestBehavior.AllowGet);
        }


        



        public ActionResult UpdateRfQStatus(string rfqNumber)
        {
            ViewData["itemStatus"] = PopulateItemStatus();            

            ViewBag.RfqNo = rfqNumber;

            return View();
        }

        public ActionResult UpdateRfQStatusByBuyer(string rfqNumber)
        {

            if (rfqNumber == null || rfqNumber == "")
            {
                rfqNumber = "0";
            }

            var rfqNo = Int64.Parse(rfqNumber);

            var rfqResult = (from a in _rfqDbContext.RfqTransactions
                             where a.RfqNumber == rfqNo
                             //&& new[] { "Submitted", "Acknowledged", "Cancelled", "Completed", "Opened" }.Contains(a.Status) 
                             select new
                             {
                                 RfqNumber = a.RfqNumber,
                                 Requestor = a.Requestor,
                                 Department = a.Department,
                                 Category = a.Category,
                                 Buyer = a.ReAssignedBuyerTo == null ? a.Buyer : a.ReAssignedBuyerTo == "" ? a.Buyer : a.ReAssignedBuyerTo,
                                 ReAssignedBuyerTo = a.ReAssignedBuyerTo,
                                 Type = a.Type,
                                 Status = a.Status
                             }).ToList();

            if (rfqResult.Count == 0)
            {
                return Json("");
            }

            else
            {
                if (rfqResult[0].Status == "Draft")
                {
                    return Json("Draft");
                }
               
                else
                {
                    return Json(rfqResult, JsonRequestBehavior.AllowGet);
                }
            }


        }

        public ActionResult UpdateReassignedBuyer(string rfqNumber, string reassignedBuyer)
        {
            var rfqNo = Int64.Parse(rfqNumber);

            RfqTransactions updateRfqReassignedBuyer = _rfqDbContext.RfqTransactions.Single(a => a.RfqNumber == rfqNo);
            
            updateRfqReassignedBuyer.ReAssignedBuyerTo = reassignedBuyer;
            updateRfqReassignedBuyer.ReAssignedBuyerBy = Common.GetWebCurrentUser(RfQWebApp.Classes.Common.WebUserInformation.Username);
            updateRfqReassignedBuyer.ReAssignedDateTo = DateTime.Now;

                using (_rfqDbContext)
                {
                    _rfqDbContext.Entry(updateRfqReassignedBuyer).State = System.Data.EntityState.Modified;                  
                    _rfqDbContext.SaveChanges();
                }            

            return Json("success", JsonRequestBehavior.AllowGet);

        }

        public ActionResult UpdateRfQStatusByBuyerToAck(string rfqNumber)
        {
            var rfqNo = Int64.Parse(rfqNumber);

            RfqTransactions updateRfqStat = _rfqDbContext.RfqTransactions.Single(a => a.RfqNumber == rfqNo);            

            if (updateRfqStat.Status == "Submitted")
            {
                updateRfqStat.Status = "Acknowledged";
                updateRfqStat.AcknowledgedBy = Common.GetWebCurrentUser(RfQWebApp.Classes.Common.WebUserInformation.Username);
                updateRfqStat.DateAcknowledged = DateTime.Now;
           
                using (_rfqDbContext)
                {
                    _rfqDbContext.Entry(updateRfqStat).State = System.Data.EntityState.Modified;                    

                    var rfqTransDetailsBuyerStat = _rfqDbContext.RfqTransactionDetails.Where(a => a.RfqNumber == rfqNumber).ToList();
                    rfqTransDetailsBuyerStat.ForEach(a => 
                            {
                                a.ItemStatusCode = "0";   
                                a.AcknowledgedBy = Common.GetWebCurrentUser(RfQWebApp.Classes.Common.WebUserInformation.Username);
                                a.DateAcknowledged = DateTime.Now;
                            });

                    _rfqDbContext.SaveChanges();
                }
            }           

            return Json("success", JsonRequestBehavior.AllowGet);

        }

        public ActionResult UpdateRfQStatusToOverAllStatus(string rfqNumber)
        {
            var rfqNo = Int64.Parse(rfqNumber);            

            RfqTransactions updateRfqStat = _rfqDbContext.RfqTransactions.Single(a => a.RfqNumber == rfqNo); 

            var stat = (from a in _rfqDbContext.RfqTransactionDetails
                        where a.RfqNumber == rfqNumber
                                 select new
                                 {
                                    ItemStatusCode = a.ItemStatusCode
                                 }).ToList();

         
               List<string> i = new List<string>();

               foreach (var item in stat)
               {                   
                   i.Add(item.ItemStatusCode.Trim());                   
               }

               string[] s = i.ToArray();

               bool cancelled = Array.TrueForAll(s, rej => rej.Equals("2"));
               bool completed = Array.TrueForAll(s, rej => rej.Equals("3"));

               if (cancelled == false && completed == false)
               { 
                    updateRfqStat.Status = "Opened";
               }
               if (cancelled)
               {
                   updateRfqStat.Status = "Cancelled";
                   updateRfqStat.CancelledBy = Common.GetWebCurrentUser(RfQWebApp.Classes.Common.WebUserInformation.Username);
                   updateRfqStat.DateCancelled = DateTime.Now;
               }
               if (completed)
               {
                   updateRfqStat.Status = "Completed";
                   updateRfqStat.CompletedBy = Common.GetWebCurrentUser(RfQWebApp.Classes.Common.WebUserInformation.Username);
                   updateRfqStat.DateCompleted = DateTime.Now;
               }

               _rfqDbContext.Entry(updateRfqStat).State = System.Data.EntityState.Modified;           

               _rfqDbContext.SaveChanges();

            return Json("success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateRfQStatusByBuyerToCan(string rfqNumber)
        {
            var rfqNo = Int64.Parse(rfqNumber);

            RfqTransactions updateRfqStat = _rfqDbContext.RfqTransactions.Single(a => a.RfqNumber == rfqNo);

            if (updateRfqStat.Status == "Acknowledged")
            {
                updateRfqStat.Status = "Canvassed";
                updateRfqStat.AcknowledgedBy = Common.GetWebCurrentUser(RfQWebApp.Classes.Common.WebUserInformation.Username);
                updateRfqStat.DateAcknowledged = DateTime.Now;

                using (_rfqDbContext)
                {
                    _rfqDbContext.Entry(updateRfqStat).State = System.Data.EntityState.Modified;

                    //var rfqTransDetailsBuyerStat = _rfqDbContext.RfqTransactionDetails.Where(a => a.RfqNumber == rfqNumber).ToList();
                    //rfqTransDetailsBuyerStat.ForEach(a =>
                    //{
                    //    a.BuyerStatusCode = "0";
                    //    a.AcknowledgedBy = Common.GetWebCurrentUser(RfQWebApp.Classes.Common.WebUserInformation.Username);
                    //    a.DateAcknowledged = DateTime.Now;
                    //});

                    _rfqDbContext.SaveChanges();
                }
            }

            return Json("success", JsonRequestBehavior.AllowGet);

        }

        public ActionResult UploadFile(HttpPostedFileBase file, string transDetailsGuid, string transRfqNumber)
        {

            RfqAttachments rfqAttachment = new RfqAttachments();

            var filename = file.FileName;
            int index = filename.LastIndexOf('\\');
            filename = filename.Substring(index + 1);


            rfqAttachment.FileID = System.Guid.NewGuid().ToString().ToUpper();
            rfqAttachment.TransDetailsGuid = transDetailsGuid;
            rfqAttachment.RfqNumber = transRfqNumber;
            rfqAttachment.FileExtension = file.ContentType;
            rfqAttachment.FileName = filename;
            rfqAttachment.FileBytes = ConvertToBytes(file);

            using (_rfqDbContext)
            {

                _rfqDbContext.RfqAttachments.Add(rfqAttachment);
                _rfqDbContext.SaveChanges();

            }            

            return RedirectToAction("UpdateRfQStatus", new { rfqNumber = transRfqNumber });            
        }

        public ActionResult LoadFile(string fileId)
        {

            var displayfile = (from f in _rfqDbContext.RfqAttachments
                               where f.FileID == fileId
                               select f.FileBytes).FirstOrDefault();

            var extfile = (from f in _rfqDbContext.RfqAttachments
                           where f.FileID == fileId
                           select f.FileExtension).FirstOrDefault();

            var filename = (from f in _rfqDbContext.RfqAttachments
                            where f.FileID == fileId
                            select f.FileName).FirstOrDefault();

            MemoryStream ms = new MemoryStream();
            ms.Write(displayfile, 0, displayfile.Length);
            ms.Position = 0;


            return File(ms, extfile, filename);

        }

        public ActionResult ViewFile(string fileId)
        {

            var file = (from f in _rfqDbContext.RfqAttachments
                        where f.FileID == fileId
                        select f.FileBytes).FirstOrDefault();

            var extfile = (from f in _rfqDbContext.RfqAttachments
                           where f.FileID == fileId
                           select f.FileExtension).FirstOrDefault();

            if (file != null)
            {

                var strm = new MemoryStream(file.ToArray());

                return File(strm, extfile);
            }
            else
            {

                return View(file);

            }


        }

        public byte[] ConvertToBytes(HttpPostedFileBase file)
        {
            byte[] fileBytes = null;
            BinaryReader rd = new BinaryReader(file.InputStream);
            fileBytes = rd.ReadBytes((int)file.ContentLength);
            return fileBytes;
        }

        [HttpPost]
        public ActionResult UploadUserFile(HttpPostedFileBase file, string transDetailsGuid, string transRfqNumber)
        {

            RfqUserAttachments rfqUserAttachment = new RfqUserAttachments();

            var filename = file.FileName;
            int index = filename.LastIndexOf('\\');
            filename = filename.Substring(index + 1);


            rfqUserAttachment.FileID = System.Guid.NewGuid().ToString().ToUpper();
            rfqUserAttachment.TransDetailsGuid = transDetailsGuid;
            rfqUserAttachment.RfqNumber = transRfqNumber;
            rfqUserAttachment.FileExtension = file.ContentType;
            rfqUserAttachment.FileName = filename;
            rfqUserAttachment.FileBytes = ConvertToBytes(file);

            using (_rfqDbContext)
            {

                _rfqDbContext.RfqUserAttachments.Add(rfqUserAttachment);
                _rfqDbContext.SaveChanges();

            }

            //return Json("fileUploaded", JsonRequestBehavior.AllowGet);

            //return Content("{\"name\":\"" + filename + "\"}", "application/json");

            return Content(Boolean.TrueString);
        }

        public ActionResult LoadUserFile(string fileId)
        {

            var displayfile = (from f in _rfqDbContext.RfqUserAttachments
                               where f.FileID == fileId
                               select f.FileBytes).FirstOrDefault();

            var extfile = (from f in _rfqDbContext.RfqUserAttachments
                           where f.FileID == fileId
                           select f.FileExtension).FirstOrDefault();

            var filename = (from f in _rfqDbContext.RfqUserAttachments
                            where f.FileID == fileId
                            select f.FileName).FirstOrDefault();

            MemoryStream ms = new MemoryStream();
            ms.Write(displayfile, 0, displayfile.Length);
            ms.Position = 0;


            return File(ms, extfile, filename);

        }


        public ActionResult SendEmailNotification(string userEmail, string managerEmail, string rfqNumber, string userFullname, string dept, string rfqBuyer)
        {
            var buyerEmail = (from a in _rfqDbContext.BuyersAccess
                              where a.FullName == rfqBuyer
                              select new
                              {
                                  Email = a.Email
                              }).ToList();


            var errorMsg = "";
            string CopyRecipient = "";
            string DLINKA = "";
            string DLINKB = "";
            string Subject = "";
            string buyEmail = buyerEmail[0].Email.ToString();

                
            CopyRecipient =  userEmail + ";" + managerEmail;
            Subject = "Request for Quotation - RFQ # "+ rfqNumber +"";
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

            //Debug.WriteLine(rfqNumber);
            //Debug.WriteLine(userFullname);
            //Debug.WriteLine(dept);
            //Debug.WriteLine(Subject);
            //Debug.WriteLine(DLINKA);
            //Debug.WriteLine(DLINKB);
            //Debug.WriteLine(userEmail);            

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
                        cmd1.CommandText = "AutoEmail_Buyer";
                        cmd1.Parameters.Add("@RfQNo", SqlDbType.NVarChar).Value = rfqNumber;
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
                        cmd2.CommandText = "AutoEmail_User";
                        cmd2.Parameters.Add("@RfQNo", SqlDbType.NVarChar).Value = rfqNumber;
                        cmd2.Parameters.Add("@REQUESTOR", SqlDbType.NVarChar).Value = userFullname;
                        cmd2.Parameters.Add("@DEPARTMENT", SqlDbType.NVarChar).Value = dept;
                        cmd2.Parameters.Add("@SUBJ", SqlDbType.NVarChar).Value = Subject;
                        cmd2.Parameters.Add("@DLINKB", SqlDbType.NVarChar).Value = DLINKB;
                        cmd2.Parameters.Add("@RECIP", SqlDbType.NVarChar).Value = userEmail;
                        cmd2.Parameters.Add("@COPYRECIP", SqlDbType.NVarChar).Value = managerEmail;

                        cmd2.ExecuteNonQuery();
                    }
                }
            }


            catch (SqlException ex)
            {

                errorMsg = ("SQL Error" + ex.Message.ToString());

            }

            //string connStr = ConfigurationManager.ConnectionStrings["RfQContext"].ConnectionString;
            
            //SqlConnection conn = new SqlConnection(connStr);
            //SqlTransaction transaction;

            //conn.Open();
            //transaction = conn.BeginTransaction();

            //try
            //{

            //    // Command Objects for the transaction
            //    SqlCommand cmd1 = new SqlCommand("AutoEmail_Buyer", conn, transaction);
            //    SqlCommand cmd2 = new SqlCommand("AutoEmail_User", conn, transaction);

            //    cmd1.CommandType = CommandType.StoredProcedure;
            //    cmd2.CommandType = CommandType.StoredProcedure;

            //    Debug.WriteLine(rfqNumber);
            //    Debug.WriteLine(userFullname);
            //    Debug.WriteLine(dept);
            //    Debug.WriteLine(Subject);
            //    Debug.WriteLine(DLINKA);
            //    Debug.WriteLine(DLINKB);
            //    Debug.WriteLine(userEmail);
            //    //for buyer
                
            //    cmd1.Parameters.Add(new SqlParameter("@RfQNo", SqlDbType.NVarChar, 50));
            //    cmd1.Parameters["@RfQNo"].Value = rfqNumber;
               
            //    cmd1.Parameters.Add(new SqlParameter("@REQUESTOR", SqlDbType.NVarChar, 100));
            //    cmd1.Parameters["@REQUESTOR"].Value = userFullname;

            //    cmd1.Parameters.Add(new SqlParameter("@DEPARTMENT", SqlDbType.NVarChar, 100));
            //    cmd1.Parameters["@DEPARTMENT"].Value = dept;

            //    cmd1.Parameters.Add(new SqlParameter("@SUBJ", SqlDbType.NVarChar, -1));
            //    cmd1.Parameters["@SUBJ"].Value = Subject;

            //    cmd1.Parameters.Add(new SqlParameter("@DLINKA", SqlDbType.NVarChar, -1));
            //    cmd1.Parameters["@DLINKA"].Value = DLINKA;

            //    cmd1.Parameters.Add(new SqlParameter("@RECIP", SqlDbType.NVarChar, 50));
            //    //cmd1.Parameters["@RECIP"].Value = buyEmail;
            //    cmd1.Parameters["@RECIP"].Value = userEmail;

            //    //for user and manager
            //    cmd2.Parameters.Add(new SqlParameter("@RfQNo", SqlDbType.NVarChar, 50));
            //    cmd2.Parameters["@RfQNo"].Value = rfqNumber;

            //    cmd2.Parameters.Add(new SqlParameter("@REQUESTOR", SqlDbType.NVarChar, 100));
            //    cmd2.Parameters["@REQUESTOR"].Value = userFullname;

            //    cmd2.Parameters.Add(new SqlParameter("@DEPARTMENT", SqlDbType.NVarChar, 100));
            //    cmd2.Parameters["@DEPARTMENT"].Value = dept;

            //    cmd2.Parameters.Add(new SqlParameter("@SUBJ", SqlDbType.NVarChar, -1));
            //    cmd2.Parameters["@SUBJ"].Value = Subject;

            //    cmd2.Parameters.Add(new SqlParameter("@DLINKB", SqlDbType.NVarChar, -1));
            //    cmd2.Parameters["@DLINKB"].Value = DLINKB;              

            //    cmd2.Parameters.Add(new SqlParameter("@RECIP", SqlDbType.NVarChar, 50));                
            //    cmd2.Parameters["@RECIP"].Value = userEmail;

            //    cmd2.Parameters.Add(new SqlParameter("@COPYRECIP", SqlDbType.NVarChar, 50));
            //    //cmd2.Parameters["@COPYRECIP"].Value = managerEmail;
            //    cmd2.Parameters["@COPYRECIP"].Value = userEmail;

            //    cmd1.ExecuteNonQuery();
            //    cmd2.ExecuteNonQuery();

            //    transaction.Commit();
            //}

            //catch (SqlException sqlEx)
            //{
            //    transaction.Rollback();
            //}

            //finally
            //{
            //    conn.Close();
            //    conn.Dispose();
            //}



            //return errorMsg;
            return Json("success", JsonRequestBehavior.AllowGet);

        
        }

        public ActionResult SendEmailNotificationFeedback(string userEmail, string rfqNumber, string rfqTransGuid, string userFullname, string dept, string rfqBuyer)
        {
            var buyerEmail = (from a in _rfqDbContext.BuyersAccess
                              where a.FullName == rfqBuyer
                              select new
                              {
                                  Email = a.Email
                              }).ToList();


            var errorMsg = "";            
            string DLINKA = "";
            string DLINKB = "";
            string Subject = "";
            string buyEmail = buyerEmail[0].Email.ToString();
            
            Subject = "RFQ User Feedback - RFQ # " + rfqNumber + "";
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
                        cmd1.CommandText = "AutoEmail_UserFeedback_BuyerView";
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
                        cmd2.CommandText = "AutoEmail_UserFeedback_UserView";
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

        public ActionResult SendEmailNotificationComments(string rfqNumber, string rfqTransGuid, string userFullname, string userEmail, string dept, string rfqBuyer)
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
            Subject = "RFQ Buyer Comment - RFQ # " + rfqNumber + "";
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
                        cmd1.CommandText = "AutoEmail_BuyerComment_BuyerView";
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
                        cmd2.CommandText = "AutoEmail_BuyerComment_UserView";
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

        public ActionResult SendEmailNotificationReassignedBuyer(string rfqNumber, string userFullname, string userEmail, string dept, string rfqFromBuyer, string rfqToBuyer)
        {
            var fromBuyerEmail = (from a in _rfqDbContext.BuyersAccess
                                  where a.FullName == rfqFromBuyer
                              select new
                              {
                                  Email = a.Email
                              }).ToList();

            var ToBuyerEmail = (from a in _rfqDbContext.BuyersAccess
                                where a.FullName == rfqToBuyer
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
            string fromBuyer = fromBuyerEmail[0].Email.ToString();
            string toBuyer = ToBuyerEmail[0].Email.ToString();
            
            userEmail = requestor[0].Username + "@allegromicro.com";            
            Subject = "RFQ Reassigned Buyer - RFQ # " + rfqNumber + "";
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
                        cmd1.CommandText = "AutoEmail_ReassignedBuyer_BuyerView";
                        cmd1.Parameters.Add("@RfQNo", SqlDbType.NVarChar).Value = rfqNumber;                        
                        cmd1.Parameters.Add("@REQUESTOR", SqlDbType.NVarChar).Value = userFullname;
                        cmd1.Parameters.Add("@FROMBUYER", SqlDbType.NVarChar).Value = rfqFromBuyer;
                        cmd1.Parameters.Add("@TOBUYER", SqlDbType.NVarChar).Value = rfqToBuyer;
                        cmd1.Parameters.Add("@DEPARTMENT", SqlDbType.NVarChar).Value = dept;
                        cmd1.Parameters.Add("@SUBJ", SqlDbType.NVarChar).Value = Subject;
                        cmd1.Parameters.Add("@DLINKA", SqlDbType.NVarChar).Value = DLINKA;
                        cmd1.Parameters.Add("@RECIP", SqlDbType.NVarChar).Value = toBuyer;
                        cmd1.Parameters.Add("@COPYRECIP", SqlDbType.NVarChar).Value = fromBuyer;
                        //cmd1.Parameters.Add("@RECIP", SqlDbType.NVarChar).Value = userEmail;
                        //cmd1.Parameters.Add("@COPYRECIP", SqlDbType.NVarChar).Value = userEmail;


                        cmd1.ExecuteNonQuery();
                    }

                    using (SqlCommand cmd2 = new SqlCommand())
                    {
                        cmd2.Connection = conn;
                        cmd2.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd2.CommandText = "AutoEmail_ReassignedBuyer_UserView";
                        cmd2.Parameters.Add("@RfQNo", SqlDbType.NVarChar).Value = rfqNumber;                        
                        cmd2.Parameters.Add("@REQUESTOR", SqlDbType.NVarChar).Value = userFullname;
                        cmd2.Parameters.Add("@FROMBUYER", SqlDbType.NVarChar).Value = rfqFromBuyer;
                        cmd2.Parameters.Add("@TOBUYER", SqlDbType.NVarChar).Value = rfqToBuyer;
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


        public ActionResult ReportsRfQ()
        {
            var currUsr = Common.GetWebCurrentUser(RfQWebApp.Classes.Common.WebUserInformation.Username);

            var userInfo = DataRetrieval.UserProfile(currUsr);

            string userDepartment = userInfo[0].Department.ToString();

            ViewData["userDepartment"] = userDepartment;

            //ViewData["userDepartment"] = "Planning (521)";

            //ViewData["userDepartment"] = "Purchasing (550)";  

            return View();
        }


        public ActionResult ChartRfQ()
        {
            var currUsr = Common.GetWebCurrentUser(RfQWebApp.Classes.Common.WebUserInformation.Username);

            var userInfo = DataRetrieval.UserProfile(currUsr);

            string userDepartment = userInfo[0].Department.ToString();

            ViewData["userDepartment"] = userDepartment;

            //ViewData["userDepartment"] = "Planning (521)";

            //ViewData["userDepartment"] = "Purchasing (550)";  

            return View();
        }

        public ActionResult ReportsRfQByOwner()
        {
            var currUsr = Common.GetWebCurrentUser(RfQWebApp.Classes.Common.WebUserInformation.Username);

            var userInfo = DataRetrieval.UserProfile(currUsr);

            string userDepartment = userInfo[0].Department.ToString();

            ViewData["userDepartment"] = userDepartment;

            return View();
        }


        public JsonResult CategoryList()
        {
            return Json(_rfqDbContext.CategoryList.OrderBy(a => a.Category), JsonRequestBehavior.AllowGet);
        }

        public JsonResult TypeList()
        {
            return Json(_rfqDbContext.Type.OrderBy(a => a.Type), JsonRequestBehavior.AllowGet);
        }

        public JsonResult BuyerList()
        {
            
            return Json(_rfqDbContext.BuyersAccess.Where(a => a.Username != "JACUPAN" && a.Username != "NPANGILINAN" && a.Username != "OMALASARTE" ).OrderBy(b => b.FullName), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ReAssignedBuyer(string fromBuyer)
        {

            return Json(_rfqDbContext.BuyersAccess.Where(a => a.Username != "JACUPAN" && a.Username != "NPANGILINAN" && a.Username != "OMALASARTE" && a.FullName != fromBuyer).OrderBy(b => b.FullName), JsonRequestBehavior.AllowGet);
        }


        public JsonResult Statuses()
        {

            return Json(_rfqDbContext.ItemStatusCodeDdl.OrderBy(a => a.ItemStatusCodeDesc), JsonRequestBehavior.AllowGet);
        } 

        public JsonResult GetBuyerName(string category)
        {
            var buyerName = (from a in _rfqDbContext.BuyersLookup
                             where a.Category == category
                             select new
                             {
                                 AssignedBuyer = a.AssignedBuyer
                             }).GroupBy(b => b.AssignedBuyer).Select(c => c.FirstOrDefault()).ToList();
          
            return Json(buyerName, JsonRequestBehavior.AllowGet);
        }

       
    }
}
