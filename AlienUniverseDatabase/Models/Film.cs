namespace AlienUniverseDatabase.Models;

public class Film
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

    // zamiana obiektu na string aby wyswietliło w listbo
    public override string ToString() => TytulPolski;
}