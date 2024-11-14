using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindTurbineBlades : MonoBehaviour
{
    [SerializeField] float baseSpeed = 50f;
    [SerializeField] WindZone windZone;
    // Update is called once per frame
    void Update()
    {
        float rotationSpeed = baseSpeed * (!windZone ? 1f : windZone.windMain);
        transform.Rotate(Vector3.forward, rotationSpeed*Time.deltaTime);
    }
}
