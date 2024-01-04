using UnityEngine;
using UnityEngine.UI;

public class FillMask : MonoBehaviour
{
    [SerializeField]private Image childImage;
    [SerializeField]private RectTransform maskRectTransform;
    public bool isFullScreenMode;

    void OnEnable()
    {
       if(isFullScreenMode) FitMaskToChild();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F)){
            FitMaskToChild();
        }
    }
    public void FitMaskToChild()
    {
        if (childImage != null && maskRectTransform != null)
        {
            // Set the size of the mask to match the child image
            if(!isFullScreenMode){
                maskRectTransform.sizeDelta = childImage.rectTransform.sizeDelta;
                SetSize(maskRectTransform.sizeDelta.x + 100 ,maskRectTransform.sizeDelta.y +100);
            }else{
                SetSize(maskRectTransform.sizeDelta.x + 100 ,maskRectTransform.sizeDelta.y +150);
            }
        }
        else
        {
            Debug.LogError("Child Image or Mask RectTransform not found!");
        }
    }

    void SetSize(float width, float height)
    {
        // Access the RectTransform of the Image component
        RectTransform rectTransform = childImage.rectTransform;

        // Set the sizeDelta to adjust the width and height
        rectTransform.sizeDelta = new Vector2(width, height);
    }
}
