using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RecenterMesh : MonoBehaviour
{

    public GameObject targetObject;
    public GameObject centerObject;
    public bool recenter;
    public bool recenterAll;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(recenter){
            Recenter();
            recenter = false;
        }
        if(recenterAll){
            RecenterAll();
            recenterAll = false;
        }
    }
    public void Recenter(){
        
        MeshFilter mf = targetObject.GetComponent<MeshFilter>();
        Mesh tmpmesh = mf.sharedMesh;
        Mesh mesh = new Mesh();
        mesh.vertices = tmpmesh.vertices;
        mesh.triangles = tmpmesh.triangles;
        mesh.uv = tmpmesh.uv;
        mesh.normals = tmpmesh.normals;
        
        Vector4[] tangents = new Vector4[mesh.vertices.Length];


        Vector3 localCenterPos = targetObject.transform.InverseTransformPoint(centerObject.transform.position);


        for(int i=0; i<tangents.Length; i++){
            tangents[i] = new Vector4(localCenterPos.x, localCenterPos.y, localCenterPos.z, 1);
        }


        mesh.tangents = tangents;
        
        mf.sharedMesh = mesh;



    }

    public void RecenterAll(){
        
        centerObject = gameObject;
        
        List<GameObject> remaining = new List<GameObject>();
        remaining.Add(gameObject);

        int c = 0;

        while(remaining.Count > 0){
            List<GameObject> tmp = new List<GameObject>();


            foreach(GameObject target in remaining){
                MeshFilter mf = target.GetComponent<MeshFilter>();
                if(mf != null){
                    targetObject = target;
                    Recenter();
                    c += 1;
                }
                for(int i=0;i<target.transform.childCount;i++){
                    tmp.Add(target.transform.GetChild(i).gameObject);
                }
            }


            remaining = tmp;
        }

        Debug.Log("Recentered " + c + " meshes");
    }
}
