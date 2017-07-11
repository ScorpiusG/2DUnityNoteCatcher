using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollower : MonoBehaviour
{
    public Camera parentCamera;

    private Camera mCamera;

    public bool followPosition = false;
    public bool followRotation = false;
    public bool followScale = false;
    public bool followOrthographicSize = false;

    private void Awake()
    {
        mCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (followPosition) transform.position = parentCamera.transform.position;
        if (followRotation) transform.rotation = parentCamera.transform.rotation;
        if (followScale) transform.localScale = parentCamera.transform.localScale;
        if (followOrthographicSize) mCamera.orthographicSize = parentCamera.orthographicSize;
    }
}
