using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ReactiveEngine : MonoBehaviour
{

    public float hoverHeight = 2.0f;
    
    private Rigidbody _rigidBody;

    public bool started = false;

    

    public void startEngine() {
        started = true;
    }

    public void stopEngine() {
        started = false;
    }

    public void rotate() {
        
    }

    public void thrust(float magnitude) {
        if (started) {
            Vector3 force = transform.TransformDirection(Vector3.up) * magnitude;
            _rigidBody.AddForce(force, ForceMode.Force);

            Debug.DrawRay(transform.position, force * 0.01f, Color.yellow);
        }
    }

    void Start() {
        _rigidBody = (Rigidbody)GetComponent(typeof(Rigidbody));
    }

    void Update() {
        
    }

    void FixedUpdate() {
        if (!started) { return; }
    }
}