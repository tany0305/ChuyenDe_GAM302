using UnityEngine;
using Fusion;
using UnityEngine.AI;

public class EnemyController : NetworkBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;

    [Networked] private NetworkBool isAttacking { get; set; }  // Đồng bộ trạng thái tấn công
    [Networked] private int currentHealth { get; set; }  // Đồng bộ máu của enemy
    [Networked] private int maxHealth { get; set; } = 100;  // Máu tối đa, đồng bộ nếu cần

    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public int damage = 10;

    private float lastAttackTime;

    private NetworkRunner runner;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;  // Đặt máu ban đầu cho enemy
    }

    public override void Spawned()
    {
        runner = GetComponentInParent<NetworkRunner>();  // Lấy NetworkRunner từ đối tượng cha (nếu có)
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;  // Chỉ update khi có quyền sở hữu đối tượng

        GameObject target = FindClosestPlayer();
        if (target == null) return;

        float dist = Vector3.Distance(transform.position, target.transform.position);
        agent.SetDestination(target.transform.position);

        anim.SetBool("isMoving", dist > attackRange);

        if (dist <= attackRange && Time.time > lastAttackTime + attackCooldown)
        {
            isAttacking = true;  // Đồng bộ trạng thái tấn công
            lastAttackTime = Time.time;
            anim.SetTrigger("Attack");

            var health = target.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }

        // Kiểm tra nếu máu của enemy <= 0 thì despawn
        if (currentHealth <= 0)
        {
            DespawnEnemy();
        }
    }

    // Hàm giảm máu khi bị tấn công
    public void TakeDamage(int amount)
    {
        if (!Object.HasStateAuthority) return;  // Chỉ giảm máu khi có quyền sở hữu

        currentHealth -= amount;
        Debug.Log("Enemy bị trúng đòn, máu còn: " + currentHealth);
    }

    // Hàm tìm player gần nhất
    GameObject FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject closest = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject go in players)
        {
            float dist = Vector3.Distance(transform.position, go.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = go;
            }
        }

        return closest;
    }

    // Hàm despawn enemy khi hết máu
    void DespawnEnemy()
    {
        if (runner != null && Object.HasStateAuthority)  // Kiểm tra runner và quyền sở hữu
        {
            runner.Despawn(gameObject.GetComponent<NetworkObject>());
            Debug.Log("Enemy despawned due to no health.");
        }
    }
}
