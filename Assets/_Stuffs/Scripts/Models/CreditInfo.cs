using System.Collections.Generic;
using UnityEngine;
using System;

namespace PUNX.Models
{
     [Serializable]
    public class CreditInfo 
    {
        public int code;
        public string msg;
        public Data data;
    }
    [Serializable]
    public class Data
    {
        public int credit;
    }
}
