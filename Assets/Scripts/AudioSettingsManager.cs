using System;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsManager : MonoBehaviour
{
    public static AudioSettingsManager Instance { get; private set; }

    [Header("UI Elements")]
    public Button masterPlusButton;
    public Button masterMinusButton;
    public Text masterValueText;
    public Button musicPlusButton;
    public Button musicMinusButton;
    public Text musicValueText;
    public Toggle musicToggle;

    [Header("Settings")]
    [SerializeField, Range(0f, 1f)]
    private float masterVolume = 1f;
    [SerializeField, Range(0f, 1f)]
    private float musicVolume = 1f;
    [SerializeField]
    private bool musicEnabled = true;

    public float MasterVolume => masterVolume;
    public float MusicVolume => musicVolume;
    public bool MusicEnabled => musicEnabled;

    private const string MasterVolumeKey = "MasterVolume";
    private const string MusicVolumeKey = "MusicVolume";
    private const string MusicEnabledKey = "MusicEnabled";

    public event Action<float> OnMusicVolumeChanged;
    public event Action<bool> OnMusicEnabledChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (masterPlusButton != null && masterMinusButton != null && masterValueText != null)
        {
            masterPlusButton.onClick.AddListener(IncreaseMasterVolume);
            masterMinusButton.onClick.AddListener(DecreaseMasterVolume);
            UpdateMasterVolumeText();
        }
        if (musicPlusButton != null && musicMinusButton != null && musicValueText != null)
        {
            musicPlusButton.onClick.AddListener(IncreaseMusicVolume);
            musicMinusButton.onClick.AddListener(DecreaseMusicVolume);
            UpdateMusicVolumeText();
        }
        if (musicToggle != null)
        {
            musicToggle.isOn = musicEnabled;
            musicToggle.onValueChanged.AddListener(SetMusicEnabled);
        }
    }

    public void SetMasterVolume(float value)
    {
        masterVolume = Mathf.Clamp01(value);
        AudioListener.volume = masterVolume;
        PlayerPrefs.SetFloat(MasterVolumeKey, masterVolume);
        PlayerPrefs.Save();
        UpdateMasterVolumeText();
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(MusicVolumeKey, musicVolume);
        PlayerPrefs.Save();
        OnMusicVolumeChanged?.Invoke(musicVolume);
        UpdateMusicVolumeText();
    }

    public void SetMusicEnabled(bool enabled)
    {
        musicEnabled = enabled;
        PlayerPrefs.SetInt(MusicEnabledKey, enabled ? 1 : 0);
        PlayerPrefs.Save();
        OnMusicEnabledChanged?.Invoke(musicEnabled);
    }

    private void LoadSettings()
    {
        masterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
        AudioListener.volume = masterVolume;
        musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        musicEnabled = PlayerPrefs.GetInt(MusicEnabledKey, 1) == 1;
        UpdateMasterVolumeText();
        UpdateMusicVolumeText();
    }

    private void IncreaseMasterVolume()
    {
        SetMasterVolume(masterVolume + 0.1f);
    }

    private void DecreaseMasterVolume()
    {
        SetMasterVolume(masterVolume - 0.1f);
    }

    private void UpdateMasterVolumeText()
    {
        if (masterValueText != null)
            masterValueText.text = Mathf.RoundToInt(masterVolume * 100) + "%";
    }

    private void IncreaseMusicVolume()
    {
        SetMusicVolume(musicVolume + 0.1f);
    }

    private void DecreaseMusicVolume()
    {
        SetMusicVolume(musicVolume - 0.1f);
    }

    private void UpdateMusicVolumeText()
    {
        if (musicValueText != null)
            musicValueText.text = Mathf.RoundToInt(musicVolume * 100) + "%";
    }
}
