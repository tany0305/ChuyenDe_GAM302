using UnityEngine;
using Fusion;
using TMPro;

public class PlayerProperties : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnHealthChanged))]
    public float health { get; set; }

    [Networked]
    [SerializeField] public float maxHealth { get; set; } // Gán giá trị mặc định

    public TextMeshProUGUI healthText;

    public override void Spawned()
    {
        base.Spawned();
        if (Object.HasStateAuthority) // Chỉ chạy trên máy chủ
        {
            health = maxHealth; // Khởi tạo máu đầy đủ khi spawn
        }

        OnHealthChanged(); // Gọi cập nhật UI ngay khi spawn
    }

    public void OnHealthChanged()
    {
        if (healthText != null)
        {
            healthText.text = $"{health}/{maxHealth}";
            healthText.ForceMeshUpdate(); // Đảm bảo TextMeshPro vẽ lại
        }
    }
}
