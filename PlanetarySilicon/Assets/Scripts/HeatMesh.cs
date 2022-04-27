using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class HeatMesh : MonoBehaviour
{

    public int samples;
    public int range;
    public bool generate;
    public bool updateVelocity;
    public bool debug;

    public Vector3 velocity;
    public float mask;

    
    Mesh mesh;
    List<Vector3> tmpVertices = new List<Vector3>();
    List<Vector3> tmpNormals = new List<Vector3>();

    void OnDrawGizmosSelected()
    {
            

        Gizmos.color = new Color(1.0f, 1.0f, 0.0f, 1f);
        

        if(debug){

            foreach(Vector3 v in tmpVertices){
                Gizmos.DrawSphere(transform.TransformPoint(v), 0.03f);
            }

        }
            

       
    }





    // Start is called before the first frame update
    void Start()
    {
        generate = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(generate){
            generateMesh();
            UpdateVelocity();
            generate = false;
        }

        if(updateVelocity){
            UpdateVelocity();
            updateVelocity = false;
            
        }

        if(!debug){
            mask = 0;
        }else{
            mask = 1;
        }
    }

    void generateMesh(){
        mesh = new Mesh();

        tmpVertices = new List<Vector3>();
        tmpNormals = new List<Vector3>();

        for(int i=0;i<samples;i++){
            Vector3 sampleDir = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
            Vector3 samplePos = transform.position - sampleDir*range;

            RaycastHit hit;

            if (Physics.Raycast(samplePos, sampleDir, out hit, range)){
                tmpVertices.Add(transform.InverseTransformPoint(hit.point));
                tmpNormals.Add(transform.InverseTransformDirection(hit.normal).normalized);
            }



        }

        Vector3[] vertices = new Vector3[tmpVertices.Count * 4];
        Vector3[] normals = new Vector3[tmpVertices.Count * 4];
        Vector2[] uv = new Vector2[tmpVertices.Count * 4];

        int[] triangles = new int[tmpVertices.Count * 6];


        for(int i=0;i<tmpVertices.Count;i++){

            vertices[i*4+0] = tmpVertices[i];
            vertices[i*4+1] = tmpVertices[i];
            vertices[i*4+2] = tmpVertices[i];
            vertices[i*4+3] = tmpVertices[i];

            normals[i*4+0] = tmpNormals[i];
            normals[i*4+1] = tmpNormals[i];
            normals[i*4+2] = tmpNormals[i];
            normals[i*4+3] = tmpNormals[i];

            uv[i*4+0] = new Vector2(0.0f,0.0f);
            uv[i*4+1] = new Vector2(1.0f,0.0f);
            uv[i*4+2] = new Vector2(0.0f,1.0f);
            uv[i*4+3] = new Vector2(1.0f,1.0f);

            triangles[i*6+0] = i*4+0;
            triangles[i*6+1] = i*4+1;
            triangles[i*6+2] = i*4+2;

            triangles[i*6+3] = i*4+1;
            triangles[i*6+4] = i*4+2;
            triangles[i*6+5] = i*4+3;

        }

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uv;
        mesh.triangles = triangles;
        
        MeshFilter mf = gameObject.GetComponent<MeshFilter>();
        if(mf==null){
            mf = gameObject.AddComponent<MeshFilter>();
            gameObject.AddComponent<MeshRenderer>();
        }
        mf.sharedMesh = mesh;

   
    }

    public void UpdateVelocity(){

        /*if(mesh==null){
            mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
        }


        Vector4[] tangents = new Vector4[mesh.vertices.Length];
        Vector4 t = new Vector4(velocity.x, velocity.y, velocity.z, mask);
        for(int i=0;i<mesh.vertices.Length;i++){
            tangents[i] = t;

        }
        mesh.tangents = tangents;
        MeshFilter mf = gameObject.GetComponent<MeshFilter>();
        mf.sharedMesh = mesh;*/
        GetComponent<MeshRenderer>().sharedMaterial.SetVector("_Velocity", velocity);
        GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_Mask", mask);
    }

}
