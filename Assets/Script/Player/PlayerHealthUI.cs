using UnityEngine;
using Fusion;

public class PlayerHealthUI : NetworkBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 offset = new Vector3(0, 1f, 0);

    void LateUpdate()
    {
        if (!HasStateAuthority) return;
        if (player != null)
        {
            transform.position = player.position + offset;
        }
    }
}