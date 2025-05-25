using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class planetRotor2 : MonoBehaviour
{
    
    public GameObject rotator;
    public GameObject planetParent;
    public Vector3 currentRotation;

    public void SetRotator(float x, float y, float z)
    {
        Vector3 planetRot = planetParent.transform.eulerAngles;
        rotator.transform.rotation = Quaternion.Euler(0, 0, 0);
        planetParent.transform.eulerAngles = planetRot;

        var rotEulers = rotator.transform.eulerAngles;
        rotEulers.x = x;
        rotEulers.y = y;
        rotEulers.z = z;
        rotator.transform.eulerAngles = rotEulers;
        //currentRotation = rotator.transform.eulerAngles;
    }

    public void SetRotator(float lat, float lon)
    {
        var an = new Vector3(lat, 0f, lon);

        planetParent.transform.eulerAngles = an;
    }

    public void Rotate(float x, float y, float z, float rotate_speed)
    {   Vector3 planetRot = planetParent.transform.eulerAngles;
        rotator.transform.rotation = Quaternion.Euler(0, 0, 0);
        planetParent.transform.eulerAngles = planetRot;
        
        var rotEulers = rotator.transform.eulerAngles;
        rotEulers.x += x * rotate_speed * Time.deltaTime;
        rotEulers.y += y * rotate_speed * Time.deltaTime;
        rotEulers.z += z * rotate_speed * Time.deltaTime;
        rotator.transform.eulerAngles = rotEulers;
        currentRotation = rotator.transform.eulerAngles;
    }

    public void Rotate(float x, float z, float rotate_speed)
    {   Vector3 planetRot = planetParent.transform.eulerAngles;
        rotator.transform.rotation = Quaternion.Euler(0, 0, 0);
        planetParent.transform.eulerAngles = planetRot;
        
        var rotEulers = rotator.transform.eulerAngles;
        rotEulers.x += x * rotate_speed * Time.deltaTime;
        rotEulers.z += z * rotate_speed * Time.deltaTime;
        rotator.transform.eulerAngles = rotEulers;
        currentRotation = rotator.transform.eulerAngles;
    }
}
