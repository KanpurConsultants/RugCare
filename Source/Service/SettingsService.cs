using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Infrastructure;
using Model.Models;
using System;
using Model;
using System.Threading.Tasks;
using Data.Models;
using Model.ViewModel;
using Core.Common;

namespace Service
{
    public interface ISettingsService : IDisposable
    {
        Settings Create(Settings pt);
        void Delete(int id);
        void Delete(Settings pt);
        Settings Find(int id);
        IEnumerable<Settings> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(Settings pt);
        Settings Add(Settings pt);
        Settings GetSettingsForDocument(string FieldName, int? DivisionId, int? SiteId, int? DocTypeId, int? DocCategoryId);
        DateTime GetDefaultDocDate(string TableName, int? DivisionId, int? SiteId,int ? DocTypeId, int? DocCategoryId);
        Task<IEquatable<Settings>> GetAsync();
        Task<Settings> FindAsync(int id);        
        int NextId(int id);
        int PrevId(int id);
    }

    public class SettingsService : ISettingsService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<Settings> _SettingsRepository;
        RepositoryQuery<Settings> SettingsRepository;
        public SettingsService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _SettingsRepository = new Repository<Settings>(db);
            SettingsRepository = new RepositoryQuery<Settings>(_SettingsRepository);
        }
        public Settings Find(int id)
        {
            return _unitOfWork.Repository<Settings>().Find(id);
        }

        public Settings GetSettingsForDocument(string FieldName, int? DivisionId, int? SiteId, int? DocTypeId, int? DocCategoryId)
        {
            Settings Setting;

            Setting = (from H in db.Settings
                         where H.FieldName == FieldName && H.SiteId == SiteId && H.DivisionId == DivisionId && H.DocTypeId == DocTypeId
                       select H
                         ).FirstOrDefault();

            if(Setting==null)
                Setting = (from H in db.Settings
                           where H.FieldName == FieldName && H.SiteId == SiteId && H.DocTypeId == DocTypeId
                           select H
                             ).FirstOrDefault();

            if (Setting == null)
                Setting = (from H in db.Settings
                           where H.FieldName == FieldName && H.DocTypeId == DocTypeId
                           select H
                             ).FirstOrDefault();

            if (Setting == null)
                Setting = (from H in db.Settings
                           where H.FieldName == FieldName 
                           select H
                             ).FirstOrDefault();

            return Setting;

        }

        public DateTime GetDefaultDocDate(string TableName, int? DivisionId, int? SiteId, int? DocTypeId, int? DocCategoryId)
        {
            DateTime DefaultDocDate;

            if(DocTypeId !=null && DocCategoryId == null)
            {
                DocumentType DT = new DocumentTypeService(_unitOfWork).Find((int)DocTypeId);
                DocCategoryId = DT.DocumentCategoryId;
            }

            Settings S = GetSettingsForDocument(SettingFieldNameConstants.DefaultDocDate,  DivisionId,  SiteId,  DocTypeId,  DocCategoryId);
            if(S.Value=="Last Entry Date")
                {
                string mQry = @"SELECT H.DocDate FROM web." + TableName + @"s H WHERE H." + TableName + @"Id =
                                (
                                SELECT Max(H." + TableName + @"Id) AS " + TableName + @"Id
                                FROM web." + TableName + @"s H WITH(Nolock)
                                WHERE H.DocTypeId =" + DocTypeId + @" AND H.SiteId =" + SiteId + @" AND H.DivisionId =" + DivisionId + @"
                                )";
                IEnumerable<DateTime> DateList = db.Database.SqlQuery<DateTime>(mQry).ToList();
                DefaultDocDate = DateList.FirstOrDefault();
                }
            else
                DefaultDocDate = DateTime.Now;
        

            return DefaultDocDate;
        }

        public Settings Create(Settings pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<Settings>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<Settings>().Delete(id);
        }

        public void Delete(Settings pt)
        {
            _unitOfWork.Repository<Settings>().Delete(pt);
        }

        public void Update(Settings pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<Settings>().Update(pt);
        }

        public IEnumerable<Settings> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<Settings>()
                .Query()
                .OrderBy(q => q.OrderBy(c => c.SettingsId))                
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }


        public Settings Add(Settings pt)
        {
            _unitOfWork.Repository<Settings>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.Settings
                        orderby p.SettingsId
                        select p.SettingsId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.Settings
                        orderby p.SettingsId
                        select p.SettingsId).FirstOrDefault();
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

                temp = (from p in db.Settings
                        orderby p.SettingsId
                        select p.SettingsId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.Settings
                        orderby p.SettingsId
                        select p.SettingsId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }



        public void Dispose()
        {
        }


        public Task<IEquatable<Settings>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Settings> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
