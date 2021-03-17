using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CharController : MonoBehaviour
{
    public Vector3 rotationOffset;
    public float speed = 6f;
    public float rollSpeed = 10f;
    public float rollDuration = 0.5f;
    public float camSmoothing = 5f;
    
    private CharacterController controller;
    private Camera cam;
    
    private float turnSmoothVelocity;
    private Vector3 camOffset;
    private float gravity;
    private Animator anim;
    private static readonly int Walk = Animator.StringToHash("Walk");

    private Vector3 moveDir;
    private Vector3 lookDir;

    private bool isRolling;
    private Vector3 rollDir;
    private float rollTimer;
    private static readonly int IsRolling = Animator.StringToHash("isRolling");

    int floorMask; // A layer mask so that a ray can be cast just at gameobjects on the floor layer.

    // Start is called before the first frame update
    void Start()
    {
        floorMask = LayerMask.GetMask ("Ground");
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        cam = Camera.main;
        camOffset = cam.transform.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        setRotation();
        handleMovement();
        if (Input.GetKeyUp("space")) startRoll();
        setWalkingAnimation();
    }

    private void move(float moveSpeed, Vector3 direction)
    {
        var targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.transform.rotation.eulerAngles.y;
        var moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        controller.Move(moveDirection.normalized * (moveSpeed * Time.deltaTime));
    }

    public bool isMoving()
    {
        return moveDir.magnitude >= 0.1f;
    }

    public bool isPlayerRolling()
    {
        return isRolling;
    }

    private void handleMovement()
    {
        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;
        if (isRolling) handleRoll();
        else if (isMoving()) move(speed, moveDir);
        handleGravity();
    }

    private void handleRoll()
    {
        rollTimer += Time.deltaTime;
        if (rollTimer >= rollDuration) isRolling = false;
        else move(rollSpeed, rollDir);
    }

    private void startRoll()
    {
        if (isRolling) return;
        isRolling = true;
        rollDir = isMoving() ? moveDir : lookDir;
        
        var angle = Mathf.Atan2(rollDir.x, rollDir.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, angle, 0);

        anim.SetTrigger(IsRolling);
        rollTimer = 0;
    }

    private void setRotation()
    {
        if (isRolling) return;
        Ray camRay = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit floorhit;
        
        if (Physics.Raycast(camRay, out floorhit, 100, floorMask))
        {
            var t = transform;
            lookDir = (floorhit.point + rotationOffset - t.position).normalized;
            t.LookAt(floorhit.point + rotationOffset); // Look at the point
            t.rotation = Quaternion.Euler(new Vector3(0, t.rotation.eulerAngles.y, 0));
        }
    }

    private void setWalkingAnimation()
    {
        if (isRolling) return;
        if (isMoving())
        {
            var angle = angleBetweenVector2(moveDir, lookDir);
            var absoluteAngle = Math.Abs(angle);
            int dir;
            if (absoluteAngle <= 45) dir = 1;
            else if (absoluteAngle > 45 && absoluteAngle < 135) dir = angle > 0 ? 2 : 3;
            else dir = 0;
            anim.SetInteger(Walk, dir);
        }
        else anim.SetInteger(Walk, -1);
    }

    private float angleBetweenVector2(Vector3 vec1, Vector3 vec2)
    {
        var referenceRight = Vector3.Cross(Vector3.up, vec1);
        var angle = Vector3.Angle(vec2, vec1);
        var sign = Mathf.Sign(Vector3.Dot(vec2, referenceRight));
        var finalAngle = sign * angle;
        return finalAngle;
    }

    private void handleGravity()
    {
        gravity -= 9.81f * Time.deltaTime;
        if (controller.isGrounded) gravity = 0;
        controller.Move(new Vector3(0, gravity, 0) * Time.deltaTime);
    }
    
    
    //TODO make own script maybe
    private void FixedUpdate()
    {
        var pos = transform.position;
        var targetCamPos = new Vector3(pos.x, 0, pos.z) + camOffset;
        cam.transform.position = Vector3.Lerp (cam.transform.position, targetCamPos, camSmoothing * Time.deltaTime);
    }
}