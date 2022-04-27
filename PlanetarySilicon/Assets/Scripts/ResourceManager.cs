using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ResourceManager : MonoBehaviour
{

    public int baseResourceIndex;
    public Material visMaterial;
    public int visIndex;
    public bool updateVis;
    public bool debug;
    public bool activate;

    ResourceDistribution[] resourceDistributions;


    //public List<ResourceDistribution> distributions = new List<ResourceDistribution>();


    // Start is called before the first frame update
    void Start()
    {
        resourceDistributions = GetComponents<ResourceDistribution>();
    }

    void OnDrawGizmosSelected()
    {
        if(debug){
            for(int i=0;i<10000;i++){
                RaycastHit hit;
                Vector3 dir = new Vector3(Random.Range(-1f,1f), Random.Range(-1f,1f), Random.Range(-1f,1f)).normalized;
            
                if (Physics.Raycast(transform.position - dir*1000, dir, out hit)){

                    if(hit.transform.gameObject.tag == "Terrain"){

                        float s = resourceDistributions[visIndex].sample(hit.point-transform.position);//sampleObjectPos(hit.point-transform.position);
                        
                        Gizmos.color = new Color(s, 0.0f, 0.0f, 1f);
                        Gizmos.DrawSphere(hit.point, 0.5f);
                        

                    }
                    
                }
            }
        }

       
    }

    // Update is called once per frame
    void Update()
    {

        if(activate){


            foreach(Transform child in transform.parent.GetChild(0)){
                child.GetComponent<MeshCollider>().enabled = false;
                child.GetComponent<MeshRenderer>().sharedMaterial = visMaterial;
            }


            activate=false;
        }

        if(updateVis){

            ResourceDistribution distribution = GetComponents<ResourceDistribution>()[visIndex];

            visMaterial.SetTexture("_MainTex", distribution.noiseTexture);
            visMaterial.SetColor("_Color", distribution.visColor);
            visMaterial.SetFloat("_Detail", distribution.noiseDetail);
            visMaterial.SetFloat("_Cutoff", distribution.noiseCutoff);
            visMaterial.SetFloat("_MeanHeight", distribution.meanHeight);
            visMaterial.SetFloat("_HeightScale", distribution.heightScale);




        }
    }

    public int sampleObjectPos(Vector3 objectPos){
        Random.seed = (int)((objectPos.x + objectPos.y + objectPos.z)*1000);
        float r = Random.Range(0f, 0.5f);
        int resource = baseResourceIndex;
        foreach(ResourceDistribution distribution in resourceDistributions){
            float noise = distribution.sample(objectPos);
            noise = noise - noise%0.1f;
            if(noise>= r){
                resource = distribution.resourceIndex;
            }

        }
        return resource;
    }
}
