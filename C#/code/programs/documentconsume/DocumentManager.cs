using System.Collections.Generic;

namespace code.programs.documentconsume
{
    public class DocumentManager
    {
        private Queue<Document> _docList = new Queue<Document>();

        public void Add(Document document)
        {
            lock (this)
            {
                _docList.Enqueue(document);
            }
        }

        public Document Get()
        {
            lock (this)
            {
                return _docList.Dequeue();
            }
        }

        public bool IsAvailable => _docList.Count > 0;
    }
}