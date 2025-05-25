using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dead_sys : MonoBehaviour
{
    public GameObject player;
    public GameObject closest_checkpoint;
    public planetRotor2 pr2;
    public bool to_respawn;

    void Start()
    {
        
    }

    public void Respawn()
    {
        var lat= 30;
        var lon = 30;

        pr2.SetRotator(lat, lon);
    }

    void Update()
    {
        if (to_respawn)
        {
            Respawn();
            to_respawn = false;
        }

    }
}
