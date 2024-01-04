using UnityEngine;
using System.Collections;
using VoxelBusters.EssentialKit;
using UnityEngine.UI;

public class ShareImageController : MonoBehaviour
{
    [SerializeField] private Image m_FullScreenImage;
    private Texture2D texture;

    private void Start()
    {
        texture = m_FullScreenImage.mainTexture as Texture2D;
    }
    public void OnShareButtonClicked()
    {
        
        texture = Resources.Load<Texture2D>("Assets/_Stuffs/Textures");
        ShareSheet shareSheet = ShareSheet.CreateInstance();
        shareSheet.AddImage(texture);
        shareSheet.SetCompletionCallback((result, error) => {
            Debug.Log("Share Sheet was closed. Result code: " + result.ResultCode);
        });
        shareSheet.Show();
    }

}