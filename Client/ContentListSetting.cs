using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System;

[System.Serializable]
//한 페이지당 필요한 데이터 
public struct ContentPage // Value 값
{
    public RectTransform page_obj; //페이지 프리팹
    public Content[] contents; //내용
}

//public struct DataGame

[System.Serializable]
public struct Content
{
    public Data data;
    public bool isTitle;

    public RectTransform spacing_obj; //빈공백
    public ContentInfo dmItem_str; //DM 텍스트 내용 
    public ContentInfo dmItem_warning_str;
    public ContentInfo dmItem_caution_str;
    public ContentInfo dmItem_note_str;

    public FolderContentSetting folderContent;

    public FigureButtonSetting figureButton;
    public TableInfo table; //DM 테이블 내용 
    public OperationSetting operBtn; //운용절차 

    public CatalogInfo catalog; //카탈로그(모델링)

    public GameObject diagramZone;
    public List<DiagramStep> diagramSteps; //다이어그램
}

public class ContentListSetting : MonoBehaviour
{
    [SerializeField] public ScrollRect scrollRect_content;
    [SerializeField] public RectTransform scrollRect_content_rectr;
    [SerializeField] public RectTransform content_box_rectr;
    [SerializeField] public Vector3 reset_rectr;
    [SerializeField] public CanvasGroup CG;

    [Header(" [ CONTENT PAGE ] ")]
    [SerializeField] public RectTransform contentPage_pre; //페이지 

    [Header(" [ CONTENT ] ")]
    [SerializeField] public ContentInfo content_txt_pre;
    [SerializeField] public ContentInfo content_txt_warning_pre;
    [SerializeField] public ContentInfo content_txt_caution_pre;
    [SerializeField] public ContentInfo content_txt_note_pre;

    [SerializeField] public RectTransform spacing_pre;

    [SerializeField] public FolderContentSetting folderContent_pre;

    [SerializeField] public FigureButtonSetting figureButton_pre;

    [SerializeField] public TableInfo table_pre;
    [SerializeField] float tableWidth;
    [SerializeField] public OperationSetting operBtn_pre;

    [SerializeField] public CatalogInfo catalog_pre;

    [SerializeField] public GameObject diagramZone_pre;
    [SerializeField] public DiagramStep diagramStep_pre;

    [SerializeField] public ContentPage[] contentPage_Field;
    [SerializeField] public ContentPage[] contentPage_Unit;

    [SerializeField] float totalHeight = 0f; //아이템마다 중첩되는 높이 계산 
    WaitForSeconds delay = new WaitForSeconds(0.1f);

    [Header(" [ TEXT SETTING ] ")]
    [SerializeField] TMP_FontAsset normalFont;
    [SerializeField] TMP_FontAsset mediumFont;
    [SerializeField] TMP_FontAsset boldFont;

    [SerializeField] float warningImage_height;
    [SerializeField] float noteImage_height;
    [SerializeField] float folderContent_height;

    [Header(" [ TABLE SETTING ] ")]
    [SerializeField] Color header_table_back_color;
    [SerializeField] Color content_table_back_color;
    [SerializeField] Color header_table_text_color, content_table_text_color;
    [SerializeField] float header_table_text_size, content_table_text_size;

    [Header(" [ CONTENT CONTROL ] ")]
    [HideInInspector] public int chapterNum;  // -1 이면 제어 불가능 
    [SerializeField] public ButtonController top_btn;

    //[Header(" [ RESOURCE ] ")]
    [SerializeField] XMLManager xmlManager;
    [SerializeField] UIManager uiManager;

    enum Operation { Field, Unit }; //개별로 사용되는 변수
    [SerializeField] Operation oper;

    private void Start()
    {
        reset_rectr = content_box_rectr.anchoredPosition;
    }

    public void PageReset()
    {
        InitPageData();

        scrollRect_content.content = content_box_rectr;

        scrollRect_content.content.anchoredPosition = Vector3.zero;
        scrollRect_content_rectr.anchoredPosition = Vector3.zero;
        content_box_rectr.anchoredPosition = reset_rectr;

        chapterNum = -1;
        top_btn.gameObject.SetActive(false);
    }

    public void CreatePage()
    {
        if (oper == Operation.Field)
        {
            contentPage_Field = new ContentPage[xmlManager.dmContents_Field.Count]; //저장 공간
            for (int i = 0; i < contentPage_Field.Length; i++)
            {
                contentPage_Field[i].page_obj = Instantiate(contentPage_pre, gameObject.transform);
            }

            PageReset();
        }
        else if (oper == Operation.Unit)
        {
            contentPage_Unit = new ContentPage[xmlManager.dmContents_Unit.Count];

            for (int i = 0; i < contentPage_Unit.Length; i++)
            {
                contentPage_Unit[i].page_obj = Instantiate(contentPage_pre, gameObject.transform);
            }

            PageReset();
        }
        else return;
    }

    bool figureControl = false;
    //컨텐츠 영역중에 Figure버튼 리소스가 필요 없을때 
    void FigureControlCheck(int pageNum)
    {
        figureControl = false;
        if (oper == Operation.Field)
        {
            for (int j = 0; j < contentPage_Field[pageNum].contents.Length; j++)
            {
                if (xmlManager.dmContents_Field[pageNum].contents[j].data == Data.CATALOG)
                {
                    figureControl = true;
                }
            }
        }
        else if (oper == Operation.Unit)
        {
            for (int j = 0; j < contentPage_Unit[pageNum].contents.Length; j++)
            {
                if (xmlManager.dmContents_Unit[pageNum].contents[j].data == Data.CATALOG)
                {
                    figureControl = true;
                }
            }
        }
    }
    bool spacingCheck = false; //연속해서 나오는 공백을 막기위함 

    //페이지 생성(해당 컨텐츠 페이지 클릭했을때 그 페이지만 로드)
    public void SelectPageDataLoad(int pageNum)
    {
        int figureCnt = 1;

        if (oper == Operation.Field)
        {
            if (xmlManager.dmContents_Field[pageNum].contents != null)
            {
                contentPage_Field[pageNum].page_obj.gameObject.SetActive(true);
                Debug.Log(xmlManager.dmContents_Field[pageNum].OwnDM.name);

                //한페이지 내용들
                contentPage_Field[pageNum].contents = new Content[xmlManager.dmContents_Field[pageNum].contents.Count];

                //Figure 제어가 필요한지 체크 함수 
                FigureControlCheck(pageNum);

                //contents 하나당 텍스트 하나 또는 테이블 하나 
                for (int j = 0; j < contentPage_Field[pageNum].contents.Length; j++)
                {
                    contentPage_Field[pageNum].contents[j].data = xmlManager.dmContents_Field[pageNum].contents[j].data;
                    contentPage_Field[pageNum].contents[j].isTitle = xmlManager.dmContents_Field[pageNum].contents[j].isTitle;

                    //빈공백일때 
                    if (xmlManager.dmContents_Field[pageNum].contents[j].data == Data.SPACING && !spacingCheck)
                    {
                        spacingCheck = true;
                        contentPage_Field[pageNum].contents[j].spacing_obj = Instantiate(spacing_pre, contentPage_Field[pageNum].page_obj.transform);
                    }

                    //text/warning text 인 경우 
                    else if (xmlManager.dmContents_Field[pageNum].contents[j].data == Data.TEXT)
                    {
                        spacingCheck = false;
                        if (xmlManager.dmContents_Field[pageNum].contents[j].texttype == TEXTTYPE.WARNING)
                        {
                            contentPage_Field[pageNum].contents[j].dmItem_warning_str = Instantiate(content_txt_warning_pre, contentPage_Field[pageNum].page_obj.transform);
                            contentPage_Field[pageNum].contents[j].dmItem_warning_str.content_txt.text = xmlManager.dmContents_Field[pageNum].contents[j].dmItem_text;
                            contentPage_Field[pageNum].contents[j].dmItem_warning_str.content_txt.fontSize = xmlManager.dmContents_Field[pageNum].contents[j].textSize;
                            contentPage_Field[pageNum].contents[j].dmItem_warning_str.content_txt.color = xmlManager.dmContents_Field[pageNum].contents[j].textColor;

                            if (xmlManager.dmContents_Field[pageNum].contents[j].textAlign == TextDefine.TEXTALIGN.CENTER)
                            {
                                contentPage_Field[pageNum].contents[j].dmItem_warning_str.content_txt.alignment = TMPro.TextAlignmentOptions.Midline;
                            }
                            else if (xmlManager.dmContents_Field[pageNum].contents[j].textAlign == TextDefine.TEXTALIGN.LEFT)
                            {
                                contentPage_Field[pageNum].contents[j].dmItem_warning_str.content_txt.alignment = TMPro.TextAlignmentOptions.MidlineLeft;
                            }
                            else if (xmlManager.dmContents_Field[pageNum].contents[j].textAlign == TextDefine.TEXTALIGN.RIGHT)
                            {
                                contentPage_Field[pageNum].contents[j].dmItem_warning_str.content_txt.alignment = TMPro.TextAlignmentOptions.MidlineRight;
                            }
                            else
                            {
                                Debug.Log("NO TEXT ALIGN");
                            }

                            if (xmlManager.dmContents_Field[pageNum].contents[j].textStyle == TextDefine.TEXTSTYLE.NORMAL)
                            {
                                contentPage_Field[pageNum].contents[j].dmItem_warning_str.content_txt.font = normalFont;
                            }
                            else if (xmlManager.dmContents_Field[pageNum].contents[j].textStyle == TextDefine.TEXTSTYLE.BOLD)
                            {
                                contentPage_Field[pageNum].contents[j].dmItem_warning_str.content_txt.font = boldFont;
                            }
                            else if (xmlManager.dmContents_Field[pageNum].contents[j].textStyle == TextDefine.TEXTSTYLE.MEDIUM)
                            {
                                contentPage_Field[pageNum].contents[j].dmItem_warning_str.content_txt.font = mediumFont;
                            }
                            else
                            {
                                Debug.Log("NO TEXT STYLE");
                            }
                        }
                        else if (xmlManager.dmContents_Field[pageNum].contents[j].texttype == TEXTTYPE.CAUTION)
                        {
                            contentPage_Field[pageNum].contents[j].dmItem_caution_str = Instantiate(content_txt_caution_pre, contentPage_Field[pageNum].page_obj.transform);
                            contentPage_Field[pageNum].contents[j].dmItem_caution_str.content_txt.text = xmlManager.dmContents_Field[pageNum].contents[j].dmItem_text;
                            contentPage_Field[pageNum].contents[j].dmItem_caution_str.content_txt.fontSize = xmlManager.dmContents_Field[pageNum].contents[j].textSize;
                            contentPage_Field[pageNum].contents[j].dmItem_caution_str.content_txt.color = xmlManager.dmContents_Field[pageNum].contents[j].textColor;

                            if (xmlManager.dmContents_Field[pageNum].contents[j].textAlign == TextDefine.TEXTALIGN.CENTER)
                            {
                                contentPage_Field[pageNum].contents[j].dmItem_caution_str.content_txt.alignment = TMPro.TextAlignmentOptions.Midline;
                            }
                            else if (xmlManager.dmContents_Field[pageNum].contents[j].textAlign == TextDefine.TEXTALIGN.LEFT)
                            {
                                contentPage_Field[pageNum].contents[j].dmItem_caution_str.content_txt.alignment = TMPro.TextAlignmentOptions.MidlineLeft;
                            }
                            else if (xmlManager.dmContents_Field[pageNum].contents[j].textAlign == TextDefine.TEXTALIGN.RIGHT)
                            {
                                contentPage_Field[pageNum].contents[j].dmItem_caution_str.content_txt.alignment = TMPro.TextAlignmentOptions.MidlineRight;
                            }
                            else
                            {
                                Debug.Log("NO TEXT ALIGN");
                            }

                            if (xmlManager.dmContents_Field[pageNum].contents[j].textStyle == TextDefine.TEXTSTYLE.NORMAL)
                            {
                                contentPage_Field[pageNum].contents[j].dmItem_caution_str.content_txt.font = normalFont;
                            }
                            else if (xmlManager.dmContents_Field[pageNum].contents[j].textStyle == TextDefine.TEXTSTYLE.BOLD)
                            {
                                contentPage_Field[pageNum].contents[j].dmItem_caution_str.content_txt.font = boldFont;
                            }
                            else if (xmlManager.dmContents_Field[pageNum].contents[j].textStyle == TextDefine.TEXTSTYLE.MEDIUM)
                            {
                                contentPage_Field[pageNum].contents[j].dmItem_caution_str.content_txt.font = mediumFont;
                            }
                            else
                            {
                                Debug.Log("NO TEXT STYLE");
                            }
                        }
                        else if (xmlManager.dmContents_Field[pageNum].contents[j].texttype == TEXTTYPE.NOTE)
                        {
                            contentPage_Field[pageNum].contents[j].dmItem_note_str = Instantiate(content_txt_note_pre, contentPage_Field[pageNum].page_obj.transform);
                            contentPage_Field[pageNum].contents[j].dmItem_note_str.content_txt.text = xmlManager.dmContents_Field[pageNum].contents[j].dmItem_text;
                            contentPage_Field[pageNum].contents[j].dmItem_note_str.content_txt.fontSize = xmlManager.dmContents_Field[pageNum].contents[j].textSize;
                            contentPage_Field[pageNum].contents[j].dmItem_note_str.content_txt.color = xmlManager.dmContents_Field[pageNum].contents[j].textColor;

                            if (xmlManager.dmContents_Field[pageNum].contents[j].textAlign == TextDefine.TEXTALIGN.CENTER)
                            {
                                contentPage_Field[pageNum].contents[j].dmItem_note_str.content_txt.alignment = TMPro.TextAlignmentOptions.Midline;
                            }
                            else if (xmlManager.dmContents_Field[pageNum].contents[j].textAlign == TextDefine.TEXTALIGN.LEFT)
                            {
                                contentPage_Field[pageNum].contents[j].dmItem_note_str.content_txt.alignment = TMPro.TextAlignmentOptions.MidlineLeft;
                            }
                            else if (xmlManager.dmContents_Field[pageNum].contents[j].textAlign == TextDefine.TEXTALIGN.RIGHT)
                            {
                                contentPage_Field[pageNum].contents[j].dmItem_note_str.content_txt.alignment = TMPro.TextAlignmentOptions.MidlineRight;
                            }
                            else
                            {
                                Debug.Log("NO TEXT ALIGN");
                            }

                            if (xmlManager.dmContents_Field[pageNum].contents[j].textStyle == TextDefine.TEXTSTYLE.NORMAL)
                            {
                                contentPage_Field[pageNum].contents[j].dmItem_note_str.content_txt.font = normalFont;
                            }
                            else if (xmlManager.dmContents_Field[pageNum].contents[j].textStyle == TextDefine.TEXTSTYLE.BOLD)
                            {
                                contentPage_Field[pageNum].contents[j].dmItem_note_str.content_txt.font = boldFont;
                            }
                            else if (xmlManager.dmContents_Field[pageNum].contents[j].textStyle == TextDefine.TEXTSTYLE.MEDIUM)
                            {
                                contentPage_Field[pageNum].contents[j].dmItem_note_str.content_txt.font = mediumFont;
                            }
                            else
                            {
                                Debug.Log("NO TEXT STYLE");
                            }
                        }
                        else if (xmlManager.dmContents_Field[pageNum].contents[j].texttype == TEXTTYPE.TEXT)
                        {
                            contentPage_Field[pageNum].contents[j].dmItem_str = Instantiate(content_txt_pre, contentPage_Field[pageNum].page_obj.transform);
                            contentPage_Field[pageNum].contents[j].dmItem_str.content_txt.text = xmlManager.dmContents_Field[pageNum].contents[j].dmItem_text;
                            contentPage_Field[pageNum].contents[j].dmItem_str.content_txt.fontSize = xmlManager.dmContents_Field[pageNum].contents[j].textSize;
                            contentPage_Field[pageNum].contents[j].dmItem_str.content_txt.color = xmlManager.dmContents_Field[pageNum].contents[j].textColor;

                            if (xmlManager.dmContents_Field[pageNum].contents[j].textAlign == TextDefine.TEXTALIGN.CENTER)
                            {
                                contentPage_Field[pageNum].contents[j].dmItem_str.content_txt.alignment = TMPro.TextAlignmentOptions.Midline;
                            }
                            else if (xmlManager.dmContents_Field[pageNum].contents[j].textAlign == TextDefine.TEXTALIGN.LEFT)
                            {
                                contentPage_Field[pageNum].contents[j].dmItem_str.content_txt.alignment = TMPro.TextAlignmentOptions.MidlineLeft;
                            }
                            else if (xmlManager.dmContents_Field[pageNum].contents[j].textAlign == TextDefine.TEXTALIGN.RIGHT)
                            {
                                contentPage_Field[pageNum].contents[j].dmItem_str.content_txt.alignment = TMPro.TextAlignmentOptions.MidlineRight;
                            }
                            else
                            {
                                Debug.Log("NO TEXT ALIGN");
                            }

                            if (xmlManager.dmContents_Field[pageNum].contents[j].textStyle == TextDefine.TEXTSTYLE.NORMAL)
                            {
                                contentPage_Field[pageNum].contents[j].dmItem_str.content_txt.font = normalFont;
                            }
                            else if (xmlManager.dmContents_Field[pageNum].contents[j].textStyle == TextDefine.TEXTSTYLE.BOLD)
                            {
                                contentPage_Field[pageNum].contents[j].dmItem_str.content_txt.font = boldFont;
                            }
                            else if (xmlManager.dmContents_Field[pageNum].contents[j].textStyle == TextDefine.TEXTSTYLE.MEDIUM)
                            {
                                contentPage_Field[pageNum].contents[j].dmItem_str.content_txt.font = mediumFont;
                            }
                            else
                            {
                                Debug.Log("NO TEXT STYLE");
                            }
                        }
                    }

                    //FolderContent 인 경우 
                    else if (xmlManager.dmContents_Field[pageNum].contents[j].data == Data.FOLDERCONTENT)
                    {
                        spacingCheck = false;
                        //header 
                        contentPage_Field[pageNum].contents[j].folderContent = Instantiate(folderContent_pre, contentPage_Field[pageNum].page_obj.transform);

                        contentPage_Field[pageNum].contents[j].folderContent.header_txt.text = xmlManager.dmContents_Field[pageNum].contents[j].dmItem_text;
                        contentPage_Field[pageNum].contents[j].folderContent.header_txt.fontSize = xmlManager.dmContents_Field[pageNum].contents[j].textSize;
                        contentPage_Field[pageNum].contents[j].folderContent.header_txt.color = xmlManager.dmContents_Field[pageNum].contents[j].textColor;
                    }

                    //Figure 인 경우 (이미지 또는 객체) - 버튼 형식 
                    else if (xmlManager.dmContents_Field[pageNum].contents[j].data == Data.FIGURE && !figureControl)
                    {
                        spacingCheck = false;
                        //header 
                        contentPage_Field[pageNum].contents[j].figureButton = Instantiate(figureButton_pre, contentPage_Field[pageNum].page_obj.transform);

                        contentPage_Field[pageNum].contents[j].figureButton.button_txt.text = "<size=20>그림 " + figureCnt + ". </size>" +
                            xmlManager.dmContents_Field[pageNum].contents[j].figure.title + " 보기";
                        figureCnt++;

                        contentPage_Field[pageNum].contents[j].figureButton.id = xmlManager.dmContents_Field[pageNum].contents[j].figure.id;
                        contentPage_Field[pageNum].contents[j].figureButton.info = xmlManager.dmContents_Field[pageNum].contents[j].figure.infoEntityIdent;

                        string id = contentPage_Field[pageNum].contents[j].figureButton.id;
                        string info = contentPage_Field[pageNum].contents[j].figureButton.info;
                        int tmp = pageNum;
                        int tmp_1 = j;

                        //버튼 이벤트 등록
                        contentPage_Field[pageNum].contents[j].figureButton.btn.onClick.AddListener(() =>
                        {
                            uiManager.detailSettingCanvas_Field.objectListSetting.FigureSetting(id, info);

                            FigureButtonController(tmp, tmp_1);
                        });
                    }

                    //정비 절차 버튼인 경우 
                    else if (xmlManager.dmContents_Field[pageNum].contents[j].data == Data.OPERATION)
                    {
                        spacingCheck = false;
                        contentPage_Field[pageNum].contents[j].operBtn = Instantiate(operBtn_pre, contentPage_Field[pageNum].page_obj.transform);

                        //OperationItems 판단 
                        for (int i = 0; i < xmlManager.dmContents_Field[pageNum].contents[j].operBtn.colume.Count; i++)
                        {
                            if (xmlManager.dmContents_Field[pageNum].contents[j].operBtn.colume[i].OperationItem_type == OperationItems.OPERATION)
                            {
                                contentPage_Field[pageNum].contents[j].operBtn.CreateOperationItems(OperationItems.OPERATION);
                            }
                            else if (xmlManager.dmContents_Field[pageNum].contents[j].operBtn.colume[i].OperationItem_type == OperationItems.WARNING)
                            {
                                contentPage_Field[pageNum].contents[j].operBtn.CreateOperationItems(OperationItems.WARNING);
                            }
                            else if (xmlManager.dmContents_Field[pageNum].contents[j].operBtn.colume[i].OperationItem_type == OperationItems.CAUTION)
                            {
                                contentPage_Field[pageNum].contents[j].operBtn.CreateOperationItems(OperationItems.CAUTION);
                            }
                            else if (xmlManager.dmContents_Field[pageNum].contents[j].operBtn.colume[i].OperationItem_type == OperationItems.NOTE)
                            {
                                contentPage_Field[pageNum].contents[j].operBtn.CreateOperationItems(OperationItems.NOTE);
                            }

                            //타입 등록 
                            contentPage_Field[pageNum].contents[j].operBtn.op_btn[i].type = xmlManager.dmContents_Field[pageNum].contents[j].operBtn.colume[i].OperationItem_type;
                        }

                        contentPage_Field[pageNum].contents[j].operBtn.title = xmlManager.dmContents_Field[pageNum].contents[j].operBtn.operTitle;
                        contentPage_Field[pageNum].contents[j].operBtn.subtitle = xmlManager.dmContents_Field[pageNum].contents[j].operBtn.operSubTitle;

                        string title = xmlManager.dmContents_Field[pageNum].contents[j].operBtn.operTitle;
                        string subTitle = xmlManager.dmContents_Field[pageNum].contents[j].operBtn.operSubTitle;
                        OperationSetting operSetting = contentPage_Field[pageNum].contents[j].operBtn;

                        //Debug.Log("Operation Title : " + title + " / Operation SubTitle : " + subTitle);
                        uiManager.detailSettingCanvas_Field.objectListSetting.OperationReady(title, subTitle, operSetting); //모델링 활성화 

                        int btnstep = 0;
                        for (int z = 0; z < contentPage_Field[pageNum].contents[j].operBtn.op_btn.Count; z++)
                        {
                            if (contentPage_Field[pageNum].contents[j].operBtn.op_btn[z].type == OperationItems.OPERATION)
                            {
                                contentPage_Field[pageNum].contents[j].operBtn.op_btn[z].stepindex = z;

                                contentPage_Field[pageNum].contents[j].operBtn.op_btn[z]._text.text = xmlManager.dmContents_Field[pageNum].contents[j].operBtn.colume[z].operationItem_text;

                                int step;
                                step = btnstep;
                                btnstep += 1;

                                //클릭 이벤트 추가 
                                contentPage_Field[pageNum].contents[j].operBtn.op_btn[z].btn.onClick.AddListener(() =>
                                {
                                    operSetting.ChildFunCheck(step);
                                    uiManager.detailSettingCanvas_Field.objectListSetting.OperationButtonClick(title, subTitle, step);
                                });
                            }
                            else if (contentPage_Field[pageNum].contents[j].operBtn.op_btn[z].type == OperationItems.WARNING
                                || contentPage_Field[pageNum].contents[j].operBtn.op_btn[z].type == OperationItems.CAUTION
                                || contentPage_Field[pageNum].contents[j].operBtn.op_btn[z].type == OperationItems.NOTE)
                            {
                                contentPage_Field[pageNum].contents[j].operBtn.op_btn[z]._text.text = xmlManager.dmContents_Field[pageNum].contents[j].operBtn.colume[z].operationItem_text;
                                contentPage_Field[pageNum].contents[j].operBtn.op_btn[z]._text.fontSize = xmlManager.dmContents_Field[pageNum].contents[j].operBtn.colume[z].textSize;
                                contentPage_Field[pageNum].contents[j].operBtn.op_btn[z]._text.color = xmlManager.dmContents_Field[pageNum].contents[j].operBtn.colume[z].textColor;

                                if (xmlManager.dmContents_Field[pageNum].contents[j].operBtn.colume[z].textAlign == TextDefine.TEXTALIGN.CENTER)
                                {
                                    contentPage_Field[pageNum].contents[j].operBtn.op_btn[z]._text.alignment = TMPro.TextAlignmentOptions.Midline;
                                }
                                else if (xmlManager.dmContents_Field[pageNum].contents[j].operBtn.colume[z].textAlign == TextDefine.TEXTALIGN.LEFT)
                                {
                                    contentPage_Field[pageNum].contents[j].operBtn.op_btn[z]._text.alignment = TMPro.TextAlignmentOptions.MidlineLeft;
                                }
                                else if (xmlManager.dmContents_Field[pageNum].contents[j].operBtn.colume[z].textAlign == TextDefine.TEXTALIGN.RIGHT)
                                {
                                    contentPage_Field[pageNum].contents[j].operBtn.op_btn[z]._text.alignment = TMPro.TextAlignmentOptions.MidlineRight;
                                }
                                else
                                {
                                    Debug.Log("NO TEXT ALIGN");
                                }

                                if (xmlManager.dmContents_Field[pageNum].contents[j].operBtn.colume[z].textStyle == TextDefine.TEXTSTYLE.NORMAL)
                                {
                                    contentPage_Field[pageNum].contents[j].operBtn.op_btn[z]._text.font = normalFont;
                                }
                                else if (xmlManager.dmContents_Field[pageNum].contents[j].operBtn.colume[z].textStyle == TextDefine.TEXTSTYLE.BOLD)
                                {
                                    contentPage_Field[pageNum].contents[j].operBtn.op_btn[z]._text.font = boldFont;
                                }
                                else if (xmlManager.dmContents_Field[pageNum].contents[j].operBtn.colume[z].textStyle == TextDefine.TEXTSTYLE.MEDIUM)
                                {
                                    contentPage_Field[pageNum].contents[j].operBtn.op_btn[z]._text.font = mediumFont;
                                }
                                else
                                {
                                    Debug.Log("NO TEXT STYLE");
                                }
                            }
                        }
                    }

                    //테이블인 경우 
                    else if (xmlManager.dmContents_Field[pageNum].contents[j].data == Data.TABLE) //널이 아닐때 체크하는게 좀 문제있는듯?
                    {
                        spacingCheck = false;
                        //테이블 값이 들어가면 내용은 null
                        contentPage_Field[pageNum].contents[j].table = Instantiate(table_pre, contentPage_Field[pageNum].page_obj.transform);

                        //COLSPEC
                        for (int z = 0; z < xmlManager.dmContents_Field[pageNum].contents[j].table.colspec.Count; z++)
                        {
                            contentPage_Field[pageNum].contents[j].table.colspec.Add(xmlManager.dmContents_Field[pageNum].contents[j].table.colspec[z]);
                        }

                        //COLUME(Header)
                        contentPage_Field[pageNum].contents[j].table.CreateHeaderLine(xmlManager.dmContents_Field[pageNum].contents[j].table.header_lineCnt);
                        for (int z = 0; z < xmlManager.dmContents_Field[pageNum].contents[j].table.header_lineCnt; z++)
                        {
                            contentPage_Field[pageNum].contents[j].table.header_lines[z].CreateColume(contentPage_Field[pageNum].contents[j].table.colspec.Count);
                        }

                        int header_ColumeCnt = 0;
                        int activeCnt_header = 0; //비활성화된 칼럼이 몇개인지 파악 (사이즈를 맞추기위해)
                                                  //Header Data Setting
                        for (int z = 0; z < contentPage_Field[pageNum].contents[j].table.header_lines.Length; z++)
                        {
                            for (int g = 0; g < contentPage_Field[pageNum].contents[j].table.header_lines[z].child_colume.Length; g++)
                            {
                                contentPage_Field[pageNum].contents[j].table.header_lines[z].child_colume[g].colume = new DMItem_Table_Colume(
                                    xmlManager.dmContents_Field[pageNum].contents[j].table.header_colume[header_ColumeCnt].morerows, xmlManager.dmContents_Field[pageNum].contents[j].table.header_colume[header_ColumeCnt].namest,
                                    xmlManager.dmContents_Field[pageNum].contents[j].table.header_colume[header_ColumeCnt].nameend, xmlManager.dmContents_Field[pageNum].contents[j].table.header_colume[header_ColumeCnt].totalColwidth,
                                    xmlManager.dmContents_Field[pageNum].contents[j].table.header_colume[header_ColumeCnt].textAlign, xmlManager.dmContents_Field[pageNum].contents[j].table.header_colume[header_ColumeCnt].colume_txt,
                                    xmlManager.dmContents_Field[pageNum].contents[j].table.header_colume[header_ColumeCnt].isEmpty);


                                contentPage_Field[pageNum].contents[j].table.header_lines[z].child_colume[g].colume_img.color = header_table_back_color;
                                contentPage_Field[pageNum].contents[j].table.header_lines[z].child_colume[g]._text.color = header_table_text_color;
                                contentPage_Field[pageNum].contents[j].table.header_lines[z].child_colume[g]._text.fontSize = header_table_text_size;


                                //테이블 아이템 비율에 맞춰 사이즈 조절 
                                contentPage_Field[pageNum].contents[j].table.header_lines[z].child_colume[g].rec_tr.sizeDelta = new Vector2(
                                    contentPage_Field[pageNum].contents[j].table.header_lines[z].child_colume[g].colume.totalColwidth * tableWidth / xmlManager.dmContents_Field[pageNum].contents[j].table.sumColWidth,
                                    contentPage_Field[pageNum].contents[j].table.header_lines[z].child_colume[g].rec_tr.sizeDelta.y);

                                contentPage_Field[pageNum].contents[j].table.header_lines[z].child_colume[g]._text.text = contentPage_Field[pageNum].contents[j].table.header_lines[z].child_colume[g].colume.colume_txt;

                                if (contentPage_Field[pageNum].contents[j].table.header_lines[z].child_colume[g].colume.textAlign == TextDefine.TEXTALIGN.CENTER)
                                {
                                    contentPage_Field[pageNum].contents[j].table.header_lines[z].child_colume[g]._text.alignment = TMPro.TextAlignmentOptions.Midline;
                                }
                                else if (contentPage_Field[pageNum].contents[j].table.header_lines[z].child_colume[g].colume.textAlign == TextDefine.TEXTALIGN.LEFT)
                                {
                                    contentPage_Field[pageNum].contents[j].table.header_lines[z].child_colume[g]._text.alignment = TMPro.TextAlignmentOptions.MidlineLeft;
                                }
                                else if (contentPage_Field[pageNum].contents[j].table.header_lines[z].child_colume[g].colume.textAlign == TextDefine.TEXTALIGN.RIGHT)
                                {
                                    contentPage_Field[pageNum].contents[j].table.header_lines[z].child_colume[g]._text.alignment = TMPro.TextAlignmentOptions.MidlineRight;
                                }
                                else
                                {
                                    Debug.Log("NO TEXT ALIGN");
                                }

                                if (contentPage_Field[pageNum].contents[j].table.header_lines[z].child_colume[g].colume.isEmpty)
                                {
                                    contentPage_Field[pageNum].contents[j].table.header_lines[z].child_colume[g].gameObject.SetActive(false);
                                    activeCnt_header += 1;
                                }

                                ++header_ColumeCnt;
                            }

                            //테이블 사이즈 간격 통일 
                            for (int g = 0; g < contentPage_Field[pageNum].contents[j].table.header_lines[z].child_colume.Length; g++)
                            {
                                if (contentPage_Field[pageNum].contents[j].table.header_lines[z].child_colume[g].gameObject.activeInHierarchy)
                                {
                                    contentPage_Field[pageNum].contents[j].table.header_lines[z].child_colume[g].rec_tr.sizeDelta = new Vector2(
                                        contentPage_Field[pageNum].contents[j].table.header_lines[z].child_colume[g].rec_tr.sizeDelta.x + activeCnt_header,
                                        contentPage_Field[pageNum].contents[j].table.header_lines[z].child_colume[g].rec_tr.sizeDelta.y);

                                    activeCnt_header = 0;

                                    break;
                                }
                            }
                        }

                        //int 
                        //COLUME(Content)
                        contentPage_Field[pageNum].contents[j].table.CreateContentLine(xmlManager.dmContents_Field[pageNum].contents[j].table.content_lineCnt);
                        for (int z = 0; z < xmlManager.dmContents_Field[pageNum].contents[j].table.content_lineCnt; z++)
                        {
                            contentPage_Field[pageNum].contents[j].table.content_lines[z].CreateColume(contentPage_Field[pageNum].contents[j].table.colspec.Count);
                        }

                        int content_ColumeCnt = 0;
                        int activeCnt_content = 0; //비활성화된 칼럼이 몇개인지 파악 (사이즈를 맞추기위해)
                                                   //Content Data Setting
                        for (int z = 0; z < contentPage_Field[pageNum].contents[j].table.content_lines.Length; z++)
                        {
                            for (int g = 0; g < contentPage_Field[pageNum].contents[j].table.content_lines[z].child_colume.Length; g++)
                            {
                                contentPage_Field[pageNum].contents[j].table.content_lines[z].child_colume[g].colume = new DMItem_Table_Colume(
                                    xmlManager.dmContents_Field[pageNum].contents[j].table.content_colume[content_ColumeCnt].morerows, xmlManager.dmContents_Field[pageNum].contents[j].table.content_colume[content_ColumeCnt].namest,
                                    xmlManager.dmContents_Field[pageNum].contents[j].table.content_colume[content_ColumeCnt].nameend, xmlManager.dmContents_Field[pageNum].contents[j].table.content_colume[content_ColumeCnt].totalColwidth,
                                    xmlManager.dmContents_Field[pageNum].contents[j].table.content_colume[content_ColumeCnt].textAlign, xmlManager.dmContents_Field[pageNum].contents[j].table.content_colume[content_ColumeCnt].colume_txt,
                                    xmlManager.dmContents_Field[pageNum].contents[j].table.content_colume[content_ColumeCnt].isEmpty);


                                contentPage_Field[pageNum].contents[j].table.content_lines[z].child_colume[g].colume_img.color = content_table_back_color;
                                contentPage_Field[pageNum].contents[j].table.content_lines[z].child_colume[g]._text.color = content_table_text_color;
                                contentPage_Field[pageNum].contents[j].table.content_lines[z].child_colume[g]._text.fontSize = content_table_text_size;


                                //테이블 아이템 비율에 맞춰 사이즈 조절 
                                contentPage_Field[pageNum].contents[j].table.content_lines[z].child_colume[g].rec_tr.sizeDelta = new Vector2(
                                    contentPage_Field[pageNum].contents[j].table.content_lines[z].child_colume[g].colume.totalColwidth * tableWidth / xmlManager.dmContents_Field[pageNum].contents[j].table.sumColWidth,
                                    contentPage_Field[pageNum].contents[j].table.content_lines[z].child_colume[g].rec_tr.sizeDelta.y);

                                contentPage_Field[pageNum].contents[j].table.content_lines[z].child_colume[g]._text.text = contentPage_Field[pageNum].contents[j].table.content_lines[z].child_colume[g].colume.colume_txt;

                                if (contentPage_Field[pageNum].contents[j].table.content_lines[z].child_colume[g].colume.textAlign == TextDefine.TEXTALIGN.CENTER)
                                {
                                    contentPage_Field[pageNum].contents[j].table.content_lines[z].child_colume[g]._text.alignment = TMPro.TextAlignmentOptions.Midline;
                                }
                                else if (contentPage_Field[pageNum].contents[j].table.content_lines[z].child_colume[g].colume.textAlign == TextDefine.TEXTALIGN.LEFT)
                                {
                                    contentPage_Field[pageNum].contents[j].table.content_lines[z].child_colume[g]._text.alignment = TMPro.TextAlignmentOptions.MidlineLeft;
                                }
                                else if (contentPage_Field[pageNum].contents[j].table.content_lines[z].child_colume[g].colume.textAlign == TextDefine.TEXTALIGN.RIGHT)
                                {
                                    contentPage_Field[pageNum].contents[j].table.content_lines[z].child_colume[g]._text.alignment = TMPro.TextAlignmentOptions.MidlineRight;
                                }
                                else
                                {
                                    Debug.Log("NO TEXT ALIGN");
                                }

                                if (contentPage_Field[pageNum].contents[j].table.content_lines[z].child_colume[g].colume.isEmpty)
                                {
                                    contentPage_Field[pageNum].contents[j].table.content_lines[z].child_colume[g].gameObject.SetActive(false);
                                    activeCnt_content += 1;
                                }

                                ++content_ColumeCnt;
                            }

                            //테이블 사이즈 간격 통일 
                            for (int g = 0; g < contentPage_Field[pageNum].contents[j].table.content_lines[z].child_colume.Length; g++)
                            {
                                if (contentPage_Field[pageNum].contents[j].table.content_lines[z].child_colume[g].gameObject.activeInHierarchy)
                                {
                                    contentPage_Field[pageNum].contents[j].table.content_lines[z].child_colume[g].rec_tr.sizeDelta = new Vector2(
                                        contentPage_Field[pageNum].contents[j].table.content_lines[z].child_colume[g].rec_tr.sizeDelta.x + activeCnt_content,
                                        contentPage_Field[pageNum].contents[j].table.content_lines[z].child_colume[g].rec_tr.sizeDelta.y);

                                    activeCnt_content = 0;

                                    break;
                                }
                            }
                        }
                    }

                    //다이어그램 
                    else if (xmlManager.dmContents_Field[pageNum].contents[j].data == Data.DIAGRAM)
                    {
                        spacingCheck = false;
                        contentPage_Field[pageNum].contents[j].diagramZone = Instantiate(diagramZone_pre, contentPage_Field[pageNum].page_obj.transform);

                        //isolationStep or isolationProcedureEnd (End는 마지막 결과만 도출된다. 그래서 - 1)
                        contentPage_Field[pageNum].contents[j].diagramSteps = new List<DiagramStep>(); //출력되는데로 출력해야된다.

                        int stepCnt = 0;
                        idCnt = 1;
                        NextStepSetting(pageNum, j, stepCnt, 0); //첫번째 데이터 추가                                         //stepCnt += 1;

                        while (true)
                        {
                            if (xmlManager.dmContents_Field[pageNum].contents[j].diagram[stepCnt].no_answer_id == null) break;

                            for (int z = 0; z < xmlManager.dmContents_Field[pageNum].contents[j].diagram.Count; z++) //갯수당 ID값을 따로 추가 해줘야된다. 
                            {
                                if (xmlManager.dmContents_Field[pageNum].contents[j].diagram[stepCnt].no_answer_id == xmlManager.dmContents_Field[pageNum].contents[j].diagram[z].id)
                                {
                                    stepCnt += 1;
                                    idCnt += 1;
                                    NextStepSetting(pageNum, j, stepCnt, z);

                                    break;
                                }
                            }
                        }
                    }

                    //카탈로그인 경우(모델링)
                    else if (xmlManager.dmContents_Field[pageNum].contents[j].data == Data.CATALOG)
                    {

                        spacingCheck = false;
                        //테이블 값이 들어가면 내용은 null
                        contentPage_Field[pageNum].contents[j].catalog = Instantiate(catalog_pre, contentPage_Field[pageNum].page_obj.transform);

                        //행추가(헤더까지 + 1)
                        contentPage_Field[pageNum].contents[j].catalog.CreateLine(xmlManager.dmContents_Field[pageNum].contents[j].catalog.lineCnt);
                        //각 행 마다 열 개수만큼 컬럼 생성
                        for (int z = 0; z < contentPage_Field[pageNum].contents[j].catalog._lines.Length; z++)
                        {
                            contentPage_Field[pageNum].contents[j].catalog._lines[z].CreateColume(xmlManager.dmContents_Field[pageNum].contents[j].catalog.rowCnt);
                        }

                        //데이터 삽입 (한줄씩 데이터 삽입)
                        for (int z = 0; z < contentPage_Field[pageNum].contents[j].catalog._lines.Length; z++)
                        {
                            //첫번째 카테고리 키값을 가지고 모델링을 생성한다. 
                            if (z == 1 && xmlManager.dmContents_Field[pageNum].contents[j].catalog.catalog_items[z].figureNumber == "01")
                            {
                                //z == 0은 헤더 
                                uiManager.detailSettingCanvas_Field.objectListSetting.ObjectSetting(xmlManager.dmContents_Field[pageNum].contents[j].catalog.catalog_items[z].subSystemCode,
                                   xmlManager.dmContents_Field[pageNum].contents[j].catalog.catalog_items[z].subsubSystemCode,
                                   xmlManager.dmContents_Field[pageNum].contents[j].catalog.catalog_items[z].assyCode, contentPage_Field[pageNum].contents[j].catalog);
                            }

                            //고유의 값 등록
                            contentPage_Field[pageNum].contents[j].catalog._lines[z].assyCode = xmlManager.dmContents_Field[pageNum].contents[j].catalog.catalog_items[z].assyCode;
                            contentPage_Field[pageNum].contents[j].catalog._lines[z].subSystemCode = xmlManager.dmContents_Field[pageNum].contents[j].catalog.catalog_items[z].subSystemCode;
                            contentPage_Field[pageNum].contents[j].catalog._lines[z].subsubSystemCode = xmlManager.dmContents_Field[pageNum].contents[j].catalog.catalog_items[z].subsubSystemCode;
                            contentPage_Field[pageNum].contents[j].catalog._lines[z].figureNumber = xmlManager.dmContents_Field[pageNum].contents[j].catalog.catalog_items[z].figureNumber;
                            contentPage_Field[pageNum].contents[j].catalog._lines[z].id = xmlManager.dmContents_Field[pageNum].contents[j].catalog.catalog_items[z].id;
                            contentPage_Field[pageNum].contents[j].catalog._lines[z].emp = xmlManager.dmContents_Field[pageNum].contents[j].catalog.catalog_items[z].emp;

                            //각 컬럼 사이즈 조정
                            for (int g = 0; g < contentPage_Field[pageNum].contents[j].catalog._lines[z].child_colume.Length; g++)
                            {
                                contentPage_Field[pageNum].contents[j].catalog._lines[z].child_colume[g].rec_tr.sizeDelta = new Vector2(
                                    xmlManager.dmContents_Field[pageNum].contents[j].catalog.item_width[g], contentPage_Field[pageNum].contents[j].catalog._lines[z].child_colume[g].rec_tr.sizeDelta.y);

                                //HEADER
                                if (z == 0)
                                {
                                    contentPage_Field[pageNum].contents[j].catalog._lines[z].child_colume[g]._text.color = header_table_text_color;
                                    contentPage_Field[pageNum].contents[j].catalog._lines[z].child_colume[g]._text.fontSize = header_table_text_size;
                                    contentPage_Field[pageNum].contents[j].catalog._lines[z].child_colume[g].colume_img.color = header_table_back_color;
                                }
                                //CONTENT
                                else
                                {
                                    contentPage_Field[pageNum].contents[j].catalog._lines[z].child_colume[g]._text.color = content_table_text_color;
                                    contentPage_Field[pageNum].contents[j].catalog._lines[z].child_colume[g]._text.fontSize = content_table_text_size;
                                    contentPage_Field[pageNum].contents[j].catalog._lines[z].child_colume[g].colume_img.color = content_table_back_color;
                                }
                            }

                            //테이블 데이터 추가 
                            //계통/그림번호
                            contentPage_Field[pageNum].contents[j].catalog._lines[z].child_colume[0]._text.text =
                                xmlManager.dmContents_Field[pageNum].contents[j].catalog.catalog_items[z].figureNumber;
                            //품목 번호
                            contentPage_Field[pageNum].contents[j].catalog._lines[z].child_colume[1]._text.text =
                                xmlManager.dmContents_Field[pageNum].contents[j].catalog.catalog_items[z].itemSeqNumberValue;


                            //국가재고번호
                            contentPage_Field[pageNum].contents[j].catalog._lines[z].child_colume[3]._text.text = "";
                                //xmlManager.dmContents_Field[pageNum].contents[j].catalog.catalog_items[z].emp;
                            //생산자 부호
                            contentPage_Field[pageNum].contents[j].catalog._lines[z].child_colume[4]._text.text = "";
                                //xmlManager.dmContents_Field[pageNum].contents[j].catalog.catalog_items[z].manufacturerCodeValue;
                            //설명
                            contentPage_Field[pageNum].contents[j].catalog._lines[z].child_colume[5]._text.text = "";
                            //xmlManager.dmContents_Field[pageNum].contents[j].catalog.catalog_items[z].descrForPart;
                            //단위당 구성 수량
                            contentPage_Field[pageNum].contents[j].catalog._lines[z].child_colume[6]._text.text = "";
                                //xmlManager.dmContents_Field[pageNum].contents[j].catalog.catalog_items[z].quantityPerNextHigherAssy;
                            //근원정비 복구성 부호 
                            contentPage_Field[pageNum].contents[j].catalog._lines[z].child_colume[7]._text.text = "";
                                //xmlManager.dmContents_Field[pageNum].contents[j].catalog.catalog_items[z].sourceMaintRecoverability;

                            //header, 몸체인 경우 
                            //|| contentPage_Field[pageNum].contents[j].catalog._lines[z].emp == "9999-99-999-9999"
                            if (contentPage_Field[pageNum].contents[j].catalog._lines[z].subSystemCode == "")
                            {
                                contentPage_Field[pageNum].contents[j].catalog._lines[z].btn.interactable = false;
                                if (contentPage_Field[pageNum].contents[j].catalog._lines[z].btn.GetComponent<ButtonTouchHandler>())
                                    contentPage_Field[pageNum].contents[j].catalog._lines[z].btn.GetComponent<ButtonTouchHandler>().enabled = false;

                                if (contentPage_Field[pageNum].contents[j].catalog._lines[z].btn.GetComponent<Synchronizer>())
                                    contentPage_Field[pageNum].contents[j].catalog._lines[z].btn.GetComponent<Synchronizer>().enabled = false;

                                if (contentPage_Field[pageNum].contents[j].catalog._lines[z].btn.GetComponent<EventTrigger>())
                                    contentPage_Field[pageNum].contents[j].catalog._lines[z].btn.GetComponent<EventTrigger>().enabled = false;

                                //contentPage_Field[pageNum].contents[j].catalog._lines[z].btn.GetComponent<CatalogLineSetting>().btn_img.color = content_table_back_color;
                            }
                            //라인마다 버튼 이벤트를 통해 모델링 강조 효과 추가
                            else
                            {
                                string assyCode = contentPage_Field[pageNum].contents[j].catalog._lines[z].assyCode;
                                string subSystemCode = contentPage_Field[pageNum].contents[j].catalog._lines[z].subSystemCode;
                                string subsubSystemCode = contentPage_Field[pageNum].contents[j].catalog._lines[z].subsubSystemCode;
                                string figureNumber = contentPage_Field[pageNum].contents[j].catalog._lines[z].figureNumber;
                                string id = contentPage_Field[pageNum].contents[j].catalog._lines[z].id;

                                CatalogInfo tmp = contentPage_Field[pageNum].contents[j].catalog;
                                int cnt;
                                cnt = z;

                                //부품 강조 이벤트 등록
                                contentPage_Field[pageNum].contents[j].catalog._lines[z].btn.onClick.AddListener(() =>
                                {
                                        tmp.ChildFunCheck(cnt); //클린된 얘만 활성화 터치 불가능 

                                        uiManager.detailSettingCanvas_Field.objectListSetting.ObjectController(subSystemCode, subsubSystemCode, figureNumber, assyCode, id);
                                });
                            }
                        }

                    }
                    else { };
                }
            }
            else
            {
                Debug.Log("NOT FOUND DM DATA : " + pageNum);
            }
        }

        else if (oper == Operation.Unit) //부대정비 
        {
            if (xmlManager.dmContents_Unit[pageNum].contents != null)
            {
                contentPage_Unit[pageNum].page_obj.gameObject.SetActive(true);

                Debug.Log(xmlManager.dmContents_Unit[pageNum].OwnDM.name);

                //한페이지 내용들
                contentPage_Unit[pageNum].contents = new Content[xmlManager.dmContents_Unit[pageNum].contents.Count];

                //Figure 제어가 필요한지 체크 함수 
                FigureControlCheck(pageNum);

                //contents 하나당 텍스트 하나 또는 테이블 하나 
                for (int j = 0; j < contentPage_Unit[pageNum].contents.Length; j++)
                {
                    contentPage_Unit[pageNum].contents[j].data = xmlManager.dmContents_Unit[pageNum].contents[j].data;
                    contentPage_Unit[pageNum].contents[j].isTitle = xmlManager.dmContents_Unit[pageNum].contents[j].isTitle;

                    //빈공백일때 
                    if (xmlManager.dmContents_Unit[pageNum].contents[j].data == Data.SPACING && !spacingCheck)
                    {
                        spacingCheck = true;
                        contentPage_Unit[pageNum].contents[j].spacing_obj = Instantiate(spacing_pre, contentPage_Unit[pageNum].page_obj.transform);
                    }

                    //text/warning text 인 경우 
                    else if (xmlManager.dmContents_Unit[pageNum].contents[j].data == Data.TEXT)
                    {
                        spacingCheck = false;
                        if (xmlManager.dmContents_Unit[pageNum].contents[j].texttype == TEXTTYPE.WARNING)
                        {
                            contentPage_Unit[pageNum].contents[j].dmItem_warning_str = Instantiate(content_txt_warning_pre, contentPage_Unit[pageNum].page_obj.transform);
                            contentPage_Unit[pageNum].contents[j].dmItem_warning_str.content_txt.text = xmlManager.dmContents_Unit[pageNum].contents[j].dmItem_text;
                            contentPage_Unit[pageNum].contents[j].dmItem_warning_str.content_txt.fontSize = xmlManager.dmContents_Unit[pageNum].contents[j].textSize;
                            contentPage_Unit[pageNum].contents[j].dmItem_warning_str.content_txt.color = xmlManager.dmContents_Unit[pageNum].contents[j].textColor;

                            if (xmlManager.dmContents_Unit[pageNum].contents[j].textAlign == TextDefine.TEXTALIGN.CENTER)
                            {
                                contentPage_Unit[pageNum].contents[j].dmItem_warning_str.content_txt.alignment = TMPro.TextAlignmentOptions.Midline;
                            }
                            else if (xmlManager.dmContents_Unit[pageNum].contents[j].textAlign == TextDefine.TEXTALIGN.LEFT)
                            {
                                contentPage_Unit[pageNum].contents[j].dmItem_warning_str.content_txt.alignment = TMPro.TextAlignmentOptions.MidlineLeft;
                            }
                            else if (xmlManager.dmContents_Unit[pageNum].contents[j].textAlign == TextDefine.TEXTALIGN.RIGHT)
                            {
                                contentPage_Unit[pageNum].contents[j].dmItem_warning_str.content_txt.alignment = TMPro.TextAlignmentOptions.MidlineRight;
                            }
                            else
                            {
                                Debug.Log("NO TEXT ALIGN");
                            }

                            if (xmlManager.dmContents_Unit[pageNum].contents[j].textStyle == TextDefine.TEXTSTYLE.NORMAL)
                            {
                                contentPage_Unit[pageNum].contents[j].dmItem_warning_str.content_txt.font = normalFont;
                            }
                            else if (xmlManager.dmContents_Unit[pageNum].contents[j].textStyle == TextDefine.TEXTSTYLE.BOLD)
                            {
                                contentPage_Unit[pageNum].contents[j].dmItem_warning_str.content_txt.font = boldFont;
                            }
                            else if (xmlManager.dmContents_Unit[pageNum].contents[j].textStyle == TextDefine.TEXTSTYLE.MEDIUM)
                            {
                                contentPage_Unit[pageNum].contents[j].dmItem_warning_str.content_txt.font = mediumFont;
                            }
                            else
                            {
                                Debug.Log("NO TEXT STYLE");
                            }
                        }
                        else if (xmlManager.dmContents_Unit[pageNum].contents[j].texttype == TEXTTYPE.CAUTION)
                        {
                            contentPage_Unit[pageNum].contents[j].dmItem_caution_str = Instantiate(content_txt_caution_pre, contentPage_Unit[pageNum].page_obj.transform);
                            contentPage_Unit[pageNum].contents[j].dmItem_caution_str.content_txt.text = xmlManager.dmContents_Unit[pageNum].contents[j].dmItem_text;
                            contentPage_Unit[pageNum].contents[j].dmItem_caution_str.content_txt.fontSize = xmlManager.dmContents_Unit[pageNum].contents[j].textSize;
                            contentPage_Unit[pageNum].contents[j].dmItem_caution_str.content_txt.color = xmlManager.dmContents_Unit[pageNum].contents[j].textColor;

                            if (xmlManager.dmContents_Unit[pageNum].contents[j].textAlign == TextDefine.TEXTALIGN.CENTER)
                            {
                                contentPage_Unit[pageNum].contents[j].dmItem_caution_str.content_txt.alignment = TMPro.TextAlignmentOptions.Midline;
                            }
                            else if (xmlManager.dmContents_Unit[pageNum].contents[j].textAlign == TextDefine.TEXTALIGN.LEFT)
                            {
                                contentPage_Unit[pageNum].contents[j].dmItem_caution_str.content_txt.alignment = TMPro.TextAlignmentOptions.MidlineLeft;
                            }
                            else if (xmlManager.dmContents_Unit[pageNum].contents[j].textAlign == TextDefine.TEXTALIGN.RIGHT)
                            {
                                contentPage_Unit[pageNum].contents[j].dmItem_caution_str.content_txt.alignment = TMPro.TextAlignmentOptions.MidlineRight;
                            }
                            else
                            {
                                Debug.Log("NO TEXT ALIGN");
                            }

                            if (xmlManager.dmContents_Unit[pageNum].contents[j].textStyle == TextDefine.TEXTSTYLE.NORMAL)
                            {
                                contentPage_Unit[pageNum].contents[j].dmItem_caution_str.content_txt.font = normalFont;
                            }
                            else if (xmlManager.dmContents_Unit[pageNum].contents[j].textStyle == TextDefine.TEXTSTYLE.BOLD)
                            {
                                contentPage_Unit[pageNum].contents[j].dmItem_caution_str.content_txt.font = boldFont;
                            }
                            else if (xmlManager.dmContents_Unit[pageNum].contents[j].textStyle == TextDefine.TEXTSTYLE.MEDIUM)
                            {
                                contentPage_Unit[pageNum].contents[j].dmItem_caution_str.content_txt.font = mediumFont;
                            }
                            else
                            {
                                Debug.Log("NO TEXT STYLE");
                            }
                        }
                        else if (xmlManager.dmContents_Unit[pageNum].contents[j].texttype == TEXTTYPE.NOTE)
                        {
                            contentPage_Unit[pageNum].contents[j].dmItem_note_str = Instantiate(content_txt_note_pre, contentPage_Unit[pageNum].page_obj.transform);
                            contentPage_Unit[pageNum].contents[j].dmItem_note_str.content_txt.text = xmlManager.dmContents_Unit[pageNum].contents[j].dmItem_text;
                            contentPage_Unit[pageNum].contents[j].dmItem_note_str.content_txt.fontSize = xmlManager.dmContents_Unit[pageNum].contents[j].textSize;
                            contentPage_Unit[pageNum].contents[j].dmItem_note_str.content_txt.color = xmlManager.dmContents_Unit[pageNum].contents[j].textColor;

                            if (xmlManager.dmContents_Unit[pageNum].contents[j].textAlign == TextDefine.TEXTALIGN.CENTER)
                            {
                                contentPage_Unit[pageNum].contents[j].dmItem_note_str.content_txt.alignment = TMPro.TextAlignmentOptions.Midline;
                            }
                            else if (xmlManager.dmContents_Unit[pageNum].contents[j].textAlign == TextDefine.TEXTALIGN.LEFT)
                            {
                                contentPage_Unit[pageNum].contents[j].dmItem_note_str.content_txt.alignment = TMPro.TextAlignmentOptions.MidlineLeft;
                            }
                            else if (xmlManager.dmContents_Unit[pageNum].contents[j].textAlign == TextDefine.TEXTALIGN.RIGHT)
                            {
                                contentPage_Unit[pageNum].contents[j].dmItem_note_str.content_txt.alignment = TMPro.TextAlignmentOptions.MidlineRight;
                            }
                            else
                            {
                                Debug.Log("NO TEXT ALIGN");
                            }

                            if (xmlManager.dmContents_Unit[pageNum].contents[j].textStyle == TextDefine.TEXTSTYLE.NORMAL)
                            {
                                contentPage_Unit[pageNum].contents[j].dmItem_note_str.content_txt.font = normalFont;
                            }
                            else if (xmlManager.dmContents_Unit[pageNum].contents[j].textStyle == TextDefine.TEXTSTYLE.BOLD)
                            {
                                contentPage_Unit[pageNum].contents[j].dmItem_note_str.content_txt.font = boldFont;
                            }
                            else if (xmlManager.dmContents_Unit[pageNum].contents[j].textStyle == TextDefine.TEXTSTYLE.MEDIUM)
                            {
                                contentPage_Unit[pageNum].contents[j].dmItem_note_str.content_txt.font = mediumFont;
                            }
                            else
                            {
                                Debug.Log("NO TEXT STYLE");
                            }
                        }
                        else if (xmlManager.dmContents_Unit[pageNum].contents[j].texttype == TEXTTYPE.TEXT)
                        {
                            contentPage_Unit[pageNum].contents[j].dmItem_str = Instantiate(content_txt_pre, contentPage_Unit[pageNum].page_obj.transform);
                            contentPage_Unit[pageNum].contents[j].dmItem_str.content_txt.text = xmlManager.dmContents_Unit[pageNum].contents[j].dmItem_text;
                            contentPage_Unit[pageNum].contents[j].dmItem_str.content_txt.fontSize = xmlManager.dmContents_Unit[pageNum].contents[j].textSize;
                            contentPage_Unit[pageNum].contents[j].dmItem_str.content_txt.color = xmlManager.dmContents_Unit[pageNum].contents[j].textColor;

                            if (xmlManager.dmContents_Unit[pageNum].contents[j].textAlign == TextDefine.TEXTALIGN.CENTER)
                            {
                                contentPage_Unit[pageNum].contents[j].dmItem_str.content_txt.alignment = TMPro.TextAlignmentOptions.Midline;
                            }
                            else if (xmlManager.dmContents_Unit[pageNum].contents[j].textAlign == TextDefine.TEXTALIGN.LEFT)
                            {
                                contentPage_Unit[pageNum].contents[j].dmItem_str.content_txt.alignment = TMPro.TextAlignmentOptions.MidlineLeft;
                            }
                            else if (xmlManager.dmContents_Unit[pageNum].contents[j].textAlign == TextDefine.TEXTALIGN.RIGHT)
                            {
                                contentPage_Unit[pageNum].contents[j].dmItem_str.content_txt.alignment = TMPro.TextAlignmentOptions.MidlineRight;
                            }
                            else
                            {
                                Debug.Log("NO TEXT ALIGN");
                            }

                            if (xmlManager.dmContents_Unit[pageNum].contents[j].textStyle == TextDefine.TEXTSTYLE.NORMAL)
                            {
                                contentPage_Unit[pageNum].contents[j].dmItem_str.content_txt.font = normalFont;
                            }
                            else if (xmlManager.dmContents_Unit[pageNum].contents[j].textStyle == TextDefine.TEXTSTYLE.BOLD)
                            {
                                contentPage_Unit[pageNum].contents[j].dmItem_str.content_txt.font = boldFont;
                            }
                            else if (xmlManager.dmContents_Unit[pageNum].contents[j].textStyle == TextDefine.TEXTSTYLE.MEDIUM)
                            {
                                contentPage_Unit[pageNum].contents[j].dmItem_str.content_txt.font = mediumFont;
                            }
                            else
                            {
                                Debug.Log("NO TEXT STYLE");
                            }
                        }
                    }

                    //FolderContent 인 경우 
                    else if (xmlManager.dmContents_Unit[pageNum].contents[j].data == Data.FOLDERCONTENT)
                    {
                        spacingCheck = false;
                        //header 
                        contentPage_Unit[pageNum].contents[j].folderContent = Instantiate(folderContent_pre, contentPage_Unit[pageNum].page_obj.transform);

                        contentPage_Unit[pageNum].contents[j].folderContent.header_txt.text = xmlManager.dmContents_Unit[pageNum].contents[j].dmItem_text;

                        contentPage_Unit[pageNum].contents[j].folderContent.header_txt.fontSize = xmlManager.dmContents_Unit[pageNum].contents[j].textSize;
                        contentPage_Unit[pageNum].contents[j].folderContent.header_txt.color = xmlManager.dmContents_Unit[pageNum].contents[j].textColor;
                    }

                    //Figure 인 경우 (이미지 또는 객체) - 버튼 형식 
                    else if (xmlManager.dmContents_Unit[pageNum].contents[j].data == Data.FIGURE && !figureControl)
                    {
                        spacingCheck = false;
                        //header 
                        contentPage_Unit[pageNum].contents[j].figureButton = Instantiate(figureButton_pre, contentPage_Unit[pageNum].page_obj.transform);

                        contentPage_Unit[pageNum].contents[j].figureButton.button_txt.text = "<size=20>그림 " + figureCnt + ". </size>" +
                            xmlManager.dmContents_Unit[pageNum].contents[j].figure.title + " 보기";
                        figureCnt++;

                        contentPage_Unit[pageNum].contents[j].figureButton.id = xmlManager.dmContents_Unit[pageNum].contents[j].figure.id;
                        contentPage_Unit[pageNum].contents[j].figureButton.info = xmlManager.dmContents_Unit[pageNum].contents[j].figure.infoEntityIdent;

                        string id = contentPage_Unit[pageNum].contents[j].figureButton.id;
                        string info = contentPage_Unit[pageNum].contents[j].figureButton.info;
                        int tmp = pageNum;
                        int tmp_1 = j;

                        //버튼 이벤트 등록
                        contentPage_Unit[pageNum].contents[j].figureButton.btn.onClick.AddListener(() =>
                        {
                            uiManager.detailSettingCanvas_Unit.objectListSetting.FigureSetting(id, info);

                            FigureButtonController(tmp, tmp_1);
                        });
                    }

                    //정비 절차 버튼인 경우 
                    else if (xmlManager.dmContents_Unit[pageNum].contents[j].data == Data.OPERATION)
                    {
                        spacingCheck = false;
                        contentPage_Unit[pageNum].contents[j].operBtn = Instantiate(operBtn_pre, contentPage_Unit[pageNum].page_obj.transform);

                        //OperationItems 판단 
                        for (int i = 0; i < xmlManager.dmContents_Unit[pageNum].contents[j].operBtn.colume.Count; i++)
                        {
                            if (xmlManager.dmContents_Unit[pageNum].contents[j].operBtn.colume[i].OperationItem_type == OperationItems.OPERATION)
                            {
                                contentPage_Unit[pageNum].contents[j].operBtn.CreateOperationItems(OperationItems.OPERATION);
                            }
                            else if (xmlManager.dmContents_Unit[pageNum].contents[j].operBtn.colume[i].OperationItem_type == OperationItems.WARNING)
                            {
                                contentPage_Unit[pageNum].contents[j].operBtn.CreateOperationItems(OperationItems.WARNING);
                            }
                            else if (xmlManager.dmContents_Unit[pageNum].contents[j].operBtn.colume[i].OperationItem_type == OperationItems.CAUTION)
                            {
                                contentPage_Unit[pageNum].contents[j].operBtn.CreateOperationItems(OperationItems.CAUTION);
                            }
                            else if (xmlManager.dmContents_Unit[pageNum].contents[j].operBtn.colume[i].OperationItem_type == OperationItems.NOTE)
                            {
                                contentPage_Unit[pageNum].contents[j].operBtn.CreateOperationItems(OperationItems.NOTE);
                            }

                            //타입 등록 
                            contentPage_Unit[pageNum].contents[j].operBtn.op_btn[i].type = xmlManager.dmContents_Unit[pageNum].contents[j].operBtn.colume[i].OperationItem_type;
                        }

                        contentPage_Unit[pageNum].contents[j].operBtn.title = xmlManager.dmContents_Unit[pageNum].contents[j].operBtn.operTitle;
                        contentPage_Unit[pageNum].contents[j].operBtn.subtitle = xmlManager.dmContents_Unit[pageNum].contents[j].operBtn.operSubTitle;

                        string title = xmlManager.dmContents_Unit[pageNum].contents[j].operBtn.operTitle;
                        string subTitle = xmlManager.dmContents_Unit[pageNum].contents[j].operBtn.operSubTitle;
                        OperationSetting operSetting = contentPage_Unit[pageNum].contents[j].operBtn;

                        //Debug.Log("Operation Title : " + title + " / Operation SubTitle : " + subTitle);
                        uiManager.detailSettingCanvas_Unit.objectListSetting.OperationReady(title, subTitle, operSetting); //모델링 활성화 

                        int btnstep = 0;
                        for (int z = 0; z < contentPage_Unit[pageNum].contents[j].operBtn.op_btn.Count; z++)
                        {
                            if (contentPage_Unit[pageNum].contents[j].operBtn.op_btn[z].type == OperationItems.OPERATION)
                            {
                                contentPage_Unit[pageNum].contents[j].operBtn.op_btn[z].stepindex = z;

                                contentPage_Unit[pageNum].contents[j].operBtn.op_btn[z]._text.text = xmlManager.dmContents_Unit[pageNum].contents[j].operBtn.colume[z].operationItem_text;

                                int step;
                                step = btnstep;
                                btnstep += 1;

                                //클릭 이벤트 추가 
                                contentPage_Unit[pageNum].contents[j].operBtn.op_btn[z].btn.onClick.AddListener(() =>
                                {
                                    operSetting.ChildFunCheck(step);
                                    uiManager.detailSettingCanvas_Unit.objectListSetting.OperationButtonClick(title, subTitle, step);
                                });
                            }
                            else if (contentPage_Unit[pageNum].contents[j].operBtn.op_btn[z].type == OperationItems.WARNING
                                || contentPage_Unit[pageNum].contents[j].operBtn.op_btn[z].type == OperationItems.CAUTION
                                || contentPage_Unit[pageNum].contents[j].operBtn.op_btn[z].type == OperationItems.NOTE)
                            {
                                contentPage_Unit[pageNum].contents[j].operBtn.op_btn[z]._text.text = xmlManager.dmContents_Unit[pageNum].contents[j].operBtn.colume[z].operationItem_text;
                                contentPage_Unit[pageNum].contents[j].operBtn.op_btn[z]._text.fontSize = xmlManager.dmContents_Unit[pageNum].contents[j].operBtn.colume[z].textSize;
                                contentPage_Unit[pageNum].contents[j].operBtn.op_btn[z]._text.color = xmlManager.dmContents_Unit[pageNum].contents[j].operBtn.colume[z].textColor;

                                if (xmlManager.dmContents_Unit[pageNum].contents[j].operBtn.colume[z].textAlign == TextDefine.TEXTALIGN.CENTER)
                                {
                                    contentPage_Unit[pageNum].contents[j].operBtn.op_btn[z]._text.alignment = TMPro.TextAlignmentOptions.Midline;
                                }
                                else if (xmlManager.dmContents_Unit[pageNum].contents[j].operBtn.colume[z].textAlign == TextDefine.TEXTALIGN.LEFT)
                                {
                                    contentPage_Unit[pageNum].contents[j].operBtn.op_btn[z]._text.alignment = TMPro.TextAlignmentOptions.MidlineLeft;
                                }
                                else if (xmlManager.dmContents_Unit[pageNum].contents[j].operBtn.colume[z].textAlign == TextDefine.TEXTALIGN.RIGHT)
                                {
                                    contentPage_Unit[pageNum].contents[j].operBtn.op_btn[z]._text.alignment = TMPro.TextAlignmentOptions.MidlineRight;
                                }
                                else
                                {
                                    Debug.Log("NO TEXT ALIGN");
                                }

                                if (xmlManager.dmContents_Unit[pageNum].contents[j].operBtn.colume[z].textStyle == TextDefine.TEXTSTYLE.NORMAL)
                                {
                                    contentPage_Unit[pageNum].contents[j].operBtn.op_btn[z]._text.font = normalFont;
                                }
                                else if (xmlManager.dmContents_Unit[pageNum].contents[j].operBtn.colume[z].textStyle == TextDefine.TEXTSTYLE.BOLD)
                                {
                                    contentPage_Unit[pageNum].contents[j].operBtn.op_btn[z]._text.font = boldFont;
                                }
                                else if (xmlManager.dmContents_Unit[pageNum].contents[j].operBtn.colume[z].textStyle == TextDefine.TEXTSTYLE.MEDIUM)
                                {
                                    contentPage_Unit[pageNum].contents[j].operBtn.op_btn[z]._text.font = mediumFont;
                                }
                                else
                                {
                                    Debug.Log("NO TEXT STYLE");
                                }
                            }
                        }
                    }

                    //테이블인 경우 
                    else if (xmlManager.dmContents_Unit[pageNum].contents[j].data == Data.TABLE) //널이 아닐때 체크하는게 좀 문제있는듯?
                    {
                        spacingCheck = false;
                        //테이블 값이 들어가면 내용은 null
                        contentPage_Unit[pageNum].contents[j].table = Instantiate(table_pre, contentPage_Unit[pageNum].page_obj.transform);

                        //COLSPEC
                        for (int z = 0; z < xmlManager.dmContents_Unit[pageNum].contents[j].table.colspec.Count; z++)
                        {
                            contentPage_Unit[pageNum].contents[j].table.colspec.Add(xmlManager.dmContents_Unit[pageNum].contents[j].table.colspec[z]);
                        }

                        //COLUME(Header)
                        contentPage_Unit[pageNum].contents[j].table.CreateHeaderLine(xmlManager.dmContents_Unit[pageNum].contents[j].table.header_lineCnt);
                        for (int z = 0; z < xmlManager.dmContents_Unit[pageNum].contents[j].table.header_lineCnt; z++)
                        {
                            contentPage_Unit[pageNum].contents[j].table.header_lines[z].CreateColume(contentPage_Unit[pageNum].contents[j].table.colspec.Count);
                        }

                        int header_ColumeCnt = 0;
                        int activeCnt_header = 0; //비활성화된 칼럼이 몇개인지 파악 (사이즈를 맞추기위해)
                                                  //Header Data Setting
                        for (int z = 0; z < contentPage_Unit[pageNum].contents[j].table.header_lines.Length; z++)
                        {
                            for (int g = 0; g < contentPage_Unit[pageNum].contents[j].table.header_lines[z].child_colume.Length; g++)
                            {
                                contentPage_Unit[pageNum].contents[j].table.header_lines[z].child_colume[g].colume = new DMItem_Table_Colume(
                                    xmlManager.dmContents_Unit[pageNum].contents[j].table.header_colume[header_ColumeCnt].morerows, xmlManager.dmContents_Unit[pageNum].contents[j].table.header_colume[header_ColumeCnt].namest,
                                    xmlManager.dmContents_Unit[pageNum].contents[j].table.header_colume[header_ColumeCnt].nameend, xmlManager.dmContents_Unit[pageNum].contents[j].table.header_colume[header_ColumeCnt].totalColwidth,
                                    xmlManager.dmContents_Unit[pageNum].contents[j].table.header_colume[header_ColumeCnt].textAlign, xmlManager.dmContents_Unit[pageNum].contents[j].table.header_colume[header_ColumeCnt].colume_txt,
                                    xmlManager.dmContents_Unit[pageNum].contents[j].table.header_colume[header_ColumeCnt].isEmpty);


                                contentPage_Unit[pageNum].contents[j].table.header_lines[z].child_colume[g].colume_img.color = header_table_back_color;
                                contentPage_Unit[pageNum].contents[j].table.header_lines[z].child_colume[g]._text.color = header_table_text_color;
                                contentPage_Unit[pageNum].contents[j].table.header_lines[z].child_colume[g]._text.fontSize = header_table_text_size;


                                //테이블 아이템 비율에 맞춰 사이즈 조절 
                                contentPage_Unit[pageNum].contents[j].table.header_lines[z].child_colume[g].rec_tr.sizeDelta = new Vector2(
                                    contentPage_Unit[pageNum].contents[j].table.header_lines[z].child_colume[g].colume.totalColwidth * tableWidth / xmlManager.dmContents_Unit[pageNum].contents[j].table.sumColWidth,
                                    contentPage_Unit[pageNum].contents[j].table.header_lines[z].child_colume[g].rec_tr.sizeDelta.y);

                                contentPage_Unit[pageNum].contents[j].table.header_lines[z].child_colume[g]._text.text = contentPage_Unit[pageNum].contents[j].table.header_lines[z].child_colume[g].colume.colume_txt;

                                if (contentPage_Unit[pageNum].contents[j].table.header_lines[z].child_colume[g].colume.textAlign == TextDefine.TEXTALIGN.CENTER)
                                {
                                    contentPage_Unit[pageNum].contents[j].table.header_lines[z].child_colume[g]._text.alignment = TMPro.TextAlignmentOptions.Midline;
                                }
                                else if (contentPage_Unit[pageNum].contents[j].table.header_lines[z].child_colume[g].colume.textAlign == TextDefine.TEXTALIGN.LEFT)
                                {
                                    contentPage_Unit[pageNum].contents[j].table.header_lines[z].child_colume[g]._text.alignment = TMPro.TextAlignmentOptions.MidlineLeft;
                                }
                                else if (contentPage_Unit[pageNum].contents[j].table.header_lines[z].child_colume[g].colume.textAlign == TextDefine.TEXTALIGN.RIGHT)
                                {
                                    contentPage_Unit[pageNum].contents[j].table.header_lines[z].child_colume[g]._text.alignment = TMPro.TextAlignmentOptions.MidlineRight;
                                }
                                else
                                {
                                    Debug.Log("NO TEXT ALIGN");
                                }

                                if (contentPage_Unit[pageNum].contents[j].table.header_lines[z].child_colume[g].colume.isEmpty)
                                {
                                    contentPage_Unit[pageNum].contents[j].table.header_lines[z].child_colume[g].gameObject.SetActive(false);
                                    activeCnt_header += 1;
                                }

                                ++header_ColumeCnt;
                            }

                            //테이블 사이즈 간격 통일 
                            for (int g = 0; g < contentPage_Unit[pageNum].contents[j].table.header_lines[z].child_colume.Length; g++)
                            {
                                if (contentPage_Unit[pageNum].contents[j].table.header_lines[z].child_colume[g].gameObject.activeInHierarchy)
                                {
                                    contentPage_Unit[pageNum].contents[j].table.header_lines[z].child_colume[g].rec_tr.sizeDelta = new Vector2(
                                        contentPage_Unit[pageNum].contents[j].table.header_lines[z].child_colume[g].rec_tr.sizeDelta.x + activeCnt_header,
                                        contentPage_Unit[pageNum].contents[j].table.header_lines[z].child_colume[g].rec_tr.sizeDelta.y);

                                    activeCnt_header = 0;

                                    break;
                                }
                            }
                        }

                        //int 
                        //COLUME(Content)
                        contentPage_Unit[pageNum].contents[j].table.CreateContentLine(xmlManager.dmContents_Unit[pageNum].contents[j].table.content_lineCnt);
                        for (int z = 0; z < xmlManager.dmContents_Unit[pageNum].contents[j].table.content_lineCnt; z++)
                        {
                            contentPage_Unit[pageNum].contents[j].table.content_lines[z].CreateColume(contentPage_Unit[pageNum].contents[j].table.colspec.Count);
                        }

                        int content_ColumeCnt = 0;
                        int activeCnt_content = 0; //비활성화된 칼럼이 몇개인지 파악 (사이즈를 맞추기위해)
                                                   //Content Data Setting
                        for (int z = 0; z < contentPage_Unit[pageNum].contents[j].table.content_lines.Length; z++)
                        {
                            for (int g = 0; g < contentPage_Unit[pageNum].contents[j].table.content_lines[z].child_colume.Length; g++)
                            {
                                contentPage_Unit[pageNum].contents[j].table.content_lines[z].child_colume[g].colume = new DMItem_Table_Colume(
                                    xmlManager.dmContents_Unit[pageNum].contents[j].table.content_colume[content_ColumeCnt].morerows, xmlManager.dmContents_Unit[pageNum].contents[j].table.content_colume[content_ColumeCnt].namest,
                                    xmlManager.dmContents_Unit[pageNum].contents[j].table.content_colume[content_ColumeCnt].nameend, xmlManager.dmContents_Unit[pageNum].contents[j].table.content_colume[content_ColumeCnt].totalColwidth,
                                    xmlManager.dmContents_Unit[pageNum].contents[j].table.content_colume[content_ColumeCnt].textAlign, xmlManager.dmContents_Unit[pageNum].contents[j].table.content_colume[content_ColumeCnt].colume_txt,
                                    xmlManager.dmContents_Unit[pageNum].contents[j].table.content_colume[content_ColumeCnt].isEmpty);


                                contentPage_Unit[pageNum].contents[j].table.content_lines[z].child_colume[g].colume_img.color = content_table_back_color;
                                contentPage_Unit[pageNum].contents[j].table.content_lines[z].child_colume[g]._text.color = content_table_text_color;
                                contentPage_Unit[pageNum].contents[j].table.content_lines[z].child_colume[g]._text.fontSize = content_table_text_size;


                                //테이블 아이템 비율에 맞춰 사이즈 조절 
                                contentPage_Unit[pageNum].contents[j].table.content_lines[z].child_colume[g].rec_tr.sizeDelta = new Vector2(
                                    contentPage_Unit[pageNum].contents[j].table.content_lines[z].child_colume[g].colume.totalColwidth * tableWidth / xmlManager.dmContents_Unit[pageNum].contents[j].table.sumColWidth,
                                    contentPage_Unit[pageNum].contents[j].table.content_lines[z].child_colume[g].rec_tr.sizeDelta.y);

                                contentPage_Unit[pageNum].contents[j].table.content_lines[z].child_colume[g]._text.text = contentPage_Unit[pageNum].contents[j].table.content_lines[z].child_colume[g].colume.colume_txt;

                                if (contentPage_Unit[pageNum].contents[j].table.content_lines[z].child_colume[g].colume.textAlign == TextDefine.TEXTALIGN.CENTER)
                                {
                                    contentPage_Unit[pageNum].contents[j].table.content_lines[z].child_colume[g]._text.alignment = TMPro.TextAlignmentOptions.Midline;
                                }
                                else if (contentPage_Unit[pageNum].contents[j].table.content_lines[z].child_colume[g].colume.textAlign == TextDefine.TEXTALIGN.LEFT)
                                {
                                    contentPage_Unit[pageNum].contents[j].table.content_lines[z].child_colume[g]._text.alignment = TMPro.TextAlignmentOptions.MidlineLeft;
                                }
                                else if (contentPage_Unit[pageNum].contents[j].table.content_lines[z].child_colume[g].colume.textAlign == TextDefine.TEXTALIGN.RIGHT)
                                {
                                    contentPage_Unit[pageNum].contents[j].table.content_lines[z].child_colume[g]._text.alignment = TMPro.TextAlignmentOptions.MidlineRight;
                                }
                                else
                                {
                                    Debug.Log("NO TEXT ALIGN");
                                }

                                if (contentPage_Unit[pageNum].contents[j].table.content_lines[z].child_colume[g].colume.isEmpty)
                                {
                                    contentPage_Unit[pageNum].contents[j].table.content_lines[z].child_colume[g].gameObject.SetActive(false);
                                    activeCnt_content += 1;
                                }

                                ++content_ColumeCnt;
                            }

                            //테이블 사이즈 간격 통일 
                            for (int g = 0; g < contentPage_Unit[pageNum].contents[j].table.content_lines[z].child_colume.Length; g++)
                            {
                                if (contentPage_Unit[pageNum].contents[j].table.content_lines[z].child_colume[g].gameObject.activeInHierarchy)
                                {
                                    contentPage_Unit[pageNum].contents[j].table.content_lines[z].child_colume[g].rec_tr.sizeDelta = new Vector2(
                                        contentPage_Unit[pageNum].contents[j].table.content_lines[z].child_colume[g].rec_tr.sizeDelta.x + activeCnt_content,
                                        contentPage_Unit[pageNum].contents[j].table.content_lines[z].child_colume[g].rec_tr.sizeDelta.y);

                                    activeCnt_content = 0;

                                    break;
                                }
                            }
                        }
                    }

                    //다이어그램 
                    else if (xmlManager.dmContents_Unit[pageNum].contents[j].data == Data.DIAGRAM)
                    {
                        spacingCheck = false;
                        contentPage_Unit[pageNum].contents[j].diagramZone = Instantiate(diagramZone_pre, contentPage_Unit[pageNum].page_obj.transform);

                        //isolationStep or isolationProcedureEnd (End는 마지막 결과만 도출된다. 그래서 - 1)
                        contentPage_Unit[pageNum].contents[j].diagramSteps = new List<DiagramStep>(); //출력되는데로 출력해야된다.

                        int stepCnt = 0;
                        idCnt = 1;
                        NextStepSetting(pageNum, j, stepCnt, 0); //첫번째 데이터 추가 
                                                                 //stepCnt += 1;

                        while (true)
                        {
                            if (xmlManager.dmContents_Unit[pageNum].contents[j].diagram[stepCnt].no_answer_id == null) break;

                            for (int z = 0; z < xmlManager.dmContents_Unit[pageNum].contents[j].diagram.Count; z++)
                            {
                                if (xmlManager.dmContents_Unit[pageNum].contents[j].diagram[stepCnt].no_answer_id == xmlManager.dmContents_Unit[pageNum].contents[j].diagram[z].id)
                                {
                                    stepCnt += 1;
                                    idCnt += 1;
                                    NextStepSetting(pageNum, j, stepCnt, z);

                                    break;
                                }
                            }
                        }
                    }

                    //카탈로그인 경우(모델링)
                    else if (xmlManager.dmContents_Unit[pageNum].contents[j].data == Data.CATALOG)
                    {
                        spacingCheck = false;
                        //테이블 값이 들어가면 내용은 null
                        contentPage_Unit[pageNum].contents[j].catalog = Instantiate(catalog_pre, contentPage_Unit[pageNum].page_obj.transform);

                        //행추가(헤더까지 + 1)
                        contentPage_Unit[pageNum].contents[j].catalog.CreateLine(xmlManager.dmContents_Unit[pageNum].contents[j].catalog.lineCnt);
                        //각 행 마다 열 개수만큼 컬럼 생성
                        for (int z = 0; z < contentPage_Unit[pageNum].contents[j].catalog._lines.Length; z++)
                        {
                            contentPage_Unit[pageNum].contents[j].catalog._lines[z].CreateColume(xmlManager.dmContents_Unit[pageNum].contents[j].catalog.rowCnt);
                        }

                        //데이터 삽입 (한줄씩 데이터 삽입)
                        for (int z = 0; z < contentPage_Unit[pageNum].contents[j].catalog._lines.Length; z++)
                        {
                            //첫번째 카테고리 키값을 가지고 모델링을 생성한다. 
                            if (z == 1 && xmlManager.dmContents_Unit[pageNum].contents[j].catalog.catalog_items[z].figureNumber == "01")
                            {
                                //z == 0은 헤더 
                                uiManager.detailSettingCanvas_Unit.objectListSetting.ObjectSetting(xmlManager.dmContents_Unit[pageNum].contents[j].catalog.catalog_items[z].subSystemCode,
                                   xmlManager.dmContents_Unit[pageNum].contents[j].catalog.catalog_items[z].subsubSystemCode,
                                   xmlManager.dmContents_Unit[pageNum].contents[j].catalog.catalog_items[z].assyCode, contentPage_Unit[pageNum].contents[j].catalog);
                            }

                            //고유의 값 등록
                            contentPage_Unit[pageNum].contents[j].catalog._lines[z].assyCode = xmlManager.dmContents_Unit[pageNum].contents[j].catalog.catalog_items[z].assyCode;
                            contentPage_Unit[pageNum].contents[j].catalog._lines[z].subSystemCode = xmlManager.dmContents_Unit[pageNum].contents[j].catalog.catalog_items[z].subSystemCode;
                            contentPage_Unit[pageNum].contents[j].catalog._lines[z].subsubSystemCode = xmlManager.dmContents_Unit[pageNum].contents[j].catalog.catalog_items[z].subsubSystemCode;
                            contentPage_Unit[pageNum].contents[j].catalog._lines[z].figureNumber = xmlManager.dmContents_Unit[pageNum].contents[j].catalog.catalog_items[z].figureNumber;
                            contentPage_Unit[pageNum].contents[j].catalog._lines[z].id = xmlManager.dmContents_Unit[pageNum].contents[j].catalog.catalog_items[z].id;
                            contentPage_Unit[pageNum].contents[j].catalog._lines[z].emp = xmlManager.dmContents_Unit[pageNum].contents[j].catalog.catalog_items[z].emp;

                            //각 컬럼 사이즈 조정
                            for (int g = 0; g < contentPage_Unit[pageNum].contents[j].catalog._lines[z].child_colume.Length; g++)
                            {
                                contentPage_Unit[pageNum].contents[j].catalog._lines[z].child_colume[g].rec_tr.sizeDelta = new Vector2(
                                    xmlManager.dmContents_Unit[pageNum].contents[j].catalog.item_width[g], contentPage_Unit[pageNum].contents[j].catalog._lines[z].child_colume[g].rec_tr.sizeDelta.y);

                                //HEADER
                                if (z == 0)
                                {
                                    contentPage_Unit[pageNum].contents[j].catalog._lines[z].child_colume[g]._text.color = header_table_text_color;
                                    contentPage_Unit[pageNum].contents[j].catalog._lines[z].child_colume[g]._text.fontSize = header_table_text_size;
                                    contentPage_Unit[pageNum].contents[j].catalog._lines[z].child_colume[g].colume_img.color = header_table_back_color;
                                }
                                //CONTENT
                                else
                                {
                                    contentPage_Unit[pageNum].contents[j].catalog._lines[z].child_colume[g]._text.color = content_table_text_color;
                                    contentPage_Unit[pageNum].contents[j].catalog._lines[z].child_colume[g]._text.fontSize = content_table_text_size;
                                    contentPage_Unit[pageNum].contents[j].catalog._lines[z].child_colume[g].colume_img.color = content_table_back_color;
                                }
                            }

                            //테이블 데이터 추가 
                            //계통/그림번호
                            contentPage_Unit[pageNum].contents[j].catalog._lines[z].child_colume[0]._text.text =
                                xmlManager.dmContents_Unit[pageNum].contents[j].catalog.catalog_items[z].figureNumber;
                            //품목 번호
                            contentPage_Unit[pageNum].contents[j].catalog._lines[z].child_colume[1]._text.text =
                                xmlManager.dmContents_Unit[pageNum].contents[j].catalog.catalog_items[z].itemSeqNumberValue;

                            //부품번호
                            contentPage_Unit[pageNum].contents[j].catalog._lines[z].child_colume[2]._text.text = "";
                                //xmlManager.dmContents_Unit[pageNum].contents[j].catalog.catalog_items[z].partNumberValue;
                            //국가재고번호
                            contentPage_Unit[pageNum].contents[j].catalog._lines[z].child_colume[3]._text.text = "";
                            //xmlManager.dmContents_Unit[pageNum].contents[j].catalog.catalog_items[z].emp;
                            //생산자 부호
                            contentPage_Unit[pageNum].contents[j].catalog._lines[z].child_colume[4]._text.text = "";
                            //xmlManager.dmContents_Unit[pageNum].contents[j].catalog.catalog_items[z].manufacturerCodeValue;
                            //설명
                            contentPage_Unit[pageNum].contents[j].catalog._lines[z].child_colume[5]._text.text = "";
                            //xmlManager.dmContents_Unit[pageNum].contents[j].catalog.catalog_items[z].descrForPart;
                            //단위당 구성 수량
                            contentPage_Unit[pageNum].contents[j].catalog._lines[z].child_colume[6]._text.text = "";
                            //xmlManager.dmContents_Unit[pageNum].contents[j].catalog.catalog_items[z].quantityPerNextHigherAssy;
                            //근원정비 복구성 부호 
                            contentPage_Unit[pageNum].contents[j].catalog._lines[z].child_colume[7]._text.text = "";
                            //xmlManager.dmContents_Unit[pageNum].contents[j].catalog.catalog_items[z].sourceMaintRecoverability;

                            //header, 몸체인 경우 
                            //|| contentPage_Unit[pageNum].contents[j].catalog._lines[z].emp == "9999-99-999-9999"
                            if (contentPage_Unit[pageNum].contents[j].catalog._lines[z].subSystemCode == "")
                            {
                                contentPage_Unit[pageNum].contents[j].catalog._lines[z].btn.interactable = false;
                                if (contentPage_Unit[pageNum].contents[j].catalog._lines[z].btn.GetComponent<ButtonTouchHandler>())
                                    contentPage_Unit[pageNum].contents[j].catalog._lines[z].btn.GetComponent<ButtonTouchHandler>().enabled = false;

                                if (contentPage_Unit[pageNum].contents[j].catalog._lines[z].btn.GetComponent<Synchronizer>())
                                    contentPage_Unit[pageNum].contents[j].catalog._lines[z].btn.GetComponent<Synchronizer>().enabled = false;

                                if (contentPage_Unit[pageNum].contents[j].catalog._lines[z].btn.GetComponent<EventTrigger>())
                                    contentPage_Unit[pageNum].contents[j].catalog._lines[z].btn.GetComponent<EventTrigger>().enabled = false;

                                //contentPage_Unit[pageNum].contents[j].catalog._lines[z].btn.GetComponent<CatalogLineSetting>().btn_img.color = content_table_back_color;
                            }
                            //라인마다 버튼 이벤트를 통해 모델링 강조 효과 추가
                            else
                            {
                                string assyCode = contentPage_Unit[pageNum].contents[j].catalog._lines[z].assyCode;
                                string subSystemCode = contentPage_Unit[pageNum].contents[j].catalog._lines[z].subSystemCode;
                                string subsubSystemCode = contentPage_Unit[pageNum].contents[j].catalog._lines[z].subsubSystemCode;
                                string figureNumber = contentPage_Unit[pageNum].contents[j].catalog._lines[z].figureNumber;
                                string id = contentPage_Unit[pageNum].contents[j].catalog._lines[z].id;

                                CatalogInfo tmp = contentPage_Unit[pageNum].contents[j].catalog;
                                int cnt;
                                cnt = z;

                                //부품 강조 이벤트 등록
                                contentPage_Unit[pageNum].contents[j].catalog._lines[z].btn.onClick.AddListener(() =>
                                {
                                    tmp.ChildFunCheck(cnt); //클린된 얘만 활성화 터치 불가능 

                                        uiManager.detailSettingCanvas_Unit.objectListSetting.ObjectController(subSystemCode, subsubSystemCode, figureNumber, assyCode, id);
                                });
                            }
                        }
                    }
                    else
                    {
                    }
                }
            }
            else
            {
                Debug.Log("NOT FOUND DM DATA : " + pageNum);
            }
        }

        StopCoroutine("HeightSetting");
        StartCoroutine("HeightSetting", pageNum);
    }

    int idCnt = 1; //초기값

    //다이어그램
    void NextStepSetting(int pageNum, int contentNum, int stepNum, int xmlNum)
    {
        if (oper == Operation.Field)
        {
            contentPage_Field[pageNum].contents[contentNum].diagramSteps.Add(Instantiate(diagramStep_pre, contentPage_Field[pageNum].contents[contentNum].diagramZone.transform));

            contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].id = xmlManager.dmContents_Field[pageNum].contents[contentNum].diagram[xmlNum].id;

            //아이템 생성
            if (xmlManager.dmContents_Field[pageNum].contents[contentNum].diagram[xmlNum].actionItems != null)
            {
                //Debug.Log("ITEMS COUNT : " + xmlManager.dmContents_Field[pageNum].contents[contentNum].diagram[xmlNum].actionItems.Count);
                contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].CreateDiagramID(idCnt);
                contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].CreateDiagramItem(xmlManager.dmContents_Field[pageNum].contents[contentNum].diagram[xmlNum].actionItems.Count);

                //Debug.Log("COUNT : " + contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems.Length);
                for (int g = 0; g < contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems.Length; g++)
                {
                    contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].para_txt.text =
                        xmlManager.dmContents_Field[pageNum].contents[contentNum].diagram[xmlNum].actionItems[g].para_txt;

                    //조치사항 (링크존재)
                    if (xmlManager.dmContents_Field[pageNum].contents[contentNum].diagram[xmlNum].actionItems[g].itemList == DiagramItemList.조치사항)
                    {
                        contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].own_img.color = contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].set_back_color;
                        contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].para_txt.color = contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].set_text_color;

                        //DM 추출 불가 
                        //if (xmlManager.dmContents_Field[pageNum].contents[contentNum].diagram[xmlNum].actionItems[g].linkDM != null) 
                        //{
                        //    contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].linkDM =
                        //        xmlManager.dmContents_Field[pageNum].contents[contentNum].diagram[xmlNum].actionItems[g].linkDM;
                        //}
                    }
                    else if (xmlManager.dmContents_Field[pageNum].contents[contentNum].diagram[xmlNum].actionItems[g].itemList == DiagramItemList.고장원인)
                    {
                        contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].own_img.color = contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].default_back_color;
                        contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].para_txt.color = contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].default_text_color;
                    }
                    else if (xmlManager.dmContents_Field[pageNum].contents[contentNum].diagram[xmlNum].actionItems[g].itemList == DiagramItemList.결과)
                    {
                        contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].own_img.color = contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].default_back_color;
                        contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].para_txt.color = contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].default_text_color;
                    }
                }
            }
            //퀘스트 생성
            if (xmlManager.dmContents_Field[pageNum].contents[contentNum].diagram[xmlNum].quest_str != null)
            {
                contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].CreateDiagramQuest();
                contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramQuest.id = xmlManager.dmContents_Field[pageNum].contents[contentNum].diagram[xmlNum].id;
                contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramQuest.quest_txt.text = xmlManager.dmContents_Field[pageNum].contents[contentNum].diagram[xmlNum].quest_str;
            }
            //엔서 생성
            if (xmlManager.dmContents_Field[pageNum].contents[contentNum].diagram[xmlNum].yes_answer_id != null
                && xmlManager.dmContents_Field[pageNum].contents[contentNum].diagram[xmlNum].no_answer_id != null)
            {
                contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].CreateDiagramAnswer();
                contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramAnswer._yesText.text = "예";
                contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramAnswer._noText.text = "아니요";
                contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramAnswer.yes_answer_id = xmlManager.dmContents_Field[pageNum].contents[contentNum].diagram[xmlNum].yes_answer_id;
                contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramAnswer.no_answer_id = xmlManager.dmContents_Field[pageNum].contents[contentNum].diagram[xmlNum].no_answer_id;
            }
        }
        else if (oper == Operation.Unit)
        {
            contentPage_Unit[pageNum].contents[contentNum].diagramSteps.Add(Instantiate(diagramStep_pre, contentPage_Unit[pageNum].contents[contentNum].diagramZone.transform));

            contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].id = xmlManager.dmContents_Unit[pageNum].contents[contentNum].diagram[xmlNum].id;

            //아이템 생성
            if (xmlManager.dmContents_Unit[pageNum].contents[contentNum].diagram[xmlNum].actionItems != null)
            {
                //Debug.Log("ITEMS COUNT : " + xmlManager.dmContents_Unit[pageNum].contents[contentNum].diagram[xmlNum].actionItems.Count);
                contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].CreateDiagramID(idCnt);
                contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].CreateDiagramItem(xmlManager.dmContents_Unit[pageNum].contents[contentNum].diagram[xmlNum].actionItems.Count);

                //Debug.Log("COUNT : " + contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems.Length);
                for (int g = 0; g < contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems.Length; g++)
                {
                    contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].para_txt.text =
                        xmlManager.dmContents_Unit[pageNum].contents[contentNum].diagram[xmlNum].actionItems[g].para_txt;

                    //조치사항 (링크존재)
                    if (xmlManager.dmContents_Unit[pageNum].contents[contentNum].diagram[xmlNum].actionItems[g].itemList == DiagramItemList.조치사항)
                    {
                        contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].own_img.color = contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].set_back_color;
                        contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].para_txt.color = contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].set_text_color;

                        //DM 추출 불가 
                        //if (xmlManager.dmContents_Unit[pageNum].contents[contentNum].diagram[xmlNum].actionItems[g].linkDM != null) 
                        //{
                        //    contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].linkDM =
                        //        xmlManager.dmContents_Unit[pageNum].contents[contentNum].diagram[xmlNum].actionItems[g].linkDM;
                        //}
                    }
                    else if (xmlManager.dmContents_Unit[pageNum].contents[contentNum].diagram[xmlNum].actionItems[g].itemList == DiagramItemList.고장원인)
                    {
                        contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].own_img.color = contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].default_back_color;
                        contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].para_txt.color = contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].default_text_color;
                    }
                    else if (xmlManager.dmContents_Unit[pageNum].contents[contentNum].diagram[xmlNum].actionItems[g].itemList == DiagramItemList.결과)
                    {
                        contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].own_img.color = contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].default_back_color;
                        contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].para_txt.color = contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].default_text_color;
                    }
                }
            }
            //퀘스트 생성
            if (xmlManager.dmContents_Unit[pageNum].contents[contentNum].diagram[xmlNum].quest_str != null)
            {
                contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].CreateDiagramQuest();
                contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramQuest.id = xmlManager.dmContents_Unit[pageNum].contents[contentNum].diagram[xmlNum].id;
                contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramQuest.quest_txt.text = xmlManager.dmContents_Unit[pageNum].contents[contentNum].diagram[xmlNum].quest_str;
            }
            //엔서 생성
            if (xmlManager.dmContents_Unit[pageNum].contents[contentNum].diagram[xmlNum].yes_answer_id != null
                && xmlManager.dmContents_Unit[pageNum].contents[contentNum].diagram[xmlNum].no_answer_id != null)
            {
                contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].CreateDiagramAnswer();
                contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramAnswer._yesText.text = "예";
                contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramAnswer._noText.text = "아니요";
                contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramAnswer.yes_answer_id = xmlManager.dmContents_Unit[pageNum].contents[contentNum].diagram[xmlNum].yes_answer_id;
                contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramAnswer.no_answer_id = xmlManager.dmContents_Unit[pageNum].contents[contentNum].diagram[xmlNum].no_answer_id;
            }
        }
    }

    //테이블 너비 높이 셋팅 
    IEnumerator HeightSetting(int pageNum)
    {
        //데이터 셋팅이 되는순간엔 제어를 뺏는다. 
        CG.alpha = 0f;
        CG.interactable = false;
        yield return delay;

        totalHeight = 0f;
        float spacingHeight = 0f;
        float textHeight = 0f;
        float figureHeight = 0f;
        float tableHeight = 0f;
        float operBtnHeight = 0f;
        float diagramHeight = 0f;
        float catalogHeight = 0f;

        if (oper == Operation.Field)
        {
            if (contentPage_Field[pageNum].page_obj == null) yield break;

            if (contentPage_Field[pageNum].contents != null)
            {
                for (int j = 0; j < contentPage_Field[pageNum].contents.Length; j++)
                {
                    spacingHeight = 0f;
                    textHeight = 0f;
                    figureHeight = 0f;
                    tableHeight = 0f;
                    operBtnHeight = 0f;
                    diagramHeight = 0f;
                    catalogHeight = 0f;

                    //공백
                    if (contentPage_Field[pageNum].contents[j].spacing_obj != null)
                    {
                        spacingHeight = xmlManager.dmContents_Field[pageNum].contents[j].spacing;

                        contentPage_Field[pageNum].contents[j].spacing_obj.sizeDelta = new Vector2(contentPage_Field[pageNum].contents[j].spacing_obj.sizeDelta.x, spacingHeight);
                    }

                    //텍스트인 경우 
                    if (contentPage_Field[pageNum].contents[j].dmItem_str != null)
                    {
                        textHeight = contentPage_Field[pageNum].contents[j].dmItem_str.GetChild_height();

                        contentPage_Field[pageNum].contents[j].dmItem_str.rect_tr.sizeDelta = new Vector2(contentPage_Field[pageNum].contents[j].dmItem_str.rect_tr.sizeDelta.x, textHeight);
                    }
                    //워닝 텍스트 
                    if (contentPage_Field[pageNum].contents[j].dmItem_warning_str != null)
                    {
                        textHeight = contentPage_Field[pageNum].contents[j].dmItem_warning_str.GetChild_height() + warningImage_height;

                        contentPage_Field[pageNum].contents[j].dmItem_warning_str.rect_tr.sizeDelta = new Vector2(contentPage_Field[pageNum].contents[j].dmItem_warning_str.rect_tr.sizeDelta.x, textHeight);
                    }
                    //경고 텍스트 
                    if (contentPage_Field[pageNum].contents[j].dmItem_caution_str != null)
                    {
                        textHeight = contentPage_Field[pageNum].contents[j].dmItem_caution_str.GetChild_height() + warningImage_height;

                        contentPage_Field[pageNum].contents[j].dmItem_caution_str.rect_tr.sizeDelta = new Vector2(contentPage_Field[pageNum].contents[j].dmItem_caution_str.rect_tr.sizeDelta.x, textHeight);
                    }
                    //노트 텍스트 
                    if (contentPage_Field[pageNum].contents[j].dmItem_note_str != null)
                    {
                        textHeight = contentPage_Field[pageNum].contents[j].dmItem_note_str.GetChild_height() + noteImage_height;

                        contentPage_Field[pageNum].contents[j].dmItem_note_str.rect_tr.sizeDelta = new Vector2(contentPage_Field[pageNum].contents[j].dmItem_note_str.rect_tr.sizeDelta.x, textHeight);
                    }

                    //폴더 컨텐츠 
                    if (contentPage_Field[pageNum].contents[j].folderContent != null)
                    {
                        textHeight = contentPage_Field[pageNum].contents[j].folderContent.GetChild_height() + folderContent_height;

                        contentPage_Field[pageNum].contents[j].folderContent.rect_tr.sizeDelta = new Vector2(contentPage_Field[pageNum].contents[j].folderContent.rect_tr.sizeDelta.x, textHeight);
                    }

                    //figure 버튼인 경우 
                    if (contentPage_Field[pageNum].contents[j].figureButton != null)
                    {
                        figureHeight = contentPage_Field[pageNum].contents[j].figureButton.GetChild_height();
                        //테이블 높이 설정 
                        contentPage_Field[pageNum].contents[j].figureButton.rect_tr.sizeDelta = new Vector2(contentPage_Field[pageNum].contents[j].figureButton.rect_tr.sizeDelta.x, figureHeight);
                    }

                    //테이블인 경우 
                    if (contentPage_Field[pageNum].contents[j].table != null)
                    {
                        tableHeight = contentPage_Field[pageNum].contents[j].table.GetChild_height();
                        //테이블 높이 설정 
                        contentPage_Field[pageNum].contents[j].table.rec_tr.sizeDelta = new Vector2(contentPage_Field[pageNum].contents[j].table.rec_tr.sizeDelta.x, tableHeight);
                    }

                    //정비 절차 버튼인 경우 
                    if (contentPage_Field[pageNum].contents[j].operBtn != null)
                    {
                        operBtnHeight = contentPage_Field[pageNum].contents[j].operBtn.GetChild_height();
                        contentPage_Field[pageNum].contents[j].operBtn.rec_tr.sizeDelta = new Vector2(contentPage_Field[pageNum].contents[j].operBtn.rec_tr.sizeDelta.x, operBtnHeight);
                    }

                    //다이어그램인 경우 
                    if (contentPage_Field[pageNum].contents[j].diagramSteps != null)
                    {
                        for (int z = 0; z < contentPage_Field[pageNum].contents[j].diagramSteps.Count; z++)
                        {
                            for (int g = 0; g < contentPage_Field[pageNum].contents[j].diagramSteps[z].diagramItems.Length; g++)
                            {
                                if (contentPage_Field[pageNum].contents[j].diagramSteps[z].diagramItems[g].GetChild_height() > 50.0f)
                                {
                                    contentPage_Field[pageNum].contents[j].diagramSteps[z].diagramItems[g].rec_tr.sizeDelta = new Vector2(contentPage_Field[pageNum].contents[j].diagramSteps[z].diagramItems[g].rec_tr.sizeDelta.x
                                        , contentPage_Field[pageNum].contents[j].diagramSteps[z].diagramItems[g].GetChild_height());
                                }
                                else //기본 값
                                {
                                    contentPage_Field[pageNum].contents[j].diagramSteps[z].diagramItems[g].rec_tr.sizeDelta = new Vector2(contentPage_Field[pageNum].contents[j].diagramSteps[z].diagramItems[g].rec_tr.sizeDelta.x
                                        , 50.0f);
                                }
                            }

                            //존재유무 파악 부터 필요 
                            //if (contentPage_Field[pageNum].contents[j].diagramSteps[z].diagramQuest.GetChild_height() > 50.0f)
                            //{
                            //    contentPage_Field[pageNum].contents[j].diagramSteps[z].diagramQuest.rec_tr.sizeDelta = new Vector2(contentPage_Field[pageNum].contents[j].diagramSteps[z].diagramQuest.rec_tr.sizeDelta.x
                            //        , contentPage_Field[pageNum].contents[j].diagramSteps[z].diagramQuest.GetChild_height());
                            //}
                            //else //기본 값
                            //{
                            //    contentPage_Field[pageNum].contents[j].diagramSteps[z].diagramQuest.rec_tr.sizeDelta = new Vector2(contentPage_Field[pageNum].contents[j].diagramSteps[z].diagramQuest.rec_tr.sizeDelta.x
                            //        , 50.0f);
                            //}

                            diagramHeight += contentPage_Field[pageNum].contents[j].diagramSteps[z].rec_tr.sizeDelta.y;
                            diagramHeight += 30.0f; //스페이싱 값 추가 
                        }
                        contentPage_Field[pageNum].contents[j].diagramZone.GetComponent<RectTransform>().sizeDelta = new Vector2(
                            contentPage_Field[pageNum].contents[j].diagramZone.GetComponent<RectTransform>().sizeDelta.x, diagramHeight);
                    }

                    //카탈로그인 경우  
                    if (contentPage_Field[pageNum].contents[j].catalog != null)
                    {
                        catalogHeight = contentPage_Field[pageNum].contents[j].catalog.GetChild_height();

                        contentPage_Field[pageNum].contents[j].catalog.rec_tr.sizeDelta = new Vector2(contentPage_Field[pageNum].contents[j].catalog.rec_tr.sizeDelta.x, catalogHeight);
                    }

                    totalHeight += spacingHeight;
                    totalHeight += textHeight;
                    totalHeight += figureHeight;
                    totalHeight += tableHeight;
                    totalHeight += operBtnHeight;
                    totalHeight += diagramHeight;
                    totalHeight += catalogHeight;
                }

                //총 길이 합산 만큼 페이지 높이 증가 
                contentPage_Field[pageNum].page_obj.sizeDelta = new Vector2(contentPage_Field[pageNum].page_obj.sizeDelta.x, totalHeight);
            }

            yield return delay;

            FolderContentButtonClick(pageNum);
        }
        else if (oper == Operation.Unit)
        {
            if (contentPage_Unit[pageNum].page_obj == null) yield break;

            if (contentPage_Unit[pageNum].contents != null)
            {
                for (int j = 0; j < contentPage_Unit[pageNum].contents.Length; j++)
                {
                    spacingHeight = 0f;
                    textHeight = 0f;
                    figureHeight = 0f;
                    tableHeight = 0f;
                    operBtnHeight = 0f;
                    diagramHeight = 0f;
                    catalogHeight = 0f;

                    //공백
                    if (contentPage_Unit[pageNum].contents[j].spacing_obj != null)
                    {
                        spacingHeight = xmlManager.dmContents_Unit[pageNum].contents[j].spacing;

                        contentPage_Unit[pageNum].contents[j].spacing_obj.sizeDelta = new Vector2(contentPage_Unit[pageNum].contents[j].spacing_obj.sizeDelta.x, spacingHeight);
                    }

                    //텍스트인 경우 
                    if (contentPage_Unit[pageNum].contents[j].dmItem_str != null)
                    {
                        textHeight = contentPage_Unit[pageNum].contents[j].dmItem_str.GetChild_height();

                        contentPage_Unit[pageNum].contents[j].dmItem_str.rect_tr.sizeDelta = new Vector2(contentPage_Unit[pageNum].contents[j].dmItem_str.rect_tr.sizeDelta.x, textHeight);
                    }
                    //워닝 텍스트 
                    if (contentPage_Unit[pageNum].contents[j].dmItem_warning_str != null)
                    {
                        textHeight = contentPage_Unit[pageNum].contents[j].dmItem_warning_str.GetChild_height() + warningImage_height;

                        contentPage_Unit[pageNum].contents[j].dmItem_warning_str.rect_tr.sizeDelta = new Vector2(contentPage_Unit[pageNum].contents[j].dmItem_warning_str.rect_tr.sizeDelta.x, textHeight);
                    }
                    //경고 텍스트 
                    if (contentPage_Unit[pageNum].contents[j].dmItem_caution_str != null)
                    {
                        textHeight = contentPage_Unit[pageNum].contents[j].dmItem_caution_str.GetChild_height() + warningImage_height;

                        contentPage_Unit[pageNum].contents[j].dmItem_caution_str.rect_tr.sizeDelta = new Vector2(contentPage_Unit[pageNum].contents[j].dmItem_caution_str.rect_tr.sizeDelta.x, textHeight);
                    }
                    //노트 텍스트 
                    if (contentPage_Unit[pageNum].contents[j].dmItem_note_str != null)
                    {
                        textHeight = contentPage_Unit[pageNum].contents[j].dmItem_note_str.GetChild_height() + noteImage_height;

                        contentPage_Unit[pageNum].contents[j].dmItem_note_str.rect_tr.sizeDelta = new Vector2(contentPage_Unit[pageNum].contents[j].dmItem_note_str.rect_tr.sizeDelta.x, textHeight);
                    }

                    //폴더 컨텐츠 
                    if (contentPage_Unit[pageNum].contents[j].folderContent != null)
                    {
                        textHeight = contentPage_Unit[pageNum].contents[j].folderContent.GetChild_height() + folderContent_height;

                        contentPage_Unit[pageNum].contents[j].folderContent.rect_tr.sizeDelta = new Vector2(contentPage_Unit[pageNum].contents[j].folderContent.rect_tr.sizeDelta.x, textHeight);
                    }

                    //figure 버튼인 경우 
                    if (contentPage_Unit[pageNum].contents[j].figureButton != null)
                    {
                        figureHeight = contentPage_Unit[pageNum].contents[j].figureButton.GetChild_height();
                        //테이블 높이 설정 
                        contentPage_Unit[pageNum].contents[j].figureButton.rect_tr.sizeDelta = new Vector2(contentPage_Unit[pageNum].contents[j].figureButton.rect_tr.sizeDelta.x, figureHeight);
                    }

                    //테이블인 경우 
                    if (contentPage_Unit[pageNum].contents[j].table != null)
                    {
                        tableHeight = contentPage_Unit[pageNum].contents[j].table.GetChild_height();
                        //테이블 높이 설정 
                        contentPage_Unit[pageNum].contents[j].table.rec_tr.sizeDelta = new Vector2(contentPage_Unit[pageNum].contents[j].table.rec_tr.sizeDelta.x, tableHeight);
                    }

                    //정비 절차 버튼인 경우 
                    if (contentPage_Unit[pageNum].contents[j].operBtn != null)
                    {
                        operBtnHeight = contentPage_Unit[pageNum].contents[j].operBtn.GetChild_height();
                        //Debug.Log("oper Hieght : " + operBtnHeight);
                        contentPage_Unit[pageNum].contents[j].operBtn.rec_tr.sizeDelta = new Vector2(contentPage_Unit[pageNum].contents[j].operBtn.rec_tr.sizeDelta.x, operBtnHeight);
                    }

                    //다이어그램인 경우 
                    if (contentPage_Unit[pageNum].contents[j].diagramSteps != null)
                    {
                        for (int z = 0; z < contentPage_Unit[pageNum].contents[j].diagramSteps.Count; z++)
                        {
                            for (int g = 0; g < contentPage_Unit[pageNum].contents[j].diagramSteps[z].diagramItems.Length; g++)
                            {
                                if (contentPage_Unit[pageNum].contents[j].diagramSteps[z].diagramItems[g].GetChild_height() > 50.0f)
                                {
                                    contentPage_Unit[pageNum].contents[j].diagramSteps[z].diagramItems[g].rec_tr.sizeDelta = new Vector2(contentPage_Unit[pageNum].contents[j].diagramSteps[z].diagramItems[g].rec_tr.sizeDelta.x
                                        , contentPage_Unit[pageNum].contents[j].diagramSteps[z].diagramItems[g].GetChild_height());
                                }
                                else //기본 값
                                {
                                    contentPage_Unit[pageNum].contents[j].diagramSteps[z].diagramItems[g].rec_tr.sizeDelta = new Vector2(contentPage_Unit[pageNum].contents[j].diagramSteps[z].diagramItems[g].rec_tr.sizeDelta.x
                                        , 50.0f);
                                }
                            }

                            //존재유무 파악 부터 필요 
                            //if (contentPage_Unit[pageNum].contents[j].diagramSteps[z].diagramQuest.GetChild_height() > 50.0f)
                            //{
                            //    contentPage_Unit[pageNum].contents[j].diagramSteps[z].diagramQuest.rec_tr.sizeDelta = new Vector2(contentPage_Unit[pageNum].contents[j].diagramSteps[z].diagramQuest.rec_tr.sizeDelta.x
                            //        , contentPage_Unit[pageNum].contents[j].diagramSteps[z].diagramQuest.GetChild_height());
                            //}
                            //else //기본 값
                            //{
                            //    contentPage_Unit[pageNum].contents[j].diagramSteps[z].diagramQuest.rec_tr.sizeDelta = new Vector2(contentPage_Unit[pageNum].contents[j].diagramSteps[z].diagramQuest.rec_tr.sizeDelta.x
                            //        , 50.0f);
                            //}

                            diagramHeight += contentPage_Unit[pageNum].contents[j].diagramSteps[z].rec_tr.sizeDelta.y;
                            diagramHeight += 30.0f; //스페이싱 값 추가 
                        }
                        contentPage_Unit[pageNum].contents[j].diagramZone.GetComponent<RectTransform>().sizeDelta = new Vector2(
                            contentPage_Unit[pageNum].contents[j].diagramZone.GetComponent<RectTransform>().sizeDelta.x, diagramHeight);
                    }

                    //카탈로그인 경우  
                    if (contentPage_Unit[pageNum].contents[j].catalog != null)
                    {
                        catalogHeight = contentPage_Unit[pageNum].contents[j].catalog.GetChild_height();

                        contentPage_Unit[pageNum].contents[j].catalog.rec_tr.sizeDelta = new Vector2(contentPage_Unit[pageNum].contents[j].catalog.rec_tr.sizeDelta.x, catalogHeight);
                    }

                    totalHeight += spacingHeight;
                    totalHeight += textHeight;
                    totalHeight += figureHeight;
                    totalHeight += tableHeight;
                    totalHeight += operBtnHeight;
                    totalHeight += diagramHeight;
                    totalHeight += catalogHeight;
                }

                //총 길이 합산 만큼 페이지 높이 증가 
                contentPage_Unit[pageNum].page_obj.sizeDelta = new Vector2(contentPage_Unit[pageNum].page_obj.sizeDelta.x, totalHeight);
            }

            yield return delay;

            FolderContentButtonClick(pageNum);
        }

        CG.alpha = 1f;
        CG.interactable = true;
    }


    public IEnumerator PageHeightSet(float height)
    {
        yield return new WaitForSeconds(1f);
        scrollRect_content.content.anchoredPosition = new Vector3(0, height, 0);
    }

    void InitPageData()
    {
        if (oper == Operation.Field)
        {
            for (int i = 0; i < contentPage_Field.Length; i++)
            {
                contentPage_Field[i].page_obj.gameObject.SetActive(false);

                if (contentPage_Field[i].page_obj.transform.childCount != 0)
                {
                    RectTransform[] child = contentPage_Field[i].page_obj.gameObject.GetComponentsInChildren<RectTransform>(true);

                    if (child.Length != 0)
                    {
                        for (int j = 1; j < child.Length; j++)
                        {
                            Destroy(child[j].gameObject);
                            
                        }
                    }

                    contentPage_Field[i].contents = null;
                }
            }
        }
        else if (oper == Operation.Unit)
        {
            for (int i = 0; i < contentPage_Unit.Length; i++)
            {
                contentPage_Unit[i].page_obj.gameObject.SetActive(false);

                if (contentPage_Unit[i].page_obj.transform.childCount != 0)
                {
                    RectTransform[] child = contentPage_Unit[i].page_obj.gameObject.GetComponentsInChildren<RectTransform>(true);

                    if (child.Length != 0)
                    {
                        for (int j = 1; j < child.Length; j++)
                        {
                            Destroy(child[j].gameObject);
                        }
                    }

                    contentPage_Unit[i].contents = null;
                }
            }
        }
    }

    public void ContentButtonClick(int chapter)
    {
        uiManager.DetailPageReset();

        InitPageData();
        if (oper == Operation.Field)
        {
            chapterNum = chapter;

            if (contentPage_Field[chapter].page_obj == null) return;

            scrollRect_content.content = contentPage_Field[chapter].page_obj.GetComponent<RectTransform>();
            contentPage_Field[chapter].page_obj.gameObject.SetActive(true);

            //확대/축소 기능 원상태로 돌려야된다. 
            uiManager.detailSettingCanvas_Field.objectListSetting.setFullMode_TB.gameObject.SetActive(false);
            uiManager.detailSettingCanvas_Field.FullScreenOffSetting();

            if (contentPage_Field[chapter].page_obj.transform.childCount == 0)
            {
                SelectPageDataLoad(chapter);

                scrollRect_content.content.anchoredPosition = Vector3.zero;
                scrollRect_content_rectr.anchoredPosition = Vector3.zero;
                content_box_rectr.anchoredPosition = reset_rectr;
            }
            else
            {
                //기존에 셋팅된 데이터 (이전 데이터만 비활성화)
                PreContentInit(chapter);

                scrollRect_content.content.anchoredPosition = Vector3.zero;
                scrollRect_content_rectr.anchoredPosition = Vector3.zero;
                content_box_rectr.anchoredPosition = reset_rectr;
            }
        }
        else if (oper == Operation.Unit)
        {
            chapterNum = chapter;

            if (contentPage_Unit[chapter].page_obj == null) return;

            scrollRect_content.content = contentPage_Unit[chapter].page_obj.GetComponent<RectTransform>();
            contentPage_Unit[chapter].page_obj.gameObject.SetActive(true);

            //확대/축소 기능 원상태로 돌려야된다. 
            uiManager.detailSettingCanvas_Unit.objectListSetting.setFullMode_TB.gameObject.SetActive(false);
            uiManager.detailSettingCanvas_Unit.FullScreenOffSetting();

            if (contentPage_Unit[chapter].page_obj.transform.childCount == 0)
            {
                SelectPageDataLoad(chapter);

                scrollRect_content.content.anchoredPosition = Vector3.zero;
                scrollRect_content_rectr.anchoredPosition = Vector3.zero;
                content_box_rectr.anchoredPosition = reset_rectr;
            }
            else
            {
                //기존에 셋팅된 데이터 (이전 데이터만 비활성화)
                PreContentInit(chapter);

                scrollRect_content.content.anchoredPosition = Vector3.zero;
                scrollRect_content_rectr.anchoredPosition = Vector3.zero;
                content_box_rectr.anchoredPosition = reset_rectr;
            }
        }
    }


    //이전데이터 불러오기 
    void PreContentInit(int chapter)
    {
        if (oper == Operation.Field)
        {
            for (int i = 0; i < contentPage_Field[chapter].contents.Length; i++)
            {
                if (contentPage_Field[chapter].contents[i].catalog != null)
                {
                    for (int z = 0; z < contentPage_Field[chapter].contents[i].catalog._lines.Length; z++)
                    {
                        //첫번째 카테고리 키값을 가지고 모델링을 생성한다. 
                        if (z == 1 && xmlManager.dmContents_Field[chapter].contents[i].catalog.catalog_items[z].figureNumber == "01")
                        {
                            uiManager.detailSettingCanvas_Field.objectListSetting.ObjectSetting(xmlManager.dmContents_Field[chapter].contents[i].catalog.catalog_items[z].subSystemCode,
                               xmlManager.dmContents_Field[chapter].contents[i].catalog.catalog_items[z].subsubSystemCode,
                               xmlManager.dmContents_Field[chapter].contents[i].catalog.catalog_items[z].assyCode, contentPage_Field[chapter].contents[i].catalog);

                            uiManager.detailSettingCanvas_Field.objectListSetting.setFullMode_TB.gameObject.SetActive(true);
                        }
                    }
                }
                else if (contentPage_Field[chapter].contents[i].operBtn != null)
                {
                    uiManager.detailSettingCanvas_Field.objectListSetting.OperationReady(contentPage_Field[chapter].contents[i].operBtn.title,
                        contentPage_Field[chapter].contents[i].operBtn.subtitle, contentPage_Field[chapter].contents[i].operBtn);
                }

                else if (contentPage_Field[chapter].contents[i].folderContent != null)
                {
                    contentPage_Field[chapter].contents[i].folderContent.ContentInit();
                }

                else if (contentPage_Field[chapter].contents[i].figureButton != null)
                {
                    FigureButtonInit(chapter, true);
                }
            }
        }
        else if(oper == Operation.Unit)
        {
            for (int i = 0; i < contentPage_Unit[chapter].contents.Length; i++)
            {
                if (contentPage_Unit[chapter].contents[i].catalog != null)
                {
                    for (int z = 0; z < contentPage_Unit[chapter].contents[i].catalog._lines.Length; z++)
                    {
                        //첫번째 카테고리 키값을 가지고 모델링을 생성한다. 
                        if (z == 1 && xmlManager.dmContents_Unit[chapter].contents[i].catalog.catalog_items[z].figureNumber == "01")
                        {
                            uiManager.detailSettingCanvas_Unit.objectListSetting.ObjectSetting(xmlManager.dmContents_Unit[chapter].contents[i].catalog.catalog_items[z].subSystemCode,
                               xmlManager.dmContents_Unit[chapter].contents[i].catalog.catalog_items[z].subsubSystemCode,
                               xmlManager.dmContents_Unit[chapter].contents[i].catalog.catalog_items[z].assyCode, contentPage_Unit[chapter].contents[i].catalog);

                            uiManager.detailSettingCanvas_Unit.objectListSetting.setFullMode_TB.gameObject.SetActive(true);
                        }
                    }
                }
                else if (contentPage_Unit[chapter].contents[i].operBtn != null)
                {
                    uiManager.detailSettingCanvas_Unit.objectListSetting.OperationReady(contentPage_Unit[chapter].contents[i].operBtn.title,
                        contentPage_Unit[chapter].contents[i].operBtn.subtitle, contentPage_Unit[chapter].contents[i].operBtn);
                }

                else if (contentPage_Unit[chapter].contents[i].folderContent != null)
                {
                    contentPage_Unit[chapter].contents[i].folderContent.ContentInit();
                }

                else if (contentPage_Unit[chapter].contents[i].figureButton != null)
                {
                    FigureButtonInit(chapter, true);
                }
            }
        }
    }

    public int SearchDMNumber(string _dmName)
    {
        int dmNumber = 0;

        if (oper == Operation.Field)
        {
            for (int i = 0; i < xmlManager.dmContents_Field.Count; i++)
            {
                if (xmlManager.dmContents_Field[i].OwnDM.name == _dmName)
                {
                    dmNumber = i;

                    break;
                }
            }
        }
        else if (oper == Operation.Unit)
        {
            for (int i = 0; i < xmlManager.dmContents_Unit.Count; i++)
            {
                if (xmlManager.dmContents_Unit[i].OwnDM.name == _dmName)
                {
                    dmNumber = i;

                    break;
                }
            }
        }

        return dmNumber;
    }


    //컨텐츠 스크롤뷰 터치 했을때 
    public void WhenScrollTouch()
    {
        if (chapterNum == -1) return;

        if (oper == Operation.Field)
        {
            if (contentPage_Field[chapterNum].page_obj.anchoredPosition.y > 500f)
                top_btn.gameObject.SetActive(true);
            else
                top_btn.gameObject.SetActive(false);
        }
        else if (oper == Operation.Unit)
        {
            if (contentPage_Unit[chapterNum].page_obj.anchoredPosition.y > 500f)
                top_btn.gameObject.SetActive(true);
            else
                top_btn.gameObject.SetActive(false);
        }
    }

    //위로가기 버튼 클릭
    public void ContentTopButtonClick()
    {
        if (oper == Operation.Field)
        {
            contentPage_Field[chapterNum].page_obj.anchoredPosition = new Vector3(0, 0, 0);

            top_btn.gameObject.SetActive(false);
        }
        else if (oper == Operation.Unit)
        {
            contentPage_Unit[chapterNum].page_obj.anchoredPosition = new Vector3(0, 0, 0);

            top_btn.gameObject.SetActive(false);
        }
    }

    //AR 인식 -> 상세보기 (폴더 데이터 열기)
    public IEnumerator FolderOpen(int pageNum, int folderNum)
    {
        yield return new WaitForSeconds(0.5f);
        int count = 0;

        if (oper == Operation.Field)
        {
            //몇번째 폴더인지 찾아야된다. 
            for (int i = 0; i < contentPage_Field[pageNum].contents.Length; i++)
            {
                if (contentPage_Field[pageNum].contents[i].folderContent != null)
                {
                    if (count == folderNum)
                    {
                        contentPage_Field[pageNum].contents[i].folderContent.ContentController();

                        break;
                    }

                    else count += 1;
                }
            }
        }
        else if (oper == Operation.Unit)
        {
            //몇번째 폴더인지 찾아야된다. 
            for (int i = 0; i < contentPage_Unit[pageNum].contents.Length; i++)
            {
                if (contentPage_Unit[pageNum].contents[i].folderContent != null)
                {
                    if (count == folderNum)
                    {
                        contentPage_Unit[pageNum].contents[i].folderContent.ContentController();

                        break;
                    }

                    else count += 1;
                }
            }
        }
    }


    //Figure 버튼 클릭시 나머지 버튼 초기화
    //trigger == true : 컨텐츠 데이터도 초기화 
    public void FigureButtonInit(int pageNum, bool trigger)
    {
        if (oper == Operation.Field)
        {
            if (contentPage_Field[pageNum].contents.Length == 0) return;

            for (int i = 0; i < contentPage_Field[pageNum].contents.Length; i++)
            {
                if (contentPage_Field[pageNum].contents[i].figureButton != null)
                {
                    contentPage_Field[pageNum].contents[i].figureButton.ButtonStateDefault();
                }
            }

            if(trigger)
                uiManager.detailSettingCanvas_Field.objectListSetting.InitFigure();
        }
        else if (oper == Operation.Unit)
        {
            if (contentPage_Unit[pageNum].contents.Length == 0) return;

            for (int i = 0; i < contentPage_Unit[pageNum].contents.Length; i++)
            {
                if (contentPage_Unit[pageNum].contents[i].figureButton != null)
                {
                    contentPage_Unit[pageNum].contents[i].figureButton.ButtonStateDefault();
                }
            }
            if (trigger)
                uiManager.detailSettingCanvas_Unit.objectListSetting.InitFigure();
        }

        
    }

    //Figure 버튼 클릭시 나머지 버튼 제어 
    public void FigureButtonController(int pageNum, int buttonNum)
    {
        if (oper == Operation.Field)
        {
            if (contentPage_Field[pageNum].contents.Length == 0) return;

            if (contentPage_Field[pageNum].contents[buttonNum].figureButton.isActive)
            {
                FigureButtonInit(pageNum, true); //컨텐츠 데이터도 초기화 
                contentPage_Field[pageNum].contents[buttonNum].figureButton.ButtonStateDefault();
            }
            else
            {
                FigureButtonInit(pageNum, false); //컨텐츠 데이터는 유지 
                contentPage_Field[pageNum].contents[buttonNum].figureButton.ButtonClickEvent();
            }
        }
        else if (oper == Operation.Unit)
        {
            if (contentPage_Unit[pageNum].contents.Length == 0) return;

            if (contentPage_Unit[pageNum].contents[buttonNum].figureButton.isActive)
            {
                FigureButtonInit(pageNum, true);
                contentPage_Unit[pageNum].contents[buttonNum].figureButton.ButtonStateDefault();
            }
            else
            {
                FigureButtonInit(pageNum, false);
                contentPage_Unit[pageNum].contents[buttonNum].figureButton.ButtonClickEvent();
            }
        }
    }

    //폴더컨텐츠 버튼 클릭 
    public void FolderContentButtonClick(int pageNum)
    {
        if (oper == Operation.Field)
        {
            if (contentPage_Field[pageNum].contents.Length == 0) return;

            int containerNum = -1;
            for (int j = 0; j < contentPage_Field[pageNum].contents.Length; j++)
            {
                //폴더 컨텐츠가 나오면 그 데이터 기준으로 그다음 폴더컨텐츠가 나오는 데이터까지 담아서 관리 한다. 
                if (contentPage_Field[pageNum].contents[j].folderContent != null)
                {
                    containerNum = j;

                    continue; //다음 순번
                }

                if (containerNum != -1) //Data.TITLE은 제외 항목 
                {
                    if (contentPage_Field[pageNum].contents[j].isTitle) continue; //title항목은 제외 시킨다. 

                    //공백
                    if (contentPage_Field[pageNum].contents[j].data == Data.SPACING)
                    {
                        //Debug.Log(contentPage_Field[pageNum].contents[containerNum].folderContent.contents);

                        if (contentPage_Field[pageNum].contents[j].spacing_obj != null)
                        {
                            contentPage_Field[pageNum].contents[containerNum].folderContent.contents.Add(contentPage_Field[pageNum].contents[j].spacing_obj.gameObject);
                            contentPage_Field[pageNum].contents[j].spacing_obj.gameObject.SetActive(false);
                        }
                    }
                    //텍스트인 경우 
                    else if (contentPage_Field[pageNum].contents[j].data == Data.TEXT)
                    {
                        //일반 텍스트 
                        if (contentPage_Field[pageNum].contents[j].dmItem_str != null)
                        {
                            contentPage_Field[pageNum].contents[containerNum].folderContent.contents.Add(contentPage_Field[pageNum].contents[j].dmItem_str.gameObject);
                            contentPage_Field[pageNum].contents[j].dmItem_str.gameObject.SetActive(false);
                        }
                        //워닝 텍스트 
                        else if (contentPage_Field[pageNum].contents[j].dmItem_warning_str != null)
                        {
                            contentPage_Field[pageNum].contents[containerNum].folderContent.contents.Add(contentPage_Field[pageNum].contents[j].dmItem_warning_str.gameObject);
                            contentPage_Field[pageNum].contents[j].dmItem_warning_str.gameObject.SetActive(false);
                        }
                        //워닝 텍스트 
                        else if (contentPage_Field[pageNum].contents[j].dmItem_caution_str != null)
                        {
                            contentPage_Field[pageNum].contents[containerNum].folderContent.contents.Add(contentPage_Field[pageNum].contents[j].dmItem_caution_str.gameObject);
                            contentPage_Field[pageNum].contents[j].dmItem_caution_str.gameObject.SetActive(false);
                        }
                        //노트 텍스트 
                        else if (contentPage_Field[pageNum].contents[j].dmItem_note_str != null)
                        {
                            contentPage_Field[pageNum].contents[containerNum].folderContent.contents.Add(contentPage_Field[pageNum].contents[j].dmItem_note_str.gameObject);
                            contentPage_Field[pageNum].contents[j].dmItem_note_str.gameObject.SetActive(false);
                        }
                    }
                    //Figure 인경우 
                    else if (contentPage_Field[pageNum].contents[j].data == Data.FIGURE)
                    {
                        if (contentPage_Field[pageNum].contents[j].figureButton != null)
                        {
                            contentPage_Field[pageNum].contents[containerNum].folderContent.contents.Add(contentPage_Field[pageNum].contents[j].figureButton.gameObject);
                            contentPage_Field[pageNum].contents[j].figureButton.gameObject.SetActive(false);
                        }
                    }
                    //테이블인 경우 
                    else if (contentPage_Field[pageNum].contents[j].data == Data.TABLE)
                    {
                        if (contentPage_Field[pageNum].contents[j].table != null)
                        {
                            contentPage_Field[pageNum].contents[containerNum].folderContent.contents.Add(contentPage_Field[pageNum].contents[j].table.gameObject);
                            contentPage_Field[pageNum].contents[j].table.gameObject.SetActive(false);
                        }
                    }
                    //정비 절차 버튼인 경우 
                    else if (contentPage_Field[pageNum].contents[j].data == Data.OPERATION)
                    {
                        if (contentPage_Field[pageNum].contents[j].operBtn != null)
                        {
                            contentPage_Field[pageNum].contents[containerNum].folderContent.contents.Add(contentPage_Field[pageNum].contents[j].operBtn.gameObject);
                            contentPage_Field[pageNum].contents[j].operBtn.gameObject.SetActive(false);
                        }
                    }
                    //다이어그램인 경우 
                    else if (contentPage_Field[pageNum].contents[j].data == Data.DIAGRAM)
                    {
                        if (contentPage_Field[pageNum].contents[j].diagramZone != null)
                        {
                            contentPage_Field[pageNum].contents[containerNum].folderContent.contents.Add(contentPage_Field[pageNum].contents[j].diagramZone.gameObject);
                            contentPage_Field[pageNum].contents[j].diagramZone.gameObject.SetActive(false);
                        }
                    }
                    //카탈로그인 경우  
                    else if (contentPage_Field[pageNum].contents[j].data == Data.CATALOG)
                    {
                        if (contentPage_Field[pageNum].contents[j].catalog != null)
                        {
                            contentPage_Field[pageNum].contents[containerNum].folderContent.contents.Add(contentPage_Field[pageNum].contents[j].catalog.gameObject);
                            contentPage_Field[pageNum].contents[j].catalog.gameObject.SetActive(false);
                        }
                    }
                }
            }

        }
        else if (oper == Operation.Unit)
        {
            if (contentPage_Unit[pageNum].contents.Length == 0) return;

            int containerNum = -1;
            for (int j = 0; j < contentPage_Unit[pageNum].contents.Length; j++)
            {
                //폴더 컨텐츠가 나오면 그 데이터 기준으로 그다음 폴더컨텐츠가 나오는 데이터까지 담아서 관리 한다. 
                if (contentPage_Unit[pageNum].contents[j].folderContent != null)
                {
                    containerNum = j;

                    continue; //다음 순번
                }

                if (containerNum != -1) //Data.TITLE은 제외 항목 
                {
                    if (contentPage_Unit[pageNum].contents[j].isTitle) continue; //title항목은 제외 시킨다. 

                    //공백
                    if (contentPage_Unit[pageNum].contents[j].data == Data.SPACING)
                    {
                        //Debug.Log(contentPage_Unit[pageNum].contents[containerNum].folderContent.contents);

                        if (contentPage_Unit[pageNum].contents[j].spacing_obj != null)
                        {
                            contentPage_Unit[pageNum].contents[containerNum].folderContent.contents.Add(contentPage_Unit[pageNum].contents[j].spacing_obj.gameObject);
                            contentPage_Unit[pageNum].contents[j].spacing_obj.gameObject.SetActive(false);
                        }
                    }
                    //텍스트인 경우 
                    else if (contentPage_Unit[pageNum].contents[j].data == Data.TEXT)
                    {
                        //일반 텍스트 
                        if (contentPage_Unit[pageNum].contents[j].dmItem_str != null)
                        {
                            contentPage_Unit[pageNum].contents[containerNum].folderContent.contents.Add(contentPage_Unit[pageNum].contents[j].dmItem_str.gameObject);
                            contentPage_Unit[pageNum].contents[j].dmItem_str.gameObject.SetActive(false);
                        }
                        //워닝 텍스트 
                        else if (contentPage_Unit[pageNum].contents[j].dmItem_warning_str != null)
                        {
                            contentPage_Unit[pageNum].contents[containerNum].folderContent.contents.Add(contentPage_Unit[pageNum].contents[j].dmItem_warning_str.gameObject);
                            contentPage_Unit[pageNum].contents[j].dmItem_warning_str.gameObject.SetActive(false);
                        }
                        else if (contentPage_Unit[pageNum].contents[j].dmItem_caution_str != null)
                        {
                            contentPage_Unit[pageNum].contents[containerNum].folderContent.contents.Add(contentPage_Unit[pageNum].contents[j].dmItem_caution_str.gameObject);
                            contentPage_Unit[pageNum].contents[j].dmItem_caution_str.gameObject.SetActive(false);
                        }
                        //노트 텍스트 
                        else if (contentPage_Unit[pageNum].contents[j].dmItem_note_str != null)
                        {
                            contentPage_Unit[pageNum].contents[containerNum].folderContent.contents.Add(contentPage_Unit[pageNum].contents[j].dmItem_note_str.gameObject);
                            contentPage_Unit[pageNum].contents[j].dmItem_note_str.gameObject.SetActive(false);
                        }
                    }
                    //Figure 인경우 
                    else if (contentPage_Unit[pageNum].contents[j].data == Data.FIGURE)
                    {
                        if (contentPage_Unit[pageNum].contents[j].figureButton != null)
                        {
                            contentPage_Unit[pageNum].contents[containerNum].folderContent.contents.Add(contentPage_Unit[pageNum].contents[j].figureButton.gameObject);
                            contentPage_Unit[pageNum].contents[j].figureButton.gameObject.SetActive(false);
                        }
                    }
                    //테이블인 경우 
                    else if (contentPage_Unit[pageNum].contents[j].data == Data.TABLE)
                    {
                        if (contentPage_Unit[pageNum].contents[j].table != null)
                        {
                            contentPage_Unit[pageNum].contents[containerNum].folderContent.contents.Add(contentPage_Unit[pageNum].contents[j].table.gameObject);
                            contentPage_Unit[pageNum].contents[j].table.gameObject.SetActive(false);
                        }
                    }
                    //정비 절차 버튼인 경우 
                    else if (contentPage_Unit[pageNum].contents[j].data == Data.OPERATION)
                    {
                        if (contentPage_Unit[pageNum].contents[j].operBtn != null)
                        {
                            contentPage_Unit[pageNum].contents[containerNum].folderContent.contents.Add(contentPage_Unit[pageNum].contents[j].operBtn.gameObject);
                            contentPage_Unit[pageNum].contents[j].operBtn.gameObject.SetActive(false);
                        }
                    }
                    //다이어그램인 경우 
                    else if (contentPage_Unit[pageNum].contents[j].data == Data.DIAGRAM)
                    {
                        if (contentPage_Unit[pageNum].contents[j].diagramZone != null)
                        {
                            contentPage_Unit[pageNum].contents[containerNum].folderContent.contents.Add(contentPage_Unit[pageNum].contents[j].diagramZone.gameObject);
                            contentPage_Unit[pageNum].contents[j].diagramZone.gameObject.SetActive(false);
                        }
                    }
                    //카탈로그인 경우  
                    else if (contentPage_Unit[pageNum].contents[j].data == Data.CATALOG)
                    {
                        if (contentPage_Unit[pageNum].contents[j].catalog != null)
                        {
                            contentPage_Unit[pageNum].contents[containerNum].folderContent.contents.Add(contentPage_Unit[pageNum].contents[j].catalog.gameObject);
                            contentPage_Unit[pageNum].contents[j].catalog.gameObject.SetActive(false);
                        }
                    }
                }
            }

        }
    }
}
