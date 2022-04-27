using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ResourceColoring : MonoBehaviour
{

    ResourceManager resourceManager;
    Resource[] resourceList;


    // Start is called before the first frame update
    void Start()
    {
        resourceManager = transform.parent.GetComponentInChildren<ResourceManager>();
        resourceList = GameObject.Find("Resources").GetComponentsInChildren<Resource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RecolorData(SurfaceData data){

            List<Vector3> vertices = data.vertices;
            Color[] colors = new Color[vertices.Count];

            for (int j = 0; j < vertices.Count; ++j){

                int res = resourceManager.sampleObjectPos(vertices[j]);

                Resource resource = resourceList[res];

                colors[j] = resource.color;
                if (res == 0){
                    colors[j].a = 0;
                }


            }
            data.colors = colors;
        
    }

    public void Recolor(GameObject child){
        
            MeshFilter mf = child.GetComponent<MeshFilter>();
            Mesh mesh = mf.sharedMesh;
            Vector3[] vertices = mesh.vertices;
            Color[] colors = new Color[vertices.Length];

            for (int j = 0; j < vertices.Length; ++j){

                int res = resourceManager.sampleObjectPos(vertices[j]);

                Resource resource = resourceList[res];

                colors[j] = resource.color;
                if (res == 0){
                    colors[j].a = 0;
                }


            }
            mesh.colors = colors;
            mf.sharedMesh = mesh;
        
    }
}
