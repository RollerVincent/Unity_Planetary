using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlanetInstance : MonoBehaviour
{
    public PlanetType planetType;
    public float radius;

    public bool decorate;

    bool loaded = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }



    // Update is called once per frame
    void Update()
    {
        if(decorate){

            
            
            Decorate();


            decorate = false;
        }
    }

    void Decorate(){

        foreach(GrassDecorator decorator in GetComponents<GrassDecorator>()){
            decorator.Place();
        }

        foreach(Decorator decorator in GetComponents<Decorator>()){
            decorator.Place();
        }

        GetComponent<RecenterMesh>().RecenterAll();
    }


    public void CheckLoad(){
        if(!loaded){

            Decorate();


            loaded = true;
        }
    }

    PlanetType getMoonType(){
        List<int> typelist = new List<int>();
        int c = 0;
        foreach(PlanetType pt in planetType.moonTypes){
            for (int i=0; i<pt.occurency; i++){
                typelist.Add(c);
            }
            c += 1;
        }
        int ind = Random.Range(0, typelist.Count);
        return planetType.moonTypes[typelist[ind]];

    }


    public void CreateMoons(){

        SystemManager systemManager = GetComponentInParent<SystemManager>();

        int moonCount = Random.Range(planetType.minMoonCount, planetType.maxMoonCount);


        for(int i=0; i<moonCount; i++){
            float distance = Random.Range(planetType.minMoonHeight, planetType.maxMoonHeight);
            Vector3 dir = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;

            Vector3 moonOffset = dir*distance;

            PlanetType moonType = getMoonType();
            GameObject newMoonInstance = moonType.GetComponent<ProceduralPlanetBuilder>().CreateInstance(transform.position + moonOffset, systemManager);



            Vector3 moonAxis = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
            moonAxis = Vector3.Cross(dir, moonAxis);

            Orbit orbit = gameObject.AddComponent<Orbit>();
            orbit.axis = moonAxis;
            orbit.orbitObject = newMoonInstance;
            orbit.amount = 1;



            //createMoonOrbit;



        }


    }

}
