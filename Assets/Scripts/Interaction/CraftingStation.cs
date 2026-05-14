using UnityEngine;

namespace Ayla
{
    public class CraftingStation : InteractableTarget
    {
        [SerializeField] private CarryItemType inputItem = CarryItemType.RawIngredient;
        [SerializeField] private CarryItemType outputItem = CarryItemType.PreparedIngredient;
        [SerializeField] private float workDuration = 3f;

        private bool hasInput;
        private bool isWorking;
        private bool isComplete;
        private float progress;
        private PlayerWorkState currentWorker;

        public bool IsEmpty => !hasInput && !isWorking && !isComplete;
        public bool HasInput => hasInput && !isWorking && !isComplete;
        public bool IsWorking => isWorking;
        public bool IsComplete => isComplete;
        public float Progress => progress;
        public float NormalizedProgress => isComplete ? 1f : Mathf.Clamp01(progress / workDuration);

        public override bool Interact(PlayerCarryState carryState)
        {
            PlayerWorkState workState = carryState != null ? carryState.GetComponent<PlayerWorkState>() : null;
            return Interact(carryState, workState);
        }

        public bool Interact(PlayerCarryState carryState, PlayerWorkState workState)
        {
            if (carryState == null)
            {
                return false;
            }

            if (IsEmpty)
            {
                return TryDepositInput(carryState);
            }

            if (HasInput)
            {
                return TryStartWork(carryState, workState);
            }

            if (isComplete)
            {
                return TryTakeOutput(carryState);
            }

            return false;
        }

        public void TickWork(float deltaTime)
        {
            if (!isWorking || deltaTime <= 0f)
            {
                return;
            }

            progress += deltaTime;

            if (progress < workDuration)
            {
                return;
            }

            progress = workDuration;
            isWorking = false;
            isComplete = true;
            currentWorker?.CompleteWork(this);
            currentWorker = null;
        }

        public void CancelWork(PlayerWorkState workState)
        {
            if (!isWorking || currentWorker != workState)
            {
                return;
            }

            isWorking = false;
            hasInput = true;
            currentWorker = null;
        }

        public void Configure(CarryItemType inputItem, CarryItemType outputItem, float workDuration)
        {
            this.inputItem = inputItem;
            this.outputItem = outputItem;
            this.workDuration = Mathf.Max(0.01f, workDuration);
        }

        private void Update()
        {
            TickWork(Time.deltaTime);
        }

        private bool TryDepositInput(PlayerCarryState carryState)
        {
            if (carryState.CurrentItem != inputItem)
            {
                return false;
            }

            carryState.Clear();
            hasInput = true;
            isComplete = false;
            progress = 0f;
            return true;
        }

        private bool TryStartWork(PlayerCarryState carryState, PlayerWorkState workState)
        {
            if (!carryState.IsEmpty || workState == null || !workState.TryStartWork(this))
            {
                return false;
            }

            hasInput = false;
            isWorking = true;
            currentWorker = workState;
            return true;
        }

        private bool TryTakeOutput(PlayerCarryState carryState)
        {
            if (!carryState.TrySetItem(outputItem))
            {
                return false;
            }

            hasInput = false;
            isWorking = false;
            isComplete = false;
            progress = 0f;
            currentWorker = null;
            return true;
        }
    }
}
