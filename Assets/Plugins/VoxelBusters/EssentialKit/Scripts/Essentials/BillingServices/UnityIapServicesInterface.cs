#if ESSENTIAL_KIT_USE_UNITY_IAP_BILLING
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using VoxelBusters.CoreLibrary;
using VoxelBusters.CoreLibrary.NativePlugins;

namespace VoxelBusters.EssentialKit.BillingServicesCore
{
    public class UnityIapInterface : NativeBillingServicesInterfaceBase, IStoreListener
    {
#region Fields

        private     IStoreController    m_storeController;

        private     IExtensionProvider  m_extensionProvider;

        private     List<Product>       m_Transactions;

#endregion

#region Constructors

        public UnityIapInterface()
            : base(isAvailable: true)
        {
            // Set properties
            m_Transactions    = new List<Product>();
        }

#endregion

#region Base class methods

        public override bool CanMakePayments()
        {
            return false;
        }

        public override void RetrieveProducts(BillingProductDefinition[] productDefinitions)
        {
            // Get product list
            var     convertedDefinitions    = System.Array.ConvertAll(productDefinitions, (item) => UnityIAPUtility.ConvertToUnityProductDefinition(item));
            var     builder                 = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            builder.AddProducts(convertedDefinitions);

            // Call operation
            UnityPurchasing.Initialize(this, builder);
        }

        public override bool StartPayment(IBillingPayment payment, out Error error)
        {
            // Guard case
            if (!CanPerformTransactions(out error))
            {
                return false;
            }

            // Initiate request
            var     product = m_storeController.products.WithID(payment.ProductId);
            if ((product == null) || !product.availableToPurchase)
            {
                error       = BillingError.ProductUnavailable;
                return false;
            }

            m_storeController.InitiatePurchase(product);
            return true;
        }

        public override IBillingTransaction[] GetTransactions()
        {
            return new IBillingTransaction[0];
        }

        public override void FinishTransactions(IBillingTransaction[] transactions)
        { 
        }

        public override void RestorePurchases(string tag)
        {
            // Guard case
            if (!CanPerformTransactions(out Error error))
            {
                SendRestorePurchasesCompleteEvent(new IBillingTransaction[0], error);
                return;
            }
            
            var     apple   = m_extensionProvider.GetExtension<IAppleExtensions>();
            if (apple != null)
            {
                apple.RestoreTransactions((result) => {
                        // Restore purchases initiated. See ProcessPurchase for any restored transactions.
                    });
            }
        }

        public override bool TryClearingUnfinishedTransactions()
        {
            return false;
        }

#endregion


#region Private methods

        private bool CanPerformTransactions(out Error error, bool includeRestore = false)
        {
            if (m_storeController == null)
            {
                error   = BillingError.StoreNotInitialized;
                return false;
            }
            if (includeRestore && (m_extensionProvider == null))
            {
                error   = BillingError.RestoreNotSupported;
                return false;
            }

            error   = null;
            return true;
        }

        private void ProcessTransactionData()
        {
            var     pendingTransactions = m_Transactions.ToArray();

            /*
            var     extension   = m_extensionProvider.GetExtension<IAppleExtensions>();
            extension.RefreshAppReceipt(
                successCallback: (receipt) =>
                {
                    //foreach (var product in receipt.)
                },
                errorCallback: () =>
                {
                });





            var     product     = e.purchasedProduct;
            var     payment     = new BillingPayment(
                productId: product.definition.id,
                productPlatformId: product.definition.storeSpecificId,
                quantity: 1,
                tag: null);
            var     transaction = new BillingTransactionPlain(
                transactionId: product.transactionID,
                payment: payment,
                transactionDate: System.DateTime.Now,
                transactionState: BillingTransactionState.Failed,
                verificationState: BillingReceiptVerificationState.NotDetermined,
                receipt: product.receipt);
            SendPaymentStateChangeEvent(transaction);
            */
        }

#endregion

#region IStoreListener implementation

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            // Store references
            m_storeController   = controller;
            m_extensionProvider = extensions;

            // Forward event data to the caller
            var     unityProductCollection  = controller.products;
            var     nativeProducts          = System.Array.ConvertAll(
                unityProductCollection.all,
                (item) => UnityIAPUtility.ConvertToNativeProduct(item));
            SendRetrieveProductsCompleteEvent(
                products: nativeProducts,
                invalidProductIds: new string[0],
                error: null);
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            SendRetrieveProductsCompleteEvent(
                products: new IBillingProduct[0],
                invalidProductIds: new string[0],
                error: UnityIAPUtility.ConvertToNativeError(error));
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            
            return PurchaseProcessingResult.Pending;
        }

        public void OnPurchaseFailed(Product item, PurchaseFailureReason r)
        {
            var     payment     = new BillingPayment(
                productId: item.definition.id,
                productPlatformId: item.definition.storeSpecificId,
                quantity: 1,
                tag: null);
            var     transaction = new BillingTransactionPlain(
                transactionId: item.transactionID,
                payment: payment,
                transactionDate: System.DateTime.Now,
                transactionState: BillingTransactionState.Failed,
                verificationState: BillingReceiptVerificationState.NotDetermined,
                receipt: item.receipt,
                error: UnityIAPUtility.ConvertToNativeError(r));
            SendPaymentStateChangeEvent(transaction);
        }

#endregion

        void ConfigureAppleFraudDetection(IAppleExtensions appleExtensions)
        {
            /*
            //To make sure the account id and profile id do not contain personally identifiable information, we obfuscate this information by hashing it.
            var hashedUsername = HashString(user.Username);

            appleExtensions.SetApplicationUsername(hashedUsername);
            */
        }

        static string HashString(string input)
        {
            var stringBuilder = new StringBuilder();
            foreach (var b in GetHash(input))
                stringBuilder.Append(b.ToString("X2"));

            return stringBuilder.ToString();
        }

        static IEnumerable<byte> GetHash(string input)
        {
            using (HashAlgorithm algorithm = SHA256.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
        }

    }
}
#endif