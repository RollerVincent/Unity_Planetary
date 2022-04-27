using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class StationConnector : MonoBehaviour
{

    public Vector3 direction;
    public float distance;
    
    public bool fit;

    public List<Vector3> worldSpacevertices = new List<Vector3>();
    public List<int> verticeIndices = new List<int>();
    public Vector3 WorldSpaceCenter = Vector3.zero;
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 1f);

        
        for(int i=0; i<this.worldSpacevertices.Count; i++){

            

           
            
            Gizmos.DrawSphere(worldSpacevertices[i], 0.1f);
        }
                        
        
        Gizmos.color = new Color(1.0f, 1.0f, 0.0f, 1f);
        
        Gizmos.DrawSphere(WorldSpaceCenter, 0.2f);


       
    }
    
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Fit(){
            Matrix4x4 localToWorld = transform.localToWorldMatrix;

            this.worldSpacevertices = new List<Vector3>();
            verticeIndices = new List<int>();


            MeshFilter mf = GetComponent<MeshFilter>();
            Mesh mesh = mf.sharedMesh;
            //Matrix4x4 localToWorld = transform.parent.localToWorldMatrix;

            //Vector3 worldDirection = localToWorld.MultiplyPoint3x4(direction).normalized;
            WorldSpaceCenter = Vector3.zero;
            for(int i=0; i<mesh.vertices.Length; i++){
                float d = 1000 - ((direction * 1000) - mesh.vertices[i]).magnitude;
                //Debug.Log(d);
                if(d>distance){
                    this.worldSpacevertices.Add(localToWorld.MultiplyPoint3x4(mesh.vertices[i]));
                    verticeIndices.Add(i);
                    WorldSpaceCenter += mesh.vertices[i];
                }

            }

            WorldSpaceCenter = WorldSpaceCenter/worldSpacevertices.Count;
            
            WorldSpaceCenter = localToWorld.MultiplyPoint3x4(WorldSpaceCenter);
    }

    // Update is called once per frame
    void Update()
    {
        if(fit){

            Fit();


            fit = false;
        }
    }
}
