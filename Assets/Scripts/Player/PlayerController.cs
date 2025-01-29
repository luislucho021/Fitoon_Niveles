using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    public CharacterItem playerCharacter;

    public float moveSpeed = 7f;
    [SerializeField] private float speedMultiplier = 1f;
    [SerializeField] private float speedBoost = 1f;
    public float rotationSpeed = 1f;
    public Animator anim;
    public GameObject trailBoost;

    [SerializeField] private bool autoRun = false;

    private float movementV = 0;
    private float movementH = 0;

    private Countdown countdownTimer;
    private FinishController finishController;
    private GoalController goalController;
    private FloatingJoystick joystick;
    private Rigidbody rB;
    private NetworkData netData;
    private Camera mainCamera;
    private int playerSPIndex = -1;
    private Transform charModel;
    private BotSpawner botSpawner;
    private GameObject goal;
    private bool isFrozen = false;
    private Vector3 gravityDirection;
    private bool isRecovering = false;
    private bool isEffectApplied = false;


    private NetworkVariable<PlayerMovement> playerMovement = new NetworkVariable<PlayerMovement>(
            new PlayerMovement
            {
                _moveV = 0,
                _moveH = 0,
                _position = Vector3.zero,
            }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public struct PlayerMovement : INetworkSerializable
    {
        public float _moveV;
        public float _moveH;
        public Vector3 _position;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _moveV);
            serializer.SerializeValue(ref _moveH);
            serializer.SerializeValue(ref _position);
        }
    }


    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (IsOwner) mainCamera.enabled = true;

        // Generate a bot in the player's place if leaving after the race has started
        //if (MovementAllowed()) botSpawner.SpawnBot(transform.position);

        //Remove Player from the Players List when despawn on Server side
        if (goalController != null) goalController.RemovePlayerFromList(transform);

        //Remove Player from the Spawnpoint List when despawn on Server side
        if (IsServer) netData.RemovePlayerFromSpawnpoint(playerSPIndex, netData.spawnpointData[playerSPIndex]._spPosition, false, "");
    }


    [ClientRpc]
    private void SendSpawnpointDataClientRpc(int spIndex, Vector3 spPosition)
    {
        transform.position = new Vector3(spPosition.x, spPosition.y + 1, spPosition.z);
        playerSPIndex = spIndex;
        Debug.Log("[CLIENT RPC]-Player spawned at SP: " + spIndex);
    }

    [ClientRpc]
    private void StartCountdownClientRpc()
    {
        countdownTimer.StartCountdown();
    }


    private void Awake()
    {
        Debug.Log("PLAYER AWAKE");
        countdownTimer = FindObjectOfType<Countdown>();
        finishController = FindObjectOfType<FinishController>();
        botSpawner = FindObjectOfType<BotSpawner>();
        goalController = FindObjectOfType<GoalController>();
        goal = goalController.gameObject;

        //Asignar personaje guardado
        GameObject playerContainer = GameObject.FindGameObjectWithTag("Player");
        GameObject characterInPrefab = GameObject.FindGameObjectWithTag("Character");
        GameObject newCharacter = Instantiate(playerCharacter.prefab, characterInPrefab.transform.position, Quaternion.identity, playerContainer.transform);
        newCharacter.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        Destroy(newCharacter.GetComponent<CapsuleCollider>());
        Destroy(newCharacter.GetComponent<RotateCharacter>());
        newCharacter.AddComponent<PlayerAnimationController>();
        newCharacter.transform.SetSiblingIndex(0);

        Destroy(characterInPrefab);
       
        
    }

    private void Start()
    {
        if (IsOwner)
        {
            joystick = FindObjectOfType<FloatingJoystick>();
            joystick.enabled = true;
            joystick.GetComponentInChildren<Transform>().gameObject.SetActive(true);

            mainCamera = Camera.main;

            if (goalController != null) goalController.Reset();
        }

        charModel = anim.transform;

        rB = GetComponent<Rigidbody>();

        rB.isKinematic = false;
        rB.useGravity = false;
        gravityDirection = new Vector3(0, Physics.gravity.y * 2, 0);
        if (IsOwner) rB.interpolation = RigidbodyInterpolation.None;
        else rB.interpolation = RigidbodyInterpolation.None;
        rB.collisionDetectionMode = CollisionDetectionMode.Continuous;

        //-------------------------------------------------------------------------------------------

        if (countdownTimer == null) countdownTimer = FindObjectOfType<Countdown>();
        if (finishController == null) finishController = FindObjectOfType<FinishController>();
        if (botSpawner == null) botSpawner = FindObjectOfType<BotSpawner>();
        if (netData == null) netData = FindObjectOfType<NetworkData>();

        countdownTimer.Reset();

        if (IsServer)
        {
            //When a Player spawn, the Server searches for a not occupied spawnpoint from the NetworkList for him.
            SearchNextFreeSpawnPoint();

            //Each time a Player joins the lobby his Server side checks num of players connected to that lobby and starts the countdown when full
            //CheckConnectedPlayers();
        }
    }

    private void SearchNextFreeSpawnPoint()
    {
        Debug.Log("[SERVER]-SP Count: " + netData.spawnpointData.Count);

        for (int i = 0; i <= netData.spawnpointData.Count; i++)
        {
            Debug.Log("[SERVER]-SP Current: " + i);

            if (i == netData.spawnpointData.Count)
            {
                Debug.Log("[SERVER]-The room is full, exiting...");
                //matchMaker.DestroyOrExitLobby();
                //mainCamera.enabled = true;
                break;
            }

            if (!netData.spawnpointData[i]._isOccupied)
            {
                //Updates the spawnpoint NetworkList with this spawnpoint as isOccupied and the playerId on it
                netData.UpdateSpawnpointsList(i, netData.spawnpointData[i]._spPosition, true, AuthenticationService.Instance.PlayerId);

                transform.position = new Vector3(netData.spawnpointData[i]._spPosition.x, netData.spawnpointData[i]._spPosition.y + 1, netData.spawnpointData[i]._spPosition.z);
                //playerMovement.Value = new PlayerMovement { _moveV = 0, _moveH = 0, _position = transform.position };
                playerSPIndex = i;
                Debug.Log("[SERVER]-Player spawned at SP: " + i);

                //Calls a ClientRPC to tell their relative Client Player his spawnpoint position
                SendSpawnpointDataClientRpc(i, netData.spawnpointData[i]._spPosition);

                break;
            }
        }
    }

    private void CheckConnectedPlayers()
    {
        if (netData.GetPlayerCount() == netData.GetCurrentMaxPlayersPerLobby())/// ToDo: Add 60s timer and fill with bots if it ends
        {
            Debug.Log("START");
            botSpawner.FillSpawnpointsWithBots(); //Spawn Bots
            StartCountdownClientRpc();//Send RPC to clients to start countdown aswell
        }
    }


    private void Update()
    {

        if (MovementAllowed())
        {
            if (goalController != null) goalController.AddPlayerToList(transform);
        }

        //The client owner controls his player with the input
        if (IsOwner)
        {
            if (MovementAllowed()) goalController.UpdatePosition();

            //The player moves instantly on his client
            movementV = Math.Clamp( Mathf.Round(joystick.Vertical * 10) / 10 + Mathf.Round(Input.GetAxis("Vertical") * 10) / 10 , 0, 1 );
            movementH = Math.Clamp( Mathf.Round(joystick.Horizontal * 10) / 10 + Mathf.Round(Input.GetAxis("Horizontal") * 10) / 10 , -1, 1 );

            movementV = Mathf.Clamp01(movementV);
            if (autoRun) movementV = 1f;

            playerMovement.Value = new PlayerMovement { _moveV = movementV, _moveH = movementH, _position = playerMovement.Value._position };
        }

        if (speedBoost > 1f)
        {
            trailBoost.GetComponent<TrailRenderer>().emitting = true;
            trailBoost.GetComponentInChildren<ParticleSystem>().Play();
            speedBoost -= 0.01f;
            speedBoost = Mathf.Clamp(speedBoost, 1f, 10f);
        }
        else
        {
            trailBoost.GetComponent<TrailRenderer>().emitting = false;
            trailBoost.GetComponentInChildren<ParticleSystem>().Stop();
        }

    }


    private void FixedUpdate()
    {
        //Applys physical movement to the player
        
        //If isOwner, apply movement instantly from the input
        if (IsOwner)
        {
            if (MovementAllowed())
            {
                Vector3 fwd = transform.forward.normalized * movementV * moveSpeed * speedMultiplier * speedBoost;
                Vector3 rotation = new Vector3(0, movementH * rotationSpeed, 0);

                Vector3 currentRotation = transform.rotation.eulerAngles;

                //Apply Forward PlayerController based on Vertical input
                //if (rB.velocity.y > 0) rB.velocity = new Vector3(fwd.x, 0, fwd.z);
                //else rB.velocity = fwd + new Vector3(0, rB.velocity.y, 0);
                rB.velocity = fwd + new Vector3(0, rB.velocity.y, 0);

                //Rotate Player based on Horizontal input
                rB.MoveRotation(Quaternion.Euler(currentRotation + rotation));
            }

            if (Input.GetKeyDown(KeyCode.Space) && MovementAllowed())
            {
                LockMovement(true);

                rB.AddForce(transform.up * 8, ForceMode.VelocityChange);
            }

            //Apply extra gravity to the Player to force fast falling
            rB.AddForce(gravityDirection, ForceMode.Acceleration);

        }

    }

    public void LockMovement(bool state)
    {
        isFrozen = state;
    }

    private bool MovementAllowed()
    {
        return (!isFrozen && countdownTimer.HasFinished() && !finishController.IsFinished());
    }

    #region CollisionChecks
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            //gravityDirection = -collision.GetContact(0).normal;
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

        if (goal !=null && goal.CompareTag(collider.gameObject.tag))
        {
            Debug.Log("FINISHED!");
            finishController.Finish();
            if (goalController != null) goalController.RemovePlayerFromList(transform);
            if (goalController != null) goalController.PlayerFinish();
            StopCharacterOnFinish();
        }
    }

    public void StopCharacterOnFinish()
    {
        rB.isKinematic = true;
        rB.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rB.velocity = Vector3.zero;
        rB.rotation = Quaternion.identity;
        rB.detectCollisions = false;
       // charModel.gameObject.SetActive(false);
        GetComponent<PlayerNameHolder>().textHolder.SetActive(false);
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
        if (collider.gameObject.GetComponent<SpeedFloor>() != null) speedMultiplier = 1f;
    }

    #endregion

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

        Debug.Log("Invencibility time has finished");
        isRecovering = false;
    }
}
