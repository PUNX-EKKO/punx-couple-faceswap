using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VoxelBusters.EssentialKit;
using VoxelBusters.CoreLibrary;
using TMPro;
using Firebase.Analytics;

public class IAPManager : MonoBehaviour
{
    public Button buyNowBtn;
    public GameObject IAPDialog;
    public Button[] iapItemBtns;
    public GameObject[] bullets;
    public TextMeshProUGUI[] rookieTexts;
    public TextMeshProUGUI[] starTexts;
    public TextMeshProUGUI[] iconTexts;
    public GameObject[] discountsUI;
    public IAPPackages iAPPackage;
    private IBillingProduct[] _storeProducts;
    
    private void OnEnable()
    {
        // register for events
        BillingServices.OnInitializeStoreComplete   += OnInitializeStoreComplete;
        BillingServices.OnTransactionStateChange    += OnTransactionStateChange;
        BillingServices.OnRestorePurchasesComplete  += OnRestorePurchasesComplete;
    }

    private void OnDisable()
    {
        // unregister from events
        BillingServices.OnInitializeStoreComplete   -= OnInitializeStoreComplete;
        BillingServices.OnTransactionStateChange    -= OnTransactionStateChange;
        BillingServices.OnRestorePurchasesComplete  -= OnRestorePurchasesComplete;
    }

    void Start()
    {
        if(BillingServices.IsAvailable()){
            BillingServices.InitializeStore();
        }
    }


  
    public void CheckSubscription(){
        if(FirestoreDatabase.instance.userData.allowImageGeneration && FirestoreDatabase.instance.userData.imageCredit > 0){
            IAPDialog.SetActive(false);
        }else{
            IAPDialog.SetActive(true);
            FirestoreDatabase.instance.GetOrders();
            SwitchSelection(0);
            SetupItemProducts();
            if(!FirestoreDatabase.instance.userData.hasIAPDiscount){
                for (int i = 0; i < discountsUI.Length; i++)
                {
                    discountsUI[i].SetActive(false);
                }
            }else{
                for (int i = 0; i < discountsUI.Length; i++)
                {
                    discountsUI[i].SetActive(true);
                }
            }
        }
    }
    public void SwitchSelection(int btnIndex){

        switch (btnIndex)
        {
            case 0:
                bullets[0].SetActive(true);
                bullets[1].SetActive(false);
                bullets[2].SetActive(false);
                iAPPackage = IAPPackages.ROOKIE;
                iapItemBtns[0].interactable = false;
                iapItemBtns[1].interactable = true;
                iapItemBtns[2].interactable = true;
            break;

            case 1:
                bullets[0].SetActive(false);
                bullets[1].SetActive(true);
                bullets[2].SetActive(false);
                iAPPackage = IAPPackages.STAR;
                iapItemBtns[0].interactable = true;
                iapItemBtns[1].interactable = false;
                iapItemBtns[2].interactable = true;
            break;

            case 2:
                bullets[0].SetActive(false);
                bullets[1].SetActive(false);
                bullets[2].SetActive(true);
                iAPPackage = IAPPackages.ICON;
                iapItemBtns[0].interactable = true;
                iapItemBtns[1].interactable = true;
                iapItemBtns[2].interactable = false;
            break;
        }
        SwitchTextColors(btnIndex);
    
        
    }

    void SwitchTextColors(int btnIndex){
        if(btnIndex.Equals(0)){
            for (int i = 0; i < rookieTexts.Length; i++)
            {
                rookieTexts[i].color = Color.white;
                starTexts[i].color = Color.black;
                iconTexts[i].color = Color.black;
                if(i == 2){
                    starTexts[i].color = Color.red;
                     iconTexts[i].color = Color.red;
                }
            }
        }
        else if(btnIndex.Equals(1)){
            for (int i = 0; i < starTexts.Length; i++)
            {
                rookieTexts[i].color = Color.black;
                starTexts[i].color = Color.white;
                iconTexts[i].color = Color.black;
                if(i == 2){
                    rookieTexts[i].color = Color.red;
                    iconTexts[i].color = Color.red;
                }
            }
        }
        else if(btnIndex.Equals(2)){
            for (int i = 0; i < iconTexts.Length; i++)
            {
                rookieTexts[i].color = Color.black;
                starTexts[i].color = Color.black;
                iconTexts[i].color = Color.white;
                if(i == 2){
                    rookieTexts[i].color = Color.red;
                    starTexts[i].color = Color.red;
                }
            }
        }
    }

  
    private void OnRestorePurchasesComplete(BillingServicesRestorePurchasesResult result, Error error)
    {
        if (error == null)
        {
            var     transactions    = result.Transactions;
            Debug.Log("Request to restore purchases finished successfully.");
            Debug.Log("Total restored products: " + transactions.Length);
            for (int iter = 0; iter < transactions.Length; iter++)
            {
                var     transaction = transactions[iter];
                Debug.Log(string.Format("[{0}]: {1}", iter, transaction.Payment.ProductId));
            }
        }
        else
        {
            Debug.Log("Request to restore purchases failed with error. Error: " +  error);
        }
    }

    private void OnTransactionStateChange(BillingServicesTransactionStateChangeResult result)
    {
        var     transactions    = result.Transactions;
        for (int iter = 0; iter < transactions.Length; iter++)
        {
            var     transaction = transactions[iter];
            switch (transaction.TransactionState)
            {
                case BillingTransactionState.Purchased:
                    if(transaction.ReceiptVerificationState == BillingReceiptVerificationState.Success){
                        TransactionSuccess(transaction.Payment.ProductPlatformId);
                        FirestoreDatabase.instance.userData.orderReceipts.Add(transaction.Receipt);
                        FirestoreDatabase.instance.PostOrders();
                    }
                    break;

                case BillingTransactionState.Failed:
                    Debug.LogError(string.Format("Buy product with id:{0} failed with error. Error: {1}", transaction.Payment.ProductId, transaction.Error));
                    AppCoreManager.instance.UiHandler.IAPLoadingPrompt.DeactivateIAPLoading();
                    break;
            }
        }
        }

    private void OnInitializeStoreComplete(BillingServicesInitializeStoreResult result, Error error)
    {
        if (error == null)
        {
            //TODO: Add Price Localization
            _storeProducts    = result.Products;
            Debug.Log("Store initialized successfully.");
            Debug.Log("Total products fetched: " + _storeProducts.Length);
            Debug.Log("Below are the available products:");
            SetupItemProducts();
        }
        else
        {
            Debug.Log("Store initialization failed with error. Error: " + error);
        }

        var     invalidIds  = result.InvalidProductIds;
        Debug.Log("Total invalid products: " + invalidIds.Length);
        if (invalidIds.Length > 0)
        {
            Debug.Log("Here are the invalid product ids:");
            for (int iter = 0; iter < invalidIds.Length; iter++)
            {
                Debug.Log(string.Format("[{0}]: {1}", iter, invalidIds[iter]));
            }
        }
    }

    private void SetupItemProducts(){
        for (int iter = 0; iter < _storeProducts.Length; iter++)
            {
               // iapItemBtns[iter].interactable = !BillingServices.IsProductPurchased(_storeProducts[iter]);
                var product = _storeProducts[iter];
                Debug.Log(string.Format("[{0}]: {1}", iter, product));
                SetItemPrice(_storeProducts[iter].PlatformId);
            }
    }

    public void PurchasePackage(){
            AppCoreManager.instance.UiHandler.IAPLoadingPrompt.SetIAPStatus("Processing Transaction...");
            if(!FirestoreDatabase.instance.userData.hasIAPDiscount){
                switch (iAPPackage)
                {
                    case IAPPackages.ROOKIE:
                        BuyItemById("com.punx.coupleai.rookiepack");
                    break;
                    case IAPPackages.STAR: 
                        BuyItemById("com.punx.coupleai.starpack");
                    break;
                    case IAPPackages.ICON: 
                        BuyItemById("com.punx.coupleai.iconpack");
                    break;
                    
                }
            }else{
                switch (iAPPackage)
                {
                    case IAPPackages.ROOKIE:
                        BuyItemById("com.punx.coupleai.rookiepack.discount10");
                    break;
                    case IAPPackages.STAR: 
                        BuyItemById("com.punx.coupleai.starpack.discount10");
                    break;
                    case IAPPackages.ICON: 
                        BuyItemById("com.punx.coupleai.iconpack.discount10");
                    break;
                    
                }
            }
        
    }

    private void BuyItemById(string id){
      
        if(BillingServices.CanMakePayments()){
             
            for (int i = 0; i < BillingServices.Products.Length; i++)
            {
                if(BillingServices.Products[i].PlatformId.Equals(id)){
                    Debug.Log($"Processing Transaction...");
                    BillingServices.BuyProduct(BillingServices.Products[i]);
                    
                 //   FirebaseAnalytics.LogEvent(BillingServices.Products[i].PlatformId);
                }
            }
        }
    }

    private void SetItemPrice(string id){
        for (int i = 0; i < BillingServices.Products.Length; i++)
            {
                if(!FirestoreDatabase.instance.userData.hasIAPDiscount){
                    if(BillingServices.Products[i].PlatformId.Equals(id)){
                        if(id.Equals("com.punx.coupleai.rookiepack")){
                            rookieTexts[2].text = BillingServices.Products[i].LocalizedPrice;
                        }else if(id.Equals("com.punx.coupleai.starpack")){
                            starTexts[2].text = BillingServices.Products[i].LocalizedPrice;
                        }
                        else if(id.Equals("com.punx.coupleai.iconpack")){
                            iconTexts[2].text = BillingServices.Products[i].LocalizedPrice;
                        }
                    }
                }else{
                    if(BillingServices.Products[i].PlatformId.Equals(id)){
                        if(id.Equals("com.punx.coupleai.rookiepack.discount10")){
                          rookieTexts[2].text = BillingServices.Products[i].LocalizedPrice;
                        }else if(id.Equals("com.punx.coupleai.starpack.discount10")){
                            starTexts[2].text = BillingServices.Products[i].LocalizedPrice;
                        }
                        else if(id.Equals("com.punx.coupleai.iconpack.discount10")){
                            iconTexts[2].text = BillingServices.Products[i].LocalizedPrice;
                        }
                    }
                }
            }
    }

    private void TransactionSuccess(string productID){
        
        Debug.Log($"Product ID: {productID}");
        if(!FirestoreDatabase.instance.userData.hasIAPDiscount){
            switch (productID)
            {
                case "com.punx.coupleai.rookiepack":
                    Debug.Log($"Rookie Package Purchased");
                    EventManager.OnPurchaseSuccess?.Invoke(IAPPackages.ROOKIE);
                // FirebaseAnalytics.LogEvent("package_rookie");
                    break;
                case "com.punx.coupleai.starpack":
                    Debug.Log($"Star Package Purchased");
                    EventManager.OnPurchaseSuccess?.Invoke(IAPPackages.STAR);
                //  FirebaseAnalytics.LogEvent(IAPPackages.STAR.ToString("package_star"));
                break;
                case "com.punx.coupleai.iconpack":
                    Debug.Log($"Icon Package Purchased");
                    EventManager.OnPurchaseSuccess?.Invoke(IAPPackages.ICON);
                //  FirebaseAnalytics.LogEvent("package_icon");
                break;
                default:
                    Debug.LogError($"Product ID not found!");
                break;
            }
        }else{
            switch (productID)
            {
                case "com.punx.coupleai.rookiepack.discount10":
                    Debug.Log($"Rookie Package Purchased");
                    EventManager.OnPurchaseSuccess?.Invoke(IAPPackages.ROOKIE);
                // FirebaseAnalytics.LogEvent("package_rookie");
                    break;
                case "com.punx.coupleai.starpack.discount10":
                    Debug.Log($"Star Package Purchased");
                    EventManager.OnPurchaseSuccess?.Invoke(IAPPackages.STAR);
                //  FirebaseAnalytics.LogEvent(IAPPackages.STAR.ToString("package_star"));
                break;
                case "com.punx.coupleai.iconpack.discount10":
                    Debug.Log($"Icon Package Purchased");
                    EventManager.OnPurchaseSuccess?.Invoke(IAPPackages.ICON);
                //  FirebaseAnalytics.LogEvent("package_icon");
                break;
                default:
                    Debug.LogError($"Product ID not found!");
                break;
            }
            FirestoreDatabase.instance.userData.hasIAPDiscount = false;
        }
        AppCoreManager.instance.UiHandler.IAPLoadingPrompt.DeactivateIAPLoading();
        IAPDialog.SetActive(false);
    }

    private void Restore(){
        BillingServices.RestorePurchases();
    }

   


}
