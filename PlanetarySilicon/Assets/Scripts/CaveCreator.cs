using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

[ExecuteInEditMode]
public class CaveCreator : MonoBehaviour
{
    public Material visMaterial;
    public Texture2D noiseTexture;
    public Color visColor;
    public float noiseDetail;
    public float noiseCutoff;
    public float meanHeight;
    public float heightScale;
    public bool activate;
    bool isactive = false;
    public bool apply;

    HashSet<string> appliedChunks;
    HashSet<string> remainingChunks;
    

    // Update is called once per frame
    void Update()
    {
        if(activate){


            foreach(Transform child in transform.GetChild(0)){
                child.GetComponent<MeshCollider>().enabled = false;
                child.GetComponent<MeshRenderer>().sharedMaterial = visMaterial;
            }
            isactive = true;


            activate=false;
        }

        if(apply){

            ApplyRecursive();


            apply=false;
        }

        if(isactive){
            visMaterial.SetTexture("_MainTex", noiseTexture);
            visMaterial.SetColor("_Color", visColor);
            visMaterial.SetFloat("_Detail", noiseDetail);
            visMaterial.SetFloat("_Cutoff", noiseCutoff);
            visMaterial.SetFloat("_MeanHeight", meanHeight);
            visMaterial.SetFloat("_HeightScale", heightScale);
        }
        
    }

   
    

    void ApplyRecursive(){
        appliedChunks = new HashSet<string>();
        remainingChunks = new HashSet<string>();

        MarchingSurfaceManager msm = GetComponent<MarchingSurfaceManager>();
        foreach(KeyValuePair<string, SurfaceData> entry in msm.SurfaceDataDict)
        {
            remainingChunks.Add(entry.Key);
        }

        
        while(remainingChunks.Count > 0){
            HashSet<string> tmp = new HashSet<string>();
            Debug.Log(remainingChunks.Count);
            foreach(string hash in remainingChunks){
                SurfaceData data = msm.SurfaceDataDict[hash];
                bool r = ApplyToChunk(data);
                if(r){

                    Vector3Int dir;
                    SurfaceData new_data;

                    dir = new Vector3Int(0,0,1);
                    new_data = newData(data.offset + dir*msm.chunkSize);
                    if(!msm.SurfaceDataDict.ContainsKey(new_data.hash)){
                        float s = 0;
                        for(int x=0; x<msm.chunkSize+1; x++){
                            for(int y=0; y<msm.chunkSize+1; y++){
                                int z = msm.chunkSize;
                                s += data.terrainMap[x,y,z];
                            }
                        }
                        if(s > 0 && s < Mathf.Pow(msm.chunkSize+1,2)){
                            tmp.Add(new_data.hash);
                            msm.SurfaceDataDict.Add(new_data.hash, new_data);
                        }           
                    }

                    dir = new Vector3Int(0,0,-1);
                    new_data = newData(data.offset + dir*msm.chunkSize);
                    if(!msm.SurfaceDataDict.ContainsKey(new_data.hash)){
                        float s = 0;
                        for(int x=0; x<msm.chunkSize+1; x++){
                            for(int y=0; y<msm.chunkSize+1; y++){
                                int z = 0;
                                s += data.terrainMap[x,y,z];
                            }
                        }
                        if(s > 0 && s < Mathf.Pow(msm.chunkSize+1,2)){
                            tmp.Add(new_data.hash);
                            msm.SurfaceDataDict.Add(new_data.hash, new_data);
                        }           
                    }




                    dir = new Vector3Int(1,0,0);
                    new_data = newData(data.offset + dir*msm.chunkSize);
                    if(!msm.SurfaceDataDict.ContainsKey(new_data.hash)){
                        float s = 0;
                        for(int z=0; z<msm.chunkSize+1; z++){
                            for(int y=0; y<msm.chunkSize+1; y++){
                                int x = msm.chunkSize;
                                s += data.terrainMap[x,y,z];
                            }
                        }
                        if(s > 0 && s < Mathf.Pow(msm.chunkSize+1,2)){
                            tmp.Add(new_data.hash);
                            msm.SurfaceDataDict.Add(new_data.hash, new_data);
                        }           
                    }

                    dir = new Vector3Int(-1,0,0);
                    new_data = newData(data.offset + dir*msm.chunkSize);
                    if(!msm.SurfaceDataDict.ContainsKey(new_data.hash)){
                        float s = 0;
                        for(int z=0; z<msm.chunkSize+1; z++){
                            for(int y=0; y<msm.chunkSize+1; y++){
                                int x = 0;
                                s += data.terrainMap[x,y,z];
                            }
                        }
                        if(s > 0 && s < Mathf.Pow(msm.chunkSize+1,2)){
                            tmp.Add(new_data.hash);
                            msm.SurfaceDataDict.Add(new_data.hash, new_data);
                        }           
                    }





                    dir = new Vector3Int(0,1,0);
                    new_data = newData(data.offset + dir*msm.chunkSize);
                    if(!msm.SurfaceDataDict.ContainsKey(new_data.hash)){
                        float s = 0;
                        for(int z=0; z<msm.chunkSize+1; z++){
                            for(int x=0; x<msm.chunkSize+1; x++){
                                int y = msm.chunkSize;
                                s += data.terrainMap[x,y,z];
                            }
                        }
                        if(s > 0 && s < Mathf.Pow(msm.chunkSize+1,2)){
                            tmp.Add(new_data.hash);
                            msm.SurfaceDataDict.Add(new_data.hash, new_data);
                        }           
                    }


                    dir = new Vector3Int(0,-1,0);
                    new_data = newData(data.offset + dir*msm.chunkSize);
                    if(!msm.SurfaceDataDict.ContainsKey(new_data.hash)){
                        float s = 0;
                        for(int z=0; z<msm.chunkSize+1; z++){
                            for(int x=0; x<msm.chunkSize+1; x++){
                                int y = 0;
                                s += data.terrainMap[x,y,z];
                            }
                        }
                        if(s > 0 && s < Mathf.Pow(msm.chunkSize+1,2)){
                            tmp.Add(new_data.hash);
                            msm.SurfaceDataDict.Add(new_data.hash, new_data);
                        }           
                    }

                  

                    /*if(data.terrainMap[msm.chunkSize/2, msm.chunkSize/2, msm.chunkSize] < 1){
                        SurfaceData new_data = newData(data.offset + new Vector3Int(0,0,1)*msm.chunkSize);
                        if(!msm.SurfaceDataDict.ContainsKey(new_data.hash)){
                            tmp.Add(new_data.hash);
                            msm.SurfaceDataDict.Add(new_data.hash, new_data);
                        }
                    }
                    if(data.terrainMap[msm.chunkSize, msm.chunkSize/2, msm.chunkSize/2] < 1){
                        SurfaceData new_data = newData(data.offset + new Vector3Int(1,0,0)*msm.chunkSize);
                        if(!msm.SurfaceDataDict.ContainsKey(new_data.hash)){
                            tmp.Add(new_data.hash);
                            msm.SurfaceDataDict.Add(new_data.hash, new_data);
                        }
                    }
                    if(data.terrainMap[msm.chunkSize/2, msm.chunkSize, msm.chunkSize/2] < 1){
                        SurfaceData new_data = newData(data.offset + new Vector3Int(0,1,0)*msm.chunkSize);
                        if(!msm.SurfaceDataDict.ContainsKey(new_data.hash)){
                            tmp.Add(new_data.hash);
                            msm.SurfaceDataDict.Add(new_data.hash, new_data);
                        }
                    }
                    if(data.terrainMap[msm.chunkSize/2, msm.chunkSize/2, 0] < 1){
                        SurfaceData new_data = newData(data.offset + new Vector3Int(0,0,-1)*msm.chunkSize);
                        if(!msm.SurfaceDataDict.ContainsKey(new_data.hash)){
                            tmp.Add(new_data.hash);
                            msm.SurfaceDataDict.Add(new_data.hash, new_data);
                        }
                    }
                    if(data.terrainMap[0, msm.chunkSize/2, msm.chunkSize/2] < 1){
                        SurfaceData new_data = newData(data.offset + new Vector3Int(-1,0,0)*msm.chunkSize);
                        if(!msm.SurfaceDataDict.ContainsKey(new_data.hash)){
                            tmp.Add(new_data.hash);
                            msm.SurfaceDataDict.Add(new_data.hash, new_data);
                        }
                    }
                    if(data.terrainMap[msm.chunkSize/2, 0, msm.chunkSize/2] < 1){
                        SurfaceData new_data = newData(data.offset + new Vector3Int(0,-1,0)*msm.chunkSize);
                        if(!msm.SurfaceDataDict.ContainsKey(new_data.hash)){
                            tmp.Add(new_data.hash);
                            msm.SurfaceDataDict.Add(new_data.hash, new_data);
                        }
                    }*/
                }
            }

            remainingChunks = tmp;
        }
    
    }

    SurfaceData newData(Vector3Int offset){
        MarchingSurfaceManager msm = GetComponent<MarchingSurfaceManager>();
        SurfaceData data = new SurfaceData();

        Vector3 worldCenter = transform.position + offset + new Vector3(msm.chunkSize/2, msm.chunkSize/2, msm.chunkSize/2);


        RaycastHit hit;
        Vector3 dir = (transform.position-worldCenter).normalized;
            
        
        data.vertices = new List<Vector3>();
        data.triangles = new int[0];
        data.offset = offset;
        data.hash = msm.gridHash(offset);
        data.terrainMap = new float[msm.chunkSize+1,msm.chunkSize+1,msm.chunkSize+1];



     /*   for(int x=0; x<msm.chunkSize+1; x++){
                for(int y=0; y<msm.chunkSize+1; y++){
                    for(int z=0; z<msm.chunkSize+1; z++){
                        
                        if (Physics.Raycast(transform.position+offset+new Vector3(x,y,z), dir, out hit)){
                            data.terrainMap[x,y,z] = 1;
                        }else{
                            data.terrainMap[x,y,z] = 0;
                        }


                    }
                }
        }*/

       
        return data;
    }

    bool ApplyToChunk(SurfaceData data){
            MarchingSurfaceManager msm = GetComponent<MarchingSurfaceManager>();
            checkLoadVolData(data);

            for(int x=0; x<msm.chunkSize+1; x++){
                for(int y=0; y<msm.chunkSize+1; y++){
                    for(int z=0; z<msm.chunkSize+1; z++){

                        float a = sample(data.offset + new Vector3(x,y,z));
                        
                        data.terrainMap[x,y,z] = Mathf.Max(data.terrainMap[x,y,z], a);
                    }
                }
            }

            marchData(data);

            return true;
    }


    void Apply(){
        MarchingSurfaceManager msm = GetComponent<MarchingSurfaceManager>();
        foreach(KeyValuePair<string, SurfaceData> entry in msm.SurfaceDataDict)
        {
            SurfaceData data = entry.Value;
            checkLoadVolData(data);
            for(int x=0; x<msm.chunkSize+1; x++){
                for(int y=0; y<msm.chunkSize+1; y++){
                    for(int z=0; z<msm.chunkSize+1; z++){

                        data.terrainMap[x,y,z] = Mathf.Max(data.terrainMap[x,y,z], sample(data.offset + new Vector3(x,y,z)));

                    }
                

                }


            }
            marchData(data);
        }
    }

    public void checkLoadVolData(SurfaceData data){
        MarchingSurfaceManager surfaceManager = GetComponent<MarchingSurfaceManager>();
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

    void marchData(SurfaceData data){
        MarchingSurfaceManager msm = GetComponent<MarchingSurfaceManager>();


        Marching marching = msm.gameObject.GetComponent<Marching>();

        marching.offsetPosition = data.offset;
        marching.terrainMap = data.terrainMap;        

        marching.CreateMeshData();



        data.vertices = new List<Vector3>();
        for(int j=0; j<marching.vertices.Count; j++){
            data.vertices.Add(marching.vertices[j] + marching.offsetPosition);
        }
        data.triangles = marching.triangles.ToArray();

        //msm.transform.GetChild(2).GetComponent<ResourceColoring>().RecolorData(surf_data);



    }

    public float sample(Vector3 objectPos){
        float sqHeight = objectPos.sqrMagnitude;
		float heightDistance = sqHeight - (meanHeight*meanHeight);
		float heightFactor = 1 + Mathf.Max(heightDistance, -heightDistance)/heightScale;

        objectPos = objectPos/noiseDetail;
        objectPos = new Vector3(Mathf.Max(objectPos.x, -objectPos.x), Mathf.Max(objectPos.y, -objectPos.y), Mathf.Max(objectPos.z, -objectPos.z));

        float dx = objectPos.x%1.0f;
        float ax = (objectPos.x - dx)%2;
        ax = (ax-0.5f)*2;
        if(ax<0){
            ax = dx;
                 
        }else{
            ax = 1-dx;
                
                
        }

        float dy = objectPos.y%1.0f;
        float ay = (objectPos.y - dy)%2;
        ay = (ay-0.5f)*2;
        if(ay<0){
            ay = dy;
                 
        }else{
            ay = 1-dy;
                
                
        }
      

        float dz = objectPos.z%1.0f;
        float az = (objectPos.z - dz)%2;
        az = (az-0.5f)*2;
        if(az<0){
            az = dz;
                 
        }else{
            az = 1-dz;
                
                
        }

        objectPos = new Vector3(ax,ay,az);		

		float xy = noiseTexture.GetPixel((int)(objectPos.x*noiseTexture.width), (int)(objectPos.y*noiseTexture.height)).r;
		float yz = noiseTexture.GetPixel((int)(objectPos.y*noiseTexture.width), (int)(objectPos.z*noiseTexture.height)).r;
		float xz = noiseTexture.GetPixel((int)(objectPos.x*noiseTexture.width), (int)(objectPos.z*noiseTexture.height)).r;

		float noise = (xz+yz+xy)/3;

		noise = Mathf.Max(noise-noiseCutoff, 0)/(1-noiseCutoff);
		noise = noise / heightFactor;
        
        if(noise<0.1f){
            noise = 0;
        }else{
            noise -= 0.1f;
            noise = Mathf.Min(1, noise*20);
        }

        return noise;
    }






















}
