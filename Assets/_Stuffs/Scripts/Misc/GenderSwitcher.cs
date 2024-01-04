using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenderSwitcher : MonoBehaviour
{
    public Button maleBtn;
    public Button femaleBtn;
    public Button proceedBtn;
    public GameObject backBtn;
    private Gender gender;
    [SerializeField]private IAPManager m_iapManager;

    void OnEnable()
    {
        
        if(PlayerPrefs.HasKey("Gender")){
            backBtn.SetActive(true);
            if(PlayerPrefs.GetString("Gender") == "Male"){
               SwitchGender(true);
            }else{
               SwitchGender(false);
            }
        }else{
            SwitchGender(true);
            backBtn.SetActive(false);
        }
    }

    
    public void SwitchGender(bool isMale){
        if(isMale){
            gender = Gender.Male;
            maleBtn.interactable = false;
            femaleBtn.interactable = true;
        }else{
            gender = Gender.Female;
            femaleBtn.interactable = false;
            maleBtn.interactable = true;
        }
    }
    public void GenderProceed(){
        switch (gender)
        {
            case Gender.Male:
            AppCoreManager.instance.SwitchGender(true);
            break;
            case Gender.Female:
            AppCoreManager.instance.SwitchGender(false);
            break;
        }
        m_iapManager.CheckSubscription();
    }

}
