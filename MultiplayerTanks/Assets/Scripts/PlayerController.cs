using UnityEngine;

/// <summary>
/// Manages the other player's components and handles input.
/// </summary>
[RequireComponent(typeof(PlayerSetup))]
[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(PlayerShooter))]
[RequireComponent(typeof(PlayerHealth))]
public class PlayerController : MonoBehaviour
{
    private PlayerSetup playerSetup;
    private PlayerMotor playerMotor;
    private PlayerShooter playerShooter;
    private PlayerHealth playerHealth;
    private Vector3 inputDirection = Vector3.zero;

    private void Start()
    {
        playerSetup = GetComponent<PlayerSetup>();
        playerMotor = GetComponent<PlayerMotor>();
        playerShooter = GetComponent<PlayerShooter>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void FixedUpdate()
    {
        playerMotor.MovePlayer(inputDirection.magnitude);
    }

    private void Update()
    {
        UpdateInput();
        playerMotor.RotateChassis(inputDirection);

        Vector3 turretDirection = 
            Utility.ScreenToElevatedWorldPoint(
                Input.mousePosition, 
                playerMotor.TurretTransform.position.y) 
            - playerMotor.TurretTransform.position;

        playerMotor.RotateTurret(turretDirection);

    }

    private void UpdateInput()
    {
        inputDirection.x = Input.GetAxis("Horizontal");
        inputDirection.z = Input.GetAxis("Vertical");
    }
}
