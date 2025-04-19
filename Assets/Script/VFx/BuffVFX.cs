using UnityEngine;
using Fusion;
using System.Collections;

public class BuffVFX : NetworkBehaviour
{
    public int healAmountPerTick = 5;
    public float healInterval = 1f;
    public float totalDuration = 30f;

    private PlayerPro targetPlayer;
    private int originalMaxHealth;

    public override void Spawned()
    {
        // Gán người chơi là cha hiện tại
        targetPlayer = GetComponentInParent<PlayerPro>();

        if (targetPlayer != null && Object.HasStateAuthority)
        {
            originalMaxHealth = targetPlayer.MaxHealth; // Lưu lại MaxHealth ban đầu
            targetPlayer.MaxHealth *= 2; // Tăng gấp đôi MaxHealth
            targetPlayer.Health = targetPlayer.MaxHealth; // Hồi full HP ngay lập tức

            StartCoroutine(HealOverTime());
        }
    }

    IEnumerator HealOverTime()
    {
        float elapsed = 0f;

        while (elapsed < totalDuration)
        {
            yield return new WaitForSeconds(healInterval);

            if (targetPlayer != null && targetPlayer.Health < targetPlayer.MaxHealth)
            {
                // Hồi máu từ từ cho đến full HP
                int newHealth = Mathf.Min(targetPlayer.Health + healAmountPerTick, targetPlayer.MaxHealth);
                targetPlayer.Health = newHealth;
            }

            elapsed += healInterval;
        }

        // Sau khi hết thời gian, khôi phục MaxHealth về giá trị ban đầu
        if (targetPlayer != null)
        {
            targetPlayer.MaxHealth = originalMaxHealth;
        }
    }
}
