# AGENTS.md

Behavioral guidelines to reduce common LLM coding mistakes in the StarlitWitch Unity project.

These instructions are for Codex and other coding agents working in this repository.

**Tradeoff:** These guidelines bias toward caution over speed. For trivial, low-risk tasks, use judgment and prefer the simplest reversible change.

---

## 1. Development Philosophy

### Think Before Coding

**Do not assume. Do not hide confusion. Surface tradeoffs.**

Before implementing:

- State assumptions explicitly when they affect the solution.
- If multiple interpretations exist, present them instead of silently choosing one.
- If a simpler approach exists, say so.
- Push back when a requested change seems risky, overcomplicated, or unnecessary.
- If something is unclear and affects architecture, data loss, asset references, serialized fields, scene references, or public APIs, stop and ask.
- For low-risk, reversible choices, state the assumption and proceed with the simplest option.

### Simplicity First

**Minimum code that solves the problem. Nothing speculative.**

- No features beyond what was asked.
- No abstractions for single-use code.
- No flexibility or configurability that was not requested.
- Avoid defensive error handling for truly impossible scenarios.
- In Unity, keep simple guards for inspector-assigned references, scene dependencies, and user-facing runtime boundaries.
- If you write 200 lines and it could be 50, rewrite it.

Ask yourself:

> Would a senior engineer say this is overcomplicated?

If yes, simplify.

### Surgical Changes

**Touch only what you must. Clean up only your own mess.**

When editing existing code:

- Do not improve adjacent code, comments, or formatting unless required.
- Do not refactor things that are not broken.
- Match the existing style, even if you would normally do it differently.
- If you notice unrelated dead code, mention it instead of deleting it.

When your changes create unused code:

- Remove imports, variables, methods, or classes that your own changes made unused.
- Do not remove pre-existing dead code unless explicitly asked.

The test:

> Every changed line should trace directly to the user's request.

---

## 2. Unity-specific Rules

This is a Unity project.

### Unity Version

- Use Unity 6000.3.x LTS.
- Do not upgrade the Unity version unless explicitly requested.
- Do not change project settings that affect the whole project unless explicitly requested.

### Files and Folders

Do not edit, delete, or generate files under:

- `Library/`
- `Temp/`
- `Obj/`
- `Logs/`
- `Builds/`
- `UserSettings/`

Do not manually delete or regenerate `.meta` files.

Avoid direct YAML edits to these files unless explicitly requested:

- `.unity`
- `.prefab`
- `.asset`
- `.mat`
- `.anim`
- `.controller`

Prefer making changes through C# scripts, Unity Editor workflows, or clearly scoped asset changes.

### Unity Serialization Safety

Be careful with:

- `[SerializeField]` fields
- public fields used by the Inspector
- prefab references
- scene references
- ScriptableObject references
- animation references
- object names referenced from code

Do not rename serialized fields casually.

If a serialized field must be renamed, use `FormerlySerializedAs` when appropriate.

Example:

```csharp
using UnityEngine;
using UnityEngine.Serialization;

public class ExampleComponent : MonoBehaviour
{
    [FormerlySerializedAs("_oldFieldName")]
    [SerializeField] private GameObject _newFieldName;
}
```

### Architecture

Prefer simple Unity patterns first.

- Prefer small MonoBehaviour components.
- Prefer readable gameplay code over clever architecture.
- Do not introduce ECS, Addressables, custom dependency injection containers, complex event buses, or large framework-style architecture unless explicitly requested.
- Do not build generalized systems before there is a clear repeated need.
- Prioritize playable prototypes over abstract systems.

---

## 3. Project Direction

StarlitWitch is a casual 3D witch management and crafting game.

The early development goal is not to build a complete architecture. The goal is to quickly verify a small playable core loop:

1. Gather materials.
2. Craft potions.
3. Sell potions.
4. Upgrade or unlock something meaningful.

Prefer small, testable gameplay slices over large speculative systems.

When unsure, optimize for:

- a working prototype
- simple controls
- clear player feedback
- low implementation risk
- easy iteration

---

## 4. Goal-driven Execution

Define success criteria. Loop until verified.

Transform tasks into verifiable goals.

Examples:

- "Add validation" means "Check invalid inputs, then make the behavior correct."
- "Fix the bug" means "Reproduce the bug, then verify that it no longer happens."
- "Refactor X" means "Confirm behavior before and after the change."

For multi-step tasks, state a brief plan:

1. `[Step]` -> verify: `[check]`
2. `[Step]` -> verify: `[check]`
3. `[Step]` -> verify: `[check]`

Strong success criteria let agents work independently.

Weak criteria like "make it work" require clarification.

---

## 5. Repository Workflow

When working with GitHub issues, commits, branches, pull requests, labels, or templates, follow `WORKFLOW.md`.

In particular:

- Use the issue, commit, and pull request naming rules from `WORKFLOW.md`.
- Use the branch naming rules from `WORKFLOW.md` before creating or renaming branches.
- Keep issue titles, commit descriptions, and pull request descriptions understandable to the project owner.
- Reference related issues in commits and pull requests according to `WORKFLOW.md`.
- After creating a pull request, apply the matching assignee and label according to `WORKFLOW.md`.
- Use pull request bodies, not commit messages, to close issues automatically.

---

## 6. Unity Verification

For Unity tasks, verification may include:

- C# scripts compile without Unity Console errors.
- The relevant scene enters Play Mode.
- The changed behavior is manually checked in Play Mode.
- The Console has no new errors caused by the change.
- Prefab and scene references remain intact.
- Inspector-assigned fields are still assigned.
- EditMode or PlayMode tests are added when the system is stable enough to test.

If Unity Editor execution is unavailable, explain what should be checked manually in the Editor.

Do not claim that a Unity behavior was verified in Play Mode unless it was actually checked.

---

## 7. Success Signals

These guidelines are working if:

- diffs contain fewer unnecessary changes
- fewer rewrites are needed due to overcomplication
- clarifying questions happen before implementation mistakes
- Unity references are not accidentally broken
- small playable features appear before large architecture
