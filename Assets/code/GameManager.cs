using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static int currentStage = 0;
    public PlayerInfo player = new PlayerInfo(PlayerInfo.playerType.Akai);
    public GameObject gameUI;
    GameObject gameCanvas;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void Awake()
	{
        GameObject[] t = GameObject.FindGameObjectsWithTag("gameManager");
        if (t.Length > 1) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        GameObject gameCanvas;
        if (SceneManager.GetActiveScene().name == "level1")
        {

            gameCanvas = Instantiate(gameUI);
            gameCanvas.name = "Canvas";
            gameCanvas.transform.parent = gameObject.transform;

        }
        else
        {
            gameCanvas = GameObject.Find("Canvas");

        }
        if (SceneManager.GetActiveScene().name == "levelComplete") gameCanvas.SetActive(false);
        else gameCanvas.SetActive(true);
	}

    public void SetStage(int newStage)
    {
        currentStage = newStage;

    }

    public void IncreaseStage(int increment)
    {

        currentStage += increment;

    }

    public int GetStage()
    {

        return (currentStage);
    }

    public void LoadNewScene()
    {

        string nextScene = "level" + (GetStage() + 1);
        GetComponent<QuestSystem>().stagePanel.SetActive(true);
        if (GetStage() > 0) GetComponent<QuestSystem>().Init();
        SceneManager.LoadScene(nextScene);

    }
}
