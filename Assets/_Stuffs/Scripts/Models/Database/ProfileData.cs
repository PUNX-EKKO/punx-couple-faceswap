using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;

[Serializable]
[FirestoreData]
public struct ProfileData 
{

    [FirestoreProperty]
    public string uuid {get; set;}
    [FirestoreProperty]
    public string deviceType {get; set;}
    [FirestoreProperty]
    public bool allowImageGeneration {get; set;}
    [FirestoreProperty]
    public int imageCredit {get; set;}

    public ProfileData(string uuid,string deviceType,bool allowImageGeneration, int imageCredit)
    {
        this.uuid = uuid;
        this.deviceType = deviceType;
        this.allowImageGeneration = allowImageGeneration;
        this.imageCredit = imageCredit;
    }
}
