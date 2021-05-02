using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plantable : MonoBehaviour
{
    
    [SerializeField] private Sprite _normalDirt;
    [SerializeField] private Sprite _upsideDownDirt;


    public bool _hasPlant;

    private SpriteRenderer _sr;
    private bool _isUpsideDown;

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    void Start(){
        _hasPlant = false;
        // _isUpsideDown = false;

        if(transform.rotation.z == 1)
            _isUpsideDown = true;
        
        // print(transform.rotation.z);
        // print(_isUpsideDown);
        _sr.sprite = _isUpsideDown ? _upsideDownDirt : _normalDirt;
    }


    void Update(){
        
    }

    void OnMouseDown(){
        // Can only grow plant if not already have one
        if(_hasPlant)
            return;
        int plantToGrow = GameManager.Instance.curSelectedSeed._id;
        print(GameManager.Instance.curSelectedSeed._amount);
        if(GameManager.Instance.curSelectedSeed._amount <= 0){
            // cant grow coz out of seeds
            return;
        }
        GameManager.Instance.curSelectedSeed._amount--;
        GameManager.Instance.UpdateSeeds();

        GrowPlant(plantToGrow);
    }

    private void GrowPlant(int plantId){
        transform.GetChild(plantId).gameObject.SetActive(true);
        transform.GetChild(plantId).GetComponent<Plant>().Init();
        _hasPlant = true;
        // Disable glowing particle
        transform.Find("growableParticle").gameObject.SetActive(false);
        // sound
        GameManager.Instance._am.PlayGrowingSound();
    }

    public void ClearPlant(){
        foreach(Transform child in transform){
            child.gameObject.SetActive(false);
        }
        _hasPlant = false;
        transform.Find("growableParticle").gameObject.SetActive(true);
    }
}
