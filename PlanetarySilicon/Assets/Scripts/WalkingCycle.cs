using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WalkingCycle : MonoBehaviour
{

    public Transform LeftFoot;
    public Transform RightFoot;

    public float stepLength;
    public float stepWidth;

    public float LiftAmount;
    public float targetAttenuation;
    public float standingHeight;

    int currentLeftLift;
    int currentRightLift;

    Vector3 rightStepPosition;
    Vector3 leftStepPosition;
    Vector3 targetPos;

    public bool debug;

     void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 1f);
        Gizmos.DrawSphere(GetStepPosition(true), 0.1f);
        Gizmos.DrawSphere(GetStepPosition(false), 0.1f);
        
        //Gizmos.DrawLine(endPointTransform.position, jointTransform.position);
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        targetPos = transform.position;
                



        targetPos = GetStepPosition(true) + transform.up * standingHeight;





        Vector3 targetDiff = (targetPos-transform.position);
        rb.AddForce(targetDiff*Time.deltaTime*targetAttenuation);
        












        if(currentLeftLift > 0){
            if(currentLeftLift<LiftAmount*2){
                currentLeftLift += 1;
            }else{
                currentLeftLift = 0;
            }
        }

        if(currentLeftLift > 0){
            if(currentLeftLift<LiftAmount){
                
                currentLeftLift += 1;
            
            }else{
                
            }
        }

       /* float leftLift = (currentLeftLift - LiftAmount);
        if(leftLift < 0){
            leftLift = - leftLift;
        }
        leftLift = LiftAmount - leftLift;*/




        if(debug){

            Vector3 p = GetStepPosition(true);
            LeftFoot.transform.position = p;

            
        }
    }

    Vector3 GetStepPosition(bool right){
        RaycastHit hit;
        if (Physics.Raycast((LeftFoot.position+RightFoot.position)/2f, -transform.up, out hit, 100f))
        {
            return hit.point;
        }
        return Vector3.zero;
    }

    Vector3 LiftFoot(bool right){
        RaycastHit hit;
        if (Physics.Raycast(transform.position+transform.forward*stepLength, -transform.up, out hit, 100f))
        {
            return hit.point;
        }
        return Vector3.zero;
    }
}
