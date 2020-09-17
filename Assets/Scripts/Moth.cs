using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Moth : FireSource
{
    public float UpdateTime = 2f;
    public float FlySpeed = 0.5f;
    public float LightDetectionRange = 20f;

    private float minDistance = 0.5f;
    private FireSource closestFireSource;
    private bool followingPlayer;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Invoke("GetClosestFireSource", UpdateTime);
    }

    public override bool Delight() {
        if (FireController.Instance.LatestBarrier == null)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        else {
            FireController.Instance.ReturnToBarrier();
            transform.position = FireController.Instance.transform.position + Vector3.up * 2f;
        }
        return true;
    }

    private void Update() {
        float f = followingPlayer ? 2f : 0f;
        if (closestFireSource == null)
            return;
        if ((closestFireSource.transform.position + Vector3.up * f - transform.position).sqrMagnitude < minDistance)
            return;

        transform.position = Vector3.MoveTowards(transform.position, closestFireSource.transform.position + Vector3.up * f, Time.deltaTime * FlySpeed);
        
    }

    private void GetClosestFireSource() {
        closestFireSource = FireSourceManager.Instance.GetClosestActiveSource(transform.position, LightDetectionRange, this);
        if (closestFireSource == null) {
            Invoke("GetClosestFireSource", UpdateTime);
            return;
        }

        if (closestFireSource.CompareTag("Player")) {
            followingPlayer = true;
        }
        else {
            followingPlayer = false;
        }
        transform.rotation = Quaternion.LookRotation(transform.forward);
        Invoke("GetClosestFireSource", UpdateTime);
    }

    public override bool Light() {
        Lit = true;
        return true;
    }
}
