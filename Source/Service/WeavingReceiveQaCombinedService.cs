using System.Collections.Generic;
using System.Linq;
using Data.Infrastructure;
using Model.Models;
using Core.Common;
using System;
using Model;
using System.Threading.Tasks;
using Data.Models;
using Model.ViewModels;
using Model.ViewModel;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Data;


namespace Service
{
    public interface IWeavingReceiveQACombinedService : IDisposable
    {
        JobReceiveHeader Create(WeavingReceiveQACombinedViewModel pt, string UserName);
        void Delete(int id);
        void Update(WeavingReceiveQACombinedViewModel pt, string UserName, IEnumerable<JobReceiveLineViewModel> ReceiveLineList);
        Task<IEquatable<JobReceiveQAAttribute>> GetAsync();
        Task<JobReceiveQAAttribute> FindAsync(int id);
        WeavingReceiveQACombinedViewModel GetJobReceiveDetailForEdit(int JobReceiveHeaderId);//JobReceiveHeaderId
        WeavingReceiveQACombinedViewModel GetJobReceiveDetailForNextCreate(string UserName);
        LastValues GetLastValues(int DocTypeId, string UserName);
        IQueryable<ComboBoxResult> GetCustomProduct(int filter, string term, int? PersonId);
        IQueryable<ComboBoxResult> TempGetCustomProduct(int filter, string term, int? PersonId);        
        IQueryable<ComboBoxResult> GetCustomPerson(int Id, string term);
        JobReceiveHeaderViewModel GetJobReceiveHeader_ByReferenceId(int ReferenceDocId, int ReferenceDocTypeId);
        JobReceiveHeaderViewModel GetJobReceiveHeader_ByProductUId(int ProductUId, int DocumentTypeId, int SiteId, int DivisionId);
    }

    public class WeavingReceiveQACombinedService : IWeavingReceiveQACombinedService
    {
        ApplicationDbContext db;

        IUnitOfWork _unitOfWork;
        public WeavingReceiveQACombinedService(ApplicationDbContext db)
        {

            this.db = db;
        }

        public WeavingReceiveQACombinedService(IUnitOfWork uow)
        {
            _unitOfWork = uow;
        }

        public JobReceiveQAAttribute Find(int id)
        {
            return db.JobReceiveQAAttribute.Find(id);
        }

        public JobReceiveHeaderViewModel GetJobReceiveHeader_ByReferenceId(int ReferenceDocId, int ReferenceDocTypeId)
        {
              var pt= (from H in db.JobReceiveHeader
                    where H.ReferenceDocId == ReferenceDocId && H.ReferenceDocTypeId == ReferenceDocTypeId
                    select new JobReceiveHeaderViewModel
                    {
                        JobReceiveHeaderId = H.JobReceiveHeaderId
                    }).FirstOrDefault();

            return pt;
        }

        public JobReceiveHeaderViewModel GetJobReceiveHeader_ByProductUId(int ProductUId, int DocumentTypeId, int SiteId, int DivisionId)
        {
            string mQry = @"SELECT H.JobReceiveHeaderId, H.DocTypeId, H.DocDate, H.DocNo, H.DivisionId, H.SiteId 
                            FROM web.JobReceiveHeaders H WITH(Nolock)
                            LEFT JOIN web.JobReceiveLines L WITH(Nolock) ON L.JobReceiveHeaderId = H.JobReceiveHeaderId
                            LEFT JOIN web.ProductUids PU WITH(Nolock) ON PU.ProductUIDId = L.ProductUidId
                            LEFT JOIN web.ProductUids PU1 WITH(Nolock) ON PU1.LotNo = L.LotNo
                            WHERE H.DocTypeId =" + DocumentTypeId + @" AND isnull(PU.ProductUIDId, PU1.ProductUIDId) = " + ProductUId + " "+
                            @" AND  H.SiteId = " + SiteId + " AND  H.DivisionId = " + DivisionId + " ";

            IEnumerable<JobReceiveHeaderViewModel> pt = db.Database.SqlQuery<JobReceiveHeaderViewModel>(mQry).ToList();
            return pt.FirstOrDefault();
        }


        public JobReceiveHeader Create(WeavingReceiveQACombinedViewModel pt, string UserName)
        {
            JobOrderLine JobOrderLine = db.JobOrderLine.Find(pt.JobOrderLineId);
            JobOrderHeader JobOrderHeader = db.JobOrderHeader.Find(JobOrderLine.JobOrderHeaderId);
            JobReceiveSettings jobreceivesetting = new JobReceiveSettingsService(_unitOfWork).GetJobReceiveSettingsForDocument(pt.DocTypeId, pt.DivisionId, pt.SiteId);


            string mProductUidName ="";
            Int64 mProductUid = 0;
            Int64 Qty;

            if (pt.JobReceiveSettings.isAllowtoGenerateMultipleBarcode)
                Qty = Convert.ToInt64(pt.ToProductUidName.ToString()) - Convert.ToInt64(pt.ProductUidName.ToString());
            else
                Qty = 0;
            Qty = Qty + 1;

            JobReceiveHeader JobReceiveHeader = new JobReceiveHeader();
            JobReceiveHeader.JobReceiveHeaderId = pt.JobReceiveHeaderId;
            JobReceiveHeader.DocTypeId = pt.DocTypeId;
            JobReceiveHeader.DocNo = pt.DocNo;
            JobReceiveHeader.DocDate = pt.DocDate;
            JobReceiveHeader.DivisionId = pt.DivisionId;
            JobReceiveHeader.SiteId = pt.SiteId;
            JobReceiveHeader.ProcessId = pt.ProcessId;
            JobReceiveHeader.JobWorkerId = pt.JobWorkerId;
            JobReceiveHeader.JobWorkerDocNo = pt.DocNo;
            JobReceiveHeader.JobReceiveById = pt.JobReceiveById;
            JobReceiveHeader.GodownId = pt.GodownId;
            JobReceiveHeader.Remark = pt.Remark;
            JobReceiveHeader.ReferenceDocId = pt.ReferenceDocId;
            JobReceiveHeader.ReferenceDocTypeId = pt.ReferenceDocTypeId;
            JobReceiveHeader.CreatedBy = UserName;
            JobReceiveHeader.CreatedBy = UserName;
            JobReceiveHeader.CreatedDate = DateTime.Now;
            JobReceiveHeader.ModifiedBy = UserName;
            JobReceiveHeader.ModifiedDate = DateTime.Now;
            JobReceiveHeader.Status = (int)StatusConstants.Drafted;

            StockHeader StockHeader = new StockHeader();
            StockHeader.StockHeaderId = pt.StockHeaderId;
            StockHeader.DocTypeId = JobReceiveHeader.DocTypeId;
            StockHeader.DocDate = JobReceiveHeader.DocDate;
            StockHeader.DocNo = JobReceiveHeader.DocNo;
            StockHeader.DivisionId = JobReceiveHeader.DivisionId;
            StockHeader.SiteId = JobReceiveHeader.SiteId;
            StockHeader.PersonId = JobReceiveHeader.JobWorkerId;
            StockHeader.ProcessId = JobReceiveHeader.ProcessId;
            StockHeader.FromGodownId = null;
            StockHeader.GodownId = JobReceiveHeader.GodownId;
            StockHeader.Remark = JobReceiveHeader.Remark;
            StockHeader.Status = JobReceiveHeader.Status;
            StockHeader.CreatedBy = JobReceiveHeader.CreatedBy;
            StockHeader.CreatedDate = JobReceiveHeader.CreatedDate;
            StockHeader.ModifiedBy = JobReceiveHeader.ModifiedBy;
            StockHeader.ModifiedDate = JobReceiveHeader.ModifiedDate;
            StockHeader.ObjectState = Model.ObjectState.Added;
            db.StockHeader.Add(StockHeader);


            JobReceiveHeader.StockHeaderId = StockHeader.StockHeaderId;
            JobReceiveHeader.ObjectState = Model.ObjectState.Added;
            db.JobReceiveHeader.Add(JobReceiveHeader);

            JobReceiveQAHeader JobReceiveQAHeader = new JobReceiveQAHeader();
            JobReceiveQAHeader.JobReceiveQAHeaderId = pt.JobReceiveQAHeaderId;
            JobReceiveQAHeader.DocTypeId = JobReceiveHeader.DocTypeId;
            JobReceiveQAHeader.DocDate = JobReceiveHeader.DocDate;
            JobReceiveQAHeader.DocNo = JobReceiveHeader.DocNo;
            JobReceiveQAHeader.DivisionId = JobReceiveHeader.DivisionId;
            JobReceiveQAHeader.SiteId = JobReceiveHeader.SiteId;
            JobReceiveQAHeader.ProcessId = JobReceiveHeader.ProcessId;
            JobReceiveQAHeader.JobWorkerId = JobReceiveHeader.JobWorkerId;
            JobReceiveQAHeader.QAById = JobReceiveHeader.JobReceiveById;
            JobReceiveQAHeader.Remark = JobReceiveHeader.Remark;
            JobReceiveQAHeader.Status = JobReceiveHeader.Status;
            JobReceiveQAHeader.CreatedBy = JobReceiveHeader.CreatedBy;
            JobReceiveQAHeader.CreatedDate = JobReceiveHeader.CreatedDate;
            JobReceiveQAHeader.ModifiedBy = JobReceiveHeader.ModifiedBy;
            JobReceiveQAHeader.ModifiedDate = JobReceiveHeader.ModifiedDate;
            JobReceiveQAHeader.ObjectState = Model.ObjectState.Added;
            db.JobReceiveQAHeader.Add(JobReceiveQAHeader);


            ProductUidHeader ProductUidHeader = new ProductUidHeader();
            if (pt.ProductUidId == null && pt.ProductUidName != null && jobreceivesetting.SqlProcGenProductUID != null)
            {
                ProductUidHeader.ProductId = pt.ProductId;
                ProductUidHeader.GenDocId = JobReceiveHeader.JobReceiveHeaderId;
                ProductUidHeader.GenDocNo = JobReceiveHeader.DocNo;
                ProductUidHeader.GenDocTypeId = JobReceiveHeader.DocTypeId;
                ProductUidHeader.GenDocDate = JobReceiveHeader.DocDate;
                ProductUidHeader.GenPersonId = JobReceiveHeader.JobWorkerId;
                ProductUidHeader.CreatedBy = UserName;
                ProductUidHeader.CreatedDate = DateTime.Now;
                ProductUidHeader.ModifiedBy = UserName;
                ProductUidHeader.ModifiedDate = DateTime.Now;
                ProductUidHeader.ObjectState = Model.ObjectState.Added;
                db.ProductUidHeader.Add(ProductUidHeader);                
            }

            int tProductUid = 1;

            for (int Pcs = 0; Pcs < Qty; Pcs++)
            {
                tProductUid = tProductUid - 1;

                if (pt.ProductUidName != null)
                {
                    mProductUid = Convert.ToInt64(pt.ProductUidName.ToString());
                    mProductUid = mProductUid + Pcs;
                    mProductUidName = mProductUid.ToString();
                }
                
                JobReceiveLine JobReceiveLine = new JobReceiveLine();
                JobReceiveLine.JobReceiveHeaderId = JobReceiveHeader.JobReceiveHeaderId;
                JobReceiveLine.JobReceiveLineId = pt.JobReceiveLineId+ Pcs;
                JobReceiveLine.ProductId = pt.ProductId;
                JobReceiveLine.JobOrderLineId = pt.JobOrderLineId;

                if (pt.ProductUidName != null)
                {
                    if (pt.ProductUidId != null)
                        JobReceiveLine.ProductUidId = pt.ProductUidId;
                    else 
                        JobReceiveLine.ProductUidId = tProductUid;
                }

                JobReceiveLine.LossQty = 0;
                //JobReceiveLine.Qty = pt.Qty;
                //JobReceiveLine.PassQty = pt.Qty;
                JobReceiveLine.Qty = pt.Qty/ Qty;
                JobReceiveLine.PassQty = pt.Qty / Qty;
                JobReceiveLine.UnitConversionMultiplier = JobOrderLine.UnitConversionMultiplier;
                //JobReceiveLine.DealQty = JobReceiveLine.Qty * JobReceiveLine.UnitConversionMultiplier;
                JobReceiveLine.isHoldForInvoice = pt.isHoldForInvoice;
                JobReceiveLine.ReasonInvoiceHold = pt.ReasonInvoiceHold;
                //JobReceiveLine.DealQty = pt.DealQty;
                JobReceiveLine.DealQty = pt.DealQty / Qty;
                JobReceiveLine.DealUnitId = pt.DealUnitId;

                if (pt.LastWeight != null && pt.LastWeight != 0)
                    JobReceiveLine.Weight = pt.LastWeight == 0 ? pt.Weight : pt.Weight - (decimal)pt.LastWeight;
                else
                    //JobReceiveLine.Weight = pt.Weight;
                    JobReceiveLine.Weight = pt.Weight / Qty;

                if (jobreceivesetting.isVisibleLotNo == true)
                    JobReceiveLine.LotNo = pt.LotNo;

                JobReceiveLine.Sr = 1+ Pcs;
                JobReceiveLine.CreatedDate = DateTime.Now;
                JobReceiveLine.ModifiedDate = DateTime.Now;
                JobReceiveLine.CreatedBy = UserName;
                JobReceiveLine.ModifiedBy = UserName;

                Dictionary<int, decimal> LineStatus = new Dictionary<int, decimal>();
                LineStatus.Add((int)JobReceiveLine.JobOrderLineId, (JobReceiveLine.Qty + JobReceiveLine.LossQty));

                Stock Stock = new Stock();
                Stock.DocDate = JobReceiveHeader.DocDate;
                //Stock.StockId = pt.StockId;
                Stock.StockId = tProductUid; 
                Stock.ProductId = pt.ProductId;
                Stock.ProcessId = JobReceiveHeader.ProcessId;
                Stock.GodownId = JobReceiveHeader.GodownId;
                Stock.LotNo = JobReceiveLine.LotNo;
                Stock.ProductUidId = JobReceiveLine.ProductUidId;
                Stock.CostCenterId = JobOrderHeader.CostCenterId;
                Stock.Qty_Iss = 0;
                Stock.Qty_Rec = JobReceiveLine.Qty;
                Stock.Remark = JobReceiveLine.Remark;
                Stock.StockHeaderId = StockHeader.StockHeaderId;
                Stock.CreatedBy = JobReceiveLine.CreatedBy;
                Stock.CreatedDate = JobReceiveLine.CreatedDate;
                Stock.ModifiedBy = JobReceiveLine.ModifiedBy;
                Stock.ModifiedDate = JobReceiveLine.ModifiedDate;






                if (pt.ProductUidId == null && pt.ProductUidName != null && jobreceivesetting.SqlProcGenProductUID != null)
                {
                    JobReceiveLine.ProductUidHeaderId = ProductUidHeader.ProductUidHeaderId;

                    ProductUid ProductUid = new ProductUid();
                    ProductUid.ProductUidHeaderId = ProductUidHeader.ProductUidHeaderId;
                    //ProductUid.ProductUidName = pt.ProductUidName;
                    ProductUid.ProductUIDId = tProductUid;
                    ProductUid.ProductUidName = mProductUidName;
                    ProductUid.LotNo = JobReceiveLine.LotNo;
                    ProductUid.ProductId = pt.ProductId;
                    ProductUid.IsActive = true;
                    ProductUid.CreatedBy = UserName;
                    ProductUid.CreatedDate = DateTime.Now;
                    ProductUid.ModifiedBy = UserName;
                    ProductUid.ModifiedDate = DateTime.Now;
                    ProductUid.GenLineId = null;
                    ProductUid.GenDocId = JobReceiveHeader.JobReceiveHeaderId;
                    ProductUid.GenDocNo = JobReceiveHeader.DocNo;
                    ProductUid.GenDocTypeId = JobReceiveHeader.DocTypeId;
                    ProductUid.GenDocDate = JobReceiveHeader.DocDate;
                    ProductUid.GenPersonId = JobReceiveHeader.JobWorkerId;
                    ProductUid.CurrenctProcessId = JobReceiveHeader.ProcessId;
                    ProductUid.CurrenctGodownId = JobReceiveHeader.GodownId;
                    ProductUid.Status = ProductUidStatusConstants.Receive;
                    ProductUid.LastTransactionDocId = JobReceiveHeader.JobReceiveHeaderId;
                    ProductUid.LastTransactionDocNo = JobReceiveHeader.DocNo;
                    ProductUid.LastTransactionDocTypeId = JobReceiveHeader.DocTypeId;
                    ProductUid.LastTransactionDocDate = JobReceiveHeader.DocDate;
                    ProductUid.LastTransactionPersonId = JobReceiveHeader.JobWorkerId;
                    ProductUid.LastTransactionLineId = null;
                    ProductUid.ObjectState = Model.ObjectState.Added;
                    db.ProductUid.Add(ProductUid);
                    JobReceiveLine.ProductUidId = JobOrderLine.ProductUidId != null ? JobOrderLine.ProductUidId : ProductUid.ProductUIDId;
                    Stock.ProductUidId = JobOrderLine.ProductUidId != null ? JobOrderLine.ProductUidId : ProductUid.ProductUIDId;
                }
                else
                {
                    ProductUid ProductUid;

                    if (JobOrderLine.ProductUidId != null)
                    {
                        ProductUid = db.ProductUid.Find(JobOrderLine.ProductUidId);

                    }
                    else if (pt.ProductUidName != null)
                    {
                        ProductUid = db.ProductUid.Where(i => i.ProductUidName == pt.ProductUidName).FirstOrDefault();

                    }
                    else
                    {
                        ProductUid = db.ProductUid.Find(pt.ProductUidId);

                    }

                    //if (pt.ProductUidId != null)
                    //{
                    //    ProductUid = db.ProductUid.Find(pt.ProductUidId);

                    //}

                    if (ProductUid != null)
                    {
                        JobReceiveLine.ProductUidLastTransactionDocId = ProductUid.LastTransactionDocId;
                        JobReceiveLine.ProductUidLastTransactionDocDate = ProductUid.LastTransactionDocDate;
                        JobReceiveLine.ProductUidLastTransactionDocNo = ProductUid.LastTransactionDocNo;
                        JobReceiveLine.ProductUidLastTransactionDocTypeId = ProductUid.LastTransactionDocTypeId;
                        JobReceiveLine.ProductUidLastTransactionPersonId = ProductUid.LastTransactionPersonId;
                        JobReceiveLine.ProductUidStatus = ProductUid.Status;
                        JobReceiveLine.ProductUidCurrentProcessId = ProductUid.CurrenctProcessId;
                        JobReceiveLine.ProductUidCurrentGodownId = ProductUid.CurrenctGodownId;

                        string ProcessesDone = "";
                        ProcessesDone = ProductUid.ProcessesDone == null ? ("|" + JobReceiveHeader.ProcessId.ToString() + "|") : (ProductUid.ProcessesDone + ",|" + JobReceiveHeader.ProcessId.ToString() + "|");
                        ProductUid.ModifiedBy = UserName;
                        ProductUid.ModifiedDate = DateTime.Now;
                        ProductUid.CurrenctProcessId = JobReceiveHeader.ProcessId;
                        ProductUid.ProcessesDone = ProcessesDone;
                        ProductUid.CurrenctGodownId = JobReceiveHeader.GodownId;
                        ProductUid.Status = ProductUidStatusConstants.Receive;
                        ProductUid.LastTransactionDocId = JobReceiveHeader.JobReceiveHeaderId;
                        ProductUid.LastTransactionDocNo = JobReceiveHeader.DocNo;
                        ProductUid.LastTransactionDocTypeId = JobReceiveHeader.DocTypeId;
                        ProductUid.LastTransactionDocDate = JobReceiveHeader.DocDate;
                        ProductUid.LastTransactionPersonId = JobReceiveHeader.JobWorkerId;
                        ProductUid.LastTransactionLineId = null;
                        ProductUid.ObjectState = Model.ObjectState.Modified;
                        db.ProductUid.Add(ProductUid);
                    }

                    //Akash
                    if (ProductUid != null)
                    {
                        JobReceiveLine.ProductUidId = JobOrderLine.ProductUidId != null ? JobOrderLine.ProductUidId : ProductUid.ProductUIDId;
                        Stock.ProductUidId = JobOrderLine.ProductUidId != null ? JobOrderLine.ProductUidId : ProductUid.ProductUIDId;
                        JobReceiveLine.ProductId = JobOrderLine.ProductId != null ? JobOrderLine.ProductId : ProductUid.ProductId;
                        Stock.ProductId = JobOrderLine.ProductId != null ? JobOrderLine.ProductId : ProductUid.ProductId;

                    }
                }


                //Akash
                if (pt.ProductUidName == null && pt.LotNo != null)
                {
                    ProductUid uid = new ProductUidService(_unitOfWork).FGetProductUidByName(pt.LotNo);
                    if (uid != null)
                    {

                        JobReceiveLine.ProductUidLastTransactionDocId = uid.LastTransactionDocId;
                        JobReceiveLine.ProductUidLastTransactionDocDate = uid.LastTransactionDocDate;
                        JobReceiveLine.ProductUidLastTransactionDocNo = uid.LastTransactionDocNo;
                        JobReceiveLine.ProductUidLastTransactionDocTypeId = uid.LastTransactionDocTypeId;
                        JobReceiveLine.ProductUidLastTransactionPersonId = uid.LastTransactionPersonId;
                        JobReceiveLine.ProductUidStatus = uid.Status;
                        JobReceiveLine.ProductUidCurrentProcessId = uid.CurrenctProcessId;
                        JobReceiveLine.ProductUidCurrentGodownId = uid.CurrenctGodownId;


                        uid.ModifiedBy = UserName;
                        uid.ModifiedDate = DateTime.Now;
                        uid.CurrenctProcessId = JobReceiveHeader.ProcessId;
                        uid.CurrenctGodownId = JobReceiveHeader.GodownId;
                        uid.Status = ProductUidStatusConstants.Receive;
                        uid.LastTransactionDocId = JobReceiveHeader.JobReceiveHeaderId;
                        uid.LastTransactionDocNo = JobReceiveHeader.DocNo;
                        uid.LastTransactionDocTypeId = JobReceiveHeader.DocTypeId;
                        uid.LastTransactionDocDate = JobReceiveHeader.DocDate;
                        uid.LastTransactionPersonId = JobReceiveHeader.JobWorkerId;
                        uid.LastTransactionLineId = null;
                        uid.ObjectState = Model.ObjectState.Modified;
                        db.ProductUid.Add(uid);

                    }
                    else
                    {
                        ProductUidHeader.ProductId = pt.ProductId;
                        ProductUidHeader.GenDocId = JobReceiveHeader.JobReceiveHeaderId;
                        ProductUidHeader.GenDocNo = JobReceiveHeader.DocNo;
                        ProductUidHeader.GenDocTypeId = JobReceiveHeader.DocTypeId;
                        ProductUidHeader.GenDocDate = JobReceiveHeader.DocDate;
                        ProductUidHeader.GenPersonId = JobReceiveHeader.JobWorkerId;
                        ProductUidHeader.CreatedBy = UserName;
                        ProductUidHeader.CreatedDate = DateTime.Now;
                        ProductUidHeader.ModifiedBy = UserName;
                        ProductUidHeader.ModifiedDate = DateTime.Now;
                        ProductUidHeader.ObjectState = Model.ObjectState.Added;
                        db.ProductUidHeader.Add(ProductUidHeader);
                        JobReceiveLine.ProductUidHeaderId = ProductUidHeader.ProductUidHeaderId;



                        ProductUid ProductUid = new ProductUid();
                        ProductUid.ProductUidHeaderId = ProductUidHeader.ProductUidHeaderId;
                        ProductUid.ProductUidName = pt.LotNo;
                        ProductUid.LotNo = pt.LotNo;
                        ProductUid.ProductId = pt.ProductId;
                        ProductUid.IsActive = true;
                        ProductUid.CreatedBy = UserName;
                        ProductUid.CreatedDate = DateTime.Now;
                        ProductUid.ModifiedBy = UserName;
                        ProductUid.ModifiedDate = DateTime.Now;
                        ProductUid.GenLineId = null;
                        ProductUid.GenDocId = JobReceiveHeader.JobReceiveHeaderId;
                        ProductUid.GenDocNo = JobReceiveHeader.DocNo;
                        ProductUid.GenDocTypeId = JobReceiveHeader.DocTypeId;
                        ProductUid.GenDocDate = JobReceiveHeader.DocDate;
                        ProductUid.GenPersonId = JobReceiveHeader.JobWorkerId;
                        ProductUid.CurrenctProcessId = JobReceiveHeader.ProcessId;
                        ProductUid.CurrenctGodownId = JobReceiveHeader.GodownId;
                        ProductUid.Status = ProductUidStatusConstants.Receive;
                        ProductUid.LastTransactionDocId = JobReceiveHeader.JobReceiveHeaderId;
                        ProductUid.LastTransactionDocNo = JobReceiveHeader.DocNo;
                        ProductUid.LastTransactionDocTypeId = JobReceiveHeader.DocTypeId;
                        ProductUid.LastTransactionDocDate = JobReceiveHeader.DocDate;
                        ProductUid.LastTransactionPersonId = JobReceiveHeader.JobWorkerId;
                        ProductUid.LastTransactionLineId = null;
                        ProductUid.ObjectState = Model.ObjectState.Added;
                        db.ProductUid.Add(ProductUid);
                        //JobReceiveLine.ProductUidId = JobOrderLine.ProductUidId != null ? JobOrderLine.ProductUidId : ProductUid.ProductUIDId;
                        //Stock.ProductUidId = JobOrderLine.ProductUidId != null ? JobOrderLine.ProductUidId : ProductUid.ProductUIDId;
                        //Stock.ProductUidId = ProductUid.ProductUIDId;
                    }
                }



                if (jobreceivesetting.isPostedInStockProcess == true)
                {
                    StockProcess StockProcess = new StockProcess();
                    StockProcess.DocDate = JobReceiveHeader.DocDate;
                    //StockProcess.StockId = pt.StockId;
                    StockProcess.ProductId = pt.ProductId;
                    StockProcess.ProcessId = JobReceiveHeader.ProcessId;
                    StockProcess.GodownId = JobReceiveHeader.GodownId;
                    StockProcess.LotNo = JobReceiveLine.LotNo;
                    StockProcess.ProductUidId = JobReceiveLine.ProductUidId;
                    StockProcess.CostCenterId = JobOrderHeader.CostCenterId;
                    StockProcess.Qty_Iss = JobReceiveLine.Qty;
                    StockProcess.Qty_Rec = 0;
                    StockProcess.Remark = JobReceiveLine.Remark;
                    StockProcess.StockHeaderId = StockHeader.StockHeaderId;
                    StockProcess.CreatedBy = JobReceiveLine.CreatedBy;
                    StockProcess.CreatedDate = JobReceiveLine.CreatedDate;
                    StockProcess.ModifiedBy = JobReceiveLine.ModifiedBy;
                    StockProcess.ModifiedDate = JobReceiveLine.ModifiedDate;

                    StockProcess.ObjectState = Model.ObjectState.Added;
                    db.StockProcess.Add(StockProcess);
                }


                Stock.ObjectState = Model.ObjectState.Added;
                db.Stock.Add(Stock);



                JobReceiveLine.StockId = Stock.StockId;
                //JobReceiveLine.StockId = tProductUid ;
                JobReceiveLine.ObjectState = Model.ObjectState.Added;
                db.JobReceiveLine.Add(JobReceiveLine);

                JobReceiveLineStatus JobReceiveLineStatus = new JobReceiveLineStatus();
                JobReceiveLineStatus.JobReceiveLineId = JobReceiveLine.JobReceiveLineId;
                JobReceiveLineStatus.ObjectState = Model.ObjectState.Added;
                db.JobReceiveLineStatus.Add(JobReceiveLineStatus);


                if (pt.Rate != pt.XRate)
                {
                    JobOrderLine.Rate = pt.Rate;
                    JobOrderLine.ObjectState = ObjectState.Modified;
                    db.JobOrderLine.Add(JobOrderLine);
                }


                //JobReceiveQAHeader JobReceiveQAHeader = new JobReceiveQAHeader();
                //JobReceiveQAHeader.JobReceiveQAHeaderId = pt.JobReceiveQAHeaderId;
                //JobReceiveQAHeader.DocTypeId = JobReceiveHeader.DocTypeId;
                //JobReceiveQAHeader.DocDate = JobReceiveHeader.DocDate;
                //JobReceiveQAHeader.DocNo = JobReceiveHeader.DocNo;
                //JobReceiveQAHeader.DivisionId = JobReceiveHeader.DivisionId;
                //JobReceiveQAHeader.SiteId = JobReceiveHeader.SiteId;
                //JobReceiveQAHeader.ProcessId = JobReceiveHeader.ProcessId;
                //JobReceiveQAHeader.JobWorkerId = JobReceiveHeader.JobWorkerId;
                //JobReceiveQAHeader.QAById = JobReceiveHeader.JobReceiveById;
                //JobReceiveQAHeader.Remark = JobReceiveHeader.Remark;
                //JobReceiveQAHeader.Status = JobReceiveHeader.Status;
                //JobReceiveQAHeader.CreatedBy = JobReceiveHeader.CreatedBy;
                //JobReceiveQAHeader.CreatedDate = JobReceiveHeader.CreatedDate;
                //JobReceiveQAHeader.ModifiedBy = JobReceiveHeader.ModifiedBy;
                //JobReceiveQAHeader.ModifiedDate = JobReceiveHeader.ModifiedDate;
                //JobReceiveQAHeader.ObjectState = Model.ObjectState.Added;
                //db.JobReceiveQAHeader.Add(JobReceiveQAHeader);


                JobReceiveQALine JobReceiveQALine = new JobReceiveQALine();
                JobReceiveQALine.JobReceiveQAHeaderId = JobReceiveQAHeader.JobReceiveQAHeaderId;
                //JobReceiveQALine.JobReceiveQALineId = pt.JobReceiveQALineId;
                JobReceiveQALine.JobReceiveQALineId = tProductUid;
                JobReceiveQALine.Sr = JobReceiveLine.Sr;
                JobReceiveQALine.ProductUidId = JobReceiveLine.ProductUidId;
                JobReceiveQALine.JobReceiveLineId = JobReceiveLine.JobReceiveLineId;
                JobReceiveQALine.QAQty = JobReceiveLine.Qty;
                JobReceiveQALine.InspectedQty = JobReceiveLine.Qty;
                JobReceiveQALine.Qty = JobReceiveLine.Qty;
                JobReceiveQALine.FailQty = 0;
                JobReceiveQALine.UnitConversionMultiplier = pt.UnitConversionMultiplier;
                JobReceiveQALine.DealQty = JobReceiveLine.DealQty;
                JobReceiveQALine.FailDealQty = 0;
                //JobReceiveQALine.Weight = JobReceiveLine.Weight;
                //JobReceiveQALine.Weight = pt.Weight / Qty;

                if (pt.LastWeight != null && pt.LastWeight != 0)
                    JobReceiveQALine.Weight = pt.LastWeight == 0 ? pt.Weight : pt.Weight - (decimal)pt.LastWeight;
                else
                    JobReceiveQALine.Weight = pt.Weight / Qty;

                JobReceiveQALine.PenaltyRate = JobReceiveLine.PenaltyRate;
                JobReceiveQALine.PenaltyAmt = JobReceiveLine.PenaltyAmt;
                JobReceiveQALine.Remark = JobReceiveLine.Remark;
                JobReceiveQALine.CreatedBy = JobReceiveLine.CreatedBy;
                JobReceiveQALine.CreatedDate = JobReceiveLine.CreatedDate;
                JobReceiveQALine.ModifiedBy = JobReceiveLine.ModifiedBy;
                JobReceiveQALine.ModifiedDate = JobReceiveLine.ModifiedDate;
                JobReceiveQALine.ObjectState = Model.ObjectState.Added;
                db.JobReceiveQALine.Add(JobReceiveQALine);


                JobReceiveQALineExtended JobReceiveQALineExtended = new JobReceiveQALineExtended();
                JobReceiveQALineExtended.JobReceiveQALineId = JobReceiveQALine.JobReceiveQALineId;
                JobReceiveQALineExtended.Length = pt.Length;
                JobReceiveQALineExtended.Width = pt.Width;
                JobReceiveQALineExtended.Height = pt.Height;
                JobReceiveQALineExtended.ObjectState = ObjectState.Added;
                db.JobReceiveQALineExtended.Add(JobReceiveQALineExtended);


                List<QAGroupLineLineViewModel> QAGroupLineList = pt.QAGroupLine;

                if (QAGroupLineList != null)
                {
                    foreach (var item in QAGroupLineList)
                    {
                        JobReceiveQAAttribute JobReceiveQAAttribute = new JobReceiveQAAttribute();
                        JobReceiveQAAttribute.JobReceiveQALineId = JobReceiveQALine.JobReceiveQALineId;
                        JobReceiveQAAttribute.QAGroupLineId = item.QAGroupLineId;
                        JobReceiveQAAttribute.Value = item.Value;
                        JobReceiveQAAttribute.Remark = item.Remarks;
                        JobReceiveQAAttribute.CreatedBy = UserName;
                        JobReceiveQAAttribute.ModifiedBy = UserName;
                        JobReceiveQAAttribute.CreatedDate = DateTime.Now;
                        JobReceiveQAAttribute.ModifiedDate = DateTime.Now;
                        JobReceiveQAAttribute.ObjectState = ObjectState.Added;
                        db.JobReceiveQAAttribute.Add(JobReceiveQAAttribute);
                    }
                }
            }

            return JobReceiveHeader;
        }


        public void Update(WeavingReceiveQACombinedViewModel pt, string UserName, IEnumerable<JobReceiveLineViewModel> ReceiveLineList)
        {
            JobReceiveHeader JobReceiveHeader = db.JobReceiveHeader.Find(pt.JobReceiveHeaderId);
            JobReceiveHeader.DocNo = pt.DocNo;
            JobReceiveHeader.DocDate = pt.DocDate;
            JobReceiveHeader.JobWorkerId = pt.JobWorkerId;
            JobReceiveHeader.JobWorkerDocNo = pt.DocNo;
            JobReceiveHeader.JobReceiveById = pt.JobReceiveById;
            JobReceiveHeader.GodownId = pt.GodownId;
            JobReceiveHeader.Remark = pt.Remark;
            JobReceiveHeader.ModifiedBy = UserName;
            JobReceiveHeader.ModifiedDate = DateTime.Now;
            JobReceiveHeader.ObjectState = ObjectState.Modified;
            db.JobReceiveHeader.Add(JobReceiveHeader);

            StockHeader StockHeader = db.StockHeader.Find(JobReceiveHeader.StockHeaderId);
            StockHeader.DocNo = pt.DocNo;
            StockHeader.DocDate = pt.DocDate;
            StockHeader.PersonId = pt.JobWorkerId;
            StockHeader.GodownId = pt.GodownId;
            StockHeader.Remark = pt.Remark;
            StockHeader.ModifiedBy = UserName;
            StockHeader.ModifiedDate = DateTime.Now;
            StockHeader.ObjectState = ObjectState.Modified;
            db.JobReceiveHeader.Add(JobReceiveHeader);

            JobReceiveQAHeader JobReceiveQAHeader = db.JobReceiveQAHeader.Find(pt.JobReceiveQAHeaderId);
            JobReceiveQAHeader.DocDate = JobReceiveHeader.DocDate;
            JobReceiveQAHeader.DocNo = JobReceiveHeader.DocNo;
            JobReceiveQAHeader.QAById = JobReceiveHeader.JobReceiveById;
            JobReceiveQAHeader.Remark = JobReceiveHeader.Remark;
            JobReceiveQAHeader.ModifiedBy = UserName;
            JobReceiveQAHeader.ModifiedDate = DateTime.Now;
            JobReceiveQAHeader.ObjectState = ObjectState.Modified;
            db.JobReceiveQAHeader.Add(JobReceiveQAHeader);

            int Qty ;

            if (pt.JobReceiveSettings.isAllowtoGenerateMultipleBarcode == true)
                Qty = ReceiveLineList.Count();
            else
                Qty = 1;

            foreach (var Item in ReceiveLineList)
            {
                JobReceiveLine JobReceiveLine = db.JobReceiveLine.Find(Item.JobReceiveLineId);
                //JobReceiveLine.JobOrderLineId = pt.JobOrderLineId;
                //JobReceiveLine.ProductUidId = pt.ProductUidId;
                JobReceiveLine.isHoldForInvoice = pt.isHoldForInvoice;
                JobReceiveLine.ReasonInvoiceHold = pt.ReasonInvoiceHold;
                JobReceiveLine.Qty = pt.Qty/ Qty;
                JobReceiveLine.LossQty = 0;
                JobReceiveLine.PassQty = pt.Qty / Qty;
                JobReceiveLine.LotNo = pt.LotNo;
                JobReceiveLine.UnitConversionMultiplier = pt.UnitConversionMultiplier;
                JobReceiveLine.DealQty = pt.DealQty / Qty;
                JobReceiveLine.DealUnitId = pt.DealUnitId;
                JobReceiveLine.Weight = pt.Weight / Qty;
                JobReceiveLine.Sr = 1;
                JobReceiveLine.ModifiedBy = UserName;
                JobReceiveLine.ModifiedDate = DateTime.Now;
                JobReceiveLine.ObjectState = ObjectState.Modified;
                db.JobReceiveLine.Add(JobReceiveLine);



                Stock Stock = db.Stock.Find(pt.StockId);

                if (Stock != null)
                {
                    Stock.DocDate = JobReceiveHeader.DocDate;
                    //Stock.ProductId = pt.ProductId;
                    //Stock.ProcessId = JobReceiveHeader.ProcessId;
                    Stock.GodownId = JobReceiveHeader.GodownId;
                    Stock.LotNo = JobReceiveLine.LotNo;
                    //Stock.ProductUidId = JobReceiveLine.ProductUidId;
                    Stock.Qty_Iss = 0;
                    Stock.Qty_Rec = JobReceiveLine.Qty;
                    Stock.Remark = JobReceiveLine.Remark;
                    Stock.ModifiedBy = UserName;
                    Stock.ModifiedDate = DateTime.Now;
                    Stock.ObjectState = ObjectState.Modified;
                    db.Stock.Add(Stock);
                }

                JobOrderLine JobOrderLine = db.JobOrderLine.Find(pt.JobOrderLineId);
                if (pt.Rate != pt.XRate)
                {
                    JobOrderLine.Rate = pt.Rate;
                    JobOrderLine.ObjectState = ObjectState.Modified;
                    db.JobOrderLine.Add(JobOrderLine);
                }

                JobReceiveQALine JobReceiveQALine = db.JobReceiveQALine.Find(Item.JobReceiveQALineId);
                JobReceiveQALine.Weight = JobReceiveLine.Weight;
                JobReceiveQALine.UnitConversionMultiplier = JobReceiveLine.UnitConversionMultiplier;
                JobReceiveQALine.DealQty = JobReceiveLine.DealQty;
                JobReceiveQALine.FailQty = 0;
                JobReceiveQALine.FailDealQty = 0;
                JobReceiveQALine.ModifiedBy = UserName;
                JobReceiveQALine.ModifiedDate = DateTime.Now;
                new JobReceiveQALineService(db, _unitOfWork).Update(JobReceiveQALine, UserName);


                JobReceiveQALineExtended JobReceiveQALineExtended = db.JobReceiveQALineExtended.Find(pt.JobReceiveQALineId);
                JobReceiveQALineExtended.Length = pt.Length;
                JobReceiveQALineExtended.Width = pt.Width;
                JobReceiveQALineExtended.Height = pt.Height;
                JobReceiveQALineExtended.ObjectState = ObjectState.Modified;
                db.JobReceiveQALineExtended.Add(JobReceiveQALineExtended);


                List<QAGroupLineLineViewModel> QAGroupLineLineList = pt.QAGroupLine;

                if (QAGroupLineLineList != null)
                {
                    foreach (var item in QAGroupLineLineList)
                    {
                        if (item.JobReceiveQAAttributeId != null && item.JobReceiveQAAttributeId != 0)
                        {
                            JobReceiveQAAttribute JobReceiveQAAttribute = Find((int)item.JobReceiveQAAttributeId);
                            JobReceiveQAAttribute.QAGroupLineId = item.QAGroupLineId;
                            JobReceiveQAAttribute.Value = item.Value;
                            JobReceiveQAAttribute.Remark = item.Remarks;
                            JobReceiveQAAttribute.ModifiedBy = UserName;
                            JobReceiveQAAttribute.ModifiedDate = DateTime.Now;
                            JobReceiveQAAttribute.ObjectState = ObjectState.Modified;
                            db.JobReceiveQAAttribute.Add(JobReceiveQAAttribute);
                        }
                        else
                        {
                            JobReceiveQAAttribute JobReceiveQAAttribute = new JobReceiveQAAttribute();
                            JobReceiveQAAttribute.JobReceiveQALineId = JobReceiveQALine.JobReceiveQALineId;
                            JobReceiveQAAttribute.QAGroupLineId = item.QAGroupLineId;
                            JobReceiveQAAttribute.Value = item.Value;
                            JobReceiveQAAttribute.Remark = item.Remarks;
                            JobReceiveQAAttribute.CreatedBy = UserName;
                            JobReceiveQAAttribute.ModifiedBy = UserName;
                            JobReceiveQAAttribute.CreatedDate = DateTime.Now;
                            JobReceiveQAAttribute.ModifiedDate = DateTime.Now;
                            JobReceiveQAAttribute.ObjectState = ObjectState.Added;
                            db.JobReceiveQAAttribute.Add(JobReceiveQAAttribute);
                        }
                    }
                }
            }
        }




        public IQueryable<JobReceivePendingToQAIndex> GetJobReceiveQAAttributeList(int DocTypeId, string Uname)
        {
            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            var settings = new JobReceiveQASettingsService(db).GetJobReceiveQASettingsForDocument(DocTypeId, DivisionId, SiteId);

            string[] contraDocTypes = null;
            if (!string.IsNullOrEmpty(settings.filterContraDocTypes)) { contraDocTypes = settings.filterContraDocTypes.Split(",".ToCharArray()); }
            else { contraDocTypes = new string[] { "NA" }; }

            return (from L in db.ViewJobReceiveBalanceForQA
                    join Jrl in db.JobReceiveLine on L.JobReceiveLineId equals Jrl.JobReceiveLineId into JobReceiveLineTable
                    from JobReceiveLineTab in JobReceiveLineTable.DefaultIfEmpty()
                    join H in db.JobReceiveHeader on L.JobReceiveHeaderId equals H.JobReceiveHeaderId into JobReceiveHeaderTable from JobReceiveHeaderTab in JobReceiveHeaderTable.DefaultIfEmpty()
                    orderby JobReceiveHeaderTab.DocDate, JobReceiveHeaderTab.DocNo
                    where JobReceiveHeaderTab.SiteId == SiteId && JobReceiveHeaderTab.DivisionId == DivisionId 
                    && (string.IsNullOrEmpty(settings.filterContraDocTypes) ? 1 == 1 : contraDocTypes.Contains(JobReceiveHeaderTab.DocTypeId.ToString()))
                    select new JobReceivePendingToQAIndex
                    {
                        JobReceiveHeaderId =JobReceiveHeaderTab.JobReceiveHeaderId,
                        JobReceiveLineId = L.JobReceiveLineId,
                        DocTypeName = JobReceiveHeaderTab.DocType.DocumentTypeName,
                        DocDate = JobReceiveHeaderTab.DocDate,
                        DocNo = JobReceiveHeaderTab.DocNo,
                        JobWorkerName = JobReceiveHeaderTab.JobWorker.Name,
                        ProductName = JobReceiveLineTab.JobOrderLine.Product.ProductName,
                        ProductUidName = JobReceiveLineTab.ProductUid.ProductUidName,
                        DocTypeId = DocTypeId
                    }
                );
        }

        public List<QAGroupLineLineViewModel> GetJobReceiveQAAttribute(int JobReceiveLineid)
        {
            List<QAGroupLineLineViewModel> JobReceiveQAAttribute = (from L in db.JobReceiveLine
                                                                    join H in db.JobReceiveHeader on L.JobReceiveHeaderId equals H.JobReceiveHeaderId into JobReceiveHeaderTable
                                                                    from JobReceiveHeaderTab in JobReceiveHeaderTable.DefaultIfEmpty()
                                                                    join Jol in db.JobOrderLine on L.JobOrderLineId equals Jol.JobOrderLineId into JobOrderLineTable
                                                                    from JobOrderLineTab in JobOrderLineTable.DefaultIfEmpty()
                                                                    join Pp in db.ProductProcess on new { X1 = JobOrderLineTab.ProductId, X2 = JobReceiveHeaderTab.ProcessId } equals new { X1 = Pp.ProductId, X2 = (Pp.ProcessId ?? 0) } into ProductProcessTable
                                                                    from ProductProcessTab in ProductProcessTable.DefaultIfEmpty()
                                                                    join QAGl in db.QAGroupLine on ProductProcessTab.QAGroupId equals QAGl.QAGroupId into QAGroupLineTable
                                                                    from QAGroupLineTab in QAGroupLineTable.DefaultIfEmpty()
                                                                    where L.JobReceiveLineId == JobReceiveLineid && ((int?)QAGroupLineTab.QAGroupLineId ?? 0) != 0
                                                                    select new QAGroupLineLineViewModel
                                                                    {
                                                                        QAGroupLineId = QAGroupLineTab.QAGroupLineId,
                                                                        DefaultValue = QAGroupLineTab.DefaultValue,
                                                                        Value = QAGroupLineTab.DefaultValue,
                                                                        Name = QAGroupLineTab.Name,
                                                                        DataType = QAGroupLineTab.DataType,
                                                                        ListItem = QAGroupLineTab.ListItem
                                                                    }).ToList();


            return JobReceiveQAAttribute;
        }



        public WeavingReceiveQACombinedViewModel GetJobReceiveDetailForEdit(int JobReceiveHeaderId)//JobReceiveHeaderId
        {




            //WeavingReceiveQACombinedViewModel WeavingReceiveQADetail = (from H in db.JobReceiveHeader
            //                                                            join L in db.JobReceiveLine on H.JobReceiveHeaderId equals L.JobReceiveHeaderId into JobReceiveLineTable
            //                                                            from JobReceiveLineTab in JobReceiveLineTable.DefaultIfEmpty()
            //                                                            join Jrql in db.JobReceiveQALine on JobReceiveLineTab.JobReceiveLineId equals Jrql.JobReceiveLineId into JobReceiveQaLineTable
            //                                                            from JobReceiveQALineTab in JobReceiveQaLineTable.DefaultIfEmpty()
            //                                                            join Jol in db.JobOrderLine on JobReceiveLineTab.JobOrderLineId equals Jol.JobOrderLineId into JobOrderLineTable
            //                                                            from JobOrderLineTab in JobOrderLineTable.DefaultIfEmpty()
            //                                                            join FP in db.FinishedProduct on JobOrderLineTab.ProductId equals FP.ProductId into FinishedProductTable
            //                                                            from FinishedProductTab in FinishedProductTable.DefaultIfEmpty()
            //                                                            join Ld in db.JobReceiveQALineExtended on JobReceiveQALineTab.JobReceiveQALineId equals Ld.JobReceiveQALineId into JobReceiveQALineExtendedTable
            //                                                            from JobReceiveQALineExtendedTab in JobReceiveQALineExtendedTable.DefaultIfEmpty()
            //                                                            where JobReceiveLineTab.JobReceiveHeaderId == JobReceiveHeaderId
            //                                                            select new WeavingReceiveQACombinedViewModel
            //                                                         {
            //                                                             JobReceiveHeaderId = H.JobReceiveHeaderId,
            //                                                             JobReceiveLineId = JobReceiveLineTab.JobReceiveLineId,
            //                                                             JobReceiveQALineId = JobReceiveQALineTab.JobReceiveQALineId,
            //                                                             JobReceiveQAHeaderId = JobReceiveQALineTab.JobReceiveQAHeaderId,
            //                                                             StockHeaderId = H.StockHeaderId ?? 0,
            //                                                             StockId = JobReceiveLineTab.StockId ?? 0,
            //                                                             JobOrderLineId = (int) JobReceiveLineTab.JobOrderLineId,
            //                                                             JobOrderHeaderDocNo = JobOrderLineTab.JobOrderHeader.DocNo,
            //                                                             CostCenterNo= JobOrderLineTab.JobOrderHeader.CostCenter.CostCenterName,
            //                                                             GodownId = H.GodownId,
            //                                                             JobWorkerId = H.JobWorkerId,
            //                                                             ProductUidId = JobReceiveLineTab.ProductUidId,
            //                                                             ProductUidName = JobReceiveLineTab.ProductUid.ProductUidName,
            //                                                             ProductId = JobOrderLineTab.ProductId,
            //                                                             ProductName = JobOrderLineTab.Product.ProductName,
            //                                                             LotNo= JobReceiveLineTab.LotNo,
            //                                                             Qty = JobReceiveLineTab.Qty,
            //                                                             UnitId = JobOrderLineTab.Product.UnitId,
            //                                                             DealUnitId = JobReceiveLineTab.DealUnitId,
            //                                                                //UnitConversionMultiplier = JobReceiveLineTab.UnitConversionMultiplier,
            //                                                                //DealQty = JobReceiveLineTab.DealQty,
            //                                                             UnitConversionMultiplier = JobReceiveQALineTab.UnitConversionMultiplier,
            //                                                             DealQty = JobReceiveQALineTab.DealQty,
            //                                                             Weight = JobReceiveLineTab.Weight,
            //                                                             UnitDecimalPlaces = JobOrderLineTab.Product.Unit.DecimalPlaces,
            //                                                             DealUnitDecimalPlaces = JobOrderLineTab.DealUnit.DecimalPlaces,
            //                                                             Rate = JobOrderLineTab.Rate,
            //                                                             XRate = JobOrderLineTab.Rate,
            //                                                             Amount = JobReceiveLineTab.DealQty * JobOrderLineTab.Rate,
            //                                                             PenaltyRate = JobReceiveLineTab.PenaltyRate,
            //                                                             PenaltyAmt = JobReceiveLineTab.PenaltyAmt,
            //                                                             DivisionId = H.DivisionId,
            //                                                             SiteId = H.SiteId,
            //                                                             ProcessId = H.ProcessId,
            //                                                             DocDate = H.DocDate,
            //                                                             DocTypeId = H.DocTypeId,
            //                                                             DocNo = H.DocNo,
            //                                                             ProductQualityName = FinishedProductTab.ProductQuality.ProductQualityName,
            //                                                             JobReceiveById = JobReceiveLineTab.JobReceiveHeader.JobReceiveById,
            //                                                             Remark = H.Remark,
            //                                                             isHoldForInvoice = (JobReceiveLineTab.isHoldForInvoice == null ? false : (bool)JobReceiveLineTab.isHoldForInvoice),
            //                                                             ReasonInvoiceHold = JobReceiveLineTab.ReasonInvoiceHold,
            //                                                             Length = JobReceiveQALineExtendedTab.Length,
            //                                                             OrderLength = JobReceiveQALineExtendedTab.Length,
            //                                                             Width = JobReceiveQALineExtendedTab.Width,
            //                                                             OrderWidth = JobReceiveQALineExtendedTab.Width,
            //                                                             Height = JobReceiveQALineExtendedTab.Height,
            //                                                             Status=H.Status,
            //                                                             CreatedBy=H.CreatedBy,
            //                                                             CreatedDate=H.CreatedDate,
            //                                                         }).FirstOrDefault();

            string mQry = "";


            mQry = @"SELECT H.JobReceiveHeaderId,  Max(L.JobReceiveLineId) AS JobReceiveLineId,  
					Max(RQ.JobReceiveQALineId) AS JobReceiveQALineId, Max(RQ.JobReceiveQAHeaderId) AS JobReceiveQAHeaderId, Max(H.StockHeaderId) AS StockHeaderId,
                    Max(L.StockId) AS StockId, Max(L.JobOrderLineId) AS JobOrderLineId, Max(JOH.DocNo) AS  JobOrderHeaderDocNo, Max(CC.CostCenterName) AS  CostCenterNo,
                    Max(H.GodownId) AS GodownId, Max(H.JobWorkerId) AS JobWorkerId,  
                    Min(L.ProductUidId) AS ProductUidId,   Min(PU.ProductUidName) AS ProductUidName, 
                    Max(L.ProductUidId) AS ToProductUidId,   Max(PU.ProductUidName) AS ToProductUidName,   
                    Max(JOL.ProductId) AS ProductId,
                    Max(P.ProductName) AS ProductName,  Max(L.LotNo) AS LotNo,  sum(L.Qty) AS Qty,    Max(P.UnitId) AS UnitId, 
                    Max(L.DealUnitId) AS DealUnitId,   Max(RQ.UnitConversionMultiplier) AS UnitConversionMultiplier,
                    sum(RQ.DealQty) AS DealQty, sum(L.Weight) AS Weight, Max(U.DecimalPlaces) AS UnitDecimalPlaces, Max(DU.DecimalPlaces) AS   DealUnitDecimalPlaces ,
                    Max(JOL.Rate) AS Rate , Max(JOL.Rate) AS  XRate, sum(L.DealQty)* max(JOL.Rate) AS  Amount,
                    Max(L.PenaltyRate) AS  PenaltyRate, sum(L.PenaltyAmt) AS PenaltyAmt,  Max(H.DivisionId) AS DivisionId , Max(H.SiteId) AS SiteId, 
                    Max(H.ProcessId) AS ProcessId,
                    Max(H.DocDate) DocDate, Max(H.DocTypeId) AS DocTypeId, Max(H.DocNo) AS DocNo, 
                    Max(PQ.ProductQualityName) ProductQualityName,
                    Max(H.JobReceiveById) AS JobReceiveById, Max(H.Remark) AS Remark, 
                    convert(BIT,Max(convert(INT,L.isHoldForInvoice))) AS isHoldForInvoice ,  
                    Max(L.ReasonInvoiceHold) AS ReasonInvoiceHold,
                    Max(RQA.Length) AS Length, Max(RQA.Length) OrderLength, Max(RQA.Width) AS Width,  Max(RQA.Width) OrderWidth,
                    Max(RQA.Height) AS Height,  Max(H.Status) AS Status,  Max(H.CreatedBy) AS CreatedBy, Max(H.CreatedDate) AS CreatedDate
                    FROM web.JobReceiveHeaders H WITH(Nolock)
                    LEFT JOIN web.JobReceiveLines L WITH(Nolock) ON L.JobReceiveHeaderId = H.JobReceiveHeaderId
                    LEFT JOIN web.JobReceiveQALines RQ WITH(Nolock) ON RQ.JobReceiveLineId = L.JobReceiveLineId
                    LEFT JOIN web.JobReceiveQALineExtendeds RQA WITH(Nolock) ON RQA.JobReceiveQALineId = RQ.JobReceiveQALineId
                    LEFT JOIN web.JobOrderLines JOL WITH(Nolock) ON JOL.JobOrderLineId = L.JobOrderLineId
                    LEFT JOIN web.JobOrderHeaders JOH WITH(Nolock) ON JOH.JobOrderHeaderId = JOL.JobOrderHeaderId
                    LEFT JOIN web.CostCenters CC WITH(Nolock) ON CC.CostCenterId = JOH.CostCenterId
                    LEFT JOIN web.ProductUids PU WITH(Nolock) ON PU.ProductUIDId = L.ProductUidId
                    LEFT JOIN web.Products P WITH(Nolock) ON P.ProductId = JOL.ProductId
                    LEFT JOIN web.FinishedProduct FP WITH(Nolock) ON FP.ProductId = P.ProductId
                    LEFT JOIN web.ProductQualities PQ WITH(Nolock) ON PQ.ProductQualityId = FP.ProductQualityId
                    LEFT JOIN web.Units U ON U.UnitId = P.UnitId
                    LEFT JOIN web.Units DU ON DU.UnitId = JOL.DealUnitId
                    WHERE H.JobReceiveHeaderId =  " + JobReceiveHeaderId + @"       
                    GROUP BY H.JobReceiveHeaderId ";

            WeavingReceiveQACombinedViewModel WeavingReceiveQADetail = db.Database.SqlQuery<WeavingReceiveQACombinedViewModel>(mQry).ToList().FirstOrDefault();


            DocumentType DT = new DocumentTypeService(_unitOfWork).Find(WeavingReceiveQADetail.DocTypeId);
          
            if (DT.DocumentTypeName == "Weaving Bazar(Over Tufting)")
            mQry = @"SELECT B1.Code AS BuyerName
							FROM web.JobReceiveHeaders H  WITH(Nolock)
							LEFT JOIN web.JobReceiveLines L  WITH(Nolock) ON L.JobReceiveHeaderId = H.JobReceiveHeaderId
                            LEFT JOIN web.ProductUids PU  WITH(Nolock) ON PU.ProductUIDId = L.ProductUidId OR PU.LotNo = L.LotNo
                            LEFT JOIN web.JobReceiveHeaders JRH ON JRH.JobReceiveHeaderId = PU.GenDocId AND JRH.DocTYpeId = PU.GenDocTYpeId
                            LEFT JOIN web.JobReceiveLines JRL ON JRL.JobReceiveHeaderId = JRH.JobReceiveHeaderId
                            LEFT JOIN web.JobOrderLines JOL1  WITH(Nolock) ON JOL1.JobOrderLineId = JRL.JobOrderLineId
                            LEFT JOIN web.ProdOrderLines POL1 WITH(Nolock) ON POL1.ProdorderLineId = JOL1.ProdorderLineId
                            LEFT JOIN web.ProdOrderHeaders POH1 WITH(Nolock) ON POH1.ProdorderHeaderId = POL1.ProdorderHeaderId
                            LEFT JOIN web.People B1 WITH(Nolock) ON B1.PersonId = POH1.BuyerId
                            WHERE H.JobReceiveHeaderId =" + JobReceiveHeaderId.ToString() + @" ";
            else
                mQry = @"SELECT isnull(B.Code,B1.Code) AS BuyerName
							FROM web.JobReceiveHeaders H  WITH(Nolock)
							LEFT JOIN web.JobReceiveLines L  WITH(Nolock) ON L.JobReceiveHeaderId = H.JobReceiveHeaderId
                            LEFT JOIN web.JobOrderLines JOL  WITH(Nolock) ON JOL.JobOrderLineId = L.JobOrderLineId
                            LEFT JOIN web.ProdOrderLines POL WITH(Nolock) ON POL.ProdorderLineId = JOL.ProdorderLineId
                            LEFT JOIN web.ProdOrderHeaders POH WITH(Nolock) ON POH.ProdorderHeaderId = POL.ProdorderHeaderId
                            LEFT JOIN web.People B WITH(Nolock) ON B.PersonId = POH.BuyerId
                            LEFT JOIN web.JobOrderLines JOL1  WITH(Nolock) ON JOL1.JobOrderLineId = POL.referenceDocLineId
                            LEFT JOIN web.ProdOrderLines POL1 WITH(Nolock) ON POL1.ProdorderLineId = JOL1.ProdorderLineId
                            LEFT JOIN web.ProdOrderHeaders POH1 WITH(Nolock) ON POH1.ProdorderHeaderId = POL1.ProdorderHeaderId
                            LEFT JOIN web.People B1 WITH(Nolock) ON B1.PersonId = POH1.BuyerId
                            WHERE H.JobReceiveHeaderId =" + JobReceiveHeaderId.ToString() + @" ";

            IEnumerable<BuyerDetail> JobOrderBalanceList = db.Database.SqlQuery<BuyerDetail>(mQry).ToList();


            WeavingReceiveQADetail.BuyerName = JobOrderBalanceList.FirstOrDefault().BuyerName;

            if (WeavingReceiveQADetail != null)
            {
                ProductDimensions ProductDimensions = new ProductService(_unitOfWork).GetProductDimensions(WeavingReceiveQADetail.ProductId, WeavingReceiveQADetail.DealUnitId, WeavingReceiveQADetail.DocTypeId);
                if (ProductDimensions != null)
                {
                    WeavingReceiveQADetail.DimensionUnitDecimalPlaces = ProductDimensions.DimensionUnitDecimalPlaces;
                }
            }

            return WeavingReceiveQADetail;
        }

        public WeavingReceiveQACombinedViewModel GetJobReceiveDetailForNextCreate(string UserName)
        {
            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            var R1 = (from H in db.JobReceiveHeader
                      where H.SiteId == SiteId && H.DivisionId == DivisionId && H.CreatedBy == UserName && H.DocTypeId ==448
                      select H.JobReceiveHeaderId).Max();

            var JRL = (from H in db.JobReceiveLine
                       where H.JobReceiveHeaderId == R1
                       select new
                       {
                           JobReceiveHeaderId=  H.JobReceiveHeaderId,
                           JobOrderLineId = H.JobOrderLineId,
                       }).FirstOrDefault();

            WeavingReceiveQACombinedViewModel LastWeavingReceive = (from H in db.JobReceiveHeader
                                                                        join L in db.JobReceiveLine on H.JobReceiveHeaderId equals L.JobReceiveHeaderId into JobReceiveLineTable
                                                                        from JobReceiveLineTab in JobReceiveLineTable.DefaultIfEmpty()
                                                                        join Jrql in db.JobReceiveQALine on JobReceiveLineTab.JobReceiveLineId equals Jrql.JobReceiveLineId into JobReceiveQaLineTable
                                                                        from JobReceiveQALineTab in JobReceiveQaLineTable.DefaultIfEmpty()
                                                                        join Jol in db.JobOrderLine on JobReceiveLineTab.JobOrderLineId equals Jol.JobOrderLineId into JobOrderLineTable
                                                                        from JobOrderLineTab in JobOrderLineTable.DefaultIfEmpty()
                                                                        join FP in db.FinishedProduct on JobOrderLineTab.ProductId equals FP.ProductId into FinishedProductTable
                                                                        from FinishedProductTab in FinishedProductTable.DefaultIfEmpty()
                                                                        join Ld in db.JobReceiveQALineExtended on JobReceiveQALineTab.JobReceiveQALineId equals Ld.JobReceiveQALineId into JobReceiveQALineExtendedTable
                                                                        from JobReceiveQALineExtendedTab in JobReceiveQALineExtendedTable.DefaultIfEmpty()
                                                                        where JobReceiveLineTab.JobReceiveHeaderId == R1
                                                                        select new WeavingReceiveQACombinedViewModel
                                                                        {
                                                                            JobReceiveHeaderId = H.JobReceiveHeaderId,
                                                                            JobReceiveLineId = JobReceiveLineTab.JobReceiveLineId,
                                                                            JobReceiveQALineId = JobReceiveQALineTab.JobReceiveQALineId,
                                                                            JobReceiveQAHeaderId = JobReceiveQALineTab.JobReceiveQAHeaderId,
                                                                            StockHeaderId = H.StockHeaderId ?? 0,
                                                                            StockId = JobReceiveLineTab.StockId ?? 0,
                                                                            JobOrderLineId = (int)JobReceiveLineTab.JobOrderLineId,
                                                                            JobOrderHeaderDocNo = JobOrderLineTab.JobOrderHeader.DocNo,
                                                                            CostCenterNo = JobOrderLineTab.JobOrderHeader.CostCenter.CostCenterName,
                                                                            GodownId = H.GodownId,
                                                                            JobWorkerId = H.JobWorkerId,
                                                                            ProductUidId = JobReceiveLineTab.ProductUidId,
                                                                            ProductUidName = JobReceiveLineTab.ProductUid.ProductUidName,
                                                                            ProductId = JobOrderLineTab.ProductId,
                                                                            ProductName = JobOrderLineTab.Product.ProductName,
                                                                            LotNo = JobReceiveLineTab.LotNo,
                                                                            Qty = JobReceiveLineTab.Qty,
                                                                            UnitId = JobOrderLineTab.Product.UnitId,
                                                                            DealUnitId = JobReceiveLineTab.DealUnitId,
                                                                            UnitConversionMultiplier = JobReceiveQALineTab.UnitConversionMultiplier,
                                                                            DealQty = JobReceiveQALineTab.DealQty,
                                                                            Weight = JobReceiveLineTab.Weight,
                                                                            UnitDecimalPlaces = JobOrderLineTab.Product.Unit.DecimalPlaces,
                                                                            DealUnitDecimalPlaces = JobOrderLineTab.DealUnit.DecimalPlaces,
                                                                            Rate = JobOrderLineTab.Rate,
                                                                            XRate = JobOrderLineTab.Rate,
                                                                            Amount = JobReceiveLineTab.DealQty * JobOrderLineTab.Rate,
                                                                            PenaltyRate = JobReceiveLineTab.PenaltyRate,
                                                                            PenaltyAmt = JobReceiveLineTab.PenaltyAmt,
                                                                            DivisionId = H.DivisionId,
                                                                            SiteId = H.SiteId,
                                                                            ProcessId = H.ProcessId,
                                                                            DocDate = H.DocDate,
                                                                            DocTypeId = H.DocTypeId,
                                                                            DocNo = H.DocNo,
                                                                            ProductQualityName = FinishedProductTab.ProductQuality.ProductQualityName,
                                                                            JobReceiveById = JobReceiveLineTab.JobReceiveHeader.JobReceiveById,
                                                                            Remark = H.Remark,
                                                                            Length = JobReceiveQALineExtendedTab.Length,
                                                                            OrderLength = JobReceiveQALineExtendedTab.Length,
                                                                            Width = JobReceiveQALineExtendedTab.Width,
                                                                            OrderWidth = JobReceiveQALineExtendedTab.Width,
                                                                            Height = JobReceiveQALineExtendedTab.Height,
                                                                            Status = H.Status,
                                                                            CreatedBy = H.CreatedBy,
                                                                            CreatedDate = H.CreatedDate,
                                                                        }).FirstOrDefault();




            var JR = (from H in db.ViewJobOrderBalance
                      join Jol in db.JobOrderLine on H.JobOrderLineId equals Jol.JobOrderLineId into JobOrderLineTable
                      from JobOrderLineTab in JobOrderLineTable.DefaultIfEmpty()
                      where H.JobOrderLineId == JRL.JobOrderLineId
                      select new WeavingReceiveQACombinedViewModel
                      {
                          JobOrderLineId = (int)H.JobOrderLineId,
                          JobOrderHeaderDocNo = JobOrderLineTab.JobOrderHeader.DocNo,
                          CostCenterNo = JobOrderLineTab.JobOrderHeader.CostCenter.CostCenterName,
                          GodownId = LastWeavingReceive.GodownId,
                          JobWorkerId = H.JobWorkerId,
                          ProductId = JobOrderLineTab.ProductId,
                          ProductName = JobOrderLineTab.Product.ProductName,
                          Qty = LastWeavingReceive.Qty,
                          UnitId = JobOrderLineTab.Product.UnitId,
                          DealUnitId = LastWeavingReceive.DealUnitId,
                          UnitConversionMultiplier = LastWeavingReceive.UnitConversionMultiplier,
                          UnitDecimalPlaces = JobOrderLineTab.Product.Unit.DecimalPlaces,
                          DealUnitDecimalPlaces = JobOrderLineTab.DealUnit.DecimalPlaces,
                          Rate = JobOrderLineTab.Rate,
                          DivisionId = H.DivisionId,
                          SiteId = H.SiteId,
                          ProcessId = LastWeavingReceive.ProcessId,
                          DocDate = LastWeavingReceive.DocDate,
                          DocTypeId = LastWeavingReceive.DocTypeId,
                          ProductQualityName = LastWeavingReceive.ProductQualityName,
                          JobReceiveById = LastWeavingReceive.JobReceiveById,
                          Length = LastWeavingReceive.Length,
                          OrderLength = LastWeavingReceive.OrderLength,
                          Width = LastWeavingReceive.Width,
                          OrderWidth = LastWeavingReceive.OrderWidth,
                          Height = LastWeavingReceive.Height,
                          Weight = LastWeavingReceive.Weight,
                      }).FirstOrDefault();

            

            if (JR != null)
            {
                var temp = GetQAGroupLine(LastWeavingReceive.JobReceiveQALineId);
                JR.QAGroupLine = temp;

                ProductDimensions ProductDimensions = new ProductService(_unitOfWork).GetProductDimensions(LastWeavingReceive.ProductId, LastWeavingReceive.DealUnitId, LastWeavingReceive.DocTypeId);
                if (ProductDimensions != null)
                {
                    JR.DimensionUnitDecimalPlaces = ProductDimensions.DimensionUnitDecimalPlaces;
                }
            }

            return JR;
        }

        public List<QAGroupLineLineViewModel> GetQAGroupLine(int JobReceiveQALineid)
        {
            List<QAGroupLineLineViewModel> QAGroupLineList = (from L in db.JobReceiveQAAttribute
                                                                    where L.JobReceiveQALineId == JobReceiveQALineid
                                                                    select new QAGroupLineLineViewModel
                                                                    {
                                                                        QAGroupLineId = L.QAGroupLineId,
                                                                        DefaultValue = L.Value,
                                                                        Value = L.Value,
                                                                        Name = L.QAGroupLine.Name,
                                                                        DataType = L.QAGroupLine.DataType,
                                                                        ListItem = L.QAGroupLine.ListItem,
                                                                        Remarks = L.Remark
                                                                    }).ToList();


            return QAGroupLineList;
        }

        public WeavingReceiveQACombinedViewModel_ByProductUid GetProductUidDetail(int ProductUidId, bool ? ForOverTufted)//ProductUidId
        {

            int IForOverTufted = 0;
            if (ForOverTufted == true)
                IForOverTufted = 1;

            string ConnectionString = (string)System.Web.HttpContext.Current.Session["DefaultConnectionString"];
            string ProcName = "Web.Proc_BarcodeInfo";
            DataSet ds = new DataSet();
            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();
                using (SqlCommand cmd = new SqlCommand(ProcName))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = sqlConnection;
                    cmd.Parameters.AddWithValue("@ProductUIDId", ProductUidId);
                    cmd.Parameters.AddWithValue("@ForOverTufted", IForOverTufted);
                    using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                    {
                        adp.Fill(ds);
                    }
                }
            }


            DataTable dt2 = ds.Tables[0];


            WeavingReceiveQACombinedViewModel_ByProductUid temp = new WeavingReceiveQACombinedViewModel_ByProductUid();
            

            foreach (DataRow dr in dt2.Rows)
            {
                var values = dr.ItemArray;


                temp.ProductUidName = values[0].ToString();
                if (values[2].ToString() != "")
                {
                    temp.DocNo = values[1].ToString();
                    temp.JobReceiveHeaderId = (int)Convert.ToInt32(values[2].ToString());
                    temp.JobReceiveLineId = (int)Convert.ToInt32(values[3].ToString());
                    temp.JobReceiveQALineId = (int)Convert.ToInt32(values[4].ToString());
                    temp.JobReceiveQAHeaderId = (int)Convert.ToInt32(values[5].ToString());
                    temp.StockHeaderId = (int?)Convert.ToInt32(values[6].ToString()) ?? 0;
                    temp.StockId = (int?)Convert.ToInt32(values[7].ToString()) ?? 0;
                    temp.JobOrderLineId = (int)Convert.ToInt32(values[8].ToString());
                    temp.JobOrderHeaderDocNo = values[9].ToString();
                    temp.JobWorkerId = (int)Convert.ToInt32(values[10].ToString());
                    temp.ProductUidId = (int)Convert.ToInt32(values[11].ToString());
                    temp.ProductId = (int)Convert.ToInt32(values[12].ToString());
                    temp.ProductCategoryName = values[13].ToString();
                    temp.ColourName = values[14].ToString();
                    temp.ProductGroupName = values[15].ToString();
                    temp.ProductName = values[16].ToString();
                    temp.ProductQualityName = values[17].ToString();
                    temp.DivisionId = (int)Convert.ToInt32(values[18].ToString());
                    temp.SiteId = (int)Convert.ToInt32(values[19].ToString());
                    temp.ProcessId = (int)Convert.ToInt32(values[20].ToString());
                    temp.DocDate = Convert.ToDateTime(values[21].ToString());
                    temp.DocTypeId = (int)Convert.ToInt32(values[22].ToString());
                    temp.Qty = (int)Convert.ToDecimal(values[23].ToString());
                    temp.UnitId = values[24].ToString();
                    temp.DealUnitId = values[25].ToString();
                    temp.PaymentSlipNo = values[26].ToString();
                    temp.GodownId = (int)Convert.ToInt32(values[27].ToString());
                    temp.NetAmount = Convert.ToDecimal(values[28].ToString());
                    temp.TDSCom = Convert.ToDecimal(values[29].ToString());
                    temp.LotNo = values[30].ToString();
                    temp.SiteName = values[31].ToString();
                    temp.PONo = values[32].ToString();
                    temp.InvoiceNo = values[33].ToString();
                    temp.InvoiceParty = values[34].ToString();
                    temp.RollNo = values[35].ToString();
                    temp.CostcenterName = values[36].ToString();


                    temp.UnitConversionMultiplier = Convert.ToDecimal(values[37].ToString());
                    temp.DealQty = Convert.ToDecimal(values[38].ToString());
                    temp.Weight = Math.Round(Convert.ToDecimal(values[39].ToString()), 3);
                    temp.UnitDecimalPlaces = Convert.ToByte(values[40].ToString());
                    temp.DealUnitDecimalPlaces = Convert.ToByte(values[40].ToString());
                    temp.Rate = Math.Round(Convert.ToDecimal(values[41].ToString()), 2);
                    temp.XRate = Convert.ToDecimal(values[41].ToString());
                    temp.Amount = Math.Round(Convert.ToDecimal(values[38].ToString()) * Convert.ToDecimal(values[41].ToString()), 2);
                    temp.Remark = values[42].ToString();
                    temp.Length = Convert.ToDecimal(values[43].ToString());
                    temp.OrderLength = Convert.ToDecimal(values[43].ToString());
                    temp.Width = Convert.ToDecimal(values[44].ToString());
                    temp.OrderWidth = Convert.ToDecimal(values[44].ToString());
                    temp.Height = Convert.ToDecimal(values[45].ToString());


                    //temp.PenaltyRate = JobReceiveLineTab.PenaltyRate,
                    //temp.PenaltyAmt = JobReceiveLineTab.PenaltyAmt,
                    //                                                           
                    //                                                            
                    //      
                }                                


            }

            return temp;



            //WeavingReceiveQACombinedViewModel_ByProductUid LastWeavingReceive = (from PU in db.ProductUid
            //                                                                         join H in db.JobReceiveHeader on PU.GenDocId equals H.JobReceiveHeaderId into HTable
            //                                                                         from HTab in HTable.DefaultIfEmpty()
            //                                                                         join L in db.JobReceiveLine on HTab.JobReceiveHeaderId equals L.JobReceiveHeaderId into JobReceiveLineTable
            //                                                            from JobReceiveLineTab in JobReceiveLineTable.DefaultIfEmpty()
            //                                                            join Jrql in db.JobReceiveQALine on JobReceiveLineTab.JobReceiveLineId equals Jrql.JobReceiveLineId into JobReceiveQaLineTable
            //                                                            from JobReceiveQALineTab in JobReceiveQaLineTable.DefaultIfEmpty()
            //                                                            join Jol in db.JobOrderLine on JobReceiveLineTab.JobOrderLineId equals Jol.JobOrderLineId into JobOrderLineTable
            //                                                            from JobOrderLineTab in JobOrderLineTable.DefaultIfEmpty()
            //                                                            join JIL in db.JobInvoiceLine on JobReceiveLineTab.JobReceiveLineId equals JIL.JobReceiveLineId into JILTable
            //                                                            from JILTab in JILTable.DefaultIfEmpty()
            //                                                            join JIH in db.JobInvoiceHeader on JILTab.JobInvoiceHeaderId equals JIH.JobInvoiceHeaderId into JIHTable
            //                                                            from JIHTab in JIHTable.DefaultIfEmpty()
            //                                                            join SOL in db.SaleOrderLine on PU.SaleOrderLineId equals SOL.SaleOrderLineId into SOLTable
            //                                                            from SOLTab in SOLTable.DefaultIfEmpty()
            //                                                            join PL in db.PackingLine on JobReceiveLineTab.ProductUidId equals PL.ProductUidId into PLTable
            //                                                            from PLTab in PLTable.DefaultIfEmpty()
            //                                                            join SD in db.SaleDispatchLine on PLTab.PackingLineId equals SD.PackingLineId into SDTable
            //                                                            from SDTab in SDTable.DefaultIfEmpty()
            //                                                            join SI in db.SaleInvoiceLine on SDTab.SaleDispatchLineId equals SI.SaleDispatchLineId into SITable
            //                                                            from SITab in SITable.DefaultIfEmpty()
            //                                                            join SIH in db.SaleInvoiceHeader on SITab.SaleInvoiceHeaderId equals SIH.SaleInvoiceHeaderId into SIHTable
            //                                                            from SIHTab in SIHTable.DefaultIfEmpty()
            //                                                            join SIB in db.Persons on SIHTab.SaleToBuyerId equals SIB.PersonID into SIBTable
            //                                                            from SIBTab in SIBTable.DefaultIfEmpty()
            //                                                            join FP in db.FinishedProduct on JobOrderLineTab.ProductId equals FP.ProductId into FinishedProductTable
            //                                                            from FinishedProductTab in FinishedProductTable.DefaultIfEmpty()
            //                                                            join P in db.Product on JobOrderLineTab.ProductId equals P.ProductId into PTable
            //                                                            from PTab in PTable.DefaultIfEmpty()
            //                                                            join Ld in db.JobReceiveQALineExtended on JobReceiveQALineTab.JobReceiveQALineId equals Ld.JobReceiveQALineId into JobReceiveQALineExtendedTable
            //                                                            from JobReceiveQALineExtendedTab in JobReceiveQALineExtendedTable.DefaultIfEmpty()
            //                                                            where PU.ProductUIDId == ProductUidId
            //                                                            select new WeavingReceiveQACombinedViewModel_ByProductUid
            //                                                            {
            //                                                                JobReceiveHeaderId = HTab.JobReceiveHeaderId,
            //                                                                JobReceiveLineId = JobReceiveLineTab.JobReceiveLineId,
            //                                                                JobReceiveQALineId = JobReceiveQALineTab.JobReceiveQALineId,
            //                                                                JobReceiveQAHeaderId = JobReceiveQALineTab.JobReceiveQAHeaderId,
            //                                                                StockHeaderId = HTab.StockHeaderId ?? 0,
            //                                                                StockId = JobReceiveLineTab.StockId ?? 0,
            //                                                                JobOrderLineId = (int) JobReceiveLineTab.JobOrderLineId,
            //                                                                JobOrderHeaderDocNo = JobOrderLineTab.JobOrderHeader.DocNo,
            //                                                                NetAmount = JILTab.Amount,
            //                                                                TDSCom= JILTab.Amount,
            //                                                                PaymentSlipNo =JIHTab.DocNo + "  " + JIHTab.DocDate,
            //                                                                GodownId = HTab.GodownId,
            //                                                                JobWorkerId = HTab.JobWorkerId,
            //                                                                ProductUidId = JobReceiveLineTab.ProductUidId,
            //                                                                ProductUidName = JobReceiveLineTab.ProductUid.ProductUidName,
            //                                                                ProductId = JobOrderLineTab.ProductId,
            //                                                                ProductCategoryName= FinishedProductTab.ProductCategory.ProductCategoryName,
            //                                                                ColourName = FinishedProductTab.Colour.ColourName,
            //                                                                ProductGroupName  = PTab.ProductGroup.ProductGroupName,
            //                                                                ProductName = JobOrderLineTab.Product.ProductName,
            //                                                                LotNo = JobReceiveLineTab.LotNo,
            //                                                                SiteName= HTab.Site.SiteName,
            //                                                                PONo= SOLTab.SaleOrderHeader.BuyerOrderNo +"{"+SOLTab.SaleOrderHeader.SaleToBuyer.Code +"}",
            //                                                                InvoiceNo =SITab.SaleInvoiceHeader.DocNo + "  " + SITab.SaleInvoiceHeader.DocDate, 
            //                                                                InvoiceParty = PLTab.SaleOrderLine.SaleOrderHeader.BuyerOrderNo + "{" + SIBTab.Code + "}",
            //                                                                RollNo =PLTab.BaleNo, 
            //                                                                CostcenterName = JobOrderLineTab.JobOrderHeader.CostCenter.CostCenterName,
            //                                                                Qty = JobReceiveLineTab.Qty,
            //                                                                UnitId = JobOrderLineTab.Product.UnitId,
            //                                                                DealUnitId = JobReceiveLineTab.DealUnitId,
            //                                                                UnitConversionMultiplier = JobReceiveQALineTab.UnitConversionMultiplier,
            //                                                                DealQty = JobReceiveQALineTab.DealQty,
            //                                                                Weight = Math.Round(JobReceiveLineTab.Weight,3),
            //                                                                UnitDecimalPlaces = JobOrderLineTab.Product.Unit.DecimalPlaces,
            //                                                                DealUnitDecimalPlaces = JobOrderLineTab.DealUnit.DecimalPlaces,
            //                                                                Rate = Math.Round(JobOrderLineTab.Rate,2),
            //                                                                XRate = JobOrderLineTab.Rate,
            //                                                                Amount = Math.Round(JobReceiveLineTab.DealQty * JobOrderLineTab.Rate,2),
            //                                                                PenaltyRate = JobReceiveLineTab.PenaltyRate,
            //                                                                PenaltyAmt = JobReceiveLineTab.PenaltyAmt,
            //                                                                DivisionId = HTab.DivisionId,
            //                                                                SiteId = HTab.SiteId,
            //                                                                ProcessId = HTab.ProcessId,
            //                                                                DocDate = HTab.DocDate,
            //                                                                DocTypeId = HTab.DocTypeId,
            //                                                                DocNo = HTab.DocNo,
            //                                                                ProductQualityName = FinishedProductTab.ProductQuality.ProductQualityName,
            //                                                                JobReceiveById = JobReceiveLineTab.JobReceiveHeader.JobReceiveById,
            //                                                                Remark = HTab.Remark,
            //                                                                Length = JobReceiveQALineExtendedTab.Length,
            //                                                                OrderLength = JobReceiveQALineExtendedTab.Length,
            //                                                                Width = JobReceiveQALineExtendedTab.Width,
            //                                                                OrderWidth = JobReceiveQALineExtendedTab.Width,
            //                                                                Height = JobReceiveQALineExtendedTab.Height,
            //                                                            }).FirstOrDefault();

            //if (LastWeavingReceive != null)
            //{
            //    ProductDimensions ProductDimensions = new ProductService(_unitOfWork).GetProductDimensions(LastWeavingReceive.ProductId, LastWeavingReceive.DealUnitId, LastWeavingReceive.DocTypeId);
            //    if (ProductDimensions != null)
            //    {
            //        LastWeavingReceive.DimensionUnitDecimalPlaces = ProductDimensions.DimensionUnitDecimalPlaces;
            //    }
            //}

            //return LastWeavingReceive;
        }

        public LastValues GetLastValues(int DocTypeId, string UserName)
        {
            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];


            var temp = (from H in db.JobReceiveHeader
                        where H.DocTypeId == DocTypeId && H.SiteId == SiteId && H.DivisionId == DivisionId && H.CreatedBy == UserName
                        orderby H.JobReceiveHeaderId descending
                        select new LastValues
                        {
                            JobReceiveById = H.JobReceiveById,
                            JobWorkerId =H.JobWorkerId,
                            DocDate=H.DocDate
                            
                        }).FirstOrDefault();

            return temp;
        }

        public void Delete(int id)//JobReceiveHeaderId
        {
            int MainSiteId = (from S in db.Site where S.SiteCode == "MAIN" select S).FirstOrDefault().SiteId;

            JobReceiveHeader JobReceiveHeader = db.JobReceiveHeader.Find(id);
            int StockHeaderId = (int)JobReceiveHeader.StockHeaderId;
            int ProductUidHeaderId = 0;
            int JobReceiveQAHeaderId = 0;

            IEnumerable<JobReceiveBom> JobReceiveBomList = (from L in db.JobReceiveBom where L.JobReceiveHeaderId == id select L).ToList();
            foreach (JobReceiveBom JobReceiveBom in JobReceiveBomList)
            {
                JobReceiveBom.ObjectState = Model.ObjectState.Deleted;
                db.JobReceiveBom.Remove(JobReceiveBom);
            }

            IEnumerable<JobReceiveLine> JobReceiveLineList = (from L in db.JobReceiveLine where L.JobReceiveHeaderId == id select L).ToList();
            foreach (JobReceiveLine JobReceiveLine in JobReceiveLineList)
            {



                ProductUid ProductUid = (from p in db.ProductUid
                                         where p.ProductUIDId == JobReceiveLine.ProductUidId
                                         select p).FirstOrDefault();

                if (JobReceiveLine.ProductUidId != null && JobReceiveLine.ProductUidId != 0)
                {
                    if (!(JobReceiveLine.ProductUidLastTransactionDocNo == ProductUid.LastTransactionDocNo && JobReceiveLine.ProductUidLastTransactionDocTypeId == ProductUid.LastTransactionDocTypeId) || JobReceiveHeader.SiteId == MainSiteId)
                    {


                        if ((JobReceiveHeader.DocNo != ProductUid.LastTransactionDocNo || JobReceiveHeader.DocTypeId != ProductUid.LastTransactionDocTypeId))
                        {
                            //ModelState.AddModelError("", "Bar Code Can't be deleted because this is already transfered to another process.");
                            //PrepareViewBag(vm);
                            //return PartialView("_Create", vm);
                        }

                        if (JobReceiveLine.ProductUidHeaderId == null || JobReceiveLine.ProductUidHeaderId == 0)
                        {
                            ProductUid.LastTransactionDocDate = JobReceiveLine.ProductUidLastTransactionDocDate;
                            ProductUid.LastTransactionDocId = JobReceiveLine.ProductUidLastTransactionDocId;
                            ProductUid.LastTransactionDocNo = JobReceiveLine.ProductUidLastTransactionDocNo;
                            ProductUid.LastTransactionDocTypeId = JobReceiveLine.ProductUidLastTransactionDocTypeId;
                            ProductUid.LastTransactionPersonId = JobReceiveLine.ProductUidLastTransactionPersonId;
                            ProductUid.CurrenctGodownId = JobReceiveLine.ProductUidCurrentGodownId;
                            ProductUid.CurrenctProcessId = JobReceiveLine.ProductUidCurrentProcessId;
                            ProductUid.Status = JobReceiveLine.ProductUidStatus;

                            ProductUid.ObjectState = Model.ObjectState.Modified;
                            db.ProductUid.Add(ProductUid);

                            new StockUidService(_unitOfWork).DeleteStockUidForDocLineDB(JobReceiveHeader.JobReceiveHeaderId, JobReceiveHeader.DocTypeId, JobReceiveHeader.SiteId, JobReceiveHeader.DivisionId, ref db);
                        }
                    }
                    else
                    {
                        var MainJobRec = (from p in db.JobReceiveLine
                                          join t in db.JobReceiveHeader on p.JobReceiveHeaderId equals t.JobReceiveHeaderId
                                          join d in db.DocumentType on t.DocTypeId equals d.DocumentTypeId
                                          where p.ProductUidId == JobReceiveLine.ProductUidId && t.SiteId != JobReceiveHeader.SiteId && d.DocumentTypeName == TransactionDoctypeConstants.WeavingBazarHalfTuft
                                          && p.LockReason != null
                                          select p).ToList().LastOrDefault();

                        if (MainJobRec != null)
                        {
                            MainJobRec.LockReason = null;
                            MainJobRec.ObjectState = Model.ObjectState.Modified;
                            db.JobReceiveLine.Add(MainJobRec);
                        }
                    }
                }



                ProductUidHeaderId = JobReceiveLine.ProductUidHeaderId ?? 0;
                IEnumerable<JobReceiveQALine> JobReceiveQALineList = (from L in db.JobReceiveQALine where L.JobReceiveLineId == JobReceiveLine.JobReceiveLineId select L).ToList();

                foreach (JobReceiveQALine JobReceiveQALine in JobReceiveQALineList)
                {
                    JobReceiveQAHeaderId = JobReceiveQALine.JobReceiveQAHeaderId;
                    IEnumerable<JobReceiveQAAttribute> JobReceiveQAAttributeList = (from L in db.JobReceiveQAAttribute where L.JobReceiveQALineId == JobReceiveQALine.JobReceiveQALineId select L).ToList();
                    foreach (var JobReceiveQAAttribute in JobReceiveQAAttributeList)
                    {
                        if (JobReceiveQAAttribute.JobReceiveQAAttributeId != null)
                        {
                            JobReceiveQAAttribute.ObjectState = Model.ObjectState.Deleted;
                            db.JobReceiveQAAttribute.Remove(JobReceiveQAAttribute);
                        }
                    }

                    IEnumerable<JobReceiveQAPenalty> JobReceiveQAPenaltyList = (from L in db.JobReceiveQAPenalty where L.JobReceiveQALineId == JobReceiveQALine.JobReceiveQALineId select L).ToList();
                    foreach (var JobReceiveQAPenalty in JobReceiveQAPenaltyList)
                    {
                        if (JobReceiveQAPenalty.JobReceiveQAPenaltyId != null)
                        {
                            JobReceiveQAPenalty.ObjectState = Model.ObjectState.Deleted;
                            db.JobReceiveQAPenalty.Remove(JobReceiveQAPenalty);
                        }
                    }

                    JobReceiveQALineExtended JobReceiveQALineExtended = (from L in db.JobReceiveQALineExtended where L.JobReceiveQALineId == JobReceiveQALine.JobReceiveQALineId select L).FirstOrDefault();
                    if (JobReceiveQALineExtended != null)
                    {
                        JobReceiveQALineExtended.ObjectState = Model.ObjectState.Deleted;
                        db.JobReceiveQALineExtended.Remove(JobReceiveQALineExtended);
                    }

                    JobReceiveQALine.ObjectState = ObjectState.Deleted;
                    db.JobReceiveQALine.Remove(JobReceiveQALine);
                }

                JobReceiveLineStatus JobReceiveLineStatus = (from L in db.JobReceiveLineStatus where L.JobReceiveLineId == JobReceiveLine.JobReceiveLineId select L).FirstOrDefault();
                if (JobReceiveLineStatus != null)
                {
                    JobReceiveLineStatus.ObjectState = ObjectState.Deleted;
                    db.JobReceiveLineStatus.Remove(JobReceiveLineStatus);
                }


                JobReceiveLine.ObjectState = ObjectState.Deleted;
                db.JobReceiveLine.Remove(JobReceiveLine);
            }

            JobReceiveQAHeader JobReceiveQAHeader = db.JobReceiveQAHeader.Find(JobReceiveQAHeaderId);
            JobReceiveQAHeader.ObjectState = ObjectState.Deleted;
            db.JobReceiveQAHeader.Remove(JobReceiveQAHeader);

            JobReceiveHeader.ObjectState = ObjectState.Deleted;
            db.JobReceiveHeader.Remove(JobReceiveHeader);

            if (StockHeaderId != null)
            {
                IEnumerable<Stock> StockList = (from L in db.Stock where L.StockHeaderId == StockHeaderId select L).ToList();
                foreach (var Stock in StockList)
                {
                    Stock.ObjectState = ObjectState.Deleted;
                    db.Stock.Remove(Stock);
                }

                IEnumerable<StockProcess> StockProcessList = (from L in db.StockProcess where L.StockHeaderId == StockHeaderId select L).ToList();
                foreach (var StockProcess in StockProcessList)
                {
                    StockProcess.ObjectState = ObjectState.Deleted;
                    db.StockProcess.Remove(StockProcess);
                }


                StockHeader StockHeader = db.StockHeader.Find(StockHeaderId);
                StockHeader.ObjectState = ObjectState.Deleted;
                db.StockHeader.Remove(StockHeader);
            }

            if (ProductUidHeaderId > 0)
            {
                ProductUidHeader ProductUidHeader = db.ProductUidHeader.Find(ProductUidHeaderId);
                IEnumerable<ProductUid> ProductUidList = (from P in db.ProductUid where P.ProductUidHeaderId == ProductUidHeaderId select P).ToList();
                foreach (var ProductUid in ProductUidList)
                {
                    if (ProductUid.LastTransactionDocId == null || (ProductUid.LastTransactionDocId == ProductUid.GenDocId && ProductUid.LastTransactionDocTypeId == ProductUid.GenDocTypeId))
                    {
                        ProductUid.ObjectState = Model.ObjectState.Deleted;
                        db.ProductUid.Remove(ProductUid);
                    }
                    else
                    {
                        throw new Exception("Record Cannot be deleted as its Unique Id's are in use by other documents");
                    }
                }
                ProductUidHeader.ObjectState = ObjectState.Deleted;
                db.ProductUidHeader.Remove(ProductUidHeader);
            }

        }


        public IQueryable<ComboBoxResult> GetCustomProduct(int filter, string term, int ? PersonId)
        {
            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"]; 
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            var settings = new JobReceiveSettingsService(_unitOfWork).GetJobReceiveSettingsForDocument(filter, DivisionId, SiteId);

            string[] contraSites = null;
            if (!string.IsNullOrEmpty(settings.filterContraSites)) { contraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            else { contraSites = new string[] { "NA" }; }

            string[] contraDivisions = null;
            if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { contraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { contraDivisions = new string[] { "NA" }; }

            string[] contraDocTypes = null;
            if (!string.IsNullOrEmpty(settings.filterContraDocTypes)) { contraDocTypes = settings.filterContraDocTypes.Split(",".ToCharArray()); }
            else { contraDocTypes = new string[] { "NA" }; }

            int CurrentSiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int CurrentDivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];


            //var list = (from p in db.ViewJobOrderBalance
            //            join t in db.JobOrderHeader on p.JobOrderHeaderId equals t.JobOrderHeaderId
            //            join t2 in db.JobOrderLine on p.JobOrderLineId equals t2.JobOrderLineId
            //            join pt in db.Product on p.ProductId equals pt.ProductId into ProductTable
            //            from ProductTab in ProductTable.DefaultIfEmpty()
            //            join D1 in db.Dimension1 on p.Dimension1Id equals D1.Dimension1Id into Dimension1Table
            //            from Dimension1Tab in Dimension1Table.DefaultIfEmpty()
            //            join D2 in db.Dimension2 on p.Dimension2Id equals D2.Dimension2Id into Dimension2Table
            //            from Dimension2Tab in Dimension2Table.DefaultIfEmpty()
            //            where p.BalanceQty > 0
            //            && ((string.IsNullOrEmpty(term) ? 1 == 1 : p.JobOrderNo.ToLower().Contains(term.ToLower()))
            //            || (string.IsNullOrEmpty(term) ? 1 == 1 : ProductTab.ProductName.ToLower().Contains(term.ToLower()))
            //            || (string.IsNullOrEmpty(term) ? 1 == 1 : Dimension1Tab.Dimension1Name.ToLower().Contains(term.ToLower()))
            //            || (string.IsNullOrEmpty(term) ? 1 == 1 : Dimension2Tab.Dimension2Name.ToLower().Contains(term.ToLower())))
            //            && (string.IsNullOrEmpty(settings.filterContraSites) ? p.SiteId == CurrentSiteId : contraSites.Contains(p.SiteId.ToString()))
            //            && (string.IsNullOrEmpty(settings.filterContraDivisions) ? p.DivisionId == CurrentDivisionId : contraDivisions.Contains(p.DivisionId.ToString()))
            //            && (string.IsNullOrEmpty(settings.filterContraDocTypes) ? 1 == 1 : contraDocTypes.Contains(t.DocTypeId.ToString()))
            //            orderby t.DocDate, t.DocNo
            //            select new ComboBoxResult
            //            {
            //                text = ProductTab.ProductName,
            //                id = p.JobOrderLineId.ToString(),
            //                TextProp1 = "Order No: " + p.JobOrderNo.ToString(),
            //                TextProp2 = "Order Date: " + t.DocDate.ToString(),
            //                AProp1 = "Job Worker: " + t.JobWorker.Person.Name,
            //                AProp2 = "BalQty: " + p.BalanceQty.ToString(),
            //            });



            //var list = (from p in db.ViewJobOrderBalance
            //            join t in db.JobOrderHeader on p.JobOrderHeaderId equals t.JobOrderHeaderId
            //            join t2 in db.JobOrderLine on p.JobOrderLineId equals t2.JobOrderLineId
            //            join PU in db.ProductUid on t2.ProductUidId equals PU.ProductUIDId into PUTable
            //            from PUTab in PUTable.DefaultIfEmpty()
            //            join pt in db.Product on p.ProductId equals pt.ProductId into ProductTable
            //            from ProductTab in ProductTable.DefaultIfEmpty()
            //            join JW in db.Persons on t.JobWorkerId equals JW.PersonID into JWTable
            //            from JWTab in JWTable.DefaultIfEmpty()
            //            join PG in db.ProductGroups on ProductTab.ProductGroupId equals PG.ProductGroupId into PGTable
            //            from PGTab in PGTable.DefaultIfEmpty()
            //            join FP in db.FinishedProduct on ProductTab.ProductId equals FP.ProductId into FPTable
            //            from FPTab in FPTable.DefaultIfEmpty()
            //            join C in db.Colour on FPTab.ColourId equals C.ColourId into CTable
            //            from CTab in CTable.DefaultIfEmpty()
            //            join RS in db.ViewRugSize on p.ProductId equals RS.ProductId into RSTable
            //            from RSTab in RSTable.DefaultIfEmpty()
            //            join SC in db.ViewSizeinCms on RSTab.ManufaturingSizeID equals SC.SizeId into SCTable
            //            from SCTab in SCTable.DefaultIfEmpty()
            //            join D1 in db.Dimension1 on p.Dimension1Id equals D1.Dimension1Id into Dimension1Table
            //            from Dimension1Tab in Dimension1Table.DefaultIfEmpty()
            //            join D2 in db.Dimension2 on p.Dimension2Id equals D2.Dimension2Id into Dimension2Table
            //            from Dimension2Tab in Dimension2Table.DefaultIfEmpty()
            //            where p.BalanceQty > 0 && JWTab.IsSisterConcern ==false
            //            && (PersonId==null || PersonId ==0  ? 1 == 1 : t.JobWorkerId ==PersonId)
            //            && ((string.IsNullOrEmpty(term) ? 1 == 1 : p.JobOrderNo.ToLower().Contains(term.ToLower()))
            //            || (string.IsNullOrEmpty(term) ? 1 == 1 : t.CostCenter.CostCenterName.ToLower().Contains(term.ToLower()))
            //            || (string.IsNullOrEmpty(term) ? 1 == 1 : ProductTab.ProductName.ToLower().Contains(term.ToLower()))
            //            || (string.IsNullOrEmpty(term) ? 1 == 1 : Dimension1Tab.Dimension1Name.ToLower().Contains(term.ToLower()))
            //            || (string.IsNullOrEmpty(term) ? 1 == 1 : Dimension2Tab.Dimension2Name.ToLower().Contains(term.ToLower()))
            //            || (string.IsNullOrEmpty(term) ? 1 == 1 : (settings.isVisibleProductUID == true && settings.SqlProcGenProductUID == null ? "Product UID: " + PUTab.ProductUidName.ToString() : "BalQty: " + p.BalanceQty.ToString()).ToLower().Contains(term.ToLower()))
            //            || (string.IsNullOrEmpty(term) ? 1 == 1 : (PGTab.ProductGroupName.ToString().Replace("-", "") + "-" + (t2.DealUnitId == "MT2" ? SCTab.SizeName.ToString() : RSTab.ManufaturingSizeName.ToString()) + "-" + CTab.ColourName.ToString()).Contains(term.ToLower())))
            //            && (string.IsNullOrEmpty(settings.filterContraSites) ? p.SiteId == CurrentSiteId : contraSites.Contains(p.SiteId.ToString()))
            //            && (string.IsNullOrEmpty(settings.filterContraDivisions) ? p.DivisionId == CurrentDivisionId : contraDivisions.Contains(p.DivisionId.ToString()))
            //            && (string.IsNullOrEmpty(settings.filterContraDocTypes) ? 1 == 1 : contraDocTypes.Contains(t.DocTypeId.ToString()))
            //            orderby t.DocDate, t.DocNo
            //            select new ComboBoxResult
            //            {
            //                text = ProductTab.ProductName,
            //                id = p.JobOrderLineId.ToString(),
            //                TextProp1 = "Order Product: " + PGTab.ProductGroupName.ToString().Replace("-","")+"-"+ (t2.DealUnitId== "MT2"? SCTab.SizeName.ToString() : RSTab.ManufaturingSizeName.ToString())  + "-" + CTab.ColourName.ToString(),
            //                TextProp2 = "Order No: " + p.JobOrderNo.ToString()+",Costcenter No: " + t.CostCenter.CostCenterName.ToString(),
            //                AProp1 = "Job Worker: " + JWTab.Name,
            //                AProp2 = settings.isVisibleProductUID==true && settings.SqlProcGenProductUID == null ? "Product UID: " + PUTab.ProductUidName.ToString() : "BalQty: " + p.BalanceQty.ToString(),
            //            });


         string   mQry = @"	SELECT H.JobOrderNo, JOH.DocDate AS JobOrderDate, isnull(CC.CostCenterName,'') AS  CostCenterNo, 
                            H.JobOrderLineId, JOH.DocTypeId AS DocTypeId,
                           	DT.DocumentTypeName AS DocTypeName,  JOH.JobWorkerId AS JobWorkerId,  JW.Name AS JobWorkerName,                           
                          	P.ProductName,  isnull(PU.ProductUidName, '') AS ProductUidName, PG.ProductGroupName, 
                            CASE WHEN JOL.DealUnitId= 'MT2' AND S.UnitId='FT' THEN VSC.SizeName ELSE VRS.ManufaturingSizeName END AS SizeName,
                            C.ColourName,   H.BalanceQty
                             FROM
                             (
                             SELECT * FROM Web.ViewJobOrderBalance H WITH(Nolock)
                            WHERE H.BalanceQty > 0 AND H.SiteId =" + CurrentSiteId.ToString() + @" AND H.DivisionId =" + CurrentDivisionId.ToString() + @"
                            AND H.JobWorkerId =" + PersonId.ToString() + @"
                            ) H
                            LEFT JOIN web.JobOrderHeaders JOH  WITH(Nolock) ON JOH.JobOrderHeaderId = H.JobOrderHeaderId
                            LEFT JOIN web.JobOrderLines JOL  WITH(Nolock) ON JOL.JobOrderLineId = H.JobOrderLineId
                            LEFT JOIN web.DocumentTypes DT  WITH(Nolock) ON DT.DocumentTypeId = JOH.DocTypeId
                            LEFT JOIN web.CostCenters CC  WITH(Nolock) ON CC.CostCenterId = JOH.CostCenterId
                            LEFT JOIN web.People JW  WITH(Nolock) ON JW.PersonID = JOH.JobWorkerId
                            LEFT JOIN web.Products P WITH(Nolock) ON P.ProductId = JOL.ProductId
                            LEFT JOIN web.ProductGroups PG WITH(Nolock) ON PG.ProductGroupId = P.ProductGroupId
                            LEFT JOIN web.ViewRugSize VRS WITH(Nolock) ON VRS.ProductId = P.ProductId
                            LEFT JOIN web.FinishedProduct  FP WITH(Nolock) ON FP.ProductId = P.ProductId
                            LEFT JOIN web.Colours C WITH(Nolock) ON C.ColourId = FP.ColourId
                            LEFT JOIN web.ViewSizeinCms VSC WITH (Nolock) ON VSC.SizeId =VRS.ManufaturingSizeID
                            LEFT JOIN web.Sizes S WITH(Nolock) ON S.SizeId = VRS.ManufaturingSizeID 
                            LEFT JOIN web.ProductUids PU  WITH(Nolock) ON PU.ProductUIDId = JOL.ProductUidId OR PU.LotNo = JOL.LotNo
                            WHERE isnull(JW.IsSisterConcern, 0) = 0 ";

            IEnumerable<JobOrderDetail> JobOrderBalanceList = db.Database.SqlQuery<JobOrderDetail>(mQry).ToList();

            var list1 = (from p in JobOrderBalanceList
                        where p.BalanceQty > 0 
                        && ((string.IsNullOrEmpty(term) ? 1 == 1 : p.JobOrderNo.ToLower().Contains(term.ToLower()))
                        || (string.IsNullOrEmpty(term) ? 1 == 1 : p.CostCenterNo.ToLower().Contains(term.ToLower()))
                        || (string.IsNullOrEmpty(term) ? 1 == 1 : p.ProductName.ToLower().Contains(term.ToLower()))
                        || (string.IsNullOrEmpty(term) ? 1 == 1 : (settings.isVisibleProductUID == true && settings.SqlProcGenProductUID == null ? "Product UID: " + p.ProductUidName.ToString() : "BalQty: " + p.BalanceQty.ToString()).ToLower().Contains(term.ToLower()))
                        || (string.IsNullOrEmpty(term) ? 1 == 1 : (p.ProductGroupName.ToString().Replace("-", "") + "-" + p.SizeName.ToString() + "-" + p.ColourName.ToString()).Contains(term.ToLower())))
                        && (string.IsNullOrEmpty(settings.filterContraDocTypes) ? 1 == 1 : contraDocTypes.Contains(p.DocTypeId.ToString()))
                        orderby p.JobOrderDate, p.JobOrderNo
                        select new ComboBoxResult
                        {
                            text = p.ProductName,
                            id = p.JobOrderLineId.ToString(),
                            TextProp1 = "Order Product: " + p.ProductGroupName.ToString().Replace("-", "") + "-" + p.SizeName.ToString() + "-" + p.ColourName.ToString(),
                            TextProp2 = "Order No: " + p.JobOrderNo.ToString() + ",Costcenter No: " + p.CostCenterNo.ToString() + ",Date : " + p.JobOrderDate.ToString("dd/MMM/yy"),
                            AProp1 = "Job Worker: " + p.JobWorkerName,
                            AProp2 = settings.isVisibleProductUID == true && settings.SqlProcGenProductUID == null ? "Product UID: " + p.ProductUidName.ToString() : "BalQty: " + p.BalanceQty.ToString(),
                        });

            var list = list1.AsQueryable();

            return list;
        }

        public class JobOrderDetail
        {
            public string JobOrderNo { get; set; }
            public DateTime JobOrderDate { get; set; }
            public string CostCenterNo { get; set; }
            public int? JobOrderLineId { get; set; }
            public int DocTypeId { get; set; }
            public string DocTypeName { get; set; }
            public int JobWorkerId { get; set; }
            public string JobWorkerName { get; set; }
            public string ProductGroupName { get; set; }
            public string SizeName { get; set; }
            public string ColourName { get; set; }
            public int ProductId { get; set; }
            public string ProductUidName { get; set; }
            public int? ProductUidId { get; set; }
            public Decimal BalanceQty { get; set; }
            public Decimal? Length { get; set; }
            public Decimal? Width { get; set; }
            public Decimal? Height { get; set; }
            public int? DimensionUnitDecimalPlaces { get; set; }
            public string ProductQualityName { get; set; }
            public string ProductName { get; set; }
            public Decimal? LastWeight { get; set; }
        }

        public class BuyerDetail
        {
            public string BuyerName { get; set; }
        }

        public IQueryable<ComboBoxResult> TempGetCustomProduct(int filter, string term, int? PersonId)
        {
            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            var settings = new JobReceiveSettingsService(_unitOfWork).GetJobReceiveSettingsForDocument(filter, DivisionId, SiteId);

            string[] contraSites = null;
            if (!string.IsNullOrEmpty(settings.filterContraSites)) { contraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            else { contraSites = new string[] { "NA" }; }

            string[] contraDivisions = null;
            if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { contraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { contraDivisions = new string[] { "NA" }; }

            string[] contraDocTypes = null;
            if (!string.IsNullOrEmpty(settings.filterContraDocTypes)) { contraDocTypes = settings.filterContraDocTypes.Split(",".ToCharArray()); }
            else { contraDocTypes = new string[] { "NA" }; }

            int CurrentSiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int CurrentDivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

            var list = (from p in db.ViewJobOrderBalance
                        join t in db.JobOrderHeader on p.JobOrderHeaderId equals t.JobOrderHeaderId
                        join t2 in db.JobOrderLine on p.JobOrderLineId equals t2.JobOrderLineId
                        join PU in db.ProductUid on t2.ProductUidId equals PU.ProductUIDId into PUTable
                        from PUTab in PUTable.DefaultIfEmpty()
                        join pt in db.Product on p.ProductId equals pt.ProductId into ProductTable
                        from ProductTab in ProductTable.DefaultIfEmpty()
                        join JW in db.Persons on t.JobWorkerId equals JW.PersonID into JWTable
                        from JWTab in JWTable.DefaultIfEmpty()
                        join PG in db.ProductGroups on ProductTab.ProductGroupId equals PG.ProductGroupId into PGTable
                        from PGTab in PGTable.DefaultIfEmpty()
                        join FP in db.FinishedProduct on ProductTab.ProductId equals FP.ProductId into FPTable
                        from FPTab in FPTable.DefaultIfEmpty()
                        join C in db.Colour on FPTab.ColourId equals C.ColourId into CTable
                        from CTab in CTable.DefaultIfEmpty()
                        join RS in db.ViewRugSize on p.ProductId equals RS.ProductId into RSTable
                        from RSTab in RSTable.DefaultIfEmpty()
                        join SC in db.ViewSizeinCms on RSTab.ManufaturingSizeID equals SC.SizeId into SCTable
                        from SCTab in SCTable.DefaultIfEmpty()
                        join D1 in db.Dimension1 on p.Dimension1Id equals D1.Dimension1Id into Dimension1Table
                        from Dimension1Tab in Dimension1Table.DefaultIfEmpty()
                        join D2 in db.Dimension2 on p.Dimension2Id equals D2.Dimension2Id into Dimension2Table
                        from Dimension2Tab in Dimension2Table.DefaultIfEmpty()
                        where p.BalanceQty > 0 
                        && (PersonId == null || PersonId == 0 ? 1 == 1 : t.JobWorkerId == PersonId)
                        && ((string.IsNullOrEmpty(term) ? 1 == 1 : p.JobOrderNo.ToLower().Contains(term.ToLower()))
                        || (string.IsNullOrEmpty(term) ? 1 == 1 : t.CostCenter.CostCenterName.ToLower().Contains(term.ToLower()))
                        || (string.IsNullOrEmpty(term) ? 1 == 1 : ProductTab.ProductName.ToLower().Contains(term.ToLower()))
                        || (string.IsNullOrEmpty(term) ? 1 == 1 : Dimension1Tab.Dimension1Name.ToLower().Contains(term.ToLower()))
                        || (string.IsNullOrEmpty(term) ? 1 == 1 : Dimension2Tab.Dimension2Name.ToLower().Contains(term.ToLower()))
                        || (string.IsNullOrEmpty(term) ? 1 == 1 : (settings.isVisibleProductUID == true && settings.SqlProcGenProductUID == null ? "Product UID: " + PUTab.ProductUidName.ToString() : "BalQty: " + p.BalanceQty.ToString()).ToLower().Contains(term.ToLower()))
                        || (string.IsNullOrEmpty(term) ? 1 == 1 : (PGTab.ProductGroupName.ToString().Replace("-", "") + "-" + (t2.DealUnitId == "MT2" ? SCTab.SizeName.ToString() : RSTab.ManufaturingSizeName.ToString()) + "-" + CTab.ColourName.ToString()).Contains(term.ToLower())))
                        && (string.IsNullOrEmpty(settings.filterContraSites) ? p.SiteId == CurrentSiteId : contraSites.Contains(p.SiteId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterContraDivisions) ? p.DivisionId == CurrentDivisionId : contraDivisions.Contains(p.DivisionId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterContraDocTypes) ? 1 == 1 : contraDocTypes.Contains(t.DocTypeId.ToString()))
                        orderby t.DocDate, t.DocNo
                        select new ComboBoxResult
                        {
                            text = ProductTab.ProductName,
                            id = p.JobOrderLineId.ToString(),
                            TextProp1 = "Order Product: " + PGTab.ProductGroupName.ToString().Replace("-", "") + "-" + (t2.DealUnitId == "MT2" ? SCTab.SizeName.ToString() : RSTab.ManufaturingSizeName.ToString()) + "-" + CTab.ColourName.ToString(),
                            TextProp2 = "Order No: " + p.JobOrderNo.ToString() + ",Costcenter No: " + t.CostCenter.CostCenterName.ToString(),
                            AProp1 = "Job Worker: " + JWTab.Name,
                            AProp2 = settings.isVisibleProductUID == true && settings.SqlProcGenProductUID == null ? "Product UID: " + PUTab.ProductUidName.ToString() : "BalQty: " + p.BalanceQty.ToString(),
                        });
            return list;
        }

        public IQueryable<ComboBoxResult> GetCustomPerson(int Id, string term)
        {
            int DocTypeId = Id;
            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

            var settings = new JobReceiveSettingsService(_unitOfWork).GetJobReceiveSettingsForDocument(DocTypeId, DivisionId, SiteId);

            string[] PersonRoles = null;
            if (!string.IsNullOrEmpty(settings.filterPersonRoles)) { PersonRoles = settings.filterPersonRoles.Split(",".ToCharArray()); }
            else { PersonRoles = new string[] { "NA" }; }

            string DivIdStr = "|" + DivisionId.ToString() + "|";
            string SiteIdStr = "|" + SiteId.ToString() + "|";

            var list = (from p in db.Persons
                        join bus in db.BusinessEntity on p.PersonID equals bus.PersonID into BusinessEntityTable
                        from BusinessEntityTab in BusinessEntityTable.DefaultIfEmpty()
                        join pp in db.PersonProcess on p.PersonID equals pp.PersonId into PersonProcessTable
                        from PersonProcessTab in PersonProcessTable.DefaultIfEmpty()
                        join pr in db.PersonRole on p.PersonID equals pr.PersonId into PersonRoleTable
                        from PersonRoleTab in PersonRoleTable.DefaultIfEmpty()
                        where PersonProcessTab.ProcessId == settings.ProcessId
                        && (string.IsNullOrEmpty(term) ? 1 == 1 : (p.Name.ToLower().Contains(term.ToLower()) || p.Code.ToLower().Contains(term.ToLower())))
                        && (string.IsNullOrEmpty(settings.filterPersonRoles) ? 1 == 1 : PersonRoles.Contains(PersonRoleTab.RoleDocTypeId.ToString()))
                        && BusinessEntityTab.DivisionIds.IndexOf(DivIdStr) != -1
                        && BusinessEntityTab.SiteIds.IndexOf(SiteIdStr) != -1
                        && (p.IsActive == null ? 1 == 1 : p.IsActive == true)
                        group new { p } by new { p.PersonID } into Result
                        orderby Result.Max(m => m.p.Name)
                        select new ComboBoxResult
                        {
                            id = Result.Key.PersonID.ToString(),
                            text = Result.Max(m => m.p.Name + "|" + m.p.Code),
                        }
              );

            return list;
        }

        public IQueryable<WeavingReceiveQaCombinedIndexViewModel> GetJobReceiveHeaderList(int DocTypeId, string Uname)
        {

            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];

            string mQry = @"SELECT H.JobReceiveHeaderId, H.DocDate,H.JobWorkerDocNo,H.DocNo, JW.Name JobWorkerName,  DT.DocumentTypeName AS   DocTypeName,
                            PG.ProductGroupName,  C.ColourName, 
                            PG.ProductGroupName +'-'+ (CASE WHEN L.DealUnitId= 'MT2' AND S.UnitId='FT' THEN VSC.SizeName ELSE VRS.ManufaturingSizeName END) +'-'+C.ColourName AS ProductName,
                            CASE WHEN L.DealUnitId= 'MT2' AND S.UnitId='FT' THEN VSC.SizeName ELSE VRS.ManufaturingSizeName END AS SizeName,                            
                            H.Remark , H.Status ,  H.ModifiedBy ,  H.ReviewCount ,   H.ReviewBy ,  
                            CASE WHEN L.DealUnitId ='MT2' THEN Replace(str(JRQE.Length,10) +'X'+str(JRQE.Width,10),' ','') ELSE replace(str(JRQE.Length,10,2) +'X'+str(JRQE.Width,10,2),' ','') END AS ActualSize,
                            JRQ.Weight, L.Qty AS TotalQty, isnull(PU.ProductUidName,L.LotNo) AS ProductUidName
                            FROM web.JobReceiveHeaders H WITH (Nolock) 
                            LEFT JOIN web.DocumentTypes DT  WITH(Nolock) ON DT.DocumentTypeId = H.DocTypeId
                            LEFT JOIN web.JobReceiveLines L WITH (Nolock) ON L.JobReceiveHeaderId = H.JobReceiveHeaderId
                            LEFT JOIN web.JobReceiveQALines JRQ WITH (Nolock) ON JRQ.JobReceiveLineId = L.JobReceiveLineId
                            LEFT JOIN web.JobReceiveQALineExtendeds JRQE WITH (Nolock) ON JRQE.JobReceiveQALineId = JRQ.JobReceiveQALineId
                            LEFT JOIN web.People JW  WITH(Nolock) ON JW.PersonID = H.JobWorkerId
                            LEFT JOIN web.Products P WITH(Nolock) ON P.ProductId = L.ProductId
                            LEFT JOIN web.ProductGroups PG WITH(Nolock) ON PG.ProductGroupId = P.ProductGroupId
                            LEFT JOIN web.ViewRugSize VRS WITH(Nolock) ON VRS.ProductId = P.ProductId
                            LEFT JOIN web.FinishedProduct  FP WITH(Nolock) ON FP.ProductId = P.ProductId
                            LEFT JOIN web.Colours C WITH(Nolock) ON C.ColourId = FP.ColourId
                            LEFT JOIN web.ViewSizeinCms VSC WITH (Nolock) ON VSC.SizeId =VRS.ManufaturingSizeID
                            LEFT JOIN web.Sizes S WITH(Nolock) ON S.SizeId = VRS.ManufaturingSizeID 
                            LEFT JOIN web.ProductUids PU  WITH(Nolock) ON PU.ProductUIDId = L.ProductUidId    
                            WHERE H.DocTypeId =" + DocTypeId.ToString() + @" AND H.DivisionId  = " + DivisionId.ToString() + @" AND H.SiteId =" + SiteId.ToString() + @"                 
                            ORDER BY H.DocDate desc, H.JobReceiveHeaderId  DESC ";


            IEnumerable<WeavingReceiveQaCombinedIndexViewModel> JobReceiveList = db.Database.SqlQuery<WeavingReceiveQaCombinedIndexViewModel>(mQry).ToList();

            var JobReceiveHeaderList = JobReceiveList.AsQueryable();

            return JobReceiveHeaderList;



            //        return (from p in db.JobReceiveHeader
            //                join L in db.JobReceiveLine on p.JobReceiveHeaderId equals L.JobReceiveHeaderId into JobReceiveLineTable
            //                from JobReceiveLineTab in JobReceiveLineTable.DefaultIfEmpty()
            //                join PR in db.Product on JobReceiveLineTab.ProductId equals PR.ProductId into PRTable
            //                from PRTab in PRTable.DefaultIfEmpty()
            //                join Jrql in db.JobReceiveQALine on JobReceiveLineTab.JobReceiveLineId equals Jrql.JobReceiveLineId into JobReceiveQaLineTable
            //                from JobReceiveQALineTab in JobReceiveQaLineTable.DefaultIfEmpty()
            //                join Ld in db.JobReceiveQALineExtended on JobReceiveQALineTab.JobReceiveQALineId equals Ld.JobReceiveQALineId into JobReceiveQALineExtendedTable
            //                from JobReceiveQALineExtendedTab in JobReceiveQALineExtendedTable.DefaultIfEmpty()
            //                orderby p.DocDate descending, p.DocNo descending
            //                where p.SiteId == SiteId && p.DivisionId == DivisionId && p.DocTypeId == DocTypeId
            //                select new WeavingReceiveQaCombinedIndexViewModel
            //                {
            //                    JobReceiveHeaderId = p.JobReceiveHeaderId,
            //                    DocDate = p.DocDate,
            //                    JobWorkerDocNo = p.JobWorkerDocNo,
            //                    DocNo = p.DocNo,
            //                    JobWorkerName = p.JobWorker.Name,
            //                    DocTypeName = p.DocType.DocumentTypeName,
            //                    ProductName = PRTab.ProductName,
            //                    Remark = p.Remark,
            //                    Status = p.Status,
            //                    ModifiedBy = p.ModifiedBy,
            //                    ReviewCount = p.ReviewCount,
            //                    ReviewBy = p.ReviewBy,
            //                    Reviewed = (SqlFunctions.CharIndex(Uname, p.ReviewBy) > 0),
            //                    ActualSize = (JobReceiveLineTab.DealUnitId == "MT2" ? JobReceiveQALineExtendedTab.Length.ToString().Replace(".0000", "") : (JobReceiveQALineExtendedTab.Length.ToString() + ",").Replace("00,", "")) + "X" + (JobReceiveLineTab.DealUnitId == "MT2" ? JobReceiveQALineExtendedTab.Width.ToString().Replace(".0000", "") : (JobReceiveQALineExtendedTab.Width.ToString() + ",").Replace("00,", "")),
            //                    Weight = p.JobReceiveLines.Sum(m => m.Weight),
            //                    TotalQty = p.JobReceiveLines.Sum(m => m.Qty),
            //                    ProductUidName = JobReceiveLineTab.ProductUid.ProductUidName == null ? JobReceiveLineTab.LotNo : JobReceiveLineTab.ProductUid.ProductUidName,
            //                    DecimalPlaces = (from o in p.JobReceiveLines
            //                                     join ol in db.JobOrderLine on o.JobOrderLineId equals ol.JobOrderLineId
            //                                     join prod in db.Product on ol.ProductId equals prod.ProductId
            //                                     join u in db.Units on prod.UnitId equals u.UnitId
            //                                     select u.DecimalPlaces).Max(),
            //                }
            //);

        }

        public IQueryable<WeavingReceiveQaCombinedIndexViewModel> GetJobReceiveHeaderListPendingToSubmit(int id, string Uname)
        {

            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];
            var JobOrderHeader = GetJobReceiveHeaderList(id, Uname).AsQueryable();

            var PendingToSubmit = from p in JobOrderHeader
                                  where p.Status == (int)StatusConstants.Drafted || p.Status == (int)StatusConstants.Import || p.Status == (int)StatusConstants.Modified && (p.ModifiedBy == Uname || UserRoles.Contains("Admin"))
                                  select p;
            return PendingToSubmit;

        }
        public IQueryable<WeavingReceiveQaCombinedIndexViewModel> GetJobReceiveHeaderListPendingToReview(int id, string Uname)
        {

            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];
            var JobOrderHeader = GetJobReceiveHeaderList(id, Uname).AsQueryable();

            var PendingToReview = from p in JobOrderHeader
                                  where p.Status == (int)StatusConstants.Submitted && (SqlFunctions.CharIndex(Uname, (p.ReviewBy ?? "")) == 0)
                                  select p;
            return PendingToReview;

        }

        public void Dispose()
        {
        }


        public Task<IEquatable<JobReceiveQAAttribute>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<JobReceiveQAAttribute> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }

    public class WeavingReceiveQaCombinedIndexViewModel
    {

        public int JobReceiveHeaderId { get; set; }
        public DateTime DocDate { get; set; }
        public string DocNo { get; set; }
        public string DocTypeName { get; set; }
        public string Remark { get; set; }
        public int Status { get; set; }
        public string JobWorkerDocNo { get; set; }
        public string JobWorkerName { get; set; }
        public string ModifiedBy { get; set; }
        public int? ReviewCount { get; set; }
        public string ReviewBy { get; set; }
        public bool? Reviewed { get; set; }
        public decimal? TotalQty { get; set; }
        public int? DecimalPlaces { get; set; }

        public string ProductUidName { get; set; }
        public string ProductName { get; set; }
        public string ProductGroupName { get; set; }
        public string ColourName { get; set; }
        public string SizeName { get; set; }
        public string ActualSize { get; set; }
        public decimal? Weight { get; set; }

    }

}
