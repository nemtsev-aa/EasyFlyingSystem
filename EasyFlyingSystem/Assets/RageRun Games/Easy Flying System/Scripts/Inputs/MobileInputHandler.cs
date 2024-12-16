using System;
using UnityEngine;

namespace RageRunGames.EasyFlyingSystem
{
    public class MobileInputHandler : BaseInputHandler, IInputHandler
    {
        [SerializeField] private ArmSwitcher _arm;
        [SerializeField] private MobileController pitchAndRollController;
        [SerializeField] private MobileController liftAndYawController;

        private void Awake()
        {
            _arm.Init(this);

            liftAndYawController.ThrottleIsZero += OnThrottleIsZero;
        }

        public void SetMobileInputControllers(MobileController[] controllers)
        {
            pitchAndRollController = controllers[0];
            liftAndYawController = controllers[1];
        }


        public void HandleInputs()
        {
            if (_arm.IsOn) {
                Pitch = pitchAndRollController.Vertical;
                Roll = pitchAndRollController.Horizontal;

                Yaw = liftAndYawController.Horizontal;
                Lift = liftAndYawController.Vertical;

                EvaluateAnyKeyDown();
            }
        }

        private void OnThrottleIsZero(bool status) {
            _arm.SetSecurity(status);
        }
    }
}
