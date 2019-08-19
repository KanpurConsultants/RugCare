using CustomEventArgs;
using Data.Models;
using System;
using StockReceiveDocumentEvents;
using System.Linq;
using Core.Common;
using Model.Models;
using System.Data;
using System.Data.SqlClient;

namespace Jobs.Controllers
{


    public class StockReceiveEvents : StockReceiveDocEvents
    {

        //For Subscribing Events
        public StockReceiveEvents()
        {
            Initialized = true;
            //_beforeHeaderSave += StockReceiveEvents__beforeHeaderSave;
            //_onHeaderSave += StockReceiveEvents__onHeaderSave;
            //_afterHeaderSave += StockReceiveEvents__afterHeaderSave;
            //_beforeHeaderDelete += StockReceiveEvents__beforeHeaderDelete;
            _onLineSave += StockReceiveEvents__onLineSave;
            _onLineDelete += StockReceiveEvents__onLineDelete;
            _onHeaderDelete += StockReceiveEvents__onHeaderDelete;
            _onHeaderSubmit += StocHeaderkEvents__onHeaderSubmit;
            //_beforeLineDelete += StockReceiveEvents__beforeLineDelete;
            //_afterHeaderDelete += StockReceiveEvents__afterHeaderDelete;
            _onLineSaveBulk += StockReceiveEvents__onLineSaveBulk;
        }

        void StocHeaderkEvents__onHeaderSubmit(object sender, StockEventArgs EventArgs, ref ApplicationDbContext db)
        {

            int Id = EventArgs.DocId;
            string ErrorText ="";
            string ConnectionString = (string)System.Web.HttpContext.Current.Session["DefaultConnectionString"];


            try
            {
                var Stock = (from H in db.StockHeader
                             join D in db.DocumentType on H.DocTypeId equals D.DocumentTypeId into DocumentTypeTable
                             from DocumentTypeTab in DocumentTypeTable.DefaultIfEmpty()
                             where H.StockHeaderId == EventArgs.DocId
                             select new { DocTypeName = DocumentTypeTab.DocumentTypeName, Status = H.Status }).FirstOrDefault();

                if (Stock.DocTypeName == "Carpet Stock Opening" || Stock.DocTypeName == "Map/Trace Stock Opening")
                {
                    using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                    {
                        sqlConnection.Open();
                        DataSet ds = new DataSet();

                        using (SqlCommand cmd = new SqlCommand("Web.SpCreate_ProductUid_FromStockOpening"))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Connection = sqlConnection;
                            cmd.Parameters.AddWithValue("@StockHeaderId", Id);
                            cmd.CommandTimeout = 1000;

                            using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                            {
                                adp.Fill(ds);
                            }

                            //cmd.Connection.Open();
                            //cmd.ExecuteNonQuery();
                            //cmd.Connection.Close();
                        }

                        if (ds.Tables[0].Rows.Count == 0)
                        {

                        }
                        else
                        {
                            for (int j = 0; j <= ds.Tables[0].Rows.Count - 1; j++)
                            {
                                if (ds.Tables[0].Rows[j]["Result"].ToString() != "")
                                {
                                    ErrorText = ErrorText + ds.Tables[0].Rows[j]["Result"].ToString();
                                }
                            }
                        }

                     }

                    if (ErrorText!="" && ErrorText != "Success")
                        throw new Exception("Something Went Wrong !" + ErrorText);
                }
            }

            catch (Exception ex)
            {
                //Header.Status = (int)StatusConstants.Drafted;
                //new StockHeaderService(_unitOfWork).Update(Header);
                //_unitOfWork.Save();
                throw new Exception("Something Went Wrong !" + ex.Message);
                //throw ex;
            }




        }

        void StockReceiveEvents__onHeaderDelete(object sender, StockEventArgs EventArgs, ref ApplicationDbContext db)
        {
            ApplicationDbContext DbContext = new ApplicationDbContext();
            int Id = EventArgs.DocId;

            var Temp = DbContext.StockLine.Where(m => m.StockHeaderId == EventArgs.DocId).ToList();
            var CostCenIds = Temp.Select(m => m.CostCenterId).ToArray();

            StockHeader Header = db.StockHeader.Find(Id);
            string DocumentTypeName = db.DocumentType.Find(Header.DocTypeId).DocumentTypeName;

            if (DocumentTypeName == TransactionDoctypeConstants.MaterialReturnFromWeaving)
            {
                var StockHeader = (from p in DbContext.StockHeader
                                   where p.StockHeaderId == EventArgs.DocId
                                   select p
                                ).FirstOrDefault();

                var DocType = (from p in DbContext.DocumentType
                               where p.DocumentTypeName == TransactionDoctypeConstants.MaterialReturnFromWeaving
                               select p).FirstOrDefault();

                var IssueLineCostCenterRecords = (from p in DbContext.StockLine
                                                  join t in DbContext.StockHeader on p.StockHeaderId equals t.StockHeaderId
                                                  where CostCenIds.Contains(p.CostCenterId) && t.DocTypeId == DocType.DocumentTypeId
                                                  && p.StockHeaderId != EventArgs.DocId
                                                  select p).ToList();


                var ReturnProductCount = (from p in IssueLineCostCenterRecords
                                          group p by p.CostCenterId into g
                                          select g).ToList();

                DbContext.Dispose();

                if (StockHeader.DocTypeId == DocType.DocumentTypeId)
                {

                    var CostCenterRecords = (from p in db.CostCenterStatusExtended
                                             where CostCenIds.Contains(p.CostCenterId)
                                             select p).ToList();

                    var GroupedTemp = (from p in Temp
                                       group p by new { p.CostCenterId } into g
                                       select g).ToList();

                    foreach (var item in GroupedTemp)
                    {

                        if (StockHeader.DocTypeId == DocType.DocumentTypeId && item.Sum(m => m.Qty) != 0)
                        {
                            if (item.Max(m => m.CostCenterId).HasValue)
                            {
                                var CostCenterStatus = (CostCenterRecords.Where(m => m.CostCenterId == item.Key.CostCenterId)).FirstOrDefault();

                                if (CostCenterStatus != null)
                                {
                                    CostCenterStatus.MaterialReturnQty = (CostCenterStatus.MaterialReturnQty ?? 0) - item.Sum(m => m.Qty);
                                    CostCenterStatus.MaterialReturnDate = StockHeader.DocDate;
                                    CostCenterStatus.MaterialReturnProductCount = (ReturnProductCount.Count == 0) ? 0 : ReturnProductCount.Where(m => m.Key == CostCenterStatus.CostCenterId).FirstOrDefault().Select(m => m.RequisitionLineId).Distinct().Count();
                                    CostCenterStatus.ObjectState = Model.ObjectState.Modified;
                                    db.CostCenterStatusExtended.Add(CostCenterStatus);
                                }
                            }
                        }
                    }
                }
            }
        }

        void StockReceiveEvents__onLineDelete(object sender, StockEventArgs EventArgs, ref ApplicationDbContext db)
        {
            ApplicationDbContext DbContext = new ApplicationDbContext();

            var Temp = DbContext.StockLine.Where(m => m.StockLineId == EventArgs.DocLineId).FirstOrDefault();

            var StockHeader = (from p in DbContext.StockHeader
                               where p.StockHeaderId == Temp.StockHeaderId
                               select p
                            ).FirstOrDefault();

            var DocType = (from p in DbContext.DocumentType
                           where p.DocumentTypeName == TransactionDoctypeConstants.MaterialReturnFromWeaving
                           select p).FirstOrDefault();

            if (DocType != null)
            {

            

            var ReceiveLineCostCenterRecords = (from p in DbContext.StockLine
                                                join t in DbContext.StockHeader on p.StockHeaderId equals t.StockHeaderId
                                                where p.CostCenterId == Temp.CostCenterId && t.DocTypeId == DocType.DocumentTypeId
                                                && p.StockLineId != EventArgs.DocLineId
                                                select p).ToList();

            var ReceiveProductCount = (from p in ReceiveLineCostCenterRecords
                                       select p.RequisitionLineId).Distinct().Count();


            DbContext.Dispose();




            if (StockHeader.DocTypeId == DocType.DocumentTypeId && Temp.Qty != 0)
            {
                if (Temp.CostCenterId.HasValue)
                {
                    var CostCenterStatus = (from p in db.CostCenterStatusExtended
                                            where p.CostCenterId == Temp.CostCenterId
                                            select p).FirstOrDefault();

                    if (CostCenterStatus != null)
                    {
                        CostCenterStatus.MaterialReturnQty = (CostCenterStatus.MaterialReturnQty ?? 0) - Temp.Qty;
                        CostCenterStatus.MaterialReturnDate = StockHeader.DocDate;
                        CostCenterStatus.MaterialReturnProductCount = ReceiveProductCount;
                        CostCenterStatus.ObjectState = Model.ObjectState.Modified;
                        db.CostCenterStatusExtended.Add(CostCenterStatus);
                    }
                }
            }
            }
        }

        void StockReceiveEvents__onLineSaveBulk(object sender, StockEventArgs EventArgs, ref ApplicationDbContext db)
        {
            var Temp = db.StockLine.Local.Where(m => m.StockHeaderId == EventArgs.DocId).ToList();
            var CostCenIds = Temp.Select(m => m.CostCenterId).ToArray();

            ApplicationDbContext DbContext = new ApplicationDbContext();
            var StockHeader = (from p in DbContext.StockHeader
                               where p.StockHeaderId == EventArgs.DocId
                               select p
                            ).FirstOrDefault();

            var DocType = (from p in DbContext.DocumentType
                           where p.DocumentTypeName == TransactionDoctypeConstants.MaterialReturnFromWeaving
                           select p).FirstOrDefault();

            if (DocType != null)
            {
                var IssueLineCostCenterRecords = (from p in DbContext.StockLine
                                                  join t in DbContext.StockHeader on p.StockHeaderId equals t.StockHeaderId
                                                  where CostCenIds.Contains(p.CostCenterId) && t.DocTypeId == DocType.DocumentTypeId
                                                  select p).ToList();

                IssueLineCostCenterRecords.AddRange(Temp);


                var IssueProductCount = (from p in IssueLineCostCenterRecords
                                         group p by p.CostCenterId into g
                                         select g).ToList();


                DbContext.Dispose();

                if (StockHeader.DocTypeId == DocType.DocumentTypeId)
                {
                    var CostCenterRecords = (from p in db.CostCenterStatusExtended
                                             where CostCenIds.Contains(p.CostCenterId)
                                             select p).ToList();

                    var GroupedTemp = (from p in Temp
                                       group p by new { p.CostCenterId } into g
                                       select g).ToList();

                    foreach (var item in GroupedTemp)
                    {

                        if (StockHeader.DocTypeId == DocType.DocumentTypeId && item.Sum(m => m.Qty) != 0)
                        {
                            if (item.Max(m => m.CostCenterId).HasValue)
                            {
                                var CostCenterStatus = (CostCenterRecords.Where(m => m.CostCenterId == item.Max(x => x.CostCenterId))).FirstOrDefault();

                                if (CostCenterStatus != null)
                                {
                                    CostCenterStatus.MaterialReturnQty = (CostCenterStatus.MaterialReturnQty ?? 0) + item.Sum(m => m.Qty);
                                    CostCenterStatus.MaterialReturnDate = StockHeader.DocDate;
                                    CostCenterStatus.MaterialReturnProductCount = IssueProductCount.Where(m => m.Key == CostCenterStatus.CostCenterId).FirstOrDefault().Select(m => m.RequisitionLineId).Distinct().Count();
                                    CostCenterStatus.ObjectState = Model.ObjectState.Modified;
                                    db.CostCenterStatusExtended.Add(CostCenterStatus);
                                }
                            }
                        }
                    }
                }
            }
        }

        void StockReceiveEvents__afterHeaderDelete(object sender, StockEventArgs EventArgs, ref ApplicationDbContext db)
        {
            throw new NotImplementedException();
        }

        bool StockReceiveEvents__beforeLineDelete(object sender, StockEventArgs EventArgs, ref ApplicationDbContext db)
        {
            throw new NotImplementedException();
        }

        void StockReceiveEvents__onLineSave(object sender, StockEventArgs EventArgs, ref ApplicationDbContext db)
        {
            var Temp = db.StockLine.Local.Where(m => m.StockLineId == EventArgs.DocLineId).FirstOrDefault();

            ApplicationDbContext DbContext = new ApplicationDbContext();

            var StockHeader = (from p in DbContext.StockHeader
                               where p.StockHeaderId == EventArgs.DocId
                               select p
                            ).FirstOrDefault();

            var DocType = (from p in DbContext.DocumentType
                           where p.DocumentTypeName == TransactionDoctypeConstants.MaterialReturnFromWeaving
                           select p).FirstOrDefault();

            if (DocType != null)
            {
                var ReceiveLineCostCenterRecords = (from p in DbContext.StockLine
                                                    join t in DbContext.StockHeader on p.StockHeaderId equals t.StockHeaderId
                                                    where p.CostCenterId == Temp.CostCenterId && t.DocTypeId == DocType.DocumentTypeId
                                                    && p.StockLineId != EventArgs.DocLineId
                                                    select p).ToList();

                ReceiveLineCostCenterRecords.Add(Temp);

                var ReceiveProductCount = (from p in ReceiveLineCostCenterRecords
                                           select p.RequisitionLineId).Distinct().Count();

                DbContext.Dispose();


                if (StockHeader.DocTypeId == DocType.DocumentTypeId && Temp.Qty != 0)
                {
                    if (Temp.CostCenterId.HasValue)
                    {
                        var CostCenterStatus = (from p in db.CostCenterStatusExtended
                                                where p.CostCenterId == Temp.CostCenterId
                                                select p).FirstOrDefault();

                        if (CostCenterStatus != null && EventArgs.DocLineId <= 0)
                        {
                            CostCenterStatus.MaterialReturnQty = ReceiveLineCostCenterRecords.Select(m => m.Qty).Sum();
                            CostCenterStatus.MaterialReturnDate = StockHeader.DocDate;
                            CostCenterStatus.MaterialReturnProductCount = ReceiveProductCount;
                            CostCenterStatus.ObjectState = Model.ObjectState.Modified;
                            db.CostCenterStatusExtended.Add(CostCenterStatus);
                        }
                        else if (CostCenterStatus != null && EventArgs.DocLineId > 0)
                        {

                            CostCenterStatus.MaterialReturnQty = ReceiveLineCostCenterRecords.Select(m => m.Qty).Sum();
                            CostCenterStatus.MaterialReturnDate = StockHeader.DocDate;
                            //CostCenterStatus.MaterialReturnProductCount = (CostCenterStatus.MaterialReturnProductCount ?? 0) + 1;
                            CostCenterStatus.MaterialIssueProductCount = ReceiveProductCount;
                            CostCenterStatus.ObjectState = Model.ObjectState.Modified;
                            db.CostCenterStatusExtended.Add(CostCenterStatus);
                        }
                    }
                }
            }
        }

        bool StockReceiveEvents__beforeHeaderDelete(object sender, StockEventArgs EventArgs, ref ApplicationDbContext db)
        {
            throw new NotImplementedException();
        }

        void StockReceiveEvents__afterHeaderSave(object sender, StockEventArgs EventArgs, ref ApplicationDbContext db)
        {
            throw new NotImplementedException();
        }

        void StockReceiveEvents__onHeaderSave(object sender, StockEventArgs EventArgs, ref ApplicationDbContext db)
        {
            //CostCenterStatus Temp = new CostCenterStatus();
            //var StockReceive = db.StockReceiveHeader.Local.Where(m => m.StockReceiveHeaderId == EventArgs.DocId).FirstOrDefault();
            //Temp.CostCenterId = db.CostCenter.Local.Where(m => m.CostCenterId == StockReceive.CostCenterId).FirstOrDefault().CostCenterId;
            //Temp.ObjectState = Model.ObjectState.Added;
            //db.CostCenterStatus.Add(Temp);
        }

        bool StockReceiveEvents__beforeHeaderSave(object sender, StockEventArgs EventArgs, ref ApplicationDbContext db)
        {
            throw new NotImplementedException();
        }

    }
}
