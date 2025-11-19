using ReactiveUI;

namespace AlienUniverseDatabase.Models;

public class Film : ReactiveObject
{
    private string _ocena;
    public string Ocena
    {
        get => _ocena;
        set
        {
            this.RaiseAndSetIfChanged(ref _ocena, value);
            this.RaisePropertyChanged(nameof(OcenaValue)); // Powiadom ProgressBar
        }
    }

    public double OcenaValue
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Ocena)) return 0;

            // Obsłuż przecinek i kropkę
            var parts = Ocena.Split('/');
            if (double.TryParse(parts[0].Trim().Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var val))
                return val;
            return 0;
        }
    }

    private string _tytulPolski;
    public string TytulPolski
    {
        get => _tytulPolski;
        set => this.RaiseAndSetIfChanged(ref _tytulPolski, value);
    }

    private string _tytulOryginalny;
    public string TytulOryginalny
    {
        get => _tytulOryginalny;
        set => this.RaiseAndSetIfChanged(ref _tytulOryginalny, value);
    }

    private string _rokPremiery;
    public string RokPremiery
    {
        get => _rokPremiery;
        set => this.RaiseAndSetIfChanged(ref _rokPremiery, value);
    }

    private string _rezyser;
    public string Rezyser
    {
        get => _rezyser;
        set => this.RaiseAndSetIfChanged(ref _rezyser, value);
    }

    private string _scenariusz;
    public string Scenariusz
    {
        get => _scenariusz;
        set => this.RaiseAndSetIfChanged(ref _scenariusz, value);
    }

    private string _gatunek;
    public string Gatunek
    {
        get => _gatunek;
        set => this.RaiseAndSetIfChanged(ref _gatunek, value);
    }

    private string _czasTrwania;
    public string CzasTrwania
    {
        get => _czasTrwania;
        set => this.RaiseAndSetIfChanged(ref _czasTrwania, value);
    }

    private string _glownePostacie;
    public string GlownePostacie
    {
        get => _glownePostacie;
        set => this.RaiseAndSetIfChanged(ref _glownePostacie, value);
    }

    private string _statek;
    public string Statek
    {
        get => _statek;
        set => this.RaiseAndSetIfChanged(ref _statek, value);
    }

    private string _opis;
    public string Opis
    {
        get => _opis;
        set => this.RaiseAndSetIfChanged(ref _opis, value);
    }

    private string _ciekawostka;
    public string Ciekawostka
    {
        get => _ciekawostka;
        set => this.RaiseAndSetIfChanged(ref _ciekawostka, value);
    }

    public override string ToString() => TytulPolski;
}
