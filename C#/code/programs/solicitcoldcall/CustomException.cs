/*
 * @Author: KingWJC
 * @Date: 2021-08-24 10:51:15
 * @LastEditors: KingWJC
 * @LastEditTime: 2021-08-24 11:43:27
 * @Descripttion: 
 * @FilePath: \code\programs\solicitcoldcall\CustomException.cs
 *
 * 自定义异常
 */
using System;

namespace code.programs.solicitcoldcall
{
    /*
     * @description: 方法执行不正确的异常
     * @param {*}
     * @return {*}
     */
    public class UnexpectedException : Exception
    {
        public UnexpectedException(string message) : base(message)
        {

        }

        public UnexpectedException(string message, Exception inner) : base(message, inner)
        {

        }
    }

    /*
     * @description: 检测到间谍的异常
     * @param {*}
     * @return {*}
     */
    public class SalesSpyFoundException : Exception
    {
        public SalesSpyFoundException(string spyName)
        : base($"Sales spy found with name : {spyName}")
        {

        }

        public SalesSpyFoundException(string spyName, Exception inner)
        : base($"Sales spy found with name : {spyName}", inner)
        {

        }
    }

    /*
     * @description: 文件格式错误的异常
     * @param {*}
     * @return {*}
     */
    public class ColdCallFileFormatException : Exception
    {
        public ColdCallFileFormatException(string message)
            : base(message)
        {
        }

        public ColdCallFileFormatException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}