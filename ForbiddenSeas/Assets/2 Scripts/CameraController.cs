using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform[] positions;
    private Transform end;
    public float smoothTimePos = 0.3f;
    public float smoothTimeRot = 0.3f;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        end = transform;
    }
    public void moveCamera(int pos)
    {
        end = positions[pos];
    }

    void LateUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, end.position, ref velocity, smoothTimePos);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, end.rotation, smoothTimeRot * Time.deltaTime);
    }
}
