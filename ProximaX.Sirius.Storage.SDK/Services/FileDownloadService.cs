﻿using System;
using System.IO;
using System.Reactive.Linq;
using ProximaX.Sirius.Storage.SDK.Connections;
using ProximaX.Sirius.Storage.SDK.PrivacyStrategies;
using ProximaX.Sirius.Storage.SDK.Services.Factories;
using ProximaX.Sirius.Storage.SDK.Services.Repositories;
using ProximaX.Sirius.Storage.SDK.Utils;
using static ProximaX.Sirius.Storage.SDK.Utils.ParameterValidationUtils;

namespace ProximaX.Sirius.Storage.SDK.Services
{
    public class FileDownloadService
    {
        private IFileRepository FileRepository { get; }

        public FileDownloadService(IFileStorageConnection fileStorageConnection)
        {
            FileRepository = FileRepositoryFactory.Create(fileStorageConnection);
        }

        internal FileDownloadService(IFileRepository fileRepository)
        {
            FileRepository = fileRepository;
        }

        public IObservable<Stream> GetByteStream(string dataHash, IPrivacyStrategy privacyStrategy, string digest)
        {
            CheckParameter(dataHash != null, "dataHash is required");

            var privacyStrategyToUse = privacyStrategy ?? PlainPrivacyStrategy.Create();

            ValidateDigest(digest, dataHash);

            return FileRepository.GetByteStream(dataHash).Select(stream => privacyStrategyToUse.DecryptStream(stream));
        }

        private void ValidateDigest(string digest, string dataHash)
        {
            if (digest != null)
            {
                FileRepository.GetByteStream(dataHash)
                    .Select(undecryptedStream => undecryptedStream.ValidateDigest(digest))
                    .Wait();
            }
        }
    }
}