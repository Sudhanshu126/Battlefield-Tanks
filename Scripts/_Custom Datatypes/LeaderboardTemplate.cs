using TMPro;
using UnityEngine;

public class LeaderboardTemplate : MonoBehaviour
{
    [SerializeField] private TMP_Text nicknameText, scoreText;

    public void SetTemplateData(string nickname, int score)
    {
        nicknameText.text = nickname;
        scoreText.text = score.ToString();
    }
}
