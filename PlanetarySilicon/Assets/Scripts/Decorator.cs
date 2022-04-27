using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Decorator : MonoBehaviour
{
    public GameObject instanceObject;
    public float abundance = 0.1f;
    public float maxHeight = 100;
    public float minHeight = 90;
    public float scaleLimit = 0.5f;
    public float seed = 1;
    public float existenceProbability = 0.5f;
    public bool debug = false;
    public bool place = false;

    

    void OnDrawGizmosSelected()
    {
        if(debug){
            Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 1f);
            List<Vector3> pos = GetTargetPositions();
            for(int i=0; i<pos.Count; i++){
                Gizmos.DrawSphere(pos[i], 0.3f);
            }
        }
        
       
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(place){
            Place();
            place=false;
        }
    }

    public void Place(){


        GameObject decoration = new GameObject(instanceObject.name + " Decoration");
        decoration.transform.parent = transform;
        decoration.transform.position = transform.position;


        List<Vector3> pos = GetTargetPositions();
        
        for(int i=0; i<pos.Count; i++){

            

           
            GameObject newInstance = GameObject.Instantiate(instanceObject);

            newInstance.transform.position = pos[i];
            newInstance.transform.up = (pos[i]-transform.position).normalized;
            newInstance.transform.parent = decoration.transform;

            newInstance.transform.RotateAround(newInstance.transform.position, newInstance.transform.up, Random.Range(0.0f, 360.0f));

            float scale = Random.Range(scaleLimit, 1.0f);

            newInstance.transform.localScale *= scale;
            
        }
    }

    

 /*   List<Vector3> GetTargetPositions2(){

        Random.seed = (int)(seed*100 + transform.parent.gameObject.GetComponent<ProceduralPlanetBuilder>().Seed*100);

        List<Vector3> pos = new List<Vector3>();

        for(int i=0; i<instances; i++){
            Vector3 dir = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
            RaycastHit hit;

            if (Physics.Raycast(transform.position - dir * maxHeight, dir, out hit, maxHeight-minHeight))
            {
                pos.Add(hit.point);
                

                
            }

        }

        return pos;

        
    }*/

    List<Vector3> GetTargetPositions(){

        Random.seed = (int)(seed);

        List<Vector3> pos = new List<Vector3>();

        for(int i=0; i<transform.GetChild(0).childCount; i++){
            GameObject child = transform.GetChild(0).GetChild(i).gameObject;  // decorate planar surface
            Vector3[] vertices = child.GetComponent<MeshFilter>().sharedMesh.vertices;

            for(int j=0; j<vertices.Length; j++){

                Vector3 worldPos = child.transform.TransformPoint(vertices[j]);

                float height = (worldPos - transform.position).magnitude;

                if(height > minHeight && height < maxHeight){

                    if(Random.Range(0.0f, 1.0f) < abundance){
                        pos.Add(worldPos);
                    }

                }

            }
        }



        return pos;

        
    }

    

}
