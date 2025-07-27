# Project Name
Bravery RPG Unity 2D Sidescrolling Platformer

# Description
This project is a 2D sidescrolling platformer built in Unity using C#. It includes systems for player movement, physics, enemy AI, item collection, UI, audio (via FMOD), and scene transitions.

# Code Style
- Use PascalCase for class names and public methods.
- Use camelCase for private fields and method parameters.
- Unity fields should be private with `[SerializeField]`.
- Group Unity lifecycle methods in this order: `Awake`, `OnEnable`, `Start`, `Update`, `FixedUpdate`, `OnDisable`, `OnDestroy`.

# Packages & Add-ons
- Text Mesh Pro
- Cinemachine Smart Camera System

# Architecture and Design
- Use `MonoBehaviour` as the base for in-scene components.
- `Player.cs` manages movement, jumping, crouching, and attacking.
- `Enemy.cs` is the abstract base class for enemy types.
- `Inventory_Base.cs` handles item tracking and crafting.
- `ToastManager.cs` displays temporary in-game messages using TextMeshPro.
- `Entity_StatusHandler.cs` manages elemental status effects.
- UI is built with Unity's Canvas + TextMeshPro.

# Tips for Copilot
- Assume Unity-specific context (`Transform`, `Rigidbody2D`, `Animator`, etc.).
- Use `TryGetComponent` where performance matters.
- Use ScriptableObjects for item and stat definitions.
- Prefer `EventChannelSO` or `UnityEvent` for decoupling systems.

# Folder Structure
```
Assets/
├── Scripts/
│ ├── Enemies/
│ ├── Entity/
│ ├── Enums/
│ ├── InteractiveObjects/
│ ├── Interfaces/
│ ├── InventorySystem/
│ ├── Parallax/
│ ├── Player/
│ ├── SaveSystem/
│ ├── SkillSystem/
│ ├── StateMachine/
│ ├── StatSystem/
│ ├── ToastSystem/
│ ├── UI/
│ ├── Utils/
│ ├── Audio/
│ └── Systems/
├── Animations/
├── Data/
├── Graphics/
├── Prefabs/
├── Scenes/
├── Audio/
```
