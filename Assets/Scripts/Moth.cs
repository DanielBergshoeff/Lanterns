using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moth : MonoBehaviour
{
    public float UpdateTime = 2f;
    public float FlySpeed = 0.5f;

    private FireSource closestFireSource;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("GetClosestFireSource", UpdateTime);
    }

    private void Update() {
        if (closestFireSource != null)
            transform.position = Vector3.MoveTowards(transform.position, closestFireSource.transform.position, Time.deltaTime * FlySpeed);
    }

    private void GetClosestFireSource() {
        closestFireSource = FireSourceManager.Instance.GetClosestActiveSource(transform.position);
        Invoke("GetClosestFireSource", UpdateTime);
    }
}
