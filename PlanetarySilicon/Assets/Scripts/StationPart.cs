using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class StationPart : MonoBehaviour
{
    public GameObject centerObject;
    public bool placeMode;
    public float maxConnectionDistance = 5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    bool checkConnect(StationConnector connectorA, StationConnector connectorB){

        float distance = (connectorA.transform.GetChild(0).position - connectorB.transform.GetChild(0).position).magnitude;

        MeshFilter mf = connectorA.GetComponent<MeshFilter>();  

        

        if(distance < maxConnectionDistance){
            

            Mesh mesh = new Mesh();
            mesh.name = "Connection";

            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();  
      
           
            
            
            vertices.Add(connectorA.transform.InverseTransformPoint(connectorA.transform.GetChild(0).position));
            vertices.Add(connectorA.transform.InverseTransformPoint(connectorA.transform.GetChild(1).position));
            vertices.Add(connectorA.transform.InverseTransformPoint(connectorA.transform.GetChild(2).position));
            vertices.Add(connectorA.transform.InverseTransformPoint(connectorA.transform.GetChild(3).position));
            vertices.Add(connectorA.transform.InverseTransformPoint(connectorA.transform.GetChild(4).position));
            vertices.Add(connectorA.transform.InverseTransformPoint(connectorA.transform.GetChild(5).position));
            vertices.Add(connectorA.transform.InverseTransformPoint(connectorA.transform.GetChild(6).position));
            vertices.Add(connectorA.transform.InverseTransformPoint(connectorA.transform.GetChild(7).position));
            
            vertices.Add(connectorA.transform.InverseTransformPoint(connectorB.transform.GetChild(2).position));
            vertices.Add(connectorA.transform.InverseTransformPoint(connectorB.transform.GetChild(1).position));
            vertices.Add(connectorA.transform.InverseTransformPoint(connectorB.transform.GetChild(0).position));
            vertices.Add(connectorA.transform.InverseTransformPoint(connectorB.transform.GetChild(7).position));
            vertices.Add(connectorA.transform.InverseTransformPoint(connectorB.transform.GetChild(6).position));
            vertices.Add(connectorA.transform.InverseTransformPoint(connectorB.transform.GetChild(5).position));
            vertices.Add(connectorA.transform.InverseTransformPoint(connectorB.transform.GetChild(4).position));
            vertices.Add(connectorA.transform.InverseTransformPoint(connectorB.transform.GetChild(3).position));
            
            
          

            triangles.Add(0+0);
            triangles.Add(1+0);
            triangles.Add(0+8);

            triangles.Add(1+0);
            triangles.Add(1+8);
            triangles.Add(0+8);


            triangles.Add(1+0);
            triangles.Add(2+0);
            triangles.Add(1+8);

            triangles.Add(2+0);
            triangles.Add(2+8);
            triangles.Add(1+8);


            triangles.Add(2+0);
            triangles.Add(3+0);
            triangles.Add(2+8);

            triangles.Add(3+0);
            triangles.Add(3+8);
            triangles.Add(2+8);


            triangles.Add(3+0);
            triangles.Add(4+0);
            triangles.Add(3+8);

            triangles.Add(4+0);
            triangles.Add(4+8);
            triangles.Add(3+8);


            triangles.Add(4+0);
            triangles.Add(5+0);
            triangles.Add(4+8);

            triangles.Add(5+0);
            triangles.Add(5+8);
            triangles.Add(4+8);


            triangles.Add(5+0);
            triangles.Add(6+0);
            triangles.Add(5+8);

            triangles.Add(6+0);
            triangles.Add(6+8);
            triangles.Add(5+8);


            triangles.Add(6+0);
            triangles.Add(7+0);
            triangles.Add(6+8);

            triangles.Add(7+0);
            triangles.Add(7+8);
            triangles.Add(6+8);


            triangles.Add(7+0);
            triangles.Add(0+0);
            triangles.Add(7+8);

            triangles.Add(0+0);
            triangles.Add(0+8);
            triangles.Add(7+8);








            triangles.Add(1+0);
            triangles.Add(0+0);
            triangles.Add(0+8);

            triangles.Add(1+0);
            triangles.Add(0+8);
            triangles.Add(1+8);


            triangles.Add(2+0);
            triangles.Add(1+0);
            triangles.Add(1+8);

            triangles.Add(2+0);
            triangles.Add(1+8);
            triangles.Add(2+8);


            triangles.Add(3+0);
            triangles.Add(2+0);
            triangles.Add(2+8);

            triangles.Add(3+0);
            triangles.Add(2+8);
            triangles.Add(3+8);


            triangles.Add(4+0);
            triangles.Add(3+0);
            triangles.Add(3+8);

            triangles.Add(4+0);
            triangles.Add(3+8);
            triangles.Add(4+8);


            triangles.Add(5+0);
            triangles.Add(4+0);
            triangles.Add(4+8);

            triangles.Add(5+0);
            triangles.Add(4+8);
            triangles.Add(5+8);


            triangles.Add(6+0);
            triangles.Add(5+0);
            triangles.Add(5+8);

            triangles.Add(6+0);
            triangles.Add(5+8);
            triangles.Add(6+8);


            triangles.Add(7+0);
            triangles.Add(6+0);
            triangles.Add(6+8);

            triangles.Add(7+0);
            triangles.Add(6+8);
            triangles.Add(7+8);


            triangles.Add(0+0);
            triangles.Add(7+0);
            triangles.Add(7+8);

            triangles.Add(0+0);
            triangles.Add(7+8);
            triangles.Add(0+8);



            





           





            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            mf.sharedMesh = mesh;

            //placeMode = false;
            

            return true;

            



        }

        return false;


        


    }

    void newMesh(){
        Mesh childMesh = new Mesh();
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;

        childMesh.vertices = mesh.vertices;
        childMesh.triangles = mesh.triangles;
        childMesh.normals = mesh.normals;

        transform.GetChild(0).gameObject.GetComponent<MeshFilter>().sharedMesh = childMesh;
    }

    bool checkConnect2(StationConnector connectorA, StationConnector connectorB){
        float distance = (connectorA.WorldSpaceCenter - connectorB.WorldSpaceCenter).magnitude;

        MeshFilter mf = connectorA.transform.GetChild(0).GetComponent<MeshFilter>();  

        if(distance < maxConnectionDistance){


            List<Vector3> targetWorldPositions = new List<Vector3>();

            for(int i=0; i<connectorA.worldSpacevertices.Count;i++){

                float min = 100000;
                Vector3 worldTargetPos = connectorA.worldSpacevertices[i];
                for(int j=0; j<connectorB.worldSpacevertices.Count;j++){

                    float d = ((connectorA.worldSpacevertices[i]-connectorA.WorldSpaceCenter) - (connectorB.worldSpacevertices[j]-connectorB.WorldSpaceCenter)).magnitude;


                    if(d<min){
                        min = d;
                        worldTargetPos = connectorB.worldSpacevertices[j];
                    }


                }

                targetWorldPositions.Add(worldTargetPos);





            }

            Vector3[] vertices = mf.sharedMesh.vertices;
            Matrix4x4 worldToLocal = connectorA.transform.worldToLocalMatrix;


            for(int i=0; i<connectorA.worldSpacevertices.Count;i++){

                vertices[connectorA.verticeIndices[i]] = worldToLocal.MultiplyPoint3x4(targetWorldPositions[i]);

            }
            mf.sharedMesh.vertices = vertices;



            return true;
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        /*Vector3 center_dir = (transform.position - centerObject.transform.position).normalized;
        
        Vector3 d_global = Vector3.Cross(center_dir, -transform.forward).normalized;
        float angle = Vector3.Angle(center_dir, -transform.forward)/180f;*/

        //transform.RotateAround(transform.position, d_global, angle);
        
        
        
        if(placeMode){

           /* MeshFilter mf = GetComponent<MeshFilter>();
            Mesh mesh = mf.sharedMesh;
            Matrix4x4 localToWorld = transform.localToWorldMatrix;*/

            newMesh();
            

            foreach(StationConnector connector in GetComponents<StationConnector>()){


                connector.Fit();

                


                foreach(StationConnector other in transform.parent.gameObject.GetComponentsInChildren<StationConnector>()){


                    if(other.transform != transform){

                       
                        if (checkConnect2(connector, other)){
                            break;
                        }


                    }



                }



            }


        }
    }
}
