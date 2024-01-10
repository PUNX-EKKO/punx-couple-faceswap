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
        public KeypointHandller keypointHandller;

        void OnEnable()
        {
            EventManager.OnSourceImagePosted += OnSourceImageUploaded;
            EventManager.OnMaleFaceDataFetched += FetchedMaleFaceData;
             EventManager.OnFemaleFaceDataFetched += FetchedFemaleFaceData;
        }

        void OnDisable()
        {
            EventManager.OnSourceImagePosted -= OnSourceImageUploaded;
            EventManager.OnMaleFaceDataFetched -= FetchedMaleFaceData;
            EventManager.OnFemaleFaceDataFetched -= FetchedFemaleFaceData;
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
                keypointHandller.face1Keypoints = new Vector2Int[4];
                keypointHandller.face2Keypoints  = new Vector2Int[4];
                // Access the landmarks array
                List<List<List<int>>> landmarks = faceDetectResponse.landmarks;
                try
                {
                    if(landmarks.Count <= 1){
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
                    keypointHandller.face1Keypoints[i] = new Vector2Int(x1, y1);
                }
              
                for (int i = 0; i < 4; i++)
                {
                    
                     // Access specific landmarks
                    List<int> landmark1 = landmarks[1][i]; // Accessing the first landmark. Note: Change the 2nd array value to change the landmark value
                    // Accessing individual values
                    int x1 = landmark1[0]; 
                    int y1 = landmark1[1]; 
                    _face2Landmarks.Add($"{x1},{y1}");
                    keypointHandller.face2Keypoints[i] = new Vector2Int(x1, y1);
                    
                }
                yield return new WaitUntil(( )=> _face1Landmarks.Count.Equals(4) && _face2Landmarks.Count.Equals(4));
                m_imageFaceSwap.sourceImage = new List<SourceImage>();         
                for (int i = 0; i < 2; i++) // Add initial 2 empty source image
                {
                    m_imageFaceSwap.sourceImage.Add(new SourceImage("",""));
                }
                keypointHandller.draggableImages[0].faceProfileSO.keypoints = keypointHandller.face1Keypoints;
                keypointHandller.draggableImages[1].faceProfileSO.keypoints = keypointHandller.face2Keypoints;
                SetupFace();
                Debug.Log($"Face Detection Complete!");
        }

        private void SetupFace(){
            EventManager.OnFaceDataFetched?.Invoke(m_faceDetect.image_url);
        }

        private void FetchedMaleFaceData(string value)
        {
            var filteredValue = value.Replace("(",String.Empty).Replace(")",String.Empty).Replace(" ",String.Empty);
            m_imageFaceSwap.sourceImage[0] = new SourceImage(m_faceDetect.image_url,filteredValue); // Setup face 1 keypoint values;
        }
        private void FetchedFemaleFaceData(string value)
        {
            var filteredValue = value.Replace("(",String.Empty).Replace(")",String.Empty).Replace(" ",String.Empty);
            m_imageFaceSwap.sourceImage[1] = new SourceImage(m_faceDetect.image_url,filteredValue); // Setup face 2 keypoint values
        }

       
        public void DoneGenderSetup(){
           
            for (int i = 0; i < keypointHandller.draggableImages.Length; i++)
            {
                if(keypointHandller.draggableImages[i].DraggableImageStatus.Equals(0)){
                    Debug.LogError($"Please Assign Gender!");
                    return;
                 }
            }
            EventManager.OnFaceDetectionComplete?.Invoke();
            keypointHandller.faceAssignUI.SetActive(false);
        }
        public void SetupCoupleTargetImage(int imgIndex, Action OnSetupDone){
            m_imageFaceSwap.targetImage = new List<TargetImage>();
            Debug.Log($"Face Data: {_imagesData.coupleTargetImages[imgIndex]} & {_imagesData.coupleFaceLandmarks[imgIndex]}");
            SplitGenderKeypoints(_imagesData.coupleTargetImages[imgIndex],_imagesData.coupleFaceLandmarks[imgIndex]);
            m_imageFaceSwap.modifyImage = _imagesData.coupleTargetImages[imgIndex];
            OnSetupDone?.Invoke();
        }

        void SplitGenderKeypoints(string url,string keypoints)
        {
            // Split the string based on "-"
            string[] groups = keypoints.Split('-');

            // Iterate through each group
            foreach (string group in groups)
            {
                Debug.Log("Group: " + group);
                 m_imageFaceSwap.targetImage.Add(new TargetImage(url,group));
            }
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

