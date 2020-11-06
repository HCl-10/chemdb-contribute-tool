using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;

namespace chemdb_contribute_tool
{
    class MolCalculator
    {
        readonly int Maxweight = 100000000;
        Dictionary<string, int> mp;
        int mod = -1;
        string moe;

        public MolCalculator(string mol, Dictionary<string, int> mass, int Mod = -1)
        {
            moe = mol;
            mod = Mod;
            mp = mass;
        }

        private int GetAtomWeight(int l, int r)
        {
            string temp = moe.Substring(l, r - l);
            if (!mp.ContainsKey(temp)) return -1;
            return mp[temp];
        }

        private int CalculateMol(int l, int r)
        {
            if (moe[l] == '(' || moe[l] == '[' || moe[l] == '{' || moe[l] == '<')
            {
                int at = 0, bt = 0, ct = 0, dt = 0;
                if (moe[0] == '(')
                {
                    at++;
                }
                else if (moe[0] == '[')
                {
                    bt++;
                }
                else if (moe[0] == '{')
                {
                    ct++;
                }
                else
                {
                    dt++;
                }
                int i = l;
                while (i < r && (at > 0 || bt > 0 || ct > 0 || dt > 0))
                {
                    if (moe[i] == '(')
                    {
                        at++;
                    }
                    else if (moe[i] == ')')
                    {
                        at--;
                    }
                    else if (moe[i] == '[')
                    {
                        bt++;
                    }
                    else if (moe[i] == ']')
                    {
                        bt--;
                    }
                    else if (moe[i] == '{')
                    {
                        ct++;
                    }
                    else if (moe[i] == '}')
                    {
                        ct--;
                    }
                    else if (moe[i] == '<')
                    {
                        dt++;
                    }
                    else if (moe[i] == '>')
                    {
                        dt--;
                    }
                    if (at < 0 || bt < 0 || ct < 0 || dt < 0)
                    {
                        return -1;
                    }
                    i++;
                }
                if(i == r && (at > 0 || bt > 0 || ct > 0 || dt > 0))
                {
                    return -1;
                }
                int j = i, tms = 0;
                while (j < r && moe[j] >= '0' && moe[j] <= '9')
                {
                    if (i == j && moe[j] == '0') return -1;
                    tms = tms * 10 + moe[j] - '0';
                    if (tms > Maxweight)
                    {
                        return -1;
                    }
                    j++;
                }
                if (tms == 0) tms = 1;
                int wl = CalculateMol(l + 1, i - 1);
                long ans = (long)wl * tms;
                if(j == r)
                {
                    if (wl == -1) return -1;
                    if (mod == -1 && ans > Maxweight) return -1;
                    if (mod != -1) ans %= mod;
                    return (int)ans;
                }
                if (wl == -1) return -1;
                if (mod == -1 && ans > Maxweight) return -1;
                int wr = CalculateMol(j, r);
                if (wr == -1) return -1;
                ans += wr;
                if (mod == -1 && ans > Maxweight) return -1;
                if (mod != -1) ans %= mod;
                return (int)ans;
            }
            if (moe[l] < 'A' || moe[l] > 'Z') return -1;
            int s = l + 1;
            while (s < r && moe[s] >= 'a' && moe[s] <= 'z') ++s;
            if (s == r) return GetAtomWeight(l, r);
            int t = s;
            int tt = 0;
            while (t < r && moe[t] >= '0' && moe[t] <= '9')
            {
                if (s == t && moe[t] == '0') return -1;
                tt = tt * 10 + moe[t] - '0';
                if (tt > Maxweight)
                {
                    return -1;
                }
                t++;
            }
            if (tt == 0) tt = 1;
            int Wl = GetAtomWeight(s, t);
            long res = (long)Wl * tt;
            if(t == r)
            {
                if (Wl == -1) return -1;
                if (mod == -1 && res > Maxweight) return -1;
                if (mod != -1) res %= mod;
                return (int)res;
            }
            int Wr = CalculateMol(t, r);
            if (Wr == -1) return -1;
            res += Wr;
            if (mod == -1 && res > Maxweight) return -1;
            if (mod != -1) res %= mod;
            return (int)res;
        }
        private int CalculateSingle(int l, int r)
        {
            if (moe[l] < '0' || moe[l] > '9') return CalculateMol(l, r);
            int i = l, num = 0;
            while(i < r && moe[i] >= '0' && moe[i] <= '9')
            {
                if (i == l && moe[i] == '0') return -1;
                num = num * 10 + moe[i] - '0';
                if (num > Maxweight) return -1;
                ++i;
            }
            if (i == r) return -1;
            int count = CalculateMol(i, r);
            if (count == -1)
            {
                return -1;
            }
            else
            {
                if (mod == -1 && (long)num * count > Maxweight){
                    return -1;
                }
                long ans = (long)num * count;
                if (mod != -1) ans %= mod;
                return (int)ans;
            }
        }
        private int CalculateFul(int l, int r)
        {
            int spl = -1;
            for(int i = l; i < r; ++i)
                if((moe[i] < '0' || moe[i] > '9') && (moe[i] < 'a' || moe[i] > 'z') &&
                    (moe[i] < 'A' || moe[i] > 'Z') && moe[i] != '(' && moe[i] != ')' &&
                    moe[i] != '[' && moe[i] != ']' && moe[i] != '{' && moe[i] != '}' &&
                    moe[i] != '<' && moe[i] != '>')
                {
                    spl = i;
                    break;
                }
            if (spl == l) return -1;
            if (spl == -1) return CalculateSingle(l, r);
            int Cl = CalculateFul(l, spl), Cr = CalculateFul(spl + 1, r);
            if (Cl == -1 || Cr == -1) return -1;
            if (mod == -1 && Cl + Cr > Maxweight) return -1;
            int res = Cl + Cr;
            if (mod != -1) res %= mod;
            return res;
        }

        public int Calculate()
        {
            return CalculateFul(0, moe.Length);
        }
    }
}
