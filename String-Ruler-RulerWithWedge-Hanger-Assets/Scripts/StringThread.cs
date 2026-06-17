using UnityEngine;

public class StringThread : MonoBehaviour
{
    [Header("Attachment Points")]
    public Transform pointA;
    public Transform pointB;

    [Header("Line Renderer")]
    public LineRenderer lineRenderer;

    [Header("String Settings")]
    public int segments = 20;
    public float sagAmount = 0.05f;
    public bool isTaut = false;

    [Header("Physics")]
    public float tensionForce = 0f;
    public float breakingTension = 50f;
    public bool isBroken = false;

    void Start()
    {
        // Auto find line renderer if not assigned
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.positionCount = segments;
    }

    void Update()
    {
        if (isBroken)
        {
            DrawBrokenString();
            return;
        }

        if (pointA == null || pointB == null) return;

        if (isTaut)
            DrawStraightString();
        else
            DrawSaggyString();

        CheckBreaking();
    }

    // Straight string — used when under high tension
    void DrawStraightString()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, pointA.position);
        lineRenderer.SetPosition(1, pointB.position);
    }

    // Saggy string — natural hanging curve
    void DrawSaggyString()
    {
        lineRenderer.positionCount = segments;

        for (int i = 0; i < segments; i++)
        {
            float t = i / (float)(segments - 1);
            Vector3 pos = Vector3.Lerp(pointA.position, pointB.position, t);

            // Parabolic sag — deepest at middle
            float sag = sagAmount * 4f * t * (1f - t);
            pos.y -= sag;

            lineRenderer.SetPosition(i, pos);
        }
    }

    // Broken string — falls and goes limp
    void DrawBrokenString()
    {
        lineRenderer.positionCount = segments;

        for (int i = 0; i < segments; i++)
        {
            float t = i / (float)(segments - 1);
            Vector3 pos = Vector3.Lerp(pointA.position, pointB.position, t);

            // Exaggerated sag when broken
            float sag = 0.5f * 4f * t * (1f - t);
            pos.y -= sag;

            lineRenderer.SetPosition(i, pos);
        }
    }

    // Called from weight scripts to apply tension
    public void SetTension(float force)
    {
        tensionForce = force;

        // Reduce sag as tension increases
        sagAmount = Mathf.Max(0f, 0.05f - force * 0.001f);

        // Go taut if tension is high enough
        isTaut = force > 5f;
    }

    // Check if string snaps
    void CheckBreaking()
    {
        if (tensionForce >= breakingTension)
        {
            isBroken = true;
            Debug.Log("String broke! Tension was: " + tensionForce + "N");
        }
    }

    // Get current tension — used by other scripts
    public float GetTension()
    {
        return tensionForce;
    }

    // Get the length of the string in metres
    public float GetLength()
    {
        if (pointA == null || pointB == null) return 0f;
        return Vector3.Distance(pointA.position, pointB.position);
    }

    // Reset string to normal
    public void ResetString()
    {
        isBroken = false;
        tensionForce = 0f;
        sagAmount = 0.05f;
        isTaut = false;
    }
}