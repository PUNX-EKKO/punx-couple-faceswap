using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace PUNX.Models
{
    [Serializable]
    public class ImageFaceSwap
    {
        public List<SourceImage> sourceImage;
        public List<TargetImage> targetImage;
        public int face_enhance;
        public string modifyImage ;

        public ImageFaceSwap(List<SourceImage> sourceImage, List<TargetImage> targetImage, int face_enhance, string modifyImage)
        {
            this.sourceImage = sourceImage;
            this.targetImage = targetImage;
            this.face_enhance = face_enhance;
            this.modifyImage = modifyImage;
        }
    }
    [Serializable]
    public class SourceImage
    {
        public string path;
        public string opts;

        public SourceImage(string path, string opts)
        {
            this.path = path;
            this.opts = opts;
        }
    }
    [Serializable]
    public class TargetImage
    {
        public string path;
        public string opts;

        public TargetImage(string path, string opts)
        {
            this.path = path;
            this.opts = opts;
        }
    }

}