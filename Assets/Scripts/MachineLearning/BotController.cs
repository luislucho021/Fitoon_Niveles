using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.Networking;

public class BotController : NetworkBehaviour
{
    public Animator anim;
    public GameObject trailBoost;

    private float movementSpeed = 7f;
    private float speedMultiplier = 1f;
    private float speedBoost = 1f;
    private float rotationSpeed = 1f;

    private float moveV;
    private float moveH;
    private bool isFrozen = false;

    private Transform charModel;
    private Rigidbody rB;
    private Countdown countdownTimer;
    private GoalController goal;
    private TextMeshPro textHolder;

    private bool isRecovering = false;
    private bool isEffectApplied = false;


    private void Awake()
    {
        charModel = anim.transform;

        countdownTimer = FindObjectOfType<Countdown>();
        goal = FindObjectOfType<GoalController>();
        goal.AddPlayerToList(transform);

        rB = GetComponent<Rigidbody>();

        textHolder = GetComponentInChildren<TextMeshPro>();
        //textHolder.text = name;
        textHolder.text = "";

        rB.isKinematic = false;
        rB.useGravity = false;
        rB.interpolation = RigidbodyInterpolation.None;
        rB.collisionDetectionMode = CollisionDetectionMode.Continuous;

        movementSpeed -= UnityEngine.Random.Range(0f, movementSpeed / 3);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        Destroy(gameObject);
    }

    private void Update()
    {
        if (IsServer)
        {
            if (anim.gameObject.activeSelf)
            {
                if (rB.velocity != Vector3.zero)
                {
                    anim.SetBool("isRunning", true);
                    anim.SetFloat("playerSpeed", 0.3f + new Vector3(rB.velocity.x, 0, rB.velocity.z).magnitude / 10);
                    UpdateAnimationParametersClientRpc(anim.GetBool("isRunning"), anim.GetFloat("playerSpeed"));
                }
                else
                {
                    anim.SetBool("isRunning", false);
                    UpdateAnimationParametersClientRpc(anim.GetBool("isRunning"), anim.GetFloat("playerSpeed"));
                }
            }
        }

        if (speedBoost > 1f)
        {
            speedBoost -= 0.01f;
            speedBoost = Mathf.Clamp(speedBoost, 1f, 10f);
        }
        else
        {
            if (trailBoost.GetComponent<TrailRenderer>().emitting) trailBoost.GetComponent<TrailRenderer>().emitting = false;
            if (trailBoost.GetComponentInChildren<ParticleSystem>().isPlaying) trailBoost.GetComponentInChildren<ParticleSystem>().Stop();
        }

    }

    [ClientRpc]
    private void UpdateAnimationParametersClientRpc(bool isRunning, float speed)
    {
        anim.SetBool("isRunning", isRunning);
        anim.SetFloat("playerSpeed", speed);
    }

    void FixedUpdate()
    {
        Vector3 fwd = transform.forward * moveV * movementSpeed * speedMultiplier * speedBoost;
        Vector3 rotation = new Vector3(0, moveH * rotationSpeed, 0);

        Vector3 currentRotation = transform.rotation.eulerAngles;

        if (!isFrozen)
        {
            if (rB.velocity.y > 0) rB.velocity = fwd + new Vector3(0, rB.velocity.y, 0);
            else rB.velocity = fwd + new Vector3(0, rB.velocity.y, 0);

            rB.MoveRotation(Quaternion.Euler(currentRotation + rotation));
            charModel.transform.rotation = Quaternion.Euler(currentRotation + rotation);
        }

        //Add extra gravity force
        rB.AddForce(new Vector3(0, Physics.gravity.y * 2, 0), ForceMode.Acceleration);


        //charModel.transform.rotation = Quaternion.LookRotation(new Vector3(rB.velocity.x, 0, rB.velocity.z));
    }
    
    public void SetMovement(float moveV, float moveH)
    {
        if (countdownTimer.HasFinished())
        {
            this.moveV = moveV;
            this.moveH = moveH;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.GetComponent<SpeedBoost>() != null)
        {
            trailBoost.GetComponent<TrailRenderer>().emitting = true;
            trailBoost.GetComponentInChildren<ParticleSystem>().Play();
            SpeedBoost sB = collider.gameObject.GetComponent<SpeedBoost>();
            speedBoost = sB.speedBoost;
            sB.FadeAndRespawn();
        }
        if (goal.CompareTag(collider.gameObject.tag))
        {
            goal.RemovePlayerFromList(transform);
            gameObject.GetComponent<NetworkObject>().Despawn();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            LockMovement(false);
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.GetComponent<SpeedFloor>() != null)
        {
            SpeedFloor sF = collider.gameObject.GetComponent<SpeedFloor>();
            speedMultiplier = sF.speedMultiplier;
        }
        Effect effect = collider.GetComponent<Effect>();
        if (effect != null && !isEffectApplied && !isRecovering)
        {
            StartCoroutine(EffectTimer(effect));
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        speedMultiplier = 1f;
    }
    
    public void LockMovement(bool state)
    {
        isFrozen = state;
    }

    public void SetBoost(float boost)
    {
        speedBoost = boost;
    }

    IEnumerator EffectTimer(Effect effect)
    {
        effect.applyEffect(gameObject);

        isEffectApplied = true;

        yield return new WaitForSeconds(effect.effectDuration);

        StartCoroutine(RecoverTimer(effect));
        isEffectApplied = false;
    }
    IEnumerator RecoverTimer(Effect effect)
    {
        effect.removeEffect(gameObject);

        Debug.Log(name + " is in invencibility time");

        isRecovering = true;

        yield return new WaitForSeconds(effect.recoverDuration);

        isRecovering = false;
    }
}
