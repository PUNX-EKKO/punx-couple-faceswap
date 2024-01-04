using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IAPLoadingPrompt : MonoBehaviour
{
    [SerializeField]private TextMeshProUGUI loadingTxt;
    public void SetIAPStatus(string status){
        if(!gameObject.activeSelf) gameObject.SetActive(true);
        loadingTxt.text = status;
    }
    public void DeactivateIAPLoading(){
        gameObject.SetActive(false);
    }
}
