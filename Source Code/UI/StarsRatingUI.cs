using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarsRatingUI : MonoBehaviour
{
    [SerializeField] private GameObject[] blackStars;
    [SerializeField] private GameObject[] goldStars;

    private void Awake()
    {
        foreach (GameObject blackStar in blackStars)
        {
            blackStar.SetActive(true);
        }
        foreach (GameObject blackStar in goldStars)
        {
            blackStar.SetActive(false);
        }
    }

    /// <summary>
    /// Enables the given amount of stars to be visible
    /// </summary>
    /// <param name="rating">the amount of stars to enable, 
    /// the number must be between 0 and the amount of stars on the scoreboard</param>
    public void SetStarRating(int rating)
    {
        if (rating < 0 || rating > goldStars.Length)
            return;

        for (int i = 0; i < rating; i++)
        {
            goldStars[i].SetActive(true);
        }
    }
}
