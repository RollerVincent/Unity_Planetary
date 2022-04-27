using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class PlaceHolder : MonoBehaviour
{

    public bool toggle;

    GameObject camera;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(toggle){
            if(checkforToggle()){
                
                Toggle();
            }
        }

        
        
    }

    public void Toggle(){

        if (transform.GetChild(0).gameObject.activeSelf){

            for(int i=0;i<transform.parent.childCount; i++){
                Transform child = transform.parent.GetChild(i);
                if(child.gameObject.tag != "ToggleExclusion"){
                    child.gameObject.SetActive(true);
                }
            }

            for(int i=0;i<transform.childCount; i++){
                Transform child = transform.GetChild(i);
                child.gameObject.SetActive(false);
            }

            PlanetInstance planetInstance = GetComponentInParent<PlanetInstance>();
            if(planetInstance != null){
                planetInstance.CheckLoad();
                
            }



            

        }else{

            for(int i=0;i<transform.parent.childCount; i++){
                Transform child = transform.parent.GetChild(i);
                if(child.gameObject.tag != "ToggleExclusion"){
                    child.gameObject.SetActive(false);
                }
            }

            for(int i=0;i<transform.childCount; i++){
                Transform child = transform.GetChild(i);
                child.gameObject.SetActive(true);
            }



            

        }





    }


    public bool checkforToggle(){
        if(camera == null){
            camera = GameObject.Find("Main Camera");
        }

        float dist = (camera.transform.position-transform.position).sqrMagnitude;
        
        if(transform.GetChild(0).gameObject.activeSelf){
            if(dist<GlobalSettings.PlaceHolderDistance*GlobalSettings.PlaceHolderDistance){

                return true;


            }
        }else{
            if(dist>=GlobalSettings.PlaceHolderDistance*GlobalSettings.PlaceHolderDistance){

                return true;


            }else{
                PlanetInstance planetInstance = GetComponentInParent<PlanetInstance>();
                if(planetInstance != null){
                    planetInstance.CheckLoad();
                    
                }
            }
        }

        return false;
        


    }



}
