using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoreManager : MonoBehaviour
{
    [Serializable]
    public class CoreItem
    {
        public Button plusButton = null;

        public Button minusButton = null;

        public Text text = null;

        private int count = 0;
        public int Count
        {
            set
            {
                count = value;
                text.text = count.ToString();
            }
            get
            {
                return count;
            }
        }

        public bool isSoul = false;

        public void SetCoreItem(CoreData coreData)
        {
            Count = coreData.count;
        }

        public void SetActiveToButton(bool isActive)
        {
            plusButton.gameObject.SetActive(isActive);
            minusButton.gameObject.SetActive(isActive);
        }
    }

    [Serializable]
    public class CardCoreItem
    {
        public List<CoreItem> coreItem = new List<CoreItem>();

        public void SetActiveToButtonList(bool isActive)
        {
            foreach (var coreItem in this.coreItem)
            {
                coreItem.SetActiveToButton(isActive);
            }
        }
    }

    [Serializable]
    public class CoreData
    {
        public int count = 0;

        public bool isSoul = false;

        public CoreData(CoreItem coreItem)
        {
            count = coreItem.Count;
            isSoul = coreItem.isSoul;
        }
    }

    [Serializable]
    public class CoreDataList
    {
        public List<CoreData> coreDataList = new List<CoreData>();

        public CoreDataList(List<CoreItem> coreItemList)
        {
            foreach (var coreItem in coreItemList)
            {
                coreDataList.Add(new CoreData(coreItem));
            }
        }
    }

    [Serializable]
    public class CoreManagerData
    {
        public CoreDataList coreDataToLife = null;

        public CoreDataList coreDataToReserve = null;

        public CoreDataList coreDataToTrash = null;

        public List<CoreDataList> coreDataToCardList = new List<CoreDataList>();
    }

    [SerializeField]
    private List<CoreItem> m_coreItemToLife = new List<CoreItem>();

    [SerializeField]
    private List<CoreItem> m_coreItemToReserve = new List<CoreItem>();

    [SerializeField]
    private List<CoreItem> m_coreItemToTrash = new List<CoreItem>();

    [SerializeField]
    private List<CardCoreItem> m_coreItemToCardList = new List<CardCoreItem>();

    public void Awake()
    {
        foreach (var coreItem in m_coreItemToLife)
        {
            coreItem.plusButton.onClick.AddListener(() => {
                coreItem.Count++;
            });
            coreItem.minusButton.onClick.AddListener(() => {
                coreItem.Count--;
            });
        }

        foreach (var coreItem in m_coreItemToReserve)
        {
            coreItem.plusButton.onClick.AddListener(() => {
                coreItem.Count++;
            });
            coreItem.minusButton.onClick.AddListener(() => {
                coreItem.Count--;
            });
        }

        foreach (var coreItem in m_coreItemToTrash)
        {
            coreItem.plusButton.onClick.AddListener(() => {
                coreItem.Count++;
            });
            coreItem.minusButton.onClick.AddListener(() => {
                coreItem.Count--;
            });
        }

        foreach (var coreItemToCard in m_coreItemToCardList)
        {
            foreach (var coreItem in coreItemToCard.coreItem)
            {
                coreItem.plusButton.onClick.AddListener(() => {
                    coreItem.Count++;
                });
                coreItem.minusButton.onClick.AddListener(() => {
                    coreItem.Count--;
                });
            }
        }
    }

    public void InitSetting()
    {
        foreach (var coreItem in m_coreItemToLife)
        {
            if (coreItem.isSoul)
            {
                continue;
            }

            coreItem.Count = 5;
        }

        foreach (var coreItem in m_coreItemToReserve)
        {
            int count = 3;
            if (coreItem.isSoul)
            {
                count = 1;
            }

            coreItem.Count = count;
        }
    }

    public void SetActiveToButton(bool isActive)
    {
        foreach (var coreItem in m_coreItemToLife)
        {
            coreItem.SetActiveToButton(isActive);
        }

        foreach (var coreItem in m_coreItemToReserve)
        {
            coreItem.SetActiveToButton(isActive);
        }

        foreach (var coreItem in m_coreItemToTrash)
        {
            coreItem.SetActiveToButton(isActive);
        }

        foreach (var coreItemToCard in m_coreItemToCardList)
        {
            coreItemToCard.SetActiveToButtonList(isActive);
        }
    }

    public void SetCoreManagerDataJson(string coreManagerDataJson)
    {
        CoreManagerData coreManagerData = JsonUtility.FromJson<CoreManagerData>(coreManagerDataJson);

        for (var index = 0; index < m_coreItemToLife.Count; index++)
        {
            foreach (var coreData in coreManagerData.coreDataToLife.coreDataList)
            {
                if (coreData.isSoul != m_coreItemToLife[index].isSoul)
                {
                    continue;
                }

                m_coreItemToLife[index].SetCoreItem(coreData);
            }
        }

        for (var index = 0; index < m_coreItemToReserve.Count; index++)
        {
            foreach (var coreData in coreManagerData.coreDataToReserve.coreDataList)
            {
                if (coreData.isSoul != m_coreItemToReserve[index].isSoul)
                {
                    continue;
                }

                m_coreItemToReserve[index].SetCoreItem(coreData);
            }
        }

        for (var index = 0; index < m_coreItemToTrash.Count; index++)
        {
            foreach (var coreData in coreManagerData.coreDataToTrash.coreDataList)
            {
                if (coreData.isSoul != m_coreItemToTrash[index].isSoul)
                {
                    continue;
                }

                m_coreItemToTrash[index].SetCoreItem(coreData);
            }
        }

        for (var index = 0; index < m_coreItemToCardList.Count; index++)
        {
            var coreDataList = coreManagerData.coreDataToCardList[index];
            for (var count = 0; count < m_coreItemToCardList[index].coreItem.Count; count++)
            {
                foreach (var coreData in coreDataList.coreDataList)
                {
                    if (coreData.isSoul != m_coreItemToCardList[index].coreItem[count].isSoul)
                    {
                        continue;
                    }

                    m_coreItemToCardList[index].coreItem[count].SetCoreItem(coreData);
                }
            }
        }
    }

    public string GetCoreManagerDataJson()
    {
        CoreManagerData coreManagerData = new CoreManagerData();

        coreManagerData.coreDataToLife = new CoreDataList(m_coreItemToLife);

        coreManagerData.coreDataToReserve = new CoreDataList(m_coreItemToReserve);

        coreManagerData.coreDataToTrash = new CoreDataList(m_coreItemToTrash);

        foreach (var coreItemToCard in m_coreItemToCardList)
        {
            coreManagerData.coreDataToCardList.Add(new CoreDataList(coreItemToCard.coreItem));
        }

        var json = JsonUtility.ToJson(coreManagerData);
        return json;
    }
}
