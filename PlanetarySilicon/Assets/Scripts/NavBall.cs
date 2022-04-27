using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavBall : MonoBehaviour
{

    public float figureOffset;
    Vector3 localPosition;


    void OnDrawGizmosSelected()
    {
            

        Gizmos.color = new Color(1.0f, 1.0f, 1.0f, 1f);
        


        GameObject player = PlayerObjects.GetActive();
        GameObject planet = player.GetComponent<AttractedObject>().closest_gravityObject;

        if(planet != null){

            transform.eulerAngles = planet.transform.eulerAngles;
            Gizmos.DrawLine(transform.position, transform.position + transform.up * 1000);
            Gizmos.DrawLine(planet.transform.position, planet.transform.position + planet.transform.up * 1000);
            
           
        }


       
            

       
    }

    // Start is called before the first frame update
    void Start()
    {
        localPosition = transform.localPosition;
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = localPosition;

        GameObject player = PlayerObjects.GetActive();
        GameObject planet = player.GetComponent<AttractedObject>().closest_gravityObject;

        if(planet != null){

            Transform playerFigure = transform.GetChild(0);
            Transform sunFigure = transform.GetChild(1);

            transform.eulerAngles = planet.transform.eulerAngles;
            float tmpDist = (playerFigure.position-transform.position).magnitude;


            Vector3 dir = (Universe.ACTIVE_SYSTEM.gameObject.transform.GetChild(0).position - planet.transform.position).normalized;
            sunFigure.position = transform.position + dir * tmpDist;






            
            
            dir = (player.transform.position - planet.transform.position).normalized;
            playerFigure.position = transform.position + dir * tmpDist;
            playerFigure.eulerAngles = player.transform.eulerAngles;


            Vector3 targetUp = transform.parent.up;
            Vector3 currentUp = dir;

            Vector3 axis = Vector3.Cross(targetUp, currentUp);
            float angle = Vector3.Angle(targetUp, currentUp);

            transform.RotateAround(transform.position, axis, -angle);


            Vector3 finalNavSunDir = (sunFigure.position-transform.position).normalized;
            Shader.SetGlobalVector("_NavSunDir", finalNavSunDir);


        }

        
        
    }
}
