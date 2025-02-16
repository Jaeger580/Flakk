/* Jakob Jaeger
 * 2/15/2025
 * Script for handling the majority of the Tutorial scene.
 * Will need to update tutorial text when events are triggered?
 * Contract terminal will handle ending and resetting of tutorial?
 */

using GeneralUtility.GameEventSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI ObjectiveText;

    [SerializeField]
    private tutorialObjective[] objectives;

    private GameEvent currentObjEvent;
    private tutorialObjective currentObj;
    private GameEventListener currentListener;

    private int objCount = 0;
    private int currentCount = 0;

    private void Awake()
    {
        ObjectiveText.text = string.Empty;
    }

    // Start is called before the first frame update
    void Start()
    {

        // On start, load the first objective and initialize event listeners.
        objCount = objectives.Length;
        currentCount = 0;

        currentObjEvent = objectives[currentCount].targetEvent;
        ObjectiveText.text = objectives[currentCount].text;

        // set up a new event listener for the new event
        currentListener = gameObject.AddComponent<GameEventListener>();
        currentListener.Events.Add(currentObjEvent);
        currentListener.Response = new();
        currentListener.Response.AddListener(() => ObjectiveComplete());

        // register
        currentObjEvent.RegisterListener(currentListener);

}

    // updates the objective text and exchanges listeners for the new objective.
    private void ObjectiveComplete() 
    {
        currentCount++;

        // If there is a next objective
        if (currentCount < objCount) 
        {
            tutorialObjective nextObj = objectives[currentCount];

            // if there was a previous objective loaded, remove its old event listener from the event.
            if (currentObjEvent != null)
            {
                //currentObjEvent.UnregisterListener(currentListener);
                currentObjEvent.UnregisterListener(currentListener);
                currentListener.Events.Remove(currentObjEvent);
            }

            ObjectiveText.text = nextObj.text;

            currentObjEvent = nextObj.targetEvent;
            currentListener.Events.Add(currentObjEvent);
            currentObjEvent.RegisterListener(currentListener);

            Debug.Log("Objective Event Added to Listener");

        }
        else 
        {
            EndMission();
        }
        

        //// set up a new event listener for the new event
        //currentListener = gameObject.AddComponent<GameEventListener>();
        //currentListener.Events.Add(currentObjEvent);
        //currentListener.Response = new();
        //currentListener.Response.AddListener(() => objectiveComplete(new tutorialObjective()));

        //// register
        //currentObjEvent.RegisterListener(currentListener);
    }

    private void EndMission()
    {
        ObjectiveText.text = "Tutorial complete. Use the contract terminal to start a real mission!";
    }
}

[Serializable]
public class tutorialObjective
{
    public string text;
    public GameEvent targetEvent;

}
