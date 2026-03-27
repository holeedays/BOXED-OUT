using NUnit.Framework.Internal;
using System.Drawing.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonForm : MonoBehaviour
{
    #region Modifiable Variables
    [Header("Function of the button")]
    // this will determine what the button will do
    public TypeOfButton Type;
    #endregion

    #region Misc
    // returns if the button is wired to a function onclick
    public bool IsDisabled { get { return isDisabled; } private set {; } }
    private bool isDisabled;

    private Button thisButton;
    #endregion 

    public enum TypeOfButton
    {
        Menu,
        Start,
        Continue,
        Instructions,
        Setup
    }

    private void Start()
    {
        Setup();
    }

    private void Update()
    {
        if (Type == TypeOfButton.Continue)
            UpdateContinueButton();
    }

    private void Setup()
    {
        thisButton = GetComponent<Button>();
        if (thisButton == null)
        {
            Debug.Log("Button component not found, cannot set onclick listener event");
            return;
        }

        switch (Type)
        {
            case TypeOfButton.Menu:

                thisButton?.onClick.AddListener(
                                            // delegate { //method or code here ;} also works, you just can't access parameters
                                            () =>
                                            {
                                                if (GameManager.Instance == null)
                                                {
                                                    Debug.LogWarning("Game manager doesn't exist, cannot go to menu.");
                                                }

                                                GameManager.Instance.GoToMenu();
                                            });
                break;
            case TypeOfButton.Start:

                thisButton?.onClick.AddListener(
                                            () =>
                                            {
                                                if (GameManager.Instance == null || SerialTest.Instance == null)
                                                {
                                                    Debug.LogWarning("Game manager or serial test doesn't exist, cannot start game.");
                                                }

                                                // in the case input is not filled, user will be prompted to fill out adequate information
                                                // technically don't need to do it for continue because serial test has already stored successful values
                                                if
                                                (
                                                SerialTest.Instance.Port == string.Empty ||
                                                SerialTest.Instance.Baudrate == 0 ||
                                                SerialTest.Instance.ReadSpeed == 0 
                                                )
                                                {
                                                    GameManager.Instance.LoadInputNotification();
                                                    return;
                                                }

                                                GameManager.Instance.StartNewGame();
                                            });
                break;
            case TypeOfButton.Continue:

                thisButton?.onClick.AddListener(
                                           () =>
                                           { 
                                               if (GameManager.Instance == null || SerialTest.Instance == null)
                                               {
                                                   Debug.LogWarning("Game manager or serial test doesn't exist, cannot start game.");
                                               }

                                               GameManager.Instance.ContinueGame();
                                           });
                break;
            case TypeOfButton.Instructions:

                thisButton?.onClick.AddListener(
                                         () =>
                                         {
                                             if (GameManager.Instance == null)
                                             {
                                                 Debug.LogWarning("Game manager doesn't exist, cannot load instructions panel.");
                                             }

                                             if (!GameManager.Instance.PanelOpen)
                                                 GameManager.Instance.LoadInstructionsPanel();
                                         });
                break;
            case TypeOfButton.Setup:

                thisButton?.onClick.AddListener(
                                        () =>
                                        {
                                            if (GameManager.Instance == null)
                                            {
                                                Debug.LogWarning("Game manager doesn't exist, cannot load setup panel.");
                                            }

                                            if (!GameManager.Instance.PanelOpen)
                                                GameManager.Instance.LoadSetupPanel();
                                        });
                break;
        }
    }

    // this method only applies to the continue button, this is a method of pseudo disabling the button in case the player has not played the game yet
    private void UpdateContinueButton()
    {
        if (GameManager.Instance == null)
        {
            Debug.Log("Game manager doesn't exist, cannot update this continue button");
            return;
        }

        if (!(GameManager.Instance.TrueCurrentSceneIndex == 0 && !isDisabled))
            return;

        Image img = GetComponent<Image>();
        if (img != null) 
            img.color = Color.gray;

        if (thisButton != null)
        {
            thisButton?.onClick.RemoveAllListeners();
            thisButton.enabled = false;
        }

        isDisabled = true;
    }

    private void OnDestroy()
    {
        thisButton?.onClick.RemoveAllListeners();
    }

    private void OnDisable()
    {
        if (thisButton != null)
        {
            thisButton.onClick.RemoveAllListeners();
            thisButton.enabled = false;
        }

        isDisabled = true;
    }
}
