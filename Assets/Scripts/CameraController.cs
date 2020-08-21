using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform cameraTarget;

    public float mouseSensitivity = 1.0f;
     
    [Range(0.1f, 1.0f)]
    public float followSmoothFactor = 0.5f;

    private Vector3 _rotation = new Vector3();
    // Start is called before the first frame update
    void Start()
    {
        if (cameraTarget != null) {
            // transform.position = cameraTarget.position; 
            
        }
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 newPos = cameraTarget.position;

        transform.position = Vector3.Slerp(transform.position, newPos, followSmoothFactor);
        

        _rotation.y += Input.GetAxis("Mouse X") * mouseSensitivity;

        Quaternion quaternion = Quaternion.Euler(0, _rotation.y, 0);
        transform.rotation = quaternion;
    }
}
