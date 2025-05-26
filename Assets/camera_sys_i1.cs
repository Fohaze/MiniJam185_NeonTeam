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
    public float current_dist = 1f;
    public float current_center_dist = 1f;
    public GameObject center;
    public GameObject perso;
    public dead_sys dead;
    public float fall_dist;

    public void set_top_view()
    {
        anim.SetBool("top_view", true);
    }
    void Start()
    {
        
    }

    void Update()
    {

        current_dist = perso.transform.position.y - player.transform.position.y;
        current_center_dist = perso.transform.position.y - center.transform.position.y;



        if (alien_ctrl == null)
        {
            alien_ctrl = GameObject.FindObjectOfType<alien_ctrl1>();
        }
        
        if (alien_ctrl != null &&  perso.transform.position.y < center.transform.position.y)
        {
            dead.Respawn();
            var a = perso.transform.position;
            //perso.transform.position = center.transform.position.y + stage_2_height;
            a.y = center.transform.position.y + stage_2_height;
            perso.transform.position = a;
        }

        if (alien_ctrl != null &&  current_center_dist> stage_1_height)
        {
            anim.SetBool("stage_1_cam", true);
        }
        else
        {
            anim.SetBool("stage_1_cam", false);
        }
        if (alien_ctrl != null && current_center_dist > stage_2_height)
        {
            anim.SetBool("stage_2_cam", true);
        }
        else
        {
            anim.SetBool("stage_2_cam", false);
        }
        
    }
}
