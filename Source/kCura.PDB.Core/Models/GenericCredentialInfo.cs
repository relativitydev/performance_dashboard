namespace kCura.PDB.Core.Models
{
	using System;

	[Serializable]
    public class GenericCredentialInfo
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool UseWindowsAuthentication { get; set; }
    }
}
