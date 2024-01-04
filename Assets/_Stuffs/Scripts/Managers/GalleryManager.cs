using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using PUNX.Helpers;
using RestClient.Core.Models;
using VoxelBusters.EssentialKit;

public class GalleryManager : PUNXWebRequest
{
    public static GalleryManager instance;

    private GalleryAccessStatus _readAccessStatus;
    private GalleryAccessStatus _readWriteAccessStatus;
    public static Texture2D currentImage;
    [Header("Reference")]
    [SerializeField] private Button m_initSelectBtn;
    [SerializeField] private Button m_mainSelectBtn;
    [SerializeField] private GameObject m_sefieInstructions;
    [SerializeField] private Button m_generateBtn;
    [SerializeField] private RawImage m_Image;

    private void Start()
    {
        instance = this;
    }

    public void GetImagesFromGallery()
    {
        _readAccessStatus = MediaServices.GetGalleryAccessStatus(GalleryAccessMode.Read);

        MediaServices.RequestGalleryAccess(GalleryAccessMode.Read, callback: (result, error) =>
        {
            Debug.Log("Request for gallery access finished.");
            Debug.Log("Gallery access status: " + result.AccessStatus);
        });
        if (_readAccessStatus == GalleryAccessStatus.Authorized)
        {
            MediaServices.SelectImageFromGallery(canEdit: true, (textureData, error) =>
            {
                if (error == null)
                {
                    m_initSelectBtn.gameObject.SetActive(false);
                    m_sefieInstructions.SetActive(false);
                    m_mainSelectBtn.gameObject.SetActive(true);
                    m_generateBtn.interactable = true;
                    currentImage = textureData.GetTexture(); // This returns the texture
                    byte[] imgArray = currentImage.EncodeToJPG();
                    EventManager.OnPickedSourceImage?.Invoke(imgArray);
                    Debug.Log("Select image from gallery finished successfully.");
                    m_Image.texture = currentImage;
                    m_Image.GetComponent<AspectRatioFitter>().aspectRatio = (float)currentImage.width / currentImage.height;
                }
                else
                {
                    Debug.Log("Select image from gallery failed with error. Error: " + error);
                    EventManager.OnFetchedError?.Invoke(400);
                }
            });
        }
    }

    public void SaveImage()
    {
        _readWriteAccessStatus = MediaServices.GetGalleryAccessStatus(GalleryAccessMode.ReadWrite);

        if(_readWriteAccessStatus == GalleryAccessStatus.NotDetermined)
        {
            MediaServices.RequestGalleryAccess(GalleryAccessMode.ReadWrite, callback: (result, error) =>
            {
                Debug.Log("Request for gallery access finished.");
                Debug.Log("Gallery access status: " + result.AccessStatus);
            });
        }

        if(_readAccessStatus == GalleryAccessStatus.Authorized)
        {
            MediaServices.SaveImageToGallery(currentImage, (result, error) =>
            {
                if (error == null)
                {
                    Debug.Log("Save image to gallery finished successfully.");
                }
                else
                {
                    Debug.Log("Save image to gallery failed with error. Error: " + error);
                }
            });
        }
    }

    public void OpenCollections(){
         SceneManager.LoadScene("Gallery");
    }

}
