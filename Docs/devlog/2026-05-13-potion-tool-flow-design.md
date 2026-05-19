# 2026-05-13 포션 제작 도구 흐름 논의

## 오늘 논의한 것

- 다음 작업 후보를 `#14 포션 제작 도구 기본 흐름 구현`으로 정리했습니다.
- 단순히 `Space` 한 번으로 아이템이 즉시 변환되는 방식은 제작 도구 느낌이 약해서 제외했습니다.
- 목표는 실제 게임 최종 구조가 아니라, 첫 playable에서 여러 도구를 동시에 굴리는 감각을 검증하는 것입니다.

## #14 기본 제작 흐름

- `Shelf`: `RawIngredient` 획득
- `Mortar`: `RawIngredient`를 넣고 작업해 `PreparedIngredient`로 만듭니다.
- `Cauldron`: `PreparedIngredient`를 넣고 작업해 `PotionBase`로 만듭니다.
- `BottlingStation`: `PotionBase`를 넣고 작업해 `Potion`으로 만듭니다.
- 각 도구는 재료 넣기, 작업 시작, 진행 대기, 완료 상태 유지, 결과물 집기 흐름을 가집니다.

## 임시 배치 방향

- `Shelf x1`
- `Mortar x2`
- `Cauldron x2`
- `BottlingStation x1`
- `TrashBin x1`

## 판단한 것

- 도구를 2개씩 두면 슬롯 UI보다 작업실 동선과 병목을 더 직접적으로 확인할 수 있습니다.
- 병입대는 1개만 두어 마지막 공정 병목이 운영/관리 느낌을 주는지 봅니다.
- 병입대 타이밍 판정, 양 조절, 품질 보너스는 이번 범위에서 제외합니다.

## 다음 구현 범위

- `CarryItemType`에 `PreparedIngredient`, `PotionBase`, `Potion`을 추가합니다.
- 도구에 아이템 넣기, 작업 시작, 대기 중 이동 가능, 완료 상태 유지, 결과물 집기 흐름을 구현합니다.
- 레시피 시스템, 인벤토리 시스템, 품질 등급, 병입대 미니게임, 복잡한 UI는 제외합니다.
