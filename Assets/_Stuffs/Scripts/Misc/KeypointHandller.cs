using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;

public class KeypointHandller : MonoBehaviour
{
    public string imageUrl; // URL of the image you want to display

    public Vector2Int[] face1Keypoints; // Array of keypoints in original image pixel coordinates
    public Vector2Int[] face2Keypoints; // Array of keypoints in original image pixel coordinates
    public int indexToCrop;
    public int cropWidth;
    public int cropHeight;
    public Image userface1;
    public Image userface2;

    private Texture2D loadedTexture;
    public GameObject faceAssignUI;


    void OnEnable()
    {
        EventManager.OnFaceDataFetched += FaceDataFetched;
    }

  

    void OnDisable()
    {
        EventManager.OnFaceDataFetched -= FaceDataFetched;
    }


    private void FaceDataFetched(string url)
    {
        faceAssignUI.SetActive(true);
       StartCoroutine(LoadImageFromURL(url));
    }

    IEnumerator LoadImageFromURL(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            loadedTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
             Sprite sprite = Sprite.Create(loadedTexture, new Rect(0, 0, loadedTexture.width, loadedTexture.height), new Vector2(0.5f, 0.5f));
            userface1.sprite = sprite;
            userface2.sprite = sprite;
            CropFace1(face1Keypoints[indexToCrop]);
            CropFace2(face2Keypoints[indexToCrop]);
        }
        else
        {
            Debug.LogError("Failed to load image: " + www.error);
        }
    }


     public void CropFace1(Vector2Int centerPoint)
    {
        if (loadedTexture != null)
        {
            int x = centerPoint.x;
            int y = userface1.mainTexture.height - centerPoint.y; // Invert the y-axis to match Unity's coordinate system

            int startX = x - cropWidth / 2;
            int startY = y - cropHeight / 2;

            startX = Mathf.Clamp(startX, 0, loadedTexture.width - cropWidth);
            startY = Mathf.Clamp(startY, 0, loadedTexture.height - cropHeight);

            Color[] pixels = loadedTexture.GetPixels(startX, startY, cropWidth, cropHeight);
            Texture2D croppedTexture = new Texture2D(cropWidth, cropHeight);
            croppedTexture.SetPixels(pixels);
            croppedTexture.Apply();

            // Display the cropped texture
            DisplayCroppedFace1(croppedTexture);
        }
        else
        {
            Debug.LogError("No image loaded to crop.");
        }
    }
    public void CropFace2(Vector2Int centerPoint)
    {
        if (loadedTexture != null)
        {
            int x = centerPoint.x;
            int y = userface2.mainTexture.height - centerPoint.y; // Invert the y-axis to match Unity's coordinate system

            int startX = x - cropWidth / 2;
            int startY = y - cropHeight / 2;

            startX = Mathf.Clamp(startX, 0, loadedTexture.width - cropWidth);
            startY = Mathf.Clamp(startY, 0, loadedTexture.height - cropHeight);

            Color[] pixels = loadedTexture.GetPixels(startX, startY, cropWidth, cropHeight);
            Texture2D croppedTexture = new Texture2D(cropWidth, cropHeight);
            croppedTexture.SetPixels(pixels);
            croppedTexture.Apply();

            // Display the cropped texture
            DisplayCroppedFace2(croppedTexture);
        }
        else
        {
            Debug.LogError("No image loaded to crop.");
        }
    }

    // Function to display the cropped image in the RawImage component
    private void DisplayCroppedFace1(Texture2D croppedTexture)
    {
            Sprite sprite = Sprite.Create(croppedTexture, new Rect(0, 0, croppedTexture.width, croppedTexture.height), new Vector2(0.5f, 0.5f));
            userface1.sprite = sprite;
    }
    private void DisplayCroppedFace2(Texture2D croppedTexture)
    {
        Sprite sprite = Sprite.Create(croppedTexture, new Rect(0, 0, croppedTexture.width, croppedTexture.height), new Vector2(0.5f, 0.5f));
        userface2.sprite = sprite;
    }
}

public class FaceData{
    public string url;
    public Vector2Int[] faceKeypoints;

    public FaceData(string url, Vector2Int[] faceKeypoints)
    {
        this.url = url;
        this.faceKeypoints = faceKeypoints;
    }
}