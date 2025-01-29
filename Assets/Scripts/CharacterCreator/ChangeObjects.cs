using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeObjects : MonoBehaviour
{
    GameObject[] selectedObjects;

    //cuando se pulsa el boton de "shoes" hay que buscar los zapatos en la escena
    public void SetSelectedObjectType(string type)
    {
        selectedObjects = GameObject.FindGameObjectsWithTag(type);
    }

    //cuando se pulsa el zapato en cuestion, cambiar la malla actual por esa
    public void ChangeObject(ObjectItem objectItem)
    {
        foreach(GameObject obj in selectedObjects)
        {
            SkinnedMeshRenderer renderer = obj.GetComponent<SkinnedMeshRenderer>();
            renderer.sharedMesh = objectItem.mesh;
            renderer.materials = objectItem.materials;
            obj.GetComponent<WhatShoeIHave>().myShoe = objectItem;
        }
    }

}
