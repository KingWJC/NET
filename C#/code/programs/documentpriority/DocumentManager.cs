using System;
using System.Collections.Generic;

namespace code.programs.documentpriority
{
    public class DocumentManager
    {
        private List<LinkedListNode<Document>> _priority;
        private LinkedList<Document> _docList;

        public DocumentManager()
        {
            _docList = new LinkedList<Document>();
            _priority = new List<LinkedListNode<Document>>(10);
            for (int i = 0; i < 10; i++)
            {
                _priority.Add(new LinkedListNode<Document>(null));
            }
        }

        public void Add(Document document)
        {
            if (_priority[document.Priority].Value == null)
            {
                _priority[document.Priority].Value = document;
                _docList.AddLast(document);
            }
        }

        public void DisplayAllNodes()
        {
            foreach (Document doc in _docList)
            {
                Console.WriteLine($"priority: {doc.Priority}, title {doc.Title}");
            }
        }

        // returns the document with the highest priority
        // (that's first in the linked list)
        public Document GetDocument()
        {
            Document doc = _docList.First.Value;
            _docList.RemoveFirst();
            return doc;
        }

    }
}