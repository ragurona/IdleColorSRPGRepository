﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenCvSharp;

using System.IO;

public enum Trigger {User, Update};

static class Constants
{
    public const int CHARACTERS_ALL_NUM = 10;
    public const int CHARACTERS_HELP_PRODUCTION_NUM = 3;
    public const int CHARACTERS_PRODUCTION_PIXEL_NUM = 5;
}

//生産画面のコントロール
public class ControllerProduction : MonoBehaviour
{
    ModelProduction ModelProduction;

    CharacterClass[] CharactersAll = new CharacterClass[Constants.CHARACTERS_ALL_NUM + 1];

    uint[] CharactersIDHelpProductionR = new uint[Constants.CHARACTERS_HELP_PRODUCTION_NUM + 1];
    uint[] CharactersIDHelpProductionG = new uint[Constants.CHARACTERS_HELP_PRODUCTION_NUM + 1];
    uint[] CharactersIDHelpProductionB = new uint[Constants.CHARACTERS_HELP_PRODUCTION_NUM + 1];

    uint[] CharactersIDProductionPixel = new uint[Constants.CHARACTERS_PRODUCTION_PIXEL_NUM + 1];
    Color[] ColorProductionPixel = new Color[Constants.CHARACTERS_PRODUCTION_PIXEL_NUM + 1];
    ushort[,] ProgressProductionPixel = new ushort[Constants.CHARACTERS_PRODUCTION_PIXEL_NUM + 1, 3 + 1];
    bool[,] WarningLackRGB = new bool[Constants.CHARACTERS_PRODUCTION_PIXEL_NUM + 1, 3 + 1];

    //現在の全カラーの個数(CurColors[0,0,0]が黒、CurColors[255,0,0]が赤)
    ulong[,,] CurPixels = new ulong[256, 256, 256];

    //現在のRGB値
    ulong CurR = 0;
    ulong CurG = 0;
    ulong CurB = 0;

    [SerializeField] Text TextR;
    [SerializeField] Text TextG;
    [SerializeField] Text TextB;
    [SerializeField] Slider SliderR;
    [SerializeField] Slider SliderG;
    [SerializeField] Slider SliderB;

    //最大値
    ulong MaxR = 1024;
    ulong MaxG = 1024;
    ulong MaxB = 1024;

    //最大値を上げるコスト
    ulong CostMaxRUp = 512;
    ulong CostMaxGUp = 512;
    ulong CostMaxBUp = 512;

    [SerializeField] Text TextCostMaxRUp;
    [SerializeField] Text TextCostMaxGUp;
    [SerializeField] Text TextCostMaxBUp;
    [SerializeField] Slider SliderCostMaxRUp;
    [SerializeField] Slider SliderCostMaxGUp;
    [SerializeField] Slider SliderCostMaxBUp;

    [SerializeField] Button ButtonMaxRUp;
    [SerializeField] Button ButtonMaxGUp;
    [SerializeField] Button ButtonMaxBUp;


    //増加値
    ulong IncreaseValueR = 1;
    ulong IncreaseValueG = 1;
    ulong IncreaseValueB = 1;

    [SerializeField] Text TextIncreaseValueRLeft;
    [SerializeField] Text TextIncreaseValueRRight;
    [SerializeField] Text TextIncreaseValueGLeft;
    [SerializeField] Text TextIncreaseValueGRight;
    [SerializeField] Text TextIncreaseValueBLeft;
    [SerializeField] Text TextIncreaseValueBRight;

    //増加値を上げるコスト
    ulong CostIncreaseValueRUp = 16;
    ulong CostIncreaseValueGUp = 16;
    ulong CostIncreaseValueBUp = 16;

    [SerializeField] Text TextCostIncreaseValueRUp;
    [SerializeField] Text TextCostIncreaseValueGUp;
    [SerializeField] Text TextCostIncreaseValueBUp;
    [SerializeField] Slider SliderCostIncreaseValueRUp;
    [SerializeField] Slider SliderCostIncreaseValueGUp;
    [SerializeField] Slider SliderCostIncreaseValueBUp;

    [SerializeField] Button ButtonIncreaseValueRUp;
    [SerializeField] Button ButtonIncreaseValueGUp;
    [SerializeField] Button ButtonIncreaseValueBUp;


    //ピクセル生産のクリック時の進捗数
    ushort UserProductionPixelNum = 10;

    //シーンパネル
    [SerializeField] GameObject PanelRGBProduction;
    [SerializeField] GameObject PanelPixelProduction;

    //キャラクターセレクトパネル
    [SerializeField] GameObject PanelSelectCharacter;

    [SerializeField] Image ImageSelectCharacter;
    [SerializeField] Text TextSelectCharacter1;
    [SerializeField] Text TextSelectCharacter2;
    [SerializeField] Text TextSelectCharacter3;

    //カラーセレクトパネル
    [SerializeField] GameObject PanelSelectColor;
    [SerializeField] GameObject PanelSelectColorMethod;
    [SerializeField] GameObject PanelSelectColorMethodRGBNum;

    [SerializeField] InputField InputFieldSpecificationNumR;
    [SerializeField] InputField InputFieldSpecificationNumG;
    [SerializeField] InputField InputFieldSpecificationNumB;

    [SerializeField] Image ImageSpecificationColorR;
    [SerializeField] Image ImageSpecificationColorG;
    [SerializeField] Image ImageSpecificationColorB;





    Button ButtonTmp = null;
    uint CharacterIDTmp = 0;
    Color ColorTmp = new Color(0.0f, 0.0f, 0.0f, 1.0f);



    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("ControllerProduction Start");

        //画面回転固定
        //縦
        Screen.autorotateToPortrait = true;
        //上下反転
        Screen.autorotateToPortraitUpsideDown = true;
        //左
        Screen.autorotateToLandscapeLeft = false;
        //右
        Screen.autorotateToLandscapeRight = false;


        //キャラ生成
        Debug.Log("キャラ生成");
        ModelProduction = GetComponent<ModelProduction>();
        for (int i = 0; i < Constants.CHARACTERS_ALL_NUM + 1; i++)
        {
            CharactersAll[i] = new CharacterClass();
        }
        //IDと配列番号を一致させる、0は初期値のままで
        CharactersAll[1].MakeCharacter(Resources.Load("Character/RedSlime8", typeof(Texture2D)) as Texture2D, 1, "LittleRedSlime");
        CharactersAll[2].MakeCharacter(Resources.Load("Character/GreenSlime8", typeof(Texture2D)) as Texture2D, 2, "LittleGreenSlime");
        CharactersAll[3].MakeCharacter(Resources.Load("Character/BlueSlime8", typeof(Texture2D)) as Texture2D, 3, "LittleBlueSlime");
        CharactersAll[4].MakeCharacter(Resources.Load("Character/WhiteSlime8", typeof(Texture2D)) as Texture2D, 4, "LittleWhiteSlime");
        CharactersAll[5].MakeCharacter(Resources.Load("Character/RBlackCat8", typeof(Texture2D)) as Texture2D, 5, "LittleRBlackCat");
        CharactersAll[8].MakeCharacter(Resources.Load("Character/WhiteCat8", typeof(Texture2D)) as Texture2D, 8, "LittleWhiteCat");

        //ColorProductionPixelの初期化
        for (int i = 0; i < Constants.CHARACTERS_PRODUCTION_PIXEL_NUM + 1; i++)
        {
            ColorProductionPixel[i].a = 1.0f;
        }

        //ロード
        SaveClass SC = new SaveClass();
        SC.Load(ref CharactersAll, Constants.CHARACTERS_ALL_NUM + 1, 
            ref CurR, ref CurG, ref CurB,
            ref MaxR, ref MaxG, ref MaxB,
            ref CostMaxRUp, ref CostMaxGUp, ref CostMaxBUp,
            ref IncreaseValueR, ref IncreaseValueG, ref IncreaseValueB,
            ref CostIncreaseValueRUp, ref CostIncreaseValueGUp, ref CostIncreaseValueBUp,
            ref CharactersIDHelpProductionR, ref CharactersIDHelpProductionG, ref CharactersIDHelpProductionB);

        //UIの更新
        UpdateRGBProductionScene();
    }

    // Update is called once per frame
    private float TimeOut = 1;
    private float TimeElapsed = 0;
    void Update()
    {
        //Debug.Log("ControllerProduction Update");

        TimeElapsed += Time.deltaTime;

        if (TimeElapsed >= TimeOut)
        {
            TimeElapsed = 0;

            ulong IncreaseValueRHelp = CharactersAll[CharactersIDHelpProductionR[1]].Stats[0].RCreates +
                                       CharactersAll[CharactersIDHelpProductionR[2]].Stats[0].RCreates +
                                       CharactersAll[CharactersIDHelpProductionR[3]].Stats[0].RCreates;
            ModelProduction.Increase(ref CurR, IncreaseValueRHelp, MaxR);
            UpdateRGBProductionOneColor(CurR, MaxR, TextR, SliderR, IncreaseValueR, CostIncreaseValueRUp, TextIncreaseValueRLeft, TextIncreaseValueRRight, TextCostIncreaseValueRUp, SliderCostIncreaseValueRUp, ButtonIncreaseValueRUp, CostMaxRUp, TextCostMaxRUp, SliderCostMaxRUp, ButtonMaxRUp);

            ulong IncreaseValueGHelp = CharactersAll[CharactersIDHelpProductionG[1]].Stats[0].GCreates +
                                       CharactersAll[CharactersIDHelpProductionG[2]].Stats[0].GCreates +
                                       CharactersAll[CharactersIDHelpProductionG[3]].Stats[0].GCreates;
            ModelProduction.Increase(ref CurG, IncreaseValueGHelp, MaxG);
            UpdateRGBProductionOneColor(CurG, MaxG, TextG, SliderG, IncreaseValueG, CostIncreaseValueGUp, TextIncreaseValueGLeft, TextIncreaseValueGRight, TextCostIncreaseValueGUp, SliderCostIncreaseValueGUp, ButtonIncreaseValueGUp, CostMaxGUp, TextCostMaxGUp, SliderCostMaxGUp, ButtonMaxGUp);

            ulong IncreaseValueBHelp = CharactersAll[CharactersIDHelpProductionB[1]].Stats[0].BCreates +
                                       CharactersAll[CharactersIDHelpProductionB[2]].Stats[0].BCreates +
                                       CharactersAll[CharactersIDHelpProductionB[3]].Stats[0].BCreates;
            ModelProduction.Increase(ref CurB, IncreaseValueBHelp, MaxB);
            UpdateRGBProductionOneColor(CurB, MaxB, TextB, SliderB, IncreaseValueB, CostIncreaseValueBUp, TextIncreaseValueBLeft, TextIncreaseValueBRight, TextCostIncreaseValueBUp, SliderCostIncreaseValueBUp, ButtonIncreaseValueBUp, CostMaxBUp, TextCostMaxBUp, SliderCostMaxBUp, ButtonMaxBUp);


            ProductionPixel(Trigger.Update);
            UpdatePixelProductionScene();
        }

    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //セーブ機能

    //セーブボタンが押されたら
    public void PushButtonSave()
    {
        SaveClass SC = new SaveClass();
        SC.Save(CharactersAll, Constants.CHARACTERS_ALL_NUM + 1,
            CurR, CurG, CurB,
            MaxR, MaxG, MaxB,
            CostMaxRUp, CostMaxGUp, CostMaxBUp,
            IncreaseValueR, IncreaseValueG, IncreaseValueB,
            CostIncreaseValueRUp, CostIncreaseValueGUp, CostIncreaseValueBUp,
            CharactersIDHelpProductionR, CharactersIDHelpProductionG, CharactersIDHelpProductionB);
    }

    //デリートセーブボタンが押されたら
    public void PushButtonDeleteSave()
    {
        File.Delete(Application.persistentDataPath + "/ICS.csv");
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //RGB生産

    // ButtonRが押されたら
    public void PushButtonR()
    {
        Debug.Log("ButtonRが押された");
        ModelProduction.Increase(ref CurR, IncreaseValueR, MaxR);
        UpdateRGBProductionOneColor(CurR, MaxR, TextR, SliderR, IncreaseValueR, CostIncreaseValueRUp, TextIncreaseValueRLeft, TextIncreaseValueRRight, TextCostIncreaseValueRUp, SliderCostIncreaseValueRUp, ButtonIncreaseValueRUp, CostMaxRUp, TextCostMaxRUp, SliderCostMaxRUp, ButtonMaxRUp);
    }
    // ButtonGが押されたら
    public void PushButtonG()
    {
        Debug.Log("ButtonGが押された");
        ModelProduction.Increase(ref CurG, IncreaseValueG, MaxG);
        UpdateRGBProductionOneColor(CurG, MaxG, TextG, SliderG, IncreaseValueG, CostIncreaseValueGUp, TextIncreaseValueGLeft, TextIncreaseValueGRight, TextCostIncreaseValueGUp, SliderCostIncreaseValueGUp, ButtonIncreaseValueGUp, CostMaxGUp, TextCostMaxGUp, SliderCostMaxGUp, ButtonMaxGUp);
    }
    // ButtonBが押されたら
    public void PushButtonB()
    {
        Debug.Log("ButtonBが押された");
        ModelProduction.Increase(ref CurB, IncreaseValueB, MaxB);
        UpdateRGBProductionOneColor(CurB, MaxB, TextB, SliderB, IncreaseValueB, CostIncreaseValueBUp, TextIncreaseValueBLeft, TextIncreaseValueBRight, TextCostIncreaseValueBUp, SliderCostIncreaseValueBUp, ButtonIncreaseValueBUp, CostMaxBUp, TextCostMaxBUp, SliderCostMaxBUp, ButtonMaxBUp);
    }

    //ButtonIncreaseValueRUpが押されたら
    public void PushButtonIncreaseValueRUp()
    {
        Debug.Log("ButtonIncreaseValueRUpが押された");
        ModelProduction.UpgradeValue(ref CurR, ref CostIncreaseValueRUp, ref IncreaseValueR, ModelProduction.TypeUpgrade.INCREASE);
        UpdateRGBProductionOneColor(CurR, MaxR, TextR, SliderR, IncreaseValueR, CostIncreaseValueRUp, TextIncreaseValueRLeft, TextIncreaseValueRRight, TextCostIncreaseValueRUp, SliderCostIncreaseValueRUp, ButtonIncreaseValueRUp, CostMaxRUp, TextCostMaxRUp, SliderCostMaxRUp, ButtonMaxRUp);
    }
    //ButtonIncreaseValueGUpが押されたら
    public void PushButtonIncreaseValueGUp()
    {
        Debug.Log("ButtonIncreaseValueGUpが押された");
        ModelProduction.UpgradeValue(ref CurG, ref CostIncreaseValueGUp, ref IncreaseValueG, ModelProduction.TypeUpgrade.INCREASE);
        UpdateRGBProductionOneColor(CurG, MaxG, TextG, SliderG, IncreaseValueG, CostIncreaseValueGUp, TextIncreaseValueGLeft, TextIncreaseValueGRight, TextCostIncreaseValueGUp, SliderCostIncreaseValueGUp, ButtonIncreaseValueGUp, CostMaxGUp, TextCostMaxGUp, SliderCostMaxGUp, ButtonMaxGUp);
    }
    //ButtonIncreaseValueBUpが押されたら
    public void PushButtonIncreaseValueBUp()
    {
        Debug.Log("ButtonIncreaseValueBUpが押された");
        ModelProduction.UpgradeValue(ref CurB, ref CostIncreaseValueBUp, ref IncreaseValueB, ModelProduction.TypeUpgrade.INCREASE);
        UpdateRGBProductionOneColor(CurB, MaxB, TextB, SliderB, IncreaseValueB, CostIncreaseValueBUp, TextIncreaseValueBLeft, TextIncreaseValueBRight, TextCostIncreaseValueBUp, SliderCostIncreaseValueBUp, ButtonIncreaseValueBUp, CostMaxBUp, TextCostMaxBUp, SliderCostMaxBUp, ButtonMaxBUp);
    }

    //ButtonMaxRUpが押されたら
    public void PushButtonMaxRUp()
    {
        Debug.Log("ButtonMaxRUpが押された");
        ModelProduction.UpgradeValue(ref CurR, ref CostMaxRUp, ref MaxR, ModelProduction.TypeUpgrade.MAX);
        UpdateRGBProductionOneColor(CurR, MaxR, TextR, SliderR, IncreaseValueR, CostIncreaseValueRUp, TextIncreaseValueRLeft, TextIncreaseValueRRight, TextCostIncreaseValueRUp, SliderCostIncreaseValueRUp, ButtonIncreaseValueRUp, CostMaxRUp, TextCostMaxRUp, SliderCostMaxRUp, ButtonMaxRUp);
    }
    //ButtonMaxGUpが押されたら
    public void PushButtonMaxGUp()
    {
        Debug.Log("ButtonMaxRUpが押された");
        ModelProduction.UpgradeValue(ref CurG, ref CostMaxGUp, ref MaxG, ModelProduction.TypeUpgrade.MAX);
        UpdateRGBProductionOneColor(CurG, MaxG, TextG, SliderG, IncreaseValueG, CostIncreaseValueGUp, TextIncreaseValueGLeft, TextIncreaseValueGRight, TextCostIncreaseValueGUp, SliderCostIncreaseValueGUp, ButtonIncreaseValueGUp, CostMaxGUp, TextCostMaxGUp, SliderCostMaxGUp, ButtonMaxGUp);
    }
    //ButtonMaxBUpが押されたら
    public void PushButtonMaxBUp()
    {
        Debug.Log("ButtonMaxRUpが押された");
        ModelProduction.UpgradeValue(ref CurB, ref CostMaxBUp, ref MaxB, ModelProduction.TypeUpgrade.MAX);
        UpdateRGBProductionOneColor(CurB, MaxB, TextB, SliderB, IncreaseValueB, CostIncreaseValueBUp, TextIncreaseValueBLeft, TextIncreaseValueBRight, TextCostIncreaseValueBUp, SliderCostIncreaseValueBUp, ButtonIncreaseValueBUp, CostMaxBUp, TextCostMaxBUp, SliderCostMaxBUp, ButtonMaxBUp);
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //シーンセレクト

    //RGB生産シーンボタンが押されたら
    public void PushButtonSelectSceneRGBProduction()
    {
        //PanelRGBProductionの表示
        ShowPanel(PanelRGBProduction);
        //UIの更新
        UpdateRGBProductionScene();


        //他のシーンを非表示
        //PanelRGBProductionの非表示
        NotShowPanel(PanelPixelProduction);
        ClearPixelProductionScene();
    }

    //ピクセル生産シーンボタンが押されたら
    public void PushButtonSelectScenePixelProduction()
    {
        //PanelRGBProductionの表示
        ShowPanel(PanelPixelProduction);
        //UIの更新
        UpdatePixelProductionScene();


        //他のシーンを非表示
        //PanelRGBProductionの非表示
        NotShowPanel(PanelRGBProduction);
    }



    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //キャラクターセレクト
    //キャラクター選択ボタンが押されたら
    public void PushButtonSelectCharacter(string argButtonName)
    {
        //どのボタンで呼び出されたか保存
        ButtonTmp = GameObject.Find(argButtonName).GetComponent<Button>();

        ShowPanelSelectCharacter();
    }

    //キャラクターセレクトの戻るボタンが押されたら
    public void PushButtonBackSelectCharacter()
    {
        //どのボタンで呼び出されたか削除
        ButtonTmp = null;
        //どのキャラクターが選ばれたか削除
        CharacterIDTmp = 0;

        NotShowPanelSelectCharacter();
    }

    //キャラクターセレクトのキャラクターボタンが押されたら
    public void PushButtonSelectCharacterCharacter(uint argCharacterID)
    {
        Debug.Log("SelectCharacterID : " + argCharacterID);
        ImageSelectCharacter.sprite = Sprite.Create(CharactersAll[argCharacterID].ImageTexture2D, new UnityEngine.Rect(0, 0, CharactersAll[argCharacterID].Size, CharactersAll[argCharacterID].Size), new Vector2(0.5f, 0.5f));
        CharacterIDTmp = argCharacterID;
        Button ButtonSelect = GameObject.Find("ButtonConfirmSelectLeft").GetComponent<Button>();
        ButtonSelect.interactable = true;
        ButtonSelect = GameObject.Find("ButtonConfirmSelectRight").GetComponent<Button>();
        ButtonSelect.interactable = true;

        //TODO:キャラクターステータスの表示
        if (ButtonTmp.name.Contains("RProductionHelpCharacter") ||
            ButtonTmp.name.Contains("GProductionHelpCharacter") ||
            ButtonTmp.name.Contains("BProductionHelpCharacter"))
        {
            //選択キャラクターのステータスを表示
            TextSelectCharacter1.text = "CreateR : " + CharactersAll[argCharacterID].Stats[0].RCreates;
            TextSelectCharacter1.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
            TextSelectCharacter2.text = "CreateG : " + CharactersAll[argCharacterID].Stats[0].GCreates;
            TextSelectCharacter2.color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
            TextSelectCharacter3.text = "CreateB : " + CharactersAll[argCharacterID].Stats[0].BCreates;
            TextSelectCharacter3.color = new Color(0.0f, 0.0f, 1.0f, 1.0f);
        }
        else
        if (ButtonTmp.name.Contains("ButtonPixelProductionCharacter"))
        {
            int ColorProductionPixelIndex = int.Parse(ButtonTmp.name.Substring(ButtonTmp.name.Length - 2, 2));

            //選択キャラクターのステータスを表示
            TextSelectCharacter1.text = "      SPD       : " + CharactersAll[argCharacterID].Stats[0].SPD;
            TextSelectCharacter1.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
            TextSelectCharacter2.text = "CreatePixel : " + CharactersAll[argCharacterID].GetCreatePixels((ushort)(ColorProductionPixel[ColorProductionPixelIndex].r * 255), (ushort)(ColorProductionPixel[ColorProductionPixelIndex].g * 255), (ushort)(ColorProductionPixel[ColorProductionPixelIndex].b * 255));
            TextSelectCharacter2.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
            TextSelectCharacter3.text = "  Pixel/sec   : " + GetCreatePixelTime(ColorProductionPixel[ColorProductionPixelIndex], argCharacterID)
                                                           * CharactersAll[argCharacterID].GetCreatePixels((ushort)(ColorProductionPixel[ColorProductionPixelIndex].r * 255), (ushort)(ColorProductionPixel[ColorProductionPixelIndex].g * 255), (ushort)(ColorProductionPixel[ColorProductionPixelIndex].b * 255));
            TextSelectCharacter3.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        }

    }

    //キャラクターセレクトの決定ボタンが押されたら
    public void PushButtonConfirmSelectCharacter()
    {
        ButtonTmp.image.sprite = ImageSelectCharacter.sprite;
        ButtonTmp.image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        ButtonTmp.GetComponentInChildren<Text>().text = "";

        //HelpRGBProductionキャラクターの管理
        if (ButtonTmp.name.Contains("RProductionHelpCharacter") ||
            ButtonTmp.name.Contains("GProductionHelpCharacter") ||
            ButtonTmp.name.Contains("BProductionHelpCharacter"))
        {
            if (ButtonTmp.name.StartsWith("ButtonRProductionHelpCharacter"))//TODO:リファクタリング
            {
                int HelpProductionIndex = int.Parse(ButtonTmp.name.Substring(ButtonTmp.name.Length - 1, 1));

                //前に設定されていたキャラの居場所変更
                CharactersAll[CharactersIDHelpProductionR[HelpProductionIndex]].Whereabouts = Place.None;
                CharactersIDHelpProductionR[HelpProductionIndex] = CharacterIDTmp;

                //居場所変更
                CharactersAll[CharacterIDTmp].Whereabouts = Place.CreateR;
            }
            else
            if (ButtonTmp.name.StartsWith("ButtonGProductionHelpCharacter"))
            {
                int HelpProductionIndex = int.Parse(ButtonTmp.name.Substring(ButtonTmp.name.Length - 1, 1));

                //前に設定されていたキャラの居場所変更
                CharactersAll[CharactersIDHelpProductionG[HelpProductionIndex]].Whereabouts = Place.None;

                CharactersIDHelpProductionG[HelpProductionIndex] = CharacterIDTmp;

                //居場所変更
                CharactersAll[CharacterIDTmp].Whereabouts = Place.CreateG;
            }
            else
            if (ButtonTmp.name.StartsWith("ButtonBProductionHelpCharacter"))
            {
                int HelpProductionIndex = int.Parse(ButtonTmp.name.Substring(ButtonTmp.name.Length - 1, 1));

                //前に設定されていたキャラの居場所変更
                CharactersAll[CharactersIDHelpProductionB[HelpProductionIndex]].Whereabouts = Place.None;

                CharactersIDHelpProductionB[HelpProductionIndex] = CharacterIDTmp;

                //居場所変更
                CharactersAll[CharacterIDTmp].Whereabouts = Place.CreateB;
            }
        }
        else
        //Pixel生産のキャラクターの管理
        if(ButtonTmp.name.Contains("ButtonPixelProductionCharacter"))
        {
            int ProductionPixelIndex = int.Parse(ButtonTmp.name.Substring(ButtonTmp.name.Length - 2, 2));
            CharactersIDProductionPixel[ProductionPixelIndex] = CharacterIDTmp;

            //居場所変更
            CharactersAll[CharacterIDTmp].Whereabouts = Place.CreatePixel;
        }

        NotShowPanelSelectCharacter();
    }

    //キャラクターセレクトの外すボタンが押されたら
    public void PushButtonRemoveCharacterSelect()
    {
        ButtonTmp.image.sprite = null;
        ButtonTmp.GetComponentInChildren<Text>().text = "+";

        //HelpProductionキャラクターの管理
        if (ButtonTmp.name.Contains("RProductionHelpCharacter") ||
            ButtonTmp.name.Contains("GProductionHelpCharacter") ||
            ButtonTmp.name.Contains("BProductionHelpCharacter"))
        {

            if (ButtonTmp.name.StartsWith("ButtonRProductionHelpCharacter"))//TODO:リファクタリング
            {
                ButtonTmp.image.color = new Color(50 / 255f, 0.0f, 0.0f, 1.0f);

                int HelpProductionIndex = int.Parse(ButtonTmp.name.Substring(ButtonTmp.name.Length - 1, 1));

                //前に設定されていたキャラの居場所変更
                CharactersAll[CharactersIDHelpProductionR[HelpProductionIndex]].Whereabouts = Place.None;

                CharactersIDHelpProductionR[HelpProductionIndex] = 0;
            }
            else
            if (ButtonTmp.name.StartsWith("ButtonGProductionHelpCharacter"))
            {
                ButtonTmp.image.color = new Color(0.0f, 50 / 255f, 0.0f, 1.0f);

                int HelpProductionIndex = int.Parse(ButtonTmp.name.Substring(ButtonTmp.name.Length - 1, 1));

                //前に設定されていたキャラの居場所変更
                CharactersAll[CharactersIDHelpProductionG[HelpProductionIndex]].Whereabouts = Place.None;

                CharactersIDHelpProductionG[HelpProductionIndex] = 0;
            }
            else
            if (ButtonTmp.name.StartsWith("ButtonBProductionHelpCharacter"))
            {
                ButtonTmp.image.color = new Color(0.0f, 0.0f, 50 / 255f, 1.0f);

                int HelpProductionIndex = int.Parse(ButtonTmp.name.Substring(ButtonTmp.name.Length - 1, 1));

                //前に設定されていたキャラの居場所変更
                CharactersAll[CharactersIDHelpProductionB[HelpProductionIndex]].Whereabouts = Place.None;

                CharactersIDHelpProductionB[HelpProductionIndex] = 0;
            }
        }
        else
        //Pixel生産のキャラクターの管理
        if (ButtonTmp.name.Contains("ButtonPixelProductionCharacter"))
        {
            int ProductionPixelIndex = int.Parse(ButtonTmp.name.Substring(ButtonTmp.name.Length - 2, 2));

            //前に設定されていたキャラの居場所変更
            CharactersAll[CharactersIDProductionPixel[ProductionPixelIndex]].Whereabouts = Place.None;

            CharactersIDProductionPixel[ProductionPixelIndex] = 0;

        }

        NotShowPanelSelectCharacter();
    }

    //キャラクターセレクトパネルの表示
    public void ShowPanelSelectCharacter()
    {
        //PanelSelectCharacterの表示
        ShowPanel(PanelSelectCharacter);

        //選択ボタンのenable設定
        Button ButtonSelect = GameObject.Find("ButtonConfirmSelectLeft").GetComponent<Button>();
        ButtonSelect.interactable = false;
        ButtonSelect = GameObject.Find("ButtonConfirmSelectRight").GetComponent<Button>();
        ButtonSelect.interactable = false;

        //外すボタンのenable設定
        Button ButtonRemove = GameObject.Find("ButtonRemoveLeft").GetComponent<Button>();
        ButtonRemove.interactable = false;
        ButtonRemove = GameObject.Find("ButtonRemoveRight").GetComponent<Button>();
        ButtonRemove.interactable = false;

        //選択キャラクターの画像とステータスの表示と外すボタンのenable設定
        if (ButtonTmp.name.Contains("RProductionHelpCharacter") ||
            ButtonTmp.name.Contains("GProductionHelpCharacter") ||
            ButtonTmp.name.Contains("BProductionHelpCharacter"))
        {
            //選択キャラクターのステータスの初期表示
            TextSelectCharacter1.text = "CreateR : ";
            TextSelectCharacter1.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
            TextSelectCharacter2.text = "CreateG : ";
            TextSelectCharacter2.color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
            TextSelectCharacter3.text = "CreateB : ";
            TextSelectCharacter3.color = new Color(0.0f, 0.0f, 1.0f, 1.0f);

            if (ButtonTmp.name.StartsWith("ButtonRProductionHelpCharacter"))//TODO:リファクタリング
            {
                int HelpProductionIndex = int.Parse(ButtonTmp.name.Substring(ButtonTmp.name.Length - 1, 1));

                if (CharactersIDHelpProductionR[HelpProductionIndex] != 0)
                {
                    //選択キャラクターの画像を表示
                    ImageSelectCharacter.sprite = Sprite.Create(CharactersAll[CharactersIDHelpProductionR[HelpProductionIndex]].ImageTexture2D, new UnityEngine.Rect(0, 0, CharactersAll[CharactersIDHelpProductionR[HelpProductionIndex]].Size, CharactersAll[CharactersIDHelpProductionR[HelpProductionIndex]].Size), new Vector2(0.5f, 0.5f));
                    //選択キャラクターのステータスを表示
                    TextSelectCharacter1.text = "CreateR : " + CharactersAll[CharactersIDHelpProductionR[HelpProductionIndex]].Stats[0].RCreates;
                    TextSelectCharacter1.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
                    TextSelectCharacter2.text = "CreateG : " + CharactersAll[CharactersIDHelpProductionR[HelpProductionIndex]].Stats[0].GCreates;
                    TextSelectCharacter2.color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
                    TextSelectCharacter3.text = "CreateB : " + CharactersAll[CharactersIDHelpProductionR[HelpProductionIndex]].Stats[0].BCreates;
                    TextSelectCharacter3.color = new Color(0.0f, 0.0f, 1.0f, 1.0f);

                    //外すボタンのenable設定
                    ButtonRemove = GameObject.Find("ButtonRemoveLeft").GetComponent<Button>();
                    ButtonRemove.interactable = true;
                    ButtonRemove = GameObject.Find("ButtonRemoveRight").GetComponent<Button>();
                    ButtonRemove.interactable = true;
                }

            }
            else
            if (ButtonTmp.name.StartsWith("ButtonGProductionHelpCharacter"))
            {
                int HelpProductionIndex = int.Parse(ButtonTmp.name.Substring(ButtonTmp.name.Length - 1, 1));

                if (CharactersIDHelpProductionG[HelpProductionIndex] != 0)
                {
                    //選択キャラクターの画像を表示
                    ImageSelectCharacter.sprite = Sprite.Create(CharactersAll[CharactersIDHelpProductionG[HelpProductionIndex]].ImageTexture2D, new UnityEngine.Rect(0, 0, CharactersAll[CharactersIDHelpProductionG[HelpProductionIndex]].Size, CharactersAll[CharactersIDHelpProductionG[HelpProductionIndex]].Size), new Vector2(0.5f, 0.5f));
                    //選択キャラクターのステータスを表示
                    TextSelectCharacter1.text = "CreateR : " + CharactersAll[CharactersIDHelpProductionG[HelpProductionIndex]].Stats[0].RCreates;
                    TextSelectCharacter1.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
                    TextSelectCharacter2.text = "CreateG : " + CharactersAll[CharactersIDHelpProductionG[HelpProductionIndex]].Stats[0].GCreates;
                    TextSelectCharacter2.color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
                    TextSelectCharacter3.text = "CreateB : " + CharactersAll[CharactersIDHelpProductionG[HelpProductionIndex]].Stats[0].BCreates;
                    TextSelectCharacter3.color = new Color(0.0f, 0.0f, 1.0f, 1.0f);

                    //外すボタンのenable設定
                    ButtonRemove = GameObject.Find("ButtonRemoveLeft").GetComponent<Button>();
                    ButtonRemove.interactable = true;
                    ButtonRemove = GameObject.Find("ButtonRemoveRight").GetComponent<Button>();
                    ButtonRemove.interactable = true;
                }

            }
            else
            if (ButtonTmp.name.StartsWith("ButtonBProductionHelpCharacter"))
            {
                int HelpProductionIndex = int.Parse(ButtonTmp.name.Substring(ButtonTmp.name.Length - 1, 1));

                if (CharactersIDHelpProductionB[HelpProductionIndex] != 0)
                {
                    //選択キャラクターの画像を表示
                    ImageSelectCharacter.sprite = Sprite.Create(CharactersAll[CharactersIDHelpProductionB[HelpProductionIndex]].ImageTexture2D, new UnityEngine.Rect(0, 0, CharactersAll[CharactersIDHelpProductionB[HelpProductionIndex]].Size, CharactersAll[CharactersIDHelpProductionB[HelpProductionIndex]].Size), new Vector2(0.5f, 0.5f));
                    //選択キャラクターのステータスを表示
                    TextSelectCharacter1.text = "CreateR : " + CharactersAll[CharactersIDHelpProductionB[HelpProductionIndex]].Stats[0].RCreates;
                    TextSelectCharacter1.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
                    TextSelectCharacter2.text = "CreateG : " + CharactersAll[CharactersIDHelpProductionB[HelpProductionIndex]].Stats[0].GCreates;
                    TextSelectCharacter2.color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
                    TextSelectCharacter3.text = "CreateB : " + CharactersAll[CharactersIDHelpProductionB[HelpProductionIndex]].Stats[0].BCreates;
                    TextSelectCharacter3.color = new Color(0.0f, 0.0f, 1.0f, 1.0f);

                    //外すボタンのenable設定
                    ButtonRemove = GameObject.Find("ButtonRemoveLeft").GetComponent<Button>();
                    ButtonRemove.interactable = true;
                    ButtonRemove = GameObject.Find("ButtonRemoveRight").GetComponent<Button>();
                    ButtonRemove.interactable = true;
                }

            }
        }
        else
        if (ButtonTmp.name.Contains("ButtonPixelProductionCharacter"))
        {
            //選択キャラクターのステータスの初期表示
            TextSelectCharacter1.text = "      SPD       : ";
            TextSelectCharacter1.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
            TextSelectCharacter2.text = "CreatePixel : ";
            TextSelectCharacter2.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
            TextSelectCharacter3.text = "  Pixel/sec   : ";
            TextSelectCharacter3.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);

            int ProductionPixelIndex = int.Parse(ButtonTmp.name.Substring(ButtonTmp.name.Length - 2, 2));

            if (CharactersIDProductionPixel[ProductionPixelIndex] != 0)
            {
                //選択キャラクターの画像を表示
                ImageSelectCharacter.sprite = Sprite.Create(CharactersAll[CharactersIDProductionPixel[ProductionPixelIndex]].ImageTexture2D, new UnityEngine.Rect(0, 0, CharactersAll[CharactersIDProductionPixel[ProductionPixelIndex]].Size, CharactersAll[CharactersIDProductionPixel[ProductionPixelIndex]].Size), new Vector2(0.5f, 0.5f));
                //選択キャラクターのステータスを表示
                TextSelectCharacter1.text = "      SPD       : " + CharactersAll[CharactersIDProductionPixel[ProductionPixelIndex]].Stats[0].SPD;
                TextSelectCharacter1.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
                TextSelectCharacter2.text = "CreatePixel : " + CharactersAll[CharactersIDProductionPixel[ProductionPixelIndex]].GetCreatePixels((ushort)(ColorProductionPixel[ProductionPixelIndex].r * 255), (ushort)(ColorProductionPixel[ProductionPixelIndex].g * 255), (ushort)(ColorProductionPixel[ProductionPixelIndex].b * 255));
                TextSelectCharacter2.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
                TextSelectCharacter3.text = "  Pixel/sec   : " + GetCreatePixelTime(ColorProductionPixel[ProductionPixelIndex], CharactersIDProductionPixel[ProductionPixelIndex])
                                                               * CharactersAll[CharactersIDProductionPixel[ProductionPixelIndex]].GetCreatePixels((ushort)(ColorProductionPixel[ProductionPixelIndex].r * 255), (ushort)(ColorProductionPixel[ProductionPixelIndex].g * 255), (ushort)(ColorProductionPixel[ProductionPixelIndex].b * 255));
                TextSelectCharacter3.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);

                //外すボタンのenable設定
                ButtonRemove = GameObject.Find("ButtonRemoveLeft").GetComponent<Button>();
                ButtonRemove.interactable = true;
                ButtonRemove = GameObject.Find("ButtonRemoveRight").GetComponent<Button>();
                ButtonRemove.interactable = true;
            }

        }

        //キャラクターボタン生成
        GameObject[] ArrayButtonCharacter = new GameObject[Constants.CHARACTERS_ALL_NUM + 1];
        for (int indexCharacter = 0; indexCharacter < Constants.CHARACTERS_ALL_NUM + 1; indexCharacter++)
        {
            if (CharactersAll[indexCharacter].OwnedNumCur != 0 && CharactersAll[indexCharacter].Whereabouts == Place.None)
            {
                //プレハブのインスタンス化
                ArrayButtonCharacter[indexCharacter] = Instantiate((GameObject)Resources.Load("PrefabButtonCharacterImage"), GameObject.Find("ContentCharacterList").transform) as GameObject;

                //spriteの指定
                ArrayButtonCharacter[indexCharacter].GetComponentInChildren<Image>().sprite = Sprite.Create(CharactersAll[indexCharacter].ImageTexture2D, new UnityEngine.Rect(0, 0, CharactersAll[indexCharacter].Size, CharactersAll[indexCharacter].Size), new Vector2(0.5f, 0.5f));

                //textの指定
                if (ButtonTmp.name.Contains("RProductionHelpCharacter") ||
                    ButtonTmp.name.Contains("GProductionHelpCharacter") ||
                    ButtonTmp.name.Contains("BProductionHelpCharacter"))
                {
                    if (ButtonTmp.name.StartsWith("ButtonRProductionHelpCharacter"))
                    {
                        ArrayButtonCharacter[indexCharacter].GetComponentInChildren<Text>().text = "CreateR/sec : " + CharactersAll[indexCharacter].Stats[0].RCreates.ToString();
                        ArrayButtonCharacter[indexCharacter].GetComponentInChildren<Text>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
                    }
                    else
                    if (ButtonTmp.name.StartsWith("ButtonGProductionHelpCharacter"))
                    {
                        ArrayButtonCharacter[indexCharacter].GetComponentInChildren<Text>().text = "CreateG/sec : " + CharactersAll[indexCharacter].Stats[0].GCreates.ToString();
                        ArrayButtonCharacter[indexCharacter].GetComponentInChildren<Text>().color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
                    }
                    else
                    if (ButtonTmp.name.StartsWith("ButtonBProductionHelpCharacter"))
                    {
                        ArrayButtonCharacter[indexCharacter].GetComponentInChildren<Text>().text = "CreateB/sec : " + CharactersAll[indexCharacter].Stats[0].BCreates.ToString();
                        ArrayButtonCharacter[indexCharacter].GetComponentInChildren<Text>().color = new Color(0.0f, 0.0f, 1.0f, 1.0f);
                    }
                }
                else
                if (ButtonTmp.name.Contains("ButtonPixelProductionCharacter"))
                {
                    int ColorProductionPixelIndex = int.Parse(ButtonTmp.name.Substring(ButtonTmp.name.Length - 2, 2));

                    ArrayButtonCharacter[indexCharacter].GetComponentInChildren<Text>().text = "Pixel/sec : " + GetCreatePixelTime(ColorProductionPixel[ColorProductionPixelIndex], (uint)indexCharacter)
                                                                       * CharactersAll[indexCharacter].GetCreatePixels((ushort)(ColorProductionPixel[ColorProductionPixelIndex].r * 255), (ushort)(ColorProductionPixel[ColorProductionPixelIndex].g * 255), (ushort)(ColorProductionPixel[ColorProductionPixelIndex].b * 255));
                    ArrayButtonCharacter[indexCharacter].GetComponentInChildren<Text>().color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
                }

                //クリックイベントを追加
                uint CharacterID = (uint)(indexCharacter);//匿名メソッドの外部変数のキャプチャの関係で、別の変数に代入
                ArrayButtonCharacter[indexCharacter].GetComponent<Button>().onClick.AddListener(() => PushButtonSelectCharacterCharacter(CharacterID));
            }
        }
    }

    //キャラクターセレクトパネルの非表示
    public void NotShowPanelSelectCharacter()
    {
        //どのボタンで呼び出されたか削除
        ButtonTmp = null;
        //どのキャラクターが選ばれたか削除
        CharacterIDTmp = 0;

        //キャラクターボタンの削除
        foreach (Transform child in GameObject.Find("ContentCharacterList").transform)
        {
            Destroy(child.gameObject);
        }

        //ImageSelectCharacterの画像を削除
        ImageSelectCharacter.sprite = null;

        //PanelSelectCharacterの非表示
        NotShowPanel(PanelSelectCharacter);
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //カラーセレクト
    //色選択ボタンが押されたら
    public void PushButtonSelectColor(string argButtonName)
    {
        //どのボタンで呼び出されたか保存
        ButtonTmp = GameObject.Find(argButtonName).GetComponent<Button>();

        ShowPanelSelectColorMethod();
    }

    //カラーセレクトの色指定方法の戻るボタンが押されたら
    public void PushButtonBackSelectColorMethod()
    {
        //どのボタンで呼び出されたか削除
        ButtonTmp = null;

        NotShowPanelSelectColorMethod();
    }

    //カラーセレクトの色指定方法のRGB値で指定ボタンが押されたら
    public void PushButtonSelectColorMethodRGBNum()
    {
        ShowPanel(PanelSelectColorMethodRGBNum);
    }

    //カラーセレクトの色指定方法のRGB値で指定で戻るボタンが押されたら
    public void PushButtonBackSelectColorMethodRGBNum()
    {
        NotShowPanel(PanelSelectColorMethodRGBNum);
        ColorTmp.r = 0.0f;
        ColorTmp.g = 0.0f;
        ColorTmp.b = 0.0f;
    }

    //カラーセレクトの色指定方法パネルの表示
    public void ShowPanelSelectColorMethod()
    {
        ShowPanel(PanelSelectColor);
        ShowPanel(PanelSelectColorMethod);
    }
    //カラーセレクトの色指定方法パネルの非表示
    public void NotShowPanelSelectColorMethod()
    {
        NotShowPanel(PanelSelectColorMethod);
        NotShowPanel(PanelSelectColor);
    }

    //RGB値で色指定のスライダーまたはインプットフィールドの入力があったら
    public void ValueChangeSliderOrInputFieldSpecificationRGBNum(string argName)
    {
        if(argName.StartsWith("SliderSpecificationNum") || argName.StartsWith("InputFieldSpecificationNum"))
        {
            string strRGB = argName.Substring(argName.Length - 1, 1);

            if (strRGB.Equals("R"))
            {
                if (argName.StartsWith("SliderSpecificationNum"))
                {
                    Slider SliderSpecificationNumR = GameObject.Find("SliderSpecificationNumR").GetComponent<Slider>();
                    ColorTmp.r = (float)(SliderSpecificationNumR.value / 255.0);
                }
                else
                if (argName.StartsWith("InputFieldSpecificationNum"))
                {
                    int num;
                    if (int.TryParse(InputFieldSpecificationNumR.text, out num))
                    {
                        if(num > 255)
                        {
                            num = 255;
                        }
                        ColorTmp.r = num / 255.0f;
                    }
                    else
                    if (InputFieldSpecificationNumR.text.Equals(""))
                    {
                        ColorTmp.r = 0.0f;
                    }
                }

                UpdateSlectColorMethodRGBNum("R");
            }
            else
            if (strRGB.Equals("G"))
            {
                if (argName.StartsWith("SliderSpecificationNum"))
                {
                    Slider SliderSpecificationNumG = GameObject.Find("SliderSpecificationNumG").GetComponent<Slider>();
                    ColorTmp.g = (float)(SliderSpecificationNumG.value / 255.0);
                }
                else
                if (argName.StartsWith("InputFieldSpecificationNum"))
                {
                    int num;
                    if (int.TryParse(InputFieldSpecificationNumG.text, out num))
                    {
                        if (num > 255)
                        {
                            num = 255;
                        }
                        ColorTmp.g = num / 255.0f;
                    }
                    else
                    if (InputFieldSpecificationNumG.text.Equals(""))
                    {
                        ColorTmp.g = 0.0f;
                    }
                }

                UpdateSlectColorMethodRGBNum("G");
            }
            else
            if (strRGB.Equals("B"))
            {
                if (argName.StartsWith("SliderSpecificationNum"))
                {
                    Slider SliderSpecificationNumB = GameObject.Find("SliderSpecificationNumB").GetComponent<Slider>();
                    ColorTmp.b = (float)(SliderSpecificationNumB.value / 255.0);
                }
                else
                if (argName.StartsWith("InputFieldSpecificationNum"))
                {
                    int num;
                    if (int.TryParse(InputFieldSpecificationNumB.text, out num))
                    {
                        if (num > 255)
                        {
                            num = 255;
                        }
                        ColorTmp.b = num / 255.0f;
                    }
                    else
                    if (InputFieldSpecificationNumB.text.Equals(""))
                    {
                        ColorTmp.b = 0.0f;
                    }
                }

                UpdateSlectColorMethodRGBNum("B");
            }
        }
    }
    //RGB値で色指定のDownまたはUpボタンが押されたら
    public void PushButtonSpecificationNumRGBDownOrUP(string argName)
    {
        if (argName.Substring(argName.Length - 5, 1).Equals("R") || argName.Substring(argName.Length - 3, 1).Equals("R"))
        {
            if (argName.Contains("Down") && ColorTmp.r > 0.0f)
            {
                ColorTmp.r -= 1 / 255.0f;
            }
            else
            if (argName.Contains("Up") && ColorTmp.r < 1.0f)
            {
                ColorTmp.r += 1 / 255.0f;
            }

            UpdateSlectColorMethodRGBNum("R");
        }
        else
        if (argName.Substring(argName.Length - 5, 1).Equals("G") || argName.Substring(argName.Length - 3, 1).Equals("G"))
        {
            if (argName.Contains("Down") && ColorTmp.g > 0.0f)
            {
                ColorTmp.g -= 1 / 255.0f;
            }
            else
            if (argName.Contains("Up") && ColorTmp.g < 1.0f)
            {
                ColorTmp.g += 1 / 255.0f;
            }

            UpdateSlectColorMethodRGBNum("G");
        }
        else
        if (argName.Substring(argName.Length - 5, 1).Equals("B") || argName.Substring(argName.Length - 3, 1).Equals("B"))
        {
            if (argName.Contains("Down") && ColorTmp.b > 0.0f)
            {
                ColorTmp.b -= 1 / 255.0f;
            }
            else
            if (argName.Contains("Up") && ColorTmp.b < 1.0f)
            {
                ColorTmp.b += 1 / 255.0f;
            }

            UpdateSlectColorMethodRGBNum("B");
        }
    }
    //RGB値で色指定の決定ボタンが押されたら
    public void PushButtonConfirmSelectColor()
    {
        int ProductionPixelIndex = int.Parse(ButtonTmp.name.Substring(ButtonTmp.name.Length - 2, 2));

        ColorProductionPixel[ProductionPixelIndex].r = ColorTmp.r;
        ColorProductionPixel[ProductionPixelIndex].g = ColorTmp.g;
        ColorProductionPixel[ProductionPixelIndex].b = ColorTmp.b;

        NotShowPanel(PanelSelectColorMethodRGBNum);
        ColorTmp.r = 0;
        ColorTmp.g = 0;
        ColorTmp.b = 0;

        //どのボタンで呼び出されたか削除
        ButtonTmp = null;

        NotShowPanelSelectColorMethod();

        UpdatePixelProductionScene();
    }

    public bool ProductionPixel(Trigger argTrigger)//TODO:RGBが0のときの対処
    {
        for (int i = 1; i < Constants.CHARACTERS_PRODUCTION_PIXEL_NUM + 1; i++)
        {
            if(CharactersIDProductionPixel[i] != 0)
            {
                Debug.Log("OK");

                ushort Progress;
                if(argTrigger == Trigger.Update)
                {
                    Progress = CharactersAll[CharactersIDProductionPixel[i]].Stats[0].SPD;
                }
                else
                if(argTrigger == Trigger.User)
                {
                    Progress = UserProductionPixelNum;
                }
                else
                {
                    Debug.Log("NG");

                    return false;
                }

                Debug.Log("ProgressProductionPixel[0, 1] : " + ProgressProductionPixel[0, 1] + "\n" +
                    "(int)(ColorProductionPixel[i].r * 255) : " + (int)(ColorProductionPixel[i].r * 255));

                if (ProgressProductionPixel[i, 1] < (int)(ColorProductionPixel[i].r * 255))
                {
                    Debug.Log("R");
                    //SPDよりも残りの進捗が少なかったら
                    if (Progress > (int)(ColorProductionPixel[i].r * 255) - ProgressProductionPixel[i, 1])
                    {
                        Progress = (ushort)(ColorProductionPixel[i].r * 255 - ProgressProductionPixel[i, 1]);
                    }

                    if (CurR < Progress)
                    {
                        WarningLackRGB[i, 1] = true;
                    }
                    else
                    {
                        WarningLackRGB[i, 1] = false;
                        CurR -= Progress;
                        ProgressProductionPixel[i, 1] += Progress;
                    }
                }
                else
                if (ProgressProductionPixel[i, 2] < (int)(ColorProductionPixel[i].g * 255))
                {
                    Debug.Log("G");
                    //SPDよりも残りの進捗が少なかったら
                    if (Progress > (int)(ColorProductionPixel[i].g * 255) - ProgressProductionPixel[i, 2])
                    {
                        Progress = (ushort)(ColorProductionPixel[i].g * 255 - ProgressProductionPixel[i, 2]);
                    }

                    if (CurG < Progress)
                    {
                        WarningLackRGB[i, 2] = true;
                    }
                    else
                    {
                        WarningLackRGB[i, 2] = false;
                        CurG -= Progress;
                        ProgressProductionPixel[i, 2] += Progress;
                    }
                }
                else
                if (ProgressProductionPixel[i, 3] < (int)(ColorProductionPixel[i].b * 255))
                {
                    Debug.Log("B");
                    //SPDよりも残りの進捗が少なかったら
                    if (Progress > (int)(ColorProductionPixel[i].b * 255) - ProgressProductionPixel[i, 3])
                    {
                        Progress = (ushort)(ColorProductionPixel[i].b * 255 - ProgressProductionPixel[i, 3]);
                    }

                    if (CurB < Progress)
                    {
                        WarningLackRGB[i, 3] = true;
                    }
                    else
                    {
                        WarningLackRGB[i, 3] = false;
                        CurB -= Progress;
                        ProgressProductionPixel[i, 3] += Progress;
                    }
                }


                if(ProgressProductionPixel[i, 3] >= (int)(ColorProductionPixel[i].b * 255))
                {
                    ProgressProductionPixel[i, 1] = 0;
                    ProgressProductionPixel[i, 2] = 0;
                    ProgressProductionPixel[i, 3] = 0;

                    CurPixels[(int)(ColorProductionPixel[i].r * 255), (int)(ColorProductionPixel[i].g * 255), (int)(ColorProductionPixel[i].b * 255)]
                        += CharactersAll[CharactersIDProductionPixel[i]].GetCreatePixels((ushort)(ColorProductionPixel[i].r * 255), (ushort)(ColorProductionPixel[i].g * 255), (ushort)(ColorProductionPixel[i].b * 255));

                    Debug.Log("CurPixels[0,0,0] : " + CurPixels[(int)(ColorProductionPixel[i].r * 255), (int)(ColorProductionPixel[i].g * 255), (int)(ColorProductionPixel[i].b * 255)]);
                }

            }
        }

        return true;
    }


    ////////////////////////////////////
    //Model?
    int GetCreatePixelTime(Color argColor, uint argCharacterID)
    {
        return GetCreatePixelTimeRGB((ushort)(argColor.r * 255), argCharacterID) +
               GetCreatePixelTimeRGB((ushort)(argColor.g * 255), argCharacterID) +
               GetCreatePixelTimeRGB((ushort)(argColor.b * 255), argCharacterID);
    }
    int GetCreatePixelTimeRGB(ushort argRGB, uint argCharacterID)
    {
        return (int)(Mathf.Ceil((argRGB + 1) / (float)(CharactersAll[argCharacterID].Stats[0].SPD)));
    }



    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //View

    //パネルの表示
    public void ShowPanel(GameObject argPanel)
    {
        CanvasGroup PanelCanvasGroup = argPanel.GetComponent<CanvasGroup>();
        PanelCanvasGroup.alpha = 1;
        PanelCanvasGroup.interactable = true;
        PanelCanvasGroup.blocksRaycasts = true;
    }
    //パネルの非表示
    public void NotShowPanel(GameObject argPanel)
    {
        CanvasGroup PanelCanvasGroup = argPanel.GetComponent<CanvasGroup>();
        PanelCanvasGroup.alpha = 0;
        PanelCanvasGroup.interactable = false;
        PanelCanvasGroup.blocksRaycasts = false;
    }


    //グラフィックをデータと同期させる
    public void UpdateRGBProductionScene()
    {
        UpdateRGBProductionOneColor(CurR, MaxR, TextR, SliderR, IncreaseValueR, CostIncreaseValueRUp, TextIncreaseValueRLeft, TextIncreaseValueRRight, TextCostIncreaseValueRUp, SliderCostIncreaseValueRUp, ButtonIncreaseValueRUp, CostMaxRUp, TextCostMaxRUp, SliderCostMaxRUp, ButtonMaxRUp);
        UpdateRGBProductionOneColor(CurG, MaxG, TextG, SliderG, IncreaseValueG, CostIncreaseValueGUp, TextIncreaseValueGLeft, TextIncreaseValueGRight, TextCostIncreaseValueGUp, SliderCostIncreaseValueGUp, ButtonIncreaseValueGUp, CostMaxGUp, TextCostMaxGUp, SliderCostMaxGUp, ButtonMaxGUp);
        UpdateRGBProductionOneColor(CurB, MaxB, TextB, SliderB, IncreaseValueB, CostIncreaseValueBUp, TextIncreaseValueBLeft, TextIncreaseValueBRight, TextCostIncreaseValueBUp, SliderCostIncreaseValueBUp, ButtonIncreaseValueBUp, CostMaxBUp, TextCostMaxBUp, SliderCostMaxBUp, ButtonMaxBUp);
        UpdateRGBProductionHelpCharacter();
    }
    public void UpdateRGBProductionOneColor(ulong ColorValue, ulong MaxValue, Text TextColorValue, Slider SliderColorValue, ulong IncreaseValue, ulong CostIncreaseUp, Text TextIncreaseValueLeft, Text TextIncreaseValueRight, Text TextCostIncreaseValueUp, Slider SliderCostIncrease, Button ButtonIncreaseValueUp, ulong CostMaxValueUp, Text TextCostMaxValueUp, Slider SliderCostMaxValueUp, Button ButtonMaxValueUp)
    {
        TextColorValue.text = string.Format("{0} / {1}", ColorValue, MaxValue);
        SliderColorValue.maxValue = MaxValue;
        SliderColorValue.minValue = 0;
        SliderColorValue.value = ColorValue;

        TextIncreaseValueLeft.text = string.Format("+{0}", IncreaseValue);
        TextIncreaseValueRight.text = string.Format("+{0}", IncreaseValue);
        TextCostIncreaseValueUp.text = string.Format("コスト : {0}", CostIncreaseUp);
        SliderCostIncrease.maxValue = CostIncreaseUp;
        SliderCostIncrease.minValue = 0;
        SliderCostIncrease.value = ColorValue;
        if (ColorValue < CostIncreaseUp)
        {
            ButtonIncreaseValueUp.interactable = false;
        }
        else
        {
            ButtonIncreaseValueUp.interactable = true;
        }

        TextCostMaxValueUp.text = string.Format("コスト : {0}", CostMaxValueUp);
        SliderCostMaxValueUp.maxValue = CostMaxValueUp;
        SliderCostMaxValueUp.minValue = 0;
        SliderCostMaxValueUp.value = ColorValue;
        if (ColorValue < CostMaxValueUp)
        {
            ButtonMaxValueUp.interactable = false;
        }
        else
        {
            ButtonMaxValueUp.interactable = true;
        }

    }


    public void UpdateRGBProductionHelpCharacter()//TODO:リファクタリング
    {
        if(CharactersIDHelpProductionR[1] != 0)
        {
            Button B = GameObject.Find("ButtonRProductionHelpCharacter1").GetComponent<Button>();
            B.image.sprite = Sprite.Create(CharactersAll[CharactersIDHelpProductionR[1]].ImageTexture2D, new UnityEngine.Rect(0, 0, CharactersAll[CharactersIDHelpProductionR[1]].Size, CharactersAll[CharactersIDHelpProductionR[1]].Size), new Vector2(0.5f, 0.5f));
            B.image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            B.GetComponentInChildren<Text>().text = "";
        }
        if (CharactersIDHelpProductionR[2] != 0)
        {
            Button B = GameObject.Find("ButtonRProductionHelpCharacter2").GetComponent<Button>();
            B.image.sprite = Sprite.Create(CharactersAll[CharactersIDHelpProductionR[2]].ImageTexture2D, new UnityEngine.Rect(0, 0, CharactersAll[CharactersIDHelpProductionR[2]].Size, CharactersAll[CharactersIDHelpProductionR[2]].Size), new Vector2(0.5f, 0.5f));
            B.image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            B.GetComponentInChildren<Text>().text = "";
        }
        if (CharactersIDHelpProductionR[3] != 0)
        {
            Button B = GameObject.Find("ButtonRProductionHelpCharacter3").GetComponent<Button>();
            B.image.sprite = Sprite.Create(CharactersAll[CharactersIDHelpProductionR[3]].ImageTexture2D, new UnityEngine.Rect(0, 0, CharactersAll[CharactersIDHelpProductionR[3]].Size, CharactersAll[CharactersIDHelpProductionR[3]].Size), new Vector2(0.5f, 0.5f));
            B.image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            B.GetComponentInChildren<Text>().text = "";
        }

        if (CharactersIDHelpProductionG[1] != 0)
        {
            Button B = GameObject.Find("ButtonGProductionHelpCharacter1").GetComponent<Button>();
            B.image.sprite = Sprite.Create(CharactersAll[CharactersIDHelpProductionG[1]].ImageTexture2D, new UnityEngine.Rect(0, 0, CharactersAll[CharactersIDHelpProductionG[1]].Size, CharactersAll[CharactersIDHelpProductionG[1]].Size), new Vector2(0.5f, 0.5f));
            B.image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            B.GetComponentInChildren<Text>().text = "";
        }
        if (CharactersIDHelpProductionG[2] != 0)
        {
            Button B = GameObject.Find("ButtonGProductionHelpCharacter2").GetComponent<Button>();
            B.image.sprite = Sprite.Create(CharactersAll[CharactersIDHelpProductionG[2]].ImageTexture2D, new UnityEngine.Rect(0, 0, CharactersAll[CharactersIDHelpProductionG[2]].Size, CharactersAll[CharactersIDHelpProductionG[2]].Size), new Vector2(0.5f, 0.5f));
            B.image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            B.GetComponentInChildren<Text>().text = "";
        }
        if (CharactersIDHelpProductionG[3] != 0)
        {
            Button B = GameObject.Find("ButtonGProductionHelpCharacter3").GetComponent<Button>();
            B.image.sprite = Sprite.Create(CharactersAll[CharactersIDHelpProductionG[3]].ImageTexture2D, new UnityEngine.Rect(0, 0, CharactersAll[CharactersIDHelpProductionG[3]].Size, CharactersAll[CharactersIDHelpProductionG[3]].Size), new Vector2(0.5f, 0.5f));
            B.image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            B.GetComponentInChildren<Text>().text = "";
        }

        if (CharactersIDHelpProductionB[1] != 0)
        {
            Button B = GameObject.Find("ButtonBProductionHelpCharacter1").GetComponent<Button>();
            B.image.sprite = Sprite.Create(CharactersAll[CharactersIDHelpProductionB[1]].ImageTexture2D, new UnityEngine.Rect(0, 0, CharactersAll[CharactersIDHelpProductionB[1]].Size, CharactersAll[CharactersIDHelpProductionB[1]].Size), new Vector2(0.5f, 0.5f));
            B.image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            B.GetComponentInChildren<Text>().text = "";
        }
        if (CharactersIDHelpProductionB[2] != 0)
        {
            Button B = GameObject.Find("ButtonBProductionHelpCharacter2").GetComponent<Button>();
            B.image.sprite = Sprite.Create(CharactersAll[CharactersIDHelpProductionB[2]].ImageTexture2D, new UnityEngine.Rect(0, 0, CharactersAll[CharactersIDHelpProductionB[2]].Size, CharactersAll[CharactersIDHelpProductionB[2]].Size), new Vector2(0.5f, 0.5f));
            B.image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            B.GetComponentInChildren<Text>().text = "";
        }
        if (CharactersIDHelpProductionB[3] != 0)
        {
            Button B = GameObject.Find("ButtonBProductionHelpCharacter3").GetComponent<Button>();
            B.image.sprite = Sprite.Create(CharactersAll[CharactersIDHelpProductionB[3]].ImageTexture2D, new UnityEngine.Rect(0, 0, CharactersAll[CharactersIDHelpProductionB[3]].Size, CharactersAll[CharactersIDHelpProductionB[3]].Size), new Vector2(0.5f, 0.5f));
            B.image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            B.GetComponentInChildren<Text>().text = "";
        }
    }


    public void UpdatePixelProductionScene()//TODO:UpdatePixelProductionScene()
    {
        ClearPixelProductionScene();

        //PixelListの生成
        GameObject[,,] ArrayShowPixelColorAndNum = new GameObject[8,8,1];
        for (int B = 0; B < 1; B++)
            {
            for (int G = 0; G < 8; G++)
            {
                for (int R = 0; R < 8; R++)
                {
                    //プレハブのインスタンス化
                    ArrayShowPixelColorAndNum[R, G, B] = Instantiate((GameObject)Resources.Load("PrefabShowPixelColorAndNum"), GameObject.Find("ContentPixelList").transform) as GameObject;

                    ArrayShowPixelColorAndNum[R, G, B].GetComponentsInChildren<Image>()[1].color = new Color(R / 255f, G / 255f, B / 255f, 1.0f);
                    ArrayShowPixelColorAndNum[R, G, B].GetComponentsInChildren<Text>()[0].text = R.ToString();
                    ArrayShowPixelColorAndNum[R, G, B].GetComponentsInChildren<Text>()[2].text = G.ToString();
                    ArrayShowPixelColorAndNum[R, G, B].GetComponentsInChildren<Text>()[4].text = B.ToString();
                    ArrayShowPixelColorAndNum[R, G, B].GetComponentsInChildren<Text>()[5].text = CurPixels[R, G, B].ToString();
                }
            }
        }

        for (int i = 0; i < Constants.CHARACTERS_PRODUCTION_PIXEL_NUM; i++)
        {
            GameObject Content = GameObject.Find("ContentPixelProductionList").transform.Find("PrefabOnePixelProduction" + (i + 1).ToString("00")).gameObject;

            Content.GetComponentsInChildren<Button>()[0].image.sprite
                = Sprite.Create(CharactersAll[CharactersIDProductionPixel[i + 1]].ImageTexture2D, new UnityEngine.Rect(0, 0, CharactersAll[CharactersIDProductionPixel[i + 1]].Size, CharactersAll[CharactersIDProductionPixel[i + 1]].Size), new Vector2(0.5f, 0.5f));

            Content.GetComponentsInChildren<Button>()[1].image.color
                = new Color(ColorProductionPixel[i + 1].r, ColorProductionPixel[i + 1].g, ColorProductionPixel[i + 1].b);
            Content.GetComponentsInChildren<Button>()[1].image.GetComponentInChildren<Text>().text
                = "#" + ((int)(ColorProductionPixel[i + 1].r * 255)).ToString("X2") + ((int)(ColorProductionPixel[i + 1].g * 255)).ToString("X2") + ((int)(ColorProductionPixel[i + 1].b * 255)).ToString("X2");

            Content.transform.Find("PanelImagePixelColor").gameObject.transform.Find("ImagePixelColorR").gameObject.GetComponent<Image>().GetComponentInChildren<Text>().text
                = (ColorProductionPixel[i + 1].r * 255).ToString();
            Content.transform.Find("PanelImagePixelColor").gameObject.transform.Find("ImagePixelColorG").gameObject.GetComponent<Image>().GetComponentInChildren<Text>().text
                = (ColorProductionPixel[i + 1].g * 255).ToString();
            Content.transform.Find("PanelImagePixelColor").gameObject.transform.Find("ImagePixelColorB").gameObject.GetComponent<Image>().GetComponentInChildren<Text>().text
                = (ColorProductionPixel[i + 1].b * 255).ToString();

            //TODO:スライダー
        }
    }
    public void ClearPixelProductionScene()
    {
        //PixelListの削除
        foreach (Transform child in GameObject.Find("ContentPixelList").transform)
        {
            Destroy(child.gameObject);
        }
    }


    public void UpdateSlectColorMethodRGBNum(string argRGB)
    {
        if (argRGB.Equals("R"))
        {
            ImageSpecificationColorR.color = new Color(ColorTmp.r, 0.0f, 0.0f, 1.0f);

            Slider SliderSpecificationNumR = GameObject.Find("SliderSpecificationNumR").GetComponent<Slider>();
            SliderSpecificationNumR.value = ColorTmp.r * 255;

            InputFieldSpecificationNumR.placeholder.GetComponent<Text>().text = (ColorTmp.r * 255).ToString();
            InputFieldSpecificationNumR.text = (ColorTmp.r * 255).ToString();

            GameObject.Find("TextSelectColorR").GetComponent<Text>().text = "R:" + (ColorTmp.r * 255).ToString();
        }
        else
        if (argRGB.Equals("G"))
        {
            ImageSpecificationColorG.color = new Color(0.0f, ColorTmp.g, 0.0f, 1.0f);

            Slider SliderSpecificationNumG = GameObject.Find("SliderSpecificationNumG").GetComponent<Slider>();
            SliderSpecificationNumG.value = ColorTmp.g * 255;

            InputFieldSpecificationNumG.placeholder.GetComponent<Text>().text = (ColorTmp.g * 255).ToString();
            InputFieldSpecificationNumG.text = (ColorTmp.g * 255).ToString();

            GameObject.Find("TextSelectColorG").GetComponent<Text>().text = "G:" + (ColorTmp.g * 255).ToString();
        }
        else
        if (argRGB.Equals("B"))
        {
            ImageSpecificationColorB.color = new Color(0.0f, 0.0f, ColorTmp.b, 1.0f);

            Slider SliderSpecificationNumB = GameObject.Find("SliderSpecificationNumB").GetComponent<Slider>();
            SliderSpecificationNumB.value = ColorTmp.b * 255;

            InputFieldSpecificationNumB.placeholder.GetComponent<Text>().text = (ColorTmp.b * 255).ToString();
            InputFieldSpecificationNumB.text = (ColorTmp.b * 255).ToString();

            GameObject.Find("TextSelectColorB").GetComponent<Text>().text = "B:" + (ColorTmp.b * 255).ToString();
        }

        GameObject.Find("ImageSelectColor").GetComponent<Image>().color = new Color(ColorTmp.r, ColorTmp.g, ColorTmp.b);
        GameObject.Find("TextSelectColorCode").GetComponent<Text>().text = "#" + ((int)(ColorTmp.r * 255)).ToString("X2") + ((int)(ColorTmp.g * 255)).ToString("X2") + ((int)(ColorTmp.b * 255)).ToString("X2");

        if (((ColorTmp.r + ColorTmp.g + ColorTmp.b) / 3.0f) < 0.5f)
        {
            GameObject.Find("TextSelectColorCode").GetComponent<Text>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            GameObject.Find("TextSelectColorR").GetComponent<Text>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            GameObject.Find("TextSelectColorG").GetComponent<Text>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            GameObject.Find("TextSelectColorB").GetComponent<Text>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }
        else
        {
            GameObject.Find("TextSelectColorCode").GetComponent<Text>().color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
            GameObject.Find("TextSelectColorR").GetComponent<Text>().color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
            GameObject.Find("TextSelectColorG").GetComponent<Text>().color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
            GameObject.Find("TextSelectColorB").GetComponent<Text>().color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        }

    }

}