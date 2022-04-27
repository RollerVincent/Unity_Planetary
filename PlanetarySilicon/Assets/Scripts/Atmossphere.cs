using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atmossphere : MonoBehaviour
{

    
    public bool inside;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float playerDist = (GameObject.Find("PlayerObjects").GetComponent<PlayerObjects>().activePlayerObject.transform.position - transform.position).magnitude;
        

        if(!inside && playerDist < transform.localScale.x/2){
            OnEnter();
            inside = true;
        }
        if(inside && playerDist >= transform.localScale.x/2){
            OnExit();
            inside = false;
        }
    }

    public bool isInside(GameObject target){
        float dist = (GameObject.Find("PlayerObjects").GetComponent<PlayerObjects>().activePlayerObject.transform.position - transform.position).magnitude;
        if(dist < transform.localScale.x/2){
            return true;
        }
        return false;
    }

    public void OnEnter(){

        

        MaterialEqualizer playerEqualizer = GameObject.Find("PlayerObjects").GetComponent<MaterialEqualizer>();
        MaterialEqualizer planetEqualizer = transform.parent.gameObject.GetComponent<MaterialEqualizer>();

        playerEqualizer.ambient = planetEqualizer.ambient;
        playerEqualizer.ambientSaturation = planetEqualizer.ambientSaturation;
        playerEqualizer.absorbanceExponent = planetEqualizer.absorbanceExponent;
        playerEqualizer.absorbanceOffset = planetEqualizer.absorbanceOffset;
        playerEqualizer.diffraction = planetEqualizer.diffraction;


        playerEqualizer.equalizeNormalBlend = false;
        playerEqualizer.equalizeFaceNoise = false;
        playerEqualizer.equalizeAmbient = true;
        playerEqualizer.equalizeAbsorbacne = true;
        playerEqualizer.equalizeDiffration = true;
        playerEqualizer.Equalize();
        playerEqualizer.equalizeAmbient = false;
        playerEqualizer.equalizeAbsorbacne = false;
        playerEqualizer.equalizeDiffration = false;



    }
    public void OnExit(){

        MaterialEqualizer playerEqualizer = GameObject.Find("PlayerObjects").GetComponent<MaterialEqualizer>();

        playerEqualizer.ambient = 0;
        playerEqualizer.ambientSaturation = 0;
        playerEqualizer.absorbanceExponent = 0;
        playerEqualizer.absorbanceOffset = 0;
        playerEqualizer.diffraction = new Vector4(0,0,0,0);


        playerEqualizer.equalizeAmbient = true;
        playerEqualizer.equalizeAbsorbacne = true;
        playerEqualizer.equalizeDiffration = true;
        playerEqualizer.Equalize();
        playerEqualizer.equalizeAmbient = false;
        playerEqualizer.equalizeAbsorbacne = false;
        playerEqualizer.equalizeDiffration = false;
    }
}
