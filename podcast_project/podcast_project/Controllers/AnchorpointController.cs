using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using podcast_project.Models;
using System;
using System.Collections.Generic;

namespace podcast_project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnchorpointController : ControllerBase
    {
        private IConfiguration configuration;
        private DBInfoCheck dbCheck;
        private cls_Logger obj;

        public AnchorpointController(IConfiguration iConfig)
        {
            configuration = iConfig;
            dbCheck = new DBInfoCheck(iConfig);
            obj = new cls_Logger(configuration);
        }

        private MySqlConnection GetConnection()
        {
            string _Conster = configuration.GetValue<string>("AppSettings:myConnString");
            return new MySqlConnection(_Conster);
        }

        [HttpPost]
        [Route("anchor")]
        public IActionResult addAnchorpoint([FromForm] AnchorpointModel anchorpoint)  //新增錨點
        {
            if (anchorpoint.content.ToString().Length > 255)
            {
                return BadRequest(new JsonModel<string>()
                {
                    errorcode = errorcode.formatError,
                    Msg = "format error"
                });
            }
            obj.LogError("Entering addAnchorpoint()");
            if (!dbCheck.findIdExsit<int>("podcastRSSId", "podcastRSS", anchorpoint.podcastRSSId) ||
                !dbCheck.findIdExsit<int>("usersId", "users", anchorpoint.usersId))
            {
                obj.LogError("usersId or podcastRSSId doesn't exist");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.notFound,
                    Msg = "usersId or podcastRSSId doesn't exist"
                });
            }
            try
            {
                obj.LogError("usersId: " + anchorpoint.usersId);
                obj.LogError("podcastRSSId: " + anchorpoint.podcastRSSId);
                obj.LogError("time: " + anchorpoint.anchorTime);
                obj.LogError("content: " + anchorpoint.content);
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    obj.LogError("Db Connection Opened");
                    MySqlCommand cmd = new MySqlCommand($"INSERT INTO anchorpoints (usersId, podcastRSSId, time, content)" +
                        $" VALUES ('{anchorpoint.usersId}', '{anchorpoint.podcastRSSId}','{anchorpoint.anchorTime}','{anchorpoint.content}');", conn);
                    cmd.CommandType = System.Data.CommandType.Text; ;
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    return Ok(new JsonModel<string>()
                    {
                        errorcode = errorcode.successed,
                        Msg = "successed"
                    });
                }
            }
            catch (Exception ex)
            {
                obj.LogError($"Error in addAnchorpoint():{ex.Message}");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.internalError,
                    Msg = "Internal Error"
                });
            }
        }

        [HttpPut]
        [Route("modify")]
        public IActionResult modifyAnchorpoint([FromForm] AnchorpointWithoutUserModel anchorpoint)  //更新錨點
        {
            if (anchorpoint.content.ToString().Length > 255)
            {
                return BadRequest(new JsonModel<string>()
                {
                    errorcode = errorcode.formatError,
                    Msg = "format error"
                });
            }
            obj.LogError("Entering modifyAnchorpoint()");
            if (!dbCheck.findIdExsit<int>("anchorpointId", "anchorpoints", anchorpoint.anchorpointId))
            {
                obj.LogError("anchorpointId doesn't exist");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.notFound,
                    Msg = "anchorpointId doesn't exist"
                });
            }
            try
            {
                obj.LogError("usersId: " + anchorpoint.anchorpointId);
                obj.LogError("time: " + anchorpoint.anchorTime);
                obj.LogError("content: " + anchorpoint.content);
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    obj.LogError("Db Connection Opened");
                    MySqlCommand cmd = new MySqlCommand($"UPDATE anchorpoints SET" +
                        $" time = '{anchorpoint.anchorTime}', content = '{anchorpoint.content}' WHERE anchorpointId = '{anchorpoint.anchorpointId}';", conn);
                    cmd.CommandType = System.Data.CommandType.Text; ;
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    return Ok(new JsonModel<string>()
                    {
                        errorcode = errorcode.successed,
                        Msg = "successed"
                    });
                }
            }
            catch (Exception ex)
            {
                obj.LogError($"Error in modifyAnchorpoint():{ex.Message}");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.internalError,
                    Msg = "Internal Error"
                });
            }
        }

        [HttpDelete]
        [Route("remove")]
        public IActionResult removeAnchorpoint([FromForm] AnchorpointIdModel anchorpoint)  //刪除錨點
        {
            obj.LogError("Entering removeAnchorpoint()");
            if (!dbCheck.findIdExsit<int>("anchorpointId", "anchorpoints", anchorpoint.anchorpointId))
            {
                obj.LogError("This record does not exist");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.notFound,
                    Msg = "This record does not exist"
                });
            }
            try
            {
                obj.LogError("anchorpointId: " + anchorpoint.anchorpointId);
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    obj.LogError("Db Connection Opened");
                    MySqlCommand cmd = new MySqlCommand(
                        $"DELETE FROM anchorpoints WHERE anchorpointId = {anchorpoint.anchorpointId}", conn);
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    return Ok(new JsonModel<string>()
                    {
                        errorcode = errorcode.successed,
                        Msg = "successed"
                    });
                }
            }
            catch (Exception ex)
            {
                obj.LogError($"Error in removeAnchorpoint():{ex.Message}");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.internalError,
                    Msg = "Internal Error"
                });
            }
        }

        [HttpGet]
        [Route("getAll")]
        public IActionResult GetAnchorpointListByAll([FromQuery] int usersId)  // 獲取使用者錨點清單
        {
            obj.LogError("Entering GetAnchorpointListByAll()");
            if (!dbCheck.findIdExsit<int>("usersId", "users", usersId))
            {
                obj.LogError("user doesn't exist");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.notFound,
                    Msg = "user doesn't exist"
                });
            }
            List<AnchorpointWithTitleModel> objList = new List<AnchorpointWithTitleModel>();
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    obj.LogError("Db Connection Opened");
                    MySqlCommand cmd = new MySqlCommand("GetAnchorpointListByAll", conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userId", usersId);
                    using (var dr = cmd.ExecuteReader())
                    {
                        obj.LogError("Data Read...");
                        while (dr.Read())
                        {
                            objList.Add(new AnchorpointWithTitleModel()
                            {
                                anchorpointId = Convert.ToInt32(dr["anchorpointId"]),
                                anchorTime = Convert.ToInt32(dr["time"]),
                                content = dr["content"].ToString(),
                                podcastRSSId = Convert.ToInt32(dr["podcastRSSId"]),
                                title = dr["title"].ToString()
                            });
                        }
                        obj.LogError("Data Read Completed");
                    }
                    conn.Close();
                    return Ok(new JsonModel<List<AnchorpointWithTitleModel>>()
                    {
                        errorcode = errorcode.successed,
                        Msg = objList
                    });
                }
            }
            catch (Exception ex)
            {
                obj.LogError($"Error in GetAnchorpointListByAll():{ex.Message}");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.internalError,
                    Msg = "Internal Error"
                });
            }
        }

        [HttpGet]
        [Route("getByEpisode")]
        public IActionResult GetAnchorpointListByEpisode([FromQuery] int usersId, [FromQuery] int podcastRSSId)  // 獲取單集錨點清單
        {
            obj.LogError("Entering GetAnchorpointListByEpisode()");
            if (!dbCheck.findIdExsitTwoParam<int>("usersId", "podcastRSSId", "anchorpoints", usersId, podcastRSSId))
            {
                obj.LogError("user or podcastRSSId doesn't exist");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.notFound,
                    Msg = "user or podcastRSSId doesn't exist"
                });
            }
            List<AnchorpointWithoutUserModel> objList = new List<AnchorpointWithoutUserModel>();
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    obj.LogError("Db Connection Opened");
                    MySqlCommand cmd = new MySqlCommand($"SELECT anchorpointId, time, content FROM anchorpoints WHERE " +
                        $"usersId = {usersId} AND podcastRSSId = {podcastRSSId}", conn);
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@userId", usersId);
                    using (var dr = cmd.ExecuteReader())
                    {
                        obj.LogError("Data Read...");
                        while (dr.Read())
                        {
                            objList.Add(new AnchorpointWithoutUserModel()
                            {
                                anchorpointId = Convert.ToInt32(dr["anchorpointId"]),
                                anchorTime = Convert.ToInt32(dr["time"]),
                                content = dr["content"].ToString(),
                            });
                        }
                        obj.LogError("Data Read Completed");
                    }
                    conn.Close();
                    return Ok(new JsonModel<List<AnchorpointWithoutUserModel>>()
                    {
                        errorcode = errorcode.successed,
                        Msg = objList
                    });
                }
            }
            catch (Exception ex)
            {
                obj.LogError($"Error in GetAnchorpointListByEpisode():{ex.Message}");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.internalError,
                    Msg = "Internal Error"
                });
            }
        }
    }
}
