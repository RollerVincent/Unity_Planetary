using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    public bool menuActive;
    public GameObject lastSelected;
    public GameObject pivotObject;
    public Camera camera;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(lastSelected == null){
            lastSelected = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;;
        }

        Transform child;
        Vector3 pos;
        RectTransform rt;
        
        int children = transform.childCount-1;
        for(int i=0; i<children; i++){
            child = transform.GetChild(i+1);
            pos = new Vector3(100000,0,0);
            if(Input.GetKey(KeyCode.X)){
                pos = new Vector3(1*GlobalSettings.MenuExpansion,0,0);
                float angle = i * 360f/children;
                pos = Quaternion.AngleAxis(angle, new Vector3(0,0,1)) * pos;

                
            }
                
            rt = child.GetComponent<RectTransform>();
            rt.anchoredPosition = pos;
        }

        child = transform.GetChild(0);
        pos = new Vector3(100000,0,0);
        if(Input.GetKey(KeyCode.X)){
            pos = new Vector3(0,0,0);
        }
                
        rt = child.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;




        







     /*   PointerEventData m_PointerEventData = new PointerEventData(UnityEngine.EventSystems.EventSystem.current);
        m_PointerEventData.position = pivotObject.transform.position;
 
        List<RaycastResult> results = new List<RaycastResult>();
 
        GetComponent<GraphicRaycaster>().Raycast(m_PointerEventData, results);

        Debug.Log(results.Count);
 
        if(results.Count > 0) Debug.Log("Hit " + results[0].gameObject.name);*/

        /*GameObject hitObject = null;
        RaycastHit hit;
        Vector3 dir = (pivotObject.transform.position-camera.transform.position).normalized;
        if (Physics.Raycast(pivotObject.transform.position-dir*10f, dir, out hit, 100f))
        {
            Debug.Log("fufuff");
            hitObject = hit.collider.gameObject;
        }

        

        if(hitObject != null){    
            Image image = hitObject.GetComponent<Image>();
            if(image!=null){
                image.color = new Color(1,0,0,1);
            }
        }*/
    }
}
