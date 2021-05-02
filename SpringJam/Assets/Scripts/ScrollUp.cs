using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollUp : MonoBehaviour
{

    private Vector2 UIScreenSize;
    private float _scrollSpeed = 0.25f;


    void Start(){
        UIScreenSize = new Vector3(Screen.width, Screen.height);
    }

    void Update(){
        transform.position += new Vector3(0f, _scrollSpeed * Time.deltaTime, 0f);

    }
    
}
