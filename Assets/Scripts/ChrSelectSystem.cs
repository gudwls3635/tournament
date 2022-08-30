using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChrSelectSystem : MonoBehaviour
{
    public GameObject canvas;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void startPlay(Button btn)
    {
        for(int i = 0; i< canvas.transform.childCount; i++)
        {
            if (canvas.transform.GetChild(i).name == btn.name)
            {
                List<int> tmp = new List<int>();
                Debug.Log(i);
                GameData.instance.curUserData.playerChr = i;
                int rnd = Random.Range(1, 9);
                GameData.instance.curEnemyData.playerChr = rnd;
                GameData.instance.curEnemyData.cardsList = GameData.instance.chrCardList[rnd];
                switch (GameData.instance.curUserData.playerChr)
                {
                    case 1:
                        tmp.Add(0); tmp.Add(2); tmp.Add(4); tmp.Add(5); tmp.Add(7); tmp.Add(8); tmp.Add(0); tmp.Add(2); tmp.Add(7); tmp.Add(8);
                        tmp.Add(1); tmp.Add(6); tmp.Add(9); tmp.Add(10);
                        GameData.instance.curUserData.cardsList = tmp;
                        break;
                    case 2:
                        tmp.Add(0); tmp.Add(2); tmp.Add(4); tmp.Add(5); tmp.Add(7); tmp.Add(8); tmp.Add(0); tmp.Add(2); tmp.Add(7); tmp.Add(8);
                        tmp.Add(11); tmp.Add(12); tmp.Add(13); tmp.Add(14);
                        GameData.instance.curUserData.cardsList = tmp;
                        break;
                    case 3:
                        tmp.Add(0); tmp.Add(2); tmp.Add(4); tmp.Add(5); tmp.Add(7); tmp.Add(8); tmp.Add(0); tmp.Add(2); tmp.Add(7); tmp.Add(8);
                        tmp.Add(15); tmp.Add(16); tmp.Add(17); tmp.Add(18);
                        GameData.instance.curUserData.cardsList = tmp;
                        break;
                    case 4:
                        tmp.Add(0); tmp.Add(2); tmp.Add(4); tmp.Add(5); tmp.Add(7); tmp.Add(8); tmp.Add(0); tmp.Add(2); tmp.Add(7); tmp.Add(8);
                        tmp.Add(19); tmp.Add(20); tmp.Add(21); tmp.Add(22);
                        GameData.instance.curUserData.cardsList = tmp;
                        break;
                    case 5:
                        tmp.Add(0); tmp.Add(2); tmp.Add(4); tmp.Add(5); tmp.Add(7); tmp.Add(8); tmp.Add(0); tmp.Add(2); tmp.Add(7); tmp.Add(8);
                        tmp.Add(23); tmp.Add(24); tmp.Add(25); tmp.Add(26);
                        GameData.instance.curUserData.cardsList = tmp;
                        break;
                    case 6:
                        tmp.Add(0); tmp.Add(2); tmp.Add(4); tmp.Add(5); tmp.Add(7); tmp.Add(8); tmp.Add(0); tmp.Add(2); tmp.Add(7); tmp.Add(8);
                        tmp.Add(27); tmp.Add(28); tmp.Add(29); tmp.Add(30);
                        GameData.instance.curUserData.cardsList = tmp;
                        break;
                    case 7:
                        tmp.Add(0); tmp.Add(2); tmp.Add(4); tmp.Add(5); tmp.Add(7); tmp.Add(8); tmp.Add(0); tmp.Add(2); tmp.Add(7); tmp.Add(8);
                        tmp.Add(31); tmp.Add(32); tmp.Add(33); tmp.Add(34);
                        GameData.instance.curUserData.cardsList = tmp;
                        break;
                    case 8:
                        tmp.Add(0); tmp.Add(2); tmp.Add(4); tmp.Add(5); tmp.Add(7); tmp.Add(8); tmp.Add(0); tmp.Add(2); tmp.Add(7); tmp.Add(8);
                        tmp.Add(35); tmp.Add(36); tmp.Add(37); tmp.Add(38);
                        GameData.instance.curUserData.cardsList = tmp;
                        break;
                }
                break;
            }
        }
        
        SceneManager.LoadScene("MainScene");
    }
}
