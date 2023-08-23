namespace MessageBusShared
{
    public enum MessageType
    {
        Unknown = 0,

        //Estimates
        EstimatesCreated = 10000,
        EstimatesUpdated = 10001,
        EstimatesDeleted =10002,
        EstimatesSigned = 10003,
        EstimatesDeclined = 10004,

        //Worksite
        WorksiteCreated = 20000,
        WorksiteUpdated = 20001,
        WorksiteDeleted = 20002,
        WorksiteBegan = 20003,
        WorksiteEnded = 20004,

        //Billing
        BillingCreated = 30000,
        BillingUpdated = 30001,
        BillingDeleted = 30002,
        BillingSent = 30003,
        BillingPaid = 30004,
        BillingDepositSent = 30005,
        BillingDepositPaid = 30006,

    }
}
