using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusManager : MonoBehaviour
{

    public float focusScale;
    public float pivotOffset;
    public Sprite deleteCursor;
    public float connectionWidth;

    public HashSet<GameObject> parentObjects = new HashSet<GameObject>();

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject Focus(GameObject target){

        GameObject newObject = null;

        if(!parentObjects.Contains(target)){
            
            parentObjects.Add(target);
            
            newObject = GameObject.Instantiate(target);
            
            newObject.transform.parent = target.transform.parent;
            newObject.transform.position = target.transform.position;
            newObject.transform.rotation = target.transform.rotation;
            newObject.transform.localScale = target.transform.localScale;

            newObject.name = target.name + " FocusObject";

            //newObject.transform.parent = target.transform.parent;

            Vector3 pivPos = GameObject.Find("InteractionManager").GetComponent<InteractionManager>().pivotObject.transform.position;
            Vector3 camPos = GameObject.Find("InteractionManager").GetComponent<InteractionManager>().camera.transform.position;
            Vector3 pivDir = (pivPos - camPos).normalized;

            newObject.transform.position = pivPos + pivotOffset * pivDir;
            newObject.transform.localScale = newObject.transform.localScale * focusScale;


            newObject.transform.parent = GameObject.Find("InteractionManager").GetComponent<InteractionManager>().camera.transform;

            newObject.transform.localEulerAngles = Vector3.zero;

            FocusObject focusObject = newObject.AddComponent<FocusObject>();
            focusObject.cursor = deleteCursor;
            focusObject.parentObject = target;

         
            LineRenderer lr = newObject.AddComponent<LineRenderer>();

            //lr.SetColors(topColor, bottomColor);
            lr.SetWidth(connectionWidth, 0);

            lr.positionCount = 2;
            lr.SetPosition(0, target.transform.position);
            lr.SetPosition(1, newObject.transform.position);



        }




        


        return newObject;

    }
}
