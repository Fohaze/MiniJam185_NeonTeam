using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class door_cmp : MonoBehaviour
{
    public Animator anim;

    public void SetOpen()
    {
        anim.SetBool("open", true);
    }
    public void SetClose()
    {
        anim.SetBool("open", false);
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
