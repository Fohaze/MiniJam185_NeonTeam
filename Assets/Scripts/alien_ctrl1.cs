using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class alien_ctrl1 : MonoBehaviour
{
    // Référence à ton InputActionAsset (assignée dans l’Inspector)
    public InputActionAsset actions;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction interactAction;
    private InputAction rotateAction;
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
    public float rotate_stick = 0f;
    public float rotate_speed_y = 45f;
    public drop_elem dropElem;
    //timeline
    public PlayableDirector planetRotatorDirector;
    //séquence a faire jouer a la timeline
    public TimelineAsset director;

    public float offset_to_ground;
    public float marge_to_magnet_ground = 0.5f;
    public float current_height = 0f;
    public bool is_on_aire;
    public float sensy_dead_zone = 0.1f;
    private bool isGrounded;

    [Header("Carry Impact")]
    [Tooltip("Script de collecte pour compter les objets portés")] public CollectItems collectItems;
    [Header("Carry Speeds")]
    [Tooltip("Vitesse quand 0 objets portés")] public float speed0 = 15f;
    [Tooltip("Vitesse quand 1 objet porté")] public float speed1 = 12f;
    [Tooltip("Vitesse quand 2 objets portés")] public float speed2 = 10f;
    [Tooltip("Vitesse quand 3 objets portés")] public float speed3 = 8f;
    [Tooltip("Vitesse quand 4+ objets portés")] public float speed4 = 5f;

    public void Stop_Controlle()
    {
        // désactive le script
        this.enabled = false;
    }
    public void Start_Controlle()
    {
        // active le script
        this.enabled = true;
    }

    private void OnEnable()
    {
        var map = actions.FindActionMap("AlienMap");
        // Directions
        moveAction = map.FindAction("Directions");
        moveAction.Enable();

        // Saut
        jumpAction = map.FindAction("Sauter");
        jumpAction.performed += jump_action;
        jumpAction.Enable();

        // Interaction
        interactAction = map.FindAction("Interagir");
        interactAction.performed += OnInteractPerformed;
        interactAction.Enable();

        // Rotation stick
        rotateAction = map.FindAction("dir_x");
        rotateAction.performed += OnRotatePerformed;
        rotateAction.canceled += OnRotateCanceled;
        rotateAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        if (jumpAction != null)
        {
            jumpAction.performed -= jump_action;
            jumpAction.Disable();
        }
        if (interactAction != null)
        {
            interactAction.performed -= OnInteractPerformed;
            interactAction.Disable();
        }
        if (rotateAction != null)
        {
            rotateAction.performed -= OnRotatePerformed;
            rotateAction.canceled -= OnRotateCanceled;
            rotateAction.Disable();
        }
    }

    // Handlers
    private void OnInteractPerformed(InputAction.CallbackContext ctx) => interact_act();
    private void OnRotatePerformed(InputAction.CallbackContext ctx) => rotate_stick = ctx.ReadValue<float>();
    private void OnRotateCanceled(InputAction.CallbackContext ctx) => rotate_stick = 0f;

    void Update_jump()
    {
        var jump_h = anim.GetFloat("jump_h");
        if (jump_h != 0f)
        {
            is_on_aire = true;
            var pos = transform.position;
            pos.y = start_jump_pos.y + (jump_h * jump_hauteur);
            transform.position = pos;
            return;
        }
        else
        {
            start_jump_pos = transform.position;
            is_on_aire = false;
        }
        //raycast sphère
        //if(!Physics.SphereCast(transform.position, 0.5f, Vector3.down, out RaycastHit hit, 1.8f))
        if(!Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.8f))
        {
            //si la normal du sol est a moins de 45° de la verticale
            if(hit.normal.y > 0.7f)
            {
                //on ne tombe pas
                return;
            }
            var max_speed = speed * 0.5f;
            vel_fall += fall_speed * Time.deltaTime;
            if(vel_fall > max_speed)
                vel_fall = max_speed;
            transform.position += Vector3.down * vel_fall * Time.deltaTime;
            
            //transform.position += Vector3.down * fall_speed * Time.deltaTime;
        }

        
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
        if (dropElem != null && dropElem.is_dockable)
        {
            //lancer la cinematique de dock
            if (planetRotatorDirector != null)
            {
                //métre la bonne cinématique dans le director
                if (director != null)
                {
                    planetRotatorDirector.playableAsset = director;
                    planetRotatorDirector.time = 0; // Revenir au début de la timeline
                    planetRotatorDirector.Play();
                }
                else
                {
                    Debug.LogError("PlayableDirector is not assigned in alien_ctrl1.");
                }
            }
            else
            {
                Debug.LogError("PlanetRotatorDirector is not assigned in alien_ctrl1.");
            }
        }
        if (dropElem == null || !dropElem.is_can_take)
            return;
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


    

    public void jump_action(InputAction.CallbackContext context)
    {
        // N’autorise le saut que si on est au sol
        if (!isGrounded) return;
        // Appel de la méthode de saut
        anim.SetTrigger("jump");
        start_jump_pos = transform.position;
        jump_time = 0f;
    }

    public void place_to_ground()
    {
        var hit = Physics.SphereCast(transform.position, 0.5f, Vector3.down, out RaycastHit hitInfo, marge_to_magnet_ground + offset_to_ground);
        Debug.DrawRay(transform.position, Vector3.down * (marge_to_magnet_ground + offset_to_ground), Color.red, 0.1f);
        if (hit)
        {
            var player_pos = transform.position;
            var hit_pos = hitInfo.point;
            player_pos.y = hit_pos.y + offset_to_ground;
            transform.position = player_pos;
            current_height = player_pos.y;
        }
        else
        {
            Debug.LogWarning("No ground detected to place the character.");
        }
    }

    private void Start()
    {
        // Référence automatique si non assignée
        if (collectItems == null)
            collectItems = GetComponent<CollectItems>();
    }

    private void Update()
    {
        // Ajuste la vitesse selon le nombre d'objets portés
        int carryCount = collectItems != null ? collectItems.InventoryCount : 0;
        float currSpeed = carryCount == 0 ? speed0 :
                           carryCount == 1 ? speed1 :
                           carryCount == 2 ? speed2 :
                           carryCount == 3 ? speed3 : speed4;
        // On ne modifie que rotate_speed pour le mouvement
        rotate_speed = currSpeed;

        // Vérifie si on est au sol
        isGrounded = Physics.Raycast(transform.position, Vector3.down, out RaycastHit groundHit, marge_to_magnet_ground + offset_to_ground + 0.1f);
        // Lit la valeur Vector2 de l’action
        Vector2 inputVector = moveAction.ReadValue<Vector2>();
        // Ici tu peux l’utiliser pour déplacer ton personnage…
        moveInput = inputVector;

        var y_cam = cam.transform.eulerAngles.y;
        var y_char = transform.eulerAngles.y;
        var mag = moveInput.magnitude;

        //orientation du personnage en fonction du joystick et de la caméra
        if (mag > sensy_dead_zone)
        {
            var angle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg + 90f;
            transform.eulerAngles = new Vector3(0, y_cam + angle, 0);
        }
        // Déplacement du personnage
        if (mag > sensy_dead_zone)
        {
            var moveDir = Quaternion.Euler(0, y_cam, 0) * new Vector3(moveInput.x, 0, moveInput.y);
            //transform.position += moveDir * speed * Time.deltaTime;
            /*var w_an = world_center.transform.eulerAngles;
            w_an.z += moveInput.y * rotate_speed * Time.deltaTime;
            w_an.x += moveInput.x * rotate_speed * Time.deltaTime;
            world_center.transform.eulerAngles = w_an;
            */
            
            moveDir = -transform.right; 
            var hit = Physics.Raycast(transform.position, moveDir, out RaycastHit hitInfo, 1f);
            //var hit = Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 1f);
            var col_ = Color.red;
            if(!hit)
            //spherecast pour éviter les collisions
            //if(!Physics.SphereCast(transform.position, 0.5f, moveDir, out RaycastHit hit, 1f))
            {
                // Inversion des directions pour correspondre aux inputs
                world_center.transform.Rotate(new Vector3(-moveInput.x, 0f, -moveInput.y) * rotate_speed * Time.deltaTime, Space.World);
            }
            else
            {
                col_ = Color.green;
            }
            Debug.DrawRay(transform.position, moveDir * 10f, col_, 0.1f);
        }
        // Rotate planet yaw based on stick
        if (Mathf.Abs(rotate_stick) > sensy_dead_zone)
        {
            // Inversion du stick pour sens de rotation
            world_center.transform.Rotate(Vector3.up * -rotate_stick * rotate_speed_y * Time.deltaTime, Space.World);
        }
        anim.SetFloat("speed", mag);
        
        /*
        if(transform.position.y < 20)
        {
            transform.position = new Vector3(0, 40, 0);
        }
        */
        place_to_ground();
    }

    void LateUpdate()
    {
        Update_jump();
    }
}
