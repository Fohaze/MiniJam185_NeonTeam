using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class position_ctrl_i1 : MonoBehaviour
{

    public float lat, lon; 
    public float offset_lat = 0f, offset_lon = 0f, offset_y = 0f;
    public GameObject world_center;
    public float length = 100f;
    public planetRotor2 planetRot;
    public bool place, place_2;

    void Start()
    {
    }

    public void SetPosition(float lat, float lon)
    {
        //placer la planette de façon a ce que le point en haut soit celui de lat lon

        //var 
        planetRot.SetRotator(lat, lon);
    }

    public void place_porte_1_setup_cinematic()
    {
        // place la porte 1 dans la position de la cinématique
        lat = -37f;
        lon = 35f;
        offset_lat = 0f;
        offset_lon = 0f;
        offset_y = 0f;

        SetPosition(lat, lon);
    }

    public void place_elevator_down_setup_cinematic()
    {
        // place l'ascenseur dans la position de la cinématique
        lat = -240f;
        lon = 180f;
        offset_lat = 0f;
        offset_lon = 0f;
        offset_y = 0f;

        SetPosition(lat, lon);
    }

    void Update()
    {
        
        // lat lon dans l'espace de world_center
        var dir_base = world_center.transform.forward;
        Debug.DrawRay(world_center.transform.position, dir_base * length, Color.red);
        var dir_lat = Quaternion.AngleAxis(lat + offset_lat, world_center.transform.right) * dir_base;
        Debug.DrawRay(world_center.transform.position, dir_lat * length, Color.green);
        var dir_lon = Quaternion.AngleAxis(lon + offset_lon, world_center.transform.up) * dir_base;
        Debug.DrawRay(world_center.transform.position, dir_lon * length, Color.blue);

        // lat + lon dans l'espace de world_center
        var dir_lat_lon = Quaternion.AngleAxis(lat + offset_lat, world_center.transform.right) * Quaternion.AngleAxis(lon + offset_lon, world_center.transform.up) * dir_base;
        Debug.DrawRay(world_center.transform.position, dir_lat_lon * length, Color.yellow);

        if (place)
        {
            
            // point de spawn
            //SetPosition(-34f, 43.3f);
            var lat_ = -38; // latitude
            var lon_ = 63f; // longitude

           // porte 1
            //var lat_ = -37f;
            //var lon_ = 35f;

            SetPosition(lat_, lon_);

            place = false;
        }
        if (place_2)
        {
            //placer a l'endroit de l'ascenseur
            place_elevator_down_setup_cinematic();
            place_2 = false;
        }
    }
}
