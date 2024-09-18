using System;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class DialogueController : MonoBehaviour
{
    [SerializeField]
    private PlayerInput menuInput;
    private string lastActionMap;
    [SerializeField]
    private InputActionReference dialogueAction;

    [Space, SerializeField]
    private GameObject dialogueObject;
    private TextMeshProUGUI dialogueText;

    [TextArea, SerializeField]
    public string[] dialogues;

    [SerializeField]
    private float timeBetweenLetters;

    private int currentDialogueIndex = 0;
    private int letterIndex;

    private bool showingText = false;
    private bool displayingDialogue = false;

    [Space, Header("Animator"), SerializeField]
    private Animator[] catAnimations;
    [SerializeField]
    private AudioClip catSound;
    [SerializeField]
    private AudioClip catDialogueSound;
    [SerializeField]
    private AudioClip clickSound;
    private int dialogueSoundIndex;

    [HideInInspector]
    public Action onDialogueEnd;
    [HideInInspector]
    public Action<int> onDialogueLineStart;

    private void Awake()
    {
        dialogueText = dialogueObject.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        //Activar Input
        dialogueAction.action.started += InputPressed;

        lastActionMap = menuInput.currentActionMap.name;
        menuInput.SwitchCurrentActionMap("Dialogue");
    }

    private void OnDisable()
    {
        //Desactivar Input
        dialogueAction.action.started -= InputPressed;
        menuInput.SwitchCurrentActionMap(lastActionMap);

    }

    private void InputPressed(InputAction.CallbackContext obj)
    {
        if (showingText)
        {
            if (displayingDialogue)
            {
                DisplayAllLetters();
            }
            else
            {
                DisplayNextDialogue();
            }
            AudioManager.instance.Play2dOneShotSound(clickSound, "Button");
        }
    }

    public void StartDialogue()
    {
        if (dialogues.Length == 0)
        {
            showingText = false;
            displayingDialogue = false;
            if(onDialogueEnd != null)
                onDialogueEnd();
            gameObject.SetActive(false);
            return;
        }


        //Empezar con el dialogo
        dialogueObject.SetActive(true);
        currentDialogueIndex = 0;
        letterIndex = 0;
        dialogueText.text = dialogues[currentDialogueIndex];
        dialogueText.maxVisibleCharacters = letterIndex;
        showingText = true;
        displayingDialogue = true;
        Invoke("DisplayLetters", timeBetweenLetters);

        foreach (Animator animator in catAnimations)
        {
            animator.SetBool("talking", true);
            animator.SetTrigger("normalTalk"); 
        }

    }

    private void DisplayNextDialogue()
    {

        if (dialogues.Length > currentDialogueIndex)
        {
            foreach (Animator animator in catAnimations)
            {
                animator.SetBool("talking", true);
                animator.SetTrigger("normalTalk");
            }
            //Si aun no se ha acabado el dialogo
            displayingDialogue = true;
            letterIndex = 0;
            dialogueText.text = dialogues[currentDialogueIndex];
            if(onDialogueLineStart != null)
                onDialogueLineStart(currentDialogueIndex);

            dialogueText.maxVisibleCharacters = letterIndex;
            Invoke("DisplayLetters", timeBetweenLetters);
            

        }
        else
        {
            //Si no hay mas dialogos
            showingText = false;
            displayingDialogue = false;
            if(onDialogueEnd != null)
                onDialogueEnd();
            gameObject.SetActive(false);
        }

    }
    private void DisplayLetters()
    {
        if (displayingDialogue)
        {
            
            if (letterIndex >= dialogues[currentDialogueIndex].Length)
            {
                //Exit
                currentDialogueIndex++;
                displayingDialogue = false;
                foreach (Animator animator in catAnimations)
                {
                    animator.SetBool("talking", false);
                }
            }
            dialogueText.maxVisibleCharacters = letterIndex;
            letterIndex++;
            Invoke("DisplayLetters", timeBetweenLetters);
            if(dialogueSoundIndex % 4 == 0)
                AudioManager.instance.Play2dOneShotSound(catDialogueSound, "SFX", 0.35f, 2f, 2.5f);

            dialogueSoundIndex++;
        }
    }

    private void DisplayAllLetters()
    {
        displayingDialogue = false;
        dialogueText.maxVisibleCharacters = dialogues[currentDialogueIndex].Length;
        currentDialogueIndex++;
        foreach (Animator animator in catAnimations)
        {
            animator.SetBool("talking", false);
        }
    }


}
