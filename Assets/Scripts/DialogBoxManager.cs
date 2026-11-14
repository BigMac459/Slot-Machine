using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class DialogBoxManager : MonoBehaviour
{
    public static DialogBoxManager Instance { get; private set; }

    [DllImport("__Internal")]
    private static extern void ReloadPage();

    [SerializeField] private List<GameObject> dialogBoxes;
    [SerializeField] private GameObject dialogBackdrop;

    private bool paused = false;
    public bool IsPaused
    {
        get => paused;
        set
        {
            Time.timeScale = value ? 0 : 1;
            dialogBackdrop.SetActive(value);
            paused = value;
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    public void HideAllDialogueBoxes()
    {
        foreach (GameObject dialog in dialogBoxes)
            dialog.SetActive(false);
    }

    public void HideAndResume()
    {
        HideAllDialogueBoxes();
        if (IsPaused)
            IsPaused = false;
    }

    public void GoToDialog(int index)
    {
        if (!IsPaused && index < dialogBoxes.Count)
        {
            HideAllDialogueBoxes();
            dialogBoxes[index].SetActive(true);
            IsPaused = true;
        }
    }

    public void RestartGame()
    {
        SlotMachineController.Instance.NewGame();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL
        ReloadPage();
#else
        Application.Quit();
#endif
    }
}
