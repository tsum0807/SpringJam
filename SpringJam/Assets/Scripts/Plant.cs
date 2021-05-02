using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    [SerializeField] private TYPE _type;
    [SerializeField] private GameObject dandelionPrefab;

    private float growthSpeed = 1f;
    private bool _isGrown;
    private bool _isDead;
    private bool _isUpsideDown;

    private enum TYPE{
        Mushroom,
        Vine,
        Tree,
        Dandelion,
        Bush
    }

    void Start(){
        Init();
    }

    public void Init(){
        transform.localScale = new Vector3(0f, 0f, 1f);
        _isGrown = false;
        _isDead = true;
        _isUpsideDown = transform.rotation.z == 1 ? true : false;

        if(_type == TYPE.Tree)
            FlipTreeBranchPlatformEffectors();
    }

    void Update(){
        if(!_isGrown){
            ScaleUp();
        }
        if(!_isDead)
            ScaleDown();

        if(_type == TYPE.Dandelion){
            TrySpawnDandelions();
        }
    }

    void OnMouseDown(){
        if(GameManager.Instance.curSelectedSeed._id == -1){
            // delete with shovel
            // start scaling down
            _isDead = false;
            GameManager.Instance.player.GetComponent<PlayerController>().TakeDamage(1);
            // Give one seed back
            int _seedID = (int)_type;
            GameManager.Instance._seedsList[_seedID]._amount++;
        }
    }

    private void ScaleUp(){
        if(transform.localScale.x < 1f){
            float step = growthSpeed * Time.deltaTime;
            transform.localScale += new Vector3(step, step, 0f);
        }else{
            _isGrown = true;
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    private void ScaleDown(){
        if(transform.localScale.x > 0f){
            float step = growthSpeed * Time.deltaTime;
            transform.localScale -= new Vector3(step, step, 0f);
        }else{
            _isDead = true;
            transform.localScale = new Vector3(0f, 0f, 1f);
            gameObject.SetActive(false);
            // Tell dirt that is gone
            GetComponentInParent<Plantable>()._hasPlant = false;
        }
    }

    /////////////////////////////////////////////
    // TREE
    /////////////////////////////////////////////

    private void FlipTreeBranchPlatformEffectors(){
        Transform branches = transform.Find("branches");
        if(branches){
            foreach(Transform branch in branches){
                branch.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }
    }

    
    /////////////////////////////////////////////
    // DANDELION
    /////////////////////////////////////////////

    private float _dandelionTimer = 3f;

    private void TrySpawnDandelions(){
        _dandelionTimer -= Time.deltaTime;
        // Spawn on both sides when timer is over
        if(_dandelionTimer <= 0f){
            Transform spawnPts = transform.Find("SpawnPoints");
            bool isRight = true;
            foreach(Transform child in spawnPts){
                SpawnDandelion(child, isRight);
                isRight = false;
            }
            _dandelionTimer = 3f;
        }
    }

    private void SpawnDandelion(Transform parent, bool isRight){
        GameObject dand = Instantiate(dandelionPrefab, parent);
        if(_isUpsideDown)
            isRight = !isRight;
        dand.GetComponent<DandelionController>().SetGoingRight(isRight);
    }

}
