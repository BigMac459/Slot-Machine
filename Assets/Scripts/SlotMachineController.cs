using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotMachineController : MonoBehaviour
{
    //public static SlotMachineController Instance { get; private set; }
    
    [Header("Gold")]
    [SerializeField, Min(10)] private int startingBalance = 500;
    [SerializeField] private List<int> betAmounts;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI balanceText;
    [SerializeField] private TextMeshProUGUI betText;
    [SerializeField] private CanvasGroup machineUI;

    [SerializeField] private Button lowerBetButton;
    [SerializeField] private Button raiseBetButton;
    private int totalBets = 0;
    [SerializeField] private TextMeshProUGUI betsCount;
    private int wins = 0;
    [SerializeField] private TextMeshProUGUI winsCount;
    [SerializeField] private int gameOverIndex;
    [SerializeField] private int exitIndex;

    [Header("Slot Machine")]
    [SerializeField] private GameObject reelsHolder;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private PayTableValues payTable;

    private readonly List<SlotReel> reels = new();

    private bool gameActive = true;
    private Coroutine messageCoroutine;
    private Animator leverAnimator;

    private int balance;
    public int Balance
    {
        get => balance;
        private set
        {
            balance = value;
            balanceText.text = $"{Balance}G";
        }
    }

    private int currentBetIndex;
    public int CurrentBetAmount
    {
        get => betAmounts[currentBetIndex];
        private set
        {
            currentBetIndex = Mathf.Clamp(betAmounts.IndexOf(value), 0, betAmounts.Count - 1);
        }
    }

    public bool CanPlay {
        get
        {
            return gameActive && !DialogBoxManager.Instance.IsPaused;
        }
    }

    public string StatusText
    {
        get => statusText.text;
        set
        {
            if (messageCoroutine != null)
            {
                StopCoroutine(messageCoroutine);
                messageCoroutine = null;
            }
            statusText.text = value;
        }
    }

    private void Awake()
    {
        // Instance = this;
        foreach (Transform child in reelsHolder.transform)
            if (child != null)
                reels.Add(child.GetComponent<SlotReel>());
    }

    private void Start()
    {
        leverAnimator = GetComponentInChildren<SlotLever>().GetComponent<Animator>();
        NewGame();
    }

    public void NewGame()
    {
        Balance = startingBalance;
        CurrentBetAmount = 0;

        DialogBoxManager.Instance.HideAndResume();

        HomePositions();
        StatusText = "Ready!";
        UpdateBetUI();
        SetMachineActive(true);
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
        if (CanPlay && currentBetIndex > 0)
        {
            currentBetIndex--;
            UpdateBetUI();
        }
    }

    public void RaiseBet()
    {
        if (CanPlay && currentBetIndex < betAmounts.Count - 1)
        {
            currentBetIndex++;
            UpdateBetUI();
        }
    }

    public void UpdateBetUI()
    {
        betText.text = $"{CurrentBetAmount}G";
        lowerBetButton.interactable = currentBetIndex > 0;
        raiseBetButton.interactable = currentBetIndex < betAmounts.Count - 1;
    }

    public void PullLeverAttempt()
    {
        if (CanPlay)
        {
            if (Balance < CurrentBetAmount)
                ShowMessage("Not enough gold!");
            else
            {
                int spinBet = CurrentBetAmount;
                Balance -= spinBet;
                betsCount.text = (++totalBets).ToString();

                SetMachineActive(false);
                StatusText = "Spinning...";

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

        float multiplier = payTable.GetPayoutMultiplier(reels, out Symbol winningSymbol);
        int payout = Mathf.RoundToInt(spinBet * multiplier);

        Balance += payout;

        if (Balance < 10)
            DialogBoxManager.Instance.GoToDialog(gameOverIndex);
        else
        {
            if (winningSymbol)
            {
                winsCount.text = (++wins).ToString();
                if (winningSymbol.type == SymbolType.Jackpot)
                    StatusText = "Jackpot!!";
                else
                    StatusText = "Win!";
            }
            else
            {
                if (payout > 0)
                    StatusText = "Consolation!";
                else
                    StatusText = "No match!";
            }
            yield return new WaitForSeconds(1.5f);
            string result = $"{payout}G (x{multiplier})";
            ShowMessage(result);
            SetMachineActive(true);
        }
    }

    private void ShowMessage(string message)
    {
        messageCoroutine = StartCoroutine(MessageRoutine(message));
    }

    private IEnumerator MessageRoutine(string message)
    {
        StatusText = message;

        yield return new WaitForSeconds(2.5f);

        statusText.text = "Ready";
        messageCoroutine = null;
    }

    public void SetMachineActive(bool active)
    {
        gameActive = active;
        machineUI.interactable = CanPlay;
        machineUI.blocksRaycasts = CanPlay;
    }

    public void ExitDialog()
    {
        DialogBoxManager.Instance.GoToDialog(exitIndex);
    }

    public void QuitGame()
    { 
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}