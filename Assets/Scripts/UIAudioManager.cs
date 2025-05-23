using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class UIAudioManager : MonoBehaviour
{
    public static UIAudioManager Instance { get; private set; }

    [Header("Highlight Sound")]
    [Tooltip("Clip joué lors du survol ou sélection des boutons UI")]
    public AudioClip highlightClip;
    [Range(0f, 1f)]
    public float highlightVolume = 1f;

    [Header("Click Sound")]
    [Tooltip("Clip joué lors du clic des boutons UI")]
    public AudioClip clickClip;
    [Range(0f, 1f)]
    public float clickVolume = 1f;

    private AudioSource audioSource;
    private HashSet<Selectable> processedSelectables = new HashSet<Selectable>();
    private bool isClickPlaying;
    private Coroutine clickCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void Start()
    {
        // Assigne survol/sélection à tous les boutons et toggles, mêmes désactivés
        var allSelectables = Resources.FindObjectsOfTypeAll<Selectable>();
        foreach (var sel in allSelectables)
        {
            if (sel.gameObject.scene.isLoaded)
                AddButtonEvents(sel);
        }
    }

    private void AddButtonEvents(Selectable uiElement)
    {
        if (!processedSelectables.Add(uiElement)) return;
        var trigger = uiElement.gameObject.GetComponent<EventTrigger>()
                     ?? uiElement.gameObject.AddComponent<EventTrigger>();

        var entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        entryEnter.callback.AddListener((data) => PlayHighlight());
        trigger.triggers.Add(entryEnter);

         // Son de clic à chaque événement PointerClick
         var entryClick = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
         entryClick.callback.AddListener((data) => PlayClick());
         trigger.triggers.Add(entryClick);
    }

    /// <summary>
    /// Joue le son de survol
    /// </summary>
    public void PlayHighlight()
    {
        if (isClickPlaying) return;
        if (highlightClip != null)
            audioSource.PlayOneShot(highlightClip, highlightVolume);
    }

    /// <summary>
    /// Joue le son de clic
    /// </summary>
    public void PlayClick()
    {
        if (clickClip != null)
        {
            audioSource.PlayOneShot(clickClip, clickVolume);
            if (clickCoroutine != null)
                StopCoroutine(clickCoroutine);
            clickCoroutine = StartCoroutine(ClickCooldownRoutine(clickClip.length));
        }
    }
    
    private IEnumerator ClickCooldownRoutine(float duration)
    {
        isClickPlaying = true;
        yield return new WaitForSecondsRealtime(duration);
        isClickPlaying = false;
    }
}
