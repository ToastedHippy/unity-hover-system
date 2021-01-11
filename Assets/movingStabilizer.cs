using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// по идее эно закрылки (flaps)
public class movingStabilizer : MonoBehaviour
{

    private bool _active = false;
    private Rigidbody _rigidBody;

    private float _currentForce = 0f;

    public float maxForce = 1000f;

    public void activate() {
        _active = true;
    }

    public void deactivate() {
        _active = false;
    }

    public void stabilize() {

        if (!_active) { 
            return; 
        }

        Vector3 v = _rigidBody.velocity;
        float vM = Vector3.Magnitude(v);

        

        float angle = Vector3.Angle(v, transform.TransformDirection(Vector3.up));
        bool needToStabilize = (Mathf.Round(vM * 10) / 10 > 0 && angle < 90 && _currentForce > 0);

        if (needToStabilize) {

            Debug.Log(_currentForce);
            
            float stabilizationM = _currentForce;

            Vector3 force = transform.TransformDirection(Vector3.down) * stabilizationM;

            Debug.DrawRay(transform.position, force, Color.green);

            _rigidBody.AddForce(force);
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * stabilizationM * 0.01f);
        }


    }

    public void increaseForce(float speed) {
        float newForce = _currentForce + speed * Time.deltaTime;
        
        if (newForce >= maxForce) {
            _currentForce = maxForce;
        } else {
            _currentForce = newForce;
        }
    }

    public void decreaseForce(float speed) {
        float newForce = _currentForce - speed * Time.deltaTime;

        if (newForce <= 0) {
            _currentForce = 0;
        } else {
            _currentForce = newForce;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = (Rigidbody)GetComponent(typeof(Rigidbody));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() {
        stabilize();
    }
}
