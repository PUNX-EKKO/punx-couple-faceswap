using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ImageHandler : MonoBehaviour
{
    public string imgUrl;
    public Image loadingImage;
    public Image imgSource;
    public FillMask fillMask;
    
    private float progressAmount;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D)) {
            Davinci.ClearAllCachedFiles();
        }
    }
    public void SetImage(string url){
        imgUrl= url;
        Davinci.get().load(url).into(imgSource)
                    .setCached(true)
                    .withStartAction(() =>
                    {
                        loadingImage.gameObject.SetActive(true);

                    })
                    .withErrorAction((error) =>
                    {

                    }).withDownloadProgressChangedAction((progress) =>
                    {
                        progressAmount = progress / 100f;
                        loadingImage.fillAmount = progressAmount;
                    })
                    .withDownloadedAction(() =>
                    {
                        loadingImage.gameObject.SetActive(false);
                        fillMask.FitMaskToChild();
                    }).start();
    }

 
}
