using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Planet : MonoBehaviour
{
    public int ChunkSize = 32;
    public int ChunksPerFace = 4;
    public int SurfaceChunkWidth = 4;
    public int Seed = 10000;
    public Material material;
    public int radius = 60;
    public int innerRadius = 30;
    public float gravity = 1;
    public bool refresh = false;
    public bool init = false;
    public bool test = true;
    public bool save = false;
    public bool load = false;
    int limit;

    Vector3Int pointer = new Vector3Int(0,0,0);
    
    public Marching[,,] chunks;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void seekChunk(){
            if(pointer.z < limit-1){
                pointer.z += 1;
            }else{
                if(pointer.y < limit-1){
                    pointer.y += 1;
                    pointer.z = 0;
                }else{
                    if(pointer.x < limit-1){
                        pointer.x += 1;
                        pointer.z = 0;
                        pointer.y = 0;
                    }else{
                        pointer = new Vector3Int(0,0,0);
                        refresh = false;
                    }

                }
                
            }
    }

    // Update is called once per frame
    void Update()
    {
        if(refresh){
            
            foreach (Marching marching in GetComponentsInChildren<Marching>()){
                marching.Refresh();
                if(marching.gameObject.tag == "terrain"){
                    
                }
            }
            
          /*  for (int x = 0; x < ChunksPerFace; x++)
            {
                for (int y = 0; y < ChunksPerFace; y++)
                {
                    for (int z = 0; z < ChunksPerFace; z++)
                    {

                        if(chunks[x,y,z] != null){
                            chunks[x,y,z].Refresh();
                        } 
                    }
                }
            }*/
            refresh = false;
            Debug.Log("Refreshed");


          /*  while(chunks[pointer.x,pointer.y,pointer.z] == null && refresh){
                seekChunk();
                
            }
            if(refresh){
                chunks[pointer.x,pointer.y,pointer.z].Refresh();
            }
            seekChunk();*/

            
            

            //RefreshChunk();

            

            
        }
        if(init){
            foreach (Transform child in transform) {
                GameObject.DestroyImmediate(child.gameObject);
            }
            InitChunkList();
            
            init = false;
        }
        if(save){
            SaveChunks();
            save = false;
        }
        if(load){
            LoadChunks();
            load = false;
        }
    }

    public void ApplyModifier(SurfaceModifier mod){
        for (int x = 0; x < limit; x++)
        {
            for (int y = 0; y < limit; y++)
            {
                for (int z = 0; z < limit; z++)
                {

                    if(chunks[x,y,z] != null){
                        chunks[x,y,z].ApplyModifier(mod);
                    } 
                }
            }
        }
    }

    public void RefreshChunk(){
        

        if(chunks[pointer.x,pointer.y,pointer.z] != null){
            chunks[pointer.x,pointer.y,pointer.z].Refresh();
            Debug.Log(pointer);
        } 
        
    }

    public void SaveChunks(){
        

        foreach (Marching marching in GetComponentsInChildren<Marching>()){
            marching.SaveData();
        }
            
    }

    public void LoadChunks(){
        

        foreach (Marching marching in GetComponentsInChildren<Marching>()){
            marching.LoadData();
        }
            
    }

    void InitChunkList(){
        int skip = 0;
        int all = 0;
        chunks = new Marching[ChunksPerFace,ChunksPerFace,ChunksPerFace];
        limit = ChunksPerFace;
        if(test){
            limit = limit/2;
        }
        for (int x = 0; x < limit; x++)
        {
            for (int y = 0; y < limit; y++)
            {
                for (int z = 0; z < limit; z++)
                {
                    if((ChunksPerFace/2f-x-0.5f)*(ChunksPerFace/2f-x-0.5f) + (ChunksPerFace/2f-y-0.5f)*(ChunksPerFace/2f-y-0.5f) + (ChunksPerFace/2f-z-0.5f)*(ChunksPerFace/2f-z-0.5f) <= (ChunksPerFace/2f+1)*(ChunksPerFace/2f+1)){
                        if((ChunksPerFace/2f-x-0.5f)*(ChunksPerFace/2f-x-0.5f) + (ChunksPerFace/2f-y-0.5f)*(ChunksPerFace/2f-y-0.5f) + (ChunksPerFace/2f-z-0.5f)*(ChunksPerFace/2f-z-0.5f) >= (ChunksPerFace/2f+1-SurfaceChunkWidth)*(ChunksPerFace/2f+1-SurfaceChunkWidth)){
                            GameObject chunk = new GameObject("Chunk_" + x + "_" + y + "_" + z);
                            chunk.transform.parent = gameObject.transform;
                            chunk.transform.position = transform.position + new Vector3(x*ChunkSize, y*ChunkSize, z*ChunkSize) - Vector3.one*ChunksPerFace/2f*ChunkSize;

                            chunk.AddComponent<MeshFilter>();
                            chunk.AddComponent<MeshCollider>();
                            chunk.AddComponent<MeshRenderer>().sharedMaterial = material;
                            Marching marching = chunk.AddComponent<Marching>();
                            marching.width = ChunkSize;
                            marching.height = ChunkSize;
                            marching.GridPosition = new Vector3Int(x,y,z);
                            marching.WorldPosition = marching.GridPosition - new Vector3Int(ChunksPerFace/2,ChunksPerFace/2,ChunksPerFace/2);
                            marching.planet = this;
                            marching.Generate();
                            chunks[x,y,z] = marching;
                            all += 1;
                        }
                        else{
                            skip += 1;
                        }
                    }
                    else{
                        skip += 1;
                    }
                    
                    



                }
            }
        }
        Debug.Log(skip);
        Debug.Log(all);
    }
    
}
