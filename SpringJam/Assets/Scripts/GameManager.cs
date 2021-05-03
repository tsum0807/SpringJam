using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Single GM
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public static bool _isGameCompleted = false;
    
    public Seed curSelectedSeed;
    public int curLevel = 0;
    public GameObject player;
    public AudioManager _am;

    [SerializeField] private GameObject LevelsContainer;
    [SerializeField] private GameObject SeedBtnsContainer;
    [SerializeField] private GameObject ShovelBtn;
    [SerializeField] private GameObject HeartsContainer;
    [SerializeField] private GameObject TutorialCanvas;
    [SerializeField] private GameObject SeedDescriptionCanvas;
    [SerializeField] private Sprite[] seedSprites;
    [SerializeField] private Sprite[] HeartSprites;

    public List<Seed> _seedsList = new List<Seed>();

    private void Awake(){
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
        _instance = this;

        InitSeeds();
        _am = GetComponent<AudioManager>();
    }

    void Start()
    {
        // Fade in scene
        StartCoroutine(GameObject.FindObjectOfType<SceneFader>().
            Fade(SceneFader.FadeDirection.Out));

        // SpawnPlayerAtLevel(1);
        curSelectedSeed = _seedsList[0];
        UpdateSeeds();
        
        curLevel = 0;
        NextLevel();
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        if(Input.GetButtonDown("Fire1")){
            //clicked elsewhere on screen
            UpdateSeeds();
        }
    }

    public void UpdateHearts(){
        int curIndex = 1;
        foreach(Transform heart in HeartsContainer.transform){
            if(curIndex > player.GetComponent<PlayerController>()._health){
                heart.GetComponent<Image>().sprite = HeartSprites[0];
            }else{
                heart.GetComponent<Image>().sprite = HeartSprites[1];
            }
            curIndex++;
        }
    }

    public void NextLevel(){
        if(curLevel > 4){
            _isGameCompleted = true;
            // back to main menu
            StartCoroutine(GameObject.FindObjectOfType<SceneFader>().
                FadeAndLoadScene(SceneFader.FadeDirection.In, "Menu"));
            return;
        }
        // Disable tutorial canvas after level 1
        if(curLevel == 1)
            TutorialCanvas.SetActive(false);

        SetAllLevelsInactive();        
        LevelsContainer.transform.GetChild(curLevel).gameObject.SetActive(true);
        player.GetComponent<PlayerController>()._health = 3;

        curLevel++;
        SpawnPlayerAtLevel(curLevel);
        GiveSeedsForLevel(curLevel);
        EnableAllSeedsInLevel(curLevel);
        ShowSeedDescInLevel(curLevel);
        UpdateHearts();
    }

    private void SetAllLevelsInactive(){
        foreach(Transform level in LevelsContainer.transform){
            level.gameObject.SetActive(false);
        }
    }

    private void EnableAllSeedsInLevel(int lvl){
        Transform seedsContainer = LevelsContainer.transform.GetChild(curLevel-1).Find("Seeds");
        if(seedsContainer){
            foreach(Transform child in seedsContainer){
                child.gameObject.SetActive(true);
            }
        }
    }

    private void ShowSeedDescInLevel(int lvl){
        foreach(Transform child in SeedDescriptionCanvas.transform){
            // disable all of them first
            child.gameObject.SetActive(false);
        }
        // enable one on this level only
        Transform seedDesc = SeedDescriptionCanvas.transform.GetChild(lvl-1);
        if(seedDesc)
            seedDesc.gameObject.SetActive(true);
    }

    public void SpawnPlayerAtLevel(int lvl){
        Vector3 spawn = LevelsContainer.transform.GetChild(curLevel-1).Find("Spawn").position;
        player.transform.position = spawn;
        player.GetComponent<PlayerController>()._groundDetector.ClearAllGround();
    }

    public void GiveSeedsForLevel(int lvl){
        SetAllSeedsZero();
        RemoveAllPlantsInLvl(lvl);
        switch(lvl){
            case 1:
                UpdateSeeds();
                break;
            case 2:
                GiveSeedTypeAmount(0, 1);
                GiveSeedTypeAmount(1, 2);
                break;
            case 3:
                GiveSeedTypeAmount(0, 1);
                GiveSeedTypeAmount(1, 2);
                GiveSeedTypeAmount(2, 2);
                break;
            case 4:
                GiveSeedTypeAmount(0, 4);
                GiveSeedTypeAmount(1, 2);
                GiveSeedTypeAmount(2, 2);
                GiveSeedTypeAmount(3, 2);
                break;
            case 5:
                GiveSeedTypeAmount(0, 4);
                GiveSeedTypeAmount(1, 2);
                GiveSeedTypeAmount(2, 2);
                GiveSeedTypeAmount(3, 2);
                GiveSeedTypeAmount(4, 3);
                break;
        }
    }

    public void GiveSeedTypeAmount(int type, int amt){
        _seedsList[type].SetAmount(amt);
        SelectSeed(type);
    }

    public void RemoveAllPlantsInLvl(int lvl){
        Transform dirtContainer = LevelsContainer.transform.GetChild(lvl-1).Find("PlantSpots");
        foreach(Transform dirt in dirtContainer){
            dirt.GetComponent<Plantable>().ClearPlant();
        }
    }

    public void SetAllSeedsZero(){
        foreach(Seed seed in _seedsList){
            seed.SetAmount(0);
        }
    }

    private void InitSeeds(){
        _seedsList.Add(new Seed(0, "mushroomSeed"));
        _seedsList.Add(new Seed(1, "vineSeed"));
        _seedsList.Add(new Seed(2, "treeSeed"));
        _seedsList.Add(new Seed(3, "dandelionSeed"));
        _seedsList.Add(new Seed(4, "bushSeed"));
        _seedsList.Add(new Seed(-1, "shovel"));
    }

    private void HandleInput(){
        // int numPressed = -1;
        // if(Input.GetKeyDown(KeyCode.Alpha1)){
        //     numPressed = 0;
        // }else if(Input.GetKeyDown(KeyCode.Alpha2)){
        //     numPressed = 1;
        // }else if(Input.GetKeyDown(KeyCode.Alpha3)){
        //     numPressed = 2;
        // }else if(Input.GetKeyDown(KeyCode.Alpha4)){
        //     numPressed = 3;
        // }else if(Input.GetKeyDown(KeyCode.Alpha5)){
        //     numPressed = 4;
        // }

        // // actually assign value
        // if(numPressed != -1 && numPressed < _seedsList.Count){
        //     SelectSeed(numPressed);
        //     print("Cur Seed: " + _seedsList[numPressed]._name);
        // }
        // if(Input.GetKeyDown("e")){
        //     NextSeed();
        // }
        // if(Input.GetKeyDown("q")){
        //     PrevSeed();
        // }
        if(Input.GetKeyDown("r")){
            curLevel--;
            NextLevel();
        }
        
    }

    // private void NextSeed(){
    //     if(_curSeeds.Count <= 1)
    //         return;
    //     int curIndex = 0;
    //     foreach(Seed seed in _curSeeds){
    //         if(seed._id == curSelectedSeed._id){
    //             break;
    //         }
    //         curIndex++;
    //     }
    //     curIndex = (curIndex + 1) % _curSeeds.Count;
    //     SelectSeed(_curSeeds[curIndex]._id);
    // }

    // private void PrevSeed(){
    //     if(_curSeeds.Count <= 1)
    //         return;
    //     int curIndex = 0;
    //     foreach(Seed seed in _curSeeds){
    //         if(seed._id == curSelectedSeed._id){
    //             break;
    //         }
    //         curIndex++;
    //     }
    //     curIndex = (curIndex - 1);
    //     curIndex = curIndex < 0 ? _curSeeds.Count-1 : curIndex % _curSeeds.Count;
    //     SelectSeed(_curSeeds[curIndex]._id);
    // }

    public void SelectShovel(){
        ShovelBtn.GetComponent<Button>().Select();
        curSelectedSeed = _seedsList[_seedsList.Count-1];
    }

    public void SelectSeed(int seedID){
        curSelectedSeed = _seedsList[seedID];
        // Enable all btns
        EnableAllSeedBtns();
        // SeedBtnsContainer.transform.GetChild(seedID).GetComponent<Button>().selected = true;
        UpdateSeeds();
    }

    public void UpdateSeeds(){
        int curSeedIndex = 0;
        foreach(Transform btn in SeedBtnsContainer.transform){
            btn.Find("SeedAmount").GetComponent<TMPro.TextMeshProUGUI>().text = "x" + _seedsList[curSeedIndex]._amount.ToString();
            if(_seedsList[curSeedIndex]._amount == 0){
                // Disable btn if no seed for it
                btn.GetComponent<Button>().interactable = false;
            }
            curSeedIndex++;
        }
        if(curSelectedSeed._id == -1){
            // select shovel
            ShovelBtn.GetComponent<Button>().Select();
            return;
        }
        // Select currently selected seed
        SeedBtnsContainer.transform.GetChild(curSelectedSeed._id).GetComponent<Button>().Select();
    }

    private void EnableAllSeedBtns(){
        foreach(Transform btn in SeedBtnsContainer.transform){
            btn.GetComponent<Button>().interactable = true;
        }
    }


}


public class Seed{
    public int _id;
    public string _name;
    public int _amount;
    
    public Seed(int id, string name){
        _id = id;
        _name = name;
        _amount = 0;
    }

    public void SetAmount(int amt){
        _amount = amt;
    }
}
