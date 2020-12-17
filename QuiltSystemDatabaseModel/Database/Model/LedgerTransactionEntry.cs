//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class LedgerTransactionEntry
    {
        public long LedgerTransactionEntryId { get; set; }
        public long LedgerTransactionId { get; set; }
        public int LedgerAccountNumber { get; set; }
        public long LedgerAccountSubtotalId { get; set; }
        public decimal TransactionEntryAmount { get; set; }
        public string DebitCreditCode { get; set; }
        public string SalesTaxJurisdiction { get; set; }
        public string LedgerReference { get; set; }

        public virtual LedgerAccount LedgerAccountNumberNavigation { get; set; }
        public virtual LedgerAccountSubtotal LedgerAccountSubtotal { get; set; }
        public virtual LedgerTransaction LedgerTransaction { get; set; }
    }
}
