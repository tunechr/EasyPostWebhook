using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EasyPost;

namespace WebhookHostTest.App_Start
{
    public class TrackingController : ApiController
    {

        public TrackingController()
        {
            ClientManager.SetCurrent("Ci8PpLVUw47bBfxNaDAyjg");
        }

        // GET: api/Tracking
        public string Get()
        {
            string carrier = "USPS";
            string trackingCode = "EZ1000000001";
            
            Tracker tracker = Tracker.Create(carrier, trackingCode);

            return tracker.tracking_code;
           //Assert.AreEqual(tracker.tracking_code, trackingCode);
           //Assert.IsNotNull(tracker.est_delivery_date);
           //Assert.IsNotNull(tracker.carrier);
           //  Assert.AreEqual(Tracker.Retrieve(tracker.id).id, tracker.id);

        }

        [Route("api/Shipping")]
        [HttpGet]
        public string testShipment()
        {
            Dictionary<string, object> parameters, toAddress, fromAddress;


            toAddress = new Dictionary<string, object>() {
                {"company", "Simpler Postage Inc"}, {"street1", "164 Townsend Street"}, {"street2", "Unit 1"},
                {"city", "San Francisco"}, {"state", "CA"}, {"country", "US"}, {"zip", "94107"},
            };
            fromAddress = new Dictionary<string, object>() {
                {"name", "Andrew Tribone"}, {"street1", "480 Fell St"}, {"street2", "#3"},
                {"city", "San Francisco"}, {"state", "CA"}, {"country", "US"}, {"zip", "94102"}
            };
            parameters = new Dictionary<string, object>() {
                {"parcel", new Dictionary<string, object>() {{"length", 8}, {"width", 6}, {"height", 5}, {"weight", 10}}},
                {"to_address", toAddress}, {"from_address", fromAddress}, {"reference", "ShipmentRef"}
            };
                        
            Shipment shipment = Shipment.Create(parameters);
            
            Shipment retrieved = Shipment.Retrieve(shipment.id);

            return "";
        }

        // GET: api/Tracking/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Tracking
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Tracking/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Tracking/5
        public void Delete(int id)
        {
        }
    }
}
