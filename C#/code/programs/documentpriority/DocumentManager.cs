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
            AddPriority(document, document.Priority);
        }

        private void AddPriority(Document document, int priority)
        {
            if (_priority[priority].Value == null)
            {
                --priority;
                if (priority >= 0)
                {
                    AddPriority(document, priority);
                }
                else
                {
                    var node = _docList.AddLast(document);
                    _priority[document.Priority] = node;
                }
                return;
            }
            else
            {
                LinkedListNode<Document> preNode = _priority[priority];
                if (priority == document.Priority)
                {
                    _docList.AddAfter(preNode, document);
                    _priority[priority] = preNode.Next;
                }
                else
                {
                    while (preNode.Previous != null && preNode.Value.Priority == preNode.Previous.Value.Priority)
                    {
                        preNode = preNode.Previous;
                    }
                    _priority[document.Priority] = _docList.AddBefore(preNode, document);
                }
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