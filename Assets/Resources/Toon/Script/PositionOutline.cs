using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[ExecuteInEditMode]
public class PositionOutline : MonoBehaviour
{
    public Shader Toon;
    public Shader OutlineShader;
    public Material outlineMaterial;
    public Renderer rendererTarget;

    [HideInInspector] public Material ToonMaterial;
    [HideInInspector] public Material New_OutlineMaterial;
    [HideInInspector] public List<Material> ListRenderMaterials;

    [HideInInspector] public float fireRate = 0.1f;
    [HideInInspector] private float nextFire = 0.0F;
    [HideInInspector] public bool isRemoved;


    void Update()
    {

        // Check if its Used
        if (New_OutlineMaterial && ToonMaterial && rendererTarget)
        {
            if (ToonMaterial.GetFloat("_OutLineMode") == 1 || ToonMaterial.GetFloat("_AddOutline") == 0 || ToonMaterial.GetFloat("_UseOutlines") == 0)
            {
                New_OutlineMaterial.SetFloat("_Scale", 0);

                return;
            }
        }

        // if its Used

        if (Time.time > nextFire)
        {
            this.hideFlags = HideFlags.HideInInspector;

            if (New_OutlineMaterial != null)
            {
                //Debug.LogWarning($"{name}: New_OutlineMaterial is assigned.");
                New_OutlineMaterial.hideFlags = HideFlags.HideInInspector;
            }
            else
            {
                Debug.LogWarning($"{name}: New_OutlineMaterial is not assigned.");
                return;
            }

            nextFire = Time.time + fireRate;

            // Check if Toon material was removed

            if (rendererTarget)
            {

                List<Material> ListRenderMaterialss = new List<Material>(rendererTarget.sharedMaterials);
                List<Material> ToonMaterials = new List<Material>();


                foreach (Material material in ListRenderMaterialss)
                {
                    if (material.shader == Toon)
                    {
                        ToonMaterials.Add(material);
                    }
                }

                isRemoved = ToonMaterials.Count > 0 ? false : true;
            }

            if (isRemoved)
            {
                RemoveEverything();
            }

        }


        CheckDuo();
        UpdateValues();
    }

    public void CheckDuo()
    {

        List<PositionOutline> PositionOutline_LIST = new List<PositionOutline>();

        if (PositionOutline_LIST.Count > 1)
        {
            if (PositionOutline_LIST.IndexOf(this) != 0)
            {
                DestroyImmediate(this);
            }
        }
    }

    public void SetUp(Material mat)
    {

        rendererTarget = gameObject.GetComponent<Renderer>();

        if (!Application.isPlaying)
        {

            New_OutlineMaterial = mat;
            ListRenderMaterials = new List<Material>(rendererTarget.sharedMaterials);

            // add toon material reference
            foreach (Material material in ListRenderMaterials)
            {
                if (material.shader == Toon)
                {
                    ToonMaterial = material;
                    break;
                }

            }

            // check if "outline material exists"
            foreach (Material material in ListRenderMaterials)
            {
                if (material.shader == OutlineShader)
                {
                    return;
                }
            }


            // add outline material
            ListRenderMaterials.Add(New_OutlineMaterial);
            rendererTarget.materials = ListRenderMaterials.ToArray();
        }
    }
    public void FixOutline()
    {
        rendererTarget = gameObject.GetComponent<Renderer>();

        // check if "outline material exists"
        for (int i = ListRenderMaterials.Count - 1; i >= 0; i--)
        {
            if (ListRenderMaterials[i].shader == OutlineShader)
            {
                ListRenderMaterials.RemoveAt(i);
                rendererTarget.materials = ListRenderMaterials.ToArray();
                SetUp(outlineMaterial);
            }
        }

    }
    public void UpdateValues()
    {
        New_OutlineMaterial.SetColor("_OutlineColor", ToonMaterial.GetColor("_OutlineColor"));
        New_OutlineMaterial.SetFloat("_Scale", ToonMaterial.GetFloat("_OutlineWidth"));
    }
    public void RemoveEverything()
    {
        rendererTarget = gameObject.GetComponent<Renderer>();

        // check the materials list
        List<Material> materialList = new List<Material>(rendererTarget.sharedMaterials);

        // remove material from list
        materialList.Remove(New_OutlineMaterial);
        rendererTarget.materials = materialList.ToArray();

        //delete this component
        DestroyImmediate(this);
    }
}
