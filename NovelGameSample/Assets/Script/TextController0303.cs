using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; // UI要素を使うため
using System.IO;


public class TextController0303 : MonoBehaviour
{
    const float TEXT_SPEED = 0.1F;
    const float TEXT_SPEED_STRING = 0.05F;
    const float COMPLETE_LINE_DELAY = 0.3F;

    [SerializeField] Text lineText;     // 文字表示Text
    [SerializeField] Text nameText;     // 名前表示Text
    //[SerializeField] string[] scenarios;    // 会話内容

    float textSpeed = 0;                    // 表示速度
    float completeLineDelay = COMPLETE_LINE_DELAY;  // 表示し終えた後の待ち時間
    int currentLine = 0;                    // 表示している行数
    string currentText = string.Empty;      // 表示している文字
    string currentName = string.Empty;      // 表示している名前
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
        currentText = csvDatas[currentLine][1];
        currentName = csvDatas[currentLine][0];

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

        if (trigerEvent[number] == currentLine)
        {
            switch (sideActor[number])
            {
                case (false):
                    switch (activeActorL[number])
                    {
                        case (false):
                            if (Actor[number] == ActorList[0])
                            {
                                for (float x = -4.8f; x >= -10f; x -= 0.01f)
                                {
                                    Actor[number].transform.position = new Vector3(-20f, -1.4f, 0f);
                                }
                            }
                            else
                            {
                                Actor[number].transform.position = new Vector3(0f, -1.4f, 0f);
                            }

                            break;
                        case (true):
                            if (Actor[number] == ActorList[0])
                            {
                                //20180307Update関数は1フレームでの処理なので、1周の間にforをかましてもアニメーションにはならない
                                for (float x = -20f; x <= -4.8f; x += 0.01f)
                                {
                                    Actor[number].transform.position = new Vector3(x, -1.4f, 0f);
                                }
                            }
                            else
                            {
                                Actor[number].transform.position = new Vector3(0f, -1.4f, 0f);
                            }
                            break;
                        default:
                            break;
                    }
                    break;

                case (true):
                    switch (activeActorL[number])
                    {
                        case (false):
                            if (Actor[number] == ActorList[0])
                            {
                                for (float x = 4.8f; x >= 20f; x += 0.01f)
                                {
                                    Actor[number].transform.position = new Vector3(20f, 0f, 0f);
                                }
                            }
                            else
                            {
                                Actor[number].transform.position = new Vector3(0f, 0f, 0f);
                            }

                            break;
                        case (true):
                            if (Actor[number] == ActorList[0])
                            {
                                //20180307Update関数は1フレームでの処理なので、1周の間にforをかましてもアニメーションにはならない
                                for (float x = 20f; x <= 4.8f; x -= 0.01f)
                                {
                                    Actor[number].transform.position = new Vector3(x, 0f, 0f);
                                }
                            }
                            else
                            {
                                Actor[number].transform.position = new Vector3(0f, 0f, 0f);
                            }
                            break;
                        default:
                            break;
                    }
                    break;

                default:
                    break;
            }
            number++;
        }

        /*
        if (currentLine < 15)
        {
            simdata.DonnaSelect.transform.position = new Vector3(-10f, -1.42f, 3.0f);
            //simdata.DonnaSelect.transform.position = new Vector3(-225f, -62.85f, 0.0f);
            //simdata.DonnaSelect.transform.position = new Vector3(0.270f, 0.30f, 1.0f);
        }
        else
        {
            simdata.DonnaSelect.transform.position = new Vector3(-5f, -1.42f, 3.0f);
            //simdata.DonnaSelect.transform.position = new Vector3(-270f, -35f, 0.0f);
        }
        */

    }

    /// <summary>
    /// 表示文字更新
    /// </summary>
    /// <param name="lineCount">Line count.</param>
    void TextUpdate(int lineCount)
    {
        lineText.text = currentText.Substring(0, lineCount);
        nameText.text = currentName;
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

    void Update()
    {
       
    }

}