using UnityEngine;
using UnityEngine.InputSystem;

public class LabEquipment
{
    public string equipmentName;
    public string description;

    public LabEquipment(string name, string desc)
    {
        equipmentName = name;
        description = desc;
    }

    public virtual string GetInfo()
    {
        return $"{equipmentName}: {description}";
    }
}

public class PulleyEquipment : LabEquipment
{
    public float mechanicalAdvantage;
    public string pulleyType;

    public PulleyEquipment() : base(
        "Single Fixed Pulley",
        "Changes direction of force. Mechanical Advantage = 1.")
    {
        pulleyType = "Fixed";
        mechanicalAdvantage = 1f;
    }

    public float CalculateEffort(float loadNewtons)
    {
        return loadNewtons / mechanicalAdvantage;
    }

    public override string GetInfo()
    {
        return $"Type: {pulleyType} | MA: {mechanicalAdvantage} | {description}";
    }
}

public class PulleySystem : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public Transform wheel;
    public Transform ropeLoad;
    public Transform ropeEffort;
    public Transform effortRope;
    public Transform loadWeight;
    public Transform weightSlot;

    [Header("Experiment Values")]
    public float loadMassKg = 1f;
    public float gravity = 9.8f;

    [Header("Animation Settings")]
    public float animSpeed = 0.2f;
    public float maxLift = 0.15f;

    private PulleyEquipment pulley;
    private bool isAnimating = false;
    private float elapsed = 0f;

    // Initial Positions
    private Vector3 initWeightPos;
    private Vector3 initSlotPos;
    private Vector3 initRopeLoadPos;
    private Vector3 initRopeEffortPos;
    private Vector3 initEffortRopePos;

    // Initial Scales (Crucial for resetting stretched/shrunk ropes)
    private Vector3 initRopeLoadScale;
    private Vector3 initEffortRopeScale;

    void Start()
    {
        pulley = new PulleyEquipment();
        Debug.Log("=== Pulley System Ready ===");
        Debug.Log(pulley.GetInfo());

        // Save exact starting positions — must be correct before pressing Play
        if (loadWeight != null) initWeightPos = loadWeight.localPosition;
        if (weightSlot != null) initSlotPos = weightSlot.localPosition;
        if (ropeLoad != null) initRopeLoadPos = ropeLoad.localPosition;
        if (ropeEffort != null) initRopeEffortPos = ropeEffort.localPosition;
        if (effortRope != null) initEffortRopePos = effortRope.localPosition;

        // Save exact starting scales
        if (ropeLoad != null) initRopeLoadScale = ropeLoad.localScale;
        if (effortRope != null) initEffortRopeScale = effortRope.localScale;
    }

    void Update()
    {
        DetectClick();
        if (isAnimating) Animate();
    }

    void DetectClick()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(
                Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == effortRope || hit.transform == ropeEffort)
                {
                    if (!isAnimating)
                    {
                        // Start animation
                        isAnimating = true;
                        elapsed = 0f;

                        float loadForce = loadMassKg * gravity;
                        float effort = pulley.CalculateEffort(loadForce);
                        Debug.Log("--- Experiment Start ---");
                        Debug.Log($"Load Mass : {loadMassKg} kg");
                        Debug.Log($"Load Force: {loadForce} N");
                        Debug.Log($"Effort    : {effort} N");
                        Debug.Log($"MA = {pulley.mechanicalAdvantage}");
                        Debug.Log("Pull effort rope DOWN ? weight rises UP");
                        Debug.Log("Conclusion: Fixed pulley reverses direction. Effort = Load.");
                    }
                }
            }
        }
    }
    void Animate()
    {
        elapsed += Time.deltaTime * animSpeed;
        float move = Mathf.Clamp(elapsed, 0f, maxLift);

        // 1. Wheel spins
        if (wheel != null)
            wheel.Rotate(Vector3.up, 80f * Time.deltaTime);

        // 2. Weight and slot move UP
        if (loadWeight != null)
            loadWeight.localPosition = initWeightPos + Vector3.up * move;

        if (weightSlot != null)
            weightSlot.localPosition = initSlotPos + Vector3.up * move;

        // 3. ROPE LOAD (Fixed Scaling)
        if (ropeLoad != null)
        {
            // Unity cylinders are 2 units tall by default. 
            // To shrink the cylinder's height by exactly 1 unit of 'move', we scale by half of 'move'.
            float scaleReduction = move / 2f;

            // Subtract from the initial saved scale so it's a stable calculation
            float newScaleY = initRopeLoadScale.y - scaleReduction;

            // Prevent the scale from hitting 0 or flipping negative
            newScaleY = Mathf.Max(0.001f, newScaleY);

            ropeLoad.localScale = new Vector3(initRopeLoadScale.x, newScaleY, initRopeLoadScale.z);

            // Keep the top attached to the wheel groove
            ropeLoad.localPosition = initRopeLoadPos + Vector3.up * (move * 0.5f);
        }

        // 4. EFFORT ROPE (Moves down)
        if (effortRope != null)
        {
            effortRope.localPosition = initEffortRopePos + Vector3.down * move;
        }

        // Stop animation when max reached
        if (elapsed >= maxLift)
        {
            isAnimating = false;
            Debug.Log("--- Animation Complete. Press R to reset. ---");
        }
    }

    void ResetAll()
    {
        elapsed = 0f;
        isAnimating = false;

        // Restore Positions
        if (loadWeight != null) loadWeight.localPosition = initWeightPos;
        if (weightSlot != null) loadWeight.localPosition = initWeightPos; // Safety check
        if (weightSlot != null) weightSlot.localPosition = initSlotPos;
        if (ropeLoad != null) ropeLoad.localPosition = initRopeLoadPos;
        if (ropeEffort != null) ropeEffort.localPosition = initRopeEffortPos;
        if (effortRope != null) effortRope.localPosition = initEffortRopePos;

        // Restore Scales
        if (ropeLoad != null) ropeLoad.localScale = initRopeLoadScale;
        if (effortRope != null) effortRope.localScale = initEffortRopeScale;
    }

    // Call reset by pressing R key
    void LateUpdate()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            ResetAll();
            Debug.Log("--- Reset ---");
        }
    }
}