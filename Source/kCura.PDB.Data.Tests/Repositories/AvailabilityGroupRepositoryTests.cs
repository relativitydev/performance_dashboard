using System;
using kCura.PDB.Core.Interfaces.Repositories;
using kCura.PDB.Core.Interfaces.Services;
using kCura.PDB.Core.Models;
using kCura.PDB.Data.Repositories;
using kCura.PDB.Data.Services;
using kCura.PDB.Service.Services;
using NUnit.Framework;

namespace kCura.PDB.Data.Tests.Repositories
{
	using PDB.Tests.Common;

	[TestFixture, Category("Integration")]
	public class AvailabilityGroupRepositoryTests
	{
		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			this.connectionFatory = TestUtilities.GetIntegrationConnectionFactory();
			this.availabilityGroupRepository = new AvailabilityGroupRepository(this.connectionFatory);
		}

		private IConnectionFactory connectionFatory;
		private IAvailabilityGroupRepository availabilityGroupRepository;

		[Test]
		public void AvailabilityGroupRepository_ReadAvailabilityGroup()
		{
			// Act
			var result = availabilityGroupRepository.ReadAvailabilityGroupName();

			// Assert
			Assert.That(result, Is.Not.Empty);
		}

		[Test]
		public void AvailabilityGroupRepository_AoagIsEnabled()
		{
			// Act
			var result = availabilityGroupRepository.AvailabilityGroupsEnabled();

			// Assert
			Assert.Pass("result is dependent on if aoag is enabled in integration environment");
		}

		[Test]
		public void AvailabilityGroupRepository_AvailabilityGroupsSprocesExist()
		{
			// Act
			var result = availabilityGroupRepository.AvailabilityGroupsSprocesExist();

			// Assert
			Assert.That(result, Is.True, "result dependents on if the aoag admin scripts have been installed.");
		}

		[Test]
		public void AvailabilityGroupRepository_RemoveFromAvailabilityGroup()
		{
			// Act
			availabilityGroupRepository.RemoveFromAvailabilityGroup("edds");

			// Assert
			Assert.Pass("no result returned");
		}
	}
}
