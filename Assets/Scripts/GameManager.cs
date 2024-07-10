using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Blade blade;
    [SerializeField] private Spawner spawner;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text comboText;
    [SerializeField] private Text livesText;
    [SerializeField] private Image fadeImage;
    [SerializeField] private GameObject MenuUi; 
    [SerializeField] private GameObject FinishUi; 
    [SerializeField] private Text finalScoreText; 
    [SerializeField] private Text highScoreText; 
    public AudioClip endGameSound;
    public AudioClip comboSound;

    private AudioSource audioSource;

    private int score;
    public int Score => score;

    private int lives;
    private const int maxLives = 3;
    private const float comboDuration = 1.0f;
    private List<float> sliceTimes = new List<float>();

    private bool gameStarted = false;

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        ShowMenuUi();
    }

    private void ShowMenuUi()
    {
        Time.timeScale = 0f;
        MenuUi.SetActive(true); 
        FinishUi.SetActive(false); 
        blade.enabled = false;
        spawner.enabled = false;
    }

    private void Update()
    {
        if (!gameStarted && Input.GetMouseButtonDown(0))
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        Time.timeScale = 1f;
        MenuUi.SetActive(false); 
        FinishUi.SetActive(false); 
        blade.enabled = true;
        spawner.enabled = true;
        gameStarted = true;

        NewGame();
    }

    private void NewGame()
    {
        ClearScene();

        score = 0;
        scoreText.text = score.ToString();
        comboText.text = "";

        lives = maxLives;
        UpdateLivesUI();
    }

    private void ClearScene()
    {
        Fruit[] fruits = FindObjectsOfType<Fruit>();

        foreach (Fruit fruit in fruits)
        {
            Destroy(fruit.gameObject);
        }

        Bomb[] bombs = FindObjectsOfType<Bomb>();

        foreach (Bomb bomb in bombs)
        {
            Destroy(bomb.gameObject);
        }
    }

    public void IncreaseScore(int points)
    {
        score += points;
        scoreText.text = score.ToString();

        float hiscore = PlayerPrefs.GetFloat("hiscore", 0);

        if (score > hiscore)
        {
            hiscore = score;
            PlayerPrefs.SetFloat("hiscore", hiscore);
        }
    }

    public void FruitSliced()
    {
        float currentTime = Time.time;
        sliceTimes.Add(currentTime);

        sliceTimes.RemoveAll(time => currentTime - time > comboDuration);

        int slicesInCombo = sliceTimes.Count;

        if (slicesInCombo > 1)
        {
            comboText.text = "Combo: x" + slicesInCombo.ToString();
        }
        else
        {
            comboText.text = "";
        }

        IncreaseScore(1);

        if (slicesInCombo >= 3)
        {
            audioSource.PlayOneShot(comboSound);
            sliceTimes.Clear();
        }
    }

    public void Explode()
    {
        lives--;
        UpdateLivesUI();

        if (lives <= 0)
        {
            audioSource.PlayOneShot(endGameSound);
            blade.enabled = false;
            spawner.enabled = false;

            StartCoroutine(ExplodeSequence());
        }
    }

    private void UpdateLivesUI()
    {
        livesText.text = "Lives: " + lives.ToString();
    }

    private IEnumerator ExplodeSequence()
    {
        float elapsed = 0f;
        float duration = 0.5f;

        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            fadeImage.color = Color.Lerp(Color.clear, Color.white, t);

            Time.timeScale = 1f - t;
            elapsed += Time.unscaledDeltaTime;

            yield return null;
        }

        yield return new WaitForSecondsRealtime(1f);

        ShowFinishUi();

        elapsed = 0f;

        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            fadeImage.color = Color.Lerp(Color.white, Color.clear, t);

            elapsed += Time.unscaledDeltaTime;

            yield return null;
        }
    }

    private void ShowFinishUi()
    {
        gameStarted = false;

        float hiscore = PlayerPrefs.GetFloat("hiscore", 0);

        finalScoreText.text = "Score: " + score.ToString();
        highScoreText.text = "High Score: " + hiscore.ToString();

        FinishUi.SetActive(true);

        StartCoroutine(WaitForRestart());
    }

    private IEnumerator WaitForRestart()
    {
        while (!Input.GetMouseButtonDown(0))
        {
            yield return null;
        }

        StartGame();
    }
}
