using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DandelionController : MonoBehaviour
{
    
    private float _speed = 0.7f;
    private bool _isGoingRight;

    private Transform playerPointer;

    void Start(){
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }


    void Update(){
        Move();
    }

    private void Move(){
        // move 1 block down for every 3 blocks across
        float dir = _isGoingRight ? 1f : -1f;
        float xStep = _speed * dir * Time.deltaTime;
        float yStep = - _speed * (1f/2f) * Time.deltaTime;

        transform.position += new Vector3(xStep, yStep, 0f);
    }

    void OnCollisionEnter2D(Collision2D collisionInfo){
        Collider2D collider = collisionInfo.collider;

        if(collider.tag != "Player"){
            if(playerPointer)
                playerPointer.SetParent(null);
            Destroy(gameObject);
        }else{
            // Colliding with player
            playerPointer = collider.transform;
            collider.transform.SetParent(transform);
        }
    }

    void OnCollisionExit2D(Collision2D collisionInfo){
        Collider2D collider = collisionInfo.collider;

        if(collider.tag == "Player"){
            // Left player
            collider.transform.SetParent(null);
        }
    }

    public void SetGoingRight(bool flag){
        _isGoingRight = flag;
    }
}
