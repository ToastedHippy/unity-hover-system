using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverController : MonoBehaviour
{
   
    public List<GameObject> reactiveEnginesGO;
    public List<GameObject> stabilizers;

    public float hoverHeight = 1.0f;

    private ReactiveEngine[] _reactiveEngines;
    private Stabilizer[] _stabilizers;

    public float maxHoverForcePercent = 1.0f;

    public float hoverForceActionDistance = 5.0f;

    public float maxHoverEngineRotationAngle = 90f;

    private float _xAngle = 0f;
    private float _zAngle = 0f;

    public bool _isBack = false;
    public bool _isRight = false;

    private void _setReactiveEngines() {

        ReactiveEngine[] engines = new ReactiveEngine[reactiveEnginesGO.Count];

        foreach(GameObject re in reactiveEnginesGO)
        {
            engines[reactiveEnginesGO.IndexOf(re)] = (ReactiveEngine)re.GetComponent(typeof(ReactiveEngine));
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
        
        foreach (ReactiveEngine re in _reactiveEngines)
        {
            re.startEngine();
        }
    }

    public void moveForvard() {
        foreach(GameObject re in reactiveEnginesGO)
        {
            ConfigurableJointRotator rotator = re.GetComponent<ConfigurableJointRotator>();
            rotator.Rotate(new Vector3(45, 0, 0) * Time.deltaTime);
        }
    }

    public void moveBackward() {
        foreach(GameObject re in reactiveEnginesGO)
        {
            ConfigurableJointRotator rotator = re.GetComponent<ConfigurableJointRotator>();
            rotator.Rotate(new Vector3(-45, 0, 0) * Time.deltaTime);
        }
    }

    private float _calcHoverForce(float distanceFromSurface) {

        float needForce = (1 + 1 + 1 + 25) * 9.81f; // engine + stabilizer + 0.25 car body mass
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
        foreach(GameObject re in reactiveEnginesGO) {
            thrustEngine(re);
        }
    }

    private void thrustEngine(GameObject engine) {
        RaycastHit hit;
        Vector3 castOrigin = engine.transform.position;
        Vector3 castDirection = Vector3.down;
        
        ReactiveEngine re = (ReactiveEngine)engine.GetComponent(typeof(ReactiveEngine));

        if (Physics.Raycast(castOrigin, castDirection, out hit, hoverForceActionDistance, ~(1 << 8)))
        {
            float hoverMagnitude = _calcHoverForce(hit.distance);
            // Debug.Log(engine.name + ' ' + hoverMagnitude + ' ' + hit.distance + ' ' + hit.collider.gameObject.name);
            Debug.DrawRay(castOrigin, castDirection * hit.distance, Color.red);

            _updateXAngle();
            _updateZAngle();

            float totalMagnitude = hoverMagnitude / Mathf.Cos(Mathf.Deg2Rad * _xAngle);

            ConfigurableJointRotator rotator = re.GetComponent<ConfigurableJointRotator>();
            rotator.Rotate(new Vector3(_xAngle, 0, _zAngle));

            re.thrust(hoverMagnitude);      
        }
    }

    private void _updateXAngle() {
        if (Input.GetKey(KeyCode.W)) {
            _xAngle += 1f;
        }

        if (Input.GetKey(KeyCode.S)) {
            _xAngle -= 1f;
        }
        
    }

    private void _updateZAngle() {
        if (Input.GetKey(KeyCode.A)) {
            _zAngle += _isBack ? -0.2f : 0.2f;
        }

        if (Input.GetKey(KeyCode.D)) {
            _zAngle += _isBack ? 0.2f : -0.2f;
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
