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
    public Image OverheadImage;
    public Sprite ProtectMagicSprite;
    public Sprite ProtectRangedSprite;
    public Sprite ProtectMeleeSprite;

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
        switch (prayer)
        {
            case ProtectPrayer.None:
                OverheadTransform.anchoredPosition = new Vector2(-10, -10);
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
