using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace RageRunGames.EasyFlyingSystem
{
    public class RotorsAnimation : MonoBehaviour
    {
        [SerializeField] private ArmSwitcher _arm;

        [FormerlySerializedAs("flyController")][SerializeField] private DroneController droneController;

        [Header("Rotors Settings")]
        [SerializeField]
        private RotorsInfo[] rotors;

        private IInputHandler InputHandler => droneController.InputHandler;

        private void Awake()
        {
            if (droneController == null)
            {
                droneController = GetComponent<DroneController>();

                if (droneController == null)
                    Debug.LogWarning("Fly controller missing on this object.");

            }
        }

        private void Update() {
            HandleRotors();
        }

        private void HandleRotors()
        {
            if (rotors.Length == 0)
                return;

            for (int i = 0; i < rotors.Length; i++)
            {
                UpdateRotation(rotors[i].rotor);
            }
        }

        private void UpdateRotation(Rotor currentRotor) {
            // Двигатели выключены
            if (_arm.IsOn == false && currentRotor.speed == 0)
                return;

            if (_arm.IsOn == false && currentRotor.speed > 0) {
                // Двигатели работают с замедлением
                SlowingDownSpeed(currentRotor);
                return;
            }

            bool executeRotation = CanRotateOnInput();

            if (executeRotation == false) {
                // Двигатели работают на холостом ходу
                IdlingSpeed(currentRotor);
                return;
            }

            // Двигатели работают с ускорением
            ThrottleSpeed(currentRotor);
        }


        private void IdlingSpeed(Rotor currentRotor) {

            Transform rotorTransform = currentRotor.rotorTransform;

            rotorTransform.Rotate(
                   (currentRotor.inverseRotation ? currentRotor.rotationSpeed * 0.2f : -currentRotor.rotationSpeed * 0.2f) *
                   Time.deltaTime * currentRotor.rotationAxis);

            currentRotor.speed = currentRotor.rotationSpeed * 0.2f;
        }

        private void SlowingDownSpeed(Rotor currentRotor)
        {
            Transform rotorTransform = currentRotor.rotorTransform;
            float currentSpeed = currentRotor.speed;
            float decelerationRate = 5000f;

            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, decelerationRate * Time.deltaTime);

            currentRotor.speed = currentSpeed;

            rotorTransform.Rotate((currentRotor.inverseRotation ? currentSpeed : -currentSpeed) * Time.deltaTime * currentRotor.rotationAxis);
        }

        private void ThrottleSpeed(Rotor currentRotor)
        {
            Transform rotorTransform = currentRotor.rotorTransform;
            float currentSpeed = currentRotor.speed;
            float normalizedInput = (InputHandler.Lift + 1f) / 2f;
            float targetSpeed = Mathf.SmoothStep(0f, 1f, normalizedInput) * currentRotor.rotationSpeed;

            Debug.Log($"Target Speed: {targetSpeed}");

            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, 500f * Time.deltaTime);

            currentRotor.speed = currentSpeed;

            rotorTransform.Rotate((currentRotor.inverseRotation ? currentSpeed : -currentSpeed) * Time.deltaTime * currentRotor.rotationAxis);
        }

        private float CurrentInputValue()
        {
            return Mathf.Abs(InputHandler.Lift + InputHandler.Yaw + InputHandler.Pitch + InputHandler.Roll);
        }

        private bool CanRotateOnInput()
        {
            return CurrentInputValue() > 0.2f;
        }
    }


    [Serializable]
    public class RotorsInfo
    {
        public Rotor rotor;
    }


    [Serializable]
    public class Rotor
    {
        public Transform rotorTransform;
        public Vector3 rotationAxis;
        [HideInInspector] public float speed;
        public float rotationSpeed = 1500f;
        public bool inverseRotation;
    }
}