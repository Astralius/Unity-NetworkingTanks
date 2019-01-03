using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// Tracks player's health and damage.
/// </summary>
public class PlayerHealth : NetworkBehaviour
{
    public GameObject DeathEffect;
    public Slider HealthSlider;
    public float MaxHealth = 3f;
    public PlayerController LastAttacker;

    [SyncVar(hook="UpdateHealthBar")]
    private float currentHealth;
    [SyncVar]
    private bool isDead;

    public bool IsDead
    {
        get { return isDead; }
    }

    private void Start()
    {
        currentHealth = MaxHealth;
        UpdateHealthBar(currentHealth);
    }

    public void Reset()
    {
        Start();
        SetActiveState(true);
        isDead = false;
    }

    public void Damage(float damage, PlayerController attacker = null)
    {
        if (isServer)
        {
            if (attacker != null && attacker != GetComponent<PlayerController>())
            {
                LastAttacker = attacker;
            }
            currentHealth -= damage;
            if (currentHealth <= 0f && !isDead)
            {
                if (LastAttacker != null)
                {
                    LastAttacker.Score++;
                    GameManager.Instance.UpdateScoreboard();
                }              
                LastAttacker = null;
                isDead = true;
                RpcDie();
            }
        }
    }

    private void UpdateHealthBar(float health)
    {
        if (HealthSlider != null)
        {
            HealthSlider.value = health / MaxHealth;
        }
    }

    [ClientRpc]
    private void RpcDie()
    {
        if (DeathEffect != null)
        {
            var effect = Instantiate(DeathEffect, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            GameObject.Destroy(effect, 3f);
        }

        SetActiveState(false);
        gameObject.SendMessage("Disable");
    }

    private void SetActiveState(bool state)
    {
        foreach (var c in GetComponentsInChildren<Collider>())
        {
            c.enabled = state;
        }

        foreach (var c in GetComponentsInChildren<Canvas>())
        {
            c.enabled = state;
        }

        foreach (var r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = state;
        }
    }
}
