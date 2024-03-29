using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Proximax.Storage.SDK.Connections;
using Proximax.Storage.SDK.Models;
using Proximax.Storage.SDK.Upload;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Proximax.Storage.SDK.Models.Constants;
using static IntegrationTests.IntegrationTestConfig;
using static IntegrationTests.TestDataRepository;
using static IntegrationTests.TestSupport.Constants;
using static IntegrationTests.TestSupport.FileHelper;

namespace IntegrationTests.Upload
{
    [TestClass]
    public class UploaderIntegrationTests
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

        [TestMethod, Timeout(10000)]
        public void ShouldReturnVersion()
        {
            var param = UploadParameter.CreateForStringUpload(
                    TestString, AccountPrivateKey1)
                .Build();

            var result = UnitUnderTest.Upload(param);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Version, SchemaVersion);

            LogAndSaveResult(result, GetType().Name + ".ShouldReturnVersion");
        }

        [TestMethod, Timeout(10000)]
        public void ShouldUploadByteArray()
        {
            var param = UploadParameter.CreateForByteArrayUpload(
                    TestByteArray, AccountPrivateKey1)
                .Build();

            var result = UnitUnderTest.Upload(param);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.TransactionHash);
            Assert.IsNull(result.Data.ContentType);
            Assert.IsNotNull(result.Data.DataHash);
            Assert.IsNull(result.Data.Description);
            Assert.IsNull(result.Data.Name);
            Assert.IsNull(result.Data.Metadata);
            Assert.IsNotNull(result.Data.Timestamp);

            LogAndSaveResult(result, GetType().Name + ".ShouldUploadByteArray");
        }

        [TestMethod, Timeout(10000)]
        public void ShouldUploadByteArrayWithCompleteDetails()
        {
            var param = UploadParameter.CreateForByteArrayUpload(
                    ByteArrayParameterData.Create(TestByteArray, "byte array description", "byte array",
                        "text/plain", new Dictionary<string, string> {{"bytearraykey", "bytearrayval"}}),
                    AccountPrivateKey1)
                .Build();

            var result = UnitUnderTest.Upload(param);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.TransactionHash);
            Assert.AreEqual(result.Data.ContentType, "text/plain");
            Assert.IsNotNull(result.Data.DataHash);
            Assert.AreEqual(result.Data.Description, "byte array description");
            Assert.AreEqual(result.Data.Name, "byte array");
            Assert.AreEqual(result.Data.Metadata.Count, 1);
            Assert.IsFalse(result.Data.Metadata
                .Except(new Dictionary<string, string> {{"bytearraykey", "bytearrayval"}}).Any());
            Assert.IsNotNull(result.Data.Timestamp);

            LogAndSaveResult(result, GetType().Name + ".ShouldUploadByteArrayWithCompleteDetails");
        }

        [TestMethod, Timeout(10000)]
        public void ShouldUploadFile()
        {
            var param = UploadParameter.CreateForFileUpload(TestTextFile, AccountPrivateKey1)
                .Build();

            var result = UnitUnderTest.Upload(param);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.TransactionHash);
            Assert.IsNull(result.Data.ContentType);
            Assert.IsNotNull(result.Data.DataHash);
            Assert.IsNull(result.Data.Description);
            Assert.AreEqual(result.Data.Name, "test_text_file.txt");
            Assert.IsNull(result.Data.Metadata);
            Assert.IsNotNull(result.Data.Timestamp);

            LogAndSaveResult(result, GetType().Name + ".ShouldUploadFile");
        }

        [TestMethod, Timeout(10000)]
        public void ShouldUploadFileWithCompleteDetails()
        {
            var param = UploadParameter.CreateForFileUpload(
                    FileParameterData.Create(TestTextFile, "file description", "file name",
                        "text/plain", new Dictionary<string, string> {{"filekey", "filename"}}),
                    AccountPrivateKey1)
                .Build();

            var result = UnitUnderTest.Upload(param);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.TransactionHash);
            Assert.AreEqual(result.Data.ContentType, "text/plain");
            Assert.IsNotNull(result.Data.DataHash);
            Assert.AreEqual(result.Data.Description, "file description");
            Assert.AreEqual(result.Data.Name, "file name");
            Assert.AreEqual(result.Data.Metadata.Count, 1);
            Assert.IsFalse(result.Data.Metadata.Except(new Dictionary<string, string> {{"filekey", "filename"}}).Any());
            Assert.IsNotNull(result.Data.Timestamp);

            LogAndSaveResult(result, GetType().Name + ".ShouldUploadFileWithCompleteDetails");
        }

        [TestMethod, Timeout(30000)]
        public void ShouldUploadUrlResource()
        {
            var param = UploadParameter.CreateForUrlResourceUpload(
                    FileUrlFromRelativePath(TestImagePngFile), AccountPrivateKey1)
                .Build();

            var result = UnitUnderTest.Upload(param);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.TransactionHash);
            Assert.IsNull(result.Data.ContentType);
            Assert.IsNotNull(result.Data.DataHash);
            Assert.IsNull(result.Data.Description);
            Assert.IsNull(result.Data.Name);
            Assert.IsNull(result.Data.Metadata);
            Assert.IsNotNull(result.Data.Timestamp);

            LogAndSaveResult(result, GetType().Name + ".ShouldUploadUrlResource");
        }

        [TestMethod, Timeout(30000)]
        public void ShouldUploadUrlResourceWithCompleteDetails()
        {
            var param = UploadParameter.CreateForUrlResourceUpload(
                    UrlResourceParameterData.Create(FileUrlFromRelativePath(TestImagePngFile), "url description",
                        "url name", "image/png", new Dictionary<string, string> {{"urlkey", "urlval"}}),
                    AccountPrivateKey1)
                .Build();

            var result = UnitUnderTest.Upload(param);

            Assert.IsNotNull(result.TransactionHash);
            Assert.AreEqual(result.Data.ContentType, "image/png");
            Assert.IsNotNull(result.Data.DataHash);
            Assert.AreEqual(result.Data.Description, "url description");
            Assert.AreEqual(result.Data.Name, "url name");
            Assert.AreEqual(result.Data.Metadata.Count, 1);
            Assert.IsFalse(result.Data.Metadata.Except(new Dictionary<string, string> {{"urlkey", "urlval"}}).Any());
            Assert.IsNotNull(result.Data.Timestamp);

            LogAndSaveResult(result, GetType().Name + ".ShouldUploadUrlResourceWithCompleteDetails");
        }

        [TestMethod, Timeout(30000)]
        public void ShouldUploadStream()
        {
            var param = UploadParameter.CreateForStreamUpload(
                    () => new FileStream(TestTextFile, FileMode.Open, FileAccess.Read), AccountPrivateKey1)
                .Build();

            var result = UnitUnderTest.Upload(param);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.TransactionHash);
            Assert.IsNull(result.Data.ContentType);
            Assert.IsNotNull(result.Data.DataHash);
            Assert.IsNull(result.Data.Description);
            Assert.IsNull(result.Data.Name);
            Assert.IsNull(result.Data.Metadata);
            Assert.IsNotNull(result.Data.Timestamp);

            LogAndSaveResult(result, GetType().Name + ".ShouldUploadStream");
        }

        [TestMethod, Timeout(30000)]
        public void ShouldUploadStreamWithCompleteDetails()
        {
            var param = UploadParameter.CreateForStreamUpload(
                    StreamParameterData.Create(() => new FileStream(TestTextFile, FileMode.Open, FileAccess.Read),
                        "stream description",
                        "stream name", "text/plain", new Dictionary<string, string> {{"streamkey", "streamval"}}),
                    AccountPrivateKey1)
                .Build();

            var result = UnitUnderTest.Upload(param);

            Assert.IsNotNull(result.TransactionHash);
            Assert.AreEqual(result.Data.ContentType, "text/plain");
            Assert.IsNotNull(result.Data.DataHash);
            Assert.AreEqual(result.Data.Description, "stream description");
            Assert.AreEqual(result.Data.Name, "stream name");
            Assert.AreEqual(result.Data.Metadata.Count, 1);
            Assert.IsFalse(result.Data.Metadata.Except(new Dictionary<string, string> {{"streamkey", "streamval"}})
                .Any());
            Assert.IsNotNull(result.Data.Timestamp);

            LogAndSaveResult(result, GetType().Name + ".ShouldUploadStreamWithCompleteDetails");
        }

        [TestMethod, Timeout(10000)]
        public void ShouldUploadFilesAsZip()
        {
            var param = UploadParameter.CreateForFilesAsZipUpload(
                    new List<string> {TestTextFile, TestHtmlFile}, AccountPrivateKey1)
                .Build();

            var result = UnitUnderTest.Upload(param);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.TransactionHash);
            Assert.AreEqual(result.Data.ContentType, "application/zip");
            Assert.IsNotNull(result.Data.DataHash);
            Assert.IsNull(result.Data.Description);
            Assert.IsNull(result.Data.Name);
            Assert.IsNull(result.Data.Metadata);
            Assert.IsNotNull(result.Data.Timestamp);

            LogAndSaveResult(result, GetType().Name + ".ShouldUploadFilesAsZip");
        }

        [TestMethod, Timeout(10000)]
        public void ShouldUploadFilesAsZipWithCompleteDetails()
        {
            var param = UploadParameter.CreateForFilesAsZipUpload(
                    FilesAsZipParameterData.Create(new List<string> {TestTextFile, TestHtmlFile}, "zip description",
                        "zip name", new Dictionary<string, string> {{"zipkey", "zipvalue"}}),
                    AccountPrivateKey1)
                .Build();

            var result = UnitUnderTest.Upload(param);

            Assert.IsNotNull(result.TransactionHash);
            Assert.AreEqual(result.Data.ContentType, "application/zip");
            Assert.IsNotNull(result.Data.DataHash);
            Assert.AreEqual(result.Data.Description, "zip description");
            Assert.AreEqual(result.Data.Name, "zip name");
            Assert.AreEqual(result.Data.Metadata.Count, 1);
            Assert.IsFalse(result.Data.Metadata.Except(new Dictionary<string, string> {{"zipkey", "zipvalue"}}).Any());
            Assert.IsNotNull(result.Data.Timestamp);

            LogAndSaveResult(result, GetType().Name + ".ShouldUploadFilesAsZipWithCompleteDetails");
        }

        [TestMethod, Timeout(10000)]
        public void ShouldUploadString()
        {
            var param = UploadParameter.CreateForStringUpload(TestString, AccountPrivateKey1)
                .Build();

            var result = UnitUnderTest.Upload(param);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.TransactionHash);
            Assert.IsNull(result.Data.ContentType);
            Assert.IsNotNull(result.Data.DataHash);
            Assert.IsNull(result.Data.Description);
            Assert.IsNull(result.Data.Name);
            Assert.IsNull(result.Data.Metadata);
            Assert.IsNotNull(result.Data.Timestamp);

            LogAndSaveResult(result, GetType().Name + ".ShouldUploadString");
        }

        [TestMethod, Timeout(10000)]
        public void ShouldUploadStringWithCompleteDetails()
        {
            var param = UploadParameter.CreateForStringUpload(
                    StringParameterData.Create(TestString, Encoding.UTF8, "string description", "string name",
                        "text/plain", new Dictionary<string, string> {{"keystring", "valstring"}}),
                    AccountPrivateKey1)
                .Build();

            var result = UnitUnderTest.Upload(param);

            Assert.IsNotNull(result.TransactionHash);
            Assert.AreEqual(result.Data.ContentType, "text/plain");
            Assert.IsNotNull(result.Data.DataHash);
            Assert.AreEqual(result.Data.Description, "string description");
            Assert.AreEqual(result.Data.Name, "string name");
            Assert.AreEqual(result.Data.Metadata.Count, 1);
            Assert.IsFalse(result.Data.Metadata.Except(new Dictionary<string, string> {{"keystring", "valstring"}})
                .Any());
            Assert.IsNotNull(result.Data.Timestamp);

            LogAndSaveResult(result, GetType().Name + ".ShouldUploadStringWithCompleteDetails");
        }

        [TestMethod, Timeout(10000)]
        public void ShouldUploadPath()
        {
            var param = UploadParameter.CreateForPathUpload(TestPathFile, AccountPrivateKey1)
                .Build();

            var result = UnitUnderTest.Upload(param);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.TransactionHash);
            Assert.AreEqual(result.Data.ContentType, PathUploadContentType);
            Assert.IsNotNull(result.Data.DataHash);
            Assert.IsNull(result.Data.Description);
            Assert.IsNull(result.Data.Name);
            Assert.IsNull(result.Data.Metadata);
            Assert.IsNotNull(result.Data.Timestamp);

            LogAndSaveResult(result, GetType().Name + ".ShouldUploadPath");
        }

        [TestMethod, Timeout(10000)]
        public void ShouldUploadPathWithCompleteDetails()
        {
            var param = UploadParameter.CreateForPathUpload(
                    PathParameterData.Create(TestPathFile, "path description", "path name",
                        new Dictionary<string, string> {{"pathkey", "pathval"}}),
                    AccountPrivateKey1)
                .Build();

            var result = UnitUnderTest.Upload(param);

            Assert.IsNotNull(result.TransactionHash);
            Assert.AreEqual(result.Data.ContentType, PathUploadContentType);
            Assert.IsNotNull(result.Data.DataHash);
            Assert.AreEqual(result.Data.Description, "path description");
            Assert.AreEqual(result.Data.Name, "path name");
            Assert.AreEqual(result.Data.Metadata.Count, 1);
            Assert.IsFalse(result.Data.Metadata.Except(new Dictionary<string, string> {{"pathkey", "pathval"}}).Any());
            Assert.IsNotNull(result.Data.Timestamp);

            LogAndSaveResult(result, GetType().Name + ".ShouldUploadPathWithCompleteDetails");
        }
    }
}