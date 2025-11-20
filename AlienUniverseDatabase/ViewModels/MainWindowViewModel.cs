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
    public ReactiveCommand<Film, Unit> UsunFilmCommand { get; }
    
    public ReactiveCommand<Unit, Unit> ShowNewFilmFormCommand { get; }
    

    public MainWindowViewModel()
    {
        var baseDir = AppContext.BaseDirectory;
        var path = Path.Combine(baseDir, "Data", "filmyAlien.txt");
        LoadFile(path);

        ShowHeroesCommand = ReactiveCommand.Create(ShowHeroes);
    
        ShowNewFilmFormCommand = ReactiveCommand.Create(ShowNewFilmForm);

        
        UsunFilmCommand = ReactiveCommand.Create<Film>(film =>
        {
            if (film != null)
            {
                UsunFilm(film);
                WybranyFilm = null;
            }
        });
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
    
    private void ShowNewFilmForm()
    {
        var newFilmFormWindow = new NewFilmForm
        {
            DataContext = new NewFilmFormViewModel()
        };

        // Subskrybuj wynik komendy ZapiszFilmCommand
        var viewModel = (NewFilmFormViewModel)newFilmFormWindow.DataContext;
        viewModel.ZapiszFilmCommand.Subscribe(film =>
        {
            // Dodaj film do ObservableCollection i zapisz go do pliku
            DodajFilm(film);
            newFilmFormWindow.Close();
        });

        newFilmFormWindow.Show();
    }

    private void DodajFilm(Film nowyFilm)
    {
        Filmy.Add(nowyFilm);
        ZapiszFilmDoPliku(nowyFilm);
    }
    
    private void UsunFilm(Film film)
    {
        // Usuwamy film z ObservableCollection
        Filmy.Remove(film);

        // Usuwamy film z pliku
        UsunFilmZPliku(film);
    }
    
    private void UsunFilmZPliku(Film film)
    {
        var baseDir = AppContext.BaseDirectory;
        var path = Path.Combine(baseDir, "Data", "filmyAlien.txt");

        // Wczytujemy wszystkie sekcje filmów
        var wszystkieSekcje = File.ReadAllLines(path);

        // Znajdujemy sekcję filmu, który chcemy usunąć
        var sekcjaDoUsuniecia = FormatFilm(film).TrimEnd();  // Usuwamy zbędne białe znaki na końcu
        Console.WriteLine("Usuwanie sekcji: " + sekcjaDoUsuniecia); // Logowanie

        // Usuwamy sekcję filmu z listy (z pominięciem białych znaków)
        var updatedSekcje = wszystkieSekcje
            .Where(l => l.Trim() != sekcjaDoUsuniecia.Trim())  // Używamy .Trim() aby usunąć nadmiarowe białe znaki
            .ToList();

        // Sprawdzamy, czy sekcja została usunięta
        if (updatedSekcje.Count == wszystkieSekcje.Length)
        {
            Console.WriteLine("Nie znaleziono sekcji do usunięcia!");
        }
        else
        {
            Console.WriteLine("Sekcja została usunięta, zapisuję plik.");
            File.WriteAllLines(path, updatedSekcje);
        }
    }



    private void ZapiszFilmDoPliku(Film film)
    {
        var baseDir = AppContext.BaseDirectory;
        var path = Path.Combine(baseDir, "Data", "filmyAlien.txt");

        // Formatujemy nowy film w odpowiednim formacie
        string filmSekcja = $"{FormatFilm(film)}\n------------------------------------------------------------\n";

        Console.WriteLine("Zapisuję film: " + film.TytulOryginalny); // Logowanie

        // Dodajemy do pliku
        File.AppendAllText(path, filmSekcja);
    }


    private string FormatFilm(Film film)
    {
        return $@"
Tytuł oryginalny:              {film.TytulOryginalny}
Tytuł polski:                  {film.TytulPolski}
Rok premiery:                  {film.RokPremiery}
Reżyser:                       {film.Rezyser}
Scenariusz:                    {film.Scenariusz}
Gatunek:                       {film.Gatunek}
Czas trwania:                  {film.CzasTrwania}
Ocena (IMDb):                  {film.Ocena}
Główne postacie:               {film.GlownePostacie}
Statek:                        {film.Statek}

Opis fabuły:
{film.Opis}

Ciekawostka:
{film.Ciekawostka}
------------------------------------------------------------";  // Usuwamy dodatkowe spacje lub nowe linie
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


    




