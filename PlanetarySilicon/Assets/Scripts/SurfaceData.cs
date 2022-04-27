using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceData
{

    public List<Vector3> vertices;
    public int[] triangles;
    public Vector3Int offset;
    public string hash;
    public float[,,] terrainMap = null;
    public Color[] colors = new Color[0];
    public Vector2[] uvs = new Vector2[0];
}
