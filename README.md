# 🍎 Fruit Match

**Match3** is a simple infinite Match-3 prototype built in Unity. The player swaps neighboring fruits to create lines of matching elements. Matched fruits disappear, the grid collapses downward, and new fruits are generated at the top.

🎥 Gameplay

(no gif yet)

## 📋 Task Summary

- Grid size: n × m
- Grid is filled with random fruits at the start
- Player can swap only adjacent fruits
- Lines of 3+ matching fruits are destroyed
- Fruits above fall down to fill empty cells
- New fruits spawn from the top

## 🧩 Architecture

The project follows the MVP (Model–View–Controller) pattern with Configs (ScriptableObjects)

## Match & Swap Logic

Swap happens only between adjacent cells

If the swap creates a valid match, fruits are removed

If no match is created, the swap is reverted

Matched fruits are cleared simultaneously, then gravity is applied

## 🔧 Plugins Used

VContainer — Dependency injection

UniTask — Lightweight async operations

DoTween — UI/animation transitions

Odin Inspector — Editor improvements
