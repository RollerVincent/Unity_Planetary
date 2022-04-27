using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDistribution : MonoBehaviour
{
    
    public Texture2D noiseTexture;
    public float noiseDetail = 10;
    public float noiseCutoff = 0;
    public float meanHeight = 0;
    public float heightScale = 100;
    public int resourceIndex;
    public Color visColor;



    public float sample(Vector3 objectPos){
        float sqHeight = objectPos.sqrMagnitude;
		float heightDistance = sqHeight - (meanHeight*meanHeight);
		float heightFactor = 1 + Mathf.Max(heightDistance, -heightDistance)/heightScale;

        objectPos = objectPos/noiseDetail;
        //objectPos = new Vector3(Mathf.Max(objectPos.x, -objectPos.x), Mathf.Max(objectPos.y, -objectPos.y), Mathf.Max(objectPos.z, -objectPos.z));

        Vector2 v = new Vector2(objectPos.x+objectPos.y, objectPos.z);
        v = new Vector2(Mathf.Max(v.x, -v.x), Mathf.Max(v.y, -v.y));

        float dxy = (v.x)%1.0f;
        float axy = ((v.x) - dxy)%2;
        axy = (axy-0.5f)*2;
        if(axy<0){
            axy = dxy;   
        }else{
            axy = 1-dxy;
        }

        float dz = v.y%1.0f;
        float az = (v.y - dz)%2;
        az = (az-0.5f)*2;
        if(az<0){
            az = dz;
                 
        }else{
            az = 1-dz;
                
                
        }

        //objectPos = new Vector3(ax,ay,az);		

		float xy = noiseTexture.GetPixel((int)((axy)*noiseTexture.width), (int)(az*noiseTexture.height)).r;
		//float yz = 1;//noiseTexture.GetPixel(objectPos.y, objectPos.z).r;
		//float xz = 1;//noiseTexture.GetPixel(objectPos.x, objectPos.z).r;

		float noise = xy;//(xz+yz+xy)/3;

		noise = Mathf.Max(noise-noiseCutoff, 0)/(1-noiseCutoff);
		noise = noise / heightFactor;

        return noise;
    }



}
