using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class BaseRunner : NetworkBehaviour
{
    protected CharacterItem character;


	[SerializeField] protected float baseSpeed;
	[SerializeField] protected GameObject trailBoost;
	[SerializeField] protected LayerMask whatIsGround = LayerMask.NameToLayer("Game");
	[SerializeField] protected float runnerHeight = 2;

	Animator animator;
	protected Rigidbody rigidBody;

	protected float speedMultiplier = 1;
	protected bool canMove = true;
	protected void BaseAwake()
	{
		animator = GetComponent<Animator>();
		rigidBody = GetComponent<Rigidbody>();
		Freeze();
	}
	protected void BaseUpdate()
	{
		UpdateAnimator();
		UpdateBoostTrail();
	}
	void UpdateBoostTrail()
	{
		if (speedMultiplier > 1)
		{
			trailBoost.GetComponent<TrailRenderer>().emitting = true;
			trailBoost.GetComponentInChildren<ParticleSystem>().Play();
		}
		else
		{
			trailBoost.GetComponent<TrailRenderer>().emitting = false;
			trailBoost.GetComponentInChildren<ParticleSystem>().Stop();
		}
	}
	void UpdateAnimator()
	{
		RaycastHit hit;
		animator.SetBool("isFalling", Physics.Raycast(transform.position, Vector3.down, out hit, runnerHeight * 0.5f + 1f, whatIsGround));
		animator.SetBool("isRunning", rigidBody.velocity.magnitude > 0.3f);
		animator.SetFloat("playerSpeed", 0.3f + new Vector3(rigidBody.velocity.x, 0, rigidBody.velocity.z).magnitude / 10);
	}
	protected void LoadCharacter()
	{
		if(character == null)
		{
			Debug.LogError("Character Data is Null");
			return;
		}
		GameObject body = Instantiate(character.prefab, transform);
		GameObject[] gameObjects = body.GetComponentsInChildren<GameObject>();
		List<GameObject> hair = new List<GameObject>();
		List<GameObject> skin = new List<GameObject>();
		List<GameObject> top = new List<GameObject>();
		List<GameObject> bottom = new List<GameObject>();
		GameObject shoes = null;
		for(int i = 0; i < gameObjects.Length; i++)
		{
			if (gameObjects[i].tag is "Hair")
				hair.Add(gameObjects[i]);
			else if (gameObjects[i].tag is "Skin")
				skin.Add(gameObjects[i]);
			else if (gameObjects[i].tag is "Top")
				top.Add(gameObjects[i]);
			else if (gameObjects[i].tag is "Bottom")
				bottom.Add(gameObjects[i]);
			else if (gameObjects[i].tag is "Shoes")
				shoes = gameObjects[i];
		}
		foreach (GameObject gameObject in hair)
		{
			gameObject.GetComponent<SkinnedMeshRenderer>().material.color = character.hairColor;
		}
		foreach (GameObject gameObject in skin)
		{
			gameObject.GetComponent<SkinnedMeshRenderer>().material.color = character.skinColor;
		}
		foreach (GameObject gameObject in top)
		{
			gameObject.GetComponent<SkinnedMeshRenderer>().material.color = character.topColor;
		}
		foreach (GameObject gameObject in bottom)
		{
			gameObject.GetComponent<SkinnedMeshRenderer>().material.color = character.bottomColor;
		}
		if(shoes != null)
		{
			shoes.GetComponent<SkinnedMeshRenderer>().sharedMesh = character.shoes.mesh;
		}
	}
	public void Boost(float amount, float duration)
	{
		StartCoroutine(BoostCoroutine(amount, duration));
	}
	public void Freeze()
	{
		rigidBody.constraints = RigidbodyConstraints.FreezeAll;
		canMove = false;
	}
	public void UnFreeze()
	{
		rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
		canMove = true;
	}
	IEnumerator BoostCoroutine(float amount, float duration)
	{
		speedMultiplier += amount;
		float decay = duration / Time.fixedDeltaTime;
		float counter = 0;
		while(counter < amount)
		{
			yield return new WaitForFixedUpdate();
			counter += decay;
			speedMultiplier -= decay;
		}
	}
}
