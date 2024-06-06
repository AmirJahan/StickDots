using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerNameEditor : MonoBehaviour
{

    [SerializeField] private GameObject _profileMenu;
    [SerializeField] private GameObject _titleMenu;
    [SerializeField] private GameObject _leaderboard;
    [SerializeField] private GameObject _howTo;
    [SerializeField] private GameObject _gameMode;
    [SerializeField] private GameObject _settingsMenu;
    [SerializeField] private GameObject _gameOptions;

    [SerializeField] private TextMeshProUGUI _nameInputField;

    void Start()
    {
        if (PlayerPrefs.GetString("Name") == null || PlayerPrefs.GetString("Name") == "")
        {
            // Set Profile screen as active
            _profileMenu.SetActive(true);

            _titleMenu.SetActive(false);
            _leaderboard.SetActive(false);
            _gameMode.SetActive(false);
            _settingsMenu.SetActive(false);
            _gameOptions.SetActive(false);

            // Let the player know they need to set a profile name
        }
    }

    // Try to set the name of the player when they hit submit in the profile
    public void TrySaveName()
    {
        if (_nameInputField.text != null)
        {
            SetPlayerName(_nameInputField.text);
        }

        if (PlayerPrefs.GetString("Name") == null)
        {
            // Tell the player they need to set a name
            return;
        }

        // Close the profile menu when name is set
        _titleMenu.SetActive(true);
        _gameMode.SetActive(true);
        _profileMenu.SetActive(false);
    }

    // Save input to player preferences
    void SetPlayerName(string newName)
    {
        PlayerPrefs.SetString("Name", newName);
    }
}
