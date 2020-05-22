using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Practice_WebApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Practice_WebApi.Controllers
{
    public class EmployeeController : ApiController
    {
        private NorthwindContext _Context = new NorthwindContext();

        public EmployeeController()
        {
            _Context.Configuration.LazyLoadingEnabled = true;
            // 將 DB 操作記錄寫在 「輸出」視窗中
            _Context.Database.Log = s => Debug.WriteLine(s);
        }

        // GET api/<controller>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        public List<Employees> GetAllEmployee()
        {
            // 使用 AsNoTracking 的話，會 always 讀取最新資料，不會從 DB 的 Cache 取得資料。
            return _Context.Employees.AsNoTracking().ToList();
        }

        // GET api/<controller>/5
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public Employees Get(int id)
        {
            return _Context.Employees.Include(x=>x.Orders).FirstOrDefault(x => x.EmployeeID == id);
        }

        // POST api/<controller>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public void Post(Employees employee)
        {
            var errorMsg = new List<string>();
            if (string.IsNullOrEmpty(employee.FirstName) && string.IsNullOrEmpty(employee.LastName))
                errorMsg.Add("姓名填寫不完整 ( First Name + Last Name )");
            if (!employee.BirthDate.HasValue)
                errorMsg.Add("未填寫生日或格式錯誤 !");
            if (errorMsg.Any())
                throw new Exception(string.Join("\r\n", errorMsg));

            _Context.Employees.Add(employee);
            _Context.SaveChanges();
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public void Delete(int id)
        {
            var employee = _Context.Employees.Find(id);
            if (employee == null)
                throw new Exception($"此紀錄已不存在 [Id: {id}]");

            /// 法1:
            // ADO.Net Framework 在刪除的時候，會連帶一起刪除關聯的資料 
            // 但是必須先載入關聯的資料
            // _Context.Orders
            //   .Where(o => o.EmployeeID == employee.EmployeeID)
            //   .Load();     // loading relational data

            /// 法2:
            // 叫用 Clear 方法
            // employee.Orders.Clear();

            /// 法3:
            // 手動處理 => 自己去刪除有關聯的資料
            // 在北風範例資料庫中，如果我們直接刪除了某個 Order 而沒有處理 OrderDetail 的話，那麼與該訂單相關的 Detail 將導至 Foreign Key 衝突。
            // 而且因為 OrderDetail 的 OrderID 欄位不允許 Null，
            // 若要刪除該筆 Order ，就必須先將相關的 OrderDetail 刪除，再刪除 Order 才不會產生錯誤。
            _Context.Order_Details.RemoveRange(employee.Orders.SelectMany(x => x.Order_Details));
            _Context.Orders.RemoveRange(employee.Orders);
            _Context.Territories.RemoveRange(employee.Territories);
            _Context.Employees.Remove(employee);
            _Context.SaveChanges();
        }
    }
}