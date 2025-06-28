using System;
using System.Collections.Generic;

namespace ZadanieB
{
    // Klasy;
    abstract class Stworzenie
    {
        public string Nazwa { get; }
        public bool Zywy { get; private set; } = true;

        // Życie
        public (uint Aktualne, uint Maksymalne) Zycie { get; private set; }

        protected Stworzenie(string nazwa, uint zycieMaks)
        {
            Nazwa = nazwa;
            Zycie = (zycieMaks, zycieMaks);
        }

        public abstract uint ZadawaneObrazenia { get; }

        public void ZadajObrazenia(uint wielkosc)
        {
            if (!Zywy) return;                    // martwy generalnie nie powinien atakować i przyjmować obrażeń

            // unikamy niedomiaru;

            if (Zycie.Aktualne == 0)
                Zywy = false;
        }

        public void Lecz(uint wielkosc)
        {
            if (!Zywy) return;                    // nie wskrzeszamy
            uint nowe = Math.Min(Zycie.Aktualne + wielkosc, Zycie.Maksymalne);
            Zycie = (nowe, Zycie.Maksymalne);
        }

        public override string ToString() =>
            $"{Nazwa} (HP {Zycie.Aktualne}/{Zycie.Maksymalne})";
    }

    class Potwor : Stworzenie
    {
        public uint Sila { get; }

        public Potwor(string nazwa, uint zycie, uint sila)
            : base(nazwa, zycie) => Sila = sila;

        public override uint ZadawaneObrazenia => Sila;
    }

    class Bohater : Stworzenie
    {
        public uint Sila { get; }
        public uint Zrecznosc { get; }
        public uint Inteligencja { get; }

        public Bohater(string nazwa, uint zycie, uint sila, uint zrecz, uint intell)
            : base(nazwa, zycie)
        {
            Sila = sila;
            Zrecznosc = zrecz;
            Inteligencja = intell;
        }

        public override uint ZadawaneObrazenia =>
            Math.Max(Sila, Math.Max(Zrecznosc, Inteligencja));
    }

    // Elementy dodatkowe
    static class Arena
    {
        // prosty „naprzemienny” pojedynek aż ktoś zginie
        public static Stworzenie Pojedynek(Stworzenie a, Stworzenie b)
        {
            bool turaA = true;
            while (a.Zywy & b.Zywy)
            {
                if (turaA) b.ZadajObrazenia(a.ZadawaneObrazenia);
                else        a.ZadajObrazenia(b.ZadawaneObrazenia);
                turaA = !turaA;
            }
            return a.Zywy ? a : b;
        }

        // Turniej
        public static Stworzenie Turniej(params Stworzenie[] uczestnicy)
        {
            List<Stworzenie> runda = new List<Stworzenie>(uczestnicy);

            uint LEczenieMiedzyRundami = 5;

            while (runda.Count > 1)
            {
                List<Stworzenie> zwyciezcy = new List<Stworzenie>();

                for (int i = 0; i < runda.Count; i += 2)
                {
                    if (i == runda.Count - 1) // „dzika karta”
                    {
                        zwyciezcy.Add(runda[i]);
                        continue;
                    }

                    Stworzenie wygrany = Pojedynek(runda[i], runda[i + 1]);
                    zwyciezcy.Add(wygrany);
                }

                // Drobne leczenie HP po rundzie
                foreach (var s in zwyciezcy)
                    s.Lecz(LEczenieMiedzyRundami);

                runda = zwyciezcy;
            }

            return runda[0];
        }
    }

    // Test;
    class Program
    {
        static void Main()
        {
            var gracze = new Stworzenie[]
            {
                new Bohater("Aria",    30, 5, 8, 3),
                new Potwor ("Goblin",  15, 4),
                new Bohater("Borin",   28, 7, 6, 6),
                new Potwor ("Ork",     24, 6),
                new Potwor ("Pająk",   10, 3)
            };

            Console.WriteLine("START TURNIEJU:\n");
            foreach (var g in gracze) Console.WriteLine(g);

            var mistrz = Arena.Turniej(gracze);

            Console.WriteLine("\n=== ZWYCIĘZCA TURNIEJU ===");
            Console.WriteLine(mistrz);
        }
    }
}
