using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using piWebAPI0;
using System.Threading.Tasks;
using Sinch.ServerSdk;
using System.Web.Http.Cors;
using System.Web.Configuration;


namespace piWebAPI0.Controllers
{    

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class houseTempsController : ApiController
    {
        // get config values from web.config 
        // these will allow me to change the behavior of the sms portion of the app by hitting 
        // the debugconsole
        public bool smsOn
        {
            get
            {
                return bool.Parse(WebConfigurationManager.AppSettings["smsOn"]);
            }
        }

        public int tempLowValue
        {
            get
            {
                return int.Parse(WebConfigurationManager.AppSettings["tempLowValue"]);
            }
        }

        public int tempHighValue
        {
            get
            {
                return int.Parse(WebConfigurationManager.AppSettings["tempHighValue"]);
            }
        }

        public string phoneNumber
        {
            get
            {
                return (WebConfigurationManager.AppSettings["phoneNumber"]).ToString();
            }
        }


        private tempEntities db = new tempEntities();

        // GET: api/houseTemps
        public IQueryable<houseTemp> GethouseTemps()
        {            
            return db.houseTemps;            
        }

      
      
        // POST: api/houseTemps
        [ResponseType(typeof(houseTemp))]
        public IHttpActionResult PosthouseTemp(houseTemp houseTemp)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.houseTemps.Add(houseTemp);
            db.SaveChanges();

            if (smsOn)
            {
                if ((houseTemp.tempF < tempLowValue) || (houseTemp.tempF > tempHighValue))
                {
                    sendSMS((decimal)houseTemp.tempF);
                }
            }
            return CreatedAtRoute("DefaultApi", new { id = houseTemp.Id }, houseTemp);
        }

      
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool houseTempExists(int id)
        {
            return db.houseTemps.Count(e => e.Id == id) > 0;
        }

        public void sendSMS(decimal fTemp)
        {
            var smsApi = SinchFactory.CreateApiFactory("XXX", "XXXXX").CreateSmsApi();
            var sendSmsResponse = smsApi.Sms(phoneNumber, string.Format("Temp is {0}", fTemp)).Send();
        }
    }
}
