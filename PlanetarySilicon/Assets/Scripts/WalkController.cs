using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WalkController : MonoBehaviour
{

    public Camera camera;

    public float speed;
    public float jumpHeight;

    
    public Transform leftFoot;
    public Transform rightFoot;

    public float footTouchDistance;

    public bool standing;
    public float standingDrag;
    public float standingStrength;


    public float standingHeight;


    public float cameraRotation;
    public float leaning;

    public float walkingPower;
    public float maxSpeed;


    Vector3 gravityDir;
    Transform currentFoot;
    Vector3 targetStandingPosition;

    public Light light;
    bool lightOn;



    // Start is called before the first frame update
    void Start()
    {
        currentFoot = rightFoot;
    }

     // Update is called once per frame
    void Update()
    {

        // gravity direction is down vector
        gravityDir = GetComponent<AttractedObject>().currentForce.normalized;





        // player feet touching the ground
        if(HasSurfaceContact()){

            Vector3 targetUp = -gravityDir;

            Vector3 targetFront = camera.transform.forward;
            Vector3 tmp = Vector3.Cross(targetUp, targetFront);
            targetFront = Vector3.Cross(tmp, targetUp);
                

            if(standing){

                
                




                ////// standing drag

                Rigidbody rb = GetComponent<Rigidbody>();
                rb.angularVelocity = rb.angularVelocity / (1f+standingDrag);

                rb.velocity = rb.velocity / (1f+standingDrag);
              


                ////// standing translation

                //UpdateTargetStandingPos();




                ////// standing rotation

                Vector3 upCross = Vector3.Cross(targetUp, -transform.up);
                    
                float angle = Vector3.Angle(targetUp, transform.up);
                
                //transform.RotateAround((leftFoot.position + rightFoot.position)/2f, upCross, angle*standingStrength);


                Vector3 torque = upCross * angle * standingStrength * 1;
                    
                rb.AddTorque(torque.x, torque.y, torque.z);
                




                if (Input.GetKey(KeyCode.W))
                {
                    WalkForward(currentFoot);

                    ////// front rotation

                    Vector3 frontCross = Vector3.Cross(targetFront, -transform.forward);
                
                    angle = Vector3.Angle(targetFront, transform.forward)/180f;
                    torque = frontCross * angle * cameraRotation * 1;
                    
                    rb.AddTorque(torque.x, torque.y, torque.z);

                }

            

            }

        }



     

    }

    bool HasSurfaceContact(){
        RaycastHit hit;
        if (Physics.Raycast(leftFoot.position, gravityDir, out hit, footTouchDistance))
        {
            return true;
        }
        if (Physics.Raycast(rightFoot.position, gravityDir, out hit, footTouchDistance))
        {
            return true;
        }
        return false;
    }

    void UpdateTargetStandingPos(){
        Vector3 mid = (leftFoot.position + rightFoot.position)/2f;

       
        
        RaycastHit hit;
        if (Physics.Raycast(mid - gravityDir*3, gravityDir, out hit, 20f))
        {
            targetStandingPosition = hit.point - gravityDir * standingHeight - transform.forward*leaning;
        }

        
        
    }


    void WalkForward(Transform foot){
        Vector3 targetUp = -gravityDir;
        Vector3 targetFront = transform.forward;
        Vector3 tmp = Vector3.Cross(targetUp, targetFront);
        targetFront = Vector3.Cross(tmp, targetUp);
        


        GetComponent<Rigidbody>().AddForce(targetFront*walkingPower);
      


        

        


    }

    public void ToggleLight(Image image){
        if(lightOn){
            light.enabled = false;
            lightOn = false;
            image.color = GlobalSettings.UiColor;
        }else{
            light.enabled = true;
            lightOn = true;
            image.color = GlobalSettings.UiActiveColor;
        }
    }
}
