using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using static FileManager;
using System;
using UnityEngine.Analytics;

[System.Serializable]
public struct SaveData
{
    public static SaveData Instance;

    public int lastEquippedWeapon;
    public int lastEquippedMagic;
    #region Weapon Bools
    public bool katanaUnlocked;
    public bool thiefsKnifeUnlocked;
    public bool woodenStickUnlocked;

    #endregion
    #region Magic Bools
    public bool kunaiUnlocked;
    public bool magic2Unlocked;
    public bool magic3Unlocked;
    public bool magic4Unlocked;
    public bool magic5Unlocked;
    public bool magic6Unlocked;
    #endregion
    #region Blessing Bools
    public bool ub1;public bool ub2;public bool ub3;public bool ub4;public bool ub5;public bool ub6;
    public bool ub7;public bool ub8;public bool ub9;public bool ub10;public bool ub11;public bool ub12;
    public bool ub13;public bool ub14;public bool ub15;public bool ub16;public bool ub17;public bool ub18;
    #endregion

    //Save Slots
    public int slotIndex;
    public bool slotUsed;
    public bool firstPlay;
    public bool firstPrefs;
    public bool firstTorii;
    public float playTime;
    public HashSet<string> unlockedAchievements;

    //Permanence
    public HashSet<string> dialogueIds;
    public HashSet<string> worldPermanenceList;
    public HashSet<string> savedLogs;
    public HashSet<string> sealsUnlocked;
    public bool learnedSeals;

    //Preferences
    public int menuChar;

    //map stuff
    public HashSet<string> visitedScenes;
    public HashSet<string> mappedScenes;
    public bool playerHasMapItem;


    //torii stuff
    public string toriiSceneName;
    public Vector2 toriiPos;

    //player data stuff
    public int playerGodSeeds;
    public int playerMaxHealth;
    public int playerHealth;
    public float playerInk;
    public bool playerHalfInk;
    public Vector2 playerPosition;
    public string lastScene;
    public int playerRice;
    public int playerCoins;

    //player unlockables
    public bool playerUnlockedWallJump;
    public bool playerUnlockedDash;
    public bool playerUnlockedDoubleJump;
    public bool playerUnlockedKunai;

    //player Dropped Katana
    public Vector2 dKatanaPos;
    public string sceneWithDKatana;
    public Quaternion dKatanaRot;
    public int dKatanaRice;
    public int dKatanaCoins;

    //NPC States
    public int hisaoStateSave;


    public void Initialize()
    {

        if (!File.Exists(Application.persistentDataPath + "/save.torii.data" + slotIndex))
        {
            BinaryWriter writer = new BinaryWriter(File.Create(Application.persistentDataPath + "/save.torii.data" + slotIndex));
            firstTorii = true;
        }
        if (!File.Exists(Application.persistentDataPath + "/save.player.data" + slotIndex))
        {
            BinaryWriter writer = new BinaryWriter(File.Create(Application.persistentDataPath + "/save.player.data" + slotIndex));
            firstPlay = true;
        }
        if (!File.Exists(Application.persistentDataPath + "/save.dKatana.data" + slotIndex))
        {
            BinaryWriter writer = new BinaryWriter(File.Create(Application.persistentDataPath + "/save.dKatana.data" + slotIndex));
        }
        if (!File.Exists(Application.persistentDataPath + "/achv.dat"))
        {
            BinaryWriter writer = new BinaryWriter(File.Create(Application.persistentDataPath + "/achv.dat"));
        }

        if (visitedScenes == null)
        {
            visitedScenes = new HashSet<string>();
        }
        if (mappedScenes == null)
        {
            mappedScenes = new HashSet<string>();
        }
        if (dialogueIds == null)
        {
            dialogueIds = new HashSet<string>();
        }
        if (worldPermanenceList == null)
        {
            worldPermanenceList = new HashSet<string>();
        }
        if (savedLogs == null)
        {
            savedLogs = new HashSet<string>();
        }
        if (unlockedAchievements == null)
        {
            unlockedAchievements = new HashSet<string>();
        }
        if (sealsUnlocked == null)
        {
            sealsUnlocked = new HashSet<string>();
        }

    }

    public void SaveTorii()
    {
        using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.torii.data" + slotIndex)))
        {
            writer.Write(toriiSceneName);
            writer.Write(toriiPos.x);
            writer.Write(toriiPos.y);
        }
    }

    public void LoadTorii()
    {
        if (File.Exists(Application.persistentDataPath + "/save.torii.data" + slotIndex))
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/save.torii.data" + slotIndex)))
            {
                if (firstTorii)
                {
                    SetUpNewToriiData();
                    firstTorii = false;
                }
                else
                {
                    toriiSceneName = reader.ReadString();
                    toriiPos.x = reader.ReadSingle();
                    toriiPos.y = reader.ReadSingle();
                }
            }
        }
    }

    public void SavePlayerData()
    {
        using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.player.data" + slotIndex)))
        {
            playerMaxHealth = PlayerController.Instance.maxHealth;
            writer.Write(playerMaxHealth);
            playerHealth = PlayerController.Instance.Health;
            writer.Write(playerHealth);
            playerGodSeeds = PlayerController.Instance.godSeeds;
            writer.Write(playerGodSeeds);
            playerInk = PlayerController.Instance.Ink;
            writer.Write(playerInk);
            playerHalfInk = PlayerController.Instance.halfInk;
            writer.Write(playerHalfInk);
            playerRice = PlayerController.Instance.riceOwned;
            writer.Write(playerRice);
            playerCoins = PlayerController.Instance.coinsOwned;
            writer.Write(playerCoins);

            playerUnlockedWallJump = PlayerController.Instance.unlockedWallJump;
            writer.Write(playerUnlockedWallJump);
            playerUnlockedDash = PlayerController.Instance.unlockedDash;
            writer.Write(playerUnlockedDash);
            playerUnlockedDoubleJump = PlayerController.Instance.unlockedDoubleJump;
            writer.Write(playerUnlockedDoubleJump);

            playerHasMapItem = PlayerController.Instance.hasMapItem;
            writer.Write(playerHasMapItem);

            playerUnlockedKunai = PlayerController.Instance.magicEquipped;
            writer.Write(playerUnlockedKunai);

            playerPosition = PlayerController.Instance.transform.position;
            writer.Write(playerPosition.x);
            writer.Write(playerPosition.y);

            lastScene = SceneManager.GetActiveScene().name;
            writer.Write(lastScene);

            playTime = GameManager.Instance.playTime;
            writer.Write(playTime);

            SavePlayerEquipment(writer);
            SaveMapStates(writer);
            SaveDialogues(writer);
            SaveWorldPermanence(writer);
            SaveUnlockedLogs(writer);
            SaveSealsInOrder(writer);

            learnedSeals = PlayerController.Instance.learnedSeals;
            writer.Write(learnedSeals);

            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowSaveIcon();
            }
        }
    }

    public void LoadPlayerData()
    {
        if (File.Exists(Application.persistentDataPath + "/save.player.data" + slotIndex))
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/save.player.data" + slotIndex)))
            {
                if (firstPlay)
                {
                    SetUpNewPlayerData();
                    firstPlay = false;
                }
                else
                {
                    playerMaxHealth = reader.ReadInt32();
                    playerHealth = reader.ReadInt32();
                    playerGodSeeds = reader.ReadInt32();
                    playerInk = reader.ReadSingle();
                    playerHalfInk = reader.ReadBoolean();
                    playerRice = reader.ReadInt32();
                    playerCoins = reader.ReadInt32();

                    playerUnlockedWallJump = reader.ReadBoolean();
                    playerUnlockedDash = reader.ReadBoolean();
                    playerUnlockedDoubleJump = reader.ReadBoolean();
                    playerHasMapItem = reader.ReadBoolean();
                    playerUnlockedKunai = reader.ReadBoolean();

                    playerPosition.x = reader.ReadSingle();
                    playerPosition.y = reader.ReadSingle();

                    lastScene = reader.ReadString();
                    playTime = reader.ReadSingle();

                    SceneManager.LoadScene(toriiSceneName);
                    PlayerController.Instance.transform.position = toriiPos;
                    PlayerController.Instance.halfInk = playerHalfInk;
                    PlayerController.Instance.maxHealth = playerMaxHealth;
                    PlayerController.Instance.Health = playerHealth;
                    PlayerController.Instance.godSeeds = playerGodSeeds;
                    PlayerController.Instance.Ink = playerInk;
                    PlayerController.Instance.riceOwned = playerRice;
                    PlayerController.Instance.coinsOwned = playerCoins;

                    PlayerController.Instance.unlockedWallJump = playerUnlockedWallJump;
                    PlayerController.Instance.unlockedDash = playerUnlockedDash;
                    PlayerController.Instance.unlockedDoubleJump = playerUnlockedDoubleJump;
                    PlayerController.Instance.hasMapItem = playerHasMapItem;
                    PlayerController.Instance.magicEquipped = playerUnlockedKunai;
                    GameManager.Instance.playTime = playTime;

                    LoadPlayerEquipment(reader);
                    LoadMapStates(reader);
                    LoadDialogues(reader);
                    LoadWorldPermanence(reader);
                    LoadUnlockedLogs(reader);
                    LoadSealsInOrder(reader);

                    learnedSeals = reader.ReadBoolean();

                    PlayerController.Instance.learnedSeals = learnedSeals;


                }
                
            }
        }
        else
        {
            Debug.Log("File does not exist");
            PlayerController.Instance.halfInk = false;
            PlayerController.Instance.maxHealth = 4;
            PlayerController.Instance.Health = PlayerController.Instance.maxHealth;
            PlayerController.Instance.godSeeds = 0;
            PlayerController.Instance.Ink = .5f;
            PlayerController.Instance.riceOwned = 0;
            PlayerController.Instance.coinsOwned = 0;

            PlayerController.Instance.unlockedWallJump = false;
            PlayerController.Instance.unlockedDash = false;
            PlayerController.Instance.unlockedDoubleJump = false;
            PlayerController.Instance.magicEquipped = false;
            PlayerController.Instance.hasMapItem = false;
            PlayerController.Instance.learnedSeals = false;

            WeaponMenuManager.instance.InitializeWeapons();
            WeaponMenuManager.instance.lastEquipped = 0;
            MagicMenuManager.instance.InitializeMagics();
            MagicMenuManager.instance.lastEquipped = 0;
            BlessingMenuManager.Instance.InitializeBlessings();
        }
    }

    public void SaveDKatanaData()
    {
        using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.dKatana.data" + slotIndex)))
        {
            if (KatanaDropped.Instance != null)
            {
                sceneWithDKatana = SceneManager.GetActiveScene().name;
                dKatanaPos = KatanaDropped.Instance.transform.position;
                dKatanaRot = KatanaDropped.Instance.transform.rotation;
                dKatanaRice = KatanaDropped.Instance.storedRice;
                dKatanaCoins = KatanaDropped.Instance.storedCoins;


                writer.Write(sceneWithDKatana);

                writer.Write(dKatanaPos.x);
                writer.Write(dKatanaPos.y);

                writer.Write(dKatanaRot.x);
                writer.Write(dKatanaRot.y);
                writer.Write(dKatanaRot.z);
                writer.Write(dKatanaRot.w);
                writer.Write(dKatanaRice);
                writer.Write(dKatanaCoins);
            }
        }
    }

    public void LoadDKatanaData()
    {
        if (File.Exists(Application.persistentDataPath + "/save.dKatana.data" + slotIndex))
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/save.dKatana.data" + slotIndex)))
            {
                sceneWithDKatana = reader.ReadString();
                dKatanaPos.x = reader.ReadSingle();
                dKatanaPos.y = reader.ReadSingle();

                float rotationX = reader.ReadSingle();
                float rotationY = reader.ReadSingle();
                float rotationZ = reader.ReadSingle();
                float rotationW = reader.ReadSingle();
                dKatanaRot = new Quaternion(rotationX, rotationY, rotationZ, rotationW);

                dKatanaRice = reader.ReadInt32();
                dKatanaCoins = reader.ReadInt32();
            }
        }
        else
        {
            Debug.Log("DKatana does not exist");
        }
    }
    public void WriteDKatanaMoney()
    {
        KatanaDropped.Instance.storedRice = dKatanaRice;
        KatanaDropped.Instance.storedCoins = dKatanaCoins;
    }

    public void SavePlayerPreferences()
    {
        using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.preferences.data")))
        {
            menuChar = PlayerPreferences.Instance.changeImageManager.currentIndex;
            writer.Write(menuChar);
            //Add Prefs
        }
    }

    public void LoadPlayerPreferences()
    {
        if (File.Exists(Application.persistentDataPath + "/save.preferences.data"))
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/save.preferences.data")))
            {
                if (firstPrefs)
                {
                    SetUpNewPlayerPrefs();
                    firstPrefs = false;
                }
                else
                {
                    menuChar = reader.ReadInt32();
                    //Add Prefs
                }
            }
        }
        else
        {
            Debug.Log("File does not exist");

            //set defaults here
            menuChar = 4;


        }
    }

    public void LoadPlayerDataForTitleScreen(int index)
    {
        if (File.Exists(Application.persistentDataPath + "/save.player.data" + index))
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/save.player.data" + index)))
            {
                playerMaxHealth = reader.ReadInt32();
                playerHealth = reader.ReadInt32();
                playerGodSeeds = reader.ReadInt32();
                playerInk = reader.ReadSingle();
                playerHalfInk = reader.ReadBoolean();
                playerRice = reader.ReadInt32();
                playerCoins = reader.ReadInt32();

                playerUnlockedWallJump = reader.ReadBoolean();
                playerUnlockedDash = reader.ReadBoolean();
                playerUnlockedDoubleJump = reader.ReadBoolean();
                playerHasMapItem = reader.ReadBoolean();
                playerUnlockedKunai = reader.ReadBoolean();

                playerPosition.x = reader.ReadSingle();
                playerPosition.y = reader.ReadSingle();

                lastScene = reader.ReadString();
                playTime = reader.ReadSingle();
            }
            slotUsed = true;
        }

        if (File.Exists(Application.persistentDataPath + "/save.torii.data" + index))
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/save.torii.data" + index)))
            {
                toriiSceneName = reader.ReadString();
            }
        }

        else
        {
            slotUsed = false;
        }

    }

    public void InitializePrefs()
    {
        if (!File.Exists(Application.persistentDataPath + "/save.preferences.data"))
        {
            BinaryWriter writer = new BinaryWriter(File.Create(Application.persistentDataPath + "/save.preferences.data"));
            firstPrefs = true;
        }
    }

    public void SetUpNewPlayerPrefs()
    {
        //Add Future Prefs here for initial setup
        menuChar = 4;
    }

    public void SetUpNewPlayerData()
    {
        PlayerController.Instance.halfInk = false;
        PlayerController.Instance.maxHealth = 4;
        PlayerController.Instance.Health = PlayerController.Instance.maxHealth;
        PlayerController.Instance.godSeeds = 0;
        PlayerController.Instance.Ink = 0;
        PlayerController.Instance.riceOwned = 0;
        PlayerController.Instance.coinsOwned = 0;

        PlayerController.Instance.unlockedWallJump = false;
        PlayerController.Instance.unlockedDash = false;
        PlayerController.Instance.unlockedDoubleJump = false;
        PlayerController.Instance.hasMapItem = false;
        PlayerController.Instance.magicEquipped = false;
        playTime = 0;

        WeaponMenuManager.instance.InitializeWeapons();
        MagicMenuManager.instance.InitializeMagics();
        BlessingMenuManager.Instance.InitializeBlessings();
        WeaponMenuManager.instance.lastEquipped = 0;
        MagicMenuManager.instance.lastEquipped = 0;

        PlayerController.Instance.hisaoState = 0;
        PlayerController.Instance.learnedSeals = false;
    }

    public void SetUpNewToriiData()
    {
        toriiSceneName = "FS_SanGamiTemple";
        toriiPos = new Vector2(-233, 35);
    }


    public void SavePlayerEquipment(BinaryWriter writer)
    {
        //WEAPONS
        katanaUnlocked = WeaponMenuManager.instance.katana;
        writer.Write(katanaUnlocked);
        thiefsKnifeUnlocked = WeaponMenuManager.instance.thiefsKnife;
        writer.Write(thiefsKnifeUnlocked);
        woodenStickUnlocked = WeaponMenuManager.instance.woodenStick;
        writer.Write(woodenStickUnlocked);
        //SET MORE WEAPONS HERE

        #region SAVE MAGIC
        kunaiUnlocked = MagicMenuManager.instance.kunai;
        writer.Write(kunaiUnlocked);
        magic2Unlocked = MagicMenuManager.instance.magic2;
        writer.Write(magic2Unlocked);
        magic3Unlocked = MagicMenuManager.instance.magic3;
        writer.Write(magic3Unlocked);
        magic4Unlocked = MagicMenuManager.instance.magic4;
        writer.Write(magic4Unlocked);
        magic5Unlocked = MagicMenuManager.instance.magic5;
        writer.Write(magic5Unlocked);
        magic6Unlocked = MagicMenuManager.instance.magic6;
        writer.Write(magic6Unlocked);
        #endregion

        lastEquippedWeapon = WeaponMenuManager.instance.lastEquipped;
        writer.Write(lastEquippedWeapon);
        lastEquippedMagic = MagicMenuManager.instance.lastEquipped;
        writer.Write(lastEquippedMagic);

        SaveBlessings(writer);
    }
    public void LoadPlayerEquipment(BinaryReader reader)
    {
        katanaUnlocked = reader.ReadBoolean();
        thiefsKnifeUnlocked = reader.ReadBoolean();
        woodenStickUnlocked = reader.ReadBoolean();

        #region LOAD MAGIC
        kunaiUnlocked = reader.ReadBoolean();
        magic2Unlocked= reader.ReadBoolean();
        magic3Unlocked= reader.ReadBoolean();
        magic4Unlocked= reader.ReadBoolean();
        magic5Unlocked= reader.ReadBoolean();
        magic6Unlocked= reader.ReadBoolean();
        #endregion

        lastEquippedWeapon = reader.ReadInt32();
        lastEquippedMagic = reader.ReadInt32();

        WeaponMenuManager.instance.katana = katanaUnlocked;
        WeaponMenuManager.instance.thiefsKnife = thiefsKnifeUnlocked;
        WeaponMenuManager.instance.woodenStick = woodenStickUnlocked;

        #region WRITE MAGIC
        MagicMenuManager.instance.kunai = kunaiUnlocked;
        MagicMenuManager.instance.magic2 = magic2Unlocked;
        MagicMenuManager.instance.magic3 = magic3Unlocked;
        MagicMenuManager.instance.magic4 = magic4Unlocked;
        MagicMenuManager.instance.magic5 = magic5Unlocked;
        MagicMenuManager.instance.magic6 = magic6Unlocked;
        #endregion

        WeaponMenuManager.instance.UpdateWeaponButtons();
        WeaponMenuManager.instance.lastEquipped = lastEquippedWeapon;

        MagicMenuManager.instance.UpdateMagicButtons();
        MagicMenuManager.instance.lastEquipped = lastEquippedMagic;

        LoadBlessings(reader);
    }

    public void SaveBlessings(BinaryWriter writer)
    {
        ub1 = BlessingMenuManager.Instance.b1; writer.Write(ub1);
        ub2 = BlessingMenuManager.Instance.b2; writer.Write(ub2);
        ub3 = BlessingMenuManager.Instance.b3; writer.Write(ub3);
        ub4 = BlessingMenuManager.Instance.b4; writer.Write(ub4);
        ub5 = BlessingMenuManager.Instance.b5; writer.Write(ub5);
        ub6 = BlessingMenuManager.Instance.b6; writer.Write(ub6);
        ub7 = BlessingMenuManager.Instance.b7; writer.Write(ub7);
        ub8 = BlessingMenuManager.Instance.b8; writer.Write(ub8);
        ub9 = BlessingMenuManager.Instance.b9; writer.Write(ub9);
        ub10 = BlessingMenuManager.Instance.b10; writer.Write(ub10);
        ub11 = BlessingMenuManager.Instance.b11; writer.Write(ub11);
        ub12 = BlessingMenuManager.Instance.b12; writer.Write(ub12);
        ub13 = BlessingMenuManager.Instance.b12; writer.Write(ub13);
        ub14 = BlessingMenuManager.Instance.b14; writer.Write(ub14);
        ub15 = BlessingMenuManager.Instance.b15; writer.Write(ub15);
        ub16 = BlessingMenuManager.Instance.b16; writer.Write(ub16);
        ub17 = BlessingMenuManager.Instance.b17; writer.Write(ub17);
        ub18 = BlessingMenuManager.Instance.b18; writer.Write(ub18);

        writer.Write(BlessingMenuManager.Instance.equippedBlessings.Count);
        foreach (int num in BlessingMenuManager.Instance.equippedBlessings)
        {
            writer.Write(num);
        }
    }
    public void LoadBlessings(BinaryReader reader)
    {
        ub1 = reader.ReadBoolean(); BlessingMenuManager.Instance.b1 = ub1;
        ub2 = reader.ReadBoolean(); BlessingMenuManager.Instance.b2 = ub2;
        ub3 = reader.ReadBoolean(); BlessingMenuManager.Instance.b3 = ub3;
        ub4 = reader.ReadBoolean(); BlessingMenuManager.Instance.b4 = ub4;
        ub5 = reader.ReadBoolean(); BlessingMenuManager.Instance.b5 = ub5;
        ub6 = reader.ReadBoolean(); BlessingMenuManager.Instance.b6 = ub6;
        ub7 = reader.ReadBoolean(); BlessingMenuManager.Instance.b7 = ub7;
        ub8 = reader.ReadBoolean(); BlessingMenuManager.Instance.b8 = ub8;
        ub9 = reader.ReadBoolean(); BlessingMenuManager.Instance.b9 = ub9;
        ub10 = reader.ReadBoolean(); BlessingMenuManager.Instance.b10 = ub10;
        ub11 = reader.ReadBoolean(); BlessingMenuManager.Instance.b11 = ub11;
        ub12 = reader.ReadBoolean(); BlessingMenuManager.Instance.b12 = ub12;
        ub13 = reader.ReadBoolean(); BlessingMenuManager.Instance.b13 = ub13;
        ub14 = reader.ReadBoolean(); BlessingMenuManager.Instance.b14 = ub14;
        ub15 = reader.ReadBoolean(); BlessingMenuManager.Instance.b15 = ub15;
        ub16 = reader.ReadBoolean(); BlessingMenuManager.Instance.b16 = ub16;
        ub17 = reader.ReadBoolean(); BlessingMenuManager.Instance.b17 = ub17;
        ub18 = reader.ReadBoolean(); BlessingMenuManager.Instance.b18 = ub18;

        BlessingMenuManager.Instance.UpdateUnlockedBlessings();

        int count = reader.ReadInt32(); // Read the size of the list
        BlessingMenuManager.Instance.equippedBlessings = new List<int>();
        for (int i = 0; i < count; i++)
        {
            BlessingMenuManager.Instance.equippedBlessings.
                Add(reader.ReadInt32());
        }
        
        if (BlessingMenuManager.Instance.equippedBlessings != null || 
            BlessingMenuManager.Instance.equippedBlessings.Count > 0)
        {
            BlessingMenuManager.Instance.BuildBlessingFromLoad();
        }
    }

    public void SaveDialogues(BinaryWriter writer)
    {
        dialogueIds = PlayerController.Instance.playedDialogueIds;

        writer.Write(dialogueIds.Count);
        foreach (var id in dialogueIds)
        {
            writer.Write(id);
        }
        CheckDialogues();
    }
    public void LoadDialogues(BinaryReader reader)
    {
        int count = reader.ReadInt32();
        dialogueIds.Clear();
        for (int i = 0; i < count; i++)
        {
            dialogueIds.Add(reader.ReadString());
        }
        PlayerController.Instance.playedDialogueIds = dialogueIds;
        CheckDialogues();
    }
    public void CheckDialogues()
    {
        if (PlayerController.Instance.playedDialogueIds != null)
        {
            if (PlayerController.Instance.playedDialogueIds.Contains("Example"))
            {
                //Do Something if a dialogue has played
            }
        }
    }

    public void SaveMapStates(BinaryWriter writer)
    {
        visitedScenes = PlayerController.Instance.scenesVisited;
        writer.Write(visitedScenes.Count);
        foreach (var scene in visitedScenes)
        {
            writer.Write(scene);
        }

        mappedScenes = PlayerController.Instance.scenesOnMap;
        writer.Write(mappedScenes.Count);
        foreach (var scene2 in mappedScenes)
        {
            writer.Write(scene2);
        }
    }
    public void LoadMapStates(BinaryReader reader)
    {
        int count = reader.ReadInt32();
        visitedScenes.Clear();
        for (int i = 0; i < count; i++)
        {
            visitedScenes.Add(reader.ReadString());
        }
        PlayerController.Instance.scenesVisited = visitedScenes;

        int count2 = reader.ReadInt32();
        mappedScenes.Clear();
        for (int i = 0;i < count2; i++)
        {
            mappedScenes.Add(reader.ReadString());
        }
        PlayerController.Instance.scenesOnMap = mappedScenes;
    }

    public void SaveWorldPermanence(BinaryWriter writer)
    {
        worldPermanenceList = PlayerController.Instance.worldPermanence;
        writer.Write(worldPermanenceList.Count);
        foreach (var id in worldPermanenceList)
        {
            writer.Write(id);
        }

        hisaoStateSave = PlayerController.Instance.hisaoState;
        writer.Write(hisaoStateSave);
    }
    public void LoadWorldPermanence(BinaryReader reader)
    {
        int count = reader.ReadInt32();
        worldPermanenceList.Clear();
        for (int i = 0; i < count; i++)
        {
            worldPermanenceList.Add(reader.ReadString());
        }

        PlayerController.Instance.worldPermanence.Clear();
        PlayerController.Instance.worldPermanence = worldPermanenceList;

        hisaoStateSave = reader.ReadInt32();
        PlayerController.Instance.hisaoState = hisaoStateSave;
    }

    public void SaveUnlockedLogs(BinaryWriter writer)
    {
        savedLogs = PlayerController.Instance.unlockedLogs;
        writer.Write(savedLogs.Count);
        foreach (var log in savedLogs)
        {
            writer.Write(log);
        }
    }
    public void LoadUnlockedLogs(BinaryReader reader)
    {
        int count = reader.ReadInt32();
        savedLogs.Clear();
        for(int i = 0;i < count;i++)
        {
            savedLogs.Add(reader.ReadString());
        }

        PlayerController.Instance.unlockedLogs.Clear();
        PlayerController.Instance.unlockedLogs = savedLogs;
    }

    public void SaveAchievements()
    {
        string filePath = Application.persistentDataPath + "/achv.dat";

        using (MemoryStream memoryStream = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(memoryStream))
            {
                writer.Write(unlockedAchievements.Count);
                foreach (var id in unlockedAchievements)
                {
                    writer.Write(id);
                }
            }

            // Encrypt the data
            byte[] encryptedData = EncryptionTool.Encrypt(memoryStream.ToArray());

            // Write encrypted data to the file
            File.WriteAllBytes(filePath, encryptedData);

            Debug.Log("Encrypted achievements saved.");
        }
    }
    public void LoadAchievements()
    {
        string filePath = Application.persistentDataPath + "/achv.dat";

        if (File.Exists(filePath))
        {
            byte[] encryptedData = File.ReadAllBytes(filePath);

            // Decrypt the data
            byte[] decryptedData = EncryptionTool.Decrypt(encryptedData);

            using (MemoryStream memoryStream = new MemoryStream(decryptedData))
            {
                using (BinaryReader reader = new BinaryReader(memoryStream))
                {
                    if (unlockedAchievements == null)
                    {
                        unlockedAchievements = new HashSet<string>();
                    }

                    int count = reader.ReadInt32();
                    unlockedAchievements.Clear();

                    for (int i = 0; i < count; i++)
                    {
                        string id = reader.ReadString();
                        unlockedAchievements.Add(id);
                    }

                    AchievementManager.instance.LoadUnlockedAchievements(unlockedAchievements);
                }
            }
        }
        else
        {
            Debug.LogWarning("No encrypted achievement file found at " + filePath);
        }
    }

    public void SaveSealsInOrder(BinaryWriter writer)
    {
        sealsUnlocked = PlayerController.Instance.unlockedSeals;
        writer.Write(sealsUnlocked.Count);
        foreach (var seal in sealsUnlocked)
        {
            writer.Write(seal);
        }
        
    }
    public void LoadSealsInOrder(BinaryReader reader)
    {
        int count = reader.ReadInt32();
        sealsUnlocked.Clear();
        for(int i = 0;i < count;i++)
        {
            savedLogs.Add(reader.ReadString());
        }

        PlayerController.Instance.unlockedSeals.Clear();
        PlayerController.Instance.unlockedSeals = savedLogs;
    }

}
