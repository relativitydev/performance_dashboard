namespace kCura.PDD.Web.Factories
{
    using kCura.PDD.Web.Services;

    public class AuthenticationServiceFactory
    {
        public IAuthenticationService GetService()
        {
            var connString = System.Configuration.ConfigurationManager.ConnectionStrings["relativity"];
            if (connString != null)
            {
                return new DevAuthenticationService();
            }
            else
            {
                return new AuthenticationService();
            }
        }
    }
}