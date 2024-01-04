using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHandler : MonoBehaviour
{
    [SerializeField]private GameObject m_generatingLoading;
    [SerializeField]private ErrorWarningHandler errorWarningHandler;
    [SerializeField]private GameObject maintenanceScreen;
    public IAPLoadingPrompt IAPLoadingPrompt;
    public GameObject[] screens;
    [SerializeField]private bool isGeneratingImages;

    void OnEnable()
    {
        EventManager.OnGeneratingAllImages += CompleteGenerating;
        EventManager.OnFetchedError += OnErrorDetected;
    }
    private void OnDisable() {
        EventManager.OnGeneratingAllImages -= CompleteGenerating;
        EventManager.OnFetchedError -= OnErrorDetected;
    }

    public void CompleteGenerating(){
        Debug.Log($"Complete Generate!");
        isGeneratingImages = false;
    }

    public void DisplayGenerateLoading(){
        m_generatingLoading.SetActive(true);
        StartCoroutine(GenLoadingRoutine());
    }

    /// <summary>
    /// Error Codes: 
    /// 200 - No Face Detected
    /// 300 - Multiple Face Detected
    /// 400 - API ERROR
    /// 500 - APP version not updated
    /// 600 - APP is Under Maintenance
    /// 700 - Not Enough App Akool Credit
    /// 800 - AkoolAPI Timeout
    /// </summary>
    /// <param name="statusCode"></param>
    
    public void OnErrorDetected(float statusCode){
      
        m_generatingLoading.SetActive(false);
        switch (statusCode)
        {
            case 200:
                errorWarningHandler.gameObject.SetActive(true);
                errorWarningHandler.SetErrorMessage(200,"Error","No face detected. Please try again with good lighting, ensuring your face is clearly visible.");
            break;

            case 300:
                errorWarningHandler.gameObject.SetActive(true);
                errorWarningHandler.SetErrorMessage(300,"Error","Multiple faces detected. Please retry with only one person's face in the frame.");
            break;

            case 400:
                Debug.Log($"Error 400");
                errorWarningHandler.gameObject.SetActive(true);
                errorWarningHandler.SetErrorMessage(400,"Error","An error occurred while processing your request. Please try again later.");
            break;

            case 500:
                errorWarningHandler.gameObject.SetActive(true);
                errorWarningHandler.SetErrorMessage(500,"Error","Your current app version is outdated. To continue using the app, please update to the latest version immediately.");
            break;
            case 600:
                maintenanceScreen.SetActive(true);
                errorWarningHandler.SetErrorMessage(600,"Error","Under Maintenance");
            break;
            case 700:
                errorWarningHandler.gameObject.SetActive(true);
                errorWarningHandler.SetErrorMessage(700,"Advisory","The app is currently undergoing an internal system upgrade. Please try again later.");
            break;
            case 800:
                errorWarningHandler.gameObject.SetActive(true);
                errorWarningHandler.SetErrorMessage(700,"Request Timeout","Sorry, we're having trouble processing your request. Please try again later.");
            break;

            default:
                Debug.LogError($"Status Code Not Found!");
            break;
        }
    }
    public IEnumerator GenLoadingRoutine(){
        isGeneratingImages = true;
        yield return new WaitUntil(()=> isGeneratingImages == false);
        ChangeScreen(5);
    }
    
    public void ChangeScreen(int index)
    {
        for (int i = 0; i < screens.Length; i++)
            screens[i].gameObject.SetActive(i == index);
    }

    public void OpenPrivacyPolicy(){
        Application.OpenURL("https://punx.ai/wp-content/uploads/2023/12/Privacy-Policy.pdf");
    }
}
