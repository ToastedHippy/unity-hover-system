using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NozzleRotationDirection {Right, Left, Forward, Backward}
public enum NozzleRotationAxis {X, Z}

public class ReactiveEngine : MonoBehaviour
{

    public Vector3 nozzleRotationLimitMax = new Vector3(40, 0, 20);
    public Vector3 nozzleRotationLimitMin = new Vector3(-40, 0, -20);

    public float nozzleRotationSpeed = 1f;
    private Transform _nozzle;

    private Vector3 _nozzleRotation = Vector3.zero;

    public Vector3 force = Vector3.zero;

    public Vector3 forceDirection => _nozzle.transform.TransformDirection(Vector3.up);
    
    private Rigidbody _rigidBody;

    private bool _started = false;

    public void startEngine() {
        _started = true;
    }

    public void stopEngine() {
        _started = false;
    }

    public void thrust(Vector3 f) {
        if (_started) {

            _rigidBody.AddForce(transform.TransformDirection(f), ForceMode.Force);

            force = f;

            Debug.DrawRay(transform.position, f * 0.01f, Color.yellow);

            _nozzle.localRotation = Quaternion.LookRotation(force.normalized * -1f);
        }
    }

    void Start() {
        _rigidBody = (Rigidbody)GetComponent(typeof(Rigidbody));
        _nozzle = transform.Find("nozzle");
    }

    void Update() {
    }
}