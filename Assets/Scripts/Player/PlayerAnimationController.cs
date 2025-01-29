using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationController : NetworkBehaviour
{
    private Animator anim;
    private Rigidbody rB;


    private void Start()
    {
        anim = GetComponent<Animator>();

        rB = transform.parent.GetComponent<Rigidbody>();
    }


    private void Update()
    {
        if (IsOwner && IsSpawned)
        {
            if (rB.velocity != Vector3.zero)
            {

                if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
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
