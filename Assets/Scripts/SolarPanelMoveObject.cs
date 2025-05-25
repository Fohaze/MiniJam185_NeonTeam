using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarPanelMoveObject : MonoBehaviour
{
    public GameObject movePart;
    public Transform onPos;
    public Transform offPos;
    public Material onMaterial;
    public Material offMaterial;
    public MeshRenderer paneauSolaireMeshRenderer;
    public TemperatureChecker temperatureChecker;
    public float speed = 10f;
    private bool _isOn = false;
    float _time = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (temperatureChecker == null)
        {
            temperatureChecker = FindObjectOfType<TemperatureChecker>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_isOn != temperatureChecker.GetTemperature(paneauSolaireMeshRenderer.transform.position) > 0)
        {
            _isOn = temperatureChecker.GetTemperature(paneauSolaireMeshRenderer.transform.position) > 0;
            paneauSolaireMeshRenderer.material = _isOn ? onMaterial : offMaterial;
        }
        _time = Mathf.Clamp01(_isOn ? _time + Time.deltaTime : _time - Time.deltaTime);
        movePart.transform.position = Vector3.Lerp(offPos.position, onPos.position, _time);
    }
}
