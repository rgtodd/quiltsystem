//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Web;

namespace RichTodd.QuiltSystem.WebAdmin.Models.User
{
    public class User
    {
        public AUser_User AUser { get; }
        public IApplicationLocale Locale { get; }

        public User(
            AUser_User aUser,
            IApplicationLocale locale)
        {
            AUser = aUser;
            Locale = locale;
        }

        public MOrder_OrderSummaryList MOrderSummaries => AUser.MOrders;
        public MSquare_PaymentSummaryList MSquarePaymentSummaries => AUser.MSquarePayments;
        public MSquare_CustomerSummary MSquareCustomerSummary => AUser.MSquareCustomer;

        [Display(Name = "User ID")]
        public string UserId => AUser.UserId;

        [Display(Name = "User Name")]
        public string UserName => AUser.UserName;

        [Display(Name = "Email")]
        public string Email => AUser.Email;

        [Display(Name = "Email Confirmed")]
        public bool EmailConfirmed => AUser.EmailConfirmed;

        [Display(Name = "Phone Number")]
        public string PhoneNumber => AUser.PhoneNumber;

        [Display(Name = "Phone Number Confirmed")]
        public bool PhoneNumberConfirmed => AUser.PhoneNumberConfirmed;

        [Display(Name = "Two-Factor Enabled")]
        public bool TwoFactorEnabled => AUser.TwoFactorEnabled;

        [Display(Name = "Lockout Enabled")]
        public bool LockedEnabled => AUser.LockoutEnabled;

        [Display(Name = "Access Failed Count")]
        public int AccessFailedCount => AUser.AccessFailedCount;

        [Display(Name = "Square Customer ID")]
        public long? SquareCustomerId => MSquareCustomerSummary?.SquareCustomerId;

        private string m_roleList;

        [Display(Name = "Roles")]
        public string RoleList
        {
            get
            {
                if (m_roleList == null)
                {
                    var prefix = "";
                    var sb = new StringBuilder();
                    foreach (var role in AUser.Roles)
                    {
                        _ = sb.Append(prefix); prefix = ", ";
                        _ = sb.Append(role);
                    }

                    m_roleList = sb.ToString();
                }

                return m_roleList;
            }
        }

        private string m_loginProviderList;

        [Display(Name = "Login Providers")]
        public string LoginProviderList
        {
            get
            {
                if (m_loginProviderList == null)
                {
                    var prefix = "";
                    var sb = new StringBuilder();
                    foreach (var loginProvider in AUser.LoginProviders)
                    {
                        _ = sb.Append(prefix); prefix = ", ";
                        _ = sb.Append(loginProvider);
                    }

                    m_loginProviderList = sb.ToString();
                }

                return m_loginProviderList;
            }
        }

        private IList<UserOrder> m_orders;
        public IList<UserOrder> Orders
        {
            get
            {
                if (m_orders == null)
                {
                    m_orders = MOrderSummaries?.Summaries != null
                        ? MOrderSummaries.Summaries
                            .Select(r => new UserOrder(r, Locale))
                            .ToList()
                        : new List<UserOrder>(0);
                }

                return m_orders;
            }
        }

        private IList<UserSquarePayment> m_squarePayments;
        public IList<UserSquarePayment> SquarePayments
        {
            get
            {
                if (m_squarePayments == null)
                {
                    m_squarePayments = MSquarePaymentSummaries?.Summaries != null
                        ? MSquarePaymentSummaries.Summaries
                            .Select(r => new UserSquarePayment(r, Locale))
                            .ToList()
                        : new List<UserSquarePayment>(0);
                }

                return m_squarePayments;
            }
        }
    }

    public class UserSquarePayment
    {
        public MSquare_PaymentSummary MSquarePaymentSummary { get; }
        public IApplicationLocale Locale { get; }

        public UserSquarePayment(
            MSquare_PaymentSummary mSquarePaymentSummary,
            IApplicationLocale locale)
        {
            MSquarePaymentSummary = mSquarePaymentSummary;
            Locale = locale;
        }

        [Display(Name = "Square Payment ID")]
        public long SquarePaymentId => MSquarePaymentSummary.SquarePaymentId;

        [Display(Name = "Square Payment Reference")]
        public string SquarePaymentReference => MSquarePaymentSummary.SquarePaymentReference;

        [Display(Name = "Square Customer ID")]
        public long SquareCustomerId => MSquarePaymentSummary.SquareCustomerId;

        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        [Display(Name = "Payment Amount")]
        public decimal PaymentAmount => MSquarePaymentSummary.PaymentAmount;

        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        [Display(Name = "Refund Amount")]
        public decimal RefundAmount => MSquarePaymentSummary.RefundAmount;

        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        [Display(Name = "Processing Fee Amount")]
        public decimal ProcessingFeeAmount => MSquarePaymentSummary.ProcessingFeeAmount;

        [Display(Name = "Square Payment Record ID")]
        public string SquarePaymentRecordId => MSquarePaymentSummary.SquarePaymentRecordId;

        [Display(Name = "Version Number")]
        public int? VersionNumber => MSquarePaymentSummary.VersionNumber;

        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        [Display(Name = "Update Date/Time")]
        public DateTime UpdateDateTime => Locale.GetLocalTimeFromUtc(MSquarePaymentSummary.UpdateDateTimeUtc);
    }

    public class UserOrder
    {
        public MOrder_OrderSummary MOrderSummary { get; }
        public IApplicationLocale Locale { get; }

        public UserOrder(
            MOrder_OrderSummary mOrderSummary,
            IApplicationLocale locale)
        {
            MOrderSummary = mOrderSummary;
            Locale = locale;
        }

        [Display(Name = "Order ID")]
        public long OrderId => MOrderSummary.OrderId;

        [Display(Name = "Orderer ID")]
        public long OrdererId => MOrderSummary.OrdererId;

        [Display(Name = "Order Number")]
        public string OrderNumber => MOrderSummary.OrderNumber;

        [Display(Name = "Order Status")]
        public string OrderStatus => MOrderSummary.OrderStatus.ToString();

        [Display(Name = "Order Date/Time")]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime OrderDateTime => Locale.GetLocalTimeFromUtc(MOrderSummary.OrderDateTimeUtc);

        [Display(Name = "Udpate Date/Time")]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime UpdateDateTime => Locale.GetLocalTimeFromUtc(MOrderSummary.UpdateDateTimeUtc);

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        [Display(Name = "Item Subtotal Amount")]
        public decimal ItemSubtotalAmount => MOrderSummary.ItemSubtotalAmount;

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        [Display(Name = "Shipping Amount")]
        public decimal ShippingAmount => MOrderSummary.ShippingAmount;

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        [Display(Name = "Discount Amount")]
        public decimal DiscountAmount => MOrderSummary.DiscountAmount;

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        [Display(Name = "Taxable Amount")]
        public decimal TaxableAmount => MOrderSummary.TaxableAmount;

        [Display(Name = "Sales Tax Jurisdiction")]
        public string SalesTaxJurisdiction => MOrderSummary.SalesTaxJurisdiction;

        [Display(Name = "Sales Tax Percent")]
        [DisplayFormat(DataFormatString = Standard.PercentFormat)]
        public decimal SalesTaxPercent => MOrderSummary.SalesTaxPercent;

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        [Display(Name = "Sales Tax Amount")]
        public decimal SalesTaxAmount => MOrderSummary.SalesTaxAmount;

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        [Display(Name = "total Amount")]
        public decimal TotalAmount => MOrderSummary.TotalAmount;
    }
}