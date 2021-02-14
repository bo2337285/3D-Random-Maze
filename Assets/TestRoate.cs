using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRoate : MonoBehaviour
{
    // public Transform player;
    public Transform target;
    public float speed = 5.0f;
    
    void Update() {
        Vector3 relativePos = target.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(relativePos);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.time * speed);//物体想慢慢朝向目标
        
    }
}
