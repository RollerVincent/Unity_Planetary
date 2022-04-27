using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class InverseKinematics : MonoBehaviour
{

    public Transform baseTransform;
    public Transform jointTransform;
    public Transform endPointTransform;
    public Transform handleTransform;

    public Vector3 adjustmentAngles;

    public bool invert;
    public bool stretch;
    public float stretchingStrength;

    public float upperLength;
    public float lowerLength;


    public bool setProportions;


     void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 1f);
        Gizmos.DrawSphere(jointTransform.position, 0.1f);
        Gizmos.DrawSphere(endPointTransform.position, 0.1f);
        Gizmos.DrawLine(jointTransform.position, jointTransform.GetChild(0).position);
        Gizmos.DrawLine(endPointTransform.position, jointTransform.position);
    }





    // Start is called before the first frame update
    void Start()
    {
        SetProportions();
    }

    // Update is called once per frame
    void Update()
    {

        if(setProportions){
            SetProportions();
            setProportions = false;
        }



        if(stretch){
            float l = (baseTransform.position - handleTransform.position).magnitude;
            float l01 = l/(lowerLength+upperLength);

           

            Vector3 strechedPos = baseTransform.position + (handleTransform.position-baseTransform.position).normalized*(lowerLength+upperLength)*stretchingStrength;
            handleTransform.position = Vector3.Lerp(handleTransform.position, strechedPos, 1);

        }



        ////  EndLength

        float currentAnchorEndLength = (baseTransform.position - handleTransform.position).magnitude;
        if(currentAnchorEndLength > (upperLength + lowerLength)*1f){
            handleTransform.position = baseTransform.position + (handleTransform.position - baseTransform.position).normalized * (upperLength + lowerLength)*1f;
        }
        float currentJointEndLength = (jointTransform.position - handleTransform.position).magnitude;
        if(currentJointEndLength < lowerLength){
            //handleTransform.position = jointTransform.position + (handleTransform.position - jointTransform.position).normalized * (lowerLength);
        }

        Vector3 endPos = handleTransform.position;


        ////  base Rotation

        Vector3 endDir = (handleTransform.position - baseTransform.position).normalized;

        Vector3 cross = Vector3.Cross(endDir, baseTransform.up).normalized;
        float angle = Vector3.Angle(endDir, baseTransform.up);
        baseTransform.RotateAround(baseTransform.position, cross, -angle);
        


        ////  Shoulder


        float currentAnchorEndLength01 = 1.0f*currentAnchorEndLength/(lowerLength+upperLength)*1f;
        float targetAngle = -(1-currentAnchorEndLength01)*90*2;


        //baseTransform.RotateAround(baseTransform.position, baseTransform.right, targetAngle);
        if(invert){
            baseTransform.RotateAround(baseTransform.position, -baseTransform.right, targetAngle);
        }else{
            baseTransform.RotateAround(baseTransform.position, baseTransform.right, targetAngle);
        }
       
      

        ////  Elbow


        Vector3 dir1 = (endPointTransform.position-jointTransform.position).normalized;
        Vector3 dir2 = (handleTransform.position-jointTransform.position).normalized;

        angle = Vector3.Angle(dir1, dir2); 
        
        jointTransform.up = dir2;
        jointTransform.localEulerAngles = Vector3.zero;
        jointTransform.LookAt(handleTransform.position);
        jointTransform.Rotate( 90, 0, 0 );
        
       
        Vector3 elbowDir = -Vector3.Cross(dir2, (jointTransform.position-baseTransform.position).normalized);


        cross = Vector3.Cross(elbowDir, jointTransform.right).normalized;
        angle = Vector3.Angle(elbowDir, jointTransform.right);
        jointTransform.RotateAround(jointTransform.position, cross, -angle);


    }

    void SetProportions(){
        upperLength = (baseTransform.position - jointTransform.position).magnitude;
        lowerLength = (endPointTransform.position - jointTransform.position).magnitude;
    }
}
