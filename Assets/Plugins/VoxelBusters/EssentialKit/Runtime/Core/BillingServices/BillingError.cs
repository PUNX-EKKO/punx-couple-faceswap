using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.EssentialKit
{
    public static class BillingError
    {
        #region Constants

        public  const   string  kDomain = "BillingServices";

        #endregion

        #region Properties

        public static Error Unknown { get; private set; } = CreateError(
            code: Code.kUnknown,
            description: "Unknown error.");

        public static Error StoreNotInitialized { get; private set; } = CreateError(
            code: Code.kStoreNotInitialized,
            description: "Store not initialized.");

        public static Error StoreNotAvailable { get; private set; } = CreateError(
            code: Code.kStoreNotAvailable,
            description: "Store not available.");

        public static Error ConfigurationInvalid { get; private set; } = CreateError(
            code: Code.kConfigurationInvalid,
            description: "Configuration invalid.");

        public static Error ProductUnavailable { get; private set; } = CreateError(
            code: Code.kProductUnavailable,
            description: "Product not found.");

        public static Error PaymentInvalid { get; private set; } = CreateError(
            code: Code.kPaymentInvalid,
            description: "Payment invalid.");

        public static Error PaymentNotAllowed { get; private set; } = CreateError(
            code: Code.kPaymentNotAllowed,
            description: "Payment not allowed.");

        public static Error PaymentDeclined { get; private set; } = CreateError(
            code: Code.kPaymentDeclined,
            description: "Payment declined.");

        public static Error PaymentCancelled { get; private set; } = CreateError(
            code: Code.kPaymentCancelled,
            description: "Payment cancelled.");

        public static Error SignatureInvalid { get; private set; } = CreateError(
            code: Code.kSignatureInvalid,
            description: "Payment cancelled.");

        public static Error RestoreNotSupported { get; private set; } = CreateError(
            code: Code.kRestoreNotSupported,
            description: "Restore purchase is not supported.");
         
        #endregion

        #region Static methods

        private static Error CreateError(int code, string description) => new Error(
            domain: kDomain,
            code: code,
            description: description);

        #endregion

        #region Nested types

        /// <summary>
        /// Constants indicating the possible error that might occur when using billing services.
        /// </summary>
        public static class Code
        {
            /// <summary> Error code indicating that an unknown or unexpected error occurred. </summary>
            public  const   int     kUnknown                    = 0;

            /// <summary> Error code indicating that product is not found. </summary>
            public  const   int     kStoreNotInitialized        = 1;

            /// <summary> Error code indicating that store is not available. </summary>
            public  const   int     kStoreNotAvailable          = 2;

            /// <summary> Error code indicating that store is not available. </summary>
            public  const   int     kConfigurationInvalid       = 3;

            /// <summary> Error code indicating that product is not found. </summary>
            public  const   int     kProductUnavailable        = 4;

            /// <summary> Error code indicating that product is not found. </summary>
            public  const   int     kPaymentInvalid             = 5;

            /// <summary> Error code indicating that product is not found. </summary>
            public  const   int     kPaymentNotAllowed          = 6;

            /// <summary> Error code indicating that product is not found. </summary>
            public  const   int     kPaymentDeclined            = 7;

            /// <summary> Error code indicating that product is not found. </summary>
            public  const   int     kPaymentCancelled           = 8;

            /// <summary> Error code indicating that product is not found. </summary>
            public  const   int     kSignatureInvalid           = 9;

            /// <summary> Error code indicating that restore operation is not supported. </summary>
            public  const   int     kRestoreNotSupported        = 10;
        }

        #endregion
    }
}