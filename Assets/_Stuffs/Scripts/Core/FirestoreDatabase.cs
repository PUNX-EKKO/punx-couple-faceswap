using Firebase;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;
using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Unity.VisualScripting;
using System.Collections;
using System;
using PUNX.Core;
public class FirestoreDatabase : MonoBehaviour
{
    public static FirestoreDatabase instance;
    public UserProfileSO userData;
    public ImagesDataSO imageData;
    private ProfileData _firebaseProfileData;
    private AppSettingsData _appSettingsData;
    private GeneratedImageData _firebaseGenImageData;
    private FirebaseFirestore _database;
    private FirebaseAuth _auth;
    private CollectionReference collection;
    private AkoolFaceswapAPI akoolFaceswap;
    [SerializeField]private VoucherManager voucherManager;
    private string appVersion;
    private bool _hasMultipleVoucherUsage;
    public AppSettingsData AppSettingsData { get{return _appSettingsData;} }


    void Awake()
    {
        akoolFaceswap = FindObjectOfType<AkoolFaceswapAPI>();
        userData.uuid ="";
        if (instance == null)
        {
            instance = this;
        }
        userData.uuid = GetLastNCharacters(GetUserUniqueId(),6);
        userData.deviceType = Application.platform.ToString();
        userData.allowImageGeneration=false;
        userData.imageCredit=0;
        appVersion = Application.version;
       
       
    }

    void Start()
    {
        _auth = FirebaseAuth.DefaultInstance;
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
        var dependencyStatus = task.Result;
        if (dependencyStatus == Firebase.DependencyStatus.Available) {
            // Create and hold a reference to your FirebaseApp,
            // where app is a Firebase.FirebaseApp property of your application class.
                FirebaseApp app = FirebaseApp.DefaultInstance;
                _database = FirebaseFirestore.DefaultInstance;
                LoadAppData();
                LoadUserProfile();
                LoginEventLog();
       

               

            // Set a flag here to indicate whether Firebase is ready to use by your app.
        } else {
            UnityEngine.Debug.LogError(System.String.Format(
            "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            EventManager.OnFetchedError?.Invoke(400);
            // Firebase Unity SDK is not safe to use here.
        }
        });
       
       StartCoroutine(CheckAppStatus());    
       StartCoroutine(LoginAsGuest()); 
    }


    IEnumerator LoginAsGuest() {
    var authTask = _auth.SignInAnonymouslyAsync();

    yield return new WaitUntil(() => authTask.IsCompleted);

    if (authTask.Exception != null) {
        Debug.LogError("Failed to sign in anonymously: " + authTask.Exception);
    } else {
        Firebase.Auth.FirebaseUser user = _auth.CurrentUser;
        // Proceed with your logic after successful authentication
    }
}
    
    private string GetUserUniqueId(){
         if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            // Use the device's unique identifier (IMEI for Android, IDFV for iOS)
            string deviceID = SystemInfo.deviceUniqueIdentifier;
            return deviceID;
        }
        else
        {
            // For other platforms, generate a random unique ID
            string EditorID = SystemInfo.deviceUniqueIdentifier;
            return EditorID;
        }
    }
    private string GetLastNCharacters(string inputString, int n)
    {
        // Check if the input string is long enough
        if (inputString.Length >= n)
        {
            // Use Substring to get the last N characters
            return inputString.Substring(inputString.Length - n);
        }
        else
        {
            // Handle the case where the input string is shorter than N characters
            return inputString;
        }
    }



    /// <summary>
    /// Init loading user profile data
    /// </summary>
    private void LoadAppData(){
        GetTargetImages();
        GetOrders();
        GetAppSettings();
    }

    private async void LoadUserProfile()
    {
        CollectionReference collectionRef = _database.Collection(AppCoreManager.instance.ENVIRONMENT.ToString()).Document("Users").Collection(userData.uuid);
        QuerySnapshot snapshot = await collectionRef.Limit(1).GetSnapshotAsync();

        if (snapshot.Count > 0)
        {
            Debug.Log($"User exist. Loading profile data!");
            EventManager.OnAddLoadingValue?.Invoke(20);
            GetUserData();
        }
        else
        {
            Debug.Log($"User did not exist. Creating new profile!");
            EventManager.OnAddLoadingValue?.Invoke(20);
            PostUserData();
        }
    }

    private IEnumerator CheckAppStatus(){
        yield return new WaitUntil(()=>AppSettingsData.androidVersion != null && AppSettingsData.iosVersion != null);
        var status = AppSettingsData.appStatus.ToUpper();
       // akoolFaceswap.CheckAppCredits();
        if(status.Equals("MAINTENANCE")){
            Debug.LogError($"Under Maintenance");
            EventManager.OnFetchedError?.Invoke(600);
            yield break;
        }
        // if (Application.platform == RuntimePlatform.Android)
        // {
        //     if(appVersion !=AppSettingsData.androidVersion){
        //         EventManager.OnFetchedError?.Invoke(500);
        //     }
        // }
        // else if (Application.platform == RuntimePlatform.IPhonePlayer)
        // {
        //     if(appVersion !=AppSettingsData.iosVersion){
        //         EventManager.OnFetchedError?.Invoke(500);
        //     }
        // }
        // else
        // {
        //      // This code runs on a platform other than Android or iOS (e.g., Windows, macOS).
        //     if(appVersion !=AppSet tingsData.androidVersion){
        //             EventManager.OnFetchedError?.Invoke(500);
        //     }
        // }       

    }

    public void OnUserDataFetched(){
        Debug.Log("User Data Fetched!");
        EventManager.OnAddLoadingValue?.Invoke(30);
        DocumentReference  docRef = _database.Collection(AppCoreManager.instance.ENVIRONMENT.ToString()).Document("Users").Collection(userData.uuid).Document("GeneratedImageData");
            docRef.GetSnapshotAsync().ContinueWith(task =>
            {
                if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
                {
                    DocumentSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                    {
                        GetGeneratedImages(()=>{
                          Debug.Log("Generated Images Fetched!");
                          imageData.newGeneratedImages = new List<string>();
                          EventManager.OnAddLoadingValue?.Invoke(20);
                          EventManager.OnGeneradImagesFetched?.Invoke();
                        });
                    }else{
                        Debug.Log($"No Generated Image Data yet!");
                        imageData.newGeneratedImages = new List<string>();
                        imageData.generatedImages = new List<string>();
                        imageData.sourceImages = new List<string>();
                        EventManager.OnAddLoadingValue?.Invoke(20);
                    }
                }
        });
    }

    public void LoginEventLog()
    {
        Firebase.Analytics.FirebaseAnalytics
        .LogEvent(Firebase.Analytics.FirebaseAnalytics.EventLogin);
    }




    #region Put Methods
    public void PutSourceImages(string newData){
        DocumentReference  docRef = _database.Collection(AppCoreManager.instance.ENVIRONMENT.ToString()).Document("Users").Collection(userData.uuid).Document("SourceImageData");
            docRef.GetSnapshotAsync().ContinueWith(task =>
            {
                if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
                {
                    DocumentSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                    {
                        // The document exists.
                        Debug.Log("Update Source Image Url");
                        GetSourceImages(()=>{
                            PostSourceImageUrl(newData);
                        });
                    }
                    else
                    {
                        Debug.Log("Added new Source Image URL");
                        PostSourceImageUrl(newData);
                    }
                }
            });
            
    }

    public void PutGenerationStatus()
    {
        DocumentReference docRef = _database.Collection(AppCoreManager.instance.ENVIRONMENT.ToString()).Document("Users").Collection(userData.uuid).Document("UserData");
        _firebaseProfileData = new ProfileData(userData.uuid,userData.deviceType,
                                userData.allowImageGeneration,userData.imageCredit);
                                
        docRef.SetAsync(_firebaseProfileData).ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {

                Debug.Log($"Allow Generation Updated!");
            }else{
                Debug.LogError($"Failed to update Generation settings!");
            }
        });
    }

     public void PutGeneratedImages(){
        DocumentReference  docRef = _database.Collection(AppCoreManager.instance.ENVIRONMENT.ToString()).Document("Users").Collection(userData.uuid).Document("GeneratedImageData");
            docRef.GetSnapshotAsync().ContinueWith(task =>
            {
                if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
                {
                    DocumentSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                    {
                        // The document exists.
                        Debug.Log("Update Generated Image Url");
                        EventManager.OnGeneratingAllImages?.Invoke();
                        GetGeneratedImages(()=>{
                           PostGeneratedImageUrl();
                        });
                    }
                    else
                    {
                        Debug.Log("Added new Generated Image URL");
                        EventManager.OnGeneratingAllImages?.Invoke();
                        PostGeneratedImageUrl();
                    }
                }
            });
    }

    public void VoucherClaimed(string voucherCode){
        PostVoucherUsers(voucherCode);
        // DocumentReference  docRef = database.Collection("Admin").Document("Vouchers").Collection(voucherCode).Document("VoucherCredit");
        // var voucherData = new VoucherData(0,voucherManager.voucherQuantity);
                                
        // docRef.SetAsync(voucherData).ContinueWithOnMainThread(task => {
        //     if (task.IsCompleted)
        //     {

        //         Debug.Log($"Updated Voucher Data");
        //     }else{
        //         Debug.LogError($"Failed to update Generation settings!");
        //     }
        // });
            
    }

    #endregion
    #region Post Methods
    public void PostUserData()
    {
        DocumentReference docRef = _database.Collection(AppCoreManager.instance.ENVIRONMENT.ToString()).Document("Users").Collection(userData.uuid).Document("UserData");
        _firebaseProfileData = new ProfileData(userData.uuid,userData.deviceType,
                                userData.allowImageGeneration,userData.imageCredit);
                                
        docRef.SetAsync(_firebaseProfileData).ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                EventManager.OnUserDataFetched?.Invoke();
                EventManager.OnAddLoadingValue?.Invoke(40);
                Debug.Log($"New user profile posted!");
                OnUserDataFetched();
            }
        });
    }

    public void PostOrders()
    {
         Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "receipts", userData.orderReceipts}
        };
        
        DocumentReference docRef = _database.Collection(AppCoreManager.instance.ENVIRONMENT.ToString()).Document("Users").Collection(userData.uuid).Document("Orders");
        docRef.SetAsync(data).ContinueWith(task => {
            if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
            {
                Debug.Log("String array added to Firestore successfully.");
                
            }
            else
            {
                Debug.LogError("Failed to add string array to Firestore: " + task.Exception);
            }
        });
    }


    public void PostVoucherUsers(string voucherCode)
    {
        voucherManager.voucherUsers.Add(userData.uuid);
         Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "voucherUsers", voucherManager.voucherUsers}
        };
        
        DocumentReference docRef = _database.Collection("Admin").Document("Vouchers").Collection(voucherCode).Document("VoucherUsers");
        docRef.SetAsync(data).ContinueWith(task => {
            if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
            {
                Debug.Log("String array added to Firestore successfully.");
                
            }
            else
            {
                Debug.LogError("Failed to add string array to Firestore: " + task.Exception);
            }
        });
    }
    public void PostGeneratedImageUrl()
    {
        for (int i = 0; i < imageData.newGeneratedImages.Count; i++)
        {
            imageData.generatedImages.Add(imageData.newGeneratedImages[i]);
        }
         Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "images_url", imageData.generatedImages}
        };

        DocumentReference docRef = _database.Collection(AppCoreManager.instance.ENVIRONMENT.ToString()).Document("Users").Collection(userData.uuid).Document("GeneratedImageData");
        docRef.SetAsync(data).ContinueWith(task => {
            if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
            {
                Debug.Log("String array added to Firestore successfully.");
            }
            else
            {
                Debug.LogError("Failed to add string array to Firestore: " + task.Exception);
                EventManager.OnFetchedError?.Invoke(400);
            }
        });
    }
    public void PostSourceImageUrl(string newData)
    {
        imageData.sourceImages.Add(newData);
        Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "images_url", imageData.sourceImages}
        };

        DocumentReference docRef = _database.Collection(AppCoreManager.instance.ENVIRONMENT.ToString()).Document("Users").Collection(userData.uuid).Document("SourceImageData");
        docRef.SetAsync(data).ContinueWith(task => {
            if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
            {
                Debug.Log("Added new source image Url.");
            }
            else
            {
                Debug.LogError("Failed to add new source image: " + task.Exception);
                EventManager.OnFetchedError?.Invoke(400);
            }
        });
    }

    /// <summary>
    /// Post initial value for firestore target images.
    /// Note: This method must used for admin purposes only.
    /// </summary>
     public void PostTargetImageUrl()
    {
        
        Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "landmarks", imageData.sourceImages}
        };

        DocumentReference docRef = _database.Collection("Admin").Document("Resources").Collection("TargetImages").Document("Couple");
        docRef.SetAsync(data).ContinueWith(task => {
            if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
            {
                Debug.Log("String array added to Firestore successfully.");
            }
            else
            {
                Debug.LogError("Failed to add string array to Firestore: " + task.Exception);
                EventManager.OnFetchedError?.Invoke(400);
            }
        });
    }


    #endregion 

  

    #region Get Methods

      void GetAppSettings(){
        DocumentReference docRef = _database.Collection("Admin").Document("AppSettings");
        docRef.GetSnapshotAsync().ContinueWith(task => {
            if (task.IsCompleted)
            {
                DocumentSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    _appSettingsData= snapshot.ConvertTo<AppSettingsData>();
                    EventManager.OnAddLoadingValue?.Invoke(10);
                }
            }else{
                Debug.LogError($"Failed to fetch App settings");
            }
        });
    }
    public void GetVoucherData(string voucherCode){
        DocumentReference docRef = _database.Collection("Admin").Document("Vouchers").Collection(voucherCode).Document("VoucherCredit");
        docRef.GetSnapshotAsync().ContinueWith(task => {
            if (task.IsCompleted)
            {
                if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
                {
                    DocumentSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                    {
                        // The document exists.
                        var voucherData= snapshot.ConvertTo<VoucherData>();
                        voucherManager.voucherQuantity = voucherData.quantity;
                        Debug.Log($"Discount: {voucherData.discount}");
                        if(voucherData.discount >=1){
                            userData.hasIAPDiscount=true;
                        }else{
                            userData.hasIAPDiscount=false;
                            if(_hasMultipleVoucherUsage){
                                EventManager.OnVoucherFailed?.Invoke("You have already redeemed this voucher code. Please try entering a different code.");  
                                return;
                            }
                        }

                        if(voucherData.quantity <= voucherManager.voucherUsers.Count){
                            EventManager.OnVoucherFailed?.Invoke("This voucher has reached its maximum usage limit. Please try using a different code.");
                            return ;
                        }else{
                            EventManager.OnVoucherReedemed?.Invoke(voucherData);
                        } 
                    }
                    
                }
            }
        });
    }

    void GetUserData(){
        DocumentReference docRef = _database.Collection(AppCoreManager.instance.ENVIRONMENT.ToString()).Document("Users").Collection(userData.uuid).Document("UserData");
        docRef.GetSnapshotAsync().ContinueWith(task => {
            if (task.IsCompleted)
            {
                DocumentSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    _firebaseProfileData= snapshot.ConvertTo<ProfileData>();
                    userData.allowImageGeneration = _firebaseProfileData.allowImageGeneration;
                    userData.imageCredit = _firebaseProfileData.imageCredit;
                    EventManager.OnUserDataFetched?.Invoke();
                    OnUserDataFetched();
                }
            }
        });
    }

    public void VerifyUserCredentials(Action<ProfileData> data){
        DocumentReference docRef = _database.Collection(AppCoreManager.instance.ENVIRONMENT.ToString()).Document("Users").Collection(userData.uuid).Document("UserData");
        docRef.GetSnapshotAsync().ContinueWith(task => {
            if (task.IsCompleted)
            {
                DocumentSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    var firebaseProfileData= snapshot.ConvertTo<ProfileData>();
                    data?.Invoke(firebaseProfileData);
                }
            }
        });
    }
    void GetGeneratedImages(Action OnLoaded)
    {
        FetchGenImageUrl(imagesUrl=>{
            if (imagesUrl != null){imageData.generatedImages = imagesUrl;
             OnLoaded.Invoke();}
            else {Debug.LogWarning("User document not found or an error occurred.");}
        });
    }
    void GetSourceImages(Action OnLoaded)
    {
        FetchSourceImageUrl(imagesUrl=>{
            if (imagesUrl != null){imageData.sourceImages = imagesUrl;
            OnLoaded.Invoke();}
            else {Debug.LogWarning("User document not found or an error occurred.");}
        });
    }
    void GetTargetImages()
    {
        //Temp
        EventManager.OnAddLoadingValue?.Invoke(10);
         FetchTargetImageUrl(imagesUrl=>{
                if (imagesUrl != null){
                    imageData.coupleTargetImages = imagesUrl;
                    GetTargetImagesLandmarks();
                    EventManager.OnAddLoadingValue?.Invoke(10);
                }   
                else {
                    Debug.LogError($"Unable to Fetch Target Images. Please Try again later");
                    EventManager.OnFetchedError?.Invoke(400);
                    }
            });
       
            
        
    }
    void GetTargetImagesLandmarks()
    {
        FetchTargetImageLandmarks(landmark=>{
                if (landmark != null){imageData.coupleFaceLandmarks = landmark;
                    EventManager.OnTargetImagesFetched?.Invoke();}
                else {Debug.LogWarning("User document not found or an error occurred.");}
        });
    }


    public void GetOrders()
    {
        FetchOrders(orderReceipt=>{
            if (orderReceipt != null){userData.orderReceipts = orderReceipt;}
        });
    }
    public void GetVouchersers(string voucherCode)
    {
        FetchVoucherUsers(voucherCode,voucherUser=>{
            if (voucherUser != null){voucherManager.voucherUsers = voucherUser;
                string result = voucherManager.voucherUsers.Find(x => x == userData.uuid);
                
                if (result != null)
                {
                    _hasMultipleVoucherUsage =true;
                }else _hasMultipleVoucherUsage =false;

                 GetVoucherData(voucherCode);
            }
          
        });
    }

    #endregion
    #region  Fetch Methods
    public void FetchGenImageUrl(System.Action<List<string>> callback)
    {
        DocumentReference collectionRef = _database.Collection(AppCoreManager.instance.ENVIRONMENT.ToString()).Document("Users").Collection(userData.uuid).Document("GeneratedImageData");
        collectionRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
             if (task.IsCompleted)
                {
                    DocumentSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                    {
                        Dictionary<string, object> data = snapshot.ToDictionary();

                        if (data.ContainsKey("images_url"))
                        {
                            List<object> arrayData = (List<object>)data["images_url"];
                            List<string> stringArray = arrayData.Select(item => item.ToString()).ToList();
                            callback(stringArray);
                            
                        }
                    }
                }
        });
    }
    public void FetchSourceImageUrl(System.Action<List<string>> callback)
    {
        DocumentReference collectionRef = _database.Collection(AppCoreManager.instance.ENVIRONMENT.ToString()).Document("Users").Collection(userData.uuid).Document("SourceImageData");
        collectionRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
             if (task.IsCompleted)
                {
                    DocumentSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                    {
                        Dictionary<string, object> data = snapshot.ToDictionary();

                        if (data.ContainsKey("images_url"))
                        {
                            List<object> arrayData = (List<object>)data["images_url"];
                            List<string> stringArray = arrayData.Select(item => item.ToString()).ToList();
                            callback(stringArray);
                            
                        }
                    }
                }
        });
    }
    public void FetchTargetImageUrl(System.Action<List<string>> callback)
    {
        DocumentReference collectionRef = _database.Collection("Admin").Document("Resources").Collection("TargetImages").Document("Couple");
        collectionRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
             if (task.IsCompleted)
                {
                    DocumentSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                    {
                        Dictionary<string, object> data = snapshot.ToDictionary();
                        Debug.Log($"Fetch Target URL");
                        if (data.ContainsKey("images_url"))
                        {
                            Debug.Log($"images_url");
                            List<object> arrayData = (List<object>)data["images_url"];
                            List<string> stringArray = arrayData.Select(item => item.ToString()).ToList();
                            callback(stringArray);
                            
                        }
                    }
                }
        });
    }

    public void FetchTargetImageLandmarks(System.Action<List<string>> callback)
    {
        DocumentReference collectionRef = _database.Collection("Admin").Document("Resources").Collection("TargetImages").Document($"CoupleFaceLandmark");
        collectionRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
             if (task.IsCompleted)
                {
                    DocumentSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                    {
                        Dictionary<string, object> data = snapshot.ToDictionary();

                        if (data.ContainsKey("landmarks"))
                        {
                            List<object> arrayData = (List<object>)data["landmarks"];
                            List<string> stringArray = arrayData.Select(item => item.ToString()).ToList();
                            callback(stringArray);
                            
                        }
                    }
                }
        });

        
    }

     public void FetchOrders(System.Action<List<string>> callback)
    {
        DocumentReference collectionRef = _database.Collection(AppCoreManager.instance.ENVIRONMENT.ToString()).Document("Users").Collection(userData.uuid).Document("Orders");
        collectionRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
             if (task.IsCompleted)
                {
                    DocumentSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                    {
                        Dictionary<string, object> data = snapshot.ToDictionary();

                        if (data.ContainsKey("receipts"))
                        {
                            List<object> arrayData = (List<object>)data["receipts"];
                            List<string> stringArray = arrayData.Select(item => item.ToString()).ToList();
                            callback(stringArray);
                            
                        }
                    }
                }
        });
    }
    public void FetchVoucherUsers(string voucherCode,System.Action<List<string>> callback)
    {
        DocumentReference collectionRef = _database.Collection("Admin").Document("Vouchers").Collection(voucherCode).Document("VoucherUsers");
        collectionRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
             if (task.IsCompleted)
                {
                    DocumentSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                    {
                        Dictionary<string, object> data = snapshot.ToDictionary();

                        if (data.ContainsKey("voucherUsers"))
                        {
                            List<object> arrayData = (List<object>)data["voucherUsers"];
                            List<string> stringArray = arrayData.Select(item => item.ToString()).ToList();
                            callback(stringArray);
                            
                        }else{
                            callback(new List<string>());
                        }
                    }else
                    {
                        EventManager.OnVoucherFailed?.Invoke("The code you entered does not match any active vouchers in our system. Please double-check and try again.");  
                    }
                }
        });
    }

    internal void VerifyUserCredentials()
    {
        throw new NotImplementedException();
    }

    #endregion;


}
