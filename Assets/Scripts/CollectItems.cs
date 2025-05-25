using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gère la collecte séquentielle d'une liste d'objets (bois, métal, etc.).
/// Affiche UI quand joueur est proche, active le modèle en main et désactive l'objet au sol.
/// </summary>
public class CollectItems : MonoBehaviour
{
    [Header("Sélection d'Objets")]
    [Tooltip("Tags des objets à collecter")]
    public List<string> collectTags;
    [Tooltip("Liste des objets en main, correspondent aux Tags")]
    public List<GameObject> handObjects;

    [Header("UI et Interaction")]
    [Tooltip("UI à activer quand on peut collecter")] public GameObject uiCanInteract;
    [Tooltip("Animator du joueur pour animations de prise/pose")] public Animator anim;
    [Tooltip("Distance max pour collecter")] public float interactionDistance = 2f;
    [Tooltip("Touche d'interaction")] public KeyCode pickKey = KeyCode.E;

    [Header("Drop")]
    [Tooltip("Prefabs correspondants aux objets tenus pour drop")] public List<GameObject> dropPrefabs;
    [Tooltip("Distance devant le joueur où drop l'objet")] public float dropDistance = 1f;
    [Tooltip("Parent sous lequel placer l'objet spawn")] public Transform dropParent;

    void Start()
    {
        // Récupère Animator si non assigné
        if (anim == null)
            anim = GetComponent<Animator>();
        // Initialise l'inventaire (désactive tout)
        foreach (var obj in handObjects)
            if (obj != null) obj.SetActive(false);
        if (uiCanInteract != null) uiCanInteract.SetActive(false);
    }

    void Update()
    {
        // Détection via OverlapSphere pour éviter la main
        Collider[] hits = Physics.OverlapSphere(transform.position, interactionDistance);
        GameObject nearest = null;
        float minDist = float.MaxValue;
        foreach (var hit in hits)
        {
            var obj = hit.gameObject;
            // ignore les objets que l'on tient
            if (handObjects.Contains(obj)) continue;
            if (!collectTags.Contains(obj.tag)) continue;
            float d = Vector3.Distance(transform.position, obj.transform.position);
            if (d < minDist)
            {
                minDist = d;
                nearest = obj;
            }
        }
        // Interactions
        if (nearest != null)
        {
            if (uiCanInteract != null) uiCanInteract.SetActive(true);
            if (Input.GetKeyDown(pickKey))
            {
                // Collecte selon tag
                string t = nearest.tag;
                int idx = collectTags.IndexOf(t);
                if (idx >= 0 && idx < handObjects.Count)
                {
                    // Jouer animation de prise
                    if (anim != null)
                        anim.SetBool("porte", true);
                    handObjects[idx]?.SetActive(true);
                    Destroy(nearest);
                }
                if (uiCanInteract != null) uiCanInteract.SetActive(false);
            }
        }
        else
        {
            if (uiCanInteract != null) uiCanInteract.SetActive(false);
            if (Input.GetKeyDown(pickKey)) DropHeld();
        }
    }

    private void DropHeld()
    {
        for (int i = 0; i < handObjects.Count; i++)
        {
            if (handObjects[i] != null && handObjects[i].activeSelf)
            {
                // Jouer animation de pose
                if (anim != null)
                    anim.SetBool("porte", false);
                handObjects[i].SetActive(false);
                // Instancie le prefab correspondant à l'index
                if (dropPrefabs != null && i < dropPrefabs.Count && dropPrefabs[i] != null)
                {
                    GameObject instance = Instantiate(dropPrefabs[i], transform.position + transform.forward * dropDistance, Quaternion.identity);
                    // Assigne le même tag pour re-collecte
                    instance.tag = collectTags[i];
                    // Place sous le parent spécifié
                    if (dropParent != null)
                        instance.transform.SetParent(dropParent);
                }
                break;
            }
        }
    }
}
