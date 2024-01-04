using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UserIDSetter : MonoBehaviour
{
    [SerializeField]private TextMeshProUGUI idText;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitUntil(()=>FirestoreDatabase.instance.userData.uuid != null);
        idText.text = $"USER ID: {FirestoreDatabase.instance.userData.uuid}";
    }
}
