using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RestClient.Core;
using RestClient.Core.Models;
using PUNX.Models;
using PUNX.Helpers;
using PUNX.Models.Response;
using Newtonsoft.Json;
namespace PUNX.Core{
    public class AkoolFaceswapAPI : PUNXWebRequest
    {
        
        [SerializeField]private string m_baseURL;
        [SerializeField]private ImageFaceSwap m_imageFaceSwap;
        [SerializeField]private FaceDetect m_faceDetect;
        private List<string> _face1Landmarks;
        private List<string> _face2Landmarks;
        

        [Space]
        [Header("Response")]

        [SerializeField]private CreditInfo m_creditInfo;
        [SerializeField]private ImageFaceswapResponse m_imageFaceSwapResponse; 
        [SerializeField]private FaceDetectResponse faceDetectResponse;

        [Space]
        [Header("Reference")]
        public ImagesDataSO _imagesData;

        void OnEnable()
        {
            EventManager.OnSourceImagePosted += OnSourceImageUploaded;
        }
        void OnDisable()
        {
            EventManager.OnSourceImagePosted -= OnSourceImageUploaded;
        }


 
        
        public void GenerateFaceSwapImage(){
            PostFaceSwap(m_baseURL,FirestoreDatabase.instance.AppSettingsData.akoolToken,m_imageFaceSwap,OnFaceSwapComplete);
        }
        public void CheckAppCredits(){
            Debug.Log($"Get App Account Credits ");
            GetCreditInfo(FirestoreDatabase.instance.AppSettingsData.akoolToken,OnCreditFetched);
        }
        /// <summary>
        /// This method called after the source image is uploaded to aws s3.
        /// </summary>
        /// <param name="url"></param>
        private void OnSourceImageUploaded(string url){
            m_faceDetect.single_face=false;
            m_faceDetect.image_url = url;
             Debug.Log($"Source Image URL: {m_faceDetect.image_url}");
            PostFaceDetect(FirestoreDatabase.instance.AppSettingsData.akoolToken,m_faceDetect,OnFaceDetectComplete);
        }
        void OnCreditFetched(Response response)
        {
             Debug.Log($"Credit: {response.Data}");
            if(response.StatusCode.Equals(200)){
               m_creditInfo = JsonConvert.DeserializeObject<CreditInfo>(response.Data.ToString());
               if(m_creditInfo.data.credit < 60){
                    EventManager.OnFetchedError?.Invoke(700);
               }
            }else{
                Debug.LogError($"Face Detection Failed!");
                EventManager.OnFetchedError?.Invoke(400);
            }
        }


        void OnFaceDetectComplete(Response response)
        {
           // Debug.Log($"Data: {response.Data}");
            Debug.Log($"Status: {response.StatusCode}");
            if(response.StatusCode.Equals(200)){
               faceDetectResponse = JsonConvert.DeserializeObject<FaceDetectResponse>(response.Data.ToString());
               Debug.Log($"Face Data: {response.Data.ToString()}");
                StartCoroutine(SetupSourceImage());
            }else{
                Debug.LogError($"Face Detection Failed!");
                EventManager.OnFetchedError?.Invoke(400);
            }
            
        }

        void OnFaceSwapComplete(Response response)
        {
            Debug.Log($"Error: {response.Error}");
            Debug.Log($"Data: {response.Data}"); 
            if(response.Error != null){
                EventManager.OnFetchedError?.Invoke(800);
                return;
            }
             m_imageFaceSwapResponse = JsonUtility.FromJson<ImageFaceswapResponse>(response.Data);
            if(m_imageFaceSwapResponse.code.Equals(1000)){
                EventManager.OnFaceSwapComplete?.Invoke(m_imageFaceSwapResponse.data.url);
            }else{
                Debug.LogError($"Face Detection Failed!");
                EventManager.OnFetchedError?.Invoke(400);
            }
            
        }


        /// <summary>
        /// Note: This face landmark detection can only handle 1 face
        /// Error Codes: 
        /// 200 - No Face Detected
        /// 300 - Multiple Face Detected
        /// </summary>
        private IEnumerator SetupSourceImage(){

                //TODO; Add error if there is no face datected!
                _face1Landmarks = new List<string>();
                _face2Landmarks = new List<string>();
                // Access the landmarks array
                List<List<List<int>>> landmarks = faceDetectResponse.landmarks;
                try
                {
                    if(landmarks.Count < 1){
                    EventManager.OnFetchedError?.Invoke(200);
                    yield break;
                    }else if (landmarks.Count >=3) {
                        EventManager.OnFetchedError?.Invoke(300);
                        yield break;
                    }
                }
                catch (System.NullReferenceException)
                {
                    EventManager.OnFetchedError?.Invoke(200);
                    throw;
                }
                
                Debug.Log($"Generate Image!");
                for (int i = 0; i < 4; i++) //Add first 4 landmark
                {
                    // Access specific landmarks
                    List<int> landmark1 = landmarks[0][i]; // Accessing the first landmark. Note: Change the 2nd array value to change the landmark value
                    // Accessing individual values
                    int x1 = landmark1[0]; 
                    int y1 = landmark1[1]; 
                    _face1Landmarks.Add($"{x1},{y1}");
                }
                for (int i = 0; i < 4; i++)
                {
                    
                     // Access specific landmarks
                    List<int> landmark1 = landmarks[1][i]; // Accessing the first landmark. Note: Change the 2nd array value to change the landmark value
                    // Accessing individual values
                    int x1 = landmark1[0]; 
                    int y1 = landmark1[1]; 
                    Debug.Log($"Landmarks: {x1},{y1}");
                    _face2Landmarks.Add($"{x1},{y1}");
                }
               yield return new WaitUntil(( )=> _face1Landmarks.Count.Equals(4) && _face2Landmarks.Count.Equals(4));
               m_imageFaceSwap.sourceImage = new List<SourceImage>();
               m_imageFaceSwap.sourceImage.Add(new SourceImage(m_faceDetect.image_url, 
                $"{_face1Landmarks[0]}:{_face1Landmarks[1]}:{_face1Landmarks[2]}:{_face1Landmarks[3]}")); // Setup face 1 keypoint values

                 m_imageFaceSwap.sourceImage.Add(new SourceImage(m_faceDetect.image_url, 
                $"{_face2Landmarks[0]}:{_face2Landmarks[1]}:{_face2Landmarks[2]}:{_face2Landmarks[3]}")); // Setup face 2 keypoint values
                Debug.Log($"Face Detection Complete!");
             //   EventManager.OnFaceDetectionComplete?.Invoke();

        }

        public void SetupMaleTargetImage(int imgIndex, Action OnSetupDone){
            m_imageFaceSwap.targetImage = new List<TargetImage>();
            m_imageFaceSwap.targetImage.Add(new TargetImage(_imagesData.maleTargetImages[imgIndex],_imagesData.maleFaceLandmarks[imgIndex]));
            m_imageFaceSwap.modifyImage = _imagesData.maleTargetImages[imgIndex];
            OnSetupDone?.Invoke();
        }
        public void SetupFemaleTargetImage(int imgIndex,Action OnSetupDone){
             m_imageFaceSwap.targetImage = new List<TargetImage>();
            m_imageFaceSwap.targetImage.Add(new TargetImage(_imagesData.femaleTargetImages[imgIndex],_imagesData.femaleFaceLandmarks[imgIndex]));
            m_imageFaceSwap.modifyImage = _imagesData.femaleTargetImages[imgIndex];
            OnSetupDone?.Invoke();
        }
    }
}
public class JSONData
{
    public int error_code;
    public string error_msg;
    public List<List<List<int>>> landmarks;
    public List<List<int>> region;
    public double seconds;
    public string trx_id;
}

