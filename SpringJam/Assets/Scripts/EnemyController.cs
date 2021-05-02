using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    
    private float _speed = 0.7f;
    private bool _isGoingRight = true;
    private Rigidbody2D _rb;

    private Transform _detector;
    private Transform _cliffDetector;

    void Awake(){
        _rb = GetComponent<Rigidbody2D>();
        _detector = transform.Find("Detector");
        _cliffDetector = transform.Find("CliffDetector");
    }

    void Start(){
        
    }

    void Update(){
        Patrol();
        CheckForCliff();
    }

    private void CheckForCliff(){
        RaycastHit2D groundInfo = Physics2D.Raycast(_cliffDetector.position, Vector2.down, 0.2f);
        if(groundInfo.collider == false){
            Turn();
        }
    }

    private void Turn(){
        _isGoingRight = !_isGoingRight;
        float dir = _isGoingRight ? 1f: -1f;
        transform.eulerAngles += new Vector3(0f, dir * 180f, 0f);
    }

    void OnCollisionEnter2D(Collision2D collisionInfo){
        // Collider2D collider = collisionInfo.contacts[0].thisCollider;
        // print(collider.name);
    }

    void OnTriggerEnter2D(Collider2D other){
        // Dont collide with bush
        if(other.tag == "Bush" || other.tag == "Player")
            return;
        // Turn around
        Turn();
    }

    private void Patrol(){
        float dir = _isGoingRight ? 1f: -1f;
        float xStep = dir * _speed;

        _rb.velocity = new Vector3(xStep, 0f, 0f);
    }
}
