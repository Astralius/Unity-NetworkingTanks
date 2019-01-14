using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Utility;

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
    public int Score;

    private IList<SpawnPoint> spawnPoints;
    private PlayerMotor playerMotor;
    private PlayerShooter playerShooter;
    private PlayerHealth playerHealth;
    private FollowTarget cameraFollower;
    private Vector3 originalPosition;
    private Vector3 inputDirection = Vector3.zero;

    public override void OnStartLocalPlayer()
    {
        originalPosition = transform.position;
        SetupCameraFollow();       
    }

    private void Start()
    {
        spawnPoints = FindObjectsOfType<SpawnPoint>();
        playerMotor = GetComponent<PlayerMotor>();
        playerShooter = GetComponent<PlayerShooter>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void FixedUpdate()
    {
        if (isLocalPlayer && !playerHealth.IsDead)
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

    private void SetupCameraFollow()
    {
        cameraFollower = Camera.main.GetComponent<FollowTarget>();
        if (cameraFollower != null)
        {
            cameraFollower.target = this.transform;
        }
    }

    private void UpdateInput()
    {
        inputDirection.x = Input.GetAxis("Horizontal");
        inputDirection.z = Input.GetAxis("Vertical");
    }

    private void Disable()
    {
        StartCoroutine(this.Respawn(GetRandomVacantSpawnPosition()));
    }

    private IEnumerator Respawn(Vector3 position)
    {
        yield return new WaitForSeconds(RespawnTime);

        transform.position = position;
        spawnPoints.ToList()
                   .ForEach(sp => sp.OccupantLeft(this));

        playerHealth.Reset();
        if (SpawnEffect != null)
        {
            var spawnEffect = Instantiate(SpawnEffect, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            Destroy(spawnEffect, 3f);
        }
    }

    private Vector3 GetRandomVacantSpawnPosition()
    {
        return spawnPoints != null && spawnPoints.Any() ? 
            spawnPoints.Where(sp => !sp.IsOccupied).ToList().Random().transform.position :
            originalPosition;
    }
}
