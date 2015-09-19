using System;

namespace Chaos.Raven
{
    public static class Constants
    {
        public const int LargeBatchSize = 1024 * 2;
        public const int MediumBatchSize = 512;
        public const int SmallBatchSize = 128;

        public const string ActionsPluginFolder = @".\Actions";

        public const int ActionDispatchFrequencyInMilliseconds = 250;

        public const int LoadGenerationDuration = 200;

        public static readonly int MaxConcurrentActions = Environment.ProcessorCount * 2;

        public static readonly int NumOfActionsPerDispatch = Environment.ProcessorCount;

        public const int NumOfEmployees = 100;
        public const int NumOfCompanies = 250;
        public const int NumOfOrders = 500;
        public const int NumOfSuppliers = 150;
        public const int NumOfShippers = 50;
        public const int NumOfRegions = 10;
        public const int NumOfTerritories = 10;
        public const int NumOfCategories = 10;
        public const int NumOfProducts = 200;
    }
}
