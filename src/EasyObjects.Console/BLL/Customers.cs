namespace EasyObjects.Console.BLL
{
    public class Customers : _Customers
    {
        public Customers() { }

        public Customers(string server, bool useIntegratedSecurity, string userID, string password)
        {
            this.ConnectionServer = server;
            this.UseIntegratedSecurity = useIntegratedSecurity;
            this.ConnectionUserID = userID;
            this.ConnectionPassword = password;
            this.ConnectionDatabase = "Northwind";
        }
    }
}
