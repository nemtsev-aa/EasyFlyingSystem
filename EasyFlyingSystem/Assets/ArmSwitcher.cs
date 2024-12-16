using RageRunGames.EasyFlyingSystem;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArmSwitcher : MonoBehaviour, IDisposable {
    public event Action LiftNotNull;

    [SerializeField] private Image _background;
    [SerializeField] private TextMeshProUGUI _label;
    [SerializeField] private Button _button;
        
    private MobileInputHandler _inputHandler;
    private bool _isSecurity;

    public bool IsOn { get; private set; } = false;

    public void Init(MobileInputHandler inputHandler) {
        _inputHandler = inputHandler;

        AddListeners();
    }

    private void AddListeners() {
        _button.onClick.AddListener(ButtonClicked);
    }

    private void RemoveListeners() {
        _button.onClick.RemoveListener(ButtonClicked);
    }

    private void ButtonClicked() {
        if (IsOn == false && _isSecurity == true) {
            IsOn = true;
            _background.color = Color.red;

            return;
        }

        IsOn = false;
        _background.color = Color.green;
        LiftNotNull?.Invoke();
    }

    public void Dispose() {
        RemoveListeners();
    }

    public void SetSecurity(bool status)
    {
        _isSecurity = status;
    }
}