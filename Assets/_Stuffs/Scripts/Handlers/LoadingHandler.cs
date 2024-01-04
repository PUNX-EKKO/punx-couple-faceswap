using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class LoadingHandler : MonoBehaviour
{
    public float targetProgress = 0;
    public float currentProgress = 0;
    public float loadingSpeed;
    [Header("Reference")]
    public Image fillingBar; // The bar that's filling
    public Image imageToChange; // The image to change
    public TextMeshProUGUI percentageText; // Text to display the current percentage using TextMeshPro
    public Sprite[] images; // List of images to display at different thresholds
    public GameObject parentObject;
    private int currentImageIndex = 0;
    private float currentThreshold;


    void OnEnable()
    {
        EventManager.OnAddGenLoadingValue += AddLoadingValue;
        fillingBar.fillAmount = 0;
        targetProgress=0;
        currentProgress=0;
        percentageText.text = "0%";
        currentImageIndex=0;
        currentThreshold=0;
        StartCoroutine(IncrementFillAmount());
    }

    void OnDisable()
    {
        EventManager.OnAddGenLoadingValue -= AddLoadingValue;
    }


    private IEnumerator IncrementFillAmount()
    {
        while (fillingBar.fillAmount < 1.0f)
        {

            if (currentProgress < targetProgress)
            {
                currentProgress += loadingSpeed * Time.deltaTime;
                fillingBar.fillAmount = currentProgress /100f;
                    if(fillingBar.fillAmount >= 1.0f){
                        LoadingComplete();
                    }
            }
            percentageText.text = currentProgress.ToString("F0") + "%";
            currentThreshold = Mathf.Floor(fillingBar.fillAmount * 4.0f) * 0.25f;

            if (currentThreshold > currentImageIndex * 0.25f)
            {
                currentImageIndex = Mathf.FloorToInt(currentThreshold * 4.0f);
                if (currentImageIndex < images.Length)
                {
                    imageToChange.sprite = images[currentImageIndex];
                }
            }

            yield return null;
        }

        // Ensure the text shows 100% when the filling is complete
        percentageText.text = "100%";
    }

    public void AddLoadingValue(float value){
           targetProgress += value;
    }

    void LoadingComplete(){
        Debug.Log($"Gen Loading Complete!");
        parentObject.SetActive(false);
    }
}
