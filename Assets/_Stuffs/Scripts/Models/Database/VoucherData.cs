using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;

[Serializable]
[FirestoreData]
public struct VoucherData 

{
    [FirestoreProperty]
    public int value {get; set;}
    [FirestoreProperty]
    public int quantity {get; set;}
    [FirestoreProperty]
    public int discount {get; set;}
 
}
