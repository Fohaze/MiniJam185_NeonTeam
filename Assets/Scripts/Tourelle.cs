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
    public float speed = 10f;
    public float rotationSpeed = 10f;

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
            bullet.transform.position += bullet.transform.up * speed * Time.deltaTime;
            bullet.transform.position = planetCenter.transform.position + (bullet.transform.position - planetCenter.transform.position).normalized * _distanceToPlanetCenter;
            Vector3 cross = Vector3.Cross(gunPoint.transform.right, (bullet.transform.position - planetCenter.transform.position).normalized);
            bullet.transform.rotation = Quaternion.LookRotation((bullet.transform.position - planetCenter.transform.position).normalized, cross);
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
        _timeSinceLastFire += Time.deltaTime;
        if (_timeSinceLastFire >= fireRate)
        {
            Fire();
        }
    }

    void Fire()
    {
        GameObject projectile = Instantiate(projectilePrefab, gunPoint.transform.position, Quaternion.LookRotation(transform.up, gunPoint.transform.forward));
        projectile.transform.parent = gunPoint.transform;
        _bullets.Add(projectile);
        _timeSinceLastFire = 0;
    }
}
