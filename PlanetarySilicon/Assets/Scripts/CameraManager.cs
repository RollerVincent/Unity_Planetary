using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public CameraController cameraController;

    public bool enableGalaxyViewMode;
    public bool enableGalaxyCenterViewMode;
    public bool enableStarViewMode;
    public bool enableObjectViewMode;
    public bool enablePlayerViewMode;
    public bool enablePlayerSurfaceViewMode;

    public static bool playerIsActive;




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(enableGalaxyViewMode){
            EnableGalaxyViewMode();
            enableGalaxyViewMode = false;
        }
        if(enableGalaxyCenterViewMode){
            EnableGalaxyCenterViewMode();
            enableGalaxyCenterViewMode = false;
        }
        if(enableStarViewMode){
            EnableStarViewMode();
            enableStarViewMode = false;
        }
        if(enableObjectViewMode){
            EnableObjectViewMode();
            enableObjectViewMode = false;
        }
        if(enablePlayerViewMode){
            EnablePlayerViewMode();
            enablePlayerViewMode = false;
        }
        if(enablePlayerSurfaceViewMode){
            EnablePlayerSurfaceViewMode();
            enablePlayerSurfaceViewMode = false;
        }
        
    }

    public void EnablePlayerViewMode(){
        
        GetComponent<VisualizationScaleManager>().visualizationScale = 1;  

        GameObject playerObject = GameObject.Find("PlayerObjects").GetComponent<PlayerObjects>().activePlayerObject;


        //cameraController.setFocus(playerObject, 0.01f, Universe.ACTIVE_SYSTEM.centerObject);
        cameraController.setFocus(playerObject, playerObject.GetComponent<PlayerObject>().cameraDistance);
        cameraController.mouseZoomSensivity = playerObject.GetComponent<PlayerObject>().cameraZoom;
    

        Universe.toggleActiveSystem(true);

        AdjustClippingPlanes(0.3f, 100000f);

        playerIsActive = true;
    
    }

    public void EnablePlayerSurfaceViewMode(){
        
        GetComponent<VisualizationScaleManager>().visualizationScale = 1;  

        GameObject playerObject = GameObject.Find("PlayerObjects").GetComponent<PlayerObjects>().activePlayerObject;


        cameraController.setFocus(playerObject, playerObject.GetComponent<PlayerObject>().cameraDistance, Universe.ACTIVE_SYSTEM.centerObject);
        cameraController.mouseZoomSensivity = playerObject.GetComponent<PlayerObject>().cameraZoom;
        //cameraController.setFocus(playerObject, 0.4f);
    

        Universe.toggleActiveSystem(true);

        AdjustClippingPlanes(0.3f, 100000f);

        playerIsActive = true;
    
    }

    
    public void EnableObjectViewMode(){

        GetComponent<VisualizationScaleManager>().visualizationScale = 1;  


        cameraController.setFocus(Universe.ACTIVE_SYSTEM.centerObject, 1000);
        cameraController.mouseZoomSensivity = 1000;



        Universe.toggleActiveSystem(true);

        AdjustClippingPlanes(0.3f, 100000f);

        playerIsActive = false;
        
    }

    public void EnableStarViewMode(){

        GetComponent<VisualizationScaleManager>().visualizationScale = 1;  


        cameraController.setFocus(Universe.ACTIVE_SYSTEM.transform.GetChild(0).gameObject, 100000);
        cameraController.mouseZoomSensivity = 100000;

        Universe.toggleActiveSystem(true);

        AdjustClippingPlanes(0.3f, 10000000f);

        playerIsActive = false;
        
    }


    // view the current center object from galaxy

    void EnableGalaxyViewMode(){

        GetComponent<VisualizationScaleManager>().visualizationScale = 8;  
        float viewScale = Mathf.Pow(10, -8);
        

        Vector3 FocusStarPosition = Shader.GetGlobalVector("_FocusStarPosition");
        Vector3 FocusStarOffset = Shader.GetGlobalVector("_FocusStarOffset");

        Vector3 pos = Vector3.zero;

        GameObject focusObject = transform.GetChild(0).gameObject;
        focusObject.transform.position = pos;

        cameraController.setFocus(focusObject, Universe.GALAXY_SCALE * viewScale *2);


        
        Universe.toggleActiveSystem(false);


        AdjustClippingPlanes(0.001f, 1000000f);

        playerIsActive = false;
        


    }

    // view the current galaxy centered

    void EnableGalaxyCenterViewMode(){

        GetComponent<VisualizationScaleManager>().visualizationScale = 8;  
        float viewScale = Mathf.Pow(10, -8);
        

        Vector3 FocusStarPosition = Shader.GetGlobalVector("_FocusStarPosition");
        Vector3 FocusStarOffset = Shader.GetGlobalVector("_FocusStarOffset");

        Vector3 pos = -(FocusStarPosition + FocusStarOffset)*viewScale;

        GameObject focusObject = transform.GetChild(0).gameObject;
        focusObject.transform.position = pos;

        cameraController.setFocus(focusObject, Universe.GALAXY_SCALE * viewScale *2);


        
        Universe.toggleActiveSystem(false);



        AdjustClippingPlanes(0.001f, 1000000f);


        playerIsActive = false;
    }

    public void Set(GameObject focusObject, float focusDistance, GameObject centerObject, float cameraZoom){
        cameraController.setFocus(focusObject, focusDistance, centerObject);
        cameraController.mouseZoomSensivity = cameraZoom;

    }


    void AdjustClippingPlanes(float near, float far){
        Camera camera = cameraController.GetComponentInChildren<Camera>();
        camera.nearClipPlane = near;
        camera.farClipPlane = far;


    }

    public void lookAtUniversePosition(Vector3 universePosition){

        Vector4 fsp = Shader.GetGlobalVector("_FocusStarPosition");

        universePosition = universePosition - new Vector3(fsp.x, fsp.y, fsp.z);


        cameraController.GetComponentInChildren<Camera>().transform.LookAt(universePosition);
    }
}
