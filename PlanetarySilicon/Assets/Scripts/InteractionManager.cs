using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class InteractionManager : MonoBehaviour
{   
    
    public Camera camera;
    public GameObject pivotObject;
    public float interactionRange;
    [Range(0f, 1f)]
    public float pivotSize;
    public float pivotOffset;
    public Sprite defaultCursor;
    public Sprite antiCursor; 
    public Color antiColor; 
    public Color placableSurfaceColor; 
    public float placableSurfaceAlpha;
    public float panSpeed;
    public bool panCursor;

    
    Vector3 dragcenter;
    
    Vector2 cursorDelta;
    Interactable draggingInteractable;
    bool dragging;
    public float currentPlacableSurfaceAlpha;

    bool interactionMode;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetCursor(Sprite cursor){
        pivotObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = cursor;
    }

    public void SetAntiCursor(){
        //pivotObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = antiCursor;
    }

    public void SetCursorColor(Color color){
        pivotObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = color;
    }

    public void SetAntiCursorColor(){
        pivotObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = antiColor;
    }

    

    // Update is called once per frame
    void Update()
    {
        Cursor.visible = false;

        if(Input.GetKey(KeyCode.Y)){
            interactionMode = true;
        }else{
            interactionMode = false;
        }




        Shader.SetGlobalColor("_PlaceableSurfaceColor", new Color(placableSurfaceColor.r, placableSurfaceColor.g, placableSurfaceColor.b, currentPlacableSurfaceAlpha));

        


        if(Input.GetKey(KeyCode.Y) || Input.GetKey(KeyCode.X)){
            panCursor = true;
            pivotObject.transform.position = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camera.nearClipPlane)) + camera.transform.forward*0.01f;
            pivotObject.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
        }else{

            panCursor = false;
            pivotObject.transform.localPosition = new Vector3(0, 0, pivotOffset);
            pivotObject.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        }

        GameObject hitObject = null;
        RaycastHit hit;
        if (Physics.Raycast(pivotObject.transform.position, (pivotObject.transform.position-camera.transform.position).normalized, out hit, interactionRange))
        {
            hitObject = hit.collider.gameObject;
        }

        

        if(interactionMode && (hitObject != null || dragging)){

            Interactable interactable;

            if(dragging){
                interactable = draggingInteractable;
            }else{
                interactable = hitObject.GetComponent<Interactable>();
            }

            if(interactable != null){
                pivotObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = interactable.cursor;


                if(Input.GetMouseButtonDown(0)){

                    dragcenter = Input.mousePosition;
                    
                }
                

                if(Input.GetMouseButton(0)){


                    
                    // pressed

                    if(Input.mousePosition != dragcenter){
                        if(interactable.dragging == false){
                            interactable.OnDragStart();
                        }
                        interactable.dragging = true;
                        dragging = true;
                        draggingInteractable = interactable;
                    }


                }else{

                    // hovering
                    interactable.OnHover();
                }

                if(Input.GetMouseButtonUp(0)){
                    if(!interactable.dragging){
                        interactable.OnClick();
                    }else{
                        interactable.OnDragEnd();
                    }
                    dragging = false; 
                    interactable.dragging = false;
                    draggingInteractable = null;
                    
                }

                if(dragging){
                    interactable.OnDrag();
                }





            }else{
                pivotObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = defaultCursor;
            }
            
            




        }else{
            pivotObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = defaultCursor;
        }

        
        
        
        
        
        pivotObject.transform.localScale = new Vector3(pivotSize, pivotSize, pivotSize);
        
        

        



        if(panCursor){
            cursorDelta += new Vector2(Input.GetAxis("Mouse X")*0.01f, Input.GetAxis("Mouse Y")*0.01f);
        }else{
            cursorDelta = Vector2.zero;
        }

        
    }
}
