using AspNetCoreHero.ToastNotification.Abstractions;
using MyPhamSoul.ModelViews;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyPhamSoul.Extension;
using MyPhamSoul.Helpper;
using MyPhamSoul.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;


namespace MyPhamSoul.Controllers
{

    public class AccountsController : Controller
    {
        private readonly _2025MyPhamContext _context;
        public INotyfService _notifyService { get; }

        public AccountsController(_2025MyPhamContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidatePhone(string SoDienThoai)
        {
            try
            {
                var khachhang = _context.Accounts.AsNoTracking().SingleOrDefault(x => x.Phone.ToLower() == SoDienThoai.ToLower());
                if (khachhang != null)
                    return Json(data: "Số điện thoại : " + SoDienThoai + " Đã được sử dụng");

                return Json(data: true);
            }
            catch
            {
                return Json(data: true);
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidateEmail(string Email)
        {
            try
            {
                var khachhang = _context.Accounts.AsNoTracking().SingleOrDefault(x => x.AccountEmail.ToLower() == Email.ToLower());
                if (khachhang != null)
                    return Json(data: "Email : " + Email + " Đã được sử dụng");

                return Json(data: true);
            }
            catch
            {

                return Json(data: true);
            }
        }
        [Route("tai-khoan-cua-toi.html", Name = "Dashboard")]
        public IActionResult Dashboard()
        {
            var taikhoanID = HttpContext.Session.GetString("AccountId");
            if (taikhoanID != null)
            {
                var khachhang = _context.Accounts.AsNoTracking().SingleOrDefault(x => x.AccountId == Convert.ToInt32(taikhoanID));
                if (khachhang != null)
                {
                    return View(khachhang);
                }
            }
            return RedirectToAction("Login");
        }



        [HttpGet]
        [Route("edit-account")]
        public IActionResult EditAccount()
        {
            var taikhoanID = HttpContext.Session.GetString("AccountId");

            if (taikhoanID != null)
            {
                var khachhang = _context.Accounts.AsNoTracking().SingleOrDefault(x => x.AccountId == Convert.ToInt32(taikhoanID));

                if (khachhang != null)
                {
                    return View("Dashboard", khachhang);
                }
            }

            // Nếu không tìm thấy tài khoản hoặc có lỗi, chuyển hướng đến trang đăng nhập
            return RedirectToAction("Login");
        }

        [HttpPost]
        [Route("edit-account")]
        public IActionResult EditAccount(Account account)
        {
            try
            {
                var taikhoanID = HttpContext.Session.GetString("AccountId");

                if (taikhoanID == null)
                {
                    return RedirectToAction("Login", "Accounts");
                }

                if (ModelState.IsValid)
                {
                    var existingAccount = _context.Accounts.Find(Convert.ToInt32(taikhoanID));

                    if (existingAccount == null)
                    {
                        return RedirectToAction("Login", "Accounts");
                    }

                    // Cập nhật thông tin tài khoản từ dữ liệu nhập vào
                    existingAccount.AccountEmail = account.AccountEmail;
                    existingAccount.FullName = account.FullName;
                    existingAccount.DiaChi = account.DiaChi;
                    existingAccount.GioiTinh = account.GioiTinh;
                    existingAccount.Phone = account.Phone;
                    existingAccount.NgaySinh = account.NgaySinh;


                    // Lưu thay đổi vào cơ sở dữ liệu
                    _context.Update(existingAccount);
                    _context.SaveChanges();

                    // Truyền thông điệp thành công để hiển thị trên view
                    ViewBag.SuccessMessage = "Cập nhật thông tin tài khoản thành công!";
                    return RedirectToAction("Dashboard", "Accounts");
                }
            }
            catch
            {
                // Truyền thông điệp lỗi để hiển thị trên view
                ViewBag.ErrorMessage = "Đã xảy ra lỗi khi cập nhật thông tin tài khoản";
            }

            // Nếu có lỗi, chuyển hướng đến trang khác theo yêu cầu
            return RedirectToAction("Dashboard", "Accounts");
        }






        private int GetRoleIdForCustomer()
        {
            // Thực hiện logic để lấy RoleId tương ứng với vai trò 'Customer' từ cơ sở dữ liệu
            // Ví dụ:
            return _context.Roles.Where(r => r.RoleName == "Customer").Select(r => r.RoleId).FirstOrDefault();
        }
        private int GetRoleIdForAdmin()
        {
            // Thực hiện logic để lấy RoleId tương ứng với vai trò 'Customer' từ cơ sở dữ liệu
            // Ví dụ:
            return _context.Roles.Where(r => r.RoleName == "Admin").Select(r => r.RoleId).FirstOrDefault();
        }
        private int GetRoleIdForEmployee()
        {
            // Thực hiện logic để lấy RoleId tương ứng với vai trò 'Customer' từ cơ sở dữ liệu
            // Ví dụ:
            return _context.Roles.Where(r => r.RoleName == "Employee").Select(r => r.RoleId).FirstOrDefault();
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("dang-ky.html", Name = "Dangky")]
        public IActionResult DangKyTaiKhoan()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("dang-ky.html", Name = "Dangky")]
        public async Task<IActionResult> DangKyTaiKhoan(RegisterVM taikhoan)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string salt = Utilities.GetRandomKey();
                    Account khachhang = new Account
                    {
                        FullName = taikhoan.TenKhachHang,
                        Phone = taikhoan.SoDienThoai.Trim().ToLower(),
                        AccountEmail = taikhoan.Email.Trim().ToLower(),
                        AccountPassword = taikhoan.Password,
                        Active = true,
                        Salt = salt,
                        RoleId = GetRoleIdForCustomer(),
                        NgaySinh = taikhoan.NgaySinh,
                        GioiTinh = taikhoan.GioiTinh,
                        CreateDate = DateTime.Now
                        
                    };
                    try
                    {
                        _context.Add(khachhang);
                        await _context.SaveChangesAsync();
                        HttpContext.Session.SetString("AccountId", khachhang.AccountId.ToString());
                        var taikhoanID = HttpContext.Session.GetString("AccountId");

                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, khachhang.FullName),
                            new Claim("AccountId", khachhang.AccountId.ToString())
                        };
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        await HttpContext.SignInAsync(claimsPrincipal);
                        _notifyService.Success("Đăng ký tài khoản thành công!");
                        return RedirectToAction("Index", "Home");
                    }
                    catch (Exception ex)
                    {
                        return RedirectToAction("DangKyTaiKhoan", "Accounts");
                    }
                }
                else
                {
                    return View(taikhoan);
                }
            }
            catch
            {
                return View(taikhoan);
            }
        }
        [AllowAnonymous]
        [Route("dang-nhap.html", Name = "Dangnhap")]
        public IActionResult Login(string? returnUrl = null)
        {
            var taikhoanID = HttpContext.Session.GetString("AccountId");
            if (taikhoanID != null)
            {
                return RedirectToAction("Dashboard", "Accounts");
            }

            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("dang-nhap.html", Name = "Dangnhap")]
        public async Task<IActionResult> Login(LoginViewModel customer, string? returnUrl = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isEmail = Utilities.IsValidEmail(customer.UserName);
                    if (!isEmail) return View(customer);

                    var khachhang = _context.Accounts.AsNoTracking().SingleOrDefault(x => x.AccountEmail.Trim() == customer.UserName);
                    if (khachhang == null) 
                        return RedirectToAction("DangKyTaiKhoan");

                    string pass = customer.Password;
                    if (khachhang.AccountPassword != pass)
                    {
                        _notifyService.Error("Thông tin đăng nhập chưa chính xác.");
                        return View(customer);
                    }
                    if (khachhang.Active == false)
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    HttpContext.Session.SetString("AccountId", khachhang.AccountId.ToString());

                    var taikhoanID = HttpContext.Session.GetString("AccountId");

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, khachhang.FullName),
                        new Claim("AccountId", khachhang.AccountId.ToString()),
           

                    };
                    if (khachhang.RoleId == GetRoleIdForAdmin())
                    {
                        claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                    }
                    if (khachhang.RoleId == GetRoleIdForCustomer())
                    {
                        claims.Add(new Claim(ClaimTypes.Role, "Customer"));
                    }
                    if (khachhang.RoleId == GetRoleIdForEmployee())
                    {
                        claims.Add(new Claim(ClaimTypes.Role, "Employee"));
                    }
                  
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);
                    if (khachhang.RoleId == GetRoleIdForAdmin())
                    {
                        // Chuyển hướng đến trang Admin/Accounts
                        _notifyService.Success("Đăng nhập tài khoản Admin thành công!");
                        return RedirectToAction("Index", "Accounts", new { area = "Admin" });
                    }
                    if (khachhang.RoleId == GetRoleIdForEmployee())
                    {
                        // Chuyển hướng đến trang Admin/Accounts
                        _notifyService.Success("Đăng nhập tài khoản nhân viên thành công!");
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    }
                    _notifyService.Success("Đăng nhập thành công!");
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                return RedirectToAction("DangKyTaiKhoan", "Accounts");
            }
            return View(customer);
        }


        public async Task Login1()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties()
            {
                RedirectUri = Url.Action("GoogleResponse")
            });
        }

        [HttpPost("signin-google")]

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (result.Succeeded)
            {
                var email = result.Principal.FindFirstValue(ClaimTypes.Email);
                var name = result.Principal.FindFirstValue(ClaimTypes.Name);



                // Kiểm tra xem EmailKh đã tồn tại trong bảng NguoiDung chưa
                var existingUser = _context.Accounts.FirstOrDefault(u => u.AccountEmail == email);
                if (existingUser == null)
                {     
                    Account khachhang = new Account
                    {
                        FullName = name,
                        Phone = "0933683420",
                        AccountEmail = email,
                        AccountPassword = "@Abc123",
                        Active = true,
                        Salt = "21^5]",
                        RoleId = 21,
                        NgaySinh = DateTime.Now,
                        GioiTinh = false,
                        CreateDate = DateTime.Now

                    };
                    // Nếu người dùng không tồn tại, tạo mới người dùng và đăng nhập


                    await _context.SaveChangesAsync();
                    HttpContext.Session.SetString("AccountId", khachhang.AccountId.ToString());
                    var taikhoanID = HttpContext.Session.GetString("AccountId");

                    var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, khachhang.FullName),
                            new Claim("AccountId", khachhang.AccountId.ToString())
                        };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);
                   
                    return RedirectToAction("Index", "Home");

                    
                }
                else
                {
                    // Nếu người dùng đã tồn tại, đăng nhập người dùng
                                  
                    if (existingUser.RoleId == GetRoleIdForAdmin())
                    {
                        // Chuyển hướng đến trang Admin/Accounts
                        _notifyService.Success("Đăng nhập tài khoản Admin thành công!");
                        return RedirectToAction("Index", "Accounts", new { area = "Admin" });
                    }
                    if (existingUser.RoleId == GetRoleIdForEmployee())
                    {
                        // Chuyển hướng đến trang Admin/Accounts
                        _notifyService.Success("Đăng nhập tài khoản nhân viên thành công!");
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    }
                    _notifyService.Success("Đăng nhập thành công!");
                    return RedirectToAction("Index", "Home");

                    
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }


        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                var taikhoanID = HttpContext.Session.GetString("AccountId");
                if (taikhoanID == null)
                {
                    return RedirectToAction("Login", "Accounts");
                }
                if (ModelState.IsValid)
                {
                    var taikhoan = _context.Accounts.Find(Convert.ToInt32(taikhoanID));
                    if (taikhoan == null) return RedirectToAction("Login", "Accounts");
                    var pass = model.PasswordNow.Trim();
                    if (pass == taikhoan.AccountPassword)
                    {
                        string passnew = model.Password.Trim();
                        taikhoan.AccountPassword = passnew;
                        _context.Update(taikhoan);
                        _context.SaveChanges();
                        _notifyService.Success("Thay đổi mật khẩu thành công!");
                        return RedirectToAction("Dashboard", "Accounts");
                    }
                    else
                    {
                        _notifyService.Error("Sai mật khẩu");
                        return RedirectToAction("Dashboard", "Accounts");
                    }
                }
            }
            catch
            {
                _notifyService.Success("Thay đổi không mật khẩu thành công");
                return RedirectToAction("Dashboard", "Accounts");
            }
            _notifyService.Error("Mật khẩu mới không giống nhau");
            return RedirectToAction("Dashboard", "Accounts");

        }

        [HttpGet]
        [Route("dang-xuat.html", Name = "Logout")]
        public IActionResult Logout()
        {
            var accountId = HttpContext.Session.GetString("AccountId");

            if (!string.IsNullOrEmpty(accountId))
            {
                var account = _context.Accounts.Find(Convert.ToInt32(accountId));

                if (account != null)
                {
                    // Cập nhật trường LastLogin với thời gian hiện tại
                    account.LastLogin = DateTime.Now;
                    _context.SaveChanges();
                }
            }

            // Đăng xuất người dùng
            HttpContext.SignOutAsync();
            HttpContext.Session.Remove("AccountId");

            return RedirectToAction("Index", "Home");
        }

        private async Task SignInUser(Account user)
        {
            // Lưu thông tin người dùng vào Session
            HttpContext.Session.SetString("AccountEmail", user.AccountEmail);
            HttpContext.Session.SetString("RoleId", user.RoleId.ToString());
            HttpContext.Session.SetString("IsLoggedIn", "true");

            // Đăng nhập người dùng
            var claims = new List<Claim>
    {
        
        new Claim(ClaimTypes.Name, user.AccountEmail),
        new Claim(ClaimTypes.Role, user.RoleId.ToString())
    };
            var userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var userPrincipal = new ClaimsPrincipal(userIdentity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);
        }
    }
}
