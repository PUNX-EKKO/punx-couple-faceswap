using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavedNotif : MonoBehaviour
{
    public float delayTime;

    void OnEnable()
    {
        if(gameObject.activeSelf){
            StartCoroutine(HideObject());
        }
    }
    public IEnumerator HideObject(){
        yield return new WaitForSeconds(delayTime);
        gameObject.SetActive(false);
    }
}
