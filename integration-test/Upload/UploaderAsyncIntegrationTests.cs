using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Proximax.Storage.SDK.Async;
using Proximax.Storage.SDK.Connections;
using Proximax.Storage.SDK.Models;
using Proximax.Storage.SDK.PrivacyStrategies;
using Proximax.Storage.SDK.Upload;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static IntegrationTests.IntegrationTestConfig;
using static IntegrationTests.TestSupport.Constants;

namespace IntegrationTests.Upload
{
    [TestClass]
    public class UploaderAsyncIntegrationTests
    {
        private Uploader UnitUnderTest { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            UnitUnderTest = new Uploader(
                ConnectionConfig.CreateWithLocalIpfsConnection(
                    new BlockchainNetworkConnection(BlockchainNetworkType.MijinTest, BlockchainApiHost,
                        BlockchainApiPort, BlockchainApiProtocol),
                    new IpfsConnection(IpfsApiHost, IpfsApiPort, BlockchainApiProtocol))
            );
        }

        [TestMethod, Timeout(30000)]
        public void ShouldUploadAsynchronouslyWithoutCallback()
        {
            var param = UploadParameter
                .CreateForFileUpload(TestPdfFile2, AccountPrivateKey1)
                .Build();

            var asyncTask = UnitUnderTest.UploadAsync(param, null);
            while (!asyncTask.IsDone())
            {
                Thread.Sleep(50);
            }

            Assert.IsTrue(asyncTask.IsDone());
        }

        [TestMethod, Timeout(30000)]
        public void ShouldUploadAsynchronouslyWithSuccessCallback()
        {
            var param = UploadParameter
                .CreateForFileUpload(TestTextFile, AccountPrivateKey1)
                .Build();
            var taskCompletionSource = new TaskCompletionSource<UploadResult>();
            var asyncCallbacks = AsyncCallbacks<UploadResult>.Create<UploadResult>(
                uploadResult => taskCompletionSource.SetResult(uploadResult), null);

            UnitUnderTest.UploadAsync(param, asyncCallbacks);
            taskCompletionSource.Task.Wait(5000);

            var result = taskCompletionSource.Task.Result;
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.TransactionHash);
        }

        [TestMethod, Timeout(10000)]
        public void ShouldUploadAsynchronouslyWithFailureCallback()
        {
            var param = UploadParameter
                .CreateForFileUpload(TestTextFile, AccountPrivateKey1)
                .WithPrivacyStrategy(new NotImplementedPrivacyStrategy())
                .Build();
            var taskCompletionSource = new TaskCompletionSource<Exception>();
            var asyncCallbacks = AsyncCallbacks<UploadResult>.Create<UploadResult>(
                null, ex => taskCompletionSource.SetResult(ex));

            UnitUnderTest.UploadAsync(param, asyncCallbacks);
            taskCompletionSource.Task.Wait(5000);

            var exception = taskCompletionSource.Task.Result;

            Assert.IsInstanceOfType(exception, exception.GetType());
        }

        private class NotImplementedPrivacyStrategy : ICustomPrivacyStrategy
        {
            public override Stream EncryptStream(Stream data)
            {
                throw new NotImplementedException();
            }

            public override Stream DecryptStream(Stream data)
            {
                throw new NotImplementedException();
            }
        }
    }
}