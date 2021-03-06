﻿using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Infrastructure;
using Model.Models;
using Core.Common;
using System;
using Model;
using System.Threading.Tasks;
using Data.Models;
using Jobs.Constants.Menu;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using Model.ViewModels;
using Model.ViewModel;

namespace Service
{
    public interface ILedgerWizardService : IDisposable
    {
        IEnumerable<LedgerWizardResultViewModel> GetLedgerDetail(LedgerWizardViewModel vm);
    }

    public class LedgerWizardService : ILedgerWizardService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        string mQry = "";
        string connectionString = (string)System.Web.HttpContext.Current.Session["DefaultConnectionString"];

        public LedgerWizardService()
        {
        }

        public IEnumerable<LedgerWizardResultViewModel> GetLedgerDetail(LedgerWizardViewModel vm)
        {
            if (vm.LedgerHeaderId == 0)
            {

                SqlParameter SqlParameterDocDate = new SqlParameter("@DocDate", vm.DocDate.ToString());
                SqlParameter SqlParameterDocTypeId = new SqlParameter("@DocTypeId", vm.DocTypeId);
                SqlParameter SqlParameterHeaderLedgerAccountId = new SqlParameter("@HeaderLedgerAccountId", vm.HeaderLedgerAccountId);
                SqlParameter SqlParameterRemark = new SqlParameter("@Remark", (vm.Remark == null) ? DBNull.Value : (object)vm.Remark);


                SqlParameter SqlParameterDepartmentId = new SqlParameter("@DepartmentId", (vm.DepartmentId == null) ? DBNull.Value : (object)vm.DepartmentId);
                SqlParameter SqlParameterWagesPayType = new SqlParameter("@WagesPayType", (vm.WagesPayType == null) ? DBNull.Value : (object)vm.WagesPayType);

                //mQry = @"SELECT 0 AS LedgerHeaderId, @DocTypeId As DocTypeId, @DocDate AS DocDate, @HeaderLedgerAccountId AS HeaderLedgerAccountId, 
                //        A.LedgerAccountId, A.LedgerAccountName, A.LedgerAccountSuffix, 
                //        0.00 AS Amount, @Remark As HeaderRemark
                //        FROM Web.People P 
                //        LEFT JOIN Web.LedgerAccounts A ON P.PersonID = A.PersonId 
                //        Order By A.LedgerAccountName ";

                mQry = @"SELECT 0 AS LedgerHeaderId, @DocTypeId As DocTypeId, @DocDate AS DocDate, @HeaderLedgerAccountId AS HeaderLedgerAccountId, 
                        A.LedgerAccountId, A.LedgerAccountName, P.Code As LedgerAccountSuffix, 
                        0.00 AS Amount, @Remark As HeaderRemark
                        FROM Web.Employees E
                        LEFT JOIN Web.People P On E.PersonId = P.PersonId
                        LEFT JOIN Web.LedgerAccounts A ON P.PersonID = A.PersonId 
                        WHERE isnull(P.IsActive,1) =1 And E.DateOfJoining <= @DocDate AND E.DateOfRelieving IS NULL " +
                        (vm.DepartmentId != null ? " AND E.DepartmentId IN (SELECT Items FROM [dbo].[Split] (@DepartmentId, ',')) " : "") +
                        (vm.WagesPayType != null ? " AND E.WagesPayType = @WagesPayType" : "") +
                        " Order By A.LedgerAccountName ";

                IEnumerable<LedgerWizardResultViewModel> LedgerWizardResultViewModel = db.Database.SqlQuery<LedgerWizardResultViewModel>(mQry, SqlParameterDocDate, SqlParameterDocTypeId, SqlParameterHeaderLedgerAccountId, SqlParameterRemark, SqlParameterDepartmentId, SqlParameterWagesPayType).ToList();
                return LedgerWizardResultViewModel;
            }
            else
            {
                SqlParameter SqlParameterLedgerHeaderId = new SqlParameter("@LedgerHeaderId", vm.LedgerHeaderId.ToString());

                mQry = @"SELECT H.LedgerHeaderId, L.LedgerAccountId, A.LedgerAccountName, A.LedgerAccountSuffix, H.DocTypeId As DocTypeId, H.DocDate AS DocDate,
                        L.Amount AS Amount, H.Remark As HeaderRemark
                        FROM Web.LedgerHeaders H
                        LEFT JOIN Web.LedgerLines L ON H.LedgerHeaderId = L.LedgerHeaderId
                        LEFT JOIN Web.LedgerAccounts A ON L.LedgerAccountId = A.LedgerAccountId
                        WHERE H.LedgerHeaderId = @LedgerHeaderId
                        ORDER BY L.LedgerLineId ";

                IEnumerable<LedgerWizardResultViewModel> LedgerWizardResultViewModel = db.Database.SqlQuery<LedgerWizardResultViewModel>(mQry, SqlParameterLedgerHeaderId).ToList();
                return LedgerWizardResultViewModel;
            }
        }

        public void Dispose()
        {
        }
    }


    public class LedgerWizardResultViewModel
    {
        public int LedgerAccountId { get; set; }
        public string LedgerAccountName { get; set; }
        public string LedgerAccountSuffix { get; set; }
        public int DocTypeId { get; set; }
        public string DocDate { get; set; }
        public int HeaderLedgerAccountId { get; set; }
        public Decimal Amount { get; set; }
        public string HeaderRemark { get; set; }
        public int? LedgerHeaderId { get; set; }
    }
}
