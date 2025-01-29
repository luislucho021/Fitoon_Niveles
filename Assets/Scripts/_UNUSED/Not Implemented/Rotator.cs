using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float forceToApply = 40;

    private Rigidbody rB;

    // Start is called before the first frame update
    void Start()
    {
        rB = GetComponent<Rigidbody>();
        rB.maxAngularVelocity = 1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rB.AddRelativeTorque(new Vector3(0, forceToApply, 0), ForceMode.Impulse);
    }
}
