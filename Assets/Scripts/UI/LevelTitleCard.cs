using TMPro;
using UnityEditor.XR;
using UnityEngine;

public class LevelTitleCard : MonoBehaviour
{
    #region Instance Setup
    public static LevelTitleCard Instance { get; private set; }
    #endregion

    #region Modifiable Variables
    [Range(0f, 3f)]
    public float AnimationSpeed;
    #endregion

    #region Misc
    // marks all animations are done (including the closing one)
    public bool DoneClosing { get; private set; }

    private TitleCardStage tcs;

    private TMP_Text text;
    private CanvasGroup cg;
    #endregion

    private enum TitleCardStage
    {
        Reveal,
        Hiding,
        Idle,
        Closing
    }


    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject);
        }

        Instance = this;
    }

    private void Start()
    {
        Setup();
    }

    private void Setup()
    {
        cg = GetComponent<CanvasGroup>();
        text = GetComponentInChildren<TMP_Text>();

        // make sure the text is clear on start
        text.color = Color.clear;
    }

    private void Update()
    {
        switch(tcs)
        {
            case TitleCardStage.Reveal:

                UpdateRevealStage();
                break;
            case TitleCardStage.Hiding:

                UpdateHidingStage();    
                break;
            case TitleCardStage.Idle:

                UpdateIdleStage();
                break;
            case TitleCardStage.Closing:

                UpdateClosingStage();
                break;
        }
    }

    private void UpdateRevealStage()
    {
        if (text.color.a < 1)
            RevealText();
        else
            tcs = TitleCardStage.Hiding;
    }
    private void UpdateHidingStage()
    {
        if (cg.alpha > 0)
            HideSelf();
        else
            tcs = TitleCardStage.Idle;
    }
    private void UpdateIdleStage()
    {
        if (LevelManager.Instance == null)
            return;

        if (LevelManager.Instance.LevelCompleted)
            tcs = TitleCardStage.Closing;
    }
    private void UpdateClosingStage()
    {
        if (text.color.a > 0)
            text.color = Color.clear;

        if (cg.alpha < 1)
            DarkenScreen();
        else
            DoneClosing = true;
    }


    private void RevealText()
    {
        text.color = new Color(1, 1, 1, text.color.a + AnimationSpeed * Time.deltaTime);
    }

    private void HideSelf()
    {
        cg.alpha -= AnimationSpeed * Time.deltaTime;
    }

    private void DarkenScreen()
    {
        cg.alpha += AnimationSpeed * Time.deltaTime;
    }
}

