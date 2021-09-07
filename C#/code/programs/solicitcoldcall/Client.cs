/*
 * @Author: KingWJC
 * @Date: 2021-08-24 10:48:26
 * @LastEditors: KingWJC
 * @LastEditTime: 2021-09-07 11:20:32
 * @Descripttion: 
 * @FilePath: \code\programs\solicitcoldcall\Client.cs
 *
 * Exception自定义异常的示例：销售人员打陌生电话cold-call，来获取客户。
 *
 */
using System.IO;
using static System.Console;

namespace code.programs.solicitcoldcall
{
    public class Client
    {
        public static void Test()
        {
            Write("Please type in the name of the file " + "containing the names of the people to be cold called > ");
            string fileName = "resource\\people.txt";//ReadLine();
            ReadLoop(fileName);
        }

        public static void ReadLoop(string fileName)
        {
            var peopleToRing = new ColdCallFileReader();
            try
            {
                peopleToRing.Open(fileName);
                for (int i = 0; i < peopleToRing.NPeopleToRing; i++)
                {
                    peopleToRing.ProcessNextPerson();
                }
                WriteLine(" All callers processed currectly");
            }
            catch (FileNotFoundException)
            {
                WriteLine($"The file {fileName} does not exist");
            }
            catch (ColdCallFileFormatException ex)
            {
                WriteLine($"The file {fileName} appears to have been corrupted");
                WriteLine($"Details of problem are: {ex.Message}");
                if (ex.InnerException != null)
                {
                    WriteLine($"Inner exception was: {ex.InnerException.Message}");
                }
            }
            catch (System.Exception ex)
            {
                WriteLine($"Exception occurred:\n{ex.Message}");
            }
            finally
            {
                peopleToRing.Dispose();
            }
        }
    }
}