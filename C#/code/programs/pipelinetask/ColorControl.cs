/*
 * @Author: KingWJC
 * @Date: 2021-08-20 14:20:44
 * @LastEditors: KingWJC
 * @LastEditTime: 2021-08-20 14:32:08
 * @Descripttion: 
 * @FilePath: \code\programs\pipelinetask\ColorConsole.cs
 *
 * 使用同步，避免返回错误的颜色输出。
 */
using System;

namespace code.programs.pipelinetask
{

    public class ColorConsole
    {
        private static object syncOutput = new object();

        public static void WriteLine(string message)
        {
            lock (syncOutput)
            {
                Console.WriteLine(message);
            }
        }

        public static void WriteLine(string message, string color)
        {
            lock (syncOutput)
            {
                Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), color);
            }
        }
    }
}