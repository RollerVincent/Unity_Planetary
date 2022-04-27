using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Beamer : MonoBehaviour
{

    public float height;
    public float expansion;
    public float expansionSpeed;
    public float placeHeight;
    public bool expand;
    public bool collapse;
    public bool place;
    public float collapseDelay;
    public float spawnDelay;

    float currentExpansion;
    bool isExpanding;
    bool isCollapsing;

    public GameObject playerObject;
    public PlayerSeat playerSeat;
    public bool beamOut;
    public bool beamIn;
    public float beamHeightExtension;


    // Start is called before the first frame update
    void Start()
    {
        currentExpansion = expansion;
    }


    // Update is called once per frame
    void Update()
    {

        if(beamOut){
            beamOut = false;
            StartCoroutine(BeamOut());
            
            
        }

        if(beamIn){
            beamIn = false;
            StartCoroutine(BeamIn());
            
            
        }

        if(place){
            PlacePlayer();
            place = false;
        }

        if(expand){
            isExpanding = true;
            isCollapsing = false;
            expand = false;
        }

        if(collapse){
            isCollapsing = true;
            isExpanding = false;
            collapse = false;
        }


        if(isExpanding){
            Expand();
        }

        if(isCollapsing){
            Collapse();
        }

        Transform child = transform.GetChild(0);

        child.localScale = new Vector3(currentExpansion, currentExpansion, height);
    }

    void Expand(){
        if(currentExpansion < expansion){
            currentExpansion += expansionSpeed;
        }else{
            currentExpansion = expansion;
        }

    }

    void Collapse(){
        if(currentExpansion > 0){
            currentExpansion -= expansionSpeed;
        }else{
            currentExpansion = 0;
        }

    }

    void PlacePlayer(){

        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, -transform.up, 50f);

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];

            if(hit.collider.gameObject.tag != "Rocket"){
                Vector3 placePosition = hit.point;
                playerObject.transform.position = placePosition + transform.up*placeHeight;
        
            }

        }


        /*RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 50f))
        {
            Vector3 placePosition = hit.point;
            playerObject.transform.position = placePosition + transform.up*placeHeight;
        }*/
    }

    IEnumerator BeamOut(){
        expand = true;
        playerSeat.sitObject.GetComponent<StrutManager>().target = beamHeightExtension;
        yield return new WaitForSeconds(spawnDelay);

        playerSeat.leave = true;
        GameObject.Find("PlayerObjects").GetComponent<PlayerObjects>().activePlayerObject = playerSeat.playerObject.gameObject;
        yield return new WaitForSeconds(0.1f);
        GameObject.Find("CameraManager").GetComponent<CameraManager>().EnablePlayerSurfaceViewMode();
        PlacePlayer();
        yield return new WaitForSeconds(collapseDelay);

        


        collapse = true;


    }

    IEnumerator BeamIn(){
        expand = true;
        playerSeat.sitObject.GetComponent<StrutManager>().target = 1;
        yield return new WaitForSeconds(spawnDelay);


        playerSeat.sit = true;
        GameObject.Find("PlayerObjects").GetComponent<PlayerObjects>().activePlayerObject = playerSeat.sitObject.gameObject;
        GameObject.Find("CameraManager").GetComponent<CameraManager>().EnablePlayerSurfaceViewMode();
        yield return new WaitForSeconds(collapseDelay);


        collapse = true;


    }

    public void ToggleSitting(Image image){
        if(playerSeat.isSitting){
            StartCoroutine(BeamOut());
            
            image.color = GlobalSettings.UiColor;
        }else{
            StartCoroutine(BeamIn());
            
            image.color = GlobalSettings.UiActiveColor;
        }
    }
}
