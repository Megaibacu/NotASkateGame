using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    [Header("----------Dialogue----------")]
    public TextMeshProUGUI textComp;
    public List<string> lines;
    public List<string> nightLines;
    public List<string> finalLines;
    public float textSpeed;
    public bool startRace;
    bool inDialogue;
    int index;

    Rigidbody player;
    public GameObject dialogue;
    DayCycle dC;

    private void Start()
    {
        dC = FindObjectOfType<DayCycle>();
        dialogue = GameObject.Find("UI").transform.GetChild(0).gameObject;
        textComp = dialogue.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
        player = FindObjectOfType<InputManager>().GetComponent<Rigidbody>();
        textComp.text = string.Empty;
    }

    private void Update()
    {
        if (inDialogue)
        {
            if (Input.GetButtonDown("DialogueAdvance"))
            {
                if (textComp.text == finalLines[index])
                {
                    NextLine();
                }
                else
                {
                    StopAllCoroutines();
                    textComp.text = finalLines[index];

                }
            }
        }
        else
        {
            if (dC.isDay)
            {
                finalLines = lines;
            }
            else
            {
                finalLines = nightLines;
            }
        }
    }

    public void StartDialogue()
    {
        player.isKinematic = true;
        dialogue.SetActive(true);

        index = 0;

        if (!inDialogue)
        {
            StartCoroutine(TypeLine());
            inDialogue = true;
        }
    }

    public void NextLine()
    {
        if (index < finalLines.Count - 1)
        {
            index++;
            textComp.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            dialogue.SetActive(false);
            player.isKinematic = false;
            inDialogue = false;
            OnDialogueEnd();
        }
    }

    IEnumerator TypeLine()
    {
        foreach (char c in finalLines[index].ToCharArray())
        {
            textComp.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    public void OnDialogueEnd()
    {
        if (startRace) { GetComponent<RaceNPC>().StartTheRace(); }

        textComp.text = string.Empty;
    }
}
