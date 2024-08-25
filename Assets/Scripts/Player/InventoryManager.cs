using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
  public List<WeaponController> weaponSlots = new List<WeaponController>(6);
  public int[] weaponLevels = new int[6];
  public List<Image> weaponUISlots = new List<Image>(6);
  public List<PassiveItem> passiveItemSlots = new List<PassiveItem>(6);
  public int[] passiveItemLevels = new int[6];
  public List<Image> passiveItemUISlots = new List<Image>(6);

  [System.Serializable]
  public class WeaponUpgrade
  {
    public GameObject initialWeapon;
    public WeaponScriptableObject weaponData;
  }

  [System.Serializable]
  public class PassiveItemUpgrade
  {
    public GameObject initialPassiveItem;
    public PassiveItemScriptableObject passiveItemData;
  }

  [System.Serializable]
  public class UpgradeUI
  {
    public TextMeshProUGUI upgradeNameDisplay;
    public TextMeshProUGUI upgradeDescriptionDisplay;
    public Image upgradeIcon;
    public Button upgradeButton;
  }

  public List<WeaponUpgrade> weaponUpgradeOptions = new List<WeaponUpgrade>();
  public List<PassiveItemUpgrade> passiveItemUpgradeOptions = new List<PassiveItemUpgrade>();
  public List<UpgradeUI> upgradeUIOptions = new List<UpgradeUI>();

  PlayerStats player;

  void Start()
  {
    player = GetComponent<PlayerStats>();
  }

  public void AddWeapon(int slotIndex, WeaponController weapon)   //Add a weapon to a specific slot
  {
    weaponSlots[slotIndex] = weapon;
    weaponLevels[slotIndex] = weapon.weaponData.Level;
    weaponUISlots[slotIndex].enabled = true;   //Enable the image component
    weaponUISlots[slotIndex].sprite = weapon.weaponData.Icon;

    if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
    {
      GameManager.instance.EndLevelUp();
    }
  }

  public void AddPassiveItem(int slotIndex, PassiveItem passiveItem)  //Add a passive item to a specific slot
  {
    passiveItemSlots[slotIndex] = passiveItem;
    passiveItemLevels[slotIndex] = passiveItem.passiveItemData.Level;
    passiveItemUISlots[slotIndex].enabled = true; //Enable the image component
    passiveItemUISlots[slotIndex].sprite = passiveItem.passiveItemData.Icon;

    if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
    {
      GameManager.instance.EndLevelUp();
    }
  }

  public void LevelUpWeapon(int slotIndex)
  {
    if (weaponSlots.Count > slotIndex)
    {
      WeaponController weapon = weaponSlots[slotIndex];
      if (!weapon.weaponData.NextLevelPrefab)  //Checks if there is a next level
      {
        Debug.LogError("NO NEXT LEVEL FOR " + weapon.name);
        return;
      }
      GameObject upgradedWeapon = Instantiate(weapon.weaponData.NextLevelPrefab, transform.position, Quaternion.identity);
      upgradedWeapon.transform.SetParent(transform);    //Set the weapon to be a child of the player
      AddWeapon(slotIndex, upgradedWeapon.GetComponent<WeaponController>());
      Destroy(weapon.gameObject);
      weaponLevels[slotIndex] = upgradedWeapon.GetComponent<WeaponController>().weaponData.Level;  //To make sure we have the correct weapon level

      if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
      {
        GameManager.instance.EndLevelUp();
      }
    }
  }

  public void LevelUpPassiveItem(int slotIndex)
  {
    if (passiveItemSlots.Count > slotIndex)
    {
      PassiveItem passiveItem = passiveItemSlots[slotIndex];
      if (!passiveItem.passiveItemData.NextLevelPrefab)  //Checks if there is a next level
      {
        Debug.LogError("NO NEXT LEVEL FOR " + passiveItem.name);
        return;
      }
      GameObject upgradedPassiveItem = Instantiate(passiveItem.passiveItemData.NextLevelPrefab, transform.position, Quaternion.identity);
      upgradedPassiveItem.transform.SetParent(transform);    //Set the passive item to be a child of the player
      AddPassiveItem(slotIndex, upgradedPassiveItem.GetComponent<PassiveItem>());
      Destroy(passiveItem.gameObject);
      passiveItemLevels[slotIndex] = upgradedPassiveItem.GetComponent<PassiveItem>().passiveItemData.Level;  //To make sure we have the correct passive item level

      if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
      {
        GameManager.instance.EndLevelUp();
      }
    }
  }

  void ApplyUpgradeOptions()
  {
    foreach (var upgradeOption in upgradeUIOptions)
    {
      int upgradeType = Random.Range(1, 3);
      if (upgradeType == 1)
      {
        WeaponUpgrade choosenWeaponUpgrade = weaponUpgradeOptions[Random.Range(0, weaponUpgradeOptions.Count)];

        if (choosenWeaponUpgrade != null)
        {
          bool newWeapon = false;
          for (int i = 0; i < weaponSlots.Count; i++)
          {
            if (weaponSlots[i] != null && weaponSlots[i].weaponData == choosenWeaponUpgrade.weaponData)
            {
              newWeapon = false;
              if (!newWeapon)
              {
                upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpWeapon(i));
                upgradeOption.upgradeDescriptionDisplay.text = choosenWeaponUpgrade.weaponData.NextLevelPrefab.GetComponent<WeaponController>().weaponData.Description;
                upgradeOption.upgradeNameDisplay.text = choosenWeaponUpgrade.weaponData.NextLevelPrefab.GetComponent<WeaponController>().weaponData.Name;
              }
              break;
            }
            else
            {
              newWeapon = true;
            }
          }
          if (newWeapon)
          {
            upgradeOption.upgradeButton.onClick.AddListener(() => player.SpawnWeapon(choosenWeaponUpgrade.initialWeapon));
            upgradeOption.upgradeDescriptionDisplay.text = choosenWeaponUpgrade.weaponData.Description;
            upgradeOption.upgradeNameDisplay.text = choosenWeaponUpgrade.weaponData.Name;
          }

          upgradeOption.upgradeIcon.sprite = choosenWeaponUpgrade.weaponData.Icon;
        }
      }
      else if (upgradeType == 2)
      {
        PassiveItemUpgrade chosenPassiveItemUpgrade = passiveItemUpgradeOptions[Random.Range(0, passiveItemUpgradeOptions.Count)];

        if (chosenPassiveItemUpgrade != null)
        {
          bool newPassiveItem = false;
          for (int i = 0; i < passiveItemSlots.Count; i++)
          {
            if (passiveItemSlots[i] != null && passiveItemSlots[i].passiveItemData == chosenPassiveItemUpgrade.passiveItemData)
            {
              newPassiveItem = false;

              if (!newPassiveItem)
              {
                upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpPassiveItem(i));
                upgradeOption.upgradeDescriptionDisplay.text = chosenPassiveItemUpgrade.passiveItemData.NextLevelPrefab.GetComponent<PassiveItem>().passiveItemData.Description;
                upgradeOption.upgradeNameDisplay.text = chosenPassiveItemUpgrade.passiveItemData.NextLevelPrefab.GetComponent<PassiveItem>().passiveItemData.Name;
              }
              break;
            }
            else
            {
              newPassiveItem = true;
            }
          }
          if (newPassiveItem)
          {
            upgradeOption.upgradeButton.onClick.AddListener(() => player.SpawnPassiveItem(chosenPassiveItemUpgrade.initialPassiveItem));
            upgradeOption.upgradeDescriptionDisplay.text = chosenPassiveItemUpgrade.passiveItemData.Description;
            upgradeOption.upgradeNameDisplay.text = chosenPassiveItemUpgrade.passiveItemData.Name;
          }
          upgradeOption.upgradeIcon.sprite = chosenPassiveItemUpgrade.passiveItemData.Icon;
        }
      }
    }
  }

  void RemoveUpgradeOptions()
  {
    foreach (var upgradeOtion in upgradeUIOptions)
    {
      upgradeOtion.upgradeButton.onClick.RemoveAllListeners();
    }
  }

  public void RemoveAndApplyUpgrades()
  {
    RemoveUpgradeOptions();
    ApplyUpgradeOptions();
  }

}