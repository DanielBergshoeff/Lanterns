using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flame : MonoBehaviour
{
    public static List<Flame> AllFlames;
    public bool Active = false;
    public FireSource TargetSource;
    public float TravelDuration = 1f;

    private Vector3 startPosition;
    private float startTime = 0f;

    private List<ParticleSystem> myParticleSystems;
    private Dictionary<ParticleSystem, Vector3> particleToScale;

    private void Awake() {
        if (AllFlames == null)
            AllFlames = new List<Flame>();

        AllFlames.Add(this);

        myParticleSystems = new List<ParticleSystem>(gameObject.GetComponentsInChildren<ParticleSystem>());
        particleToScale = new Dictionary<ParticleSystem, Vector3>();
        foreach(ParticleSystem ps in myParticleSystems) {
            particleToScale.Add(ps, ps.transform.localScale);
        }
    }

    private void OnDestroy() {
        AllFlames.Remove(this);
    }

    public void UpdateSize(float size) {
        foreach (ParticleSystem ps in myParticleSystems) {
            ps.transform.localScale = particleToScale[ps] * size;
        }
    }

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
