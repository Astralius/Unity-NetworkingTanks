using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

#pragma warning disable 0109

[RequireComponent(typeof(Rigidbody))]
public class Bullet : NetworkBehaviour
{
    public int Speed = 100;
    public float Delay = 0.03f;
    public float Lifetime = 5f;
    public int Bounces = 2;
    public int Damage = 1;
    public GameObject ExplosionEffect;
    public List<string> BounceTags;
    public List<string> CollisionTags;
    public PlayerController Owner;

    private List<ParticleSystem> allParticles;
    private new Rigidbody rigidbody;
    private new Collider collider;

    private void Start()
    {
        allParticles = GetComponentsInChildren<ParticleSystem>().ToList();
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = transform.forward * Speed;
        collider = GetComponent<Collider>();
        StartCoroutine(SelfdestructAfterDelay(Lifetime));
    }

    private void OnCollisionEnter(Collision collision)
    {
        CheckCollision(collision);
        CheckBounce(collision);
    }

    private void CheckCollision(Collision collision)
    {
        if (CollisionTags.Contains(collision.gameObject.tag))
        {          
            var playerHealth = collision.gameObject.GetComponentInParent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.Damage(Damage, Owner);
            }
            SelfDestruct();
        }
    }

    private void CheckBounce(Collision collision)
    {
        if (BounceTags.Contains(collision.gameObject.tag))
        {
            if (Bounces <= 0)
            {
                SelfDestruct();
            }
            else
            {
                Bounces--;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (rigidbody.velocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(rigidbody.velocity);
        }
    }

    private IEnumerator SelfdestructAfterDelay(float lifetime)
    {
        collider.enabled = false;
        yield return new WaitForSeconds(Delay);
        collider.enabled = true;
        yield return new WaitForSeconds(lifetime);
        SelfDestruct();
    }

    private void SelfDestruct()
    {
        collider.enabled = false;
        rigidbody.velocity = Vector3.zero;
        rigidbody.Sleep();

        foreach (ParticleSystem ps in allParticles)
        {
            ps.Stop();
        }

        if (ExplosionEffect != null)
        {
            var explosion = Instantiate(ExplosionEffect, transform.position, Quaternion.identity);
            GameObject.Destroy(explosion, 2f);
        }

        if (isServer)
        {
            Destroy(this.gameObject);
        }       
    }
}
