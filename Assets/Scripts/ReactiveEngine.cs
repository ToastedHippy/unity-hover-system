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

    public Vector3 forceDirection => _nozzle.transform.TransformDirection(Vector3.up);
    
    private Rigidbody _rigidBody;

    private bool _started = false;

    public void startEngine() {
        _started = true;
    }

    public void stopEngine() {
        _started = false;
    }

    public void thrust(float magnitude) {
        if (_started) {
            Vector3 force = forceDirection * magnitude;

            _rigidBody.AddForce(force, ForceMode.Force);

            Debug.DrawRay(transform.position, force * 0.01f, Color.yellow);
        }
    }

    public void rotateNozzleInDirection(NozzleRotationDirection direction ) {

        float newX = direction == NozzleRotationDirection.Forward 
            ? nozzleRotationSpeed * Time.deltaTime + _nozzleRotation.x 
            : direction == NozzleRotationDirection.Backward
                ? -nozzleRotationSpeed * Time.deltaTime + _nozzleRotation.x
                : _nozzleRotation.x;

        float newZ = direction == NozzleRotationDirection.Right 
            ? -nozzleRotationSpeed * Time.deltaTime + _nozzleRotation.z 
            : direction == NozzleRotationDirection.Left 
                ? nozzleRotationSpeed * Time.deltaTime + _nozzleRotation.z
                : _nozzleRotation.z;

        Vector3 newEulers = new Vector3(newX, 0 , newZ);  

        if(newEulers.x > nozzleRotationLimitMax.x) {
            newEulers.x = nozzleRotationLimitMax.x;
        }
        if(newEulers.z > nozzleRotationLimitMax.z) {
            newEulers.z = nozzleRotationLimitMax.z;
        }

        if(newEulers.x < nozzleRotationLimitMin.x) {
            newEulers.x = nozzleRotationLimitMin.x;
        }
        if(newEulers.z < nozzleRotationLimitMin.z) {
            newEulers.z = nozzleRotationLimitMin.z;
        }

        _rotateNozzle(newEulers);
        
    }

    private void _rotateNozzle(Vector3 eulers) {

        if (!eulers.Equals(_nozzleRotation)) {
            _nozzleRotation = eulers;
            _nozzle.localRotation = Quaternion.Euler(eulers);
        }
    }

    public void rotateNozzleToDefault(NozzleRotationAxis axis) {

        float newX = _nozzleRotation.x;
        float newZ = _nozzleRotation.z;

        if (axis == NozzleRotationAxis.X) {
            newX = _nozzleRotation.x > 0
                ? _nozzleRotation.x - nozzleRotationSpeed * Time.deltaTime
                : _nozzleRotation.x < 0
                    ? _nozzleRotation.x + nozzleRotationSpeed * Time.deltaTime
                    : 0;
        }


        if (axis == NozzleRotationAxis.Z) {
            newZ = _nozzleRotation.z > 0
                ? _nozzleRotation.z - nozzleRotationSpeed * Time.deltaTime
                : _nozzleRotation.z < 0
                    ? _nozzleRotation.z + nozzleRotationSpeed * Time.deltaTime
                    : 0;
        }

        Vector3 newEulers = new Vector3(newX, 0 , newZ);

        _rotateNozzle(newEulers);
    }

    void Start() {
        _rigidBody = (Rigidbody)GetComponent(typeof(Rigidbody));
        _nozzle = transform.Find("nozzle");
    }

    void Update() {
    }
}