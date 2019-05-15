using Myapp.Data;
using Myapp.Data.Providers;
using Myapp.Models;
using Myapp.Models.Domain;
using Myapp.Models.Enums;
using Myapp.Models.Requests;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Myapp.Services
{
    public class UserService : IUserService
    {
        private IAuthenticationService<int> _authenticationService;
        private IDataProvider _dataProvider;

        public UserService(IAuthenticationService<int> authService, IDataProvider dataProvider)
        {
            _authenticationService = authService;
            _dataProvider = dataProvider;
        }

        
        public async Task<bool> LogInAsync(LoginUser model)
        {
            bool loggedIn = false;
            User user = GetByEmail(model.Email);
            bool isValidCredentials = BCrypt.BCryptHelper.CheckPassword(model.Password, user.Password);
            if (isValidCredentials)
            {
                IUserAuthData response = new UserBase
                {
                    Id = user.Id,
         
                    Name = user.Email,
         
                    Roles = user.Roles,
         
                    TenantId = ""
                };
                Claim fullName = new Claim("CustomClaim", "Sabio Bootcamp");
                await _authenticationService.LogInAsyncFacebook(response, new Claim[] { fullName });
                loggedIn = isValidCredentials;
            }
            return loggedIn;
        }

        public async Task<bool> LogInAsyncFacebook(FacebookUser model)
        {
            bool loggedIn = false;
            User user = GetByEmail(model.Email);

            if (user != null)
            {
                IUserAuthData response = new FacebookUserBase
                {
                    Id = user.Id
         ,
                    Name = user.Email
         ,
                    Roles = user.Roles
         ,
                    TenantId = "merchant"
                };
                Claim fullName = new Claim("CustomClaim", "Sabio Bootcamp");
                await _authenticationService.LogInAsyncFacebook(response, new Claim[] { fullName });
                loggedIn = true;
            }
            else
            {
                FacebookAddRequest modelAdd = new FacebookAddRequest();

                modelAdd.Email = model.Email;
                modelAdd.AccessToken = model.AccessToken;

                int userId = FacebookInsert(modelAdd);
                model.UserId = userId;
                loggedIn = false;
            }

            return loggedIn;
        }

        public User GetByEmail(string email)
        {
            User user = null;
            List<string> roles = null;
            string role = null;

            _dataProvider.ExecuteCmd(
                "dbo.Users_SelectByEmail_V2",
                (parameters) =>
                {
                    parameters.AddWithValue("@Email", email);
                },
                singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    switch (set)
                    {
                        case 0:
                            user = Mapper(reader);
                            break;

                        case 1:
                            int startingindex = 0;
                            role = reader.GetSafeString(startingindex++);
                            if (roles == null)
                            {
                                roles = new List<string>();
                            }
                            roles.Add(role);
                            break;
                    }
                }
                );
            if (user != null)
            {
                user.Roles = roles;
            }
            return user;
        }

        public User GetByEmailv2(string email)
        {
            User model = null;
            _dataProvider.ExecuteCmd(
                "dbo.Users_SelectByEmail",
                inputParamMapper: delegate (SqlParameterCollection paramCol)
                 {
                     paramCol.AddWithValue("@Email", email);
                 },
                singleRecordMapper: delegate (IDataReader reader, short set)
                 {
                     model = Mapper(reader);
                 }
                );
            return model;
        }

        public void UpdateConfirmStatus(int userId)
        {
            _dataProvider.ExecuteNonQuery(
                "dbo.Users_UpdateConfirmStatus",
                inputParamMapper: delegate (SqlParameterCollection paramCol)
                {
                    paramCol.AddWithValue("@Id", userId);
                });
        }

        public List<User> SelectAll()
        {
            List<User> allUsers = new List<User>();

            _dataProvider.ExecuteCmd(
                "dbo.Users_SelectAll",
                 (parameters) =>
                 {
                 },
                 (reader, recordSetIndex) =>
                 {
                     User singleUser = new User();
                     int startingIndex = 0;

                     singleUser.Id = reader.GetSafeInt32(startingIndex++);
                     singleUser.Email = reader.GetSafeString(startingIndex++);
                     singleUser.Password = reader.GetSafeString(startingIndex++);
                     singleUser.IsConfirmed = reader.GetSafeBool(startingIndex++);
                     singleUser.ReferralCode = reader.GetSafeGuid(startingIndex++);
                     singleUser.DateCreated = reader.GetSafeDateTime(startingIndex++);
                     singleUser.DateModified = reader.GetSafeDateTime(startingIndex++);

                     allUsers.Add(singleUser);
                 });
            return allUsers;
        }

        public User GetById(int id)
        {
            User model = null;
            List<string> roles = null;
            _dataProvider.ExecuteCmd(
                "dbo.Users_SelectById_V2",
                (parameter) =>
                {
                    parameter.AddWithValue("@Id", id);
                },
                singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    switch (set)
                    {
                        case 0:
                            model = Mapper(reader);
                            break;

                        case 1:
                            int startingIndex = 0;
                            string role = reader.GetSafeString(startingIndex++);
                            if (roles == null)
                            {
                                roles = new List<string>();
                            }
                            roles.Add(role);
                            break;
                    }
                });

            if (model != null)
            {
                model.Roles = roles;
            }
            return model;
        }

        public int Insert(UserInsertRequest model)
        {
            int id = 0;
            string hashedPassword = BCrypt.BCryptHelper.HashPassword(model.Password, BCrypt.BCryptHelper.GenerateSalt(12));
            _dataProvider.ExecuteNonQuery(
                "dbo.Users_Insert",
                inputParamMapper: delegate (SqlParameterCollection paramCol)
                {
                    SqlParameter param = new SqlParameter
                    {
                        ParameterName = "@Id",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Output
                    };

                    paramCol.Add(param);
                    paramCol.AddWithValue("@Email", model.Email);
                    paramCol.AddWithValue("@Password", hashedPassword);
                    paramCol.AddWithValue("@IsConfirmed", model.IsConfirmed);
                },
                returnParameters: delegate (SqlParameterCollection paramCol)
                {
                    id = (int)paramCol["@Id"].Value;
                });
            return id;
        }

        public int FacebookInsert(FacebookAddRequest model)
        {
            int id = 0;

            _dataProvider.ExecuteNonQuery(
                                "dbo.Users_Insert",
                inputParamMapper: delegate (SqlParameterCollection paramCol)
                {
                    SqlParameter param = new SqlParameter
                    {
                        ParameterName = "@Id",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Output
                    };

                    paramCol.Add(param);
                    paramCol.AddWithValue("@Email", model.Email);
                    paramCol.AddWithValue("@Password", DBNull.Value);
                    paramCol.AddWithValue("@IsConfirmed", 1);
                },
                returnParameters: delegate (SqlParameterCollection paramCol)
                {
                    id = (int)paramCol["@Id"].Value;
                });
            return id;
        }

        public void Update(UserUpdateRequest model)
        {
            string hashedPassword = BCrypt.BCryptHelper.HashPassword(model.Password, BCrypt.BCryptHelper.GenerateSalt(12));
            _dataProvider.ExecuteNonQuery(
                "dbo.Users_Update",
                inputParamMapper: delegate (SqlParameterCollection paramCol)
                {
                    paramCol.AddWithValue("@Id", model.Id);
                    paramCol.AddWithValue("@Email", model.Email);
                    paramCol.AddWithValue("@Password", hashedPassword);
                    paramCol.AddWithValue("@IsConfirmed", model.IsConfirmed);
                });
        }

        public void UpdatePassword(int id, string password)
        {
            string hashedPassword = BCrypt.BCryptHelper.HashPassword(password, BCrypt.BCryptHelper.GenerateSalt(12));
            _dataProvider.ExecuteNonQuery(
                "dbo.Users_UpdatePassword",
                inputParamMapper: delegate (SqlParameterCollection paramCol)
                {
                    paramCol.AddWithValue("@Id", id);
                    paramCol.AddWithValue("@Password", hashedPassword);
                });
        }

        public void Delete(int id)
        {
            _dataProvider.ExecuteNonQuery(
                "dbo.Users_Delete",
                (parameters) =>
                {
                    parameters.AddWithValue("@Id", id);
                });
        }

        public void AddRole(int userId, int userRoleType)
        {
            _dataProvider.ExecuteNonQuery(
                "dbo.UsersRoles_Insert",
                (paramCol) =>
                {
                    paramCol.AddWithValue("@UserId", userId);
                    paramCol.AddWithValue("@UserRoleType", userRoleType);
                },
                null
                );
        }

        public void ChangeStatus(int id, UserRoleUpdateRequest req)
        {
            _dataProvider.ExecuteNonQuery(
                "dbo.UsersRoles_ChangeStatus",
                (paramCol) =>
                {
                    paramCol.AddWithValue("@UserId", id);
                    paramCol.AddWithValue("@IsActive", req.IsActive);
                },
                null
                );
        }

        private User Mapper(IDataReader reader)
        {
            User user = new User();
            int startingIndex = 0;

            user.Id = reader.GetSafeInt32(startingIndex++);
            user.Email = reader.GetSafeString(startingIndex++);
            user.Password = reader.GetSafeString(startingIndex++);
            user.IsConfirmed = reader.GetSafeBool(startingIndex++);
            user.ReferralCode = reader.GetSafeGuid(startingIndex++);
            user.DateCreated = reader.GetSafeDateTime(startingIndex++);
            user.DateModified = reader.GetSafeDateTime(startingIndex++);

            return user;
        }

        #region user tokens

        public Guid AddToken(int userId)
        {
            Guid generatedToken = Guid.NewGuid();
            _dataProvider.ExecuteNonQuery("dbo.UserTokens_Insert", (parameters) =>
            {
                parameters.AddWithValue("@UserId", userId);
                parameters.AddWithValue("@Token", generatedToken);
                parameters.AddWithValue("@TokenTypeId", (int)TokenType.Register);
            });

            return generatedToken;
        }

        public Guid AddLoginToken(int userId)
        {
            Guid generatedToken = Guid.NewGuid();

            _dataProvider.ExecuteNonQuery("dbo.UserTokens_Delete", (parameters) =>
            {
                parameters.AddWithValue("@UserId", userId);
                parameters.AddWithValue("@TokenTypeId", (int)TokenType.Login);
            });

            _dataProvider.ExecuteNonQuery("dbo.UserTokens_Insert", (parameters) =>
            {
                parameters.AddWithValue("@UserId", userId);
                parameters.AddWithValue("@Token", generatedToken);
                parameters.AddWithValue("@TokenTypeId", (int)TokenType.Login);
            });

            return generatedToken;
        }

        public Guid AddChangePasswordToken(int userId)
        {
            Guid generatedToken = Guid.NewGuid();
            _dataProvider.ExecuteNonQuery("dbo.UserTokens_Insert", (parameters) =>
            {
                parameters.AddWithValue("@UserId", userId);
                parameters.AddWithValue("@Token", generatedToken);
                parameters.AddWithValue("@TokenTypeId", (int)TokenType.ChangePassword);
            });

            return generatedToken;
        }

        public void DeleteChangePasswordToken(int userId)
        {
            _dataProvider.ExecuteNonQuery("dbo.UserTokens_Delete", (parameters) =>
            {
                parameters.AddWithValue("@UserId", userId);
                parameters.AddWithValue("@TokenTypeId", (int)TokenType.ChangePassword);
            });
        }

        public void DeleteLoginToken(int userId)
        {
            _dataProvider.ExecuteNonQuery("dbo.UserTokens_Delete", (parameters) =>
            {
                parameters.AddWithValue("@UserId", userId);
                parameters.AddWithValue("@TokenTypeId", (int)TokenType.Login);
            });
        }

        public void AddFacebookToken(int userId)
        {
            Guid generatedToken = Guid.NewGuid();
            _dataProvider.ExecuteNonQuery("dbo.UserTokens_Insert", (parameters) =>
            {
                parameters.AddWithValue("@UserId", userId);
                parameters.AddWithValue("@Token", generatedToken);
                parameters.AddWithValue("@TokenTypeId", (int)TokenType.Facebook);
            });
        }

        public int GetUserIdByToken(Guid token)
        {
            int userId = 0;
            _dataProvider.ExecuteNonQuery("dbo.UserTokens_SelectUserIdByToken", (para) =>
            {
                para.AddWithValue("@Token", token);
                //Output parameter
                SqlParameter userIdParameter = new SqlParameter("@UserId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                para.Add(userIdParameter);
            },
            (returnParams) =>
            {
                Int32.TryParse(returnParams["@UserId"].Value.ToString(), out userId);
            }
            );

            return userId;
        }

        public object SelectByUserIdTokenTypePassword(int userId)
        {
            object userObject = new object();

            _dataProvider.ExecuteCmd(
                "dbo.UserTokens_SelectByUserIdTokenType", (parameters) =>
                {
                    parameters.AddWithValue("@UserId", userId);
                    parameters.AddWithValue("@TokenTypeId", (int)TokenType.ChangePassword);
                },
                (reader, recordSetIndex) =>
                {
                    int index = 0;
                    userObject = new
                    {
                        UserId = reader.GetSafeInt32(index++),
                        Token = reader.GetSafeGuid(index++),
                        TokenTypeId = reader.GetSafeInt32(index++),
                        DateCreated = reader.GetSafeDateTime(index++)
                    };

                });
            return userObject;
        }

        #endregion user tokens
    }
}
