using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BackPack : MonoBehaviour
{

    public float viewDistance;

    bool viewActive;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleView(Image image){

        if(viewActive){
            GameObject.Find("CameraManager").GetComponent<CameraManager>().EnablePlayerSurfaceViewMode();
            image.color = GlobalSettings.UiColor;
            viewActive = false;
        }else{

            GameObject.Find("PlayerCamera").GetComponent<CameraController>().setFocus(gameObject, viewDistance, Universe.ACTIVE_SYSTEM.centerObject);


            image.color = GlobalSettings.UiActiveColor;
            viewActive = true;
        }

        

    }
}
