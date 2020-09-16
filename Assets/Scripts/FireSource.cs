using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FireSource : MonoBehaviour
{
    public GameObject FlamePrefab;
    public float ThrowCooldown;
    public bool Activated = false;
    protected float lastThrow = 0f;

    public bool Lit = false;

    protected virtual void Start() {
        FireSourceManager.Instance.AddFireSource(this);
        if (Lit) {
            Light();
        }
    }

    void Update() {
        if (!Activated)
            return;

        if (Input.GetMouseButtonDown(0)) {
            ThrowFlame();
        }
    }

    public virtual void Light() {
        Lit = true;
    }

    public virtual void Delight() {
        Lit = false;
    }

    protected void ThrowFlame() {
        if (Time.time - lastThrow < ThrowCooldown)
            return;

        lastThrow = Time.time;
        GameObject flame = Instantiate(FlamePrefab);
        flame.transform.position = transform.position + transform.right * 1f;
        flame.GetComponent<Rigidbody>().AddForce(FlameController.Instance.transform.forward * 1000f);
    }
}
