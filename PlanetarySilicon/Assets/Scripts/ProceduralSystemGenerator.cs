using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ProceduralSystemGenerator : MonoBehaviour
{


    public List<PlanetType> planetTypes;
    public int minPlanetCount;
    public int maxPlanetCount;
    public float minPlanetDistance;
    public float maxPlanetDistance;
    public float AxisVarianceZ;
    public int galaxyStarIndex;
    public bool generateAtStarIndex;
    public bool clearAllSystems;

    Vector3[] currentMainAxes;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(generateAtStarIndex){
            GenerateAtStarIndex();
            generateAtStarIndex = false;
        }

        if(clearAllSystems){
            ClearAllSystems();
            clearAllSystems = false;
        }
    }

    public void ClearAllSystems(){
        int childCount = transform.childCount;
        List<GameObject> tmp = new List<GameObject>();
        for(int i=0; i<childCount; i++){
            //DestroyImmediate(transform.GetChild(i).gameObject);
            tmp.Add(transform.GetChild(i).gameObject);
        }
        foreach(GameObject child in tmp){
            DestroyImmediate(child);
        }


    }

    PlanetType getType(){
        List<int> typelist = new List<int>();
        int c = 0;
        foreach(PlanetType pt in planetTypes){
            for (int i=0; i<pt.occurency; i++){
                typelist.Add(c);
            }
            c += 1;
        }
        int ind = Random.Range(0, typelist.Count);
        Debug.Log(ind);
        return planetTypes[typelist[ind]];

    }

    GameObject CreatePlanet(Vector3 position, SystemManager parentSystem){
        PlanetType planetType = getType();
        GameObject newInstance = planetType.GetComponent<ProceduralPlanetBuilder>().CreateInstance(position, parentSystem);
        return newInstance;
    }


    Vector3[] orbitStartPositionAxis(SystemManager systemManager, float distance){
        
        Vector3 cross = Vector3.Cross(currentMainAxes[0], currentMainAxes[1]);
        Vector3 dir = (currentMainAxes[0] * Random.Range(-1.0f,1.0f) + currentMainAxes[1] * Random.Range(-1.0f,1.0f)).normalized;
        Vector3 startPos = (dir + cross*Random.Range(-AxisVarianceZ,AxisVarianceZ))*distance;
        
        Vector3 axis = Vector3.Cross(startPos.normalized, currentMainAxes[0]);

        axis = Quaternion.AngleAxis(Random.Range(-180.0f*AxisVarianceZ,180.0f*AxisVarianceZ), startPos.normalized) * axis;

        return new Vector3[]{startPos, axis};
    }

    public SystemManager GenerateAtStarIndex(){

        


        GameObject newSystemObj = new GameObject("System " + galaxyStarIndex);
        newSystemObj.transform.parent = transform;

        SystemManager systemManager = newSystemObj.AddComponent<SystemManager>();
        systemManager.galaxyStarIndex = galaxyStarIndex;

        

        GameObject newStarObj = new GameObject("Star " + galaxyStarIndex);
        newStarObj.transform.parent = newSystemObj.transform;
        newStarObj.AddComponent<GravityObject>().mass = 100000;


        GameObject newOrbitTraceManagerObj = new GameObject("OrbitTraceManager");
        newOrbitTraceManagerObj.transform.parent = newSystemObj.transform;
        newOrbitTraceManagerObj.AddComponent<OrbitTraceManager>();



        systemManager.centerObject = newStarObj;

        Vector3 up = systemManager.UniversePosition();
        Random.seed = (int)((up.x+up.y+up.z)/100000f);

        currentMainAxes = systemManager.GetMainAxes();
        
        int planetCount = Random.Range(minPlanetCount, maxPlanetCount);

        Vector3[,] orbitInfos = new Vector3[planetCount, 2];
        for(int i=0; i<planetCount;i++){
            Vector3[] orbitInfo = orbitStartPositionAxis(systemManager, minPlanetDistance + (maxPlanetDistance-minPlanetDistance)*Random.Range(0.0f, 1.0f));
            orbitInfos[i, 0] = orbitInfo[0];
            orbitInfos[i, 1] = orbitInfo[1];
        }

        for(int i=0; i<planetCount;i++){
            GameObject newInstance = CreatePlanet(orbitInfos[i,0], systemManager);
            Orbit orbit = newStarObj.AddComponent<Orbit>();
            orbit.axis = orbitInfos[i, 1];
            orbit.orbitObject = newInstance;
            orbit.amount = 1;

            Spin spin = newInstance.AddComponent<Spin>();
            spin.axis = (orbit.axis + new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f))*0.4f).normalized;
            spin.spin = 0.005f;
        }


        //systemManager.RecenterSystem();
        //systemManager.RecenterMaterials();

        newSystemObj.SetActive(false);

        return systemManager;


    }
}
