using UnityEngine;
using CityZero.Gameplay.Vehicles;

namespace CityZero.Gameplay.Player
{
    public sealed partial class PlayerController
    {
        public void EnterVehicleLegacyAdapter(VehicleControllerClean vehicle)
        {
            if (vehicle == null)
            {
                return;
            }

            _currentVehicleClean = vehicle;
            vehicle.SetDriver(this);
            gameObject.SetActive(false);
        }

        public void ExitVehicleClean()
        {
            if (_currentVehicleClean == null)
            {
                return;
            }

            Vector3 exitPosition = _currentVehicleClean.GetExitPoint(transform.position);
            VehicleControllerClean previousVehicle = _currentVehicleClean;
            _currentVehicleClean = null;

            transform.position = exitPosition;
            gameObject.SetActive(true);
            previousVehicle.RemoveDriver();
        }
    }
}
