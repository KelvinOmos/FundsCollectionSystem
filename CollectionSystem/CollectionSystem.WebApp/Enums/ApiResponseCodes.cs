using System.ComponentModel;

namespace CollectionSystem.WebApp.Enums
{
    public enum ApiResponseCodes
    {
        [Description("Server error occured, please try again.")]
        EXCEPTION = -5,
        [Description("Unauthorized Access")]
        UNAUTHORIZED = -4,
        NOT_FOUND = -3,
        INVALID_REQUEST = -2,
        [Description("ERROR")]
        ERROR = -1,
        [Description("FAIL")]
        FAIL = 2,
        [Description("SUCCESS")]
        OK = 1,
        INACTIVE_ACCOUNT = -10,
        EMAIL_NOT_CONFIRMED = -11,
    }
}
