using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SurfaceManagerSaveObject
{

  public List<float[]> verticesX = new List<float[]>();
  public List<float[]> verticesY = new List<float[]>();
  public List<float[]> verticesZ = new List<float[]>();

  public List<float[]> colorsR = new List<float[]>();
  public List<float[]> colorsG = new List<float[]>();
  public List<float[]> colorsB = new List<float[]>();
  public List<float[]> colorsA = new List<float[]>();
  
  public List<float[]> uvsX = new List<float[]>();
  public List<float[]> uvsY = new List<float[]>();

  
  public List<int[]> triangles = new List<int[]>();

  public int[] offsetX;
  public int[] offsetY;
  public int[] offsetZ;

  public string[] hashes;

}

