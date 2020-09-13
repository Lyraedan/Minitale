using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast : MonoBehaviour
{

    public static Raycast instance;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public RaycastHit GetHit()
    {
        int layerMask = 1 << 8;
        layerMask = ~layerMask;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
        }
        return hit;
    }

    public bool IsWithinRange(Vector3 toCheck, float dist)
    {
        return Vector3.Distance(gameObject.transform.position, toCheck) < dist;
    }
}
