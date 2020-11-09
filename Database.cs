using Avalonia.Controls;
using Avalonia.Media;
using Serilog.Debugging;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        public static List<string> names = new List<string>();

        public class Data
        {
            public string formula, name, cas;
            public List<int> contrib;
            public int mol;
            int hash1, hash2;
            public bool mode;

            public Data(string F, string N, string C, List<int> Cs, bool Mode = false)
            {
                formula = F; name = N; cas = C;
                contrib = Cs;
                mol = new MolCalculator(F, AtomDB.mass).Calculate();
                hash1 = new MolCalculator(F, AtomDB.hash1, 998244353).Calculate();
                hash2 = new MolCalculator(F, AtomDB.hash2, 1234567891).Calculate();
                mode = Mode;
            }

            // 转换为一个可以输出的字符串
            public string toString()
            {
                string convert = formula.Replace(".", "·") + " | " + name;
                if (cas != "" && cas != null) convert += " | " + cas;
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
        public List<Data> datas = new List<Data>();
        public Dictionary<string, int> position = new Dictionary<string, int>();
        public int id = -1;

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
                names.Add("");
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
                    position[F] = datas.Count;
                    datas.Add(new Data(F, N, C, vs1));
                    bt = input.ReadByte();
                }
                input.Close();
            }
            catch(Exception) { }
        }

        public void Append(string file, string name)
        {
            BufferedStream input = new BufferedStream(File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read), 16777216);
            id = 0;
            for (int i = 1; i < names.Count; ++i)
                if (names[i] == name)
                    id = i;
            if(id == 0)
            {
                id = names.Count;
                names.Add(name);
            }
            int bt = input.ReadByte();
            while(bt != 0)
            {
                string F = "", N = "", C = "";
                while (bt != 0)
                {
                    F += (char)bt;
                    bt = input.ReadByte();
                }
                bt = input.ReadByte();
                List<byte> vs = new List<byte>();
                while (bt != 0)
                {
                    vs.Add((byte)bt);
                    bt = input.ReadByte();
                }
                bt = input.ReadByte();
                N = Encoding.UTF8.GetString(vs.ToArray());
                while (bt != 0)
                {
                    C += (char)bt;
                    bt = input.ReadByte();
                }
                if(position.ContainsKey(F))
                {
                    int pos = position[F];
                    if (!datas[pos].contrib.Contains(id))
                        datas[pos].contrib.Add(id);
                    datas[pos] = new Data(F, N, C, datas[pos].contrib, true);
                }
                else
                {
                    position[F] = datas.Count;
                    datas.Add(new Data(F, N, C, new List<int>(), true));
                    datas[datas.Count - 1].contrib.Add(id);
                }
                bt = input.ReadByte();
            }
            input.Close();
        }

        public IEnumerable<ListBoxItem> GetList()
        {
            List<ListBoxItem> list = new List<ListBoxItem>();
            for(int i = 0; i < datas.Count; ++i)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = datas[i].toString();
                if(datas[i].mode)
                {
                    SolidColorBrush brush = new SolidColorBrush();
                    brush.Color = Colors.DarkRed;
                    item.Background = brush.ToImmutable();
                }
                else if(datas[i].contrib.Contains(id))
                {
                    SolidColorBrush brush = new SolidColorBrush();
                    brush.Color = Colors.DarkViolet;
                    item.Background = brush.ToImmutable();
                }
                item.DataContext = datas[i];
                list.Add(item);
            }
            return list.ToImmutableArray();
        }

        public IEnumerable<ListBoxItem> Add(string F, string N, string C)
        {
            if (position.ContainsKey(F))
            {
                int pos = position[F];
                if (!datas[pos].contrib.Contains(id))
                    datas[pos].contrib.Add(id);
                datas[pos] = new Data(F, N, C, datas[pos].contrib, true);
            }
            else
            {
                position[F] = datas.Count;
                datas.Add(new Data(F, N, C, new List<int>(), true));
                datas[datas.Count - 1].contrib.Add(id);
            }
            return GetList();
        }

        public IEnumerable<ListBoxItem> Delete(string F)
        {
            if (position.ContainsKey(F))
            {
                int pos = position[F];
                datas.RemoveAt(pos);
                position.Remove(F);
            }
            for(int i = 0; i < datas.Count; ++i)
            {
                position[datas[i].name] = i;
            }
            return GetList();
        }

        public void SaveUser(string file)
        {
            BufferedStream output = new BufferedStream(File.Open(file, FileMode.Create, FileAccess.Write, FileShare.Write), 16777216);
            foreach(Data data in datas)
            {
                if (!data.mode) continue;
                string F = data.formula, N = data.name, C = data.cas;
                for (int i = 0; i < F.Length; ++i) output.WriteByte((byte)F[i]);
                output.WriteByte(0);
                byte[] vs = Encoding.UTF8.GetBytes(N);
                output.Write(vs, 0, vs.Length);
                output.WriteByte(0);
                if (C == null) C = "";
                for (int i = 0; i < C.Length; ++i) output.WriteByte((byte)C[i]);
                output.WriteByte(0);
            }
            output.WriteByte(0);
            output.Flush(); output.Close();
        }
    }
}
