﻿using System;
using System.IO;
using System.Reactive.Linq;
using IO.Proximax.SDK.PrivacyStrategies;
using IO.Proximax.SDK.Services.Repositories;
using IO.Proximax.SDK.Utils;
using static IO.Proximax.SDK.Utils.ParameterValidationUtils;

namespace IO.Proximax.SDK.Services
{
    public class FileUploadService
    {
        private IFileRepository FileRepository { get; }

        public FileUploadService(IFileRepository fileRepository)
        {
            FileRepository = fileRepository;
        }

        public IObservable<FileUploadResponse> UploadByteStream(Func<Stream> byteStreamFunction,
            IPrivacyStrategy privacyStrategy, bool? computeDigest)
        {
            CheckParameter(byteStreamFunction != null, "byteStreamFunction is required");

            var computeDigestToUse = computeDigest ?? false;
            var privacyStrategyToUse = privacyStrategy ?? PlainPrivacyStrategy.Create();

            var digest = ComputeDigest(byteStreamFunction, privacyStrategyToUse, computeDigestToUse);
            var dataHashOb =
                FileRepository.AddByteStream(privacyStrategyToUse.EncryptStream(byteStreamFunction?.Invoke()));

            return dataHashOb.Select(dataHash => new FileUploadResponse(dataHash, CurrentTimeMillis(), digest));
        }

        public IObservable<FileUploadResponse> UploadPath(string path)
        {
            CheckParameter(path != null, "path is required");

            return FileRepository.AddPath(path)
                .Select(dataHash => new FileUploadResponse(dataHash, CurrentTimeMillis()));
        }

        private string ComputeDigest(Func<Stream> byteStreamFunction,
            IPrivacyStrategy privacyStrategy, bool computeDigest)
        {
            return computeDigest
                ? privacyStrategy.EncryptStream(byteStreamFunction.Invoke()).Digest()
                : null;
        }

        private long CurrentTimeMillis()
        {
            return (long) (DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }
    }
}