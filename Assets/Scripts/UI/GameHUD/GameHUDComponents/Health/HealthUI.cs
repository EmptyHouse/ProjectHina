using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    [SerializeField]
    private Image HealthBarSegmentPrefab;
    [SerializeField]
    private RectTransform DefaultHealthContainer;
    [SerializeField]
    private float MarginSpacing = 10f;

    private int CachedMaxHealth;
    private int CachedCurrentHealth;
    private List<Image> HealthBarImages = new List<Image>();

    private void Awake()
    {
        foreach (Image HealthBarSegment in DefaultHealthContainer.GetComponentsInChildren<Image>())
        {
            HealthBarImages.Add(HealthBarSegment);
        }
    }

    private void Start()
    {
        EHPlayerCharacter PlayerCharacter = BaseGameOverseer.Instance.PlayerController.GetComponent<EHPlayerCharacter>();
        if (PlayerCharacter)
        {
            PlayerCharacter.DamageableComponent.OnCharacterHealthChanged += OnHealthUpdated;
            CachedMaxHealth = PlayerCharacter.DamageableComponent.MaxHealth;
            CachedCurrentHealth = PlayerCharacter.DamageableComponent.Health;
            SpawnHealthBarsBasedOnMaxHealth();
            SetHealthUI();
        }
    }

    #region event methods
    public void OnHealthUpdated(FDamageData DamageData)
    {

    }
    #endregion event methods

    public void SpawnHealthBarsBasedOnMaxHealth()
    {
        int OriginalLength = HealthBarImages.Count;
        if (OriginalLength < CachedMaxHealth)
        {
            
            for (int i = 0; i < CachedMaxHealth - OriginalLength; ++i)
            {
                Image NewHealthImage = Instantiate<Image>(HealthBarSegmentPrefab);
                NewHealthImage.transform.SetParent(DefaultHealthContainer);
                NewHealthImage.transform.localPosition = new Vector3(HealthBarImages.Count * MarginSpacing, 0, 0);
                HealthBarImages.Add(NewHealthImage);
            }
        }
        else
        {
            for (int i = OriginalLength - 1; i > OriginalLength - CachedMaxHealth; --i)
            {
                Destroy(HealthBarImages[i].gameObject);
                HealthBarImages.RemoveAt(i);
            }
        }
    }

    private void SetHealthUI()
    {

    }
}
