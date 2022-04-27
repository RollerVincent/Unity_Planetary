using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityVisualizer : MonoBehaviour
{
    float stepSize = 0.5f; // never change == 0.5f !!!!!
    public float topWidth;
    public float bottomWidth;
    
    public Color topColor;
    public Color bottomColor;

    public bool debug;
    public bool updateLine;

    TrajectoryInfo trajectoryInfo;

    float accumulation = 0;
    


     

    void UpdateLine(){

        
        trajectoryInfo = GameObject.Find("GravityManager").GetComponent<GravityManager>().getTrajectoryPositions(GetComponent<Rigidbody>(), GlobalSettings.TraceLength, stepSize);
        

        

        TrajectoryInfo ti = trajectoryInfo;

    


        LineRenderer lr = GetComponentInChildren<LineRenderer>();
        lr.gameObject.transform.position = ti.positions[0];
        //lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.SetColors(topColor, bottomColor);
        lr.SetWidth(topWidth, bottomWidth);

        lr.positionCount = ti.positions.Count;

        for(int i=0;i<ti.positions.Count;i++){
            lr.SetPosition(i, ti.positions[i]);
        }

        

        
    }


    


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(updateLine){
            UpdateLine();

            int i = 0;
            if(accumulation >= 1){
                accumulation -= 1;
                i=1;
            }

            

            transform.position = trajectoryInfo.positions[i];
            GetComponent<Rigidbody>().velocity = trajectoryInfo.velocities[i];
            

            int s = GlobalSettings.PlayBackSpeed;
            if(s==1){
                s -= 1;
            }
            
            accumulation += s*Time.deltaTime;
                
        }
    }
}
