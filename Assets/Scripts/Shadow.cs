using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    public float UpdateTime = 2f;
    public float FlySpeed = 0.5f;
    public float LightEatDisplacement = 1f;
    public float LightDetectionRange = 20f;

    private FireSource closestFireSource;
    private bool dying = false;
    private float dyingProcess = 0f;
    private float barrierScale = 0f;
    private MeshRenderer myMeshRenderer;
    private SphereCollider myCollider;

    private float startDisplacement = 0.18f;
    private float currentDisplacement = 0f;
    private bool eating = false;
    private FireSource eatingFs;
    private bool eatingPlayer = false;

    // Start is called before the first frame update
    void Start()
    {
        myMeshRenderer = GetComponent<MeshRenderer>();
        myCollider = GetComponent<SphereCollider>();
        myMeshRenderer.material = Instantiate(myMeshRenderer.material);
        Invoke("GetClosestFireSource", UpdateTime);
        startDisplacement = myMeshRenderer.material.GetFloat("_Displacement");
    }

    // Update is called once per frame
    void Update()
    {
        if (closestFireSource != null)
            transform.position = Vector3.MoveTowards(transform.position, closestFireSource.transform.position, Time.deltaTime * FlySpeed);

        if (dying) {
            dyingProcess += Time.deltaTime * 0.5f;
            if(dyingProcess > 1f) {
                dyingProcess = 1f;
                dying = false;
                Destroy(gameObject);
            }

            myMeshRenderer.material.SetFloat("_DiffusionStrength", barrierScale / 2f + dyingProcess * transform.localScale.x * 2f);
        }

        if (eating) {
            currentDisplacement += Time.deltaTime * 0.3f;
            myMeshRenderer.material.SetFloat("_Displacement", currentDisplacement);
            if(currentDisplacement >= LightEatDisplacement) {
                eating = false;
                if (eatingPlayer) {
                    StartDeath(eatingFs.transform);
                    eatingPlayer = false;
                }
                eatingFs.Delight();
            }
        }
        else if(currentDisplacement > startDisplacement) {
            currentDisplacement -= Time.deltaTime;
            myMeshRenderer.material.SetFloat("_Displacement", currentDisplacement);
        }
    }

    private void GetClosestFireSource() {
        closestFireSource = FireSourceManager.Instance.GetClosestActiveSource(transform.position, LightDetectionRange);
        Invoke("GetClosestFireSource", UpdateTime);
    }

    private void StartDeath(Transform other) {
        barrierScale = other.localScale.x;
        dying = true;
        dyingProcess = 0f;
        myMeshRenderer.material.SetVector("_DiffusePosition", other.position);
        myCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other) {
        if (eating)
            return;

        if (other.CompareTag("Barrier")) {
            StartDeath(other.transform);
        }
        else if (other.CompareTag("FireSource") || other.CompareTag("Player")) {
            FireSource fs = other.GetComponent<FireSource>();
            if (fs != null && fs.Lit && fs == closestFireSource) {
                EatLight(fs);
                if (other.CompareTag("Player")) {
                    eatingPlayer = true;
                }
            }
        }
        else if (other.CompareTag("Beam")) {
            StartDeath(transform);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (!eating)
            return;

        if (other.CompareTag("FireSource") || other.CompareTag("Player")) {
            if (!eating)
                return;
            FireSource fs = other.GetComponent<FireSource>();

            if (eatingFs != fs)
                return;

            eating = false;
            eatingPlayer = false;
            eatingFs = null;
        }
    }

    private void EatLight(FireSource fs) {
        eating = true;
        currentDisplacement = startDisplacement;
        eatingFs = fs;
    }
}
