using UnityEngine;
using UnityEngine.UI;
using VoxelBusters.EssentialKit;

public class CameraManager : MonoBehaviour
{
    Texture2D texture;
    public void Selfie()
    {
        MediaServices.CaptureImageFromCameraWithUserPermision(true, (textureData, error) =>
        {
            if (error == null)
            {
                texture = textureData.GetTexture(); 
                MediaServices.SaveImageToGallery(texture, (result, error) =>
                {
                    if (error == null)
                    {
                        Debug.Log("Save image to gallery finished successfully.");
                    }
                    else
                    {
                        Debug.Log("Save image to gallery failed with error. Error: " + error);
                    }
                });
            }
            else
            {
                Debug.Log("Capture image using camera failed with error. Error: " + error);
            }
        });
    }
}