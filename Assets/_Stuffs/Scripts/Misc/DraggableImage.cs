using UnityEngine;
using UnityEngine.EventSystems;
using System;
using PUNX.Core;

public class DraggableImage : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public FaceProfileSO faceProfileSO;
    [SerializeField]private AkoolFaceswapAPI m_akool;
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Transform currentRawImage;

    /// <summary>
    /// Draggable Image Status
    ///  Not Assigned = 0, 
    ///  Assigned to Male = 1, 
    ///  Assigned to Female = 2
    /// </summary>
    public int DraggableImageStatus; 
    private Transform panelTransform; // Reference to the panel's transform
    [SerializeField] GameObject defaultPosition; // Default position to return the button

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        panelTransform = transform.parent; // Assume the panel is the initial parent
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        currentRawImage = null;
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPos);
        rectTransform.localPosition = localPos;

        // Reset the parent to the panel when dragging
        transform.SetParent(panelTransform);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        // Check if the button is currently over a RawImage
        Collider2D[] overlappingColliders = Physics2D.OverlapBoxAll(rectTransform.position, rectTransform.rect.size, 0f);

        foreach (Collider2D collider in overlappingColliders)
        {
            if (collider.CompareTag("IconTag"))
            {
                // Set the button as a child of the RawImage
                transform.SetParent(collider.transform);

                // Center the button inside the RawImage
                rectTransform.localPosition = Vector3.zero;

                // Update the currentRawImage reference
                currentRawImage = collider.transform;

                if(collider.gameObject.name == "MaskedFaceSlot(Male)")
                {
                    DraggableImageStatus = 1;
                    EventManager.OnMaleFaceDataFetched?.Invoke($"{faceProfileSO.keypoints[0]}:{faceProfileSO.keypoints[1]}:{faceProfileSO.keypoints[2]}:{faceProfileSO.keypoints[3]}");
                    
                }
                else if(collider.gameObject.name == "MaskedFaceSlot(Female)")
                {
                    DraggableImageStatus = 2;
                    EventManager.OnFemaleFaceDataFetched?.Invoke($"{faceProfileSO.keypoints[0]}:{faceProfileSO.keypoints[1]}:{faceProfileSO.keypoints[2]}:{faceProfileSO.keypoints[3]}");
                }
                return; // Stop checking for RawImages once one is found
            }
        }

        // If not over any RawImage, set the parent back to the panel
        transform.SetParent(panelTransform);
        DraggableImageStatus = 0;
        //TODO: Add Checking if the item is assigned
        // Return the button to the default position
        rectTransform.localPosition = defaultPosition.transform.localPosition;
    }
}