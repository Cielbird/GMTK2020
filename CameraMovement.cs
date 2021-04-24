using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public PlayerMovement player;
    public Transform launcher;
    public Transform meleeHolder;

    public bool aimLocked;
    
    public float sensVertical;
    public float sensHorizontal;
    public float camAdjustSpeed;
    
    float camDistance;
    Vector3 startCamPos;
    LayerMask ignorePlayerMask;
    
    void Start()
    {
        startCamPos = transform.localPosition;
        ignorePlayerMask = LayerMask.GetMask("Terrain");
        camDistance = Vector3.Distance(transform.position, transform.parent.position);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update()
    {
        RaycastHit camRayHit;
        Transform parent = transform.parent;
        if(Physics.Raycast(new Ray(parent.position, transform.position-parent.position), out camRayHit, camDistance, ignorePlayerMask))
        {
            transform.position = Vector3.Lerp(transform.position, camRayHit.point, camAdjustSpeed);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, startCamPos, camAdjustSpeed);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                aimLocked = true;
            }
        }
        if (Input.GetMouseButtonDown(0) && Cursor.lockState == CursorLockMode.None && !player.isDead())
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            aimLocked = false;
        }
        //AIMING
        if (!aimLocked && Cursor.lockState == CursorLockMode.Locked)
        {
            transform.parent.parent.Rotate(new Vector3(0, Input.GetAxis("Mouse X") * sensHorizontal, 0));
            transform.parent.Rotate(new Vector3(-Input.GetAxis("Mouse Y") * sensVertical, 0, 0));
        }

        //make the launcher face the same way
        if (player.componentHealths[2] > 0) 
        {
            if (player.componentHealths[5] > 0)
            {
                meleeHolder.rotation = transform.rotation;
                meleeHolder.Rotate(new Vector3(-90, 0, 0));
            }
            if (player.componentHealths[4] > 0)
            {
                launcher.rotation = transform.rotation;
                launcher.Rotate(new Vector3(100, 180, 0));
            }
        }
    }

    public void lookAt(Transform target)
    {
        //Rotate base by finding signed angle from top view
        Vector3 forwardsHor = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        Vector3 targetHor = new Vector3(target.position.x - transform.position.x, 0, target.position.z - transform.position.z).normalized;
        float angle = Vector3.SignedAngle(forwardsHor, targetHor, transform.up);

        transform.parent.Rotate(new Vector3(0, angle * Time.deltaTime, 0));

        //rotate cam by finding two pitch angles (target and camera) and subtracting to pitch camera up or down
        Vector3 targetPos = target.position-transform.position+new Vector3(0, 1.5f, 0); //target
        Vector3 flatTargetPos = new Vector3(targetPos.x, 0, targetPos.z);
        Vector3 axis = new Vector3(targetPos.z, 0,-targetPos.x); //Axis: vector perpendicular to flat target position vector
        float targetAngle = Vector3.SignedAngle(targetPos, flatTargetPos, axis);

        Vector3 lookPos = transform.forward; //cam
        Vector3 flatLookPos = new Vector3(lookPos.x, 0, lookPos.z);
        float lookAngle = Vector3.SignedAngle(lookPos, flatLookPos, transform.right);

        transform.Rotate(new Vector3((lookAngle-targetAngle) * Time.deltaTime, 0, 0), Space.Self);

    }
}
