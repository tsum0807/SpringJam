using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedController : MonoBehaviour
{
    
    // degrees per sec
    private float _rotSpeed = 90f;

    [SerializeField] public int _seedType;
    [SerializeField] public int _seedAmount;

    void Start(){
        
    }

    // Update is called once per frame
    void Update(){
        RotateY();
    }

    private void RotateY(){
        transform.Rotate(0f, _rotSpeed * Time.deltaTime, 0f);
    }
}
