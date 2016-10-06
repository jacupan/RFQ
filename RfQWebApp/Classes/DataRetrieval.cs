using System;
using System.Collections.Generic;
using System.Linq;
using AMPI;

namespace RfQWebApp.Classes
{
    public static class DataRetrieval
    {

        //public static List<Approvers> ApproverList(string id)
        //{
        //    GatePassDBContext GPContext = new GatePassDBContext();
        //    var results = (from a in GPContext.Approvers
        //                   select a);

        //    if (id != null && id != "")
        //    {
        //        results = (from a in GPContext.Approvers
        //                   where a.GPID == id
        //                   select a);
        //    }
        //    if (id == null)
        //    {
        //        return null;
        //    }

        //    List<Approvers> GPApproverList = new List<Approvers>();


        //    foreach (var item in results)
        //    {

        //       Approvers GPApprover = new Approvers();
               
        //       GPApprover.GPID = item.GPID;
        //       GPApprover.SectionHeadName = item.SectionHeadName;
 
        //       GPApprover.SectionHeadEmail = item.SectionHeadEmail;
        //       GPApprover.ApproveBySectionHead = item.ApproveBySectionHead;
        //       GPApprover.MisManager = item.MisManager;
        //       GPApprover.MisManagerEmail = item.MisManagerEmail;
        //       GPApprover.ApproveByMISManager = item.ApproveByMISManager;
        //       GPApprover.ProductionManager = item.ProductionManager;
        //       GPApprover.ProductionManagerEmail = item.ProductionManagerEmail;
        //       GPApprover.ApproveByDivisionManager = item.ApproveByDivisionManager;
        //       GPApprover.AccountingDepartment = item.AccountingDepartment;
        //       GPApprover.AccountingDepartmentMail = item.AccountingDepartmentMail;
        //       GPApprover.ApproveByAccountingDepartment = item.ApproveByAccountingDepartment;
        //       GPApprover.GuardName = item.GuardName;
        //       GPApprover.GuardEmail = item.GuardEmail;
        //       GPApprover.ApproveBySecurityDepartment = item.ApproveBySecurityDepartment;
        //       GPApproverList.Add(GPApprover);
        //    }

        //    return GPApproverList;
        //}


        //public static List<DefaultApprover> DefaultApprover() {

        //    GatePassDBContext GPContext = new GatePassDBContext();
           
        //    var results = (from a in GPContext.DefaultApprovers
        //                   select a);


        //    List<DefaultApprover> DefaultApproverList = new List<DefaultApprover>();


        //    foreach (var item in results)
        //    {

        //        DefaultApprover DefaultApprover = new DefaultApprover();

        //        DefaultApprover.ApproverID = item.ApproverID;
        //        DefaultApprover.AccountingDepartment = item.AccountingDepartment;
        //        DefaultApprover.AccountingDepartmentMail = item.AccountingDepartmentMail;
        //        DefaultApprover.ITDepartment = item.ITDepartment;
        //        DefaultApprover.ITDepartmentMail = item.ITDepartmentMail;
        //        DefaultApprover.ProductionDepartment = item.ProductionDepartment;
        //        DefaultApprover.ProductionEmail = item.ProductionEmail;
        //        DefaultApprover.SecurityDepartment = item.SecurityDepartment;
        //        DefaultApprover.SecurityEmail = item.SecurityEmail;

        //        DefaultApproverList.Add(DefaultApprover);
        //    }

        //    return DefaultApproverList;
        
        //}

        //public static List<GatePassTypeSubSections> GatePassTypeSubSectionList(string txntype) 
        //{
        //    GatePassDBContext gpContext = new GatePassDBContext();

        //    var subsectionid = (from gptss in gpContext.GatePassTypeSubSections
        //                        select gptss);

        //    if (txntype != null && txntype != "")
        //    {
        //        subsectionid = (from gptss in gpContext.GatePassTypeSubSections
        //                        where gptss.GPTxnType == txntype
                               
        //                        select gptss);
        //    }
        //    else 
        //    {
        //        return null;
        //    }


        //    List<GatePassTypeSubSections> GPSubSectionList = new List<GatePassTypeSubSections>();

        //    foreach (var items in subsectionid) 
        //    {

        //        GatePassTypeSubSections Subsection = new GatePassTypeSubSections();

        //        Subsection.GPTypeSubSectionsID = items.GPTypeSubSectionsID;
        //        Subsection.GPTypeName = items.GPTypeName;

        //        GPSubSectionList.Add(Subsection);
        //    }

        //    return GPSubSectionList;
        //}


        //public static List<GatePassItemCategory> GatePassItemCategories() {

        //    GatePassDBContext itemdbcontext = new GatePassDBContext();

        //    var category = from cat in itemdbcontext.GatePassItemCategories
        //                   select cat;

        //    List<GatePassItemCategory> GPCategoryList = new List<GatePassItemCategory>();

        //    foreach (var items in category) {

        //        GatePassItemCategory ItemCategory = new GatePassItemCategory();

        //        ItemCategory.GPItemCategoryID = items.GPItemCategoryID;
        //        ItemCategory.GPItemCategoryName = items.GPItemCategoryName;
        //        ItemCategory.GPItemCategoryDesc = items.GPItemCategoryDesc;
        //        ItemCategory.CreatedBy = items.CreatedBy;
        //        ItemCategory.ModifiedBy = items.ModifiedBy;
        //        ItemCategory.DateCreated = items.DateCreated;
        //        ItemCategory.DateModified = items.DateModified;

        //        GPCategoryList.Add(ItemCategory);
        //    }

        //    return GPCategoryList;
        //}


        //public static List<GatePassSupplier> GatePassSupplierList(string supplier) 
        //{

        //    GatePassDBContext supplierContext = new GatePassDBContext();

        //    var supplierlist = (from s in supplierContext.GatePassSuppliers
        //                        select s);


        //    if (supplier != null && supplier != "")
        //    {
        //        supplierlist = (from s in supplierContext.GatePassSuppliers
        //                        where s.CompanyName == supplier
        //                        orderby s.CompanyName ascending
        //                        select s);
        //    }
        //    else
        //    {
        //        supplierlist = (from s in supplierContext.GatePassSuppliers
        //                        //where s.CompanyName == supplier
        //                        orderby s.CompanyName ascending
        //                        select s);
        //    }


        //    List<GatePassSupplier> GPSupplierList = new List<GatePassSupplier>();
        //    foreach (var items in supplierlist)
        //    {
        //       GatePassSupplier splList = new GatePassSupplier();

        //       splList.CompanyName = items.CompanyName;
        //       splList.HouseNo = items.HouseNo;
        //       splList.Street = items.Street;
        //       splList.Barangay = items.Barangay;
        //       splList.Municipality = items.Municipality;
        //       splList.City = items.City;

        //       GPSupplierList.Add(splList);
        //    }

        //    return GPSupplierList;
        //}

        //public static List<GatePassTransactionDetails> GatePassTransactionItemDetails(string id)
        //{
        //    GatePassDBContext GPContext = new GatePassDBContext();
        //    var results = (from gpt in GPContext.GatePassTransactionDetails
        //                   select gpt);

        //    if (id != null && id != "")
        //    {
        //        results = (from gpt in GPContext.GatePassTransactionDetails
        //                   where gpt.GPID == id
        //                   select gpt);
        //    }
        //    if (id == null)
        //    {
        //        return null;
        //    }

        //    List<GatePassTransactionDetails> GPTransactionItemDetailList = new List<GatePassTransactionDetails>();


        //    foreach (var item in results)
        //    {
        //        GatePassTransactionDetails GPTransactionItem = new GatePassTransactionDetails();
        //        //UserModels UM = new UserModels();
        //        GPTransactionItem.GPTxnDetailsID = item.GPTxnDetailsID;
        //        GPTransactionItem.GPID = item.GPID;
        //        GPTransactionItem.TxnDate = item.TxnDate;
        //        GPTransactionItem.Comment = item.Comment;
        //        GPTransactionItem.TransactedBy = item.TransactedBy;
        //        GPTransactionItem.ItemName = item.ItemName;
        //        GPTransactionItem.ItemStatus = item.ItemStatus;
        //        GPTransactionItem.Serial = item.Serial;
        //        GPTransactionItem.Tag = item.Tag;
        //        GPTransactionItem.Stock = item.Stock;
        //        GPTransactionItem.CreatedBy = item.CreatedBy;
        //        GPTransactionItem.ModifiedBy = item.ModifiedBy;
        //        GPTransactionItem.DateCreated = item.DateCreated;
        //        GPTransactionItem.DateModified = item.DateModified;
        //        GPTransactionItem.GPItemCategoryID = item.GPItemCategoryID;
        //        GPTransactionItemDetailList.Add(GPTransactionItem);

        //    }

        //    return GPTransactionItemDetailList;
        //}


        //public static List<GatePassTransactionDetails> GatePassTransactionDetailsList(string itemsid)
        //{

        //    GatePassDBContext itemcontext = new GatePassDBContext();

        //    var itemdetails = (from i in itemcontext.GatePassTransactionDetails

        //                       select i);


        //   if (itemsid != null && itemsid != ""){

        //       itemdetails = from i in itemcontext.GatePassTransactionDetails
        //                     where i.GPTxnDetailsID == itemsid && i.IsDeleted == null
        //                     select i; 
                             
        //   }
        //   else {

        //       return null;
           
        //   }

        //   List<GatePassTransactionDetails> GPItemDetails = new List<GatePassTransactionDetails>();
          
        //    foreach (var details in itemdetails) {

        //        GatePassTransactionDetails gpd = new GatePassTransactionDetails();
             

        //      gpd.GPTxnDetailsID = details.GPTxnDetailsID;
        //      gpd.GPID = details.GPID;
        //       gpd.TxnDate = details.TxnDate;
        //       gpd.Comment = details.Comment;
        //       gpd.TransactedBy = details.TransactedBy;
        //       gpd.ItemName = details.ItemName;
        //       gpd.GPQuantity = details.GPQuantity;
        //       gpd.ItemStatus = details.ItemStatus;
        //       gpd.Serial = details.Serial;
        //       gpd.Tag = details.Tag;
        //       gpd.Stock = details.Stock;
        //       gpd.CreatedBy = details.CreatedBy;
        //       gpd.DateCreated = details.DateCreated;
        //       gpd.ModifiedBy = details.ModifiedBy;
        //       gpd.DateModified = details.DateModified;
        //       gpd.GPItemCategoryID = details.GPItemCategoryID;

        //       //gpd.gpicGPItemCategoryID = details.gpicGPItemCategoryID;
        //       //gpd.gpicGPItemCategoryName = details.gpicGPItemCategoryName;
        //       //gpd.gpicGPItemCategoryDesc = details.gpicGPItemCategoryDesc;
        //       //gpd.gpicCreatedBy = details.gpicCreatedBy;
        //       //gpd.gpicModifiedBy = details.gpicModifiedBy;
        //       //gpd.gpicDateCreated = details.gpicDateCreated;
        //       //gpd.gpicDateModified = details.gpicDateModified;
               

        //       GPItemDetails.Add(gpd);
        //   }

        //   return GPItemDetails;
        //}

        //public static List<GatePassTransactionDetails> GatePassTransactionDetailsListByGPID(string gpid)
        //{

        //    GatePassDBContext itemcontext = new GatePassDBContext();

        //    var itemdetails = (from i in itemcontext.GatePassTransactionDetails

        //                       select i);


        //    if (gpid != null && gpid != "")
        //    {

        //        itemdetails = from i in itemcontext.GatePassTransactionDetails
        //                      where i.GPID == gpid && i.IsDeleted == null
        //                      select i;

        //    }
        //    else
        //    {

        //        return null;

        //    }

        //    List<GatePassTransactionDetails> GPItemDetailsByGPID = new List<GatePassTransactionDetails>();

        //    foreach (var details in itemdetails)
        //    {

        //        GatePassTransactionDetails gpd = new GatePassTransactionDetails();


        //        gpd.GPTxnDetailsID = details.GPTxnDetailsID;
        //        gpd.GPID = details.GPID;
        //        gpd.TxnDate = details.TxnDate;
        //        gpd.Comment = details.Comment;
        //        gpd.TransactedBy = details.TransactedBy;
        //        gpd.ItemName = details.ItemName;
        //        gpd.GPQuantity = details.GPQuantity;
        //        gpd.ItemStatus = details.ItemStatus;
        //        gpd.Serial = details.Serial;
        //        gpd.Tag = details.Tag;
        //        gpd.Stock = details.Stock;
        //        gpd.CreatedBy = details.CreatedBy;
        //        gpd.DateCreated = details.DateCreated;
        //        gpd.ModifiedBy = details.ModifiedBy;
        //        gpd.DateModified = details.DateModified;
        //        gpd.GPItemCategoryID = details.GPItemCategoryID;

        //        //gpd.gpicGPItemCategoryID = details.gpicGPItemCategoryID;
        //        //gpd.gpicGPItemCategoryName = details.gpicGPItemCategoryName;
        //        //gpd.gpicGPItemCategoryDesc = details.gpicGPItemCategoryDesc;
        //        //gpd.gpicCreatedBy = details.gpicCreatedBy;
        //        //gpd.gpicModifiedBy = details.gpicModifiedBy;
        //        //gpd.gpicDateCreated = details.gpicDateCreated;
        //        //gpd.gpicDateModified = details.gpicDateModified;


        //        GPItemDetailsByGPID.Add(gpd);
        //    }

        //    return GPItemDetailsByGPID;
        //}

        //public static List<GatePassTypeSubSections> GatePassSubSectionByID(string subID) {

        //    GatePassDBContext gp2 = new GatePassDBContext();

        //    var results = from e in gp2.GatePassTypeSubSections
        //                  where e.GPTypeSubSectionsID == subID
        //                  select e;

        //    List<GatePassTypeSubSections> GPSubSectionByIDList = new List<GatePassTypeSubSections>();

        //    foreach (var item in results)
        //    {
        //        GatePassTypeSubSections GPSubSectionByID = new GatePassTypeSubSections();

        //        GPSubSectionByID.GPTxnType = item.GPTxnType;
        //        GPSubSectionByIDList.Add(GPSubSectionByID);
            
        //    }

        //    return GPSubSectionByIDList;
        //}

        //public static List<GatePassTransactionDetails> GatePassTransactionDetailsUpdate(string updateID) {

        //    GatePassDBContext GPContext = new GatePassDBContext();

        //    var results = (from gpt in GPContext.GatePassTransactionDetails
        //                   select gpt);

        //    if (updateID != null && updateID != "")
        //    {
        //        results = (from gpt in GPContext.GatePassTransactionDetails
        //                   where gpt.GPTxnDetailsID == updateID
        //                   select gpt);
        //    }
        //    if (updateID == null)
        //    {
        //        return null;
        //    }

        //    List<GatePassTransactionDetails> GPTransactionItemDetailList = new List<GatePassTransactionDetails>();


        //    foreach (var item in results)
        //    {
        //        GatePassTransactionDetails GPTransactionItem = new GatePassTransactionDetails();
        //        //UserModels UM = new UserModels();
        //        GPTransactionItem.GPTxnDetailsID = item.GPTxnDetailsID;
        //        GPTransactionItem.GPID = item.GPID;
        //        GPTransactionItem.TxnDate = item.TxnDate;
        //        GPTransactionItem.GPQuantity = item.GPQuantity;
        //        GPTransactionItem.Comment = item.Comment;
        //        GPTransactionItem.TransactedBy = item.TransactedBy;
        //        GPTransactionItem.ItemName = item.ItemName;
        //        GPTransactionItem.ItemStatus = item.ItemStatus;
        //        GPTransactionItem.Serial = item.Serial;
        //        GPTransactionItem.Tag = item.Tag;
        //        GPTransactionItem.Stock = item.Stock;
        //        GPTransactionItem.CreatedBy = item.CreatedBy;
        //        GPTransactionItem.ModifiedBy = item.ModifiedBy;
        //        GPTransactionItem.DateCreated = item.DateCreated;
        //        GPTransactionItem.DateModified = item.DateModified;
        //        GPTransactionItem.GPItemCategoryID = item.GPItemCategoryID;
        //        GPTransactionItem.IsDeleted = item.IsDeleted;
        //        GPTransactionItemDetailList.Add(GPTransactionItem);

        //    }
            
        //    return GPTransactionItemDetailList;
        
        
        //}


        public static List<ActiveDirectoryUser> Approvers(string user)
        {

            List<ActiveDirectoryUser> apprvList = ActiveDirectory.GetCurrenUserManagers(OrganizationalUnitScope.AMPI, user);

            return apprvList;
            
        
        }

        public static List<ActiveDirectoryUser> UserProfile(string username) {

            List<ActiveDirectoryUser> userInfo = ActiveDirectory.GetCurrentUserProfile(OrganizationalUnitScope.AMPI, username);

            return userInfo;
        
        }

        public static List<ActiveDirectoryUser> DepartmentList() {


          List<ActiveDirectoryUser> deptList = ActiveDirectory.GetAllDepartments(OrganizationalUnitScope.AMPI);

          return deptList;
        
        }


        //public static List<Images> ImageList(string txnID) {


        //    GatePassDBContext imgDBContext = new GatePassDBContext();

        //    var imgdetails = from img in imgDBContext.Images
        //                     select img;

        //    if (txnID != null && txnID != "")
        //    {

        //        imgdetails = from img in imgDBContext.Images
        //                     where img.TxnDetailsID == txnID
        //                     select img;
        //    }

        //    else {

        //        return null;
        //    }

        //    List<Images> ImgList = new List<Images>();

        //    foreach (var item in imgdetails)
        //    {

        //        Images Image = new Images();

        //        Image.ImageName = item.ImageName;
        //        Image.ImageBytes = item.ImageBytes;
        //        Image.ImageExtension = item.ImageExtension;
        //        ImgList.Add(Image);
            
        //    }

        //    return ImgList;
        //} 
    }
}