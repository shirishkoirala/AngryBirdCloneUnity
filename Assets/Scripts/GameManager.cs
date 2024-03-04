using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int MaxNumberOfShots = 3;
    [SerializeField] private float _secondsToWaitBeforeDeathCheck = 3f;
    private int _usedNumberOfShots;
    private IconHandler _iconHandler;
    private List<Pig> _pigs = new List<Pig>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        _iconHandler = GameObject.FindObjectOfType<IconHandler>();
        Pig[] pigs = FindObjectsOfType<Pig>();
        for (int i = 0; i < pigs.Length; i++)
        {
            _pigs.Add(pigs[i]);
        }
    }

    public void UsedShots()
    {
        _usedNumberOfShots++;
        _iconHandler.UseShot(_usedNumberOfShots);
        CheckForLastShot();
    }

    public bool HasEnoughShots()
    {
        if (_usedNumberOfShots < MaxNumberOfShots)
        {
            return true;
        }
        return false;
    }

    public void CheckForLastShot()
    {
        if (_usedNumberOfShots == MaxNumberOfShots)
        {
            StartCoroutine(CheckAfterWaitTime());
        }
    }

    private IEnumerator CheckAfterWaitTime()
    {
        yield return new WaitForSeconds(_secondsToWaitBeforeDeathCheck);
        if(_pigs.Count == 0){
            WinGame();
        }else{
            LoseGame();
        }
    }

    public void RemovePig(Pig pig)
    {
        _pigs.Remove(pig);
    }

    private void CheckForAllDeadPigs(){
        if(_pigs.Count == 0){
            WinGame();
        }
    }
    #region Win/Lose
    private void WinGame(){

    }
    private void LoseGame(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion
}
