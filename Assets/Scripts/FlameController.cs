using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameController : MonoBehaviour
{
    public static FlameController Instance;
    public bool InSource = false;

    public GameObject FlamePrefab;
    public float ThrowCooldown = 1f;
    private float lastThrow = 0f;

    public Material GlassMat;
    public Material LitMat;

    private FireSource targetFireSource;
    private FireSource currentFireSource;
    private float lerpToSource = 0f;
    private Vector3 startPosition;

    private bool switchingFireSource = false;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(switchingFireSource) {
            lerpToSource += Time.deltaTime;
            if (lerpToSource < 1f) {
                transform.position = Vector3.Lerp(startPosition, targetFireSource.transform.position, lerpToSource);
            }
            else {
                transform.position = targetFireSource.transform.position;
                switchingFireSource = false;
                currentFireSource = targetFireSource;
                currentFireSource.Activated = true;
                targetFireSource = null;
            }
        }

        if (InSource)
            return;

        if (Input.GetMouseButtonDown(0)) {
            ThrowFlame();
        }
    }

    public void NewFireSource(FireSource fs) {
        targetFireSource = fs;
        if (currentFireSource != null)
            currentFireSource.Activated = false;

        startPosition = transform.position;
        switchingFireSource = true;
        lerpToSource = 0f;
    }

    private void ThrowFlame() {
        if (Time.time - lastThrow < ThrowCooldown)
            return;

        lastThrow = Time.time;
        GameObject flame = Instantiate(FlamePrefab);
        flame.transform.position = transform.position;
        flame.GetComponent<Rigidbody>().AddForce(transform.forward * 1000f);
    }
}
