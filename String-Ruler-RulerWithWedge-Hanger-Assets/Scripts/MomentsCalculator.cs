using UnityEngine;
using TMPro;

public class MomentsCalculator : MonoBehaviour
{
    [Header("Pivot Settings")]
    public float pivotPosition = 0.5f;

    [Header("Left Side")]
    public float massLeft = 0f;
    public float distanceLeft = 0.2f;

    [Header("Right Side")]
    public float massRight = 0f;
    public float distanceRight = 0.2f;

    [Header("Display")]
    public TextMeshPro statusText;
    public TextMeshPro momentLeftText;
    public TextMeshPro momentRightText;

    [Header("Tilt Settings")]
    public float tiltSpeed = 2f;
    public float maxTiltAngle = 30f;

    private const float g = 9.81f;
    private float currentTilt = 0f;

    void Update()
    {
        CalculateMoments();
    }

    void CalculateMoments()
    {
        // Calculate moments
        float momentLeft = massLeft * g * distanceLeft;
        float momentRight = massRight * g * distanceRight;
        float netMoment = momentLeft - momentRight;

        // Update left moment text
        if (momentLeftText != null)
            momentLeftText.text = $"Left: {momentLeft:F2} Nm";

        // Update right moment text
        if (momentRightText != null)
            momentRightText.text = $"Right: {momentRight:F2} Nm";

        // Update status text
        if (statusText != null)
        {
            if (Mathf.Abs(netMoment) < 0.01f)
                statusText.text = "BALANCED";
            else if (netMoment > 0)
                statusText.text = $"Tilts LEFT\n{netMoment:F2} Nm";
            else
                statusText.text = $"Tilts RIGHT\n{Mathf.Abs(netMoment):F2} Nm";
        }

        // Smoothly tilt the ruler
        float targetTilt = Mathf.Clamp(
            -netMoment * 15f, -maxTiltAngle, maxTiltAngle);
        currentTilt = Mathf.Lerp(
            currentTilt, targetTilt, Time.deltaTime * tiltSpeed);
        transform.localRotation = Quaternion.Euler(0f, 0f, currentTilt);
    }

    public void SetLeftMass(float mass, float distance)
    {
        massLeft = mass;
        distanceLeft = distance;
    }

    public void SetRightMass(float mass, float distance)
    {
        massRight = mass;
        distanceRight = distance;
    }

    public bool IsBalanced()
    {
        float momentLeft = massLeft * g * distanceLeft;
        float momentRight = massRight * g * distanceRight;
        return Mathf.Abs(momentLeft - momentRight) < 0.01f;
    }
}