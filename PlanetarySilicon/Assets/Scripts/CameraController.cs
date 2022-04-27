using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public GameObject focusObject;
    public float focusDistance;
    public GameObject centerObject;
    public float mouseSensitivity;
    public float targetAttenuation;
    public float rotationAttenuation;

    public float mouseZoomSensivity;

    Vector2 mouseVelocity = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        //transform.rotation = ship.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if(true){


            transform.position = focusObject.transform.position;

            Camera camera = GetComponentInChildren<Camera>();

            
    
            Vector3 center_dir = (transform.position - centerObject.transform.position).normalized;
            Vector3 dir = (transform.position-camera.transform.position).normalized;
            if(transform.position==camera.transform.position){
                dir = Vector3.up;
            }
            
            Vector3 crossRight;
            if(center_dir == Vector3.zero){
                crossRight = Vector3.Cross(dir, -camera.transform.up).normalized;
            }else{
                crossRight = Vector3.Cross(dir, -center_dir).normalized;
            }
            
            Vector3 crossUp = Vector3.Cross(dir, crossRight).normalized;


            focusDistance -= Input.mouseScrollDelta.y*mouseZoomSensivity*0.1f;

            Vector3 targetPos = transform.position - dir * focusDistance;

            camera.transform.position = camera.transform.position + (targetPos-camera.transform.position) * targetAttenuation*0.1f;
            


            float angle = Vector3.Angle(crossRight, camera.transform.right);
            camera.transform.RotateAround(camera.transform.position, -Vector3.Cross(crossRight, camera.transform.right), angle * targetAttenuation*0.4f);

            angle = Vector3.Angle(crossUp, camera.transform.up);
            camera.transform.RotateAround(camera.transform.position, -Vector3.Cross(crossUp, camera.transform.up), angle * targetAttenuation*0.4f);
            
            if(!GameObject.Find("Canvas").GetComponent<MenuManager>().menuActive && !GameObject.Find("InteractionManager").GetComponent<InteractionManager>().panCursor){
                mouseVelocity = (mouseVelocity*rotationAttenuation) + new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"))*mouseSensitivity;
            }
            else{
                mouseVelocity = Vector2.zero;
            }



            camera.transform.RotateAround(transform.position, crossRight, -mouseVelocity.y);
            camera.transform.RotateAround(transform.position, crossUp, mouseVelocity.x);


            





        }
        
        
      

    }

    /*
    void Update()
    {
        if(true){
            transform.position = focusObject.transform.position;

            Camera camera = GetComponentInChildren<Camera>();

            GameObject pivot = transform.parent.GetChild(1);
    
            Vector3 center_dir = (transform.position - centerObject.transform.position).normalized;
            Vector3 dir = (transform.position-camera.transform.position).normalized;
            if(transform.position==camera.transform.position){
                dir = Vector3.up;
            }
            
            Vector3 crossRight;
            if(center_dir == Vector3.zero){
                crossRight = Vector3.Cross(dir, -camera.transform.up).normalized;
            }else{
                crossRight = Vector3.Cross(dir, -center_dir).normalized;
            }
            
            Vector3 crossUp = Vector3.Cross(dir, crossRight).normalized;

            focusDistance -= Input.mouseScrollDelta.y*mouseZoomSensivity*0.1f;


            camera.transform.position = transform.position - dir * focusDistance;
            


            float angle = Vector3.Angle(crossRight, camera.transform.right);
            camera.transform.RotateAround(camera.transform.position, -Vector3.Cross(crossRight, camera.transform.right), angle);

            angle = Vector3.Angle(crossUp, camera.transform.up);
            camera.transform.RotateAround(camera.transform.position, -Vector3.Cross(crossUp, camera.transform.up), angle);
            

            camera.transform.RotateAround(transform.position, crossRight, -mouseSensitivity*Input.GetAxis("Mouse Y"));
            camera.transform.RotateAround(transform.position, crossUp, mouseSensitivity*Input.GetAxis("Mouse X"));


        }
        
        
      

    }

    */

    public void setFocus(GameObject focusObject, float focusDistance, GameObject centerObject=null){
        this.focusObject = focusObject;
        this.focusDistance = focusDistance;
        if(centerObject==null){
            centerObject = focusObject;
        }
        this.centerObject=centerObject;
    }
}
