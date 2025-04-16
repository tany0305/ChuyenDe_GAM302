using UnityEngine;
using Fusion;
using TMPro;
using UnityEngine.AI;

public class BossAI : NetworkBehaviour
{
    [Header("Movement")]
    public float patrolRadius = 10f;
    public float patrolWaitTime = 2f;

    [Header("Health")]
    public int maxHealth = 100;
    private int currentHealth;

    private NavMeshAgent agent;
    private Animator anim;
    private float patrolTimer;
    private Vector3 spawnPosition;

    public TMP_Text healthText;

    public override void Spawned()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        spawnPosition = transform.position; // Lưu lại vị trí gốc
        SetNewPatrolDestination();

        currentHealth = maxHealth; // Thiết lập máu ban đầu
        UpdateHealthText(); // Cập nhật hiển thị máu ban đầu
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            patrolTimer += Time.deltaTime;
            if (patrolTimer >= patrolWaitTime)
            {
                SetNewPatrolDestination();
                patrolTimer = 0f;
            }
            anim.SetBool("IsMoving", false);
        }
        else
        {
            anim.SetBool("IsMoving", true);
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
        if (currentHealth <= 0) return; // Nếu đã chết thì không trừ máu nữa

        currentHealth = Mathf.Max(currentHealth - dmg, 0);
        anim.SetTrigger("TakeDamage");

        UpdateHealthText(); // Cập nhật lại máu sau khi bị trừ

        if (currentHealth <= 0)
        {
            anim.SetTrigger("Die");
            Runner.Despawn(Object);
        }
    }

    void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = $"Health: {currentHealth}/{maxHealth}"; // Cập nhật giá trị máu
        }
    }
}
