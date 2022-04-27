using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class OrbitTraceManager : MonoBehaviour
{

    public bool showAll;
    public bool hideAll;

    public bool showing = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(showAll){
            ShowAll();
            showAll = false;
        }
        if(hideAll){
            HideAll();
            hideAll = false;
        }

        if(showing){
            //HideAll();
            //ShowAll();
            foreach(OrbitLine orbitLine in transform.GetComponentsInChildren<OrbitLine>()){
                orbitLine.transform.position = orbitLine.parent.position;
            }
        }
        


    }

    public void ShowAll(){
        foreach(Orbit orbit in transform.parent.GetComponentsInChildren<Orbit>()){
            orbit.drawLine = true;
        }
        showing = true;
    }

    public void HideAll(){
        List<GameObject> tmp = new List<GameObject>();
        for(int i=0;i<transform.childCount;i++){
            tmp.Add(transform.GetChild(i).gameObject);
        }
        foreach(GameObject child in tmp){
            DestroyImmediate(child);
        }
        showing = false;
    }
}
