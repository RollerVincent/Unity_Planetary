using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FixNormal : MonoBehaviour
{

    public bool fix;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(fix){
            Fix();
            fix=false;
        }
    }

     public void Fix(){
        
        Vector3 center = transform.position;
        int children = transform.childCount;
        for (int i = 0; i < children; ++i){
            GameObject child = transform.GetChild(i).gameObject;
            MeshFilter mf = child.GetComponent<MeshFilter>();
            Mesh mesh = mf.sharedMesh;
            Vector3[] vertices = mesh.vertices;
            Vector3[] normals = mesh.normals;
            for (int j = 0; j < vertices.Length; ++j){

                Vector3 worldPos = transform.InverseTransformPoint(vertices[j]);
                Vector3 worldDir = (worldPos-center).normalized;

                Vector3 right = Vector3.Cross(Vector3.right, worldDir);
                Vector3 up = Vector3.Cross(right, worldDir);



                RaycastHit hitr;
                RaycastHit hitu;

                if (Physics.Raycast(worldPos + right*0.1f + worldDir * 5, -worldDir, out hitr, 10))
                {
                    if (Physics.Raycast(worldPos + up*0.1f + worldDir * 5, -worldDir, out hitu, 10))
                    {
                        
                        Vector3 norm = -Vector3.Cross((hitr.point-worldPos).normalized,  (hitu.point-worldPos).normalized);
                    
                        normals[j] = transform.TransformPoint(norm);
                    
                    }
                

                
                }


            }
            mesh.normals = normals;
            mf.sharedMesh = mesh;
        }

        Debug.Log("Fixed");
            
    }
}
