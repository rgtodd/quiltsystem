//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.ComponentModel.DataAnnotations;

using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Web;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Transaction
{
    public class TransactionListItem
    {
        public MCommon_TransactionSummary MSummary { get; }
        public IApplicationLocale Locale { get; }

        public TransactionListItem(
            MCommon_TransactionSummary mSummary,
            IApplicationLocale locale)
        {
            MSummary = mSummary;
            Locale = locale;
        }

        [Display(Name = "Source")]
        public string Source => MSummary.Source;

        [Display(Name = "Transaction ID")]
        public long TransactionId => MSummary.TransactionId;

        [Display(Name = "Entity ID")]
        public long EntityId => MSummary.EntityId;

        [Display(Name = "Transaction Date/Time")]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime TransactionDateTime => Locale.GetLocalTimeFromUtc(MSummary.TransactionDateTimeUtc);

        [Display(Name = "Description")]
        public string Description => MSummary.Description;

        [Display(Name = "Unit of Work")]
        public string UnitOfWork => MSummary.UnitOfWork;
    }
}
