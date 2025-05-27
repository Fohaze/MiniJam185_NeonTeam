using UnityEngine;
using System.Collections;

/// <summary>
/// Attaché à un Canvas pour masquer l'UI au démarrage et l'afficher après un délai configurable.
/// </summary>
public class ShowCanvasWithDelay : MonoBehaviour
{
    [Tooltip("Canvas à afficher")] public Canvas targetCanvas;
    [Tooltip("Délai en secondes avant d'afficher le Canvas.")] public float delayTime = 5f;

    private void Awake()
    {
        if (targetCanvas != null)
            targetCanvas.gameObject.SetActive(false);
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(delayTime);
        if (targetCanvas != null)
            targetCanvas.gameObject.SetActive(true);
    }
}
