using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameData : MonoBehaviour
{

    [Serializable]
    public class UserData
    {
        public int playerChr;
        public bool firstImplement = false; // 처음 실행인지
        public List<int> cardsList = new List<int>();

    }
    public class CharacterData
    {
        public int characterId;
        public int hp;
        public int energy;
        public int guard;
        public string name;

        public int coin;
        public int boost;
    }
    public class CardData
    {
        public int cardId;
        public Sprite img;
        public int kind; // move : 2, attack : 0, util : 1
        public int dm, en;
        public string name; // 유틸용 heal, guard
        public List<Vector2> pos = new List<Vector2>();
    }
    [HideInInspector] public UserData curUserData; // 현재 유저 데이터
    [HideInInspector] public UserData curEnemyData; // 현재 적 데이터
    [HideInInspector] public static GameData instance; // 싱글톤
    [HideInInspector] public List<Dictionary<string, object>> dic; // csv 데이터
    [HideInInspector] public List<CardData> cardDatas = new List<CardData>();

    public Sprite[] cardImgs = new Sprite[10];
    public CharacterData user = new CharacterData();
    public CharacterData enemy = new CharacterData();

    public Sprite[] chrImgs = new Sprite[9];
    public List<int>[] chrCardList = new List<int>[9];
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        LoadUserData();
        dic = CSVReader.Read("datas");

        // 이동 ->
        CardData card = new CardData();
        card.cardId = 0;
        card.img = cardImgs[0];
        card.kind = 2;
        card.en = 10;
        card.pos.Add(new Vector2(1, 0));
        cardDatas.Add(card);
        // 플레1 공격1
        card = new CardData();
        card.cardId = 1;
        card.img = cardImgs[1];
        card.kind = 0;
        card.dm = 30;
        card.en = 25;
        card.pos.Add(new Vector2(0, 1));
        card.pos.Add(new Vector2(0, 0));
        card.pos.Add(new Vector2(0, -1));
        cardDatas.Add(card);
        // 이동 <-
        card = new CardData();
        card.cardId = 2;
        card.img = cardImgs[2];
        card.kind = 2;
        card.dm = 20;
        card.en = 0;
        card.pos.Add(new Vector2(-1, 0));
        cardDatas.Add(card);
        // 회복
        card = new CardData();
        card.cardId = 3;
        card.img = cardImgs[3];
        card.kind = 1;
        card.dm = 20;
        card.en =0;
        card.name = "healHp";
        card.pos.Add(new Vector2(1, 0));
        cardDatas.Add(card);
        // 가드
        card = new CardData();
        card.cardId = 4;
        card.img = cardImgs[4];
        card.kind = 1;
        card.dm = 15;
        card.name = "guard";
        card.en = 0;
        card.pos.Add(new Vector2(1, 0));
        cardDatas.Add(card);
        // 회복
        card = new CardData();
        card.cardId = 5;
        card.img = cardImgs[5];
        card.kind = 1;
        card.dm = 15;
        card.en = 0;
        card.name = "healEnergy";
        card.pos.Add(new Vector2(1, 0));
        cardDatas.Add(card);
        // 플레1 공격2
        card = new CardData();
        card.cardId = 6;
        card.img = cardImgs[6];
        card.kind = 0;
        card.dm =50;
        card.en = 50;
        card.pos.Add(new Vector2(0, 0));
        card.pos.Add(new Vector2(1, -1));
        card.pos.Add(new Vector2(-1, -1));
        card.pos.Add(new Vector2(0, -1));
        cardDatas.Add(card);
        // 이동 아래
        card = new CardData();
        card.cardId = 7;
        card.img = cardImgs[7];
        card.kind = 2;
        card.dm = 20;
        card.en = 10;
        card.pos.Add(new Vector2(0, -1));
        cardDatas.Add(card);
        // 이동 위
        card = new CardData();
        card.cardId = 8;
        card.img = cardImgs[8];
        card.kind = 2;
        card.dm = 20;
        card.en = 10;
        card.pos.Add(new Vector2(0, 1));
        cardDatas.Add(card);
        // 플레1 공격3
        card = new CardData();
        card.cardId = 9;
        card.img = cardImgs[9];
        card.kind = 0;
        card.dm = 25;
        card.en = 25;
        card.pos.Add(new Vector2(0, 1));
        card.pos.Add(new Vector2(-1, 0));
        card.pos.Add(new Vector2(0, 0));
        card.pos.Add(new Vector2(1, 0));
        card.pos.Add(new Vector2(0, -1));
        cardDatas.Add(card);
        // 플레1 공격4
        card = new CardData();
        card.cardId = 10;
        card.img = cardImgs[10];
        card.kind = 0;
        card.dm = 25;
        card.en = 35;
        card.pos.Add(new Vector2(-1, 1));
        card.pos.Add(new Vector2(1, 1));
        card.pos.Add(new Vector2(0, 0));
        card.pos.Add(new Vector2(-1, -1));
        card.pos.Add(new Vector2(1, -1));
        cardDatas.Add(card);
        // 플레2 공격1
        card = new CardData();
        card.cardId = 11;
        card.img = cardImgs[11];
        card.kind = 0;
        card.dm = 50;
        card.en = 50;
        card.pos.Add(new Vector2(-1, 1));
        card.pos.Add(new Vector2(1, 1));
        card.pos.Add(new Vector2(0, 0));
        cardDatas.Add(card);
        // 플레2 공격2
        card = new CardData();
        card.cardId = 12;
        card.img = cardImgs[12];
        card.kind = 0;
        card.dm = 15;
        card.en = 15;
        card.pos.Add(new Vector2(-1, 1));
        card.pos.Add(new Vector2(0, 1));
        card.pos.Add(new Vector2(1, 1));
        card.pos.Add(new Vector2(-1, 0));
        card.pos.Add(new Vector2(0, 0));
        card.pos.Add(new Vector2(1, 0));
        card.pos.Add(new Vector2(-1, -1));
        card.pos.Add(new Vector2(0, -1));
        card.pos.Add(new Vector2(1, -1));
        cardDatas.Add(card);
        // 플레2 공격3
        card = new CardData();
        card.cardId = 13;
        card.img = cardImgs[13];
        card.kind = 0;
        card.dm = 25;
        card.en = 15;
        card.pos.Add(new Vector2(-1, 0));
        card.pos.Add(new Vector2(0, 0));
        card.pos.Add(new Vector2(1, 0));
        cardDatas.Add(card);
        // 플레2 공격4
        card = new CardData();
        card.cardId = 14;
        card.img = cardImgs[14];
        card.kind = 0;
        card.dm = 25;
        card.en = 25;
        card.pos.Add(new Vector2(0,1));
        card.pos.Add(new Vector2(-1, 0));
        card.pos.Add(new Vector2(0, 0));
        card.pos.Add(new Vector2(1, 0));
        card.pos.Add(new Vector2(0, -1));
        cardDatas.Add(card);
        // 플레3 공격1
        card = new CardData();
        card.cardId = 15;
        card.img = cardImgs[15];
        card.kind = 0;
        card.dm = 40;
        card.en = 45;
        card.pos.Add(new Vector2(-1, 0));
        card.pos.Add(new Vector2(0, 0));
        card.pos.Add(new Vector2(1, 0));
        cardDatas.Add(card);
        // 플레3 공격2
        card = new CardData();
        card.cardId = 16;
        card.img = cardImgs[16];
        card.kind = 0;
        card.dm = 25;
        card.en = 30;
        card.pos.Add(new Vector2(-1, 1));
        card.pos.Add(new Vector2(0, 1));
        card.pos.Add(new Vector2(1, 1));
        card.pos.Add(new Vector2(0, 0));
        card.pos.Add(new Vector2(-1, -1));
        card.pos.Add(new Vector2(0, -1));
        card.pos.Add(new Vector2(1, -1));
        cardDatas.Add(card);
        // 플레3 공격3
        card = new CardData();
        card.cardId = 17;
        card.img = cardImgs[17];
        card.kind = 0;
        card.dm = 15;
        card.en = 25;
        card.pos.Add(new Vector2(-1, 1));
        card.pos.Add(new Vector2(1, 1));
        card.pos.Add(new Vector2(-1, 0));
        card.pos.Add(new Vector2(1, 0));
        card.pos.Add(new Vector2(-1, -1));
        card.pos.Add(new Vector2(1, -1));
        cardDatas.Add(card);
        // 플레3 공격4
        card = new CardData();
        card.cardId = 18;
        card.img = cardImgs[18];
        card.kind = 0;
        card.dm = 30;
        card.en = 15;
        card.pos.Add(new Vector2(0, 1));
        card.pos.Add(new Vector2(0, 0));
        card.pos.Add(new Vector2(0, -1));
        cardDatas.Add(card);
        // 플레4 공격1
        card = new CardData();
        card.cardId = 19;
        card.img = cardImgs[19];
        card.kind = 0;
        card.dm = 50;
        card.en = 50;
        card.pos.Add(new Vector2(-1, 1));
        card.pos.Add(new Vector2(0, 1));
        card.pos.Add(new Vector2(1, 1));
        card.pos.Add(new Vector2(0, 0));
        cardDatas.Add(card);
        // 플레4 공격2
        card = new CardData();
        card.cardId = 20;
        card.img = cardImgs[20];
        card.kind = 0;
        card.dm = 15;
        card.en = 15;
        card.pos.Add(new Vector2(-1, 1));
        card.pos.Add(new Vector2(0, 1));
        card.pos.Add(new Vector2(1, 1));
        card.pos.Add(new Vector2(-1, 0));
        card.pos.Add(new Vector2(0, 0));
        card.pos.Add(new Vector2(1, 0));
        card.pos.Add(new Vector2(-1, -1));
        card.pos.Add(new Vector2(0, -1));
        card.pos.Add(new Vector2(1, -1));
        cardDatas.Add(card);
        // 플레4 공격3
        card = new CardData();
        card.cardId = 21;
        card.img = cardImgs[21];
        card.kind = 0;
        card.dm = 25;
        card.en = 35;
        card.pos.Add(new Vector2(-1, 1));
        card.pos.Add(new Vector2(1, 1));
        card.pos.Add(new Vector2(0, 0));
        card.pos.Add(new Vector2(-1, -1));
        card.pos.Add(new Vector2(1, -1));
        cardDatas.Add(card);
        // 플레4 공격4
        card = new CardData();
        card.cardId = 22;
        card.img = cardImgs[22];
        card.kind = 0;
        card.dm = 25;
        card.en = 20;
        card.pos.Add(new Vector2(-1, 0));
        card.pos.Add(new Vector2(0, 0));
        card.pos.Add(new Vector2(1, 0));
        cardDatas.Add(card);
        // 플레5 공격1
        card = new CardData();
        card.cardId = 23;
        card.img = cardImgs[23];
        card.kind = 0;
        card.dm = 40;
        card.en = 50;
        card.pos.Add(new Vector2(-1, 1));
        card.pos.Add(new Vector2(1, 1));
        card.pos.Add(new Vector2(-1, 0));
        card.pos.Add(new Vector2(0, 0));
        card.pos.Add(new Vector2(1, 0));
        card.pos.Add(new Vector2(-1, -1));
        card.pos.Add(new Vector2(1, -1));
        cardDatas.Add(card);
        // 플레5 공격2
        card = new CardData();
        card.cardId = 24;
        card.img = cardImgs[24];
        card.kind = 0;
        card.dm = 25;
        card.en = 30;
        card.pos.Add(new Vector2(0, 1));
        card.pos.Add(new Vector2(-1, 0));
        card.pos.Add(new Vector2(0, 0));
        card.pos.Add(new Vector2(1, 0));
        card.pos.Add(new Vector2(0, -1));
        cardDatas.Add(card);
        // 플레5 공격3
        card = new CardData();
        card.cardId = 25;
        card.img = cardImgs[25];
        card.kind = 0;
        card.dm = 25;
        card.en = 15;
        card.pos.Add(new Vector2(-1, 0));
        card.pos.Add(new Vector2(0, 0));
        card.pos.Add(new Vector2(1, 0));
        cardDatas.Add(card);
        // 플레5 공격4
        card = new CardData();
        card.cardId = 26;
        card.img = cardImgs[26];
        card.kind = 0;
        card.dm = 25;
        card.en = 30;
        card.pos.Add(new Vector2(-1, 1));
        card.pos.Add(new Vector2(1, 1));
        card.pos.Add(new Vector2(0, 0));
        card.pos.Add(new Vector2(-1, -1));
        card.pos.Add(new Vector2(1, -1));
        cardDatas.Add(card);
        // 플레6 공격1
        card = new CardData();
        card.cardId = 27;
        card.img = cardImgs[27];
        card.kind = 0;
        card.dm = 35;
        card.en = 25;
        card.pos.Add(new Vector2(-1, 0));
        card.pos.Add(new Vector2(0, 0));
        card.pos.Add(new Vector2(1, 0));
        cardDatas.Add(card);
        // 플레6 공격2
        card = new CardData();
        card.cardId = 28;
        card.img = cardImgs[28];
        card.kind = 0;
        card.dm = 50;
        card.en = 45;
        card.pos.Add(new Vector2(0, 0));
        card.pos.Add(new Vector2(-1, -1));
        card.pos.Add(new Vector2(0, -1));
        card.pos.Add(new Vector2(1, -1));
        cardDatas.Add(card);
        // 플레6 공격3
        card = new CardData();
        card.cardId = 29;
        card.img = cardImgs[29];
        card.kind = 0;
        card.dm = 25;
        card.en = 15;
        card.pos.Add(new Vector2(0, 1));
        card.pos.Add(new Vector2(-1, 0));
        card.pos.Add(new Vector2(0, 0));
        card.pos.Add(new Vector2(1, 0));
        cardDatas.Add(card);
        // 플레6 공격4
        card = new CardData();
        card.cardId = 30;
        card.img = cardImgs[30];
        card.kind = 0;
        card.dm = 20;
        card.en = 35;
        card.pos.Add(new Vector2(-1, 1));
        card.pos.Add(new Vector2(1, 1));
        card.pos.Add(new Vector2(-1, 0));
        card.pos.Add(new Vector2(0, 0));
        card.pos.Add(new Vector2(1, 0));
        card.pos.Add(new Vector2(-1, -1));
        card.pos.Add(new Vector2(1, -1));
        cardDatas.Add(card);
        // 플레7 공격1
        card = new CardData();
        card.cardId = 31;
        card.img = cardImgs[31];
        card.kind = 0;
        card.dm = 40;
        card.en = 50;
        card.pos.Add(new Vector2(-1, 1));
        card.pos.Add(new Vector2(1, 1));
        card.pos.Add(new Vector2(0, 1));
        card.pos.Add(new Vector2(0, 0));
        card.pos.Add(new Vector2(0, -1));
        card.pos.Add(new Vector2(-1, -1));
        card.pos.Add(new Vector2(1, -1));
        cardDatas.Add(card);
        // 플레7 공격2
        card = new CardData();
        card.cardId = 32;
        card.img = cardImgs[32];
        card.kind = 0;
        card.dm = 25;
        card.en = 25;
        card.pos.Add(new Vector2(-1, 1));
        card.pos.Add(new Vector2(1, 1));
        card.pos.Add(new Vector2(0, 0));
        card.pos.Add(new Vector2(-1, -1));
        card.pos.Add(new Vector2(1, -1));
        cardDatas.Add(card);
        // 플레7 공격3
        card = new CardData();
        card.cardId = 33;
        card.img = cardImgs[33];
        card.kind = 0;
        card.dm = 30;
        card.en = 20;
        card.pos.Add(new Vector2(-1, 0));
        card.pos.Add(new Vector2(0, 0));
        card.pos.Add(new Vector2(1, 0));
        cardDatas.Add(card);
        // 플레7 공격4
        card = new CardData();
        card.cardId = 34;
        card.img = cardImgs[34];
        card.kind = 0;
        card.dm = 30;
        card.en = 20;
        card.pos.Add(new Vector2(-1, 0));
        card.pos.Add(new Vector2(0, 0));
        card.pos.Add(new Vector2(1, 0));
        cardDatas.Add(card);
        // 플레8 공격1
        card = new CardData();
        card.cardId = 35;
        card.img = cardImgs[35];
        card.kind = 0;
        card.dm = 60;
        card.en = 70;
        card.pos.Add(new Vector2(-1, 0));
        card.pos.Add(new Vector2(1, 0));
        cardDatas.Add(card);
        // 플레8 공격2
        card = new CardData();
        card.cardId = 36;
        card.img = cardImgs[36];
        card.kind = 0;
        card.dm = 20;
        card.en = 15;
        card.pos.Add(new Vector2(-1, 0));
        card.pos.Add(new Vector2(0, 0));
        card.pos.Add(new Vector2(1, 0));
        card.pos.Add(new Vector2(0, -1));
        card.pos.Add(new Vector2(-1, -1));
        card.pos.Add(new Vector2(1, -1));
        cardDatas.Add(card);
        // 플레8 공격3
        card = new CardData();
        card.cardId = 37;
        card.img = cardImgs[37];
        card.kind = 0;
        card.dm = 40;
        card.en = 50;
        card.pos.Add(new Vector2(0, 1));
        card.pos.Add(new Vector2(-1, 0));
        card.pos.Add(new Vector2(0, 0));
        card.pos.Add(new Vector2(1, 0));
        card.pos.Add(new Vector2(0, -1));
        cardDatas.Add(card);
        // 플레8 공격4
        card = new CardData();
        card.cardId = 38;
        card.img = cardImgs[38];
        card.kind = 0;
        card.dm = 25;
        card.en = 20;
        card.pos.Add(new Vector2(-1, 1));
        card.pos.Add(new Vector2(1, 1));
        card.pos.Add(new Vector2(0, 0));
        card.pos.Add(new Vector2(-1, -1));
        card.pos.Add(new Vector2(1, -1));
        cardDatas.Add(card);
        for (int i = 0; i < 11; i++)
            curUserData.cardsList.Add(i % 6);

        List<int> tmp = new List<int>();
        tmp.Add(0); tmp.Add(2); tmp.Add(4); tmp.Add(5); tmp.Add(7); tmp.Add(8);
        tmp.Add(1); tmp.Add(6); tmp.Add(9); tmp.Add(10);
        chrCardList[1] = tmp;
        tmp = new List<int>();
        tmp.Add(0); tmp.Add(2); tmp.Add(4); tmp.Add(5); tmp.Add(7); tmp.Add(8);
        tmp.Add(11); tmp.Add(12); tmp.Add(13); tmp.Add(14);
        chrCardList[2] = tmp;
        tmp = new List<int>();
        tmp.Add(0); tmp.Add(2); tmp.Add(4); tmp.Add(5); tmp.Add(7); tmp.Add(8);
        tmp.Add(15); tmp.Add(16); tmp.Add(17); tmp.Add(18);
        chrCardList[3] = tmp;
        tmp = new List<int>();
        tmp.Add(0); tmp.Add(2); tmp.Add(4); tmp.Add(5); tmp.Add(7); tmp.Add(8);
        tmp.Add(19); tmp.Add(20); tmp.Add(21); tmp.Add(22);
        chrCardList[4] = tmp;
        tmp = new List<int>();
        tmp.Add(0); tmp.Add(2); tmp.Add(4); tmp.Add(5); tmp.Add(7); tmp.Add(8);
        tmp.Add(23); tmp.Add(24); tmp.Add(25); tmp.Add(26);
        chrCardList[5] = tmp;
        tmp = new List<int>();
        tmp.Add(0); tmp.Add(2); tmp.Add(4); tmp.Add(5); tmp.Add(7); tmp.Add(8);
        tmp.Add(27); tmp.Add(28); tmp.Add(29); tmp.Add(30);
        chrCardList[6] = tmp;
        tmp = new List<int>();
        tmp.Add(0); tmp.Add(2); tmp.Add(4); tmp.Add(5); tmp.Add(7); tmp.Add(8);
        tmp.Add(31); tmp.Add(32); tmp.Add(33); tmp.Add(34);
        chrCardList[7] = tmp;
        tmp = new List<int>();
        tmp.Add(0); tmp.Add(2); tmp.Add(4); tmp.Add(5); tmp.Add(7); tmp.Add(8);
        tmp.Add(35); tmp.Add(36); tmp.Add(37); tmp.Add(38);
        chrCardList[8] = tmp;
    }
    void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        if (GameData.instance == null)
        {
            GameData.instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
        DontDestroyOnLoad(this);

    }
    // Update is called once per frame
    void Update()
    {

    }
    public void SaveUserData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/UserData.dat");

        UserData data = curUserData;

        bf.Serialize(file, data);
        file.Close();
    }
    public void LoadUserData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        try
        {
            FileStream file = File.Open(Application.persistentDataPath + "/UserData.dat", FileMode.Open);
            if (file != null && file.Length > 0)
            {
                UserData data = (UserData)bf.Deserialize(file);
                curUserData = data;
            }
            file.Close();
        }
        catch (Exception e)
        {
            curUserData = new UserData();
            SaveUserData();
        }
    }
    public CardData GetCardData(int id)
    {
        for (int i = 0; i < cardDatas.Count; i++)
        {
            if (cardDatas[i].cardId == id)
                return cardDatas[i];
        }
        return null;
    }
}