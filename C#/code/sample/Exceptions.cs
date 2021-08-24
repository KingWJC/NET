/*
 * @Author: KingWJC
 * @Date: 2021-08-24 09:26:23
 * @LastEditors: KingWJC
 * @LastEditTime: 2021-08-24 10:34:06
 * @Descripttion: 
 * @FilePath: \code\sample\Exceptions.cs
 *
 * #line 预处理命令 可以改变代码的行号，获取调用堆栈的更好的信息
 *
 * 
 */

using System;
using static System.Console;
using System.Runtime.CompilerServices;

namespace code.sample
{
    public class Exceptions
    {
        public static void Test()
        {
            var e = new Exceptions();
            e.Log();
            Action al = () => e.Log();

            var methods = new Action[]{
                ExceptionSample,
                HandleAndThrowAgain,
                HandleAndThrowWithInner,
                HandleAndRethrow,
                HanddleWithFilter
            };

            foreach (var item in methods)
            {
                try
                {
                    item();
                }
                catch (Exception ex)
                {
                    WriteLine(ex.Message);
                    WriteLine(ex.StackTrace);
                    if (ex.InnerException != null)
                    {
                        WriteLine($"Inner Exception : {ex.InnerException.Message}");
                        WriteLine(ex.InnerException.StackTrace);
                    }
                }
            }
        }

        public static void ThrowAnException(string message)
        {
            var ex = new MyCustomException(message, new Exception("innerException"));
            ex.ErrorCode = 404;
            throw ex;
        }

        public static void ExceptionSample()
        {
            try
            {
                ThrowAnException("sample Exception");
            }
            catch (MyCustomException ex) when (ex.ErrorCode == 404)
            {
                WriteLine($"Exception caught with filter {ex.Message} and {ex.ErrorCode}");
            }
            catch (MyCustomException ex)
            {
                WriteLine($"Exception caught {ex.Message} and {ex.ErrorCode}");
            }
            catch (Exception ex)
            {
                WriteLine($"Exception caught {ex.Message}");
            }
            finally
            {
                WriteLine("Finlly Over\n");
            }
        }
#line 1000
        public static void HanddleWithFilter()
        {
            try
            {
                ThrowAnException("test 4");
            }
            catch (Exception ex) when (Filter(ex))
            {
                WriteLine(ex.Message + " block never invoked");
            }
        }

        public static bool Filter(Exception ex)
        {
            WriteLine($"just log {ex.Message}");
            return false;
        }

#line 2000
        public static void HandleAndRethrow()
        {
            try
            {
                ThrowAnException("test 3");
            }
            catch (Exception ex)
            {
                WriteLine($"Log exception {ex.Message} and rethrow");
                throw;  // line 2009
            }
        }
#line 3000
        public static void HandleAndThrowWithInner()
        {
            try
            {
                ThrowAnException("test 2");
            }
            catch (Exception ex)
            {
                WriteLine($"Log exception {ex.Message} and throw again");
                throw new MyCustomException("throw with inner exception", ex);
            }
        }

#line 4000
        public static void HandleAndThrowAgain()
        {
            try
            {
                ThrowAnException("test 1");
            }
            catch (Exception ex)
            {
                WriteLine($"Log exception {ex.Message} and throw again");
                throw ex;  // you shouldn't do that - line 4009
            }
        }

        /*
         * @description: 从代码中获取行号，文件名，成员名
         * @param {*}
         * @return {*}
         */
        public void Log([CallerLineNumber] int line = -1, [CallerFilePath] string path = null, [CallerMemberName] string name = null)
        {
            WriteLine((line < 0) ? "No line" : "Line " + line);
            WriteLine((path == null) ? "No file path" : path);
            WriteLine((name == null) ? "No member name" : name);
            WriteLine();
        }
    }

    /*
     * @description: 自定义异常
     * @param {*}
     * @return {*}
     */
    public class MyCustomException : Exception
    {
        public int ErrorCode { get; set; }

        public MyCustomException(string message, Exception innerException)
        : base(message, innerException)
        {

        }
    }
}