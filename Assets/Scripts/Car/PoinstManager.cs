using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PoinstManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private int points = 0;
    [SerializeField] private int lastCall = 0;
    [SerializeField] private GameObject chainSaw2;
    [SerializeField] private GameObject chainSaw3;
    [SerializeField] private GameObject wreckingBall;


    public void GainPoints()
    {
        points += 10;
        if (points >= 100)
        {
            givePowerUp();
            points -= 100;
        }
        pointsText.text = "Points: " + points.ToString();
    }

    private void givePowerUp()
    {
        switch (lastCall)
        {
            case 0:
                chainSaw2.SetActive(true);
                lastCall = 1;
                break;
            case 1:
                chainSaw3.SetActive(true);
                lastCall = 2;
                break;
            case 2:
                wreckingBall.SetActive(true);
                lastCall = 3;
                break;
        }
    }
}
