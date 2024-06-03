using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;

public class UI_Dialogue : MonoBehaviour
{
    public RawImage NPCIcon;
    public TMP_Text NPCName;

    public TMP_Text lineContent;

    public List<GameObject> responses;
    
    
    private NPC_Speech currentDialogue;


    public void StartDialogue(NPC npc)
    {

        Instantiate(npc.token.rend, NPCIcon.transform);

        //NPCIcon.texture = Races.GetRace(data.raceType).sprite;
        NPCName.text = npc.data.name;

        
        currentDialogue = npc.speech;

        currentDialogue.OnNextLine += NextLine;
        currentDialogue.OnChoices += Choices;

    
    }

    public void EndDialogue()
    {
        currentDialogue.OnNextLine -= NextLine;
        currentDialogue.OnChoices -= Choices;

        currentDialogue = null;
    }

    private void NextLine(string line)
    {
        lineContent.text = line;
    }

    private void Choices(List<Choice> choices)
    {
        for (int i = 0; i < responses.Count; i++)
        {
            if (i < choices.Count)
            {
                responses[i].SetActive(true);
                responses[i].GetComponentInChildren<TMP_Text>().text = choices[i].text;
            }
            else
            {
                responses[i].SetActive(false);
            }
        }
        
    }

    public void ClickedChoice(int index)
    {
        foreach (GameObject response in responses)
        {
            response.SetActive(false);
        }

        currentDialogue.SelectedChoice = index;
    
    }


}
