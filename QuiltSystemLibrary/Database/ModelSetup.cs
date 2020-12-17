//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using RichTodd.QuiltSystem.Business.ComponentProviders;
using RichTodd.QuiltSystem.Business.Libraries;
using RichTodd.QuiltSystem.Database.Domain;
using RichTodd.QuiltSystem.Database.Model;
using RichTodd.QuiltSystem.Design.Component.Standard;
using RichTodd.QuiltSystem.Design.Nodes.Generator;
using RichTodd.QuiltSystem.Properties;
using RichTodd.QuiltSystem.Security;
using RichTodd.QuiltSystem.Service.Core;
using RichTodd.QuiltSystem.Service.Database.Abstractions;
using RichTodd.QuiltSystem.Utility;

namespace RichTodd.QuiltSystem.Database
{
    public static class ModelSetup
    {
        public static void CreateDatabase(IQuiltContextFactory quiltContextFactory)
        {
            using var ctx = quiltContextFactory.Create();

            _ = ctx.Database.EnsureCreated();
            ExecuteScript(ctx, Resources.ModelCreateViews);
        }

        public static async Task CreateStandardEntitiesAsync(IConfiguration configuration, IQuiltContextFactory quiltContextFactory, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            CreateStandardDomains(quiltContextFactory);
            CreateStandardStates(configuration, quiltContextFactory);
            CreateStandardShippingVendors(quiltContextFactory);
            CreateStandardResources(quiltContextFactory);
            await CreateStandardRolesAsync(roleManager).ConfigureAwait(false);
            await CreateStandardUsersAsync(userManager).ConfigureAwait(false);
        }

        private static void ExecuteScript(QuiltContext ctx, string script)
        {
            StringBuilder sb = null;
            var lines = script.Split('\r', '\n');
            foreach (var rawLine in lines)
            {
                var line = rawLine.Trim();
                if (string.IsNullOrEmpty(line))
                {
                    // Ignore
                }
                else if (line == "GO")
                {
                    if (sb != null)
                    {
                        _ = ctx.Database.ExecuteSqlRaw(sb.ToString());
                        sb = null;
                    }
                }
                else
                {
                    if (sb == null)
                    {
                        sb = new StringBuilder();
                    }
                    _ = sb.AppendLine(line);
                }
            }
            if (sb != null)
            {
                _ = ctx.Database.ExecuteSqlRaw(sb.ToString());
            }
        }

        private static void CreateAccountingYearStatusType(QuiltContext ctx, string AccountingYearStatusType, string name)
        {
            Console.WriteLine("CreateAccountingYearStatusType {0}", AccountingYearStatusType);

            var dbAccountingYearStatusType = ctx.AccountingYearStatusTypes.Where(r => r.AccountingYearStatusCode == AccountingYearStatusType).SingleOrDefault();
            if (dbAccountingYearStatusType == null)
            {
                dbAccountingYearStatusType = new AccountingYearStatusType()
                {
                    AccountingYearStatusCode = AccountingYearStatusType,
                    Name = name
                };
                _ = ctx.AccountingYearStatusTypes.Add(dbAccountingYearStatusType);
            }
            else
            {
                dbAccountingYearStatusType.Name = name;
            }
        }

        private static void CreateAddressType(QuiltContext ctx, string addressType, string name)
        {
            Console.WriteLine("CreateAddressType {0}", addressType);

            var dbAddressType = ctx.AddressTypes.Where(r => r.AddressTypeCode == addressType).SingleOrDefault();
            if (dbAddressType == null)
            {
                dbAddressType = new AddressType()
                {
                    AddressTypeCode = addressType,
                    Name = name
                };
                _ = ctx.AddressTypes.Add(dbAddressType);
            }
            else
            {
                dbAddressType.Name = name;
            }
        }

        private static void CreateAlertType(QuiltContext ctx, string alertTypeCode, string name)
        {
            Console.WriteLine("CreateAlertType {0}", alertTypeCode);

            var dbAlertType = ctx.AlertTypes.Where(r => r.AlertTypeCode == alertTypeCode).SingleOrDefault();
            if (dbAlertType == null)
            {
                dbAlertType = new AlertType()
                {
                    AlertTypeCode = alertTypeCode,
                    Name = name
                };
                _ = ctx.AlertTypes.Add(dbAlertType);
            }
            else
            {
                dbAlertType.Name = name;
            }
        }

        private static void CreateArtifactType(QuiltContext ctx, string artifactTypeCode, string name)
        {
            Console.WriteLine("CreateArtifactType {0}", artifactTypeCode);

            var dbArtifactType = ctx.ArtifactTypes.Where(r => r.ArtifactTypeCode == artifactTypeCode).SingleOrDefault();
            if (dbArtifactType == null)
            {
                dbArtifactType = new ArtifactType()
                {
                    ArtifactTypeCode = artifactTypeCode,
                    Name = name
                };
                _ = ctx.ArtifactTypes.Add(dbArtifactType);
            }
            else
            {
                dbArtifactType.Name = name;
            }
        }

        private static void CreateArtifactValueType(QuiltContext ctx, string artifactValueTypeCode, string name)
        {
            Console.WriteLine("CreateArtifactValueType {0}", artifactValueTypeCode);

            var dbArtifactValueType = ctx.ArtifactValueTypes.Where(r => r.ArtifactValueTypeCode == artifactValueTypeCode).SingleOrDefault();
            if (dbArtifactValueType == null)
            {
                dbArtifactValueType = new ArtifactValueType()
                {
                    ArtifactValueTypeCode = artifactValueTypeCode,
                    Name = name
                };
                _ = ctx.ArtifactValueTypes.Add(dbArtifactValueType);
            }
            else
            {
                dbArtifactValueType.Name = name;
            }
        }

        private static void CreateInventoryItemType(QuiltContext ctx, string inventoryItemTypeCode, string name)
        {
            Console.WriteLine("CreateInventoryItemType {0}", inventoryItemTypeCode);

            var dbInventoryItemType = ctx.InventoryItemTypes.Where(r => r.InventoryItemTypeCode == inventoryItemTypeCode).SingleOrDefault();
            if (dbInventoryItemType == null)
            {
                dbInventoryItemType = new InventoryItemType()
                {
                    InventoryItemTypeCode = inventoryItemTypeCode,
                    Name = name
                };
                _ = ctx.InventoryItemTypes.Add(dbInventoryItemType);
            }
            else
            {
                dbInventoryItemType.Name = name;
            }
        }

        private static void CreateLedgerAccount(QuiltContext ctx, int ledgerAccountNumber, string name, string debitCreditCode)
        {
            Console.WriteLine("CreateLedgerAccountType {0}", ledgerAccountNumber);

            var dbLedgerAccount = ctx.LedgerAccounts.Where(r => r.LedgerAccountNumber == ledgerAccountNumber).SingleOrDefault();
            if (dbLedgerAccount == null)
            {
                dbLedgerAccount = new LedgerAccount()
                {
                    LedgerAccountNumber = ledgerAccountNumber,
                    Name = name,
                    DebitCreditCode = debitCreditCode
                };
                _ = ctx.LedgerAccounts.Add(dbLedgerAccount);
            }
            else
            {
                dbLedgerAccount.Name = name;
                dbLedgerAccount.DebitCreditCode = debitCreditCode;
            }
        }

        private static void CreateLogEntryType(QuiltContext ctx, string logEntryTypeCode, string name)
        {
            Console.WriteLine("CreateLogEntryType {0}", logEntryTypeCode);

            var dbLogEntryType = ctx.LogEntryTypes.Where(r => r.LogEntryTypeCode == logEntryTypeCode).SingleOrDefault();
            if (dbLogEntryType == null)
            {
                dbLogEntryType = new LogEntryType()
                {
                    LogEntryTypeCode = logEntryTypeCode,
                    Name = name
                };
                _ = ctx.LogEntryTypes.Add(dbLogEntryType);
            }
            else
            {
                dbLogEntryType.Name = name;
            }
        }

        private static void CreateNotificationType(QuiltContext ctx, string notificationTypeCode, string name, string subject, string body, string bodyType)
        {
            Console.WriteLine("CreateNotificationType {0}", notificationTypeCode);

            var dbNotificationType = ctx.NotificationTypes.Where(r => r.NotificationTypeCode == notificationTypeCode).SingleOrDefault();
            if (dbNotificationType == null)
            {
                dbNotificationType = new NotificationType()
                {
                    NotificationTypeCode = notificationTypeCode,
                    Name = name,
                    Subject = subject,
                    Body = body,
                    BodyTypeCode = bodyType
                };
                _ = ctx.NotificationTypes.Add(dbNotificationType);
            }
            else
            {
                dbNotificationType.Name = name;
                dbNotificationType.Subject = subject;
                dbNotificationType.Body = body;
                dbNotificationType.BodyTypeCode = bodyType;
            }
        }

        private static void CreateReturnRequestReasonType(QuiltContext ctx, string returnRequestReasonCode, string name, bool allowRefund, bool allowReplacement, bool active, int sortOrder)
        {
            Console.WriteLine("CreateOrderReturnRequestReasonType {0}", returnRequestReasonCode);

            var dbReturnRequestReason = ctx.ReturnRequestReasons.Where(r => r.ReturnRequestReasonCode == returnRequestReasonCode).SingleOrDefault();
            if (dbReturnRequestReason == null)
            {
                dbReturnRequestReason = new ReturnRequestReason()
                {
                    ReturnRequestReasonCode = returnRequestReasonCode,
                    Name = name,
                    AllowRefund = allowRefund,
                    AllowReplacement = allowReplacement,
                    Active = active,
                    SortOrder = sortOrder
                };
                _ = ctx.ReturnRequestReasons.Add(dbReturnRequestReason);
            }
            else
            {
                dbReturnRequestReason.Name = name;
                dbReturnRequestReason.AllowRefund = allowRefund;
                dbReturnRequestReason.AllowReplacement = allowReplacement;
                dbReturnRequestReason.Active = active;
                dbReturnRequestReason.SortOrder = sortOrder;
            }
        }

        private static void CreateReturnRequestStatusType(QuiltContext ctx, string orderReturnRequestStatusType, string name)
        {
            Console.WriteLine("CreateOrderReturnRequestStatusType {0}", orderReturnRequestStatusType);

            var dbReturnRequestStatusType = ctx.ReturnRequestStatusTypes.Where(r => r.ReturnRequestStatusCode == orderReturnRequestStatusType).SingleOrDefault();
            if (dbReturnRequestStatusType == null)
            {
                dbReturnRequestStatusType = new ReturnRequestStatusType()
                {
                    ReturnRequestStatusCode = orderReturnRequestStatusType,
                    Name = name
                };
                _ = ctx.ReturnRequestStatusTypes.Add(dbReturnRequestStatusType);
            }
            else
            {
                dbReturnRequestStatusType.Name = name;
            }
        }

        private static void CreateReturnRequestType(QuiltContext ctx, string orderReturnRequestType, string name)
        {
            Console.WriteLine("CreateOrderReturnRequestType {0}", orderReturnRequestType);

            var dbReturnRequestType = ctx.ReturnRequestTypes.Where(r => r.ReturnRequestTypeCode == orderReturnRequestType).SingleOrDefault();
            if (dbReturnRequestType == null)
            {
                dbReturnRequestType = new ReturnRequestType()
                {
                    ReturnRequestTypeCode = orderReturnRequestType,
                    Name = name
                };
                _ = ctx.ReturnRequestTypes.Add(dbReturnRequestType);
            }
            else
            {
                dbReturnRequestType.Name = name;
            }
        }

        private static void CreateReturnStatusType(QuiltContext ctx, string orderReturnStatusType, string name)
        {
            Console.WriteLine("CreateOrderReturnStatusType {0}", orderReturnStatusType);

            var dbReturnStatusType = ctx.ReturnStatusTypes.Where(r => r.ReturnStatusCode == orderReturnStatusType).SingleOrDefault();
            if (dbReturnStatusType == null)
            {
                dbReturnStatusType = new ReturnStatusType()
                {
                    ReturnStatusCode = orderReturnStatusType,
                    Name = name
                };
                _ = ctx.ReturnStatusTypes.Add(dbReturnStatusType);
            }
            else
            {
                dbReturnStatusType.Name = name;
            }
        }

        private static void CreateShipmentRequestStatusType(QuiltContext ctx, string shipmentRequestStatusType, string name)
        {
            Console.WriteLine("CreateShipmentRequestStatusType {0}", shipmentRequestStatusType);

            var dbShipmentRequestStatusType = ctx.ShipmentRequestStatusTypes.Where(r => r.ShipmentRequestStatusCode == shipmentRequestStatusType).SingleOrDefault();
            if (dbShipmentRequestStatusType == null)
            {
                dbShipmentRequestStatusType = new ShipmentRequestStatusType()
                {
                    ShipmentRequestStatusCode = shipmentRequestStatusType,
                    Name = name
                };
                _ = ctx.ShipmentRequestStatusTypes.Add(dbShipmentRequestStatusType);
            }
            else
            {
                dbShipmentRequestStatusType.Name = name;
            }
        }

        private static void CreateShipmentStatusType(QuiltContext ctx, string shipmentStatusType, string name)
        {
            Console.WriteLine("CreateShipmentStatusType {0}", shipmentStatusType);

            var dbShipmentStatusType = ctx.ShipmentStatusTypes.Where(r => r.ShipmentStatusCode == shipmentStatusType).SingleOrDefault();
            if (dbShipmentStatusType == null)
            {
                dbShipmentStatusType = new ShipmentStatusType()
                {
                    ShipmentStatusCode = shipmentStatusType,
                    Name = name
                };
                _ = ctx.ShipmentStatusTypes.Add(dbShipmentStatusType);
            }
            else
            {
                dbShipmentStatusType.Name = name;
            }
        }

        private static void CreateOrderStatusType(QuiltContext ctx, string orderStatusCode, string name)
        {
            Console.WriteLine("CreateOrderStatusType {0}", orderStatusCode);

            var dbOrderStatusType = ctx.OrderStatusTypes.Where(r => r.OrderStatusCode == orderStatusCode).SingleOrDefault();
            if (dbOrderStatusType == null)
            {
                dbOrderStatusType = new OrderStatusType()
                {
                    OrderStatusCode = orderStatusCode,
                    Name = name
                };
                _ = ctx.OrderStatusTypes.Add(dbOrderStatusType);
            }
            else
            {
                dbOrderStatusType.Name = name;
            }
        }

        private static void CreateOrderTransactionType(QuiltContext ctx, string orderTransactionTypeCode, string name)
        {
            Console.WriteLine("CreateOrderTransactionType {0}", orderTransactionTypeCode);

            var dbOrderTransactionType = ctx.OrderTransactionTypes.Where(r => r.OrderTransactionTypeCode == orderTransactionTypeCode).SingleOrDefault();
            if (dbOrderTransactionType == null)
            {
                dbOrderTransactionType = new OrderTransactionType()
                {
                    OrderTransactionTypeCode = orderTransactionTypeCode,
                    Name = name
                };
                _ = ctx.OrderTransactionTypes.Add(dbOrderTransactionType);
            }
            else
            {
                dbOrderTransactionType.Name = name;
            }
        }

        private static void CreateProjectType(QuiltContext ctx, string projectTypeCode, string name)
        {
            Console.WriteLine("CreateProjectType {0}", projectTypeCode);

            var dbProjectType = ctx.ProjectTypes.Where(r => r.ProjectTypeCode == projectTypeCode).SingleOrDefault();
            if (dbProjectType == null)
            {
                dbProjectType = new ProjectType()
                {
                    ProjectTypeCode = projectTypeCode,
                    Name = name
                };
                _ = ctx.ProjectTypes.Add(dbProjectType);
            }
            else
            {
                dbProjectType.Name = name;
            }
        }

        private static void CreateShippingVendor(QuiltContext ctx, string name)
        {
            var dbShippingVendor = ctx.ShippingVendors.Where(r => r.Name == name).SingleOrDefault();
            if (dbShippingVendor == null)
            {
                dbShippingVendor = new ShippingVendor()
                {
                    Name = name
                };
                _ = ctx.ShippingVendors.Add(dbShippingVendor);
            }
        }

        private static void CreateShippingVendor(QuiltContext ctx, string shippingVendorId, string name)
        {
            Console.WriteLine("CreateShippingVendor {0}", shippingVendorId);

            var dbShippingVendor = ctx.ShippingVendors.Where(r => r.ShippingVendorId == shippingVendorId).SingleOrDefault();
            if (dbShippingVendor == null)
            {
                dbShippingVendor = new ShippingVendor()
                {
                    ShippingVendorId = shippingVendorId,
                    Name = name
                };
                _ = ctx.ShippingVendors.Add(dbShippingVendor);
            }
            else
            {
                dbShippingVendor.Name = name;
            }
        }

        private static void CreateStandardDomains(IQuiltContextFactory quiltContextFactory)
        {
            using var ctx = quiltContextFactory.Create();

            CreateAccountingYearStatusType(ctx, AccountingYearStatusTypeCodes.Open, "Open");

            CreateAddressType(ctx, AddressTypeCodes.Shipping, "Shipping");

            CreateAlertType(ctx, AlertTypeCodes.OperationException, "Operation Exception");
            CreateAlertType(ctx, AlertTypeCodes.OrderReceipt, "Order Receipt");
            CreateAlertType(ctx, AlertTypeCodes.OrderReceiptFailure, "Order Receipt Failure");
            CreateAlertType(ctx, AlertTypeCodes.OrderReceiptMismatch, "Order Receipt Mismatch");
            CreateAlertType(ctx, AlertTypeCodes.OrderPaymentMismatch, "Order Payment Mismatch");
            CreateAlertType(ctx, AlertTypeCodes.PayPalIpnFailure, "PayPal Failure");
            CreateAlertType(ctx, AlertTypeCodes.PayPalIpnSuccess, "PayPal Success");
            CreateAlertType(ctx, AlertTypeCodes.UnexpectedOrderPayment, "Unexpected Order Payment");

            CreateArtifactType(ctx, ArtifactTypeCodes.Component, "Component");
            CreateArtifactType(ctx, ArtifactTypeCodes.Design, "Design");
            CreateArtifactType(ctx, ArtifactTypeCodes.Kit, "Kit");

            CreateArtifactValueType(ctx, ArtifactValueTypeCodes.Json, "JSON");

            CreateInventoryItemType(ctx, InventoryItemTypeCodes.Fabric, "Fabric");

            // 1XXX - Assets (Debit)
            CreateLedgerAccount(ctx, LedgerAccountNumbers.Cash, "Cash", LedgerAccountCodes.Debit);
            CreateLedgerAccount(ctx, LedgerAccountNumbers.AccountReceivable, "Account Receivable", LedgerAccountCodes.Debit);
            CreateLedgerAccount(ctx, LedgerAccountNumbers.SalesTaxReceivable, "Sales Tax Receivable", LedgerAccountCodes.Debit);
            CreateLedgerAccount(ctx, LedgerAccountNumbers.FabricSupplyAsset, "Fabric Supply Asset", LedgerAccountCodes.Debit);
            CreateLedgerAccount(ctx, LedgerAccountNumbers.FabricSupplySuspense, "Fabric Supply Suspense", LedgerAccountCodes.Debit);

            // 2XXX - Liabilities (Credit)
            CreateLedgerAccount(ctx, LedgerAccountNumbers.AccountPayable, "Account Payable", LedgerAccountCodes.Credit);
            CreateLedgerAccount(ctx, LedgerAccountNumbers.SalesTaxPayable, "Sales Tax Payable", LedgerAccountCodes.Credit);
            CreateLedgerAccount(ctx, LedgerAccountNumbers.FundsSuspense, "Funds Suspense", LedgerAccountCodes.Credit);

            // 3XXX - Equity(Credit)
            CreateLedgerAccount(ctx, LedgerAccountNumbers.OwnersEquity, "Owners Equity", LedgerAccountCodes.Credit);

            // 4XXX - Income (Credit)
            CreateLedgerAccount(ctx, LedgerAccountNumbers.Income, "Income", LedgerAccountCodes.Credit);

            // 5XXX - Expense (Debit)
            CreateLedgerAccount(ctx, LedgerAccountNumbers.PaymentFeeExpense, "Payment Fee Expense", LedgerAccountCodes.Debit);
            CreateLedgerAccount(ctx, LedgerAccountNumbers.FabricSupplyExpense, "Fabric Supply Expense", LedgerAccountCodes.Debit);

            CreateLogEntryType(ctx, LogEntryTypeCodes.Controller, "Controller");
            CreateLogEntryType(ctx, LogEntryTypeCodes.Job, "Job");
            CreateLogEntryType(ctx, LogEntryTypeCodes.Service, "Service");

            CreateNotificationType(ctx, NotificationTypeCodes.OrderShipped, "Order Shipped", "Your order has shipped", Resources.OrderShippedNotification, "text/plain");
            CreateNotificationType(ctx, NotificationTypeCodes.OrderShipping, "Order Shipping", "Your order is being processed", Resources.OrderShippingNotification, "text/plain");
            CreateNotificationType(ctx, NotificationTypeCodes.RefundIssued, "Refund Issued", "A refund for your order has been issued", Resources.RefundIssuedNotification, "text/plain");

            CreateOrderStatusType(ctx, OrderStatusCodes.Pending, "PENDING");
            CreateOrderStatusType(ctx, OrderStatusCodes.Submitted, "SUBMITTED");
            CreateOrderStatusType(ctx, OrderStatusCodes.Fulfilling, "FULFILLING");
            CreateOrderStatusType(ctx, OrderStatusCodes.Closed, "CLOSED");

            CreateReturnRequestReasonType(ctx, ReturnRequestReasonCodes.ItemNotOrdered, "Item not ordered", true, true, true, 0);
            CreateReturnRequestReasonType(ctx, ReturnRequestReasonCodes.ItemDefective, "Item defective", true, true, true, 0);
            CreateReturnRequestReasonType(ctx, ReturnRequestReasonCodes.ItemDamagedDuringShipping, "Item damaged during shipping", true, true, true, 0);
            CreateReturnRequestReasonType(ctx, ReturnRequestReasonCodes.NoLongerNeeded, "No longer needed", true, true, true, 0);
            CreateReturnRequestReasonType(ctx, ReturnRequestReasonCodes.ItemArrivedTooLate, "Item arrived too late", true, true, true, 0);

            CreateReturnRequestStatusType(ctx, ReturnRequestStatusCodes.Open, "Open");
            CreateReturnRequestStatusType(ctx, ReturnRequestStatusCodes.Posted, "Posted");
            CreateReturnRequestStatusType(ctx, ReturnRequestStatusCodes.Complete, "Complete");
            CreateReturnRequestStatusType(ctx, ReturnRequestStatusCodes.Cancelled, "Cancelled");

            CreateReturnRequestType(ctx, ReturnRequestTypeCodes.Manual, "Manual");
            CreateReturnRequestType(ctx, ReturnRequestTypeCodes.Replace, "Replace");
            CreateReturnRequestType(ctx, ReturnRequestTypeCodes.Return, "Return");

            CreateReturnStatusType(ctx, ReturnStatusCodes.Open, "Open");
            CreateReturnStatusType(ctx, ReturnStatusCodes.Posted, "Posted");
            CreateReturnStatusType(ctx, ReturnStatusCodes.Complete, "Complete");
            CreateReturnStatusType(ctx, ReturnStatusCodes.Cancelled, "Cancelled");

            CreateShipmentRequestStatusType(ctx, ShipmentRequestStatusCodes.Pending, "Pending");
            CreateShipmentRequestStatusType(ctx, ShipmentRequestStatusCodes.Open, "Open");
            CreateShipmentRequestStatusType(ctx, ShipmentRequestStatusCodes.Complete, "Complete");
            CreateShipmentRequestStatusType(ctx, ShipmentRequestStatusCodes.Cancelled, "Cancelled");
            CreateShipmentRequestStatusType(ctx, ShipmentRequestStatusCodes.Exception, "Exception");

            CreateShipmentStatusType(ctx, ShipmentStatusCodes.Open, "Open");
            CreateShipmentStatusType(ctx, ShipmentStatusCodes.Posted, "Posted");
            CreateShipmentStatusType(ctx, ShipmentStatusCodes.Complete, "Complete");
            CreateShipmentStatusType(ctx, ShipmentStatusCodes.Cancelled, "Cancelled");
            CreateShipmentStatusType(ctx, ShipmentStatusCodes.Exception, "Exception");

            CreateOrderTransactionType(ctx, OrderTransactionTypeCodes.Submit, "Submit");
            CreateOrderTransactionType(ctx, OrderTransactionTypeCodes.FundsReceived, "Funds Received");
            CreateOrderTransactionType(ctx, OrderTransactionTypeCodes.FundsRequired, "Funds Required");
            CreateOrderTransactionType(ctx, OrderTransactionTypeCodes.FulfillmentRequired, "Fulfillment Required");
            CreateOrderTransactionType(ctx, OrderTransactionTypeCodes.FulfillmentComplete, "Fulfillment Complete");
            CreateOrderTransactionType(ctx, OrderTransactionTypeCodes.FulfillmentReturn, "Fulfillment Return");
            CreateOrderTransactionType(ctx, OrderTransactionTypeCodes.Close, "Close");

            CreateProjectType(ctx, ProjectTypeCodes.Kit, "Kit");

            CreateShippingVendor(ctx, ShippingVendorIds.FedEx, "FedEx");
            CreateShippingVendor(ctx, ShippingVendorIds.Ups, "UPS");
            CreateShippingVendor(ctx, ShippingVendorIds.Usps, "USPS");

            CreateTagCategory(ctx, TagCategoryCodes.InventoryItem, "Inventory Item");
            CreateTagCategory(ctx, TagCategoryCodes.Resource, "Resource");

            CreateTagType(ctx, TagTypeCodes.Collection, TagCategoryCodes.InventoryItem, "Collection");
            CreateTagType(ctx, TagTypeCodes.Manufacturer, TagCategoryCodes.InventoryItem, "Manufacturer");
            CreateTagType(ctx, TagTypeCodes.Block, TagCategoryCodes.Resource, "Block");

            CreateUnitOfMeasure(ctx, UnitOfMeasureCodes.FatQuarter, "Fat Quarter");
            CreateUnitOfMeasure(ctx, UnitOfMeasureCodes.HalfYardage, "Half Yard");
            CreateUnitOfMeasure(ctx, UnitOfMeasureCodes.Yardage, "Yard");
            CreateUnitOfMeasure(ctx, UnitOfMeasureCodes.TwoYards, "Two Yard");
            CreateUnitOfMeasure(ctx, UnitOfMeasureCodes.ThreeYards, "Three Yard");

            _ = ctx.SaveChanges();
        }

        public static void CreateStandardResources(IQuiltContextFactory quiltContextFactory)
        {
            var library = DatabaseResourceLibrary.Load(quiltContextFactory, Constants.DefaultComponentCategory);

            foreach (var nodeGenerator in GetNodeGenerators())
            {
                foreach (var item in nodeGenerator.Generate())
                {
                    var entry = library.GetEntry(item.Name);
                    if (entry == null)
                    {
                        library.CreateEntry(item.Name, DatabaseBlockComponentProvider.ResourceTypePrefix + BlockComponent.TypeName, item.Node.JsonSave().ToString(), item.Tags);
                    }
                    else
                    {
                        library.UpdateEntry(item.Name, item.Node.JsonSave().ToString(), item.Tags);
                    }
                }
            }
        }

        //private static void CreateStandardResourcesX(IApplicationConfiguration appConfiguration, QuiltContextFactory quiltContextFactory)
        //{
        //    var json = AbstractResourceLibrary.AzureLoad(appConfiguration, Constants.DefaultComponentCategory);
        //    DatabaseResourceLibrary.Create(quiltContextFactory, json);

        //    var library = DatabaseResourceLibrary.Load(quiltContextFactory, Constants.DefaultComponentCategory);
        //    foreach (var entry in library.GetEntries())
        //    {
        //        var node = ResourceNodeFactory.Create(entry);
        //        library.UpdateEntry(entry.Name, node.JsonSave().ToString());
        //    }
        //}

        private static void CreateStandardShippingVendors(IQuiltContextFactory quiltContextFactory)
        {
            using var ctx = quiltContextFactory.Create();

            CreateShippingVendor(ctx, "USPS");
            CreateShippingVendor(ctx, "FedEx");
            CreateShippingVendor(ctx, "UPS");

            _ = ctx.SaveChanges();
        }

        private static void CreateStandardStates(IConfiguration configuration, IQuiltContextFactory quiltContextFactory)
        {
            var text = AzureUtility.LoadAzureStringBlob(configuration, "database-load", "states.csv");
            var lines = text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            using var ctx = quiltContextFactory.Create();

            var dbCountry = ctx.Countries.Where(r => r.CountryCode == "US").SingleOrDefault();
            if (dbCountry == null)
            {
                dbCountry = new Country()
                {
                    CountryCode = "US",
                    Name = "United States"
                };
                _ = ctx.Countries.Add(dbCountry);
            }

            foreach (var line in lines)
            {
                var fields = line.Split(new char[] { ',' });

                var stateCode = fields[0];
                var name = fields[1];
                var countryCode = fields[2];

                CreateState(ctx, stateCode, name, countryCode);
            }

            _ = ctx.SaveChanges();
        }

        private static async Task CreateStandardRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            await EnsureRoleExistsAsync(roleManager, ApplicationRoles.Administrator).ConfigureAwait(false);
            await EnsureRoleExistsAsync(roleManager, ApplicationRoles.Service).ConfigureAwait(false);
            await EnsureRoleExistsAsync(roleManager, ApplicationRoles.User).ConfigureAwait(false);

            await EnsureRoleExistsAsync(roleManager, ApplicationRoles.FinancialViewer).ConfigureAwait(false);
            await EnsureRoleExistsAsync(roleManager, ApplicationRoles.FinancialEditor).ConfigureAwait(false);
            await EnsureRoleExistsAsync(roleManager, ApplicationRoles.FulfillmentViewer).ConfigureAwait(false);
            await EnsureRoleExistsAsync(roleManager, ApplicationRoles.FulfillmentEditor).ConfigureAwait(false);
            await EnsureRoleExistsAsync(roleManager, ApplicationRoles.UserViewer).ConfigureAwait(false);
            await EnsureRoleExistsAsync(roleManager, ApplicationRoles.UserEditor).ConfigureAwait(false);
        }

        private static async Task EnsureRoleExistsAsync(RoleManager<IdentityRole> roleManager, string role)
        {
            var roleExists = await roleManager.RoleExistsAsync(role).ConfigureAwait(false);
            if (!roleExists)
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole(role)).ConfigureAwait(false);
                if (!roleResult.Succeeded)
                {
                    throw new Exception("Role " + role + " could not be created.");
                }
            }
        }

        private static async Task CreateStandardUsersAsync(UserManager<IdentityUser> userManager)
        {
            await CreateUser(userManager, BuiltInUsers.AdminUser, "admin@richtodd.com", "admin@richtodd.com", "testtest", ApplicationRoles.Administrator).ConfigureAwait(false);

            await CreateUser(userManager, BuiltInUsers.ServiceUser, "AdminService", "admin-service@richtodd.com", "testtest", ApplicationRoles.Service).ConfigureAwait(false);

            await CreateUser(userManager, null, "user@richtodd.com", "user@richtodd.com", "testtest", ApplicationRoles.User).ConfigureAwait(false);

            await CreateUser(userManager, null, "ship@richtodd.com", "ship@richtodd.com", "testtest", ApplicationRoles.FulfillmentEditor).ConfigureAwait(false);
        }

        private static void CreateState(QuiltContext ctx, string stateCode, string name, string countryCode)
        {
            var dbState = ctx.States.Where(r => r.StateCode == stateCode).SingleOrDefault();
            if (dbState == null)
            {
                dbState = new State()
                {
                    StateCode = stateCode,
                    Name = name,
                    CountryCode = countryCode
                };
                _ = ctx.States.Add(dbState);
            }
            else
            {
                dbState.Name = name;
                dbState.CountryCode = countryCode;
            }
        }

        private static void CreateTagCategory(QuiltContext ctx, string tagCategoryCode, string name)
        {
            Console.WriteLine("CreateTagCategory {0}", tagCategoryCode);

            var dbTagCategory = ctx.TagCategories.Where(r => r.TagCategoryCode == tagCategoryCode).SingleOrDefault();
            if (dbTagCategory == null)
            {
                dbTagCategory = new TagCategory()
                {
                    TagCategoryCode = tagCategoryCode,
                    Name = name
                };
                _ = ctx.TagCategories.Add(dbTagCategory);
            }
            else
            {
                dbTagCategory.Name = name;
            }
        }

        private static void CreateTagType(QuiltContext ctx, string tagTypeCode, string tagCategoryCode, string name)
        {
            Console.WriteLine("CreateTagType {0}", tagTypeCode);

            var dbTagType = ctx.TagTypes.Where(r => r.TagTypeCode == tagTypeCode).SingleOrDefault();
            if (dbTagType == null)
            {
                dbTagType = new TagType()
                {
                    TagTypeCode = tagTypeCode,
                    TagCategoryCode = tagCategoryCode,
                    Name = name
                };
                _ = ctx.TagTypes.Add(dbTagType);
            }
            else
            {
                dbTagType.TagCategoryCode = tagCategoryCode;
                dbTagType.Name = name;
            }
        }

        private static void CreateUnitOfMeasure(QuiltContext ctx, string unitOfMeasureCode, string name)
        {
            Console.WriteLine("CreateUnitOfMeasure {0}", unitOfMeasureCode);

            var dbUnitOfMeasure = ctx.UnitOfMeasures.Where(r => r.UnitOfMeasureCode == unitOfMeasureCode).SingleOrDefault();
            if (dbUnitOfMeasure == null)
            {
                dbUnitOfMeasure = new UnitOfMeasure()
                {
                    UnitOfMeasureCode = unitOfMeasureCode,
                    Name = name
                };
                _ = ctx.UnitOfMeasures.Add(dbUnitOfMeasure);
            }
            else
            {
                dbUnitOfMeasure.Name = name;
            }
        }

        private static async Task CreateUser(UserManager<IdentityUser> userManager, string id, string userName, string email, string password, string role)
        {
            var user = await userManager.FindByNameAsync(userName).ConfigureAwait(false);
            if (user != null)
            {
                return;
            }

            // HACK: UserManager creates ID automatically?
            if (id == null)
            {
                id = Guid.NewGuid().ToString();
            }

            user = new IdentityUser
            {
                Id = id,
                UserName = userName,
                Email = email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                throw new Exception("Could not create user " + userName);
            }

            user = await userManager.FindByNameAsync(userName).ConfigureAwait(false);
            if (user == null)
            {
                throw new Exception("Could not find user " + userName);
            }

            result = await userManager.AddToRoleAsync(user, role).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                throw new Exception("Could not add role " + role + " to user " + userName);
            }
        }

        private static IEnumerable<INodeGenerator> GetNodeGenerators()
        {
            //yield return new BlockNodeGenerator();
            yield return new HalfSquareTriangleNodeGenerator();
        }
    }
}