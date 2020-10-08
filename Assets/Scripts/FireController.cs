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
    public float MaxFireRange = 100f;

    [Header("Flame info")]
    public float MinFlameSize = 0.3f;
    public float MaxFlameSize = 1f;
    public float MinDistance = 1f;
    public float MaxDistance = 10f;


    private Vector2 rightStickValues;

    private float fireXPosition;
    private float fireYPosition;
    private bool startFire = false;
    private bool stopFire = false;
    private bool mouseUsed = false;

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
            Light(go.GetComponent<Flame>());
        }
    }

    public override bool Light(Flame flame) {
        Lit = true;

        if (MyFlames.Count >= MaxFirePower)
            return false;

        MyFlames.Add(flame);
        float f = (float)MyFlames.Count / MaxFirePower;
        PlayerMat.SetColor("_EmissionColor", emissionColor * ((MaxEmissionIntensity - MinEmissionIntensity) * f + MinEmissionIntensity));
        PlayerLight.intensity = (MyFlames.Count / MaxFirePower) * MaxLightIntensity;
        FlameImage.sprite = flameSprites[MyFlames.Count];
        return true;
    }

    public override bool Delight(FireSource target) {
        if (MyFlames.Count <= 0)
            return false;

        if (target != null) {
            MyFlames[MyFlames.Count - 1].SetToActive(target);
        }
        else {
            Destroy(MyFlames[MyFlames.Count - 1].gameObject);
        }
        
        MyFlames.RemoveAt(MyFlames.Count - 1);
        float f = (float)MyFlames.Count / MaxFirePower;
        PlayerMat.SetColor("_EmissionColor", emissionColor * ((MaxEmissionIntensity - MinEmissionIntensity) * f + MinEmissionIntensity));
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

        foreach(Flame flame in Flame.AllFlames) {
            float dist = Vector3.Distance(transform.position, flame.transform.position);
            dist = Mathf.Clamp(dist, MinDistance, MaxDistance);
            dist = dist / (MaxDistance - MinDistance);
            dist = dist * (MaxFlameSize - MinFlameSize) + MinFlameSize;
            flame.UpdateSize(dist);
        }

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

    private void OnLeftMouse() {
        if (stopFire)
            return;

        mouseUsed = true;
        FreeLookCam.Instance.OnLeftShoulder();
        if (!startFire) {
            startFire = true;
            AimTransform = AimTransformLight;
            AimTransform.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            startFire = false;
            AimTransform.gameObject.SetActive(false);
        }
    }

    private void OnRightMouse() {
        if (startFire)
            return;

        mouseUsed = true;
        FreeLookCam.Instance.OnRightShoulder();
        if (!stopFire) {
            stopFire = true;
            AimTransform = AimTransformDouse;
            AimTransform.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            stopFire = false;
            AimTransform.gameObject.SetActive(false);
        }
    }

    private void OnLeftShoulder() {
        if (stopFire)
            return;

        mouseUsed = false;
        FreeLookCam.Instance.OnLeftShoulder();
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

    private void OnRightShoulder() {
        if (startFire)
            return;

        mouseUsed = false;
        FreeLookCam.Instance.OnRightShoulder();
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
        if (!mouseUsed) {
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
        else {
            AimTransform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
        }
    }

    private void CheckForLitSource() {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(AimTransform.position.x, AimTransform.position.y, 0f));
        if (Physics.Raycast(ray, out hit, MaxFireRange, LitLayer)) {
            if (hit.collider.CompareTag("FireSource")) {
                FireSource fs = hit.collider.GetComponent<FireSource>();
                if (fs.CanSendLight())
                    fs.Delight(this);
            }
        }
    }

    private void CheckForUnlitSource() {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(AimTransform.position.x, AimTransform.position.y, 0f));
        if (Physics.Raycast(ray, out hit, MaxFireRange, UnlitLayer)) {
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
