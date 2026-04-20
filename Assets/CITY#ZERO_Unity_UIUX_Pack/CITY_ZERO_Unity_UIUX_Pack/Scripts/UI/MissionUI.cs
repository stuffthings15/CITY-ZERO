using TMPro;
using UnityEngine;

namespace CityZero.UI
{
    /// <summary>
    /// Handles the mission UI, including the mission title, objective text,
    /// and optional progress display. Attach this to your mission UI panel.
    /// Call the provided methods whenever mission data changes.
    /// </summary>
    public class MissionUI : MonoBehaviour
    {
        [Tooltip("Text component for the mission title")]
        public TMP_Text titleText;

        [Tooltip("Text component for the current objective")]
        public TMP_Text objectiveText;

        [Tooltip("Text component for progress (e.g. 2/5)")]
        public TMP_Text progressText;

        /// <summary>
        /// Sets the mission title. Pass null or empty string to hide the title.
        /// </summary>
        public void SetTitle(string title)
        {
            if (titleText == null) return;
            if (string.IsNullOrEmpty(title))
            {
                titleText.gameObject.SetActive(false);
            }
            else
            {
                titleText.gameObject.SetActive(true);
                titleText.text = title;
            }
        }

        /// <summary>
        /// Sets the objective text. Pass null or empty string to clear the objective.
        /// </summary>
        public void SetObjective(string objective)
        {
            if (objectiveText == null) return;
            objectiveText.text = string.IsNullOrEmpty(objective) ? string.Empty : objective;
        }

        /// <summary>
        /// Sets the progress display. Use negative values or zero count to hide the progress field.
        /// </summary>
        public void SetProgress(int current, int total)
        {
            if (progressText == null) return;
            if (total <= 0)
            {
                progressText.gameObject.SetActive(false);
                return;
            }
            progressText.gameObject.SetActive(true);
            progressText.text = string.Format(UIConstants.ProgressFormat, current, total);
        }

        /// <summary>
        /// Clears all mission UI fields.
        /// </summary>
        public void Clear()
        {
            SetTitle(null);
            SetObjective(null);
            SetProgress(0, 0);
        }
    }
}