# WORKFLOW.md

Ayla repository workflow rules.

## Issue Titles

Use bracketed type names for issue titles.

Examples:

- `[Feat] 포션 제작 기본 흐름 추가`
- `[Bug] 제작 UI에서 빈 레시피 선택 시 오류 수정`
- `[Refactor] 인벤토리 책임 분리`
- `[Chore] GitHub 템플릿 정리`
- `[Docs] 포션 시스템 기획 정리`

## Commit Messages

Use Conventional Commit style, write the description in Korean, and include the related issue number.

Format:

```text
type: description (#issue-number)
```

Examples:

```text
feat: 포션 제작 기본 흐름 추가 (#12)
fix: 제작 UI에서 빈 레시피 선택 시 오류 수정 (#18)
refactor: 인벤토리 책임 분리 (#27)
chore: GitHub 템플릿 정리 (#31)
docs: 포션 시스템 기획 정리 (#34)
```

## Branch Names

Use lowercase type names, a kebab-case English description, and the related issue number.

Format:

```text
type/kebab-case-description/issue-number
```

Examples:

```text
feat/basic-potion-crafting/12
fix/empty-recipe-selection-error/18
refactor/inventory-responsibilities/27
chore/github-templates/31
docs/potion-system-direction/34
```

## Base Branch

Use `develop` as the default integration branch.

- Create feature, docs, chore, fix, and refactor branches from `develop`.
- Open pull requests against `develop`.
- Do not open pull requests against `main`.
- `main` is not used for active development.

## Pull Request Titles

Use the same style as commit messages and include the related issue number.

Format:

```text
type: description (#issue-number)
```

Example:

```text
feat: 포션 제작 기본 흐름 추가 (#12)
```

## Pull Request Metadata

After creating a pull request, assign the owner and add the matching label with `gh pr edit`.

Format:

```text
gh pr edit <pr-number> --add-assignee hnaru0901 --add-label "<label-name>"
```

Examples:

```text
gh pr edit 12 --add-assignee hnaru0901 --add-label "✨ feature"
gh pr edit 18 --add-assignee hnaru0901 --add-label "🐛 bug"
gh pr edit 27 --add-assignee hnaru0901 --add-label "♻️ refactor"
gh pr edit 31 --add-assignee hnaru0901 --add-label "🧹 chore"
gh pr edit 34 --add-assignee hnaru0901 --add-label "📝 docs"
```

## Closing Issues

Use the PR body to close issues automatically.

Example:

```text
Closes #12
```

Do not rely on commit messages to close issues automatically. Commit messages should reference issues with `(#12)`, while PR bodies should use `Closes #12`.

## Types

Use these types by default:

- `feat`: new feature or gameplay behavior
- `fix`: bug fix
- `refactor`: internal structure change without behavior change
- `chore`: maintenance, setup, tooling, or small non-feature work
- `docs`: documentation, planning, design notes, or art direction
