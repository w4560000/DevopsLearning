using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;

namespace dotnet_core_exporter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModelTestController : ControllerBase
    {
        /// <summary>
        /// https://localhost:5001/api/ModelTest/QueryGetTest1?id=1&isCheck=true => ok
        /// https://localhost:5001/api/ModelTest/QueryGetTest1?isCheck=true => error
        /// id 必填, isCheck 則無限制
        /// </summary>
        [HttpGet(nameof(QueryGetTest1))]
        public string QueryGetTest1([BindRequired]int id, bool isCheck)
        {
            return JsonSerializer.Serialize(new { id, isCheck});
        }

        /// <summary>
        /// https://localhost:5001/api/ModelTest/QueryGetTest2?id=1&isCheck=true
        /// id、isCheck 皆可 binding 到 (V)
        /// </summary>
        [HttpGet(nameof(QueryGetTest2))]
        public string QueryGetTest2([FromQuery, Bind(include: "id")] int id, bool isCheck)
        {
            return JsonSerializer.Serialize(new { id, isCheck });
        }

        /// <summary>
        /// https://localhost:5001/api/ModelTest/QueryGetTest3?id=1&isCheck=true
        /// id、isCheck 皆無法 binding 到 (X)
        /// </summary>
        [HttpGet(nameof(QueryGetTest3))]
        public string QueryGetTest3([FromQuery, BindNever] int id, [FromQuery, BindNever] bool isCheck)
        {
            return JsonSerializer.Serialize(new { id, isCheck });
        }

        /// <summary>
        /// https://localhost:5001/api/ModelTest/QueryGetTest4?id=1&isCheck=true
        /// id、isCheck 皆可 binding 到  (V)
        /// </summary>
        [HttpGet(nameof(QueryGetTest4))]
        public string QueryGetTest4([FromQuery] TestClass model)
        {
            return JsonSerializer.Serialize(model);
        }

        /// <summary>
        /// https://localhost:5001/api/ModelTest/QueryGetTest5?id=1&isCheck=true
        /// id 可 binding 到  (V)
        /// isCheck 無法 binding 到  (X)
        /// </summary>
        [HttpGet(nameof(QueryGetTest5))]
        public string QueryGetTest5([FromQuery, Bind(include: "Id")] TestClass model)
        {
            return JsonSerializer.Serialize(model);
        }

        /// <summary>
        /// https://localhost:5001/api/ModelTest/QueryGetTest6?id=1&isCheck=true
        /// id 可 binding 到  (V)
        /// isCheck 無法 binding 到  (X)
        /// </summary>
        [HttpGet(nameof(QueryGetTest6))]
        public string QueryGetTest6([FromQuery, Bind(Prefix = "Test2")] TestClass model)
        {
            return JsonSerializer.Serialize(model);
        }

        /// <summary>
        /// https://localhost:5001/api/ModelTest/RouteGetTest1/1?isCheck=true
        /// id、isCheck 皆可 binding 到 (V)
        /// 
        /// 若無傳 isCheck 則會被擋掉
        /// </summary>
        [HttpGet(nameof(RouteGetTest1) + "/{id}")]
        public string RouteGetTest1([FromRoute] int id,[BindRequired] bool isCheck)
        {
            return JsonSerializer.Serialize(new { id, isCheck });
        }

        /// <summary>
        /// https://localhost:5001/api/ModelTest/FormPostTest1
        /// 
        /// Id: 1
        /// IsCheck: true
        /// 
        /// id、isCheck 皆可 binding 到  (V)
        /// </summary>
        [HttpPost(nameof(FormPostTest1))]
        public string FormPostTest1([FromForm, Bind(include: "Id, IsCheck")] TestClass model)
        {
            return JsonSerializer.Serialize(model);
        }

        /// <summary>
        /// https://localhost:5001/api/ModelTest/FormPostTest2
        /// 
        /// Id: 1
        /// IsCheck: true
        /// 
        /// id 可 binding 到  (V)
        /// isCheck 無法 binding 到  (X)
        /// </summary>
        [HttpPost(nameof(FormPostTest2))]
        public string FormPostTest2([FromForm, Bind(include: "Id")] TestClass model)
        {
            return JsonSerializer.Serialize(model);
        }

        /// <summary>
        /// https://localhost:5001/api/ModelTest/FormPostTest3
        /// 
        /// Test2.Id = 1
        /// Test2.IsCheck = true
        /// 
        /// id、isCheck 皆可 binding 到  (V)
        /// 
        /// 若是 Id = 1, IsCheck = true 則無法 binding 到 (X)
        /// </summary>
        [HttpPost(nameof(FormPostTest3))]
        public string FormPostTest3([FromForm, Bind(Prefix = "Test2")] TestClass model)
        {
            return JsonSerializer.Serialize(model);
        }

        /// <summary>
        /// https://localhost:5001/api/ModelTest/BodyPostTest1
        /// id 可 binding 到  (V)
        /// isCheck 無法 binding 到  (X)
        /// </summary>
        [HttpPost(nameof(BodyPostTest1))]
        public string BodyPostTest1([FromBody, Bind(Prefix = "Test2")] TestClass1 model)
        {
            return JsonSerializer.Serialize(model);
        }
    }

    public class TestClass
    {
        public int Id { get; set; }

        public bool IsCheck { get; set; }
    }

    public class TestClass1
    {
        public int Id { get; set; }

        public bool IsCheck { get; set; }

        public TestClass2 Test2 { get; set; } = new TestClass2();
    }

    public class TestClass2
    {
        public int A { get; set; }
    }
}