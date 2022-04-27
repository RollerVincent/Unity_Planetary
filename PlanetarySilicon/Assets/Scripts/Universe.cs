using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



[ExecuteInEditMode]
public class Universe : MonoBehaviour
{
    public static GalaxyManager ACTIVE_GALAXY;
    public static SystemManager ACTIVE_SYSTEM;
    public static int GALAXY_SCALE = 100000000;

    

    public SystemManager currentSystem;
    public bool activateSystem;

    bool orbitView;

    // Start is called before the first frame update
    void Start()
    {
        //ActivateCurrentSystem();
        
    }

    

    // todo  set centerobject to star ???
    public void ActivateCurrentSystem(){

        currentSystem.gameObject.SetActive(true);

        ACTIVE_GALAXY = currentSystem.GetComponentInParent<GalaxyManager>();
        ACTIVE_SYSTEM = currentSystem;

        ACTIVE_GALAXY.focusStarIndex = ACTIVE_SYSTEM.galaxyStarIndex;
        ACTIVE_SYSTEM.RecenterSystem();
        ACTIVE_SYSTEM.RecenterMaterials();
        ACTIVE_GALAXY.RefocusStarIndex(Vector3.zero);

        
    }

    // todo function toggleActivesystem(true/false);
    public static void toggleActiveSystem(bool mode){
        if(ACTIVE_SYSTEM != null){
            ACTIVE_SYSTEM.gameObject.SetActive(mode);
        }
        
    }

    void DeactivateAllSystems(){

        foreach(SystemManager systemManager in GetComponentsInChildren<SystemManager>()){
            systemManager.gameObject.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {

        if(activateSystem){

            DeactivateAllSystems();
            ActivateCurrentSystem();

            activateSystem = false;
        }


        /*if(activeGalaxyIndex != currentActiveGalaxyIndex){
            ACTIVE_GALAXY = GetComponentsInChildren<GalaxyManager>()[activeGalaxyIndex];
            ACTIVE_GALAXY.RefocusStarIndex(Vector3.zero);
            currentActiveGalaxyIndex = activeGalaxyIndex;
        }*/



    }

    public void ToggleOrbitView(Image image){
        if(orbitView){
            ACTIVE_SYSTEM.GetComponentInChildren<OrbitTraceManager>().HideAll();
            image.color = GlobalSettings.UiColor;
            orbitView = false;
        }else{
            ACTIVE_SYSTEM.GetComponentInChildren<OrbitTraceManager>().ShowAll();
            image.color = GlobalSettings.UiActiveColor;
            orbitView = true;
        }
    }
}
