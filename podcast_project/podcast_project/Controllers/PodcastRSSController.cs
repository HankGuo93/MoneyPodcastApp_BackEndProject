using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using podcast_project.Models;
using System;
using System.Collections.Generic;

namespace podcast_project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PodcastRSSController : ControllerBase
    {
        private IConfiguration configuration;
        private DBInfoCheck dbCheck;
        private cls_Logger obj;
        public PodcastRSSController(IConfiguration iConfig)
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

        [HttpGet]
        [Route("GetNewestList")]
        public IActionResult GetNewestList([FromQuery] int userId)  // 獲取推薦清單
        {
            obj.LogError("Entering GetNewestList()");
            if (!dbCheck.findIdExsit<int>("usersId", "users", userId))
            {
                obj.LogError("user doesn't exist");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.notFound,
                    Msg = "user doesn't exist"
                });
            }
            List<PodcastRSSModel> objList = new List<PodcastRSSModel>();
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    obj.LogError("Db Connection Opened");
                    MySqlCommand cmd = new MySqlCommand("GetNewestList", conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userId", userId);
                    using (var dr = cmd.ExecuteReader())
                    {
                        obj.LogError("Data Read...");
                        while (dr.Read())
                        {
                            objList.Add(new PodcastRSSModel()
                            {
                                podcastRSSId = Convert.ToInt32(dr["podcastRSSId"]),
                                author = dr["author"].ToString(),
                                title = dr["title"].ToString(),
                                mp3Link = dr["mp3Link"].ToString(),
                                description = dr["description"].ToString(),
                                duration = Convert.ToInt32(dr["duration"]),
                                imgLink = dr["imgLink"].ToString(),
                                pubdate = Convert.ToDateTime(dr["pubdate"]),
                                timestamp = Convert.ToInt32(dr["timestamp"]),
                                isCollect = Convert.ToInt32(dr["isCollect"]) == 1 ? true : false,
                                isToListen = Convert.ToInt32(dr["isToListen"]) == 1 ? true : false
                            });
                        }
                        obj.LogError("Data Read Completed");
                    }
                    conn.Close();
                    return Ok(new JsonModel<List<PodcastRSSModel>>()
                    {
                        errorcode = errorcode.successed,
                        Msg = objList
                    });
                }
            }
            catch (Exception ex)
            {
                obj.LogError($"Error in GetNewestList():{ex.Message}");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.internalError,
                    Msg = "Internal Error"
                });
            }
        }

        [HttpGet]
        [Route("GetAllTag")]
        public IActionResult GetAllTag()    //獲取所有Tag
        {
            obj.LogError("Entering GetAllTag()");
            List<HashTagModel> objList = new List<HashTagModel>();
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    obj.LogError("Db Connection Opened");
                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM hashtag;", conn);
                    cmd.CommandType = System.Data.CommandType.Text;
                    using (var dr = cmd.ExecuteReader())
                    {
                        obj.LogError("Data Read...");
                        while (dr.Read())
                        {
                            objList.Add(new HashTagModel()
                            {
                                hashTagId = Convert.ToInt32(dr["hashTagId"]),
                                hashTag = dr["hashTag"].ToString(),
                            });
                        }
                        obj.LogError("Data Read Completed");
                    }
                    conn.Close();
                    return Ok(new JsonModel<List<HashTagModel>>()
                    {
                        errorcode = errorcode.successed,
                        Msg = objList
                    });
                }
            }
            catch (Exception ex)
            {
                obj.LogError($"Error in GetAllTag():{ex.Message}");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.internalError,
                    Msg = "Internal Error"
                });
            }
        }

        [HttpGet]
        [Route("GetTagList")]
        public IActionResult GetTagList([FromQuery] int hashtagId, [FromQuery] int userId)  //按照Tag回傳清單
        {
            obj.LogError("Entering GetTagList()");
            if (!(dbCheck.findIdExsit<int>("usersId", "users", userId) && dbCheck.findIdExsit<int>("hashtagId", "hashtag", hashtagId)))
            {
                obj.LogError("user or hashtag doesn't exist");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.notFound,
                    Msg = "user or hashtag doesn't exist"
                });
            }
            List<PodcastRSSModel> objList = new List<PodcastRSSModel>();
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    obj.LogError("Db Connection Opened");
                    MySqlCommand cmd = new MySqlCommand("GetTagList", conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@tagId", hashtagId);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    using (var dr = cmd.ExecuteReader())
                    {
                        obj.LogError("Data Read...");
                        while (dr.Read())
                        {
                            objList.Add(new PodcastRSSModel()
                            {
                                podcastRSSId = Convert.ToInt32(dr["podcastRSSId"]),
                                author = dr["author"].ToString(),
                                title = dr["title"].ToString(),
                                mp3Link = dr["mp3Link"].ToString(),
                                description = dr["description"].ToString(),
                                duration = Convert.ToInt32(dr["duration"]),
                                imgLink = dr["imgLink"].ToString(),
                                pubdate = Convert.ToDateTime(dr["pubdate"]),
                                timestamp = Convert.ToInt32(dr["timestamp"]),
                                isCollect = Convert.ToInt32(dr["isCollect"]) == 1 ? true : false,
                                isToListen = Convert.ToInt32(dr["isToListen"]) == 1 ? true : false
                            });
                        }
                        obj.LogError("Data Read Completed");
                    }
                    conn.Close();
                    return Ok(new JsonModel<List<PodcastRSSModel>>()
                    {
                        errorcode = errorcode.successed,
                        Msg = objList
                    });
                }
            }
            catch (Exception ex)
            {
                obj.LogError($"Error in GetTagList():{ex.Message}");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.internalError,
                    Msg = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("GetCollectList")]
        public IActionResult GetCollectList([FromQuery] int userId)  //獲取收藏清單
        {
            obj.LogError("Entering GetCollectList()");
            if (!dbCheck.findIdExsit<int>("usersId", "users", userId))
            {
                obj.LogError("user doesn't exist");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.notFound,
                    Msg = "user doesn't exist"
                });
            }
            List<PodcastRSSModel> objList = new List<PodcastRSSModel>();
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    obj.LogError("Db Connection Opened");
                    MySqlCommand cmd = new MySqlCommand("GetCollectList", conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userId", userId);
                    using (var dr = cmd.ExecuteReader())
                    {
                        obj.LogError("Data Read...");
                        while (dr.Read())
                        {
                            objList.Add(new PodcastRSSModel()
                            {
                                podcastRSSId = Convert.ToInt32(dr["podcastRSSId"]),
                                author = dr["author"].ToString(),
                                title = dr["title"].ToString(),
                                mp3Link = dr["mp3Link"].ToString(),
                                description = dr["description"].ToString(),
                                duration = Convert.ToInt32(dr["duration"]),
                                imgLink = dr["imgLink"].ToString(),
                                pubdate = Convert.ToDateTime(dr["pubdate"]),
                                timestamp = Convert.ToInt32(dr["timestamp"]),
                                isCollect = Convert.ToInt32(dr["isCollect"]) == 1 ? true : false,
                                isToListen = Convert.ToInt32(dr["isToListen"]) == 1 ? true : false
                            });
                        }
                        obj.LogError("Data Read Completed");
                    }
                    conn.Close();
                    return Ok(new JsonModel<List<PodcastRSSModel>>()
                    {
                        errorcode = errorcode.successed,
                        Msg = objList
                    });
                }
            }
            catch (Exception ex)
            {
                obj.LogError($"Error in GetCollectList():{ex.Message}");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.internalError,
                    Msg = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("GetToListenList")]
        public IActionResult GetToListenList([FromQuery] int userId)  //獲取待聽清單
        {
            obj.LogError("Entering GetToListenList()");
            if (!dbCheck.findIdExsit<int>("usersId", "users", userId))
            {
                obj.LogError("user doesn't exist");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.notFound,
                    Msg = "user doesn't exist"
                });
            }
            List<PodcastRSSModel> objList = new List<PodcastRSSModel>();
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    obj.LogError("Db Connection Opened");
                    MySqlCommand cmd = new MySqlCommand("GetToListenList", conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userId", userId);
                    using (var dr = cmd.ExecuteReader())
                    {
                        obj.LogError("Data Read...");
                        while (dr.Read())
                        {
                            objList.Add(new PodcastRSSModel()
                            {
                                podcastRSSId = Convert.ToInt32(dr["podcastRSSId"]),
                                author = dr["author"].ToString(),
                                title = dr["title"].ToString(),
                                mp3Link = dr["mp3Link"].ToString(),
                                description = dr["description"].ToString(),
                                duration = Convert.ToInt32(dr["duration"]),
                                imgLink = dr["imgLink"].ToString(),
                                pubdate = Convert.ToDateTime(dr["pubdate"]),
                                timestamp = Convert.ToInt32(dr["timestamp"]),
                                isCollect = Convert.ToInt32(dr["isCollect"]) == 1 ? true : false,
                                isToListen = Convert.ToInt32(dr["isToListen"]) == 1 ? true : false
                            });
                        }
                        obj.LogError("Data Read Completed");
                    }
                    conn.Close();
                    return Ok(new JsonModel<List<PodcastRSSModel>>()
                    {
                        errorcode = errorcode.successed,
                        Msg = objList
                    });
                }
            }
            catch (Exception ex)
            {
                obj.LogError($"Error in GetCollectList():{ex.Message}");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.internalError,
                    Msg = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("GetRecentList")]
        public IActionResult GetRecentList([FromQuery] int userId)  //獲取近期收聽紀錄
        {
            obj.LogError("Entering GetRecentList()");
            if (!dbCheck.findIdExsit<int>("usersId", "users", userId))
            {
                obj.LogError("user doesn't exist");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.notFound,
                    Msg = "user doesn't exist"
                });
            }
            List<PodcastRSSModel> objList = new List<PodcastRSSModel>();
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    obj.LogError("Db Connection Opened");
                    MySqlCommand cmd = new MySqlCommand("GetRecentList", conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userId", userId);
                    using (var dr = cmd.ExecuteReader())
                    {
                        obj.LogError("Data Read...");
                        while (dr.Read())
                        {
                            objList.Add(new PodcastRSSModel()
                            {
                                podcastRSSId = Convert.ToInt32(dr["podcastRSSId"]),
                                author = dr["author"].ToString(),
                                title = dr["title"].ToString(),
                                mp3Link = dr["mp3Link"].ToString(),
                                description = dr["description"].ToString(),
                                duration = Convert.ToInt32(dr["duration"]),
                                imgLink = dr["imgLink"].ToString(),
                                pubdate = Convert.ToDateTime(dr["pubdate"]),
                                timestamp = Convert.ToInt32(dr["timestamp"]),
                                isCollect = Convert.ToInt32(dr["isCollect"]) == 1 ? true : false,
                                isToListen = Convert.ToInt32(dr["isToListen"]) == 1 ? true : false
                            });
                        }
                        obj.LogError("Data Read Completed");
                    }
                    conn.Close();
                    return Ok(new JsonModel<List<PodcastRSSModel>>()
                    {
                        errorcode = errorcode.successed,
                        Msg = objList
                    });
                }
            }
            catch (Exception ex)
            {
                obj.LogError($"Error in GetRecentList():{ex.Message}");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.internalError,
                    Msg = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("GetEpisode")]
        public IActionResult GetEpisode([FromQuery] int userId, [FromQuery] int podcastRSSId)  // 獲取單集資訊
        {
            obj.LogError("Entering GetEpisode()");
            if (!(dbCheck.findIdExsit<int>("usersId", "users", userId) &&
                dbCheck.findIdExsit<int>("podcastRSSId", "podcastRSS", podcastRSSId)))
            {
                obj.LogError("user or podcastRSSId doesn't exist");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.notFound,
                    Msg = "user or podcastRSSId doesn't exist"
                });
            }
            List<PodcastRSSModel> objList = new List<PodcastRSSModel>();
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    obj.LogError("Db Connection Opened");
                    MySqlCommand cmd = new MySqlCommand("GetEpisode", conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@podcastRSSId", podcastRSSId);
                    using (var dr = cmd.ExecuteReader())
                    {
                        obj.LogError("Data Read...");
                        while (dr.Read())
                        {
                            objList.Add(new PodcastRSSModel()
                            {
                                podcastRSSId = Convert.ToInt32(dr["podcastRSSId"]),
                                author = dr["author"].ToString(),
                                title = dr["title"].ToString(),
                                mp3Link = dr["mp3Link"].ToString(),
                                description = dr["description"].ToString(),
                                duration = Convert.ToInt32(dr["duration"]),
                                imgLink = dr["imgLink"].ToString(),
                                pubdate = Convert.ToDateTime(dr["pubdate"]),
                                timestamp = Convert.ToInt32(dr["timestamp"]),
                                isCollect = Convert.ToInt32(dr["isCollect"]) == 1 ? true : false,
                                isToListen = Convert.ToInt32(dr["isToListen"]) == 1 ? true : false
                            });
                        }
                        obj.LogError("Data Read Completed");
                    }
                    conn.Close();
                    return Ok(new JsonModel<List<PodcastRSSModel>>()
                    {
                        errorcode = errorcode.successed,
                        Msg = objList
                    });
                }
            }
            catch (Exception ex)
            {
                obj.LogError($"Error in GetEpisode():{ex.Message}");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.internalError,
                    Msg = "Internal Error"
                });
            }
        }
    }
}
