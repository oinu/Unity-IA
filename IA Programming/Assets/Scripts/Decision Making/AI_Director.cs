using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AI_Director : MonoBehaviour
{
    public AnimationCurve curve;
    public GameObject enemyPrefab;
    private float lineTime, crono;
    private int minEnemys, maxEnemys, round, wave, currentTime;
    private List<GameObject> enemyList;
    private Vector3 mouseClickDownScreenPosition;
    private Camera sceneCamera;
    private Text roundText, timeText;
    // Start is called before the first frame update
    void Start()
    {
        lineTime = 0;
        minEnemys = 0;
        maxEnemys = 10;
        round = 1;
        wave = 0;
        enemyList = new List<GameObject>();
        sceneCamera = Camera.main;

        roundText = GameObject.Find("RoundText").GetComponent<Text>();
        roundText.text = "Round: " + round + "  Wave: " + wave;

        timeText = GameObject.Find("TimeText").GetComponent<Text>();
        currentTime = 60;
        timeText.text = currentTime.ToString();
        crono = Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        // If the player has time
        if (currentTime > 0)
        {
            //Increase the crono
            crono += Time.deltaTime;

            //If the time passed is 1 second decrease the timer
            if (crono >= 1.0f)
            {
                crono = Time.deltaTime;
                currentTime--;
                timeText.text = currentTime.ToString();
            }

            // If don't exist a enemy in scene, create spawn it.
            if (enemyList.Count == 0)
            {
                SpawnEnemy();
            }

            // When the player press the mouse button
            if (Input.GetMouseButtonDown(0))
            {
                //Get the mouse position on the screen
                mouseClickDownScreenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
                //Send a ray
                Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                //If the ray hist with some object
                if (Physics.Raycast(ray, out hit))
                {
                    //If hit on a enemy, destroy it.
                    if (hit.collider.gameObject.tag == "Enemy")
                    {
                        enemyList.RemoveAt(enemyList.IndexOf(hit.transform.gameObject));
                        Destroy(hit.transform.gameObject);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Return the number of enemys. It takes the value from the curve in the time
    /// </summary>
    /// <param name="t">Is the time in the curve</param>
    /// <returns>The total number of enemys</returns>
    int PosLine(float t)
    {
        // If the time is inside the curve
        if (t <= 1.0)
        {
            wave++;
            return (int)Mathf.Lerp(minEnemys, maxEnemys, curve.Evaluate(t));
        }

        // Is a new round
        else
        {
            minEnemys++;
            maxEnemys++;
            lineTime = 0;
            wave = 0;
            round++;
            return 0;
        }
    }

    /// <summary>
    /// Instance a enemys on the scene and add all in to the list
    /// </summary>
    void SpawnEnemy()
    {
        //Increase the current time in the curve
        lineTime += 0.1f;

        // Get the max number of enemys
        int size = PosLine(lineTime);
        roundText.text = "Round: " + round + "  Wave: " + wave;

        //Instantiate all the enemys in random positions
        for (int i = 0; i < size; i++)
        {
            GameObject g = Instantiate(enemyPrefab);
            g.transform.position = new Vector3(Random.Range(-4.5f, 4.5f), 0.25f, Random.Range(-4.5f, 4.5f));
            enemyList.Add(g);
        }
    }
}
