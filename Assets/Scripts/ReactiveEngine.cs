using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ReactiveEngine : MonoBehaviour
{
    
    private Rigidbody _rigidBody;

    private bool _started = false;
    private Vector3 _currentForce = Vector3.zero;

    public void startEngine() {
        _started = true;
    }

    public void stopEngine() {
        _started = false;
    }

    public void thrust(float magnitude) {
        if (_started) {
            Vector3 force = transform.TransformDirection(Vector3.up) * magnitude;

            _rigidBody.AddForce(force, ForceMode.Force);

            _currentForce = force;

            Debug.DrawRay(transform.position, force * 0.01f, Color.yellow);
        }
    }

    void Start() {
        _rigidBody = (Rigidbody)GetComponent(typeof(Rigidbody));
    }

    void Update() {
    }
}