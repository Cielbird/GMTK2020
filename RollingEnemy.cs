using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class RollingEnemy : Enemy
{
    public GameObject explosionPrefab;
    
    protected override void Update()
    {
        base.Update();

        if (agent.enabled)
        {
            agent.destination = player.transform.position;
        }

        if (Vector3.Distance(transform.position, player.transform.position) < 1f && agent.enabled)
        {
            InitiateDeath();
        }
    }

    protected override void InitiateDeath()
    {
        base.InitiateDeath();
        StartCoroutine(Explode());
    }

    IEnumerator Explode()
    {
        yield return new WaitForSeconds(timeToDie+0.1f);
        Instantiate(explosionPrefab, transform.position, transform.rotation);
    }
}
