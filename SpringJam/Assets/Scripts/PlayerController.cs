using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _jumpForce = 10f;

    [SerializeField] private GameObject runParticles;
    [SerializeField] private Sprite _idleSprite;

    private static float GRAVITY = 1f;

    public int _health = 3;
    public GroundDetector _groundDetector;

    private Rigidbody2D _rb;
    private SpriteRenderer _sr;
    private AudioSource _audio;
    private Animator _animator;

    private STATE _state;
    private float _runParticleTime = 0.5f;
    private bool _canClimb;
    private Vector3 _hurtVel;
    private float _hurtTimer;
    private float _invulnTimer;
    private bool _isPlayingWalkingSound;
    private bool _isLoadingNext;
    private enum STATE{
        Grounded,
        Walking,
        Jumping,
        Climbing,
        Planting,
        Watering,
        Hurt,
        Invuln
    }

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _audio = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        _groundDetector = GetComponentInChildren<GroundDetector>();
        _state = STATE.Grounded;
        _canClimb = false;
        _isPlayingWalkingSound = false;
        _isLoadingNext = false;
    }

    void Start(){}

    void Update()
    {
        HandleMovement();
        HandleState();
        HandleAnim();
        // print(_rb.velocity);
        // print(_state);

        _runParticleTime -= Time.deltaTime;
    }

    private void HandleMovement() {
        // Cant control during hurt
        if(_state == STATE.Hurt){
            _hurtTimer -= Time.deltaTime;
            if(_hurtTimer <= 0f){
                _state = STATE.Invuln;
            }
            return;
        }
        if(_state == STATE.Invuln){
            // Can move but cant take dmg
            _invulnTimer -= Time.deltaTime;
            if(_invulnTimer <= 0f){
                _state = STATE.Grounded;
            }
        }
        float x = Input.GetAxisRaw("Horizontal"); 
        float y = Input.GetAxisRaw("Vertical"); 
        float stepX = x * _speed; 
        float stepY = _rb.velocity.y;

        // JUMP
        // can only jump when not already jumping
        if(_state != STATE.Jumping && Input.GetButtonDown("Jump")) { 
            stepY = _jumpForce;
            GameManager.Instance._am.PlayJumpSound();
        }

        // Going up down
        if(_canClimb && (y != 0 || _state == STATE.Climbing)){
            stepY = y * _speed;
            // no gravity while climbing
            _rb.gravityScale = 0f;
        }

        if(y == -1){
            // Dropping through platforms
            // when pressing down
            SetPlayerPlatformCollision(false);
        }else{
            SetPlayerPlatformCollision(true);
        }

        _rb.velocity = new Vector2(stepX, stepY);
    }

    private void HandleAnim(){
        float curX = _rb.velocity.x;
        float curY = _rb.velocity.y;

        if(_state == STATE.Hurt){
            // dont do anim while hurt
            _sr.sprite = _idleSprite;
            _animator.enabled = false;
        }

        if(curX > 0){
            _animator.enabled = true;
            _sr.flipX = false;
        }else if(curX < 0){
            _animator.enabled = true;
            _sr.flipX = true;
        }else{
            _sr.sprite = _idleSprite;
            _animator.enabled = false;
        }
        
    }

    private void HandleState(){
        if(_state == STATE.Hurt || _state == STATE.Invuln){
            return;
        }

        float x = Input.GetAxisRaw("Horizontal"); 
        float y = Input.GetAxisRaw("Vertical");
        Vector2 curVel = _rb.velocity;
   
        
        if(Mathf.Abs(curVel.y) > 0.2f)
            _state = STATE.Jumping;
            
        if(_groundDetector._isGrounded)
            _state = STATE.Grounded;

        if(curVel.x != 0 && _groundDetector._isGrounded){
            _state = STATE.Walking;
            PlayWalkSound();
        }else{
            StopWalkSound();
        }

        if(_canClimb && y != 0)
            _state = STATE.Climbing;


        // Stuff at each state
        switch(_state){
            case STATE.Walking:
                SpawnRunParticle();
                break;
        }   
    }


    public void TakeDamage(int dmg){
        if(_state == STATE.Hurt || _state == STATE.Invuln)
            return;

        _health -= dmg;
        GameManager.Instance._am.PlayHurtSound();
        
        _state = STATE.Hurt;
        // Randomly knock back in upwards and in some direction
        _hurtVel = new Vector3(Random.Range(-1f, 1f), 2.5f, 0f);
        _rb.velocity = _hurtVel;
        _hurtTimer = 0.5f;
        // Flash
        StartCoroutine("FlashCoroutine", 0.1f);

        if(_health <= 0){
            //die respawn current level with full health
            _health = 3;
            // refresh seeds
            GameManager.Instance.curLevel--;
            GameManager.Instance.NextLevel();
        }

        GameManager.Instance.UpdateHearts();

        // Give Iframes
        InvulnFor(1f);
    }

    private void InvulnFor(float time){
        _invulnTimer = time;
    }

    void OnTriggerEnter2D(Collider2D collider){
        if(collider.tag == "Vines")
            _canClimb = true;

        if(collider.tag == "Goal"){
            GameManager.Instance._am.PlayFinishSound();
            StartCoroutine(GameObject.FindObjectOfType<SceneFader>().
                Fade(SceneFader.FadeDirection.In));
            // Play flag particles
            collider.transform.GetChild(0).gameObject.SetActive(true);
            if(!_isLoadingNext){
                _isLoadingNext = true;
                Invoke("CallNextLevel", 2.5f);
            }
        }

        if(collider.tag == "Seed"){
            // Unlock seed
            int type = collider.GetComponent<SeedController>()._seedType;
            int amt = collider.GetComponent<SeedController>()._seedAmount;
            GameManager.Instance.GiveSeedTypeAmount(type, amt);
            collider.gameObject.SetActive(false);
            GameManager.Instance._am.PlayPickUpSound();
        }

        // Disable collision with enemies while behind bush
        if(collider.tag == "Bush")
            SetPlayerEnemiesCollision(false);

    }

    private void CallNextLevel(){
        // _groundDetector.ClearAllGround();
        GameManager.Instance.NextLevel();
        _isLoadingNext = false;
        StartCoroutine(GameObject.FindObjectOfType<SceneFader>().
            Fade(SceneFader.FadeDirection.Out));
    }

    private void PlayWalkSound(float volume = 1f){
        if (volume == 0)
            return;
        if(_isPlayingWalkingSound)
            return;

        float roll = Random.Range(-0.1f, 0.1f);
        float volToPlayAt = volume + roll <= 0 ? volume : volume + roll;
        _isPlayingWalkingSound = true;

        _audio.Play();
    }

    private void StopWalkSound(){
        if(_isPlayingWalkingSound){
            _audio.Stop();
            _isPlayingWalkingSound = false;
        }
    }

    void OnTriggerExit2D(Collider2D collider){
        if(collider.tag == "Vines"){
            _canClimb = false;
            _rb.gravityScale = GRAVITY;
        }
        
        if(collider.tag == "Bush")
            SetPlayerEnemiesCollision(true);
    }

    void OnCollisionEnter2D(Collision2D collisionInfo){
        Collider2D collider = collisionInfo.collider;
        if(collider.tag == "Enemy" && _state != STATE.Hurt){
            // hit an enemy so take damage and possible get knocked back?
            TakeDamage(1);
        }
    }

    private void SpawnRunParticle(){
        if(_runParticleTime <= 0f && _state == STATE.Walking){
            // spawn dirt particles at player feet
            Vector3 pos = transform.position - new Vector3(0f, 0.25f, 0f);
            Instantiate(runParticles, pos, Quaternion.identity);
            _runParticleTime = 0.5f;
        }
    }

    private void SetPlayerPlatformCollision(bool flag){
        int platformLayer = LayerMask.NameToLayer("Platforms");
        int playerLayer = LayerMask.NameToLayer("Player");
        Physics2D.IgnoreLayerCollision(platformLayer, playerLayer, !flag);
    }

    private void SetPlayerEnemiesCollision(bool flag){
        int enemyLayer = LayerMask.NameToLayer("Enemies");
        int playerLayer = LayerMask.NameToLayer("Player");
        Physics2D.IgnoreLayerCollision(enemyLayer, playerLayer, !flag);
    }
    
    IEnumerator FlashCoroutine(float secs){
        // change colour to opposite colour
        Color ogColour = _sr.color;
        Color tmpColour = ogColour;
        tmpColour.a = 0.2f;

        _sr.color = tmpColour;

        yield return new WaitForSeconds(secs); //Count is the amount of time in seconds that you want to wait.
        // Turn back to original colour

        _sr.color = ogColour;
        
        yield return new WaitForSeconds(secs);

        // blink again if still in invuln
        if(_state == STATE.Invuln || _state == STATE.Hurt)
            StartCoroutine("FlashCoroutine", 0.1f);
        
        yield return null;
    }
}
