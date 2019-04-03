using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kCura.PDD.Web.Models
{
    public class AdministrationInstallationModel
    {
        public String DatabaseUsername { get; set; }
        public String DatabasePassword { get; set; }

        //page element toggles
        public Boolean DisplayErrorMessage { get; set; }
        public Boolean DisplaySuccessMessage { get; set; }
        public string ExceptionMessage { get; set; }
        public Boolean DisplayInstallationProgressPane { get; set; }

        //databind information
        public List<InstallationStep> InstallStepList { get; set; }

        public Boolean DisplayScriptInstallationSubmitButton  { get; set; }
        public Boolean DisplayScriptInstallationSubmitMockButton { get; set; }
    }
}