/*
 * @Author: KingWJC
 * @Date: 2021-08-20 09:43:09
 * @LastEditors: KingWJC
 * @LastEditTime: 2021-08-20 11:51:22
 * @Descripttion: 
 * @FilePath: \code\sample\CollectionSepcial.cs
 *
 * 特殊集合
 *
 */
using System;
using System.Text;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace code.sample
{
    public class CollectionSepcial
    {
        public static void Test()
        {
            "BitArray 操作样例".WriteTemplate();
            BitArraySample();

            "BitVector 操作样例".WriteTemplate();
            BitVector32Sample();

            "可观察的集合".WriteTemplate();
            var data = new ObservableCollection<string>();
            data.CollectionChanged += Data_CollectionChanged;
            data.Add("Two");
            data.Insert(0, "One");
            data.Remove("One");
            data[0] = "Updated";
            data.CollectionChanged -= Data_CollectionChanged;

            "不变的集合".WriteTemplate();
            ImmutableSample();
        }

        private static void ImmutableSample()
        {
            ImmutableArray<string> a1 = ImmutableArray.Create<string>("Ferrari");
            // 所有修改的操作，不修改原集合，返回一个新的不可变集合
            ImmutableArray<string> a2 = a1.Add("Williams");

            List<Person> persons = new List<Person>(){
                new Person{Name="Scrooge McDuck"},
                new Person{Name="Donald Duck"}
            };
            ImmutableList<Person> immutablePerson = persons.ToImmutableList();
            immutablePerson.ForEach(persons => WriteLine($"Person: {persons.Name}"));

            //使用构建器修改不可变集合
            ImmutableList<Person>.Builder builder = immutablePerson.ToBuilder();
            for (int i = 0; i < builder.Count;)
            {
                if (builder[i].Name == "Donald Duck")
                {
                    builder.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
            ImmutableList<Person> overdrawnPersons = builder.ToImmutable();
            overdrawnPersons.ForEach(persons => WriteLine($"Over drwan Person: {persons.Name}"));

            //使用linq
            ImmutableArray<string> arr = ImmutableArray.Create<string>("one", "two", "three", "four");
            arr.Where(s => s.StartsWith('t'));
        }

        /*
         * @description: 监测集合的变化
         * @param {*}
         * @return {*}
         */
        public static void Data_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            WriteLine($"action: {e.Action.ToString()}");

            if (e.OldItems != null)
            {
                WriteLine($"starting index for old item(s): {e.OldStartingIndex}");
                WriteLine("old item(s):");
                foreach (var item in e.OldItems)
                {
                    WriteLine(item);
                }
            }
            if (e.NewItems != null)
            {
                WriteLine($"starting index for new item(s): {e.NewStartingIndex}");
                WriteLine("new item(s): ");
                foreach (var item in e.NewItems)
                {
                    WriteLine(item);
                }
            }
            WriteLine();
        }

        public static void BitVector32Sample()
        {
            //创建掩码,并且位置连续
            int bit1 = BitVector32.CreateMask();
            int bit2 = BitVector32.CreateMask(bit1);
            int bit3 = BitVector32.CreateMask(bit2);
            int bit4 = BitVector32.CreateMask(bit3);
            int bit5 = BitVector32.CreateMask(bit4);

            BitVector32 bits1 = new BitVector32();
            bits1[bit1] = true;
            bits1[bit2] = false;
            bits1[bit3] = true;
            bits1[bit4] = true;
            bits1[bit5] = true;
            WriteLine(bits1);

            //自定掩码
            int receiver = 0x79abcdef;
            BitVector32 bits2 = new BitVector32(receiver);
            WriteLine(bits2);

            //创建片段,包含偏移量和掩码
            BitVector32.Section sectionA = BitVector32.CreateSection(0xfff);
            BitVector32.Section sectionB = BitVector32.CreateSection(0xff, sectionA);
            BitVector32.Section sectionC = BitVector32.CreateSection(0x7, sectionB);

            WriteLine($"Section A: {IntToBinaryString(bits2[sectionA], true)}");
            WriteLine($"Section B: {IntToBinaryString(bits2[sectionB], true)}");
            WriteLine($"Section C: {IntToBinaryString(bits2[sectionC], true)}");
        }

        /*
         * @description: int类型转二进制字符串
         * @param {*}
         * @return {*}
         */
        private static string IntToBinaryString(int bits, bool removeTrailingZero)
        {
            var sb = new StringBuilder(32);
            for (int i = 0; i < 32; i++)
            {
                if ((bits & 0x80000000) != 0)
                {
                    sb.Append("1");
                }
                else
                {
                    sb.Append("0");
                }
                //右移一位，否则结果为空
                bits = bits << 1;
            }
            string s = sb.ToString();
            if (removeTrailingZero)
            {
                return s.TrimStart('0');
            }
            else
            {
                return s;
            }
        }

        private static void BitArraySample()
        {
            BitArray bitArray = new BitArray(8);
            bitArray.SetAll(true);
            bitArray.Set(1, false);
            bitArray[5] = false;
            Write("initialized:");
            DisplayBitArray(bitArray);
            WriteLine();

            Write("not ");
            DisplayBitArray(bitArray);
            bitArray.Not();
            Write(" = ");
            DisplayBitArray(bitArray);
            WriteLine();

            BitArray bitArray1 = new BitArray(bitArray);
            bitArray1[0] = true;
            bitArray1[1] = false;
            bitArray1[7] = true;

            DoWork(bitArray, bitArray1, "or", (x, y) => x.Or(y));
            DoWork(bitArray, bitArray1, "and", (x, y) => x.And(y));
            DoWork(bitArray, bitArray1, "xor", (x, y) => x.Xor(y));
        }

        private static void DisplayBitArray(BitArray bits)
        {
            foreach (bool item in bits)
            {
                Write(item ? 1 : 0);
            }
        }

        private static void DoWork(BitArray bitArray, BitArray bitArray1, string operateName, Action<BitArray, BitArray> action)
        {
            DisplayBitArray(bitArray);
            Write($" {operateName} ");
            DisplayBitArray(bitArray1);
            Write(" = ");
            action(bitArray, bitArray1);
            DisplayBitArray(bitArray);
            WriteLine();
        }
    }
}