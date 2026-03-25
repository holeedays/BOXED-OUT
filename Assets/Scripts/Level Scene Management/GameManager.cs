using UnityEngine;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Instance Setup 
    public static GameManager Instance { get; private set; }
    #endregion

    #region Modifiable Variaibles
    [Tooltip("An array of the scenes you want to place in order")]
    public string[] Scenes;
    #endregion

    #region Misc
    private int currentSceneIndex = 0;
    #endregion

    private void Awake()
    {
        Init();
    }

    // slightly modified singleton instance for a persistent obj throughout all scenes
    private void Init()
    {
        DontDestroyOnLoad(this.gameObject);

        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void UpdateScene()
    {
        currentSceneIndex++;
        SceneManager.LoadScene(Scenes[currentSceneIndex]);
        Debug.Log("Scene is loading...");
    }
}
