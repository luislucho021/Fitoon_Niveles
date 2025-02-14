using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class PlayerSkinController : NetworkBehaviour
{

    public Material matPlayer;

	/*private NetworkVariable<RandomColor> randColor = new NetworkVariable<RandomColor>(
            new RandomColor
            {
                _hue = 1f,
                _saturation = 0.8f,
                _value = 0.8f,
            }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public struct RandomColor : INetworkSerializable
    {
        public float _hue;
        public float _saturation;
        public float _value;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _hue);
            serializer.SerializeValue(ref _saturation);
            serializer.SerializeValue(ref _value);
        }
    }*/



	/*public override void OnStartServer()
	{
		base.OnStartServer();
        if (IsServer)
        {
            /*randColor.Value = new RandomColor
            {
                _hue = Random.Range(0f, 1f),
                _saturation = 0.8f,
                _value = Random.Range(0.5f, 0.8f),
            };

            Debug.Log("SERVER SKIN: " + randColor.Value);

            //GetComponentInChildren<MeshRenderer>().material.color = Color.HSVToRGB(randColor.Value._hue, randColor.Value._saturation, randColor.Value._value);
            foreach (SkinnedMeshRenderer mR in GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                foreach (Material mat in mR.materials)
                {
                    //if (mat.name == (matPlayer.name + " (Instance)")) mat.color = Color.HSVToRGB(randColor.Value._hue, randColor.Value._saturation, randColor.Value._value);
                }
            }                
                
            RandomColorClientRpc(randColor.Value._hue, randColor.Value._saturation, randColor.Value._value);
        }
        else
        {
            Debug.Log("CLIENT SKIN");
            //GetComponentInChildren<MeshRenderer>().material.color = Color.HSVToRGB(randColor.Value._hue, randColor.Value._saturation, randColor.Value._value);
            foreach (SkinnedMeshRenderer mR in GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                foreach (Material mat in mR.materials)
                {
                    if (mat.name == (matPlayer.name + " (Instance)")) mat.color = Color.HSVToRGB(randColor.Value._hue, randColor.Value._saturation, randColor.Value._value);
                }
            }
        }
    }*/

    //[TargetRpc]
    private void RandomColorClientRpc(float hue, float sat, float val)
    {
        //GetComponentInChildren<MeshRenderer>().material.color = Color.HSVToRGB(hue, sat, val);
        foreach (SkinnedMeshRenderer mR in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            foreach (Material mat in mR.materials)
            {
                if (mat.name == (matPlayer.name + " (Instance)")) mat.color = Color.HSVToRGB(hue, sat, val);
            }
        }
    }

}
