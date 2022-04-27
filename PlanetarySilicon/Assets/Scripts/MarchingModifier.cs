using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;


[ExecuteInEditMode]
public class MarchingModifier : MonoBehaviour
{

    
    public MarchingSurfaceManager surfaceManager;

    public bool apply = false;

    public int radius = 2;
    public float amount = 0.1f;
    public bool reverse = false;
    public bool loadReady = false;

    public InventoryObject inventoryObject;

    public Dictionary<int, int> collectedResources = new Dictionary<int, int>();

    Vector3 targetOffestPos;
    

    void OnDrawGizmosSelected()
    {

        foreach(KeyValuePair<Vector3Int, SurfaceData> entry in surfaceManager.modifiableSurfaceData)
        {
            SurfaceData data = entry.Value;
            Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 1f);
            Gizmos.DrawSphere(surfaceManager.transform.position + data.offset, 2f);
        }
    }

    


    // Start is called before the first frame update
    void Start()
    {


        

        surfaceManager.gameObject.GetComponent<Marching>().Generate();
        surfaceManager.gameObject.GetComponent<Marching>().width = surfaceManager.chunkSize;
        surfaceManager.gameObject.GetComponent<Marching>().height = surfaceManager.chunkSize;
        

        //max resource types = 100 
        for (int i = 0; i < 100; i++)
        {
            collectedResources.Add(i, 0);
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {

        if(loadReady){
            surfaceManager.LoadSurfaceData();
            surfaceManager.GenerateDividables();
            loadReady=false;
        }

        if (apply){
            Apply();
            apply = false;
        }
        
    }

    public void checkLoadVolData(SurfaceData data){
        if(data.terrainMap == null){
            string path = Application.persistentDataPath + "/"+surfaceManager.gameObject.name+"_"+data.hash+"_vol.save";
            if (File.Exists(path))
            {

                    
                    
                    // 2
                    BinaryFormatter bf = new BinaryFormatter();
                    FileStream file = File.Open(path, FileMode.Open);
                    VolumeSaveObject vol_data = (VolumeSaveObject)bf.Deserialize(file);
                    file.Close();

                   

                    data.terrainMap = vol_data.terrainMap;
                    
                    
                    
            }
            else
            {
                Debug.Log("NO VOLUME DATA SAVED");
            }
            

        }
    }

    

    void ApplyToOffset(Vector3Int targetOffestPos, Vector3 targetPos){

        if(!surfaceManager.SurfaceDataDict.ContainsKey(surfaceManager.gridHash(targetOffestPos))){
            // create new chunk
            


            SurfaceData newData = new SurfaceData();
            newData.offset = targetOffestPos;
            newData.hash = surfaceManager.gridHash(targetOffestPos);
            newData.terrainMap = new float[surfaceManager.chunkSize+1, surfaceManager.chunkSize+1, surfaceManager.chunkSize+1];
            newData.vertices = new List<Vector3>();
            newData.triangles = new int[0];
            newData.colors = new Color[0];
            newData.uvs = new Vector2[0];


            Vector3Int divPos = new Vector3Int(Mathf.FloorToInt(1f*targetOffestPos.x/surfaceManager.DividableSize/surfaceManager.chunkSize), Mathf.FloorToInt(1f*targetOffestPos.y/surfaceManager.DividableSize/surfaceManager.chunkSize), Mathf.FloorToInt(1f*targetOffestPos.z/surfaceManager.DividableSize/surfaceManager.chunkSize));

            if(!surfaceManager.dividables.ContainsKey(divPos)){
                surfaceManager.dividables.Add(divPos, new Dividable());
                
                
                GameObject d = surfaceManager.GenerateDividableFromList(new List<SurfaceData>(), divPos);
                surfaceManager.dividables[divPos].obj = d;
                surfaceManager.dividables[divPos].gridPos = divPos;
            }



            Dividable dividable = surfaceManager.dividables[divPos];




           
            
            dividable.datalist.Add(newData);
            surfaceManager.SurfaceDataDict.Add(newData.hash, newData);

            surfaceManager.DivideAtTarget();
            
            //surfaceManager.modifiableSurfaceData.Add(targetOffestPos, newData);
            //surfaceManager.modifiableSurfaceObjects.Add(targetOffestPos, newObj);
        }
        
        
        
        GameObject obj = surfaceManager.modifiableSurfaceObjects[targetOffestPos];
        MeshFilter mf = obj.GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();

        
        SurfaceData surf_data = surfaceManager.modifiableSurfaceData[targetOffestPos];

        //check vol data
        checkLoadVolData(surf_data);


        Marching marching = surfaceManager.gameObject.GetComponent<Marching>();

        marching.offsetPosition = surf_data.offset;
        marching.terrainMap = surf_data.terrainMap;


        int r = -1;
        if(reverse){
            r = 1;
        }

        marching.PlaceTerrain(targetPos, radius, amount, r);


        

        marching.CreateMeshData();



        surf_data.vertices = new List<Vector3>();
        for(int j=0; j<marching.vertices.Count; j++){
            surf_data.vertices.Add(marching.vertices[j] + marching.offsetPosition);
        }
        surf_data.triangles = marching.triangles.ToArray();
        surf_data.terrainMap = marching.terrainMap;

        surfaceManager.transform.GetChild(2).GetComponent<ResourceColoring>().RecolorData(surf_data);


        if(surf_data.vertices.Count > 2){
            mesh.vertices = surf_data.vertices.ToArray();
            mesh.colors = surf_data.colors;
            //mesh.uv = surf_data.uvs;
            mesh.triangles = surf_data.triangles;
            mesh.RecalculateNormals();
            mf.sharedMesh = mesh;
            obj.GetComponent<MeshCollider>().sharedMesh = mesh;
            surf_data.uvs = surfaceManager.transform.GetChild(1).GetComponent<UvMapper>().RecolorObject(obj);

            //surfaceManager.transform.GetChild(2).GetComponent<ResourceColoring>().Recolor(obj);
        }
        

    }

    void Apply(){

        if(surfaceManager.isDividing == false){
            surfaceManager.isDividing = true;
            surfaceManager.DivideAtTarget();
        }

        surfaceManager.gameObject.GetComponent<Marching>().Generate();
        surfaceManager.gameObject.GetComponent<Marching>().width = surfaceManager.chunkSize;
        surfaceManager.gameObject.GetComponent<Marching>().height = surfaceManager.chunkSize;


        Vector3 targetPos = surfaceManager.divisionTarget.position;
        Vector3 ttmp = (targetPos-surfaceManager.transform.position) / (surfaceManager.chunkSize);
        Vector3Int targetOffestPos = new Vector3Int(Mathf.FloorToInt(ttmp.x), Mathf.FloorToInt(ttmp.y), Mathf.FloorToInt(ttmp.z))*surfaceManager.chunkSize;

        targetOffestPos = surfaceManager.World2Offset(targetPos);

        HashSet<Vector3Int> applied = new HashSet<Vector3Int>();

        ApplyToOffset(targetOffestPos, targetPos);

        applied.Add(targetOffestPos);


        for (int x = -1; x < 2; x++) {
			for (int y = -1; y < 2; y++) {
				for (int z = -1; z < 2; z++) {

                    Vector3 pos = targetPos + new Vector3(x,y,z)*(radius+1);
                    ttmp = (pos-surfaceManager.transform.position) / (surfaceManager.chunkSize);
                    Vector3Int offpos = new Vector3Int(Mathf.FloorToInt(ttmp.x), Mathf.FloorToInt(ttmp.y), Mathf.FloorToInt(ttmp.z))*surfaceManager.chunkSize;
                    if(offpos != targetOffestPos && !applied.Contains(offpos)){
                        ApplyToOffset(offpos, targetPos);
                        applied.Add(offpos);
                    }
                }
            }
        }

        Vector3 objectPos = targetPos-surfaceManager.transform.position;
        int resource = surfaceManager.gameObject.GetComponentInChildren<ResourceManager>().sampleObjectPos(objectPos);
        
        //inventoryObject.Add(resource, 1);



    }





   /* public void Apply(){
        RaycastHit hit;
        
        if (Physics.Raycast(emitter.transform.position, camera.transform.forward, out hit)){

            if(hit.transform.gameObject.tag == "Terrain"){
                Marching marching = surfaceManager.gameObject.GetComponent<Marching>();

                if(reverse){
                    marching.PlaceTerrain(hit.point, radius, amount, -1);
                }else{
                    int resource = marching.PlaceTerrain(hit.point, radius, amount, 1);
                    collectedResources[resource] = collectedResources[resource] + 1;
                    Debug.Log("Resource " + resource + " :    " + collectedResources[resource] + " collected");
                }

                

            }
            
        }
    }*/
}

/*
 marching.offsetPosition = gridPos*chunkSize;


                                marching.createByRayCast(transform.position, castedPositions);

                                marching.CreateMeshData();

                                SurfaceData surf_data = new SurfaceData();


                                surf_data.vertices = new List<Vector3>();
                                for(int j=0; j<marching.vertices.Count; j++){
                                    surf_data.vertices.Add(marching.vertices[j] + marching.offsetPosition);
                                }

                                surf_data.offset = marching.offsetPosition;

                                surf_data.triangles = marching.triangles.ToArray();

                                surf_data.hash = hash;

                                SurfaceDataDict.Add(hash, surf_data);
*/