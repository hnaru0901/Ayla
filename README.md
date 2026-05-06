# Ayla

Ayla는 Unity로 제작하는 캐주얼 3D 마녀 경영 및 제작 게임입니다.

초기 목표는 큰 시스템을 먼저 만드는 것이 아니라, 작은 플레이 가능한 핵심 루프를 빠르게 검증하는 것입니다.

## 핵심 루프

1. 재료를 수집합니다.
2. 포션을 제작합니다.
3. 포션을 판매합니다.
4. 의미 있는 업그레이드나 해금을 진행합니다.

## 개발 방향

- 작은 기능 단위로 만들고 검증합니다.
- 초반에는 복잡한 아키텍처보다 빠른 프로토타입을 우선합니다.
- Unity 씬, 프리팹, 직렬화 필드 변경은 신중하게 다룹니다.
- GitHub 이슈, 브랜치, 커밋, PR 규칙은 `WORKFLOW.md`를 따릅니다.
- Codex 및 coding agent 작업 규칙은 `AGENTS.md`를 따릅니다.

## 문서

- `AGENTS.md`: coding agent 작업 지침
- `WORKFLOW.md`: GitHub 이슈, 브랜치, 커밋, PR 규칙
- `Docs/`: 기획, 컨셉, 프로토타입 계획 문서

## Unity

- Unity 6000.3.x LTS 사용
- `Library/`, `Temp/`, `Obj/`, `Logs/`, `Builds/`, `UserSettings/`는 Git에서 제외
- 큰 로컬 전용 에셋은 `.gitignore` 규칙에 맞는 폴더에 보관
