using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fire_detect_sys : MonoBehaviour
{

    public FireCamp fireCamp;
    public door_cmp door_cmp;
    public bool is_fire_detected = false;

    void Start()
    {
        
    }

    void Update()
    {

        var cnt = fireCamp.fireValue;

        door_cmp.state = cnt <= 0f;
    }
}
