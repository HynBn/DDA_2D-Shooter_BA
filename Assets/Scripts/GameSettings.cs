using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance { get; private set; }

    public enum DDAMode { Behavioral, Numerical};
    public DDAMode currentDDAMode = DDAMode.Behavioral;

    public int enemyCount = 1;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // public void SetMode(DDAMode mode)
    // {
    //     currentDDAMode = (DDAMode)mode;
    //     Debug.Log($"DDA Mode set to: {currentDDAMode}");
    // }
}
