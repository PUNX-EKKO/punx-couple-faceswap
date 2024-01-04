using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PUNX.Models.Response
{
    [Serializable]
    public class ImageFaceswapResponse
    {
        public int code;
        public string msg;
        public Data data;
    }

    [Serializable]
    public class Data
    {
        public string _id;
        public string job_id;
        public string url;
    }
}
