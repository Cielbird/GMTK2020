using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public bool isFriendly;
    public float radius;
    public float maxDamage;
    public float duration = 1f;
    public float pitchVariance;
    public AudioSource source;

    void Start()
    {
        Collider[] recipients = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider recipient in recipients)
        {
            if (isFriendly)
            {
                Enemy enemy = recipient.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(maxDamage * Mathf.Max(radius - Vector3.Distance(transform.position, recipient.transform.position), 0f) / radius);
                }
            }
            else
            {
                PlayerMovement player = recipient.GetComponent<PlayerMovement>();
                if (player != null)
                {
                    player.TakeDamage(maxDamage * Mathf.Max(radius - Vector3.Distance(transform.position, recipient.transform.position), 0f) / radius);
                }
            }
        }
        source.pitch = source.pitch + UnityEngine.Random.Range(-pitchVariance / 2f, pitchVariance / 2f);
        Destroy(gameObject, duration); 
    }


}
