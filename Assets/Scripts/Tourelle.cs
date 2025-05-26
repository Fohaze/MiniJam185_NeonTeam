using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tourelle : MonoBehaviour
{
    public GameObject projectilePrefab;
    public GameObject gunPoint;
    public GameObject planetCenter;
    public Material onMaterial;
    public Material offMaterial;
    public MeshRenderer paneauSolaireMeshRenderer;
    public TemperatureChecker temperatureChecker;
    public float fireRate = 1f;
    public float range = 10f;

    [Header("Projectile & Turret Speeds")]
    [Tooltip("Vitesse de déplacement du projectile")]
    public float projectileSpeed = 10f;
    [Tooltip("Vitesse de rotation de la tourelle")]
    public float rotationSpeed = 10f;
    [Tooltip("Multiplicateur pour projectile quand joueur détecté")]
    public float playerProjectileMultiplier = 2f;
    [Tooltip("Multiplicateur pour rotation quand joueur détecté")]
    public float playerRotationMultiplier = 2f;

    private float _baseProjectileSpeed;
    private float _baseRotationSpeed;

    private List<GameObject> _bullets = new List<GameObject>();
    private float _timeSinceLastFire = 0f;

    private float _distanceToPlanetCenter;
    private bool _isOn = false;

    // Start is called before the first frame update
    void Start()
    {
        if (temperatureChecker == null)
        {
            temperatureChecker = FindObjectOfType<TemperatureChecker>();
        }
        _distanceToPlanetCenter = Vector3.Distance(gunPoint.transform.position, planetCenter.transform.position);
        // Enregistre les vitesses de base
        _baseProjectileSpeed = projectileSpeed;
        _baseRotationSpeed = rotationSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isOn != temperatureChecker.GetTemperature(transform.position) > 0)
        {
            _isOn = temperatureChecker.GetTemperature(transform.position) > 0;
            paneauSolaireMeshRenderer.material = _isOn ? onMaterial : offMaterial;
        }
        for (int i = _bullets.Count - 1; i >= 0; i--)
        {
            GameObject bullet = _bullets[i];
            // Mouvement le long de l'axe local up pour suivre la surface
            bullet.transform.position += bullet.transform.up * projectileSpeed * Time.deltaTime;
            // Reprojection sur la surface sphérique pour rester à la même distance
            Vector3 dir = (bullet.transform.position - planetCenter.transform.position).normalized;
            bullet.transform.position = planetCenter.transform.position + dir * _distanceToPlanetCenter;
            // Ajuste la rotation pour suivre la surface
            Vector3 cross = Vector3.Cross(gunPoint.transform.right, dir);
            bullet.transform.rotation = Quaternion.LookRotation(dir, cross);
            if (Physics.Raycast(bullet.transform.position - bullet.transform.up * .25f, bullet.transform.up, out RaycastHit hit, .5f))
            {
                var playerHealth = hit.transform.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    //Debug.Log("Touché !!!");
                    playerHealth.TakeDamage(20);
                }
                else
                {
                    //Debug.Log("Pas de PlayerHealth !!!");
                }
                Destroy(bullet);
                _bullets.RemoveAt(i);
            }
        }

        if (!_isOn)
        {
            _timeSinceLastFire = 0;
            return;
        }
        // Réinitialise aux vitesses de base
        projectileSpeed = _baseProjectileSpeed;
        rotationSpeed = _baseRotationSpeed;
        // Détection du joueur et visée
        Collider[] hits = Physics.OverlapSphere(transform.position, range);
        Transform player = null;
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player")) { player = hit.transform; break; }
        }
        if (player != null)
        {
            // Vérifie healing zone
            var ph = player.GetComponent<PlayerHealth>();
            if (ph != null && ph.InHealingZone) player = null;
        }
        if (player != null)
        {
            // Accélère quand cible présente
            projectileSpeed = _baseProjectileSpeed * playerProjectileMultiplier;
            rotationSpeed = _baseRotationSpeed * playerRotationMultiplier;
            Vector3 dir = (player.position - transform.position).normalized;
            Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
        _timeSinceLastFire += Time.deltaTime;
        if (player != null && _timeSinceLastFire >= fireRate)
            Fire();
    }

    void Fire()
    {
        // Instancie et rattache au canon pour hériter de la rotation planétaire
        GameObject projectile = Instantiate(projectilePrefab, gunPoint.transform.position, gunPoint.transform.rotation);
        projectile.transform.parent = gunPoint.transform;
        _bullets.Add(projectile);
        _timeSinceLastFire = 0;
    }
}
