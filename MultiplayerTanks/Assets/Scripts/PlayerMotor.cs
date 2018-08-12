using UnityEngine;
#pragma warning disable 649     // suppress 'never assigned'
#pragma warning disable IDE0044 // suppress 'convert to readonly'

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    public float moveSpeed = 100f;
    public float chassisRotateSpeed = 1f;
    public float turretRotateSpeed = 3f;
    
    [SerializeField]
    private Transform chassis;
    [SerializeField]
    private Transform turret;

    private new Rigidbody rigidbody;


    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void Move()
    {
        rigidbody.velocity = chassis.rotation.eulerAngles * moveSpeed * Time.deltaTime;
    }

    public void RotateChassis(Vector3 direction)
    {
        FaceDirection(chassis, direction, chassisRotateSpeed);
    }

    public void RotateTurret(Vector3 direction)
    {
        FaceDirection(turret, direction, turretRotateSpeed);
    }

    private static void FaceDirection(Transform objectTransform, Vector3 direction, float rotationSpeed)
    {
        if (objectTransform != null)
        {
            var desiredRotation = Quaternion.LookRotation(direction);
            objectTransform.rotation = Quaternion.Slerp(objectTransform.rotation,
                                                        desiredRotation,
                                                        rotationSpeed * Time.deltaTime);
        }
    }
}
 