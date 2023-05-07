using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WorldDebugHelper : MonoSingleton<WorldDebugHelper>
{
    public class WorldDebugText
    {
        public Vector3 position;
        public Vector3 velocity;
        public TextMeshPro textMeshPro;
        public float duration;
        public Color color;
    }

    private static readonly float DEFAULT_DURATION = 2f;
    private static readonly float GRAVITY = -1f;

    private GameObject textParent;

    public Dictionary<Color, List<WorldDebugText>> worldDebugTexts = new Dictionary<Color, List<WorldDebugText>>();

    protected override void Instantiate()
    {
        base.Instantiate();

        if (textParent == null)
        {
            textParent = new GameObject("WorldDebugTexts");
            DontDestroyOnLoad(textParent);
        }
    }

    public static void ShowText(Vector3 position, string message, float duration = default, Color color = default)
    {
        color = color == default ? Color.white : color;
        duration = duration == default ? DEFAULT_DURATION : duration;

        WorldDebugText worldDebugText;

        if (!Instance.worldDebugTexts.TryGetValue(color, out var texts))
        {
            texts = new List<WorldDebugText>();
            Instance.worldDebugTexts.Add(color, texts);
        }

        worldDebugText = new WorldDebugText
        {
            position = position,
            velocity = Camera.main.transform.forward * 1f,
            textMeshPro = CreateTextMeshPro(position, message, color),
            duration = Time.time + duration,
            color = color
        };

        texts.Add(worldDebugText);
    }

    public static void ClearText()
    {
        foreach (var texts in Instance.worldDebugTexts.Values)
        {
            foreach (var worldDebugText in texts)
            {
                Destroy(worldDebugText.textMeshPro.gameObject);
            }
            texts.Clear();
        }
    }

    public static void ClearText(Color color)
    {
        if (Instance.worldDebugTexts.TryGetValue(color, out var texts))
        {
            foreach (var worldDebugText in texts)
            {
                Destroy(worldDebugText.textMeshPro.gameObject);
            }
            texts.Clear();
        }
    }

    private static TextMeshPro CreateTextMeshPro(Vector3 position, string message, Color color)
    {
        var textMeshPro = new GameObject("WorldDebugText").AddComponent<TextMeshPro>();
        textMeshPro.transform.SetParent(Instance.textParent.transform);
        textMeshPro.gameObject.layer = LayerMask.NameToLayer("PlayerHands");
        textMeshPro.rectTransform.position = position + new Vector3(0f, 1f, 0f);
        textMeshPro.rectTransform.sizeDelta = new Vector2(2f, 0.5f);
        textMeshPro.fontSize = 4f;
        textMeshPro.horizontalAlignment = HorizontalAlignmentOptions.Center;
        textMeshPro.verticalAlignment = VerticalAlignmentOptions.Middle; 
        textMeshPro.text = message;
        textMeshPro.color = color;
        return textMeshPro;
    }

    void LateUpdate()
    {
        foreach (var texts in Instance.worldDebugTexts.Values)
        {
            for (int i = texts.Count - 1; i >= 0; i--)
            {
                var worldDebugText = texts[i];

                if (worldDebugText.duration != -1f && Time.time >= worldDebugText.duration)
                {
                    Destroy(worldDebugText.textMeshPro.gameObject);
                    texts.RemoveAt(i);
                }
                else
                {
                    // worldDebugText.textMeshPro.rectTransform.position = worldDebugText.position + new Vector3(0f, 1f, 0f);

                    var position = worldDebugText.textMeshPro.rectTransform.position;
                    var deltaTime = Time.deltaTime;
                    var velocity = worldDebugText.velocity + Vector3.down * GRAVITY;
                    position += velocity * deltaTime;
                    worldDebugText.textMeshPro.rectTransform.position = position;
                }
            }
        }
    }
}
