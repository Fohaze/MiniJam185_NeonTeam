using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemperatureChecker : MonoBehaviour
{
    [SerializeField]
    private GameObject planet;

    [SerializeField]
    private GameObject sun;

    [SerializeField]
    private GameObject player;
    
    public float GetTemperature(Vector3 position){
        Vector3 playerDirection = (position - planet.transform.position).normalized;
        Vector3 sunDirection = (sun.transform.position - planet.transform.position).normalized;

        float angle = Vector3.Angle(playerDirection, sunDirection);

        return Mathf.Lerp(50, -50, angle / 180f);
    }
}
