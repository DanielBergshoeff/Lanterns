using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float MoveSpeed = 1f;
    public float RotateSpeed = 3f;
    public float JumpForce = 1f;

    public InputActionAsset playerControls;
    public Animator PlayerAnimator;
    public Camera MyCamera;

    private InputAction movement;
    private InputAction jump;
    private Vector2 moveDir;

    private Rigidbody myRigidbody;
    private bool jumping = false;
    private bool moving = false;
    private AudioSource myAudioSource;


    // Start is called before the first frame update
    void Start() {
        myRigidbody = GetComponent<Rigidbody>();
        myAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        Move();

        if (jumping) {
            RaycastHit hit1;
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, -transform.up, out hit1, 0.51f)) {
                //Landed
                jumping = false;
            }
            return;
        }
        else {
            RaycastHit hit1;
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, -transform.up, out hit1, 1f)) {
                if (hit1.distance > 0.51f) {
                    jumping = true;
                }
            }
            else {
                jumping = true;
            }
        }
    }

    private void Move() {
        if (moveDir.magnitude < 0.1f) {
            moving = false;
            myAudioSource.Stop();
            return;
        }

        if (!moving && !jumping) {
            myAudioSource.loop = true;
            myAudioSource.clip = AudioManager.Instance.PlayerRun;
            //myAudioSource.Play();
            moving = true;
        }
        else {
            if (jumping) {
                moving = false;
                myAudioSource.Stop();
            }
        }

        Vector3 targetDir = new Vector3(moveDir.x, 0f, moveDir.y);
        targetDir = MyCamera.transform.TransformDirection(targetDir);
        targetDir.y = 0.0f;

        transform.position = transform.position + targetDir * Time.deltaTime * MoveSpeed;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDir, Time.deltaTime * RotateSpeed, 0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
    }

    private void OnMove(InputValue context) {
        moveDir = context.Get<Vector2>();
        PlayerAnimator.SetFloat("Speed", context.Get<Vector2>().magnitude);
    }

    private void OnJump() {
        if (jumping)
            return;


        myRigidbody.AddForce(Vector3.up * (JumpForce + JumpForce * FireController.Instance.FirePower));
        PlayerAnimator.SetTrigger("Jump");
    }
}
