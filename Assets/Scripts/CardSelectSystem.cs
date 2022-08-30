using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CardSelectSystem : MonoBehaviour
{
    public GameObject scrollContent;
    public GameObject curCardObj;
    public GameObject cardPrefab;
    public GameObject cardSelectCan,battleCan;
    public GameObject battleNextBtn;
    public GameObject userHpBar, userEnergyBar,userBoostBar,enemyHpBar,enemyEnergyBar, enemyBoostBar;
    public GameObject cells;
    
    public GameObject enemyUI, userUI;

    List<int> userCards = new List<int>();
    public int[,] curCards = new int[3,2]; // 0:카드 id, 1:카드 리스트 index
    public int[] enemyCards = new int[3];
    public Sprite nullImg;
    public GameObject userCharacter, enemyCharacter;
    public GameObject userCurCard, enemyCurCard; // 유저와 적 object
    Vector2 userCurPos, enemyCurPos;
    int colSize = 455, yColSize = 170; // 한칸당 position 크기

    int userTmpEnergy;
    int roundEnergyHealAmount;
    Coroutine roundCor;
    public Text winOrDfeatText;
    // Start is called before the first frame update
    void Start()
    {
        userUI.transform.GetChild(0).GetComponent<Image>().sprite = GameData.instance.chrImgs[GameData.instance.curUserData.playerChr];
        enemyUI.transform.GetChild(0).GetComponent<Image>().sprite = GameData.instance.chrImgs[GameData.instance.curEnemyData.playerChr];

        GameData.instance.user.hp = 100; GameData.instance.user.energy = 100; GameData.instance.user.boost = 0;
        GameData.instance.enemy.hp = 100; GameData.instance.enemy.energy = 100; GameData.instance.enemy.boost = 0;
        userHpBar.GetComponent<Image>().fillAmount =1.0f;
        userEnergyBar.GetComponent<Image>().fillAmount = 1.0f;
        userBoostBar.GetComponent<Image>().fillAmount = 0.0f;
        enemyHpBar.GetComponent<Image>().fillAmount = 1.0f;
        enemyEnergyBar.GetComponent<Image>().fillAmount = 1.0f;
        enemyBoostBar.GetComponent<Image>().fillAmount = 0.0f;

        userPos = userCharacter.transform.localPosition;
        enemyPos = enemyCharacter.transform.localPosition;

        //라운드 끝날 떄마다 에너지 회복량
        roundEnergyHealAmount=30;
        startCardSelect();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    Vector3 userPos,enemyPos;
    public void startBattle()
    {
        //userCharacter.transform.localPosition = userPos;
        //enemyCharacter.transform.localPosition = enemyPos;
        userCurPos = GameManager.instance.userCurPos; enemyCurPos =GameManager.instance.enemyCurPos;
        
        //적 카드 장착
        ResponEnemyCard();

        battleCan.SetActive(true);
        roundCor = StartCoroutine(NextRound(0));// 라운드 진행
        
    }
    public void ResponEnemyCard()
    {
        int enemyTmpEnergy = GameData.instance.enemy.energy;
        int[] randomCard = new int[3];
        int sumEnergy;
        do
        {
            sumEnergy = 0;
            for (int i = 0; i < 3; i++)
            {
                int card = UnityEngine.Random.Range(0, GameData.instance.curEnemyData.cardsList.Count);
                randomCard[i] = GameData.instance.curEnemyData.cardsList[card];
                sumEnergy += GameData.instance.cardDatas[randomCard[i]].en;
            }

        } while (sumEnergy > enemyTmpEnergy);
        for (int i = 0; i < 3; i++)
        {
            enemyCards[i] = randomCard[i];
        }
    }
    public void exitBattle()
    {
        userCurCard.GetComponent<Image>().sprite = nullImg;
        enemyCurCard.GetComponent<Image>().sprite = nullImg;
        GameManager.instance.userCurPos = userCurPos;
        GameManager.instance.enemyCurPos = enemyCurPos;
        battleNextBtn.SetActive(false);
        battleCan.SetActive(false);
        
    }
    public void startCardSelect()
    {
        
        cardSelectCan.SetActive(true);
        curCards = new int[3, 2];
        userCards = new List<int>();
        userCards = GameData.instance.curUserData.cardsList;
        for (int i = 0; i < curCards.GetLength(0); i++)
        {
            curCards[i, 0] = -1;
            curCards[i, 1] = -1;
        }

        CreateCardList();
        userTmpEnergy = GameData.instance.user.energy;
    }
    public void exitCardSelect ()
    {
        // 카드 리스트 삭제
        for (int i = scrollContent.transform.childCount -1; i >=0 ; i--)
        {
            Destroy(scrollContent.transform.GetChild(i).gameObject);
        }
        // 장착 칸  원래대로
        for (int i = 0; i < curCardObj.transform.childCount; i++)
        {
            curCardObj.transform.GetChild(i).GetComponent<Image>().sprite = nullImg;
        }
        cardSelectCan.SetActive(false);
        startBattle();
    }
    void CreateCardList()
    {
        for(int i = 0;i < userCards.Count; i++)
        {
            GameObject newObj = Instantiate(cardPrefab,new Vector3(0,0,0),transform.rotation,scrollContent.transform);
            newObj.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { cardClick(newObj.GetComponent<UnityEngine.UI.Button>()); });
            newObj.GetComponent<Image>().sprite = GameData.instance.cardDatas[userCards[i]].img;
        }
    }
    public void cardClick(Button btn) // 카드 리스트 중에서 클릭
    {
        int index=-1;
        int equipIndex = -1;
        
        for(int i  = 0; i < scrollContent.transform.childCount; i++)
        {
            if (scrollContent.transform.GetChild(i) == btn.transform)
            {
                index = i;
                break;
            }
        }
        
        // 카드를 장착할 수 있는 에너지 보유 하고 있는지
        if(GameData.instance.GetCardData(userCards[index]).en <= userTmpEnergy)
        {
            for (int i = 0; i < curCards.GetLength(0); i++)
            {
                // 장착칸 비어있는 곳 찾기
                if (curCards[i, 0] == -1)
                {
                    int ch = 0;
                    for (int j = 0; j < curCards.GetLength(0); j++)
                    {
                        if (curCards[j, 1] == index)
                        {
                            ch = 1;
                            break;
                        }
                    }
                    if (ch == 0)
                    {
                        curCards[i, 0] = userCards[index];
                        curCards[i, 1] = index;
                        equipIndex = i;
                        break;
                    }
                }
            }
            if (equipIndex != -1)
            {
                // 장착칸 있으면
                btn.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f);
                curCardObj.transform.GetChild(equipIndex).GetComponent<Image>().sprite = btn.GetComponent<Image>().sprite;
                // 장착한 만큼 에너지 감소
                userTmpEnergy -= GameData.instance.GetCardData(userCards[index]).en;
            }
            else
            {
                // 장착칸 없을 시 
            }
        }
        
        
    }
    public void curCardClick(Button btn) // 현재 장착된 카드 클릭
    {
        int index = int.Parse(btn.name.Substring(4));

        // 장착 해체로 인한 에너지 증가
        userTmpEnergy += GameData.instance.GetCardData(curCards[index, 0]).en;

        // 카드 장착칸에서 제거
        btn.GetComponent<Image>().sprite = nullImg;
        // 카드 리스트 원래대로
        scrollContent.transform.GetChild(curCards[index, 1]).GetComponent<Image>().color = new Color(1,1,1);
        // 장착칸 초기화
        curCards[index, 0] = -1; curCards[index, 1] = -1;
       
    }
    public void nextBtn()
    {
        int ch = 0;
        for(int i = 0; i< curCards.GetLength(0); i++)
        {
            if(curCards[i,0] == -1)
            {
                ch = 1;
                break;
            }
        }
        if(ch == 0 )
        {
            // 장착칸 완료
            exitCardSelect();
        }
    }
    public void BattleNextBtn()
    {
        exitBattle();
        GameData.instance.user.energy += roundEnergyHealAmount; // 유저 에너지 회복
        if (GameData.instance.user.energy > 100) GameData.instance.user.energy = 100;
        GameData.instance.enemy.energy += roundEnergyHealAmount; // 적 에너지 회복
        if (GameData.instance.enemy.energy > 100) GameData.instance.enemy.energy = 100;
        userCharacter.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        enemyCharacter.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        UpdateUIBar();
        startCardSelect();
    }
    IEnumerator NextRound(int round)
    {
        while(round <3)
        {
            Debug.Log("차례 : " + (round+1) );
            yield return new WaitForSeconds(1f);
 
            int userCard = curCards[round, 0], enemyCard = enemyCards[round];
            userCurCard.GetComponent<Image>().sprite = GameData.instance.cardDatas[userCard].img;
            enemyCurCard.GetComponent<Image>().sprite = GameData.instance.cardDatas[enemyCard].img;
            userCharacter.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
            enemyCharacter.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
            // 유저와 적 방어력 초기화 
            GameData.instance.user.guard = 0;
            GameData.instance.enemy.guard = 0;
            if (GameData.instance.GetCardData(userCard).kind > GameData.instance.GetCardData(enemyCard).kind)
            {
                // 유저 먼저
                ExCard(0, userCard, round);
                yield return new WaitForSeconds(2f);
                resetCell();
                ExCard(1, enemyCard, round);
                yield return new WaitForSeconds(2f);
                resetCell();
            }
            else
            {
                // 적 먼저
                ExCard(1, enemyCard, round);
                yield return new WaitForSeconds(2f);
                resetCell();
                ExCard(0, userCard, round);
                yield return new WaitForSeconds(2f);
                resetCell();
            }
            round += 1;
            
        }
        yield return new WaitForSeconds(1f);
        
        if ( !(GameData.instance.user.hp == 0 || GameData.instance.enemy.hp == 0) )
            battleNextBtn.SetActive(true);

    }
    // 색칠 초기화
    void resetCell()
    {
        for (int i = 0; i < cells.transform.childCount; i++)
            cells.transform.GetChild(i).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        userCharacter.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        enemyCharacter.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
    }
    void ExCard(int who, int cardKind,int round) // who  0: user 1: enemy 카드 실행
    {
        

        if (GameData.instance.cardDatas[cardKind].kind == 0)
        {
            // attack
            if (who == 0)
            {
                // user
                userCharacter.GetComponent<SpriteRenderer>().color = new Color(1, 0,0);
                if (GameData.instance.user.energy >= GameData.instance.cardDatas[cardKind].en)
                {
                    // 맵에 색칠
                    for (int i = 0; i < GameData.instance.cardDatas[cardKind].pos.Count; i++)
                    {
                        Vector2 cellPos = userCurPos + GameData.instance.cardDatas[cardKind].pos[i];
                        if(cellPos.x > 0 && cellPos.x <4 && cellPos.y> 0 && cellPos.y < 3)
                            cells.transform.GetChild((int)cellPos.x+(int)cellPos.y*4).GetComponent<SpriteRenderer>().color = new Color(1f,0.28f,0.28f);

                    }
                    for (int i = 0; i < GameData.instance.cardDatas[cardKind].pos.Count; i++)
                    {
                        if (userCurPos + GameData.instance.cardDatas[cardKind].pos[i] == enemyCurPos)
                        {
                            if (GameData.instance.enemy.guard == 0) // 필살 게이지 차징
                            {
                                GameData.instance.user.boost += GameData.instance.cardDatas[cardKind].dm * 2;
                                if (GameData.instance.user.boost > 100) GameData.instance.user.boost = 100;
                            }
                            if (round == 0 && GameData.instance.user.boost ==100) // 필살 게이지 차면 데미지 다르게
                            {
                                GameData.instance.enemy.hp -= GameData.instance.cardDatas[cardKind].dm * 2 - GameData.instance.enemy.guard;
                                GameData.instance.user.boost = 0;
                                
                            }
                            else
                                GameData.instance.enemy.hp -= GameData.instance.cardDatas[cardKind].dm - GameData.instance.enemy.guard;
                            
                            if (GameData.instance.enemy.hp < 0) GameData.instance.enemy.hp = 0;
                        }
                    }
                    GameData.instance.user.energy -= GameData.instance.cardDatas[cardKind].en;
                }

            }
            else
            {
                // enemy
                enemyCharacter.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0);
                if (GameData.instance.enemy.energy >= GameData.instance.cardDatas[cardKind].en)
                {
                    // 맵에 색칠
                    for (int i = 0; i < GameData.instance.cardDatas[cardKind].pos.Count; i++)
                    {
                        Vector2 cellPos = enemyCurPos + GameData.instance.cardDatas[cardKind].pos[i];
                        if (cellPos.x > 0 && cellPos.x < 4 && cellPos.y > 0 && cellPos.y < 3)
                            cells.transform.GetChild((int)cellPos.x + (int)cellPos.y * 4).GetComponent<SpriteRenderer>().color = new Color(1f, 0.28f, 0.28f);

                    }
                    for (int i = 0; i < GameData.instance.cardDatas[cardKind].pos.Count; i++)
                    {
                        if (enemyCurPos + GameData.instance.cardDatas[cardKind].pos[i] == userCurPos)
                        {
                            if (GameData.instance.user.guard == 0) // 필살 게이지 차징
                            {
                                GameData.instance.enemy.boost += GameData.instance.cardDatas[cardKind].dm * 2;
                                if (GameData.instance.enemy.boost > 100) GameData.instance.enemy.boost = 100;
                            }
                            if (round == 0 && GameData.instance.enemy.boost == 100) // 필살 게이지 차면 데미지 다르게
                            {
                                GameData.instance.user.hp -= GameData.instance.cardDatas[cardKind].dm * 2 - GameData.instance.user.guard;
                                GameData.instance.enemy.boost = 0;
                            }
                            else
                                GameData.instance.user.hp -= GameData.instance.cardDatas[cardKind].dm - GameData.instance.user.guard;
                            
                            if (GameData.instance.user.hp < 0) GameData.instance.user.hp = 0;
                        }
                    }
                    GameData.instance.enemy.energy -= GameData.instance.cardDatas[cardKind].en;
                }

            }
        }
        else if(GameData.instance.cardDatas[cardKind].kind == 1)
        {
            // util
            if (who == 0)
            {
                // user
                Vector2 cellPos;
                switch (GameData.instance.cardDatas[cardKind].name)
                {
                    case "healHp":
                        // 맵에 색칠
                        cellPos = userCurPos;
                        cells.transform.GetChild((int)cellPos.x + (int)cellPos.y * 4).GetComponent<SpriteRenderer>().color = new Color(0.28f, 0.28f, 1f);

                        userCharacter.GetComponent<SpriteRenderer>().color = new Color(0, 0, 1);
                        if (round == 0 && GameData.instance.user.boost == 100) // 필살 게이지 차면 다르게
                            GameData.instance.user.hp += 100;
                        else
                            GameData.instance.user.hp += GameData.instance.cardDatas[cardKind].dm;
                        if (GameData.instance.user.hp > 100) GameData.instance.user.hp = 100;
                        break;
                    case "healEnergy":
                        // 맵에 색칠
                        cellPos = userCurPos;
                        cells.transform.GetChild((int)cellPos.x + (int)cellPos.y * 4).GetComponent<SpriteRenderer>().color = new Color(0.28f, 0.28f, 1f);

                        userCharacter.GetComponent<SpriteRenderer>().color = new Color(0, 0, 1);
                        if (round == 0 && GameData.instance.user.boost == 100) // 필살 게이지 차면 다르게
                            GameData.instance.user.energy += 100;
                        else
                            GameData.instance.user.energy += GameData.instance.cardDatas[cardKind].dm;
                        if (GameData.instance.user.energy > 100) GameData.instance.user.energy = 100;
                        break;
                    case "guard":
                        // 맵에 색칠
                        cellPos = userCurPos;
                        cells.transform.GetChild((int)cellPos.x + (int)cellPos.y  * 4).GetComponent<SpriteRenderer>().color = new Color(0.28f, 1f, 0.28f);

                        userCharacter.GetComponent<SpriteRenderer>().color = new Color(0, 1,0);
                        if (round == 0 && GameData.instance.user.boost == 100) // 필살 게이지 차면 다르게
                            GameData.instance.user.guard = 100;
                        else
                            GameData.instance.user.guard = GameData.instance.cardDatas[cardKind].dm;
                        break;
                }
                GameData.instance.user.energy -= GameData.instance.cardDatas[cardKind].en;

            }
            else
            {
                // enemy
                Vector2 cellPos;
                switch (GameData.instance.cardDatas[cardKind].name)
                {
                    case "healHp":
                        // 맵에 색칠
                        cellPos =enemyCurPos;
                        cells.transform.GetChild((int)cellPos.x + (int)cellPos.y  * 4).GetComponent<SpriteRenderer>().color = new Color(0.28f, 0.28f, 1f);

                        enemyCharacter.GetComponent<SpriteRenderer>().color = new Color(0, 0, 1);
                        if (round == 0 && GameData.instance.enemy.boost == 100) // 필살 게이지 차면 다르게
                            GameData.instance.enemy.hp += 100;
                        else
                            GameData.instance.enemy.hp += GameData.instance.cardDatas[cardKind].dm;
                        if (GameData.instance.enemy.hp > 100) GameData.instance.enemy.hp = 100;
                        break;
                    case "healEnergy":
                        // 맵에 색칠
                        cellPos = enemyCurPos;
                        cells.transform.GetChild((int)cellPos.x + (int)cellPos.y  * 4).GetComponent<SpriteRenderer>().color = new Color(0.28f, 0.28f, 1f);

                        enemyCharacter.GetComponent<SpriteRenderer>().color = new Color(0, 0, 1);
                        if (round == 0 && GameData.instance.enemy.boost == 100) // 필살 게이지 차면 다르게
                            GameData.instance.enemy.energy += 100;
                        else
                            GameData.instance.enemy.energy += GameData.instance.cardDatas[cardKind].dm;
                        if (GameData.instance.enemy.energy > 100) GameData.instance.enemy.energy = 100;
                        break;
                    case "guard":
                        // 맵에 색칠
                        cellPos = enemyCurPos;
                        cells.transform.GetChild((int)cellPos.x + (int)cellPos.y * 4).GetComponent<SpriteRenderer>().color = new Color(0.28f, 1f, 0.28f);
                        enemyCharacter.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0);
                        if (round == 0 && GameData.instance.enemy.boost == 100) // 필살 게이지 차면 다르게
                            GameData.instance.enemy.guard = 100;
                        else
                            GameData.instance.enemy.guard = GameData.instance.cardDatas[cardKind].dm;
                        break;
                }
                GameData.instance.enemy.energy -= GameData.instance.cardDatas[cardKind].en;

            }

        }
        else if(GameData.instance.cardDatas[cardKind].kind == 2)
        {
            // move
            if( who == 0)
            {
                // user
                if (GameData.instance.user.energy >= GameData.instance.cardDatas[cardKind].en)
                {
                    if (round == 0 && GameData.instance.user.boost == 100) // 필살 게이지 차면 다르게
                    {
                        // 이동 계산
                        int xAbs = math.abs((int)GameData.instance.cardDatas[cardKind].pos[0].x*2);
                        int yAbs = math.abs((int)GameData.instance.cardDatas[cardKind].pos[0].y*2);
                        Vector2 tmp = userCurPos;
                        for (int r = 1; r <= xAbs; r++)
                        {
                            int xUnit = (int)GameData.instance.cardDatas[cardKind].pos[0].x*2 / xAbs;
                            if (-1 < tmp.x + xUnit && 4 > tmp.x + xUnit)
                            {
                                tmp += new Vector2(xUnit, 0);
                            }

                        }
                        for (int c = 1; c <= yAbs; c++)
                        {
                            int yUnit = (int)GameData.instance.cardDatas[cardKind].pos[0].y*2 / yAbs;
                            if (-1 < tmp.y + yUnit && 3 > tmp.y + yUnit)
                            {
                                tmp += new Vector2(0, yUnit);
                            }
                        }
                        // 맵에 색칠
                        cells.transform.GetChild((int)tmp.x + (int)tmp.y  * 4).GetComponent<SpriteRenderer>().color = new Color(0.28f, 1f, 0.28f);

                        // 실제 이동
                        tmp = userCurPos;
                        for (int r = 1; r <= xAbs; r++)
                        {
                            int xUnit = (int)GameData.instance.cardDatas[cardKind].pos[0].x * 2 / xAbs;
                            if (-1 < tmp.x + xUnit && 4 > tmp.x + xUnit)
                            {
                                userCharacter.transform.localPosition += new Vector3(xUnit * colSize, 0);
                                userCurPos += new Vector2(xUnit, 0);
                            }

                        }
                        for (int c = 1; c <= yAbs; c++)
                        {
                            int yUnit = (int)GameData.instance.cardDatas[cardKind].pos[0].y * 2 / yAbs;
                            if (-1 < tmp.y + yUnit && 3 > tmp.y + yUnit)
                            {
                                userCharacter.transform.localPosition += new Vector3(0, yUnit * yColSize);
                                userCurPos += new Vector2(yUnit, 0);
                            }
                        }
                        GameData.instance.user.boost = 0;
                    }
                    else
                    {
                        // 이동 계산
                        Debug.Log(GameData.instance.cardDatas[cardKind].pos[0]);
                        int xAbs = math.abs((int)GameData.instance.cardDatas[cardKind].pos[0].x);
                        int yAbs = math.abs((int)GameData.instance.cardDatas[cardKind].pos[0].y);
                        Vector2 tmp = userCurPos;
                        for (int r = 1; r <= xAbs; r++)
                        {
                            int xUnit = (int)GameData.instance.cardDatas[cardKind].pos[0].x / xAbs;
                            if (-1 < tmp.x + xUnit && 4 > tmp.x + xUnit)
                            {
                                tmp += new Vector2(xUnit, 0);
                            }

                        }
                        for (int c = 1; c <= yAbs; c++)
                        {
                            int yUnit = (int)GameData.instance.cardDatas[cardKind].pos[0].y / yAbs;
                            if (-1 < tmp.y + yUnit && 3 > tmp.y + yUnit)
                            {
                                tmp += new Vector2(0, yUnit);
                            }
                        }
                        // 맵에 색칠
                        Debug.Log(userCurPos);
                        cells.transform.GetChild((int)tmp.x + (int)tmp.y * 4).GetComponent<SpriteRenderer>().color = new Color(0.28f, 1f, 0.28f);
                        // 실제 이동
                        tmp = userCurPos;
                        for (int r = 1; r <= xAbs; r++)
                        {
                            int xUnit = (int)GameData.instance.cardDatas[cardKind].pos[0].x / xAbs;
                            if (-1 < tmp.x + xUnit && 4 > tmp.x + xUnit)
                            {
                                userCharacter.transform.localPosition += new Vector3(xUnit * colSize, 0);
                                userCurPos += new Vector2(xUnit, 0);
                            }

                        }
                        for (int c = 1; c <= yAbs; c++)
                        {
                            int yUnit = (int)GameData.instance.cardDatas[cardKind].pos[0].y / yAbs;
                            if (-1 < tmp.y + yUnit && 3 > tmp.y + yUnit)
                            {
                                userCharacter.transform.localPosition += new Vector3(0, yUnit * yColSize);
                                userCurPos += new Vector2(0, yUnit);
                            }
                        }
                    }
                    GameData.instance.user.energy -= GameData.instance.cardDatas[cardKind].en;
                }
                    
            }
            else
            {
                // enemy
                if (GameData.instance.enemy.energy >= GameData.instance.cardDatas[cardKind].en)
                {
                    if (round == 0 && GameData.instance.enemy.boost == 100) // 필살 게이지 차면 다르게
                    {
                        // 이동 계산
                        int xAbs = math.abs((int)GameData.instance.cardDatas[cardKind].pos[0].x * 2 );
                        int yAbs = math.abs((int)GameData.instance.cardDatas[cardKind].pos[0].y * 2);
                        Vector2 tmp = enemyCurPos;
                        for (int r = 1; r <= xAbs; r++)
                        {
                            int xUnit = (int)GameData.instance.cardDatas[cardKind].pos[0].x*2 / xAbs;
                            if (-1 < tmp.x + xUnit && 4 > tmp.x + xUnit)
                            {
                                tmp += new Vector2(xUnit, 0);
                            }

                        }
                        for (int c = 1; c <= yAbs; c++)
                        {
                            int yUnit = (int)GameData.instance.cardDatas[cardKind].pos[0].y*2 / yAbs;
                            if (-1 < tmp.y + yUnit && 3 > tmp.y + yUnit)
                            {
                                tmp += new Vector2(0, yUnit);
                            }
                        }
                        // 맵에 색칠
                        cells.transform.GetChild((int)tmp.x + (int)tmp.y * 4).GetComponent<SpriteRenderer>().color = new Color(0.28f, 1f, 0.28f);
                        // 실제 이동
                        tmp = enemyCurPos;
                        for (int r = 1; r <= xAbs; r++)
                        {
                            int xUnit = (int)GameData.instance.cardDatas[cardKind].pos[0].x * 2 / xAbs;
                            if (-1 < tmp.x + xUnit && 4 > tmp.x + xUnit)
                            {
                                enemyCharacter.transform.localPosition += new Vector3(xUnit * colSize, 0);
                                enemyCurPos += new Vector2(xUnit, 0);
                            }

                        }
                        for (int c = 1; c <= yAbs; c++)
                        {
                            int yUnit = (int)GameData.instance.cardDatas[cardKind].pos[0].y * 2 / yAbs;
                            if (-1 < tmp.y + yUnit && 3 > tmp.y + yUnit)
                            {
                                enemyCharacter.transform.localPosition += new Vector3(0, yUnit * yColSize);
                                enemyCurPos += new Vector2(0, yUnit);
                            }
                        }
                        GameData.instance.enemy.boost = 0;
                    }
                    else
                    {
                        // 이동 계산
                        int xAbs = math.abs((int)GameData.instance.cardDatas[cardKind].pos[0].x);
                        int yAbs = math.abs((int)GameData.instance.cardDatas[cardKind].pos[0].y);
                        Vector2 tmp = enemyCurPos;
                        for (int r = 1; r <= xAbs; r++)
                        {
                            int xUnit = (int)GameData.instance.cardDatas[cardKind].pos[0].x / xAbs;
                            if (-1 < tmp.x + xUnit && 4 > tmp.x + xUnit)
                            {
                                tmp += new Vector2(xUnit, 0);
                            }

                        }
                        for (int c = 1; c <= yAbs; c++)
                        {
                            int yUnit = (int)GameData.instance.cardDatas[cardKind].pos[0].y / yAbs;
                            if (-1 < tmp.y + yUnit && 3 > tmp.y + yUnit)
                            {
                                tmp += new Vector2(0, yUnit);
                            }
                        }
                        // 맵에 색칠
                        cells.transform.GetChild((int)tmp.x + (int)tmp.y  * 4).GetComponent<SpriteRenderer>().color = new Color(0.28f, 1f, 0.28f);
                        // 실제 이동
                        tmp = enemyCurPos;
                        for (int r = 1; r <= xAbs; r++)
                        {
                            int xUnit = (int)GameData.instance.cardDatas[cardKind].pos[0].x / xAbs;
                            if (-1 < tmp.x + xUnit && 4 > tmp.x + xUnit)
                            {
                                enemyCharacter.transform.localPosition += new Vector3(xUnit * colSize, 0);
                                enemyCurPos += new Vector2(xUnit, 0);
                            }

                        }
                        for (int c = 1; c <= yAbs; c++)
                        {
                            int yUnit = (int)GameData.instance.cardDatas[cardKind].pos[0].y / yAbs;
                            if (-1 < tmp.y + yUnit && 3 > tmp.y + yUnit)
                            {
                                enemyCharacter.transform.localPosition += new Vector3(0, yUnit * yColSize);
                                enemyCurPos += new Vector2(0, yUnit);
                            }
                        }
                    }
                        

                    GameData.instance.enemy.energy -= GameData.instance.cardDatas[cardKind].en;
                }
                    
            }
           
        }
        UpdateUIBar();
        if (GameData.instance.user.hp == 0)
        {
            // 플레이어 패배
            StartCoroutine(WinOrDfeat(0));
            StopCoroutine(roundCor);
        }
        else if (GameData.instance.enemy.hp == 0)
        {
            // 플레이어 승리
            StartCoroutine(WinOrDfeat(1));
            StopCoroutine(roundCor);
        }
    }
    IEnumerator WinOrDfeat(int kind) // 1 : 플레이어 승리, 0: 플레이어 패배
    {
        winOrDfeatText.gameObject.SetActive(true);
        if (kind == 1)
        {
            winOrDfeatText.text = "Player Win!!!";
        }
        else
        {
            winOrDfeatText.text = "Player Defeat!!!";
        }
        
        yield return new WaitForSeconds(3);
        winOrDfeatText.gameObject.SetActive(false);
        if (kind == 1)
        {
            SceneManager.LoadScene("StartScene");
        }
        else
        {
            SceneManager.LoadScene("MainScene");
        }
    }
    void UpdateUIBar()
    {
        userHpBar.transform.GetChild(0).GetComponent<Image>().fillAmount = GameData.instance.user.hp/ 100.0f;
        userEnergyBar.transform.GetChild(0).GetComponent<Image>().fillAmount = GameData.instance.user.energy / 100.0f;
        userBoostBar.transform.GetChild(0).GetComponent<Image>().fillAmount = GameData.instance.user.boost / 100.0f;
        enemyHpBar.transform.GetChild(0).GetComponent<Image>().fillAmount = GameData.instance.enemy.hp / 100.0f;
        enemyEnergyBar.transform.GetChild(0).GetComponent<Image>().fillAmount = GameData.instance.enemy.energy / 100.0f;
        enemyBoostBar.transform.GetChild(0).GetComponent<Image>().fillAmount = GameData.instance.enemy.boost / 100.0f;
        userHpBar.transform.GetChild(1).GetComponent<Text>().text = GameData.instance.user.hp.ToString();
        userEnergyBar.transform.GetChild(1).GetComponent<Text>().text = GameData.instance.user.energy.ToString();
        userBoostBar.transform.GetChild(1).GetComponent<Text>().text = GameData.instance.user.boost.ToString();
        enemyHpBar.transform.GetChild(1).GetComponent<Text>().text = GameData.instance.enemy.hp.ToString();
        enemyEnergyBar.transform.GetChild(1).GetComponent<Text>().text = GameData.instance.enemy.energy.ToString();
        enemyBoostBar.transform.GetChild(1).GetComponent<Text>().text = GameData.instance.enemy.boost.ToString();
    }

}
