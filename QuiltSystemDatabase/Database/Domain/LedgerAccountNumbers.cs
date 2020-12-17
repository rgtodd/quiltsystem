//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Database.Domain
{
    public static class LedgerAccountNumbers
    {
        // 1XXX - Assets (Debit)

        public const int Cash = 1000;
        public const int AccountReceivable = 1100;
        public const int SalesTaxReceivable = 1200;
        public const int FabricSupplyAsset = 1300;
        public const int FabricSupplySuspense = 1310;

        // 2XXX - Liabilities (Credit)

        public const int AccountPayable = 2100;
        public const int SalesTaxPayable = 2200;
        public const int FundsSuspense = 2300;

        // 3XXX - Equity (Credit)
        public const int OwnersEquity = 3000;

        // 4XXX - Income (Credit)

        public const int Income = 4100;

        // 5XXX - Expense (Debit)

        public const int PaymentFeeExpense = 5100;
        public const int FabricSupplyExpense = 5200;
    }
}