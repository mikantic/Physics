using System;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _speed;
    void FixedUpdate()
    {
        _rigidbody.MoveRotation(_rigidbody.rotation * Quaternion.Euler(0, 0, _speed * Time.fixedDeltaTime));
    }
}
