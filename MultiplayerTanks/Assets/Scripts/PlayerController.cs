using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Manages the other player's components and handles input.
/// </summary>
[RequireComponent(typeof(PlayerSetup))]
[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(PlayerShooter))]
[RequireComponent(typeof(PlayerHealth))]
public class PlayerController : NetworkBehaviour
{
    [Range(1f, 10f)]
    public float RespawnTime = 3f;
    public GameObject SpawnEffect;

    private PlayerSetup playerSetup;
    private PlayerMotor playerMotor;
    private PlayerShooter playerShooter;
    private PlayerHealth playerHealth;
    private Vector3 originalPosition;
    private Vector3 inputDirection = Vector3.zero;

    public override void OnStartLocalPlayer()
    {
        originalPosition = transform.position;
    }

    private void Start()
    {
        playerSetup = GetComponent<PlayerSetup>();
        playerMotor = GetComponent<PlayerMotor>();
        playerShooter = GetComponent<PlayerShooter>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            playerMotor.MovePlayer(inputDirection.magnitude);
        }
    }

    private void Update()
    {
        if (isLocalPlayer && !playerHealth.IsDead)
        {
            if (Input.GetMouseButtonDown(0))
            {
                playerShooter.Shoot();
            }

            UpdateInput();
            playerMotor.RotateChassis(inputDirection);

            Vector3 turretDirection =
                Utility.ScreenToElevatedWorldPoint(
                    Input.mousePosition,
                    playerMotor.TurretTransform.position.y)
                - playerMotor.TurretTransform.position;

            playerMotor.RotateTurret(turretDirection);
        }
    }

    private void UpdateInput()
    {
        inputDirection.x = Input.GetAxis("Horizontal");
        inputDirection.z = Input.GetAxis("Vertical");
    }

    private void Disable()
    {
        Invoke("Respawn", RespawnTime);
    }

    private void Respawn()
    {
        transform.position = originalPosition;
        playerHealth.Reset();
        if (SpawnEffect != null)
        {
            var spawnEffect = Instantiate(SpawnEffect, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            Destroy(spawnEffect, 3f);
        }
    }
}
