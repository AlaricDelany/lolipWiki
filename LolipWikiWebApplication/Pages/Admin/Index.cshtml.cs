using System.ComponentModel.DataAnnotations;
using LolipWikiWebApplication.BusinessLogic.Logic;
using LolipWikiWebApplication.DataAccess.Models;
using LolipWikiWebApplication.PageModels;
using Microsoft.AspNetCore.Mvc;

namespace LolipWikiWebApplication.Pages.Admin
{
    public class IndexModel : BasePageModel
    {
        private readonly IConfigurationLogic _configurationLogic;

        public IndexModel(IUserManagementLogic userManagementLogic, IAccessControlLogic accessControlLogic, IConfigurationLogic configurationLogic) : base(userManagementLogic, accessControlLogic, true)
        {
            _configurationLogic = configurationLogic;
        }

        [Required]
        [Display(Name = "Write Access")]
        [BindProperty]
        public AccessControlType WriteControlType { get; set; }

        [Required]
        [Display(Name = "Read Access")]
        [BindProperty]
        public AccessControlType ReadControlType { get; set; }

        public void OnGet()
        {
            ReadControlType  = _configurationLogic.GetReadControlType(Requestor);
            WriteControlType = _configurationLogic.GetWriteControlType(Requestor);
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            _configurationLogic.Update(Requestor, ReadControlType, WriteControlType);

            return RedirectToPage("/Index");
        }
    }
}