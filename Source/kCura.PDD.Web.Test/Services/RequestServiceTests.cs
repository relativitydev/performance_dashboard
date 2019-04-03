namespace kCura.PDD.Web.Test.Services
{
	using System.Net.Http;
	using kCura.PDD.Web.Services;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class RequestServiceTests
	{
		private RequestService requestService;

		public enum HttpMethodEnum
		{
			Get,
			Post
		}

		private HttpMethod MethodFromEnum(HttpMethodEnum httpMethodEnum)
		{
			switch (httpMethodEnum)
			{
				case HttpMethodEnum.Get:
					return HttpMethod.Get;
				case HttpMethodEnum.Post:
					return HttpMethod.Post;
				default:
					return HttpMethod.Get;
			}
		}

		[SetUp]
		public void SetUp()
		{
			this.requestService = new RequestService();
		}

		[Test, Explicit("TODO")]
		[TestCase(
			 "?TimezoneOffset=-300&StartDate=8/25/2017&EndDate=8/25/2017&ServerArtifactId=&ServerSelection=&sEcho=1&iColumns=11&sColumns=%2C%2C%2C%2C%2C%2C%2C%2C%2C%2C&iDisplayStart=0&iDisplayLength=10&mDataProp_0=0&sSearch_0=&sOperand_0=&bRegex_0=false&bSearchable_0=true&bSortable_0=true&mDataProp_1=1&sSearch_1=&sOperand_1=&bRegex_1=false&bSearchable_1=true&bSortable_1=true&mDataProp_2=2&sSearch_2=&sOperand_2=&bRegex_2=false&bSearchable_2=true&bSortable_2=true&mDataProp_3=3&sSearch_3=&sOperand_3=&bRegex_3=false&bSearchable_3=true&bSortable_3=true&mDataProp_4=4&sSearch_4=&sOperand_4=&bRegex_4=false&bSearchable_4=true&bSortable_4=true&mDataProp_5=5&sSearch_5=&sOperand_5=&bRegex_5=false&bSearchable_5=true&bSortable_5=true&mDataProp_6=6&sSearch_6=&sOperand_6=&bRegex_6=false&bSearchable_6=true&bSortable_6=true&mDataProp_7=7&sSearch_7=&sOperand_7=&bRegex_7=false&bSearchable_7=true&bSortable_7=true&mDataProp_8=8&sSearch_8=&sOperand_8=&bRegex_8=false&bSearchable_8=true&bSortable_8=true&mDataProp_9=9&sSearch_9=&sOperand_9=&bRegex_9=false&bSearchable_9=true&bSortable_9=true&mDataProp_10=10&sSearch_10=&sOperand_10=&bRegex_10=false&bSearchable_10=true&bSortable_10=true&sSearch=&bRegex=false&iSortingCols=0&_=1503690745063",
			HttpMethodEnum.Get
		 )]
		public void GetQueryParams(string requestUrl, HttpMethodEnum httpMethodEnum)
		{
			// Arrange
			var httpMethod = this.MethodFromEnum(httpMethodEnum);
			var requestMessage = new HttpRequestMessage(httpMethod, requestUrl);

			// Act
			var result = this.requestService.GetQueryParams(requestMessage);

			// Assert
		}

		[Test, Explicit("TODO")]
		public void GetQueryParamsDecoded(string requestUrl, HttpMethodEnum httpMethodEnum)
		{
			// Arrange
			var httpMethod = this.MethodFromEnum(httpMethodEnum);
			var requestMessage = new HttpRequestMessage(httpMethod, requestUrl);

			// Act
			var result = this.requestService.GetQueryParamsDecoded(requestMessage);

			// Assert
		}
	}
}
