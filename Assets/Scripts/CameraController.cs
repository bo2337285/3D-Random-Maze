using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    [SerializeField] public float panSpeed;
    [SerializeField] public float rotationAmount;
    [SerializeField] public float zoomAmount;
    [SerializeField] public float moveTime; //平滑系数
    private Vector3 _zoomAmount;
    private Vector3 newPos;
    private Quaternion newRotation;
    private Vector3 newZoom;
    private Transform cameraTF;
    private Vector3 dragStartPos, dragCurrPos;
    private Vector3 rotateStartPos, rotateCurrPos;

    void Start () {
        newPos = transform.position;
        newRotation = transform.rotation;
        cameraTF = transform.GetChild (0);
        newZoom = cameraTF.localPosition;
        _zoomAmount = zoomAmount * new Vector3 (0, -1, 1);
    }

    void Update () {
        HandleMovementInput ();
        HandleRotationInput ();
        HandleMouseZoomInput ();
        HandleMouseDragInput ();
        HandleMouseRotationInput ();
        UpdateCamera ();
    }
    void UpdateCamera () {
        transform.position = Vector3.Lerp (transform.position, newPos, moveTime * Time.deltaTime);
        transform.rotation = Quaternion.Lerp (transform.rotation, newRotation, moveTime * Time.deltaTime);
        cameraTF.localPosition = Vector3.Lerp (cameraTF.localPosition, newZoom, moveTime * Time.deltaTime);
    }

    void HandleMouseRotationInput () {
        if (Input.GetMouseButtonDown (2)) {
            rotateStartPos = Input.mousePosition;
        }
        if (Input.GetMouseButton (2)) {
            rotateCurrPos = Input.mousePosition;
            Vector3 diff = rotateCurrPos - rotateStartPos;
            rotateStartPos = rotateCurrPos;
            newRotation *= Quaternion.Euler ((Vector3.up * diff.x + Vector3.right * diff.y) * rotationAmount * Time.deltaTime);
        }
    }

    void HandleMouseDragInput () {
        if (Input.GetMouseButtonDown (0)) //按下
        {
            Plane plane = new Plane (Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            float entry;
            if (plane.Raycast (ray, out entry)) {
                dragStartPos = ray.GetPoint (entry);
            }
        }
        if (Input.GetMouseButton (0)) //按住
        {
            Plane plane = new Plane (Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            float entry;
            if (plane.Raycast (ray, out entry)) {
                dragCurrPos = ray.GetPoint (entry);
                newPos = transform.position + (dragStartPos - dragCurrPos);
            }

        }
    }

    void HandleMovementInput () {
        if (Input.GetAxis ("Vertical") != 0) {
            newPos += Input.GetAxis ("Vertical") * transform.forward * panSpeed * Time.deltaTime;
        }
        if (Input.GetAxis ("Horizontal") != 0) {
            newPos += Input.GetAxis ("Horizontal") * transform.right * panSpeed * Time.deltaTime;
        }

    }
    void HandleRotationInput () {
        if (Input.GetAxis ("Rotate") != 0) {
            newRotation *= Quaternion.Euler (Vector3.down * rotationAmount * Input.GetAxis ("Rotate") * Time.deltaTime);
        }

    }
    void HandleMouseZoomInput () {
        if (Input.GetAxis ("Zoom") != 0) {
            newZoom += Input.GetAxis ("Zoom") * _zoomAmount * Time.deltaTime;
        }
    }
}