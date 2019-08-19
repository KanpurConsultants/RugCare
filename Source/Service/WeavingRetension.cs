using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Infrastructure;
using Model.Models;
using Model.ViewModels;
using Core.Common;
using System;
using Model;
using System.Threading.Tasks;
using Data.Models;

namespace Service
{
    public interface IWeavingRetensionService : IDisposable
    {
        WeavingRetension Create(WeavingRetension pt);
        void Delete(int id);
        void Delete(WeavingRetension pt);
        //WeavingRetension Find(string Name);
        WeavingRetension Find(int id);
        IEnumerable<WeavingRetension> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(WeavingRetension pt);
        WeavingRetension Add(WeavingRetension pt);
        IEnumerable<WeavingRetensionViewModel> GetWeavingRetensionList();
        Task<IEquatable<WeavingRetension>> GetAsync();
        Task<WeavingRetension> FindAsync(int id);
        int NextId(int id);
        int PrevId(int id);
    }

    public class WeavingRetensionService : IWeavingRetensionService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<WeavingRetension> _WeavingRetensionRepository;
        RepositoryQuery<WeavingRetension> WeavingRetensionRepository;
        public WeavingRetensionService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _WeavingRetensionRepository = new Repository<WeavingRetension>(db);
            WeavingRetensionRepository = new RepositoryQuery<WeavingRetension>(_WeavingRetensionRepository);
        }

        //public WeavingRetension Find(string Name)
        //{
        //    return WeavingRetensionRepository.Get().Where(i => i.WeavingRetensionName == Name).FirstOrDefault();
        //}


        public WeavingRetension Find(int id)
        {
            return _unitOfWork.Repository<WeavingRetension>().Find(id);
        }

        public WeavingRetension Create(WeavingRetension pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<WeavingRetension>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<WeavingRetension>().Delete(id);
        }

        public void Delete(WeavingRetension pt)
        {
            _unitOfWork.Repository<WeavingRetension>().Delete(pt);
        }

        public void Update(WeavingRetension pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<WeavingRetension>().Update(pt);
        }

        public IEnumerable<WeavingRetension> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<WeavingRetension>()
                .Query()
                .OrderBy(q => q.OrderBy(c => c.WeavingRetensionId))                
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IEnumerable<WeavingRetensionViewModel> GetWeavingRetensionList()
        {

            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];

            return (from p in db.WeavingRetension
                    orderby p.ProductCategory.ProductCategoryName , p.ProductQuality.ProductQualityName, p.Person.Name 
                    where p.DivisionId == DivisionId && p.SiteId == SiteId
                    select new WeavingRetensionViewModel
                    {
                        WeavingRetensionId = p.WeavingRetensionId,
                        ProductCategoryName = p.ProductCategory.ProductCategoryName,
                        ProductQualityName = p.ProductQuality.ProductQualityName,
                        PersonName = p.Person.Name,
                        RetensionPer = p.RetensionPer,
                        MinimumAmount = p.MinimumAmount,
                        ModifiedBy = p.ModifiedBy,
                    }).ToList();

        }

        public WeavingRetension Add(WeavingRetension pt)
        {
            _unitOfWork.Repository<WeavingRetension>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.WeavingRetension
                        orderby p.WeavingRetensionId
                        select p.WeavingRetensionId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.WeavingRetension
                        orderby p.WeavingRetensionId
                        select p.WeavingRetensionId).FirstOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        public int PrevId(int id)
        {

            int temp = 0;
            if (id != 0)
            {

                temp = (from p in db.WeavingRetension
                        orderby p.WeavingRetensionId
                        select p.WeavingRetensionId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.WeavingRetension
                        orderby p.WeavingRetensionId
                        select p.WeavingRetensionId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        public void Dispose()
        {
        }


        public Task<IEquatable<WeavingRetension>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<WeavingRetension> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
