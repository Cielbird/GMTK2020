using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PlayerMovement : MonoBehaviour, IDamageable
{
    public float maxHP;
    //side, forewards, aim, jump, ranged, melee
    public float[] componentHealths;
    public MeshRenderer[] componentRenderers;
    
    public int ammo = 15;
    
    public float timeToFullSpeed;
    public float maxVelocity;
    public float minSpeedCutoff;
    public float gravity;
    public float jumpHeight;
    
    public CharacterController cc;
    public UIManager uiManager;
    public ArenaManager arenaManager;
    public GameObject deathSystem;
    public AudioSource source;
    public AudioSource source2;
    public AudioClip malfunction;
    public float malfunctionVolume = 2f;
    public float malfunctionPitch = 2f;
    public AudioClip cogSound;
    public float cogClipPitch = 0.5f;
    public float cogClipVolume = 0.5f;
    public float cogClipPitchVariance = 0.1f;
    public AudioClip ammoSound;
    public float ammoClipPitch = 0.5f;
    public float ammoClipVolume = 0.5f;
    public float ammoClipPitchVariance = 0.1f;
    public AudioClip landingClip;
    public float landingClipPitch = 0.5f;
    public float landingClipVolume = 0.5f;
    public float landingClipPitchVariance = 0.1f;

    public Vector2 walkDirection; //player's intentional mvmnt                     m/s/s
    public Vector2 walkMovement; //player's final capped walk movement      m/s
    public float verticalVelocity; //vertical velocity of player. is 0 if player is grounded
    Vector3 movement; //player's final movement
    float acceleration;
    float lastVerticalVelocity;
    bool grounded;
    bool lastGroundingState;
    public Transform bot;
    CameraMovement cam;

    private void Start()
    {
        acceleration = maxVelocity / timeToFullSpeed;
        movement = new Vector3();
        cam = GetComponentInChildren<CameraMovement>();

        componentHealths = new float[] { maxHP, maxHP, maxHP, maxHP, maxHP, maxHP };

        for (int i = 0; i < 6; i++)
        {
            uiManager.UpdateComponentHP(maxHP, i, true);
        }
    }

    private void Update()
    {
        //AIMING
        walkDirection = Vector2.zero; //reset mvmnt

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            
            if (Input.GetKey(KeyCode.A))
            {
                walkDirection.x += -1;
            }
            if (Input.GetKey(KeyCode.D))
            {
                walkDirection.x += 1;
            }
            if (Input.GetKey(KeyCode.W))
            {
                walkDirection.y += 1;
            }
            if (Input.GetKey(KeyCode.S))
            {
                walkDirection.y += -1;
            }

            if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)) && (componentHealths[0] <= 0))
            {
                Malfunction(0);
            }
            if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S)) && (componentHealths[1] <= 0))
            {
                Malfunction(1);
            }
        }
        walkDirection = walkDirection.normalized; //diagonal acceleration should be same size as other mvmnts 

        walkMovement += walkDirection * acceleration * Time.deltaTime;
        walkMovement = Vector3.ClampMagnitude(walkMovement, maxVelocity); //clamp velocity to maxVelocity


        if (walkDirection == Vector2.zero)
        {
            if (walkMovement.magnitude < minSpeedCutoff)
            {
                walkMovement = Vector2.zero;
            }
            else
            {
                walkMovement -= walkMovement.normalized * acceleration * Time.deltaTime;
            }
        }
        if (Physics.OverlapSphere(transform.TransformPoint(new Vector3(0, 0.4f, 0)), 0.5f, LayerMask.GetMask("Default", "Terrain")).Length > 0)
        {
            grounded = true;
            if (grounded && !lastGroundingState)
            {
                source.pitch = landingClipPitch + UnityEngine.Random.Range(-landingClipPitchVariance / 2f, landingClipPitchVariance / 2f);
                source.volume = landingClipVolume;// * lastVerticalVelocity;
                source.PlayOneShot(landingClip);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (componentHealths[3] > 0)
                {
                    verticalVelocity = jumpHeight;
                }
                else
                {
                    Malfunction(3);
                }
            }
            else
            {
                verticalVelocity = 0;
            }
        }
        else
        {
            grounded = false;
            verticalVelocity += gravity * Time.deltaTime;
        }
        movement = new Vector3(walkMovement.x, verticalVelocity, walkMovement.y) * Time.deltaTime;

        if (componentHealths[0] <= 0)
        {
            movement.Scale(new Vector3(0.1f, 1, 1));
        }
        if (componentHealths[1] <= 0)
        {
            movement.Scale(new Vector3(1, 1, 0.1f));
        }

        movement = transform.TransformDirection(movement);
        cc.Move(movement);


        lastGroundingState = grounded;
        lastVerticalVelocity = verticalVelocity;
    }

    public void Malfunction(int i)
    {
        source.pitch = malfunctionPitch;
        source.volume = malfunctionVolume;
        source.PlayOneShot(malfunction);
        uiManager.animateMalfunctioningControls(i);
    }

    public void TakeDamage(float amount)
    {
        List<int> healthyPartIDs = new List<int>();
        for(int i=0; i<6; i++)
        {
            if (componentHealths[i] > 0)
            {
                healthyPartIDs.Add(i);
            }
        }

        if (healthyPartIDs.Count == 0)
        {
            return;
        }

        int partDamaged = healthyPartIDs[Random.Range(0, healthyPartIDs.Count)];
        float newHP = Mathf.Max(componentHealths[partDamaged] - amount, 0);
        uiManager.UpdateComponentHP(newHP, partDamaged, false);
        componentHealths[partDamaged] = newHP;
        componentRenderers[partDamaged].material.SetFloat("_Dissolve", .37f - Mathf.Clamp(componentHealths[partDamaged] / maxHP, 0f, maxHP));

        
        if (isDead())
        {
            StartCoroutine(death());
        }
    }

    IEnumerator death()
    {
        arenaManager.endGame();
        cam.aimLocked = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Instantiate(deathSystem, transform.position, Quaternion.Euler(-90f, 0f, 0f));
        float t = Time.time;
        while (Time.time - t < 1.5)
        {
            foreach (MeshRenderer mesh in componentRenderers)
            {
                mesh.material.SetFloat("_Dissolve", mesh.material.GetFloat("_Dissolve") + Time.deltaTime * 0.5f);
            }
            yield return null;
        }

        yield return new WaitForSeconds(3);
        uiManager.playerDeath();
    }

    public bool isDead()
    {
        bool isDead = true;
        foreach (float componentHp in componentHealths)
        {
            if (componentHp > 0)
            {
                isDead = false;
                break;
            }
        }
        return isDead;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isDead())
        {
            if (other.tag == "Ammo")
            {
                Destroy(other.transform.parent.gameObject);
                source.pitch = ammoClipPitch + UnityEngine.Random.Range(-ammoClipPitchVariance / 2f, ammoClipPitchVariance / 2f);
                source.volume = ammoClipVolume;
                source.PlayOneShot(ammoSound);
                ammo += 5;
                uiManager.UpdateAmmo(ammo);
            }
            else if (other.tag == "Cog")
            {
                Destroy(other.transform.parent.gameObject);
                source2.pitch = cogClipPitch + UnityEngine.Random.Range(-cogClipPitchVariance / 2f, cogClipPitchVariance / 2f);
                source2.volume = cogClipVolume;
                source2.PlayOneShot(cogSound);
                //heal random component
                int index = Random.Range(0, 6);
                float newHP = Mathf.Min(componentHealths[index] + maxHP / 3, maxHP);
                uiManager.UpdateComponentHP(newHP, index, true);
                componentHealths[index] = newHP;
                componentRenderers[index].material.SetFloat("_Dissolve", .37f - Mathf.Clamp(componentHealths[index] / maxHP, 0f, maxHP));
            }
        }
    }
   
}
