using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlaneSphere : MonoBehaviour
{

    public int planesPerFace = 2;
    public int planeResolution = 10;
    public Material material;

    public bool generate = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(generate){

            createSphere();

            generate = false;
        }
    }

    public void createSphere(){

        foreach (Transform child in transform) {
            if(child.gameObject.tag != "Planet"){
                GameObject.DestroyImmediate(child.gameObject);
            }
            
        }
        
        createFace(Vector3.right, Vector3.up);
        createFace(-Vector3.right, Vector3.up);

        createFace(Vector3.right, Vector3.forward);
        createFace(Vector3.right, -Vector3.forward);

        createFace(Vector3.forward, Vector3.up);
        createFace(Vector3.forward, -Vector3.up);


        
    }

    void createFace(Vector3 right, Vector3 up){
        
        Vector3 forward = Vector3.Cross(right, up);

        float step = 1f/planesPerFace;

        for(int x=0; x<planesPerFace; x++){
            for(int y=0; y<planesPerFace; y++){
            
                Vector3 position = forward*0.5f + up*x*step + right*y*step - up*0.5f - right*0.5f;

                createPlane(position, right, up);



            }
        }
    }


    void createPlane(Vector3 position, Vector3 right, Vector3 up){

        GameObject plane = new GameObject("Plane");
        plane.transform.parent = transform;
        plane.transform.position = transform.position;
        MeshFilter mf = plane.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        MeshRenderer mr = plane.AddComponent<MeshRenderer>();
        mr.sharedMaterial = material;

        
        float step = 1f / (planesPerFace*planeResolution);

        Vector3[] vertices = new Vector3[(planeResolution+1)*(planeResolution+1)];
        Vector3[] normals = new Vector3[(planeResolution+1)*(planeResolution+1)];
        List<int> triangles = new List<int>();


        int i=0;
        for(int x=0; x<planeResolution+1; x++){
            for(int y=0; y<planeResolution+1; y++){
            
                Vector3 vertex = position + right * x * step + up * y * step;
                vertices[i] = vertex.normalized;
                normals[i] = vertices[i];

                if(x>0 && y>0){
                    triangles.Add(i);
                    triangles.Add(i - (planeResolution+1) + 0);
                    triangles.Add(i - (planeResolution+1) - 1);

                    triangles.Add(i);
                    triangles.Add(i - (planeResolution+1) - 1);
                    triangles.Add(i - 1);

                
                }






                i += 1;
            }


        }

        mesh.vertices = vertices;
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals;

        mf.sharedMesh = mesh;




    }

}
