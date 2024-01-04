using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEditor;

public class VersionLoader : MonoBehaviour
{
    [SerializeField]private TextMeshProUGUI versionText;
    private string environmentCode;
    private int buildNumCode;
    void Start()
    {
        switch (AppCoreManager.instance.ENVIRONMENT)
        {
            case Environment.production:
                environmentCode="p";
            break;
            case Environment.staging:
                environmentCode="s";
            break;
            case Environment.dev:
                environmentCode="d";
            break;
            
        }
       // buildNumCode = PlayerSettings.Android.bundleVersionCode;
        versionText.text = $"v{Application.version}.{environmentCode}";
    }
}
