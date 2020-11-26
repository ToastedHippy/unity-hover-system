using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigurableJointRotator : MonoBehaviour
{

    private ConfigurableJoint _joint;
    private Quaternion _previousRotation;
    public void Rotate(Vector3 euler)
    {
        // _previousRotation = _joint.targetRotation;
        
        Quaternion q = Quaternion.Euler(euler);
        _joint.targetRotation = Quaternion.identity * (_previousRotation * Quaternion.Inverse(q));
    }


    // Start is called before the first frame update
    void Start()
    {
        _joint = GetComponent<ConfigurableJoint>();
        _previousRotation = _joint.targetRotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
