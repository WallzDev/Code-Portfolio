using GameCreator.Runtime.VisualScripting;
using System.Collections;
using UnityEngine;

public class DispositivoRecargaSolar : MonoBehaviour
{
    public Animator anim;
    public AnimationClip clip;
    public string stateName;
    public Actions correctActions;
    public Actions incorrectActions;

    private float frameRate;
    private int currentFrame;
    private int totalFrames;

    private string password;
    public string enteredPass;
    private int passState;
    private int returnOnReset;

    public AudioSource audiosrc;
    public AudioClip correctAudio;
    public AudioClip incorrectAudio;
    

    private void Start()
    {
        if (clip == null)
        {
            Debug.LogWarning("No AnimationClip assigned!");
            return;
        }

        frameRate = clip.frameRate;
        totalFrames = Mathf.FloorToInt(clip.length * frameRate);
        anim.speed = 0f;
        ShowFrame(currentFrame);
    }

    void ShowFrame(int frame)
    {
        if (clip == null) return;

        frame = (frame % totalFrames + totalFrames) % totalFrames;
        currentFrame = frame;

        float normalizedTime = (float)frame / totalFrames;

        anim.speed = 0f;
        anim.Play(stateName, 0, normalizedTime);
        anim.Update(0);
    }

    public void PlayNextFrame()
    {
        ShowFrame(currentFrame + 1);
        enteredPass += "R";
    }
    public void PlayPreviousFrame()
    {
        ShowFrame(currentFrame - 1);
        enteredPass += "L";
    }
    public void ResetAnim()
    {
        returnOnReset = 0;
        passState = 0;
        incorrectActions.Invoke();
        currentFrame = 0;
        
        enteredPass = "";
    }

    public void CheckPass()
    {
        switch (passState)
        {
            case 0:
                password = "LL"; break;
            case 1:
                password = "RRRRRRR"; break;
            case 2:
                password = "LLLLLLLL"; break;
            case 3:
                password = "L"; break;
            case 4:
                password = "RRRRRRRR"; break;
            case 5:
                password = "RRRRR"; break;
        }

        if (enteredPass == password)
        {
            if (passState == 5) // Cuando ya se pone el ultimo password
            {
                correctActions?.Invoke();
                anim.speed = 1;
                anim.Play(stateName);
            }

            audiosrc.PlayOneShot(correctAudio);
            passState++;
            enteredPass = "";
        }
        else if (enteredPass != password)
        {
            audiosrc.PlayOneShot(incorrectAudio);
            passState = 0;
            enteredPass = "";
            incorrectActions.Invoke();
        }

    }

    public void PlayWrongAnim()
    {
        StartCoroutine(WrongAnim());
    }

    public IEnumerator WrongAnim()
    {
        anim.speed = 1;
        anim.Play(stateName);
        yield return new WaitForSeconds(.666f);
        anim.Play("Spin");
        yield return new WaitForSeconds(.666f);
        anim.speed = 0;
        ResetAnim();
        ShowFrame(0);
    }

}
