using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class PlayerScore : MonoBehaviour
{
    protected int m_Score = 0;
    public int GetScore { get => m_Score; }
    // Make sure that this variable matches the number of Scores
    protected int m_MaxScoreInLevel;
    public TextMeshProUGUI m_TxtScore;
    public GameObject m_CompleteWindow;

    private void Start()
    {
        // Get Score count > This is important, 
        // NOTE: if you do not have an Object that holds all the Scores, you have to enter the value manually
        m_MaxScoreInLevel = transform.Find("/Scores").childCount;
    }
    public void AddNewScore(int score)
    {
        m_Score += score;
        // Update UI => txt Score
        if (m_TxtScore) m_TxtScore.text = "Score : " + m_Score;
    }
    private void MoveNewScoreToScore(GameObject goScore)
    {
        // Convert (m_TxtScore) To World Position 
        var _ToScore = Camera.main.ScreenToWorldPoint(m_TxtScore.transform.position);
        // Move + Roatet + Scale
        LeanTween.move(goScore, _ToScore, 0.3f).setOnStart(() => RoateScore(goScore).setOnStart(() => ScaleScore(goScore))).setOnComplete(() => Destroy(goScore));
    }
    protected LTDescr RoateScore(GameObject goScore) => LeanTween.rotate(goScore, Vector3.up * -170, 0.1f).setLoopPingPong(5);
    protected LTDescr ScaleScore(GameObject goScore) => LeanTween.scale(goScore, Vector3.one * .45f, 0.3f).setEaseLinear();

    // 100/3 =>  33% lone star ; 66% tow star ; 99% all star
    public float GetPercent => ((float)m_Score / (float)m_MaxScoreInLevel) * 100;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Score"))
        {
            // Add score
            AddNewScore(1);
            // Move the Score to score
            MoveNewScoreToScore(other.gameObject);
        }
        // The player collecting one star ,  can move to the next stage
        m_CompleteWindow.SetActive(other.CompareTag("Complete") && GetPercent >= 30f);
    }
}
