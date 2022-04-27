using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingObject : MonoBehaviour
{
    public Camera camera;

    public float speed;
    public float jumpHeight;
    public float surfaceContactHeight;
    public float playerHeight;

    public float targetAttenuation;
    public float rotationForce;
    public float maxSpeed;
    


    public bool hasContact;
    public bool firstContact;

    Vector3 targetPos;

    public Transform leftFoot;
    public Transform rightFoot;

    public float footTouchDistance;

    public bool standing;
    public float standingDrag;
    public float muscleStrength;
    public float hingeDrag;
    public float standingStrength;
    public float standingHeight;
    public float walkingPower;
    public float footUpAmount;
    public float footVelocity;
    public float stepSize;
    public float pauseSize;
    public float cameraRotation;
    public float leaning;


    Vector3 gravityDir;
    Transform currentFoot;
    Vector3 targetStandingPosition;

    float stepAccumulation;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0.5f, 0.5f, 0.2f);
        
        
        Gizmos.DrawSphere(leftFoot.position, footTouchDistance);
        Gizmos.DrawSphere(rightFoot.position, footTouchDistance);


        //Gizmos.DrawLine(transform.position, transform.position+transform.forward);
    }





    // Start is called before the first frame update
    void Start()
    {
        currentFoot = rightFoot;
    }

    void OnCollisionEnter(Collision collision)
    {
        firstContact=true;
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

    void SetMuscleStrength(float strength){
        foreach(HingeJoint hinge in GetComponentsInChildren<HingeJoint>()){
            JointSpring spring = hinge.spring;
            spring.spring = strength;

            hinge.spring = spring;
            hinge.useSpring = true;
        }
    }

    void SetHingeDrag(float drag){
        foreach(HingeJoint hinge in GetComponentsInChildren<HingeJoint>()){
            JointSpring spring = hinge.spring;
            spring.damper = drag;

            hinge.spring = spring;
            hinge.useSpring = true;
        }
    }

    void WalkForward(Transform foot){
        Vector3 targetUp = -gravityDir;
        Vector3 targetFront = transform.forward;
        Vector3 tmp = Vector3.Cross(targetUp, targetFront);
        targetFront = Vector3.Cross(tmp, targetUp);
        


        float currentVelocity = foot.GetComponent<Rigidbody>().velocity.magnitude;

        float vdiff = Mathf.Max(0, footVelocity - currentVelocity);



        


        if(stepAccumulation < stepSize){
            //foot.GetComponent<Rigidbody>().AddForce(targetFront*walkingPower*vdiff);
            //foot.GetComponent<Rigidbody>().AddForce(targetUp*footUpAmount*vdiff);

            foot.parent.GetComponent<Rigidbody>().AddForce(targetFront*walkingPower*vdiff);
            foot.parent.parent.GetComponent<Rigidbody>().AddForce(-targetFront*walkingPower*vdiff);
            GetComponent<Rigidbody>().AddForce(targetFront*walkingPower*vdiff);
            //foot.parent.position += (targetFront*walkingPower*vdiff) * Time.deltaTime;
            //foot.parent.position += (targetUp*walkingPower*vdiff) * Time.deltaTime;
            
            
            
            
            //foot.parent.GetComponent<Rigidbody>().AddForce(targetUp*footUpAmount*vdiff);
        }




        stepAccumulation += 1;




        
        if(stepAccumulation >= stepSize + pauseSize){
            stepAccumulation = 0;
            if(currentFoot == rightFoot){
                currentFoot = leftFoot;
            }else{
                currentFoot = rightFoot;
            }
        }


        

        


    }

    void UpdateTargetStandingPos(){
        Vector3 mid = (leftFoot.position + rightFoot.position)/2f;

       
        
        RaycastHit hit;
        if (Physics.Raycast(mid - gravityDir*3, gravityDir, out hit, 20f))
        {
            targetStandingPosition = hit.point - gravityDir * standingHeight - transform.forward*leaning;
        }

        
        
    }
    void UpdateTargetStandingPos2(){
        Vector3 mid = (leftFoot.position + rightFoot.position)/2f;

       
        targetStandingPosition = mid - gravityDir * standingHeight;
        

        
        
    }

    void UpdateTargetStandingPos3(){

        

        RaycastHit hit;
        if (Physics.Raycast(transform.position, gravityDir, out hit, 20f))
        {
            targetStandingPosition = hit.point - gravityDir * standingHeight;
        }

        
        
    }



    // Update is called once per frame
    void Update()
    {

        // gravity direction is down vector
        gravityDir = GetComponent<AttractedObject>().currentForce.normalized;





        // set muscleStrength
        SetMuscleStrength(muscleStrength);

        // hinge drag
        SetHingeDrag(hingeDrag);


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
               // rb.velocity = rb.velocity / (1f+standingDrag);
                



                ////// standing translation

                UpdateTargetStandingPos();

                //Vector3 targetDiff = (targetStandingPosition-transform.position);
                //rb.AddForce(targetDiff*standingStrength);




                ////// standing rotation

                Vector3 upCross = Vector3.Cross(targetUp, -transform.up);
                    
                float angle = Vector3.Angle(targetUp, transform.up)/180f;
                Vector3 torque = upCross * angle * standingStrength * 1;
                    
                rb.AddTorque(torque.x, torque.y, torque.z);
                

                if (Input.GetKeyUp(KeyCode.W))
                {
                    stepAccumulation = 0;
                    if(currentFoot == rightFoot){
                        currentFoot = leftFoot;
                    }else{
                        currentFoot = rightFoot;
                    }

                }



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



        if(false && PlayerObjects.GetActive() == gameObject){

            GameObject center = GetComponent<AttractedObject>().closest_gravityObject;
            
            Vector3 dir = (transform.position - center.transform.position).normalized;

            Vector3 contactPoint;
            RaycastHit hit;

            if (Physics.Raycast(transform.position, -dir, out hit, surfaceContactHeight))
            {
                hasContact=true;
                contactPoint = hit.point;
            }else{
                hasContact=false;
            }



            if(hasContact && firstContact){


                targetPos = transform.position;
                Vector3 targetUp = dir;
                Vector3 targetFront = camera.transform.forward;
                
                ////// up rotation

                Vector3 upCross = Vector3.Cross(targetUp, -transform.up);
                
                float angle = Vector3.Angle(targetUp, transform.up)/180f;
                Vector3 torque = upCross * angle * rotationForce * 10;
                
                Rigidbody rb = GetComponent<Rigidbody>();
                rb.AddTorque(torque.x, torque.y, torque.z);

                

                ////// walking
                
                if(Input.GetKeyDown(KeyCode.Space)){
                    rb.AddForce(targetUp*jumpHeight);
                    firstContact=false;
                }


                if (Input.GetKey(KeyCode.W))
                {

                    targetPos += camera.transform.forward*speed;

                    ////// front rotation

                    Vector3 frontCross = Vector3.Cross(targetFront, -transform.forward);
                
                    angle = Vector3.Angle(targetFront, transform.forward)/180f;
                    torque = frontCross * angle * rotationForce;
                    
                    rb.AddTorque(torque.x, torque.y, torque.z);

                }


                if (Input.GetKey(KeyCode.S))
                {

                    targetPos -= camera.transform.forward*speed;

                }
                if (Input.GetKey(KeyCode.A))
                {
                    targetPos -= camera.transform.right*speed;
                    
                }
                if (Input.GetKey(KeyCode.D))
                {
                    targetPos += camera.transform.right*speed;

                }


                Vector3 targetDiff = (targetPos-transform.position);
                if(rb.velocity.magnitude <= maxSpeed){
                    rb.AddForce(targetDiff*Time.deltaTime*targetAttenuation);
                }else{
                    rb.velocity = rb.velocity.normalized * maxSpeed;
                }
                


                //transform.position = transform.position + (targetPos-transform.position)*targetAttenuation*Time.deltaTime;



            }

       
        }


    }
}
