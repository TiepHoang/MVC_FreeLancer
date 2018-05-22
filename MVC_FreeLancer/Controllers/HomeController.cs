using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Facebook;

namespace MVC_FreeLancer.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string ID, string Project)
        {
            var obj = new Models.ManagerProjectEntities().Accounts.FirstOrDefault(q => q.Username == ID);
            if (obj == null || obj.State == null)
            {
                ModelState.AddModelError("", "Tài khoản không tồn tại");
            }
            else
            {
                switch (obj.State)
                {
                    case 1:
                        string link_project = $"https://github.com/projectdev1024/{Project}";
                        if (check(link_project))
                        {
                            string project_download = $"{link_project}/archive/master.zip";
                            ModelState.AddModelError("", $"{DateTime.Now} Project đã được tải...");
                            return Redirect(project_download);
                        }
                        else
                        {
                            ModelState.AddModelError("", "Project không tồn tại");
                        }
                        break;
                    case 3:
                        ModelState.AddModelError("", "ID đã bị khóa");
                        break;
                    default:
                        break;
                }
            }
            return View();
        }

        bool check(string Url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        Uri RedirectFB()
        {
            return new UriBuilder(Request.Url)
            {
                Query = null,
                Fragment = null,
                Path = Url.Action("FBCallback"),
            }.Uri;
        }

        public ActionResult FBCallback(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = "252307928840475",
                client_secret = "99c0d215e7d85a38953a69b7ac398005",
                redirect_uri = RedirectFB().AbsoluteUri,
                code = code
            });
            var access_token = result.access_token;
            if (string.IsNullOrWhiteSpace(access_token))
            {
                ModelState.AddModelError("", "access_token IsNullOrWhiteSpace");
            }
            else
            {
                fb.AccessToken = access_token;
                dynamic me = fb.Get("me?fields=first_name,email");
                string email = me.email;
                string firstname = me.firstname;
                ModelState.AddModelError("", me+"");
            }
            return View();
        }

        public ActionResult LoginFB()
        {
            var fb = new FacebookClient();
            var loginurl = fb.GetLoginUrl(new
            {
                client_id = "252307928840475",
                client_secret = "99c0d215e7d85a38953a69b7ac398005",
                redirect_uri = RedirectFB().AbsoluteUri,
                response_type = "code",
                scope = "email"
            });

            return Redirect(loginurl.AbsoluteUri);
        }
    }
}