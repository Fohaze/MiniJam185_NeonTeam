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

    public Vector3 start_jump_pos;
    public float jump_hauteur = 1f;
    public AnimationCurve jump_curve;
    public float fall_speed = 1f;
    public GameObject world_center;
    public float rotate_speed = 5f;
    public GameObject world;

    public float jump_time = -1;
    public float vel_fall = 0f;

    private void OnEnable()
    {
        // Récupère l’ActionMap "basic_move"
        var map = actions.FindActionMap("AlienMap");
        // Récupère l’Action "move"
        moveAction = map.FindAction("Directions");
        // Active-la pour qu’elle commence à écouter
        
        // Récupère le bouton "Sauter"
        var jumpAction = map.FindAction("Sauter");
        //link l’action de saut à la méthode jump_action
        jumpAction.performed += jump_action;
        // Active l’action de saut
        jumpAction.Enable();

        // Récupère le bouton "Interagir"
        var interactAction = map.FindAction("Interagir");
        //link l’action de saut à la méthode jump_action
        interactAction.performed += ctx => interact_act();
        // Active l’action de saut
        interactAction.Enable();

        moveAction.Enable();
    }

    void Update_jump()
    {
        //if(!Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.8f))
        //raycast sphère
        if(!Physics.SphereCast(transform.position, 0.5f, Vector3.down, out RaycastHit hit, 1.8f))
        {
            var max_speed = speed * 0.5f;
            vel_fall += fall_speed * Time.deltaTime;
            if(vel_fall > max_speed)
                vel_fall = max_speed;
            transform.position += Vector3.down * vel_fall * Time.deltaTime;
            
            //transform.position += Vector3.down * fall_speed * Time.deltaTime;
        }

        //var jump_h = anim.GetFloat("jump_h");
        if(jump_time < 0)
            return;
        /*
        jump_time += Time.deltaTime;
        var pt = transform.position;
        pt.y = start_jump_pos.y + jump_curve.Evaluate(jump_time) * jump_hauteur;
        transform.position = pt;
        */
        if(jump_time > jump_curve.keys[jump_curve.keys.Length - 1].time)
        {
            jump_time = -1;
        }
    }

    public void interact_act()
    {
        anim.SetTrigger("interact");
    }

    public void OnFootLeft()
{
    if (foot_l == null || particulesPrefab == null)
        return;

    // Instancie le prefab de particules à la position du pied gauche
    var instance = Instantiate(particulesPrefab, foot_l.transform.position, Quaternion.identity);
    var ps = instance.GetComponent<ParticleSystem>();

    // Accès au MainModule pour régler l'espace de simulation
    var main = ps.main;
    main.simulationSpace = ParticleSystemSimulationSpace.Custom;
    main.customSimulationSpace = world.transform;
}

public void OnFootRight()
{
    if (foot_r == null || particulesPrefab == null)
        return;

    // Instancie le prefab de particules à la position du pied droit
    var instance = Instantiate(particulesPrefab, foot_r.transform.position, Quaternion.identity);
    var ps = instance.GetComponent<ParticleSystem>();

    var main = ps.main;
    main.simulationSpace = ParticleSystemSimulationSpace.Custom;
    main.customSimulationSpace = world.transform;
}


    

    private void OnDisable()
    {
        moveAction.Disable();
    }

    public void jump_action(InputAction.CallbackContext context)
    {
        // Appel de la méthode de saut
        anim.SetTrigger("jump");
        start_jump_pos = transform.position;
        jump_time = 0f;
    }

    private void Update()
    {
        // Lit la valeur Vector2 de l’action
        Vector2 inputVector = moveAction.ReadValue<Vector2>();
        // Ici tu peux l’utiliser pour déplacer ton personnage…
        //Debug.Log($"Input move : {inputVector}");
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
            //transform.position += moveDir * speed * Time.deltaTime;
            /*var w_an = world_center.transform.eulerAngles;
            w_an.z += moveInput.y * rotate_speed * Time.deltaTime;
            w_an.x += moveInput.x * rotate_speed * Time.deltaTime;
            world_center.transform.eulerAngles = w_an;
            */
            if(!Physics.Raycast(transform.position , moveDir, out RaycastHit hit, 1f))
            //spherecast pour éviter les collisions
            //if(!Physics.SphereCast(transform.position, 0.5f, moveDir, out RaycastHit hit, 1f))
            {
                // Fait tourner la planète autour de son centre en fonction de l’entrée
                world_center.transform.Rotate(new Vector3(moveInput.x, 0f, moveInput.y) * rotate_speed * Time.deltaTime, Space.World);
                //Debug.Log("hit Object " + hit.collider.gameObject.name);
            }
        }
        anim.SetFloat("speed", mag);
        
        /*
        if(transform.position.y < 20)
        {
            transform.position = new Vector3(0, 40, 0);
        }
        */

    }

    void LateUpdate()
    {
        Update_jump();
    }
}
