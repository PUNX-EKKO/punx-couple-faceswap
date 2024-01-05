using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{
    #region DELEGATES
    public delegate void NativeEvent(byte[] byteArray);
    public delegate void FirebaseEvent();
    public delegate void AWSEvent(string value);
    public delegate void AkoolEvent();
    public delegate void AkoolUrlEvent(string value);
    public delegate void AkoolFaceDataEvent(string value);
    public delegate void AppEvent(float value);
    public delegate void AppEventBool(bool value);
    public delegate void AppEventVoucher(VoucherData value);
    public delegate void AppEventVoucheError(string value);
    public delegate void AppEventIAP(IAPPackages packages);
    #endregion


    #region EVENT CALLBACKS

    public static NativeEvent OnPickedSourceImage;
    public static FirebaseEvent OnUserDataFetched;
    public static FirebaseEvent OnGeneradImagesFetched;
    public static FirebaseEvent OnSourceImagesFetched;
    public static FirebaseEvent OnTargetImagesFetched;
    public static FirebaseEvent OnGeneratingAllImages;
  
    public static AWSEvent OnSourceImagePosted;

    public static AkoolUrlEvent OnFaceSwapComplete;
    public static AkoolEvent OnFaceDetectionComplete;
    public static AppEvent OnAddLoadingValue;
    public static AppEvent OnAddGenLoadingValue;
    public static AppEventBool OnNewUserDetected;
    public static AppEventVoucher OnVoucherReedemed;
    public static AppEventVoucheError OnVoucherFailed;
    public static AppEvent OnFetchedError;
    public static AppEventIAP OnPurchaseSuccess;
    public static AkoolUrlEvent OnFaceDataFetched;
    
    public static AkoolFaceDataEvent OnMaleFaceDataFetched;
    public static AkoolFaceDataEvent OnFemaleFaceDataFetched;
    #endregion
}
