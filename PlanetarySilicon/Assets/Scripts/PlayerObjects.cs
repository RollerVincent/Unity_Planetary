using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObjects : MonoBehaviour
{

    public GameObject activePlayerObject;
    public bool switchToActivePlayerObject;

    public float cameraCorrectionDistance = 200f;

    private GameObject currentGravityObject;
    private float lastGravityObjectDist = 0;

    public static GameObject GetActive(){
        return GameObject.Find("PlayerObjects").GetComponent<PlayerObjects>().activePlayerObject;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SwitchToActivePlayerObject(){
            


            /*foreach(PlayerObject playerObject in GetComponentsInChildren<PlayerObject>()){

                if(playerObject.controllingComponent != null){


                    if(playerObject.gameObject == activePlayerObject){
                        playerObject.controllingComponent.enabled = true;
                    }else{
                        playerObject.controllingComponent.enabled = false;
                    }

                }

            }*/
    }

    // Update is called once per frame
    void Update()
    {
        if(switchToActivePlayerObject){

            
            SwitchToActivePlayerObject();




            switchToActivePlayerObject = false;
        }


        checkGravityObject();
        if(CameraManager.playerIsActive && currentGravityObject != null){
            correctPlayerCamera();
        }
    }
    void correctPlayerCamera(){
        float gravityObjectDist = (activePlayerObject.transform.position - currentGravityObject.transform.position).magnitude;



        if(gravityObjectDist < cameraCorrectionDistance && lastGravityObjectDist >= cameraCorrectionDistance){
            GameObject.Find("CameraManager").GetComponent<CameraManager>().enablePlayerSurfaceViewMode = true;
            Debug.Log("Camera corrected to surface");
        }
        if(gravityObjectDist >= cameraCorrectionDistance && lastGravityObjectDist < cameraCorrectionDistance){
            GameObject.Find("CameraManager").GetComponent<CameraManager>().enablePlayerViewMode = true;
            Debug.Log("Camera corrected to space");
        }

        lastGravityObjectDist = gravityObjectDist;

       

    }

    void checkGravityObject(){

        if(currentGravityObject != activePlayerObject.GetComponent<AttractedObject>().closest_gravityObject){

            Vector3 oldPos;
            if(currentGravityObject == null){
                oldPos = activePlayerObject.GetComponent<AttractedObject>().closest_gravityObject.transform.localPosition;
            }else{
                oldPos = currentGravityObject.transform.localPosition;
            }

            currentGravityObject = activePlayerObject.GetComponent<AttractedObject>().closest_gravityObject;

            Vector3 newPos = currentGravityObject.transform.localPosition;

            activePlayerObject.transform.position += oldPos-newPos;

            Debug.Log("new Gravity Object");

            Universe.ACTIVE_SYSTEM.centerObject = currentGravityObject;
            Universe.ACTIVE_SYSTEM.RecenterSystem();
            Universe.ACTIVE_SYSTEM.RecenterMaterials();



            
        }
    }
}
