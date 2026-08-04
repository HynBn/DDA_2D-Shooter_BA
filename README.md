# Project DDA: 2D Shooter

A fast-paced 1v1 top-down shooter developed as part of a bachelor thesis at HTW Berlin.

Test your aiming, movement and reflexes against an AI-controlled opponent. Choose between predefined difficulty levels or a custom-built Dynamic Difficulty Adjustment system that reacts to your performance during the match.

## Play the Game

The compiled game is available for Windows, macOS and Linux:

[Play Project DDA: 2D Shooter on itch.io](https://hyn-bn.itch.io/project-dda-2d-shooter)

This repository contains the Unity source project.

## About the Game

In Project DDA, the player competes against an AI-controlled opponent across multiple rounds.

The goal is not simply to create the strongest possible opponent. Instead, the project explores whether an adaptive AI can provide a suitable challenge without requiring the player to change the difficulty manually during a match.

Before starting, the player can select one of two difficulty systems:

- Static difficulty
- Dynamic Difficulty Adjustment

## Difficulty Systems

### Static Difficulty

The opponent uses predefined parameters that remain unchanged throughout the match.

Three static difficulty levels are available:

- **Easy**
- **Medium**
- **Hard**

The levels differ in parameters such as accuracy, movement, firing behavior and reaction capabilities.

### Dynamic Difficulty Adjustment

The DDA system evaluates the player's performance during and between rounds. Based on the collected metrics, it can increase or decrease the opponent's difficulty.

Two adaptive variants are available:

#### Behavioral DDA

The behavioral variant primarily changes behavior-related opponent parameters, including:

- Aiming accuracy
- Attack and retreat distances
- Awareness range
- Strafing behavior
- Dodging behavior
- Dash behavior

The intention is to adjust the opponent while preserving the basic numerical balance between the player and the AI.

#### Numerical DDA

The numerical variant includes the behavioral adjustments and can additionally modify direct gameplay values, including:

- Movement speed
- Firing rate
- Projectile speed
- Dash cooldown

This allows stronger difficulty changes but can also make the differences between difficulty levels more noticeable to the player.

## Performance Evaluation

The DDA system evaluates several aspects of player performance, including:

- Hit rate
- Damage taken
- Round duration
- Round outcome
- Shooting activity
- Dash usage
- Successful avoidance behavior

Adjustments are performed both during gameplay and after completed rounds. The opponent's parameters are restricted by predefined limits to prevent extreme or unplayable configurations.

## Controls

| Action | Input |
|---|---|
| Movement | W, A, S, D |
| Aim | Mouse |
| Shoot | Left mouse button |
| Dash | Space |
| Pause or open menu | Escape |
| Menu interaction | Mouse |

## Academic Background

This prototype was developed as part of a bachelor thesis in International Media and Computing at HTW Berlin.

The thesis examines the implementation and evaluation of Dynamic Difficulty Adjustment in a 2D top-down shooter. Static difficulty settings are compared with performance-based adaptive difficulty systems.

The evaluation focuses on questions such as:

- How can player performance be measured during gameplay?
- Which opponent parameters are suitable for dynamic adjustment?
- How do behavioral and numerical adjustments differ?
- Can adaptive difficulty provide a more appropriate challenge?

The official evaluation was conducted with a closed group of playtest participants. The collected telemetry was analyzed descriptively as part of the thesis.

## Telemetry and Data Export

Telemetry is recorded only after the player has explicitly consented within the application.

After each completed round, the game can store the following information:

- Timestamp
- Match round
- Selected difficulty system
- Selected difficulty variant
- Round duration
- Match outcome
- Player hit rate
- Damage taken
- Number of dashes
- Number and type of DDA interventions

The data is stored locally in:

`Playtest_Telemetry.csv`

The corresponding directory can be opened through the data button in the application.

No telemetry is transmitted to an external server.

## Opening the Unity Project

### Requirements

- Unity Hub
- Unity Editor **6000.4.5f1**
- Windows or macOS for development

Using the specified Unity version is recommended to avoid compatibility problems.

### Instructions

1. Download or clone this repository.
2. Open Unity Hub.
3. Select **Add project from disk**.
4. Select the project directory.
5. Open the project using Unity **6000.4.5f1**.
6. If it is not opened automatically, open `Assets/Scenes/Main Scene.unity`.
7. Press the Play button in the Unity Editor.

The `Library` directory is intentionally excluded from the repository. Unity regenerates it when the project is opened for the first time, so the initial import may take several minutes.

## Third-Party Assets

The project uses the following assets from the Unity Asset Store:

- [Shooting Sound](https://assetstore.unity.com/packages/audio/sound-fx/shooting-sound-177096)
- [Free UI Click Sound Pack](https://assetstore.unity.com/packages/audio/sound-fx/free-ui-click-sound-pack-244644)
- [Free Sci-Fi and Cyberpunk Music Pack](https://assetstore.unity.com/packages/audio/ambient/sci-fi/free-sci-fi-and-cyberpunk-music-pack-264590)
- [Game Input Controller Icons Free](https://assetstore.unity.com/packages/2d/gui/icons/game-input-controller-icons-free-285953)
- [Pixel Art UI Essentials](https://assetstore.unity.com/packages/2d/gui/pixel-art-ui-essentials-329983)
- [RPG Essentials Sound Effects Free](https://assetstore.unity.com/packages/audio/sound-fx/rpg-essentials-sound-effects-free-227708)

## Licensing Notice

This repository is provided for the examination, documentation and reproducibility of the associated bachelor thesis.

All third-party assets remain the property of their respective authors and are subject to their individual licence terms. They are not covered by any licence applying to the original source code.

Unless explicitly stated otherwise, no open-source licence is granted for the original source code of this project.
