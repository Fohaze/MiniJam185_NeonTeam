using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class alien_ctrl : MonoBehaviour
{
    // Référence à ton InputActionAsset (assignée dans l’Inspector)
    public InputActionAsset actions;

    private InputAction moveAction;
    public Vector2 moveInput;
    public float speed = 5f;
    public Animator anim;

    public GameObject cam;

    public GameObject foot_l, foot_r;
    public GameObject particulesPrefab;

    private void OnEnable()
    {
        // Récupère l’ActionMap "basic_move"
        var map = actions.FindActionMap("basic_move");
        // Récupère l’Action "move"
        moveAction = map.FindAction("move");
        // Active-la pour qu’elle commence à écouter
        moveAction.Enable();
    }

    public void OnFootLeft()
    {
        if (foot_l == null || particulesPrefab == null)
            return;
        // Instancie le prefab de particules à la position du pied gauche
        Instantiate(particulesPrefab, foot_l.transform.position, Quaternion.identity);
    }
    public void OnFootRight()
    {
        if (foot_r == null || particulesPrefab == null)
            return;
        // Instancie le prefab de particules à la position du pied droit
        Instantiate(particulesPrefab, foot_r.transform.position, Quaternion.identity);
    }

    

    private void OnDisable()
    {
        moveAction.Disable();
    }

    private void Update()
    {
        // Lit la valeur Vector2 de l’action
        Vector2 inputVector = moveAction.ReadValue<Vector2>();
        // Ici tu peux l’utiliser pour déplacer ton personnage…
        Debug.Log($"Input move : {inputVector}");
        moveInput = inputVector;

        var y_cam = cam.transform.eulerAngles.y;
        var y_char = transform.eulerAngles.y;

        //orientation du personnage en fonction du joystick et de la caméra
        if (moveInput != Vector2.zero)
        {
            var angle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg + 90f;
            transform.eulerAngles = new Vector3(0, y_cam + angle, 0);
        }
        // Déplacement du personnage
        var mag = moveInput.magnitude;
        if (mag > 0.1f)
        {
            var moveDir = Quaternion.Euler(0, y_cam, 0) * new Vector3(moveInput.x, 0, moveInput.y);
            transform.position += moveDir * speed * Time.deltaTime;
        }
        anim.SetFloat("speed", mag);

    }
}
