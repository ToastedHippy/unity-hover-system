using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverController : MonoBehaviour
{
   
    public List<GameObject> reactiveEngines;
    public List<GameObject> stabilizers;

    public float hoverHeight = 1.0f;

    private ReactiveEngine[] _reactiveEngines;
    private Stabilizer[] _stabilizers;

    public float maxHoverForcePercent = 1.0f;

    public float hoverForceActionDistance = 5.0f;

    private void _setReactiveEngines() {

        ReactiveEngine[] engines = new ReactiveEngine[reactiveEngines.Count];

        foreach(GameObject re in reactiveEngines)
        {
            engines[reactiveEngines.IndexOf(re)] = (ReactiveEngine)re.GetComponent(typeof(ReactiveEngine));
        }

        _reactiveEngines = engines;
    }

    private void _setStabilizers() {

        Stabilizer[] stabs = new Stabilizer[stabilizers.Count];

        foreach(GameObject s in stabilizers)
        {
            stabs[stabilizers.IndexOf(s)] = (Stabilizer)s.GetComponent(typeof(Stabilizer));
        }

        _stabilizers = stabs;
    }

    public void startEngines() {
        
        foreach (var re in _reactiveEngines)
        {
            re.startEngine();
        }
    }

    private float _calcHoverForce(float distanceFromSurface) {

        float needForce = (1 + 1 + 25) * 9.81f; // engine + stabilizer + 0.25 car body mass
        float maxForce = needForce * maxHoverForcePercent;
        float maxDistance = Mathf.Max(hoverForceActionDistance, hoverHeight);
        float forceDiff = maxForce - needForce;

        float hoverForce = 0.0f;

        if (distanceFromSurface < hoverHeight) {
            hoverForce = Mathf.Clamp((1 - distanceFromSurface / hoverHeight) * forceDiff, 0, forceDiff) + needForce;
        }
        else {
            hoverForce = Mathf.Clamp((1 - ((distanceFromSurface - hoverHeight) / (maxDistance - hoverHeight))) * needForce, 0, needForce);
        }

         return hoverForce;
    }

    private void _thrustEngines() {
        foreach(GameObject re in reactiveEngines) {
            thrustEngine(re);
        }
    }

    private void thrustEngine(GameObject engine) {
        RaycastHit hit;
        Vector3 castOrigin = engine.transform.position;
        Vector3 castDirection = engine.transform.TransformDirection(Vector3.down);
        
        ReactiveEngine re = (ReactiveEngine)engine.GetComponent(typeof(ReactiveEngine));

        if (Physics.Raycast(castOrigin, castDirection, out hit, hoverForceActionDistance, ~(1 << 8)))
        {
            float magnitude = _calcHoverForce(hit.distance);
            Debug.Log(engine.name + ' ' + magnitude + ' ' + hit.distance + ' ' + hit.collider.gameObject.name);
            Debug.DrawRay(castOrigin, castDirection * hit.distance, Color.red);
            re.thrust(magnitude);      
        }
    }

    private void _activateStabilizers() {
        foreach (Stabilizer s in _stabilizers)
        {
            s.activate();
        }
    }

    void FixedUpdate() {
        _thrustEngines();
    }
    void Start() 
    {
        _setReactiveEngines();
        _setStabilizers();

        _activateStabilizers();
        startEngines();
    }

    void Update()
    {
        
    }
}
