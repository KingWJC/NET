/*
 * @Author: KingWJC
 * @Date: 2021-08-24 11:01:45
 * @LastEditors: KingWJC
 * @LastEditTime: 2021-08-24 11:59:41
 * @Descripttion: 
 * @FilePath: \code\programs\solicitcoldcall\ColdCallFileReader.cs
 *
 * 文件读取
 *
 */
using System;
using System.IO;

namespace code.programs.solicitcoldcall
{
    public class ColdCallFileReader : IDisposable
    {
        private FileStream _fs;
        private StreamReader _sr;
        private uint _nPeopleToRing;
        private bool _isOpen = false;
        private bool _isDisopsed = false;

        public uint NPeopleToRing
        {
            get
            {
                if (_isDisopsed)
                {
                    throw new ObjectDisposedException("NPeopleToRing");
                }

                if (!_isOpen)
                {
                    throw new UnexpectedException(
                        "Attempted to access cold–call file that is not open");
                }

                return _nPeopleToRing;
            }
        }

        public void Open(string fileName)
        {
            if (_isDisopsed)
            {
                throw new ObjectDisposedException(nameof(ColdCallFileReader));
            }

            _fs = new FileStream(fileName, FileMode.Open);
            _sr = new StreamReader(_fs);

            try
            {
                var firstLine = _sr.ReadLine();
                _nPeopleToRing = uint.Parse(firstLine);
                _isOpen = true;
            }
            catch (FormatException ex)
            {
                throw new ColdCallFileFormatException($"First line isn\'t an integer {ex}");

            }
        }

        public void ProcessNextPerson()
        {
            if (_isDisopsed)
            {
                throw new ObjectDisposedException(nameof(ColdCallFileReader));
            }

            if (!_isOpen)
            {
                throw new UnexpectedException("Attempted to access coldcall file that is not open");
            }

            try
            {
                string name = _sr.ReadLine();
                if (name == null)
                {
                    throw new ColdCallFileFormatException("Not enough names");
                }
                if (name[0] == 'B')
                {
                    throw new SalesSpyFoundException(name);
                }
                Console.WriteLine(name);
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public void Dispose()
        {
            if (_isDisopsed)
            {
                return;
            }

            _isDisopsed = true;
            _isOpen = false;

            _fs?.Dispose();
            _fs = null;
            _sr?.Dispose();
            _sr = null;
        }
    }
}