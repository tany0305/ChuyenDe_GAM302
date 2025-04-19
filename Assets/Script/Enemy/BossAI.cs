using UnityEngine;
using Fusion;
using TMPro;
using UnityEngine.AI;
using System.Collections;

public class BossAI : NetworkBehaviour
{
    [Header("Movement")]
    public float patrolRadius = 10f;
    public float patrolWaitTime = 2f;
    public float detectionRadius = 5f;
    public float attackCooldown = 2f;
    public int attackDamage = 20;

    [Header("Health")]
    public int maxHealth = 100;
    private int currentHealth;

    private NavMeshAgent agent;
    private Animator anim;
    private float patrolTimer;
    private float attackTimer;
    private Vector3 spawnPosition;

    private GameObject targetPlayer;
    private bool isPlayerInRange = false;

    public TMP_Text healthText;

    public GameObject weaponCollider;  // Collider của vũ khí (Weapon) để va chạm với Player

    public override void Spawned()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        spawnPosition = transform.position;
        SetNewPatrolDestination();

        currentHealth = maxHealth;
        UpdateHealthText();
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        attackTimer += Time.deltaTime;

        // Kiểm tra có Player gần không
        if (targetPlayer != null && isPlayerInRange)
        {
            float distance = Vector3.Distance(transform.position, targetPlayer.transform.position);

            // Boss di chuyển về phía Player
            agent.SetDestination(targetPlayer.transform.position);
            anim.SetBool("IsMoving", true);  // Boss đang di chuyển

            // Nếu Boss gần Player, dừng lại và tấn công
            if (distance <= agent.stoppingDistance)
            {
                anim.SetBool("IsMoving", false);  // Dừng di chuyển khi gần Player

                // Kiểm tra thời gian cooldown và tấn công
                if (attackTimer >= attackCooldown )
                {
                    // Gọi animation Attack (chuyển sang Slash)
                    anim.SetTrigger("Slash");
                    Debug.Log("Attack animation triggered!");  // Kiểm tra Trigger có hoạt động không
                    attackTimer = 0f;

                    // Bật collider vũ khí để gây sát thương
                    weaponCollider.SetActive(true);
                    Debug.Log("Boss is attacking the player!");

                    // Gây sát thương cho Player nếu trúng
                    PlayerPro player = targetPlayer.GetComponent<PlayerPro>();
                    if (player != null)
                    {
                        Debug.Log("Player is hit, causing damage!");
                        player.RpcTakeDamage(attackDamage);
                    }

                    // Tắt collider vũ khí sau một thời gian ngắn
                    StartCoroutine(DisableWeaponColliderAfterDelay(0.5f));  // 0.5s delay
                }
            }

            return; // Nếu có Player thì không tuần tra
        }

        // Nếu không có player, quay lại tuần tra
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            patrolTimer += Time.deltaTime;
            if (patrolTimer >= patrolWaitTime)
            {
                SetNewPatrolDestination();
                patrolTimer = 0f;
            }
            anim.SetBool("IsMoving", false);  // Dừng di chuyển khi quay lại tuần tra
        }
        else
        {
            anim.SetBool("IsMoving", true);  // Tiếp tục di chuyển khi tuần tra
        }
    }

    void SetNewPatrolDestination()
    {
        Vector2 randomPoint = Random.insideUnitCircle * patrolRadius;
        Vector3 destination = spawnPosition + new Vector3(randomPoint.x, 0, randomPoint.y);

        if (NavMesh.SamplePosition(destination, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcTakeDamage(int dmg)
    {
        if (currentHealth <= 0) return;

        currentHealth = Mathf.Max(currentHealth - dmg, 0);
        anim.SetTrigger("Imp");  // Dùng animation Slash khi bị tấn công

        UpdateHealthText();

        if (currentHealth <= 0)
        {
            anim.SetTrigger("Die");  // Gọi animation Die khi Boss chết
            Runner.Despawn(Object);  // Xóa boss khi chết
        }
    }

    void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = $"Health: {currentHealth}/{maxHealth}";
        }
    }

    // Kiểm tra khi Player vào vùng collider Boss
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            targetPlayer = other.gameObject;
            isPlayerInRange = true;
            Debug.Log("Player entered Boss's attack range.");
        }
    }

    // Khi Player ra khỏi vùng collider, Boss quay lại tuần tra
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            targetPlayer = null;
            SetNewPatrolDestination();  // Quay lại tuần tra
            Debug.Log("Player left Boss's attack range.");
        }
    }

    // Bật tắt collider vũ khí (Khi tấn công)
    public void EnableWeaponCollider()
    {
        weaponCollider.SetActive(true);
        Debug.Log("Weapon collider enabled.");
    }

    // Tắt collider vũ khí sau một thời gian ngắn sau khi tấn công
    public IEnumerator DisableWeaponColliderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        weaponCollider.SetActive(false);
        Debug.Log("Weapon collider disabled.");
    }
}
