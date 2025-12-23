# 🏏 Cricket Bowling System

![Unity](https://img.shields.io/badge/Unity-6%20%2F%202022.3%2B-black?style=flat&logo=unity)
![Language](https://img.shields.io/badge/Language-C%23-blue)
![Architecture](https://img.shields.io/badge/Architecture-Object%20Pooling-orange)
![Status](https://img.shields.io/badge/Status-Prototype-success)

**Cricket Bowling System** is a physics-based sports prototype developed in Unity. The core mechanic revolves around **Dual-Mode Ball Physics** (Swing & Spin) and a precision timing system, simulating the technical nuances of professional cricket bowling.

This project demonstrates **memory-managed C# scripting**, utilizing Object Pooling and Singleton patterns to ensure smooth 60fps performance on mobile-grade hardware.

## 🎮 Gameplay Features

* **Dual-Mode Physics Engine**:
    * **Swing Mode**: Uses aerodynamic drag logic to drift the ball in the air based on seam position and speed.
    * **Spin Mode**: Utilizes angular velocity and torque to generate realistic turn (deviation) *after* the ball bounces on the pitch.
* **Precision Meter System**: A UI-based timing mechanic ("PingPong" slider) that determines delivery accuracy. Hitting the "Blue Zone" results in perfect accuracy; missing it introduces error variance.
* **Dynamic Trajectory Calculation**: Real-time vector mathematics calculate the required launch velocity to hit the target length regardless of the selected ball speed.
* **Bowler Side Switching**: Seamless toggling between "Around the Wicket" and "Over the Wicket" with automated model positioning.
* **Pitch Marker Control**: Fully interactive landing spot selection (Line and Length) using keyboard input.

## 🕹️ Controls

| Key | Action |
| :--- | :--- |
| **W, A, S, D** | Move Landing Marker (Pitching Spot) |
| **Space** | **Bowl** (Stop Meter) |
| **UI Buttons** | Toggle Swing/Spin, Select Direction, Switch Side |

## 🛠️ Technical Architecture & Optimizations

This project focuses on **Memory Management** and **Physics Stability**.

### 1. Design Patterns
* **Object Pooling Pattern**:
    * Instead of `Instantiate` and `Destroy`, balls are managed via a `Queue<GameObject>`.
    * *Benefit*: Eliminates Garbage Collection (GC) spikes caused by frequent memory allocation, ensuring stutter-free performance during rapid bowling.
* **Singleton Pattern**: Implemented on `CricketGameManager` to provide a centralized access point for game state, UI updates, and input handling.
* **State Reset Logic**: A dedicated `ResetPhysics()` method ensures reused objects have their linear and angular velocity completely zeroed out before the next throw, preventing "ghost forces" from previous simulations.

### 2. Performance Optimizations
* **Physics Throttling**:
    * Cached `Rigidbody` and `Component` references in `Awake()` to avoid expensive `GetComponent()` calls during the physics loop.
    * Used `WaitForSeconds` caching in Coroutines to avoid allocating new memory for delay timers.
* **Input Safety**:
    * Implemented boolean flags (`canBowl`) to lock inputs during active states, preventing double-execution errors or state desynchronization.

### 3. Core Scripts Overview
* **`CricketGameManager.cs`**: The brain of the application. Handles the game loop, object pooling, UI updates, and the math for calculating launch velocity ($v = d/t$).
* **`BallController.cs`**: Handles the individual physics of the ball. It applies `ForceMode.Acceleration` for swing drift and modifies angular velocity upon collision for spin.
* **`MeterSystem.cs`**: A standalone module managing the UI timing slider and returning a normalized accuracy float (0.0 to 1.0).

## 🚀 How to Play (Installation)

1.  **Clone this repo**: `git clone https://github.com/YOUR_USERNAME/Unity-Cricket-Bowling-System.git`
2.  **Open in Unity**:
    * Launch Unity Hub.
    * Add the project folder.
    * Open `Assets/Scenes/SampleScene` (or Main Scene).
3.  **Play**: Press the Play button in the Editor.

## 📄 License
This project is for educational purposes as part of a Unity Developer portfolio.

---
*Developed by [Your Name]*
