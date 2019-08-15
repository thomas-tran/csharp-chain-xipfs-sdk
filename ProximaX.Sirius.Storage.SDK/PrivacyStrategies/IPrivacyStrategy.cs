﻿using System.IO;

namespace ProximaX.Sirius.Storage.SDK.PrivacyStrategies
{
    public abstract class IPrivacyStrategy
    {
        public abstract int GetPrivacyType();
        public abstract Stream EncryptStream(Stream data);
        public abstract Stream DecryptStream(Stream data);
    }
}