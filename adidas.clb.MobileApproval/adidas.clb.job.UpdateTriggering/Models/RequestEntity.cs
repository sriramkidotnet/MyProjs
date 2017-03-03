﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
namespace adidas.clb.job.UpdateTriggering.Models
{
    class RequestEntity : TableEntity
    {
        public RequestEntity(string type, string requsetID)
        {
            this.PartitionKey = type;
            this.RowKey = requsetID;
        }
        public RequestEntity() { }
        public string BackendID { get; set; }
        public string ServiceLayerReqID { get; set; }
        public string ID { get; set; }
        public string Title { get; set; }
        public DateTime Created { get; set; }
        public string Status { get; set; }
        public int Latency { get; set; }
        public string RequesterID { get; set; }
        public string RequesterName { get; set; }
        public int Agentpullingfrequency { get; set; }
        public int Defaultupdatefrequency { get; set; }
        public int Averagerequestsize { get; set; }
        public int Lastrequestsize { get; set; }
        public int Averageallrequestslatency { get; set; }
        public int Lastallrequestslatency { get; set; }
        public int Averagerequestlatency { get; set; }
        public int Lastrequestlatency { get; set; }
        public int Missingconfirmationslimit { get; set; }
        private DateTime? lastUpdate = null;
        public DateTime LastUpdate
        {
            get
            {
                return this.lastUpdate.HasValue ? this.lastUpdate.Value : DateTime.Now;
            }

            set { this.lastUpdate = value; }
        }
        public bool UpdateTriggered { get; set; }
        public string PDFUri { get; set; }
        private DateTime? expectedupdate = null;
        public DateTime? ExpectedUpdate
        {
            get
            {
                return this.expectedupdate.HasValue ? this.expectedupdate.Value : (DateTime?)null;
            }

            set { this.expectedupdate = value; }
        }

    }
}