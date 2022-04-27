using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TreePlacer : MonoBehaviour
{


    public Planet planet;
    public Tree sample_tree;
    public float placing_prob = 0.05f;
    public float max_height = 100f;
    public float max_steep = 1f;
    public float noise_detail = 1f;
    public float noise_cutoff = 1f;
    public Material material;
    public bool place = false;
    public bool debug = false;
    public int seed = 100;


    // Start is called before the first frame update
    void Start()
    {
        
    }
    void OnDrawGizmosSelected()
    {
        Random.seed = seed;
        if(debug){
            int children = planet.transform.childCount;
            int count = 0;
            for (int i = 0; i < children; ++i){
                GameObject child = planet.transform.GetChild(i).gameObject;
                MeshFilter mf = child.GetComponent<MeshFilter>();
                Mesh mesh = mf.sharedMesh;
                Vector3[] vertices = mesh.vertices;
                Vector3[] normals = mesh.normals;
                Vector2[] uv = mesh.uv;
                for (int j = 0; j < vertices.Length; ++j){

                    float r = Random.Range(0.0f, 1.0f);
                    if(r<=placing_prob){

                        if(uv[j].x<max_height && uv[j].y<max_steep){
                            if(Mathf.PerlinNoise((child.transform.position + vertices[j]).x/noise_detail, (child.transform.position + vertices[j]).y/noise_detail) > noise_cutoff){

                                count += 1;
                                Gizmos.color = Color.red;
                                Gizmos.DrawSphere(child.transform.position+vertices[j], 1f);
                            }
                        }

                        
                    }


                }
            }
        }
        



        
    }

    // Update is called once per frame
    void Update()
    {
        if(place){
            Random.seed = seed;
            Place();
            place=false;
        }   
    }

    void Place(){



        
        int children = transform.childCount;
        for (int i = 0; i < children; ++i){
            GameObject child = transform.GetChild(i).gameObject;
            GameObject.DestroyImmediate(child);
        }
        children = planet.transform.childCount;
        Vector3 center = new Vector3(planet.ChunksPerFace/2*planet.ChunkSize, planet.ChunksPerFace/2*planet.ChunkSize, planet.ChunksPerFace/2*planet.ChunkSize);

        int count = 0;
        for (int i = 0; i < children; ++i){
            GameObject child = planet.transform.GetChild(i).gameObject;
            MeshFilter mf = child.GetComponent<MeshFilter>();
            Mesh mesh = mf.sharedMesh;
            Vector3[] vertices = mesh.vertices;
            Vector3[] normals = mesh.normals;
            Vector2[] uv = mesh.uv;
            for (int j = 0; j < vertices.Length; ++j){

                float r = Random.Range(0.0f, 1.0f);
                if(r<=placing_prob){

                    if(uv[j].x<max_height && uv[j].y<max_steep){
                        if(Mathf.PerlinNoise((child.transform.position + vertices[j]).x/noise_detail, (child.transform.position + vertices[j]).y/noise_detail) > noise_cutoff){
                            count += 1;
                            GameObject g = new GameObject("Tree_"+j);
                            g.transform.parent = transform;
                            
                            Marching marching = g.AddComponent<Marching>();
                            MeshFilter mfilter = g.AddComponent<MeshFilter>();
                            MeshCollider mc = g.AddComponent<MeshCollider>();
                            MeshRenderer mr = g.AddComponent<MeshRenderer>();
                            Tree tree = g.AddComponent<Tree>();
                            marching.terrainSurface = 0.5f;
                            marching.width = sample_tree.GetComponent<Marching>().width;
                            marching.height = marching.width;
                            mr.sharedMaterial = material;

                            g.transform.position = child.transform.position + vertices[j] - new Vector3(3 ,marching.width,marching.width)/2;

                            Vector3 diff = child.transform.position + vertices[j] - center;
                            diff = diff.normalized;
                            Vector3 cross = Vector3.Cross(diff, new Vector3(1,0,0));
                            float angle = Vector3.SignedAngle(diff, new Vector3(1,0,0), cross);

                            g.transform.RotateAround(child.transform.position + vertices[j], cross, -angle);


                            tree.base_width = sample_tree.base_width;
                            tree.tip_width = sample_tree.tip_width;
                            tree.trunk_height = sample_tree.trunk_height;
                            tree.crown_size = sample_tree.crown_size;
                            tree.crown_scale = sample_tree.crown_scale;
                            tree.wave_scale = sample_tree.wave_scale;
                            tree.wave_effect = sample_tree.wave_effect;
                            tree.dir_change = sample_tree.dir_change;
                            tree.splitting_prob = sample_tree.splitting_prob;
                            tree.repulsion = sample_tree.repulsion;
                            tree.upgrowing = sample_tree.upgrowing;
                            tree.maxBranches = sample_tree.maxBranches;
                            tree.leaf_material = sample_tree.leaf_material;
                           
                            

                            tree.Generate();
                            tree.Recolor();
                            tree.Recolor_Leaves();
                        }


                    }

                    
                }


            }
        }
        Debug.Log(count);
    }
}
