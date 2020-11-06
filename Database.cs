using Serilog.Debugging;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace chemdb_contribute_tool
{
    class Restore
    {
        public static Database db = new Database();
    }

    class Database
    {
        static List<string> names = new List<string>();

        class Data
        {
            string formula, name, cas;
            List<int> contrib;
            int mol;
            int hash1, hash2;

            public Data(string F, string N, string C, List<int> Cs)
            {
                formula = F; name = N; cas = C;
                contrib = Cs;
                mol = new MolCalculator(F, AtomDB.mass).Calculate();
                hash1 = new MolCalculator(F, AtomDB.hash1, 998244353).Calculate();
                hash2 = new MolCalculator(F, AtomDB.hash2, 1234567891).Calculate();
            }

            // 转换为一个可以输出的字符串
            public string toString()
            {
                string convert = formula + " | " + name;
                if (cas != "") convert += " | " + cas;
                convert += " [";
                foreach(int i in contrib) {
                    convert += names[i];
                }
                return convert + ']';
            }

            // 判断这个分子信息是否可以在指定的搜索字符串下显示
            public bool Filte(string search)
            {
                if (formula.ToLower().Contains(search.ToLower()) || name.ToLower().Contains(search.ToLower())
                    || cas.ToLower().Contains(search.ToLower())) return true;
                MolCalculator calculator = new MolCalculator(search, AtomDB.hash1, 998244353);
                if(calculator.Calculate() == hash1)
                {
                    MolCalculator calculator1 = new MolCalculator(search, AtomDB.hash2, 1234567891);
                    if (calculator1.Calculate() == hash2) return true; // 同分异构体
                }
                try
                {
                    double val = Convert.ToDouble(search);
                    double del = val * 10 - mol;
                    if (del < 0) del = -del;
                    if (del < 0.4) return true;
                } catch(Exception) { }
                return false;
            }
        }

        public string version = "";
        List<Data> datas;
        Dictionary<string, int> position;

        public void Read(string file)
        {
            try
            {
                BufferedStream input = new BufferedStream(File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read), 16777216);
                int bt = input.ReadByte();
                while(bt != 0)
                {
                    version += (char)bt;
                    bt = input.ReadByte();
                }
                bt = input.ReadByte();
                while(bt != 0)
                {
                    List<byte> vs = new List<byte>();
                    while(bt != 0)
                    {
                        vs.Add((byte)bt);
                        bt = input.ReadByte();
                    }
                    bt = input.ReadByte();
                    names.Add(Encoding.UTF8.GetString(vs.ToArray()));
                }
                int peopleCount = names.Count;
                int bNeed = 1;
                if (peopleCount > 255) bNeed = 2;
                if (peopleCount > 65535) bNeed = 3;
                if (peopleCount > 16777215) bNeed = 4;
                bt = input.ReadByte();
                while(bt != 0)
                {
                    string F = "", N = "", C = "";
                    while(bt != 0)
                    {
                        F += (char)bt;
                        bt = input.ReadByte();
                    }
                    bt = input.ReadByte();
                    List<byte> vs = new List<byte>();
                    while(bt != 0)
                    {
                        vs.Add((byte)bt);
                        bt = input.ReadByte();
                    }
                    bt = input.ReadByte();
                    N = Encoding.UTF8.GetString(vs.ToArray());
                    while(bt != 0)
                    {
                        C += (char)bt;
                        bt = input.ReadByte();
                    }
                    List<int> vs1 = new List<int>();
                    while(true)
                    {
                        int num = 0;
                        for (int i = 0; i < bNeed; i++)
                            num = num * 256 + input.ReadByte();
                        if (num == 0) break;
                        vs1.Add(num);
                    }
                    datas.Add(new Data(F, N, C, vs1));
                    bt = input.ReadByte();
                }
            }
            catch(Exception) { }
        }
    }
}
