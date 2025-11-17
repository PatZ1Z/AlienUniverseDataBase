namespace AlienUniverseDatabase.ViewModels;
using System;
using System.IO;
using ReactiveUI;

public class MainWindowViewModel : ViewModelBase
{
    private string _filmyContent;

    public string FilmyContent
    {
        get => _filmyContent;
        set => this.RaiseAndSetIfChanged(ref _filmyContent, value);
    }

    private string FilmyPath = "/home/pat/RiderProjects/AlienUniverseDatabase/AlienUniverseDatabase/Assets/Data/filmyAlien.txt";

    public MainWindowViewModel()
    {
        FilmyContent = ReadFile(FilmyPath);
    }

    private string ReadFile(string path)
    {
        try
        {
            return File.ReadAllText(path);
        }
        catch (Exception ex)
        {
            return $"Błąd: {ex.Message}";
        }
    }
}