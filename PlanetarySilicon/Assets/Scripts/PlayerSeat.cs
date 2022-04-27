using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[ExecuteInEditMode]
public class PlayerSeat : MonoBehaviour
{

    public PlayerObject playerObject;
    public PlayerObject sitObject;
    public Vector3 sittingOffset;
    public bool sit;
    public bool toggleView;
    public bool leave;

    bool viewIsActive = false;
    public bool isSitting = false;

    public string triggerKey;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(sit){



            playerObject.GetComponent<Collider>().enabled = false;
            foreach(Collider collider in playerObject.gameObject.GetComponentsInChildren<Collider>()){
                collider.enabled = false;
            }

            PlayerObjects playerObjects = GameObject.Find("PlayerObjects").GetComponent<PlayerObjects>();
            playerObjects.activePlayerObject = sitObject.GetComponent<PlayerObject>().gameObject;
            playerObjects.SwitchToActivePlayerObject();




            isSitting = true;
            sit = false;
            
        }

        


        if(isSitting){
            playerObject.GetComponent<Rigidbody>().velocity = sitObject.GetComponent<Rigidbody>().velocity;
            playerObject.transform.position = transform.position + transform.up*sittingOffset.z + transform.forward*-sittingOffset.x;
            playerObject.transform.rotation = transform.rotation;
        }

        if(toggleView){
            if(!viewIsActive){

                GameObject.Find("CameraManager").GetComponent<CameraManager>().Set(playerObject.gameObject, playerObject.cameraDistance, gameObject, playerObject.cameraZoom);


                viewIsActive = true;
            }else{

                
                GameObject.Find("CameraManager").GetComponent<CameraManager>().EnablePlayerViewMode();


                viewIsActive=false;
            }
            toggleView = false;
        }

        if(leave){
            isSitting = false;
            playerObject.GetComponent<Collider>().enabled = true;
            foreach(Collider collider in playerObject.gameObject.GetComponentsInChildren<Collider>()){
                collider.enabled = true;
            }

            PlayerObjects playerObjects = GameObject.Find("PlayerObjects").GetComponent<PlayerObjects>();
            playerObjects.activePlayerObject = playerObject.gameObject;
            playerObjects.SwitchToActivePlayerObject();
            

            leave = false;
        }

        


        if (Input.GetKeyDown(triggerKey) && !GameObject.Find("Canvas").GetComponent<MenuManager>().menuActive)
        {
            if(isSitting){
                leave = true;
            }else{
                sit = true;
            }
        }
    }

    public void ToggleSitting(Image image){
        if(isSitting){
            leave = true;
            GameObject.Find("PlayerObjects").GetComponent<PlayerObjects>().activePlayerObject = playerObject.gameObject;
            GameObject.Find("CameraManager").GetComponent<CameraManager>().EnablePlayerSurfaceViewMode();
            
            image.color = GlobalSettings.UiColor;
        }else{
            sit = true;
            GameObject.Find("PlayerObjects").GetComponent<PlayerObjects>().activePlayerObject = sitObject.gameObject;
            GameObject.Find("CameraManager").GetComponent<CameraManager>().EnablePlayerSurfaceViewMode();
            
            image.color = GlobalSettings.UiActiveColor;
        }
    }
}
