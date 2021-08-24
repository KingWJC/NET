/*
 * @Author: KingWJC
 * @Date: 2021-08-20 09:22:29
 * @LastEditors: KingWJC
 * @LastEditTime: 2021-08-24 10:46:51
 * @Descripttion: 
 * @FilePath: \code\programs\documentpriority\Client.cs
 * 
 * 集合：链表和列表的示例， 列表管理优先级，链表管理文档，按优先级插入文档，然后遍历
 *
 */
namespace code.programs.documentpriority
{
    public class Client
    {
        public static void Test()
        {
            DocumentManager pdm = new DocumentManager();
            pdm.Add(new Document("one", "Sample", 8));
            pdm.Add(new Document("two", "Sample", 3));
            pdm.Add(new Document("three", "Sample", 4));
            pdm.Add(new Document("four", "Sample", 8));
            pdm.Add(new Document("five", "Sample", 1));
            pdm.Add(new Document("six", "Sample", 9));
            pdm.Add(new Document("seven", "Sample", 1));
            pdm.Add(new Document("eight", "Sample", 1));

            pdm.DisplayAllNodes();
        }
    }
}