using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ErrorWarningHandler : MonoBehaviour
{
    public float statusCode;
    [SerializeField] private TextMeshProUGUI m_title;
    [SerializeField] private TextMeshProUGUI m_description;
    public void SetErrorMessage(float statCode,string title,string message){
        m_title.text = title;
        m_description.text = message;
        statusCode = statCode;
    }
    public void OkayBtnClicked(){
        
        if(statusCode.Equals(400)){ // External API Error
            gameObject.SetActive(false);
            Debug.Log($"Restart App!");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
           
        }
        if(statusCode.Equals(700)){ // External API Error
            Debug.Log($"Quit App!");
            Application.Quit();
            return; 
        }
        if(statusCode.Equals(500)){ // App Version not match!
            Debug.Log($"Quit App!");
            Application.Quit();
            return;
        }else{
            gameObject.SetActive(false);
        }
    }
}
