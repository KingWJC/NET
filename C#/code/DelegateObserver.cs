/*
 * @Author: KingWJC
 * @Date: 2021-08-12 16:32:22
 * @LastEditors: KingWJC
 * @LastEditTime: 2021-08-12 17:18:17
 * @Descripttion: 
 * @FilePath: \code\DelegateObserver.cs
 * 
 * 委托、事件与Observer设计模式
 *
 */
using System;

namespace code
{
    public class DelegateObserver
    {
        public static void Test()
        {
            "Delegate Event Observer".WriteTemplate();
            Heater heater = new Heater();
            Alarm alarm = new Alarm();
            heater.Boiled += alarm.MakeAlarm;
            heater.Boiled += Display.MakeDisplay;
            heater.BoiledWater();
        }
    }
    /* 烧水壶 */
    class Heater
    {
        public string Type { get; set; } = "RealFire 001";
        public string Area { get; set; } = "China";
        private int temperature;
        public delegate void BoiledEventHandler(object obj, BoiledEventArgs eventArgs);
        public event BoiledEventHandler Boiled;

        private void OnBoiled(BoiledEventArgs eventArgs)
        {
            if (Boiled != null)
            {
                Boiled(this, eventArgs);
            }
        }

        public void BoiledWater()
        {
            for (int i = 0; i < 100; i++)
            {
                temperature = i;
                if (temperature > 97)
                {
                    OnBoiled(new BoiledEventArgs(temperature));
                }
            }
        }
    }
    /* 事件数据 */
    class BoiledEventArgs : EventArgs
    {
        public int Temperature { get; set; }

        public BoiledEventArgs(int temp)
        {
            Temperature = temp;
        }
    }
    /* 警报器 */
    class Alarm
    {
        public void MakeAlarm(object sender, BoiledEventArgs e)
        {
            Heater heater = (Heater)sender;
            Console.WriteLine("Alarm：{0} - {1}: ", heater.Area, heater.Type);
            Console.WriteLine("Alarm：水快烧开了，当前温度：{0}度。", e.Temperature);
            Console.WriteLine();
        }
    }
    /* 显示器 */
    class Display
    {
        public static void MakeDisplay(object sender, BoiledEventArgs e)
        {
            Heater heater = (Heater)sender;
            Console.WriteLine("Display：{0} - {1}: ", heater.Area, heater.Type);
            Console.WriteLine("Display：水快烧开了，当前温度：{0}度。", e.Temperature);
            Console.WriteLine();
        }
    }
}