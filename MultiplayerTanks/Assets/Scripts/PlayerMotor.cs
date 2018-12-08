using UnityEngine;
using UnityEngine.Networking;
#pragma warning disable 649     // suppress 'never assigned'
#pragma warning disable IDE0044 // suppress 'convert to readonly'

/// <summary>
/// Controls player tank's movement.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : NetworkBehaviour
{
    public float MoveSpeed = 150f;
    public float ChassisRotateSpeed = 1f;
    public float TurretRotateSpeed = 3f;
    
    [SerializeField]
    private Transform chassis;
    [SerializeField]
    private Transform turret;

    private new Rigidbody rigidbody;

    public Transform TurretTransform
    {
        get { return turret.transform; }
    }

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void MovePlayer(float magnitude)
    {
        rigidbody.velocity = chassis.forward * MoveSpeed * magnitude * Time.fixedDeltaTime;
    }

    public void RotateChassis(Vector3 direction)
    {
        FaceDirection(chassis, direction, ChassisRotateSpeed);
    }

    public void RotateTurret(Vector3 direction)
    {
        FaceDirection(turret, direction, TurretRotateSpeed);
    }

    private static void FaceDirection(Transform objectTransform, Vector3 direction, float rotationSpeed)
    {
        if (objectTransform != null && direction != Vector3.zero)
        {
            var desiredRotation = Quaternion.LookRotation(direction);
            objectTransform.rotation = Quaternion.Slerp(objectTransform.rotation,
                                                        desiredRotation,
                                                        rotationSpeed * Time.deltaTime);
        }
    }

    private void Disable()
    {
        rigidbody.velocity = Vector3.zero;
    }
}
 