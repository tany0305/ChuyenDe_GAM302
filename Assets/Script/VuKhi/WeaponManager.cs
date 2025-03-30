using UnityEngine;
using Fusion;
public class WeaponManager : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("vao OntriggerEnter");
        // Kiểm tra xem đối tượng va chạm có phải là player không
        if (other.CompareTag("Player"))
        {
            PlayerPro opponentCombat = other.GetComponent<PlayerPro>();
            if (opponentCombat != null)
            {
                // Gọi RPC để đối thủ nhận sát thương
                opponentCombat.RpcTakeDamage(10);
                Debug.Log("Sword hit opponent! Damage dealt: " + 10);
            }
        }
    }
}
