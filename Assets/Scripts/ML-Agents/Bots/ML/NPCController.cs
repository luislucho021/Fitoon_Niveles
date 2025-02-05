using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] Animator anim;
    private Countdown countdownTimer;
    private FinishController finishController;
    private GoalController goalController;
    public GameObject trailBoost;
    private Transform charModel;
    private CreateBots botSpawner;
    private GameObject goal;

    private float moveV;
    private float moveH;
    private bool isFrozen = false;
    private float speedBoost = 1f;

    [Header("Movement")]
    public float moveSpeed = 7f;
    public float rotationSpeed = 1f;
    Vector3 moveDirection;

    [Header("Ground check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;
    public float groundDrag;

    private void OnEnable()
    {
        GoalController.onRaceFinishEvent += EndRace;
    }

    private void OnDisable()
    {
        GoalController.onRaceFinishEvent -= EndRace;
    }

    void EndRace()
    {
        LockMovement(true);
        //charModel.gameObject.SetActive(false);
        anim.SetBool("isRunning", false);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        countdownTimer = FindObjectOfType<Countdown>();
        finishController = FindObjectOfType<FinishController>();
        botSpawner = FindObjectOfType<CreateBots>();
        goalController = FindObjectOfType<GoalController>();

        charModel = anim.transform;
        goal = goalController.gameObject;
    }

    private void Start()
    {
        try
        {
            //si es botML_customizable
            anim = transform.GetChild(0).GetChild(0).GetComponentInChildren<Animator>();
        }
        catch
        {
            //si es botML
            anim = transform.GetChild(0).GetComponent<Animator>();
        }

        if (goalController != null) goalController.Reset();

        LockMovement(false);

        if (countdownTimer == null) countdownTimer = FindObjectOfType<Countdown>();
        if (finishController == null) finishController = FindObjectOfType<FinishController>();
        if (botSpawner == null) botSpawner = FindObjectOfType<CreateBots>();

        countdownTimer.Reset();
    }

    void Update()
    {
        //Ground check
        RaycastHit hit;
        grounded = Physics.Raycast(transform.position, Vector3.down, out hit, playerHeight * 0.5f + 3f, whatIsGround);

        SpeedControl();

        //Handle drag
        if (grounded)
        {
            rb.drag = groundDrag;
            anim.SetBool("isFalling", false);
        }

        else if(!grounded)
        {
            rb.drag = 0;
            anim.SetBool("isFalling", true);
        }

        if (MovementAllowed())
        {
            if (goalController != null)
            {
                goalController.AddPlayerToList(transform);
                goalController.UpdatePosition();
            }
        }

        if (speedBoost > 1f)
        {
            trailBoost.GetComponent<TrailRenderer>().emitting = true;
            trailBoost.GetComponentInChildren<ParticleSystem>().Play();
            speedBoost -= 0.04f;
            speedBoost = Mathf.Clamp(speedBoost, 1f, 10f);
        }

        else if (speedBoost <= 1f)
        {
            trailBoost.GetComponent<TrailRenderer>().emitting = false;
            trailBoost.GetComponentInChildren<ParticleSystem>().Stop();
        }
    }

    public void LockMovement(bool state)
    {
        isFrozen = state;
    }

    public bool MovementAllowed()
    {
        return (!isFrozen && countdownTimer.HasFinished() && !finishController.IsFinished());
    }

    private void FixedUpdate()
    {
        //MOVEMENT
        if (MovementAllowed())
        {
            anim.SetBool("isRunning", true);
            MoveNPC();
        }
    }

    #region CollisionChecks
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor") && countdownTimer.HasFinished() && !finishController.IsFinished())
        {
            LockMovement(false); //Unfreeze movement when the player touches the floor
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        SpeedBoost sB = collider.gameObject.GetComponent<SpeedBoost>();
        if (sB != null)
        {
            SetBoost(sB.speedBoost);
            sB.FadeAndRespawn();
        }

        if (goal != null && goal.CompareTag(collider.gameObject.tag))
        {
            //Debug.Log("FINISHED!");
            //finishController.Finish();
            if (goalController != null) goalController.RemovePlayerFromList(transform);
            //if (goalController != null) goalController.PlayerFinish();
        }
    }

    #endregion

    public void SetMovement(float moveV, float moveH)
    {
        if (countdownTimer.HasFinished())
        {
            this.moveV = moveV;
            this.moveH = moveH;
        }
    }

    public void SetBoost(float boost)
    {
        speedBoost = boost;
    }

    private void MoveNPC()
    {
        rb.AddForce(transform.forward * moveSpeed * 10f, ForceMode.Force);

        //Rotate Player based on Horizontal input
        Vector3 rotation = new Vector3(0, moveH * rotationSpeed, 0);
        Vector3 currentRotation = transform.rotation.eulerAngles;
        Vector3 limitedRotation = RotationLimited(currentRotation);
        rb.MoveRotation(Quaternion.Euler(limitedRotation + rotation));

        moveDirection = transform.forward * moveV + transform.right * moveH;
        if (moveV != 0) rb.AddForce(moveDirection.normalized * moveSpeed * 10f * speedBoost, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //Limit velocity
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private Vector3 RotationLimited(Vector3 rotation)
    {
        if (rotation.y > 90 && rotation.y < 270)
        {
            if (rotation.y < 180)
            {
                rotation.y = 90; // Ajustar a 90 si está entre 90 y 180
            }
            else
            {
                rotation.y = 270; // Ajustar a 270 si está entre 180 y 270
            }
        }
        return rotation;
    }

}
