using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CollectionSystem.Application.Wrappers
{
    [Serializable]
    public abstract class BaseViewModel<T>
    {
        public BaseViewModel()
        {
            ErrorList = new List<string>();
        }

        public string Id { get; set; }
        [JsonIgnore]
        public virtual Boolean HasError
        {
            get
            {
                if (this.ErrorList.Any())
                    return true;

                return false;
            }
        }
        [JsonIgnore]
        public virtual List<string> ErrorList { get; set; } = new List<string>();
        [JsonIgnore]
        public string Created_Id { get; set; }
        [JsonIgnore]
        public int TotalCount { get; set; }
        public T Payload { get; set; }
        public string CreatedOn { get; set; }
        public string ModifiedOn { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public long RowNo { get; set; }
        public int PageNo { get; set; }
        public int PageCount { get; set; }
        public int TotalPages { get; set; }
    }

    [Serializable]
    public abstract class BaseViewModel : BaseViewModel<string>, IValidatableObject
    {
        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //if (this.Created_Id != Guid.Empty)
            //{
            //    yield return new ValidationResult("User editting record couldn't be determined");
            //}
            yield return null;
        }
    }

    public class AuditBaseViewModel
    {
        public string ReasonForChange { get; set; }
    }
}
