using System;
using System.Collections.Generic;
using System.Text;

namespace chemdb_contribute_tool
{
    class AtomDB
    {
        public static string[] Elements = {
            "H", "D", "T", "He", "Li", "Be", "B", "C", "N", "O", "F", "Ne", "Na", "Mg", "Al", "Si", "P",
            "S", "Cl", "Ar", "K", "Ca", "Sc", "Ti", "V", "Cr", "Mn", "Fe", "Co", "Ni", "Cu", "Zn",
            "Ga", "Ge", "As", "Se", "Br", "Kr", "Rb", "Sr", "Y", "Zr", "Nb", "Mo", "Tc", "Ru", "Rh",
            "Pd", "Ag", "Cd", "In", "Sn", "Sb", "Te", "I", "Xe", "Cs", "Ba", "La", "Ce", "Pr", "Nd",
            "Pm", "Sm", "Eu", "Gd", "Tb", "Dy", "Ho", "Er", "Tm", "Yb", "Lu", "Hf", "Ta", "W", "Re",
            "Os", "Ir", "Pt", "Au", "Hg", "Tl", "Pb", "Bi", "Po", "At", "Rn", "Fr", "Ra", "Ac", "Th",
            "Pa", "U", "Np", "Pu", "Am", "Cm", "Bk", "Cf", "Es", "Fm", "Md", "No", "Lr", "Rf", "Db",
            "Sg", "Bh", "Hs", "Mt", "Ds", "Rg", "Cn", "Nh", "Fl", "Mc", "Lv", "Ts", "Og"
        };
        public static Dictionary<string, int> mass = new Dictionary<string, int>();
        public static Dictionary<string, int> hash1 = new Dictionary<string, int>();
        public static Dictionary<string, int> hash2 = new Dictionary<string, int>();

        public static void init()
        {
            mass["H"] = 10;
            mass["D"] = 20;
            mass["T"] = 30;
            mass["He"] = 40;
            mass["Li"] = 70;
            mass["Be"] = 90;
            mass["B"] = 110;
            mass["C"] = 120;
            mass["N"] = 140;
            mass["O"] = 160;
            mass["F"] = 190;
            mass["Ne"] = 200;
            mass["Na"] = 230;
            mass["Mg"] = 240;
            mass["Al"] = 270;
            mass["Si"] = 280;
            mass["P"] = 310;
            mass["S"] = 320;
            mass["Cl"] = 355;
            mass["Ar"] = 400;
            mass["K"] = 390;
            mass["Ca"] = 400;
            mass["Sc"] = 450;
            mass["Ti"] = 480;
            mass["V"] = 510;
            mass["Cr"] = 520;
            mass["Mn"] = 550;
            mass["Fe"] = 560;
            mass["Co"] = 590;
            mass["Ni"] = 590;
            mass["Cu"] = 640;
            mass["Zn"] = 650;
            mass["Ga"] = 700;
            mass["Ge"] = 730;
            mass["As"] = 750;
            mass["Se"] = 790;
            mass["Br"] = 800;
            mass["Kr"] = 840;
            mass["Rb"] = 855;
            mass["Sr"] = 880;
            mass["Y"] = 890;
            mass["Zr"] = 910;
            mass["Nb"] = 930;
            mass["Mo"] = 960;
            mass["Tc"] = 980;
            mass["Ru"] = 1010;
            mass["Rh"] = 1030;
            mass["Pd"] = 1060;
            mass["Ag"] = 1080;
            mass["Cd"] = 1120;
            mass["In"] = 1150;
            mass["Sn"] = 1190;
            mass["Sb"] = 1220;
            mass["Te"] = 1280;
            mass["I"] = 1270;
            mass["Xe"] = 1310;
            mass["Cs"] = 1330;
            mass["Ba"] = 1370;
            mass["La"] = 1390;
            mass["Ce"] = 1400;
            mass["Pr"] = 1410;
            mass["Nd"] = 1440;
            mass["Pm"] = 1450;
            mass["Sm"] = 1500;
            mass["Eu"] = 1520;
            mass["Gd"] = 1570;
            mass["Tb"] = 1590;
            mass["Dy"] = 1625;
            mass["Ho"] = 1650;
            mass["Er"] = 1670;
            mass["Tm"] = 1690;
            mass["Yb"] = 1730;
            mass["Lu"] = 1750;
            mass["Hf"] = 1785;
            mass["Ta"] = 1810;
            mass["W"] = 1840;
            mass["Re"] = 1860;
            mass["Os"] = 1900;
            mass["Ir"] = 1922;
            mass["Pt"] = 1950;
            mass["Au"] = 1970;
            mass["Hg"] = 2010;
            mass["Tl"] = 2040;
            mass["Pb"] = 2070;
            mass["Bi"] = 2090;
            mass["Po"] = 2090;
            mass["At"] = 2100;
            mass["Rn"] = 2220;
            mass["Fr"] = 2230;
            mass["Ra"] = 2260;
            mass["Ac"] = 2270;
            mass["Th"] = 2320;
            mass["Pa"] = 2310;
            mass["U"] = 2380;
            mass["Np"] = 2370;
            mass["Pu"] = 2440;
            mass["Am"] = 2430;
            mass["Cm"] = 2470;
            mass["Bk"] = 2470;
            mass["Cf"] = 2510;
            mass["Es"] = 2520;
            mass["Fm"] = 2570;
            mass["Md"] = 2580;
            mass["No"] = 2590;
            mass["Lr"] = 2620;
            mass["Rf"] = 2650;
            mass["Db"] = 2680;
            mass["Sg"] = 2710;
            mass["Bh"] = 2700;
            mass["Hs"] = 2770;
            mass["Mt"] = 2760;
            mass["Ds"] = 2810;
            mass["Rg"] = 2800;
            mass["Cn"] = 2850;
            mass["Nh"] = 2840;
            mass["Fl"] = 2890;
            mass["Mc"] = 2880;
            mass["Lv"] = 2930;
            mass["Ts"] = 2940;
            mass["Og"] = 2940;

            Random random = new Random((int)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds);

            foreach(string s in Elements)
            {
                hash1[s] = random.Next(985, 996251404);
                hash2[s] = random.Next(985, 996251404);
            }
        }
    }
}
