using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Fido.Core;
using Fido.Service;
using Fido.Action.Implementation;

// http://odetocode.com/blogs/scott/archive/2013/03/11/dropdownlistfor-with-asp-net-mvc.aspx

namespace Fido.Action.Models
{
    public class RoleModel : Model<RoleModel>, IModelCRUD
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        public Guid Id { get; set; }

        [Display(Name = "role name")]
        [Required(ErrorMessage = "The role name field cannot be left blank")]
        public string Name { get; set; }

        [Display(Name = "created date")]
        public DateTime CreatedUtc { get; set; }
        [Display(Name = "record age")]
        public int? CreatedAgeDays { get; set; }
        public bool IsNew { get; set; }
        public byte[] RowVersion { get; set; }
        #endregion

        public RoleModel() { } // pure model
        public RoleModel(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI LoginAPI,
            IModelAPI ModelAPI)
                : base (FeedbackAPI, LoginAPI, ModelAPI,
                        RequiresAuthentication: true)
        { }

        public override bool Delete(RoleModel Model)
        {
            using (new FunctionLogger(Log))
            {
                var RoleService = ServiceFactory.CreateService<IRoleService>();

                RoleService.Delete(Model.Id);

                FeedbackAPI.DisplaySuccess("The role record has been deleted");
                return true;
            }
        }
    }
}
