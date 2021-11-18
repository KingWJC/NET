namespace code.synchronization
{
    public class RaceCondition
    {
        public void Test()
        {

        }
    }

    private class StateObject
    {
        private int count;

        public void ChangeState()
        {
            count++;
        }
    }
}