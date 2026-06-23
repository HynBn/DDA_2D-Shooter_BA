using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance { get; private set; }

    public enum DifficultySystem { Static, DDA};

    public enum StaticDifficulty { Easy, Medium, Hard};

    public enum DDAMode { Behavioral, Numerical};

    public DifficultySystem currentDifficultySystem = DifficultySystem.DDA;
    public StaticDifficulty currentStaticDifficulty = StaticDifficulty.Medium;
    public DDAMode currentDDAMode = DDAMode.Behavioral;

    public bool IsDDAEnabled => currentDifficultySystem == DifficultySystem.DDA;

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

    public void SelectStaticSystem()
    {
        currentDifficultySystem = DifficultySystem.Static;
        Debug.Log("Difficulty system selected: Static");
    }

    public void SelectDDASystem()
    {
        currentDifficultySystem = DifficultySystem.DDA;
        Debug.Log("Difficulty system selected: DDA");
    }

    public void SetStaticDifficulty(int difficultyIndex)
    {
        currentStaticDifficulty =
            (StaticDifficulty)Mathf.Clamp(difficultyIndex, 0, 2);

        Debug.Log($"Static difficulty selected: {currentStaticDifficulty}");
    }

    public void SetDDAMode(int modeIndex)
    {
        currentDDAMode =
            (DDAMode)Mathf.Clamp(modeIndex, 0, 1);

        Debug.Log($"DDA mode selected: {currentDDAMode}");
    }
}
