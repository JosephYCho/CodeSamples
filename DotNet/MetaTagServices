using Myapp.Models.Domain;
using Myapp.Models.Requests;
using Myapp.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Myapp.Data.Providers;

namespace Myapp.Services
{
    public class MetaTagServices : IMetaTagService
    {
        private readonly IDataProvider _dataProvider;

        public MetaTagServices(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public List<MetaTag> Select()
        {
            List<MetaTag> metaTags = null;
            _dataProvider.ExecuteCmd(
                "dbo.MetaTags_SelectAll",
                null,
                (reader, set) =>
                {
                    if (metaTags == null)
                    {
                        metaTags = new List<MetaTag>();
                    }
                    MetaTag oneMeta = Mapper(reader);
                    metaTags.Add(oneMeta);
                }
                );
            return metaTags;
        }

        public MetaTag Select(int id)
        {
            MetaTag selectedMeta = null;
            _dataProvider.ExecuteCmd(
                "dbo.MetaTags_SelectById",
                (paramCol) =>
                {
                    paramCol.AddWithValue("@Id", id);
                },
                (reader, set) =>
                {
                    selectedMeta = Mapper(reader);
                });
            return selectedMeta;
        }

        public int Insert(MetaTagCreateRequest model, int userId)
        {
            int id = 0;

            _dataProvider.ExecuteNonQuery(
                "dbo.MetaTags_Insert",
                (paramCol) =>
                {
                    paramCol.AddWithValue("@Data", model.Data);
                    paramCol.AddWithValue("@MetaTagTypeId", model.MetaTagTypeId);
                    paramCol.AddWithValue("@CreatedBy", userId);

                    SqlParameter idParameter = new SqlParameter("@Id", SqlDbType.Int);
                    idParameter.Direction = ParameterDirection.Output;
                    paramCol.Add(idParameter);
                },
                (param) =>
                {
                    Int32.TryParse(param["@id"].Value.ToString(), out id);
                }
                );
            return id;
        }

        public void Update(MetaTagUpdateRequest model)
        {
            _dataProvider.ExecuteNonQuery(
                "dbo.MetaTags_Update",
                (paramCol) =>
                {
                    paramCol.AddWithValue("@Id", model.Id);
                    paramCol.AddWithValue("@Data", model.Data);
                    paramCol.AddWithValue("@MetaTagTypeId", model.MetaTagTypeId);
                }, null
                );
        }

        public void Delete(int id)
        {
            _dataProvider.ExecuteNonQuery(
                "dbo.MetaTags_Delete",
                (paramCol) =>
                {
                    paramCol.AddWithValue("@Id", id);
                }, null
                );
        }

        private MetaTag Mapper(IDataReader reader)
        {
            MetaTag metaTag = new MetaTag();
            int startingIndex = 0;

            metaTag.Id = reader.GetSafeInt32(startingIndex++);
            metaTag.Data = reader.GetSafeString(startingIndex++);
            metaTag.MetaTagTypeId = reader.GetSafeInt32(startingIndex++);
            metaTag.DateCreated = reader.GetSafeDateTime(startingIndex++);
            metaTag.DateModified = reader.GetSafeDateTime(startingIndex++);
            return metaTag;
        }
    }
}
