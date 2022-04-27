using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemJumper : MonoBehaviour
{

    public SystemManager targetSystem;
    public float targetStartDistance;
    public int jumpSteps;

    public bool jump;

    private float currentStep;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Vector3 targetVector;
    private Vector3 targetPlayerPosition;
    private Vector3 startPlayerPosition;
    private Vector3 targetPlayerVector;
    


    // Start is called before the first frame update
    void Start()
    {
        currentStep = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(jump){

            int intCurrentStep = (int)currentStep;

            GameObject activePlayerObject = GameObject.Find("PlayerObjects").GetComponent<PlayerObjects>().activePlayerObject;

            if(intCurrentStep == 0){
                startPosition = Universe.ACTIVE_SYSTEM.UniversePosition() + Universe.ACTIVE_SYSTEM.centerObject.transform.localPosition;
                targetSystem.centerObject = targetSystem.transform.GetChild(0).gameObject;
                targetSystem.RecenterSystem();
                targetSystem.RecenterMaterials();
                targetPosition = targetSystem.UniversePosition();
                targetVector = targetPosition - startPosition;

                

                startPlayerPosition = activePlayerObject.transform.position;
                targetPlayerPosition = (startPlayerPosition-targetPosition).normalized*targetStartDistance;
                targetPlayerVector = targetPlayerPosition - startPlayerPosition;

                Universe.ACTIVE_SYSTEM.gameObject.SetActive(false);

                


                GameObject.Find("Universe").transform.GetChild(0).gameObject.SetActive(false);


            }

            

            int targetVectorMagnitude = (int)(targetVector.magnitude/100000f);

            float lerp = 1.0f*(intCurrentStep+1)/(jumpSteps*targetVectorMagnitude);
            Vector3 lerpPosition = startPosition + targetVector * lerp;

            /*Shader.SetGlobalColor("_StarColor", stars.targetColors[focusStarIndex]);*/
            Shader.SetGlobalVector("_FocusStarPosition", lerpPosition);
            Shader.SetGlobalVector("_FocusStarOffset", Vector3.zero);

            Vector3 playerLerpPosition = startPlayerPosition + targetPlayerVector * lerp;
            
            activePlayerObject.transform.position = playerLerpPosition;


          /*  if(intCurrentStep == (int)(jumpSteps/2)){
                Universe.ACTIVE_SYSTEM.gameObject.SetActive(false);
                
                targetSystem.gameObject.SetActive(true);
                
                targetSystem.centerObject = targetSystem.transform.GetChild(0).gameObject;
                targetSystem.RecenterSystem();
                targetSystem.RecenterMaterials();
            }*/

            

            currentStep += Time.deltaTime*100;
            intCurrentStep += 1;
            if(intCurrentStep>=jumpSteps*targetVectorMagnitude){

                targetSystem.gameObject.SetActive(true);

                targetSystem.centerObject = targetSystem.transform.GetChild(0).gameObject;
                //targetSystem.RecenterSystem();
                //targetSystem.RecenterMaterials();

                Universe.ACTIVE_SYSTEM = targetSystem;
                Universe.ACTIVE_GALAXY = targetSystem.GetComponentInParent<GalaxyManager>();
                Universe.ACTIVE_GALAXY.focusStarIndex = Universe.ACTIVE_SYSTEM.galaxyStarIndex;

                

                /*targetSystem.centerObject = targetSystem.transform.GetChild(0).gameObject;
                targetSystem.RecenterSystem();
                targetSystem.RecenterMaterials();*/

                GameObject.Find("Universe").transform.GetChild(0).gameObject.SetActive(true);
                //Shader.SetGlobalColor("_StarColor", Universe.ACTIVE_GALAXY.GetComponent<Stars>().targetColors[Universe.ACTIVE_SYSTEM.galaxyStarIndex]);
                
                Universe.ACTIVE_GALAXY.UpdateStar(targetSystem.galaxyStarIndex);


                GameObject.Find("CameraManager").GetComponent<CameraManager>().enablePlayerViewMode = true;



                jump = false;
                currentStep = 0;

            }

            //////////////////////GameObject.Find("CameraManager").GetComponent<CameraManager>().lookAtUniversePosition(targetPosition);

        }
    }

   
}
