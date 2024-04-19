using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paintable : MonoBehaviour
{
    public Shader paintableShader;
    private Texture2D objectTexture;
    private void Start()
    {
        objectTexture = GetComponent<Texture2D>();
    }

    public void AddPaint(Vector2 position, Texture2D texture, Color color)  // Straight outta gpt, probably doesn't work.
    {
        // Sample grayscale texture at UV position
        float alpha = texture.GetPixelBilinear(position.x, position.y).grayscale;

        // Create a paint texture with the desired color and alpha
        Texture2D paintTexture = new Texture2D(objectTexture.width, objectTexture.height);
        Color[] paintPixels = paintTexture.GetPixels();

        for (int i = 0; i < paintPixels.Length; i++)
        {
            paintPixels[i] = new Color(color.r, color.g, color.b, alpha);
        }

        paintTexture.SetPixels(paintPixels);
        paintTexture.Apply();

        // Blend paint texture with object's texture
        // (This depends on how you want to blend the textures)

        // Apply the modified texture to the object's material
        GetComponent<Renderer>().material.mainTexture = paintTexture;
    }
    
}
