using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using System.ComponentModel;
using UnityEngine.Networking.Types;
using UnityEngine.Networking;
using static RestClientExample;
using PUNX.Models;
using System.Threading;
using VoxelBusters.EssentialKit;

public class ImagePicker : MonoBehaviour
{
    public CollectionType CollectionType;
    [SerializeField] private ImagesDataSO m_ImageData;
    [SerializeField] private List<GameObject> m_ListOfImages = new List<GameObject>();
    [SerializeField] private GameObject m_PrefabToSpawn;
    [SerializeField] private GameObject m_FullScreenPanel;
    [SerializeField] private GameObject m_SelectedImage;
    [SerializeField] private GameObject m_preparingPanel;
    [SerializeField] private Image m_FullScreenImage;
    [SerializeField] private Button m_FullScreenDelete;
    [SerializeField] private Button m_FullScreenDownload;
    [SerializeField] private Transform m_Content;
    [SerializeField] private GameObject m_saveNotif;
    [SerializeField] private LoadingHandler genLoadingHandler;
    private Texture2D texture;
    // Start is called before the first frame update
    void OnEnable()
    {
        switch (CollectionType)
        {
            case CollectionType.Collections:
                StartCoroutine(GetCollections());
            break;

            case CollectionType.NewGeneratedCollections:
               StartCoroutine(GetNewGeneratedImages());
            break;
        }
    }


    IEnumerator GetCollections()
    {
        if(m_Content.childCount ==m_ImageData.generatedImages.Count) yield break;
        m_preparingPanel.SetActive(true);
        foreach (Transform child in m_Content)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < m_ImageData.generatedImages.Count; i++)
        {
            yield return new WaitForSeconds(0.3f);
            GameObject spawnedPrefab = Instantiate(m_PrefabToSpawn) as GameObject;
            m_ListOfImages.Add(spawnedPrefab);
            spawnedPrefab.transform.SetParent(m_Content,false);
            
            if (m_ImageData != null && i < m_ImageData.generatedImages.Count)
            {
                string imageURL = m_ImageData.generatedImages[i];

                ImageHandler imageHandler = spawnedPrefab.GetComponent<ImageHandler>();
                if (imageHandler != null)
                {
                    imageHandler.SetImage(imageURL);
                }
                spawnedPrefab.GetComponent<Button>().onClick.AddListener(() =>
                {
                    SelectImage(imageURL);
                    m_SelectedImage = spawnedPrefab;
                });
            }
           if(i ==m_ImageData.generatedImages.Count -1){
            m_preparingPanel.SetActive(false);
           }
        }
    }

    public IEnumerator GetNewGeneratedImages(){
        foreach (Transform child in m_Content)
        {
            Destroy(child.gameObject);
        }
        EventManager.OnAddGenLoadingValue?.Invoke(10);
        yield return new WaitForSeconds(5);
        for (int i = 0; i < m_ImageData.newGeneratedImages.Count; i++)
        {
            yield return new WaitForSeconds(0.5f);
            GameObject spawnedPrefab = Instantiate(m_PrefabToSpawn) as GameObject;
            m_ListOfImages.Add(spawnedPrefab);
            spawnedPrefab.transform.SetParent(m_Content,false);
            
            if (m_ImageData != null && i < m_ImageData.newGeneratedImages.Count)
            {
                string imageURL = m_ImageData.newGeneratedImages[i];

                ImageHandler imageHandler = spawnedPrefab.GetComponent<ImageHandler>();
                if (imageHandler != null)
                {
                    imageHandler.SetImage(imageURL);
                }
                
                spawnedPrefab.GetComponent<Button>().onClick.AddListener(() =>
                {
                    SelectImage(imageURL);
                    m_SelectedImage = spawnedPrefab;
                });
            }

            if(i ==m_ImageData.newGeneratedImages.Count -1){
                EventManager.OnAddGenLoadingValue?.Invoke(10);
                genLoadingHandler.targetProgress=100;
            }
           
        }
    }
    public void SelectImage(string imageURL)
    {
        m_FullScreenPanel.SetActive(true);
        Davinci.get().load(imageURL).into(m_FullScreenImage).start();
        m_FullScreenDelete.GetComponent<Button>().onClick.AddListener(() =>
        {
            DeleteImage(m_SelectedImage);
        });
        m_FullScreenDownload.GetComponent<Button>().onClick.AddListener(() =>
        {
            DownloadImage(m_FullScreenImage);
        });
    }

    public void CloseImage()
    {
        m_FullScreenDelete.GetComponent<Button>().onClick.RemoveAllListeners();
        m_FullScreenDownload.GetComponent<Button>().onClick.RemoveAllListeners();
        m_FullScreenImage.sprite = null;
        m_FullScreenPanel.SetActive(false);
    }

    private void DownloadImage(Image image)
    {
        texture = m_FullScreenImage.mainTexture as Texture2D;
        if (texture != null) 
        {
            texture.SetPixels(texture.GetPixels());
            texture.Apply();
        }
        MediaServices.RequestGalleryAccess(GalleryAccessMode.ReadWrite, callback: (result, error) =>
        {
            MediaServices.SaveImageToGallery(texture, (result, error) =>
            {

                if (error == null)
                {
                    Debug.Log("Save image to gallery finished successfully.");
                    m_saveNotif.SetActive(true);
                }
                else
                {
                    Debug.Log("Save image to gallery failed with error. Error: " + error);
                }
            });
        });
        
    }

    public void DeleteImage(GameObject target)
    {
        m_ListOfImages.Remove(target); // Remove the object from the list.
        Destroy(target); // Destroy the GameObject.
    }

    public void ShareImage()
    {
        // Check the platform
        if (Application.platform == RuntimePlatform.Android)
        {
            ShareImageAndroid();
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            ShareImageIOS();
        }
        else
        {
            Debug.LogWarning("Sharing is not supported on this platform.");
        }
    }

    private void ShareImageAndroid()
    {
        texture = m_FullScreenImage.mainTexture as Texture2D;
        ShareSheet shareSheet = ShareSheet.CreateInstance();
        shareSheet.AddImage(texture);
        shareSheet.SetCompletionCallback((result, error) => {
            Debug.Log("Share Sheet was closed. Result code: " + result.ResultCode);
        });
        shareSheet.Show();
    }

    private void ShareImageIOS()
    {
        // Ensure the image file exists
        string imagePath = Application.persistentDataPath + "/" + this.m_SelectedImage;
        if (File.Exists(imagePath))
        {
            // Native iOS sharing code can be implemented using plugins or native code.
            // You would typically use a plugin like "SocialConnector" or write native code
            // to open the sharing options on iOS.
            // For more information on how to implement native sharing on iOS, consult the Unity documentation and iOS developer resources.
        }
        else
        {
            Debug.LogError("Image file does not exist: " + imagePath);
        }
    }

}
