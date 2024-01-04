using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewImageData", menuName = "PUNX/ImagesData")]
public class ImagesDataSO : ScriptableObject
{

   public List<string> newGeneratedImages;
   public List<string> generatedImages;
   public List<string> sourceImages;

   [Header("===Male Target Images===")]
   public List<string> maleTargetImages;
   public List<string> maleFaceLandmarks;


   [Header("===Female Target Images===")]
   public List<string> femaleTargetImages;
   public List<string> femaleFaceLandmarks;
}
