using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotMachineController : MonoBehaviour
{
    [Header("Gold")]
    [SerializeField, Min(10)] private int startingBalance = 500;
    [SerializeField] private TextMeshProUGUI balanceText;
    [SerializeField] private TextMeshProUGUI betText;
    [SerializeField] private List<int> bets;

    [Header("UI")]
    [SerializeField] private Button lowerBetButton;
    [SerializeField] private Button raiseBetButton;
    [SerializeField] private CanvasGroup gameUI;

    [Header("Dialog Box")]
    [SerializeField] private GameObject exitDialog;
    [SerializeField] private GameObject gameOverDialog;
    [SerializeField] private GameObject dialogBlocker;

    [Header("Slot Machine")]
    [SerializeField] private GameObject reelsHolder;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private PayTableValues payTable;
    
    private readonly List<SlotReel> reels = new();

    private int betIndex;
    private bool gameActive = true;
    private Coroutine messageCoroutine;
    private Animator leverAnimator;

    public int Balance { get; private set; }
    public int CurrentBet => bets[betIndex];
    public bool CanPlay => gameActive && !exitDialog.activeSelf && !gameOverDialog.activeSelf;

    private void Awake()
    {
        foreach(Transform child in reelsHolder.transform)
        {
            if (child != null)
                reels.Add(child.GetComponent<SlotReel>());
        }
    }

    private void Start()
    {
        leverAnimator = GetComponentInChildren<SlotLever>().GetComponent<Animator>();
        NewGame();
        UpdateUI();
    }

    public void Restart()
    {
        if (gameOverDialog.activeSelf)
            NewGame();
    }

    private void NewGame()
    {
        StopMessage();
        Balance = startingBalance;
        betIndex = 0;

        gameOverDialog.SetActive(false);
        exitDialog.SetActive(false);

        HomePositions();
        statusText.text = "Ready!";
        SetGameActive(true);
    }

    public void HomePositions()
    {
        var symbols = new List<Symbol>(payTable.threeMatches.Count);
        foreach (var match in payTable.threeMatches)
            symbols.Add(match.symbol);
        foreach (SlotReel reel in reels)
            reel.Initialize(symbols);
    }

    public void LowerBet()
    {
        if (CanPlay && betIndex > 0)
        {
            betIndex--;
            UpdateUI();
        }
    }

    public void RaiseBet()
    {
        if (CanPlay && betIndex < bets.Count - 1)
        {
            betIndex++;
            UpdateUI();
        }
    }

    public void PullLeverAttempt()
    {
        if (CanPlay)
        {
            if (Balance < CurrentBet)
                ShowMessage("Not enough gold!");
            else
            {
                int spinBet = CurrentBet;
                Balance -= spinBet;

                SetGameActive(false);
                StopMessage();
                statusText.text = "Spinning...";

                leverAnimator.SetTrigger("Pulled");
                StartCoroutine(SpinReels(spinBet));
            }
        }
    }

    private IEnumerator SpinReels(int spinBet)
    {
        yield return new WaitForSeconds(0.25f);

        // Start together. Each reel stops at different times.
        foreach (SlotReel reel in reels)
            reel.Spin();
        foreach (SlotReel reel in reels)
            while (reel.IsSpinning)
                yield return null;

        float multiplier = payTable.GetPayoutMultiplier(reels);
        int payout = Mathf.RoundToInt(spinBet * multiplier);

        Balance += payout;

        if (Balance < 10)
        {
            StopMessage();
            statusText.text = "Game Over!";
            SetGameActive(false);
            gameOverDialog.SetActive(true);
        }
        else
        {
            if (multiplier == 777)
            {
                StopMessage();
                statusText.text = "Jackpot!";
                yield return new WaitForSeconds(2.5f);
            }
            string result = payout > 0 ? $"Won {payout}G! (x{multiplier})" : "No match!";
            ShowMessage(result);
            SetGameActive(true);
        }
    }

    private void ShowMessage(string message)
    {
        StopMessage();
        messageCoroutine = StartCoroutine(MessageRoutine(message));
    }

    private IEnumerator MessageRoutine(string message)
    {
        statusText.text = message;

        yield return new WaitForSeconds(2.5f);

        statusText.text = "Ready";
        messageCoroutine = null;
    }

    private void StopMessage()
    {
        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
            messageCoroutine = null;
        }
    }

    public void ExitDialog(bool open)
    {
        if (!open || !gameOverDialog.activeSelf)
        {
            exitDialog.SetActive(open);
            Time.timeScale = open ? 0f : 1f;
            UpdateUI();
        }
    }

    public void SetGameActive(bool active)
    {
        gameActive = active;
        UpdateUI();
    }

    public void UpdateUI()
    {
        balanceText.text = $"Balance: {Balance}G";
        betText.text = $"{CurrentBet}G";
        lowerBetButton.interactable = betIndex > 0;
        raiseBetButton.interactable = betIndex < bets.Count - 1;

        gameUI.interactable = CanPlay;
        gameUI.blocksRaycasts = CanPlay;
        dialogBlocker.SetActive(exitDialog.activeSelf || gameOverDialog.activeSelf);
    }

    public void QuitGame()
    {
        if (exitDialog.activeSelf || gameOverDialog.activeSelf)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}