using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private new Transform transform;
    private Transform cameraTransform;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        transform = GetComponent<Transform>();
    }

    private void Update()
    {
        transform.LookAt(worldPosition: transform.position + cameraTransform.rotation * Vector3.forward,
                         worldUp: cameraTransform.rotation * Vector3.up);
    }
}
