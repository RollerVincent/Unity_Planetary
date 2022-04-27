using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SurfaceModifier : MonoBehaviour
{

    Planet planet;

    public float affection = 20f;
    public float detail = 100f;
    public string mode = "refresh";
    public int resourceIndex = 1;
    public bool apply = false;
    public bool isworley = true;

    // Start is called before the first frame update
    void Start()
    {
        planet = GetComponent<Planet>();
    }

    // Update is called once per frame
    void Update()
    {
        if(apply){
            planet.ApplyModifier(this);
            Debug.Log("Applied");
            apply = false;
        }
    }

    public float[] Apply(Vector3 worldPosition, float sqHeight, int currentResource, float currentPivot, float currentTerrain){
        
        //worldPosition = worldPosition + new Vector3(planet.Seed, planet.Seed, planet.Seed);

        float noise = 0;
        if(mode!="limit"){
            float ax = Vector3.Angle(worldPosition, Vector3.up);
            float ay = Vector3.Angle(worldPosition, Vector3.right);
            float az = Vector3.Angle(worldPosition, Vector3.forward);

            

            noise = Mathf.PerlinNoise((ax)/detail+planet.Seed, (ay)/detail+planet.Seed);
            noise += Mathf.PerlinNoise((ax)/detail+planet.Seed, (az)/detail+planet.Seed);
            noise += Mathf.PerlinNoise((ay)/detail+planet.Seed, (az)/detail+planet.Seed);
            noise = noise/3; 

            if(isworley){
                noise = (noise)*2-1;
                if(noise<0){
                    noise*=-1;
                }

                noise = 1-noise;
            }

            //noise = 1-Mathf.Pow(noise, 10);
            noise = noise*affection;
        }

        

        int point = 0;
        float pivot = 0;

        if(mode == "refresh"){
            
            pivot = planet.radius + noise;
            

        }else if(mode == "add"){
            pivot = planet.radius + currentPivot + noise;
            
        }
        else if(mode == "mult"){
            
            pivot = planet.radius + currentPivot * Mathf.Max(0,(noise-0.5f));
            
        }

        else if(mode == "limit"){
            
            pivot = Mathf.Min(planet.radius + currentPivot, affection);
            
        }

        
        float terrain = Mathf.Sqrt(sqHeight) - (pivot);

        if(terrain < 1){
            point = resourceIndex;
        }else{
            point = 0;
        }
        



        return new float[]{point, pivot-planet.radius, terrain};
        
        
    }



}
