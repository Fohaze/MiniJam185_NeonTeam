using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class taker : MonoBehaviour
{
    public List<GameObject> objectsToTake;

    public GameObject player;
    public GameObject ui;
    public float dist_taking = 2f;
    public InputActionAsset actions;

    public float current_dist = 0f;
    public Text text_score;

    void OnEnable()
    {
        var map = actions.FindActionMap("AlienMap");

        // Récupère l’ActionMap "basic_move"
        // Récupère interaction
        var interactAction = map.FindAction("Interagir");
        interactAction.performed += ctx => take_object();

        interactAction.Enable();

        
    }

    void Start()
    {
        
    }

    public void take_object()
    {
        var obj = closest_object();
        Debug.Log("Take object");
        var a = closest_object();
        var tmp = a.transform.position - player.transform.position;
        if (tmp.magnitude > dist_taking)
        {
            Debug.Log("Trop loin");
            return;
        }
        //enlever l'objet de la liste
        objectsToTake.Remove(obj);
        //desactiver l'objet
        Destroy(obj);
    }

    GameObject closest_object()
    {
        GameObject closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject obj in objectsToTake)
        {
            float distance = Vector3.Distance(player.transform.position, obj.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = obj;
            }
        }

        return closest;
    }
    
    void Update()
    {
        if (objectsToTake.Count == 0)
        {
            ui.SetActive(false);
            return;
        }


        var a = closest_object();
        var tmp = a.transform.position - player.transform.position;
        current_dist = tmp.magnitude;
        if (tmp.magnitude < dist_taking)
        {
            ui.SetActive(true);
        }
        else
        {
            ui.SetActive(false);
        }
        var n = objectsToTake.Count;
        var manque = n;
        text_score.text = "Il vous manque " + manque + " objets";

        
    }
}
