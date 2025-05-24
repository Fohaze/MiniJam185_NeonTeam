using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class planetRotor : MonoBehaviour
{

    public InputActionAsset actions;
    public float speed = 5f;
    public float rotate_stick = 0f;
    public GameObject world_center;

    private void OnEnable()
    {
        var map = actions.FindActionMap("AlienMap");
        var interactAction = map.FindAction("dir_x");
        interactAction.performed += ctx => rotate_stick = ctx.ReadValue<float>();
        interactAction.Enable();

    }
    void Start()
    {

    }

    void Update()
    {

        //faire tourner le monde autour de l'axe Y
        if (rotate_stick != 0f)
        {
            world_center.transform.Rotate(Vector3.up, rotate_stick * speed * Time.deltaTime);
        }

    }
}