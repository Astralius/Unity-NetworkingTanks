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
    public ParticleSystem MisfireEffect;
    public LayerMask ObstacleMask;

    private new Transform transform;
    private int shotsLeft;
    private bool isReloading;


    private void Start()
    {
        transform = GetComponent<Transform>();
        shotsLeft = ShotsPerRound;
        isReloading = false;
    }

    public void Shoot()
    {
        if (!isReloading && BulletPrefab != null)
        {
            RaycastHit hit;
            var center = new Vector3(transform.position.x, BulletSpawn.position.y, transform.position.z);
            Vector3 direction = (BulletSpawn.position - center).normalized;
            float distance = (BulletSpawn.position - center).magnitude;

            if (Physics.SphereCast(center, 0.25f, direction, out hit, distance * 1.4f, ObstacleMask,
                                   QueryTriggerInteraction.Ignore))
            {
                if (MisfireEffect != null)
                {
                    ParticleSystem effect = Instantiate(MisfireEffect, hit.point, Quaternion.identity);
                    effect.Stop();
                    effect.Play();
                    Destroy(effect.gameObject, 3f);
                }
            }
            else
            {
                
                CmdShoot();
                shotsLeft--;
                if (shotsLeft <= 0)
                {
                    StartCoroutine(Reload());
                }
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
