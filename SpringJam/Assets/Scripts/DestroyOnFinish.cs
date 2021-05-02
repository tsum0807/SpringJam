using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnFinish : MonoBehaviour
{
    private ParticleSystem _ps;
 
 
    public void Start() 
    {
        _ps = GetComponent<ParticleSystem>();
    }

    public void Update(){
        // if(_ps){
        //     if(!_ps.IsAlive())
        //     {
        //         Destroy(gameObject);
        //     }
        // }
    }


    void OnParticleCollision(GameObject other){
        if(other.tag!= "Player"){
            Destroy(gameObject);
        }
    }
    
}
