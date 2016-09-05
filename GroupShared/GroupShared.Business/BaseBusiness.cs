using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GroupShared.Business.Entities;
using Newtonsoft.Json;

namespace GroupShared.Business
{
    public class BaseBusiness<T> where T : BaseEntity
    {
        public BaseBusiness()
        {
            if (!Directory.Exists(ConfigHelper.BaselineFolder))
            {
                Directory.CreateDirectory(ConfigHelper.BaselineFolder);
            }
            if (!Directory.Exists(ConfigHelper.ExportFolder))
            {
                Directory.CreateDirectory(ConfigHelper.ExportFolder);
            }
        }

        public List<T> GetList(string fileName = null)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                if (typeof(T) == typeof(User))
                {
                    fileName = ConfigHelper.UserFile;
                }
                else
                {
                    fileName = ConfigHelper.PaymentFile;
                }
            }

            if (!File.Exists(fileName))
            {
                return new List<T>();
            }

            var json = File.ReadAllText(fileName);

            var list = JsonConvert.DeserializeObject<List<T>>(json);

            return list;
        }

        public void SaveList(List<T> entities, string fileName = null)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                if (typeof(T) == typeof(User))
                {
                    fileName = ConfigHelper.UserFile;
                }
                else
                {
                    fileName = ConfigHelper.PaymentFile;
                }
            }
            
            var json = JsonConvert.SerializeObject(entities);

            File.WriteAllText(fileName, json);
        }

        public BaseEntity Get(Guid id)
        {
            return GetList().FirstOrDefault(x => x.Id == id);
        }

        public void Save(T entity)
        {
            var list = GetList();
            var oldEntity = list.FirstOrDefault(x => x.Id == entity.Id);
            if (oldEntity != null)
            {
                UpdateProperties(entity, oldEntity);
            }
            else
            {
                entity.Created = entity.Updated = DateTime.Now;
                list.Add(entity);
            }

            SaveList(list);
        }

        public void Delete(Guid id)
        {
            var list = GetList();
            var entity = list.FirstOrDefault(x => x.Id == id);
            if (entity!=null)
            {
                list.Remove(entity);
            }

            SaveList(list);
        }

        public virtual void UpdateProperties(T source, T destination) { }
    }
}