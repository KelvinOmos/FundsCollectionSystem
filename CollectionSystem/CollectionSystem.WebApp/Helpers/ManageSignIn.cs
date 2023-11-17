namespace CollectionSystem.WebApp.Helpers
{
    public class ManageSignIn
    {
        private IHttpContextAccessor _accessor { get; }
        public ManageSignIn(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }


        public string UserName { get { return _accessor.HttpContext.Session.GetString("userName"); } }
        public string Email { get { return _accessor.HttpContext.Session.GetString("email"); } }
        public string AccessToken { get { return _accessor.HttpContext.Session.GetString("accessToken"); } }
        public string UserId { get { return _accessor.HttpContext.Session.GetString("UserId"); } }

        public void SessionSet(string name, string value)
        {
            _accessor.HttpContext.Session.SetString(name, value);
        }
    }
}
