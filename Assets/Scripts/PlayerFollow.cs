using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{

    public Transform playerTransform;

    private Vector3 _cameraOffcet;

    [Range(0.1f, 1.0f)]
    public float smoothFactor = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        _cameraOffcet = transform.position - playerTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate() {
        Vector3 newPos = playerTransform.position + _cameraOffcet;

        transform.position = Vector3.Slerp(transform.position, newPos, smoothFactor);
    }
}
