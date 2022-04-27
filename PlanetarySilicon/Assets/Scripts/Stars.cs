using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[ExecuteInEditMode]
public class Stars : MonoBehaviour
{
    public Texture2D starDensities;
    public int count;
    public float spread;
    public float thicknessSpread;
    public float thicknessExponent;
    public bool inverseThickness;
    public int seed;
    public bool debug;
    public bool refresh;
    public bool generate;

    public List<Color> targetColors = new List<Color>();
    public List<Vector3> targetPositions = new List<Vector3>();
    public List<int> additionalStars = new List<int>();
    public float additionalStarProbability = 0.5f;
    bool isCached = false;


    void OnDrawGizmosSelected()
    {
        if(debug){
            Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 1f);


            List<Vector3> pos = GetTargetPositions();
            
            for(int i=0; i<pos.Count; i++){
                Gizmos.DrawSphere(pos[i], Universe.GALAXY_SCALE/1000f);
            }

        }
        
                        
        
        


       
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if(refresh){
            isCached = false;
            refresh = false;
        }


        if(generate){


            GenerateNew();


            generate=false;
        }
    }

    void GenerateNew(){

        List<Vector3> positions = GetTargetPositions();

        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[positions.Count*4];
        Vector2[] uv = new Vector2[positions.Count*4];
        Color[] colors = new Color[positions.Count*4];
        int[] triangles = new int[positions.Count*6];

        

        for(int i=0;i<positions.Count;i++){
            vertices[i*4+0] = positions[i];
            vertices[i*4+1] = positions[i];
            vertices[i*4+2] = positions[i];
            vertices[i*4+3] = positions[i];

            colors[i*4+0] = targetColors[i];
            colors[i*4+1] = targetColors[i];
            colors[i*4+2] = targetColors[i];
            colors[i*4+3] = targetColors[i];
        
            uv[i*4+0] = new Vector2(0,0);
            uv[i*4+1] = new Vector2(0,1);
            uv[i*4+2] = new Vector2(1,1);
            uv[i*4+3] = new Vector2(1,0);

            triangles[i*6+0] = i*4+0;
            triangles[i*6+1] = i*4+1;
            triangles[i*6+2] = i*4+2;

            triangles[i*6+3] = i*4+2;
            triangles[i*6+4] = i*4+3;
            triangles[i*6+5] = i*4+0;
        
        
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.SetColors(colors);
        mesh.RecalculateBounds();

        GetComponent<MeshFilter>().sharedMesh = mesh;



    }

    public List<Vector3> GetTargetPositions(){
        Random.seed = seed;
        if(!isCached){
            isCached = true;
            
            targetColors = new List<Color>();
            Random.seed = seed;
            targetPositions = new List<Vector3>();
            additionalStars = new List<int>();
            
            for(int i=0;i<count;i++){


                Vector2 uv = new Vector2(Random.Range(0, 1.0f), Random.Range(0, 1.0f));
                Color color = starDensities.GetPixel(Mathf.FloorToInt(uv.x*starDensities.width), Mathf.FloorToInt(uv.y*starDensities.height));
                float density = color.grayscale;
                color.a = density;

                

                if(density>Random.Range(0.0f,1.0f)){
                    float z = Random.Range(0f,1.0f);
                    z = Mathf.Pow(z,thicknessExponent) * thicknessSpread;
                    if(Random.Range(0.0f,1.0f) < 0.5f){
                        z = -z;
                    }

                    if(inverseThickness){
                        density = 1-density;
                    }

                    targetColors.Add(color);
                    targetPositions.Add(new Vector3((uv.x-0.5f)*spread*2,(uv.y-0.5f)*spread*2,z*density)*Universe.GALAXY_SCALE);

                    float p = Random.Range(0.0f, 1.0f);
                    if(p<additionalStarProbability){
                        additionalStars.Add(0);
                    }else{
                        additionalStars.Add(-1);
                    }
                }



                //targetPositions[i] = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f))*spread*Random.Range(0.0f, 1.0f);

            }
        }

        int c = targetPositions.Count;
        for(int i=0;i<c;i++){
            if(additionalStars[i] != -1){
                additionalStars[i] = Random.Range(0,c);
            }
        }


        return targetPositions;
    }

   
}
