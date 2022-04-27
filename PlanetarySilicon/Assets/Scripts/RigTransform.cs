using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RigTransform : MonoBehaviour
{

    public Transform pose;
    public bool adopt;
    public bool keepAdopting;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(adopt || keepAdopting){
            Adopt();
            adopt = false;
        }
    }

    void Adopt(){
        Transform[] childBones = gameObject.GetComponentsInChildren<Transform>();
        Transform[] poseBones = pose.gameObject.GetComponentsInChildren<Transform>();


        for(int i=0; i<childBones.Length; i++){
            childBones[i].transform.localPosition = poseBones[i].transform.localPosition;
            childBones[i].transform.localRotation = poseBones[i].transform.localRotation;
            childBones[i].transform.localScale = poseBones[i].transform.localScale;
        }

    }
}
