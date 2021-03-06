﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; // UI要素を使うため
using System.IO;


public class NameController : MonoBehaviour
{
    const float TEXT_SPEED = 0.1F;
    const float TEXT_SPEED_STRING = 0.05F;
    const float COMPLETE_LINE_DELAY = 0.3F;

    [SerializeField] Text lineText;     // 文字表示Text
    //[SerializeField] string[] scenarios;    // 会話内容

    float textSpeed = 0;                    // 表示速度
    float completeLineDelay = COMPLETE_LINE_DELAY;  // 表示し終えた後の待ち時間
    int currentLine = 0;                    // 表示している行数
    string currentText = string.Empty;      // 表示している文字
    bool isCompleteLine = false;      // １文が全部表示されたか？

    string CSVName;
    string CSVNumber;
    private TextAsset csvFile; // CSVファイル
    private List<string[]> csvDatas = new List<string[]>(); // CSVの中身を入れるリス
    string StringReader;
    int height = 0;


    //20180307追記
    public int numberEvent = 1;
    //public bool activeActor = false;
    [SerializeField] GameObject[] ActorList;
    [SerializeField] GameObject[] Actor;
    //falseがLeft（）左
    public bool[] sideActor;
    public bool[] activeActorL;
    public bool[] activeActorR;
    public int[] trigerEvent;
    private int number = 0;

    //イベント開始時のフェードイン（演出）時にテキストをクリックできなくする用
    public bool isEventActive = false;

    void Start()
    {
        Debug.Log("start");

        CSVName = "NovelSampleScenario";
        CSVNumber = "";
        // Resouces/CSV下のCSV読み込み 
        csvFile = Resources.Load("CSV/" + CSVName + CSVNumber) as TextAsset;
        StringReader reader = new StringReader(csvFile.text);

        while (reader.Peek() > -1)
        {
            string line = reader.ReadLine();
            // 配列に格納
            csvDatas.Add(line.Split(','));
            // 行数加算
            height++;
        }

        Show();
    }

    /// <summary>
    /// 会話シーン表示
    /// </summary>
    void Show()
    {
        Initialize();
        StartCoroutine(ScenarioCoroutine());
    }

    /// <summary>
    /// 初期化
    /// </summary>
    void Initialize()
    {
        isCompleteLine = false;

        //180303追記
        lineText.text = "";


        //currentText = senarios[currentline];
        currentText = csvDatas[currentLine][0];

        textSpeed = TEXT_SPEED + (currentText.Length * TEXT_SPEED_STRING);

        LineUpdate();
    }

    /// <summary>
    /// 会話シーン更新
    /// </summary>
    /// <returns>The coroutine.</returns>
    IEnumerator ScenarioCoroutine()
    {
        while (true)
        {
            yield return null;

            // 画面を押した時、次の内容へ
            if (isCompleteLine && Input.GetMouseButton(0))
            {
                yield return new WaitForSeconds(completeLineDelay);

                //if(currentLine > scenarios.Length - 1)
                if (currentLine > height - 1)
                {
                    ScenarioEnd();
                    yield break;
                }

                Initialize();
            }

            // 表示中にボタンが押されたら全部表示
            else if (!isCompleteLine && Input.GetMouseButton(0))
            {
                iTween.Stop();
                TextUpdate(currentText.Length); // 全部表示
                TextEnd();
                yield return new WaitForSeconds(completeLineDelay);
            }
        }
    }

    /// <summary>
    /// 文字を少しずつ表示
    /// </summary>
    void LineUpdate()
    {
        iTween.ValueTo(this.gameObject, iTween.Hash(
            "from", 0,
            "to", currentText.Length,
            "time", textSpeed,
            "onupdate", "TextUpdate",
            "oncompletetarget", this.gameObject,
            "oncomplete", "TextEnd"
        ));
    }

    /// <summary>
    /// 表示文字更新
    /// </summary>
    /// <param name="lineCount">Line count.</param>
    void TextUpdate(int lineCount)
    {
        lineText.text = currentText.Substring(0, lineCount);
    }

    /// <summary>
    /// 表示完了
    /// </summary>
    void TextEnd()
    {
        Debug.Log("表示完了");
        isCompleteLine = true;
        currentLine++;
    }

    /// <summary>
    /// 会話終了
    /// </summary>
    void ScenarioEnd()
    {
        Debug.Log("会話終了");
    }

}