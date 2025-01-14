using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class groundSpawner : MonoBehaviour
{
    
    [Header("Level Generator")]
    
    [SerializeField] bool spawnTileOnSceneLoad = true;
    [SerializeField] bool spawnDynamicObstacle = true;

    [SerializeField] GameObject groundTile;
    [SerializeField] GameObject finishTile;
    

    [Range(0,40)]   [SerializeField] int lengthMinValue;
    [Range(0,40)]   [SerializeField] int lengthMaxValue;


    [Range(0,5)]   [SerializeField] float widthMinValue;
    [Range(0,5)]   [SerializeField] float widthMaxValue;

    [Header("Spawner")]

    [Range(0,40)]   [SerializeField] int minCoinsToDrop;
    [Range(0,40)]   [SerializeField] int maxCoinsToDrop;


    [Range(0,5)]   [SerializeField] int minObstacleToDrop;
    [Range(0,5)]   [SerializeField] int maxObstacleToDrop;
    [SerializeField] public List<GameObject> spawnedElements = new List<GameObject>();

    [Header("Give it some life")]

    [SerializeField] private List<Color> Colors = new List<Color>();


    [Header("save the level as a prefab")]

    [SerializeField] string location = "Assets/_Prefabs/Levels/";


    Vector3 nextSpawnPoint;
    GameObject temp;
    float randomValue;
    GameObject player;
    GameObject LevelObj;

    void Start() 
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if(spawnTileOnSceneLoad || spawnedElements.Count <= 0)
        {
            spawn();
        }else{
            player.GetComponent<SnakeMovement>().setMinXandMaxX(spawnedElements[0].transform.GetChild(0).localScale.x , spawnedElements[0].transform.GetChild(0).localScale.x);
        }
    }

    public void SpawnTile (bool spawnItems)
    {
        temp = Instantiate(groundTile, nextSpawnPoint, Quaternion.identity , LevelObj.transform);
        nextSpawnPoint = temp.transform.GetChild(1).transform.position;

        temp.transform.GetChild(0).localScale = new Vector3(temp.transform.localScale.x + randomValue ,temp.transform.localScale.y,temp.transform.localScale.z );

        spawnedElements.Add( temp );

        if (spawnItems)
        {
            temp.GetComponent<groundTile>().SpawnCoins(Random.Range(minCoinsToDrop,maxCoinsToDrop));
            temp.GetComponent<groundTile>().SpawnObstacles(Random.Range(minObstacleToDrop,maxObstacleToDrop), spawnDynamicObstacle);
        }
        
    }

    private void Colorize( Renderer gameObjectToColorize , Color colors)
    {
        gameObjectToColorize.material.SetColor("_Color" , colors ) ;
    }

    public void SpawnFinishLine()
    {
        GameObject temp = Instantiate(finishTile, nextSpawnPoint, Quaternion.identity , LevelObj.transform);

        spawnedElements.Add( temp );
        nextSpawnPoint = temp.transform.GetChild(1).transform.position;
        temp.transform.GetChild(0).localScale = new Vector3(temp.transform.localScale.x + randomValue ,temp.transform.localScale.y,temp.transform.localScale.z );

    }

    public void spawn () 
    {
        ResetToDefault();
        randomValue = Random.Range(widthMinValue, widthMaxValue);
        int randomLength = Random.Range(lengthMinValue,lengthMaxValue);

        LevelObj = new GameObject("Level 01");

        for (int i = 0; i < randomLength; i++) 
        {
            if(i < 1 ){
               SpawnTile(false);
            }else 
                if(Random.value >= 0.5f)
                {
                    SpawnTile(true);
                }else
                    SpawnTile(false);
        }

        SpawnFinishLine();
        // update the snake boundraise
        player.GetComponent<SnakeMovement>().setMinXandMaxX(temp.transform.GetChild(0).localScale.x,temp.transform.GetChild(0).localScale.x);

        // change color for each spawned elements to a random color 
        Color color = Colors[Random.Range( 0 , Colors.Count)];
        for (int i = 0; i < spawnedElements.Count; i++)
        {        
            Colorize( spawnedElements[i].transform.GetChild(0).GetComponent<Renderer>() , color );
        }
    }

    // SAVE the spawned gameObject as a prefab, to use it later ...

    public void SaveGeneratedLevelAsPrefab()
    {
        if(spawnedElements.Count >= 1){

        // Create folder Prefabs and set the path as within the Prefabs folder,
        // and name it as the GameObject's name with the .Prefab format
        if (!Directory.Exists(location))
            AssetDatabase.CreateFolder("Assets", "Prefabs" );
            AssetDatabase.CreateFolder("Prefabs", "Levels" );

        string localPath = location + LevelObj.name + ".prefab";

        // Make sure the file name is unique, in case an existing Prefab has the same name.
        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

        // Create the new Prefab and log whether Prefab was saved successfully.
        bool prefabSuccess;
        PrefabUtility.SaveAsPrefabAsset(LevelObj, localPath, out prefabSuccess);
        if (prefabSuccess == true)
            Debug.Log("Level was saved successfully");
        else
            Debug.Log("Level failed to save" + prefabSuccess);
        }
        else {
            Debug.Log("Please Click Generate First ,then you Can Save ");
        }
}


    internal void ResetToDefault()
    {
        for (int i = 0; i < spawnedElements.Count; i++)
        {            
            Colorize( spawnedElements[i].transform.GetChild(0).GetComponent<Renderer>() , Color.white );
            DestroyImmediate(spawnedElements[i].gameObject);

        }

        DestroyImmediate(LevelObj);
        nextSpawnPoint = Vector3.zero;
        spawnedElements.Clear();
    }
}
