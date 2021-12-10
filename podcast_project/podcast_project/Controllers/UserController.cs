using Microsoft.AspNetCore.Authorization;
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
    public class UserController : ControllerBase
    {
        private IConfiguration configuration;
        private DBInfoCheck dbCheck;
        private cls_Logger obj;
        public UserController(IConfiguration iConfig)
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
        [Route("users")]
        public IActionResult addUser([FromForm] IDFAdModel user)  //新增使用者
        {
            List<UserIdModel> objList = new List<UserIdModel>();
            obj.LogError("Entering addUser()");
            if (dbCheck.findIdExsit<string>("IDFA", "users", user.IDFA))
            {
                obj.LogError("user already exist");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.alreadyExist,
                    Msg = "user already exist"
                });
            }
            try
            {
                obj.LogError("IDFA: " + user.IDFA);
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    obj.LogError("Db Connection Opened");
                    MySqlCommand cmd = new MySqlCommand($"INSERT INTO users (IDFA) VALUES ('{user.IDFA}');" +
                        $"SELECT usersId FROM users WHERE IDFA = '{user.IDFA}';", conn);
                    cmd.CommandType = System.Data.CommandType.Text; ;
                    using (var dr = cmd.ExecuteReader())
                    {
                        obj.LogError("Data Read...");
                        dr.Read();
                        objList.Add(new UserIdModel()
                        {
                            usersId = Convert.ToInt32(dr["usersId"]),
                        });
                        obj.LogError("Data Read Completed");

                    }
                    conn.Close();
                    return Ok(new JsonModel<List<UserIdModel>>()
                    {
                        errorcode = errorcode.successed,
                        Msg = objList
                    });
                }
            }
            catch (Exception ex)
            {
                obj.LogError($"Error in addUser():{ex.Message}");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.internalError,
                    Msg = "Internal Error"
                });
            }
        }

        [Authorize]
        [HttpGet]
        [Route("users")]
        public IActionResult getUserId([FromQuery] string IDFA)    //取得使用者ID
        {
            obj.LogError("Entering getUserId");
            if (!dbCheck.findIdExsit<string>("IDFA", "users", IDFA))
            {
                obj.LogError("user doesn't exist");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.notFound,
                    Msg = "user doesn't exist"
                });
            }
            List<UserIdModel> objList = new List<UserIdModel>();
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    obj.LogError("Db Connection Opened");
                    MySqlCommand cmd = new MySqlCommand($"SELECT usersId FROM users WHERE IDFA = '{IDFA}';", conn);
                    cmd.CommandType = System.Data.CommandType.Text;
                    string usersid;
                    using (var dr = cmd.ExecuteReader())
                    {
                        obj.LogError("Data Read...");
                        dr.Read();
                        objList.Add(new UserIdModel()
                        {
                            usersId = Convert.ToInt32(dr["usersId"])
                        });
                        usersid = dr["usersId"].ToString();
                        obj.LogError("Data Read Completed");

                    }

                    conn.Clone();
                    return Ok(new JsonModel<List<UserIdModel>>()
                    {
                        errorcode = errorcode.successed,
                        Msg = objList
                    });
                }
            }
            catch (Exception ex)
            {
                obj.LogError($"Error in getUserId():{ex.Message}");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.internalError,
                    Msg = "Internal Error"
                });
            }
        }

        [HttpPost]
        [Route("collect")]
        public IActionResult userCollect([FromForm] UserRSSModel collect)  //使用者收藏
        {
            obj.LogError("Entering userCollect()");
            if (!(dbCheck.findIdExsit<int>(
                "usersId", "users", collect.usersId) && dbCheck.findIdExsit<int>("podcastRSSId", "podcastRSS", collect.podcastRSSId)))
            {
                obj.LogError("user or podcastRSS doesn't exist");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.notFound,
                    Msg = "user or podcastRSS doesn't exist"
                });
            }
            else if (dbCheck.findIdExsitTwoParam<int>("usersId", "podcastRSSId", "user_collect", collect.usersId, collect.podcastRSSId))
            {
                obj.LogError("This record already exist");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.alreadyExist,
                    Msg = "This record already exist"
                });
            }
            try
            {
                obj.LogError("usersId: " + collect.usersId);
                obj.LogError("podcastRSSId: " + collect.podcastRSSId);
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    obj.LogError("Db Connection Opened");
                    MySqlCommand cmd = new MySqlCommand(
                        $"INSERT INTO user_collect (usersId, podcastRSSId) VALUES ({collect.usersId}, {collect.podcastRSSId})", conn);
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
                obj.LogError($"Error in userCollect():{ex.Message}");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.internalError,
                    Msg = "Internal Error"
                });
            }
        }

        [HttpDelete]
        [Route("collect")]
        public IActionResult userCollectDelet([FromForm] UserRSSModel collect)  //使用者取消收藏
        {
            obj.LogError("Entering userCollectDelet()");
            if (!dbCheck.findIdExsitTwoParam<int>("usersId", "podcastRSSId", "user_collect", collect.usersId, collect.podcastRSSId))
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
                obj.LogError("usersId: " + collect.usersId);
                obj.LogError("podcastRSSId: " + collect.podcastRSSId);
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    obj.LogError("Db Connection Opened");
                    MySqlCommand cmd = new MySqlCommand(
                        $"DELETE FROM user_collect WHERE usersId = {collect.usersId} and podcastRssId = {collect.podcastRSSId}", conn);
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
                obj.LogError($"Error in userCollectDelet():{ex.Message}");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.internalError,
                    Msg = "Internal Error"
                });
            }
        }

        [HttpPost]
        [Route("toListen")]
        public IActionResult userToListen([FromForm] UserRSSModel toListen)  //使用者待放加入
        {
            obj.LogError("Entering userToListen()");
            if (!(dbCheck.findIdExsit<int>(
                "usersId", "users", toListen.usersId) && dbCheck.findIdExsit<int>("podcastRSSId", "podcastRSS", toListen.podcastRSSId)))
            {
                obj.LogError("user or podcastRSS doesn't exist");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.notFound,
                    Msg = "user or podcastRSS doesn't exist"
                });
            }
            else if (dbCheck.findIdExsitTwoParam<int>("usersId", "podcastRSSId", "user_toListen", toListen.usersId, toListen.podcastRSSId))
            {
                obj.LogError("This record already exist");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.alreadyExist,
                    Msg = "This record already exist"
                });
            }
            try
            {
                obj.LogError("usersId: " + toListen.usersId);
                obj.LogError("podcastRSSId: " + toListen.podcastRSSId);
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    obj.LogError("Db Connection Opened");
                    MySqlCommand cmd = new MySqlCommand(
                        $"INSERT INTO user_toListen (usersId, podcastRSSId) VALUES ({toListen.usersId}, {toListen.podcastRSSId})", conn);
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
                obj.LogError($"Error in userToListen():{ex.Message}");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.internalError,
                    Msg = "Internal Error"
                });
            }
        }

        [HttpDelete]
        [Route("toListen")]
        public IActionResult userToListenDelet([FromForm] UserRSSModel toListen)  //使用者取消待放
        {
            obj.LogError("Entering userCollectDelet()");
            if (!dbCheck.findIdExsitTwoParam<int>("usersId", "podcastRSSId", "user_toListen", toListen.usersId, toListen.podcastRSSId))
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
                obj.LogError("usersId: " + toListen.usersId);
                obj.LogError("podcastRSSId: " + toListen.podcastRSSId);
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    obj.LogError("Db Connection Opened");
                    MySqlCommand cmd = new MySqlCommand(
                        $"DELETE FROM user_toListen WHERE usersId = {toListen.usersId} and podcastRssId = {toListen.podcastRSSId}", conn);
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
                obj.LogError($"Error in userCollectDelet():{ex.Message}");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.internalError,
                    Msg = "Internal Error"
                });
            }
        }

        [HttpPost]
        [Route("listen")]
        public IActionResult listenRecord([FromForm] ListenRecordModel record)  //使用者收聽時間紀錄
        {
            obj.LogError("Entering listenRecord()");
            if (!(dbCheck.findIdExsit<int>("usersId", "users", record.usersId) &&
                dbCheck.findIdExsit<int>("podcastRSSId", "podcastRSS", record.podcastRSSId)))
            {
                obj.LogError("user or podcastRSS doesn't exist");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.notFound,
                    Msg = "user or podcastRSS doesn't exist"
                });
            }
            try
            {
                obj.LogError("usersId: " + record.usersId);
                obj.LogError("podcastRSSId: " + record.podcastRSSId);
                obj.LogError("timestamp: " + record.timestamp);
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    obj.LogError("Db Connection Opened");
                    MySqlCommand cmd = new MySqlCommand(
                        $"REPLACE INTO user_record (usersId, podcastRSSId, timestamp, uploadtime)" +
                        $" VALUES ({record.usersId}, {record.podcastRSSId}, {record.timestamp}, '{DateTime.Now:yyyy-MM-dd hh:mm:ss}')", conn);
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
                obj.LogError($"Error in listenRecord():{ex.Message}");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.internalError,
                    Msg = "Internal Error"
                });
            }
        }

        [HttpPost]
        [Route("comment")]
        public IActionResult userCommentt([FromForm] CommentModel comment)  //單集留言
        {
            if (comment.content == null
                || comment.content.ToString().Length > 255)
            {
                return BadRequest(new JsonModel<string>()
                {
                    errorcode = errorcode.formatError,
                    Msg = "format error"
                });
            }
            obj.LogError("Entering userCommentt()");
            if (!dbCheck.findIdExsit<int>("podcastRSSId", "podcastRSS", comment.podcastRSSId))
            {
                obj.LogError("podcastRSSId doesn't exist");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.notFound,
                    Msg = "podcastRSSId doesn't exist"
                });
            }
            try
            {
                obj.LogError("podcastRSSId: " + comment.podcastRSSId);
                obj.LogError("content: " + comment.content);
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    obj.LogError("Db Connection Opened");
                    MySqlCommand cmd = new MySqlCommand(
                        $"INSERT INTO comments (podcastRSSId, content) VALUES ({comment.podcastRSSId}, '{comment.content}')", conn);
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
                obj.LogError($"Error in userCommentt():{ex.Message}");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.internalError,
                    Msg = "Internal Error"
                });
            }
        }

        [HttpGet]
        [Route("comment")]
        public IActionResult getComment([FromQuery] int podcastRSSId)    //取得單集留言
        {
            obj.LogError("Entering getComment");
            if (!dbCheck.findIdExsit<int>("podcastRSSId", "podcastRSS", podcastRSSId))
            {
                obj.LogError("podcastRSS doesn't exist");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.notFound,
                    Msg = "podcastRSS doesn't exist"
                });
            }
            List<CommentContentModel> objList = new List<CommentContentModel>();
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    obj.LogError("Db Connection Opened");
                    MySqlCommand cmd = new MySqlCommand($"SELECT * FROM comments WHERE podcastRSSId = {podcastRSSId};", conn);
                    cmd.CommandType = System.Data.CommandType.Text;
                    using (var dr = cmd.ExecuteReader())
                    {
                        obj.LogError("Data Read...");
                        while (dr.Read())
                        {
                            objList.Add(new CommentContentModel()
                            {
                                commentsId = Convert.ToInt32(dr["commentsId"]),
                                content = dr["content"].ToString(),
                                likeCount = Convert.ToInt32(dr["likecount"])
                            });
                            obj.LogError("Data Read Completed");
                        }
                    }

                    conn.Clone();
                    return Ok(new JsonModel<List<CommentContentModel>>()
                    {
                        errorcode = errorcode.successed,
                        Msg = objList
                    });
                }
            }
            catch (Exception ex)
            {
                obj.LogError($"Error in getComment():{ex.Message}");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.internalError,
                    Msg = "Internal Error"
                });
            }
        }

        [HttpPost]
        [Route("like")]
        public IActionResult commentLike([FromForm] CommentUserModel comment)  //留言喜歡
        {
            obj.LogError("Entering commentLike()");
            if (!dbCheck.findIdExsit<int>("commentsId", "comments", comment.commentsId) ||
                !dbCheck.findIdExsit<int>("usersId", "users", comment.usersId))
            {
                obj.LogError("commentId or usersId doesn't exist");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.notFound,
                    Msg = "commentId or usersId doesn't exist"
                });
            }
            else if (dbCheck.findIdExsitTwoParam<int>("usersId", "commentsId", "user_like_comment", comment.usersId, comment.commentsId))
            {
                obj.LogError("This record already exist");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.alreadyExist,
                    Msg = "This record already exist"
                });
            }
            try
            {
                obj.LogError("commentId: " + comment.commentsId);
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    obj.LogError("Db Connection Opened");
                    MySqlCommand cmd = new MySqlCommand(
                        $"UPDATE  comments set likecount = likecount + 1 WHERE commentsId = {comment.commentsId};" +
                        $"INSERT INTO user_like_comment (usersId, commentsId) VALUES ({comment.usersId}, {comment.commentsId});", conn);
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
                obj.LogError($"Error in commentLike():{ex.Message}");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.internalError,
                    Msg = "Internal Error"
                });
            }
        }

        [HttpDelete]
        [Route("like")]
        public IActionResult undoCommentLike([FromForm] CommentUserModel comment)  //留言取消喜歡
        {
            obj.LogError("Entering userCollectDelet()");
            if (!dbCheck.findIdExsitTwoParam<int>("usersId", "commentsId", "user_like_comment", comment.usersId, comment.commentsId))
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
                obj.LogError("commentsId: " + comment.commentsId);
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    obj.LogError("Db Connection Opened");
                    MySqlCommand cmd = new MySqlCommand(
                        $"DELETE FROM user_like_comment WHERE usersId = {comment.usersId} and commentsId = {comment.commentsId};" +
                        $"UPDATE  comments set likecount = likecount - 1 WHERE commentsId = {comment.commentsId};", conn);
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
                obj.LogError($"Error in userCollectDelet():{ex.Message}");
                return NotFound(new JsonModel<string>()
                {
                    errorcode = errorcode.internalError,
                    Msg = "Internal Error"
                });
            }
        }
    }
}