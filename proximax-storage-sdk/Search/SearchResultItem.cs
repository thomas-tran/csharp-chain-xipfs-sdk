﻿using Proximax.Storage.SDK.Models;

namespace Proximax.Storage.SDK.Search
{
    public class SearchResultItem
    {
        public string TransactionHash { get; }
        public string TransactionId { get; }
        public ProximaxMessagePayloadModel MessagePayload { get; }

        public SearchResultItem(string transactionHash, string transactionId,
            ProximaxMessagePayloadModel messagePayload)
        {
            TransactionHash = transactionHash;
            TransactionId = transactionId;
            MessagePayload = messagePayload;
        }
    }
}