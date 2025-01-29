using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectItem", menuName = "ScriptableObjects/ObjectItem", order = 2)]
[System.Serializable]
public class ObjectItem : ScriptableObject
{
    public int id;
    public Sprite icon;
    public Mesh mesh;
    public Material[] materials;
}
