using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Moth : FireSource
{
    public float UpdateTime = 2f;
    public float FlySpeed = 0.5f;
    public float LightDetectionRange = 20f;

    private FireSource closestFireSource;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Invoke("GetClosestFireSource", UpdateTime);
    }

    public override void Delight() {
        base.Delight();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Update() {
        if (closestFireSource != null)
            transform.position = Vector3.MoveTowards(transform.position, closestFireSource.transform.position, Time.deltaTime * FlySpeed);
    }

    private void GetClosestFireSource() {
        closestFireSource = FireSourceManager.Instance.GetClosestActiveSource(transform.position, LightDetectionRange, this);
        transform.rotation = Quaternion.LookRotation(transform.forward);
        Invoke("GetClosestFireSource", UpdateTime);
    }
}
