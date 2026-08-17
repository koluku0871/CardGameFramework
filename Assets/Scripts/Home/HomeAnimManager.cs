using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class HomeAnimManager : MonoBehaviour
{
    [SerializeField]
    private Image m_bgImage = null;

    [SerializeField]
    private Image m_titleIconImage = null;

    [SerializeField]
    private Image m_userIconImage = null;

    [SerializeField]
    private TMPro.TextMeshProUGUI m_fpsText = null;

    [SerializeField]
    private TMPro.TextMeshProUGUI m_userNameText = null;

    [SerializeField]
    private TMPro.TextMeshProUGUI m_vText = null;

    [SerializeField]
    private TMPro.TextMeshProUGUI m_logContentText = null;

    [SerializeField]
    private Image m_logContentImage = null;

    [SerializeField]
    private List<Button> m_buttonList = new List<Button> ();

    [SerializeField]
    private List<Image> m_buttonImageList = new List<Image>();

    [SerializeField]
    private List<Image> m_buttonImage2List = new List<Image>();

    private void Awake()
    {
        m_bgImage.fillAmount = 0;
        m_titleIconImage.fillAmount = 0;
        m_userIconImage.fillAmount = 0;

        m_fpsText.gameObject.SetActive(false);
        m_userNameText.gameObject.SetActive(false);
        m_vText.gameObject.SetActive(false);

        m_logContentText.gameObject.SetActive(false);

        m_logContentImage.rectTransform.sizeDelta = new Vector2(m_logContentImage.rectTransform.sizeDelta.x, 0);

        foreach (Button button in m_buttonList)
        {
            button.image.fillAmount = 0;
            button.enabled = false;
        }
        foreach (Image image in m_buttonImageList)
        {
            image.fillAmount = 0;
        }
        foreach (Image image in m_buttonImage2List)
        {
            SetImageColorA(image, 0);
        }
    }

    private void Start()
    {
        Task thread1 = Thread1();
        Task thread2 = Thread2();
    }

    public async Task Thread1()
    {
        int count = 0;
        while (count < 10)
        {
            m_bgImage.fillAmount += 0.1f;

            await Task.Delay(1);
            count++;
        }

        m_bgImage.fillAmount = 1;

        count = 0;
        while (count < 10)
        {
            m_titleIconImage.fillAmount += 0.1f;

            await Task.Delay(1);
            count++;
        }

        m_titleIconImage.fillAmount = 1;

        count = 0;
        while (count < 10)
        {
            m_userIconImage.fillAmount += 0.1f;

            await Task.Delay(1);
            count++;
        }

        m_fpsText.gameObject.SetActive(true);
        m_userNameText.gameObject.SetActive(true);
        m_vText.gameObject.SetActive(true);

        m_logContentText.gameObject.SetActive(true);

        count = 0;
        while (count < 10)
        {
            m_logContentImage.rectTransform.sizeDelta = new Vector2(
                m_logContentImage.rectTransform.sizeDelta.x,
                m_logContentImage.rectTransform.sizeDelta.y + 40
            );

            await Task.Delay(1);
            count++;
        }
    }

    public async Task Thread2()
    {
        int count = 0;

        foreach (Button button in m_buttonList)
        {
            while (count < 3)
            {
                button.image.fillAmount += 0.33f;
                await Task.Delay(1);
                count++;
            }
            count = 0;
        }

        foreach (Button button in m_buttonList)
        {
            button.image.fillAmount = 1;
        }

        count = 0;
        while (count < 10)
        {
            foreach (Image image in m_buttonImageList)
            {
                image.fillAmount += 0.1f;
                await Task.Delay(1);
                count++;
            }
        }

        foreach (Image image in m_buttonImageList)
        {
            image.fillAmount = 1;
        }

        count = 0;
        while (count < 10)
        {
            foreach (Image image in m_buttonImage2List)
            {
                SetImageColorA(image, image.color.a + 0.1f);
                await Task.Delay(1);
                count++;
            }
        }

        foreach (Image image in m_buttonImage2List)
        {
            SetImageColorA(image, 1);
        }

        await Task.Delay(1);

        foreach (Button button in m_buttonList)
        {
            button.enabled = true;
        }
    }

    public void SetImageColorA(Image image, float a)
    {
        Color color = image.color;
        color.a = a;
        image.color = color;
    }
}
