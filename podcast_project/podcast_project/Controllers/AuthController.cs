using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using podcast_project.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace podcast_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private DBInfoCheck dbCheck;
        private cls_Logger obj;

        public AuthController(IConfiguration iConfig)
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


        public string Authenticate(string IDFA)
        {
            // STEP1: 建立使用者的 Claims 聲明，這會是 JWT Payload 的一部分
            var userClaims = new ClaimsIdentity(new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, IDFA),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    });
            // STEP2: 取得對稱式加密 JWT Signature 的金鑰
            // 這部分是選用，但此範例在 Startup.cs 中有設定 ValidateIssuerSigningKey = true 所以這裡必填
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["AppSettings:Secret"]));
            // STEP3: 建立 JWT TokenHandler 以及用於描述 JWT 的 TokenDescriptor
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = configuration["AppSettings:Issuer"],
                Audience = configuration["AppSettings:Issuer"],
                Subject = userClaims,
                //Expires = DateTime.Now.AddMinutes(300),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
            };
            // 產出所需要的 JWT Token 物件
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            // 產出序列化的 JWT Token 字串
            var serializeToken = tokenHandler.WriteToken(securityToken);

            return serializeToken;
        }

        [HttpPost, Route("login")]
        public IActionResult Login([FromForm] IDFAdModel user)
        {
            List<UserIdWithTokenModel> objList = new List<UserIdWithTokenModel>();
            obj.LogError("Entering Login()");
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

                string token = Authenticate(user.IDFA);

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
                        objList.Add(new UserIdWithTokenModel()
                        {
                            usersId = Convert.ToInt32(dr["usersId"]),
                            token = token
                        });
                        obj.LogError("Data Read Completed");

                    }
                    conn.Close();
                    return Ok(new JsonModel<List<UserIdWithTokenModel>>()
                    {
                        errorcode = errorcode.successed,
                        Msg = objList
                    }); ;
                }
            }
            catch (Exception ex)
            {
                obj.LogError($"Error in Login():{ex.Message}");
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
    }
}
