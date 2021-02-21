using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRB : Player {
    Vector3 rotateEuler = Vector3.up;
    Rigidbody rb;
    // float turnSmoothVelocity;
    void Start () {
        rb = GetComponent<Rigidbody> ();
        cam = Camera.main.transform;
    }

    void Update () {
        MovementInput ();
    }
    void FixedUpdate () {
        LockRotation ();
        Movement ();
    }
    void LockRotation () {
        rotateEuler.Set (0, transform.localEulerAngles.y, 0);
        transform.localEulerAngles = rotateEuler;
    }
    void Movement () {
        if (direction.magnitude > 0.1f) {
            // 获取相机的转向
            float targetAngle = Mathf.Atan2 (direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle (transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler (0, angle, 0);
            Vector3 moveDirection = Quaternion.Euler (0f, targetAngle, 0f) * Vector3.forward;
            // Translate会无视刚体
            // transform.Translate (direction * moveSpeed * Time.deltaTime, Space.World);
            // AddForce会取决于摩擦面,实际速度不理想
            // rb.AddForce (direction * moveSpeed * Time.fixedDeltaTime);
            // 直接设置速度比较合适
            rb.velocity = (moveDirection * moveSpeed * Time.fixedDeltaTime);
        }
    }
    void MovementInput () {
        // 正交相机的移动
        // direction = Vector3.forward * Input.GetAxisRaw ("Vertical") + Vector3.right * Input.GetAxisRaw ("Horizontal");
        direction.Set (Input.GetAxis ("Horizontal"), 0f, Input.GetAxis ("Vertical"));
        direction = direction.normalized;
    }
}