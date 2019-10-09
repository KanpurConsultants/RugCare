using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Infrastructure;
using Model.Models;

using Core.Common;
using System;
using Model;
using System.Threading.Tasks;
using Data.Models;
using Model.ViewModels;

namespace Service
{
    public interface IProductQualityBomDetailService : IDisposable
    {
        ProductQualityBomDetail Create(ProductQualityBomDetail pt);
        void Delete(int id);
        void Delete(ProductQualityBomDetail pt);
        ProductQualityBomDetail Find(int id);
        IEnumerable<ProductQualityBomDetail> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(ProductQualityBomDetail pt);
        ProductQualityBomDetail Add(ProductQualityBomDetail pt);
        IEnumerable<ProductQualityConsumptionLineViewModel> GetContentForIndexForProduct(int ProductQualityId);
        ProductQualityConsumptionLineViewModel GetDesignConsumptionLineForEditForProduct(int ProductQualityBomDetailId);

    }

    public class ProductQualityBomDetailService : IProductQualityBomDetailService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<ProductQualityBomDetail> _ProductQualityBomDetailRepository;
        RepositoryQuery<ProductQualityBomDetail> ProductQualityBomDetailRepository;
        //int OverTuftProcessId = 0;


        public ProductQualityBomDetailService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _ProductQualityBomDetailRepository = new Repository<ProductQualityBomDetail>(db);
            ProductQualityBomDetailRepository = new RepositoryQuery<ProductQualityBomDetail>(_ProductQualityBomDetailRepository);

            //var OverTuftProcess = new ProcessService(_unitOfWork).Find(ProcessConstants.OverTuft);
            //if (OverTuftProcess != null)
            //{
            //    OverTuftProcessId = OverTuftProcess.ProcessId;
            //}
        }




        public ProductQualityBomDetail Find(int id)
        {
            return _unitOfWork.Repository<ProductQualityBomDetail>().Find(id);
        }

        public ProductQualityBomDetail Create(ProductQualityBomDetail pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<ProductQualityBomDetail>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<ProductQualityBomDetail>().Delete(id);
        }

        public void Delete(ProductQualityBomDetail pt)
        {
            _unitOfWork.Repository<ProductQualityBomDetail>().Delete(pt);
        }

        public void Update(ProductQualityBomDetail pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<ProductQualityBomDetail>().Update(pt);
        }

        public IEnumerable<ProductQualityBomDetail> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<ProductQualityBomDetail>()
                .Query()
                //.OrderBy(q => q.OrderBy(c => c.ProductQualityBomDetailName))                
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IEnumerable<ProductQualityBomDetail> GetProductQualityBomDetailList()
        {
            var pt = _unitOfWork.Repository<ProductQualityBomDetail>().Query().Get();//.OrderBy(m=>m.ProductQualityBomDetailName);
            return pt;
        }

        public IEnumerable<ProductQualityBomDetail> GetProductQualityBomDetailList(int ProductQualityId)
        {
            var pt = _unitOfWork.Repository<ProductQualityBomDetail>().Query().Get().Where(m => m.ProductQualityId == ProductQualityId);
            return pt;
        }

        public ProductQualityBomDetail Add(ProductQualityBomDetail pt)
        {
            _unitOfWork.Repository<ProductQualityBomDetail>().Insert(pt);
            return pt;
        }


        public IEnumerable<ProductQualityConsumptionLineViewModel> GetContentForIndexForProduct(int ProductQualityId)
        {

            IEnumerable<ProductQualityConsumptionLineViewModel> svm = (from b in db.ProductQualityBomDetail
                                                                       join d in db.Dimension1 on b.Dimension1Id equals d.Dimension1Id into Dimension1Table
                                                                from Dimension1Tab in Dimension1Table.DefaultIfEmpty()
                                                                join p in db.Product on b.ProductId equals p.ProductId into ProductTable
                                                                from ProductTab in ProductTable.DefaultIfEmpty()
                                                                join pg in db.ProductGroups on ProductTab.ProductGroupId equals pg.ProductGroupId into ProductGroupTable
                                                                from ProductGroupTab in ProductGroupTable.DefaultIfEmpty()
                                                                join U in db.Units on ProductTab.UnitId equals U.UnitId into UnitTable
                                                                from UnitTab in UnitTable.DefaultIfEmpty()
                                                                where b.ProductQualityId == ProductQualityId 
                                                                select new ProductQualityConsumptionLineViewModel
                                                                {
                                                                    ProductQualityBomDetailId = b.ProductQualityBomDetailId,
                                                                    ProductQualityId = b.ProductQualityId,
                                                                    ProcessName = b.Process.ProcessName,
                                                                    ProductName = ProductTab.ProductName,
                                                                    Dimension1Name = Dimension1Tab.Dimension1Name,
                                                                    ProductGroupName = ProductGroupTab.ProductGroupName,
                                                                    Qty = b.Qty,
                                                                    UnitName = UnitTab.UnitName
                                                                });


            return svm.ToList();
        }


        public ProductQualityConsumptionLineViewModel GetDesignConsumptionLineForEditForProduct(int ProductQualityBomDetailId)
        {
            ProductQualityConsumptionLineViewModel svm = (from b in db.ProductQualityBomDetail
                                                          join p in db.Product on b.ProductId equals p.ProductId into ProductTable
                                                   from ProductTab in ProductTable.DefaultIfEmpty()
                                                   join pg in db.ProductGroups on ProductTab.ProductGroupId equals pg.ProductGroupId into ProductGroupTable
                                                   from ProductGroupTab in ProductGroupTable.DefaultIfEmpty()
                                                   join U in db.Units on ProductTab.UnitId equals U.UnitId into UnitTable
                                                   from UnitTab in UnitTable.DefaultIfEmpty()
                                                   where b.ProductQualityBomDetailId == ProductQualityBomDetailId
                                                    select new ProductQualityConsumptionLineViewModel
                                                       {
                                                       ProductQualityId=b.ProductQualityId,
                                                       ProductQualityBomDetailId = b.ProductQualityBomDetailId,
                                                       ProductId = b.ProductId,
                                                       Dimension1Id = b.Dimension1Id,
                                                       ProductGroupName = ProductGroupTab.ProductGroupName,
                                                       ProcessId = (int)b.ProcessId,
                                                       Qty = b.Qty,
                                                       UnitName = UnitTab.UnitName
                                                   }).FirstOrDefault();
            return svm;
        }



        public void Dispose()
        {
        }


        public Task<IEquatable<ProductQualityBomDetail>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ProductQualityBomDetail> FindAsync(int id)
        {
            throw new NotImplementedException();
        }










    }




}
