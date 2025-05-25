using UnityEngine;
using UnityEngine.Events;

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

    private int bouCount = 0;
    private int scrappyCount = 0;
    private int batterieCount = 0;
    // Assure qu'une seule invocation de fin de jeu
    private bool hasFinished = false;

    private void Update()
    {
        if (hasFinished) return;
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange);
        foreach (var hit in hits)
        {
            var obj = hit.gameObject;
            if (bouCount < requiredBouDeBoa && obj.CompareTag("BouDeBoa"))
            {
                bouCount++;
                Destroy(obj);
                Debug.Log($"BouDeBoa détectés: {bouCount}/{requiredBouDeBoa}");
            }
            else if (scrappyCount < requiredScrappyScrappa && obj.CompareTag("ScrappyScrappa"))
            {
                scrappyCount++;
                Destroy(obj);
                Debug.Log($"ScrappyScrappa détectés: {scrappyCount}/{requiredScrappyScrappa}");
            }
            else if (batterieCount < requiredBatterie && obj.CompareTag("Batterie"))
            {
                batterieCount++;
                Destroy(obj);
                Debug.Log($"Batterie détectées: {batterieCount}/{requiredBatterie}");
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
            onGameFinished?.Invoke();
        }
    }
}
