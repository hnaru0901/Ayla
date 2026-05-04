# Ayla First Prototype Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 손님 응대, 포션 제조, 판매 정산, 설비 업그레이드가 하루 단위로 순환하는 첫 플레이 가능 프로토타입을 Unity 안에서 검증한다.

**Architecture:** 핵심 게임 로직은 테스트 가능한 순수 C# 도메인 클래스로 분리하고, Unity 씬은 그 로직을 표시하고 입력을 중계하는 얇은 레이어로 둔다. 첫 버전은 임시 3D 프리미티브와 기본 UI로 루프 검증에 집중하고, 서사/탐험/전투는 제외한다.

**Tech Stack:** Unity 6000.3.8f1, C#, Unity Input System, Unity UI, Unity Test Framework (EditMode)

---

## File Structure

- Create: `Assets/Scripts/Domain/Customers/CustomerOrder.cs`
- Create: `Assets/Scripts/Domain/Ingredients/IngredientDefinition.cs`
- Create: `Assets/Scripts/Domain/Potions/PotionProfile.cs`
- Create: `Assets/Scripts/Domain/Brewing/BrewingStep.cs`
- Create: `Assets/Scripts/Domain/Brewing/BrewingSession.cs`
- Create: `Assets/Scripts/Domain/Shop/DayConfig.cs`
- Create: `Assets/Scripts/Domain/Shop/DayResult.cs`
- Create: `Assets/Scripts/Domain/Shop/ShopUpgrade.cs`
- Create: `Assets/Scripts/Domain/Shop/ShopState.cs`
- Create: `Assets/Scripts/Domain/Shop/ShopSimulation.cs`
- Create: `Assets/Scripts/Runtime/Bootstrap/GameBootstrap.cs`
- Create: `Assets/Scripts/Runtime/Customers/CustomerQueuePresenter.cs`
- Create: `Assets/Scripts/Runtime/Brewing/BrewingStationController.cs`
- Create: `Assets/Scripts/Runtime/UI/OrderPanelView.cs`
- Create: `Assets/Scripts/Runtime/UI/BrewingPanelView.cs`
- Create: `Assets/Scripts/Runtime/UI/DaySummaryView.cs`
- Create: `Assets/Scripts/Runtime/UI/UpgradePanelView.cs`
- Create: `Assets/Tests/EditMode/Domain/BrewingSessionTests.cs`
- Create: `Assets/Tests/EditMode/Domain/ShopSimulationTests.cs`
- Create: `Assets/Tests/EditMode/Domain/UpgradeProgressionTests.cs`
- Modify: `Assets/Scenes/SampleScene.unity`
- Modify: `Assets/PlayerController.cs` (replace or retire if unused; do not leave dead template code)

## Task 1: Build the testable domain model

**Files:**
- Create: `Assets/Scripts/Domain/Customers/CustomerOrder.cs`
- Create: `Assets/Scripts/Domain/Ingredients/IngredientDefinition.cs`
- Create: `Assets/Scripts/Domain/Potions/PotionProfile.cs`
- Create: `Assets/Scripts/Domain/Brewing/BrewingStep.cs`
- Create: `Assets/Scripts/Domain/Brewing/BrewingSession.cs`
- Create: `Assets/Tests/EditMode/Domain/BrewingSessionTests.cs`

- [ ] **Step 1: Write the failing brewing domain tests**

```csharp
using NUnit.Framework;
using Ayla.Domain.Brewing;
using Ayla.Domain.Customers;
using Ayla.Domain.Ingredients;
using Ayla.Domain.Potions;
using System.Collections.Generic;

namespace Ayla.Tests.Domain
{
    public class BrewingSessionTests
    {
        [Test]
        public void CompletesPotion_WhenCorrectIngredientsAndStepsAreApplied()
        {
            var order = new CustomerOrder(
                customerName: "Mina",
                symptomNotes: new[] { "잠이 안 와요", "몸이 차가워요" },
                requiredTraits: new[] { "calm", "warm" },
                patienceSeconds: 45f,
                rewardGold: 20);

            var ingredients = new List<IngredientDefinition>
            {
                new("MoonHerb", "달풀", new[] { "calm" }),
                new("SunDrop", "햇방울", new[] { "warm" })
            };

            var session = BrewingSession.Start(order, ingredients);

            session.AddIngredient("MoonHerb");
            session.AddIngredient("SunDrop");
            session.ApplyStep(BrewingStep.Stir);
            session.ApplyStep(BrewingStep.Boil);

            PotionProfile result = session.Finish();

            CollectionAssert.AreEquivalent(new[] { "calm", "warm" }, result.Traits);
            Assert.That(result.QualityScore, Is.GreaterThan(0.8f));
        }

        [Test]
        public void PenalizesPotion_WhenStepOrderIsWrong()
        {
            var order = new CustomerOrder(
                "Yuri",
                new[] { "머리가 멍해요" },
                new[] { "focus" },
                45f,
                20);

            var session = BrewingSession.Start(order, new[]
            {
                new IngredientDefinition("MistLeaf", "안개잎", new[] { "focus" })
            });

            session.ApplyStep(BrewingStep.Boil);
            session.AddIngredient("MistLeaf");
            session.ApplyStep(BrewingStep.Stir);

            var result = session.Finish();

            Assert.That(result.QualityScore, Is.LessThan(0.6f));
            Assert.That(result.HasCriticalMistake, Is.True);
        }
    }
}
```

- [ ] **Step 2: Run EditMode tests to verify they fail**

Run:

```powershell
& 'C:\Program Files\Unity\Hub\Editor\6000.3.8f1\Editor\Unity.exe' -batchmode -projectPath 'D:\Unity\Ayla' -runTests -testPlatform EditMode -testResults 'D:\Unity\Ayla\Logs\editmode-tests.xml' -quit
```

Expected: FAIL with compile errors because the `Ayla.Domain.*` classes do not exist yet.

- [ ] **Step 3: Write the minimal brewing domain implementation**

```csharp
namespace Ayla.Domain.Brewing
{
    public enum BrewingStep
    {
        Stir,
        Boil
    }
}

namespace Ayla.Domain.Ingredients
{
    public sealed class IngredientDefinition
    {
        public IngredientDefinition(string id, string displayName, IReadOnlyList<string> traits)
        {
            Id = id;
            DisplayName = displayName;
            Traits = traits;
        }

        public string Id { get; }
        public string DisplayName { get; }
        public IReadOnlyList<string> Traits { get; }
    }
}

namespace Ayla.Domain.Customers
{
    public sealed class CustomerOrder
    {
        public CustomerOrder(string customerName, IReadOnlyList<string> symptomNotes, IReadOnlyList<string> requiredTraits, float patienceSeconds, int rewardGold)
        {
            CustomerName = customerName;
            SymptomNotes = symptomNotes;
            RequiredTraits = requiredTraits;
            PatienceSeconds = patienceSeconds;
            RewardGold = rewardGold;
        }

        public string CustomerName { get; }
        public IReadOnlyList<string> SymptomNotes { get; }
        public IReadOnlyList<string> RequiredTraits { get; }
        public float PatienceSeconds { get; }
        public int RewardGold { get; }
    }
}

namespace Ayla.Domain.Potions
{
    public sealed class PotionProfile
    {
        public PotionProfile(IReadOnlyList<string> traits, float qualityScore, bool hasCriticalMistake)
        {
            Traits = traits;
            QualityScore = qualityScore;
            HasCriticalMistake = hasCriticalMistake;
        }

        public IReadOnlyList<string> Traits { get; }
        public float QualityScore { get; }
        public bool HasCriticalMistake { get; }
    }
}

namespace Ayla.Domain.Brewing
{
    using Ayla.Domain.Customers;
    using Ayla.Domain.Ingredients;
    using Ayla.Domain.Potions;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class BrewingSession
    {
        private readonly CustomerOrder _order;
        private readonly Dictionary<string, IngredientDefinition> _ingredients;
        private readonly List<string> _selectedIngredientIds = new();
        private readonly List<BrewingStep> _steps = new();

        private BrewingSession(CustomerOrder order, IEnumerable<IngredientDefinition> ingredients)
        {
            _order = order;
            _ingredients = ingredients.ToDictionary(x => x.Id);
        }

        public static BrewingSession Start(CustomerOrder order, IEnumerable<IngredientDefinition> ingredients)
        {
            return new BrewingSession(order, ingredients);
        }

        public void AddIngredient(string ingredientId) => _selectedIngredientIds.Add(ingredientId);

        public void ApplyStep(BrewingStep step) => _steps.Add(step);

        public PotionProfile Finish()
        {
            var traits = _selectedIngredientIds
                .Where(_ingredients.ContainsKey)
                .SelectMany(id => _ingredients[id].Traits)
                .Distinct()
                .ToList();

            float quality = 0.25f;
            bool matchesAllTraits = _order.RequiredTraits.All(traits.Contains);
            bool correctStepOrder = _steps.SequenceEqual(new[] { BrewingStep.Stir, BrewingStep.Boil });

            if (matchesAllTraits) quality += 0.5f;
            if (correctStepOrder) quality += 0.25f;

            return new PotionProfile(traits, quality, !correctStepOrder);
        }
    }
}
```

- [ ] **Step 4: Run EditMode tests to verify they pass**

Run:

```powershell
& 'C:\Program Files\Unity\Hub\Editor\6000.3.8f1\Editor\Unity.exe' -batchmode -projectPath 'D:\Unity\Ayla' -runTests -testPlatform EditMode -testResults 'D:\Unity\Ayla\Logs\editmode-tests.xml' -quit
```

Expected: PASS for `BrewingSessionTests` and XML written to `Logs/editmode-tests.xml`.

- [ ] **Step 5: Commit**

```bash
git add Assets/Scripts/Domain Assets/Tests/EditMode/Domain
git commit -m "feat: add core brewing domain model"
```

## Task 2: Implement day simulation and upgrade progression

**Files:**
- Create: `Assets/Scripts/Domain/Shop/DayConfig.cs`
- Create: `Assets/Scripts/Domain/Shop/DayResult.cs`
- Create: `Assets/Scripts/Domain/Shop/ShopUpgrade.cs`
- Create: `Assets/Scripts/Domain/Shop/ShopState.cs`
- Create: `Assets/Scripts/Domain/Shop/ShopSimulation.cs`
- Create: `Assets/Tests/EditMode/Domain/ShopSimulationTests.cs`
- Create: `Assets/Tests/EditMode/Domain/UpgradeProgressionTests.cs`

- [ ] **Step 1: Write failing day loop and upgrade tests**

```csharp
using NUnit.Framework;
using Ayla.Domain.Shop;
using Ayla.Domain.Customers;
using System.Collections.Generic;

namespace Ayla.Tests.Domain
{
    public class ShopSimulationTests
    {
        [Test]
        public void DayResultAddsGold_WhenOrdersAreServed()
        {
            var state = new ShopState(startingGold: 30, brewingStations: 1, queueCapacity: 1);
            var config = new DayConfig(new List<CustomerOrder>
            {
                new("Mina", new[] { "피곤해요" }, new[] { "vigor" }, 30f, 15),
                new("Rin", new[] { "추워요" }, new[] { "warm" }, 30f, 20)
            });

            var simulation = new ShopSimulation(state, config);
            simulation.ResolveOrder(successQuality: 1.0f);
            simulation.ResolveOrder(successQuality: 0.5f);

            DayResult result = simulation.FinishDay();

            Assert.That(result.EarnedGold, Is.EqualTo(35));
            Assert.That(result.CustomersServed, Is.EqualTo(2));
        }
    }

    public class UpgradeProgressionTests
    {
        [Test]
        public void CanPurchaseBrewingStationUpgrade_WhenEnoughGold()
        {
            var state = new ShopState(startingGold: 100, brewingStations: 1, queueCapacity: 1);
            var upgrade = new ShopUpgrade("extra_station", "추가 작업대", 60, UpgradeType.BrewingStation);

            bool purchased = state.TryPurchase(upgrade);

            Assert.That(purchased, Is.True);
            Assert.That(state.Gold, Is.EqualTo(40));
            Assert.That(state.BrewingStations, Is.EqualTo(2));
        }
    }
}
```

- [ ] **Step 2: Run EditMode tests to verify they fail**

Run the same Unity EditMode command as Task 1.

Expected: FAIL because `ShopState`, `DayConfig`, `DayResult`, `ShopUpgrade`, and `ShopSimulation` do not exist.

- [ ] **Step 3: Write minimal shop simulation and upgrade implementation**

```csharp
namespace Ayla.Domain.Shop
{
    public enum UpgradeType
    {
        BrewingStation,
        QueueCapacity
    }

    public sealed class ShopUpgrade
    {
        public ShopUpgrade(string id, string displayName, int cost, UpgradeType type)
        {
            Id = id;
            DisplayName = displayName;
            Cost = cost;
            Type = type;
        }

        public string Id { get; }
        public string DisplayName { get; }
        public int Cost { get; }
        public UpgradeType Type { get; }
    }
}

namespace Ayla.Domain.Shop
{
    public sealed class ShopState
    {
        public ShopState(int startingGold, int brewingStations, int queueCapacity)
        {
            Gold = startingGold;
            BrewingStations = brewingStations;
            QueueCapacity = queueCapacity;
        }

        public int Gold { get; private set; }
        public int BrewingStations { get; private set; }
        public int QueueCapacity { get; private set; }

        public void AddGold(int amount) => Gold += amount;

        public bool TryPurchase(ShopUpgrade upgrade)
        {
            if (Gold < upgrade.Cost) return false;

            Gold -= upgrade.Cost;
            if (upgrade.Type == UpgradeType.BrewingStation) BrewingStations += 1;
            if (upgrade.Type == UpgradeType.QueueCapacity) QueueCapacity += 1;
            return true;
        }
    }
}

namespace Ayla.Domain.Shop
{
    using Ayla.Domain.Customers;
    using System.Collections.Generic;

    public sealed class DayConfig
    {
        public DayConfig(IReadOnlyList<CustomerOrder> orders) => Orders = orders;
        public IReadOnlyList<CustomerOrder> Orders { get; }
    }

    public sealed class DayResult
    {
        public DayResult(int earnedGold, int customersServed)
        {
            EarnedGold = earnedGold;
            CustomersServed = customersServed;
        }

        public int EarnedGold { get; }
        public int CustomersServed { get; }
    }

    public sealed class ShopSimulation
    {
        private readonly ShopState _state;
        private readonly DayConfig _config;
        private int _resolvedCount;
        private int _earnedGold;

        public ShopSimulation(ShopState state, DayConfig config)
        {
            _state = state;
            _config = config;
        }

        public void ResolveOrder(float successQuality)
        {
            var order = _config.Orders[_resolvedCount];
            int payout = (int)System.Math.Round(order.RewardGold * successQuality);
            _earnedGold += payout;
            _resolvedCount += 1;
        }

        public DayResult FinishDay()
        {
            _state.AddGold(_earnedGold);
            return new DayResult(_earnedGold, _resolvedCount);
        }
    }
}
```

- [ ] **Step 4: Run EditMode tests to verify they pass**

Run the same Unity EditMode command as Task 1.

Expected: PASS for `ShopSimulationTests` and `UpgradeProgressionTests`.

- [ ] **Step 5: Commit**

```bash
git add Assets/Scripts/Domain/Shop Assets/Tests/EditMode/Domain
git commit -m "feat: add day loop and upgrade progression"
```

## Task 3: Bootstrap the playable sample scene with placeholder assets

**Files:**
- Create: `Assets/Scripts/Runtime/Bootstrap/GameBootstrap.cs`
- Create: `Assets/Scripts/Runtime/Customers/CustomerQueuePresenter.cs`
- Modify: `Assets/Scenes/SampleScene.unity`
- Modify: `Assets/PlayerController.cs`

- [ ] **Step 1: Write a failing Play Mode smoke test for scene bootstrap**

```csharp
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using System.Collections;

namespace Ayla.Tests.Scene
{
    public class SampleSceneBootstrapTests
    {
        [UnityTest]
        public IEnumerator SampleScene_CreatesBootstrapObjects()
        {
            yield return SceneManager.LoadSceneAsync("SampleScene");

            Assert.That(Object.FindFirstObjectByType<GameBootstrap>(), Is.Not.Null);
            Assert.That(Object.FindFirstObjectByType<CustomerQueuePresenter>(), Is.Not.Null);
        }
    }
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run the same Unity EditMode command as Task 1 after moving the smoke test under `Assets/Tests/PlayMode` if needed.

Expected: FAIL because the runtime bootstrap classes and scene objects do not exist.

- [ ] **Step 3: Build the minimal scene bootstrap**

```csharp
using Ayla.Domain.Customers;
using Ayla.Domain.Shop;
using System.Collections.Generic;
using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private CustomerQueuePresenter customerQueuePresenter;

    private ShopState _shopState;
    private ShopSimulation _simulation;

    private void Start()
    {
        _shopState = new ShopState(startingGold: 30, brewingStations: 1, queueCapacity: 1);

        var dayConfig = new DayConfig(new List<CustomerOrder>
        {
            new("Mina", new[] { "잠이 안 와요", "몸이 차가워요" }, new[] { "calm", "warm" }, 45f, 20),
            new("Rin", new[] { "머리가 멍해요" }, new[] { "focus" }, 40f, 18),
        });

        _simulation = new ShopSimulation(_shopState, dayConfig);
        customerQueuePresenter.Show(dayConfig.Orders);
    }

    public ShopState CurrentShopState => _shopState;
    public ShopSimulation CurrentSimulation => _simulation;
}
```

```csharp
using Ayla.Domain.Customers;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CustomerQueuePresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI queueLabel;

    public void Show(IReadOnlyList<CustomerOrder> orders)
    {
        queueLabel.text = string.Join("\n", orders.Select(x => $"{x.CustomerName}: {string.Join(", ", x.SymptomNotes)}"));
    }
}
```

Scene requirements for `SampleScene`:
- Add one `GameBootstrap` GameObject.
- Add one `Canvas` with queue text, order panel placeholder, brewing panel placeholder, summary panel placeholder.
- Add one countertop area built from primitive cubes.
- Add one customer waiting spot built from a primitive capsule.
- Remove or repurpose the template `PlayerController` component; do not keep an unused movement script in the prototype scene.

- [ ] **Step 4: Run scene smoke tests**

Run the same Unity test command as before, targeting both EditMode and PlayMode when the PlayMode test is in place.

Expected: PASS and `SampleScene` loads with bootstrap objects present.

- [ ] **Step 5: Commit**

```bash
git add Assets/Scripts/Runtime Assets/Scenes/SampleScene.unity Assets/PlayerController.cs
git commit -m "feat: bootstrap playable shop scene"
```

## Task 4: Implement the brewing station interaction loop

**Files:**
- Create: `Assets/Scripts/Runtime/Brewing/BrewingStationController.cs`
- Create: `Assets/Scripts/Runtime/UI/OrderPanelView.cs`
- Create: `Assets/Scripts/Runtime/UI/BrewingPanelView.cs`
- Modify: `Assets/Scenes/SampleScene.unity`

- [ ] **Step 1: Write failing tests for station interaction state**

```csharp
using NUnit.Framework;
using Ayla.Domain.Brewing;
using Ayla.Domain.Customers;
using Ayla.Domain.Ingredients;

namespace Ayla.Tests.Domain
{
    public class BrewingStationFlowTests
    {
        [Test]
        public void FinishesValidOrder_WhenSelectedTraitsMatchRequest()
        {
            var order = new CustomerOrder("Mina", new[] { "잠이 안 와요" }, new[] { "calm" }, 30f, 20);
            var session = BrewingSession.Start(order, new[]
            {
                new IngredientDefinition("MoonHerb", "달풀", new[] { "calm" })
            });

            session.AddIngredient("MoonHerb");
            session.ApplyStep(BrewingStep.Stir);
            session.ApplyStep(BrewingStep.Boil);

            Assert.That(session.Finish().QualityScore, Is.GreaterThan(0.8f));
        }
    }
}
```

- [ ] **Step 2: Run tests to verify they fail if any interaction APIs are missing**

Run the Unity EditMode command.

Expected: FAIL if Task 1 interfaces were changed or incomplete.

- [ ] **Step 3: Implement the station controller and UI wiring**

```csharp
using Ayla.Domain.Brewing;
using Ayla.Domain.Ingredients;
using UnityEngine;

public class BrewingStationController : MonoBehaviour
{
    [SerializeField] private BrewingPanelView brewingPanelView;

    private BrewingSession _session;

    public void Bind(BrewingSession session, IngredientDefinition[] ingredients)
    {
        _session = session;
        brewingPanelView.RenderIngredients(ingredients);
    }

    public void OnIngredientSelected(string ingredientId)
    {
        _session.AddIngredient(ingredientId);
    }

    public void OnStirPressed()
    {
        _session.ApplyStep(BrewingStep.Stir);
    }

    public void OnBoilPressed()
    {
        _session.ApplyStep(BrewingStep.Boil);
    }
}
```

```csharp
using Ayla.Domain.Customers;
using TMPro;
using UnityEngine;

public class OrderPanelView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI customerNameLabel;
    [SerializeField] private TextMeshProUGUI symptomMemoLabel;

    public void Render(CustomerOrder order)
    {
        customerNameLabel.text = order.CustomerName;
        symptomMemoLabel.text = string.Join("\n", order.SymptomNotes);
    }
}
```

```csharp
using Ayla.Domain.Ingredients;
using TMPro;
using UnityEngine;

public class BrewingPanelView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ingredientListLabel;

    public void RenderIngredients(IngredientDefinition[] ingredients)
    {
        ingredientListLabel.text = string.Join("\n", ingredients.Select(x => $"{x.DisplayName} [{string.Join(", ", x.Traits)}]"));
    }
}
```

Scene requirements:
- Add ingredient buttons for 3 starting ingredients.
- Add `Stir` and `Boil` buttons.
- Add a simple timer label for customer patience.
- Keep all visuals placeholder-grade; no art polish work in this task.

- [ ] **Step 4: Run tests and manual play check**

Run:

```powershell
& 'C:\Program Files\Unity\Hub\Editor\6000.3.8f1\Editor\Unity.exe' -batchmode -projectPath 'D:\Unity\Ayla' -runTests -testPlatform EditMode -testResults 'D:\Unity\Ayla\Logs\editmode-tests.xml' -quit
```

Then open `SampleScene` in the editor and manually confirm:
- order text appears,
- ingredient traits are readable at a glance,
- ingredient, stir, and boil buttons react,
- one potion can be completed end-to-end.

- [ ] **Step 5: Commit**

```bash
git add Assets/Scripts/Runtime/Brewing Assets/Scripts/Runtime/UI Assets/Scenes/SampleScene.unity
git commit -m "feat: implement basic brewing station loop"
```

## Task 5: Add end-of-day summary and upgrade purchasing

**Files:**
- Create: `Assets/Scripts/Runtime/UI/DaySummaryView.cs`
- Create: `Assets/Scripts/Runtime/UI/UpgradePanelView.cs`
- Modify: `Assets/Scripts/Runtime/Bootstrap/GameBootstrap.cs`
- Modify: `Assets/Scenes/SampleScene.unity`

- [ ] **Step 1: Write failing tests for end-of-day gold carryover**

```csharp
using NUnit.Framework;
using Ayla.Domain.Shop;

namespace Ayla.Tests.Domain
{
    public class DayEndFlowTests
    {
        [Test]
        public void CarriesGoldIntoNextDayAfterUpgradeChoice()
        {
            var state = new ShopState(startingGold: 50, brewingStations: 1, queueCapacity: 1);
            var upgrade = new ShopUpgrade("queue", "대기열 확장", 20, UpgradeType.QueueCapacity);

            state.TryPurchase(upgrade);

            Assert.That(state.Gold, Is.EqualTo(30));
            Assert.That(state.QueueCapacity, Is.EqualTo(2));
        }
    }
}
```

- [ ] **Step 2: Run tests to verify they fail if upgrade flow regressed**

Run the Unity EditMode command.

Expected: FAIL if upgrade state APIs are missing or changed.

- [ ] **Step 3: Implement summary and upgrade UI**

```csharp
using Ayla.Domain.Shop;
using TMPro;
using UnityEngine;

public class DaySummaryView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI summaryLabel;

    public void Render(DayResult result, ShopState state)
    {
        summaryLabel.text = $"오늘 수입: {result.EarnedGold}\n응대한 손님: {result.CustomersServed}\n보유 골드: {state.Gold}";
    }
}
```

```csharp
using Ayla.Domain.Shop;
using TMPro;
using UnityEngine;

public class UpgradePanelView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI upgradesLabel;

    public void Render(ShopUpgrade[] upgrades)
    {
        upgradesLabel.text = string.Join("\n", upgrades.Select(x => $"{x.DisplayName} - {x.Cost}G"));
    }
}
```

`GameBootstrap` changes:
- end the day when the order list is exhausted,
- render a summary panel,
- offer at least two upgrades (`추가 작업대`, `대기열 확장`),
- apply the selected upgrade to `ShopState`,
- restart the next day with updated capacity values.

- [ ] **Step 4: Run tests and manual day-loop verification**

Run the Unity EditMode command.

Expected: PASS.

Manual verification in editor:
- day ends after all prototype customers are served,
- summary shows earned gold,
- at least one upgrade can be purchased,
- next day starts with the upgraded stat applied.

- [ ] **Step 5: Commit**

```bash
git add Assets/Scripts/Runtime/UI Assets/Scripts/Runtime/Bootstrap Assets/Scenes/SampleScene.unity
git commit -m "feat: add day end summary and upgrades"
```

## Task 6: Clean the prototype into a usable base branch

**Files:**
- Modify: `Assets/PlayerController.cs`
- Modify: any runtime scripts touched above
- Test: `Assets/Tests/EditMode/Domain/*.cs`

- [ ] **Step 1: Remove dead template code and rename unclear symbols**

```csharp
// Delete the unused PlayerController template entirely if it is not referenced,
// or replace it with a small documented input bridge if the file is still needed.
```

- [ ] **Step 2: Run the full EditMode suite**

Run:

```powershell
& 'C:\Program Files\Unity\Hub\Editor\6000.3.8f1\Editor\Unity.exe' -batchmode -projectPath 'D:\Unity\Ayla' -runTests -testPlatform EditMode -testResults 'D:\Unity\Ayla\Logs\editmode-tests.xml' -quit
```

Expected: PASS for all EditMode tests.

- [ ] **Step 3: Perform the final manual prototype checklist**

Check in `SampleScene`:
- one full day can be played without missing references,
- at least two customer orders can be completed,
- brewing actions are readable without opening a separate codex,
- end-of-day upgrade choice changes the next day,
- the prototype communicates the “small potion shop” feeling even with placeholder art.

- [ ] **Step 4: Commit**

```bash
git add Assets docs/plans/2026-04-29-ayla-first-prototype-plan.md
git commit -m "chore: finalize first potion shop prototype base"
```
