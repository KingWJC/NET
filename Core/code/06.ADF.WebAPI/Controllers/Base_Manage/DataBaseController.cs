using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ADF.IBusiness;
using ADF.Entity;

namespace ADF.WebAPI.Controllers
{
    public class DataBaseController : BaseController
    {

        IDataBaseBusiness _dataBaseBus { get; }

        public DataBaseController(IDataBaseBusiness dataBaseBusiness)
        {
            _dataBaseBus = dataBaseBusiness;
        }

        [HttpGet]
        public List<TableSpace> GetTableSpaceData()
        {
            return _dataBaseBus.GetTableSpaces();
        }

        [HttpGet]
        public List<LockData> GetLockDatas()
        {
            return _dataBaseBus.GetLockDatas();
        }
    }
}