using UnityEngine;
using UnityEngine.InputSystem;

public class CameraOrbitNew : MonoBehaviour
{
    [SerializeField] private Transform planetCenter;
    [SerializeField] private float sensitivity = 0.1f;

    private Alien_map2 controls;
    private Vector2 lookInput; // uses Vector2 for mouse look

    public bool IsOrbiting => lookInput.sqrMagnitude > 0f;

    private void Awake()
    {
        controls = new Alien_map2();
        controls.AlienMap.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        controls.AlienMap.Look.canceled  += ctx => lookInput = Vector2.zero;
    }

    private void OnEnable() => controls.AlienMap.Enable();
    private void OnDisable() => controls.AlienMap.Disable();

    private void LateUpdate()
    {
        if (planetCenter == null) return;
        transform.RotateAround(planetCenter.position, Vector3.up, lookInput.x * sensitivity);
        transform.RotateAround(planetCenter.position, transform.right, -lookInput.y * sensitivity);
    }
}
