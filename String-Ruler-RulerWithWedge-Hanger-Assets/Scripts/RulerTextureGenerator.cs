using UnityEngine;

[ExecuteInEditMode]
public class RulerTextureGenerator : MonoBehaviour
{
    [Header("Texture Settings")]
    public int width = 2048;
    public int height = 128;

    [Header("Colors")]
    public Color rulerColor = new Color(0.95f, 0.87f, 0.65f);
    public Color lineColor = Color.black;

    void Start()
    {
        ApplyTexture();
    }

    public void ApplyTexture()
    {
        Texture2D tex = new Texture2D(width, height);

        // Fill background
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                tex.SetPixel(x, y, rulerColor);

        // Border lines
        for (int x = 0; x < width; x++)
        {
            tex.SetPixel(x, 0, lineColor);
            tex.SetPixel(x, height - 1, lineColor);
        }

        // Draw markings
        for (int cm = 0; cm <= 100; cm++)
        {
            int x = Mathf.RoundToInt((cm / 100f) * (width - 1));

            // Every 10cm — tall thick line + number
            if (cm % 10 == 0)
            {
                int tallHeight = (int)(height * 0.75f);
                for (int y = 0; y < tallHeight; y++)
                {
                    if (x < width) tex.SetPixel(x, height - 1 - y, lineColor);
                    if (x + 1 < width) tex.SetPixel(x + 1, height - 1 - y, lineColor);
                }
                // Draw number
                DrawNumber(tex, cm, x + 3, 8, lineColor);
            }
            // Every 1cm — medium line
            else
            {
                int cmHeight = height / 2;
                for (int y = 0; y < cmHeight; y++)
                    if (x < width) tex.SetPixel(x, height - 1 - y, lineColor);
            }

            // mm lines
            if (cm < 100)
            {
                for (int mm = 1; mm < 10; mm++)
                {
                    int mx = Mathf.RoundToInt(((cm + mm / 10f) / 100f) * (width - 1));
                    int mmHeight = mm == 5 ? height / 3 : height / 4;
                    for (int y = 0; y < mmHeight; y++)
                        if (mx < width) tex.SetPixel(mx, height - 1 - y, lineColor);
                }
            }
        }

        tex.Apply();

        Material mat = GetComponent<Renderer>().sharedMaterial;
        mat.mainTexture = tex;
        mat.mainTextureScale = new Vector2(1f, 1f);
        mat.mainTextureOffset = new Vector2(0f, 0f);
    }

    // Draws a number using a pixel font
    void DrawNumber(Texture2D tex, int number, int startX, int startY, Color col)
    {
        string numStr = number.ToString();
        int charWidth = 5;
        int charHeight = 7;
        int spacing = 1;

        for (int i = 0; i < numStr.Length; i++)
        {
            int digit = numStr[i] - '0';
            bool[,] pixels = GetDigitPixels(digit);
            int offsetX = startX + i * (charWidth + spacing);

            for (int py = 0; py < charHeight; py++)
                for (int px = 0; px < charWidth; px++)
                    if (pixels[py, px])
                    {
                        int tx = offsetX + px;
                        int ty = startY + py;
                        if (tx < tex.width && ty < tex.height)
                            tex.SetPixel(tx, ty, col);
                    }
        }
    }

    // Pixel font — each digit is 5x7 pixels
    bool[,] GetDigitPixels(int digit)
    {
        bool T = true, F = false;
        bool[][,] digits = new bool[][,]
        {
            // 0
            new bool[,]{
                {F,T,T,T,F},
                {T,F,F,F,T},
                {T,F,F,T,T},
                {T,F,T,F,T},
                {T,T,F,F,T},
                {T,F,F,F,T},
                {F,T,T,T,F}},
            // 1
            new bool[,]{
                {F,F,T,F,F},
                {F,T,T,F,F},
                {F,F,T,F,F},
                {F,F,T,F,F},
                {F,F,T,F,F},
                {F,F,T,F,F},
                {F,T,T,T,F}},
            // 2
            new bool[,]{
                {F,T,T,T,F},
                {T,F,F,F,T},
                {F,F,F,F,T},
                {F,F,T,T,F},
                {F,T,F,F,F},
                {T,F,F,F,F},
                {T,T,T,T,T}},
            // 3
            new bool[,]{
                {T,T,T,T,F},
                {F,F,F,F,T},
                {F,F,F,F,T},
                {F,T,T,T,F},
                {F,F,F,F,T},
                {F,F,F,F,T},
                {T,T,T,T,F}},
            // 4
            new bool[,]{
                {F,F,F,T,F},
                {F,F,T,T,F},
                {F,T,F,T,F},
                {T,F,F,T,F},
                {T,T,T,T,T},
                {F,F,F,T,F},
                {F,F,F,T,F}},
            // 5
            new bool[,]{
                {T,T,T,T,T},
                {T,F,F,F,F},
                {T,F,F,F,F},
                {T,T,T,T,F},
                {F,F,F,F,T},
                {F,F,F,F,T},
                {T,T,T,T,F}},
            // 6
            new bool[,]{
                {F,T,T,T,F},
                {T,F,F,F,F},
                {T,F,F,F,F},
                {T,T,T,T,F},
                {T,F,F,F,T},
                {T,F,F,F,T},
                {F,T,T,T,F}},
            // 7
            new bool[,]{
                {T,T,T,T,T},
                {F,F,F,F,T},
                {F,F,F,T,F},
                {F,F,T,F,F},
                {F,T,F,F,F},
                {F,T,F,F,F},
                {F,T,F,F,F}},
            // 8
            new bool[,]{
                {F,T,T,T,F},
                {T,F,F,F,T},
                {T,F,F,F,T},
                {F,T,T,T,F},
                {T,F,F,F,T},
                {T,F,F,F,T},
                {F,T,T,T,F}},
            // 9
            new bool[,]{
                {F,T,T,T,F},
                {T,F,F,F,T},
                {T,F,F,F,T},
                {F,T,T,T,T},
                {F,F,F,F,T},
                {F,F,F,F,T},
                {F,T,T,T,F}}
        };

        return digits[digit];
    }
}