using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class door_cmp : MonoBehaviour
{
    public Animator anim;
    public bool state;
    public void SetOpen()
    {
        anim.SetBool("open", true);
        state = true;
    }
    public void SetClose()
    {
        anim.SetBool("open", false);
        state = false;
    }
    void Start()
    {
        
    }

    void Update()
    {

        if (anim.GetBool("open") && !state)
        {
            SetClose();
        }
        if (!anim.GetBool("open") && state)
        {
            SetOpen();
        }

        
    }
}
