using UnityEngine;
using GameCreator;
using GameCreator.Runtime.VisualScripting;

public class SequenceManager : MonoBehaviour
{
    public string inputPass; // El Password que tu elijes
    public string secondPass; // Por si se ocupa otro pass

    public Actions actions; // Las acciones que se haran 
    public Actions actions2; // lode arriba
    public Actions wrongActions; //Acciones si el jugador se equivoca

    public AudioSource auS; // Audio source y clip de sonido para el sonido de fallo en secuencia
    public AudioClip clip;

    public int maxButtonPresses; // Cantidad maxima de botones antes de que el pass se resetee
    public int buttonPresses;
    public string storedText;

    public void ButtonPress(string meaning)
    {
        storedText += meaning;
        buttonPresses++;
        Debug.Log(storedText);
        SequenceCheck();
    }

    public void SequenceCheck()
    {
        // IF WRONG
        if (buttonPresses >= (maxButtonPresses) && storedText != inputPass)
        {
            Debug.Log(storedText + " IS WRONG");
            storedText = "";
            buttonPresses = 0;
            auS.clip = clip;
            auS.Play();

            if (wrongActions != null)
            {
                wrongActions.Run();
            }
        }
        // IF CORRECT
        else if (buttonPresses >= (maxButtonPresses) && storedText == inputPass)
        {
            Debug.Log(storedText + " IS CORRECT");
            buttonPresses = 0;
            storedText = "";
            actions.Run();
        }
    }

    public void RestartSequence()
    {
        buttonPresses = 0;
        storedText = "";
    }

    public void ChangePass()
    {
        inputPass = secondPass;
        actions = actions2;
    }

}
