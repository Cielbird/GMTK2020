using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public Image[] fillImages;
    public Image[] coverImages;
    public Animation[] uiAnims;

    public Animation ammoUpAnim;
    public Text ammoCount;
    public Text levelText;

    public GameObject startCamera;
    public GameObject player;
    public GameObject gameUIPanel;
    public GameObject menuUIPanel;
    public GameObject deathMenu;
    public Button continueButton;
    public Text finalScore;
    public ArenaManager arena;

    public void Begin()
    {
        player.SetActive(true);
        Destroy(startCamera);
        gameUIPanel.SetActive(true);
        menuUIPanel.SetActive(false);
        arena.StartCoroutine(arena.SpawnEnemies());
    }

    public void UpdateComponentHP(float newPercentHP, int index, bool isHeal)
    {
        fillImages[index].fillAmount = 1 - newPercentHP;
        if (newPercentHP <= 0)
        {
            coverImages[index].color = Color.red;
        }
        else
        {
            coverImages[index].color = Color.white;
        }
        if(!isHeal)
        {
            uiAnims[index].Play("UIMalfunction");
        }
        else if(isHeal)
        {
            uiAnims[index].Play("UIHeal");
        }
    }

    public void UpdateAmmo(int newAmmo)
    {
        if (ammoCount.text != newAmmo.ToString())
        {
            ammoCount.text = newAmmo.ToString();
            ammoUpAnim.Play();
        }
    }
    
    public void PlayerDeath()
    {
        gameUIPanel.SetActive(false);
        deathMenu.SetActive(true);
        finalScore.text = "WAVE: " + arena.currentLevel;
    }

    public void Replay()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void UpdateLevel(int level)
    {
        levelText.text = level.ToString();
        levelText.GetComponent<Animation>().Play();
    }

    public void AnimateMalfunctioningControls(int i)
    {
        uiAnims[i].Play();
    }

    public void TimedContinueButton()
    {
        StartCoroutine(ContButton());
    }

    IEnumerator ContButton()
    {
        yield return new WaitForSeconds(5);
        continueButton.interactable = true;
    }
}
