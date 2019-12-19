using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility_Based : MonoBehaviour
{
    // Canviar per que hi hagi gana i entretaniment. 
    // Tres opcions per cada un d'elles.
    // Cada opció amb un pes diferent.
    // Aplicar Dual Utility

    public AnimationCurve hungry, entertainment;
    private GameObject chips, water, lunch, tv, pc, talk;
    private float currentTime, hungryTime, entretanimentTime;
    private int randomOption;
    // Start is called before the first frame update
    void Start()
    {
        lunch = GameObject.Find("Lunch");
        chips = GameObject.Find("Chips");
        water = GameObject.Find("Water");
        tv = GameObject.Find("TV");
        pc = GameObject.Find("PC");
        talk = GameObject.Find("Talk");

        currentTime = Time.deltaTime;
        hungryTime = 1.0f;
        entretanimentTime = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime >= 1.0f)
        {
            hungryTime -= 0.1f;
            entretanimentTime -= 0.1f;
            currentTime = Time.deltaTime;
        }


        if (hungry.Evaluate(hungryTime) < entertainment.Evaluate(entretanimentTime) && hungry.Evaluate(hungryTime) < 0.5)
        {
            Debug.Log("HUNGRY");
            if (randomOption == -1) randomOption = Random.Range(0, 3);
            switch (randomOption)
            {
                case 0:
                    hungryTime += 0.5f;
                    break;

                case 1:
                    hungryTime += 0.25f;
                    break;

                case 2:
                    hungryTime += 0.1f;
                    break;
            }
            if (hungryTime > 1.0f) hungryTime = 1.0f;

        }
        else if (hungry.Evaluate(hungryTime) > entertainment.Evaluate(entretanimentTime)
         && entertainment.Evaluate(entretanimentTime) < 0.5)
        {
            Debug.Log("ENTRETAIMENT");
            if (randomOption == -1) randomOption = Random.Range(0, 3);
            switch (randomOption)
            {
                case 0:
                    entretanimentTime += 0.5f;
                    break;

                case 1:
                    entretanimentTime += 0.25f;
                    break;

                case 2:
                    entretanimentTime += 0.1f;
                    break;
            }
            if (hungryTime > 1.0f) hungryTime = 1.0f;
        }
        else
        {
            randomOption = -1;
        }

    }
}
