﻿//-----------------------------------------------------------
// <copyright file="StoreApprovalModel.cs" company="adidas AG">
// Copyright (C) 2016 adidas AG.
// </copyright>
//-----------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adidas.clb.job.UpdateTriggering.Models
{
    /// <summary>
    /// class which implements model for store pdf details obj.
    /// </summary>
    class StoreApprovalModel
    {
        public StoreApprovalBasicInfo StoreBasicInformation { get; set; }
        public IEnumerable<StoreExecutiveSummary> StoreSummaryDetails { get; set; }

    }
    class StoreApprovalBasicInfo
    {
        public string DisplayBPMID { get; set; }
        public string RequestName { get; set; }
        public string MarketName { get; set; }
        public string ProjectName { get; set; }
        public string BrandName { get; set; }
        public decimal SecurityDeposit { get; set; }
        public decimal TotalInvestment { get; set; }
        public decimal KeyMoney { get; set; }
        public decimal Brand { get; set; }
        public int CaseID { get; set; }
        public string StoreTypeName { get; set; }
        public int NetSellingSpace { get; set; }
        public Nullable<DateTime> OpeningDate { get; set; }
        public Nullable<DateTime> LeaseEndDate { get; set; }
        public decimal LeasingPeriodDec { get; set; }
        public int CancelPeriod { get; set; }
        public int LeaseBreakOption { get; set; }
        public int CapexSpendYear { get; set; }
        public int GrossLeasedArea { get; set; }
    }
    class StoreExecutiveSummary
    {
        public string LineID { get; set; }
        public string Description { get; set; }
        public string Y0 { get; set; }
        public string Y0Val { get; set; }
        public string Y1 { get; set; }
        public string Y1Val { get; set; }
        public string Y2 { get; set; }
        public string Y2Val { get; set; }
        public string Y3 { get; set; }
        public string Y3Val { get; set; }
        public string Y4 { get; set; }
        public string Y4Val { get; set; }
        public string Y5 { get; set; }
        public string Y5Val { get; set; }
        public string GRSVal { get; set; }
    }
}