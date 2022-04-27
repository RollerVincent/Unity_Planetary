using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{

    public Vector3 axis;
    public float spin;


    void OnDrawGizmosSelected()
    {

        
            Gizmos.color = new Color(1.0f, 0.0f, 1.0f, 1f);

                
            Gizmos.DrawLine(transform.position-axis*10000, transform.position+axis*10000);
           
        
        
                        
        
        


       
    }


    // Start is called before the first frame update
    void Start()
    {
    }

    public void applySpin(){
        transform.RotateAround(transform.position, axis, spin*GlobalSettings.OrbitSpeed*GlobalSettings.PlayBackSpeed);
    }

    public Vector3 applyToPoint(Vector3 point){
        Vector3 diff = point - transform.position;
        return (Quaternion.AngleAxis(spin*GlobalSettings.OrbitSpeed*GlobalSettings.PlayBackSpeed, axis) * diff) + transform.position;
    }

    
}
