using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System;

[System.Serializable]
//�� �������� �ʿ��� ������ 
public struct ContentPage // Value ��
{
    public RectTransform page_obj; //������ ������
    public Content[] contents; //����
}

//public struct DataGame

[System.Serializable]
public struct Content
{
    public Data data;
    public bool isTitle;

    public RectTransform spacing_obj; //�����
    public ContentInfo dmItem_str; //DM �ؽ�Ʈ ���� 
    public ContentInfo dmItem_warning_str;
    public ContentInfo dmItem_caution_str;
    public ContentInfo dmItem_note_str;

    public FolderContentSetting folderContent;

    public FigureButtonSetting figureButton;
    public TableInfo table; //DM ���̺� ���� 
    public OperationSetting operBtn; //������� 

    public CatalogInfo catalog; //īŻ�α�(�𵨸�)

    public GameObject diagramZone;
    public List<DiagramStep> diagramSteps; //���̾�׷�
}

public class ContentListSetting : MonoBehaviour
{
    [SerializeField] public ScrollRect scrollRect_content;
    [SerializeField] public RectTransform scrollRect_content_rectr;
    [SerializeField] public RectTransform content_box_rectr;
    [SerializeField] public Vector3 reset_rectr;
    [SerializeField] public CanvasGroup CG;

    [Header(" [ CONTENT PAGE ] ")]
    [SerializeField] public RectTransform contentPage_pre; //������ 

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

    [SerializeField] float totalHeight = 0f; //�����۸��� ��ø�Ǵ� ���� ��� 
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
    [HideInInspector] public int chapterNum;  // -1 �̸� ���� �Ұ��� 
    [SerializeField] public ButtonController top_btn;

    //[Header(" [ RESOURCE ] ")]
    [SerializeField] XMLManager xmlManager;
    [SerializeField] UIManager uiManager;

    enum Operation { Field, Unit }; //������ ���Ǵ� ����
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
            contentPage_Field = new ContentPage[xmlManager.dmContents_Field.Count]; //���� ����
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
    //������ �����߿� Figure��ư ���ҽ��� �ʿ� ������ 
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
    bool spacingCheck = false; //�����ؼ� ������ ������ �������� 

    //������ ����(�ش� ������ ������ Ŭ�������� �� �������� �ε�)
    public void SelectPageDataLoad(int pageNum)
    {
        int figureCnt = 1;

        if (oper == Operation.Field)
        {
            if (xmlManager.dmContents_Field[pageNum].contents != null)
            {
                contentPage_Field[pageNum].page_obj.gameObject.SetActive(true);
                Debug.Log(xmlManager.dmContents_Field[pageNum].OwnDM.name);

                //�������� �����
                contentPage_Field[pageNum].contents = new Content[xmlManager.dmContents_Field[pageNum].contents.Count];

                //Figure ��� �ʿ����� üũ �Լ� 
                FigureControlCheck(pageNum);

                //contents �ϳ��� �ؽ�Ʈ �ϳ� �Ǵ� ���̺� �ϳ� 
                for (int j = 0; j < contentPage_Field[pageNum].contents.Length; j++)
                {
                    contentPage_Field[pageNum].contents[j].data = xmlManager.dmContents_Field[pageNum].contents[j].data;
                    contentPage_Field[pageNum].contents[j].isTitle = xmlManager.dmContents_Field[pageNum].contents[j].isTitle;

                    //������϶� 
                    if (xmlManager.dmContents_Field[pageNum].contents[j].data == Data.SPACING && !spacingCheck)
                    {
                        spacingCheck = true;
                        contentPage_Field[pageNum].contents[j].spacing_obj = Instantiate(spacing_pre, contentPage_Field[pageNum].page_obj.transform);
                    }

                    //text/warning text �� ��� 
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

                    //FolderContent �� ��� 
                    else if (xmlManager.dmContents_Field[pageNum].contents[j].data == Data.FOLDERCONTENT)
                    {
                        spacingCheck = false;
                        //header 
                        contentPage_Field[pageNum].contents[j].folderContent = Instantiate(folderContent_pre, contentPage_Field[pageNum].page_obj.transform);

                        contentPage_Field[pageNum].contents[j].folderContent.header_txt.text = xmlManager.dmContents_Field[pageNum].contents[j].dmItem_text;
                        contentPage_Field[pageNum].contents[j].folderContent.header_txt.fontSize = xmlManager.dmContents_Field[pageNum].contents[j].textSize;
                        contentPage_Field[pageNum].contents[j].folderContent.header_txt.color = xmlManager.dmContents_Field[pageNum].contents[j].textColor;
                    }

                    //Figure �� ��� (�̹��� �Ǵ� ��ü) - ��ư ���� 
                    else if (xmlManager.dmContents_Field[pageNum].contents[j].data == Data.FIGURE && !figureControl)
                    {
                        spacingCheck = false;
                        //header 
                        contentPage_Field[pageNum].contents[j].figureButton = Instantiate(figureButton_pre, contentPage_Field[pageNum].page_obj.transform);

                        contentPage_Field[pageNum].contents[j].figureButton.button_txt.text = "<size=20>�׸� " + figureCnt + ". </size>" +
                            xmlManager.dmContents_Field[pageNum].contents[j].figure.title + " ����";
                        figureCnt++;

                        contentPage_Field[pageNum].contents[j].figureButton.id = xmlManager.dmContents_Field[pageNum].contents[j].figure.id;
                        contentPage_Field[pageNum].contents[j].figureButton.info = xmlManager.dmContents_Field[pageNum].contents[j].figure.infoEntityIdent;

                        string id = contentPage_Field[pageNum].contents[j].figureButton.id;
                        string info = contentPage_Field[pageNum].contents[j].figureButton.info;
                        int tmp = pageNum;
                        int tmp_1 = j;

                        //��ư �̺�Ʈ ���
                        contentPage_Field[pageNum].contents[j].figureButton.btn.onClick.AddListener(() =>
                        {
                            uiManager.detailSettingCanvas_Field.objectListSetting.FigureSetting(id, info);

                            FigureButtonController(tmp, tmp_1);
                        });
                    }

                    //���� ���� ��ư�� ��� 
                    else if (xmlManager.dmContents_Field[pageNum].contents[j].data == Data.OPERATION)
                    {
                        spacingCheck = false;
                        contentPage_Field[pageNum].contents[j].operBtn = Instantiate(operBtn_pre, contentPage_Field[pageNum].page_obj.transform);

                        //OperationItems �Ǵ� 
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

                            //Ÿ�� ��� 
                            contentPage_Field[pageNum].contents[j].operBtn.op_btn[i].type = xmlManager.dmContents_Field[pageNum].contents[j].operBtn.colume[i].OperationItem_type;
                        }

                        contentPage_Field[pageNum].contents[j].operBtn.title = xmlManager.dmContents_Field[pageNum].contents[j].operBtn.operTitle;
                        contentPage_Field[pageNum].contents[j].operBtn.subtitle = xmlManager.dmContents_Field[pageNum].contents[j].operBtn.operSubTitle;

                        string title = xmlManager.dmContents_Field[pageNum].contents[j].operBtn.operTitle;
                        string subTitle = xmlManager.dmContents_Field[pageNum].contents[j].operBtn.operSubTitle;
                        OperationSetting operSetting = contentPage_Field[pageNum].contents[j].operBtn;

                        //Debug.Log("Operation Title : " + title + " / Operation SubTitle : " + subTitle);
                        uiManager.detailSettingCanvas_Field.objectListSetting.OperationReady(title, subTitle, operSetting); //�𵨸� Ȱ��ȭ 

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

                                //Ŭ�� �̺�Ʈ �߰� 
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

                    //���̺��� ��� 
                    else if (xmlManager.dmContents_Field[pageNum].contents[j].data == Data.TABLE) //���� �ƴҶ� üũ�ϴ°� �� �����ִµ�?
                    {
                        spacingCheck = false;
                        //���̺� ���� ���� ������ null
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
                        int activeCnt_header = 0; //��Ȱ��ȭ�� Į���� ����� �ľ� (����� ���߱�����)
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


                                //���̺� ������ ������ ���� ������ ���� 
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

                            //���̺� ������ ���� ���� 
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
                        int activeCnt_content = 0; //��Ȱ��ȭ�� Į���� ����� �ľ� (����� ���߱�����)
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


                                //���̺� ������ ������ ���� ������ ���� 
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

                            //���̺� ������ ���� ���� 
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

                    //���̾�׷� 
                    else if (xmlManager.dmContents_Field[pageNum].contents[j].data == Data.DIAGRAM)
                    {
                        spacingCheck = false;
                        contentPage_Field[pageNum].contents[j].diagramZone = Instantiate(diagramZone_pre, contentPage_Field[pageNum].page_obj.transform);

                        //isolationStep or isolationProcedureEnd (End�� ������ ����� ����ȴ�. �׷��� - 1)
                        contentPage_Field[pageNum].contents[j].diagramSteps = new List<DiagramStep>(); //��µǴµ��� ����ؾߵȴ�.

                        int stepCnt = 0;
                        idCnt = 1;
                        NextStepSetting(pageNum, j, stepCnt, 0); //ù��° ������ �߰�                                         //stepCnt += 1;

                        while (true)
                        {
                            if (xmlManager.dmContents_Field[pageNum].contents[j].diagram[stepCnt].no_answer_id == null) break;

                            for (int z = 0; z < xmlManager.dmContents_Field[pageNum].contents[j].diagram.Count; z++) //������ ID���� ���� �߰� ����ߵȴ�. 
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

                    //īŻ�α��� ���(�𵨸�)
                    else if (xmlManager.dmContents_Field[pageNum].contents[j].data == Data.CATALOG)
                    {

                        spacingCheck = false;
                        //���̺� ���� ���� ������ null
                        contentPage_Field[pageNum].contents[j].catalog = Instantiate(catalog_pre, contentPage_Field[pageNum].page_obj.transform);

                        //���߰�(������� + 1)
                        contentPage_Field[pageNum].contents[j].catalog.CreateLine(xmlManager.dmContents_Field[pageNum].contents[j].catalog.lineCnt);
                        //�� �� ���� �� ������ŭ �÷� ����
                        for (int z = 0; z < contentPage_Field[pageNum].contents[j].catalog._lines.Length; z++)
                        {
                            contentPage_Field[pageNum].contents[j].catalog._lines[z].CreateColume(xmlManager.dmContents_Field[pageNum].contents[j].catalog.rowCnt);
                        }

                        //������ ���� (���پ� ������ ����)
                        for (int z = 0; z < contentPage_Field[pageNum].contents[j].catalog._lines.Length; z++)
                        {
                            //ù��° ī�װ� Ű���� ������ �𵨸��� �����Ѵ�. 
                            if (z == 1 && xmlManager.dmContents_Field[pageNum].contents[j].catalog.catalog_items[z].figureNumber == "01")
                            {
                                //z == 0�� ��� 
                                uiManager.detailSettingCanvas_Field.objectListSetting.ObjectSetting(xmlManager.dmContents_Field[pageNum].contents[j].catalog.catalog_items[z].subSystemCode,
                                   xmlManager.dmContents_Field[pageNum].contents[j].catalog.catalog_items[z].subsubSystemCode,
                                   xmlManager.dmContents_Field[pageNum].contents[j].catalog.catalog_items[z].assyCode, contentPage_Field[pageNum].contents[j].catalog);
                            }

                            //������ �� ���
                            contentPage_Field[pageNum].contents[j].catalog._lines[z].assyCode = xmlManager.dmContents_Field[pageNum].contents[j].catalog.catalog_items[z].assyCode;
                            contentPage_Field[pageNum].contents[j].catalog._lines[z].subSystemCode = xmlManager.dmContents_Field[pageNum].contents[j].catalog.catalog_items[z].subSystemCode;
                            contentPage_Field[pageNum].contents[j].catalog._lines[z].subsubSystemCode = xmlManager.dmContents_Field[pageNum].contents[j].catalog.catalog_items[z].subsubSystemCode;
                            contentPage_Field[pageNum].contents[j].catalog._lines[z].figureNumber = xmlManager.dmContents_Field[pageNum].contents[j].catalog.catalog_items[z].figureNumber;
                            contentPage_Field[pageNum].contents[j].catalog._lines[z].id = xmlManager.dmContents_Field[pageNum].contents[j].catalog.catalog_items[z].id;
                            contentPage_Field[pageNum].contents[j].catalog._lines[z].emp = xmlManager.dmContents_Field[pageNum].contents[j].catalog.catalog_items[z].emp;

                            //�� �÷� ������ ����
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

                            //���̺� ������ �߰� 
                            //����/�׸���ȣ
                            contentPage_Field[pageNum].contents[j].catalog._lines[z].child_colume[0]._text.text =
                                xmlManager.dmContents_Field[pageNum].contents[j].catalog.catalog_items[z].figureNumber;
                            //ǰ�� ��ȣ
                            contentPage_Field[pageNum].contents[j].catalog._lines[z].child_colume[1]._text.text =
                                xmlManager.dmContents_Field[pageNum].contents[j].catalog.catalog_items[z].itemSeqNumberValue;


                            //��������ȣ
                            contentPage_Field[pageNum].contents[j].catalog._lines[z].child_colume[3]._text.text = "";
                                //xmlManager.dmContents_Field[pageNum].contents[j].catalog.catalog_items[z].emp;
                            //������ ��ȣ
                            contentPage_Field[pageNum].contents[j].catalog._lines[z].child_colume[4]._text.text = "";
                                //xmlManager.dmContents_Field[pageNum].contents[j].catalog.catalog_items[z].manufacturerCodeValue;
                            //����
                            contentPage_Field[pageNum].contents[j].catalog._lines[z].child_colume[5]._text.text = "";
                            //xmlManager.dmContents_Field[pageNum].contents[j].catalog.catalog_items[z].descrForPart;
                            //������ ���� ����
                            contentPage_Field[pageNum].contents[j].catalog._lines[z].child_colume[6]._text.text = "";
                                //xmlManager.dmContents_Field[pageNum].contents[j].catalog.catalog_items[z].quantityPerNextHigherAssy;
                            //�ٿ����� ������ ��ȣ 
                            contentPage_Field[pageNum].contents[j].catalog._lines[z].child_colume[7]._text.text = "";
                                //xmlManager.dmContents_Field[pageNum].contents[j].catalog.catalog_items[z].sourceMaintRecoverability;

                            //header, ��ü�� ��� 
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
                            //���θ��� ��ư �̺�Ʈ�� ���� �𵨸� ���� ȿ�� �߰�
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

                                //��ǰ ���� �̺�Ʈ ���
                                contentPage_Field[pageNum].contents[j].catalog._lines[z].btn.onClick.AddListener(() =>
                                {
                                        tmp.ChildFunCheck(cnt); //Ŭ���� �길 Ȱ��ȭ ��ġ �Ұ��� 

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

        else if (oper == Operation.Unit) //�δ����� 
        {
            if (xmlManager.dmContents_Unit[pageNum].contents != null)
            {
                contentPage_Unit[pageNum].page_obj.gameObject.SetActive(true);

                Debug.Log(xmlManager.dmContents_Unit[pageNum].OwnDM.name);

                //�������� �����
                contentPage_Unit[pageNum].contents = new Content[xmlManager.dmContents_Unit[pageNum].contents.Count];

                //Figure ��� �ʿ����� üũ �Լ� 
                FigureControlCheck(pageNum);

                //contents �ϳ��� �ؽ�Ʈ �ϳ� �Ǵ� ���̺� �ϳ� 
                for (int j = 0; j < contentPage_Unit[pageNum].contents.Length; j++)
                {
                    contentPage_Unit[pageNum].contents[j].data = xmlManager.dmContents_Unit[pageNum].contents[j].data;
                    contentPage_Unit[pageNum].contents[j].isTitle = xmlManager.dmContents_Unit[pageNum].contents[j].isTitle;

                    //������϶� 
                    if (xmlManager.dmContents_Unit[pageNum].contents[j].data == Data.SPACING && !spacingCheck)
                    {
                        spacingCheck = true;
                        contentPage_Unit[pageNum].contents[j].spacing_obj = Instantiate(spacing_pre, contentPage_Unit[pageNum].page_obj.transform);
                    }

                    //text/warning text �� ��� 
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

                    //FolderContent �� ��� 
                    else if (xmlManager.dmContents_Unit[pageNum].contents[j].data == Data.FOLDERCONTENT)
                    {
                        spacingCheck = false;
                        //header 
                        contentPage_Unit[pageNum].contents[j].folderContent = Instantiate(folderContent_pre, contentPage_Unit[pageNum].page_obj.transform);

                        contentPage_Unit[pageNum].contents[j].folderContent.header_txt.text = xmlManager.dmContents_Unit[pageNum].contents[j].dmItem_text;

                        contentPage_Unit[pageNum].contents[j].folderContent.header_txt.fontSize = xmlManager.dmContents_Unit[pageNum].contents[j].textSize;
                        contentPage_Unit[pageNum].contents[j].folderContent.header_txt.color = xmlManager.dmContents_Unit[pageNum].contents[j].textColor;
                    }

                    //Figure �� ��� (�̹��� �Ǵ� ��ü) - ��ư ���� 
                    else if (xmlManager.dmContents_Unit[pageNum].contents[j].data == Data.FIGURE && !figureControl)
                    {
                        spacingCheck = false;
                        //header 
                        contentPage_Unit[pageNum].contents[j].figureButton = Instantiate(figureButton_pre, contentPage_Unit[pageNum].page_obj.transform);

                        contentPage_Unit[pageNum].contents[j].figureButton.button_txt.text = "<size=20>�׸� " + figureCnt + ". </size>" +
                            xmlManager.dmContents_Unit[pageNum].contents[j].figure.title + " ����";
                        figureCnt++;

                        contentPage_Unit[pageNum].contents[j].figureButton.id = xmlManager.dmContents_Unit[pageNum].contents[j].figure.id;
                        contentPage_Unit[pageNum].contents[j].figureButton.info = xmlManager.dmContents_Unit[pageNum].contents[j].figure.infoEntityIdent;

                        string id = contentPage_Unit[pageNum].contents[j].figureButton.id;
                        string info = contentPage_Unit[pageNum].contents[j].figureButton.info;
                        int tmp = pageNum;
                        int tmp_1 = j;

                        //��ư �̺�Ʈ ���
                        contentPage_Unit[pageNum].contents[j].figureButton.btn.onClick.AddListener(() =>
                        {
                            uiManager.detailSettingCanvas_Unit.objectListSetting.FigureSetting(id, info);

                            FigureButtonController(tmp, tmp_1);
                        });
                    }

                    //���� ���� ��ư�� ��� 
                    else if (xmlManager.dmContents_Unit[pageNum].contents[j].data == Data.OPERATION)
                    {
                        spacingCheck = false;
                        contentPage_Unit[pageNum].contents[j].operBtn = Instantiate(operBtn_pre, contentPage_Unit[pageNum].page_obj.transform);

                        //OperationItems �Ǵ� 
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

                            //Ÿ�� ��� 
                            contentPage_Unit[pageNum].contents[j].operBtn.op_btn[i].type = xmlManager.dmContents_Unit[pageNum].contents[j].operBtn.colume[i].OperationItem_type;
                        }

                        contentPage_Unit[pageNum].contents[j].operBtn.title = xmlManager.dmContents_Unit[pageNum].contents[j].operBtn.operTitle;
                        contentPage_Unit[pageNum].contents[j].operBtn.subtitle = xmlManager.dmContents_Unit[pageNum].contents[j].operBtn.operSubTitle;

                        string title = xmlManager.dmContents_Unit[pageNum].contents[j].operBtn.operTitle;
                        string subTitle = xmlManager.dmContents_Unit[pageNum].contents[j].operBtn.operSubTitle;
                        OperationSetting operSetting = contentPage_Unit[pageNum].contents[j].operBtn;

                        //Debug.Log("Operation Title : " + title + " / Operation SubTitle : " + subTitle);
                        uiManager.detailSettingCanvas_Unit.objectListSetting.OperationReady(title, subTitle, operSetting); //�𵨸� Ȱ��ȭ 

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

                                //Ŭ�� �̺�Ʈ �߰� 
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

                    //���̺��� ��� 
                    else if (xmlManager.dmContents_Unit[pageNum].contents[j].data == Data.TABLE) //���� �ƴҶ� üũ�ϴ°� �� �����ִµ�?
                    {
                        spacingCheck = false;
                        //���̺� ���� ���� ������ null
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
                        int activeCnt_header = 0; //��Ȱ��ȭ�� Į���� ����� �ľ� (����� ���߱�����)
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


                                //���̺� ������ ������ ���� ������ ���� 
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

                            //���̺� ������ ���� ���� 
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
                        int activeCnt_content = 0; //��Ȱ��ȭ�� Į���� ����� �ľ� (����� ���߱�����)
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


                                //���̺� ������ ������ ���� ������ ���� 
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

                            //���̺� ������ ���� ���� 
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

                    //���̾�׷� 
                    else if (xmlManager.dmContents_Unit[pageNum].contents[j].data == Data.DIAGRAM)
                    {
                        spacingCheck = false;
                        contentPage_Unit[pageNum].contents[j].diagramZone = Instantiate(diagramZone_pre, contentPage_Unit[pageNum].page_obj.transform);

                        //isolationStep or isolationProcedureEnd (End�� ������ ����� ����ȴ�. �׷��� - 1)
                        contentPage_Unit[pageNum].contents[j].diagramSteps = new List<DiagramStep>(); //��µǴµ��� ����ؾߵȴ�.

                        int stepCnt = 0;
                        idCnt = 1;
                        NextStepSetting(pageNum, j, stepCnt, 0); //ù��° ������ �߰� 
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

                    //īŻ�α��� ���(�𵨸�)
                    else if (xmlManager.dmContents_Unit[pageNum].contents[j].data == Data.CATALOG)
                    {
                        spacingCheck = false;
                        //���̺� ���� ���� ������ null
                        contentPage_Unit[pageNum].contents[j].catalog = Instantiate(catalog_pre, contentPage_Unit[pageNum].page_obj.transform);

                        //���߰�(������� + 1)
                        contentPage_Unit[pageNum].contents[j].catalog.CreateLine(xmlManager.dmContents_Unit[pageNum].contents[j].catalog.lineCnt);
                        //�� �� ���� �� ������ŭ �÷� ����
                        for (int z = 0; z < contentPage_Unit[pageNum].contents[j].catalog._lines.Length; z++)
                        {
                            contentPage_Unit[pageNum].contents[j].catalog._lines[z].CreateColume(xmlManager.dmContents_Unit[pageNum].contents[j].catalog.rowCnt);
                        }

                        //������ ���� (���پ� ������ ����)
                        for (int z = 0; z < contentPage_Unit[pageNum].contents[j].catalog._lines.Length; z++)
                        {
                            //ù��° ī�װ� Ű���� ������ �𵨸��� �����Ѵ�. 
                            if (z == 1 && xmlManager.dmContents_Unit[pageNum].contents[j].catalog.catalog_items[z].figureNumber == "01")
                            {
                                //z == 0�� ��� 
                                uiManager.detailSettingCanvas_Unit.objectListSetting.ObjectSetting(xmlManager.dmContents_Unit[pageNum].contents[j].catalog.catalog_items[z].subSystemCode,
                                   xmlManager.dmContents_Unit[pageNum].contents[j].catalog.catalog_items[z].subsubSystemCode,
                                   xmlManager.dmContents_Unit[pageNum].contents[j].catalog.catalog_items[z].assyCode, contentPage_Unit[pageNum].contents[j].catalog);
                            }

                            //������ �� ���
                            contentPage_Unit[pageNum].contents[j].catalog._lines[z].assyCode = xmlManager.dmContents_Unit[pageNum].contents[j].catalog.catalog_items[z].assyCode;
                            contentPage_Unit[pageNum].contents[j].catalog._lines[z].subSystemCode = xmlManager.dmContents_Unit[pageNum].contents[j].catalog.catalog_items[z].subSystemCode;
                            contentPage_Unit[pageNum].contents[j].catalog._lines[z].subsubSystemCode = xmlManager.dmContents_Unit[pageNum].contents[j].catalog.catalog_items[z].subsubSystemCode;
                            contentPage_Unit[pageNum].contents[j].catalog._lines[z].figureNumber = xmlManager.dmContents_Unit[pageNum].contents[j].catalog.catalog_items[z].figureNumber;
                            contentPage_Unit[pageNum].contents[j].catalog._lines[z].id = xmlManager.dmContents_Unit[pageNum].contents[j].catalog.catalog_items[z].id;
                            contentPage_Unit[pageNum].contents[j].catalog._lines[z].emp = xmlManager.dmContents_Unit[pageNum].contents[j].catalog.catalog_items[z].emp;

                            //�� �÷� ������ ����
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

                            //���̺� ������ �߰� 
                            //����/�׸���ȣ
                            contentPage_Unit[pageNum].contents[j].catalog._lines[z].child_colume[0]._text.text =
                                xmlManager.dmContents_Unit[pageNum].contents[j].catalog.catalog_items[z].figureNumber;
                            //ǰ�� ��ȣ
                            contentPage_Unit[pageNum].contents[j].catalog._lines[z].child_colume[1]._text.text =
                                xmlManager.dmContents_Unit[pageNum].contents[j].catalog.catalog_items[z].itemSeqNumberValue;

                            //��ǰ��ȣ
                            contentPage_Unit[pageNum].contents[j].catalog._lines[z].child_colume[2]._text.text = "";
                                //xmlManager.dmContents_Unit[pageNum].contents[j].catalog.catalog_items[z].partNumberValue;
                            //��������ȣ
                            contentPage_Unit[pageNum].contents[j].catalog._lines[z].child_colume[3]._text.text = "";
                            //xmlManager.dmContents_Unit[pageNum].contents[j].catalog.catalog_items[z].emp;
                            //������ ��ȣ
                            contentPage_Unit[pageNum].contents[j].catalog._lines[z].child_colume[4]._text.text = "";
                            //xmlManager.dmContents_Unit[pageNum].contents[j].catalog.catalog_items[z].manufacturerCodeValue;
                            //����
                            contentPage_Unit[pageNum].contents[j].catalog._lines[z].child_colume[5]._text.text = "";
                            //xmlManager.dmContents_Unit[pageNum].contents[j].catalog.catalog_items[z].descrForPart;
                            //������ ���� ����
                            contentPage_Unit[pageNum].contents[j].catalog._lines[z].child_colume[6]._text.text = "";
                            //xmlManager.dmContents_Unit[pageNum].contents[j].catalog.catalog_items[z].quantityPerNextHigherAssy;
                            //�ٿ����� ������ ��ȣ 
                            contentPage_Unit[pageNum].contents[j].catalog._lines[z].child_colume[7]._text.text = "";
                            //xmlManager.dmContents_Unit[pageNum].contents[j].catalog.catalog_items[z].sourceMaintRecoverability;

                            //header, ��ü�� ��� 
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
                            //���θ��� ��ư �̺�Ʈ�� ���� �𵨸� ���� ȿ�� �߰�
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

                                //��ǰ ���� �̺�Ʈ ���
                                contentPage_Unit[pageNum].contents[j].catalog._lines[z].btn.onClick.AddListener(() =>
                                {
                                    tmp.ChildFunCheck(cnt); //Ŭ���� �길 Ȱ��ȭ ��ġ �Ұ��� 

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

    int idCnt = 1; //�ʱⰪ

    //���̾�׷�
    void NextStepSetting(int pageNum, int contentNum, int stepNum, int xmlNum)
    {
        if (oper == Operation.Field)
        {
            contentPage_Field[pageNum].contents[contentNum].diagramSteps.Add(Instantiate(diagramStep_pre, contentPage_Field[pageNum].contents[contentNum].diagramZone.transform));

            contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].id = xmlManager.dmContents_Field[pageNum].contents[contentNum].diagram[xmlNum].id;

            //������ ����
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

                    //��ġ���� (��ũ����)
                    if (xmlManager.dmContents_Field[pageNum].contents[contentNum].diagram[xmlNum].actionItems[g].itemList == DiagramItemList.��ġ����)
                    {
                        contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].own_img.color = contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].set_back_color;
                        contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].para_txt.color = contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].set_text_color;

                        //DM ���� �Ұ� 
                        //if (xmlManager.dmContents_Field[pageNum].contents[contentNum].diagram[xmlNum].actionItems[g].linkDM != null) 
                        //{
                        //    contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].linkDM =
                        //        xmlManager.dmContents_Field[pageNum].contents[contentNum].diagram[xmlNum].actionItems[g].linkDM;
                        //}
                    }
                    else if (xmlManager.dmContents_Field[pageNum].contents[contentNum].diagram[xmlNum].actionItems[g].itemList == DiagramItemList.�������)
                    {
                        contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].own_img.color = contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].default_back_color;
                        contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].para_txt.color = contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].default_text_color;
                    }
                    else if (xmlManager.dmContents_Field[pageNum].contents[contentNum].diagram[xmlNum].actionItems[g].itemList == DiagramItemList.���)
                    {
                        contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].own_img.color = contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].default_back_color;
                        contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].para_txt.color = contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].default_text_color;
                    }
                }
            }
            //����Ʈ ����
            if (xmlManager.dmContents_Field[pageNum].contents[contentNum].diagram[xmlNum].quest_str != null)
            {
                contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].CreateDiagramQuest();
                contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramQuest.id = xmlManager.dmContents_Field[pageNum].contents[contentNum].diagram[xmlNum].id;
                contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramQuest.quest_txt.text = xmlManager.dmContents_Field[pageNum].contents[contentNum].diagram[xmlNum].quest_str;
            }
            //���� ����
            if (xmlManager.dmContents_Field[pageNum].contents[contentNum].diagram[xmlNum].yes_answer_id != null
                && xmlManager.dmContents_Field[pageNum].contents[contentNum].diagram[xmlNum].no_answer_id != null)
            {
                contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].CreateDiagramAnswer();
                contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramAnswer._yesText.text = "��";
                contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramAnswer._noText.text = "�ƴϿ�";
                contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramAnswer.yes_answer_id = xmlManager.dmContents_Field[pageNum].contents[contentNum].diagram[xmlNum].yes_answer_id;
                contentPage_Field[pageNum].contents[contentNum].diagramSteps[stepNum].diagramAnswer.no_answer_id = xmlManager.dmContents_Field[pageNum].contents[contentNum].diagram[xmlNum].no_answer_id;
            }
        }
        else if (oper == Operation.Unit)
        {
            contentPage_Unit[pageNum].contents[contentNum].diagramSteps.Add(Instantiate(diagramStep_pre, contentPage_Unit[pageNum].contents[contentNum].diagramZone.transform));

            contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].id = xmlManager.dmContents_Unit[pageNum].contents[contentNum].diagram[xmlNum].id;

            //������ ����
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

                    //��ġ���� (��ũ����)
                    if (xmlManager.dmContents_Unit[pageNum].contents[contentNum].diagram[xmlNum].actionItems[g].itemList == DiagramItemList.��ġ����)
                    {
                        contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].own_img.color = contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].set_back_color;
                        contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].para_txt.color = contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].set_text_color;

                        //DM ���� �Ұ� 
                        //if (xmlManager.dmContents_Unit[pageNum].contents[contentNum].diagram[xmlNum].actionItems[g].linkDM != null) 
                        //{
                        //    contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].linkDM =
                        //        xmlManager.dmContents_Unit[pageNum].contents[contentNum].diagram[xmlNum].actionItems[g].linkDM;
                        //}
                    }
                    else if (xmlManager.dmContents_Unit[pageNum].contents[contentNum].diagram[xmlNum].actionItems[g].itemList == DiagramItemList.�������)
                    {
                        contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].own_img.color = contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].default_back_color;
                        contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].para_txt.color = contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].default_text_color;
                    }
                    else if (xmlManager.dmContents_Unit[pageNum].contents[contentNum].diagram[xmlNum].actionItems[g].itemList == DiagramItemList.���)
                    {
                        contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].own_img.color = contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].default_back_color;
                        contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].para_txt.color = contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramItems[g].default_text_color;
                    }
                }
            }
            //����Ʈ ����
            if (xmlManager.dmContents_Unit[pageNum].contents[contentNum].diagram[xmlNum].quest_str != null)
            {
                contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].CreateDiagramQuest();
                contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramQuest.id = xmlManager.dmContents_Unit[pageNum].contents[contentNum].diagram[xmlNum].id;
                contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramQuest.quest_txt.text = xmlManager.dmContents_Unit[pageNum].contents[contentNum].diagram[xmlNum].quest_str;
            }
            //���� ����
            if (xmlManager.dmContents_Unit[pageNum].contents[contentNum].diagram[xmlNum].yes_answer_id != null
                && xmlManager.dmContents_Unit[pageNum].contents[contentNum].diagram[xmlNum].no_answer_id != null)
            {
                contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].CreateDiagramAnswer();
                contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramAnswer._yesText.text = "��";
                contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramAnswer._noText.text = "�ƴϿ�";
                contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramAnswer.yes_answer_id = xmlManager.dmContents_Unit[pageNum].contents[contentNum].diagram[xmlNum].yes_answer_id;
                contentPage_Unit[pageNum].contents[contentNum].diagramSteps[stepNum].diagramAnswer.no_answer_id = xmlManager.dmContents_Unit[pageNum].contents[contentNum].diagram[xmlNum].no_answer_id;
            }
        }
    }

    //���̺� �ʺ� ���� ���� 
    IEnumerator HeightSetting(int pageNum)
    {
        //������ ������ �Ǵ¼����� ��� ���´�. 
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

                    //����
                    if (contentPage_Field[pageNum].contents[j].spacing_obj != null)
                    {
                        spacingHeight = xmlManager.dmContents_Field[pageNum].contents[j].spacing;

                        contentPage_Field[pageNum].contents[j].spacing_obj.sizeDelta = new Vector2(contentPage_Field[pageNum].contents[j].spacing_obj.sizeDelta.x, spacingHeight);
                    }

                    //�ؽ�Ʈ�� ��� 
                    if (contentPage_Field[pageNum].contents[j].dmItem_str != null)
                    {
                        textHeight = contentPage_Field[pageNum].contents[j].dmItem_str.GetChild_height();

                        contentPage_Field[pageNum].contents[j].dmItem_str.rect_tr.sizeDelta = new Vector2(contentPage_Field[pageNum].contents[j].dmItem_str.rect_tr.sizeDelta.x, textHeight);
                    }
                    //���� �ؽ�Ʈ 
                    if (contentPage_Field[pageNum].contents[j].dmItem_warning_str != null)
                    {
                        textHeight = contentPage_Field[pageNum].contents[j].dmItem_warning_str.GetChild_height() + warningImage_height;

                        contentPage_Field[pageNum].contents[j].dmItem_warning_str.rect_tr.sizeDelta = new Vector2(contentPage_Field[pageNum].contents[j].dmItem_warning_str.rect_tr.sizeDelta.x, textHeight);
                    }
                    //��� �ؽ�Ʈ 
                    if (contentPage_Field[pageNum].contents[j].dmItem_caution_str != null)
                    {
                        textHeight = contentPage_Field[pageNum].contents[j].dmItem_caution_str.GetChild_height() + warningImage_height;

                        contentPage_Field[pageNum].contents[j].dmItem_caution_str.rect_tr.sizeDelta = new Vector2(contentPage_Field[pageNum].contents[j].dmItem_caution_str.rect_tr.sizeDelta.x, textHeight);
                    }
                    //��Ʈ �ؽ�Ʈ 
                    if (contentPage_Field[pageNum].contents[j].dmItem_note_str != null)
                    {
                        textHeight = contentPage_Field[pageNum].contents[j].dmItem_note_str.GetChild_height() + noteImage_height;

                        contentPage_Field[pageNum].contents[j].dmItem_note_str.rect_tr.sizeDelta = new Vector2(contentPage_Field[pageNum].contents[j].dmItem_note_str.rect_tr.sizeDelta.x, textHeight);
                    }

                    //���� ������ 
                    if (contentPage_Field[pageNum].contents[j].folderContent != null)
                    {
                        textHeight = contentPage_Field[pageNum].contents[j].folderContent.GetChild_height() + folderContent_height;

                        contentPage_Field[pageNum].contents[j].folderContent.rect_tr.sizeDelta = new Vector2(contentPage_Field[pageNum].contents[j].folderContent.rect_tr.sizeDelta.x, textHeight);
                    }

                    //figure ��ư�� ��� 
                    if (contentPage_Field[pageNum].contents[j].figureButton != null)
                    {
                        figureHeight = contentPage_Field[pageNum].contents[j].figureButton.GetChild_height();
                        //���̺� ���� ���� 
                        contentPage_Field[pageNum].contents[j].figureButton.rect_tr.sizeDelta = new Vector2(contentPage_Field[pageNum].contents[j].figureButton.rect_tr.sizeDelta.x, figureHeight);
                    }

                    //���̺��� ��� 
                    if (contentPage_Field[pageNum].contents[j].table != null)
                    {
                        tableHeight = contentPage_Field[pageNum].contents[j].table.GetChild_height();
                        //���̺� ���� ���� 
                        contentPage_Field[pageNum].contents[j].table.rec_tr.sizeDelta = new Vector2(contentPage_Field[pageNum].contents[j].table.rec_tr.sizeDelta.x, tableHeight);
                    }

                    //���� ���� ��ư�� ��� 
                    if (contentPage_Field[pageNum].contents[j].operBtn != null)
                    {
                        operBtnHeight = contentPage_Field[pageNum].contents[j].operBtn.GetChild_height();
                        contentPage_Field[pageNum].contents[j].operBtn.rec_tr.sizeDelta = new Vector2(contentPage_Field[pageNum].contents[j].operBtn.rec_tr.sizeDelta.x, operBtnHeight);
                    }

                    //���̾�׷��� ��� 
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
                                else //�⺻ ��
                                {
                                    contentPage_Field[pageNum].contents[j].diagramSteps[z].diagramItems[g].rec_tr.sizeDelta = new Vector2(contentPage_Field[pageNum].contents[j].diagramSteps[z].diagramItems[g].rec_tr.sizeDelta.x
                                        , 50.0f);
                                }
                            }

                            //�������� �ľ� ���� �ʿ� 
                            //if (contentPage_Field[pageNum].contents[j].diagramSteps[z].diagramQuest.GetChild_height() > 50.0f)
                            //{
                            //    contentPage_Field[pageNum].contents[j].diagramSteps[z].diagramQuest.rec_tr.sizeDelta = new Vector2(contentPage_Field[pageNum].contents[j].diagramSteps[z].diagramQuest.rec_tr.sizeDelta.x
                            //        , contentPage_Field[pageNum].contents[j].diagramSteps[z].diagramQuest.GetChild_height());
                            //}
                            //else //�⺻ ��
                            //{
                            //    contentPage_Field[pageNum].contents[j].diagramSteps[z].diagramQuest.rec_tr.sizeDelta = new Vector2(contentPage_Field[pageNum].contents[j].diagramSteps[z].diagramQuest.rec_tr.sizeDelta.x
                            //        , 50.0f);
                            //}

                            diagramHeight += contentPage_Field[pageNum].contents[j].diagramSteps[z].rec_tr.sizeDelta.y;
                            diagramHeight += 30.0f; //�����̽� �� �߰� 
                        }
                        contentPage_Field[pageNum].contents[j].diagramZone.GetComponent<RectTransform>().sizeDelta = new Vector2(
                            contentPage_Field[pageNum].contents[j].diagramZone.GetComponent<RectTransform>().sizeDelta.x, diagramHeight);
                    }

                    //īŻ�α��� ���  
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

                //�� ���� �ջ� ��ŭ ������ ���� ���� 
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

                    //����
                    if (contentPage_Unit[pageNum].contents[j].spacing_obj != null)
                    {
                        spacingHeight = xmlManager.dmContents_Unit[pageNum].contents[j].spacing;

                        contentPage_Unit[pageNum].contents[j].spacing_obj.sizeDelta = new Vector2(contentPage_Unit[pageNum].contents[j].spacing_obj.sizeDelta.x, spacingHeight);
                    }

                    //�ؽ�Ʈ�� ��� 
                    if (contentPage_Unit[pageNum].contents[j].dmItem_str != null)
                    {
                        textHeight = contentPage_Unit[pageNum].contents[j].dmItem_str.GetChild_height();

                        contentPage_Unit[pageNum].contents[j].dmItem_str.rect_tr.sizeDelta = new Vector2(contentPage_Unit[pageNum].contents[j].dmItem_str.rect_tr.sizeDelta.x, textHeight);
                    }
                    //���� �ؽ�Ʈ 
                    if (contentPage_Unit[pageNum].contents[j].dmItem_warning_str != null)
                    {
                        textHeight = contentPage_Unit[pageNum].contents[j].dmItem_warning_str.GetChild_height() + warningImage_height;

                        contentPage_Unit[pageNum].contents[j].dmItem_warning_str.rect_tr.sizeDelta = new Vector2(contentPage_Unit[pageNum].contents[j].dmItem_warning_str.rect_tr.sizeDelta.x, textHeight);
                    }
                    //��� �ؽ�Ʈ 
                    if (contentPage_Unit[pageNum].contents[j].dmItem_caution_str != null)
                    {
                        textHeight = contentPage_Unit[pageNum].contents[j].dmItem_caution_str.GetChild_height() + warningImage_height;

                        contentPage_Unit[pageNum].contents[j].dmItem_caution_str.rect_tr.sizeDelta = new Vector2(contentPage_Unit[pageNum].contents[j].dmItem_caution_str.rect_tr.sizeDelta.x, textHeight);
                    }
                    //��Ʈ �ؽ�Ʈ 
                    if (contentPage_Unit[pageNum].contents[j].dmItem_note_str != null)
                    {
                        textHeight = contentPage_Unit[pageNum].contents[j].dmItem_note_str.GetChild_height() + noteImage_height;

                        contentPage_Unit[pageNum].contents[j].dmItem_note_str.rect_tr.sizeDelta = new Vector2(contentPage_Unit[pageNum].contents[j].dmItem_note_str.rect_tr.sizeDelta.x, textHeight);
                    }

                    //���� ������ 
                    if (contentPage_Unit[pageNum].contents[j].folderContent != null)
                    {
                        textHeight = contentPage_Unit[pageNum].contents[j].folderContent.GetChild_height() + folderContent_height;

                        contentPage_Unit[pageNum].contents[j].folderContent.rect_tr.sizeDelta = new Vector2(contentPage_Unit[pageNum].contents[j].folderContent.rect_tr.sizeDelta.x, textHeight);
                    }

                    //figure ��ư�� ��� 
                    if (contentPage_Unit[pageNum].contents[j].figureButton != null)
                    {
                        figureHeight = contentPage_Unit[pageNum].contents[j].figureButton.GetChild_height();
                        //���̺� ���� ���� 
                        contentPage_Unit[pageNum].contents[j].figureButton.rect_tr.sizeDelta = new Vector2(contentPage_Unit[pageNum].contents[j].figureButton.rect_tr.sizeDelta.x, figureHeight);
                    }

                    //���̺��� ��� 
                    if (contentPage_Unit[pageNum].contents[j].table != null)
                    {
                        tableHeight = contentPage_Unit[pageNum].contents[j].table.GetChild_height();
                        //���̺� ���� ���� 
                        contentPage_Unit[pageNum].contents[j].table.rec_tr.sizeDelta = new Vector2(contentPage_Unit[pageNum].contents[j].table.rec_tr.sizeDelta.x, tableHeight);
                    }

                    //���� ���� ��ư�� ��� 
                    if (contentPage_Unit[pageNum].contents[j].operBtn != null)
                    {
                        operBtnHeight = contentPage_Unit[pageNum].contents[j].operBtn.GetChild_height();
                        //Debug.Log("oper Hieght : " + operBtnHeight);
                        contentPage_Unit[pageNum].contents[j].operBtn.rec_tr.sizeDelta = new Vector2(contentPage_Unit[pageNum].contents[j].operBtn.rec_tr.sizeDelta.x, operBtnHeight);
                    }

                    //���̾�׷��� ��� 
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
                                else //�⺻ ��
                                {
                                    contentPage_Unit[pageNum].contents[j].diagramSteps[z].diagramItems[g].rec_tr.sizeDelta = new Vector2(contentPage_Unit[pageNum].contents[j].diagramSteps[z].diagramItems[g].rec_tr.sizeDelta.x
                                        , 50.0f);
                                }
                            }

                            //�������� �ľ� ���� �ʿ� 
                            //if (contentPage_Unit[pageNum].contents[j].diagramSteps[z].diagramQuest.GetChild_height() > 50.0f)
                            //{
                            //    contentPage_Unit[pageNum].contents[j].diagramSteps[z].diagramQuest.rec_tr.sizeDelta = new Vector2(contentPage_Unit[pageNum].contents[j].diagramSteps[z].diagramQuest.rec_tr.sizeDelta.x
                            //        , contentPage_Unit[pageNum].contents[j].diagramSteps[z].diagramQuest.GetChild_height());
                            //}
                            //else //�⺻ ��
                            //{
                            //    contentPage_Unit[pageNum].contents[j].diagramSteps[z].diagramQuest.rec_tr.sizeDelta = new Vector2(contentPage_Unit[pageNum].contents[j].diagramSteps[z].diagramQuest.rec_tr.sizeDelta.x
                            //        , 50.0f);
                            //}

                            diagramHeight += contentPage_Unit[pageNum].contents[j].diagramSteps[z].rec_tr.sizeDelta.y;
                            diagramHeight += 30.0f; //�����̽� �� �߰� 
                        }
                        contentPage_Unit[pageNum].contents[j].diagramZone.GetComponent<RectTransform>().sizeDelta = new Vector2(
                            contentPage_Unit[pageNum].contents[j].diagramZone.GetComponent<RectTransform>().sizeDelta.x, diagramHeight);
                    }

                    //īŻ�α��� ���  
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

                //�� ���� �ջ� ��ŭ ������ ���� ���� 
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

            //Ȯ��/��� ��� �����·� �����ߵȴ�. 
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
                //������ ���õ� ������ (���� �����͸� ��Ȱ��ȭ)
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

            //Ȯ��/��� ��� �����·� �����ߵȴ�. 
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
                //������ ���õ� ������ (���� �����͸� ��Ȱ��ȭ)
                PreContentInit(chapter);

                scrollRect_content.content.anchoredPosition = Vector3.zero;
                scrollRect_content_rectr.anchoredPosition = Vector3.zero;
                content_box_rectr.anchoredPosition = reset_rectr;
            }
        }
    }


    //���������� �ҷ����� 
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
                        //ù��° ī�װ� Ű���� ������ �𵨸��� �����Ѵ�. 
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
                        //ù��° ī�װ� Ű���� ������ �𵨸��� �����Ѵ�. 
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


    //������ ��ũ�Ѻ� ��ġ ������ 
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

    //���ΰ��� ��ư Ŭ��
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

    //AR �ν� -> �󼼺��� (���� ������ ����)
    public IEnumerator FolderOpen(int pageNum, int folderNum)
    {
        yield return new WaitForSeconds(0.5f);
        int count = 0;

        if (oper == Operation.Field)
        {
            //���° �������� ã�ƾߵȴ�. 
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
            //���° �������� ã�ƾߵȴ�. 
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


    //Figure ��ư Ŭ���� ������ ��ư �ʱ�ȭ
    //trigger == true : ������ �����͵� �ʱ�ȭ 
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

    //Figure ��ư Ŭ���� ������ ��ư ���� 
    public void FigureButtonController(int pageNum, int buttonNum)
    {
        if (oper == Operation.Field)
        {
            if (contentPage_Field[pageNum].contents.Length == 0) return;

            if (contentPage_Field[pageNum].contents[buttonNum].figureButton.isActive)
            {
                FigureButtonInit(pageNum, true); //������ �����͵� �ʱ�ȭ 
                contentPage_Field[pageNum].contents[buttonNum].figureButton.ButtonStateDefault();
            }
            else
            {
                FigureButtonInit(pageNum, false); //������ �����ʹ� ���� 
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

    //���������� ��ư Ŭ�� 
    public void FolderContentButtonClick(int pageNum)
    {
        if (oper == Operation.Field)
        {
            if (contentPage_Field[pageNum].contents.Length == 0) return;

            int containerNum = -1;
            for (int j = 0; j < contentPage_Field[pageNum].contents.Length; j++)
            {
                //���� �������� ������ �� ������ �������� �״��� ������������ ������ �����ͱ��� ��Ƽ� ���� �Ѵ�. 
                if (contentPage_Field[pageNum].contents[j].folderContent != null)
                {
                    containerNum = j;

                    continue; //���� ����
                }

                if (containerNum != -1) //Data.TITLE�� ���� �׸� 
                {
                    if (contentPage_Field[pageNum].contents[j].isTitle) continue; //title�׸��� ���� ��Ų��. 

                    //����
                    if (contentPage_Field[pageNum].contents[j].data == Data.SPACING)
                    {
                        //Debug.Log(contentPage_Field[pageNum].contents[containerNum].folderContent.contents);

                        if (contentPage_Field[pageNum].contents[j].spacing_obj != null)
                        {
                            contentPage_Field[pageNum].contents[containerNum].folderContent.contents.Add(contentPage_Field[pageNum].contents[j].spacing_obj.gameObject);
                            contentPage_Field[pageNum].contents[j].spacing_obj.gameObject.SetActive(false);
                        }
                    }
                    //�ؽ�Ʈ�� ��� 
                    else if (contentPage_Field[pageNum].contents[j].data == Data.TEXT)
                    {
                        //�Ϲ� �ؽ�Ʈ 
                        if (contentPage_Field[pageNum].contents[j].dmItem_str != null)
                        {
                            contentPage_Field[pageNum].contents[containerNum].folderContent.contents.Add(contentPage_Field[pageNum].contents[j].dmItem_str.gameObject);
                            contentPage_Field[pageNum].contents[j].dmItem_str.gameObject.SetActive(false);
                        }
                        //���� �ؽ�Ʈ 
                        else if (contentPage_Field[pageNum].contents[j].dmItem_warning_str != null)
                        {
                            contentPage_Field[pageNum].contents[containerNum].folderContent.contents.Add(contentPage_Field[pageNum].contents[j].dmItem_warning_str.gameObject);
                            contentPage_Field[pageNum].contents[j].dmItem_warning_str.gameObject.SetActive(false);
                        }
                        //���� �ؽ�Ʈ 
                        else if (contentPage_Field[pageNum].contents[j].dmItem_caution_str != null)
                        {
                            contentPage_Field[pageNum].contents[containerNum].folderContent.contents.Add(contentPage_Field[pageNum].contents[j].dmItem_caution_str.gameObject);
                            contentPage_Field[pageNum].contents[j].dmItem_caution_str.gameObject.SetActive(false);
                        }
                        //��Ʈ �ؽ�Ʈ 
                        else if (contentPage_Field[pageNum].contents[j].dmItem_note_str != null)
                        {
                            contentPage_Field[pageNum].contents[containerNum].folderContent.contents.Add(contentPage_Field[pageNum].contents[j].dmItem_note_str.gameObject);
                            contentPage_Field[pageNum].contents[j].dmItem_note_str.gameObject.SetActive(false);
                        }
                    }
                    //Figure �ΰ�� 
                    else if (contentPage_Field[pageNum].contents[j].data == Data.FIGURE)
                    {
                        if (contentPage_Field[pageNum].contents[j].figureButton != null)
                        {
                            contentPage_Field[pageNum].contents[containerNum].folderContent.contents.Add(contentPage_Field[pageNum].contents[j].figureButton.gameObject);
                            contentPage_Field[pageNum].contents[j].figureButton.gameObject.SetActive(false);
                        }
                    }
                    //���̺��� ��� 
                    else if (contentPage_Field[pageNum].contents[j].data == Data.TABLE)
                    {
                        if (contentPage_Field[pageNum].contents[j].table != null)
                        {
                            contentPage_Field[pageNum].contents[containerNum].folderContent.contents.Add(contentPage_Field[pageNum].contents[j].table.gameObject);
                            contentPage_Field[pageNum].contents[j].table.gameObject.SetActive(false);
                        }
                    }
                    //���� ���� ��ư�� ��� 
                    else if (contentPage_Field[pageNum].contents[j].data == Data.OPERATION)
                    {
                        if (contentPage_Field[pageNum].contents[j].operBtn != null)
                        {
                            contentPage_Field[pageNum].contents[containerNum].folderContent.contents.Add(contentPage_Field[pageNum].contents[j].operBtn.gameObject);
                            contentPage_Field[pageNum].contents[j].operBtn.gameObject.SetActive(false);
                        }
                    }
                    //���̾�׷��� ��� 
                    else if (contentPage_Field[pageNum].contents[j].data == Data.DIAGRAM)
                    {
                        if (contentPage_Field[pageNum].contents[j].diagramZone != null)
                        {
                            contentPage_Field[pageNum].contents[containerNum].folderContent.contents.Add(contentPage_Field[pageNum].contents[j].diagramZone.gameObject);
                            contentPage_Field[pageNum].contents[j].diagramZone.gameObject.SetActive(false);
                        }
                    }
                    //īŻ�α��� ���  
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
                //���� �������� ������ �� ������ �������� �״��� ������������ ������ �����ͱ��� ��Ƽ� ���� �Ѵ�. 
                if (contentPage_Unit[pageNum].contents[j].folderContent != null)
                {
                    containerNum = j;

                    continue; //���� ����
                }

                if (containerNum != -1) //Data.TITLE�� ���� �׸� 
                {
                    if (contentPage_Unit[pageNum].contents[j].isTitle) continue; //title�׸��� ���� ��Ų��. 

                    //����
                    if (contentPage_Unit[pageNum].contents[j].data == Data.SPACING)
                    {
                        //Debug.Log(contentPage_Unit[pageNum].contents[containerNum].folderContent.contents);

                        if (contentPage_Unit[pageNum].contents[j].spacing_obj != null)
                        {
                            contentPage_Unit[pageNum].contents[containerNum].folderContent.contents.Add(contentPage_Unit[pageNum].contents[j].spacing_obj.gameObject);
                            contentPage_Unit[pageNum].contents[j].spacing_obj.gameObject.SetActive(false);
                        }
                    }
                    //�ؽ�Ʈ�� ��� 
                    else if (contentPage_Unit[pageNum].contents[j].data == Data.TEXT)
                    {
                        //�Ϲ� �ؽ�Ʈ 
                        if (contentPage_Unit[pageNum].contents[j].dmItem_str != null)
                        {
                            contentPage_Unit[pageNum].contents[containerNum].folderContent.contents.Add(contentPage_Unit[pageNum].contents[j].dmItem_str.gameObject);
                            contentPage_Unit[pageNum].contents[j].dmItem_str.gameObject.SetActive(false);
                        }
                        //���� �ؽ�Ʈ 
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
                        //��Ʈ �ؽ�Ʈ 
                        else if (contentPage_Unit[pageNum].contents[j].dmItem_note_str != null)
                        {
                            contentPage_Unit[pageNum].contents[containerNum].folderContent.contents.Add(contentPage_Unit[pageNum].contents[j].dmItem_note_str.gameObject);
                            contentPage_Unit[pageNum].contents[j].dmItem_note_str.gameObject.SetActive(false);
                        }
                    }
                    //Figure �ΰ�� 
                    else if (contentPage_Unit[pageNum].contents[j].data == Data.FIGURE)
                    {
                        if (contentPage_Unit[pageNum].contents[j].figureButton != null)
                        {
                            contentPage_Unit[pageNum].contents[containerNum].folderContent.contents.Add(contentPage_Unit[pageNum].contents[j].figureButton.gameObject);
                            contentPage_Unit[pageNum].contents[j].figureButton.gameObject.SetActive(false);
                        }
                    }
                    //���̺��� ��� 
                    else if (contentPage_Unit[pageNum].contents[j].data == Data.TABLE)
                    {
                        if (contentPage_Unit[pageNum].contents[j].table != null)
                        {
                            contentPage_Unit[pageNum].contents[containerNum].folderContent.contents.Add(contentPage_Unit[pageNum].contents[j].table.gameObject);
                            contentPage_Unit[pageNum].contents[j].table.gameObject.SetActive(false);
                        }
                    }
                    //���� ���� ��ư�� ��� 
                    else if (contentPage_Unit[pageNum].contents[j].data == Data.OPERATION)
                    {
                        if (contentPage_Unit[pageNum].contents[j].operBtn != null)
                        {
                            contentPage_Unit[pageNum].contents[containerNum].folderContent.contents.Add(contentPage_Unit[pageNum].contents[j].operBtn.gameObject);
                            contentPage_Unit[pageNum].contents[j].operBtn.gameObject.SetActive(false);
                        }
                    }
                    //���̾�׷��� ��� 
                    else if (contentPage_Unit[pageNum].contents[j].data == Data.DIAGRAM)
                    {
                        if (contentPage_Unit[pageNum].contents[j].diagramZone != null)
                        {
                            contentPage_Unit[pageNum].contents[containerNum].folderContent.contents.Add(contentPage_Unit[pageNum].contents[j].diagramZone.gameObject);
                            contentPage_Unit[pageNum].contents[j].diagramZone.gameObject.SetActive(false);
                        }
                    }
                    //īŻ�α��� ���  
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
