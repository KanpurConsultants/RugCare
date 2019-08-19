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
using Model.ViewModel;
using Model.ViewModels;

namespace Service
{
    public interface IDocumentTypeTimeExtensionRequestService : IDisposable
    {
        DocumentTypeTimeExtensionRequest Create(DocumentTypeTimeExtensionRequest pt,string UserName);
        void Delete(int id);
        void Delete(DocumentTypeTimeExtensionRequest pt);
        void Update(DocumentTypeTimeExtensionRequest pt,string UserName);
        IQueryable<DocumentTypeTimeExtensionRequestViewModel> GetDocumentTypeTimeExtensionRequestList(string UName);
        Task<IEquatable<DocumentTypeTimeExtensionRequest>> GetAsync();
        Task<DocumentTypeTimeExtensionRequest> FindAsync(int id);
        DocumentTypeTimeExtensionRequest Find(int id);
        int NextId(int id);
        int PrevId(int id);
        IQueryable<ComboBoxResult> GetDocTypesHelpList(string Type,string SearchTerm);
        DocumentTypeTimeExtensionRequestViewModel GetHistory(string UserName);
    }

    public class DocumentTypeTimeExtensionRequestService : IDocumentTypeTimeExtensionRequestService
    {
        private ApplicationDbContext db;
        public DocumentTypeTimeExtensionRequestService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public DocumentTypeTimeExtensionRequest Find(int id)
        {
            return db.DocumentTypeTimeExtensionRequest.Find(id);
        }

        public DocumentTypeTimeExtensionRequest Create(DocumentTypeTimeExtensionRequest pt, string UserName)
        {
            pt.CreatedBy = UserName;
            pt.CreatedDate = DateTime.Now;
            pt.ModifiedBy = UserName;
            pt.ModifiedDate = DateTime.Now;

            pt.ObjectState = ObjectState.Added;
            db.DocumentTypeTimeExtensionRequest.Add(pt);

            return pt;
        }

        public void Delete(int id)
        {
            DocumentTypeTimeExtensionRequest pd = db.DocumentTypeTimeExtensionRequest.Find(id);

            pd.ObjectState = Model.ObjectState.Deleted;
            db.DocumentTypeTimeExtensionRequest.Remove(pd);
        }

        public void Delete(DocumentTypeTimeExtensionRequest pt)
        {
            pt.ObjectState = Model.ObjectState.Deleted;
            db.DocumentTypeTimeExtensionRequest.Remove(pt);
        }

        public void Update(DocumentTypeTimeExtensionRequest pt, string UserName)
        {
            pt.ModifiedBy = UserName;
            pt.ModifiedDate = DateTime.Now;
            pt.ObjectState = ObjectState.Modified;
            db.DocumentTypeTimeExtensionRequest.Add(pt);
        }

        public IQueryable<DocumentTypeTimeExtensionRequestViewModel> GetDocumentTypeTimeExtensionRequestList(string UName)
        {
            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];
            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

            bool Admin = UserRoles.Contains("Admin");

            return (from p in db.DocumentTypeTimeExtensionRequest
                    where p.IsApproved  == null 
                    && (Admin ? 1 == 1 : p.CreatedBy == UName)
                    orderby p.DocumentTypeTimeExtensionRequestId descending
                    select new DocumentTypeTimeExtensionRequestViewModel
                    {
                        DocTypeId = p.DocTypeId,
                        DocTypeName = p.DocType.DocumentTypeName,
                        SiteName =p.Site.SiteCode,
                        DivisionName=p.Division.DivisionName,
                        DivisionId = p.DivisionId,
                        SiteId = p.SiteId,
                        DocDate = p.DocDate,
                        DocumentTypeTimeExtensionRequestId = p.DocumentTypeTimeExtensionRequestId,
                        ExpiryDate = p.ExpiryDate,
                        NoOfRecords = p.NoOfRecords,
                        Reason = p.Reason,
                        Type = p.Type,
                        UserName = p.UserName,
                    });

        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.DocumentTypeTimeExtensionRequest
                        orderby p.DocumentTypeTimeExtensionRequestId
                        select p.DocumentTypeTimeExtensionRequestId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.DocumentTypeTimeExtensionRequest
                        orderby p.DocumentTypeTimeExtensionRequestId
                        select p.DocumentTypeTimeExtensionRequestId).FirstOrDefault();
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

                temp = (from p in db.DocumentTypeTimeExtensionRequest
                        orderby p.DocumentTypeTimeExtensionRequestId
                        select p.DocumentTypeTimeExtensionRequestId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.DocumentTypeTimeExtensionRequest
                        orderby p.DocumentTypeTimeExtensionRequestId
                        select p.DocumentTypeTimeExtensionRequestId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }


        public IQueryable<ComboBoxResult> GetDocTypesHelpList(string Type,string SearchTerm)
        {

            var Query = from p in db.DocumentType
                        select new
                        {
                            Id = p.DocumentTypeId,
                            Name = p.DocumentTypeName,
                            GatePassGenerated=p.SupportGatePass,
                        };

            if (Type == DocumentTimePlanTypeConstants.GatePassCancel || Type == DocumentTimePlanTypeConstants.GatePassCreate)
                Query = Query.Where(m => m.GatePassGenerated == true);

            if (!string.IsNullOrEmpty(SearchTerm))
                Query = Query.Where(m => m.Name.ToLower().Contains(SearchTerm.ToLower()));

            return from p in Query
                   where p.GatePassGenerated == p.GatePassGenerated
                   orderby p.Name
                   select new ComboBoxResult
                   {
                       id = p.Id.ToString(),
                       text = p.Name,
                   };


        }

        public DocumentTypeTimeExtensionRequestViewModel GetHistory(string UserName)
        {
            return (from p in db.DocumentTypeTimeExtensionRequest
                    where p.CreatedBy == UserName
                    orderby p.DocumentTypeTimeExtensionRequestId descending
                    select new DocumentTypeTimeExtensionRequestViewModel
                    {
                        NoOfRecords = p.NoOfRecords,
                        DocTypeId = p.DocTypeId,
                        DocTypeName = p.DocType.DocumentTypeName,
                        ExpiryDate = p.ExpiryDate,
                        DocDate = p.DocDate,
                        Type = p.Type,
                        UserName = p.UserName,
                        Reason = p.Reason,
                    }).FirstOrDefault();
        }

        public void Dispose()
        {
        }


        public Task<IEquatable<DocumentTypeTimeExtensionRequest>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<DocumentTypeTimeExtensionRequest> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
