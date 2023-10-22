using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour
{
    public float timeRemaining = 10;
    public bool timerIsRunning;
    public TextMeshPro TMP_timer;
    
    private float startTime;
    private float timePercentageForPieChart = 1f;
    private float levelCompletionTime;
    private bool levelCompleted;
    private float rankTimePercent_Bronze = 0.75f;
    private float rankTimePercent_Silver = 0.6f;
    private float rankTimePercent_Gold = 0.36f;

    public PieChartControllerSimple rankPie_Bronze;
    public PieChartControllerSimple rankPie_Silver;
    public PieChartControllerSimple rankPie_Gold;
    
    public GameObject UI_RankingDisplay_DNF;
    public GameObject UI_RankingDisplay_Bronze;
    public GameObject UI_RankingDisplay_Silver;
    public GameObject UI_RankingDisplay_Gold;

    
    public enum Ranking
    {
        DNF,
        Gold,
        Silver,
        Bronze
    };

    public Ranking myScoreRanking = Ranking.DNF;

    public PieChartControllerSimple pChart;
    private void Start()
    {
        TMP_timer = GetComponent<TextMeshPro>();
        startTime = timeRemaining;
        // Starts the timer automatically
        timerIsRunning = true;

        if (rankPie_Bronze)
            rankPie_Bronze.SetValue(rankTimePercent_Bronze);
        if (rankPie_Silver)
            rankPie_Silver.SetValue(rankTimePercent_Silver); 
        if (rankPie_Gold)
            rankPie_Gold.SetValue(rankTimePercent_Gold);
    }
    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0 && !levelCompleted)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else if (levelCompleted)
            {
                DisplayTime(levelCompletionTime);
                DetermineRanking();
            }
            else
            {
                TMP_timer.text = "Time's Up!";
                timeRemaining = 0;
                timerIsRunning = false;
            }
        }
    }
    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60); 
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        TMP_timer.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (pChart != null)
        {
            timePercentageForPieChart = (timeRemaining / startTime);
            pChart.SetValue(timePercentageForPieChart);
        }
    }

    public void LevelCompleted()
    {
        levelCompletionTime = timeRemaining;
        levelCompleted = true;
    }

    public void DetermineRanking()
    {
        float currentTimePercent = (timeRemaining / startTime);

        if (currentTimePercent <= rankTimePercent_Bronze)
        {
            myScoreRanking = Ranking.DNF;
        }
        else if (currentTimePercent <= rankTimePercent_Silver)
        {
            myScoreRanking = Ranking.Bronze;
        }
        else if (currentTimePercent <= rankTimePercent_Gold)
        {
            myScoreRanking = Ranking.Silver;
        }
        else if (currentTimePercent > rankTimePercent_Gold)
        {
            myScoreRanking = Ranking.Gold;
        }
        
        DisplayRankingUI(myScoreRanking);
    }

    private void DisplayRankingUI(Ranking myRanking)
    {
        switch (myRanking)
        {
            case Ranking.DNF:
                UI_RankingDisplay_DNF.SetActive(true);
                return;
            case Ranking.Bronze:
                UI_RankingDisplay_Bronze.SetActive(true);
                return;
            case Ranking.Silver:
                UI_RankingDisplay_Silver.SetActive(true);
                return;
            case Ranking.Gold:
                UI_RankingDisplay_Gold.SetActive(true);
                return;
        }
    }
}