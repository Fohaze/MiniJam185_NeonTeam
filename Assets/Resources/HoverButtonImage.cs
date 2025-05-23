using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoverButtonImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Tooltip("RawImage à changer")]
    public RawImage targetImage;
    [Tooltip("Texture affichée normalement")]
    public Texture normalTexture;
    [Tooltip("Texture affichée au survol")]
    public Texture hoverTexture;

    void Reset()
    {
        // Si aucun targetImage, on prend l'image sur le même GameObject
        if (targetImage == null)
            targetImage = GetComponent<RawImage>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverTexture != null)
            targetImage.texture = hoverTexture;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (normalTexture != null)
            targetImage.texture = normalTexture;
    }
}
