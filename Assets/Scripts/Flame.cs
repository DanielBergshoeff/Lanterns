using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flame : MonoBehaviour
{
    public bool Active = false;
    public FireSource TargetSource;
    public float TravelDuration = 1f;

    private Vector3 startPosition;
    private float startTime = 0f;

    // Update is called once per frame
    void Update()
    {
        if (!Active)
            return;

        float t = Time.time - startTime;

        if(t < TravelDuration) {
            transform.position = Vector3.Slerp(startPosition, TargetSource.Core, t / TravelDuration);
        }
        else {
            transform.position = TargetSource.Core;
            Active = false;
            if (!TargetSource.Light(this)) {
                Destroy(gameObject);
            }
        }
    }

    public void SetToActive(FireSource target) {
        TargetSource = target;
        startPosition = transform.position;
        Active = true;
        startTime = Time.time;
    }
}
