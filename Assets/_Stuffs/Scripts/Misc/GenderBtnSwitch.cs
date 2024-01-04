using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenderBtnSwitch : MonoBehaviour
{
    public Sprite[] genderSprites;
    public Button genderBtn;
    void OnEnable()
    {
        if(PlayerPrefs.HasKey("Gender")){
         
            if(PlayerPrefs.GetString("Gender") == "Male"){
               genderBtn.image.sprite = genderSprites[0];
            }else{
                genderBtn.image.sprite = genderSprites[1];
            }
        }
    }
}
