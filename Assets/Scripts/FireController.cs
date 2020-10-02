using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FireController : FireSource
{
    public static FireController Instance;
    public GameObject FlamePrefab;
    public List<Flame> MyFlames;

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
    public int StartFirePower = 6;
    public int MaxFirePower = 6;
    public float MaxLightIntensity = 0.5f;

    private Vector2 rightStickValues;

    private float fireXPosition;
    private float fireYPosition;
    private bool startFire = false;
    private bool stopFire = false;

    public Image FlameImage;
    private Sprite[] flameSprites;

    private Color emissionColor;

    public static int LatestBarrier = 0;

    private void Awake() {
        Instance = this;

        LoadSpriteSheet();
    }

    private void LoadSpriteSheet() {
        flameSprites = Resources.LoadAll<Sprite>("Spritesheet_LightsCount_Lanterns");
    }

    public void ReturnToBarrier() {
        transform.position = BarrierSourceManager.Instance.GetBarrierByNr(LatestBarrier).transform.position;

        if (LatestBarrier != 0)
            BarrierSourceManager.Instance.GetBarrierByNr(LatestBarrier).Light(null);
    }

    public void RefillFire() {
        while(MyFlames.Count < MaxFirePower) {
            GameObject go = Instantiate(FlamePrefab);
            go.transform.position = transform.position;
            Light(go.GetComponent<Flame>());
        }
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        emissionColor = PlayerMat.GetColor("_EmissionColor");
        PlayerMat = Instantiate(PlayerMat);
        GetComponentInChildren<SkinnedMeshRenderer>().material = PlayerMat;
        base.Start();

        ReturnToBarrier();
        SpawnFires();
    }

    private void SpawnFires() {
        for (int i = 0; i < StartFirePower; i++) {
            GameObject go = Instantiate(FlamePrefab);
            go.transform.position = transform.position;
            MyFlames.Add(go.GetComponent<Flame>());
        }
    }

    public override bool Light(Flame flame) {
        Lit = true;

        if (MyFlames.Count >= MaxFirePower)
            return false;


        MyFlames.Add(flame);
        PlayerMat.SetColor("_EmissionColor", emissionColor * ((MaxEmissionIntensity - MinEmissionIntensity) * MyFlames.Count + MinEmissionIntensity));
        PlayerLight.intensity = (MyFlames.Count / MaxFirePower) * MaxLightIntensity;
        FlameImage.sprite = flameSprites[MyFlames.Count];
        return true;
    }

    public override bool Delight(FireSource target) {
        if (MyFlames.Count <= 0)
            return false;

        if (target != null) {
            MyFlames[MyFlames.Count - 1].Active = true;
            MyFlames[MyFlames.Count - 1].TargetSource = target;
        }
        else {
            Destroy(MyFlames[MyFlames.Count - 1].gameObject);
        }
        
        MyFlames.RemoveAt(MyFlames.Count - 1);
        PlayerMat.SetColor("_EmissionColor", emissionColor * ((MaxEmissionIntensity - MinEmissionIntensity) * MyFlames.Count + MinEmissionIntensity));
        PlayerLight.intensity = (MyFlames.Count / MaxFirePower) * MaxLightIntensity;
        FlameImage.sprite = flameSprites[MyFlames.Count];

        if (MyFlames.Count <= 0f)
            Lit = false;

        return true;
    }

    // Update is called once per frame
    void Update()
    {
        Core = transform.position + Vector3.up * 1f;

        if (startFire) {
            AimFire();
            CheckForUnlitSource();
        }

        if (stopFire) {
            AimFire();
            CheckForLitSource();
        }

        float flameHeight = Mathf.Sin(Time.time * 2f) / 3.6f + 0.5f;

        for (int i = 0; i < MyFlames.Count; i++) {
            MyFlames[i].transform.position = Vector3.MoveTowards(MyFlames[i].transform.position, transform.position + Quaternion.Euler(0f, i * (360f / MyFlames.Count), 0f) * transform.forward + Vector3.up * flameHeight, 3f * Time.deltaTime);
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
                if (fs.CanSendLight())
                    fs.Delight(this);
            }
        }
    }

    private void CheckForUnlitSource() {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * fireXPosition / 2f + 0.5f * Screen.width, Screen.height * fireYPosition / 2f + 0.5f * Screen.height, 0f));
        if (Physics.Raycast(ray, out hit, 100f, UnlitLayer)) {
            if (hit.collider.CompareTag("FireSource")) {
                if (MyFlames.Count > 0) {
                    FireSource fs = hit.collider.GetComponent<FireSource>();

                    if (fs.CanReceiveLight()) {
                        Delight(fs);
                    }
                }
            }
        }
    }

    public override bool CanReceiveLight() {
        return true;
    }

    public override bool CanSendLight() {
        if (MyFlames.Count <= 0)
            return false;

        return true;
    }
}
