using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Class for controlling how player's tank shoots projectiles.
/// </summary>
public class PlayerShooter : NetworkBehaviour
{
    public Bullet BulletPrefab;
    public Transform BulletSpawn;
    public int ShotsPerRound = 2;
    public float RealoadTime = 1f;

    private int shotsLeft;
    private bool isReloading;


    private void Start()
    {
        shotsLeft = ShotsPerRound;
        isReloading = false;
    }

    public void Shoot()
    {
        if (!isReloading && BulletPrefab != null)
        {
            CmdShoot();
            shotsLeft--;
            if (shotsLeft <= 0)
            {
                StartCoroutine(Reload());
            }
        }
    }

    [Command]
    private void CmdShoot()
    {
        var bullet = Instantiate(BulletPrefab, BulletSpawn.position, BulletSpawn.rotation);
        bullet.Owner = GetComponent<PlayerController>();
        NetworkServer.Spawn(bullet.gameObject);
    }

    private IEnumerator Reload()
    {
        shotsLeft = ShotsPerRound;
        isReloading = true;
        yield return new WaitForSeconds(RealoadTime);
        isReloading = false;
    }
}
