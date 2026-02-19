using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager instance;

    [Header("Achievements")]
    public List<AchievementSO> allAchievements;

    private HashSet<string> unlockedAchievements = new HashSet<string>();

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        SaveData.Instance.LoadAchievements();

    }

    public bool IsAchievementUnlocked(AchievementSO achievement)
    {
        return unlockedAchievements.Contains(achievement.id);
    }

    public void UnlockAchievement(AchievementSO achievement)
    {
        if (unlockedAchievements.Contains(achievement.id))
        {
            Debug.Log($"Achievement already unlocked: {achievement.achievementName}");
            return;
        }

        unlockedAchievements.Add(achievement.id);
        SaveData.Instance.unlockedAchievements.Add(achievement.id);
        Debug.Log($"Achievement Unlocked: {achievement.achievementName}");

        Notifier.instance.NotifyLogEntry($"Achievement Unlocked: " +
            "<color=#feff37>"+ achievement.achievementName + "</color>");

        
        SaveData.Instance.SaveAchievements();
    }

    public void UnlockAchievementById(string id)
    {
        AchievementSO achievement = allAchievements.Find(a => a.id == id);

        if (achievement == null)
        {
            Debug.LogError($"Achievement with ID '{id}' not found.");
            return;
        }

        UnlockAchievement(achievement);
    }

    public void LoadUnlockedAchievements(HashSet<string> loadedAchievements)
    {
        unlockedAchievements = new HashSet<string>(loadedAchievements);
    }

}
