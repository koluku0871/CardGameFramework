using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class RoomAnimManager : MonoBehaviour
{
    [SerializeField]
    private Image m_line1Image = null;
    [SerializeField]
    private Image m_line2Image = null;
    [SerializeField]
    private Image m_line3Image = null;
    [SerializeField]
    private Image m_line4Image = null;
    [SerializeField]
    private Image m_line5Image = null;
    [SerializeField]
    private Image m_line6Image = null;
    [SerializeField]
    private Image m_line7Image = null;
    [SerializeField]
    private Image m_line8Image = null;

    [SerializeField]
    private Image m_line9Image = null;

    [SerializeField]
    private Image m_area1Image = null;
    [SerializeField]
    private Image m_area2Image = null;
    [SerializeField]
    private Image m_area3Image = null;
    [SerializeField]
    private Image m_area4Image = null;

    private Vector3 m_area1ImageScale = new Vector3();
    private Vector3 m_area2ImageScale = new Vector3();
    private Vector3 m_area3ImageScale = new Vector3();
    private Vector3 m_area4ImageScale = new Vector3();

    private void Awake()
    {
        m_area1ImageScale = m_area1Image.rectTransform.localScale;
        m_area2ImageScale = m_area2Image.rectTransform.localScale;
        m_area3ImageScale = m_area3Image.rectTransform.localScale;
        m_area4ImageScale = m_area4Image.rectTransform.localScale;

        m_line1Image.fillAmount = 0;
        m_line2Image.fillAmount = 0;
        m_line3Image.fillAmount = 0;
        m_line4Image.fillAmount = 0;
        m_line5Image.fillAmount = 0;
        m_line6Image.fillAmount = 0;
        m_line7Image.fillAmount = 0;
        m_line8Image.fillAmount = 0;

        m_line9Image.fillAmount = 0;
        m_line9Image.gameObject.SetActive(false);
    }

    private void Start()
    {
        Task thread = Thread1();
    }

    public async Task Thread1()
    {
        int count = 0;
        while (count < 100)
        {
            if (m_line1Image.fillAmount < 1)
            {
                m_line1Image.fillAmount += 0.03f;
            }
            if (count > 1 && m_line2Image.fillAmount < 1)
            {
                m_line2Image.fillAmount += 0.06f;
            }
            if (count > 2 && m_line3Image.fillAmount < 1)
            {
                m_line3Image.fillAmount += 0.06f;
            }
            if (count > 5 && m_line4Image.fillAmount < 1)
            {
                m_line4Image.fillAmount += 0.06f;
            }
            if (count > 20 && m_line7Image.fillAmount < 1)
            {
                m_line7Image.fillAmount += 0.08f;
            }
            if (count > 20 && m_line6Image.fillAmount < 1)
            {
                m_line6Image.fillAmount += 0.04f;
            }
            if (count > 20 && m_line5Image.fillAmount < 1)
            {
                m_line5Image.fillAmount += 0.04f;
            }
            if (count > 25 && m_line8Image.fillAmount < 1)
            {
                m_line8Image.fillAmount += 0.16f;
            }

            if (count > 25 && m_area1Image.rectTransform.localScale.y > 0)
            {
                m_area1Image.rectTransform.localScale = new Vector3(
                    m_area1ImageScale.x, m_area1Image.rectTransform.localScale.y - 0.04f
                );
            }
            if (count > 40 && m_area2Image.rectTransform.localScale.x > 0)
            {
                m_area2Image.rectTransform.localScale = new Vector3(
                    m_area2Image.rectTransform.localScale.x - 0.04f, m_area2ImageScale.y
                );
            }

            await Task.Delay(1);
            count++;
        }

        m_line1Image.fillAmount = 1;
        m_line2Image.fillAmount = 1;
        m_line3Image.fillAmount = 1;
        m_line4Image.fillAmount = 1;
        m_line5Image.fillAmount = 1;
        m_line6Image.fillAmount = 1;
        m_line7Image.fillAmount = 1;
        m_line8Image.fillAmount = 1;
    }

    public void SetActiveToRoomContents(bool isActive)
    {
        Task thread = Thread2(isActive);
    }

    public async Task Thread2(bool isActive)
    {
        if (isActive)
        {
            m_line9Image.gameObject.SetActive(true);

            m_area2Image.rectTransform.localScale = m_area2ImageScale;
        }

        int count = 0;
        while (count < 30)
        {
            if (isActive)
            {
                if (m_line9Image.fillAmount < 1)
                {
                    m_line9Image.fillAmount += 0.03f;
                }

                if (m_area3Image.rectTransform.localScale.x > 0)
                {
                    m_area3Image.rectTransform.localScale = new Vector3(
                        m_area3Image.rectTransform.localScale.x - 0.04f, m_area3ImageScale.y
                    );
                }
                if (m_area4Image.rectTransform.localScale.x > 0)
                {
                    m_area4Image.rectTransform.localScale = new Vector3(
                        m_area4Image.rectTransform.localScale.x - 0.04f, m_area4ImageScale.y
                    );
                }
            }
            else
            {
                if (m_line9Image.fillAmount > 0)
                {
                    m_line9Image.fillAmount -= 0.03f;
                }

                if (m_area3Image.rectTransform.localScale.x < m_area3ImageScale.x)
                {
                    m_area3Image.rectTransform.localScale = new Vector3(
                        m_area3Image.rectTransform.localScale.x + 0.04f, m_area3ImageScale.y
                    );
                }
                if (m_area4Image.rectTransform.localScale.x < m_area4ImageScale.x)
                {
                    m_area4Image.rectTransform.localScale = new Vector3(
                        m_area4Image.rectTransform.localScale.x + 0.04f, m_area4ImageScale.y
                    );
                }
            }
            await Task.Delay(1);
            count++;
        }

        if (isActive == false)
        {
            m_line9Image.gameObject.SetActive(false);

            m_area2Image.rectTransform.localScale = new Vector3(m_area2ImageScale.x, 0);
        }
    }
}
