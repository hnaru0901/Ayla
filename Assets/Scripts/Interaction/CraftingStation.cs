using UnityEngine;

namespace Ayla
{
    [System.Serializable]
    public struct CraftingStationRecipe
    {
        [SerializeField] private CarryItemType inputItem;
        [SerializeField] private CarryItemType outputItem;
        [SerializeField] private float workDuration;

        public CraftingStationRecipe(CarryItemType inputItem, CarryItemType outputItem)
            : this(inputItem, outputItem, 0f)
        {
        }

        public CraftingStationRecipe(CarryItemType inputItem, CarryItemType outputItem, float workDuration)
        {
            this.inputItem = inputItem;
            this.outputItem = outputItem;
            this.workDuration = workDuration;
        }

        public CarryItemType InputItem => inputItem;
        public CarryItemType OutputItem => outputItem;
        public float WorkDuration => workDuration;
    }

    public class CraftingStation : InteractableTarget
    {
        [SerializeField] private CarryItemType inputItem = CarryItemType.RawIngredient;
        [SerializeField] private CarryItemType outputItem = CarryItemType.PreparedIngredient;
        [SerializeField] private CraftingStationRecipe[] recipes = new CraftingStationRecipe[0];
        [SerializeField] private float workDuration = 3f;
        [SerializeField] private bool autoStartWork;

        private bool hasInput;
        private bool isWorking;
        private bool isComplete;
        private float progress;
        private float activeWorkDuration;
        private CarryItemType displayInputItem = CarryItemType.None;
        private CarryItemType queuedOutputItem = CarryItemType.None;
        private PlayerWorkState currentWorker;

        public bool IsEmpty => !hasInput && !isWorking && !isComplete;
        public bool HasInput => hasInput && !isWorking && !isComplete;
        public bool IsWorking => isWorking;
        public bool IsComplete => isComplete;
        public float Progress => progress;
        public float NormalizedProgress => isComplete ? 1f : Mathf.Clamp01(progress / ActiveWorkDuration);
        public CarryItemType DisplayItem => isComplete ? queuedOutputItem : displayInputItem;

        private float ActiveWorkDuration => activeWorkDuration > 0f ? activeWorkDuration : workDuration;

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

            if (progress < ActiveWorkDuration)
            {
                return;
            }

            progress = ActiveWorkDuration;
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

        // 테스트에서 Inspector 설정 없이 단일 입출력 레시피를 초기화할 때 사용합니다.
        public void Configure(CarryItemType inputItem, CarryItemType outputItem, float workDuration)
        {
            this.inputItem = inputItem;
            this.outputItem = outputItem;
            recipes = new CraftingStationRecipe[0];
            this.workDuration = Mathf.Max(0.01f, workDuration);
        }

        // 테스트에서 Inspector 설정 없이 여러 레시피를 초기화할 때 사용합니다.
        public void Configure(float workDuration, params CraftingStationRecipe[] recipes)
        {
            this.recipes = recipes ?? new CraftingStationRecipe[0];
            this.workDuration = Mathf.Max(0.01f, workDuration);
        }

        private void Update()
        {
            TickWork(Time.deltaTime);
        }

        private bool TryDepositInput(PlayerCarryState carryState)
        {
            if (!TryGetRecipe(carryState.CurrentItem, out CraftingStationRecipe recipe))
            {
                return false;
            }

            carryState.Clear();
            hasInput = true;
            isComplete = false;
            activeWorkDuration = recipe.WorkDuration > 0f ? recipe.WorkDuration : workDuration;
            displayInputItem = recipe.InputItem;
            queuedOutputItem = recipe.OutputItem;
            progress = 0f;

            if (autoStartWork)
            {
                hasInput = false;
                isWorking = true;
            }

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
            CarryItemType nextOutputItem = queuedOutputItem == CarryItemType.None ? outputItem : queuedOutputItem;

            if (!carryState.TrySetItem(nextOutputItem))
            {
                return false;
            }

            hasInput = false;
            isWorking = false;
            isComplete = false;
            progress = 0f;
            activeWorkDuration = 0f;
            displayInputItem = CarryItemType.None;
            queuedOutputItem = CarryItemType.None;
            currentWorker = null;
            return true;
        }

        private bool TryGetRecipe(CarryItemType currentItem, out CraftingStationRecipe recipe)
        {
            for (int i = 0; recipes != null && i < recipes.Length; i++)
            {
                if (recipes[i].InputItem != currentItem)
                {
                    continue;
                }

                recipe = recipes[i];
                return true;
            }

            if (currentItem == inputItem)
            {
                recipe = new CraftingStationRecipe(inputItem, outputItem, workDuration);
                return true;
            }

            recipe = default;
            return false;
        }

        // 테스트에서 Inspector 설정 없이 자동 작업 시작 여부를 초기화할 때 사용합니다.
        public void SetAutoStartWorkForTests(bool autoStartWork)
        {
            this.autoStartWork = autoStartWork;
        }
    }
}
