using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flame : MonoBehaviour
{
    public bool Active = false;
    public FireSource TargetSource;
    public float MoveSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!Active)
            return;

        Vector3 direction = TargetSource.Core - transform.position;
        if(direction.sqrMagnitude > 0.005f) {
            Vector3 heading = direction.normalized * Mathf.Clamp(direction.magnitude, 1f, 1000f);
            transform.position = transform.position + heading * MoveSpeed * Time.deltaTime;
        }
        else {
            Active = false;
            if (!TargetSource.Light(this)) {
                Destroy(gameObject);
            }
        }
    }
}
