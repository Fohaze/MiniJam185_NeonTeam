using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [Tooltip("Points de vie maximum du joueur")]
    public int maxHealth = 100;
    [Tooltip("Dégats appliqué par tick")]
    public int damagePerTick = 5;
    [Tooltip("Interval en secondes entre chaque tick de dommage lorsque DoT est actif")]
    public float damageInterval = 1f;

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

    [Header("Flame UI")]
    [Tooltip("RawImage UI element for flame fill")]
    public RawImage flameImage;
    [Tooltip("Durée (sec) pour que la flamme disparaisse")]
    public float flameDecreaseDuration = 5f;
    [Tooltip("Durée (sec) pour que la flamme réapparaisse")]
    public float flameIncreaseDuration = 5f;

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

    [Header("Environnement")]
    [Tooltip("Indique si le joueur est dans le noir")]
    public bool dansLeNoir;
    private bool damageStarted;
    private float flameFill;
    private float floconFill;
    private Color flameInitColor;
    private Color floconInitColor;
    private Color frostInitColor;

    private int currentHealth;
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
        if (flameImage != null)
        {
            flameInitColor = flameImage.color;
            flameFill = 1f;
            var c = flameInitColor;
            c.a = flameFill;
            flameImage.color = c;
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
    }

    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);
        OnTakeDamage?.Invoke();
        if (healthSlider != null)
            healthSlider.value = currentHealth;
        if (currentHealth <= 0)
            KillPlayer();
    }

    public void KillPlayer()
    {
        if (currentHealth <= 0) return;
        currentHealth = 0;
        OnDeath?.Invoke();
        Destroy(gameObject);
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

    public int GetCurrentHealth() => currentHealth;

    private void Update()
    {
        // Override 'dansLeNoir' based on temperature (<0°C)
        if (temperatureChecker != null)
        {
            float temp = temperatureChecker.GetTemperature(transform.position);
            dansLeNoir = (temp < 0f);
        }
        if (flameImage == null || floconImage == null) return;

        if (dansLeNoir)
        {
            if (flameFill > 0f)
            {
                flameFill = Mathf.Max(flameFill - Time.deltaTime / flameDecreaseDuration, 0f);
                var c = flameInitColor;
                c.a = flameFill;
                flameImage.color = c;
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
                if (floconFill >= 1f && !damageStarted)
                {
                    damageStarted = true;
                    StartDamageOverTime();
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
                if (flameFill < 1f)
                {
                    flameFill = Mathf.Min(flameFill + Time.deltaTime / flameIncreaseDuration, 1f);
                    var c = flameInitColor;
                    c.a = flameFill;
                    flameImage.color = c;
                }
            }
            if (damageStarted && floconFill <= 0f)
            {
                damageStarted = false;
                StopDamageOverTime();
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
                frostAnimationState = true;
                frostAnimator.SetTrigger(frostAppearTrigger);
            }
            else if (floconFill <= 0f && frostAnimationState)
            {
                frostAnimationState = false;
                frostAnimator.SetTrigger(frostDisappearTrigger);
            }
        }

        // Update temperature display
        if (temperatureText != null && temperatureChecker != null)
        {
            float temp = temperatureChecker.GetTemperature(transform.position);
            temperatureText.text = $"{temp:F1} °C";
        }
    }
}