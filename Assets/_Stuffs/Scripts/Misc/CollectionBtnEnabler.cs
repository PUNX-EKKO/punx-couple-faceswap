using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionBtnEnabler : MonoBehaviour
{
    [SerializeField]private Button m_collectionBtn;
    // Start is called before the first frame update

   void OnEnable()
   {
        if(FirestoreDatabase.instance.imageData.generatedImages.Count <=0 ){
            m_collectionBtn.interactable=false;
            return;
        }
        m_collectionBtn.interactable=true;
   }
}
