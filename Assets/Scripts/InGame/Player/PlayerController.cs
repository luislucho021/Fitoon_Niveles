using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseRunner
{
	[SerializeField] float rotationSpeed = .5f;
    FaceTrackingToMovement faceTracking;

	void Awake()
    {
        BaseAwake();
		LoadCharacterData();
		LoadCharacter();
    }

	private void Start()
	{
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

	void LoadCharacterData()
	{
		SaveData.ReadFromJson();
		character = CharacterLoader.GetCharacter(SaveData.player.playerCharacterData);
	}
}
