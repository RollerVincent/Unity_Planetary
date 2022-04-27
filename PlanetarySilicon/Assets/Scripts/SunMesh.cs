using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SunMesh : MonoBehaviour
{

    public int rayNum;
    public bool generate;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(generate){
            Generate();
            generate=false;
        }
    }

    void Generate(){

        MeshFilter mf = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mesh.name = "SunMesh";
        Vector3[] vertices = new Vector3[rayNum*3];
        Vector2[] uv = new Vector2[rayNum*3];
        int[] triangles = new int[rayNum*3];

        for(int i=0; i<rayNum; i++){
            Vector3 dir = new Vector3(Random.Range(-1.0f,1.0f), Random.Range(-1.0f,1.0f), Random.Range(-1.0f,1.0f)).normalized;
            
            vertices[i*3+0] = dir/2;
            vertices[i*3+1] = dir/2;
            vertices[i*3+2] = dir/2;

            uv[i*3+0] = new Vector2(0,0);
            uv[i*3+1] = new Vector2(1,0);
            uv[i*3+2] = new Vector2(0.5f,1);

            triangles[i*3+0] = i*3+0;
            triangles[i*3+1] = i*3+1;
            triangles[i*3+2] = i*3+2;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        mf.sharedMesh = mesh;

    }
}
