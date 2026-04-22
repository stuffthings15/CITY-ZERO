using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CityZero.UI
{
    /// <summary>
    /// Controls a simple minimap that projects world positions to a UI panel. It supports
    /// registering dynamic points of interest (POIs) such as enemies or objectives. The
    /// minimap uses a scaling factor to map world space to minimap space and assumes
    /// that the minimap panel is square. You can extend this class to support more
    /// complex shapes or to render the world with a camera and render texture.
    /// </summary>
    public class MinimapController : MonoBehaviour
    {
        [Tooltip("Transform of the player object. The minimap will follow this position.")]
        public Transform playerTransform;

        [Tooltip("RectTransform of the minimap panel. Icons will be constrained to this area.")]
        public RectTransform minimapRect;

        [Tooltip("UI Image representing the player's icon on the minimap.")]
        public RectTransform playerIcon;

        [Tooltip("Container for dynamically created POI icons.")]
        public RectTransform poiContainer;

        [Tooltip("Prefab used for point of interest icons. Must contain an Image component.")]
        public GameObject poiIconPrefab;

        [Tooltip("World units per minimap unit. A larger value zooms the minimap out.")]
        public float worldToMinimapScale = 10f;

        // Dictionary to track POIs and their corresponding UI icons
        private readonly Dictionary<Transform, RectTransform> _poiIcons = new();

        private void LateUpdate()
        {
            if (playerTransform == null || minimapRect == null || playerIcon == null) return;

            // Update player icon position (centered) and rotation
            playerIcon.anchoredPosition = Vector2.zero;
            playerIcon.localRotation = Quaternion.Euler(0f, 0f, -playerTransform.eulerAngles.y);

            // Update POI icons
            foreach (var kvp in _poiIcons)
            {
                Transform target = kvp.Key;
                RectTransform icon = kvp.Value;

                if (target == null)
                {
                    // Clean up destroyed targets
                    Destroy(icon.gameObject);
                    _poiIcons.Remove(target);
                    break;
                }

                Vector3 offset = target.position - playerTransform.position;
                Vector2 minimapPos = new Vector2(offset.x, offset.z) / worldToMinimapScale;
                icon.anchoredPosition = minimapPos;

                // Optionally rotate icons to match target orientation (not implemented)
            }
        }

        /// <summary>
        /// Sets the player transform the minimap follows.
        /// </summary>
        public void SetPlayer(Transform player)
        {
            playerTransform = player;
        }

        /// <summary>
        /// Registers a point of interest with a given sprite. The sprite can be any icon
        /// representing the target. The method returns the created icon for optional
        /// customization (e.g. change color) after registration.
        /// </summary>
        public RectTransform RegisterPOI(Transform target, Sprite iconSprite)
        {
            if (target == null || poiIconPrefab == null || poiContainer == null)
            {
                Debug.LogWarning("Cannot register POI: missing references");
                return null;
            }
            if (_poiIcons.ContainsKey(target)) return _poiIcons[target];

            GameObject iconObj = Instantiate(poiIconPrefab, poiContainer);
            RectTransform rect = iconObj.GetComponent<RectTransform>();
            Image img = iconObj.GetComponent<Image>();
            if (img != null && iconSprite != null)
            {
                img.sprite = iconSprite;
            }
            rect.anchoredPosition = Vector2.zero;
            _poiIcons[target] = rect;
            return rect;
        }

        /// <summary>
        /// Removes a registered POI. Call this when the target is destroyed or no longer relevant.
        /// </summary>
        public void UnregisterPOI(Transform target)
        {
            if (_poiIcons.TryGetValue(target, out RectTransform rect))
            {
                if (rect != null)
                {
                    Destroy(rect.gameObject);
                }
                _poiIcons.Remove(target);
            }
        }
    }
}