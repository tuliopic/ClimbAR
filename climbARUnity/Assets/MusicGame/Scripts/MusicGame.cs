﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MusicGame : MonoBehaviour
{

    bool DEBUG = false;
    public GameObject prefabHold;
    public LoopManager loopManager;
    string[] soundItems = new string[] { "Brass", "Bass", "Drums", "Piano" }; //path relative to Resources folder
    GameObject[] holds;

    // Use this for initialization
    void Start()
    {

        holds = GameObject.FindGameObjectsWithTag("Hold");

        loopManager = gameObject.GetComponent<LoopManager>();

        loopManager.Setup(soundItems);

        // If starting directly into music scene, holds will be empty
        if (DEBUG)
        {
            holds = ClimbARHandhold.InstantiateHandholds(prefabHold, GetComponent<Camera>(), new float[] { 1f, 1f, 0.2f, 0.2f, 2f, 2f, 0.2f, 0.2f, 1f, 1f, 0.1f, 0.1f, 0.4f, 0.4f, 0.3f, 0.2f });
            holds[0].transform.localPosition = new Vector2(-1, -1);
            holds[1].transform.localPosition = new Vector2(1, 1);
            holds[2].transform.localPosition = new Vector2(-1, 1);
            holds[3].transform.localPosition = new Vector2(1, -1);
        }
        // If not debugging and no holds, just return
        else if (holds.Length <= 0)
        {
            return;
        }

        // Otherwise we actually have holds, assign it to hold
        HashSet<int> usedHoldIndexes = new HashSet<int>();
        HashSet<int> usedSoundIndexes = new HashSet<int>();

        if (holds.Length < soundItems.Length)
        {
            Debug.Log("Not enough handholds for the number of sound items");
        }

        for (int i = 0; i < Mathf.Min(holds.Length, soundItems.Length); i++)
        {
            GameObject soundHold = holds[i];
            if (soundHold == null)
            {
                Debug.Log("no valid hold found");
            }
            else
            {
                ClimbARHandhold.HoldLineRendererActive(soundHold, true);
                ClimbingHold holdScript = soundHold.GetComponent<ClimbingHold>();
                Destroy(holdScript);

                SoundHold soundHoldScript = soundHold.AddComponent<SoundHold>();

                soundHoldScript.Setup(soundItems[i], i, loopManager);

                loopManager.RegisterHold(soundHoldScript.holdIndex, i);

                soundHoldScript.GetComponent<LineRenderer>()
                    .startColor = UnityEngine.Color.cyan;
                soundHoldScript.GetComponent<LineRenderer>()
                    .endColor = UnityEngine.Color.cyan;

                GameObject holdText = new GameObject();
                HoldText holdTextScript = holdText.AddComponent<HoldText>();
                holdTextScript.addText("   "+soundItems[i], holdText, soundHold);
            }
        }
    }

    private int getUniqueRandom(HashSet<int> used, int maxExclusive)
    {
        int index;
        do
        {
            index = UnityEngine.Random.Range(0, maxExclusive);
        } while (used.Contains(index));
        used.Add(index);
        return index;
    }

    // Update is called once per frame
    void Update()
    {
        // Example of how to add a behavior to ConfirmationCanvas:
        // In music game we want to mute the tracks when escape is hit. We add a listner for escape here to 
        // add that behavior. We then add the function UnPauseSounds to the cancel button in this particular 
        // scene to start back up the sounds if the user doesn't choose to quit
        if (Input.GetKeyDown("escape"))
        {
            loopManager.PauseSounds();
        }
    }
    private void OnDisable()
    {
        foreach (GameObject hold in holds)
        {
            if (hold != null)
            {
                ClimbARHandhold.HoldLineRendererActive(hold, false);

                HoldText hTextScript = hold.GetComponent<HoldText>();

                SoundHold script = hold.GetComponent<SoundHold>();
                Destroy(hTextScript);
                Destroy(script);

                ClimbARHandhold.DestroyChildren(hold);
            }
        }
    }

}
