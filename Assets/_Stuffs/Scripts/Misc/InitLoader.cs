using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InitLoader : MonoBehaviour
{
    
    [SerializeField]private Slider loadingSlider;
    public Image logoLoading;
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
        targetProgress=0;
        // Initialize the slider value
        //loadingSlider.value = currentProgress;
        logoLoading.fillAmount = currentProgress;
    }

    private void Update()
    {
        if (currentProgress < targetProgress)
        {
            currentProgress += loadingSpeed * Time.deltaTime;
            //loadingSlider.value = currentProgress;
            logoLoading.fillAmount = currentProgress/100;
            if (logoLoading.fillAmount >= 1){
                    LoadingComplete();
                }
        }
    }

    public void AddLoadingValue(float value){
           targetProgress += value;
    }

    void LoadingComplete(){
        gameObject.SetActive(false);
        AppCoreManager.instance.InitLoad();
    }

}
