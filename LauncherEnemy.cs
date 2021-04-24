using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class LauncherEnemy : Enemy
{
    public float attackPauseLength;

    public GameObject rocketPrefab;
    public Transform leftLauncher;
    public Transform rightLauncher;

    
    protected override void Start()
    {
        base.Start();

        StartCoroutine(Shoot());
    }
    
    protected override void Update()
    {
        base.Update();


        if (agent.enabled)
        {
            transform.forward = new Vector3(player.transform.position.x - transform.position.x, 0, player.transform.position.z - transform.position.z);
            if (Vector3.Distance(transform.position, player.transform.position) < 5f && agent.enabled)
            {
                agent.destination = transform.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            }
            else
            {
                agent.destination = player.transform.position;
            }
        }
    }

    protected override void InitiateDeath()
    {
        base.InitiateDeath();

        StopCoroutine(Shoot());
    }

    IEnumerator Shoot()
    {
        while (agent.enabled)
        {
            Vector3 leftPos = leftLauncher.position + leftLauncher.TransformVector(new Vector3(0, -1f, 0));
            Transform rightRocket = Instantiate(rocketPrefab, leftPos, Quaternion.LookRotation((player.transform.position + Vector3.up * 2f - leftPos).normalized, Vector3.up)).transform;//leftLauncher.rotation).transform;
            //rightRocket.Rotate(new Vector3(90, 0, 180));
            rightRocket.Translate(new Vector3(0, 0, 1), Space.Self);
            yield return new WaitForSeconds(attackPauseLength);
            Vector3 rightPos = rightLauncher.position + rightLauncher.TransformVector(new Vector3(0, -1f, 0));
            Transform leftRocket = Instantiate(rocketPrefab, rightPos, Quaternion.LookRotation((player.transform.position + Vector3.up * 2f - rightPos).normalized, Vector3.up)).transform;
            //leftRocket.Rotate(new Vector3(90, 0, 180));
            leftRocket.Translate(new Vector3(0, 0, 1), Space.Self);
            yield return new WaitForSeconds(attackPauseLength);
        }
    }
}
