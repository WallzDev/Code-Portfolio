using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    public AudioSource audioSource;
    private Coroutine fadeCoroutine;
    public Coroutine lowerCoroutine;
    public Coroutine restoreCoroutine;
    [Space(10)]

    public AudioClip[] areaMusic;
    private int currentAreaIndex = -1;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }


    public void PlayMusic(int areaIndex)
    {
        if (currentAreaIndex == areaIndex && audioSource.isPlaying) return;
        currentAreaIndex = areaIndex;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeMusic(areaMusic[areaIndex], areaIndex));
    }

    // True lowers the vol, False Restores it
    public void ChangeMusicVol(bool lowerVol, float amount, float speed)
    {

        if (lowerCoroutine != null)
        {
            StopCoroutine(lowerCoroutine);
        }
        if (restoreCoroutine != null)
        {
            StopCoroutine(restoreCoroutine);
        }


        if (lowerVol)
        {
            lowerCoroutine = StartCoroutine(LowerMusicVolume(amount, speed));
        }
        else if (!lowerVol)
        {
            restoreCoroutine = StartCoroutine(RestoreVolumeToFull(speed));
        }
    }
    private IEnumerator FadeMusic(AudioClip newClip, int areaIndex)
    {
        float fadeInTime = 3f;
        float fadeOutTime = 1.5f;
        float startVolume = audioSource.volume;

        // Fade out
        for (float t = 0; t < fadeOutTime; t += Time.unscaledDeltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeOutTime);
            yield return null;
        }

        audioSource.volume = 0;
        audioSource.clip = newClip;
        audioSource.Play();

        if (currentAreaIndex != areaIndex) yield break;

        // Fade in
        for (float t = 0; t < fadeInTime; t += Time.unscaledDeltaTime)
        {
            float targetVol = 1;
            if (GameManager.Instance.gameIsPaused)
            {
                targetVol = .2f;
            }
            if (PlayerController.Instance.pState.gameMenu)
            {
                targetVol = .4f;
            }
            audioSource.volume = Mathf.Lerp(0, targetVol, t / fadeInTime);
            yield return null;
        }

    }

    public IEnumerator LowerMusicVolume(float vol, float speed)
    {
        float startVolume = audioSource.volume;
        float targetVolume = vol;

        for (float t = 0; t < 1; t += Time.unscaledDeltaTime * speed)
        {
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, t);
            yield return null;
        }

        audioSource.volume = targetVolume;
    }

    public IEnumerator RestoreVolumeToFull(float speed)
    {
        float startVolume = audioSource.volume;
        for (float t = 0; t < 1; t += Time.unscaledDeltaTime * speed)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 1, t);
            yield return null;
        }
        audioSource.volume = 1;
    }
}

