using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
[FirestoreData]
public struct GeneratedImageData 
{

    [FirestoreProperty] public string id {get; set;}
    [FirestoreProperty] public string imageUrl {get; set;}
    [FirestoreProperty] public string createdAt {get; set;}

    public GeneratedImageData(string id, string imageUrl, string createdAt)
    {
        this.id = id;
        this.imageUrl = imageUrl;
        this.createdAt = createdAt;
    }
    
}
