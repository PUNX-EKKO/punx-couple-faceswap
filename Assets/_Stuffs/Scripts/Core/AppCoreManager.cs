    using System.Collections;
using System.Collections.Generic;
using RestClient.Core.Models;
using UnityEngine;
using PUNX.Helpers;
using PUNX.Core;
using System;
using UnityEngine.SceneManagement;
using Models;
using Unity.VisualScripting.Antlr3.Runtime;
using Unity.VisualScripting;

public class AppCoreManager : PUNXWebRequest
{
    
    public static AppCoreManager instance;
    [SerializeField] private Environment m_environment;
    [SerializeField] private Gender m_gender;
    [SerializeField] private bool m_isRandomImage;
    private int m_noOfImageToGenerate;
    private int[] randomNumbers;
    private List<int> imageIndexToGenerate;
    protected byte[] _sourceImgArray;
    protected string _sourceImgUrl;
    protected float _creditStatus;

    [Header("Reference")]
    public UIHandler UiHandler;
    [SerializeField] ImagesDataSO imagesData;
    [SerializeField] private AkoolFaceswapAPI akool;

    public Environment ENVIRONMENT {get{return m_environment;}}
    public Gender GENDER {get{return m_gender;}}

    public int NoOfImageToGenerate { get{return m_noOfImageToGenerate;}}

    void Awake()
    {
       instance = this;
    }

 
    
    void OnEnable()
    {
        EventManager.OnPickedSourceImage += SourceImageSelected;
        EventManager.OnFaceDetectionComplete += FaceDetectionCompleted;
        EventManager.OnFaceSwapComplete += FaceSwapComplete;
        EventManager.OnPurchaseSuccess += ItemPurchaseSuccess;
        EventManager.OnVoucherReedemed += AddVoucherCredit;
    }
    void OnDisable()
    {
        EventManager.OnPickedSourceImage -= SourceImageSelected;
        EventManager.OnFaceDetectionComplete -= FaceDetectionCompleted;
        EventManager.OnFaceSwapComplete -= FaceSwapComplete;
        EventManager.OnPurchaseSuccess -= ItemPurchaseSuccess;
        EventManager.OnVoucherReedemed -= AddVoucherCredit;
    }

    

    private void ItemPurchaseSuccess(IAPPackages package){


        if(m_environment == Environment.production){
            switch (package)
            {
                case IAPPackages.ROOKIE:
                    m_noOfImageToGenerate = 15;
                break;
                case IAPPackages.STAR:
                    m_noOfImageToGenerate = 30;
                break;
                case IAPPackages.ICON:
                    m_noOfImageToGenerate = 45;
                break;
                
            }
        }
        else if(m_environment == Environment.staging ){
            switch (package)
            {
                case IAPPackages.ROOKIE:
                    m_noOfImageToGenerate = 3;
                break;
                case IAPPackages.STAR:
                    m_noOfImageToGenerate = 4;
                break;
                case IAPPackages.ICON:
                    m_noOfImageToGenerate = 5;
                break;
                
            }
        }else if(m_environment == Environment.dev){
             switch (package)
            {
                case IAPPackages.ROOKIE:
                    m_noOfImageToGenerate = 1;
                break;
                case IAPPackages.STAR:
                    m_noOfImageToGenerate = 2;
                break;
                case IAPPackages.ICON:
                    m_noOfImageToGenerate = 3;
                break;
                
            }
        }
        FirestoreDatabase.instance.userData.allowImageGeneration=true;
        FirestoreDatabase.instance.userData.imageCredit = NoOfImageToGenerate;
        FirestoreDatabase.instance.PostUserData();
        

    }

    public void AddVoucherCredit(VoucherData voucherData){
        if(!FirestoreDatabase.instance.userData.hasIAPDiscount){
            FirestoreDatabase.instance.userData.allowImageGeneration=true;
            FirestoreDatabase.instance.userData.imageCredit = voucherData.value;
            Debug.Log($"Added Voucher Credit: {voucherData.value}");
            FirestoreDatabase.instance.PostUserData();
        }
    }
    /// <summary>
    /// Event called when the user choosed an image as source image.
    /// </summary>
    /// <param name="imgArray"></param>
    private void SourceImageSelected(byte[] imgArray ){
        _sourceImgArray = imgArray;
    }

    /// <summary>
    /// Generate Button Action
    /// _creditStatus = 1 (allowImageGeneration is true)
    /// _creditStatus = 2 (allowImageGeneration is false) or (Zero image Credit)
    /// </summary>
    public void GenerateSwappedImage(){ 
        _creditStatus = 0;
        StartCoroutine(CheckCredentialsRoutine());
        FirestoreDatabase.instance.VerifyUserCredentials(loadedData => { // Check first if the account has a credit to generate images
            Debug.Log($"Credit Image: {loadedData.imageCredit}");

           if(loadedData.allowImageGeneration && loadedData.imageCredit > 0){
                _creditStatus = 1;
            }
            else if(loadedData.imageCredit <=0 || !loadedData.allowImageGeneration){ // Checking if the user have enough credit to generate or allowed to generate images
                _creditStatus = 2;
            }      
        });
      
    }

    private IEnumerator CheckCredentialsRoutine(){
        yield return new WaitUntil(()=>_creditStatus.Equals(1) || _creditStatus.Equals(2));
        if(_creditStatus.Equals(2)){
            EventManager.OnFetchedError?.Invoke(400);
            yield break;
        }
        PutSourceImage("dev",FirestoreDatabase.instance.userData.uuid,
                _sourceImgArray,OnSourceImgUploaded,(_sourceUrl)=>{
                _sourceImgUrl =_sourceUrl;
                });

    }

    
    
    void OnSourceImgUploaded(Response response)
    {
        EventManager.OnAddGenLoadingValue?.Invoke(10);
        if(response.StatusCode.Equals(200)){
            Debug.Log($"Success Upload!");
            EventManager.OnAddGenLoadingValue?.Invoke(10);
            EventManager.OnSourceImagePosted?.Invoke(_sourceImgUrl);
            FirestoreDatabase.instance.PutSourceImages(_sourceImgUrl);
        }
        else {Debug.LogError($"Upload Failed! Please Try Again Later");
            EventManager.OnFetchedError?.Invoke(400);
        }
    }

    void FaceDetectionCompleted(){
        Debug.Log($"Complete Face Detection");
        EventManager.OnAddGenLoadingValue?.Invoke(10);
        SetImageIndexToGenerate();
        imagesData.newGeneratedImages = new List<string>();
        FirestoreDatabase.instance.userData.allowImageGeneration=false;
        FirestoreDatabase.instance.userData.imageCredit=0;
        FirestoreDatabase.instance.PutGenerationStatus();
        StartCoroutine(InitTargetImageSetupRoutine());
    }

    private IEnumerator InitTargetImageSetupRoutine(){
        yield return new WaitUntil(()=> imageIndexToGenerate.Count>=1);
        SetTargetImage();
    }

    /// <summary>
    /// Initial Image to Generate
    /// </summary>
    private void SetTargetImage(){
        if(imageIndexToGenerate.Count<=0){
            FirestoreDatabase.instance.PutGeneratedImages();
            return;} // Check if all index are generated
        switch (m_gender)
        {
            case Gender.Male:
                akool.SetupCoupleTargetImage(imageIndexToGenerate[0],OnFaceSwapReady);
            break; 
        }
    }
    float percentageValPerItem;
    

    public void OnFaceSwapReady(){
        akool.GenerateFaceSwapImage();
    }
    private void FaceSwapComplete(string url){
        EventManager.OnAddGenLoadingValue?.Invoke(percentageValPerItem);
        imagesData.newGeneratedImages.Add(url);
        imageIndexToGenerate.RemoveAt(0);
        SetTargetImage();
    }



    /// <summary>
    ///  Set of image index that akool will get as target images
    /// </summary>
    private void SetImageIndexToGenerate(){
        if(!m_isRandomImage){
            for (int i = 0; i < GetTargetImageCount()+1; i++)
            {
                imageIndexToGenerate.Add(i);
            }
        }
        else ShuffleImageIndex();
        CalculateLoadValue();
    }

    private void ShuffleImageIndex(){
        randomNumbers = new int[GetTargetImageCount() + 1];
        for (int i = 0; i < randomNumbers.Length; i++)
        {
            randomNumbers[i] = 0 + i;
        }
        ShuffleArray(randomNumbers);
        imageIndexToGenerate = new List<int>();
        for (int i = 0; i < FirestoreDatabase.instance.userData.imageCredit; i++)
        {
            int uniqueRandomNumber = randomNumbers[i];
            imageIndexToGenerate.Add(uniqueRandomNumber);
        }
    }

    void ShuffleArray(int[] arr)
    {
        for (int i = arr.Length - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            int temp = arr[i];
            arr[i] = arr[j];
            arr[j] = temp;
        }
    }
    /// <summary>
    /// Current Generationg Percentage Destribution 10,10,10,50,10,10
    /// </summary>
    private void CalculateLoadValue(){
        percentageValPerItem = 50 / imageIndexToGenerate.Count;
    }
    private int GetTargetImageCount(){
         return imagesData.coupleTargetImages.Count - 1;
    }

    public void SwitchGender(bool isMale){
        if(isMale) m_gender = Gender.Male;
        else  m_gender = Gender.Female;
        PlayerPrefs.SetString("Gender",m_gender.ToString());
    } 
    public void InitLoad(){
        akool.CheckAppCredits();
        UiHandler.ChangeScreen(2);

    }


}
