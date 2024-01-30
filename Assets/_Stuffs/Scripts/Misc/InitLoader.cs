using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InitLoader : MonoBehaviour
{
    [SerializeField] private Slider loadingSlider;
    public Image logoLoading;
    public TMP_Text percentageText; // Reference to TextMeshPro component
    public float targetProgress;
    public float currentProgress;
    public float loadingSpeed;

    void OnEnable()
    {
        EventManager.OnAddLoadingValue += AddLoadingValue;
    }

    void OnDisable()
    {
        EventManager.OnAddLoadingValue -= AddLoadingValue;
    }

    private void Start()
    {
        targetProgress = 0;
        logoLoading.fillAmount = currentProgress;
    }

    private void Update()
    {
        if (currentProgress < targetProgress)
        {
            currentProgress += loadingSpeed * Time.deltaTime;
            logoLoading.fillAmount = currentProgress / 100;
            UpdatePercentageText(); // Update the TextMeshPro text
            if (logoLoading.fillAmount >= 1)
            {
                LoadingComplete();
            }
        }
    }

    public void AddLoadingValue(float value)
    {
        targetProgress += value;
    }

    void LoadingComplete()
    {
        gameObject.SetActive(false);
        AppCoreManager.instance.InitLoad();
    }

    void UpdatePercentageText()
    {
        // Display the rounded percentage in TextMeshPro text
        int roundedPercentage = Mathf.RoundToInt(currentProgress);
        percentageText.text = "Loading: " + roundedPercentage + "%";
    }
}
