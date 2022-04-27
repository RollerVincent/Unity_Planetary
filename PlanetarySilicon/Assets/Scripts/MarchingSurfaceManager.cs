using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

[ExecuteInEditMode]
public class MarchingSurfaceManager : MonoBehaviour
{

    public int chunkSize = 16;
    public int DividableSize = 20;
    public bool generateNewSurface = false;
    public bool updateColoring = false;
    public bool saveSurface = false;
    public bool loadSurface = false;
    
    public bool generateNewMesh = false;
    public bool isDividing = false;
    public bool startDividing = false;
    public bool stopDividing = false;

    public Material material;
    public Material debugMaterial;


    public HashSet<string> availableDividableHashes = new HashSet<string>();
    bool dividableHashesLoaded = false;

    public HashSet<string> loadedChunkHashes = new HashSet<string>();
    public HashSet<string> savedChunkHashes = new HashSet<string>();

    public Dictionary<string, float> castedPositions = new Dictionary<string, float>();

    public Dictionary<string, SurfaceData> SurfaceDataDict = new Dictionary<string, SurfaceData>();
    public Dictionary<Vector3Int, Dividable> dividables;
    public Dictionary<Vector3Int, List<GameObject>> dividableObjects;

    HashSet<Vector3Int> activeDividables = new HashSet<Vector3Int>();

    public Transform divisionTarget;
    Vector3Int currentTargetOffset = new Vector3Int(100000,100000,100000);

    
    public Dictionary<Vector3Int, SurfaceData> modifiableSurfaceData = new Dictionary<Vector3Int, SurfaceData>();
    public Dictionary<Vector3Int, GameObject> modifiableSurfaceObjects = new Dictionary<Vector3Int, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        loadedChunkHashes = new HashSet<string>();
        
        Marching marching = GetComponent<Marching>();
        marching.width = chunkSize;
        marching.height = chunkSize;
        
    }

    // Update is called once per frame
    void Update()
    {
        if(generateNewSurface){
            GenerateSurfaceData();
            generateNewSurface = false;
        }
        if(generateNewMesh){
            //GenerateSurfaceMeshes();
            GenerateDividables();
            generateNewMesh = false;
        }
        if(saveSurface){
            SaveSurfaceData();
            saveSurface = false;
        }
        if(updateColoring){
            UpdateColoring();
            updateColoring = false;
        }
        if(loadSurface){
            LoadSurfaceData();
            loadSurface = false;
        }
        if(startDividing){
            isDividing = true;
            startDividing = false;
            DivideAtTarget();
        }
        if(stopDividing){
            isDividing = false;
            stopDividing = false;
            ClearDividing();
            
        }
        if(isDividing){
            checkForTargetChange();
        }
    }

    void UpdateColoring(){
        ResourceColoring rc = transform.GetChild(2).GetComponent<ResourceColoring>();
        foreach(KeyValuePair<string, SurfaceData> entry in SurfaceDataDict)
        {
            SurfaceData data = entry.Value;
            rc.RecolorData(data);
            
        }
        UvMapper uvm = transform.GetChild(1).GetComponent<UvMapper>();
        foreach(KeyValuePair<Vector3Int, Dividable> entry in dividables)
        {
            Dividable div = entry.Value;
            uvm.RecolorDividable(div);
            
        }
    }
   

    void SaveSurfaceData(){
        // 1

        SurfaceManagerSaveObject save_manager = new SurfaceManagerSaveObject();


        int count = SurfaceDataDict.Count;
        save_manager.hashes = new string[count];

        save_manager.offsetX = new int[count];
        save_manager.offsetY = new int[count];
        save_manager.offsetZ = new int[count];


        int c = 0;
        BinaryFormatter bf;
        FileStream file;
        foreach(KeyValuePair<string, SurfaceData> entry in SurfaceDataDict)
        {

            save_manager.hashes[c] = entry.Key;

            SurfaceData data = entry.Value;
            int vertexCount = data.vertices.Count;

            float [] verticesX = new float[vertexCount];
            float [] verticesY = new float[vertexCount];
            float [] verticesZ = new float[vertexCount];

            float [] colorsR = new float[vertexCount];
            float [] colorsG = new float[vertexCount];
            float [] colorsB = new float[vertexCount];
            float [] colorsA = new float[vertexCount];
            
            float [] uvsX = new float[vertexCount];
            float [] uvsY = new float[vertexCount];


            VolumeSaveObject vol_save_data = new VolumeSaveObject();


            for(int i=0; i<vertexCount; i++){
                verticesX[i] = data.vertices[i].x;
                verticesY[i] = data.vertices[i].y;
                verticesZ[i] = data.vertices[i].z;  
            }
            for(int i=0; i<data.colors.Length; i++){ 
                colorsR[i] = data.colors[i].r;
                colorsG[i] = data.colors[i].g;
                colorsB[i] = data.colors[i].b;
                colorsA[i] = data.colors[i].a;
            }
            for(int i=0; i<data.uvs.Length; i++){
                uvsX[i] = data.uvs[i].x;
                uvsY[i] = data.uvs[i].y;
            }

            save_manager.verticesX.Add(verticesX);
            save_manager.verticesY.Add(verticesY);
            save_manager.verticesZ.Add(verticesZ);

            save_manager.colorsR.Add(colorsR);
            save_manager.colorsG.Add(colorsG);
            save_manager.colorsB.Add(colorsB);
            save_manager.colorsA.Add(colorsA);
            
            save_manager.uvsX.Add(uvsX);
            save_manager.uvsY.Add(uvsY);


            save_manager.triangles.Add(data.triangles);
            save_manager.offsetX[c] = data.offset.x;
            save_manager.offsetY[c] = data.offset.y;
            save_manager.offsetZ[c] = data.offset.z;

            if(data.terrainMap != null){
                vol_save_data.terrainMap = data.terrainMap;

                bf = new BinaryFormatter();
                file = File.Create(Application.persistentDataPath + "/"+transform.gameObject.name+"_"+entry.Key+"_vol.save");
                bf.Serialize(file, vol_save_data);
                file.Close();
            }

            

            c += 1;
        }

		bf = new BinaryFormatter();
		file = File.Create(Application.persistentDataPath + "/"+transform.gameObject.name+"_manager.save");
		bf.Serialize(file, save_manager);
		file.Close();


    }

    bool checkForTargetChange(){

        Vector3 targetPos = divisionTarget.position;

        Vector3 tmp = (targetPos-transform.position) / (chunkSize);
        Vector3Int pos = new Vector3Int(Mathf.FloorToInt(tmp.x), Mathf.FloorToInt(tmp.y), Mathf.FloorToInt(tmp.z)*chunkSize);

        if(currentTargetOffset != pos){
            currentTargetOffset = pos;
            DivideAtTarget();
            return true;
        }
        return false;
    }

  

    public void LoadSurfaceData(){

        if (File.Exists(Application.persistentDataPath + "/"+transform.gameObject.name+"_manager.save"))
		{

            SurfaceDataDict = new Dictionary<string, SurfaceData>();
			
			// 2
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/"+transform.gameObject.name+"_manager.save", FileMode.Open);
			SurfaceManagerSaveObject save_manager = (SurfaceManagerSaveObject)bf.Deserialize(file);
			file.Close();

            string[] hashes = save_manager.hashes;

            int c = 0;

            for(int i=0; i<hashes.Length; i++)
            {
                string hash = hashes[i];
                //bf = new BinaryFormatter();
			    //file = File.Open(Application.persistentDataPath + "/"+transform.gameObject.name+"_"+hash+"_surf.save", FileMode.Open);

                //SurfaceSaveObject save_data = (SurfaceSaveObject)bf.Deserialize(file);
			    //file.Close();

                SurfaceData data = new SurfaceData();
                data.vertices = new List<Vector3>();
                data.colors = new Color[save_manager.verticesX[i].Length];
                data.uvs = new Vector2[save_manager.verticesX[i].Length];
                data.triangles = save_manager.triangles[i];
                data.offset = new Vector3Int(save_manager.offsetX[i], save_manager.offsetY[i], save_manager.offsetZ[i]);
                data.hash = hash;

                for(int j=0; j<save_manager.verticesX[i].Length; j++){

                    data.vertices.Add(new Vector3(save_manager.verticesX[i][j], save_manager.verticesY[i][j], save_manager.verticesZ[i][j]));
                    data.colors[j] = new Color(save_manager.colorsR[i][j], save_manager.colorsG[i][j], save_manager.colorsB[i][j], save_manager.colorsA[i][j]);
                    data.uvs[j] = new Vector2(save_manager.uvsX[i][j], save_manager.uvsY[i][j]);
                }

  
                SurfaceDataDict.Add(hash, data);
                c += 1;

            }


			
			Debug.Log("Loaded " + c + " surface data chunks");
			
		}
		else
		{
			Debug.Log("NO DATA SAVED");
		}


    }

    void ClearDividing(){
       
        foreach(Vector3Int pos in activeDividables){
            if(dividables.ContainsKey(pos)){
                Dividable div = dividables[pos];

                DestroyImmediate(div.obj);
                
                GameObject obj = GenerateDividableFromList(div.datalist, pos);
                obj.name = "Dividable_"+pos.x+"_"+pos.z+"_"+pos.z;
                obj.transform.parent = transform.GetChild(1);
                div.obj = obj;
                div.isSplitted = false;
            }
            
            
            
        }
        activeDividables = new HashSet<Vector3Int>();
    }


    public Vector3Int World2Offset(Vector3 worldPos){




        Vector3 localPos = (worldPos-transform.position)/chunkSize;
        //Vector3 scaledPos = new Vector3(Mathf.Round(localPos.x), Mathf.Round(localPos.y), Mathf.Round(localPos.z))/chunkSize;
        Vector3Int gridPos = new Vector3Int(Mathf.FloorToInt(localPos.x), Mathf.FloorToInt(localPos.y), Mathf.FloorToInt(localPos.z)) * chunkSize;
        return gridPos;
    }

    public string gridHash(Vector3Int gridPos){
        return gameObject.name+"_"+gridPos.x+"_"+gridPos.y+"_"+gridPos.z;
    }

    public void GenerateSurfaceData(){
        loadedChunkHashes = new HashSet<string>();
        castedPositions = new Dictionary<string, float>();
        SurfaceDataDict = new Dictionary<string, SurfaceData>();

        Marching marching = GetComponent<Marching>();
        marching.width = chunkSize;
        marching.height = chunkSize;

        marching.Generate();

        List<Transform> t = new List<Transform>();
        foreach (Transform child in transform.GetChild(0)) {
            t.Add(child);
        }

        foreach (Transform child in t) {
            GameObject obj = child.gameObject;
            Mesh mesh = obj.GetComponent<MeshFilter>().sharedMesh;
            Vector3[] vertices = mesh.vertices;
            for(int i=0; i<vertices.Length; i++){
                Vector3 worldPosition = transform.position + vertices[i];
                
                for(int x=-1; x<2; x++){
                    for(int y=-1; y<2; y++){
                        for(int z=-1; z<2; z++){


                            Vector3Int offsetPos = World2Offset(worldPosition + new Vector3(x,y,z));
                            
                            
                            string hash = gridHash(offsetPos);
                            if(!loadedChunkHashes.Contains(hash)){

                                marching.offsetPosition = offsetPos;

                                marching.terrainMap = new float[chunkSize+1,chunkSize+1,chunkSize+1];

                                marching.createByRayCast(transform.position, castedPositions);

                                marching.CreateMeshData();

                                SurfaceData surf_data = new SurfaceData();


                                surf_data.vertices = new List<Vector3>();
                                for(int j=0; j<marching.vertices.Count; j++){
                                    surf_data.vertices.Add(marching.vertices[j] + marching.offsetPosition);
                                }

                                surf_data.offset = offsetPos;
                                surf_data.terrainMap = marching.terrainMap;

                                surf_data.triangles = marching.triangles.ToArray();

                                surf_data.hash = hash;

                                SurfaceDataDict.Add(hash, surf_data);



                            

                                loadedChunkHashes.Add(hash);
                            }
                        }
                    }
                }

            }
        }
        castedPositions = new Dictionary<string, float>();
        //transform.GetChild(1).GetComponent<UvMapper>().Recolor();
        Debug.Log("Done");

        
    }

    public void GenerateSurfaceMeshes(){

        int meshIndex = 1;

        GameObject SurfaceMesh = new GameObject("SurfaceMesh_"+meshIndex);
        MeshFilter mf = SurfaceMesh.AddComponent<MeshFilter>();
        SurfaceMesh.AddComponent<MeshRenderer>().sharedMaterial = material;
        SurfaceMesh.AddComponent<MeshCollider>();
        SurfaceMesh.transform.position = transform.position;
        SurfaceMesh.transform.parent = transform.GetChild(1);
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        int c = 0;

        Mesh mesh = null;

        foreach(KeyValuePair<string, SurfaceData> entry in SurfaceDataDict)
        {

            int l = entry.Value.vertices.Count;
            if(c + l >= 60000){

                mesh = new Mesh();
                mesh.SetVertices(vertices);
                mesh.SetTriangles(triangles, 0);
                mesh.RecalculateNormals();
                mf.sharedMesh = mesh;
                
                meshIndex += 1;
                SurfaceMesh = new GameObject("SurfaceMesh_"+meshIndex);
                mf = SurfaceMesh.AddComponent<MeshFilter>();
                SurfaceMesh.AddComponent<MeshRenderer>().sharedMaterial = material;
                SurfaceMesh.AddComponent<MeshCollider>();
                SurfaceMesh.transform.position = transform.position;
                SurfaceMesh.transform.parent = transform.GetChild(1);
                vertices = new List<Vector3>();
                triangles = new List<int>();
                c = 0;

            }
            vertices.AddRange(entry.Value.vertices);
            for(int i=0; i< entry.Value.triangles.Length; i++){
                triangles.Add(entry.Value.triangles[i]+c);
            }
                
            c += l;
            


            
        }

        mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        mf.sharedMesh = mesh;

        transform.GetChild(1).GetComponent<UvMapper>().Recolor();


    }
    


    public GameObject GenerateDividableFromList(List<SurfaceData> datalist, Vector3Int gridPos){


        GameObject dividable = new GameObject("Dividable_"+gridPos.x+"_"+gridPos.y+"_"+gridPos.z);
        MeshFilter mf = dividable.AddComponent<MeshFilter>();
        dividable.AddComponent<MeshRenderer>().sharedMaterial = material;
        MeshCollider mc = dividable.AddComponent<MeshCollider>();
        dividable.transform.position = transform.position;
        dividable.transform.parent = transform.GetChild(1);
        dividable.gameObject.tag = "Terrain";
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();
        List<Color> colors = new List<Color>();
        int c = 0;

        Mesh mesh = null;

        foreach(SurfaceData data in datalist)
        {

            int l = data.vertices.Count;
            vertices.AddRange(data.vertices);
            for(int i=0; i< data.triangles.Length; i++){
                triangles.Add(data.triangles[i]+c);
            }
            for(int i=0; i< data.colors.Length; i++){
                colors.Add(data.colors[i]);
            }
            for(int i=0; i< data.colors.Length; i++){
                uvs.Add(data.uvs[i]);
            }


                
            c += l;
            


            
        }
        //Debug.Log(vertices.Count);
        //Debug.Log(colors.Count);
        //Debug.Log("");

        mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uvs);
        mesh.SetColors(colors);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        //mesh.RecalculateBounds();
        mf.sharedMesh = mesh;
        mc.sharedMesh = mesh;


        return dividable;




    }

    public void DivideAtTarget(){
        
        Vector3 targetPos = divisionTarget.position;

        modifiableSurfaceData = new Dictionary<Vector3Int, SurfaceData>();
        modifiableSurfaceObjects = new Dictionary<Vector3Int, GameObject>();
        

        Vector3 tmp = (targetPos-transform.position) / (chunkSize*DividableSize);
        Vector3Int gridPos = new Vector3Int(Mathf.FloorToInt(tmp.x), Mathf.FloorToInt(tmp.y), Mathf.FloorToInt(tmp.z));

        activeDividables.Add(gridPos);



        for(int x=-1; x<2; x++){
            for(int y=-1; y<2; y++){
                for(int z=-1; z<2; z++){
                    if(!(x==0 && y==0 && z==0)){
                        Vector3Int p = new Vector3Int(x,y,z);
                        tmp = (targetPos-transform.position+p*chunkSize) / (chunkSize*DividableSize);
                        Vector3Int pos = new Vector3Int(Mathf.FloorToInt(tmp.x), Mathf.FloorToInt(tmp.y), Mathf.FloorToInt(tmp.z));
                        if(pos != gridPos){
                            activeDividables.Add(pos);
                        }
                    }
                    

                }
            }
        }

       /* Vector3 tmp = (targetPos-transform.position) / (chunkSize*DividableSize);
        Vector3Int gridPos = new Vector3Int(Mathf.RoundToInt(tmp.x), Mathf.RoundToInt(tmp.y), Mathf.RoundToInt(tmp.z));



        activeDividables.Add(gridPos);*/




        List<Vector3Int> divs = new List<Vector3Int>();
        foreach(Vector3Int pos in activeDividables){
            divs.Add(pos);
        }
        foreach(Vector3Int div in divs){
            if(dividables.ContainsKey(div)){
                Divide(div, targetPos);
            }
            
        }

        //Divide(gridPos, targetPos);



    }

    void Divide(Vector3Int gridPos, Vector3 targetPos){

        Dividable dividable = dividables[gridPos];

        



        if(!dividable.isSplitted){
                GameObject.DestroyImmediate(dividable.obj);
                GameObject newObj = new GameObject("Dividable_"+gridPos.x+"_"+gridPos.y+"_"+gridPos.z);
                newObj.transform.parent = transform.GetChild(1);
                dividable.obj = newObj;
                dividable.isSplitted = true;
        }else{

            List<GameObject> objs = new List<GameObject>();
            int c = dividable.obj.transform.childCount;
            for(int i = 0; i<c; i++){
                objs.Add(dividable.obj.transform.GetChild(i).gameObject);
            }
            foreach(GameObject g in objs){
                GameObject.DestroyImmediate(g);
            }

        }
        
        
        List<SurfaceData> datalist = dividable.datalist;

        List<SurfaceData> tmp = new List<SurfaceData>();

        Vector3 ttmp = (targetPos-transform.position) / (chunkSize);
        Vector3Int pos = new Vector3Int(Mathf.FloorToInt(ttmp.x), Mathf.FloorToInt(ttmp.y), Mathf.FloorToInt(ttmp.z))*chunkSize;

        foreach(SurfaceData data in datalist)
        {

            

            //Vector3 worldPos = data.offset + transform.position;
            Vector3Int diff = (data.offset - pos)/chunkSize;


            


            if((diff.x==1 || diff.x == -1 || diff.x == 0) && (diff.y==1 || diff.y == -1 || diff.y == 0) && (diff.z==1 || diff.z == -1 || diff.z == 0)){
                

                List<SurfaceData> dl = new List<SurfaceData>();
                dl.Add(data);

                GameObject obj = GenerateDividableFromList(dl, gridPos);
                obj.name = "ActiveDividable";
                obj.GetComponent<MeshRenderer>().sharedMaterial = debugMaterial;
                obj.transform.parent = dividable.obj.transform;
                //hei
                //transform.GetChild(2).GetComponent<ResourceColoring>().Recolor(obj);


                modifiableSurfaceData.Add(data.offset, data);
                modifiableSurfaceObjects.Add(data.offset, obj);


            }else{
                tmp.Add(data);
            }


        }

        GameObject divObj = GenerateDividableFromList(tmp, gridPos);
        //transform.GetChild(2).GetComponent<ResourceColoring>().Recolor(divObj);

        if(tmp.Count == datalist.Count){
            // dividable not affected
            GameObject.DestroyImmediate(dividable.obj);
            divObj.transform.parent = transform.GetChild(1);
            dividable.obj = divObj;
            dividable.isSplitted = false;
            activeDividables.Remove(gridPos);
        }else{
            divObj.name = "SurroundingDividable";
            divObj.transform.parent = dividable.obj.transform;

        }



        


    }



    public void GenerateDividables(){

        dividables = new Dictionary<Vector3Int, Dividable>();
        activeDividables = new HashSet<Vector3Int>();

        List<GameObject> objs = new List<GameObject>();
        int c = transform.GetChild(1).childCount;
        for(int i = 0; i<c; i++){
            objs.Add(transform.GetChild(1).GetChild(i).gameObject);
        }
        foreach(GameObject g in objs){
            GameObject.DestroyImmediate(g);
        }


        foreach(KeyValuePair<string, SurfaceData> entry in SurfaceDataDict)
        {
            SurfaceData data = entry.Value;
            Vector3 pos = data.offset;
            Vector3Int gridPos = new Vector3Int(Mathf.FloorToInt(1f * pos.x/DividableSize/chunkSize), Mathf.FloorToInt(1f* pos.y/DividableSize/chunkSize), Mathf.FloorToInt(1f* pos.z/DividableSize/chunkSize));



            if(!dividables.ContainsKey(gridPos)){
                dividables.Add(gridPos, new Dividable());
                
            }
            
            dividables[gridPos].datalist.Add(data);
        }

        foreach(KeyValuePair<Vector3Int, Dividable> entry in dividables)
        {
            entry.Value.gridPos = entry.Key;
            GameObject obj = GenerateDividableFromList(entry.Value.datalist, entry.Key);
            

            entry.Value.obj = obj;


        }





    }

}
