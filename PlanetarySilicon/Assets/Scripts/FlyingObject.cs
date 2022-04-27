using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FlyingObject : MonoBehaviour
{

    public Booster mainBooster;
    public List<Booster> throttledBoosters;
    public float gyroPower;
    public float gyroDrag;
    public bool pointUp;
    public bool autoHeight;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        AdjustMainThrottle();
        AdjustThrottle();
        AdjustGyros();
        AdjustHeight();
        
    }

    public void TogglePointUp(Image image){
        if(pointUp){
            pointUp = false;
            image.color = GlobalSettings.UiColor;
        }else{
            pointUp = true;
            image.color = GlobalSettings.UiActiveColor;
        }
        
    }

    public void ToggleAutoHeight(Image image){
        if(autoHeight){
            autoHeight = false;
            image.color = GlobalSettings.UiColor;
        }else{
            autoHeight = true;
            image.color = GlobalSettings.UiActiveColor;
        }
        
    }

    void AdjustHeight(){
        if(autoHeight){
            Vector3 targetVelocity = Vector3.zero;
            Vector3 velocity = GetComponent<Rigidbody>().velocity;
            Vector3 diff = targetVelocity-velocity;

            foreach(Booster booster in throttledBoosters){
                Vector3 boosterDir = -booster.transform.forward;
                float dot = Mathf.Max(0, Vector3.Dot(boosterDir, diff.normalized)*diff.magnitude);
                booster.throttle = dot;
                booster.throttle = Mathf.Min(1, booster.throttle);
            }
        }
    }

    void AdjustMainThrottle(){
       

        if(PlayerObjects.GetActive() == gameObject){
            if (Input.GetKey(KeyCode.Space) && !GameObject.Find("Canvas").GetComponent<MenuManager>().menuActive)
            {    
                mainBooster.throttle = 1f;
            }
            else
            {     
                mainBooster.throttle = 0.0f;
            }
        }

        

    }

    void AdjustThrottle(){
       
        if(PlayerObjects.GetActive() == gameObject){
            if (Input.GetKeyDown(KeyCode.UpArrow) && !GameObject.Find("Canvas").GetComponent<MenuManager>().menuActive)
            {     
                foreach(Booster booster in throttledBoosters){
                    booster.throttle += 0.2f;
                    booster.throttle = Mathf.Min(1, booster.throttle);
                }
                            
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) && !GameObject.Find("Canvas").GetComponent<MenuManager>().menuActive)
            {     
                foreach(Booster booster in throttledBoosters){
                    booster.throttle -= 0.2f;
                    booster.throttle = Mathf.Max(0, booster.throttle);
                }
            
            }
        }
    }

    void AdjustGyros(){

        Camera camera = GameObject.Find("PlayerCamera").GetComponentInChildren<Camera>();
        Rigidbody rb = GetComponent<Rigidbody>();

        Vector3 cross = Vector3.zero;
        
        if(pointUp){
            Vector3 dir = (transform.position - GetComponent<AttractedObject>().closest_gravityObject.transform.position).normalized;
            cross = Vector3.Cross(transform.forward, dir*gyroPower);
        }
        
        
        if(PlayerObjects.GetActive() == gameObject){
            if (Input.GetKey(KeyCode.W) && !GameObject.Find("Canvas").GetComponent<MenuManager>().menuActive)
            {
                cross = Vector3.Cross(transform.forward, camera.transform.forward*gyroPower);
            }
        }

        

        if(cross != null){
            rb.AddTorque(cross.x, cross.y, cross.z);
        }

        rb.angularDrag = gyroDrag;

        
        
    }
}
