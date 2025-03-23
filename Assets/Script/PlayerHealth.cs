using UnityEngine;
using Fusion;
using TMPro;

public class PlayerHealth : NetworkBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;

    [Networked] // Đồng bộ máu qua mạng
    public float Health { get; set; }

    [SerializeField] TMP_Text healthText;
    private bool isDead = false;

    public override void Spawned()
    {
        Health = maxHealth;
        if (healthText == null)
        {
            healthText = GetComponentInChildren<TMP_Text>();
            if (healthText == null)
            {
                Debug.LogWarning($"Player {Object.Id}: Không tìm thấy TMP_Text trong hierarchy!");
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (!Object.HasStateAuthority) return;
        Health = Mathf.Max(Health - damage, 0);
        Debug.Log($"Player {Object.Id} took {damage} damage. Health now: {Health}");
    }

    public override void Render()
    {
        if (healthText != null)
        {
            healthText.text = $"HP: {Health}/{maxHealth}";
        }
        else
        {
            Debug.LogWarning($"Player {Object.Id}: healthText chưa được gán!");
        }

        if (Health <= 0 && !isDead)
        {
            isDead = true;
            if (Object.HasStateAuthority)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        Debug.Log($"Player {Object.Id} has died!");
        NetworkObject rootNetworkObject = GetComponentInParent<NetworkObject>();
        if (rootNetworkObject != null)
        {
            Debug.Log($"Despawning root object {rootNetworkObject.Id}");
            Runner.Despawn(rootNetworkObject);
        }
        else
        {
            Debug.LogError("Không tìm thấy NetworkObject ở object cha!");
            Runner.Despawn(Object);
        }
    }


}