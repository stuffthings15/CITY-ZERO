# Unity Technical Overview

## Recommended Unity Version
- Unity 6 LTS preferred for production stability
- URP recommended for performance, clarity, and stylized top-down lighting
- Input System required
- Cinemachine recommended
- Addressables required
- Unity AI Navigation required
- Entities/DOTS optional for late-stage traffic and crowd scaling

## Core Packages
- Input System
- Cinemachine
- AI Navigation
- Addressables
- Timeline
- TextMeshPro
- URP
- Unity Test Framework
- ProBuilder
- Splines optional

## Architecture Approach
Use a hybrid architecture:
- ScriptableObjects for design-time static content
- JSON for bulk tunable content and external editing
- MonoBehaviour gameplay layer for MVP
- Job System or DOTS later for traffic and crowd scaling

## Scene Layout Strategy
- `Bootstrap.unity` initializes services, save profile, settings, addressables
- `MainMenu.unity` holds title, profile select, options, accessibility
- `Sandbox_Global.unity` is the persistent systems scene
- District scenes load additively by streaming volumes
- Separate test scenes for combat, traffic, AI, missions, save/load

## Assembly Definitions
- `CityZero.Core`
- `CityZero.Gameplay`
- `CityZero.UI`
- `CityZero.Data`
- `CityZero.Editor`
- `CityZero.Tests.EditMode`
- `CityZero.Tests.PlayMode`
