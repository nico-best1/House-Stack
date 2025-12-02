using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Vuforia;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Objeto contenedor del juego")]
    public GameObject gameBase;

    public ObserverBehaviour imageTarget;
    public Transform spawnPoint;
    public Transform houseContainer;

    public float maxRangeHouses = 50;
    public GameObject[] housesList;

    private GameObject mainHouse;

    [Header("UI")]
    public GameObject instructionText;
    public TextMeshProUGUI counterText;

    [Header("Final UI")]
    public GameObject endGameUI;
    public TextMeshProUGUI finalText;
    public Button restartButton;

    [Header("Sonidos")]
    public AudioClip winSFX;
    public AudioClip loseSFX;

    public ParticleSystem confetti;

    public enum FinalState { NONE, STARTWIN, FAILWIN, WIN, LOSE }

    private FinalState estadoFinal = FinalState.NONE;
    private bool yaMostroResultado = false;

    private float counterTime = 0;
    public float maxTime = 5.0f;

    // NUEVO: control de colisión con el techo
    private bool contactoConTecho = false;
    private float tiempoEnTecho = 0f;
    public float tiempoParaGanar = 1.0f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (gameBase != null)
            gameBase.SetActive(false);

        if (endGameUI != null)
            endGameUI.SetActive(false);
    }

    private void Start()
    {
        estadoFinal = FinalState.NONE;
        counterTime = 0;
        mainHouse = null;
        yaMostroResultado = false;
        restartButton.onClick.AddListener(RestartGame);
    }

    void OnEnable()
    {
        if (imageTarget != null)
            imageTarget.OnTargetStatusChanged += OnTargetStatusChanged;
    }

    void OnDisable()
    {
        if (imageTarget != null)
            imageTarget.OnTargetStatusChanged -= OnTargetStatusChanged;
    }

    void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        if (gameBase == null) return;

        if (status.Status == Status.TRACKED)
        {
            gameBase.SetActive(true);
            if (instructionText != null)
                instructionText.SetActive(false);
        }
        else
        {
            gameBase.SetActive(false);
            if (instructionText != null)
                instructionText.SetActive(true);
        }
    }

    void Update()
    {
        if (!gameBase.activeSelf) return;

        // Colisión con techo
        if (estadoFinal != FinalState.WIN && estadoFinal != FinalState.LOSE)
        {
            if (contactoConTecho)
            {
                tiempoEnTecho += Time.deltaTime;
                if (tiempoEnTecho >= tiempoParaGanar)
                {
                    StartEnding();
                }
            }
        }

        // Progreso hacia victoria total
        if (estadoFinal == FinalState.STARTWIN && counterTime >= maxTime)
        {
            estadoFinal = FinalState.WIN;
        }

        // Lógica de lanzar casas
        if ((estadoFinal != FinalState.WIN && estadoFinal != FinalState.LOSE) &&
            ((Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began) || Input.GetKeyDown(KeyCode.Space)))
        {
            LaunchHouse();
        }

        if (estadoFinal != FinalState.WIN && estadoFinal != FinalState.LOSE)
        {
            foreach (Transform child in houseContainer)
            {
                if (child == null) continue;

                float distancia = Vector3.Distance(child.position, spawnPoint.position);
                if (distancia > maxRangeHouses)
                {
                    if (child.gameObject == mainHouse)
                        mainHouse = null;

                    Destroy(child.gameObject);
                    estadoFinal = FinalState.LOSE;
                }
            }

            if (mainHouse == null)
            {
                AddRandomHouse();
            }
            else
            {
                HouseComponent HCmainHouse = mainHouse.GetComponent<HouseComponent>();
                if (HCmainHouse != null && HCmainHouse.GetIfCollide())
                {
                    AddRandomHouse();
                }
            }
        }

        if (mainHouse != null && mainHouse.TryGetComponent<BoxCollider>(out BoxCollider bc))
        {
            if (!bc.enabled)
                mainHouse.transform.position = spawnPoint.position;
        }
        else
        {
            mainHouse = null;
        }

        // Estados finales
        switch (estadoFinal)
        {
            case FinalState.STARTWIN:
                counterTime += Time.deltaTime;
                ShowCounter();
                break;
            case FinalState.FAILWIN:
                estadoFinal = FinalState.NONE;
                counterTime = 0;
                counterText.text = "";
                break;
            case FinalState.WIN:
                if (!yaMostroResultado)
                {
                    WhenWin();
                    yaMostroResultado = true;
                }
                break;
            case FinalState.LOSE:
                if (!yaMostroResultado)
                {
                    WhenLose();
                    yaMostroResultado = true;
                }
                break;
        }
    }

    public void NotifyCeilingContactStart()
    {
        contactoConTecho = true;
        tiempoEnTecho = 0f;
    }

    public void NotifyCeilingContactEnd()
    {
        contactoConTecho = false;
        FailEnding();
    }

    void AddRandomHouse()
    {
        if (estadoFinal == FinalState.WIN || estadoFinal == FinalState.LOSE) return;
        if (housesList.Length <= 0) return;

        int numero = Random.Range(0, housesList.Length);

        float randomY = Random.Range(0f, 360f);
        Quaternion rotInicial = gameBase.transform.rotation;
        Quaternion rotacion = Quaternion.Euler(0f, randomY, 0f);
        rotacion = rotInicial * rotacion;

        GameObject nuevaCasa = Instantiate(housesList[numero], spawnPoint.position, rotacion, houseContainer);
        Rigidbody RGnuevaCasa = nuevaCasa.GetComponent<Rigidbody>();
        RGnuevaCasa.useGravity = false;

        BoxCollider BCnuevaCasa = nuevaCasa.GetComponent<BoxCollider>();
        BCnuevaCasa.enabled = false;

        mainHouse = nuevaCasa;

        GravedadPersonalizada gpCasa = mainHouse.GetComponent<GravedadPersonalizada>();

        if (gpCasa != null)
        {
            gpCasa.enabled = false;
        }

        spawnPoint.GetComponent<PointMovement>().ChangePositions();
    }

    void LaunchHouse()
    {
        if (estadoFinal == FinalState.WIN || estadoFinal == FinalState.LOSE || mainHouse == null) return;

        //Rigidbody rgCasa = mainHouse.GetComponent<Rigidbody>();
        //rgCasa.useGravity = true;

        GravedadPersonalizada gpCasa = mainHouse.GetComponent<GravedadPersonalizada>();

        if (gpCasa != null)
        {
            gpCasa.enabled = true;
        }


        BoxCollider bcCasa = mainHouse.GetComponent<BoxCollider>();
        bcCasa.enabled = true;

        HouseComponent hc = mainHouse.GetComponent<HouseComponent>();
        if (hc != null)
        {
            hc.isLaunched = true;
        }
    }

    public void StartEnding()
    {
        SetAllHousesMat(true);
        estadoFinal = FinalState.STARTWIN;
    }

    public void FailEnding()
    {
        SetAllHousesMat(false);
        estadoFinal = FinalState.FAILWIN;
    }

    void SetAllHousesMat(bool active)
    {
        foreach (Transform child in houseContainer)
        {
            if (child == null) continue;

            MaterialOscilation matOs = child.GetComponent<MaterialOscilation>();
            if (matOs == null && child.childCount > 0)
            {
                matOs = child.GetChild(0).GetComponent<MaterialOscilation>();
            }

            if (matOs != null)
            {
                matOs.enabled = active;
            }
        }
    }

    IEnumerator ReloadSceneSafely()
    {
        this.enabled = false;
        yield return null;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void RestartGame()
    {
        StartCoroutine(ReloadSceneSafely());
    }

    void ShowCounter()
    {
        int segundos = (int)(maxTime - counterTime);
        counterText.text = segundos.ToString();
    }

    void WhenWin()
    {
        ShowFinalUI("¡YOU WIN!");
        confetti.Play();
        AudioManager.Instance.PlaySFX(winSFX);
    }

    void WhenLose()
    {
        ShowFinalUI("YOU LOSE");
        AudioManager.Instance.PlaySFX(loseSFX);
    }

    void ShowFinalUI(string message)
    {
        if (endGameUI != null && finalText != null)
        {
            endGameUI.SetActive(true);
            finalText.text = message;
        }
    }
}
