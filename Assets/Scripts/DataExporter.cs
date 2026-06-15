using System;
using System.IO;
using UnityEngine;

public class DataExporter : MonoBehaviour
{
    public static DataExporter Instance { get; private set; }

    private string filePath;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        filePath = Path.Combine(Application.persistentDataPath, "Playtest_Telemetry.csv");
    }

    public void ExportRoundData(float roundTime, bool playerWon)
    {
        bool fileExists = File.Exists(filePath);

        using (StreamWriter sw = new StreamWriter(filePath, true))
        {
            if (!fileExists)
            {
                sw.WriteLine("Timestamp;Match_Round;DDA_Mode;Enemy_Count;Duration_Sec;Win;HitRate;Damage_Taken;Dashes;DDA_Harder;DDA_Easier;DDA_Aiming;DDA_Strafe");            
            }

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string round = GameManager.Instance != null ? GameManager.Instance.currentRound.ToString() : "0";
            string mode = GameSettings.Instance != null ? GameSettings.Instance.currentDDAMode.ToString() : "Unknown";
            string enemies = GameSettings.Instance != null ? GameSettings.Instance.enemyCount.ToString() : "1";
            string won = playerWon ? "Yes" : "No";

            float hitRate = DataTracker.Instance != null ? DataTracker.Instance.GetRoundPlayerHitRate() : 0f;
            float damage = DataTracker.Instance != null ? DataTracker.Instance.roundDamageTaken : 0f;
            int dashes = DataTracker.Instance != null ? DataTracker.Instance.roundTotalDashes : 0;

            int ddaHarder = EnemyDifficultyManager.Instance != null ? EnemyDifficultyManager.Instance.totalHarderInterventions : 0;
            int ddaEasier = EnemyDifficultyManager.Instance != null ? EnemyDifficultyManager.Instance.totalEasierInterventions : 0;
            int ddaAim = EnemyDifficultyManager.Instance != null ? EnemyDifficultyManager.Instance.totalAccuracyInterventions : 0;
            int ddaStrafe = EnemyDifficultyManager.Instance != null ? EnemyDifficultyManager.Instance.totalStrafeInterventions : 0;

            string hitRateStr = hitRate.ToString("N2");
            string timeStr = roundTime.ToString("N1");
            string damageStr = damage.ToString("N0");

            string row = $"{timestamp};{round};{mode};{enemies};{timeStr};{won};{hitRateStr};{damageStr};{dashes};{ddaHarder};{ddaEasier};{ddaAim};{ddaStrafe}";            
            sw.WriteLine(row);
        }

        Debug.Log($"[DataExporter] Telemetry safed! Path: {filePath}");
    }

    public void OpenDataFolder()
    {
        string path = Application.persistentDataPath;

        #if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            System.Diagnostics.Process.Start("open", $"\"{path}\"");
            
        #elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            System.Diagnostics.Process.Start("explorer.exe", path.Replace('/', '\\'));
            
        #else
            Application.OpenURL("file://" + path);
        #endif
    }
}