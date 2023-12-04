using DbData;
using JWTAuthentication.NET6._0.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyDraftAPI_v2;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService
{
    public class AuthSvc : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        private readonly AppDataContext _db;
        private DraftEngine_v2 _draftEngine;
        private UtilityService.Utility _utility;

        public AuthSvc(
                       UserManager<IdentityUser> userManager,
                                  RoleManager<IdentityRole> roleManager,
                                             IConfiguration configuration,
                                                        AppDataContext db,
                                                                   DraftEngine_v2 draftEngine)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _db = db;
            _draftEngine = draftEngine;
            _utility = new UtilityService.Utility(_db, _configuration);
        }

        /// <summary>
        ///              User Login
        /// </summary>
        /// <param name="vVariable"></param>
        /// <returns></returns>
        public async Task<IActionResult> Login(LoginModel model)
        {
            var result = new DataModel.Response.ReturnResult();
            try
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    var userRoles = await _userManager.GetRolesAsync(user);

                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };

                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }

                    var token = GetToken(authClaims);

                    var myDraftUser = _db.MyDraftUser
                                    .Include(x => x.UserLeagues)
                                    .Where(x => x.UserUniqueID == user.Id).FirstOrDefault();
                    if (myDraftUser != null)
                    {
                        _draftEngine.MyDraftUser = new ViewModel.UserInfo()
                        {
                            UserName = myDraftUser.UserName,
                            UserEmail = myDraftUser.UserEmail,
                            IsLoggedIn = true,
                            UserLeagues = new List<ViewModel.UserLeagueItem>()
                        };

                        foreach (var userLeague in myDraftUser.UserLeagues)
                        {
                            _draftEngine.MyDraftUser.UserLeagues.Add(new ViewModel.UserLeagueItem()
                            {
                                Value = userLeague.ID,
                                Label = userLeague.Name
                            });
                        }

                        _draftEngine.InitializeLeagueData_v2(myDraftUser.ID);
                    }

                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo
                    });
                   
                }
                else
                {
                    return Unauthorized(new Response { Status = "Failed", Message = "Username or password is incorrect..." });
                    //return _utility.Unauthorized();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        #region // Method Template //
        /// <summary>
        ///              TEMPLATE FOR NEW METHODS
        /// </summary>
        /// <param name="vVariable"></param>
        /// <returns></returns>
        public DataModel.Response.ReturnResult TemplateMethod(int vVariable)
        {
            var result = new DataModel.Response.ReturnResult();
            try
            {
                // Code Here

                return _utility.SuccessResult(new { EMFTest = new { Success = true } });
            }
            catch (Exception ex)
            {
                return _utility.ExceptionReturnResult(ex);
            }
        }
        #endregion
    }
}
