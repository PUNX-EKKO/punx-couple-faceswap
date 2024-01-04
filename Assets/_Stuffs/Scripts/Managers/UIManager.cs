using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private GameObject m_LoginUI;
    [SerializeField] private GameObject m_RegisterUI;
    [SerializeField] private GameObject m_LogoutUI;


    private void Awake()
    {
        if( Instance == null)
        {
            Instance = this;
        }
        else if( Instance != null)
        {
            Debug.Log("Instance already exists");
            Destroy(this);
        }
    }
    
    public void LoginScreen()
    {
        m_LoginUI.SetActive(true);
        m_RegisterUI.SetActive(false);
        m_LogoutUI.SetActive(false);
    }

    public void RegisterScreen()
    {
        m_LoginUI.SetActive(false);
        m_RegisterUI.SetActive(true);
    }

    public void LogoutScreen()
    {
        m_LoginUI.SetActive(false);
        m_LogoutUI.SetActive(true);
    }

    
}
