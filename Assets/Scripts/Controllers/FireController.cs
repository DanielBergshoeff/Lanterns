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

    public bool NewAimSystem = false;


    private Vector2 rightStickValues;

    private float fireXPosition;
    private float fireYPosition;
    private bool startFire = false;
    private bool stopFire = false;
    private bool mouseUsed = false;

    public Image FlameImage;
    public GameObject EndScreen;
    private Sprite[] flameSprites;

    private Color emissionColor;

    public static int LatestBarrier = 0;

    private void Awake() {
        Instance = this;

        LoadSpriteSheet();
        EndScreen.SetActive(false);
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
        if (!NewAimSystem) {
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
            FreeLookCam.Instance.OnLeftShoulder(startFire);
        }
        else {
            startFire = !startFire;
            AimTransformLight.gameObject.SetActive(startFire);
        }
    }

    private void OnRightMouse() {
        if (startFire)
            return;

        mouseUsed = true;
        if (!NewAimSystem) {
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
                AimTransformDouse.gameObject.SetActive(false);
            }
            FreeLookCam.Instance.OnRightShoulder(stopFire);
        }
        else {
            stopFire = !stopFire;
            AimTransformDouse.gameObject.SetActive(stopFire);
        }
    }

    /// <summary>
    /// While user presses the left shoulder button (or left mouse button), light fires at mouse position
    /// </summary>
    private void OnLeftShoulder() {
        if (stopFire)
            return;

        mouseUsed = false;


        if (!NewAimSystem) {
            if (!startFire) {
                startFire = true;
                AimTransform = AimTransformLight;
                AimTransform.gameObject.SetActive(true);
            }
            else {
                startFire = false;
                AimTransform.gameObject.SetActive(false);
            }
            FreeLookCam.Instance.OnLeftShoulder(startFire);
        }
        else {
            startFire = !startFire;
            AimTransformLight.gameObject.SetActive(startFire);
        }
    }

    /// <summary>
    /// While user presses the right shoulder button (or right mouse button), douse fires at mouse position
    /// </summary>
    private void OnRightShoulder() {
        if (startFire)
            return;

        mouseUsed = false;

        if (!NewAimSystem) {
            if (!stopFire) {
                stopFire = true;
                AimTransform = AimTransformDouse;
                AimTransform.gameObject.SetActive(true);
            }
            else {
                stopFire = false;
                AimTransform.gameObject.SetActive(false);
            }
            FreeLookCam.Instance.OnRightShoulder(stopFire);
        }
        else {
            stopFire = !stopFire;
            AimTransformDouse.gameObject.SetActive(stopFire);
        }
    }

    /// <summary>
    /// Update aiming position based on user input
    /// </summary>
    private void AimFire() {
        if (!NewAimSystem) {
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
                if (Mathf.Abs(AimTransform.position.x) / (Screen.width / 2f) > 0.75f){
                    FreeLookCam.Instance.OnRightShoulder(false);
                }
                else {
                    FreeLookCam.Instance.OnRightShoulder(true);
                }
            }
            else {
                AimTransform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
            }

            float x = Mathf.Abs(AimTransform.position.x) / Screen.width;
            float y = Mathf.Abs(AimTransform.position.y) / Screen.height;

            if (x > 0.8f || x < 0.2f || y > 0.8f || y < 0.2f) {
                if (x > 0.8f)
                    x = 1f - ((1f - x) / 0.2f);
                else if (x < 0.2f)
                    x = (1f - (x / 0.2f)) * -1f;
                else
                    x = 0f;

                if (y > 0.8f)
                    y = 1f - ((1f - y) / 0.2f);
                else if (y < 0.2f)
                    y = (1f - (y / 0.2f)) * -1f;
                else
                    y = 0f;

                FreeLookCam.Instance.HandleRotationMovement(x, y);
            }
        }
    }

    /// <summary>
    /// Raycast to find a lit firesource
    /// </summary>
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

    /// <summary>
    /// Raycast to find an unlit firesource
    /// </summary>
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

    /// <summary>
    /// FireController can always receive light
    /// </summary>
    /// <returns></returns>
    public override bool CanReceiveLight() {
        return true;
    }


    /// <summary>
    /// If FireController has more than 0 flames return true;
    /// </summary>
    /// <returns></returns>
    public override bool CanSendLight() {
        if (MyFlames.Count <= 0)
            return false;

        return true;
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.CompareTag("EndScreen")) {
            EndScreen.SetActive(true);

            gameObject.GetComponent<PlayerController>().enabled = false;
            enabled = false;
        }
    }
}
