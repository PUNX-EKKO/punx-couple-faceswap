using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class VoucherManager : MonoBehaviour
{
    [SerializeField]private TMP_InputField m_inputField;
    [SerializeField]private Button m_ireedemBtn;
    [SerializeField]private GameObject m_voucherWarning;
    [SerializeField]private GameObject m_successPrompt;
    [SerializeField]private TextMeshProUGUI m_warningMsg;
    [SerializeField]private IAPManager m_iapManager;
    public List<string> voucherUsers;
    public int voucherQuantity;
    private string voucherCodeUsed;
    private string warnMsg;



    void OnEnable()
    {
        EventManager.OnVoucherReedemed += OnVoucherRedeemed;
        EventManager.OnVoucherFailed += OnVoucherFailed;
        if(m_voucherWarning.activeSelf)m_voucherWarning.SetActive(false);
        m_inputField.text = "";
       
    }
    void OnDisable()
    {
        EventManager.OnVoucherReedemed -= OnVoucherRedeemed;
        EventManager.OnVoucherFailed -= OnVoucherFailed;
    }

 
    public void InputFieldupdate(){
       if(m_inputField.text != "") m_ireedemBtn.interactable=true;
       else m_ireedemBtn.interactable=false;
    }
    public void ReedemVoucher(){
        voucherUsers = new List<string>();
        voucherCodeUsed = m_inputField.text;
        warnMsg ="";
        FirestoreDatabase.instance.GetVouchersers(voucherCodeUsed);
        StartCoroutine(ResponseListener());
    }
    private void OnVoucherRedeemed(VoucherData voucherData){
        Debug.Log($"Redeemed: {voucherData.value}");
        warnMsg ="None";
        FirestoreDatabase.instance.VoucherClaimed(voucherCodeUsed);
      
    }
    private void OnVoucherFailed(string errorMsg){
        Debug.LogError($"Failed: {errorMsg}s");
        warnMsg = errorMsg;
    }

    public IEnumerator ResponseListener(){
        while (true)
        {
            yield return new WaitUntil(()=>warnMsg != "" );
            if(m_voucherWarning.activeSelf)m_voucherWarning.SetActive(false);
            if(warnMsg.Equals("None")){
                if(FirestoreDatabase.instance.userData.hasIAPDiscount){
                    m_iapManager.IAPDialog.SetActive(true);
                    m_iapManager.CheckSubscription();
                    Debug.Log($"DISCOUNTED PRICE ACTIVATED!");
                }
                m_successPrompt.SetActive(true);
                gameObject.SetActive(false);
                yield break;
            }
            m_warningMsg.text = warnMsg;
            m_voucherWarning.SetActive(true);
            yield return new WaitForSeconds(10);
            m_voucherWarning.SetActive(false);
            warnMsg="";
            yield break;
        }    
    }
}
