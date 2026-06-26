using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class MenuMouseParallax : MonoBehaviour
{
    [Header("Movement")]
    public float moveAmount = 35f;
    public float smoothSpeed = 8f;
    public bool invertMovement = false;

    [Header("Optional Rotation")]
    public bool useRotation = true;
    public float rotationAmount = 2f;

    private RectTransform rectTransform;
    private Vector2 startAnchoredPosition;
    private Quaternion startRotation;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        startAnchoredPosition = rectTransform.anchoredPosition;
        startRotation = rectTransform.localRotation;
    }

    private void Update()
    {
        Vector2 mousePos = GetMousePosition();

        // Converts mouse position to a range from -1 to 1
        Vector2 normalizedMouse = new Vector2(
            (mousePos.x / Screen.width - 0.5f) * 2f,
            (mousePos.y / Screen.height - 0.5f) * 2f
        );

        if (invertMovement)
            normalizedMouse *= -1f;

        Vector2 targetPosition = startAnchoredPosition + normalizedMouse * moveAmount;

        rectTransform.anchoredPosition = Vector2.Lerp(
            rectTransform.anchoredPosition,
            targetPosition,
            1f - Mathf.Exp(-smoothSpeed * Time.unscaledDeltaTime)
        );

        if (useRotation)
        {
            Quaternion targetRotation = startRotation * Quaternion.Euler(
                -normalizedMouse.y * rotationAmount,
                normalizedMouse.x * rotationAmount,
                0f
            );

            rectTransform.localRotation = Quaternion.Lerp(
                rectTransform.localRotation,
                targetRotation,
                1f - Mathf.Exp(-smoothSpeed * Time.unscaledDeltaTime)
            );
        }
    }

    private Vector2 GetMousePosition()
    {
#if ENABLE_INPUT_SYSTEM
        if (Mouse.current != null)
            return Mouse.current.position.ReadValue();
#endif
        return Input.mousePosition;
    }
}