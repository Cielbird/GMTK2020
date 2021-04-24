using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BotMovement : MonoBehaviour
{
    public PlayerMovement player;
    public Transform meleeWeapon;
    public Transform rangedWeapon;
    public Transform rotator;
    public Transform wheel;
    public Animator animator;
    public Animation animation;

    public GameObject rocketPrefab;
    public GameObject sparksPrefab;
    public AudioSource source;
    public AudioClip melee;
    public AudioClip miss;
    public AudioClip click;
    public float clickVolume = 1f;
    public float meleePitch = 1f;
    public float meleePitchVariance = 0.2f;
    public float meleeVolume = 1f;
    public float missPitch = 1f;
    public float missPitchVariance = 0.2f;
    public float missVolume = 1f;

    public bool aimWasBroken;
    public float animationSmoothingMultiplier = 1f;
    public Quaternion normalAim;
    public Quaternion aimWhenBroken;
    
    Vector2 smoothedMovement = Vector2.zero;
    RaycastHit aimTarget;

    void Update()
    {
        if (player.componentHealths[2] <= 0) 
        {
            if (!aimWasBroken)
            {
                aimWhenBroken = transform.parent.rotation;
            }
            transform.parent.rotation = aimWhenBroken;
        }
        else
        {
            transform.parent.localRotation = Quaternion.identity;
        }
        aimWasBroken = player.componentHealths[2] <= 0;

        if (Input.GetMouseButtonDown(1) && !player.isDead())
        {
            MeleeAttack(10);
        }
        if (Input.GetMouseButtonDown(0) && !player.isDead())
        {
            RangedAttack();
        }

        Vector3 walkVector = player.walkDirection;
        smoothedMovement = Vector2.MoveTowards(smoothedMovement, walkVector, Time.deltaTime * Vector3.Magnitude(player.walkDirection - smoothedMovement) * animationSmoothingMultiplier);

        animator.SetFloat("Forward", smoothedMovement.y);
        animator.SetFloat("Strafe", smoothedMovement.x);
    }
    
    public void MeleeAttack(float amount)
    {
        if (player.componentHealths[5] > 0)
        {
            if (!animation.isPlaying)
            {
                animation.Play();
                RaycastHit hit;
                if (Physics.SphereCast(new Ray(meleeWeapon.position, -meleeWeapon.up), 0.5f, out hit, 5f, LayerMask.GetMask("Terrain", "Default")))
                {
                    source.volume = meleeVolume;
                    source.pitch = meleePitch + Random.Range(-meleePitchVariance / 2f, meleePitchVariance / 2f);
                    source.PlayOneShot(melee);
                    Instantiate(sparksPrefab, hit.point, Quaternion.identity).transform.up = hit.normal;
                    IDamageable damageable = hit.transform.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.TakeDamage(amount);
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
        else
        {
            player.Malfunction(5);
        }
    }

    public void RangedAttack()
    {
        if (player.componentHealths[4] > 0)
        {
            if (player.ammo > 0)
            {
                Transform rocket = Instantiate(rocketPrefab, rangedWeapon.position, rangedWeapon.rotation).transform;
                bool aimingAtSomething = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out aimTarget, 1000f, LayerMask.GetMask("Default", "Terrain"));
                if (player.componentHealths[2]>0 && aimingAtSomething)
                {
                    rocket.forward = aimTarget.point - rocket.position;
                }
                else
                {
                    rocket.forward = -rangedWeapon.up;
                }
                rocket.Translate(new Vector3(0, 0, 1), Space.Self);
                player.ammo--;
                player.uiManager.UpdateAmmo(player.ammo);
            }
            else
            {
                source.volume = clickVolume;
                source.PlayOneShot(click);
            }
        }
        else
        {
            player.Malfunction(4);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(aimTarget.point, 0.5f);
    }
}
