using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class KeypointPainter : MonoBehaviour
{
 
   
     public Image imageUI; // The UI Image where you want to draw the keypoints
    public Texture2D inputImage; // The source image containing the keypoints
    public Vector2Int[] keypoints; // Array of keypoints in original image pixel coordinates

    // Start is called before the first frame update
    void Start()
    {
        if (imageUI == null || inputImage == null)
        {
            Debug.LogError("Image or UI Image not assigned.");
            return;
        }

        DrawKeypoints();
    }

    void DrawKeypoints()
    {
        // Clone the input image to avoid modifying the original
        Texture2D outputImage = new Texture2D(inputImage.width, inputImage.height);
        Graphics.CopyTexture(inputImage, outputImage);

        foreach (Vector2Int keypoint in keypoints)
        {
            int x = keypoint.x;
            int y = inputImage.height - keypoint.y; // Invert the y-axis to match Unity's coordinate system

            // Draw a red keypoint at the calculated position
            Color keypointColor = Color.red;
            for (int i = x - 5; i <= x + 5; i++)
            {
                for (int j = y - 5; j <= y + 5; j++)
                {
                    outputImage.SetPixel(i, j, keypointColor);
                }
            }
        }

        outputImage.Apply(); // Apply the changes to the texture

        // Set the modified texture to the UI Image
        Sprite sprite = Sprite.Create(outputImage, new Rect(0, 0, outputImage.width, outputImage.height), new Vector2(0.5f, 0.5f));
        imageUI.sprite = sprite;
    }
}