using UnityEngine;
using Fusion;
using TMPro;

public class BossAI : NetworkBehaviour
{
    [Header("Stats")]
    [Networked, OnChangedRender(nameof(OnHealthChanged))]
    public float currentHealth { get; set; }

    [Networked, OnChangedRender(nameof(OnHealthChanged))]
    public float maxHealth { get; set; } = 200f;

    [Header("UI")]
    public Canvas healthCanvas;
    public TextMeshProUGUI healthText; // TextMeshPro để hiển thị HP

    private Animator anim;
    private Transform targetPlayer;
    private bool isAttacking = false;

    public override void Spawned()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority || targetPlayer == null) return;

        float distance = Vector3.Distance(transform.position, targetPlayer.position);

        // Kiểm tra nếu boss ở gần player để tấn công
        if (distance <= 2f) // Phạm vi tấn công
        {
            if (!isAttacking)
            {
                isAttacking = true;
                anim.SetTrigger("Attack");
            }
        }
        else
        {
            isAttacking = false;
            anim.SetBool("IsMoving", true);
            MoveToPlayer(); // Di chuyển đến player
        }

        UpdateHealthUI(); // Cập nhật thông tin HP lên UI
        UpdateUIFacing(); // Đảm bảo Canvas hướng về camera
    }

    void MoveToPlayer()
    {
        if (targetPlayer != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPlayer.position, 2f * Time.deltaTime); // Di chuyển đến player
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Object.HasStateAuthority) return;
        if (other.CompareTag("Player") && targetPlayer == null)
        {
            targetPlayer = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!Object.HasStateAuthority) return;
        if (other.CompareTag("Player") && other.transform == targetPlayer)
        {
            targetPlayer = null;
            anim.SetBool("IsMoving", false);
        }
    }

    // RPC để nhận sát thương và trừ HP
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcTakeDamage(int dmg)
    {
        currentHealth = Mathf.Max(currentHealth - dmg, 0f); // Trừ HP khi nhận sát thương
        if (currentHealth <= 0)
        {
            anim.SetTrigger("Die");
            Runner.Despawn(Object); // Khi chết, despawn đối tượng
        }
    }

    // Hàm cập nhật TextMeshPro khi HP thay đổi
    void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = $"{Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth)}"; // Cập nhật TextMeshPro với HP
        }
    }

    // Cập nhật hướng nhìn của Canvas về phía Camera
    void UpdateUIFacing()
    {
        if (healthCanvas != null && Camera.main != null)
        {
            healthCanvas.transform.LookAt(Camera.main.transform);
            healthCanvas.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        }
    }

    // Hàm này gọi khi HP thay đổi, sẽ làm cho UI được cập nhật
    private void OnHealthChanged()
    {
        UpdateHealthUI(); // Cập nhật UI mỗi khi HP thay đổi
    }
}
