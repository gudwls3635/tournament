using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    [HideInInspector] public static GameManager instance; // 싱글톤
    public Vector2 userCurPos, enemyCurPos;
    void Start()
    {
        SceneManager.LoadScene("StartScene");
        userCurPos = new Vector2(0,1); enemyCurPos =  new Vector2(3, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Awake()
    {

        if (GameManager.instance == null)
        {
            GameManager.instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
        DontDestroyOnLoad(this);

    }
    public void CreateEnemy()
    {
        GameData.instance.enemy = new GameData.CharacterData();
        GameData.instance.enemy.characterId = 0;
        GameData.instance.enemy.hp = 100;
        GameData.instance.enemy.energy = 100;
        GameData.instance.enemy.name = "적1";
    }
}
