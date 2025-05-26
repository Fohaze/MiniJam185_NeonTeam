using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.UI;
using System;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [Tooltip("Points de vie maximum du joueur")]
    public int maxHealth = 100;
    [Tooltip("Dégats appliqué par tick")]
    public int damagePerTick = 1;
    [Tooltip("Interval en secondes entre chaque tick de dommage lorsque DoT est actif")]
    public float damageInterval = 1f;
    [Header("Dégât Externe (Type Balle)")]
    [Tooltip("Dégâts appliqués lorsque ApplyDamage est appelé")]
    public int externalDamageAmount = 10;

    [Header("Events")]
    public UnityEvent OnTakeDamage;
    public UnityEvent OnDeath;

    [Header("UI")]
    [Tooltip("Slider UI element for health bar")]
    public Slider healthSlider;

    [Header("Température UI")]
    [Tooltip("Text Legacy UI for displaying temperature")]
    public Text temperatureText;
    [Tooltip("TemperatureChecker component to retrieve temperature")]
    public TemperatureChecker temperatureChecker;

    [Header("Soleil UI")]
    [Tooltip("RawImage UI element for soleil fill")]
    public RawImage soleilImage;
    [Tooltip("Durée (sec) pour que le soleil diminue")]
    public float soleilDecreaseDuration = 5f;
    [Tooltip("Durée (sec) pour que le soleil réapparaisse")]
    public float soleilIncreaseDuration = 5f;

    [Header("Flocon UI")]
    [Tooltip("RawImage UI element for flocon fill")]
    public RawImage floconImage;
    [Tooltip("Durée (sec) pour que le flocon grandisse")]
    public float floconIncreaseDuration = 5f;
    [Tooltip("Durée (sec) pour que le flocon disparaisse")]
    public float floconDecreaseDuration = 5f;

    [Header("Givre UI")]
    [Tooltip("RawImage UI element for frost effect")]
    public RawImage frostImage;
    [Header("Frost Animator")]
    [Tooltip("Animator Controller on frostImage GameObject")]
    public Animator frostAnimator;
    [Tooltip("Trigger name for frost appear animation")]
    public string frostAppearTrigger = "FrostAppear";
    [Tooltip("Trigger name for frost disappear animation")]
    public string frostDisappearTrigger = "FrostDisappear";

    [Header("Flame UI")]
    [Tooltip("RawImage UI element for flame fill")]
    public RawImage flameImage;
    [Tooltip("Durée (sec) pour que la flamme disparaisse")]
    public float flameDecreaseDuration = 5f;
    [Tooltip("Durée (sec) pour que la flamme réapparaisse")]
    public float flameIncreaseDuration = 5f;

    [Header("Fire Temperature")]
    [Tooltip("Température max près du feu")]
    public float fireMaxTemperature = 40f;
    [Tooltip("Durée (sec) pour atteindre la temperature max près du feu")]
    public float fireTempIncreaseDuration = 5f;

    [Header("Environnement")]
    [Tooltip("Indique si le joueur est dans le noir")]
    public bool dansLeNoir;
    private bool damageStarted;
    private float soleilFill;
    private float floconFill;
    private float flameFill;
    private Color soleilInitColor;
    private Color floconInitColor;
    private Color flameInitColor;
    private Color frostInitColor;
    private float displayedTemperature;

    [Header("Audio")]
    [Tooltip("AudioSource pour les sons de santé")]
    public AudioSource audioSource;
    [Tooltip("Son à jouer quand le joueur subit des dégâts")]
    public AudioClip damageClip;
    [Tooltip("Son à jouer quand le joueur est soigné")]
    public AudioClip healClip;

    // Healing zone proximity settings
    [Header("Healing Zone Settings")]
    [Tooltip("GameObject du feu de camp")]
    public GameObject campfireObject;
    [Tooltip("Rayon pour que le joueur soit soigné")]
    public float healRadius = 5f;
    [Tooltip("Délai avant de commencer le soin une fois dans la zone")]
    public float healDelay = 0f;

    // Healing zone
    private bool inHealingZone;
    // Permet à d'autres scripts de savoir si le joueur est dans la zone de soin
    public bool InHealingZone { get { return inHealingZone; } }
    private bool _prevInHealingZone = false;
    private float _zoneEnterTime;
    // Healing over time settings
    [Tooltip("Points de vie récupérés par tick")]
    public int healPerTick = 5;
    [Tooltip("Interval en secondes entre chaque tick de soin")]
    public float healInterval = 1f;
    private Coroutine healCoroutine;

    public int currentHealth;
    private Coroutine damageCoroutine;
    private bool frostAnimationState = false;

    private void Awake()
    {
        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
        if (soleilImage != null)
        {
            soleilInitColor = soleilImage.color;
            soleilFill = 1f;
            var c = soleilInitColor;
            c.a = soleilFill;
            soleilImage.color = c;
        }
        if (floconImage != null)
        {
            floconInitColor = floconImage.color;
            floconFill = 0f;
            var c2 = floconInitColor;
            c2.a = floconFill;
            floconImage.color = c2;
        }
        if (frostImage != null)
        {
            frostInitColor = frostImage.color;
            var cf = frostInitColor;
            cf.a = 0f;
            frostImage.color = cf;
        }
        if (flameImage != null)
        {
            flameInitColor = flameImage.color;
            flameFill = 0f;
            var cf2 = flameInitColor;
            cf2.a = flameFill;
            flameImage.color = cf2;
        }
        // Init displayed temperature
        if (temperatureChecker != null)
            displayedTemperature = temperatureChecker.GetTemperature(transform.position);
        // Initialise AudioSource si non assignée
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void TakeDamage(int amount)
    {
        Debug.Log($"Player takes {amount} damage. Current health: {currentHealth}");
        if (currentHealth <= 0) return;
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);
        OnTakeDamage?.Invoke();
        // Son de dégât
        if (audioSource != null && damageClip != null)
            audioSource.PlayOneShot(damageClip);
        if (healthSlider != null)
            healthSlider.value = currentHealth;
        if (currentHealth <= 0)
            KillPlayer();
    }

    public void KillPlayer()
    {
        Debug.Log("Player has died.");
        currentHealth = 0;
        OnDeath?.Invoke();
        //Destroy(gameObject);
        if (currentHealth <= 0) return;
    }

    public void StartDamageOverTime()
    {
        StopDamageOverTime();
        damageCoroutine = StartCoroutine(DamageOverTime());
    }

    public void StopDamageOverTime()
    {
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }
    }

    private IEnumerator DamageOverTime()
    {
        while (true)
        {
            TakeDamage(damagePerTick);
            yield return new WaitForSeconds(damageInterval);
        }
    }

    public void StartHealingOverTime()
    {
        StopHealingOverTime();
        healCoroutine = StartCoroutine(HealingOverTime());
    }

    public void StopHealingOverTime()
    {
        if (healCoroutine != null)
        {
            StopCoroutine(healCoroutine);
            healCoroutine = null;
        }
    }

    private IEnumerator HealingOverTime()
    {
        while (true)
        {
            if (currentHealth < maxHealth)
            {
                currentHealth += healPerTick;
                currentHealth = Mathf.Min(currentHealth, maxHealth);
                if (healthSlider != null)
                    healthSlider.value = currentHealth;
                // Son de soin
                if (audioSource != null && healClip != null)
                    audioSource.PlayOneShot(healClip);
            }
            yield return new WaitForSeconds(healInterval);
        }
    }

    public int GetCurrentHealth() => currentHealth;

    public void ApplyDamage()
    {
        TakeDamage(externalDamageAmount);
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        if (healthSlider != null)
            healthSlider.value = currentHealth;
    }

    private void Update()
    {
        // Compute healing zone proximity only if campfireObject is active
        if (campfireObject != null && campfireObject.activeInHierarchy)
            inHealingZone = Vector3.Distance(transform.position, campfireObject.transform.position) <= healRadius;
        else
            inHealingZone = false;

        // Override 'dansLeNoir' based on healing zone and temperature (<0°C)
        if (inHealingZone)
        {
            dansLeNoir = false;
        }
        else if (temperatureChecker != null)
        {
            float temp = temperatureChecker.GetTemperature(transform.position);
            dansLeNoir = (temp < 0f);
        }
        if (soleilImage == null || floconImage == null || flameImage == null) return;

        // Update flame UI: instant disappearance when leaving zone
        if (inHealingZone)
            flameFill = Mathf.Min(flameFill + Time.deltaTime / flameIncreaseDuration, 1f);
        else
            flameFill = 0f;
        var cf3 = flameInitColor;
        cf3.a = flameFill;
        flameImage.color = cf3;

        // Hide soleil and flocon UI when in healing zone
        if (inHealingZone)
        {
            // Hide soleil instantly
            var csole = soleilInitColor;
            csole.a = 0f;
            soleilImage.color = csole;
            // Fade out frost gradually
            if (floconFill > 0f)
                floconFill = Mathf.Max(floconFill - Time.deltaTime / floconDecreaseDuration, 0f);
            var cfloc2 = floconInitColor;
            cfloc2.a = floconFill;
            floconImage.color = cfloc2;
        }

        // Soleil and flocon UI
        if (!inHealingZone)
        {
            if (dansLeNoir)
            {
                if (soleilFill > 0f)
                {
                    soleilFill = Mathf.Max(soleilFill - Time.deltaTime / soleilDecreaseDuration, 0f);
                    var c = soleilInitColor;
                    c.a = soleilFill;
                    soleilImage.color = c;
                }
                else
                {
                    if (floconFill < 1f)
                    {
                        floconFill = Mathf.Min(floconFill + Time.deltaTime / floconIncreaseDuration, 1f);
                        var c2 = floconInitColor;
                        c2.a = floconFill;
                        floconImage.color = c2;
                    }
                }
            }
            else
            {
                if (floconFill > 0f)
                {
                    floconFill = Mathf.Max(floconFill - Time.deltaTime / floconDecreaseDuration, 0f);
                    var c2 = floconInitColor;
                    c2.a = floconFill;
                    floconImage.color = c2;
                }
                else
                {
                    if (soleilFill < 1f)
                    {
                        soleilFill = Mathf.Min(soleilFill + Time.deltaTime / soleilIncreaseDuration, 1f);
                        var c = soleilInitColor;
                        c.a = soleilFill;
                        soleilImage.color = c;
                    }
                }
            }
        }

        // Update frost alpha based on floconFill
        if (frostImage != null)
        {
            var cf = frostInitColor;
            cf.a = floconFill;
            frostImage.color = cf;
        }

        // Trigger frost appear/disappear animations once at transitions
        if (frostAnimator != null)
        {
            if (floconFill > 0f && !frostAnimationState)
            {
                // play appear
                frostAnimator.ResetTrigger(frostDisappearTrigger);
                frostAnimationState = true;
                frostAnimator.SetTrigger(frostAppearTrigger);
            }
            else if (floconFill <= 0f && frostAnimationState)
            {
                // play disappear
                frostAnimator.ResetTrigger(frostAppearTrigger);
                frostAnimationState = false;
                frostAnimator.SetTrigger(frostDisappearTrigger);
            }
        }

        // Update temperature display with fire proximity heating
        if (temperatureText != null && temperatureChecker != null)
        {
            float ambientTemp = temperatureChecker.GetTemperature(transform.position);
            if (inHealingZone)
                displayedTemperature = Mathf.MoveTowards(displayedTemperature, fireMaxTemperature, (fireMaxTemperature - ambientTemp) / fireTempIncreaseDuration * Time.deltaTime);
            else
                displayedTemperature = ambientTemp;
            temperatureText.text = $"{displayedTemperature:F1} °C";
        }

        // Handle healing zone entry timing
        if (inHealingZone && !_prevInHealingZone)
            _zoneEnterTime = Time.time;
        _prevInHealingZone = inHealingZone;
        // Start or stop healing based on healDelay
        if (inHealingZone && Time.time - _zoneEnterTime >= healDelay)
        {
            if (healCoroutine == null)
                StartHealingOverTime();
        }
        else
        {
            if (healCoroutine != null)
                StopHealingOverTime();
        }

        // Unified Damage Over Time control based on floconFill threshold
        if (!inHealingZone && floconFill >= 1f)
        {
            if (!damageStarted)
            {
                damageStarted = true;
                StartDamageOverTime();
            }
        }
        else
        {
            if (damageStarted)
            {
                damageStarted = false;
                StopDamageOverTime();
            }
        }
    }
}