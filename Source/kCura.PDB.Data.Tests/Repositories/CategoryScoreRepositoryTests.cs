namespace kCura.PDB.Data.Tests.Repositories
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Repositories;
	using NUnit.Framework;
	using PDB.Tests.Common;

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")] // For TeamCity to ignore this test
	public class CategoryScoreRepositoryTests
	{
		[OneTimeSetUp]
		public async Task OneTimeSetup()
		{
			var hourRepo = new HourRepository(ConnectionFactorySetup.ConnectionFactory);
			var categoryRepo = new CategoryRepository(ConnectionFactorySetup.ConnectionFactory);
			hour = await hourRepo.CreateAsync(new Hour { HourTimeStamp = DateTime.Now });
			category = await categoryRepo.CreateAsync(new Category { HourId = hour.Id, CategoryType = CategoryType.Uptime });

			categoryScoreRepository = new CategoryScoreRepository(ConnectionFactorySetup.ConnectionFactory);
			categoryScore = await categoryScoreRepository.CreateAsync(new CategoryScore { CategoryId = category.Id, Score = 100.0m });
			categoryScore.Score = 75.0m;
			await categoryScoreRepository.UpdateAsync(categoryScore);
			categoryScore = await categoryScoreRepository.ReadAsync(categoryScore.Id);
		}

		private Hour hour;
		private Category category;
		private CategoryScore categoryScore;
		private CategoryScoreRepository categoryScoreRepository;

		[Test]
		public void CategoryScore_CreateAsync_Success()
		{
			//Assert
			Assert.That(categoryScore, Is.Not.Null);
			Assert.That(categoryScore.Id, Is.GreaterThan(0));
			Assert.That(categoryScore.CategoryId, Is.EqualTo(category.Id));
			Assert.That(categoryScore.Score, Is.EqualTo(75.0m));
		}

		[Test]
		public void CategoryScore_ReadAsync_ByID_Success()
		{
			//Assert
			Assert.That(categoryScore, Is.Not.Null);
			Assert.That(categoryScore.Id, Is.GreaterThan(0));
			Assert.That(categoryScore.CategoryId, Is.EqualTo(category.Id));
			Assert.That(categoryScore.Score, Is.EqualTo(75.0m));
		}

		[Test]
		public async Task CategoryScore_ReadAsync_ByHour_Success()
		{
			//Act
			var result = await categoryScoreRepository.ReadAsync(hour);

			//Assert
			Assert.That(result.Any(r => r.Id == categoryScore.Id), Is.True);
		}

		[Test]
		public async Task CategoryScore_ReadAsync_ByCategory_Success()
		{
			//Act
			var result = await categoryScoreRepository.ReadAsync(category);

			//Assert
			Assert.That(result.Any(r => r.Id == categoryScore.Id), Is.True);
		}

		[Test]
		public void CategoryScore_UpdateAsync_Success()
		{
			//Assert
			Assert.That(categoryScore.Score, Is.EqualTo(75.0m));
		}

		[Test]
		public async Task CategoryScore_ZDeleteAsync_Success()
		{
			//Act
			await categoryScoreRepository.DeleteAsync(categoryScore);

			//Assert
			var readResult = await categoryScoreRepository.ReadAsync(categoryScore.Id);
			Assert.That(readResult, Is.Null);
		}
	}
}
