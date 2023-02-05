using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using MovieStreaming.Custom.DatabaseHelpers;
using MovieStreaming.Custom.Models.Configuration;
using MovieStreaming.Areas.Admin.Models.ChangeLogs;
using MovieStreaming.Custom.Models.LogsModel;

namespace MovieStreaming.Custom.Helpers
{
    public class ChangeLogHelper
    {
        public ChangeLog SerializeObject(object before, object after, int idTable, int idEntryUser, int actionType)
        {
            ChangeLog changeLog = new ChangeLog()
            {
                TableId = idTable,
                EntryUserId = idEntryUser,
                ActionType = actionType
            };



            BinaryFormatter binaryFormatterBefore = new BinaryFormatter();
            if (before != null)
            {
                try
                {
                    changeLog.Before = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(before));
                }
                catch (Exception e) { }
                //using (MemoryStream memoryStream = new MemoryStream())
                //{
                //    try
                //    {
                //        binaryFormatterBefore.Serialize(memoryStream, JsonConvert.SerializeObject(before));
                //    }
                //    catch(Exception e) { }



                //    changeLog.Before = memoryStream.ToArray();
                //}
            }



            if (after != null)
            {
                try
                {
                    changeLog.After = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(after));
                }
                catch (Exception e)
                {



                }
                //using (MemoryStream memoryStream = new MemoryStream())
                //{
                //    try
                //    {
                //        binaryFormatterBefore.Serialize(memoryStream, JsonConvert.SerializeObject(after));
                //    }
                //    catch (Exception e) { 

                //    }
                //    changeLog.After = memoryStream.ToArray();
                //}
            }



            return changeLog;
        }
        private string DeserializeObject(byte[] bytes)
        {
            //BinaryFormatter binForm = new BinaryFormatter();
            //using (MemoryStream memStream = new MemoryStream())
            //{
            //    memStream.Write(bytes, 0, bytes.Length);
            //    memStream.Seek(0, SeekOrigin.Begin);
            //    return (string)binForm.Deserialize(memStream);
            //}



            return (string)Encoding.UTF8.GetString(bytes);
        }
        public List<LogData> GetDeserializeObject(byte[] beforeParameter, byte[] afterParameter)
        {
            var logData = new List<LogData>();
            if (beforeParameter != null && afterParameter != null)
            {
                var before = new ChangeLogHelper().DeserializeObject(beforeParameter);
                var after = new ChangeLogHelper().DeserializeObject(afterParameter);

                var jObjBefore = (JObject)JsonConvert.DeserializeObject<dynamic>(before);
                var jObjAfter = (JObject)JsonConvert.DeserializeObject<dynamic>(after);

                for (int i = 0; i < jObjBefore.Children().Count(); i++)
                {
                    var propBefore = jObjBefore.Children().ElementAt(i) as JProperty;
                    var propAfter = jObjAfter.Children().ElementAt(i) as JProperty;
                    logData.Add(new LogData { Column = propAfter.Name, After = propAfter.Value.ToString(), Before = propBefore.Value.ToString() });
                }
                for (int i = 0; i < logData.Count(); i++)
                {
                    if (logData[i].Column == "Password")
                    {
                        logData[i].Before = "*****";
                        logData[i].After = "*****";
                    }
                    if (logData[i].Column == "ConfirmPassword")
                    {
                        logData[i].Before = "*****";
                        logData[i].After = "*****";
                    }
                    if (logData[i].Column == "Salt")
                    {
                        logData[i].Before = "*****";
                        logData[i].After = "*****";
                    }
                }
                return logData;
            }
            else
            {
                var result = new ChangeLogHelper().DeserializeObject((beforeParameter != null ? beforeParameter : afterParameter));
                var jsonResult = (JObject)JsonConvert.DeserializeObject<dynamic>(result);

                for (int i = 0; i < jsonResult.Children().Count(); i++)
                {
                    var jProperty = jsonResult.Children().ElementAt(i) as JProperty;
                    logData.Add(new LogData { Column = jProperty.Name, Before = jProperty.Value.ToString(), After = (beforeParameter != null ? "Deleted" : "Inserted") });
                }
                for (int i = 0; i < logData.Count(); i++)
                {
                    if (logData[i].Column == "Password")
                    {
                        logData[i].Before = "*****";
                        logData[i].After = "*****";
                    }
                    if (logData[i].Column == "ConfirmPassword")
                    {
                        logData[i].Before = "*****";
                        logData[i].After = "*****";
                    }
                    if (logData[i].Column == "Salt")
                    {
                        logData[i].Before = "*****";
                        logData[i].After = "*****";
                    }
                }
                return logData;
            }
        }
        public bool CompareHash(byte[] first, byte[] second)
        {
            bool result = true;
            for (int i = 0; i < first.Length; i++)
            {
                if (first[i] != second[i])
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        public async Task<DatabaseExecuteResult> AddLog(ChangeLog changeLog)
        {
            var createUpdateResult = await new Query().Execute("CreateUpdateDeleteChangeLog @CRUDOperation,@TableId,@EntryUserId,@Before,@After,@ActionType", new
            {
                CRUDOperation = (int)CRUDOperation.Create,
                @TableId = changeLog.TableId,
                @EntryUserId = changeLog.EntryUserId,
                @Before = changeLog.Before,
                @After = changeLog.After,
                @ActionType = changeLog.ActionType

            });
            return createUpdateResult;
        }
    }
}
