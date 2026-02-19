using Discord;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscordManager : MonoBehaviour
{
    public static DiscordManager Instance { get; private set; }

    private Discord.ActivityManager activityManager;
    private Discord.Activity currentActivity;
    private Discord.Discord discord;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        try
        {
            discord = new Discord.Discord(myToken, (ulong)Discord.CreateFlags.NoRequireDiscord);
            activityManager = discord.GetActivityManager();

            currentActivity = new Discord.Activity
            {
                State = "Development",
                Details = "Main Menu",
                Assets =
            {
                LargeImage = "solvsmal",
                LargeText = "Solveig"
            },
                Timestamps =
            {
                Start = (long)(System.DateTime.UtcNow - new System.DateTime(1970, 1, 1)).TotalSeconds
            },
                Party =
            {
                Id = "NONE",
                Size =
                {
                    CurrentSize = 0,
                    MaxSize = 0,
                }
            }

            };
            UpdateActivity();
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to Initialize Discord: "+ e.Message);
        }

    }

    private void OnDisable()
    {
        DisposeDiscord();
    }

    private void OnApplicationQuit()
    {
        DisposeDiscord();
    }

    public void UpdateActivity()
    {
        if (discord == null || activityManager == null) return;

        activityManager.UpdateActivity(currentActivity, (res) =>
        {
            if (res == Discord.Result.Ok)
            {

            }
            else
            {
                Debug.Log("Failed to update Activity");
            }
        });
    }


    public void SetDetails(string details)
    {
        currentActivity.Details = details;
        UpdateActivity();
    }

    public void SetState(string state)
    {
        currentActivity.State = state;
        UpdateActivity();
    }

    public void SetLargeImage(string imageKey, string imageText)
    {
        currentActivity.Assets.LargeImage = imageKey;
        currentActivity.Assets.LargeText = imageText;
        UpdateActivity();
    }


    private void Update()
    {
        if (discord != null)
        {
            discord.RunCallbacks();
        }
    }

    private void DisposeDiscord()
    {
        if (discord != null)
        {
            discord.Dispose();
            discord = null;
        }
    }
}
