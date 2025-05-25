using UnityEngine;

public class FireCamp : MonoBehaviour
{
    [Header("Paramètres du Camp de Feu")]
    [Range(0f, 10f)]
    [Tooltip("Valeur du feu de 10 (plein) à 0 (éteint)")]
    public float fireValue = 10f;

    [Header("Références")]
    [Tooltip("GameObject actif lorsque le feu n'est pas plein (valeur < max)")]
    public GameObject notFullObject;
    [Tooltip("GameObject actif lorsque le feu est plein (valeur == max)")]
    public GameObject fullObject;
    [Tooltip("TextMesh 3D pour afficher la valeur du feu")]
    public TextMesh valueText;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip soundDecrement;
    public AudioClip soundFull;

    private const float maxValue = 10f;

    private void Start()
    {
        MettreAJourEtat();
        MettreAJourTexte();
    }

    public void AjouterCombustible()
    {
        // Enlève une unité de combustible
        fireValue = Mathf.Clamp(fireValue - 1f, 0f, maxValue);
        // Joue le son de décrément
        if (audioSource != null && soundDecrement != null)
            audioSource.PlayOneShot(soundDecrement);
        MettreAJourEtat();
        MettreAJourTexte();
    }

    private void MettreAJourEtat()
    {
        bool prevEmpty = fullObject != null && fullObject.activeSelf;
        bool isEmpty = fireValue <= 0f;
        if (notFullObject != null)
            notFullObject.SetActive(!isEmpty);
        if (fullObject != null)
            fullObject.SetActive(isEmpty);
        // Joue le son de feu éteint lors de la transition
        if (isEmpty && !prevEmpty && audioSource != null && soundFull != null)
            audioSource.PlayOneShot(soundFull);
    }

    private void MettreAJourTexte()
    {
        if (valueText != null)
        {
            if (Mathf.Approximately(fireValue, Mathf.Round(fireValue)))
                valueText.text = Mathf.RoundToInt(fireValue).ToString();
            else
                valueText.text = fireValue.ToString("0.0");
        }
    }

    private void Update()
    {
        MettreAJourTexte();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        fireValue = Mathf.Clamp(fireValue, 0f, maxValue);
        MettreAJourEtat();
        MettreAJourTexte();
    }
#endif
}
