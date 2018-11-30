using IO.Proximax.SDK.Connections;
using IO.Proximax.SDK.Download;
using IO.Proximax.SDK.Models;
using IO.Proximax.SDK.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static IntegrationTests.IntegrationTestConfig;
using static IntegrationTests.TestSupport.Constants;

namespace IntegrationTests.StorageConnection
{
	[TestClass]
	public class DownloaderDirectDownloadStorageConnectionIntegrationTests
	{
		private Downloader UnitUnderTest { get; set; }

		[TestInitialize]
		public void TestInitialize()
		{
			UnitUnderTest = new Downloader(
				ConnectionConfig.CreateWithStorageConnection(
					new BlockchainNetworkConnection(BlockchainNetworkType.MijinTest, BlockchainApiHost, BlockchainApiPort, BlockchainApiProtocol),
					new IO.Proximax.SDK.Connections.StorageConnection(
						StorageNodeApiHost,
						StorageNodeApiPort,
						StorageNodeApiProtocol,
						StorageNodeApiBearer,
						StorageNodeApiNemAddress))
			);
		}

		[TestMethod, Timeout(30000)]
		public void ShouldDirectDownloadFromStorageConnectionUsingTransactionHash() {
			var transactionHash = TestDataRepository
				.GetData("UploaderStorageConnectionIntegrationTests.ShouldUploadToStorageConnection", "transactionHash");
			var param = DirectDownloadParameter.CreateFromTransactionHash(transactionHash).Build();
	
			var result = UnitUnderTest.DirectDownload(param);

			Assert.IsNotNull(result);
			Assert.AreEqual(result.GetContentAsString(), TestString);
		}

		[TestMethod, Timeout(30000)]
		public void ShouldDirectDownloadFromStorageConnectionUsingDataHash() {
			var dataHash = TestDataRepository
				.GetData("UploaderStorageConnectionIntegrationTests.ShouldUploadToStorageConnection", "dataHash");
			var param = DirectDownloadParameter.CreateFromDataHash(dataHash).Build();
	
			var result = UnitUnderTest.DirectDownload(param);

			Assert.IsNotNull(result);
			Assert.AreEqual(result.GetContentAsString(), TestString);
		}

	}
}