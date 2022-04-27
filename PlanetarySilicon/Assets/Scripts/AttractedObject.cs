using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttractedObject : MonoBehaviour
{

    public GameObject closest_gravityObject;
    public Atmossphere currentAtmosphere;

    private bool velocityPreview;

    public Vector3 currentForce;

    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("GravityManager").GetComponent<GravityManager>().attractedObjects.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        if(closest_gravityObject != null){
            currentAtmosphere = closest_gravityObject.GetComponentInChildren<Atmossphere>();
            if(currentAtmosphere != null){
                if(!currentAtmosphere.isInside(gameObject)){
                    currentAtmosphere = null;
                };
            }
        }
    }

  

    public void ToggleVelocityPreview(Image image){
        if(velocityPreview){
            GetComponent<VelocityVisualizer>().enabled = false; 
            GetComponentInChildren<LineRenderer>().enabled = false; 
            velocityPreview = false;
            image.color = GlobalSettings.UiColor;
        }else{
            GetComponent<VelocityVisualizer>().enabled = true; 
            GetComponentInChildren<LineRenderer>().enabled = true; 
            velocityPreview = true;
            image.color = GlobalSettings.UiActiveColor;
        }
        
    }
}
