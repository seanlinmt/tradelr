using System;
using Shipping.WebReference;

namespace Shipping
{
    public class FedEx
    {

        private string m_accountNumber;
        private string m_meterNumber;
        private string m_serviceKey;
        private string m_servicePassword;
        

        public FedEx(string accountNumber, string meterNumber, string serviceKey, string servicePassword)
        {
            m_accountNumber = accountNumber;
            m_meterNumber = meterNumber;
            m_serviceKey = serviceKey;
            m_servicePassword = servicePassword;
            
        }

        public decimal GetRate(string weight, string shipperZipCode, string shipperCountryCode, string destinationZipCode, string destinationCountryCode, string serviceCode)
        {

            decimal rate = 0;
            // Build the RateRequest
            RateRequest request = new RateRequest();
            //
            request.WebAuthenticationDetail = new WebAuthenticationDetail();
            request.WebAuthenticationDetail.UserCredential = new WebAuthenticationCredential();
            request.WebAuthenticationDetail.UserCredential.Key = m_serviceKey; // Replace "XXX" with the Key
            request.WebAuthenticationDetail.UserCredential.Password = m_servicePassword; // Replace "XXX" with the Password
            //
            request.ClientDetail = new ClientDetail();
            request.ClientDetail.AccountNumber = m_accountNumber; // Replace "XXX" with client's account number
            request.ClientDetail.MeterNumber = m_meterNumber; // Replace "XXX" with client's meter number
            //
            request.TransactionDetail = new TransactionDetail();
            request.TransactionDetail.CustomerTransactionId = "***Rate v7 Request using VC#***"; // This is a reference field for the customer.  Any value can be used and will be provided in the response.
            //
            request.Version = new VersionId(); // WSDL version information, value is automatically set from wsdl            
            //
            request.ReturnTransitAndCommit = true;
            request.ReturnTransitAndCommitSpecified = true;
            request.CarrierCodes = new CarrierCodeType[1];
            request.CarrierCodes[0] = CarrierCodeType.FDXE;

            //set the shipment details
            SetShipmentDetails(request, serviceCode);
            //Set the Origin
            SetOrigin(request, shipperZipCode, shipperCountryCode);
            //Set the Destination
            SetDestination(request, destinationZipCode, destinationCountryCode);

            //Set the Payment
            SetPayment(request);

            //Set the Summary
            SetSummaryPackageLineItems(request, weight);

            RateService service = new RateService(); // Initialize the service
            RateReply reply = service.getRates(request);
            if (reply.HighestSeverity == NotificationSeverityType.SUCCESS || reply.HighestSeverity == NotificationSeverityType.NOTE || reply.HighestSeverity == NotificationSeverityType.WARNING) // check if the call was successful
            {
                rate = reply.RateReplyDetails[0].RatedShipmentDetails[0].ShipmentRateDetail.TotalNetCharge.Amount;
            }

            return rate;
            
        }

        private  void SetShipmentDetails(RateRequest request, string serviceType)
        {
            
            
            request.RequestedShipment = new RequestedShipment();
            request.RequestedShipment.DropoffType = DropoffType.REGULAR_PICKUP; //Drop off types are BUSINESS_SERVICE_CENTER, DROP_BOX, REGULAR_PICKUP, REQUEST_COURIER, STATION
            switch (serviceType)
            {
                case "FEDEX_GROUND":
                    request.RequestedShipment.ServiceType = ServiceType.FEDEX_GROUND; // Service types are STANDARD_OVERNIGHT, PRIORITY_OVERNIGHT, FEDEX_GROUND ...
                    break;
                case "FEDEX_2_DAY":
                    request.RequestedShipment.ServiceType = ServiceType.FEDEX_2_DAY; // Service types are STANDARD_OVERNIGHT, PRIORITY_OVERNIGHT, FEDEX_GROUND ...
                    break;
                case "FEDEX_EXPRESS_SAVER":
                    request.RequestedShipment.ServiceType = ServiceType.FEDEX_EXPRESS_SAVER; // Service types are STANDARD_OVERNIGHT, PRIORITY_OVERNIGHT, FEDEX_GROUND ...
                    break;
                case "STANDARD_OVERNIGHT":
                    request.RequestedShipment.ServiceType = ServiceType.STANDARD_OVERNIGHT; // Service types are STANDARD_OVERNIGHT, PRIORITY_OVERNIGHT, FEDEX_GROUND ...
                    break;
                case "PRIORITY_OVERNIGHT":
                    request.RequestedShipment.ServiceType = ServiceType.PRIORITY_OVERNIGHT; // Service types are STANDARD_OVERNIGHT, PRIORITY_OVERNIGHT, FEDEX_GROUND ...
                    break;
                case "GROUND_HOME_DELIVERY":
                    request.RequestedShipment.ServiceType = ServiceType.GROUND_HOME_DELIVERY;
                    break;
                case "INTERNATIONAL_ECONOMY":
                    request.RequestedShipment.ServiceType = ServiceType.INTERNATIONAL_ECONOMY;
                    break;
                case "INTERNATIONAL_FIRST":
                    request.RequestedShipment.ServiceType = ServiceType.INTERNATIONAL_FIRST;
                    break;
                case "INTERNATIONAL_PRIORITY":
                    request.RequestedShipment.ServiceType = ServiceType.INTERNATIONAL_PRIORITY;
                    break;
                case "FIRST_OVERNIGHT" :
                    request.RequestedShipment.ServiceType = ServiceType.FIRST_OVERNIGHT;
                    break;
                default:
                    request.RequestedShipment.ServiceType = ServiceType.FEDEX_2_DAY; // Service types are STANDARD_OVERNIGHT, PRIORITY_OVERNIGHT, FEDEX_GROUND ...
                    break;
            }
            
            
            
            request.RequestedShipment.ServiceTypeSpecified = true;
            request.RequestedShipment.PackagingType = PackagingType.YOUR_PACKAGING; // Packaging type FEDEX_BOK, FEDEX_PAK, FEDEX_TUBE, YOUR_PACKAGING, ...
            request.RequestedShipment.PackagingTypeSpecified = true;
            //
            request.RequestedShipment.RateRequestTypes = new RateRequestType[1];
            request.RequestedShipment.RateRequestTypes[0] = RateRequestType.ACCOUNT;
        }

        private  void SetPayment(RateRequest request)
        {
            request.RequestedShipment.ShippingChargesPayment = new Payment();
            request.RequestedShipment.ShippingChargesPayment.PaymentType = PaymentType.SENDER; // Payment options are RECIPIENT, SENDER, THIRD_PARTY
            request.RequestedShipment.ShippingChargesPayment.PaymentTypeSpecified = true;
            request.RequestedShipment.ShippingChargesPayment.Payor = new Payor();
            request.RequestedShipment.ShippingChargesPayment.Payor.AccountNumber = m_accountNumber ; // Replace "XXX" with client's account number
        }

        private void SetOrigin(RateRequest request, string shipperZipCode, string shipperCountryCode)
        {
            request.RequestedShipment.Shipper = new Party();
            request.RequestedShipment.Shipper.Address = new Address();
            request.RequestedShipment.Shipper.Address.PostalCode = shipperZipCode;
            request.RequestedShipment.Shipper.Address.CountryCode = shipperCountryCode;
            request.RequestedShipment.Shipper.AccountNumber = m_accountNumber;

        }

        private  void SetDestination(RateRequest request, string destinationZipCode, string destinationCountryCode)
        {
            request.RequestedShipment.Recipient = new Party();
            request.RequestedShipment.Recipient.Address = new Address();
            request.RequestedShipment.Recipient.Address.PostalCode = destinationZipCode;
            request.RequestedShipment.Recipient.Address.CountryCode = destinationCountryCode;

        }

        private  void SetSummaryPackageLineItems(RateRequest request, string weight)
        {
            // -----------------------------------------
            // Passing multi piece shipment rate request
            // -----------------------------------------
            request.RequestedShipment.TotalWeight = new Weight();
            request.RequestedShipment.TotalWeight.Value = Convert.ToDecimal(weight);
            request.RequestedShipment.TotalWeight.Units = WeightUnits.LB;
            //
            request.RequestedShipment.PackageCount = "1";
            //
            request.RequestedShipment.RequestedPackageLineItems = new RequestedPackageLineItem[1];
            request.RequestedShipment.RequestedPackageLineItems[0] = new RequestedPackageLineItem();
            //request.RequestedShipment.RequestedPackageLineItems[0].Dimensions = new Dimensions(); // package dimensions, applies to each package 
            //request.RequestedShipment.RequestedPackageLineItems[0].Dimensions.Length = "10";
            //request.RequestedShipment.RequestedPackageLineItems[0].Dimensions.Width = "10";
            //request.RequestedShipment.RequestedPackageLineItems[0].Dimensions.Height = "3";
            //request.RequestedShipment.RequestedPackageLineItems[0].Dimensions.Units = LinearUnits.IN;
            request.RequestedShipment.RequestedPackageLineItems[0].Weight = request.RequestedShipment.TotalWeight;

        }

    }
}
