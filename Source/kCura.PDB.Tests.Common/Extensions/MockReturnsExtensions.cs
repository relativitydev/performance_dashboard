namespace kCura.PDB.Tests.Common.Extensions
{
	using System;
	using System.Threading.Tasks;
	using Moq.Language;
	using Moq.Language.Flow;

	public static class MockReturnsExtensions
	{
		public static IReturnsResult<T> ReturnsAsyncDefault<T>(this ISetup<T, Task> mock)
			where T : class
		{
			return mock.Returns(Task.CompletedTask);
		}
	}
}
