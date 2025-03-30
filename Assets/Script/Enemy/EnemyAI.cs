using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public GameObject[] targets;
    // Update is called once per frame
    void Update()
    {
        targets = GameObject.FindGameObjectsWithTag("Player");
        if(targets.Length == 0 ) return;

        //tìm target gần nhất
        GameObject target = null;
        float minDistance = Mathf.Infinity;
        foreach( var t in targets )
        {
            var distance = Vector3.Distance(t.transform.position, transform.position);
            if( distance < minDistance )
            {
                minDistance = distance;
                target = t; 
            }    
        } 
        if(target != null)
        {
            agent.SetDestination(target.transform.position);    
        }
    }
}
