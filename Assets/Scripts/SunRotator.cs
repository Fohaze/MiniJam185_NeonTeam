using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunRotator : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private Vector3 rotationEulers = Vector3.zero;
    [SerializeField] private Transform rotator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rotator.Rotate(rotationEulers * speed * Time.deltaTime);
    }
}
