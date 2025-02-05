using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class BaseRunner : NetworkBehaviour
{
    CharacterItem character;


	[SerializeField] float baseSpeed;
	[SerializeField] GameObject trailBoost;

	Animator animator;
	Rigidbody rigidBody;

    bool canMove;
	List<float> speedMultipliers = new List<float>();
	protected void Awake()
	{
		animator = GetComponent<Animator>();
		rigidBody = GetComponent<Rigidbody>();
		canMove = false;
	}
	protected void LoadCharacter()
	{
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
}
