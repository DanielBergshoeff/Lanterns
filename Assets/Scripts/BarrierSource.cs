using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierSource : FireSource
{
    public Light MyPointLight;
    public MeshRenderer MyRenderer;
    public GameObject Barrier;

    public AudioSource MyAudioSource;
    private bool expectingLight;

    private void Awake() {
        MyRenderer = GetComponent<MeshRenderer>();
        MyAudioSource = gameObject.AddComponent<AudioSource>();
        MyPointLight = GetComponentInChildren<Light>();
    }

    protected override void Start() {
        if (MyPointLight != null)
            Core = MyPointLight.transform.position;
        else
            Core = transform.position;
        //Don't remove
    }

    public override bool Light(Flame flame) {
        expectingLight = false;
        Lit = true;
        MyFlame = flame;
        MyRenderer.material = FireController.Instance.LitGreenMat;
        MyPointLight.enabled = true;
        Barrier.SetActive(true);
        gameObject.layer = 8;
        FireController.LatestBarrier = BarrierSourceManager.Instance.GetBarrierNr(this);
        MyAudioSource.PlayOneShot(AudioManager.Instance.LightBigFire, 0.1f);
        FireController.Instance.RefillFire();
        return false;
    }

    public override bool Delight(FireSource target) {
        return false;
    }

    public override bool CanReceiveLight() {
        if (Lit || expectingLight)
            return false;

        expectingLight = true;
        return true;
    }

    public override bool CanSendLight() {
        return false;
    }
}
