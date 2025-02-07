using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class PlayerController : BaseRunner
{
    FaceTrackingToMovement faceTracking;

	void Awake()
    {
        BaseAwake();
		LoadCharacter(LoadCharacterData());
    }

	private void Start()
	{
		GameManager.AddPlayer(this);
		faceTracking = GetComponent<FaceTrackingToMovement>();
	}

	private void FixedUpdate()
	{
		if (faceTracking == null || !faceTracking.detectado || !canMove)
		{
			return;
		}

		rigidBody.rotation = Quaternion.Slerp(rigidBody.rotation, faceTracking.faceRotation, rotationSpeed);
		rigidBody.AddForce(transform.forward * faceTracking.speed * Mathf.Max(0.1f, speedMultiplier) * baseSpeed, ForceMode.VelocityChange);
	}

	void Update()
	{
		BaseUpdate();
	}

	public override void OnStopClient()
	{
		GameManager.RemoveRunner(this);
	}
	Character LoadCharacterData()
	{
		SaveData.ReadFromJson();
		return CharacterLoader.GetCharacter(SaveData.player.playerCharacterData);
	}
}
