using UnityEngine;

namespace Ayla
{
    public class PlayerWorkState : MonoBehaviour
    {
        private CraftingStation currentStation;

        public bool IsWorking => currentStation != null;

        public bool TryStartWork(CraftingStation station)
        {
            if (station == null || IsWorking)
            {
                return false;
            }

            currentStation = station;
            return true;
        }

        public void CancelWork()
        {
            if (currentStation == null)
            {
                return;
            }

            CraftingStation station = currentStation;
            currentStation = null;
            station.CancelWork(this);
        }

        public void CompleteWork(CraftingStation station)
        {
            if (currentStation != station)
            {
                return;
            }

            currentStation = null;
        }
    }
}
