using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; } // Singleton

    [Header("Score Datas"), Space(10)]
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text highScoreText;
    private int score = 0;
    private int highSchore = 0;

    [Header("Upcoming Atom Data"), Space(10)]
    AtomManager atomManager;
    [SerializeField] public AtomData upcomingAtomData => atomManager.upcomingAtomData;
    [SerializeField] public Image upcomingAtomImage;

    [SerializeField] GameObject gameOverPanel;
    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        atomManager = GameObject.Find("AtomManager").GetComponent<AtomManager>();
        highSchore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = highSchore.ToString("00000");
        scoreText.text = score.ToString("00000");

        upcomingAtomImage.color = Color.red;
    }

    void OpenPanel(GameObject panel)
    {
        panel.SetActive(true);
    }

    void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
    }

    public void UpdateScore(int points)
    {
        score += points;
        scoreText.text = score.ToString("00000");

        if (score > highSchore)
        {
            highSchore = score;
            highScoreText.text = highSchore.ToString("00000");
            PlayerPrefs.SetInt("HighScore", highSchore);
        }
    }
    public void UpdateUpcomingAtom()
    {
        upcomingAtomImage.color = upcomingAtomData.atomColor;
    }
    public void GameOver()
    {
        Debug.Log("Game Over triggered in UIManager.");
        gameOverPanel.SetActive(true);
        LeanTween.scale(gameOverPanel, Vector3.one, 0.5f).setEaseOutBack();
        
    }
    public void RestartTheGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("Restarting the game...");
    }
}