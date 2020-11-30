using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverController : MonoBehaviour
{
   
    public List<GameObject> reactiveEngines;
    public List<GameObject> stabilizers;
    public float hoverHeight = 1.0f;

    public float maxHoverForcePercent = 2.0f;

    public void startEngines() {
        
        foreach(GameObject re in reactiveEngines)
        {
            ReactiveEngine r = re.GetComponent<ReactiveEngine>();
            r.startEngine();
        }
    }

    public void moveForvard() {
        foreach(GameObject re in reactiveEngines)
        {
            ReactiveEngine r = re.GetComponent<ReactiveEngine>();
            r.rotateNozzleInDirection(NozzleRotationDirection.Forward);
        }
    }

    public void moveBackward() {
        foreach(GameObject re in reactiveEngines)
        {
            ReactiveEngine r = re.GetComponent<ReactiveEngine>();
            r.rotateNozzleInDirection(NozzleRotationDirection.Backward);
        }
    }

    public void moveToDefault() {
        foreach(GameObject re in reactiveEngines)
        {
            ReactiveEngine r = re.GetComponent<ReactiveEngine>();
            r.rotateNozzleToDefault(NozzleRotationAxis.X);
        }
    }

    public void turnRight() {
        foreach(GameObject re in reactiveEngines)
        {
            ReactiveEngine r = re.GetComponent<ReactiveEngine>();

            if (re.tag == "front reactive engine") {
                r.rotateNozzleInDirection(NozzleRotationDirection.Right);
            }

            if (re.tag == "back reactive engine") {
                r.rotateNozzleInDirection(NozzleRotationDirection.Left);
            }
            
        }
    }

    public void turnLeft() {
        foreach(GameObject re in reactiveEngines)
        {
            ReactiveEngine r = re.GetComponent<ReactiveEngine>();

            if (re.tag == "front reactive engine") {
                r.rotateNozzleInDirection(NozzleRotationDirection.Left);
            }

            if (re.tag == "back reactive engine") {
                r.rotateNozzleInDirection(NozzleRotationDirection.Right);
            }
            
        }
    }

    public void turnToDefault() {
        foreach(GameObject re in reactiveEngines)       
        {
            ReactiveEngine r = re.GetComponent<ReactiveEngine>();
            r.rotateNozzleToDefault(NozzleRotationAxis.Z);
        }
    }

    private float _calcHoverForce(float distanceFromSurface) {

        float needForce = (10 + 1 + 25f) * 9.81f; // engine + stabilizer + 0.25 car body mass
        float maxForce = needForce * maxHoverForcePercent;
        float forceDiff = maxForce - needForce;

        float hoverForce = 0.0f;

        if (distanceFromSurface < hoverHeight) {
            hoverForce = Mathf.Clamp((1 - distanceFromSurface / hoverHeight) * forceDiff, 0, forceDiff) + needForce;
        }
        else {
            hoverForce = Mathf.Clamp((1 - (distanceFromSurface - hoverHeight)) * needForce, 0, needForce);
        }

         return hoverForce;
    }

    private void _thrustEngines() {
        foreach(GameObject re in reactiveEngines)
        {
            _thrustEngine(re);
        }
    }

    private void _thrustEngine(GameObject engine) {
        RaycastHit hit;
        Vector3 castOrigin = engine.transform.position;
        Vector3 castDirection = engine.transform.TransformDirection(Vector3.down);
        
        ReactiveEngine re = (ReactiveEngine)engine.GetComponent(typeof(ReactiveEngine));

        if (Physics.Raycast(castOrigin, castDirection, out hit, hoverHeight * 2, ~(1 << 8)))
        {
            float hoverMagnitude = _calcHoverForce(hit.distance);
            // Debug.Log(engine.name + ' ' + hoverMagnitude + ' ' + hit.distance + ' ' + hit.collider.gameObject.name);
            Debug.DrawRay(castOrigin, castDirection * hit.distance, Color.red);

            float angle = Vector3.Angle(re.transform.TransformDirection(Vector3.up), re.forceDirection);

            float cos = Mathf.Cos(Mathf.Deg2Rad * angle);
            float thrustMagnitude = hoverMagnitude / cos;

            re.thrust(thrustMagnitude);      
        }
    }

    private void _activateStabilizers() {
        foreach (GameObject s in stabilizers)
        {
            Stabilizer st = s.GetComponent<Stabilizer>();
            st.activate();
        }
    }

    void FixedUpdate() {
        _thrustEngines();

    }
    void Start() 
    {
        startEngines();
        _activateStabilizers();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W)) {
            moveForvard();
        } else if (Input.GetKey(KeyCode.S)) {
            moveBackward();
        } else {
            moveToDefault();
        }

        if (Input.GetKey(KeyCode.A)) {
            turnLeft();
        } else if (Input.GetKey(KeyCode.D)) {
            turnRight();
        } else {
            turnToDefault();
        }

        
    }
    
}
