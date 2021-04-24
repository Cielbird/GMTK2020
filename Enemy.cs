using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, IDamageable
{
    public float hp;
    public float maxhp;
    public float speed;

    public float timeToDie = 2f;
    public float timeToDestroy = 0.5f;
    public float dissolveSpeed = 2f;
    public float explosionGrowthSpeed = 2f;

    public int dropQuantity;

    public PlayerMovement player;
    public NavMeshAgent agent;
    public Animation deathAnim;
    public Transform canvas;
    public Slider hpIndicator;
    public GameObject ammoPrefab;
    public GameObject cogPrefab;
    public bool useDeathSystem = false;
    public GameObject deathSystem;
    public MeshRenderer[] disolvingMeshes;

    protected float dissolveAmount = 0f;

    protected virtual void Start()
    {
        agent.speed = speed;
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }

    protected virtual void Update()
    {
        hpIndicator.value = 1 - hp / maxhp;
        canvas.forward = transform.position - Camera.main.transform.position;
    }

    public void TakeDamage(float amount)
    {
        if (hp > 0)
        {
            amount = Mathf.Clamp(amount, 0, hp);
            hp -= amount;

            if (hp <= 0)
            {
                InitiateDeath();
            }
        }
    }

    protected virtual void InitiateDeath()
    {
        StartCoroutine(Die());
    }

    protected IEnumerator Die()
    {
        hp = 0f;
        agent.enabled = false;
        canvas.gameObject.SetActive(false);
        deathAnim.Play();
        Instantiate(deathSystem, transform.position, Quaternion.Euler(-90f, 0f, 0f));

        yield return new WaitForSeconds(timeToDie);

        float t = Time.time;
        while (Time.time - t < timeToDestroy)
        {
            dissolveAmount += Time.deltaTime * dissolveSpeed;

            foreach(MeshRenderer mesh in disolvingMeshes)
            {
                mesh.material.SetFloat("_Dissolve", dissolveAmount);
            }
            yield return null;
        }

        //spawn loot
        for (int i = 0; i < dropQuantity; i++)
        {
            Vector3 pos = transform.position + new Vector3(Random.Range(-0.5f, 0.5f), 1.5f, Random.Range(-0.5f, 0.5f));
            if (Random.Range(0f, 1f) > 0.5f)
                Instantiate(ammoPrefab, pos, transform.rotation);
            else
                Instantiate(cogPrefab, pos, transform.rotation);
        }

        Destroy(gameObject);
    }
}
