using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Progression")]
    public int currentDay = 1;
    public float realSecondsPerDay = 60f;

    [Header("Financial System")]
    public int currentMoney = 10000;
    public int dailyCost = 1500;

    [Header("UI References")]
    public TextMeshProUGUI dayTextUI;
    public TextMeshProUGUI moneyTextUI;
    public TextMeshProUGUI timeTextUI;
    public GameObject gameOverPanel;
    public TextMeshProUGUI interactTextUI;

    private bool isGameOver = false;
    private float dayStartTime;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // TỰ ĐỘNG TÌM KIẾM CÁC Ô TEXT THEO TÊN NẾU BẠN CHƯA KÉO THẢ
        if (timeTextUI == null)
        {
            GameObject foundText = GameObject.Find("Text_DateTime");
            if (foundText != null)
            {
                timeTextUI = foundText.GetComponent<TextMeshProUGUI>();
            }
        }

        dayStartTime = Time.time;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        UpdateUI();
        HideInteractText();
    }

    private void Update()
    {
        if (isGameOver) return;

        float elapsedSeconds = Time.time - dayStartTime;

        if (elapsedSeconds >= realSecondsPerDay || Input.GetKeyDown(KeyCode.T))
        {
            AdvanceToNextDay();
        }

        UpdateUI();
    }

    private void AdvanceToNextDay()
    {
        currentDay++;
        currentMoney -= dailyCost;
        dayStartTime = Time.time;

        if (currentMoney < 0)
        {
            TriggerGameOver();
        }
    }

    private void TriggerGameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void UpdateUI()
    {
        if (dayTextUI != null) dayTextUI.text = "Day: " + currentDay + ".";
        if (moneyTextUI != null) moneyTextUI.text = "Money: " + currentMoney + "VND";

        if (timeTextUI != null)
        {
            float elapsedSeconds = Time.time - dayStartTime;

            // Dùng trực tiếp số giây thực tế nhân với hệ số tốc độ (ví dụ 1 giây thực tế = 10 phút trong game)
            // Cách này giúp thời gian nhích lên từng phút rất rõ ràng ngay trước mắt bạn
            float totalMinutes = 480f + (elapsedSeconds * 10f);

            int currentTotalMins = Mathf.FloorToInt(totalMinutes) % 1440;

            int hour24 = currentTotalMins / 60;
            int minute = currentTotalMins % 60;

            string ampm = hour24 >= 12 ? "PM" : "AM";
            int hour12 = hour24 % 12;
            if (hour12 == 0) hour12 = 12;

            string timeString = $"{hour12:00}:{minute:00} {ampm}";

            System.DateTime baseDate = new System.DateTime(2026, 2, 9).AddDays(currentDay - 1);
            string dateString = baseDate.ToString("yyyy.MM.dd");

            timeTextUI.text = timeString + "\n" + dateString;
        }
    }

    public void ShowInteractText(string message)
    {
        if (interactTextUI != null)
        {
            interactTextUI.text = message;
            interactTextUI.gameObject.SetActive(true);
        }
    }

    public void HideInteractText()
    {
        if (interactTextUI != null)
        {
            interactTextUI.gameObject.SetActive(false);
        }
    }
}
//