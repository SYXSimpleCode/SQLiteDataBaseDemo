namespace SQLiteConsole_Local.Model
{
    public class User
    {
        public int id { get; set; }
        public string code { get; set; }
        public int orgid { set; get; }
        public string name { get; set; }
        public string gender { get; set; }

        public string birthday { get; set; }

        public int u_validate { get; set; }
        public string u_registtime { get; set; }
    }
}