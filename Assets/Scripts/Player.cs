using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public float moveSpeed = 0f;
    CharacterController cc;
    [SerializeField] protected Vector3 direction = Vector3.zero;
    protected Transform cam;
    public float turnSmoothTime = 0.1f;
    protected float turnSmoothVelocity;
    void Start () {
        cc = GetComponent<CharacterController> ();
        cam = Camera.main.transform;
    }

    void Update () {
        MovementInput ();
    }
    void MovementInput () {
        // direction = Vector3.forward * Input.GetAxisRaw ("Vertical") + Vector3.right * Input.GetAxisRaw ("Horizontal");
        direction.Set (Input.GetAxisRaw ("Horizontal"), 0f, Input.GetAxisRaw ("Vertical"));
        direction = direction.normalized;
        if (direction.magnitude > 0.1f) {
            float targetAngle = Mathf.Atan2 (direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle (transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler (0, angle, 0);

            Vector3 moveDirection = Quaternion.Euler (0f, targetAngle, 0f) * Vector3.forward;
            cc.Move (moveDirection.normalized * moveSpeed * Time.deltaTime);
            // cc.Move (direction * moveSpeed * Time.deltaTime);
        }
    }
}