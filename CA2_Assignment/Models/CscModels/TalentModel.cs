using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace CA2_Assignment.Models.CscModels
{
    public class Talent
    {
        public string Id { get; set; }
        public string ShortName { get; set; }
        public string UploadedByName { get; set; }
        public string UploadedById { get; set; }

        public virtual string Name { get; set; }
        public virtual string Bio { get; set; }
    }
    public class TalentView : Talent
    {
        [Required(ErrorMessage = "Name"), Display(Name = "Name")]
        public override string Name { get; set; }

        [Required(ErrorMessage = "Biography"), Display(Name = "Biography")]
        public override string Bio { get; set; }

        [Required(ErrorMessage = "Photo"), Display(Name = "Photo")]
        public IFormFile Photo { get; set; }

        public Talent GetBase()
        {

            Talent baseObj = new Talent();
            PropertyInfo[] targetProp = typeof(Talent)
            .GetProperties()
            .Where(
                p => p.GetValue(this, null) != null
            )
            .ToArray();

            foreach (PropertyInfo property in targetProp)
            {
                property.SetValue(baseObj, property.GetValue(this, null));
            }

            return baseObj;
        }
    }
}
