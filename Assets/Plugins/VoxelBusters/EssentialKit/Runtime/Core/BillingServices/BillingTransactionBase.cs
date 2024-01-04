using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.CoreLibrary;
using VoxelBusters.CoreLibrary.NativePlugins;

namespace VoxelBusters.EssentialKit.BillingServicesCore
{
    public abstract class BillingTransactionBase : NativeObjectBase, IBillingTransaction
    {
        #region Properties

        protected BillingTransactionBase(string transactionId, IBillingPayment payment)
        {
            // set properties
            Id          = transactionId;
            Payment     = payment;
        }

        ~BillingTransactionBase()
        {
            Dispose(false);
        }

        #endregion

        #region Abstract methods

        protected abstract DateTime GetTransactionDateUTCInternal();

        protected abstract BillingTransactionState GetTransactionStateInternal();

        protected abstract BillingReceiptVerificationState GetReceiptVerificationStateInternal();

        protected abstract void SetReceiptVerificationStateInternal(BillingReceiptVerificationState value);
        
        protected abstract string GetReceiptInternal();

        protected abstract Error GetErrorInternal();

        protected abstract BillingTransactionAndroidProperties GetAndroidPropertiesInternal();

        #endregion

        #region IBillingTransaction implementation

        public string Id { get; private set; }

        public IBillingPayment Payment { get; private set; }

        public DateTime DateUTC => GetTransactionDateUTCInternal();

        public DateTime Date => DateUTC.ToLocalTime();

        public BillingTransactionState TransactionState => GetTransactionStateInternal();

        public BillingReceiptVerificationState ReceiptVerificationState
        {
            get => GetReceiptVerificationStateInternal();
            set => SetReceiptVerificationStateInternal(value);
        }

        public string Receipt => GetReceiptInternal();

        public Error Error => GetErrorInternal();

        public BillingTransactionAndroidProperties AndroidProperties => GetAndroidPropertiesInternal();

        #endregion
    }
}