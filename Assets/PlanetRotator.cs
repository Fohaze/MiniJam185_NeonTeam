using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetRotator : MonoBehaviour
{
    public GameObject rotator;
    public GameObject planetParent;


    public void Rotate(float x, float y, float z, float rotate_speed)
    {   Vector3 planetRot = planetParent.transform.eulerAngles;
        rotator.transform.rotation = Quaternion.Euler(0, 0, 0);
        planetParent.transform.eulerAngles = planetRot;
        
        var rotEulers = rotator.transform.eulerAngles;
        rotEulers.x += x * rotate_speed * Time.deltaTime;
        rotEulers.y += y * rotate_speed * Time.deltaTime;
        rotEulers.z += z * rotate_speed * Time.deltaTime;
        rotator.transform.eulerAngles = rotEulers;
    }

    public void Rotate(float x, float z, float rotate_speed)
    {   Vector3 planetRot = planetParent.transform.eulerAngles;
        rotator.transform.rotation = Quaternion.Euler(0, 0, 0);
        planetParent.transform.eulerAngles = planetRot;
        
        var rotEulers = rotator.transform.eulerAngles;
        rotEulers.x += x * rotate_speed * Time.deltaTime;
        rotEulers.z += z * rotate_speed * Time.deltaTime;
        rotator.transform.eulerAngles = rotEulers;
    }
}
