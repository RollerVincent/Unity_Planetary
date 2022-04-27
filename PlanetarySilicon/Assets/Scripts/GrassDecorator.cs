using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]

public class GrassDecorator : MonoBehaviour
{

    public Material material;
    public int activeDistance;
    public GameObject targetObject;
    public int chunkSize;
    public int instances;
    public float abundance = 0.1f;

    public float width;
    public float height;
    
    public float maxHeight = 100;
    public float minHeight = 90;
    public int seed = 1;
    public bool debug = false;
    public bool place = false;

    List<Vector3> targetNormals;
    List<Vector3> targetPositions;

    Dictionary<Vector3Int, GameObject> chunks;
    HashSet<Vector3Int> activeChunks;

    Vector3Int currentTargetGridPos = new Vector3Int(-100000, -1000000, -100000);
    

    void OnDrawGizmosSelected()
    {

        if(debug){
            Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 1f);


            GetTargetPositions();
            
            for(int i=0; i<targetPositions.Count; i++){

                
            
                
                Gizmos.DrawSphere(targetPositions[i], 0.3f);
                Gizmos.DrawLine(targetPositions[i], targetPositions[i] + targetNormals[i] * 5);
            }
        }
        
                        
        
        


       
    }

    bool TargetGridPosChange(){

      

        Vector3Int gridPosition = gridPos(targetObject.transform.position);
        if(gridPosition != currentTargetGridPos){

            currentTargetGridPos = gridPosition;

            return true;



            
        }
        return false;
    }





    // Start is called before the first frame update
    void Start()
    {
        activeChunks = new HashSet<Vector3Int>();
        place = true;
    }

    void checkUpDate(){
        if(TargetGridPosChange()){
            
            foreach(Vector3Int gridPosition in activeChunks){
                chunks[gridPosition].SetActive(false);
            }

            activeChunks = new HashSet<Vector3Int>();
            for(int x=-activeDistance; x<activeDistance+1; x++){
                for(int y=-activeDistance; y<activeDistance+1; y++){
                    for(int z=-activeDistance; z<activeDistance+1; z++){

                        Vector3Int v = currentTargetGridPos + new Vector3Int(x,y,z);
                        if(chunks.ContainsKey(v)){
                            chunks[v].SetActive(true);
                            activeChunks.Add(v);
                        }



                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(place){
            currentTargetGridPos = new Vector3Int(-100000, -1000000, -100000);
            Place();
            Debug.Log(currentTargetGridPos);
            Debug.Log(gridPos(targetObject.transform.position));
            Debug.Log("");
            checkUpDate();
            place=false;
        }

        checkUpDate();
    }

    public void Place(){

        chunks = new Dictionary<Vector3Int, GameObject>();
        activeChunks = new HashSet<Vector3Int>();

        GameObject decoration = new GameObject(transform.childCount + " GrassDecoration");
        decoration.transform.parent = transform;


        GetTargetPositions();

        Dictionary<Vector3Int, List<Vector3>> chunkPositions = new Dictionary<Vector3Int, List<Vector3>>();
        Dictionary<Vector3Int, List<Vector3>> chunkNormals = new Dictionary<Vector3Int, List<Vector3>>();
        
        for(int i=0; i<targetPositions.Count; i++){

            Vector3Int gridPosition = gridPos(targetPositions[i]);

            if(!chunkPositions.ContainsKey(gridPosition)){
                chunkPositions.Add(gridPosition, new List<Vector3>());
                chunkNormals.Add(gridPosition, new List<Vector3>());
                
            }

            chunkPositions[gridPosition].Add(targetPositions[i]);
            chunkNormals[gridPosition].Add(targetNormals[i]);
           
          /*  GameObject newInstance = GameObject.Instantiate(instanceObject);

            newInstance.transform.position = pos[i];
            newInstance.transform.up = (pos[i]-transform.position).normalized;
            newInstance.transform.parent = decoration.transform;

            newInstance.transform.RotateAround(newInstance.transform.position, newInstance.transform.up, Random.Range(0.0f, 360.0f));*/

            
        }

        int c = 0;
        foreach(Vector3Int gridPosition in chunkPositions.Keys){
            GameObject newObject = new GameObject("GrassPatch"+c);
            c+=1;
            newObject.transform.parent = decoration.transform;

            chunks.Add(gridPosition, newObject);
            activeChunks.Add(gridPosition);

            MeshFilter mf = newObject.AddComponent<MeshFilter>();
            newObject.AddComponent<MeshRenderer>().sharedMaterial = material;

            List<Vector3> targetChunkPositions = chunkPositions[gridPosition];
            List<Vector3> targetChunkNormals = chunkNormals[gridPosition];


            Mesh mesh = new Mesh();
            Vector3[] vertices = new Vector3[targetChunkPositions.Count*3];
            int[] triangles = new int[targetChunkPositions.Count*6];
            Vector3[] normals = new Vector3[targetChunkPositions.Count*3];
            Vector2[] uvs = new Vector2[targetChunkPositions.Count*3];

            Debug.Log(targetChunkPositions.Count);

            for(int i=0; i<targetChunkPositions.Count; i++){

                Vector3 right = Vector3.Cross(targetChunkNormals[i], new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f))).normalized;

                vertices[i*3+0] = targetChunkPositions[i] - (targetChunkNormals[i]*0.5f) - right*width/2;
                vertices[i*3+1] = targetChunkPositions[i] - (targetChunkNormals[i]*0.5f) + right*width/2;// + right*width;
                vertices[i*3+2] = targetChunkPositions[i] - (targetChunkNormals[i]*0.5f);// + (targetChunkNormals[i])*(height+0.5f);

                normals[i*3+0] = targetChunkNormals[i];
                normals[i*3+1] = targetChunkNormals[i];
                normals[i*3+2] = targetChunkNormals[i];

                triangles[i*6+0] = i*3+0;
                triangles[i*6+1] = i*3+1;
                triangles[i*6+2] = i*3+2;

                triangles[i*6+3] = i*3+0;
                triangles[i*6+4] = i*3+2;
                triangles[i*6+5] = i*3+1;

              

                uvs[i*3+0] = new Vector2(0,0);
                uvs[i*3+1] = new Vector2(1,0);
                uvs[i*3+2] = new Vector2(0.5f,1);

            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.normals = normals;

            mesh.RecalculateBounds();

            mf.sharedMesh = mesh;
            
            //newObject.SetActive(false);

        }
    }

    Vector3Int gridPos(Vector3 worldPos){
        Vector3 tmp = new Vector3(worldPos.x, worldPos.y, worldPos.z);
        Vector3 objPos = tmp;
        return new Vector3Int(Mathf.FloorToInt(objPos.x/chunkSize), Mathf.FloorToInt(objPos.y/chunkSize), Mathf.FloorToInt(objPos.z/chunkSize));
    }


    void GetTargetPositions(){

        targetNormals = new List<Vector3>();
        targetPositions = new List<Vector3>();

        Random.seed = seed;


        for(int i=0; i<instances; i++){
            Vector3 dir = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
            RaycastHit hit;

            if (Physics.Raycast(transform.position - dir * maxHeight, dir, out hit, maxHeight-minHeight))
            {
                targetNormals.Add(hit.normal);
                targetPositions.Add(hit.point);
                //targetNormals.Add(hit.normal.normalized);
                
                
                //targetNormals.Add(Vector3.right);
                

                
            }

        }


        
    }

    void GetTargetPositions2(){

        Random.seed = (int)(seed*100 + transform.parent.gameObject.GetComponent<ProceduralPlanetBuilder>().Seed*100);

        targetNormals = new List<Vector3>();
        targetPositions = new List<Vector3>();

        for(int i=0; i<transform.GetChild(0).childCount; i++){
            GameObject child = transform.GetChild(0).GetChild(i).gameObject;
            Vector3[] vertices = child.GetComponent<MeshFilter>().sharedMesh.vertices;
            Vector3[] normals = child.GetComponent<MeshFilter>().sharedMesh.normals;

            for(int j=0; j<vertices.Length; j++){

                Vector3 worldPos = child.transform.TransformPoint(vertices[j]);

                float height = (worldPos - transform.position).magnitude;

                if(height > minHeight && height < maxHeight){

                    if(Random.Range(0.0f, 1.0f) < abundance){
                        targetPositions.Add(worldPos);
                        targetNormals.Add(normals[j]);
                    }

                }

            }
        }




        
    }
}
