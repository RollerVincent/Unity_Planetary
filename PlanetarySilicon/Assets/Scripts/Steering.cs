using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steering : MonoBehaviour
{

    public string mode = "planet";
    public Camera camera;

    public float sensitivity;
    public float mouseSensitivity;

    public Vector3 pivotPosition;

    public float accumulation = 0.01f;

    float accumulated = 0;

    public bool hovermode = true;

    public float gyroStrength = 2;

    // Start is called before the first frame update
    void Start()
    {
        pivotPosition = transform.position;
    }

    // Update is called once per frame

    void LateUpdate(){
        
    }

    

    void Update()
    {

        
        Vector3 planet_dir = (transform.position - GetComponent<RCS>().center).normalized;
        Vector3 dir = (transform.position-camera.transform.position).normalized;
        Vector3 crossRight = Vector3.Cross(dir, -planet_dir).normalized;
        Vector3 crossUp = Vector3.Cross(dir, crossRight).normalized;


        


        /*float angle = Vector3.Angle(crossRight, camera.transform.right);
        camera.transform.RotateAround(camera.transform.position, -Vector3.Cross(crossRight, camera.transform.right), angle);

        angle = Vector3.Angle(crossUp, camera.transform.up);
        camera.transform.RotateAround(camera.transform.position, -Vector3.Cross(crossUp, camera.transform.up), angle);
        

        camera.transform.RotateAround(transform.position, crossRight, -mouseSensitivity*Input.GetAxis("Mouse Y"));
        camera.transform.RotateAround(transform.position, crossUp, mouseSensitivity*Input.GetAxis("Mouse X"));*/


        
        Vector3 cross3 = Vector3.Cross(crossRight, planet_dir).normalized; 


        

       
        if (Input.GetKeyDown(KeyCode.P))
        {
            GetComponent<RCS>().reversed = true;
            hovermode = true;
            GetComponent<RCS>().mainEngineEnabled = true;
            GetComponent<RCS>().targetP = transform.position;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            GetComponent<RCS>().reversed = true;
            hovermode = false;
            GetComponent<RCS>().mainEngineEnabled = false;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            GetComponent<RCS>().reversed = false;
            hovermode = false;
            GetComponent<RCS>().mainEngineEnabled = false;
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            if(hovermode){
                hovermode = false;
                GetComponent<RCS>().mainEngineEnabled = false;
            }else{
                hovermode = true;
                GetComponent<RCS>().mainEngineEnabled = true;
                GetComponent<RCS>().targetP = transform.position;
                //GetComponent<RCS>().hoverHeight = (transform.position-GetComponent<RCS>().center).magnitude;
            }
        }
        if(!hovermode){

            GetComponent<RCS>().rocketUp = Vector3.zero;

            if (Input.GetKey(KeyCode.Space))
            {
                
                GetComponent<RCS>().mainBooster.boost = GetComponent<RCS>().mainBooster.maxBoost;
                
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                
                GetComponent<RCS>().mainBooster.boost = 0;
                
            }

            if (Input.GetKey(KeyCode.W))
            {
                GetComponent<RCS>().rocketUp = camera.transform.forward*gyroStrength;
            }
            if (Input.GetKey(KeyCode.S))
            {
                GetComponent<RCS>().rocketUp = -camera.transform.forward*gyroStrength;
            }
            if (Input.GetKey(KeyCode.D))
            {
                GetComponent<RCS>().rocketUp = camera.transform.right*gyroStrength;
            }
            if (Input.GetKey(KeyCode.A))
            {
                GetComponent<RCS>().rocketUp = -camera.transform.right*gyroStrength;
            }


        }else{

            GetComponent<RCS>().storeHeight();

            if (Input.GetKey(KeyCode.D))
            {
                GetComponent<RCS>().targetP = transform.position + crossRight*sensitivity*accumulated;
                accumulated += accumulation;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                GetComponent<RCS>().targetP = transform.position - crossRight*sensitivity*accumulated;
                accumulated += accumulation;

            }
            else if (Input.GetKey(KeyCode.S))
            {
                GetComponent<RCS>().targetP = transform.position - cross3*sensitivity*accumulated;
                accumulated += accumulation;

            }
            else if (Input.GetKey(KeyCode.W))
            {
                GetComponent<RCS>().targetP = transform.position + cross3*sensitivity*accumulated;
                accumulated += accumulation;

            }else{
                accumulated = 0;
            }

            accumulated = Mathf.Min(1, accumulated);
        }
        

        

        
        
    }
}
