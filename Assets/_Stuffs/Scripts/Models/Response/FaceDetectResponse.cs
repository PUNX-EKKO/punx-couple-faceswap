using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace PUNX.Models.Response
{
    [Serializable]
    public class FaceDetectResponse
    {
        public int error_code;
        public string error_msg;
        public List<List<List<int>>> landmarks;
        public List<List<int>> region;
        public double seconds;
        public string trx_id;
    }
}
