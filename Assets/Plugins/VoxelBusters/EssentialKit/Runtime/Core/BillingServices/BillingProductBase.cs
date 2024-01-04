using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VoxelBusters.CoreLibrary;
using VoxelBusters.CoreLibrary.NativePlugins;

namespace VoxelBusters.EssentialKit.BillingServicesCore
{
    public abstract class BillingProductBase : NativeObjectBase, IBillingProduct
    {
        #region Constructors

        protected BillingProductBase(string id, string platformId,
            IEnumerable<BillingProductPayoutDefinition> payouts, bool isAvailable = true)
        {
            // Set properties
            Id              = id;
            PlatformId      = platformId;
            Payouts         = payouts;
            IsAvailable     = isAvailable;
        }

        ~BillingProductBase()
        {
            Dispose(false);
        }

        #endregion

        #region Abstract methods

        protected abstract string GetLocalizedTitleInternal();

        protected abstract string GetLocalizedDescriptionInternal();

        protected abstract string GetPriceInternal();

        protected abstract string GetLocalizedPriceInternal();

        protected abstract string GetPriceCurrencyCodeInternal();

        #endregion

        #region Base methods

        public override string ToString()
        {
            var     sb  = new StringBuilder(128);
            sb.Append("BillingProduct { ")
                .Append($"Id: {Id} ")
                .Append($"Title: {LocalizedTitle} ")
                .Append($"Price: {Price} ")
                .Append($"LocalizedPrice: {LocalizedPrice}")
                .Append($"Payout: {Payouts}")
                .Append("}");
            return sb.ToString();
        }

        #endregion

        #region IBillingProduct implementation

        public string Id { get; private set; }

        public string PlatformId { get; private set; }

        public string LocalizedTitle => GetLocalizedTitleInternal();

        public string LocalizedDescription => GetLocalizedDescriptionInternal();

        public string Price => GetPriceInternal();

        public string LocalizedPrice => GetLocalizedPriceInternal();

        public string PriceCurrencyCode => GetPriceCurrencyCodeInternal();

        public bool IsAvailable { get; private set; }

        public IEnumerable<BillingProductPayoutDefinition> Payouts { get; private set; }

        public object Tag => null;

        #endregion
    }
}