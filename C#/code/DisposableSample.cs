using System;
using System.IO;
using static System.Console;

namespace code
{
    public class DisposableSample
    {
        public static void Test()
        {
            "托管资源和非托管资源的回收".WriteTemplate();
            UsefulResource resource;
            using (resource = new UsefulResource())
            {
                resource.Foo();
            }
            //Unhandled exception. System.ObjectDisposedException: Cannot access a disposed object.
            //Object name: 'InnerResource'.
            //resource.InnerResource.Foo();
        }
    }

    class InnerResource : IDisposable
    {
        // To detect redundant calls
        private bool disposedValue = false;
        private object[] objArray = new object[10];

        public InnerResource()
        {
            WriteLine("simulation to allocate native memory for inner resource");
        }

        public void Foo()
        {
            if (disposedValue) throw new ObjectDisposedException(nameof(InnerResource));
            WriteLine($"{nameof(InnerResource)}.{nameof(Foo)}");
        }

        public virtual void Disposed(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                    objArray = null;
                }
                WriteLine("simulation to release native memory");
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Disposed(true);
        }
    }

    class UsefulResource : IDisposable
    {
        private bool disposedValue;
        private object[] objList;
        private InnerResource innerresource;
        BinaryReader reader;

        public InnerResource InnerResource => innerresource;

        public UsefulResource()
        {
            WriteLine("simulation to allocate native memory for UsefulResource");
            disposedValue = false;
            innerresource = new InnerResource();
            objList = new object[10];
            reader = new BinaryReader(File.Open("./Program.cs", FileMode.Open));
        }

        public void Foo()
        {
            if (disposedValue) throw new ObjectDisposedException(nameof(UsefulResource));
            WriteLine($"{nameof(UsefulResource)}.{nameof(Foo)}");

            double d = reader.ReadDouble();
            int i = reader.ReadInt32();
            long l = reader.ReadInt64();
            string s = reader.ReadString();
            WriteLine($"d: {d}, i: {i}, l: {l}, s: {s}");
        }

        public virtual void Disposed(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects).
                    objList = null;
                    innerresource?.Dispose();
                }
                // free unmanaged resources (unmanaged objects) and override a finalizer below
                // set large fields to null
                reader = null;

                disposedValue = true;
                WriteLine("simulation to release native memory");
            }
        }

        //override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~UsefulResource()
        {
            Disposed(false);
        }

        public void Dispose()
        {
            Disposed(true);
            GC.SuppressFinalize(this);
        }
    }
}