using System;
using System.Threading;
using System.Threading.Tasks;

namespace code.programs.documentconsume
{
    public class ProcessDocument
    {
        public static void Start(DocumentManager dm)
        {
            Task.Run(new ProcessDocument(dm).Run);
        }

        private DocumentManager _documentManager;

        private ProcessDocument(DocumentManager documentManager)
        {
            if (documentManager == null)
                throw new ArgumentNullException(nameof(documentManager));
            _documentManager = documentManager;
        }

        /*
         * @description:  异步执行方法，每次循环都会进行等待。相当于 Thread.Sleep(new Random().Next(20));
         * @param {*}
         * @return {*}
         */        
        public async Task Run()
        {
            while (true)
            {
                if (_documentManager.IsAvailable)
                {
                    var doc = _documentManager.Get();
                    Console.WriteLine($"Processing {doc}");
                }
                await Task.Delay(new Random().Next(20));
            }
        }
    }
}