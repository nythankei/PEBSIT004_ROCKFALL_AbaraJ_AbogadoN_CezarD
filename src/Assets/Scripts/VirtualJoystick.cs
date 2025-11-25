using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VirtualJoystick : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform thumb;
    [Tooltip("If true, joystick stays at original position. If false, joystick follows touch.")]
    public bool fixedPosition = false;

    private Vector2 originalPosition;
    private Vector2 originalThumbPosition;
    private RectTransform rectTransform;
    private Canvas canvas;
    private RectTransform canvasRect;

    public Vector2 delta;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
        originalThumbPosition = thumb.localPosition;
        thumb.gameObject.SetActive(false);
        delta = Vector2.zero;
        
        // Cache canvas
        canvas = GetComponentInParent<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();
        
        // Ensure camera is assigned for Screen Space - Camera mode
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera == null)
        {
            canvas.worldCamera = Camera.main;
            Debug.LogWarning("Canvas worldCamera was null! Assigned Camera.main automatically.");
        }
    }
    
    private Camera GetEventCamera()
    {
        return canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        thumb.gameObject.SetActive(true);

        if (!fixedPosition)
        {
            // Convert screen point to anchored position
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                eventData.position,
                GetEventCamera(),
                out localPoint);
            
            // Clamp position to keep joystick visible on screen
            float halfWidth = rectTransform.rect.width / 2f;
            float halfHeight = rectTransform.rect.height / 2f;
            
            localPoint.x = Mathf.Clamp(localPoint.x, 
                -canvasRect.rect.width / 2f + halfWidth, 
                canvasRect.rect.width / 2f - halfWidth);
            localPoint.y = Mathf.Clamp(localPoint.y, 
                -canvasRect.rect.height / 2f + halfHeight, 
                canvasRect.rect.height / 2f - halfHeight);
            
            rectTransform.anchoredPosition = localPoint;
        }
        
        thumb.localPosition = originalThumbPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Move thumb relative to joystick base
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position,
            GetEventCamera(),
            out localPoint);
        
        thumb.localPosition = localPoint;

        // Calculate delta with correct sensitivity formula
        var size = rectTransform.rect.size;
        delta = thumb.localPosition;

        float sensitivity = 0.5f;
        delta.x = (delta.x / (size.x / 2.0f)) * sensitivity;
        delta.y = (delta.y / (size.y / 2.0f)) * sensitivity;

        delta.x = Mathf.Clamp(delta.x, -1.0f, 1.0f);
        delta.y = Mathf.Clamp(delta.y, -1.0f, 1.0f);
        
        // Debug to verify both X and Y are working
        Debug.Log($"Joystick Delta - X: {delta.x:F3}, Y: {delta.y:F3}, Thumb pos: {thumb.localPosition}");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition = originalPosition;
        delta = Vector2.zero;
        thumb.gameObject.SetActive(false);
    }
}