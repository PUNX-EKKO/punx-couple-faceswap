using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using RestClient.Core;
using RestClient.Core.Models;
using PUNX.Models;
namespace PUNX.Helpers
{
    public class PUNXWebRequest : MonoBehaviour
    {
        public void PostFaceSwap(string baseURL, string token,ImageFaceSwap faceSwap,Action<Response>OnRequestComplete){
            RequestHeader contentHeader = new RequestHeader {
                Key = "content-type",
                Value = "application/json"
            };
            RequestHeader acceptHeader = new RequestHeader {
                Key = "accept",
                Value = "application/json"
            };
            RequestHeader authorization = new RequestHeader {
                Key = "Authorization",
                Value = "Bearer " + token
            };

            //Send a post request
            StartCoroutine(RestWebClient.Instance.HttpPost($"{baseURL}/api/v2/faceswap/highquality/specifyimage", JsonUtility.ToJson(faceSwap), (r) => OnRequestComplete(r), new List<RequestHeader> 
            {
                contentHeader,
                acceptHeader,
                authorization
            }));
        }

        public void PostFaceDetect(string token,FaceDetect faceDetect,Action<Response>OnRequestComplete){
            RequestHeader contentHeader = new RequestHeader {
                Key = "content-type",
                Value = "application/json"
            };
            RequestHeader acceptHeader = new RequestHeader {
                Key = "accept",
                Value = "application/json"
            };
            RequestHeader authorization = new RequestHeader {
                Key = "Authorization",
                Value = "Bearer " + token
            };

            //Send a post request
            StartCoroutine(RestWebClient.Instance.HttpPost($"https://sg3.akool.com/detect", JsonUtility.ToJson(faceDetect), (r) => OnRequestComplete(r), new List<RequestHeader> 
            {
                contentHeader,
                acceptHeader,
                authorization
            }));
        }

        public void PutSourceImage(string environment,string userId,byte[] bytes,Action<Response>OnRequestComplete,Action<String> sourceUrl){
            RequestHeader contentHeader = new RequestHeader {
                Key = "content-type",
                Value = "image/jpg"
            };
            RequestHeader acceptHeader = new RequestHeader {
                Key = "accept",
                Value = "application/json"
            };
            string imageID = System.Guid.NewGuid().ToString();
            string imgUrl = $"https://kidol-punx-filestorage.s3.ap-southeast-2.amazonaws.com/sourceimages/{userId}%2Fimg{imageID}.jpg";
            sourceUrl.Invoke(imgUrl.Replace("%2F","/"));
            //Send a post request
            StartCoroutine(RestWebClient.Instance.HttpPut($"https://3p2kpq11b7.execute-api.ap-southeast-2.amazonaws.com/{environment}/kidol-punx-filestorage/sourceimages/{userId}%2Fimg{imageID}.jpg", bytes, (r) => OnRequestComplete(r), new List<RequestHeader> 
            {
                contentHeader,
                acceptHeader
            }));
        }

        public void GetCreditInfo(string token,Action<Response>OnRequestComplete){
            RequestHeader contentHeader = new RequestHeader {
                Key = "content-type",
                Value = "application/json"
            };
            RequestHeader acceptHeader = new RequestHeader {
                Key = "accept",
                Value = "application/json"
            };
            RequestHeader authorization = new RequestHeader {
                Key = "Authorization",
                Value = "Bearer " + token
            };

            //Send a post request
            StartCoroutine(RestWebClient.Instance.HttpGet($"https://faceswap.akool.com/api/v2/faceswap/quota/info",(r) => OnRequestComplete(r), new List<RequestHeader> 
            {
                contentHeader,
                acceptHeader,
                authorization
            }));
        }
    }
}
