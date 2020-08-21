using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverCar : MonoBehaviour
{
    public GameObject hoverEngineObjFR;
    public GameObject hoverEngineObjFL;
    public GameObject hoverEngineObjBR;
    public GameObject hoverEngineObjBL;

    public float rotationAcceleration = 1.0f;
    public float movingAcceleration = 5.0f;
    public float hoverHeight = 1.0f;

    private HoverEngine[] _hoverEngines;

    private void _setHoverEngines() {
        GameObject[] hoverEnginesObjs = new GameObject[]{hoverEngineObjFR, hoverEngineObjFL, hoverEngineObjBR, hoverEngineObjBL};

        HoverEngine[] hoverEngines = new HoverEngine[hoverEnginesObjs.Length];

        for (int i = 0; i < hoverEnginesObjs.Length; i++)
        {
            GameObject heObj = hoverEnginesObjs[i];
            HoverEngine he = (HoverEngine)heObj.GetComponent(typeof(HoverEngine));

            hoverEngines[i] = he;
        }

        _hoverEngines = hoverEngines;
    }

    private void _updateHoverEngineProps() 
    {
        foreach (HoverEngine he in _hoverEngines)
        {
            he.hoverHeight = hoverHeight;
            he.movingAcceleration = movingAcceleration;
            he.rotationAcceleration = rotationAcceleration;
        }
    }

    public void startEngines() {
        
        foreach (var he in _hoverEngines)
        {
            HoverEngine hes = (HoverEngine)he.GetComponent(typeof(HoverEngine));
            hes.startEngine();
        }
    }

    public void updateMovingState(MoveDirection direction) {

        foreach (var he in _hoverEngines)
        {
            he.updateMovingEngineState(direction);
        }
    }

    public void updateRotatingState(RotationDirection direction) {

        HoverEngine hesFR = (HoverEngine)hoverEngineObjFR.GetComponent(typeof(HoverEngine));
        HoverEngine hesFL = (HoverEngine)hoverEngineObjFL.GetComponent(typeof(HoverEngine));

        HoverEngine hesBL = (HoverEngine)hoverEngineObjBL.GetComponent(typeof(HoverEngine));
        HoverEngine hesBR = (HoverEngine)hoverEngineObjBR.GetComponent(typeof(HoverEngine));

        switch (direction)
        {
            case RotationDirection.left:
                
                hesFR.updateRotatingEngineState(RotationDirection.left);
                hesFL.updateRotatingEngineState(RotationDirection.left);

                hesBL.updateRotatingEngineState(RotationDirection.right);
                hesBR.updateRotatingEngineState(RotationDirection.right);
                break;
            case RotationDirection.right:
                hesFL.updateRotatingEngineState(RotationDirection.right);
                hesFR.updateRotatingEngineState(RotationDirection.right);

                hesBR.updateRotatingEngineState(RotationDirection.left);
                hesBL.updateRotatingEngineState(RotationDirection.left);
                break;
            case RotationDirection.none:
                foreach (var he in _hoverEngines)
                {
                    he.updateRotatingEngineState(RotationDirection.none);
                }
                break;
            default:
                break;
        }
    }

    void Start() 
    {
        _setHoverEngines();
        _updateHoverEngineProps();
        startEngines();
    }

    void FixedUpdate() {
        _updateHoverEngineProps();

        if (Input.GetKey(KeyCode.W)) 
        {
            updateMovingState(MoveDirection.forvard);
        } 
        else if (Input.GetKey(KeyCode.S)) 
        {    
            updateMovingState(MoveDirection.backvard);
        } 
        else 
        {
            updateMovingState(MoveDirection.none);
        }

        if (Input.GetKey(KeyCode.A)) {
            updateRotatingState(RotationDirection.left);
        } 
        else if (Input.GetKey(KeyCode.D)) 
        {
            updateRotatingState(RotationDirection.right);
        }
        else 
        {
            updateRotatingState(RotationDirection.none);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
