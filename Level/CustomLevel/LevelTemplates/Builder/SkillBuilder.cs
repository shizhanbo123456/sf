namespace LevelCreator
{
    public static class SkillBuilder
    {
        public static void Create(string name,string des)
        {

        }
        public static void Create(string name, string des,int cd)
        {

        }
        public static void Create(string name, string des,int cd,int maxStoreTime)
        {

        }
        public static void AddAction(int actionId,int delay)
        {

        }
        public static void Upload()
        {

        }
    }
    public struct SkillInfo:Info
    {
        public string name;
        public string des;
        public int cd;//毫秒
        public int maxStoreTime;
    }
}