using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placeable : Interactable
{

    

    bool onSurface;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OverSurface(RaycastHit hit){
        GameObject placables = GameObject.Find("Placables");
        if(!onSurface){
            GameObject.Find("InteractionManager").GetComponent<InteractionManager>().SetCursor(cursor);
            GameObject.Find("InteractionManager").GetComponent<InteractionManager>().SetCursorColor(new Color(1,1,1,1));
            onSurface = true;
        }
        
        transform.position = hit.point; 

        transform.parent = placables.transform;

        Vector3 cross = Vector3.Cross(hit.collider.gameObject.transform.up, transform.right);
        float angle = Vector3.Angle(hit.collider.gameObject.transform.up, transform.right);
        transform.RotateAround(transform.position, cross, -angle);

        cross = Vector3.Cross(hit.normal, -transform.forward);
        angle = Vector3.Angle(hit.normal, -transform.forward);
        transform.RotateAround(transform.position, cross, -angle);

        

        //
        //transform.LookAt(hit.point - hit.normal);
        //transform.rotation = hit.collider.gameObject.transform.rotation;
        transform.parent = hit.collider.gameObject.transform;
        
    }

    void OverNothing(){
        if(onSurface){
            GameObject.Find("InteractionManager").GetComponent<InteractionManager>().SetAntiCursor();
            GameObject.Find("InteractionManager").GetComponent<InteractionManager>().SetAntiCursorColor();
            onSurface = false;
        }
        

        
    }

    public override void OnDrag(){
        
        GetComponent<Collider>().enabled = false;
        

        Vector3 pivPos = GameObject.Find("InteractionManager").GetComponent<InteractionManager>().pivotObject.transform.position;
        Vector3 camPos = GameObject.Find("InteractionManager").GetComponent<InteractionManager>().camera.transform.position;
        Vector3 pivDir = (pivPos - camPos).normalized;

        GameObject.Find("InteractionManager").GetComponent<InteractionManager>().currentPlacableSurfaceAlpha = GameObject.Find("InteractionManager").GetComponent<InteractionManager>().placableSurfaceAlpha;
        
        
        GameObject hitObject = null;
        RaycastHit hit;
        if (Physics.Raycast(pivPos, pivDir, out hit, 100f))
        {
            hitObject = hit.collider.gameObject;
            if(hitObject.GetComponent<PlacementSurface>() != null){


                OverSurface(hit);
                GetComponent<Collider>().enabled = true;
                return;

               
                

                
            }
            
        }
        OverNothing();
        GetComponent<Collider>().enabled = true;
       



        

        
    }

    public override void OnDragEnd(){
        GameObject.Find("InteractionManager").GetComponent<InteractionManager>().SetCursorColor(new Color(1,1,1,1));
        GameObject.Find("InteractionManager").GetComponent<InteractionManager>().currentPlacableSurfaceAlpha = 0;

    }



    
}
