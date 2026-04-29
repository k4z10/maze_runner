namespace maze_runner.Core.Frontend.Raylib;
using Raylib_cs;

public class QuitConfirmationOverlay(IApplicationState appState) : IOverlay
{
    public bool IsFinished { get; private set; }

    public bool ProcessInput(char key)
    {
        if (key == 'y')
        {
            appState.RequestQuit(); // Uruchamia flagę zamykającą główną pętlę
            IsFinished = true;
        }
        else if (key == 'n' || key == (char)27) // ESC
        {
            IsFinished = true; // Ściąga overlay, wracając do gry
        }
        return true; // Pochłania input, żeby gracz nie chodził z otwartym promptem
    }

    public void Render()
    {
        if (IsFinished) return;
        
        int screenWidth = Raylib.GetScreenWidth();
        int screenHeight = Raylib.GetScreenHeight();

        // Filtr zaciemniający na całą powierzchnię ekranu
        Raylib.DrawRectangle(0, 0, screenWidth, screenHeight, Raylib.Fade(Color.Black, 0.6f));

        // Parametry geometryczne okna nakładki
        int rectWidth = 400;
        int rectHeight = 150;

        // Wektory przesunięcia początku układu współrzędnych okna
        int rectX = (screenWidth / 2) - (rectWidth / 2);
        int rectY = (screenHeight / 2) - (rectHeight / 2);

        Raylib.DrawRectangle(rectX, rectY, rectWidth, rectHeight, Color.DarkBlue);
        Raylib.DrawRectangleLines(rectX, rectY, rectWidth, rectHeight, Color.White);

        // Wymiarowanie przestrzeni znakowej do poprawnego centrowania
        string promptText = "Czy na pewno chcesz wyjsc?";
        int promptFontSize = 20;
        int promptTextWidth = Raylib.MeasureText(promptText, promptFontSize);
        
        string optionsText = "[Y] Tak    [N] Nie";
        int optionsFontSize = 20;
        int optionsTextWidth = Raylib.MeasureText(optionsText, optionsFontSize);

        // Translacja punktu kotwiczenia tekstów
        int promptX = (screenWidth / 2) - (promptTextWidth / 2);
        int promptY = rectY + 40; // Przesunięcie wektora Y relatywnie do górnej krawędzi okna

        int optionsX = (screenWidth / 2) - (optionsTextWidth / 2);
        int optionsY = rectY + 90;

        Raylib.DrawText(promptText, promptX, promptY, promptFontSize, Color.RayWhite);
        Raylib.DrawText(optionsText, optionsX, optionsY, optionsFontSize, Color.Gray);
    }
}