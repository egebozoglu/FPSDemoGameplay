using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{
    #region Variables
    public static GameHandler instance;
    public int enemyCount = 8;
    public Text enemyCountText;
    public Text timeText;
    float timeRate = 0f;
    int time;
    public bool playable;
    public GameObject endingPanel;
    public Text endingTimeText;
    #endregion

    private void Awake()
    {
        if (instance==null)
        {
            instance = this;
        }

        playable = true;
    }

    private void Update()
    {
        if (enemyCount == 0)
        {
            playable = false;
        }

        if (playable)
        {
            Cursor.visible = false;
            Timer();
            enemyCountText.text = "Enemy: " + enemyCount.ToString() + "/8";
            timeText.text = "Time: " + time.ToString();
        }
        else
        {
            Cursor.visible = true;
            enemyCountText.text = string.Empty;
            timeText.text = string.Empty;
            endingPanel.SetActive(true);
            endingTimeText.text = "Time: " + time.ToString();
        }
    }

    void Timer()
    {
        timeRate += Time.deltaTime;
        if (timeRate>1f)
        {
            timeRate = 0f;
            time += 1;
        }
    }
}
