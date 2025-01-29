using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationController : NetworkBehaviour
{
    private Animator anim;
    private FloatingJoystick joystick;
    private Rigidbody rB;


    private void Start()
    {
        anim = GetComponent<Animator>();

        joystick = FindObjectOfType<FloatingJoystick>();
        joystick.enabled = true;
        joystick.GetComponentInChildren<Transform>().gameObject.SetActive(true);

        rB = transform.parent.GetComponent<Rigidbody>();
    }


    private void Update()
    {
        if (IsOwner && IsSpawned)
        {
            if (rB.velocity != Vector3.zero)
            {
                if (joystick.Horizontal != 0 || joystick.Vertical != 0 || Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
                {
                    anim.SetBool("isRunning", true);
                    anim.SetFloat("playerSpeed", 0.3f + new Vector3(rB.velocity.x, 0, rB.velocity.z).magnitude / 10);
                    UpdateAnimationParametersServerRpc(anim.GetBool("isRunning"), anim.GetFloat("playerSpeed"));
                }
                else 
                {
                    anim.SetBool("isRunning", false);
                    UpdateAnimationParametersServerRpc(anim.GetBool("isRunning"), anim.GetFloat("playerSpeed"));
                } 
            }
            else
            {
                anim.SetBool("isRunning", false);
                UpdateAnimationParametersServerRpc(anim.GetBool("isRunning"), anim.GetFloat("playerSpeed"));
            }
        }
            
    }

    [ServerRpc]
    private void UpdateAnimationParametersServerRpc(bool isRunning, float speed)
    {
        anim.SetBool("isRunning", isRunning);
        anim.SetFloat("playerSpeed", speed);
    }


}
