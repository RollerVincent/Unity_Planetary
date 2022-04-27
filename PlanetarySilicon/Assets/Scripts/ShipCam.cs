using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCam : MonoBehaviour
{

    public GameObject focus;
    public GameObject center_object;
    public float mouseSensitivity;
    public Steering rocketObject;
    public WalkingObject playerObject;
    public MarchingModifier marchingModifier;
    public float targetAttenuation;
    public float rotationAttenuation;
    Vector3 pivot;
    // Start is called before the first frame update
    void Start()
    {
        //transform.rotation = ship.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        center_object = focus.GetComponent<AttractedObject>().closest_gravityObject;
        
        transform.position =  focus.transform.position;//transform.position + (focus.transform.position-transform.position)*targetAttenuation*Time.deltaTime;

        Camera camera = GetComponentInChildren<Camera>();



             
        Vector3 center_dir = (transform.position - center_object.transform.position).normalized;
        
        Vector3 dir = (transform.position-camera.transform.position).normalized;
        
        Vector3 crossRight;
        if(center_dir == Vector3.zero){
            crossRight = Vector3.Cross(dir, -camera.transform.up).normalized;
        }else{
            crossRight = Vector3.Cross(dir, -center_dir).normalized;
        }
        
        Vector3 crossUp = Vector3.Cross(dir, crossRight).normalized;


        


        float angle = Vector3.Angle(crossRight, camera.transform.right);
        camera.transform.RotateAround(camera.transform.position, -Vector3.Cross(crossRight, camera.transform.right), angle*rotationAttenuation);

        angle = Vector3.Angle(crossUp, camera.transform.up);
        camera.transform.RotateAround(camera.transform.position, -Vector3.Cross(crossUp, camera.transform.up), angle*rotationAttenuation);
        

        camera.transform.RotateAround(transform.position, crossRight, -mouseSensitivity*Input.GetAxis("Mouse Y"));
        camera.transform.RotateAround(transform.position, crossUp, mouseSensitivity*Input.GetAxis("Mouse X"));


        if (Input.GetKeyDown(KeyCode.E))
        {
            if(rocketObject.enabled){
                rocketObject.enabled = false;
                playerObject.enabled = true;
                focus = playerObject.gameObject;
                GetComponentInChildren<Camera>().transform.parent.position = playerObject.transform.position;
                GetComponentInChildren<Camera>().transform.position = playerObject.transform.position + new Vector3(0,0,-0.01f);


                playerObject.gameObject.transform.position = rocketObject.gameObject.transform.position;
                playerObject.gameObject.SetActive(true);



                
            }else{
                rocketObject.enabled = true;
                playerObject.enabled = false;
                focus = rocketObject.gameObject;
                GetComponentInChildren<Camera>().transform.parent.position = rocketObject.transform.position;
                GetComponentInChildren<Camera>().transform.position = rocketObject.transform.position + new Vector3(0,0,-10f);

                playerObject.gameObject.SetActive(false);


            }
            
        }

        if(playerObject.enabled && Input.GetMouseButton(0)){
            RaycastHit hit;
        
            if (Physics.Raycast(playerObject.transform.position, camera.transform.forward, out hit)){

                if(hit.transform.gameObject.tag == "Terrain"){

                    marchingModifier.surfaceManager.divisionTarget.transform.position = hit.point;
                    marchingModifier.apply = true;

                    

                }
                
            }
        }

    }
}
