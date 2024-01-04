using System.Collections;
using System.Collections.Generic;
using UnityEngine;
﻿using System;
using UnityEngine.UI;
using UnityEngine.Networking;
using Firebase;
using Firebase.Extensions;
using Firebase.Storage;

public class FirebaseImgLoader : MonoBehaviour
{   
    [SerializeField]private string m_userId;
    [SerializeField]private List<string> m_maleImageIds;
    [SerializeField]private List<string> m_femaleImageIds;

    [SerializeField]private List<string> m_maleImageUrl;
    [SerializeField]private List<string> m_femaleImageUrl;

     [SerializeField]private string m_sourceUrl;

    FirebaseStorage storage;
    StorageReference storageReference;
    public string SourceUrl(){return m_sourceUrl;}
    public List<string> MaleImageUrl(){return m_maleImageUrl;}

    IEnumerator LoadImage(string MediaUrl){
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl); //Create a request
        yield return request.SendWebRequest(); //Wait for the request to complete
        if(request.isNetworkError || request. isHttpError){
            Debug.Log(request.error);
        }
        else{
            // rawImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture; 
            // setting the loaded image to our object
        }
    }

    // Start is called before the first frame update
    void Start()
    {    
        m_userId = SystemInfo.deviceUniqueIdentifier;
        //initialize storage reference
        storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl("gs://k-idol-2d13b.appspot.com/");
      //  StartCoroutine(GetMaleIdolsImageUrl());
        StartCoroutine(GetSourceImage());
    }



    private IEnumerator GetMaleIdolsImageUrl(){
        for (int i = 0; i < m_maleImageIds.Count; i++)
        {
            //get reference of image
            StorageReference image = storageReference.Child($"target_images/male/{m_maleImageIds[i]}.jpg");
            //Get the download link of file
            yield return image.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
            {
                if(!task.IsFaulted && !task.IsCanceled){
                    Debug.Log($"URL: {task.Result}");
                    m_maleImageUrl.Add(Convert.ToString(task.Result));
                }
                else{
                    Debug.Log(task.Exception);
                }
            });
        }
        yield return new WaitUntil(()=>m_maleImageUrl.Count ==  m_maleImageIds.Count);
        Debug.Log($"URL Fetching Complete!");
    }
    private IEnumerator GetFemaleIdolsImageUrl(){
        for (int i = 0; i < m_femaleImageIds.Count; i++)
        {
            //get reference of image
            StorageReference image = storageReference.Child($"target_images/female/{m_femaleImageIds[i]}.jpg");
            //Get the download link of file
            yield return image.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
            {
                if(!task.IsFaulted && !task.IsCanceled){
                    Debug.Log($"URL: {task.Result}");
                    m_femaleImageUrl.Add(Convert.ToString(task.Result));
                }
                else{
                    Debug.LogError(task.Exception);
                }
            });
        }
        yield return new WaitUntil(()=>m_femaleImageUrl.Count ==  m_femaleImageIds.Count);
        Debug.Log($"URL Fetching Complete!");
    }


    private IEnumerator GetSourceImage(){
            //get reference of image
            StorageReference image = storageReference.Child($"{m_userId}/source_image/profile.jpeg");
            //Get the download link of file
            yield return image.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
            {
                if(!task.IsFaulted && !task.IsCanceled){
                    Debug.Log($"URL: {task.Result}");
                    m_sourceUrl = Convert.ToString(task.Result);
                }
                else{
                    Debug.LogError(task.Exception);
                }
            });

    }
     IEnumerator DownloadFileFromFirebaseStorage(string url)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to download file: " + www.error);
            }
            else
            {
                 Debug.LogError("Download file: " + www.url);
                // File has been successfully downloaded
                byte[] data = www.downloadHandler.data;
                // You can now use 'data' to save or process the downloaded file as needed.
            }
        }
    }
}




