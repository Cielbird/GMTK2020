using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class SpearEnemy : Enemy
{
    public float attackPauseLength;
    
    public Transform leftSpear;
    public Transform rightSpear;
    public Animation leftSpearAnim;
    public Animation rightSpearAnim;
    public GameObject sparksPrefab;
    public AudioSource source;
    public AudioClip melee;
    public float meleeVolume = 0.8f;
    public float meleePitch = 1f;
    public float meleePitchVariance = 0.1f;
    public AudioClip miss;
    public float missPitch = 7f;
    public float missPitchVariance = 0.5f;
    public float missVolume = 0.5f;


    public float damage;
    

    protected override void Start()
    {
        base.Start();

        StartCoroutine(RepeatStab());
    }

    protected override void Update()
    {
        base.Update();


        if (agent.enabled)
        {
            transform.forward = new Vector3(player.transform.position.x - transform.position.x, 0, player.transform.position.z - transform.position.z);
            agent.destination = player.transform.position;
        }
        if (Vector3.Distance(transform.position, player.transform.position) < 4f && agent.enabled)
        {
            print("book");
            agent.speed = 0;
        }
        else
        {
            agent.speed = speed;
        }
    }
    
    protected override void InitiateDeath()
    {
        base.InitiateDeath();

        StopCoroutine(RepeatStab());
    }

    IEnumerator RepeatStab()
    {
        while (agent.enabled)
        {
            if (Vector3.Distance(transform.position, player.transform.position) < 5f)
            {
                Stab(leftSpear, leftSpearAnim);
                Stab(rightSpear, rightSpearAnim);
            }
            yield return new WaitForSeconds(attackPauseLength);
        }
    }

    void Stab(Transform spear, Animation spearAnim)
    {
        spearAnim.Play();
        RaycastHit hit;
        if (Physics.SphereCast(new Ray(spear.position, -spear.up), 0.5f, out hit, 3f, LayerMask.GetMask("Player")))
        {
            source.volume = meleeVolume;
            source.pitch = meleePitch + Random.Range(-meleePitchVariance / 2f, meleePitchVariance / 2f);
            source.PlayOneShot(melee);
            Instantiate(sparksPrefab, hit.point, Quaternion.identity).transform.up = hit.normal;
            IDamageable damageable = hit.transform.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
        }
        else
        {
            source.volume = missVolume;
            source.pitch = missPitch + Random.Range(-missPitchVariance / 2f, missPitchVariance / 2f);
            source.PlayOneShot(miss);
        }
    }
}
