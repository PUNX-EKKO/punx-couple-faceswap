using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//For Picking files
using System.IO;
using System;
using SimpleFileBrowser;


//For firebase storage
using Firebase;
using Firebase.Extensions;
using Firebase.Storage;
using PUNX.Helpers;
using RestClient.Core.Models;

public class AwsImageUploader : PUNXWebRequest
{
[SerializeField]private string m_userId;
    FirebaseStorage storage;
    StorageReference storageReference;
    // Start is called before the first frame update
    void Start()
    {
        m_userId = SystemInfo.deviceUniqueIdentifier;
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".jpg", ".png"), new FileBrowser.Filter("Text Files", ".txt", ".pdf"));

        FileBrowser.SetDefaultFilter(".jpg");

        
        FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");
        storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl("gs://k-idol-2d13b.appspot.com/");


    }

    public void UploadImage(){
        StartCoroutine(ShowLoadDialogCoroutine());

    }

    IEnumerator ShowLoadDialogCoroutine()
    {
       
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, true, null, null, "Load Files and Folders", "Load");

        Debug.Log(FileBrowser.Success);

        if (FileBrowser.Success)
        {
            // Print paths of the selected files (FileBrowser.Result) (null, if FileBrowser.Success is false)
            for (int i = 0; i < FileBrowser.Result.Length; i++)
                Debug.Log(FileBrowser.Result[i]);

            Debug.Log("File Selected");
            byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(FileBrowser.Result[0]);
            //Editing Metadata
            var newMetadata = new MetadataChange();
            newMetadata.ContentType = "image/jpeg";

            //Create a reference to where the file needs to be uploaded
            // StorageReference uploadRef = storageReference.Child($"{m_userId}/source_image/profile.jpeg");
            // Debug.Log("File upload started");
            // uploadRef.PutBytesAsync(bytes, newMetadata).ContinueWithOnMainThread((task) => { 
            //     if(task.IsFaulted || task.IsCanceled){
            //         Debug.Log(task.Exception.ToString());
            //     }
            //     else{
            //         Debug.Log("File Uploaded Successfully!");
            //     }
            // });


        }
    }

     void OnCompleteUpoad(Response response)
    {
            Debug.Log($"Error: {response.Error}");
            Debug.Log($"Data: {response.Data}");
            Debug.Log($"Success Upload!");
    }
   
}