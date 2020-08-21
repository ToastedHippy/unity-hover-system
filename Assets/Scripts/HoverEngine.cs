using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveDirection {
    none,
    forvard,
    backvard
}

public enum RotationDirection {
    none,
    right,
    left
}

public class HoverEngine : MonoBehaviour
{

    public float hoverHeight = 2.0f;
    public float movingAcceleration = 1.0f;

    public float rotationAcceleration = 1.0f;
    
    private Rigidbody _rigidBody;

    public bool started = true;

    private MoveDirection movingDirection = MoveDirection.none;

    private RotationDirection _rotationDirection = RotationDirection.none;

    public float powerPercent = 1.0f;

    public float maxHoverForcePercent = 1.5f;

    public void startEngine() {
        started = true;
    }

    public void stopEngine() {
        started = false;
    }

    private void _calcAndApplyHover(float distanceFromSurface) {

        // float powerPercent = 

        // powerPercent = powerPercent < 0 
        // ? 0 
        // : powerPercent > 1
        //     ? 1
        //     : powerPercent;

        
        float needForce = (_rigidBody.mass + 10) * 9.81f;
        float maxForce = needForce * maxHoverForcePercent;
        float maxDistance = hoverHeight * 15;
        float forceDiff = maxForce - needForce;

        float hoverForce = 0.0f;

        if (distanceFromSurface < hoverHeight) {
            hoverForce = Mathf.Clamp((1 - distanceFromSurface / hoverHeight) * forceDiff, 0, forceDiff) + needForce;
        }
        else {
            hoverForce = Mathf.Clamp((1 - ((distanceFromSurface - hoverHeight) / (maxDistance - hoverHeight))) * needForce, 0, needForce);
        }

        Vector3 force = transform.TransformDirection(Vector3.up) * hoverForce;

        // Debug.Log(force);

        _rigidBody.AddForce(force, ForceMode.Force);
    }


    private void _stabilize(float distanceFromSurface) {
        Vector3 v = _rigidBody.velocity;
        float vM = Vector3.Magnitude(v);

        // Debug.DrawRay(transform.position, v, Color.green, 0.2f);
        // Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up), Color.gray, 0.2f);

        float angle = Vector3.Angle(v, transform.TransformDirection(Vector3.up));
        bool needToStabilize = (Mathf.Round(vM * 10) / 10 > 0 && angle < 90);

        if (needToStabilize) {
            
            
            float stabilizationM = vM / Mathf.Sqrt(3);

            stabilizationM = stabilizationM * (1 - angle / 90) * 400;

            _rigidBody.AddForce(Vector3.down * stabilizationM);

        }

    }

    public void updateMovingEngineState(MoveDirection direction) {
        movingDirection = direction;
    }

    public void updateRotatingEngineState(RotationDirection direction) {
        _rotationDirection = direction;
    }

    // Start is called before the first frame update
    void Start() {
        _rigidBody = (Rigidbody)GetComponent(typeof(Rigidbody));
    }

    // Update is called once per frame
    void Update() {
        
    }

    void FixedUpdate() {
        if (!started) { return; }

        RaycastHit hit;
        Vector3 castDirection = transform.TransformDirection(Vector3.down);
        
        Debug.DrawRay(transform.position, castDirection * hoverHeight * 8, Color.yellow);
        
        if (Physics.Raycast(transform.position, castDirection, out hit, hoverHeight * 8))
        {
            Debug.Log(hit.distance);
            _calcAndApplyHover(hit.distance);
            _stabilize(hit.distance);        
        }

        

        switch (movingDirection)
        {
            case MoveDirection.forvard:
                _rigidBody.AddForce(transform.TransformDirection(Vector3.forward) * movingAcceleration, ForceMode.Force);
                break;
            case MoveDirection.backvard:
                _rigidBody.AddForce(transform.TransformDirection(Vector3.back) * movingAcceleration, ForceMode.Force);
                break;
            default:
                break;
        }

        switch (_rotationDirection)
        {
            case RotationDirection.right:
                _rigidBody.AddForce(transform.TransformDirection(Vector3.right) * rotationAcceleration, ForceMode.Force);
                break;
            case RotationDirection.left:
                _rigidBody.AddForce(transform.TransformDirection(Vector3.left) * rotationAcceleration, ForceMode.Force);
                break;
            default:
                break;
        }


    }
}