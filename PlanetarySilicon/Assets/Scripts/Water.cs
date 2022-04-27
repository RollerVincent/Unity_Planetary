using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Water : MonoBehaviour
{

    public bool updateDepth;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(updateDepth){
            UpdateDepth();
            updateDepth = false;
        }
    }

    void UpdateDepth(){

        int childCount = transform.GetChild(0).childCount;
        for(int i=0;i<childCount;i++){
            GameObject child = transform.GetChild(0).GetChild(i).gameObject;
            MeshFilter mf = child.GetComponent<MeshFilter>();
            Mesh mesh = mf.sharedMesh;
            Vector3[] vertices = mesh.vertices;
            Vector3[] normals = mesh.normals;
            Vector2[] uv = new Vector2[vertices.Length];

            for(int j=0;j<vertices.Length;j++){

                RaycastHit hit;
                if (Physics.Raycast(child.transform.TransformPoint(vertices[j]), -(child.transform.TransformPoint(vertices[j])).normalized, out hit, 100f))
                {
                    Vector3 diff = child.transform.TransformPoint(vertices[j]) - hit.point;
                    float depth = diff.magnitude;
                    uv[j] = new Vector2(0.5f, depth);

                }

            }


            mesh.uv = uv;
            mf.sharedMesh = mesh;

        }


    }
}
