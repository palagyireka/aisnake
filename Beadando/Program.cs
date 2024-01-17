using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;


namespace AISnake
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Kígyó AIKigyo = new Kígyó();
            Csúcs anyaCsúcs = new Csúcs(AIKigyo);
            Gráf megoldások = new Gráf(anyaCsúcs);
            megoldások.Keresés();
        }
    }


    class Kígyó : IEquatable<Kígyó>
    {
        public bool Equals(Kígyó? other)
        {
            if (other == null)
            {
                return false;
            }
            return
                this.fejPoz.Equals(other.fejPoz) &&
                this.farokPoz.Equals(other.farokPoz) &&
                ArraysEqual(this.Tabla, other.Tabla) &&
                ArraysEqual(this.testPoz, other.testPoz);
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + this.fejPoz.GetHashCode();
            hash = hash * 31 + this.farokPoz.GetHashCode();
            hash = hash * 31 + GetArrayHashCode(this.Tabla);
            hash = hash * 31 + GetArrayHashCode(this.testPoz);
            return hash;
        }

        private int GetArrayHashCode<T>(T[,] array)
        {
            int hash = 17;
            foreach (var item in array)
            {
                hash = hash * 31 + (item != null ? item.GetHashCode() : 0);
            }
            return hash;
        }

        private bool ArraysEqual<T>(T[,] array1, T[,] array2)
        {
            if (array1 == null && array2 == null)
                return true;

            if (array1 == null || array2 == null)
                return false;

            if (array1.GetLength(0) != array2.GetLength(0) || array1.GetLength(1) != array2.GetLength(1))
                return false;

            for (int i = 0; i < array1.GetLength(0); i++)
            {
                for (int j = 0; j < array1.GetLength(1); j++)
                {
                    if (!EqualityComparer<T>.Default.Equals(array1[i, j], array2[i, j]))
                        return false;
                }
            }

            return true;
        }

        public Kígyó DeepClone()
        {
            Kígyó clonedKigyo = new Kígyó();

            clonedKigyo.fejPoz = this.fejPoz;
            clonedKigyo.farokPoz = this.farokPoz;

            clonedKigyo.Tabla = (int[,])this.Tabla.Clone();

            clonedKigyo.testPoz = new (int, int)[this.testPoz.GetLength(0), this.testPoz.GetLength(1)];
            for (int i = 0; i < this.testPoz.GetLength(0); i++)
            {
                for (int j = 0; j < this.testPoz.GetLength(1); j++)
                {
                    clonedKigyo.testPoz[i, j] = this.testPoz[i, j];
                }
            }

            return clonedKigyo;
        }


        (int,int) fejPoz = (0,2), farokPoz = (0,0);

        int[,] Tabla = { { 0,1,1,1 },
                         { 1,1,1,1 },
                         { 1,0,1,0} };
        // Sötét (1) és világos(0) részek

        (int,int)[,] testPoz = {
                           { (1,0),(-1,-1),(0,0),(-1,-1) },
                           { (1,1),(2,1),(0,2),(1,2) },
                           { (-1,-1),(2,2),(2,3),(1,3) }
                           };
        // (-1,-1) ahol üres, 0. sortól indul


        public Kígyó(){ }

        public bool SzuperOperátor(Kígyó bemenet,int i, out Kígyó kimenet)
        {
            if (bemenet != null)
            {
                switch (i)
                {
                    case 0: return Mozgás(bemenet,"fej", "b", out kimenet);
                    case 1: return Mozgás(bemenet,"fej", "j", out kimenet);
                    case 2: return Mozgás(bemenet, "fej", "l", out kimenet);
                    case 3: return Mozgás(bemenet, "fej", "f", out kimenet);
                    case 4: return Mozgás(bemenet, "farok", "b", out kimenet);
                    case 5: return Mozgás(bemenet, "farok", "j", out kimenet);
                    case 6: return Mozgás(bemenet, "farok", "l", out kimenet);
                    case 7: return Mozgás(bemenet, "farok", "f", out kimenet);
                    default: kimenet = null; return false;
                }
            }
            kimenet = null;
            return false;
        }

        public void ToConsoleString()
        {
            if (this != null)
            {
                string testRajz = "";
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (testPoz[i, j] != (-1, -1))
                        {
                            if ((i, j) == fejPoz)
                            {
                                testRajz += $"  [=]  ";
                            }
                            else if ((i, j) == farokPoz)
                            {
                                testRajz += $"  |V|  ";
                            }
                            else
                            {
                                testRajz += $"  [ ]  ";
                            }
                        }
                        else
                        {
                            testRajz += "       ";
                        }
                    }
                    testRajz += "\n";
                }
                Console.WriteLine($"A fej {fejPoz}, a farok {farokPoz}.");
                Console.WriteLine(testRajz);
            }
        }
        
        public bool CélÁllapotE()
        {
            int hvil = 0;
            for(int i = 0;i< 3;i++)
            {
                for(int j = 0;j < 4; j++)
                {
                    if (Tabla[i,j] == 0 && testPoz[i,j] == (-1,-1))
                    {
                        hvil++;
                        if (hvil > 2)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
            // Megpróbálja megkeresni a három üres világos táblát.
        }
        
        public bool ÁllapotE()
        {
            for(int i = 0; i < 3; i++)
            {
                for(int j = 0;j< 4; j++)
                {
                    for(int k = 0;k < 3; k++)
                    {
                        for(int l = 0;l < 4; l++)
                        {
                            if(!(i==k && j==l) && testPoz[i,j] == testPoz[k,l] && testPoz[i,j] != (-1,-1) && testPoz[k,l] != (-1, -1))
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        private bool Mozgás(Kígyó be,string fejVagyFarok, string irany, out Kígyó ujAllapot)
        {
            Kígyó ujA = be.DeepClone(); // Új kígyó ami a következő állapotot testesíti meg, ha érvényes lesz a kijövetel.
            int hovaS = -1; // Hová kerül a mozgó testrész koordinátái, először érvénytelenül indul
            int hovaO = -1;
            void irányÁllít(string iranyb,(int,int) poz) // Visszaadja a fej vagy a farok mellett adott irányban lévő elem koordinátáit
            {
                switch (iranyb)
                {
                    case "b":
                        hovaS = poz.Item1;
                        hovaO = poz.Item2 - 1;
                        break;
                    case "j":
                        hovaS = poz.Item1;
                        hovaO = poz.Item2 + 1;
                        break;
                    case "l":
                        hovaS = poz.Item1 + 1;
                        hovaO = poz.Item2;
                        break;
                    case "f":
                        hovaS = poz.Item1 - 1;
                        hovaO = poz.Item2;
                        break;
                }
            }
            
                switch (fejVagyFarok)
                {
                    case "fej":
                        irányÁllít(irany, be.fejPoz); // Megvan a konkrét irány
                    if (hovaO >= 0 && hovaO < 4 && hovaS >= 0 && hovaS < 3 && be.testPoz[hovaS,hovaO] == (-1,-1)) // Az egész csakkor indul, ha a koordináták érvényesek
                        {                                                                                         // és szabad a hely 
                        for (int i = 0; i < 3; i++)
                            {
                                for (int j = 0; j < 4; j++)
                                {
                                    if ((i, j) == fejPoz) // Megérkeztünk a fejhez
                                    {
                                        (int, int) ujFarokKoor = ujA.testPoz[be.farokPoz.Item1, be.farokPoz.Item2]; // Az új farok koordinátái a régi farokban voltak tárolva.
                                        
                                        ujA.testPoz[i, j] = (hovaS, hovaO); // Az új fej koordinátái a régi fejben lesznek tárolva.
                                        ujA.fejPoz = (hovaS, hovaO); // Az új fej helye.
                                        ujA.testPoz[hovaS, hovaO] = ujFarokKoor; // Az új fej továbbmutat az új farokra.
                                        ujA.testPoz[be.farokPoz.Item1, be.farokPoz.Item2] = (-1, -1); // A régi farok helye üres lesz.
                                        ujA.farokPoz = ujFarokKoor; // Új farok koordináták.
                                        if (ujA.ÁllapotE()) // Ellenőrzi, hogy érvényes állapot-e
                                        {
                                            ujAllapot = ujA; // Visszaadja outban
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case "farok":
                        irányÁllít(irany,be.farokPoz); // Farokhoz képest x irány felé koordináták
                        if (hovaO >= 0 && hovaO < 4 && hovaS >= 0 && hovaS < 3 && be.testPoz[hovaS, hovaO] == (-1, -1)) // Korábbi alapfeltételek
                        {
                            (int, int) ujFej = (-1, -1); // Megkeresi a fej előtt következő testrészt ami az új fej lesz, hiszen azzal hátrál.
                            for(int i = 0; i < 3; i++)
                                {
                                    for(int j = 0; j < 4; j++)
                                        {
                                            if(be.fejPoz == be.testPoz[i,j]) // Az az elem van a fej előtt, amelyik a fej koordinátáit tárolja.
                                                {
                                                    ujFej = (i, j);
                                                    break;  
                                                }
                                        }
                                }
                            if (ujFej != (-1, -1))
                            {
                                for (int i = 0; i < 3; i++)
                                {
                                    for (int j = 0; j < 4; j++)
                                    {
                                        if ((i, j) == farokPoz) // Megvan a farok
                                        {
                                            ujA.testPoz[be.fejPoz.Item1, be.fejPoz.Item2] = (-1, -1); // Régi fej helye üres lesz.
                                            ujA.testPoz[hovaS, hovaO] = be.farokPoz; //Új farok a régi farokra mutat.
                                            ujA.farokPoz = (hovaS, hovaO); // Új farok koordináták
                                            ujA.testPoz[ujFej.Item1, ujFej.Item2] = ujA.farokPoz; // Már az új farok koordinátái, amire az új fej mutat.
                                            ujA.fejPoz = ujFej; // Fej pozíciója is módosítva
                                            if (ujA.ÁllapotE())
                                            {
                                                ujAllapot = ujA;
                                                return true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                }
            ujAllapot = null;
            return false;
        }
        
    }

    class Csúcs : Kígyó, IEquatable<Csúcs>
    {
        public int mélység = 0;
        public Csúcs? szülő;
        public Kígyó? állapot;
        public Csúcs(Csúcs szülő, int i) {
            this.szülő = szülő;
            this.mélység = szülő.mélység + 1;
            Kígyó? kimenet = null;
            SzuperOperátor(szülő.állapot,i, out kimenet);
            this.állapot = kimenet;
        }

        public bool Equals(Csúcs? other)
        {
            if (other == null)
            {
                return false;
            }

            return this.állapot.Equals(other.állapot);
        }

        public override int GetHashCode()
        {
            return this.állapot.GetHashCode();
        }

        public Csúcs(Kígyó állapot) { this.állapot = állapot;this.szülő = this;this.mélység = 0; }

        public Csúcs()
        {
        }

        public List<Csúcs> Kiterjesztés()
        {
            List<Csúcs> újCsúcsok = new List<Csúcs>();
            for(int i = 0; i < 8; i++)
            {
                Csúcs újCsúcs = new Csúcs(this, i);
                if(újCsúcs.állapot != null)
                {
                    újCsúcsok.Add(újCsúcs);
                }
            }
            return újCsúcsok;
        }

        public bool MegoldásE(List<Csúcs> szint)
        {
            foreach(Csúcs csucs in szint)
            {
                if (csucs.állapot != null && csucs.állapot.CélÁllapotE())
                {
                    return true;
                }
            }
            return false;
        }
    }

    class Gráf : Csúcs
    {
        public List<List<Csúcs>> csúcsok = new List<List<Csúcs>>();
        public Gráf(Csúcs anyaCsúcs) :base() {
            this.csúcsok.Add(new List<Csúcs> { anyaCsúcs });
        }

        public void SzintHozzáad(List<Csúcs> újSzint)
        {
            this.csúcsok.Add(újSzint);
        }

        public void Keresés()
        {
            List<Csúcs> ebbenKeresek = csúcsok[0][0].Kiterjesztés();
            this.SzintHozzáad(ebbenKeresek);
            int első = 1;
            int második = 0;
            List<Csúcs> mindenÚjCsúcs = new List<Csúcs>();
            while (!MegoldásE(ebbenKeresek))
            {
                // Először a csúcsok[1] elemei közt keresünk, azaz a három első állapotot terjesztjük ki
                foreach(Csúcs csucs in ebbenKeresek)
                {
                    List<Csúcs> újCsúcsok = csúcsok[első][második].Kiterjesztés();
                    foreach(Csúcs csucsi in újCsúcsok)
                    {
                        mindenÚjCsúcs.Add(csucsi);
                    }
                    második++;
                } // második eljut 2-ig, azaz végigloopoltunk a három első állapoton
                // Először le kéne ellenőrizni, kerültünk-e egyanabba az állapotba többször
                mindenÚjCsúcs = mindenÚjCsúcs.Distinct().ToList(); // Ez elv eltávolítja az egyformákat
                ebbenKeresek.Clear(); // Letisztítom az előző szintet, ami már hozzá van adva a gráfhoz
                foreach(Csúcs csucsika in mindenÚjCsúcs) {  ebbenKeresek.Add(csucsika); }
                this.SzintHozzáad(ebbenKeresek);
                első++; // Eggyel mélyebb szinten vagyunk
                második = 0; // Számozás újrakezdődik
                
            }
            Console.WriteLine("Megoldás:");
            List<Csúcs> Megoldás = new List<Csúcs>();
            for (int i = 0; i < this.csúcsok[első].Count; i++)
            {
                if (this.csúcsok[első][i].állapot.CélÁllapotE())
                {
                    
                    Megoldás.Add(this.csúcsok[első][i]);
                    Csúcs következőLépés = this.csúcsok[első][i].szülő;
                    Csúcs következőUtáni = következőLépés.szülő;
                    while(következőUtáni != következőLépés && következőUtáni != null && következőLépés != null)
                    {
                        Megoldás.Add(következőLépés);
                        Megoldás.Add(következőUtáni);
                        következőLépés = következőUtáni.szülő;
                        következőUtáni = következőLépés.szülő;
                    }
                }
            }
            for(int i = Megoldás.Count - 1; i>=0;i--){
                Megoldás[i].állapot.ToConsoleString();
            }
        }
    }

}
