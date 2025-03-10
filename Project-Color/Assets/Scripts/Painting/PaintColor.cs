using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PaintColor : MonoBehaviour
{
    public Color color1 = new Color(1,.5f,0);
    public Color color2 = new Color(1,0,0);
    public Color color3 = new Color(0,1,0);
    public Color color4 = new Color(0,0,1);

    public List<Shader> shaders = null;

    public static List<Material> GetAllMaterialsInScene()
    {
        List<Material> allMaterials = new List<Material>();
        HashSet<Material> uniqueMaterials = new HashSet<Material>();

        // Find all renderers in the scene
        Renderer[] renderers = GameObject.FindObjectsOfType<Renderer>();

        // Iterate through all renderers
        foreach (Renderer renderer in renderers)
        {
            // Get materials used by the renderer
            Material[] materials = renderer.sharedMaterials;

            // Add materials to the hash set to ensure uniqueness
            foreach (Material material in materials)
            {
                if (material != null && !uniqueMaterials.Contains(material))
                {
                    uniqueMaterials.Add(material);
                    allMaterials.Add(material);
                }
            }
        }

        return allMaterials;
    }
    
    void Start()
    {
        List<Material> materialsInScene = GetAllMaterialsInScene();

        // Change the shader splat colours for all materials
        foreach (Material material in materialsInScene)
        {
            foreach (Shader s in shaders)
            {
                if (material.shader == s)
                {
                    material.SetColor("_SplatColor1", color1);
                    material.SetColor("_SplatColor2", color2);
                    material.SetColor("_SplatColor3", color3);
                    material.SetColor("_SplatColor4", color4);
                }
            }
        }
    }
}
