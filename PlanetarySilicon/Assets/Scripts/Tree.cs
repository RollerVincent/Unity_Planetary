using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Tree : MonoBehaviour
{


    public float base_width = 10f;
    public float tip_width = 1f;
    public float trunk_height = 20f;
    public float crown_size = 10f;
    public float crown_scale = 2f;
    public float wave_scale = 3f;
    public float wave_effect = 3f;
    public float dir_change = 0.1f;
    public float splitting_prob = 0.01f;
    public float repulsion = 1f;
    public float upgrowing = 1f;
    public int maxBranches = 10;
    
    public bool generate = false;
    public Material leaf_material;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnDrawGizmosSelected()
    {
           /* Marching marching = GetComponent<Marching>();

            for (int h = 0; h < marching.width+1; ++h){
                for (int x = 0; x < marching.width+1; ++x){
                    for (int y = 0; y < marching.width+1; ++y){

                        if(marching.resourcesMap[h,x,y] == 1){
                            Gizmos.color = Color.yellow;
                            Gizmos.DrawSphere(transform.position+new Vector3(h,x,y), 0.1f);
                        }else if (marching.resourcesMap[h,x,y] == 2){
                            Gizmos.color = Color.red;
                            Gizmos.DrawSphere(transform.position+new Vector3(h,x,y), 0.1f);
                        }else{
                            
                        }
                        
                        

                    }
                

                }

            }*/
    }

    // Update is called once per frame
    void Update()
    {
        if(generate){
            Generate();
            Recolor();
            Recolor_Leaves();
            






            generate = false;
        }
    }

    public void Generate2(){
        
        Marching marching = GetComponent<Marching>();
        marching.Generate();
        float mid = (marching.width-1)/2;

        for (int h = 0; h < marching.width+1; ++h){
            for (int x = 0; x < marching.width+1; ++x){
                for (int y = 0; y < marching.width+1; ++y){
                    marching.terrainMap[h,x,y] = 1;
                }
            

            }

        }

        for (int h = 1; h < marching.width; ++h){
            float radius = Mathf.Max(0, base_width - (base_width-tip_width)*(h/trunk_height));
            for (int x = 1; x < marching.width; ++x){
                for (int y = 1; y < marching.width; ++y){
                    float dist = Mathf.Sqrt((x-mid)*(x-mid) + (y-mid)*(y-mid));
                    if(dist<radius){
                        marching.terrainMap[h,x,y] = Mathf.Min(1, dist/radius)-Random.Range(0f, 0.1f);
                        //place_resource(marching, h, x, y, 2);
                        
                        
                    }
                }
            }

        }

        create_bubble(mid,mid,trunk_height,crown_size, crown_scale, 1);
        create_bubble(mid,mid,0,2, 1, 2);


        marching.Refresh();


    }

    void place_resource(Marching marching, int x, int y, int z, int res){
        int ii = -1;
        if(res == 2){
            ii=0;
        }
        for (int i = ii; i < 2; ++i){
            for (int j = -1; j < 2; ++j){
                for (int k = -1; k < 2; ++k){
                    marching.resourcesMap[x+i,y+j,z+k]=Mathf.Max(marching.resourcesMap[x+i,y+j,z+k], res);
                }
            }
        }
    }

    public void Recolor(){
        Marching marching = GetComponent<Marching>();
        MeshFilter mf = GetComponent<MeshFilter>();
        Mesh mesh = mf.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        Vector2[] uv = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; ++i){
                
            uv[i] = new Vector2(Random.Range(0.0f,1.0f), Random.Range(0.0f,1.0f));
                
        }
        mesh.uv = uv;
        mf.sharedMesh = mesh;
    }

    public void Recolor_Leaves(){
        Marching marching = transform.GetChild(0).gameObject.GetComponent<Marching>();
        MeshFilter mf = transform.GetChild(0).gameObject.GetComponent<MeshFilter>();
        Mesh mesh = mf.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        Vector2[] uv = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; ++i){
            Vector3 v = vertices[i];
            
                
            uv[i] = new Vector2(v.x, v.y);
                
        }
        mesh.uv = uv;
        mf.sharedMesh = mesh;
    }

    void create_bubble(float cx, float cy, float cz, float r, float scale, int resource){
        Marching marching = transform.GetChild(0).gameObject.GetComponent<Marching>();
        
        for (int h = 1; h < marching.width; ++h){

            
            float radius = r;// + Mathf.Sin(h*3.14f/wave_scale)*wave_effect;

            for (int x = 1; x < marching.width; ++x){
                for (int y = 1; y < marching.width; ++y){

                   
                    float dist = Mathf.Sqrt((x-cx)*(x-cx)*scale + (y-cy)*(y-cy)*scale + (h-cz)*(h-cz));


                    if(dist<radius){
                        
                        //marching.resourcesMap[h,x,y] = Mathf.Max(marching.resourcesMap[h,x,y], resource);
                        //place_resource(marching, h, x, y, resource);
                        
                        /*marching.resourcesMap[h-1,x-1,y-1]=Mathf.Max(marching.resourcesMap[h-1,x-1,y-1], resource);
                        marching.resourcesMap[h-1,x-1,y-0]=Mathf.Max(marching.resourcesMap[h-1,x-1,y-0], resource);
                        marching.resourcesMap[h-1,x-1,y+1]=Mathf.Max(marching.resourcesMap[h-1,x-1,y+1], resource);

                        marching.resourcesMap[h-1,x-0,y-1]=Mathf.Max(marching.resourcesMap[h-1,x-0,y-1], resource);
                        marching.resourcesMap[h-1,x-0,y-0]=Mathf.Max(marching.resourcesMap[h-1,x-0,y-0], resource);
                        marching.resourcesMap[h-1,x-0,y+1]=Mathf.Max(marching.resourcesMap[h-1,x-0,y+1], resource);

                        marching.resourcesMap[h-1,x+1,y-1]=Mathf.Max(marching.resourcesMap[h-1,x+1,y-1], resource);
                        marching.resourcesMap[h-1,x+1,y-0]=Mathf.Max(marching.resourcesMap[h-1,x+1,y-0], resource);
                        marching.resourcesMap[h-1,x+1,y+1]=Mathf.Max(marching.resourcesMap[h-1,x+1,y+1], resource);


                        marching.resourcesMap[h-0,x-1,y-1]=Mathf.Max(marching.resourcesMap[h-0,x-1,y-1], resource);
                        marching.resourcesMap[h-0,x-1,y-0]=Mathf.Max(marching.resourcesMap[h-0,x-1,y-0], resource);
                        marching.resourcesMap[h-0,x-1,y+1]=Mathf.Max(marching.resourcesMap[h-0,x-1,y+1], resource);

                        marching.resourcesMap[h-0,x-0,y-1]=Mathf.Max(marching.resourcesMap[h-0,x-0,y-1], resource);
                        marching.resourcesMap[h-0,x-0,y-0]=Mathf.Max(marching.resourcesMap[h-0,x-0,y-0], resource);
                        marching.resourcesMap[h-0,x-0,y+1]=Mathf.Max(marching.resourcesMap[h-0,x-0,y+1], resource);

                        marching.resourcesMap[h-0,x+1,y-1]=Mathf.Max(marching.resourcesMap[h-0,x+1,y-1], resource);
                        marching.resourcesMap[h-0,x+1,y-0]=Mathf.Max(marching.resourcesMap[h-0,x+1,y-0], resource);
                        marching.resourcesMap[h-0,x+1,y+1]=Mathf.Max(marching.resourcesMap[h-0,x+1,y+1], resource);


                        marching.resourcesMap[h+1,x-1,y-1]=Mathf.Max(marching.resourcesMap[h+1,x-1,y-1], resource);
                        marching.resourcesMap[h+1,x-1,y-0]=Mathf.Max(marching.resourcesMap[h+1,x-1,y-0], resource);
                        marching.resourcesMap[h+1,x-1,y+1]=Mathf.Max(marching.resourcesMap[h+1,x-1,y+1], resource);

                        marching.resourcesMap[h+1,x-0,y-1]=Mathf.Max(marching.resourcesMap[h+1,x-0,y-1], resource);
                        marching.resourcesMap[h+1,x-0,y-0]=Mathf.Max(marching.resourcesMap[h+1,x-0,y-0], resource);
                        marching.resourcesMap[h+1,x-0,y+1]=Mathf.Max(marching.resourcesMap[h+1,x-0,y+1], resource);

                        marching.resourcesMap[h+1,x+1,y-1]=Mathf.Max(marching.resourcesMap[h+1,x+1,y-1], resource);
                        marching.resourcesMap[h+1,x+1,y-0]=Mathf.Max(marching.resourcesMap[h+1,x+1,y-0], resource);
                        marching.resourcesMap[h+1,x+1,y+1]=Mathf.Max(marching.resourcesMap[h+1,x+1,y+1], resource);*/
                        
                        
                        
                        marching.terrainMap[h,x,y]=dist-radius+Random.Range(0.0f,1f);
                        //marching.terrainMap[h,x,y]=0;

                    }else if(dist<radius+1){
                        marching.terrainMap[h,x,y] = Mathf.Min(marching.terrainMap[h,x,y], dist-radius+Random.Range(0.0f,1f));
                        //place_resource(marching, h, x, y, resource);
                    }
                    
                    
                    
                    
                    


                }
            

            }

        }
    }

    public void create_child(){
        GameObject g = new GameObject("Leaves");
        g.transform.parent = transform;
                            
        Marching marching = g.AddComponent<Marching>();
        MeshFilter mfilter = g.AddComponent<MeshFilter>();
        MeshCollider mc = g.AddComponent<MeshCollider>();
        MeshRenderer mr = g.AddComponent<MeshRenderer>();
        marching.terrainSurface = 0.5f;
        marching.width = GetComponent<Marching>().width;
        marching.height = marching.width;
        mr.sharedMaterial = leaf_material;

        g.transform.position = transform.position;
        g.transform.rotation = transform.rotation;

    }


    public void Generate(){
        if(transform.childCount == 0){
            create_child();
        }
        Marching marching = GetComponent<Marching>();
        Marching marching_leaves = transform.GetChild(0).gameObject.GetComponent<Marching>();
        marching.Generate();
        marching_leaves.Generate();

        float mid = (marching.width-1)/2;

        for (int h = 0; h < marching.width+1; ++h){
            for (int x = 0; x < marching.width+1; ++x){
                for (int y = 0; y < marching.width+1; ++y){
                    marching.terrainMap[h,x,y] = 1;
                    marching_leaves.terrainMap[h,x,y] = 1;
                }
            

            }

        }

        List<float[]> balls = new List<float[]>();
        balls.Add(new float[]{0,mid,mid,base_width,1,0,0});


        for (int step = 0; step < trunk_height; ++step){
            List<float[]> tmp = new List<float[]>();
            int l = balls.Count;
            for (int h = 1; h < marching.width; ++h){
                float radius = Mathf.Max(0, base_width - (base_width-tip_width)*(h/trunk_height));
                for (int x = 1; x < marching.width; ++x){
                    for (int y = 1; y < marching.width; ++y){


                        for (int i = 0; i < l; ++i){
                            float[] ball = balls[i];
                            radius = ball[3];
                            float dist = Mathf.Sqrt((h-ball[0])*(h-ball[0]) + (x-ball[1])*(x-ball[1]) + (y-ball[2])*(y-ball[2]));
                            if(dist<=radius-0.0f){
                                marching.terrainMap[h,x,y] = dist-radius;//Mathf.Min(marching.terrainMap[h,x,y], dist/(radius*2));
                                //marching.terrainMap[h,x,y] = 0;//Mathf.Min(marching.terrainMap[h,x,y], dist/(radius*2));
                                //place_resource(marching, h, x, y, 1);
                                
                            }else{
                                marching.terrainMap[h,x,y] = Mathf.Min(marching.terrainMap[h,x,y], dist-radius);

                                
                            }/*else if (dist<radius+0.5f){
                                marching.terrainMap[h,x,y] = Mathf.Min(marching.terrainMap[h,x,y], dist-radius);
                                place_resource(marching, h, x, y, 1);


                            }else{
                                marching.terrainMap[h,x,y] = Mathf.Min(marching.terrainMap[h,x,y], dist-radius);
                                
                            }*/


                            
                        }

                        
                    }


                    
                }

            }
            for (int i = 0; i < l; ++i){
                float[] ball = balls[i];

                //Vector3 dir = new Vector3(ball[4]+Random.Range(-1,8), ball[5]+Random.Range(-1,2), ball[6]+Random.Range(-1,2));
                Vector3 dir = new Vector3(ball[4]+Random.Range(-dir_change,dir_change)+upgrowing, ball[5]+Random.Range(-dir_change,dir_change), ball[6]+Random.Range(-dir_change,dir_change)).normalized;
                
                Vector3 force = new Vector3(0,0,0);
                for (int j = 0; j < l; ++j){
                    Vector3 diff = new Vector3(ball[0], ball[1], ball[2]) - new Vector3(balls[j][0], balls[j][1], balls[j][2]);
                    float strength = 1/(1+diff.magnitude);
                    force += diff.normalized*strength;
                }
                force = force / l;
                
                dir += force * repulsion;
                
                dir = dir.normalized;
                
                
                //Vector3 dir = new Vector3(ball[4], ball[5], ball[6]);
                /*if (dir.x < -1 || dir.x > 1){
                    if(dir.x<0){
                        dir.x = -1;
                    }else{
                        dir.x = 1;
                    }
                }
                if (dir.y < -1 || dir.y > 1){
                    if(dir.y<0){
                        dir.y = -1;
                    }else{
                        dir.y = 1;
                    }
                }
                if (dir.z < -1 || dir.z > 1){
                    if(dir.z<0){
                        dir.z = -1;
                    }else{
                        dir.z = 1;
                    }
                }*/


                ball = new float[]{ball[0]+(dir.x), ball[1]+(dir.y), ball[2]+(dir.z), ball[3] - crown_scale, dir.x, dir.y, dir.z};

                /*if(ball[3]>0){
                    tmp.Add(ball);
                }*/
                if(ball[3]<tip_width){
                    ball[3] = tip_width;
                }
                
                tmp.Add(ball);
                
                

                if(Random.Range(0.0f,1.0f)<=splitting_prob*(step) && balls.Count < maxBranches){

                    dir = new Vector3(ball[4], ball[5], ball[6]);
                    dir = (Vector3.Cross(dir, new Vector3(Random.Range(-dir_change,dir_change),Random.Range(-dir_change,dir_change),Random.Range(-dir_change,dir_change)))+new Vector3(0,0,0)).normalized;
                    //dir = Quaternion.AngleAxis(Random.Range(-180f,180f), dir) * dir;
                    //dir = new Vector3(0, ball[1] - mid, ball[2] - mid).normalized;
                    tmp.Add(new float[]{ball[0]+dir.x, ball[1]+dir.y, ball[2]+dir.z, ball[3], dir.x, dir.y, dir.z});
                    //Debug.Log("Added");
                }
                
            }
            balls = tmp;
        }

        int addBubbles = 10;

        for (int i = 0; i < balls.Count; ++i){
                float[] ball = balls[i];
                float cr = ball[3]*15*crown_size*0.9f;

                create_bubble(ball[1]+ball[5]*cr, ball[2]+ball[6]*cr, ball[0]+ball[4]*cr, ball[3]*15*crown_size, 1f, 2);
                for (int j = 0; j < addBubbles; ++j){
                    Vector3 dir = new Vector3(ball[4], ball[5], ball[6]);
                    dir = (Vector3.Cross(dir, new Vector3(Random.Range(-1f,1f),Random.Range(-1f,1f),Random.Range(-1f,1f)))).normalized;
                    create_bubble(ball[1]+dir.y*cr+ball[5]*cr, ball[2]+dir.z*cr+ball[6]*cr, ball[0]+dir.x*cr+ball[4]*cr, ball[3]*15*crown_size/1f, 1f, 2);
                }

                
        }


        marching.Refresh();
        marching_leaves.Refresh();


    }
}
