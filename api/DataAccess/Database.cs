namespace api.DataAccess
{
    public class Database
    {
        public string host {get;set;}
        public string database {get;set;}
        public string username {get;set;}
        public string port {get;set;}
        public string password {get;set;}
        public string cs {get;set;}

        public Database()
        {
            host = "lg7j30weuqckmw07.cbetxkdyhwsb.us-east-1.rds.amazonaws.com";
            database = "ybe2foypdhm3r5x0";
            username = "upfdl1blggm67rwx";
            port = "3306";
            password = "thw0mwy5wgd0w2ew";
            cs = $"Server={host};Database={database};User Id={username};Password={password};Port={port}";
        }
    }
}