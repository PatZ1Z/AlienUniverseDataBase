using ReactiveUI;
using System.Reactive;
using AlienUniverseDatabase.Models;

namespace AlienUniverseDatabase.ViewModels
{
    public class NewFilmFormViewModel : ReactiveObject
    {
        
        public string TytulOryginalny { get; set; }
        public string TytulPolski { get; set; }
        public string RokPremiery { get; set; }
        public string Rezyser { get; set; }
        public string Scenariusz { get; set; }
        public string Gatunek { get; set; }
        public string CzasTrwania { get; set; }
        public string Ocena { get; set; }
        public string GlownePostacie { get; set; }
        public string Statek { get; set; }
        public string Opis { get; set; }
        public string Ciekawostka { get; set; }

        
        public ReactiveCommand<Unit, Film> ZapiszFilmCommand { get; }

        public NewFilmFormViewModel()
        {
            
            ZapiszFilmCommand = ReactiveCommand.Create(() =>
            {
                var nowyFilm = new Film
                {
                    TytulOryginalny = TytulOryginalny,
                    TytulPolski = TytulPolski,
                    RokPremiery = RokPremiery,
                    Rezyser = Rezyser,
                    Scenariusz = Scenariusz,
                    Gatunek = Gatunek,
                    CzasTrwania = CzasTrwania,
                    Ocena = Ocena,
                    GlownePostacie = GlownePostacie,
                    Statek = Statek,
                    Opis = Opis,
                    Ciekawostka = Ciekawostka
                };
                return nowyFilm;
            });
        }
    }
}