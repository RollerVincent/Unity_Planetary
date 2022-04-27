using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SystemManager : MonoBehaviour
{

    public int galaxyStarIndex;
    public GameObject centerObject;
    public bool recenter;
    public bool recenterSystem;
    public int skippingFrames;
    public GameObject playerObject;

    int currentSkip;

    GameObject currentCenterObject = null;


    void OnDrawGizmosSelected()
    {
        


        Vector3 up = UniversePosition();
        Random.seed = (int)(up.x+up.y+up.z);

            

        Vector3[] axes = GetMainAxes();
        Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 1f);
        Gizmos.DrawLine(transform.position - axes[0]*100000, transform.position + axes[0]*100000);
        Gizmos.color = new Color(0.0f, 1.0f, 0.0f, 1f);
        Gizmos.DrawLine(transform.position - axes[1]*100000, transform.position + axes[1]*100000);
                        
        
        


       
    }


    // Start is called before the first frame update
    void Start()
    {
        currentSkip = 0;
    }

    // Update is called once per frame
    void Update()
    {
        currentSkip +=1;
        if(currentSkip>=skippingFrames){
            if(currentCenterObject != centerObject){
                currentCenterObject = centerObject;
            }
            
            if(recenter){
                //RecenterMaterials();
            }
            currentSkip=0;
        }
        if(GlobalSettings.ApplyOrbits){
            applyOrbits();     
            applySpins(); 
            //if(playerObject.GetComponent<WalkingObject>().hasContact){
                //spinPlayer();
            //}
            RecenterSystem();
        }

        if(recenterSystem){
            RecenterSystem();
            RecenterMaterials();
            recenterSystem=false;
        }
        
        


    }

    List<GameObject> childOrbitObjects(Orbit orbit){
        List<GameObject> objects = new List<GameObject>();
        List<GameObject> remaining = new List<GameObject>();
        remaining.Add(orbit.orbitObject);

        while(remaining.Count>0){
            List<GameObject> tmp_remaining = new List<GameObject>();
            foreach(GameObject orbitObject in remaining){
                objects.Add(orbitObject);
                foreach(Orbit childOrbit in orbitObject.GetComponents<Orbit>()){
                    tmp_remaining.Add(childOrbit.orbitObject);
                }
                
            }
            remaining = tmp_remaining;
        }
        return objects;
    } 

    void applyOrbit(Orbit orbit){
        List<GameObject> childObjects = childOrbitObjects(orbit);
        Vector3 target = childObjects[0].transform.position-orbit.transform.position;
        float targetMag = target.magnitude;
        Vector3 rotated = Quaternion.AngleAxis(orbit.amount*GlobalSettings.OrbitSpeed*GlobalSettings.PlayBackSpeed/targetMag, orbit.axis) * target;
        childObjects[0].transform.position = orbit.transform.position+rotated;
        Vector3 diff = childObjects[0].transform.position - (orbit.transform.position+target);
        for(int i=1; i<childObjects.Count; i++){
            childObjects[i].transform.position += diff;
        }
        
    }

    void applyOrbits(){
        GameObject sun = transform.GetChild(0).gameObject;
        List<Orbit> remaining = new List<Orbit>();
        foreach(Orbit orbit in sun.GetComponents<Orbit>()){
            remaining.Add(orbit);
        }
        while(remaining.Count>0){
            List<Orbit> tmp_remaining = new List<Orbit>();
            foreach(Orbit orbit in remaining){   
                //Debug.Log("applying orbit from " + orbit.gameObject.name + " to " + orbit.orbitObject.name);   
                applyOrbit(orbit);
                foreach(Orbit childOrbit in orbit.orbitObject.GetComponents<Orbit>()){
                    tmp_remaining.Add(childOrbit);
                }

            } 



            remaining = tmp_remaining;
        }
    }

    

    public void RecenterSystem(){

        GameObject lightSource = transform.parent.parent.GetChild(0).GetChild(0).gameObject;
        
      /*  if(sun.transform.position != Vector3.zero){
            lightSource.transform.forward = (centerObject.transform.position-sun.transform.position).normalized;
        }*/

        transform.position = -centerObject.transform.localPosition;
        transform.parent.parent.GetChild(0).position = transform.position;

        //GetComponentInParent<GalaxyManager>().RefocusStarIndex(centerObject.transform.localPosition);

        Shader.SetGlobalVector("_LightCenter", lightSource.transform.position);
        Shader.SetGlobalVector("_PlanetCenter", centerObject.transform.position);
    }

    public void RecenterMaterials(){
        GameObject sun = transform.GetChild(0).gameObject;
        foreach(MaterialCenterer centerer in GetComponentsInChildren<MaterialCenterer>()){
            centerer.lightOrigin = sun.transform.position;
            centerer.center = true;
        }
    }

   


    void applySpins(){
        foreach(Spin spin in GetComponentsInChildren<Spin>()){
            spin.applySpin();
        }
        Spin starSpin = GameObject.Find("ActiveStar").GetComponent<Spin>();
        starSpin.axis = starSpin.transform.up;
        starSpin.applySpin();
    }

    public Vector3[] GetMainAxes(){
        Vector3[] r = new Vector3[2];
        
        r[0] = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
        r[1] = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;

        r[1] = Vector3.Cross(r[0], r[1]);

        return r;
    }

    public Vector3 GetMainAxis2(){
        
        return new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
    }


    public Vector3 UniversePosition(){

        Stars stars = GetComponentInParent<Stars>();
        List<Vector3> starPositions = stars.GetTargetPositions();
        Vector3 focusStarPosition = starPositions[galaxyStarIndex];
        focusStarPosition += transform.parent.position*Universe.GALAXY_SCALE;
        
        //stars.GetComponent<MeshRenderer>().sharedMaterial.SetVector("_FocusStarPosition", focusStarPosition);

        /*Shader.SetGlobalColor("_StarColor", stars.targetColors[focusStarIndex]);
        Shader.SetGlobalVector("_FocusStarPosition", focusStarPosition);
        Shader.SetGlobalVector("_FocusStarOffset", offset);*/

        return focusStarPosition;
    }

}
