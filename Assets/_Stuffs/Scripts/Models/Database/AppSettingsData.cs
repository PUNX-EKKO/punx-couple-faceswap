using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;

[Serializable]
[FirestoreData]
public struct AppSettingsData
{
    [FirestoreProperty]
    public string akoolToken {get; set;}
    [FirestoreProperty]
    public string appStatus {get; set;}
    [FirestoreProperty]
    public string androidVersion {get; set;}
    [FirestoreProperty]
    public bool enableVoucher { get; set; }
    [FirestoreProperty]
    public string iosVersion {get; set;}
    
    
}
