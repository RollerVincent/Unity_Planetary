using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonController : MonoBehaviour
{

    public Transform leftFoot;
    public Transform rightFoot;

    public Transform leftHand;
    public Transform rightHand;

    public Transform upperBody;


    public float upForce;
    public float angularDrag;
    public float innerForce;

    

    

    [Range(-90f,90f)]
    public float hipBending;

    [Range(0f, 1f)]
    public float shoulderDragMultiplier;


    [Range(0f, 1f)]
    public float leftKneeBend;
    public bool setLeftKneeBend;
    [Range(0f, 1f)]
    public float rightKneeBend;
    public bool setRightKneeBend;


    [Range(0f, 1f)]
    public float leftElbowBend;
    public bool setLeftElbowBend;
    [Range(0f, 1f)]
    public float rightElbowBend;
    public bool setRightElbowBend;

    [Range(0f, 1f)]
    public float ElbowForceMultiplier;
    


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SetAllDrags(angularDrag);
        SetAllInnerForces(innerForce);
        MultiplyElbows();
        //SetZeroForce();
        SetBends();
        //MultiplyShoulderDrags();
        
        ////// global angular drag

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.angularVelocity = rb.angularVelocity / (1f+angularDrag);


        pointUp();



        SetHipBending(hipBending);

    }

    void SetJointAngle(Transform joint, float angle){
        HingeJoint hinge = joint.GetComponent<HingeJoint>();
        JointSpring hingeSpring = hinge.spring;
        hingeSpring.targetPosition = angle;
        
        hinge.spring = hingeSpring;
        hinge.useSpring = true;

    }

    void SetZeroForce(){
        HingeJoint hinge = leftHand.parent.GetComponent<HingeJoint>();
        JointSpring spring = hinge.spring;
        spring.spring = 0;
        hinge.spring = spring;
        hinge.useSpring = true;

        hinge = rightHand.parent.GetComponent<HingeJoint>();
        spring = hinge.spring;
        spring.spring = 0;
        hinge.spring = spring;
        hinge.useSpring = true;

        hinge = leftFoot.parent.GetComponent<HingeJoint>();
        spring = hinge.spring;
        spring.spring = 0;
        hinge.spring = spring;
        hinge.useSpring = true;

        hinge = rightFoot.parent.GetComponent<HingeJoint>();
        spring = hinge.spring;
        spring.spring = 0;
        hinge.spring = spring;
        hinge.useSpring = true;

        
    }



    void pointUp(){

        Rigidbody rb = GetComponent<Rigidbody>();

        Vector3 gravityDir = GetComponent<AttractedObject>().currentForce.normalized;
        Vector3 targetUp = -gravityDir;


        Vector3 upCross = Vector3.Cross(targetUp, -transform.up);  
        float angle = Vector3.Angle(targetUp, transform.up);


        Vector3 torque = upCross * angle * upForce;
                    
        rb.AddTorque(torque.x, torque.y, torque.z);
    }

    void SetHipBending(float bending){

        Transform leftHip = leftFoot.parent.parent;
        SetJointAngle(leftHip, bending);

        Transform rightHip = rightFoot.parent.parent;
        SetJointAngle(rightHip, bending);

    }

    void SetBends(){

        HingeJoint hinge;
        JointSpring spring;

        if(setLeftElbowBend){
            hinge = leftHand.parent.GetComponent<HingeJoint>();
            spring = hinge.spring;
            spring.targetPosition = leftElbowBend*180f;
            hinge.spring = spring;
            hinge.useSpring = true;

        }
        if(setRightElbowBend){
            hinge = rightHand.parent.GetComponent<HingeJoint>();
            spring = hinge.spring;
            spring.targetPosition = rightElbowBend*180f;
            hinge.spring = spring;
            hinge.useSpring = true;
        }
        if(setLeftKneeBend){
            hinge = leftFoot.parent.GetComponent<HingeJoint>();
            spring = hinge.spring;
            spring.targetPosition = leftKneeBend*180f;
            hinge.spring = spring;
            hinge.useSpring = true;
        }
        if(setRightKneeBend){
            hinge = rightFoot.parent.GetComponent<HingeJoint>();
            spring = hinge.spring;
            spring.targetPosition = rightKneeBend*180f;
            hinge.spring = spring;
            hinge.useSpring = true;
        }

    }

    void SetAllDrags(float drag){
        
        foreach(HingeJoint hinge in GetComponentsInChildren<HingeJoint>()){
            JointSpring spring = hinge.spring;
            spring.damper = drag;
            hinge.spring = spring;
            hinge.useSpring = true;
        }

        foreach(SpringJoint spring in GetComponentsInChildren<SpringJoint>()){
            spring.damper = drag;
        }

        foreach(CharacterJoint joint in GetComponentsInChildren<CharacterJoint>()){
            SoftJointLimitSpring spring = joint.swingLimitSpring;
            spring.damper = drag;
            joint.swingLimitSpring = spring;

            spring = joint.twistLimitSpring;
            spring.damper = drag;
            joint.twistLimitSpring = spring;
            
        }
    }

    void SetAllInnerForces(float force){
        
        foreach(HingeJoint hinge in GetComponentsInChildren<HingeJoint>()){
            JointSpring spring = hinge.spring;
            spring.spring = force;
            hinge.spring = spring;
            hinge.useSpring = true;
        }

        foreach(SpringJoint spring in GetComponentsInChildren<SpringJoint>()){
            spring.spring = force;
        }

        foreach(CharacterJoint joint in GetComponentsInChildren<CharacterJoint>()){
            SoftJointLimitSpring spring = joint.swingLimitSpring;
            spring.spring = force;
            joint.swingLimitSpring = spring;

            spring = joint.twistLimitSpring;
            spring.spring = force;
            joint.twistLimitSpring = spring;
            
        }
    }

    void MultiplyShoulderDrags(){

        //////  Shoulders
        
        Transform leftShoulder = leftHand.parent.parent;
        CharacterJoint joint = leftShoulder.GetComponent<CharacterJoint>();

        SoftJointLimitSpring spring = joint.swingLimitSpring;
        spring.damper = shoulderDragMultiplier * angularDrag;
        joint.swingLimitSpring = spring;

        

        Transform rightShoulder = rightHand.parent.parent;
        joint = rightShoulder.GetComponent<CharacterJoint>();

        spring = joint.swingLimitSpring;
        spring.damper = shoulderDragMultiplier * angularDrag;
        joint.swingLimitSpring = spring;


            



        

    }

    void MultiplyElbows(){
        
        HingeJoint hinge  = leftHand.parent.GetComponent<HingeJoint>();
        JointSpring spring = hinge.spring;
        spring.spring = innerForce * ElbowForceMultiplier;
        hinge.spring = spring;
        hinge.useSpring = true;


        hinge  = rightHand.parent.GetComponent<HingeJoint>();
        spring = hinge.spring;
        spring.spring = innerForce * ElbowForceMultiplier;
        hinge.spring = spring;
        hinge.useSpring = true;
        
    }

}
