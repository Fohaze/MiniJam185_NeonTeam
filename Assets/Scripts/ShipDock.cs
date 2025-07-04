using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Gère la réception d'objets déposés autour du vaisseau.
/// Compte les tags "BouDeBoa" et "ScrappyScrappa" et invoque un événement de fin de partie.
/// </summary>
[RequireComponent(typeof(Collider))]
public class ShipDock : MonoBehaviour
{
    [Header("Configuration des Récoltes")]
    [Tooltip("Quantité requise de BouDeBoa")] public int requiredBouDeBoa = 5;
    [Tooltip("Quantité requise de ScrappyScrappa")] public int requiredScrappyScrappa = 5;
    [Tooltip("Quantité requise de Batterie")] public int requiredBatterie = 1;

    [Header("Événement de Fin de Partie")]
    public UnityEvent onGameFinished;

    [Header("Détection")]
    [Tooltip("Portée de détection pour collecte")] public float detectionRange = 3f;

    [Header("UI Text 3D")]
    [Tooltip("3D Text pour BouDeBoa")] public TextMesh bouCountText;
    [Tooltip("3D Text pour ScrappyScrappa")] public TextMesh scrappyCountText;
    [Tooltip("3D Text pour Batterie")] public TextMesh batterieCountText;

    [Header("UI Text Legacy")]
    [Tooltip("UI Text pour BouDeBoa")] public Text bouUIText;
    [Tooltip("UI Text pour ScrappyScrappa")] public Text scrappyUIText;
    [Tooltip("UI Text pour Batterie")] public Text batterieUIText;

    [Header("Audio")]
    [Tooltip("AudioSource pour jouer les sons du dock")]
    public AudioSource audioSource;
    [Tooltip("Son joué lorsqu'un objet est collecté")] public AudioClip collectClip;
    [Tooltip("Son joué à la fin de la partie")] public AudioClip gameFinishedClip;

    private int bouCount = 0;
    private int scrappyCount = 0;
    private int batterieCount = 0;
    // Assure qu'une seule invocation de fin de jeu
    private bool hasFinished = false;

    private void Start()
    {
        // Initial UI
        if (bouCountText != null) bouCountText.text = $"{bouCount}/{requiredBouDeBoa}";
        if (scrappyCountText != null) scrappyCountText.text = $"{scrappyCount}/{requiredScrappyScrappa}";
        if (batterieCountText != null) batterieCountText.text = $"{batterieCount}/{requiredBatterie}";
        if (bouUIText != null) bouUIText.text = $"{bouCount}/{requiredBouDeBoa}";
        if (scrappyUIText != null) scrappyUIText.text = $"{scrappyCount}/{requiredScrappyScrappa}";
        if (batterieUIText != null) batterieUIText.text = $"{batterieCount}/{requiredBatterie}";
        // Initialise AudioSource si non assigné
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (hasFinished) return;
        // Ne détecter que les tags dont les quantités ne sont pas encore atteintes
        var tagsToDetect = new List<string>();
        if (bouCount < requiredBouDeBoa) tagsToDetect.Add("BouDeBoa");
        if (scrappyCount < requiredScrappyScrappa) tagsToDetect.Add("ScrappyScrappa");
        if (batterieCount < requiredBatterie) tagsToDetect.Add("Batterie");
        if (tagsToDetect.Count == 0) return;
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange);
        foreach (var hit in hits)
        {
            var obj = hit.gameObject;
            if (!tagsToDetect.Contains(obj.tag)) continue;
            if (bouCount < requiredBouDeBoa && obj.CompareTag("BouDeBoa"))
            {
                bouCount++;
                if (bouCountText != null) bouCountText.text = $"{bouCount}/{requiredBouDeBoa}";
                if (bouUIText != null) bouUIText.text = $"{bouCount}/{requiredBouDeBoa}";
                Destroy(obj);
                Debug.Log($"BouDeBoa détectés: {bouCount}/{requiredBouDeBoa}");
                if (audioSource != null && collectClip != null) audioSource.PlayOneShot(collectClip);
            }
            else if (scrappyCount < requiredScrappyScrappa && obj.CompareTag("ScrappyScrappa"))
            {
                scrappyCount++;
                if (scrappyCountText != null) scrappyCountText.text = $"{scrappyCount}/{requiredScrappyScrappa}";
                if (scrappyUIText != null) scrappyUIText.text = $"{scrappyCount}/{requiredScrappyScrappa}";
                Destroy(obj);
                Debug.Log($"ScrappyScrappa détectés: {scrappyCount}/{requiredScrappyScrappa}");
                if (audioSource != null && collectClip != null) audioSource.PlayOneShot(collectClip);
            }
            else if (batterieCount < requiredBatterie && obj.CompareTag("Batterie"))
            {
                batterieCount++;
                if (batterieCountText != null) batterieCountText.text = $"{batterieCount}/{requiredBatterie}";
                if (batterieUIText != null) batterieUIText.text = $"{batterieCount}/{requiredBatterie}";
                Destroy(obj);
                Debug.Log($"Batterie détectées: {batterieCount}/{requiredBatterie}");
                if (audioSource != null && collectClip != null) audioSource.PlayOneShot(collectClip);
            }
        }
        CheckCompletion();
    }

    private void CheckCompletion()
    {
        if (!hasFinished && bouCount >= requiredBouDeBoa && scrappyCount >= requiredScrappyScrappa && batterieCount >= requiredBatterie)
        {
            hasFinished = true;
            Debug.Log("Objectifs atteints, fin de partie déclenchée !");
            if (audioSource != null && gameFinishedClip != null) audioSource.PlayOneShot(gameFinishedClip);
            onGameFinished?.Invoke();
        }
    }
}
