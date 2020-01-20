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
    public interface IPackingUnitSettingService : IDisposable
    {
        PackingUnitSetting Create(PackingUnitSetting pt);
        void Delete(int id);
        void Delete(PackingUnitSetting pt);
        //PackingUnitSetting Find(string Name);
        PackingUnitSetting Find(int id);
        IEnumerable<PackingUnitSetting> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(PackingUnitSetting pt);
        PackingUnitSetting Add(PackingUnitSetting pt);
        IEnumerable<PackingUnitSettingViewModel> GetPackingUnitSettingList();
        Task<IEquatable<PackingUnitSetting>> GetAsync();
        Task<PackingUnitSetting> FindAsync(int id);
        int NextId(int id);
        int PrevId(int id);
    }

    public class PackingUnitSettingService : IPackingUnitSettingService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<PackingUnitSetting> _PackingUnitSettingRepository;
        RepositoryQuery<PackingUnitSetting> PackingUnitSettingRepository;
        public PackingUnitSettingService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _PackingUnitSettingRepository = new Repository<PackingUnitSetting>(db);
            PackingUnitSettingRepository = new RepositoryQuery<PackingUnitSetting>(_PackingUnitSettingRepository);
        }

        //public PackingUnitSetting Find(string Name)
        //{
        //    return PackingUnitSettingRepository.Get().Where(i => i.PackingUnitSettingName == Name).FirstOrDefault();
        //}


        public PackingUnitSetting Find(int id)
        {
            return _unitOfWork.Repository<PackingUnitSetting>().Find(id);
        }

        public PackingUnitSetting Create(PackingUnitSetting pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<PackingUnitSetting>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<PackingUnitSetting>().Delete(id);
        }

        public void Delete(PackingUnitSetting pt)
        {
            _unitOfWork.Repository<PackingUnitSetting>().Delete(pt);
        }

        public void Update(PackingUnitSetting pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<PackingUnitSetting>().Update(pt);
        }

        public IEnumerable<PackingUnitSetting> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<PackingUnitSetting>()
                .Query()
                .OrderBy(q => q.OrderBy(c => c.PackingUnitSettingId))                
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IEnumerable<PackingUnitSettingViewModel> GetPackingUnitSettingList()
        {

            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];

            return (from p in db.PackingUnitSetting
                    orderby p.ProductCategory.ProductCategoryName , p.ProductQuality.ProductQualityName, p.ProductGroup.ProductGroupName, p.Size.SizeName
                    where p.DivisionId == DivisionId && p.SiteId == SiteId
                    select new PackingUnitSettingViewModel
                    {
                        PackingUnitSettingId = p.PackingUnitSettingId,
                        ProductCategoryName = p.ProductCategory.ProductCategoryName,
                        ProductQualityName = p.ProductQuality.ProductQualityName,
                        ProductGroupName = p.ProductGroup.ProductGroupName,
                        SizeName = p.Size.SizeName,
                        PackingLength = p.PackingLength,
                        PackingWidth = p.PackingWidth,
                        PackingHeight = p.PackingHeight,
                        PackingUnitId = p.PackingUnitId,
                        ModifiedBy = p.ModifiedBy,
                    }).ToList();

        }

        public PackingUnitSetting Add(PackingUnitSetting pt)
        {
            _unitOfWork.Repository<PackingUnitSetting>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.PackingUnitSetting
                        orderby p.PackingUnitSettingId
                        select p.PackingUnitSettingId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.PackingUnitSetting
                        orderby p.PackingUnitSettingId
                        select p.PackingUnitSettingId).FirstOrDefault();
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

                temp = (from p in db.PackingUnitSetting
                        orderby p.PackingUnitSettingId
                        select p.PackingUnitSettingId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.PackingUnitSetting
                        orderby p.PackingUnitSettingId
                        select p.PackingUnitSettingId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        public void Dispose()
        {
        }


        public Task<IEquatable<PackingUnitSetting>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<PackingUnitSetting> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
