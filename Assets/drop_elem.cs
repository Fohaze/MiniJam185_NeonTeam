using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class drop_elem : MonoBehaviour
{

    public GameObject ui_can_take;
    public bool is_can_take = false;
    public bool has_element = false;
    public GameObject object_to_drop;
    public GameObject batterie_in_hand;
    public float drop_dist = 1f;
    public float current_dist = 0f;
    public GameObject target_dock;
    public bool is_dockable = false;
    public Animator anim;

    void Start()
    {
        
    }

    public void drop_batterie()
    {
        if (has_element)
        {
            has_element = false;
            ui_can_take.SetActive(false);
            batterie_in_hand.SetActive(false);

        }
    }

    public void take_batterie()
    {
        has_element = true;
        ui_can_take.SetActive(false);
        batterie_in_hand.SetActive(true);
        object_to_drop.SetActive(false);
        var anim = GetComponent<Animator>();
        anim.SetBool("porte", true);
    }

    public void valid_element()
    {
        batterie_in_hand.SetActive(false);
        anim.SetBool("porte", false);
        //détruire ce composant
        Destroy(this);
    }

    public void has_and_click()
    {
        if (has_element && is_dockable)
        {
            valid_element();
        }
    }

    void Update()
    {
        var diff = object_to_drop.transform.position - transform.position;
        if (!has_element)
        {
            current_dist = diff.magnitude;
        }
        else
        {
            current_dist = Vector3.Distance(transform.position, target_dock.transform.position);
        }
        if (current_dist < drop_dist)
        {
            ui_can_take.SetActive(true);
            if (has_element == false)
            {
                is_can_take = true;
            }
            else
            {
                is_dockable = true;
            }
        }
        else
        {
            ui_can_take.SetActive(false);
            is_can_take = false;
            is_dockable = false;
        }
    }
}
