using Database.Model;
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
using System.Web;

namespace JWTAuthentication.NET6._0.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly AppDataContext _db;
        private DraftEngine_v2 _draftEngine;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private UtilityService.Utility _utility;

        public AuthenticateController(
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

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
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

                    #region // Set User Info & Initialize Data //
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
                    #endregion

                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo
                    });
                }
                else
                {
                    return Unauthorized(new Response { Status = "FAILED", Message = "Username or password is incorrect..." });
                }

                //var service = new AuthService.AuthSvc(_userManager, _roleManager, _configuration, _db, _draftEngine);

                //var result = await Task.Run(() => service.Login(model));

                //return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] CodeRequestModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var code = _utility.GenerateRandomNumber(1000000, 9999999);

                    // create update MyDraftUser ResetCode record with code
                    var myDraftUser = _db.MyDraftUser.Where(x => x.UserUniqueID == user.Id).FirstOrDefault();
                    if (myDraftUser != null)
                    {
                        myDraftUser.ResetCode = code;
                        _db.SaveChanges();
                    }

                    StringBuilder welcomeHTML = new StringBuilder(Email.Temaplate.ResetCodeHTML.GetHTML(code.ToString()));

                    var service = new EmailService.EmailSvs(_configuration);
                    service.SendEmail(user.NormalizedEmail, user.NormalizedUserName, "MyDraft Password Reset Code", welcomeHTML.ToString());

                    return Ok(new Response { Status = code.ToString(), Message = "Code for Password Reset sent to email!" });
                }
                else
                {
                    return Unauthorized(new Response { Status = "FAILED", Message = "Email not Found..." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("Reset-Password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPssWrdModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.ForgotEmail);
                if (user != null)
                {
                    var myDraftUser = _db.MyDraftUser
                                    .Include(x => x.UserLeagues)
                                    .Where(x => x.UserUniqueID == user.Id).FirstOrDefault();
                    if (myDraftUser != null)
                    {
                        if (myDraftUser.ResetCode == model.CodeFromEmail)
                        {
                            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                            if (result.Succeeded)
                            {
                                StringBuilder resetConfrimHTML = new StringBuilder(Email.Temaplate.PasswordWasResetHTML.GetHTML(user.UserName));

                                var service = new EmailService.EmailSvs(_configuration);
                                service.SendEmail(user.NormalizedEmail, user.NormalizedUserName, "MyDraft Password Reset", resetConfrimHTML.ToString());

                                #region // Set User Info & Initialize Data //
                                //var myDraftUser = _db.MyDraftUser
                                //                .Include(x => x.UserLeagues)
                                //                .Where(x => x.UserUniqueID == user.Id).FirstOrDefault();
                                //if (myDraftUser != null)
                                //{
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
                                //}
                                #endregion

                                return Ok(new Response { Status = "SUCCESS", Message = "Successful Password Reset!" });
                            }
                            else
                            {
                                return Unauthorized(new Response { Status = "FAILED", Message = "Password Reset Failed..." });
                            }
                        }
                        else
                        {
                            return Unauthorized(new Response { Status = "FAILED", Message = "Code is incorrect..." });
                        }
                    }
                    else
                    {
                        return Unauthorized(new Response { Status = "FAILED", Message = "Code is incorrect..." });
                    }   
                }
                else
                {
                    return Unauthorized(new Response { Status = "FAILED", Message = "Email not Found..." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                var userExists = await _userManager.FindByNameAsync(model.Username);
                if (userExists != null)
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "FAILED", Message = "User already exists!" });

                IdentityUser user = new()
                {
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = model.Username
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "FAILED", Message = "User creation failed! Please check user details and try again." });

                #region // Send Email Confirmation
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var encodedToken = HttpUtility.UrlEncode(token);

                var emailConfirmationHTML = new StringBuilder(Email.Temaplate.EmailConfirmationHTML.GetHTML(model.Username, encodedToken, model.Email));

                var service = new EmailService.EmailSvs(_configuration);
                service.SendEmail(user.NormalizedEmail, user.NormalizedUserName, "MyDraft Email Confirmation", emailConfirmationHTML.ToString());
                #endregion

                #region // Add User to MyDraftUser Table
                //var newUser = await _userManager.FindByNameAsync(model.Username);
                //MyDraftUser myDraftUser = new()
                //{
                //    UserUniqueID = newUser.Id,
                //    UserName = newUser.NormalizedUserName,
                //    UserEmail = newUser.NormalizedEmail,
                //};
                //_db.MyDraftUser.Add(myDraftUser);
                //_db.SaveChanges();
                //int myDraftUserID = myDraftUser.ID;

                //_draftEngine.InitializeLeagueData_v2(myDraftUser.ID);
                #endregion

                #region // Send Email
                //StringBuilder welcomeHTML = new StringBuilder(Email.Temaplate.WelcomeHTML.GetHTML(model.Username));

                //var service = new EmailService.EmailSvs(_configuration);
                //service.SendEmail(newUser.NormalizedEmail, newUser.NormalizedUserName, "Welcome to MyDraft!", welcomeHTML.ToString());
                #endregion

                return Ok(new Response { Status = "SUCCESS", Message = "Please, click the link in confrimation email sent to registration email to proceed with login!" });
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error in Register method.");
                return BadRequest(ex.Message);
            }

        }

        [HttpPost]
        [Route("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.email);
                if (user == null)
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "FAILED", Message = "User not found!" });

                var result = await _userManager.ConfirmEmailAsync(user, model.token);
                if (result.Succeeded)
                {
                    return Ok(new Response { Status = "SUCCESS", Message = "Email Confirmed!" });
                }
                else
                {
                    return Unauthorized(new Response { Status = "FAILED", Message = "Email Confirmation Failed..." });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            IdentityUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Admin);
            }
            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.User);
            }
            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
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
    }
}