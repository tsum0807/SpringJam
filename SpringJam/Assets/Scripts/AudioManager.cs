using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    
    [SerializeField] private AudioClip finish;
    [SerializeField] private AudioClip playerHurt;
    [SerializeField] private AudioClip jump;
    [SerializeField] private AudioClip plantGrowing;
    [SerializeField] private AudioClip step1;
    [SerializeField] private AudioClip pickUp;

    private AudioSource _audio;

    void Awake(){
        _audio = GetComponent<AudioSource>();
    }

    private void PlaySound(AudioClip clipToPlay, float volume = 1f){
        if (volume == 0)
        {
            // volume is muted so dont try to play anything
            return;
        }
        // Randomize sound volume slightly
        float roll = Random.Range(-0.1f, 0.1f);
        float volToPlayAt = volume + roll <= 0 ? volume : volume + roll;

        _audio.PlayOneShot(clipToPlay, volToPlayAt);
    }

    public void PlayHurtSound(){
        PlaySound(playerHurt);
    }
    public void PlayJumpSound(){
        PlaySound(jump, 0.3f);
    }
    public void PlayFinishSound(){
        PlaySound(finish);
    }
    public void PlayGrowingSound(){
        PlaySound(plantGrowing);
    }
    public void PlayPickUpSound(){
        PlaySound(pickUp);
    }
}
