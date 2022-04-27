using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class VitalParameter : MonoBehaviour
{

    [Range(0, 100)]
    public float fitness = 100;             //  0-100
    [Range(0, 200)]
    public float nutrition = 200;           //  0-200
    [Range(0, 100)]
    public float caloryLevel = 30;          //  0-100
    [Range(0, 100)]
    public float hydration = 40;            //  0-100
    [Range(32, 42)]
    public float bodyTemperature = 37;      //  32-42  ,  34-40
    [Range(0, 2)]
    public float bodyPressure = 1;          //  0-2  ,  0.2-1.8
    [Range(1, 2)]
    public float bodyRadiation = 1;         //  1-2
    [Range(0, 100)]
    public float injuries = 50;             //  0-100

    public float catabolism = 0.001f;
    public float anabolism = 0.01f;
    public float struggle = 0.01f;
    
    public float stomachProteinAmount = 0;
    public float stomachCaloryAmount = 0;
    public float stomachHydroAmount = 0;
    public float stomachMedecinAmount = 0;


    public bool updatePatrameter;
    public bool loadStarterPreset;


    private float nutritionDelayFactor = 2;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if(loadStarterPreset){
            LoadStarterPreset();
            loadStarterPreset = false;
        }
    
        
        if(updatePatrameter){
            UpdateParameter();

            Parameter[] parameters = GetComponents<Parameter>();
            int count = parameters.Length;
            for(int i=0;i<count;i++){

                if(parameters[i].name == "Fitness"){
                    parameters[i].current = fitness;
                }
                else if(parameters[i].name == "Nutrition"){
                    parameters[i].current = nutrition;
                }
                else if(parameters[i].name == "Calory Level"){
                    parameters[i].current = caloryLevel;

                }
                else if(parameters[i].name == "Hydration"){
                    parameters[i].current = hydration;

                }
                else if(parameters[i].name == "Body Temperature"){
                    parameters[i].current = bodyTemperature;

                }
                else if(parameters[i].name == "Body Pressure"){
                    parameters[i].current = bodyPressure;

                }
                else if(parameters[i].name == "Body Radiation"){
                    parameters[i].current = bodyRadiation;

                }
                else if(parameters[i].name == "Injuries"){
                    parameters[i].current = injuries;

                }
                

            }


        }


        

    }

    void LoadStarterPreset(){

        fitness = 50;                  //  0-100
        nutrition = 70;                //  0-200
        caloryLevel = 30;              //  0-100
        hydration = 40;                //  0-100
        bodyTemperature = 36;          //  32-42
        bodyPressure = 1;             //  0-2
        bodyRadiation = 1.3f;            //  1-2
        injuries = 100;                 //  0-100


    }

    void UpdateParameter(){
        /////////  CATABOLISM


        // more weight more calories used   0.5 - 1
        float caloryusage = nutrition/(200*nutritionDelayFactor) + 0.5f;

        

        // the further from 37 body temp the more calories used   *  1 - 2
        caloryusage *= (bodyTemperature-32)/5f;
        

        // calories are reduced hunger increases
        caloryLevel -= caloryusage * catabolism;


        // more weight more water used   0.5 - 1
        float hydroUsage = nutrition/(200*nutritionDelayFactor) + 0.5f;

        // the further from 37 body temp the more water used   *  1 - 2
        hydroUsage *= (bodyTemperature-32)/5f;

        hydration -= hydroUsage * catabolism * 1.25f;



        // if no calories loose weight
        if(caloryLevel < 0){
            nutrition += caloryLevel;
            caloryLevel = 0;

            
            
        }

        

       












        /////////  ANABOLISM


        // if stomach has protein --> strength gain
        if(stomachProteinAmount > 0){
            stomachProteinAmount -= anabolism;
            nutrition += anabolism;
        }

        // if stomach has calory --> calory gain
        if(stomachCaloryAmount > 0){
            stomachCaloryAmount -= anabolism;
            caloryLevel += anabolism;
        }

        // if stomach has sugar --> bloodsugar gain
        if(stomachHydroAmount > 0){
            stomachHydroAmount -= anabolism;
            hydration += anabolism;
        }

        // if stomach has medecin --> bloodsugar gain
        if(stomachMedecinAmount > 0){
            stomachMedecinAmount -= anabolism*50;
            if(injuries<100){
                fitness += anabolism*50;
            }
            injuries += anabolism*50;
            
            
        }

        stomachProteinAmount = Mathf.Max(0, stomachProteinAmount);
        stomachCaloryAmount = Mathf.Max(0, stomachCaloryAmount);
        stomachHydroAmount = Mathf.Max(0, stomachHydroAmount);
        stomachMedecinAmount = Mathf.Max(0, stomachMedecinAmount);




        // if calories and water full gain fitness
        if(caloryLevel > 100){
            if(hydration > 100){
                fitness += anabolism;//(caloryLevel-100)*anabolism;
                hydration = 100;
            }
            caloryLevel = 100;

            // if fitness full gain strength
            if(fitness > 100){
                nutrition += anabolism;//(fitness-100)*anabolism;
                fitness = 100;
            }
        }

       







        /////////  STRUGGLE



        // too warm
        if(bodyTemperature < 33){
            fitness -= struggle;
        }

        // too cold
        if(bodyTemperature > 41){
            fitness -= struggle;
        }

        // no nutrition
        if(nutrition<0){
            fitness -= struggle;
            nutrition = 0;
        }

        // no water
        if(hydration < 0){
            fitness -= struggle;
            hydration = 0;
        }

        // too high pressure
        if(bodyPressure > 1.8f){
            fitness -= struggle;
        }

        // too low pressure
        if(bodyPressure < 0.2f){
            fitness -= struggle;
        }

        // too high radiation
        if(bodyRadiation > 1){
            fitness -= struggle * Mathf.Max(0,((bodyRadiation-1-0.5f))*2)  * 5f; 
        }

      




        /////////  INJURIES
        
        float health = injuries;

        // cannot be fitter than haelth
        fitness = Mathf.Min(fitness, health);
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        /////////  LIMITS



        fitness = Mathf.Max(0,fitness);
        injuries = Mathf.Max(0,injuries);


        
        nutrition = Mathf.Min(100*nutritionDelayFactor,nutrition);
        fitness = Mathf.Min(100,fitness);
        hydration = Mathf.Min(100,hydration);
        injuries = Mathf.Min(100, injuries);

        bodyTemperature = Mathf.Max(32, bodyTemperature);
        bodyTemperature = Mathf.Min(42, bodyTemperature);
        
        bodyPressure = Mathf.Max(0, bodyPressure);
        bodyPressure = Mathf.Min(2, bodyPressure);

        bodyRadiation = Mathf.Max(1, bodyRadiation);
        bodyRadiation = Mathf.Min(2, bodyRadiation);
        

        stomachCaloryAmount = Mathf.Min(200, bodyRadiation);
        stomachHydroAmount = Mathf.Min(200, stomachHydroAmount);
        stomachMedecinAmount = Mathf.Min(100, stomachMedecinAmount);
        stomachProteinAmount = Mathf.Min(200, stomachProteinAmount);





    }


   

    
    

    
}
