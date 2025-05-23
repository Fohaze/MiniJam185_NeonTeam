using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetRotator : MonoBehaviour
{
    public GameObject rotatorX;
    public GameObject rotatorZ;


    public void Rotate(float x, float y, float rotate_speed)
    {
            var w_an = transform.eulerAngles;
            w_an.z += y * rotate_speed * Time.deltaTime;
            w_an.x += x * rotate_speed * Time.deltaTime;
            transform.eulerAngles = w_an;
}
}
