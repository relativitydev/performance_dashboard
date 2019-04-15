using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kCura.PDB.Service.Integration.Tests
{
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using Moq;
	using Ninject;
	using NUnit.Framework;

	public abstract class BaseServiceIntegrationTest
	{
		[SetUp]
		public void BaseOneTimeSetUp()
		{
			// Init the Kernel + MetricSystem components (Hour, Metrics, MetricDatas, Categories, CategoryScores)
			Kernel = IntegrationSetupFixture.CreateNewKernel();
			Logger = IntegrationSetupFixture.Logger;
			Hour = IntegrationSetupFixture.Hour;
			Metrics = IntegrationSetupFixture.Metrics;
			MetricDatas = IntegrationSetupFixture.MetricDatas;
			Categories = IntegrationSetupFixture.Categories;
			CategoryScores = IntegrationSetupFixture.CategoryScores;
		}

		protected IKernel Kernel;

		protected Mock<ILogger> Logger { get; private set; }

		protected Hour Hour { get; private set; }

		protected IList<Metric> Metrics { get; private set; }

		protected IList<MetricData> MetricDatas { get; private set; }

		protected IList<Category> Categories { get; private set; }

		protected IList<CategoryScore> CategoryScores { get; private set; }
	}
}
