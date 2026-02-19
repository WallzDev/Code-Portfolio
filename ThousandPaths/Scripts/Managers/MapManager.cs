using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    //Player Icon follow player stuff
    public Transform player;
    public RectTransform minimap;
    public RectTransform playerIcon;
    public RectTransform dKatanaIcon;

    [SerializeField] GameObject[] maps;

    public GameObject areaForest;
    public GameObject areaTestCave;
    public GameObject areaDarkForest;
    public GameObject areaVillage;
    public GameObject areaAsaTown;
    public GameObject areaSpiralCave;
    public GameObject areaGiantBattleground;
    public GameObject areaRyukken;
    public GameObject areaTheBridge;
    public GameObject areaBoneForest;
    public GameObject areaCity;
    public GameObject areaKingsCastle;

    //Tests
    private GameManager gm;
    public GameObject currentSceneBounds;
    public Vector3 currentScenePos;
    public GameObject currentScene;
    public float mapScaleFactor;


    Torii torii;

    public void Awake()
    {
        gm = GameManager.Instance;
    }

    public void Start()
    {
        player = PlayerController.Instance.GetComponent<Transform>();
        UpdateMap();
    }

    public void Update()
    {
        if (currentScene != null)
        {
            Vector2 playerOffset = new Vector2(
                player.localPosition.x * mapScaleFactor,
                player.localPosition.y * mapScaleFactor
            );

            playerIcon.transform.localPosition = currentScenePos + player.localPosition/*(Vector3)playerOffset*/;
        }
    }

    private void OnEnable()
    {
        PositionPlayerIcon();
        PositionDkatana();
        torii = FindObjectOfType<Torii>();
        if (torii != null )
        {
            if(torii.interacted)
            {
                UpdateMap();
            }
        }
    }


    void UpdateMap()
    {
        var savedScenes = PlayerController.Instance.scenesOnMap;

        for (int i = 0; i < maps.Length; i++)
        {
            foreach (string item in savedScenes)
            {
                if (maps[i].name == item)
                {
                    maps[i].SetActive(true);
                }

            }

        }
    }

    public void PositionPlayerIcon()
    {
        GameObject gameObject = null;
        string currentMapArea = gm.GetCurrentMapAreaName();

        switch (currentMapArea)
        {

            case "FOREST":

                gameObject = areaForest;
                for (int ar1 = 0; ar1 < areaForest.transform.childCount; ar1++)
                {
                    GameObject gameObject1 = areaForest.transform.GetChild(ar1).gameObject;
                    {
                        if (gameObject1.name == GameManager.Instance.currentSceneName)
                        {
                            currentScene = gameObject1;
                            break;
                        }
                    }
                }
                break;
                  

            case "TEST_CAVE":
                gameObject = areaTestCave;
                for (int ar2 = 0; ar2 < areaTestCave.transform.childCount; ar2++)
                {
                    GameObject gameObject2 = areaTestCave.transform.GetChild (ar2).gameObject;
                    {
                        if (gameObject2.name == GameManager.Instance.currentSceneName)
                        {
                            currentScene = gameObject2;
                            break;
                        }
                    }
                }
                break;
        }

        if (currentScene != null)
        {
            currentScenePos = new Vector3(currentScene.transform.localPosition.x + gameObject.transform.localPosition.x,
                currentScene.transform.localPosition.y + gameObject.transform.localPosition.y, 0f);

            playerIcon.transform.localPosition = currentScenePos;
            playerIcon.gameObject.SetActive(true);
        }

    }

    public void PositionDkatana()
    {

        if (PlayerController.Instance.halfInk)
        {
            if (SaveData.Instance.sceneWithDKatana != null)
            {
                for (int i = 0; i < maps.Length; i++)
                {
                    if (maps[i].name == SaveData.Instance.sceneWithDKatana.ToString())
                    {
                        dKatanaIcon.position = maps[i].transform.position;
                        dKatanaIcon.gameObject.SetActive(true);
                    }

                }
            }
        }
        else
        {
            dKatanaIcon.gameObject.SetActive(false);
        }
    }
}
