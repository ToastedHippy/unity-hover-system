using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stabilizer : MonoBehaviour
{

    private bool _active = false;
    private Rigidbody _rigidBody;

    public void activate() {
        _active = true;
    }

    public void deactivate() {
        _active = false;
    }

    public void stabilize() {

        if (!_active) { return; }

        Vector3 v = _rigidBody.velocity;
        float vM = Vector3.Magnitude(v);

        // Debug.DrawRay(transform.position, v, Color.green, 0.2f);
        // Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up), Color.gray, 0.2f);

        float angle = Vector3.Angle(v, transform.TransformDirection(Vector3.up));
        bool needToStabilize = (Mathf.Round(vM * 10) / 10 > 0 && angle < 90);

        // Debug.Log(angle);

        if (needToStabilize) {
            
            float stabilizationM = (vM / Mathf.Sqrt(3)) * (1 - angle / 90) * 400;

            _rigidBody.AddForce(Vector3.down * stabilizationM);
        }

    }

    void Start()
    {
        _rigidBody = (Rigidbody)GetComponent(typeof(Rigidbody));
    }

    void Update()
    {
        
    }

    void FixedUpdate() {
        stabilize();
    }
}
