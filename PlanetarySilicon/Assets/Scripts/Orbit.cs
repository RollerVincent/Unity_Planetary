using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Orbit : MonoBehaviour
{

    public static float SPEED = 1;
    public GameObject orbitObject;
    public float amount;

    public Vector3 axis;
    public bool drawLine;


    float centerDistance;
    LineRenderer line;


   

    void OnDrawGizmosSelected()
    {
            
                Gizmos.color = new Color(1.0f, 1.0f, 0.0f, 1f);
                Gizmos.DrawLine(transform.position-axis*10000, transform.position+axis*10000);
            

            
           
       
                
                int vertexCount = 100;
                
             

                Vector3[] vertices = new Vector3[vertexCount];
                Vector3 startPos = orbitObject.transform.position;
                Vector3 diff = startPos - transform.position;
           
               /* for(int i=0;i<vertexCount;i++){
                    vertices[i] = Quaternion.AngleAxis(360f/vertexCount*i, axis) * diff; //diff.RotateAround(transform.position, axis, 360f/vertexCount);
                    Gizmos.DrawSphere(vertices[i], 100f);
                }*/

             
        
        


       
    }

   


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

        if(drawLine){

                GameObject newLine = new GameObject("Line");
                newLine.transform.parent = transform.parent.gameObject.GetComponentInChildren<OrbitTraceManager>().transform;
                newLine.transform.position = transform.position;

                newLine.AddComponent<OrbitLine>().parent = transform;
                
                LineRenderer lr = newLine.AddComponent<LineRenderer>();
                lr.material = GlobalSettings.LineMaterial;
                lr.useWorldSpace = false;
                //lr.material.SetFloat("_MainColor", orbitObject.GetComponent<PlanetInstance>().planetType.typeColor);
                
                PlanetInstance pi = orbitObject.GetComponent<PlanetInstance>();
                
                int vertexCount = 100;
                
                lr.SetWidth(40, 40);
                lr.startColor = pi.planetType.typeColor;
                lr.endColor = new Color(1,1,1,1);
                lr.SetVertexCount(vertexCount+1);

                Vector3[] vertices = new Vector3[vertexCount+1];
                Vector3 startPos = orbitObject.transform.position;
                Vector3 diff = startPos - transform.position;

                


                for(int i=0;i<vertexCount+1;i++){
                    vertices[i] = Quaternion.AngleAxis(360f/vertexCount*i, axis) * diff; //diff.RotateAround(transform.position, axis, 360f/vertexCount);
                }

                lr.SetPositions(vertices);


                drawLine = false;
        }






       /* if(drawLine){

            LineRenderer lr = GetComponent<LineRenderer>();

            if(true || lr == null){
                lr = gameObject.AddComponent<LineRenderer>();

                lr.material = GlobalSettings.LineMaterial;
                
                PlanetInstance pi = orbitObject.GetComponent<PlanetInstance>();
                
                int vertexCount = 100;
                
                lr.SetWidth(pi.radius, pi.radius);
                lr.startColor = pi.planetType.typeColor;
                lr.endColor = new Color(1,1,1,1);
                lr.SetVertexCount(vertexCount+1);

                Vector3[] vertices = new Vector3[vertexCount+1];
                Vector3 startPos = orbitObject.transform.position;
                Vector3 diff = startPos - transform.position;

                


                for(int i=0;i<vertexCount+1;i++){
                    vertices[i] = Quaternion.AngleAxis(360f/vertexCount*i, axis) * diff; //diff.RotateAround(transform.position, axis, 360f/vertexCount);
                }

                lr.SetPositions(vertices);
            }

        }else{
            /*LineRenderer lr = GetComponent<LineRenderer>();

            if(lr != null){
                DestroyImmediate(lr);
            }
        }

      /*  if(drawLine){

            if(line == null){
                GameObject newLine = new GameObject("Line");
                newLine.transform.parent = GameObject.Find("OrbitLines").transform;
                
                LineRenderer lr = newLine.AddComponent<LineRenderer>();
                lr.material = GlobalSettings.LineMaterial;
                //lr.material.SetFloat("_MainColor", orbitObject.GetComponent<PlanetInstance>().planetType.typeColor);
                
                PlanetInstance pi = orbitObject.GetComponent<PlanetInstance>();
                
                int vertexCount = 100;
                
                lr.SetWidth(pi.radius, pi.radius);
                lr.startColor = pi.planetType.typeColor;
                lr.endColor = new Color(1,1,1,1);
                lr.SetVertexCount(vertexCount+1);

                Vector3[] vertices = new Vector3[vertexCount+1];
                Vector3 startPos = orbitObject.transform.position;
                Vector3 diff = startPos - transform.position;

                


                for(int i=0;i<vertexCount+1;i++){
                    vertices[i] = Quaternion.AngleAxis(360f/vertexCount*i, axis) * diff; //diff.RotateAround(transform.position, axis, 360f/vertexCount);
                }

                lr.SetPositions(vertices);

                line = lr;
            }
        }*/
       


        
     /*   Vector3 diff = (transform.position-centerObject.transform.position).normalized;
        Quaternion rotation = Quaternion.Euler(axis.x*amount*SPEED, axis.y*amount*SPEED, axis.z*amount*SPEED);
        diff = rotation * diff;//.RotateAround(centerObject.transform.position, axis, amount*SPEED);
        //transform.RotateAround(centerObject.transform.position, axis, amount*SPEED);
        transform.position = centerObject.transform.position + diff * centerDistance;
        //GetComponent<MaterialCenterer>().center = true;*/
    }
}
