using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    [SerializeField] private Transform _pagesContainer;
    
    private int _curPage = 0;

    void Start(){
        if(GameManager._isGameCompleted)
            SelectPage(2);

        StartCoroutine(GameObject.FindObjectOfType<SceneFader>().
            Fade(SceneFader.FadeDirection.Out));
    }

    void Update(){
        // start game
        if(Input.GetButtonDown("Fire1")){
            //clicked elsewhere on screen
            switch(_curPage){
                case 1:
                    StartGame();
                    break;
                case 2:
                    SelectPage(0);
                    break;
            }
            
        }
    }

    public void SelectPage(int page){
        SetAllPagesOff();
        _pagesContainer.GetChild(page).gameObject.SetActive(true);
        _curPage = page;
    }

    private void SetAllPagesOff(){
        foreach(Transform page in _pagesContainer){
            page.gameObject.SetActive(false);
        }
    }

    private void StartGame(){
        StartCoroutine(GameObject.FindObjectOfType<SceneFader>().
            FadeAndLoadScene(SceneFader.FadeDirection.In, "Levels"));
        // SceneManager.LoadScene("Levels");
    }
}
