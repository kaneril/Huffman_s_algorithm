﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Huffman_s_algorithm
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Выберите действие:\n 1 Закодировать текст\n 2 Раскодировать текст\n 3 Завершить работу");
                var str = Console.ReadLine();
                switch (str)
                {
                    case "1":
                        EncryptText();
                        break;
                    case "2":
                        DecryptText();
                        break;
                    case "3":
                        System.Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Неверный код действия");
                        break;
                }
            }
        }
        static void EncryptText()
        {
            var NormalText = ReadText();
            var symbols = ReadSymbols(NormalText);
            symbols = MakeSymbolsTree(symbols);
            Console.WriteLine("Введите путь к файлу с закодированным текстом");
            var path = Console.ReadLine();
            WriteInfo(symbols);
            WriteSymbolsTree(path, symbols);
            WriteEncryptText(path, NormalText, symbols);
        }
        static void DecryptText()
        {
            var DecryptText = ReadText();
            var symbols = ReadSymbolsTree(DecryptText.Split('\r')[0]);
            WriteInfo(symbols);
            DecryptText = DecryptText.Split('\n')[1];
            var NormalText = MakeDecryptText(symbols, DecryptText);
            WriteNormalText(NormalText);
        }        
        static List<Symbol> ReadSymbols(string NormalText)
        {
            List<Symbol> symbols = new List<Symbol>();
            foreach (var s in NormalText)
            {
                MakeSymbolsList(s, symbols);
            }
            return symbols;
        }
        static void MakeSymbolsList(int s, List<Symbol> symbols)
        {
            foreach (var i in symbols)
            {
                if (i.name == s)
                {
                    i.frequency++;
                    s = -1;
                    break;
                }
            }
            if (s != -1)
            {
                Symbol ss = new Symbol(s);
                symbols.Add(ss);
            }

        }
        static List<SymbolNode> MakeSymbolsNodeList(List<Symbol> symbols)
        {
            List<SymbolNode> SymbolsNodeList = new List<SymbolNode>();
            foreach (var i in symbols)
            {
                SymbolsNodeList.Add(new SymbolNode(i));
            }
            symbols.Clear();
            return SymbolsNodeList;
        }
        static void SymbolsNodeListInsert(List<SymbolNode> SymbolsNodeList, SymbolNode node)
        {
            int i;
            for (i = 0; i < SymbolsNodeList.Count; i++)
            {
                if (SymbolsNodeList[i].frequency >= node.frequency)
                {
                    break;
                }
            }
            SymbolsNodeList.Insert(i, node);
        }
        static List<Symbol> MakeSymbolsTree(List<Symbol> symbols)
        {
            symbols.Sort(new SymbolComparer());
            var SymbolsNodeList = MakeSymbolsNodeList(symbols);
            while ((SymbolsNodeList.Count != 1))
            {
                var node = new SymbolNode(SymbolsNodeList[0], SymbolsNodeList[1]);
                SymbolsNodeList.RemoveRange(0, 2);
                SymbolsNodeListInsert(SymbolsNodeList, node);
            }
            return SymbolsNodeList[0].children;
        }
        static void WriteSymbolsTree(string path, List<Symbol> symbols)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.Default))
                {
                    foreach (var i in symbols)
                    {
                        var SymbolCode = Convert.ToString(i.name, 2);
                        SymbolCode = SymbolCode.PadLeft(8, '0');
                        sw.Write(i.level + i.code + SymbolCode);
                    }
                    sw.Write("\r\n");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        static string FindSymbols(char s, List<Symbol> symbols)
        {
            foreach (var i in symbols)
            {
                if ((char)i.name == s)
                {
                    return i.code;
                }
            }
            return "";
        }
        static void WriteEncryptText(string path, string NormalText, List<Symbol> symbols)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default))
                {
                    for (var i = 0; i < NormalText.Length; i++)
                    {
                        var s = FindSymbols(NormalText[i], symbols);
                        sw.Write(s);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        static void WriteInfo(List<Symbol> symbols)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter("info.txt", false, System.Text.Encoding.Default))
                {
                    sw.WriteLine($"Длина алфавита {symbols.Count}");
                    foreach (var i in symbols)
                    {
                        sw.WriteLine($"Символ {(char)i.name} Частота {i.frequency} Код {i.code} Уровень {i.level}");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        static string ReadText()
        {
            Console.WriteLine("Введите путь к файлу с исходным текстом");
            var path = Console.ReadLine();
            string DecryptText = "";
            try
            {
                using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
                {
                    DecryptText = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return DecryptText;
        }
        static List<Symbol> ReadSymbolsTree(string DecryptText)
        {
            List<Symbol> symbols = new List<Symbol>();
            var i = 0;
            while (i < DecryptText.Length)
            {
                var level = 0;
                while (DecryptText[i] != '1')
                {
                    level++;
                    i++;
                }
                var code = DecryptText.Substring(i + 1, level);
                var name = Convert.ToUInt32(DecryptText.Substring(i + 1 + level, 8), 2);
                Symbol ss = new Symbol((int)name, code);
                symbols.Add(ss);
                i += level + 9;
            }
            return symbols;
        }
        static string MakeDecryptText(List<Symbol> symbols, string DecryptText)
        {
            string NewText = "";
            while (DecryptText.Length!=0)
            {
                foreach (var i in symbols)
                {
                    if ((DecryptText.Length != 0) && (DecryptText.StartsWith(i.code)))
                    {
                        NewText += (char)i.name;
                        DecryptText = DecryptText.Substring(i.code.Length);

                    }
                }
            }
            return NewText;
        }
        static void WriteNormalText(string NormalText)
        {
            Console.WriteLine("Введите путь к файлу с раскодированным текстом");
            var path = Console.ReadLine();
            try
            {
                using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.Default))
                {
                    sw.Write(NormalText);
                    sw.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    class SymbolNode: IComparable<SymbolNode>
    {
        public int name { get; set; }
        public int frequency { get; set; }
        public List<Symbol> children { get; set; }
        public SymbolNode(Symbol s)
        {
            name = -1;
            frequency = s.frequency;
            children = new List<Symbol>() { s };
        }
        public SymbolNode(SymbolNode s1, SymbolNode s2)
        {
            name = -1;
            frequency = s1.frequency+s2.frequency;
            foreach(var i in s1.children)
            {
                i.code = "0"+ i.code;
                i.level = "0"+ i.level;
            }
            foreach (var i in s2.children)
            {
                i.code = "1" + i.code;
                i.level = "0" + i.level;
            }
            children = new List<Symbol>() ;
            children.AddRange(s1.children);
            children.AddRange(s2.children);
        }
        public int CompareTo(SymbolNode s)
        {
            return this.name.CompareTo(s.name);
        }
        
    }
    class SymbolNodeComparer : IComparer<SymbolNode>
    {
        public int Compare(SymbolNode s1, SymbolNode s2)
        {
            if (s1.frequency > s2.frequency)
                return 1;
            else if (s1.frequency < s2.frequency)
                return -1;
            else
                return 0;
        }
    }

    class Symbol : IComparable<Symbol>
    {
        public int name { get; set; }
        public int frequency { get; set; }
        public string code { get; set; }
        public string level { get; set; }
        public Symbol(int n) { name = n; frequency = 1; code = ""; level = "1"; }
        public Symbol(int n, string c) { name = n; code = c;}
        public int CompareTo(Symbol s)
        {
            return this.name.CompareTo(s.name);
        }
        
    }
    class SymbolComparer : IComparer<Symbol>
    {
        public int Compare(Symbol s1, Symbol s2)
        {
            if (s1.frequency > s2.frequency)
                return 1;
            else if (s1.frequency < s2.frequency)
                return -1;
            else
                return 0;
        }
    }

   
}
