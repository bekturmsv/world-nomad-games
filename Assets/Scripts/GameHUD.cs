using UnityEngine;

public class GameHUD : MonoBehaviour
{
    public ScanSystem scanSystem;
    public EagleHunter eagleHunter;
    public MeatCollector meatCollector;

    private void OnGUI()
    {
        if (scanSystem != null && scanSystem.IsScanning)
        {
            Color oldColor = GUI.color;
            GUI.color = new Color(0.2f, 1f, 0.3f, 0.12f);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = oldColor;
        }

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 18;
        style.normal.textColor = Color.white;

        GUI.Box(new Rect(12, 12, 330, 95), "");

        if (meatCollector != null) GUI.Label(new Rect(24, 20, 300, 25), $"Meats: {meatCollector.MeatCount}", style);

        string hint = "Hold E — to search animals";

        if (scanSystem != null && scanSystem.IsScanning)
        {
            if (scanSystem.CurrentTarget != null) hint = $"Target: {scanSystem.CurrentTarget.DisplayName} — press R";
            else hint = "Look at the animal and press R";
        }

        if (eagleHunter != null && eagleHunter.IsFlying) hint = "Eagle hunting...";
        if (meatCollector != null && meatCollector.NearbyCarcass != null) hint = "Press F — to collect meat";

        GUI.Label(new Rect(24, 52, 310, 30), hint, style);
        GUI.Label(new Rect(Screen.width / 2 - 10, Screen.height / 2 - 16, 30, 30), "+", style);
    }
}