//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Linq;

using RichTodd.QuiltSystem.Database.Builders;

namespace RichTodd.QuiltSystem.Database.Model.Extensions
{
    public static class QuiltContextExtensions
    {
        public static OrderTransactionBuilder CreateOrderTransactionBuilder(this QuiltContext ctx)
        {
            return new OrderTransactionBuilder(ctx);
        }

        public static FundableTransactionBuilder CreateFundableTransactionBuilder(this QuiltContext ctx)
        {
            return new FundableTransactionBuilder(ctx);
        }

        public static FunderTransactionBuilder CreateFunderTransactionBuilder(this QuiltContext ctx)
        {
            return new FunderTransactionBuilder(ctx);
        }

        public static FulfillmentTransactionBuilder CreateFulfillmentTransactionBuilder(this QuiltContext ctx)
        {
            return new FulfillmentTransactionBuilder(ctx);
        }

        public static ReturnRequestTransactionBuilder CreateReturnRequestTransactionBuilder(this QuiltContext ctx)
        {
            return new ReturnRequestTransactionBuilder(ctx);
        }

        public static ReturnTransactionBuilder CreateReturnTransactionBuilder(this QuiltContext ctx)
        {
            return new ReturnTransactionBuilder(ctx);
        }

        public static ShipmentRequestTransactionBuilder CreateShipmentRequestTransactionBuilder(this QuiltContext ctx)
        {
            return new ShipmentRequestTransactionBuilder(ctx);
        }

        public static ShipmentTransactionBuilder CreateShipmentTransactionBuilder(this QuiltContext ctx)
        {
            return new ShipmentTransactionBuilder(ctx);
        }

        public static InventoryItemStockTransactionBuilder GetInventoryItemStockTransactionBuilder(this QuiltContext ctx, DateTime utcNow, DateTime localNow)
        {
            return new InventoryItemStockTransactionBuilder(ctx, utcNow, localNow);
        }

        public static LedgerAccountTransactionBuilder CreateLedgerAccountTransactionBuilder(this QuiltContext ctx)
        {
            return new LedgerAccountTransactionBuilder(ctx);
        }

        public static string GetOrderNumber(this QuiltContext ctx, DateTime utcNow)
        {
            var dbOrderNumber = ctx.OrderNumbers.Where(r => r.OrderDateUtc == utcNow.Date).SingleOrDefault();
            if (dbOrderNumber == null)
            {
                dbOrderNumber = new OrderNumber()
                {
                    OrderDateUtc = utcNow.Date,
                    Number = 1
                };
                _ = ctx.OrderNumbers.Add(dbOrderNumber);
            }
            else
            {
                dbOrderNumber.Number += 1;
            }

            var orderNumber = string.Format("{0:00}-{1:000}-{2:0000}", utcNow.Year - 2000, utcNow.DayOfYear, dbOrderNumber.Number);

            return orderNumber;
        }

        public static string GetProjectNumber(this QuiltContext ctx, DateTime utcNow)
        {
            return ctx.GetOrderNumber(utcNow);
        }

        public static string GetShipmentRequestNumber(this QuiltContext ctx, DateTime utcNow)
        {
            return ctx.GetOrderNumber(utcNow);
        }

        public static string GetShipmentNumber(this QuiltContext ctx, DateTime utcNow)
        {
            return ctx.GetOrderNumber(utcNow);
        }

        public static string GetReturnRequestNumber(this QuiltContext ctx, DateTime utcNow)
        {
            return ctx.GetOrderNumber(utcNow);
        }

        public static string GetReturnNumber(this QuiltContext ctx, DateTime utcNow)
        {
            return ctx.GetOrderNumber(utcNow);
        }

        public static UserProfile GetUserProfile(this QuiltContext ctx, string userId)
        {
            var userProfileAspNetUser = ctx.UserProfileAspNetUsers.Where(r => r.AspNetUserId == userId).SingleOrDefault();
            if (userProfileAspNetUser != null)
            {
                return userProfileAspNetUser.UserProfile;
            }

            var userProfile = new UserProfile()
            {
                UserProfileReference = userId
            };
            _ = ctx.UserProfiles.Add(userProfile);

            userProfileAspNetUser = new UserProfileAspNetUser()
            {
                AspNetUserId = userId
            };
            userProfile.UserProfileAspNetUser = userProfileAspNetUser;

            return userProfile;
        }

        public static AddressType AddressType(this QuiltContext ctx, string code)
        {
            return ctx.AddressTypes.Where(r => r.AddressTypeCode == code).Single();
        }

        public static LedgerAccount LedgerAccount(this QuiltContext ctx, int number)
        {
            return ctx.LedgerAccounts.Where(r => r.LedgerAccountNumber == number).Single();
        }

        public static LogEntryType LogEntryType(this QuiltContext ctx, string code)
        {
            return ctx.LogEntryTypes.Where(r => r.LogEntryTypeCode == code).Single();
        }

        public static ReturnRequestStatusType ReturnRequestStatusType(this QuiltContext ctx, string code)
        {
            return ctx.ReturnRequestStatusTypes.Where(r => r.ReturnRequestStatusCode == code).Single();
        }

        public static ReturnRequestType ReturnRequestType(this QuiltContext ctx, string code)
        {
            return ctx.ReturnRequestTypes.Where(r => r.ReturnRequestTypeCode == code).Single();
        }

        public static ReturnStatusType ReturnStatusType(this QuiltContext ctx, string code)
        {
            return ctx.ReturnStatusTypes.Where(r => r.ReturnStatusCode == code).Single();
        }

        public static ShipmentRequestStatusType ShipmentRequestStatusType(this QuiltContext ctx, string code)
        {
            return ctx.ShipmentRequestStatusTypes.Where(r => r.ShipmentRequestStatusCode == code).Single();
        }

        public static ShipmentStatusType ShipmentStatusType(this QuiltContext ctx, string code)
        {
            return ctx.ShipmentStatusTypes.Where(r => r.ShipmentStatusCode == code).Single();
        }

        public static OrderStatusType OrderStatusType(this QuiltContext ctx, string code)
        {
            return ctx.OrderStatusTypes.Where(r => r.OrderStatusCode == code).Single();
        }

        public static OrderTransactionType OrderTransactionType(this QuiltContext ctx, string code)
        {
            return ctx.OrderTransactionTypes.Where(r => r.OrderTransactionTypeCode == code).Single();
        }

        public static TagType TagType(this QuiltContext ctx, string code)
        {
            return ctx.TagTypes.Where(r => r.TagTypeCode == code).Single();
        }

        public static UnitOfMeasure UnitOfMeasure(this QuiltContext ctx, string code)
        {
            return ctx.UnitOfMeasures.Where(r => r.UnitOfMeasureCode == code).Single();
        }
    }
}
