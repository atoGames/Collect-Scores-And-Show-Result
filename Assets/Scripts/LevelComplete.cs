using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelComplete : MonoBehaviour
{
    protected PlayerScore m_PlayerScore;
    public TextMeshProUGUI m_TxtTotalScore;
    public Button m_BtnTryAgain;
    public Transform m_Stars;
    private const float m_CompletePercentForEveryStar = 100 / 3;
    public bool m_UseFillAmount = false;

    // Fill amount
    protected float m_Timer = 0;
    protected float m_Duration = 1;
    protected int m_CurrentStarIndex = 0; // start from 0 to last star
    protected List<Image> m_StarList = new List<Image>();
    protected float _Percent = 0;
    private int TotalScore = 0;

    private void Awake() // this call once if (GameObject) be active
    {
        // Get player score
        m_PlayerScore ??= FindObjectOfType<PlayerScore>();
        // Try again button
        m_BtnTryAgain?.onClick.RemoveAllListeners();
        m_BtnTryAgain?.onClick.AddListener(TryAgain);

        // Set default =>  OR You can do this manually
        for (var i = 0; i < m_Stars.childCount; i++)
        {
            var _Star = m_Stars.GetChild(i).GetChild(0).GetComponent<Image>();
            _Star.transform.localScale = m_UseFillAmount ? Vector3.one : Vector3.zero;
            _Star.type = m_UseFillAmount ? _Star.type = Image.Type.Filled : _Star.type = Image.Type.Simple;
            _Star.fillAmount = m_UseFillAmount ? 0 : 1;
            _Star.gameObject.SetActive(m_UseFillAmount);
        }
        // Start Update total score
        StartCoroutine(UpdateTextTotalScore(0.3f));
    }
    private void OnEnable() // This call every time the GameObject set to be active
    {
        // Get percent
        _Percent = m_PlayerScore.GetPercent;

        for (var i = 0; i < m_Stars.childCount; i++)
        {
            // Get satr
            var _Star = m_Stars.GetChild(i).GetChild(0).GetComponent<Image>();
            //it star unlock ??
            var _itStarUnlock = _Percent > m_CompletePercentForEveryStar * (i + 1) && !_Star.gameObject.activeSelf;
            // Show Star => Scale 
            if (!m_UseFillAmount && _itStarUnlock)
                ShowStar(_Star.transform);
            // Add star to list => for fill amount
            if (!m_StarList.Contains(_Star)) // Add star to list
                m_StarList.Add(_Star);
        }
        if (m_UseFillAmount)
            StartCoroutine(UpdateFillAmount(0.01f));
    }

    private IEnumerator UpdateTextTotalScore(float delay)
    {
        // Total score 
        TotalScore = 0;
        while (TotalScore < m_PlayerScore.GetScore)
        {
            TotalScore++;
            m_TxtTotalScore.text = "Total Score " + TotalScore;
            yield return new WaitForSeconds(delay);
        }
        print("Update Text Total Score  Done");
    }
    private IEnumerator UpdateFillAmount(float delay)
    {
        while (!(TotalScore >= m_PlayerScore.GetScore) && m_CurrentStarIndex < m_StarList.Count)
        {
            // Add time to timer
            m_Timer += delay;
            // Add percent to fill amount  + Lerp to next star  
            m_StarList[m_CurrentStarIndex].fillAmount = Mathf.Lerp(0, _Percent / m_CompletePercentForEveryStar, m_Timer / m_Duration);
            // Check if fill amount complete
            if (m_Timer >= m_Duration && m_StarList[m_CurrentStarIndex].fillAmount == 1)
            {
                // Take percent
                _Percent -= m_CompletePercentForEveryStar;
                // Reset timer
                m_Timer = 0;
                // Go to next star
                m_CurrentStarIndex++;
            }
            yield return new WaitForSeconds(delay);
        }
        print("Update Fill Amount  Done");
    }

    protected void ShowStar(Transform star) => LeanTween.scale(star.gameObject, Vector3.one, 0.4f).setOnStart(() => star.gameObject.SetActive(true));
    protected void TryAgain() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}
