using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEngine.InputSystem;

public class orchestrator_i1 : MonoBehaviour
{

    [SerializeField] private PlayableDirector director;
    public InputActionAsset actions;
    public PlayerHealth playerH;

    public alien_ctrl1 alc1;
    public bool cinematic_is_played = false;
    public GameObject dead_sceen;

    
    private void OnEnable()
    {
        var map = actions.FindActionMap("AlienMap");
        var interactAction = map.FindAction("Interagir");

        interactAction.performed += ctx => skip_cinematic();
        interactAction.Enable();

        //playerH.onDeath += loose_screen; // Assure que la fonction loose_screen est appelée lors de la mort du joueur

    }

    void loose_screen()
    {
        Debug.Log("Player has died, showing loose screen.");
        dead_sceen.SetActive(true);
    }

    private void skip_cinematic()
    {
        cinematic_is_played = true;
        //frame 1616
        if (director != null)
        {
            //si l'on a pas déja joué dépassé la frame 1616
            if (director.time >= 1616)
            {
                Debug.Log("Cinematic already played or skipped.");
                return;
            }
            director.time = 1616; // Skip to frame 1616
            director.Evaluate();
        }
        else
        {
            Debug.LogError("PlayableDirector is not assigned in orchestrator_i1.");
        }
    }

    public void go_controlle()
    {
        // Assure que l'alien_ctrl1 est activé
        if (alc1 != null)
        {
            alc1.enabled = true;
        }
        else
        {
            Debug.LogError("alien_ctrl1 is not assigned in orchestrator_i1.");
        }
    }
    void Start()
    {
        
    }

    void Update()
    {
            if (director.time >= 1616)
            {
                cinematic_is_played = true;
            }

            /*
            if (currentHealth <= 0)
            {

            }
            */
        
    }
}
