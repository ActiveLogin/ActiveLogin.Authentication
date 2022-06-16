using System.Collections;

using Spectre.Console;
using Spectre.Console.Rendering;

namespace Console.FlowSample;

public class QrCode
{
    public readonly Canvas Canvas;
    
    public QrCode(string asciiMatrix)
    {
        if (string.IsNullOrEmpty(asciiMatrix))
        {
            Canvas = new Canvas(0, 0);
            return;
        }

        Canvas = GetQrCanvas(asciiMatrix);
    }
    
    private static Canvas GetQrCanvas(string asciiMatrix)
    {
        var lines = asciiMatrix.Split('\n');
        var firstLine = lines.First();
        var width = firstLine.Length;
        var height = lines.Length;
        var canvas = new Canvas(width, height);

        SetCanvasPixels(canvas, lines);

        return canvas;
    }

    public Canvas SetCanvasPixels(string asciiMatrix)
    {
        var lines = asciiMatrix.Split('\n');
        return SetCanvasPixels(Canvas, lines);
    }
    
    private static Canvas SetCanvasPixels(Canvas canvas, string[] lines)
    {
        for (var y = 0; y < lines.Length; y++)
        {
            for (var x = 0; x < lines[y].Length; x++)
            {
                canvas.SetPixel(x, y, lines[y][x] == '*' ? Color.Black : Color.White);
            }
        }

        return canvas;
    }
}
