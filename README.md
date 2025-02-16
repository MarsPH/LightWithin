# Light Within  
[![Game Jam](https://img.shields.io/badge/Game%20Jam-McGameJam%202025-blue)](https://your-mcgamejam-link.com)
[![Unity](https://img.shields.io/badge/Made%20With-Unity-FF5733)](https://unity.com/)  

**Overview**  
Light Within is a 2D video game developed for McGameJam 2025 within 48 hours. The game follows the journey of a shadow that has lost its physical body and is trying to regain it. Players must navigate through the environment while avoiding sources of light, as exposure increases their stress levels. Developed on a 3D X-Z plane in Unity, the game casts shadows onto the ground to serve as the movement surface, achieved through a custom script that generates a fake shadow with physical properties.  

<p align="center">
  <img src="https://media0.giphy.com/media/v1.Y2lkPTc5MGI3NjExa3JoNHBncm1hY3J3enl1cGJnczYwaXh6a3psaG53cXpoc2lqczkweSZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/4eUNU8X7ShshIpyLHU/giphy.gif">
</p>

---  

## 🧩 Key Components  

- **🕶️ Shadow3DPlatform.cs**  
  - Projects dynamic shadows onto designated surfaces.  
  - Supports both directional and point lights by generating and updating a custom shadow mesh.  

- **🎮 ShadowController.cs**  
  - Manages movement, jumping, and collision detection on shadow surfaces.  
  - Integrates sound effects and animations to enhance player interaction.  

- **💡 LightDamageEffect.cs**  
  - Applies damage and recovery mechanics based on player exposure to light.  
  - Adjusts post-processing effects (bloom, vignette) and displays dynamic "Darkness" text effects.  
  - Handles fade-to-black transitions and scene reloads upon game over.  

---  

## 📌 Installation & Running the Game  

```bash
git clone https://github.com/your-username/light-within.git
cd light-within
```
Run in Unity **2022.x or newer**.

---  

## 🏆 Contributors  
- **[Mahan](https://github.com/MarsPH)** – Developer  
