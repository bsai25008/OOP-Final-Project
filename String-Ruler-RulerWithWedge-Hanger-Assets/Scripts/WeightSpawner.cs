using UnityEngine;
using System.Collections.Generic;

public class WeightSpawner : MonoBehaviour
{
    [Header("Weight Prefabs")]
    public GameObject prefab10g;
    public GameObject prefab20g;
    public GameObject prefab50g;
    public GameObject prefab100g;

    [Header("Spawn Settings")]
    public Transform panTransform;
    public float stackSpacing = 0.015f;

    private List<GameObject> spawnedWeights = new List<GameObject>();
    private SlottedWeightSystem weightSystem;

    void Awake()
    {
        weightSystem = GetComponent<SlottedWeightSystem>();
    }

    [ContextMenu("Add 10g")]
    public void Add10g() { Spawn(prefab10g, 10); }

    [ContextMenu("Add 20g")]
    public void Add20g() { Spawn(prefab20g, 20); }

    [ContextMenu("Add 50g")]
    public void Add50g() { Spawn(prefab50g, 50); }

    [ContextMenu("Add 100g")]
    public void Add100g() { Spawn(prefab100g, 100); }

    [ContextMenu("Remove Last")]
    public void RemoveLast()
    {
        if (spawnedWeights.Count == 0) return;
        GameObject last = spawnedWeights[spawnedWeights.Count - 1];
        string rawName = last.name.Replace("(Clone)", "").Trim();
        int grams = int.Parse(rawName.Replace("WeightDisc_", "").Replace("g", "")); 
        Destroy(last);
        spawnedWeights.RemoveAt(spawnedWeights.Count - 1);
        weightSystem.RemoveWeight(grams);
    }

    [ContextMenu("Clear All")]
    public void ClearAll()
    {
        foreach (var w in spawnedWeights) Destroy(w);
        spawnedWeights.Clear();
        weightSystem.num10g = 0;
        weightSystem.num20g = 0;
        weightSystem.num50g = 0;
        weightSystem.num100g = 0;
    }

    void Spawn(GameObject prefab, int grams)
    {
        if (prefab == null) { Debug.LogError("Prefab for " + grams + "g is null!"); return; }
        if (panTransform == null) { Debug.LogError("Pan Transform not assigned!"); return; }

        float y = spawnedWeights.Count * stackSpacing;
        Vector3 pos = panTransform.position + new Vector3(0, y + 0.005f, 0);
        GameObject disc = Instantiate(prefab, pos, Quaternion.identity, panTransform);
        disc.name = "WeightDisc_" + grams + "g";
        spawnedWeights.Add(disc);
        weightSystem.AddWeight(grams);
        Debug.Log("Spawned " + grams + "g at Y=" + pos.y);
    }
}