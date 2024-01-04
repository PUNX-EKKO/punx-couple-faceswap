using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
[CreateAssetMenu(fileName = "NewProfile", menuName = "PUNX/UserProfile")]
[Serializable]
public class UserProfileSO : ScriptableObject
{
    public string uuid; 
    public string deviceType; 
    public string akoolToken; 
    public bool allowImageGeneration;
    public bool hasIAPDiscount;
    public int imageCredit;
    public List<string> orderReceipts;
}
