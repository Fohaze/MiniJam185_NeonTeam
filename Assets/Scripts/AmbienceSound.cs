using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AmbienceSound : MonoBehaviour
{
    private AudioSource _audioSource;
    private AudioSettingsManager _audioSettings;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSettings = FindObjectOfType<AudioSettingsManager>();
        if (_audioSettings == null)
            Debug.LogWarning("AmbienceSound: No AudioSettingsManager found in scene.");
    }

    void Update()
    {
        if (_audioSettings == null)
            return;

        // Active or mute based on the audioMusic boolean in settings
        _audioSource.mute = !_audioSettings.MusicEnabled;
        // Set volume based on musicVolume (0.0 - 1.0)
        _audioSource.volume = _audioSettings.MusicVolume;
    }
}
