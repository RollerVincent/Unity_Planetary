using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UvMapper : MonoBehaviour
{
    public float offsetRadius = 0;
    public bool update = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(update){

            Recolor();

            update = false;
        }
    }
    public void RecolorDividable(Dividable dividable){
        MeshFilter mf = dividable.obj.GetComponent<MeshFilter>();
        Mesh mesh = mf.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        Vector2[] uv = new Vector2[vertices.Length];
        Vector3 center = transform.position;
        for (int j = 0; j < vertices.Length; ++j){

                Vector3 worldPosition = dividable.obj.transform.position + vertices[j] - center;
                float mag = worldPosition.magnitude - offsetRadius;


                Vector3 worldNormal = worldPosition/mag;
                float angle = Vector3.Angle(normals[j], worldNormal)/90f;


                uv[j] = new Vector2(mag, angle);


        }
        mesh.uv = uv;
        mf.sharedMesh = mesh;

        int c = 0;
        foreach(SurfaceData data in dividable.datalist){
            data.uvs = new Vector2[data.vertices.Count];
            for (int j = 0; j < data.vertices.Count; ++j){
                data.uvs[j] = uv[c];
                c += 1;
            }

        }
    }

    public Vector2[] RecolorObject(GameObject child){
        MeshFilter mf = child.GetComponent<MeshFilter>();
        Mesh mesh = mf.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        Vector2[] uv = new Vector2[vertices.Length];
        Vector3 center = transform.position;
        for (int j = 0; j < vertices.Length; ++j){

                Vector3 worldPosition = child.transform.position + vertices[j] - center;
                float mag = worldPosition.magnitude - offsetRadius;


                Vector3 worldNormal = worldPosition/mag;
                float angle = Vector3.Angle(normals[j], worldNormal)/90f;


                uv[j] = new Vector2(mag, angle);


        }
        mesh.uv = uv;
        mf.sharedMesh = mesh;
        return uv;
    }

    public void Recolor(){
        //Planet planet = GetComponent<Planet>();
        //Vector3 center = new Vector3(planet.ChunksPerFace/2*planet.ChunkSize, planet.ChunksPerFace/2*planet.ChunkSize, planet.ChunksPerFace/2*planet.ChunkSize);
        Vector3 center = transform.position;
        int children = transform.childCount;
        for (int i = 0; i < children; ++i){
            GameObject child = transform.GetChild(i).gameObject;
            MeshFilter mf = child.GetComponent<MeshFilter>();
            Mesh mesh = mf.sharedMesh;
            Vector3[] vertices = mesh.vertices;
            Vector3[] normals = mesh.normals;
            Vector2[] uv = new Vector2[vertices.Length];
            for (int j = 0; j < vertices.Length; ++j){

                Vector3 worldPosition = child.transform.position + vertices[j] - center;
                float mag = worldPosition.magnitude - offsetRadius;


                Vector3 worldNormal = worldPosition/mag;
                float angle = Vector3.Angle(normals[j], worldNormal)/90f;







                uv[j] = new Vector2(mag, angle);


            }
            mesh.uv = uv;
            mf.sharedMesh = mesh;
        }

        Debug.Log("Recolored");
            
    }
}
