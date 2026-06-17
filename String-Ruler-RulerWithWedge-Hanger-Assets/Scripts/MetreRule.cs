using UnityEngine;

public class MetreRule : MonoBehaviour
{
    [Header("Orientation")]
    public bool isVertical = false;

    [Header("Rotation Speed")]
    public float rotationSpeed = 5f;

    private Quaternion horizontalRot = Quaternion.Euler(0, 0, 0);
    private Quaternion verticalRot = Quaternion.Euler(0, 0, 90);

    void Update()
    {
#if ENABLE_LEGACY_INPUT_MANAGER
    if (Input.GetKeyDown(KeyCode.R))
    {
        isVertical = !isVertical;
    }
#endif

        Quaternion targetRot = isVertical ? verticalRot : horizontalRot;
        transform.rotation = Quaternion.Lerp(
            transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
    }

    // Returns cm reading at any world position along the ruler
    // Use this from other scripts like MomentsCalculator
    public float GetCmReading(Vector3 worldPoint)
    {
        Vector3 local = transform.InverseTransformPoint(worldPoint);
        // Local X goes from -0.5 to +0.5 (since scale X = 1)
        float cm = (local.x + 0.5f) * 100f;
        return Mathf.Clamp(cm, 0f, 100f);
    }
}