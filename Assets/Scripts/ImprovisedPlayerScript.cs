using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UIElements;

public class ImprovisedPlayerScript : NetworkBehaviour
{
    //Audio
    public AudioSource source;
    public PlayerAudio playerAudio;

    //Camera Movement
    public float yaw;
    public float pitch;
    public float mouseSensitivity;
    public Camera playercamera;

    //Player Movement
    public float extraGravity;
    public float dashDistance;
    private Vector3 direction;
    private bool isMoving;
    public Animator animator;

    private Coroutine dashCorou;
    private Coroutine slamCorou;
    private Coroutine atkCorou;

    //private Vector3 direction2;

    //Etcetera
    private Stats playerStats;
    public PlayerCanvas playerCanvas;
    public ViewModel viewModel;
    public Projectile dodgeballPrefab;
    private Rigidbody body;
    private bool isGrounded;
    private bool isDashing;
    private bool swingCooldown;
    

    //Attack
    public Attack attack;
    public WeaponController weaponController;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            playercamera.enabled = false;
            gameObject.layer = LayerMask.NameToLayer("Default");
            foreach (Transform child in transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Default");
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {        
        animator = GetComponent<Animator>();
        playerStats = GetComponent<Stats>();
        swingCooldown = true;
        playerCanvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<PlayerCanvas>();
        playerCanvas.SetHealthBar(playerStats.GetHealth());
        body = GetComponent<Rigidbody>();
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }

    void CameraControl()
    {
        yaw = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= mouseSensitivity * Input.GetAxis("Mouse Y");

        // Clamp pitch thing idk
        pitch = Mathf.Clamp(pitch, -80, 80);

        transform.localEulerAngles = new Vector3(0, yaw, 0);
        playercamera.transform.localEulerAngles = new Vector3(pitch, 0, 0);
    }

    void Move()
    {
        direction = new Vector3(0, 0, 0);
        isMoving = false;

        if (Input.GetKey("w"))
        {
            direction += transform.forward;
        }
        if (Input.GetKey("a"))
        {
            direction -= transform.right;
        }
        if (Input.GetKey("s"))
        {
            direction -= transform.forward;
        }
        if (Input.GetKey("d"))
        {
            direction += transform.right;
        }

        if (direction.x != 0 || direction.y != 0 || direction.z != 0) isMoving = true;
        animator.SetBool("isRunning", isMoving);

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && isGrounded)
        {
            if (dashCorou != null) StopCoroutine(dashCorou);

            isDashing = true;
            dashCorou = StartCoroutine(Dashing(direction));
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && !isGrounded)
        {
            if (slamCorou != null) StopCoroutine(slamCorou);

            isDashing = true;
            slamCorou = StartCoroutine(Slam());
        }

            if (!isDashing) transform.position += (direction.normalized * playerStats.GetSpeed() * Time.deltaTime);
    }

    IEnumerator Dashing(Vector3 direction)
    {
        body.AddForce(direction.normalized * dashDistance * 200);
        yield return new WaitForSeconds(0.3f);
        body.velocity *= 0.2f;
        body.angularVelocity *= 0.2f;

        isDashing = false;
        yield return new WaitForSeconds(0.4f);

    }

    IEnumerator Slam()
    {
        body.AddForce(new Vector3(0,-1000,0));
        
        while (!isGrounded)
        {
            yield return null;
            body.AddForce(new Vector3(0, -50, 0));
        }

        body.velocity *= 0f;
        body.angularVelocity *= 0f;

        isDashing = false;
        yield return new WaitForSeconds(1f);

    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            playerAudio.PlayAudio("jump");
            isGrounded = false;
            body.velocity += new Vector3(0, playerStats.GetJumpHeight(), 0);
        }
    }

    void PlayerAttackActivation()
    {
        if (Input.GetMouseButtonDown(0) && swingCooldown)
        {
            playerAudio.PlayAudio("attack");
            if (atkCorou != null) { StopCoroutine(atkCorou); }
            atkCorou = StartCoroutine(AttackCoroutine());
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Vector3 cameraForward = playercamera.transform.forward;

            Vector3 spawnPos = cameraForward*3 + transform.position;
            playerAudio.PlayAudio("projectile");
            spawnPos.y += 0.5f;
            Projectile projectile = Instantiate(dodgeballPrefab, spawnPos, Quaternion.identity);
            Rigidbody pBody = projectile.GetComponent<Rigidbody>();
            pBody.AddForce(cameraForward * projectile.GetForce());
        }
    }

    IEnumerator AttackCoroutine()
    {
        swingCooldown = false;
        viewModel.PlayAttackAnim();
        yield return new WaitForSeconds(0.1f);
        attack.Activate(playerStats.GetAttack() + weaponController.weapon.attackPower);
        yield return new WaitForSeconds(0.3f);
        attack.Deactivate();
        yield return new WaitForSeconds(0.45f);
        swingCooldown = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        
        
        if(playerStats.GetHealth() <= 0)
        {
            playerCanvas.ShowGOS();
            return;
        }

        CameraControl();
        Move();
        PlayerAttackActivation();
        Jump();
    }

    void FixedUpdate()
    {
        body.AddForce(Vector3.down * extraGravity);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<AI>())
        {
            AI zombie = collision.gameObject.GetComponent<AI>();
            if (zombie.GetIsActive()) 
            {
                playerStats.TakeDamage(zombie.enemyStats.GetAttack());
                playerAudio.PlayAudio("damage");
                playerCanvas.SetHealthBar(playerStats.GetHealth());
            }
        }
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }
    public bool GetGroundedState()
    {
        return isGrounded;
    }
    public bool GetIsMovingState()
    {
        return isMoving;
    }

    public void Heal(float heal)
    {
        if (playerStats.GetHealth() <= 100-heal)
        {
            playerStats.TakeDamage(heal * -1);
        }
        else
        {
            playerStats.SetHealth(100);
        }
        playerCanvas.SetHealthBar(playerStats.GetHealth());
    }

    public void AddGold()
    {
        playerStats.AddCoins(5);
        playerCanvas.UpdateGoldCounter(playerStats.GetCoins());
    }

    public void ShowShop()
    {
        playerCanvas.ShowShop();
    }

    public void HideShop()
    {
        playerCanvas.HideShop();
    }

    public int GetCoins()
    {
        return playerStats.GetCoins();
    }

    public void SetCoins(int gold)
    {
        playerStats.SetCoins(gold);
        playerCanvas.UpdateGoldCounter(playerStats.GetCoins());
    }
}
