using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SSlotImageChooser : MonoBehaviour
{
    [SerializeField] private TMP_Text locationText;
    [SerializeField] private Image locationImage;

    [SerializeField] private Sprite defaultLocationSprite;
    [SerializeField] private Sprite[] locationSprites;
    [SerializeField] private string[] locationKeys;

    [SerializeField] public Dictionary<string, Sprite> locationSpriteMap;

    private void Awake()
    {
        locationSpriteMap = new Dictionary<string, Sprite>();

        for (int i = 0; i < locationKeys.Length && i < locationSprites.Length; i++)
        {
            locationSpriteMap[locationKeys[i]] = locationSprites[i];
        }
    }

    public Sprite GetLocationSprite(string locationId)
    {
        if (locationSpriteMap.TryGetValue(locationId, out Sprite sprite))
        {
            return sprite;
        }
        return defaultLocationSprite;
    }

    private void OnEnable()
    {
        StartCoroutine(DoLocation());
    }

    public IEnumerator DoLocation()
    {
        yield return new WaitForSeconds(.01f);
	    locationImage.sprite = GetLocationSprite(locationText.text + "Location");
    }

}
