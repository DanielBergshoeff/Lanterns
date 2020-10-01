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
        transform.position = FireController.Instance.transform.position + Vector3.up * 2f;
    }

    public override bool Delight() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        return true;
    }

    private void Update() {
        float f = followingPlayer ? 2f : 0f;
        if (closestFireSource == null)
            return;
        if ((closestFireSource.transform.position + Vector3.up * f - transform.position).sqrMagnitude < minDistance)
            return;

        if(followingPlayer)
            transform.position = Vector3.MoveTowards(transform.position, closestFireSource.transform.position + Vector3.up * f, Time.deltaTime * FlySpeed);
        else
            transform.position = Vector3.MoveTowards(transform.position, ((Lantern)closestFireSource).MyPointLight.transform.position, Time.deltaTime * FlySpeed);
    }

    private void GetClosestFireSource() {
        closestFireSource = FireSourceManager.Instance.GetClosestActiveLantern(transform.position, LightDetectionRange);
        followingPlayer = false;
        if (closestFireSource == null) {
            float playerDist = (transform.position - FireController.Instance.transform.position).sqrMagnitude;
            if (playerDist < LightDetectionRange * LightDetectionRange) {
                closestFireSource = FireController.Instance;
                followingPlayer = true;
            }
            else {
                Invoke("GetClosestFireSource", UpdateTime);
                return;
            }
        }

        transform.rotation = Quaternion.LookRotation(transform.forward);
        Invoke("GetClosestFireSource", UpdateTime);
    }

    public override bool Light() {
        Lit = true;
        return true;
    }
}
