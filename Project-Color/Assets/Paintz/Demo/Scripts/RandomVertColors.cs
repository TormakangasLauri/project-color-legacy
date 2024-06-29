using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomVertColors : MonoBehaviour
{

	void Start ()
    {
        Mesh mesh = this.GetComponent<MeshFilter>().mesh;

        Color[] colors = new Color[mesh.vertexCount];

        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = new Color(Random.value, Random.value, Random.value, 1.0f);
        }

        mesh.colors = colors;
	}

}
