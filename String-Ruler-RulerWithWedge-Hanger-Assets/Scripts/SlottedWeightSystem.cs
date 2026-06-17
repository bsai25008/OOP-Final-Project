using UnityEngine;
using TMPro;

public class SlottedWeightSystem : MonoBehaviour
{
    [Header("Hanger Mass")]
    public float hangerMass = 0.05f;

    [Header("Slotted Weights")]
    public int num10g = 0;
    public int num20g = 0;
    public int num50g = 0;
    public int num100g = 0;

    [Header("Display")]
    public TextMeshPro totalLabel;

    [Header("Connected String")]
    public StringThread connectedString;

    private Rigidbody rb;

    void Start() { rb = GetComponent<Rigidbody>(); UpdateMass(); }
    void OnValidate()
    {
        UpdateMass();
    }
    public void AddWeight(int grams)
    {
        switch (grams)
        {
            case 10: num10g++; break;
            case 20: num20g++; break;
            case 50: num50g++; break;
            case 100: num100g++; break;
        }
        UpdateMass();
    }

    public void RemoveWeight(int grams)
    {
        switch (grams)
        {
            case 10: num10g = Mathf.Max(0, num10g - 1); break;
            case 20: num20g = Mathf.Max(0, num20g - 1); break;
            case 50: num50g = Mathf.Max(0, num50g - 1); break;
            case 100: num100g = Mathf.Max(0, num100g - 1); break;
        }
        UpdateMass();
    }

    void UpdateMass()
    {
        float totalGrams = (hangerMass * 1000f)
            + (num10g * 10f) + (num20g * 20f)
            + (num50g * 50f) + (num100g * 100f);
        float totalKg = totalGrams / 1000f;
        float forceN = totalKg * 9.81f;
        if (rb != null) rb.mass = totalKg;
        if (totalLabel != null)
            totalLabel.text = $"{totalGrams:F0}g\n{forceN:F2}N";
        if (connectedString != null)
            connectedString.SetTension(forceN);
        Debug.Log($"Total: {totalGrams}g | {forceN:F2}N");
    }

    public float GetTotalMassKg() => rb != null ? rb.mass : hangerMass;
    public float GetForceN() => GetTotalMassKg() * 9.81f;
    public float GetTotalGrams() =>
        (hangerMass * 1000f) + (num10g * 10f) + (num20g * 20f)
        + (num50g * 50f) + (num100g * 100f);


}