using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using AlienUniverseDatabase.Models;
using AlienUniverseDatabase.Views;
using ReactiveUI;

namespace AlienUniverseDatabase.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<Film> Filmy { get; } = new();

    private Film _wybranyFilm;
    public Film WybranyFilm
    {
        get => _wybranyFilm;
        set
        {
            this.RaiseAndSetIfChanged(ref _wybranyFilm, value);
            ShowSections = (value == null) ? false : true;
        } 
    }

    private bool _showSections;
    public bool ShowSections
    {
        get => _showSections;
        set => this.RaiseAndSetIfChanged(ref _showSections, value);
    }
    
    public ReactiveCommand<Unit, Unit> ShowHeroesCommand { get; }

    public MainWindowViewModel()
    {
        var baseDir = AppContext.BaseDirectory;
        var path = Path.Combine(baseDir, "Data", "filmyAlien.txt");
        LoadFile(path);
        
        ShowHeroesCommand = ReactiveCommand.Create(ShowHeroes);
    }
    
    private void ShowHeroes()
    {
        if (WybranyFilm == null)
            return;

        var window = new HeroesWindow
        {
            DataContext = new HeroesViewModel(WybranyFilm)
        };

        window.Show();
    }

    private void LoadFile(string path)
    {
        if (!File.Exists(path))
        {
            return;
        }

        var text = File.ReadAllText(path);

        // Sekcje oddzielone przez "-----"
        var sections = text.Split("------------------------------------------------------------",
                                  StringSplitOptions.RemoveEmptyEntries);

        foreach (var sec in sections)
        {
            var lines = sec.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length == 0)
                continue;

            Film f = new Film();

            
            //metoda do przypiywania danych dla danego prefixu
            string getValue(string prefix) => 
                lines.FirstOrDefault(l => l.StartsWith(prefix))?
                     .Split(':', 2)[1].Trim() ?? "";

            f.TytulOryginalny = getValue("Tytuł oryginalny");
            f.TytulPolski = getValue("Tytuł polski");
            f.RokPremiery = getValue("Rok premiery");
            f.Rezyser = getValue("Reżyser");
            f.Scenariusz = getValue("Scenariusz");
            f.Gatunek = getValue("Gatunek");
            f.CzasTrwania = getValue("Czas trwania");
            f.Ocena = getValue("Ocena");
            f.GlownePostacie = getValue("Główne postacie");
            f.Statek = getValue("Statek");

            // Opis fabuły – całość między „Opis fabuły:” a „Ciekawostka:”
            if (sec.Contains("Opis fabuły:") && sec.Contains("Ciekawostka:"))
            {
                f.Opis = sec.Split("Opis fabuły:")[1]
                            .Split("Ciekawostka:")[0]
                            .Trim();
            }

            // Ciekawostka – całość po "Ciekawostka:"
            if (sec.Contains("Ciekawostka:"))
            {
                f.Ciekawostka = sec.Split("Ciekawostka:")[1]
                                   .Trim();
            }

            Filmy.Add(f);
        }
    }
}
