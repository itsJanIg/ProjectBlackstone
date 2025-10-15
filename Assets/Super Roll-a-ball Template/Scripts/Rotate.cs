using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Rotate : MonoBehaviour
{
    public Vector3 RotationSpeed = new Vector3();

    Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();  

        if (_rigidbody == null)
        {
            Debug.LogError("Rotate script requires a Rigidbody component!");
        }
    }

    void FixedUpdate()
    {
        if (_rigidbody != null)
        {
            Quaternion deltaRotation = Quaternion.Euler(RotationSpeed * Time.fixedDeltaTime);
            _rigidbody.MoveRotation(_rigidbody.rotation * deltaRotation);
        }
    }
}
