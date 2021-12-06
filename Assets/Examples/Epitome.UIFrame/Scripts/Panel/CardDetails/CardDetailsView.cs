using UnityEngine.UI;
using System;
using UnityEngine;
using System.Collections.Generic;
using Epitome;

/// <summary>卡牌详情视图</summary>
public class CardDetailsView : BaseView
{
    /// <summary>属性</summary>
    public Toggle attributeTog;

    /// <summary>技能</summary>
    public Toggle skillTog;

    /// <summary>关闭</summary>
    public Button closButton;

    // ========== 属性

    private Text cardName;
    private Text description;

    private GameObject[,] attributeValue;

    // ========== 技能

    private GameObject[] skills;
    private Image[] skillImage;
    private Text[] skillName;
    private Text[] skillDescription;

    private List<GameObject[]> skillValue;

    // ========== 节点

    private GameObject attributeShow;
    private GameObject skillShow;

    private CardData cardData;

    protected override void Awake()
    {
        // 监听选项卡按钮
        attributeTog.onValueChanged.AddListener((bool value) => { CardAttributeSkill("Attribute", value); });
        skillTog.onValueChanged.AddListener((bool value) => { CardAttributeSkill("Skill", value); });

        // 关闭面板
        closButton.onClick.AddListener(() => {
            UIControl.CloseUI(UIPanelType.CardDetailsPanel);
            attributeTog.isOn = true;
            skillTog.isOn = false;
        });

        attributeShow = transform.Find("ContentPanel/AttributeShow").gameObject;
        skillShow = transform.Find("ContentPanel/SkillShow").gameObject;

        // 获取属性组件
        cardName = attributeShow.transform.Find("Name").GetComponent<Text>();
        description = attributeShow.transform.Find("Description").GetComponent<Text>();
        attributeValue = new GameObject[4, 6];
        for (int j = 1; j <= 4; j++)
        {
            for (int i = 1; i <= 6; i++)
            {
                attributeValue[j - 1, i - 1] = attributeShow.transform.Find(string.Format("capacity/{0}/Image/{1}", j, i)).gameObject;
            }
        }

        // 获取技能组件
        skills = new GameObject[3];
        skillImage = new Image[3];
        skillName = new Text[3];
        skillDescription = new Text[3];
        skillValue = new List<GameObject[]>();
        for (int i = 1; i <= 3; i++)
        {
            skills[i - 1] = skillShow.transform.Find(string.Format("Skill{0}", i)).gameObject;
            skillImage[i - 1] = skillShow.transform.Find(string.Format("Skill{0}/Image", i)).GetComponent<Image>();
            skillName[i - 1] = skillShow.transform.Find(string.Format("Skill{0}/Name", i)).GetComponent<Text>();
            skillDescription[i - 1] = skillShow.transform.Find(string.Format("Skill{0}/Description", i)).GetComponent<Text>();
            skillValue.Add(new GameObject[3]);
            for (int j = 1; j <= 3; j++)
            {
                skillValue[i - 1][j - 1] = skillShow.transform.Find(string.Format("Skill{0}/grade/{1}", i, j)).gameObject;
            }
        }

        MessageCenter.Instance.AddListener("CardDetailsPanelOpen", CardDetailsPanelOpen);
    }

    protected override void OnDestroy()
    {
        MessageCenter.Instance.RemoveListener("CardDetailsPanelOpen", CardDetailsPanelOpen);
    }

    /// <summary>卡牌属性面板打开</summary>
    private void CardDetailsPanelOpen(object obj)
    {
        cardData = CardDataManage.Instance.selectCardUnit.cardData;

        CardDataManage.Instance.CancelSelectCard();

        CardAttributeSkill("Attribute", true);
    }

    /// <summary>切换属性技能</summary>
    private void CardAttributeSkill(string str, bool value)
    {
        SwitchAttributeSkill(str);

        // 刷新内容
        switch (str)
        {
            case "Attribute":
                cardName.text = cardData.cardName;
                description.text = cardData.cardDescription;
                SetAttributeValue(cardData.cardAttribute);
                break;
            case "Skill":
                SetCardSkillValue(cardData.cardSkills);
                break;
        }
    }

    /// <summary>设置卡牌属性值</summary>
    private void SetAttributeValue(CardAttribute attribute)
    {
        int[] value = new int[] { attribute.attackValue, attribute.defenseValue, attribute.evasionValue, attribute.critValue };
        for (int j = 0; j < value.Length; j++)
        {
            for (int i = 0; i < value[j]; i++)
            {
                attributeValue[j, i].SetActive(i <= value[j] ? true : false);
            }
        }
    }

    /// <summary>设置卡牌技能数据</summary>
    private void SetCardSkillValue(List<CardSkill> cardSkills)
    {
        // 显示技能个数
        for (int i = 0; i < 3; i++)
        {
            skills[i].SetActive(i >= cardSkills.Count ? false : true);

            if (i < cardSkills.Count)
            {
                // skillImage[i].sprite = null;
                skillName[i].text = cardSkills[i].skillName;
                skillDescription[i].text = cardSkills[i].skillDescription;
                
                for (int j = 0; j < 3; j++)
                {
                    skillValue[i][j].SetActive(j >= cardSkills[i].skillGrade ? false : true);
                }
            }
        }
    }

    /// <summary>切换属性技能</summary>
    private void SwitchAttributeSkill(string str)
    {
        attributeShow.SetActive(str == "Attribute" ? true : false);
        skillShow.SetActive(str == "Skill" ? true : false);
    }
}