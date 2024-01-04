using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableImage : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Transform currentRawImage;
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

                return; // Stop checking for RawImages once one is found
            }
        }

        // If not over any RawImage, set the parent back to the panel
        transform.SetParent(panelTransform);

        // Return the button to the default position
        rectTransform.localPosition = defaultPosition.transform.localPosition;
    }
}
