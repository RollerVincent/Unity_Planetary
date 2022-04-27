using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class GalaxyManager : MonoBehaviour
{

    public int focusStarIndex;
    public bool refocusStar;

    public List<int> sytemStarIndices;
    


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(refocusStar){
            RefocusStarIndex(Vector3.zero);
            refocusStar = false;
        }
    }

    /*public void SwitchSystem(){
        ACTIVE_GALAXY = this;
        
        int c=0;
        foreach(int index in sytemStarIndices){
            if(ACTIVE_SYSTEM != null){
                ACTIVE_SYSTEM.gameObject.SetActive(false);
            }
            if(index == focusStarIndex){
                
                transform.GetChild(c).gameObject.SetActive(true);
                ACTIVE_SYSTEM = transform.GetChild(c).GetComponent<SystemManager>();
                //ACTIVE_SYSTEM.recenterSystem = true;
                ACTIVE_SYSTEM.RecenterSystem();
                ACTIVE_SYSTEM.RecenterMaterials();
                break;
            }
            c += 1;
        }
    }*/


    public void UpdateStar(int starIndex){
        
        Stars stars = GetComponent<Stars>();
        List<Vector3> starPositions = stars.GetTargetPositions();

        Shader.SetGlobalColor("_StarColor", stars.targetColors[focusStarIndex]);

        float cdist1 = (stars.targetColors[focusStarIndex].b-stars.targetColors[focusStarIndex].r);
        cdist1 = Mathf.Sqrt(cdist1 * cdist1);
        cdist1 = Mathf.Min(1f,(cdist1*5));
        float scale1 = 5000f * (1-cdist1);

        if(stars.additionalStars[focusStarIndex] != -1){
            Shader.SetGlobalColor("_AddStarColor", stars.targetColors[stars.additionalStars[focusStarIndex]]);

            float cdist = (stars.targetColors[stars.additionalStars[focusStarIndex]].b-stars.targetColors[stars.additionalStars[focusStarIndex]].r);
            cdist = Mathf.Sqrt(cdist * cdist);
            cdist = Mathf.Min(1f,(cdist*5));

            float scale2 = 5000f * (1-cdist);
            Debug.Log(scale2);

            if(scale1<scale2){
                float tmpscale = scale1;
                scale1 = scale2;
                scale2 = tmpscale;

                Shader.SetGlobalColor("_StarColor", stars.targetColors[stars.additionalStars[focusStarIndex]]);
                Shader.SetGlobalColor("_AddStarColor", stars.targetColors[focusStarIndex]);
                
            }

            scale2 *= 0.8f;

            GameObject.Find("ActiveStar").transform.localScale = Vector3.one * scale1;
            GameObject.Find("AddActiveStar").transform.localScale = Vector3.one * (1f/scale1 * scale2);
            GameObject.Find("AddActiveStar").GetComponent<MeshRenderer>().enabled = true;
            GameObject.Find("AddActiveStar").transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
            GameObject.Find("AddActiveStar").transform.position = GameObject.Find("ActiveStar").transform.position + (Vector3.right * scale1*0.5f*(1f/scale1 * scale2) + Vector3.right * scale2*0.5f*(1f/scale1 * scale2)) / (1f/scale1 * scale2) * 0.9f;
        }else{
            GameObject.Find("ActiveStar").transform.localScale = Vector3.one * scale1;
            GameObject.Find("AddActiveStar").GetComponent<MeshRenderer>().enabled = false;
            GameObject.Find("AddActiveStar").transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
        }
    }

    // set the focus of the universe to a star index of this galaxy
    // activates corresponding system
 
    public void RefocusStarIndex(Vector3 offset){

        Stars stars = GetComponent<Stars>();
        List<Vector3> starPositions = stars.GetTargetPositions();
        Vector3 focusStarPosition = starPositions[focusStarIndex];
        focusStarPosition += transform.position*Universe.GALAXY_SCALE;
        
        Shader.SetGlobalVector("_FocusStarPosition", focusStarPosition);
        Shader.SetGlobalVector("_FocusStarOffset", offset);

        

        UpdateStar(focusStarIndex);


        
        

        



    }
}
