namespace kCura.PDB.Core.Exception
{
	using System;

	public class RetryException : Exception
	{
		public RetryException(Exception ex) 
			: base(ex.Message, ex)
		{
		}
	}
}
