using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_sys_i1 : MonoBehaviour
{
    public Animator anim;
    public GameObject player;
    public alien_ctrl1 alien_ctrl;
    public float stage_1_height = -76f;
    public float stage_2_height = -70f;

    public void set_top_view()
    {
        anim.SetBool("top_view", true);
    }
    void Start()
    {
        
    }

    void Update()
    {
        if (alien_ctrl == null)
        {
            alien_ctrl = GameObject.FindObjectOfType<alien_ctrl1>();
        }
        if (alien_ctrl != null && alien_ctrl.current_height > stage_1_height)
        {
            anim.SetBool("stage_1_cam", true);
        }
        else
        {
            anim.SetBool("stage_1_cam", false);
        }
        if (alien_ctrl != null && alien_ctrl.current_height > stage_2_height)
        {
            anim.SetBool("stage_2_cam", true);
        }
        else
        {
            anim.SetBool("stage_2_cam", false);
        }
        
    }
}
