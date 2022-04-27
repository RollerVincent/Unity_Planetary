using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusObject : Interactable
{

    public GameObject parentObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        LineRenderer lr = GetComponent<LineRenderer>();
        lr.SetPosition(0, parentObject.transform.position);
        lr.SetPosition(1, transform.position);
    }

    public override void OnClick(){

        GameObject.Find("FocusManager").GetComponent<FocusManager>().parentObjects.Remove(parentObject);
        DestroyImmediate(gameObject);

    }

    public override void OnDrag(){
        Vector3 pivPos = GameObject.Find("InteractionManager").GetComponent<InteractionManager>().pivotObject.transform.position;
        Vector3 camPos = GameObject.Find("InteractionManager").GetComponent<InteractionManager>().camera.transform.position;
        Vector3 pivDir = (pivPos - camPos).normalized;

        transform.position = pivPos +  GameObject.Find("FocusManager").GetComponent<FocusManager>().pivotOffset * pivDir;

        LineRenderer lr = GetComponent<LineRenderer>();
    }
}
