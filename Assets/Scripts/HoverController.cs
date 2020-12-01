using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverController : MonoBehaviour
{
   
    public List<GameObject> reactiveEngines;
    public List<GameObject> stabilizers;
    public float hoverHeight = 1.0f;

    public float maxHoverForcePercent = 2.0f;

    public float maxEngineForvardMovingForce = 500f;
    public float maxEngineBackvardMovingForce = 300f;
    public float movingAcceleration = 100f;
    public float maxEngineTurningForce = 500f;
    public float turningAcceleration = 100f;

    public void startEngines() {
        
        foreach(GameObject re in reactiveEngines)
        {
            ReactiveEngine r = re.GetComponent<ReactiveEngine>();
            r.startEngine();
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
            Vector3 hoverDirection = Vector3.up;
            Vector3 hoverForce = hoverDirection * hoverMagnitude;
            // Debug.Log(engine.name + ' ' + hoverMagnitude + ' ' + hit.distance + ' ' + hit.collider.gameObject.name);
            Debug.DrawRay(castOrigin, engine.transform.TransformDirection(hoverForce) * 0.01f, Color.red);

            Vector3 movingForce = getMovingForceForEngine(engine);
            Debug.DrawRay(castOrigin, engine.transform.TransformDirection(movingForce) * 0.01f, Color.magenta);

            Vector3 turningForce = getTurningForceForEngine(engine);
            Debug.DrawRay(castOrigin, engine.transform.TransformDirection(turningForce) * 0.01f, Color.blue);
            
            re.thrust(movingForce + hoverForce + turningForce);      
        }
    }

    private Vector3 getMovingForceForEngine(GameObject engine) {
        Vector3 movingForce = Vector3.zero;

        ReactiveEngine re = (ReactiveEngine)engine.GetComponent(typeof(ReactiveEngine));

        movingForce.z = re.force.z;

        if (Input.GetKey(KeyCode.W)) {
            
            if (movingForce.z + movingAcceleration * Time.deltaTime >= maxEngineForvardMovingForce) {
                movingForce.z = maxEngineForvardMovingForce;
            } else {
                movingForce.z += movingAcceleration * Time.deltaTime;
            }
            
        } else if (Input.GetKey(KeyCode.S)) {
            if (movingForce.z - movingAcceleration * Time.deltaTime <= -maxEngineBackvardMovingForce) {
                movingForce.z = -maxEngineBackvardMovingForce;
            } else {
                movingForce.z -= movingAcceleration * Time.deltaTime;
            }
        } else {
            if (movingForce.z < 0) {
                if (movingForce.z + movingAcceleration * Time.deltaTime >= 0) {
                    movingForce.z = 0;
                } else {
                    movingForce.z += movingAcceleration * Time.deltaTime;
                }
            } else if (movingForce.z > 0) {
                if (movingForce.z - movingAcceleration * Time.deltaTime <= 0) {
                    movingForce.z = 0;
                } else {
                    movingForce.z -= movingAcceleration * Time.deltaTime;
                }   
            }
        }

        if (movingForce.z < 0) {
            Debug.Log("");
        }

        return movingForce;
    }

    private Vector3 getTurningForceForEngine(GameObject engine) {
        Vector3 turningForce = Vector3.zero;

        ReactiveEngine re = (ReactiveEngine)engine.GetComponent(typeof(ReactiveEngine));

        turningForce.x = re.force.x;

        if ((Input.GetKey(KeyCode.D) && engine.tag == "front reactive engine") || (Input.GetKey(KeyCode.A) && engine.tag == "back reactive engine")) {
            
            if (turningForce.x + turningAcceleration * Time.deltaTime >= maxEngineTurningForce) {
                turningForce.x = maxEngineTurningForce;
            } else {
                turningForce.x += turningAcceleration * Time.deltaTime;
            }
            
        } else if ((Input.GetKey(KeyCode.D) && engine.tag == "back reactive engine") || (Input.GetKey(KeyCode.A) && engine.tag == "front reactive engine")) {
            if (turningForce.x - turningAcceleration * Time.deltaTime <= -maxEngineTurningForce) {
                turningForce.x = -maxEngineTurningForce;
            } else {
                turningForce.x -= turningAcceleration * Time.deltaTime;
            }
        } else {
            if (turningForce.x < 0) {
                if (turningForce.x + movingAcceleration * Time.deltaTime >= 0) {
                    turningForce.x = 0;
                } else {
                    turningForce.x += movingAcceleration * Time.deltaTime;
                }
            } else if (turningForce.x > 0) {
                if (turningForce.x - movingAcceleration * Time.deltaTime <= 0) {
                    turningForce.x = 0;
                } else {
                    turningForce.x -= movingAcceleration * Time.deltaTime;
                }   
            }
        }

        return turningForce;
    }

    private void _activateStabilizers() {
        foreach (GameObject s in stabilizers)
        {
            Stabilizer st = s.GetComponent<Stabilizer>();
            st.activate();
        }
    }

    void FixedUpdate() {

        

        // if (Input.GetKey(KeyCode.A)) {
        //     turnLeft();
        // } else if (Input.GetKey(KeyCode.D)) {
        //     turnRight();
        // } else {
        //     turnToDefault();
        // }

        _thrustEngines();

    }
    void Start() 
    {
        startEngines();
        _activateStabilizers();
    }

    void Update()
    {

        
    }
    
}
