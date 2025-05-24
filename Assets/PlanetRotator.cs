using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class planetRotor : MonoBehaviour
{

    [SerializeField] private float speed = 5f;

    private Alien_map2 controls;
    private Vector2 lookInput;

    private void Awake()
    {
        controls = new Alien_map2();
        controls.AlienMap.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        controls.AlienMap.Look.canceled  += ctx => lookInput = Vector2.zero;
    }

    private void OnEnable() => controls.AlienMap.Enable();
    private void OnDisable() => controls.AlienMap.Disable();

    void Update()
    {
        if (Mathf.Abs(lookInput.x) < 0.01f) return;
        transform.Rotate(Vector3.up, lookInput.x * speed * Time.deltaTime, Space.World);
    }
}