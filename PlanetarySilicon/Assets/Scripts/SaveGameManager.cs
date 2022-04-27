using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;


[ExecuteInEditMode]
public class SaveGameManager : MonoBehaviour
{

    public string saveGame;
    public bool save;
    public bool load;

    public int currentPlanet;
    public int currentStar;
    public int currentGalaxy;
    public int currentPlayer;

    public Vector3 playerPosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(save){
            Save();
            save = false;
        }
        if(load){
            Load();
            Activate();
            load = false;
        }
    }

    void Load(){
        if (File.Exists(Application.persistentDataPath + "/"+saveGame+".save"))
		{

			
			// 2
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/"+saveGame+".save", FileMode.Open);
			SaveGameObject save = (SaveGameObject)bf.Deserialize(file);
			file.Close();



            currentPlanet = save.currentPlanet;
            currentStar = save.currentStar;
            currentGalaxy = save.currentGalaxy;
            currentPlayer = save.currentPlayer;

            playerPosition = new Vector3(save.playerX, save.playerY, save.playerZ);


			Debug.Log("LOADED SAVE GAME :  " + saveGame);
			
		}
		else
		{
			Debug.Log("SAVE GAME NOT FOUND");
		}
    }

    void Save(){

        SaveGameObject save = new SaveGameObject();


        save.currentPlanet = Universe.ACTIVE_SYSTEM.centerObject.transform.GetSiblingIndex();
        save.currentStar = Universe.ACTIVE_SYSTEM.galaxyStarIndex;
        save.currentGalaxy = Universe.ACTIVE_GALAXY.transform.GetSiblingIndex();
        GameObject currentPlayer = GameObject.Find("PlayerObjects").GetComponent<PlayerObjects>().activePlayerObject;
        save.currentPlayer = currentPlayer.transform.GetSiblingIndex();
        save.playerX = currentPlayer.transform.position.x;
        save.playerY = currentPlayer.transform.position.y;
        save.playerZ = currentPlayer.transform.position.z;
        
      

        BinaryFormatter bf;
        FileStream file;
      

		bf = new BinaryFormatter();
		file = File.Create(Application.persistentDataPath + "/"+saveGame+".save");
		bf.Serialize(file, save);
		file.Close();




    }

    void Activate(){
        GameObject universe = GameObject.Find("Universe");

        foreach(ProceduralSystemGenerator sg in universe.GetComponentsInChildren<ProceduralSystemGenerator>()){
            sg.ClearAllSystems();
        }


        ProceduralSystemGenerator currentGenerator = universe.transform.GetChild(currentGalaxy).GetComponent<ProceduralSystemGenerator>();

        currentGenerator.galaxyStarIndex = currentStar;
        SystemManager systemManager = currentGenerator.GenerateAtStarIndex();

        systemManager.centerObject = systemManager.transform.GetChild(currentPlanet).gameObject;

        universe.GetComponent<Universe>().currentSystem = systemManager;
        universe.GetComponent<Universe>().ActivateCurrentSystem();

        GameObject currentPlayer = GameObject.Find("PlayerObjects").GetComponent<PlayerObjects>().activePlayerObject;
        currentPlayer.transform.position = playerPosition;
        GameObject.Find("PlayerObjects").GetComponent<PlayerObjects>().SwitchToActivePlayerObject();


        


    }
}


[System.Serializable]
public class SaveGameObject
{
  public int currentPlanet;
  public int currentStar;
  public int currentGalaxy;
  public int currentPlayer;

  public float playerX;
  public float playerY;
  public float playerZ;

  




}
