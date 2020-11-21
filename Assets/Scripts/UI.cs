using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public GameObject PrayerPanel;
    public GameObject ProtectMagicOn;
    public GameObject ProtectRangedOn;
    public GameObject ProtectMeleeOn;
    public RectTransform OverheadTransform;
    public GameObject OverheadPrayer;
    public Image OverheadImage;
    public Sprite ProtectMagicSprite;
    public Sprite ProtectRangedSprite;
    public Sprite ProtectMeleeSprite;
    public Slider HealthSlider;

    public void ShowPrayerPanel(bool doShow)
    {
        PrayerPanel.SetActive(doShow);
    }

    public void SetClientProtectPrayer(ProtectPrayer prayer)
    {
        ProtectMagicOn.SetActive(false);
        ProtectRangedOn.SetActive(false);
        ProtectMeleeOn.SetActive(false);

        switch (prayer)
        {
            case ProtectPrayer.Magic:
                ProtectMagicOn.SetActive(true);
                break;
            case ProtectPrayer.Ranged:
                ProtectRangedOn.SetActive(true);
                break;
            case ProtectPrayer.Melee:
                ProtectMeleeOn.SetActive(true);
                break;
        }
    }

    public void SetServerProtectPrayer(ProtectPrayer prayer, Vector2 pos)
    {
        OverheadTransform.anchoredPosition = pos;
        OverheadPrayer.SetActive(true);
        switch (prayer)
        {
            case ProtectPrayer.None:
                OverheadPrayer.SetActive(false);
                break;
            case ProtectPrayer.Magic:
                OverheadImage.sprite = ProtectMagicSprite;
                break;
            case ProtectPrayer.Ranged:
                OverheadImage.sprite = ProtectRangedSprite;
                break;
            case ProtectPrayer.Melee:
                OverheadImage.sprite = ProtectMeleeSprite;
                break;
        }
    }

    public void SetHealth(int health)
    {
        HealthSlider.value = health;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
