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

    public override bool Delight(FireSource target) {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        return true;
    }

    private void Update() {
        Core = transform.position;
        if (closestFireSource == null)
            return;
        if ((closestFireSource.Core - transform.position).sqrMagnitude < minDistance)
            return;

        transform.position = Vector3.MoveTowards(transform.position, closestFireSource.Core, Time.deltaTime * FlySpeed);
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

    public override bool Light(Flame flame) {
        MyFlame = flame;
        Lit = true;
        MyFlame.gameObject.SetActive(false);
        return true;
    }

    public override bool CanReceiveLight() {
        return false;
    }

    public override bool CanSendLight() {
        return false;
    }
}
