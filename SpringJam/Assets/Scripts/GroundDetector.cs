using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    public bool _isGrounded;

    private int _numEntered = 0;

    void Start()
    {
        _isGrounded = true;
    }


    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collider){
        if(collider.tag != "Player"){
            _isGrounded = true;
            _numEntered++;
        }
    }


    void OnTriggerExit2D(Collider2D collider){
        if(collider.tag != "Player"){
            _numEntered--;
            if(_numEntered <= 0)
                _isGrounded = false;
        }
    }

    public void ClearAllGround(){
        _isGrounded = false;
        _numEntered = 0;
    }
}
