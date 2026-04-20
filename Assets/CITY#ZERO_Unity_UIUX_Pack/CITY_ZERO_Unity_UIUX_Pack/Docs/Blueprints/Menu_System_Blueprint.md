# Menu System Blueprint

This document outlines a recommended structure for the main menu and pause menu prefabs. These menus should be easily navigable with keyboard, controller, or mouse.

## Main Menu Hierarchy

```
MainMenuCanvas (Canvas)
├── Background (Image)
├── Title (TextMeshProUGUI)
├── ButtonGroup (Vertical Layout Group)
│   ├── NewGameButton (Button)
│   ├── LoadGameButton (Button)
│   ├── OptionsButton (Button)
│   ├── QuitButton (Button)
└── OptionsPanel (Panel) [disabled by default]
```

### Notes

- **MainMenuCanvas** – Screen Space Canvas with a canvas scaler. Use a separate `EventSystem` in the scene or rely on a global one.
- **Background** – Fullscreen image or video player displaying your game’s logo or background art.
- **ButtonGroup** – Use a `Vertical Layout Group` to automatically space buttons. Each `Button` should contain a `TextMeshProUGUI` child with the label.
- **OptionsPanel** – Contains audio/video/control settings. Disable this panel by default and enable when the options button is pressed.

## Pause Menu Hierarchy

```
PauseMenuCanvas (Canvas)
├── Dimmer (Image)
├── MenuBox (Image)
│   ├── ResumeButton (Button)
│   ├── OptionsButton (Button)
│   ├── SaveButton (Button)
│   ├── ExitToMainMenuButton (Button)
└── OptionsPanel (Panel) [disabled by default]
```

### Notes

- **Dimmer** – A semi-transparent overlay to darken the gameplay when the menu is open.
- **MenuBox** – A panel or card containing the menu buttons. Use a `Vertical Layout Group` to stack them.
- **OptionsPanel** – Shared with main menu, or a separate instance if preferred.

## Scripting

Attach the `MenuController` script to the `MainMenuCanvas` and `PauseMenuCanvas`. This script handles showing/hiding panels, responding to button events, and toggling between menus.

### Basic Usage Example

```csharp
public class MenuBootstrap : MonoBehaviour
{
    public MenuController menuController;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Toggle the pause menu
            menuController.TogglePauseMenu();
        }
    }
}
```

Ensure that time is paused when the pause menu is active. The `MenuController` includes a boolean property `isPaused` to help manage your game state.

For controller navigation, set the Navigation settings on each `Button` and use an `EventSystem` with `InputSystemUIInputModule` if using the new Input System.