namespace StreamChat.Models
{
    /// <summary>
    /// User privacy settings
    /// </summary>
    public class PrivacySettings
    {
        /// <summary>
        /// Delivery receipts settings
        /// </summary>
        public DeliveryReceipts DeliveryReceipts { get; set; }
    }

    /// <summary>
    /// Delivery receipts configuration
    /// </summary>
    public class DeliveryReceipts
    {
        /// <summary>
        /// Whether delivery receipts are enabled
        /// </summary>
        public bool? Enabled { get; set; }
    }
}