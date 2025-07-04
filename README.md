# Unity2DRPG
Unity 2D RPG (Role Playing) Game.


# Environment Setup

Unity version: **6000.1.2f1** - May 6, 2025  

https://unity.com/releases/editor/archive  


# VS Code Preferences

Enable Setting 'Format On Save' - The current file will be formatted when you press CMD+S.

Open Settings, search for "exclude", under "Files: Exclude", click on **Add Pattern**. Type
`**/bin` and click **OK**. And do the same for `**/obj`. This will hide these folders from the
Solution Explorer, since we won't interact with them very often.

Within Settings, search for "bracket" and make sure these two settings are Enabled:
- Auto Closing Brackets - Always
- Bracket Pair Colorization: Enabled - Checked
- Bracket Pair Colorization: Independent Color Pool Per Bracket Type - Unchecked
- Guides: Bracket Pairs - True

Open the **Command Palette** with: `SHIFT + CMD + P` (MacOS).

Open the editor's **More Actions...** contextual menu with `CMD + .` (MacOS); this will provide 
you with helpful quick actions like "Remove unnecessary usings", or "Generate constructor".

Open the **Keyboard Shortcuts** window under Settings, then click on the small icon in the
top-right corner with tooltip "Open Keyboard Shortcuts (JSON)" (the icon looks like a piece
of paper with a folded corner, and a circular arrow on the left). In the `keybindings.json`
file, add this entry:

```json
{
    "key": "shift shift",
    "command": "workbench.action.quickOpen"
}
```

Save the `keybindings.json` file and then close it. Now, when you double-tap SHIFT, it will open
up a sort of "Global Object Search" form field, and you can type the name of an entity, like
our `AppUser.cs`, and then press RETURN to open the file. Super-handy to have!

More details:
https://stackoverflow.com/questions/29613191/intellij-shift-shift-shortcut-in-visual-studio-global-search  

Under **Settings > CodeLens**, turn off "Show Main Code Lens". It adds extraneous noise to every 
method signature in the editor UI, with a bunch of "N references" indicators everywhere.


# Unity Tips & Tricks

## General
* Unity 6 Engine Lifecycle Execution Order: https://docs.unity3d.com/6000.0/Documentation/Manual/execution-order.html
* The Unity documentation can be found [here](https://docs.unity3d.com/Manual/index.html) 
  and the scripting reference can be found [here](https://docs.unity3d.com/ScriptReference/index.html)
* You can modify the template used by Unity when creating new scripts by editing 
  `Unity\Editor\Data\Resources\ScriptTemplates\81-C# Script-NewBehaviourScript.cs.txt`. 
  You can add a header, tidy up the using statements, setup tabs to your liking, add regions etc.
* When creating a new Unity project make sure to select `2D` or `3D` depending on what 
  type of project you are working on
* C# is probably the best choice of language for Unity. The vast majority of resources 
  available use C# e.g. documentation, tutorials, YouTube videos

## User Interface
* To reset all the editor views back to default you can click `Window -> Layouts -> Default`
* You can drag windows to different locations by left clicking and holding the title bar 
  and dragging it to where you want it
* When playing the game in Unity any changes you make in the editor will not be persisted. 
  To make that obvious you can change the UI tint used when playing with `Edit -> Preferences -> Colors -> Playmode tint`
* The five buttons on the top left of the main window are (with keyboard shortcuts):
  * Pan scene (`Q`)
  * Move objects (`W`)
  * Rotate objects (`E`)
  * Scale objects (`R`)
  * Rect tool used for UI tranforms (`T`)
* The five buttons on the top left of the `Scene` window are:
  * Render mode
  * 3D or 2D mode
  * Lighting on or off
  * Audio on or off
  * Effects on or off
* In 3D mode you can use the `mouse wheel` to zoom in and out, hold the `middle mouse button` 
  to pan and hold the `right mouse button` to look around. In 2D mode it is the same except 
  `right mouse button` also pans
* To zoom in/out on a specific object, double click it in the `Hierarchy` window
* You can drag and drop files in to Unity
* Get a larger view when playing the game in Unity by clicking the `Game` window 
  and enabling `Maximize On Play`
* You can use the mouse to increase and decrease numeric values in the `Inspector` 
  window by holding down the left mouse button on the label and moving the mouse
* You can see how your game will look in different resolutions by clicking the second 
  dropdown on the top of the `Game` window and selecting a resolution

## Keyboard Shortcuts
* Other useful keyboard shortcuts are:
  * `Ctrl + P` to play or stop

## Engine
* Some of the more important event functions are (full details can be found [here](https://docs.unity3d.com/Manual/ExecutionOrder.html)):
  * `Awake` is called on each object when the scene starts and before any Start functions are called
  * `Start` is called once only on each object before the first frame update
  * `Update` is called on each object every frame
* You can change the background color when playing by selecting the camera and changing 
  the `Background` property in the `Inspector` window

## Git
* To turn off the `LF will be replaced by CRLF` warnings in Windows execute the following 
  in a command prompt: `git config --global core.safecrlf false`

## Prefabs
* You can use prefabs to make it easier to work with multiples of the same object that share some or all of the same properties
* To do this you must first have the base object in the scene, so either create it as required or drag it from the `Assets` window
* Drag the object from the `Hierarchy` window to a `Prefabs` folder in the `Assets` window
* It will turn a blue color in the `Hierarchy` window to indicate it is a prefab
* You can now delete it from the `Hierarchy` window if needed
* Click the prefab in the `Assets` window and make whatever changes you need e.g. rename it, set some base properties
* Now drag the prefab as many times as you need in to the scene
* Any settings that differ for the object in the scene from the prefab will be shown in bold in the `Inspector` window
* You can revert an object in the scene back to the prefab defaults by clicking `Prefab -> Revert` in the `Inspector` window


# Screenshots

| Sprite Sheet Editor - Grid By Cell Size |
| :---: |
| ![Sprite Sheet Editor](Screenshots/sprite-sheet-editor-01.png) |

| Animator UI - Exit Time & Transition |
| :---: |
| ![Animator UI](Screenshots/animator-ui-01.png) |



# Architecture Overview

The Bravery RPG project implements a sophisticated State Machine pattern for both player and enemy control:

## Core Architecture

1. **Entity** is the base class for all game actors and:
   - Inherits from MonoBehaviour
   - Contains common properties like Animator, RigidBody2D
   - Handles collision detection, flipping sprites, and velocity setting
   - Contains a StateMachine reference

2. **StateMachine** manages state transitions and tracks the current state for any Entity.

3. **EntityState** is the abstract base class for all states with standard lifecycle methods:
   - Enter: Called when entering a state
   - Update: Called every frame while in a state 
   - Exit: Called when leaving a state

## Player Architecture

1. **Player** extends Entity with:
   - Input handling via PlayerInputSet
   - Multiple state instances for different player behaviors
   - Combat mechanics like attack velocity and damage dealing
   - Movement parameters like speed, jump force, and dash duration

2. Player States are organized in a hierarchy:
   - **PlayerState** (base for all player states)
     - **Player_GroundedState** (base for states when player is on ground)
       - Player_IdleState
       - Player_MoveState
     - **Player_AiredState** (base for states when player is in air)
       - Player_JumpState
       - Player_FallState
     - Player_DashState
     - Player_BasicAttackState
     - Player_JumpAttackState
     - Player_WallSlideState
     - Player_WallJumpState

3. **Player_AnimationTriggers** allows animation events to trigger player state changes.

## Enemy Architecture

1. **Enemy** extends Entity with:
   - Basic damage handling
   - Movement parameters and idle time configuration
   - Visual feedback (color change when damaged)

2. **Enemy_Skeleton** extends Enemy for specific enemy type implementation.

3. Enemy States:
   - **EnemyState** (base for all enemy states)
     - Enemy_IdleState
     - Enemy_MoveState

This architecture allows for:
- Clean separation between different behaviors through the state pattern
- Code reuse through inheritance hierarchies
- Easy addition of new states and behaviors
- Clear distinction between player and enemy behaviors


# Diagrams

## Class Hierarchy for Bravery RPG

### Entity Hierarchy Diagram

```mermaid
---
title: Entity Hierarchy Diagram
config:
  theme: neutral
  layout: elk
---
classDiagram
    direction TB
    
    MonoBehaviour <|-- Entity
    Entity <|-- Player
    Entity <|-- Enemy
    Enemy <|-- Enemy_Skeleton
    MonoBehaviour <|-- Player_AnimationTriggers
    
    class Entity {
        +Animator Anim
        +Rigidbody2D Rb
        #StateMachine stateMachine
        +int FacingDir
        +bool GroundDetected
        +bool WallDetected
        #virtual void Awake()
        #virtual void Start()
        #virtual void Update()
        +void CallOnNextActionInputReadyTrigger()
        +void CallOnAnimationEndedTrigger()
        +void SetVelocity(float, float)
        +void Flip()
    }
    
    class Player {
        +PlayerInputSet Input
        +Player_IdleState IdleState
        +Player_MoveState MoveState
        +Player_JumpState JumpState
        +Player_FallState FallState
        +Player_DashState DashState
        +Player_BasicAttackState BasicAttackState
        +Player_JumpAttackState JumpAttackState
        +Player_WallSlideState WallSlideState
        +Player_WallJumpState WallJumpState
        +Vector2[] attackVelocity
        +Vector2 jumpAttackVelocity
        +float moveSpeed
        +float jumpForce
        +Vector2 wallJumpForce
        +float dashDuration
        +float dashSpeed
        +Vector2 MoveInput
        +void DamageEnemies()
        +void EnterAttackStateWithDelay()
    }
    
    class Enemy {
        +Enemy_IdleState IdleState
        +Enemy_MoveState MoveState
        +float idleTime
        +float moveSpeed
        +float moveAnimSpeedMultiplier
        +float timer
        +void TakeDamage()
    }
    
    class Enemy_Skeleton {
    }
    
    class Player_AnimationTriggers {
        -Player player
        +void OnNextActionInputReady()
        +void OnAnimationEnded()
    }
    
    Player_AnimationTriggers --> Player : triggers animations
    Entity *-- StateMachine
    Player *-- PlayerInputSet
    
    class StateMachine {
        +EntityState CurrentState
        +Initialize(EntityState startState)
        +ChangeState(EntityState newState)
        +UpdateActiveState()
    }
```


### EntityState Hierarchy Diagram

```mermaid
---
title: EntityState Hierarchy Diagram
config:
  theme: neutral
  layout: dagre
---
classDiagram
    direction TB
    
    class EntityState {
        #Entity entity
        #StateMachine stateMachine
        #string animBoolName
        #float stateTimer
        +Enter()
        +Update()
        +Exit()
        +CallOnNextActionInputReadyTrigger()
        +CallOnAnimationEndedTrigger()
    }
    
    EntityState <|-- PlayerState
    EntityState <|-- EnemyState
    
    PlayerState <|-- Player_GroundedState
    PlayerState <|-- Player_AiredState
    PlayerState <|-- Player_DashState
    PlayerState <|-- Player_BasicAttackState
    PlayerState <|-- Player_JumpAttackState
    PlayerState <|-- Player_WallSlideState
    PlayerState <|-- Player_WallJumpState
    
    Player_GroundedState <|-- Player_IdleState
    Player_GroundedState <|-- Player_MoveState
    
    Player_AiredState <|-- Player_JumpState
    Player_AiredState <|-- Player_FallState
    
    EnemyState <|-- Enemy_IdleState
    EnemyState <|-- Enemy_MoveState
    
    class PlayerState {
        #Player player
        #PlayerInputSet input
        #Animator anim
        #Rigidbody2D rb
        #bool onNextComboAttackReadyTrigger
        #bool onAnimationEndedTrigger
    }
    
    class EnemyState {
        #Enemy enemy
    }
    
    class Player_GroundedState {
        +Update()
    }
    
    class Player_AiredState {
        +Update()
    }
```

# Unit Testing Framework

## Standalone Testing (No Unity Editor Required) ✅

This project includes a comprehensive unit testing framework that runs independently of Unity Editor using standard .NET tooling and the Moq mocking framework. This approach is ideal for:

- **Corporate environments** with proxy/firewall restrictions
- **CI/CD pipelines** without Unity licenses  
- **Fast developer feedback** loops (~1 second test execution)
- **Testing pure game logic** and calculations
- **Mock-based testing** for complex dependencies

### Quick Start

```bash
# Run all tests with mocking support
./run_simple_test.sh

# Make script executable if needed
chmod +x run_simple_test.sh
```

### VS Code Integration

1. Press `Cmd+Shift+P` (macOS) or `Ctrl+Shift+P` (Windows/Linux)
2. Type "Tasks: Run Task"
3. Select "Run Standalone Tests"

### Current Test Coverage (27 Tests Passing) ✅

**RPGMath Utility Functions (12 tests):**
- ✅ Damage calculation with bonuses
- ✅ Percentage conversions
- ✅ Armor mitigation formulas
- ✅ Critical hit damage calculations
- ✅ Value clamping utilities
- ✅ Edge case handling (zero/negative values)

**StateMachine Behavior (8 tests):**
- ✅ State initialization and transitions
- ✅ Lifecycle management (Enter/Update/Exit)
- ✅ State change permissions
- ✅ Multi-state transition chains
- ✅ **Moq framework integration** for mocking

**Vector2Utils Operations (7 tests):**
- ✅ Deep copy functionality with mock vectors
- ✅ Array independence verification
- ✅ Edge cases (null, empty arrays)
- ✅ Performance testing for large datasets

### Testing Architecture

- **Pure Logic Separation**: Game calculations extracted from Unity MonoBehaviour dependencies
- **Mocking Support**: Uses Moq framework for testing complex interactions
- **Test Doubles**: Custom test implementations for Unity-dependent classes
- **Cross-Platform**: Works on any machine with .NET SDK (no Unity required)

### Framework Features

- **NUnit 3.x** - Industry standard testing framework
- **Moq 4.x** - Professional mocking library for .NET
- **Test Discovery** - Automatic test detection and execution
- **Detailed Reporting** - Console output with test results and timing
- **VS Code Tasks** - Integrated development workflow

### Adding New Tests

1. **Extract pure logic** from Unity classes into utility classes (like `RPGMath`)
2. **Create corresponding tests** in the `StandaloneTests/` project
3. **Use mocking** for complex dependencies with `Mock<T>` from Moq
4. **Run tests** with `./run_simple_test.sh` to verify functionality

### Requirements

- **.NET SDK 6.0+** (no Unity Editor or license required)
- **Corporate proxy compatible** (no Unity package downloads)
- **Offline capable** (all dependencies cached locally)
