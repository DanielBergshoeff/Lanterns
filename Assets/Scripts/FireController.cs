using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FireController : FireSource
{
    public static FireController Instance;
    public Animator PlayerAnimator;

    public RectTransform AimTransform;
    public RectTransform AimTransformLight;
    public RectTransform AimTransformDouse;
    public Material GlassMat;
    public Material LitMat;
    public Material LitGreenMat;
    public Material PlayerMat;
    public Light PlayerLight;

    public LayerMask UnlitLayer;
    public LayerMask LitLayer;

    public float MinEmissionIntensity = 0f;
    public float MaxEmissionIntensity = 2f;
    public float FirePower = 1f;

    private Vector2 rightStickValues;

    private float fireXPosition;
    private float fireYPosition;
    private bool startFire = false;
    private bool stopFire = false;

    private Color emissionColor;

    [System.NonSerialized]
    public BarrierSource LatestBarrier;

    private void Awake() {
        Instance = this;
    }

    public void ReturnToBarrier() {
        if (LatestBarrier == null)
            return;

        transform.position = LatestBarrier.transform.position;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        emissionColor = PlayerMat.GetColor("_EmissionColor");
        PlayerMat = Instantiate(PlayerMat);
        GetComponentInChildren<SkinnedMeshRenderer>().material = PlayerMat;
        base.Start();
    }

    public override bool Light() {
        Lit = true;
        FirePower += 0.2f;
        PlayerMat.SetColor("_EmissionColor", emissionColor * ((MaxEmissionIntensity - MinEmissionIntensity) * FirePower + MinEmissionIntensity));
        PlayerLight.intensity = FirePower;
        return true;
    }

    public override bool Delight() {
        FirePower -= 0.2f;
        PlayerMat.SetColor("_EmissionColor", emissionColor * ((MaxEmissionIntensity - MinEmissionIntensity) * FirePower + MinEmissionIntensity));
        PlayerLight.intensity = FirePower;

        if (FirePower <= 0f)
            Lit = false;

        return true;
    }

    // Update is called once per frame
    void Update()
    {
        if (startFire) {
            AimFire();
            CheckForUnlitSource();
        }

        if (stopFire) {
            AimFire();
            CheckForLitSource();
        }
    }

    private void OnRightStick(InputValue value) {
        FreeLookCam.Instance.OnRightStick(value);
        rightStickValues = value.Get<Vector2>();
    }

    private void OnRightShoulder() {
        FreeLookCam.Instance.OnRightShoulder();
        if (!startFire) {
            startFire = true;
            AimTransform = AimTransformLight;
            AimTransform.gameObject.SetActive(true);
        }
        else {
            startFire = false;
            AimTransform.gameObject.SetActive(false);
        }
    }

    private void OnLeftShoulder() {
        FreeLookCam.Instance.OnLeftShoulder();
        if (!stopFire) {
            stopFire = true;
            AimTransform = AimTransformDouse;
            AimTransform.gameObject.SetActive(true);
        }
        else {
            stopFire = false;
            AimTransform.gameObject.SetActive(false);
        }
    }

    private void AimFire() {
        if (rightStickValues.x > fireXPosition + 0.05f)
            fireXPosition += 3f * Time.deltaTime;
        else if (rightStickValues.x < fireXPosition - 0.05f) {
            fireXPosition -= 3f * Time.deltaTime;
        }

        if (rightStickValues.y > fireYPosition + 0.05f) {
            fireYPosition += 3f * Time.deltaTime;
        }
        else if (rightStickValues.y < fireYPosition - 0.05f) {
            fireYPosition -= 3f * Time.deltaTime;
        }

        AimTransform.position = new Vector3(Screen.width * fireXPosition / 2f + 0.5f * Screen.width, Screen.height * fireYPosition / 2f + 0.5f * Screen.height, 0f);
    }

    private void CheckForLitSource() {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * fireXPosition / 2f + 0.5f * Screen.width, Screen.height * fireYPosition / 2f + 0.5f * Screen.height, 0f));
        if (Physics.Raycast(ray, out hit, 100f, LitLayer)) {
            if (hit.collider.CompareTag("FireSource")) {
                FireSource fs = hit.collider.GetComponent<FireSource>();
                if (fs.Delight())
                    Light();
            }
        }
    }

    private void CheckForUnlitSource() {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * fireXPosition / 2f + 0.5f * Screen.width, Screen.height * fireYPosition / 2f + 0.5f * Screen.height, 0f));
        if (Physics.Raycast(ray, out hit, 100f, UnlitLayer)) {
            if (hit.collider.CompareTag("FireSource")) {
                if (FirePower > 0f) {
                    FireSource fs = hit.collider.GetComponent<FireSource>();
                    if (fs.Light())
                        Delight();
                }
            }
        }
    }
}
