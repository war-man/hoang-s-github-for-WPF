namespace GroupShared.Business
{
    public static class ConfigHelper
    {
        public const string UserFile = "current_users.json";
        public const string PaymentFile = "current_payments.json";
        //Baseline
        public const string BaselineFolder = "Baselines";
        public const string BaselineUserFile = "{0}_users.json";
        public const string BaselinePaymentFile = "{0}_payments.json";
        //Export
        public const string ExportFolder = "Exports";
        public const string ExportPaymentFile = "{0}_payments.csv";
        public const string ExportUserFile = "{0}_userbalance.csv";
    }
}
