using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    public float speedBoost = 1f;
    public Material matFade;
    public List<MeshRenderer> mrBoost;
    public bool isTemporary = true;

    [SerializeField] private List<Material> defaultMaterials;
    private Collider coll;


    private bool isRespawning = false;
    private float timer = 0f;
    private float timerMax = 200f;

    private void Start()
    {
        coll = GetComponent<Collider>();

        defaultMaterials.Capacity = mrBoost.Count;
        for (int i=0; i< mrBoost.Count; i++)
        {
            defaultMaterials.Add(mrBoost[i].material);
        }

    }

    private void Update()
    {
        if (isRespawning)
        {
            if (timer < timerMax)
            {
                timer += 1f;
            }
            else
            {
                for (int i = 0; i < mrBoost.Count; i++)
                {
                    mrBoost[i].material = defaultMaterials[i];
                }

                coll.enabled = true;
                timer = 0f;
                isRespawning = false;
            }
        }
        
    }

    public void FadeAndRespawn()
    {
        if (!isTemporary) return;

        for (int i = 0; i < mrBoost.Count; i++)
        {
            mrBoost[i].material = matFade;
        }

        coll.enabled = false;
        isRespawning = true;
    }


}
