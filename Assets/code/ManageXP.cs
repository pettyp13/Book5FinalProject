using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class ManageXP : MonoBehaviour
{
    float initialPower, initialAccuracy, initialCommunication, initialXP;
    float currentPower, currentAccuracy, currentCommunication, currentXP;
    float previousPower, previousAccuracy, previousCommunication, previousXP;
    GameObject powerSlider, accuracySlider, communicationSlider, initialXPTextUI, gameManager;
    float deltaPower, deltaAccuracy, deltaCommunication, deltaXP;


    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager");
        //initialPower = 40;
        //initialAccuracy = 40;
        //initialCommunication = 40;
        //initialXP = 50;

        initialPower = gameManager.GetComponent<GameManager>().player.power;
        initialAccuracy = gameManager.GetComponent<GameManager>().player.accuracy;
        initialCommunication = gameManager.GetComponent<GameManager>().player.communication;
        initialXP = gameManager.GetComponent<GameManager>().player.XP;


        currentPower = initialPower;
        currentAccuracy = initialAccuracy;
        currentCommunication = initialCommunication;

        powerSlider = GameObject.Find("powerSlider");
        accuracySlider = GameObject.Find("accuracySlider");
        communicationSlider = GameObject.Find("communicationSlider");
        initialXPTextUI = GameObject.Find("xpGained");

        initialXPTextUI.GetComponent<Text>().text = "" + initialXP;

        powerSlider.GetComponent<Slider>().minValue = initialPower;
        powerSlider.GetComponent<Slider>().maxValue= initialPower + initialXP;


        accuracySlider.GetComponent<Slider>().minValue = initialAccuracy;
        accuracySlider.GetComponent<Slider>().maxValue = initialAccuracy + initialXP;


        communicationSlider.GetComponent<Slider>().minValue = initialCommunication;
        communicationSlider.GetComponent<Slider>().maxValue = initialCommunication + initialXP; 


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {


            gameManager.GetComponent<GameManager>().player.power = (int)currentPower;
            gameManager.GetComponent<GameManager>().player.accuracy = (int)currentAccuracy;
            gameManager.GetComponent<GameManager>().player.communication = (int)currentCommunication;
            gameManager.GetComponent<GameManager>().player.XP = (int)currentXP;
            gameManager.GetComponent<GameManager>().IncreaseStage(1);
            gameManager.GetComponent<GameManager>().LoadNewScene();

        }
        
    }

    void saveCurrentValues()
    {
        previousPower = currentPower;
        previousAccuracy = currentAccuracy;
        previousCommunication = currentCommunication;

    }


    void getNewValues()
    {
        currentPower = powerSlider.GetComponent<Slider>().value;
        currentAccuracy = accuracySlider.GetComponent<Slider>().value;
        currentCommunication = communicationSlider.GetComponent<Slider>().value;
    }

    void calculatDeltas()
    {

        deltaPower = currentPower - initialPower;
        deltaAccuracy = currentAccuracy - initialAccuracy;
        deltaCommunication = currentCommunication - initialCommunication;

        deltaXP = deltaAccuracy + deltaPower + deltaCommunication;

        GameObject.Find("xpGained").GetComponent<Text>().text = "" + (int)(initialXP - deltaXP);

    }

    public void SetXP()
    {
        string currentXPAsText = initialXPTextUI.GetComponent<Text>().text;
        currentXP = Int32.Parse(currentXPAsText);
        saveCurrentValues();
        getNewValues();
        if (currentXP == 0)
        {



            if (currentPower > previousPower) currentPower = previousPower;
            if (currentAccuracy > previousAccuracy) currentAccuracy = previousAccuracy;
            if (currentCommunication > previousCommunication) currentCommunication = previousCommunication;

            powerSlider.GetComponent<Slider>().value = currentPower;
            accuracySlider.GetComponent<Slider>().value = currentAccuracy;
            communicationSlider.GetComponent<Slider>().value = currentCommunication;

        }

        calculatDeltas();
        GameObject.Find("powerLabel").GetComponent<Text>().text = "Power (" + (int)currentPower + ")";
        GameObject.Find("accuracyLabel").GetComponent<Text>().text = "Accuracy (" + (int)currentAccuracy+ ")";
        GameObject.Find("communicationLabel").GetComponent<Text>().text = "Communication (" + (int)currentCommunication + ")";



    }

}
