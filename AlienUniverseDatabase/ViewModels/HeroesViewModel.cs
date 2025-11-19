using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using AlienUniverseDatabase.Models;
using ReactiveUI;

namespace AlienUniverseDatabase.ViewModels;

public class HeroesViewModel : ViewModelBase
{
    public ObservableCollection<Hero> Bohaterowie { get; } = new();
    private ObservableCollection<Hero> _allBohaterowie = new(); // wszystkie postacie

    public ObservableCollection<string> Rasy { get; } = new();
    
    private string _wybranaRasa;
    public string WybranaRasa
    {
        get => _wybranaRasa;
        set
        {
            this.RaiseAndSetIfChanged(ref _wybranaRasa, value);
            ApplyFilter();
        }
    }

    public HeroesViewModel(Film film)
    {
        Rasy.Add("Wszystkie");
        WybranaRasa = "Wszystkie";
        
        LoadHeroes(film);
    }

    private void LoadHeroes(Film film)
    {
        var baseDir = AppContext.BaseDirectory;
        var path = Path.Combine(baseDir, "Data", "bohaterowieAlien.txt");

        if (!File.Exists(path))
            return;

        string text = File.ReadAllText(path);

        var sections = text.Split("------------------------------------------------------------",
                                  StringSplitOptions.RemoveEmptyEntries);

        foreach (var sec in sections)
        {
            var lines = sec.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            string get(string prefix) =>
                lines.FirstOrDefault(l => l.StartsWith(prefix))?
                     .Split(':', 2)[1].Trim() ?? "";

            Hero h = new Hero
            {
                ImieNazwisko = get("Imię i nazwisko"),
                Film = get("Film"),
                Rola = get("Rola"),
                Aktor = get("Aktor"),
                Rasa = get("Rasa / Gatunek"),
                RokUrodzenia = get("Rok urodzenia (fikcyjny)"),
                Funkcja = ExtractBlock(sec, "Funkcja w załodze", "Charakterystyka"),
                Charakterystyka = ExtractBlock(sec, "Charakterystyka", "Los"),
                Los = ExtractBlock(sec, "Los", "Ciekawostka"),
                Ciekawostka = ExtractBlock(sec, "Ciekawostka", null)
            };

            if (h.Film.Contains(film.TytulOryginalny) || h.Film.Contains(film.TytulPolski))
            {
                _allBohaterowie.Add(h);

                // dodaj rasę do listy ComboBox jeśli jeszcze nie istnieje
                if (!Rasy.Contains(h.Rasa))
                    Rasy.Add(h.Rasa);
            }
        }

        ApplyFilter();
    }

    private void ApplyFilter()
    {
        Bohaterowie.Clear();

        var filtered = string.IsNullOrWhiteSpace(WybranaRasa) || WybranaRasa == "Wszystkie"
            ? _allBohaterowie
            : _allBohaterowie.Where(h => h.Rasa == WybranaRasa);

        foreach (var h in filtered)
            Bohaterowie.Add(h);
    }

    private string ExtractBlock(string sec, string start, string? end)
    {
        if (!sec.Contains(start)) return "";

        var result = sec.Split(start + ":")[1];

        if (end != null && sec.Contains(end + ":"))
            result = result.Split(end + ":")[0];

        return result.Trim();
    }
}
