using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Harpon : MonoBehaviour
{
    
    public InputActionAsset actions;
    public GameObject player;
    public GameObject arrow;
    public GameObject harpon;
    public GameObject uiInteract;
    public GameObject harponPos0;
    public GameObject harponPos1;
    public GameObject firePosition;
    public GameObject objectsRecupPosition;
    public LineRenderer lineRenderer;
    public AnimationCurve harponCurve;
    public GameObject rotator;
    public float rotateSpeed = 10f;
    public float harponSpeed = 10f;
    public float harponTime = 10f;
    public List<GameObject> objectsToPick;

    bool canInteract = false;
    

    void OnEnable(){
        var map = actions.FindActionMap("AlienMap");
        // Récupère le bouton "Interagir"
        var interactAction = map.FindAction("Interagir");

        interactAction.performed += ctx => interact_act();
        interactAction.Enable();
    }   

    void OnDisable(){
        /*interactAction.performed -= ctx => interact_act();
        interactAction.Disable();*/
    }

    // Update is called once per frame
    void Update()
    {
        canInteract = Vector3.Distance(player.transform.position, firePosition.transform.position) < 1.5f;
        uiInteract.SetActive(canInteract);
        rotator.transform.localRotation *= Quaternion.Euler(rotateSpeed * Time.deltaTime, 0, 0);
        lineRenderer.SetPosition(0, arrow.transform.position);
        lineRenderer.SetPosition(1, harpon.transform.position);
        arrow.transform.position = Vector3.Lerp(harponPos0.transform.position, harponPos1.transform.position, harponCurve.Evaluate(harponTime));
        harponTime += Time.deltaTime;
        GameObject pick = null;
        if(harponTime < harponCurve.keys[harponCurve.keys.Length - 1].time){
            for(int i = objectsToPick.Count - 1; i >= 0; i--){
                if(Vector3.Distance(arrow.transform.position, objectsToPick[i].transform.position) < 3){
                    pick = objectsToPick[i];
                    pick.transform.parent = arrow.transform;
                    objectsToPick.RemoveAt(i);
                }
            }
        }
        else{
            if(pick != null){
                pick.transform.parent = harpon.transform;
                pick.transform.position = objectsRecupPosition.transform.position;
            }
        }
    }

    public void interact_act()
    {
        Debug.Log("TRY INTERACT HARPON");
        if(canInteract){
            harponTime = 0;
        }
    }
}
