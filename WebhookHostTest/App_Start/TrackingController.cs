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
