using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeShaders : MonoBehaviour
{
    [SerializeField] List<Material> tema1_materials;
    [SerializeField] List<Material> tema2_materials;
    [SerializeField] List<Material> tema3_materials;

    public void OnChangeShaderClick(int tema)
    {
        if(tema == 1)
        {
            foreach(Material mat in tema1_materials)
            {
                ChangeOutline(mat);
            }
        }

        else if(tema == 2)
        {
            foreach (Material mat in tema2_materials)
            {
                ChangeOutline(mat);
            }
        }

        else
        {
            foreach (Material mat in tema3_materials)
            {
                ChangeOutline(mat);
            }
        }
    }

    void ChangeOutline(Material mat)
    {
        if (mat.GetFloat("_OutlineSize") == 0) mat.SetFloat("_OutlineSize", 100);
        else mat.SetFloat("_OutlineSize", 0);
    }
}
