namespace kCura.PDB.Data.Tests.Repositories
{
	using System;
	using System.Threading.Tasks;
	using Core.Models;
	using Data.Repositories;
	using NUnit.Framework;

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")] // For TeamCity to ignore this test
	public class CategoryRepositoryTests
	{
		[OneTimeSetUp]
		public async Task OneTimeSetup()
		{
			var hourRepo = new HourRepository(ConnectionFactorySetup.ConnectionFactory);
			hour = await hourRepo.CreateAsync(new Hour { HourTimeStamp = DateTime.Now });

			categoryRepository = new CategoryRepository(ConnectionFactorySetup.ConnectionFactory);
			category = await categoryRepository.CreateAsync(new Category { HourId = hour.Id, CategoryType = CategoryType.InfrastructurePerformance });
			category = await categoryRepository.ReadAsync(category.Id);
			category.CategoryType = CategoryType.Uptime;
			await categoryRepository.UpdateAsync(category);
		}

		private Hour hour;
		private Category category;
		private CategoryRepository categoryRepository;

		[Test]
		public void Category_CreateAsync_Success()
		{
			//Assert
			Assert.That(category, Is.Not.Null);
			Assert.That(category.Id, Is.GreaterThan(0));
		}

		[Test]
		public void Category_ReadAsync_ByID_Success()
		{
			//Assert
			Assert.That(category, Is.Not.Null);
			Assert.That(category.Id, Is.GreaterThan(0));
			Assert.That(category.CategoryType, Is.EqualTo(CategoryType.Uptime));
			Assert.That(category.HourId, Is.EqualTo(hour.Id));
		}

		[Test]
		public void Category_UpdateAsync_Success()
		{
			// Assert
			Assert.That(category.CategoryType, Is.EqualTo(CategoryType.Uptime));
		}

		[Test]
		public async Task Category_ZDeleteAsync_Success()
		{
			// Act
			await categoryRepository.DeleteAsync(category);

			// Assert
			var readResult = await categoryRepository.ReadAsync(category.Id);
			Assert.That(readResult, Is.Null);
		}
	}
}
