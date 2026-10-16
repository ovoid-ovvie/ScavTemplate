using BepInEx.Configuration;
using UnityEngine;

namespace SubEx
{
    public class EpilepsyWarning : MonoBehaviour
    {
        private bool showWarning = true;
        private GUIStyle boxStyle;
        private GUIStyle promptStyle;
        private float originalTimeScale;
        private ConfigEntry<bool> configWarningDismissed;
        private Texture2D blackTexture;

        public void Initialize(ConfigFile config)
        {
            configWarningDismissed = config.Bind("General", "WarningDismissed", false, "Has the player dismissed the epilepsy warning?");

            if (configWarningDismissed.Value == true)
            {
                showWarning = false;
                Destroy(this);
            }
            else
            {
                originalTimeScale = Time.timeScale;
                Time.timeScale = 0f;

                blackTexture = new Texture2D(1, 1);
                blackTexture.SetPixel(0, 0, Color.black);
                blackTexture.Apply();
            }
        }

        private void Update()
        {
            if (!showWarning) return;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (Input.anyKeyDown && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2))
            {
                DismissWarning();
            }
        }

        private void OnGUI()
        {
            if (!showWarning) return;

            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), blackTexture);

            if (boxStyle == null)
            {
                boxStyle = new GUIStyle(GUI.skin.box);
                boxStyle.fontSize = 16;
                boxStyle.alignment = TextAnchor.MiddleCenter;
                boxStyle.normal.textColor = Color.white;
                boxStyle.wordWrap = true;
                boxStyle.padding = new RectOffset(25, 25, 25, 25);

                promptStyle = new GUIStyle();
                promptStyle.fontSize = 14;
                promptStyle.alignment = TextAnchor.MiddleCenter;
                promptStyle.normal.textColor = Color.gray;
            }

            float width = 580;
            float height = 300;
            float x = (Screen.width - width) / 2;
            float y = (Screen.height - height) / 2;

            string warningText = "<color=red><b>PHOTOSENSITIVITY / EPILEPSY WARNING</b></color>\n\n" +
                                 "Substances Expanded contains a specific item that generates intense flashing lights when used.\n\n" +
                                 "If you have a history of epilepsy, seizures, or severe photosensitivity, please avoid using the item <b>dimethyltryptamine</b>.\n\n" +
                                 "If you experience dizziness or discomfort, stop playing immediately.";

            GUI.Box(new Rect(x, y, width, height), warningText, boxStyle);
            GUI.Label(new Rect(x, y + height - 50, width, 30), "[ Press any key to continue ]", promptStyle);
        }

        private void DismissWarning()
        {
            showWarning = false;
            Time.timeScale = originalTimeScale;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            configWarningDismissed.Value = true;
            configWarningDismissed.ConfigFile.Save();

            Destroy(blackTexture);
            Destroy(this);
        }
    }
}
