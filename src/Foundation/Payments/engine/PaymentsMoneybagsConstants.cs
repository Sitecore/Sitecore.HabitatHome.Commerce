namespace Sitecore.HabitatHome.Foundation.Payments.Engine
{
    /// <summary>
    /// The payments constants.
    /// </summary>
    public static class PaymentsMoneybagsConstants
    {
        /// <summary>
        /// The name of the payments pipelines.
        /// </summary>
        public static class Pipelines
        {
            /// <summary>
            /// The name of the payment pipelines blocks.
            /// </summary>
            public static class Blocks
            {
                /// <summary>
                /// The get client token block name.
                /// </summary>
                public const string GetClientTokenBlock = "PaymentsMoneybags.block.getclienttoken";

                /// <summary>
                /// The add federated payment block
                /// </summary>
                public const string UpdateFederatedPaymentBlock = "PaymentsMoneybags.block.updatefederatedpayment";

                /// <summary>
                /// The update order after federated payment settlement block name
                /// </summary>
                public const string UpdateOrderAfterFederatedPaymentSettlementBlock = "PaymentsMoneybags.block.UpdateOrderAfterFederatedPaymentSettlement";

                /// <summary>
                /// The create federated payment block
                /// </summary>
                public const string CreateFederatedPaymentBlock = "PaymentsMoneybags.block.createfederatedpayment";

                /// <summary>
                /// The settle federated payment block name
                /// </summary>
                public const string SettleFederatedPaymentBlock = "PaymentsMoneybags.block.SettleFederatedPayment";

                /// <summary>
                /// The void federated payment block
                /// </summary>
                public const string VoidFederatedPaymentBlock = "PaymentsMoneybags.block.voidfederatedpayment";

                /// <summary>
                /// The void cancel order federated payment block
                /// </summary>
                public const string VoidCancelOrderFederatedPaymentBlock = "PaymentsMoneybags.block.voidcancelorderfederatedpayment";

                /// <summary>
                /// The refund federated payment block
                /// </summary>
                public const string RefundFederatedPaymentBlock = "PaymentsMoneybags.block.refundfederatedpayment";             

                /// <summary>
                /// The void on hold order federated payment block
                /// </summary>
                public const string VoidOnHoldOrderFederatedPaymentBlock = "PaymentsMoneybags.block.voidonholdorderfederatedpayment";

                /// <summary>
                /// The registered plugin block name.
                /// </summary>
                public const string RegisteredPluginBlock = "PaymentsMoneybags.block.RegisteredPlugin";

                /// <summary>
                /// Settle order sales activities block name.
                /// </summary>
                public const string SettleOrderSalesActivitiesBlock = "PaymentsMoneybags.block.SettleOrderSalesActivities";
            }
        }
    }
}
