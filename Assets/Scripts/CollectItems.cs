using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

[System.Serializable]
public class ItemEntry
{
    [Tooltip("Tag de l'objet à collecter")] public string tag;
    [Tooltip("Objet à afficher en main")] public GameObject handObject;
    [Tooltip("Prefab à instancier lors du drop")] public GameObject dropPrefab;
}

public class CollectItems : MonoBehaviour
{
    [Header("Configuration des Items")]
    public List<ItemEntry> itemEntries;
    [Header("UI et Interaction")]
    public GameObject uiCanInteract;
    public Animator anim;
    public float interactionDistance = 2f;

    [Header("Drop")]
    public float dropDistance = 1f;
    public Transform dropParent;

    [Header("Audio")]
    [Tooltip("Source audio pour jouer le son de ramassage")]
    public AudioSource audioSource;
    [Tooltip("Clip de son à jouer lors du ramassage")]
    public AudioClip pickUpClip;

    private Alien_map2 controls;
    private GameObject nearestObject;
    private int nearestIndex = -1;
    private List<int> inventoryIndices = new List<int>();

    /// <summary>
    /// Retourne le nombre d'objets actuellement tenus.
    /// </summary>
    public int InventoryCount => inventoryIndices.Count;

    void Awake()
    {
        controls = new Alien_map2();
        controls.AlienMap.Interagir.performed += _ => OnInteract();
    }
    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    void Start()
    {
        if (anim == null) anim = GetComponent<Animator>();
        inventoryIndices.Clear();
        UpdateHandVisuals();
        if (uiCanInteract != null) uiCanInteract.SetActive(false);
    }

    void Update()
    {
        nearestObject = null;
        nearestIndex = -1;
        float minDist = float.MaxValue;
        Collider[] hits = Physics.OverlapSphere(transform.position, interactionDistance);
        foreach (var hit in hits)
        {
            var obj = hit.gameObject;
            int idx = itemEntries.FindIndex(e => e.tag == obj.tag);
            if (idx < 0) continue;
            float d = Vector3.Distance(transform.position, obj.transform.position);
            if (d < minDist)
            {
                minDist = d;
                nearestObject = obj;
                nearestIndex = idx;
            }
        }
        if (uiCanInteract != null)
            uiCanInteract.SetActive(nearestObject != null);
    }

    private void OnInteract()
    {
        if (nearestObject != null)
        {
            // Trouve tous les indices d'entries correspondant au tag
            var tag = nearestObject.tag;
            var allIndices = itemEntries
                .Select((entry, i) => new { entry.tag, i })
                .Where(x => x.tag == tag)
                .Select(x => x.i)
                .ToList();
            // Choisit le prochain index libre
            var possible = allIndices.Except(inventoryIndices).ToList();
            if (possible.Count > 0)
            {
                var idxToAdd = possible[0];
                if (anim != null) anim.SetBool("porte", true);
                inventoryIndices.Add(idxToAdd);
                UpdateHandVisuals();
                Destroy(nearestObject);
                if (audioSource != null && pickUpClip != null) audioSource.PlayOneShot(pickUpClip);
                if (uiCanInteract != null) uiCanInteract.SetActive(false);
                return;
            }
        }
        // Sinon lâche le dernier item
        DropHeld();
    }

    private void UpdateHandVisuals()
    {
        // Désactive tous
        foreach (var entry in itemEntries)
            if (entry.handObject != null) entry.handObject.SetActive(false);
        // Active selon inventaire
        for (int i = 0; i < inventoryIndices.Count; i++)
        {
            int idx = inventoryIndices[i];
            if (idx >= 0 && idx < itemEntries.Count && itemEntries[idx].handObject != null)
                itemEntries[idx].handObject.SetActive(true);
        }
    }

    private void DropHeld()
    {
        if (inventoryIndices.Count == 0) return;
        int last = inventoryIndices[inventoryIndices.Count - 1];
        if (anim != null) anim.SetBool("porte", false);
        inventoryIndices.RemoveAt(inventoryIndices.Count - 1);
        UpdateHandVisuals();
        var entry = itemEntries[last];
        if (entry.dropPrefab != null)
        {
            var pos = transform.position + transform.forward * dropDistance;
            GameObject inst = Instantiate(entry.dropPrefab, pos, Quaternion.identity);
            inst.tag = entry.tag;
            if (dropParent != null) inst.transform.SetParent(dropParent);
        }
    }
}
