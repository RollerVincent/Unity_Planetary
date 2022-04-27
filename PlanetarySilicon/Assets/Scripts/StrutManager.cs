using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class StrutManager : MonoBehaviour
{
    public List<Transform> struts;

    
    public float currentFootDistance;
    public float minFootDistance;
    public float maxFootDistance;
    [Range(0f, 3f)]
    public float target;
    [Range(0, 0.02f)]
    public float step;

    List<float> startDistances;

    bool isExpanded;

    // Start is called before the first frame update
    void Start()
    {
        startDistances = new List<float>();
        currentFootDistance = minFootDistance;
        
    }
    // Update is called once per frame
    void Update()
    {


        float targetFootDistance = (minFootDistance + (maxFootDistance-minFootDistance)*target);

        float d = targetFootDistance - currentFootDistance;
        float m = Mathf.Sqrt(d*d);
        if(m>=step){
            if(d<0){
                currentFootDistance -= step;
            }else{
                currentFootDistance += step;
            }
        }else{
            currentFootDistance = targetFootDistance;
        }



        int c = 0;
        foreach(Transform strut in struts){
            Transform pole = strut.GetChild(1);
            Transform foot = strut.GetChild(0);
            Transform footAnchor = strut.GetChild(2);

            Vector3 diff = pole.position - foot.position;
            Vector3 targetDir = diff.normalized;
            float dist = diff.magnitude;

            if(startDistances.Count < struts.Count){
                startDistances.Add(dist);
            }

            pole.forward = targetDir;
            
            foot.GetComponent<SpringJoint>().connectedAnchor = new Vector3(0,0,3-currentFootDistance);


            float distanceFactor = dist/startDistances[c];

            pole.localScale = new Vector3(pole.localScale.x, pole.localScale.y, distanceFactor);

            //footAnchor.localPosition = new Vector3(footAnchor.localPosition.x, footAnchor.localPosition.y, -currentFootDistance);





            c += 1;




            /*
            MeshFilter mf = pole.GetComponent<MeshFilter>();

            Mesh mesh = mf.sharedMesh;
            Vector3[] vertices = mesh.vertices;
            Debug.Log(mesh.vertices.Length);
        

            Vector3 center = Vector3.zero;
            for(int i=mesh.vertices.Length/2;i<mesh.vertices.Length;i++){
                center += mesh.vertices[i];
            }
            center = center/mesh.vertices.Length/2;


            Transform foot = strut.GetChild(0);
            Vector3 target = pole.TransformPoint(foot.position);

            Vector3 diff = target - center;

            for(int i=mesh.vertices.Length/2;i<mesh.vertices.Length;i++){
                vertices[i] += diff;
            }

            mesh.vertices = vertices;
            mf.sharedMesh = mesh;
            */
        }
    }

    public void Toggle(Image image){
        if(isExpanded){
            isExpanded = false;

            target = 0;
            
            image.color = GlobalSettings.UiColor;
        }else{
            isExpanded = true;

            target = 1;
            
            image.color = GlobalSettings.UiActiveColor;
        }
    }
}
