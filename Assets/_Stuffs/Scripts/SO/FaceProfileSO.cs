using System.Collections.Generic;
using UnityEngine;
using System;
[CreateAssetMenu(fileName = "NewProfile", menuName = "PUNX/FaceProfile")]
[Serializable]
public class FaceProfileSO : ScriptableObject
{
    public string faceID;
    public Vector2Int[] keypoints;
}
