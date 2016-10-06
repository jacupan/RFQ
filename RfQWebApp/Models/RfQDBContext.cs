using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Objects;
using System.Linq;
using System.Web;
using System.Data.Entity.Infrastructure;

namespace RFQWebApp.Models
{
    public class RfQDBContext : DbContext
    {

        public int SaveChanges(bool refreshOnConcurrencyException, RefreshMode refreshMode = RefreshMode.ClientWins)
        {
            try
            {
                return SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                foreach (DbEntityEntry entry in ex.Entries)
                {
                    if (refreshMode == RefreshMode.ClientWins)
                        entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                    else
                        entry.Reload();
                }
                return SaveChanges();
            }
        }

        public DbSet<BuyersAccess> BuyersAccess { get; set; }

        public DbSet<BuyersLookup> BuyersLookup { get; set; }

        public DbSet<CategoryList> CategoryList { get; set; }

        public DbSet<RfqType> Type { get; set; }

        public DbSet<RepeatOrderCode> RepeatOrderCode { get; set; }

        public DbSet<OverallStatus> OverallStatus { get; set; }

        public DbSet<ItemStatusCodeDdl> ItemStatusCodeDdl { get; set; }

        public DbSet<UomCode> UomCode { get; set; }

        public DbSet<RfqAttachments> RfqAttachments { get; set; }

        public DbSet<RfqUserAttachments> RfqUserAttachments { get; set; }

        public DbSet<RfqTransactionDetails> RfqTransactionDetails { get; set; }

        public DbSet<RfqTransactionDetailsBuyer> RfqTransactionDetailsBuyer { get; set; }

        public DbSet<RfqTransactions> RfqTransactions { get; set; }

        public DbSet<RfqCount> RfqCount { get; set; }

        public DbSet<RfqCountChart> RfqCountChart { get; set; }

        public DbSet<RfqReAssignedBuyer> RfqReAssignedBuyer { get; set; }

        public DbSet<RfqReportByDateSubmitted> RfqReportByDateSubmitted { get; set; }      

        public RfQDBContext() : base("name=RfQContext") { }

        public void Refresh(RefreshMode clientWins, DbSet<RfqTransactionDetails> rfqTransactionDetails)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }
    }
}