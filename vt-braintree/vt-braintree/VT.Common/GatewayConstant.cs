using Braintree;

namespace VT.Common
{
    public class GatewayConstant
    {
        public static BraintreeGateway Gateway = new BraintreeGateway
        {
            Environment = ApplicationSettings.IsGatewayLive ? Environment.PRODUCTION : Environment.SANDBOX,
            PublicKey = ApplicationSettings.PublicKey,
            PrivateKey = ApplicationSettings.PrivateKey,
            MerchantId = ApplicationSettings.MerchantId
        };
    }
}
