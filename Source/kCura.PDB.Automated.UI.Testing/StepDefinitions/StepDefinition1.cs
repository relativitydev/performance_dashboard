using kCura.PDB.Automated.UI.Testing.PageModels.Relativity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace kCura.PDB.Automated.UI.Testing.Gherkin
{
    [Binding]
    public sealed class StepDefinition1
    {
        // For additional details on SpecFlow step definitions see http://go.specflow.org/doc-stepdef

        RelativityLoginPage loginpage = new RelativityLoginPage();

        [Given(@"I have access to Relativity\.")]
        public void GivenIHaveAccessToRelativity_()
        {
            loginpage.NavigateToRelativityLogin();
        }

        [When(@"I enter Relativity Admin Credentials to the Login Page\.")]
        public void WhenIEnterRelativityAdminCredentialsToTheLoginPage_()
        {
            loginpage.EnterRelativityLoginCredentials();
        }

        [Then(@"I will be logged into Relativity\.")]
        public void ThenIWillBeLoggedIntoRelativity_()
        {
            loginpage.VerifyRelativityLogin();
        }

    }
}

